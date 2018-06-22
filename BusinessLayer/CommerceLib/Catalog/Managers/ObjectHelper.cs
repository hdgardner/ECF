using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Commerce.Catalog.Objects;
using Mediachase.Commerce.Shared;
using Mediachase.MetaDataPlus;
using System.Data;
using System.Collections;
using Mediachase.MetaDataPlus.Configurator;
using Mediachase.Commerce.Storage;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Mediachase.Commerce.Engine.Images;
using Mediachase.Commerce.Catalog.Dto;

namespace Mediachase.Commerce.Catalog.Managers
{
    /// <summary>
    /// Implements helper methods for catalog managers.
    /// </summary>
    public static class ObjectHelper
    {
        /// <summary>
        /// Creates <see cref="NonNegativeIntegerWithUnits"/> element.
        /// </summary>
        /// <param name="units">Name field value for ArgumentsArgument.</param>
        /// <param name="val">Value field value for ArgumentsArgument.</param>
        /// <returns>Returns new <see cref="NonNegativeIntegerWithUnits"/> element.</returns>
        public static NonNegativeIntegerWithUnits CreateUnitsAttribute(string units, string val)
        {
            NonNegativeIntegerWithUnits attr = new NonNegativeIntegerWithUnits();
            attr.Units = units;
            attr.Value = val;
            return attr;
        }

        /// <summary>
        /// Creates the price.
        /// </summary>
        /// <param name="amount">The amount.</param>
        /// <param name="currencyCode">The currency code.</param>
        /// <returns></returns>
        public static Price CreatePrice(decimal amount, string currencyCode)
        {
            Price price = new Price();
            price.FormattedPrice = CurrencyFormatter.FormatCurrency(amount, currencyCode);
            price.Amount = amount;
            price.CurrencyCode = currencyCode;
            return price;
        }

        /// <summary>
        /// Creates <see cref="ItemAttribute"/> element.
        /// </summary>
        /// <param name="name">Name field value for ItemAttribute.</param>
        /// <param name="friendlyname">FriendlyName field value for ItemAttribute.</param>
        /// <param name="type">Type field value for ItemAttribute.</param>
        /// <param name="val">Value field value for ItemAttribute.</param>
        /// <returns>Returns new <see cref="ItemAttribute"/> element.</returns>
        public static ItemAttribute CreateAttribute(string name, string friendlyname, string type, string[] val)
        {
            ItemAttribute attr = new ItemAttribute();
            attr.FriendlyName = friendlyname;
            attr.Name = name;
            attr.Type = type;
            attr.Value = val;
            return attr;
        }

        /// <summary>
        /// Creates the attribute.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="friendlyname">The friendlyname.</param>
        /// <param name="type">The type.</param>
        /// <param name="val">The val.</param>
        /// <returns></returns>
        public static ItemAttribute CreateAttribute(string name, string friendlyname, Type type, object val)
        {
			if (type == null)
				throw new ArgumentNullException("type");

            if (type == typeof(string[]))
            {
                return CreateAttribute(name, friendlyname, type.Name, (string[])val);
            }
            else if (type == typeof(string))
            {
                return CreateAttribute(name, friendlyname, type.Name, new string[] { val == DBNull.Value ? String.Empty : (string)val });
            }

            throw new ApplicationException("Type " + type.ToString() + " was not recognized by meta system");
        }

        /// <summary>
        /// Creates <see cref="ItemFile"/> element.
        /// </summary>
        /// <param name="name">Name field value for ItemFile.</param>
        /// <param name="type">Type field value for ItemFile.</param>
        /// <param name="val">Value field value for ItemFile.</param>
        /// <returns>Returns new <see cref="ItemFile"/> element.</returns>
        public static ItemFile CreateFile(string name, string type, string[] val)
        {
            ItemFile attr = new ItemFile();
            attr.Name = name;
            attr.Type = type;
            attr.Value = val;
            return attr;
        }

        /// <summary>
        /// Creates <see cref="Image"/> element.
        /// </summary>
        /// <param name="name">Name field value for Image.</param>
        /// <param name="url">URL field value for Image.</param>
        /// <returns>Returns new <see cref="Image"/> element.</returns>
        public static Image CreateImage(string name, string url)
        {
            Image attr = new Image();
            attr.Name = name;
            attr.Url = url;
            return attr;
        }

        /// <summary>
        /// Creates the attributes.
        /// </summary>
        /// <param name="metaAttributes">The meta attributes.</param>
        /// <param name="row">The row.</param>
        public static void CreateAttributes(ItemAttributes metaAttributes, DataRow row)
        {
            ArrayList attributes = new ArrayList();
            ArrayList files = new ArrayList();
            ArrayList images = new ArrayList();

            // Make sure we don't loose someone elses data
            if (metaAttributes != null && metaAttributes.Attribute != null)
            {
                foreach (ItemAttribute attr in metaAttributes.Attribute)
                    attributes.Add(attr);
            }

            // Make sure we don't loose someone elses data
            if (metaAttributes != null && metaAttributes.Files != null && metaAttributes.Files.File != null)
            {
                foreach (ItemFile file in metaAttributes.Files.File)
                    files.Add(file);
            }

            // Make sure we don't loose someone elses data
            if (metaAttributes != null && metaAttributes.Images != null)
            {
                //Changed this loop from a foreach to a for loop because metaAttributes.Image
                //was originally used but has been deprecated. metaAttributes.Images.Image
                //used instead
                for (int i = 0; i < metaAttributes.Images.Image.Length; i++)
                    images.Add(metaAttributes.Images.Image[i]);
            }


            // Get meta class id
            int metaClassId = (int)row["MetaClassId"];

            if (metaClassId == 0)
                return;

            


            // load list of MetaFields for MetaClass
            MetaClass metaClass = MetaHelper.LoadMetaClassCached(CatalogContext.MetaDataContext, metaClassId);

            if (metaClass == null)
                return;

            MetaFieldCollection mfs = metaClass.MetaFields;
            if (mfs == null)
                return;

            Hashtable hash = GetMetaFieldValues(row, metaClass, ref metaAttributes);
            /*
            Hashtable hash = null;

            // try loading from serialized binary field first
            if (row.Table.Columns.Contains(MetaObjectSerialized.SerializedFieldName) && row[MetaObjectSerialized.SerializedFieldName] != DBNull.Value)
            {
                IFormatter formatter = null;
                try
                {
                    formatter = new BinaryFormatter();
                    MetaObjectSerialized metaObjSerialized = (MetaObjectSerialized)formatter.Deserialize(new MemoryStream((byte[])row[MetaObjectSerialized.SerializedFieldName]));
                    if (metaObjSerialized != null)
                    {
                        metaAttributes.CreatedBy = metaObjSerialized.CreatorId;
                        metaAttributes.CreatedDate = metaObjSerialized.Created;
                        metaAttributes.ModifiedBy = metaObjSerialized.ModifierId;
                        metaAttributes.ModifiedDate = metaObjSerialized.Modified;
                        hash = metaObjSerialized.GetValues(CatalogContext.MetaDataContext.Language);
                    }
                }
                finally
                {
                    formatter = null;
                }
            }

            // Load from database
            if (hash == null)
            {
                MetaObject metaObj = MetaObject.Load(CatalogContext.MetaDataContext, (int)row[0], metaClass);
                if (metaObj != null)
                {
                    metaAttributes.CreatedBy = metaObj.CreatorId;
                    metaAttributes.CreatedDate = metaObj.Created;
                    metaAttributes.ModifiedBy = metaObj.ModifierId;
                    metaAttributes.ModifiedDate = metaObj.Modified;
                    hash = metaObj.GetValues();
                }
            }
             * */

            if (hash == null)
            {
                return;
            }

            // fill in MetaField DataSet
            foreach (MetaField mf in mfs)
            {
                // skip system MetaFields
                if (!mf.IsUser)
                    continue;

                // get meta field's value
                object value = null;
                if (hash.ContainsKey(mf.Name))
                    value = MetaHelper.GetMetaFieldValue(mf, hash[mf.Name]);

                // create row in dataset for current meta field
                switch (mf.DataType)
                {
                    case MetaDataType.File:
                        MetaFile metaFile = value as MetaFile;

                        if (metaFile != null)
                        {
                            ItemFile file = new ItemFile();
                            file.ContentType = metaFile.ContentType;
                            file.FileContents = metaFile.Buffer;
                            file.FileName = metaFile.Name;
                            file.Name = mf.Name;
                            file.Type = mf.DataType.ToString();
                            file.FriendlyName = mf.FriendlyName;

                            files.Add(file);
                        }
                        break;
                    case MetaDataType.Image:
                    case MetaDataType.ImageFile:

                        string fileName = String.Format("{0}-{1}-{2}", metaClassId, (int)row[0], mf.Name);

                        bool createThumbnail = false;
                        int imageWidth = 0;
                        int imageHeight = 0;
                        int thumbWidth = 0;
                        int thumbHeight = 0;
                        bool thumbStretch = false;

                        object createThumbnaleObj = mf.Attributes["CreateThumbnail"];
                        if (createThumbnaleObj != null && Boolean.Parse(createThumbnaleObj.ToString()))
                        {
                            createThumbnail = true;

                            object var = mf.Attributes["AutoResize"];

                            if (var != null && Boolean.Parse(var.ToString()))
                            {
                                var = mf.Attributes["ImageHeight"];
                                if (var != null)
                                    imageHeight = Int32.Parse(var.ToString());

                                var = mf.Attributes["ImageWidth"];
                                if (var != null)
                                    imageHeight = Int32.Parse(var.ToString());
                            }

                            var = mf.Attributes["CreateThumbnail"];

                            if (var != null && Boolean.Parse(var.ToString()))
                            {
                                var = mf.Attributes["ThumbnailHeight"];
                                if (var != null)
                                    thumbHeight = Int32.Parse(var.ToString());

                                var = mf.Attributes["ThumbnailWidth"];
                                if (var != null)
                                    thumbWidth = Int32.Parse(var.ToString());

                                var = mf.Attributes["StretchThumbnail"];
                                if (var != null && Boolean.Parse(var.ToString()))
                                    thumbStretch = true;
                            }
                        }

                        //string[] val = MetaHelper.GetCachedImageUrl((MetaFile)hash[mf.Name], mf, fileName, createThumbnail, thumbHeight, thumbWidth, thumbStretch);

                        string imageUrl = ImageService.RetrieveImageUrl(fileName);
                        string imageThumbUrl = ImageService.RetrieveThumbnailImageUrl(fileName);

                        //if (val != null)
                        {
                            Image attr = CreateImage(mf.Name, imageUrl);

                            if (createThumbnail)
                                attr.ThumbnailUrl = imageThumbUrl;

                            if (imageHeight != 0)
                                attr.Height = imageHeight.ToString();

                            if (imageWidth != 0)
                                attr.Width = imageWidth.ToString();

                            if (thumbHeight != 0)
                                attr.ThumbnailHeight = thumbHeight.ToString();

                            if (thumbWidth != 0)
                                attr.ThumbnailWidth = thumbWidth.ToString();

                            images.Add(attr);
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
                        attributes.Add(ObjectHelper.CreateAttribute(mf.Name, mf.FriendlyName, mf.DataType.ToString(), new string[] { value == null ? String.Empty : value.ToString() }));
                        break;
                    case MetaDataType.EnumMultiValue:
                    case MetaDataType.DictionaryMultiValue:
                        attributes.Add(ObjectHelper.CreateAttribute(mf.Name, mf.FriendlyName, "string[]", (string[])value));
                        break;
                    case MetaDataType.StringDictionary:
                        MetaStringDictionary stringDictionary = value as MetaStringDictionary;
                        ArrayList strvals = new ArrayList();
                        if (stringDictionary != null)
                        {
                            foreach (string key in stringDictionary.Keys)
                                strvals.Add(String.Format("{0};{1}", key, stringDictionary[key]));
                        }
                        attributes.Add(ObjectHelper.CreateAttribute(mf.Name, mf.FriendlyName, "string[]", stringDictionary == null ? null : (string[])strvals.ToArray(typeof(string))));
                        break;
                    default:
                        break;
                }
            }

            metaAttributes.Attribute = (ItemAttribute[])attributes.ToArray(typeof(ItemAttribute));
            metaAttributes.Files = new ItemFiles();
            metaAttributes.Files.File = (ItemFile[])files.ToArray(typeof(ItemFile));
            metaAttributes.Images = new Images();
            metaAttributes.Images.Image = (Image[])images.ToArray(typeof(Image));
        }


        /// <summary>
        /// Gets the meta field values.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <returns></returns>
        public static Hashtable GetMetaFieldValues(DataRow row)
        {
            // Get meta class id
            int metaClassId = (int)row["MetaClassId"];

            if (metaClassId == 0)
                return null;

            ItemAttributes attr = new ItemAttributes();
            MetaClass metaClass = MetaHelper.LoadMetaClassCached(CatalogContext.MetaDataContext, metaClassId);

            if (metaClass == null)
                return null;

            MetaFieldCollection mfs = metaClass.MetaFields;
            if (mfs == null)
                return null;

            return GetMetaFieldValues(row, metaClass, ref attr);
        }

        /// <summary>
        /// Gets the meta field values.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="metaAttributes">The meta attributes.</param>
        /// <returns></returns>
        private static Hashtable GetMetaFieldValues(DataRow row, MetaClass metaClass, ref ItemAttributes metaAttributes)
        {
            Hashtable hash = null;

            // try loading from serialized binary field first
            if (row.Table.Columns.Contains(MetaObjectSerialized.SerializedFieldName) && row[MetaObjectSerialized.SerializedFieldName] != DBNull.Value)
            {
                IFormatter formatter = null;
                try
                {
                    formatter = new BinaryFormatter();
                    MetaObjectSerialized metaObjSerialized = (MetaObjectSerialized)formatter.Deserialize(new MemoryStream((byte[])row[MetaObjectSerialized.SerializedFieldName]));
                    if (metaObjSerialized != null)
                    {
                        if (metaAttributes != null)
                        {
                            metaAttributes.CreatedBy = metaObjSerialized.CreatorId;
                            metaAttributes.CreatedDate = metaObjSerialized.Created;
                            metaAttributes.ModifiedBy = metaObjSerialized.ModifierId;
                            metaAttributes.ModifiedDate = metaObjSerialized.Modified;
                        }
                        hash = metaObjSerialized.GetValues(CatalogContext.MetaDataContext.Language);
                    }
                }
                finally
                {
                    formatter = null;
                }
            }

            // Load from database
            if (hash == null)
            {
                MetaObject metaObj = MetaObject.Load(CatalogContext.MetaDataContext, (int)row[0], metaClass);
                if (metaObj != null)
                {
                    metaAttributes.CreatedBy = metaObj.CreatorId;
                    metaAttributes.CreatedDate = metaObj.Created;
                    metaAttributes.ModifiedBy = metaObj.ModifierId;
                    metaAttributes.ModifiedDate = metaObj.Modified;
                    hash = metaObj.GetValues();
                }
            }

            if (hash == null)
            {
                return null;
            }

            return hash;
        }
    }
}
