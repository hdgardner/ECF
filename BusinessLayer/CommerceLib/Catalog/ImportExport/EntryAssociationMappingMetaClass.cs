using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Mediachase.MetaDataPlus;
using Mediachase.MetaDataPlus.Configurator;
using Mediachase.Commerce.Catalog.Managers;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Catalog.Objects;
using Mediachase.Commerce.Catalog.Search;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Orders.Dto;
using Mediachase.Commerce.Orders.Managers;
using Mediachase.Commerce.Storage;
using Mediachase.MetaDataPlus.Import;
using Mediachase.Data.Provider;
using System.Web.Script.Serialization;

namespace Mediachase.Commerce.Catalog.CSVImport
{
    /// <summary>
    /// Summary description for EntryAssociationMappingMetaClass.
    /// </summary>
    public class EntryAssociationMappingMetaClass : MappingMetaClass
    {
        private int _metaClassId;
        private int _CatalogId;
        private bool _isSystemClass = false;

        public EntryAssociationMappingMetaClass(MetaDataContext context, int CatalogId)
            : base(context)
        {
            InnerMetaClassName = "CatalogEntry";

            MetaClass mc = MetaClass.Load(context, InnerMetaClassName);

            _isSystemClass = mc.IsSystem;
            _metaClassId = mc.Id;
            _CatalogId = CatalogId;
        }

        public EntryAssociationMappingMetaClass(MetaDataContext context, int metaClassId, int CatalogId)
            : base(context)
        {
            MetaClass mc = MetaClass.Load(context, metaClassId);

            InnerMetaClassName = mc.Name;
            _metaClassId = metaClassId;
            _CatalogId = CatalogId;
        }

		public EntryAssociationMappingMetaClass(MetaDataContext context, string metaClassName, int CatalogId)
			: base(context)
		{
			MetaClass mc = MetaClass.Load(context, metaClassName);

			_isSystemClass = mc.IsSystem;
			InnerMetaClassName = mc.Name;
			_metaClassId = mc.Id;
			_CatalogId = CatalogId;
		}

        protected override void FillSystemColumnInfo(System.Collections.ArrayList array)
        {
            MetaClass mc = MetaClass.Load(Context, _metaClassId);

            FillType fillTypes = FillType.CopyValue | FillType.Custom | FillType.Default;

			array.Add(new ColumnInfo(MetaField.CreateVirtual(this.Context, "eCF.50.Import", "sys_RowAction", "Action (Insert/Update/Delete or I/U/D)", "", MetaDataType.NVarChar, 6, true, false, false, false, false), fillTypes));

            //CatalogAssociation
            array.Add(new ColumnInfo(MetaField.CreateVirtual(this.Context, "eCF.50.Import", "AssociationName", "Association Name<sup>1,2</sup>", "", MetaDataType.NVarChar, 150, false, false, false, false, false), fillTypes));
            array.Add(new ColumnInfo(MetaField.CreateVirtual(this.Context, "eCF.50.Import", "ParentCode", "Parent Entry Code<sup>1,2</sup>", "", MetaDataType.NVarChar, 100, false, false, false, false, false), fillTypes));

            //EntryAssociation
            array.Add(new ColumnInfo(MetaField.CreateVirtual(this.Context, "eCF.50.Import", "ChildCode", "Child Entry Code<sup>1,2</sup>", "", MetaDataType.NVarChar, 100, false, false, false, false, false), fillTypes));
            array.Add(new ColumnInfo(MetaField.CreateVirtual(this.Context, "eCF.50.Import", "SortOrder", "Sort Order", "", MetaDataType.Int, 4, true, false, false, false, false), fillTypes));
            array.Add(new ColumnInfo(MetaField.CreateVirtual(this.Context, "eCF.50.Import", "AssociationType", "Association Type", "", MetaDataType.NVarChar, 50, true, false, false, false, false), fillTypes, true));
        }

		protected override int CreateSystemRow(FillDataMode Mode, int RowIndex, params object[] Item)
        {
            int i = 0;
			object objSysRowAction		 = Item[i++];
            object objAssociationName	 = Item[i++];
            object objParentCode		 = Item[i++];
            object objChildCode			 = Item[i++];
            object objSortOrder			 = Item[i++];
            object objAssociationType	 = Item[i++];

            try
            {
				RowAction sysRowAction = RowAction.Default;

				if (objSysRowAction != null)
					sysRowAction = GetRowActionEnum((string)objSysRowAction);

                string AssociationName;
                if (!String.IsNullOrEmpty((string)objAssociationName))
                    AssociationName = (string)objAssociationName;
                else
                    throw new AbsentValue("Association Name");

                string parentCode;
                if (!String.IsNullOrEmpty((string)objParentCode))
                    parentCode = (string)objParentCode;
                else 
                    throw new AbsentValue("Parent Entry Code");

                string childCode;
                if (!String.IsNullOrEmpty((string)objChildCode))
                    childCode = (string)objChildCode;
                else 
                    throw new AbsentValue("Child Entry Code");

                bool bIsNew = false;

				//Parent Entry
                CatalogEntryDto.CatalogEntryRow parentEntryRow = null;
                CatalogEntryDto catalogEntryDto = CatalogEntryManager.GetCatalogEntryDto(parentCode, new CatalogEntryResponseGroup(CatalogEntryResponseGroup.ResponseGroup.Associations));
                if (catalogEntryDto.CatalogEntry.Count > 0)
                    parentEntryRow = catalogEntryDto.CatalogEntry[0];
                else
                    throw new MDPImportException(String.Format("The Parent Entry with code '{0}' does not exists.", parentCode));

				//Child Entry
                CatalogEntryDto.CatalogEntryRow childEntryRow = null;
                CatalogEntryDto childEntryDto = CatalogEntryManager.GetCatalogEntryDto(childCode, new CatalogEntryResponseGroup(CatalogEntryResponseGroup.ResponseGroup.CatalogEntryInfo));
                if (childEntryDto.CatalogEntry.Count > 0)
                    childEntryRow = childEntryDto.CatalogEntry[0];
                else
                    throw new MDPImportException(String.Format("The Child Entry with code '{0}' does not exists.", childCode));

				//CatalogAssociation (define CatalogAssociationId)
                int catalogAssociationId = 0;
                CatalogEntryDto.CatalogAssociationRow[] catalogAssociationRows = (CatalogEntryDto.CatalogAssociationRow[])catalogEntryDto.CatalogAssociation.Select(String.Format("AssociationName = '{0}'", AssociationName));
                if (catalogAssociationRows.Length == 0)
                {
                    CatalogEntryDto.CatalogAssociationRow newCatalogAssociationRow = catalogEntryDto.CatalogAssociation.NewCatalogAssociationRow();
                    newCatalogAssociationRow.CatalogEntryId = parentEntryRow.CatalogEntryId;
                    newCatalogAssociationRow.AssociationName = AssociationName;
                    newCatalogAssociationRow.AssociationDescription = String.Empty;
                    newCatalogAssociationRow.SortOrder = 0;
                    catalogEntryDto.CatalogAssociation.AddCatalogAssociationRow(newCatalogAssociationRow);

                    CatalogContext.Current.SaveCatalogEntry(catalogEntryDto);

                    catalogAssociationId = newCatalogAssociationRow.CatalogAssociationId;
                }
                else
                    catalogAssociationId = catalogAssociationRows[0].CatalogAssociationId;

				//catalogEntryAssociationRow
                CatalogAssociationDto catalogAssociationDto = CatalogAssociationManager.GetCatalogAssociationDto(catalogAssociationId);
                CatalogAssociationDto.CatalogAssociationRow catalogAssociationRow = catalogAssociationDto.CatalogAssociation[0];

                CatalogAssociationDto.CatalogEntryAssociationRow catalogEntryAssociationRow = null;

                CatalogAssociationDto.CatalogEntryAssociationRow[] catalogEntryAssociationRows = (CatalogAssociationDto.CatalogEntryAssociationRow[])catalogAssociationDto.CatalogEntryAssociation.Select(String.Format("CatalogEntryId = {0}", childEntryRow.CatalogEntryId));
                if (catalogEntryAssociationRows.Length == 0)
                {
					if (sysRowAction == RowAction.Update)
						throw new MDPImportException(String.Format("The Catalog Entry Association with name '{0}' for entry code '{1}' and child code '{2}' does not exists.", AssociationName, parentCode, childCode));

					if (sysRowAction == RowAction.Delete)
						throw new MDPImportException(String.Format("The Catalog Entry Association with name '{0}' for entry code '{1}' and child code '{2}' does not exists.", AssociationName, parentCode, childCode));

                    catalogEntryAssociationRow = catalogAssociationDto.CatalogEntryAssociation.NewCatalogEntryAssociationRow();
                    catalogEntryAssociationRow.CatalogAssociationId = catalogAssociationId;
                    catalogEntryAssociationRow.CatalogEntryId = childEntryRow.CatalogEntryId;
                    catalogEntryAssociationRow.SortOrder = 0;
                    if(catalogAssociationDto.AssociationType.Count > 0)
                        catalogEntryAssociationRow.AssociationTypeId = catalogAssociationDto.AssociationType[0].AssociationTypeId;

                    bIsNew = true;
                }
                else
                {
					if (sysRowAction == RowAction.Insert)
						throw new MDPImportException(String.Format("The Catalog Entry Association with name '{0}' for entry code '{1}' and child code '{2}' already exists.", AssociationName, parentCode, childCode));

                    catalogEntryAssociationRow = catalogEntryAssociationRows[0];

					if (sysRowAction == RowAction.Delete)
					{
						catalogEntryAssociationRow.Delete();
						CatalogContext.Current.SaveCatalogAssociation(catalogAssociationDto);
						return 0;
					}
                }

                if (objSortOrder != null)
                    catalogEntryAssociationRow.SortOrder = (int)objSortOrder;

                if (objAssociationType != null)
                {
                    string associationType = (string)objAssociationType;
                    if (!catalogEntryAssociationRow.AssociationTypeId.Equals(associationType))
                    {
                        CatalogAssociationDto.AssociationTypeRow[] associationTypeRows = (CatalogAssociationDto.AssociationTypeRow[])catalogAssociationDto.AssociationType.Select(String.Format("AssociationTypeId = '{0}'", associationType));
                        if (associationTypeRows.Length > 0)
                        {
                            catalogEntryAssociationRow.AssociationTypeId = associationTypeRows[0].AssociationTypeId;
                        }
                    }
                }

                if (bIsNew)
                    catalogAssociationDto.CatalogEntryAssociation.AddCatalogEntryAssociationRow(catalogEntryAssociationRow);

                using (TransactionScope tx = new TransactionScope())
                {
                    // Save modifications
                    if (catalogAssociationDto.HasChanges())
                        CatalogContext.Current.SaveCatalogAssociation(catalogAssociationDto);

                    tx.Complete();
                }
            }
			catch (Exception ex)
			{
				throw new MDPImportException(ex.Message, null, RowIndex, null, null, Item);
			}

            return 1;
        }

        private MetaDictionaryItem findDictionaryItem(string value, string DestFieldName)
        {
            MetaField field = MetaField.Load(this.Context, DestFieldName);
            MetaDictionary dictionary = field.Dictionary;

            foreach (MetaDictionaryItem item in dictionary)
            {
                if (String.Compare(value, item.Value, true) == 0)
                    return item;
            }
            return null;
        }

        protected override object ConvertToDictionary(object value, MetaDataType DestType, string DestFieldName, int RowIndex, out MDPImportWarning[] warnings)
        {
            warnings = null;
			if (DestType == MetaDataType.StringDictionary)
			{
				MetaStringDictionary dic = new MetaStringDictionary();
				JavaScriptSerializer serializer = new JavaScriptSerializer();
				object[] col = serializer.DeserializeObject((string)value) as object[];
				foreach (Dictionary<string, object> item in col)
				{
					dic.Add((string)item["Key"], (string)item["Value"]);
				}
				value = dic;
			}
			else if (DestType == MetaDataType.DictionaryMultiValue || DestType == MetaDataType.EnumMultiValue)
			{
				string[] values = value.ToString().Split(new char[] { ',' });
				for (int i = 0; i < values.Length; i++)
                {
                    ((MetaDictionaryItem[])value)[i] = findDictionaryItem(values[i].Trim(), DestFieldName);
                    if (((MetaDictionaryItem[])value)[i] == null)
                        throw new Mediachase.MetaDataPlus.Import.InvalidCastException(DestType.ToString(), value.GetType().ToString());
                }
            }
            else
            {
                object val = findDictionaryItem(value.ToString().Trim(), DestFieldName);
                if (val == null)
                    throw new Mediachase.MetaDataPlus.Import.MDPImportException(String.Format("Invalid value '{0}' for dictionary '{1}'.", value, DestFieldName));
                value = val;
            }
            return value;
        }

        protected override object ConvertToFile(object value, MetaDataType Dest, string DestFieldName, int RowIndex, out MDPImportWarning[] warnings)
        {
            warnings = null;
            string path = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~"), value.ToString());
            try
            {
                using (FileStream fs = File.OpenRead(path))
                {
                    byte[] b = new byte[fs.Length];
                    fs.Read(b, 0, b.Length);

                    Mediachase.MetaDataPlus.MetaFile mf = new MetaFile(Path.GetFileName(path), b);

                    return mf;
                }
            }
            catch (Exception ex)
            {
                MDPImportWarning w = new MDPImportWarning(RowIndex, ex.Message);
                warnings = new MDPImportWarning[] { w };
                return null;
            }
        }

    }
}
