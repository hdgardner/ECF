using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.MetaDataPlus.Configurator;
using Mediachase.MetaDataPlus;
using Mediachase.Data.Provider;
using System.Data;
using System.Collections;
using Mediachase.Commerce.Catalog.Objects;
using Mediachase.Commerce.Catalog;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Mediachase.Commerce.Shared;
using System.Security.Permissions;
using System.Globalization;
using Mediachase.Commerce.Engine.Images;

namespace Mediachase.Commerce.Storage
{
    /// <summary>
    /// Implements operations for the meta helper.
    /// </summary>
    public static class MetaHelper
    {
        /*
        public static void FillMetaData(DataRow row, int metaClassId, int objectId, bool loadComplex)
        {
            if (metaClassId == 0)
                return;

            // load list of MetaFields for MetaClass
            MetaClass metaClass = LoadMetaClassCached(metaClassId);

            if (metaClass == null)
                return;

            MetaFieldCollection mfs = metaClass.MetaFields;
            if (mfs == null)
                return;

            // source table
            DataTable sourceTable = row.Table;

            // get MetaFields' values for current sku
            MetaObject metaObj = null;
            // try loading from serialized binary field first
            if (row.Table.Columns.Contains(MetaObject.SerializedFieldName) && row[MetaObject.SerializedFieldName] != DBNull.Value)
            {
                IFormatter formatter = null;
                try
                {
                    formatter = new BinaryFormatter();
                    metaObj = (MetaObject)formatter.Deserialize(new MemoryStream((byte[])row[MetaObject.SerializedFieldName]));
                }
                finally
                {
                    formatter = null;
                }
            }

            // Load from database
            if (metaObj == null)
			    metaObj = MetaObject.Load(objectId, metaClassId);

            if (metaObj == null)
            {
				metaObj = MetaObject.NewObject(objectId, metaClassId, FrameworkContext.Current.Profile.UserName);
                metaObj.AcceptChanges();
            }

            Hashtable hash = metaObj.GetValues();


            // fill in MetaField DataSet
            foreach (MetaField mf in mfs)
            {
                // skip system MetaFields
                if (!mf.IsUser)
                    continue;

                // add new column if one does not exist already, make sure not to override any existing one
                DataColumn column = new DataColumn(mf.Name);

                // get meta field's value
                object columnValue = null;
                object value = null;
                if (hash.ContainsKey(mf.Name))
                    value = MetaHelper.GetMetaFieldValue(mf, hash[mf.Name]);
                
                
                //else if (hashComplex.ContainsKey(mf.Name))
                //    value = MetaHelper.GetMetaFieldValue(mf, hashComplex[mf.Name]);

                // create row in dataset for current meta field
                switch (mf.DataType)
                {
                    case MetaDataType.File:
                        column.DataType = typeof(MetaFile);
                        columnValue = (value as MetaFile);
                        break;
                    case MetaDataType.Image:
                    case MetaDataType.ImageFile:
                        //if (mf.Value != null)
                        {
                            //string[] urls = Helper.GetCachedImageUrl(mf);
                            //Image attr = CreateImage(mf.Name, urls[0]);
                            //attr.ThumbnailURL = urls[1];

                            //if (mf.ImageProperty != null)
                            //{
                            //    if (mf.ImageProperty.ImageHeight != 0)
                            //        attr.Height = mf.ImageProperty.ImageHeight.ToString();

                            //    if (mf.ImageProperty.ImageWidth != 0)
                            //        attr.Width = mf.ImageProperty.ImageWidth.ToString();

                            //    if (mf.ImageProperty.ThumbHeight != 0)
                            //        attr.ThumbnailHeight = mf.ImageProperty.ThumbHeight.ToString();

                            //    if (mf.ImageProperty.ThumbWidth != 0)
                            //        attr.ThumbnailWidth = mf.ImageProperty.ThumbWidth.ToString();
                            //}

                            //images.Add(attr);

                            column.DataType = typeof(string[]);
                            columnValue = GetCachedImageUrl(metaObj, mf, metaClassId);
                        }
                        break;
                    case MetaDataType.BigInt:
                    case MetaDataType.Bit:
                    case MetaDataType.Boolean:
                    case MetaDataType.Char:
                    case MetaDataType.Date:
                    case MetaDataType.DateTime:
                    case MetaDataType.Decimal:
                    case MetaDataType.Email:
                    case MetaDataType.Float:
                    case MetaDataType.Int:
                    case MetaDataType.Integer:
                    case MetaDataType.LongHtmlString:
                    case MetaDataType.LongString:
                    case MetaDataType.Money:
                    case MetaDataType.NChar:
                    case MetaDataType.NText:
                    case MetaDataType.Numeric:
                    case MetaDataType.NVarChar:
                    case MetaDataType.Real:
                    case MetaDataType.ShortString:
                    case MetaDataType.SmallDateTime:
                    case MetaDataType.SmallInt:
                    case MetaDataType.SmallMoney:
                    case MetaDataType.Sysname:
                    case MetaDataType.Text:
                    case MetaDataType.Timestamp:
                    case MetaDataType.TinyInt:
                    case MetaDataType.UniqueIdentifier:
                    case MetaDataType.URL:
                    case MetaDataType.VarChar:
                    case MetaDataType.Variant:
                    case MetaDataType.DictionarySingleValue:
                    case MetaDataType.EnumSingleValue:
                        //column.DataType = mf.ty
                        columnValue = value;
                        //skuDto.ItemAttribute.AddItemAttributeRow(parentSkuRow, mf.Name, mf.FriendlyName, mf.DataType.ToString(), new object[] { value });
                        break;
                    case MetaDataType.EnumMultiValue:
                    case MetaDataType.DictionaryMultiValue:
                        string[] values = value as string[];
                        column.DataType = typeof(string[]);
                        row[mf.Name] = value;
                        break;
                    case MetaDataType.StringDictionary:
                        MetaStringDictionary stringDictionary = value as MetaStringDictionary;
                        ArrayList strvals = new ArrayList();
                        if (stringDictionary != null)
                        {
                            foreach (string key in stringDictionary.Keys)
                                strvals.Add(String.Format("{0};{1}", key, stringDictionary[key]));
                        }

                        column.DataType = typeof(string[]);

                        columnValue = stringDictionary == null ? null : (string[])strvals.ToArray(typeof(string));
                        break;
                    default:
                        break;
                }

               
                if (!sourceTable.Columns.Contains(column.ColumnName))
                {
                    sourceTable.Columns.Add(column);
                }

                // TODO: check if column is built in and if so, generate the error


                row[mf.Name] = columnValue;
                column.Caption = mf.FriendlyName;

                // Mark column as not to persist, that way column won't be auto saved
                if (!column.ExtendedProperties.Contains("Persist"))
                    column.ExtendedProperties.Add("Persist", false);

                if (!column.ExtendedProperties.Contains("IsMetaAttribute"))
                    column.ExtendedProperties.Add("IsMetaAttribute", true);

                if (!column.ExtendedProperties.Contains("MetaDataType"))
                    column.ExtendedProperties.Add("MetaDataType", mf.DataType);

                column.ExtendedProperties["Persist"] = false;
                column.ExtendedProperties["IsMetaAttribute"] = true;
                column.ExtendedProperties["MetaDataType"] = mf.DataType;

            }
        }
         * */

        /// <summary>
        /// Loads the meta class cached.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="metaClassId">The meta class id.</param>
        /// <returns></returns>
        public static MetaClass LoadMetaClassCached(MetaDataContext context, int metaClassId)
        {
            // Assign new cache key, specific for site guid and response groups requested
            string cacheKey = CatalogCache.CreateCacheKey("catalogschema", "metadata-i", metaClassId.ToString());

            // load list of MetaFields for MetaClass
            MetaClass metaClass = null;

            // check cache first
            object cachedObject = CatalogCache.Get(cacheKey);

            if (cachedObject != null)
                metaClass = (MetaClass)cachedObject;

            if (metaClass == null)
            {
                metaClass = MetaClass.Load(context, metaClassId);

                if (metaClass == null)
                    return null;

                // Insert to the cache collection
                CatalogCache.Insert(cacheKey, metaClass, CatalogConfiguration.Instance.Cache.CatalogSchemaTimeout);
            }

            return metaClass;
        }

        /// <summary>
        /// Loads the meta class cached.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="metaClassName">Name of the meta class.</param>
        /// <returns></returns>
        public static MetaClass LoadMetaClassCached(MetaDataContext context, string metaClassName)
        {
            // Assign new cache key, specific for site guid and response groups requested
            string cacheKey = CatalogCache.CreateCacheKey("catalogschema", "metadata-s", metaClassName);

            // load list of MetaFields for MetaClass
            MetaClass metaClass = null;

            // check cache first
            object cachedObject = CatalogCache.Get(cacheKey);

            if (cachedObject != null)
                metaClass = (MetaClass)cachedObject;

            if (metaClass == null)
            {
                metaClass = MetaClass.Load(context, metaClassName);

                if (metaClass == null)
                    return null;

                // Insert to the cache collection
                CatalogCache.Insert(cacheKey, metaClass, CatalogConfiguration.Instance.Cache.CatalogSchemaTimeout);
            }

            return metaClass;
        }

        /// <summary>
        /// Gets the meta field value.
        /// </summary>
        /// <param name="mf">The mf.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static object GetMetaFieldValue(MetaField mf, object value)
        {
            if (value == null)
                return value;

            if (mf.DataType == MetaDataType.DictionarySingleValue || mf.DataType == MetaDataType.EnumSingleValue)
                return ((MetaDictionaryItem)value).Value;
            else if (mf.DataType == MetaDataType.DictionaryMultiValue || mf.DataType == MetaDataType.EnumMultiValue)
            {
                ArrayList arr = new ArrayList();
                foreach (MetaDictionaryItem item in (MetaDictionaryItem[])value)
                    arr.Add(item.Value);

                return (string[])arr.ToArray(typeof(string));
            }
            else
                return value;
        }

        /// <summary>
        /// Sets values for the meta object's meta field. Doesn't set file and image values.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="obj">The obj.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="values">The values.</param>
        /// <returns>True, if value was set successfully.</returns>
        public static bool SetMetaFieldValue(MetaDataContext context, MetaObject obj, string fieldName, object[] values)
        {
            MetaField mf = MetaField.Load(context, fieldName);
            if (mf == null)
                return false;

            // assign default meta field value if value is not specified and meta field doesn't allow null values
            if (values == null || values.Length == 0)
            {
				obj[mf.Name] = mf.AllowNulls ? null : MetaObject.GetDefaultValue(mf.DataType);
                return true;
            }

            // assign the value depending on meta field's type
            switch (mf.DataType)
            {
                case MetaDataType.File:
                    // do not assign file values in this function
                    throw new ApplicationException("do not assign file values in this function");
                case MetaDataType.Image:
                case MetaDataType.ImageFile:
                    // do not assign image values in this function
                    throw new ApplicationException("do not assign file values in this function");
                case MetaDataType.BigInt:
                case MetaDataType.Bit:
                case MetaDataType.Boolean:
                case MetaDataType.Char:
                case MetaDataType.Date:
                case MetaDataType.DateTime:
                case MetaDataType.Decimal:
                case MetaDataType.Email:
                case MetaDataType.Float:
                case MetaDataType.Int:
                case MetaDataType.Integer:
                case MetaDataType.LongHtmlString:
                case MetaDataType.LongString:
                case MetaDataType.Money:
                case MetaDataType.NChar:
                case MetaDataType.NText:
                case MetaDataType.Numeric:
                case MetaDataType.NVarChar:
                case MetaDataType.Real:
                case MetaDataType.ShortString:
                case MetaDataType.SmallDateTime:
                case MetaDataType.SmallInt:
                case MetaDataType.SmallMoney:
                case MetaDataType.Sysname:
                case MetaDataType.Text:
                case MetaDataType.Timestamp:
                case MetaDataType.TinyInt:
                case MetaDataType.UniqueIdentifier:
                case MetaDataType.URL:
                case MetaDataType.VarChar:
                case MetaDataType.Variant:
                    obj[mf.Name] = values[0];
                    break;
                case MetaDataType.DictionarySingleValue:
                case MetaDataType.EnumSingleValue:
                    // find a dictionary if type is string
                    if (values[0] == null || values[0].GetType() == typeof(MetaDictionaryItem))
                    {
                        obj[mf.Name] = values[0];
                        break;
                    }
                    else
                    {
                        string value = values[0].ToString();
                        foreach (MetaDictionaryItem dic in mf.Dictionary)
                        {
                            if (String.Compare(dic.Value, value, true) == 0)
                            {
                                obj[mf.Name] = dic;
                                break;
                            }
                        }
                    }
                    break;
                case MetaDataType.EnumMultiValue:
                case MetaDataType.DictionaryMultiValue:
                    ArrayList dics = new ArrayList();
                    foreach (string val1 in values)
                    {
                        int valueInt = 0;
                        Int32.TryParse(val1.ToString(), out valueInt);
                        foreach (MetaDictionaryItem dic in mf.Dictionary)
                        {
                            if (dic.Id==Int32.Parse(val1))
                            {
                                dics.Add(dic);
                                break;
                            }
                        }
                    }
                    obj[mf.Name] = (MetaDictionaryItem[])dics.ToArray(typeof(MetaDictionaryItem));
                    break;
                case MetaDataType.StringDictionary:
                    // if meta field type is StringDictionary, then values must be one of the following:
                    // 1. values[0] is MetaStringDictionary
                    // 2. values is array of string dictionary values where each values[i] is a combination of key and value divided by semicolon, i.e. value[i]="key;value"
                    if (values[0] == null || values[0].GetType() == typeof(MetaStringDictionary))
                    {
                        obj[mf.Name] = values[0];
                        break;
                    }
                    else
                    {   
                        MetaStringDictionary strDict = new MetaStringDictionary();
                        foreach (string row in (string[])values)
                        {
                            string[] val = row.Split(';');
                            if (val.Length >= 2)
                                strDict.Add(val[0], val[1]);
                        }
                        obj[mf.Name] = strDict;
                    }
                    break;
                default:
                    break;
            }

            return true;
        }

        /// <summary>
        /// Sets MetaFile and Image values
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="obj">The obj.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="name">The name.</param>
        /// <param name="contentType">Type of the content.</param>
        /// <param name="fileContents">The file contents.</param>
        /// <returns></returns>
        public static bool SetMetaFile(MetaDataContext context, MetaObject obj, string fieldName, string name, string contentType, byte[] fileContents)
        {
            MetaField mf = MetaField.Load(context, fieldName);
            if (mf == null || (mf.DataType != MetaDataType.File && mf.DataType != MetaDataType.Image && mf.DataType != MetaDataType.ImageFile))
                return false;

            // assign default meta field value if value is not specified and meta field doesn't allow null values
            if (fileContents == null || fileContents.Length == 0)
            {
				if (!mf.AllowNulls)
                    obj[mf.Name] = MetaObject.GetDefaultValue(mf.DataType);
				else
                    obj[mf.Name] = null;
                return true;
            }

            // assign the file value
            MetaFile metaFile = new MetaFile(name, contentType, fileContents);
            obj[mf.Name] = metaFile;

            return true;
        }

        /// <summary>
        /// Returns the cached image urls. First will be regular image url,
        /// second will be thumbnail if thumbnail generation is available.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="metaField">The meta field.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="createThumbnail">if set to <c>true</c> [create thumbnail].</param>
        /// <param name="thumbnailHeight">Height of the thumbnail.</param>
        /// <param name="thumbnailWidth">Width of the thumbnail.</param>
        /// <param name="thumbnailStretch">if set to <c>true</c> [thumbnail stretch].</param>
        /// <returns></returns>
        public static string[] GetCachedImageUrl(MetaFile file, MetaField metaField, string fileName, bool createThumbnail, int thumbnailHeight, int thumbnailWidth, bool thumbnailStretch)
        {
            // if there is no data, stop processing
            if (file == null)
                return null;

            //int classid, int objectid, string field
            //string name = String.Format("{0}-{1}-{2}", metaClassId, metaObject.Id, metaField.Name);
            string name = fileName;
            string tname = name + "-thumb";

            string returnName = String.Empty;
            string returnThumbnailName = String.Empty;

            // Create thumbnail
            if (createThumbnail)
            {
                if (ImageService.Exists(tname))
                {
                    returnThumbnailName = ImageService.RetrieveImageUrl(tname);
                }
                else
                {
                    //MetaFile file = ((MetaFile)metaObject.GetFile(metaField));
                    if (file != null)
                    {
                        if (!file.ContentType.Contains("gif")) // dont create thumbnails for gifs
                        {
                            byte[] data = ImageGenerator.CreateImageThumbnail(file.Buffer, file.ContentType, thumbnailHeight, thumbnailWidth, thumbnailStretch);
                            ImageService.SaveImage(tname, data, file.ContentType);
                        }
                        else
                        {
                            ImageService.SaveImage(tname, file.Buffer, file.ContentType);
                        }

                        returnThumbnailName = ImageService.RetrieveImageUrl(tname);
                    }                    
                }
            }

            // create regular image
            if (ImageService.Exists(name))
            {
                returnName = ImageService.RetrieveImageUrl(name);
            }
            else // create a cached image
            {
                //MetaFile file = ((MetaFile)metaObject.GetFile(metaField));
                if (file != null)
                {
                    try
                    {
                        ImageService.SaveImage(name, file.Buffer, file.ContentType);
                    }
                    catch (ArgumentException) // Format of the image is wrong
                    {
                    }

                    returnName = ImageService.RetrieveImageUrl(name);
                }                
            }

            if (!String.IsNullOrEmpty(returnName))
                return new string[] { returnName, returnThumbnailName };

            return null;
        }

        #region Serialize/Deserialize Helper methods

        #endregion
    }

    /// <summary>
    /// This is the object that will serialized in the system field to improve read performance of the
    /// meta data engine.
    /// </summary>
    [Serializable]
    public class MetaObjectSerialized : ISerializable
    {
        private string _version = "5.0.0.2"; // serialization version
        /// <summary>
        /// Public string literal for the serialized field name.
        /// </summary>
        public const string SerializedFieldName = "SerializedData";
        private Hashtable _FieldStorage = new Hashtable();

        private string _CreatorId = String.Empty;
        private DateTime _Created = DateTime.MinValue;

        private string _ModifierId = String.Empty;
        private DateTime _Modified = DateTime.MinValue;

        /// <summary>
        /// Gets or sets the creator id.
        /// </summary>
        /// <value>The creator id.</value>
        public string CreatorId
        {
            get
            {
                return _CreatorId;
            }
            set
            {
                _CreatorId = value;
            }
        }

        /// <summary>
        /// Gets or sets the created.
        /// </summary>
        /// <value>The created.</value>
        public DateTime Created
        {
            get
            {
                return _Created;
            }
            set
            {
                _Created = value;
            }
        }

        /// <summary>
        /// Gets or sets the modifier id.
        /// </summary>
        /// <value>The modifier id.</value>
        public string ModifierId
        {
            get
            {
                return _ModifierId;
            }
            set
            {
                _ModifierId = value;
            }
        }

        /// <summary>
        /// Gets or sets the modified.
        /// </summary>
        /// <value>The modified.</value>
        public DateTime Modified
        {
            get
            {
                return _Modified;
            }
            set
            {
                _Modified = value;
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
        /// Gets the values.
        /// </summary>
        /// <param name="language">The language.</param>
        /// <returns></returns>
        public Hashtable GetValues(string language)
        {
            Hashtable retVal = new Hashtable();

            foreach (string fieldName in this.FieldStorage.Keys)
            {
                // Load the value
                if (!fieldName.Contains("."))
                    retVal.Add(fieldName, _FieldStorage[fieldName]);
                else
                {
                    string languageMetaField = fieldName.Substring(0, fieldName.IndexOf("."));
                    if (languageMetaField.Equals(language, StringComparison.InvariantCultureIgnoreCase))
                        retVal.Add(fieldName.Substring(fieldName.IndexOf(".") + 1), _FieldStorage[fieldName]);
                }
            }

            return retVal;
        }

        /// <summary>
        /// Gets the binary value.
        /// </summary>
        /// <value>The binary value.</value>
        public byte[] BinaryValue
        {
            get
            {
                byte[] buffer = null;
                
                IFormatter formatter = new BinaryFormatter();
                Stream serializationStream = new MemoryStream();

                try
                {
                    formatter.Serialize(serializationStream, this);
                    buffer = new byte[serializationStream.Length];
                    serializationStream.Seek(0L, SeekOrigin.Begin);
                    serializationStream.Read(buffer, 0, (int)serializationStream.Length);
                }
                finally
                {
                    serializationStream.Close();
                    serializationStream = null;
                }
                formatter = null;

                return buffer;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MetaObjectSerialized"/> class.
        /// </summary>
        public MetaObjectSerialized()
        {
        }

        /// <summary>
        /// Adds the meta object.
        /// </summary>
        /// <param name="language">The language.</param>
        /// <param name="metaObject">The meta object.</param>
        public void AddMetaObject(string language, MetaObject metaObject)
        {
            this.Created = metaObject.Created;
            this.CreatorId = metaObject.CreatorId;
            this.Modified = metaObject.Modified;
            this.ModifierId = metaObject.ModifierId;

            foreach (MetaField field in metaObject.MetaClass.MetaFields)
            {
                if (field.IsSystem)
                    continue;

                string fieldName = field.Name;

                if (field.MultiLanguageValue)
                    fieldName = String.Format("{0}.{1}", language, field.Name);

                if (_FieldStorage.Contains(fieldName))
                    _FieldStorage[fieldName] = metaObject[field.Name];
                else
                    _FieldStorage.Add(fieldName, metaObject[field.Name]);
            }
        }

        #region ISerializable Members

        /// <summary>
        /// Initializes a new instance of the <see cref="MetaObjectSerialized"/> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        protected MetaObjectSerialized(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }

            if(!info.GetString("__Version").Equals(_version))
                throw new SerializationException(string.Format(CultureInfo.CurrentCulture, "Meta Object version mismatch"));

            //_State = (MetaObjectState)info.GetValue("__State", typeof(MetaObjectState));

            // Save user fields
            foreach(SerializationEntry entry in info)
            {
                if (entry.Name.Equals("__Created"))
                    this.Created = info.GetDateTime("__Created");

                if (entry.Name.Equals("__CreatorId"))
                    this.CreatorId = info.GetString("__CreatorId");

                if (entry.Name.Equals("__Modified"))
                    this.Modified = info.GetDateTime("__Modified");

                if (entry.Name.Equals("__ModifierId"))
                    this.ModifierId = info.GetString("__ModifierId");

                // skip system meta fields
                if (entry.Name.StartsWith("__"))
                    continue;

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

            info.AddValue("__Version", _version);
            info.AddValue("__Created", Created);
            info.AddValue("__CreatorId", CreatorId);
            info.AddValue("__Modified", Modified);
            info.AddValue("__ModifierId", ModifierId);


            foreach (string key in _FieldStorage.Keys)
            {
                info.AddValue(key, _FieldStorage[key]);
            }
        }

        #endregion
    }
}
