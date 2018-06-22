using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.MetaDataPlus;
using Mediachase.Data.Provider;
using Mediachase.MetaDataPlus.Configurator;
using System.Data;
using System.Globalization;
using System.Collections;
using System.Runtime.Serialization;
using System.Reflection;
using System.Security.Permissions;
using System.Xml.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Mediachase.Commerce.Storage
{
    /// <summary>
    /// Base class for objects that contain meta fields.
    /// </summary>
    [Serializable]
    public abstract class MetaStorageBase : MetaObject, IStorageObject, ICloneable, ISerializable
    {
        private Hashtable _SystemFieldStorage = new Hashtable();
        private StorageCollectionBase _StorageCollection;

        /// <summary>
        /// Gets the system field storage.
        /// </summary>
        /// <value>The system field storage.</value>
        protected Hashtable SystemFieldStorage
        {
            get
            {
                return _SystemFieldStorage;
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

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="MetaStorageBase"/> class.
        /// </summary>
        /// <param name="metaClass">The meta class.</param>
        /// <param name="reader">The reader.</param>
        internal MetaStorageBase(MetaClass metaClass, IDataReader reader)
        {
            #region ArgumentNullExceptions
            if (metaClass == null)
            {
                throw new ArgumentNullException("metaClass");
            }

            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }
            #endregion

            // Populate object with system fields
            foreach (MetaField systemField in metaClass.SystemMetaFields)
            {
                _SystemFieldStorage[systemField.Name] = reader[systemField.Name];
            }

            // Load base meta object data
            this.Load(metaClass, reader);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MetaStorageBase"/> class.
        /// </summary>
        /// <param name="metaClass">The meta class.</param>
        internal MetaStorageBase(MetaClass metaClass)
        {
            // Load base meta object data
            base.Init(-1, metaClass, null, DateTime.UtcNow, metaClass.MetaFields);
        }
        #endregion

        /// <summary>
        /// Gets or sets the <see cref="System.Object"/> with the specified field name.
        /// </summary>
        /// <value></value>
        public override object this[string fieldName]
        {
            get
            {
                // Check if field is system
                MetaField field = FindMetaField(fieldName);
                if (field != null)
                {
                    return this[field];
                }
                else
                {
                    throw new ArgumentOutOfRangeException(fieldName);
                }
            }
            set
            {
                // Check if field is system
                MetaField field = FindMetaField(fieldName);

                if (field != null)
                {
                    this[field] = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException(fieldName);
                }
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="System.Object"/> with the specified field name.
        /// </summary>
        /// <value></value>
        public override object this[MetaField field]
        {
            get
            {
                // Check if field is system
                if (field != null)
                {
                    if (field.IsSystem)
                        return this.SystemFieldStorage[field.Name];
                    else
                        return base[field.Name];
                }
                else
                {
                    throw new NullReferenceException("field can't be null");
                }
            }
            set
            {
                // keep track if item value has actually changed
                bool isStateChanged = false;

                if (field != null)
                {
                    if (field.IsSystem)
                    {
                        if (SystemFieldStorage[field.Name] != value)
                        {
                            isStateChanged = true;
                            SystemFieldStorage[field.Name] = value;
                        }
                    }
                    else
                    {
                        if (base[field.Name] != value)
                        {
                            isStateChanged = true;
                            base[field.Name] = value;
                        }
                    }

                    // changes state for the whole object, might be better approach to 
                    // seperated changes state between system and user fields, but will do for now
                    if (isStateChanged)
                        ChangeState();
                }
                else
                {
                    throw new NullReferenceException("field can't be null");
                }
            }
        }

        #region Private Methods
        /// <summary>
        /// Loads the specified reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        internal void Load(IDataReader reader)
        {
            if (this.MetaClass == null)
            {
                throw new NullReferenceException("Class must have meta class defined");
            }

            // Populate object with system fields
            foreach (MetaField systemField in this.MetaClass.SystemMetaFields)
            {
                _SystemFieldStorage[systemField.Name] = reader[systemField.Name];
            }

            base.Load(this.MetaClass, reader);
        }

        /// <summary>
        /// Loads the specified row.
        /// </summary>
        /// <param name="row">The row.</param>
        internal void Load(DataRowView row)
        {
            if (this.MetaClass == null)
            {
                throw new NullReferenceException("Class must have meta class defined");
            }

            // Populate object with system fields
            foreach (MetaField systemField in this.MetaClass.SystemMetaFields)
            {
                _SystemFieldStorage[systemField.Name] = row[systemField.Name];
            }

            base.Load(this.MetaClass, row);
        }

        /// <summary>
        /// Finds the name of the meta field.
        /// </summary>
        /// <param name="fieldId">The field id.</param>
        /// <returns></returns>
        private string FindMetaFieldName(int fieldId)
        {
            MetaField field = FindMetaField(fieldId);
            if (field == null)
                return String.Empty;
            else
                return field.Name;
        }

        /// <summary>
        /// Finds the meta field.
        /// </summary>
        /// <param name="fieldId">The field id.</param>
        /// <returns></returns>
        private MetaField FindMetaField(int fieldId)
        {
            foreach (MetaField field in this.MetaClass.MetaFields)
            {
                if (field.Id == fieldId)
                    return field;
            }

            return null;
        }

        /// <summary>
        /// Finds the meta field.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        private MetaField FindMetaField(string name)
        {
            foreach (MetaField field in this.MetaClass.MetaFields)
            {
                if (field.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                    return field;
            }

            return null;
        }

        /// <summary>
        /// Gets the system meta class.
        /// </summary>
        /// <value>The system meta class.</value>
        private MetaClass SystemMetaClass
        {
            get
            {
                MetaClass parent = MetaClass.Parent;
                while (!parent.IsSystem)
                {
                    parent = MetaClass.Parent;
                }

                return parent;
            }
        }

        /// <summary>
        /// Accepts the changes.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="saveSystem">if set to <c>true</c> [save system].</param>
        internal void AcceptChanges(MetaDataContext context, bool saveSystem)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                if (saveSystem)
                {
                    switch (this.ObjectState)
                    {
                        case MetaObjectState.Added:
                            ProcessInsertUpdateResults(DataService.ExecuteNonExec(CreateInsertCommand(context)));
                            break;
                        case MetaObjectState.Modified:
                            ProcessInsertUpdateResults(DataService.ExecuteNonExec(CreateUpdateCommand(context)));
                            break;
                        case MetaObjectState.Deleted:
                            DataService.Run(CreateDeleteCommand(context));
                            // need to remove item from the collection
                            if (StorageCollection != null)
                                StorageCollection.RemovedDeletedItem(this);
                            break;
                        case MetaObjectState.Unchanged:
                            break;
                    }
                }

                base.AcceptChanges(context);
                scope.Complete();
            }
        }


        /// <summary>
        /// Accepts the changes.
        /// </summary>
        public abstract void AcceptChanges();

        /// <summary>
        /// Accepts the changes.
        /// </summary>
        /// <param name="context">The context.</param>
        public override void AcceptChanges(MetaDataContext context)
        {
            AcceptChanges(context, true);
        }

        /// <summary>
        /// Deletes this instance.
        /// </summary>
        public override void Delete()
        {
            // If item has been just added, simply remove it from the collection
            if (this.ObjectState == MetaObjectState.Added)
            {
                if (StorageCollection != null)
                    StorageCollection.Remove(this);
                return;
            }

            base.Delete();

            // need to remove item from the collection
            if (StorageCollection != null)
                StorageCollection.DeleteItem(this);
        }
        #endregion

        #region Data Access Functions
        /// <summary>
        /// Processes the insert update results.
        /// </summary>
        /// <param name="result">The result.</param>
        private void ProcessInsertUpdateResults(DataResult result)
        {
            if (this.Id < 0) // new record has been added
            {
                // set the object id, the first parameter should be a primary key (always)
                this.Id = (int)result.Parameters[0].Value;

                // Set the primary key of current object
                this[this.MetaClass.SystemMetaFields[0].Name] = this.Id;
            }
        }

        /// <summary>
        /// Creates the parameters.
        /// </summary>
        /// <param name="command">The command.</param>
        protected virtual void CreateParameters(DataCommand command)
        {
            foreach (MetaField metaField in MetaClass.SystemMetaFields)
            {
                object fieldValue = this[metaField] == null ? DBNull.Value : this[metaField];

                if (fieldValue != DBNull.Value)
                {
                }
                else
                {
                    if (!metaField.AllowNulls && this.ObjectState == MetaObjectState.Added)
                    {
                        fieldValue = MetaObject.GetDefaultValue(metaField.DataType);
                    }
                }

                // Fix Problem: DbNull.Value in an image column [2/17/2005]
                if (fieldValue == DBNull.Value && metaField.DataType == MetaDataType.Image)
                {
                    DataParameter tmpSqlPrm = new DataParameter(metaField.Name, DataParameterType.Image);
                    tmpSqlPrm.Value = DBNull.Value;
                    command.Parameters.Add(tmpSqlPrm);
                }
                else if (fieldValue == DBNull.Value && metaField.DataType == MetaDataType.Binary)
                {
                    DataParameter tmpSqlPrm = new DataParameter(metaField.Name, DataParameterType.Binary);
                    tmpSqlPrm.Value = DBNull.Value;
                    command.Parameters.Add(tmpSqlPrm);
                }
                else if (fieldValue == DBNull.Value && metaField.DataType == MetaDataType.Money)
                {
                    DataParameter tmpSqlPrm = new DataParameter(metaField.Name, DataParameterType.Money);
                    tmpSqlPrm.Value = DBNull.Value;
                    command.Parameters.Add(tmpSqlPrm);
                }
                else if (metaField.DataType == MetaDataType.NText ||
               metaField.DataType == MetaDataType.LongHtmlString ||
               metaField.DataType == MetaDataType.LongString)
                {
                    DataParameter tmpSqlPrm = new DataParameter(metaField.Name, DataParameterType.NText);
                    tmpSqlPrm.Value = fieldValue;
                    command.Parameters.Add(tmpSqlPrm);
                }
                else
                {
                    command.Parameters.Add(new DataParameter(metaField.Name, fieldValue, (DataParameterType)metaField.DataType.GetHashCode()));
                }
            }
        }

        /// <summary>
        /// Creates the insert command.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        protected virtual DataCommand CreateInsertCommand(MetaDataContext context)
        {
            string tableName = SystemMetaClass.TableName;
            DataCommand cmd = new DataCommand();
            cmd.CommandText = DataHelper.CreateInsertStoredProcedureName(tableName);
            cmd.CommandType = CommandType.StoredProcedure;
            if (context != null)
                cmd.ConnectionString = context.ConnectionString;
            cmd.Parameters = new DataParameters();
            CreateParameters(cmd);
            //cmd.Parameters.Sort();

            // Assume first parameter is the key
            cmd.Parameters[0].Direction = ParameterDirection.InputOutput;

            // we expect null value in the procedure
            if (this.ObjectState == MetaObjectState.Added/* && (int)cmd.Parameters[0].Value == 0*/)
                cmd.Parameters[0].Value = DBNull.Value;

            return cmd;
        }

        /// <summary>
        /// Creates the update command.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        protected virtual DataCommand CreateUpdateCommand(MetaDataContext context)
        {
            string tableName = SystemMetaClass.TableName;
            DataCommand cmd = new DataCommand();
            cmd.CommandText = DataHelper.CreateUpdateStoredProcedureName(tableName);
            cmd.CommandType = CommandType.StoredProcedure;
            if (context != null)
                cmd.ConnectionString = context.ConnectionString;
            cmd.Parameters = new DataParameters();
            CreateParameters(cmd);
            return cmd;
        }

        /// <summary>
        /// Creates the delete command.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        protected virtual DataCommand CreateDeleteCommand(MetaDataContext context)
        {
            string tableName = SystemMetaClass.TableName;
            DataCommand cmd = new DataCommand();
            cmd.CommandText = DataHelper.CreateDeleteStoredProcedureName(tableName);
            cmd.CommandType = CommandType.StoredProcedure;
            if (context != null)
                cmd.ConnectionString = context.ConnectionString;
            cmd.Parameters = new DataParameters();

            // We assume here that the first system field is the primary key
            MetaField primaryField = MetaClass.SystemMetaFields[0];
            object fieldValue = this[primaryField.Name];
            cmd.Parameters.Add(new DataParameter(primaryField.Name, fieldValue, (DataParameterType)primaryField.DataType.GetHashCode()));
            return cmd;
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Populates collections within table. The tables used will be removed from
        /// the table collection.
        /// Override this method to populate your custom collection objects.
        /// </summary>
        /// <param name="tables">The tables.</param>
        /// <param name="filter">The filter.</param>
        protected virtual void PopulateCollections(DataTableCollection tables, string filter)
        {
        }


        /// <summary>
        /// Marks current instance as new which will cause new record to be created in the database for the specified object.
        /// This is useful for creating duplicates of existing objects.
        /// </summary>
        internal virtual void MarkNew()
        {
            this.Id = -1;
        }
        #endregion

        /// <summary>
        /// Sets the parent.
        /// </summary>
        /// <param name="Parent">The parent.</param>
        public abstract void SetParent(object Parent);

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
            MetaStorageBase metaObj = (MetaStorageBase)formatter.Deserialize(stream);
            metaObj.MarkNew();
            return metaObj;
        }

        #endregion

        #region ISerializable Members
        /// <summary>
        /// Initializes a new instance of the <see cref="MetaStorageBase"/> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        protected MetaStorageBase(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }

            // Save user fields
            foreach(SerializationEntry entry in info)
            {
                // skip system meta fields
                if (entry.Name.StartsWith("__"))
                    continue;

                MetaField field = FindMetaField(entry.Name);

                // Only restore system fields here
                if (field != null && field.IsSystem)
                {
                    // Load the value
                    SystemFieldStorage[entry.Name] = entry.Value;
                }
            }            
        }

        /// <summary>
        /// Gets the object data.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }

            base.GetObjectData(info, context);

            foreach (string key in this.SystemFieldStorage.Keys)
            {
                info.AddValue(key, SystemFieldStorage[key]);
            }
        }
        #endregion
    }
}
