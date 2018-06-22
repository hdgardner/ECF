using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.MetaDataPlus;
using Mediachase.MetaDataPlus.Configurator;
using System.Collections;
using Mediachase.Data.Provider;
using System.Data;
using System.Runtime.Serialization;
using System.Reflection;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Permissions;
using log4net;

namespace Mediachase.Commerce.Storage
{
    /// <summary>
    /// Implements operations for simple storage objects. (Inherits <see cref="IStorageObject"/>, <see cref="ICloneable"/>, <see cref="ISerializable"/>.)
    /// </summary>
    [Serializable]
    public abstract class SimpleObject : IStorageObject, ICloneable, ISerializable
    {
        private readonly ILog Logger;
        private MetaObjectState _State = MetaObjectState.Added;
        private Hashtable _FieldStorage = new Hashtable();
        private StorageCollectionBase _StorageCollection;

        /// <summary>
        /// Gets the state of the object.
        /// </summary>
        /// <value>The state of the object.</value>
        public MetaObjectState ObjectState
        {
            get
            {
                return _State;
            }
        }

        /// <summary>
        /// Gets or sets the storage collection.
        /// </summary>
        /// <value>The storage collection.</value>
        internal StorageCollectionBase StorageCollection
        {
            get
            {
                return _StorageCollection;
            }
            set
            {
                _StorageCollection = value;
            }
        }

        /// <summary>
        /// Gets the name of the table.
        /// </summary>
        /// <value>The name of the table.</value>
        public virtual string TableName
        {
            get
            {
                return this.GetType().Name;
            }
        }

        /// <summary>
        /// Gets the field storage.
        /// </summary>
        /// <value>The field storage.</value>
        protected Hashtable FieldStorage
        {
            get
            {
                return _FieldStorage;
            }
        }

        /// <summary>
        /// Deletes this instance.
        /// </summary>
        public virtual void Delete()
        {
            // If item has been just added, simply remove it from the collection
            if (this.ObjectState == MetaObjectState.Added)
            {
                if (StorageCollection != null)
                    StorageCollection.Remove(this);

                return;
            }

            this._State = MetaObjectState.Deleted;

            // need to remove item from the collection
            if (StorageCollection != null)
                StorageCollection.DeleteItem(this);
        }

        /// <summary>
        /// Changes the state.
        /// </summary>
        protected void ChangeState()
        {
            switch (this.ObjectState)
            {
                case MetaObjectState.Added:
                    break;
                case MetaObjectState.Deleted:
                    throw new DeletedObjectInaccessibleException();
                case MetaObjectState.Modified:
                    break;
                case MetaObjectState.Unchanged:
                    this._State = MetaObjectState.Modified;
                    break;
            }
        }

        /// <summary>
        /// Loads the specified reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        public virtual void Load(IDataReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            for (int index = 0; index < reader.FieldCount; index++)
            {
                this[reader.GetName(index)] = reader[index];
            }

            this._State = MetaObjectState.Unchanged;
        }

        /// <summary>
        /// Loads the specified row.
        /// </summary>
        /// <param name="row">The row.</param>
        public virtual void Load(DataRowView row)
        {
            if (row == null)
                throw new ArgumentNullException("row");

            foreach (DataColumn column in row.DataView.Table.Columns)
            {
                this[column.ColumnName] = row[column.ColumnName];
            }

            this._State = MetaObjectState.Unchanged;
        }

        /// <summary>
        /// Gets or sets the <see cref="System.Object"/> with the specified field name.
        /// </summary>
        /// <value></value>
        public virtual object this[string fieldName]
        {
            get
            {
                return this.FieldStorage[fieldName];
            }
            set
            {
                // keep track if item value has actually changed
                bool isStateChanged = false;

                //if (FieldStorage[fieldName] != value)
                {
                    isStateChanged = true;
                    this.FieldStorage[fieldName] = value;
                }

                //ValidateMetaField(fieldName, value);
                if (isStateChanged)
                    ChangeState();                
            }
        }

        /// <summary>
        /// Marks current instance as new which will cause new record to be created in the database for the specified object.
        /// This is useful for creating duplicates of existing objects.
        /// </summary>
        internal virtual void MarkNew()
        {
            if(_State != MetaObjectState.Deleted)
                _State = MetaObjectState.Added;
        }

        /// <summary>
        /// Accepts the changes.
        /// </summary>
        public virtual void AcceptChanges()
        {
            Logger.Debug(String.Format("Accepting changes with a following state {0}.", this.ObjectState.ToString()));
            using (TransactionScope scope = new TransactionScope())
            {
                switch (this.ObjectState)
                {
                    case MetaObjectState.Added:
                        OnSaved(DataService.ExecuteNonExec(CreateInsertCommand()));
                        this._State = MetaObjectState.Unchanged;
                        break;
                    case MetaObjectState.Modified:
                        OnSaved(DataService.ExecuteNonExec(CreateUpdateCommand()));
                        this._State = MetaObjectState.Unchanged;
                        break;
                    case MetaObjectState.Deleted:
                        DataService.Run(CreateDeleteCommand());
                        // need to remove item from the collection
                        if (StorageCollection != null)
                            StorageCollection.RemovedDeletedItem(this);
                        break;
                    case MetaObjectState.Unchanged:
                        break;
                }
                scope.Complete();
            }
        }

        /// <summary>
        /// Called when [saved].
        /// </summary>
        /// <param name="result">The result.</param>
        protected abstract void OnSaved(DataResult result);
        /// <summary>
        /// Creates the insert command.
        /// </summary>
        /// <returns></returns>
        protected abstract DataCommand CreateInsertCommand();
        /// <summary>
        /// Creates the update command.
        /// </summary>
        /// <returns></returns>
        protected abstract DataCommand CreateUpdateCommand();
        /// <summary>
        /// Creates the delete command.
        /// </summary>
        /// <returns></returns>
        protected abstract DataCommand CreateDeleteCommand();

        #region Safe Get Methods
        /// <summary>
        /// Gets the string.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns></returns>
        public string GetString(string fieldName)
        {
            object val = this[fieldName];
            if (IsNullValue(val))
                return String.Empty;
            else
                return (string)val;
        }
        /// <summary>
        /// Determines whether [is null value] [the specified val].
        /// </summary>
        /// <param name="val">The val.</param>
        /// <returns>
        /// 	<c>true</c> if [is null value] [the specified val]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsNullValue(object val)
        {
            if (val == null || val == DBNull.Value)
                return true;
            else
                return false;
        }
        /// <summary>
        /// Gets the int.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns></returns>
        public int GetInt(string fieldName)
        {
            if (!IsNullValue(this[fieldName]))
                return (int)this[fieldName];
            else
                return 0;
        }

        /// <summary>
        /// Gets the bool.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns></returns>
        public bool GetBool(string fieldName)
        {
            return (bool)this[fieldName];
        }
        /// <summary>
        /// Gets the GUID.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns></returns>
        public Guid GetGuid(string fieldName)
        {
            return (Guid)this[fieldName];
        }
        /// <summary>
        /// Gets the int32.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns></returns>
        public Int32 GetInt32(string fieldName)
        {
            return (Int32)this[fieldName];
        }
        /// <summary>
        /// Gets the date time.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns></returns>
        public DateTime GetDateTime(string fieldName)
        {
            return (DateTime)this[fieldName];
        }
        /// <summary>
        /// Gets the decimal.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns></returns>
        public Decimal GetDecimal(string fieldName)
        {
            if (!IsNullValue(this[fieldName]))
                return (Decimal)this[fieldName];
            else
                return 0;
        }
        #endregion

        #region ICloneable Members

        /// <summary>
        /// Clone the object, and returning a reference to a cloned object.
        /// </summary>
        /// <returns>Reference to the new cloned 
        /// object.</returns>
        public object Clone()
        {
            MemoryStream stream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, this);
            stream.Position = 0;
            return formatter.Deserialize(stream);
        }

        #endregion

        /*
            //First we create an instance of this specific type.
            object newObject = new ClassInfo(GetType()).CreateInstance();

            //We get the array of fields for the new type instance.
            FieldInfo[] fields = newObject.GetType().GetFields();

            int i = 0;

            foreach (FieldInfo fi in this.GetType().GetFields())
            {
                //We query if the fiels support the ICloneable interface.
                Type ICloneType = fi.FieldType.
                            GetInterface("ICloneable", true);

                if (ICloneType != null)
                {
                    //Getting the ICloneable interface from the object.
                    ICloneable IClone = (ICloneable)fi.GetValue(this);

                    //We use the clone method to set the new value to the field.
                    fields[i].SetValue(newObject, IClone.Clone());
                }
                else
                {
                    // If the field doesn't support the ICloneable 
                    // interface then just set it.
                    fields[i].SetValue(newObject, fi.GetValue(this));
                }

                //Now we check if the object support the 
                //IEnumerable interface, so if it does
                //we need to enumerate all its items and check if 
                //they support the ICloneable interface.
                Type IEnumerableType = fi.FieldType.GetInterface
                                ("IEnumerable", true);
                if (IEnumerableType != null)
                {
                    //Get the IEnumerable interface from the field.
                    IEnumerable IEnum = (IEnumerable)fi.GetValue(this);

                    //This version support the IList and the 
                    //IDictionary interfaces to iterate on collections.
                    Type IListType = fields[i].FieldType.GetInterface
                                        ("IList", true);
                    Type IDicType = fields[i].FieldType.GetInterface
                                        ("IDictionary", true);

                    int j = 0;
                    if (IListType != null)
                    {
                        //Getting the IList interface.
                        IList list = (IList)fields[i].GetValue(newObject);

                        foreach (object obj in IEnum)
                        {
                            //Checking to see if the current item 
                            //support the ICloneable interface.
                            ICloneType = obj.GetType().
                                GetInterface("ICloneable", true);

                            if (ICloneType != null)
                            {
                                //If it does support the ICloneable interface, 
                                //we use it to set the clone of
                                //the object in the list.
                                ICloneable clone = (ICloneable)obj;

                                list[j] = clone.Clone();
                            }

                            //NOTE: If the item in the list is not 
                            //support the ICloneable interface then in the 
                            //cloned list this item will be the same 
                            //item as in the original list
                            //(as long as this type is a reference type).

                            j++;
                        }
                    }
                    else if (IDicType != null)
                    {
                        //Getting the dictionary interface.
                        IDictionary dic = (IDictionary)fields[i].
                                            GetValue(newObject);
                        j = 0;

                        foreach (DictionaryEntry de in IEnum)
                        {
                            //Checking to see if the item 
                            //support the ICloneable interface.
                            ICloneType = de.Value.GetType().
                                GetInterface("ICloneable", true);

                            if (ICloneType != null)
                            {
                                ICloneable clone = (ICloneable)de.Value;

                                dic[de.Key] = clone.Clone();
                            }
                            j++;
                        }
                    }
                }
                i++;
            }
            return newObject;
         * */

        #region ISerializable Members
        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleObject"/> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        protected SimpleObject(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }

            Logger = LogManager.GetLogger(GetType());
            _State = (MetaObjectState)info.GetValue("__State", typeof(MetaObjectState));

            // Save user fields
            foreach(SerializationEntry entry in info)
            {
                // skip system meta fields
                if (entry.Name.StartsWith("__"))
                    continue;

                // Load the value
                _FieldStorage[entry.Name] = entry.Value;
            }            
        }

        /// <summary>
        /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> to populate with data.</param>
        /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext"></see>) for this serialization.</param>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }

            info.AddValue("__State", _State, typeof(MetaObjectState));

            foreach (string key in this.FieldStorage.Keys)
            {
                info.AddValue(key, _FieldStorage[key]);
            }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleObject"/> class.
        /// </summary>
        public SimpleObject()
        {
            Logger = LogManager.GetLogger(GetType());
        }
    }
}