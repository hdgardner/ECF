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
    /// Summary description for SkuMappingMetaClass.
    /// </summary>
    public class EntryRelationMappingMetaClass : MappingMetaClass
    {
        private int _metaClassId;
        private int _CatalogId;
        private bool _isSystemClass = false;

        public EntryRelationMappingMetaClass(MetaDataContext context, int CatalogId)
            : base(context)
        {
            InnerMetaClassName = "CatalogEntry";

            MetaClass mc = MetaClass.Load(context, InnerMetaClassName);

            _isSystemClass = mc.IsSystem;
            _metaClassId = mc.Id;
            _CatalogId = CatalogId;
        }

        public EntryRelationMappingMetaClass(MetaDataContext context, int metaClassId, int CatalogId)
            : base(context)
        {
            MetaClass mc = MetaClass.Load(context, metaClassId);

            InnerMetaClassName = mc.Name;
            _metaClassId = metaClassId;
            _CatalogId = CatalogId;
        }

		public EntryRelationMappingMetaClass(MetaDataContext context, string metaClassName, int CatalogId)
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
            array.Add(new ColumnInfo(MetaField.CreateVirtual(this.Context, "eCF.50.Import", "ParentCode", "Parent Entry Code<sup>1,2</sup>", "", MetaDataType.NVarChar, 100, false, false, false, false, false), fillTypes));
            array.Add(new ColumnInfo(MetaField.CreateVirtual(this.Context, "eCF.50.Import", "ChildCode", "Child Entry Code<sup>1,2</sup>", "", MetaDataType.NVarChar, 100, false, false, false, false, false), fillTypes));

            //CatalogEntryRelation
            array.Add(new ColumnInfo(MetaField.CreateVirtual(this.Context, "eCF.50.Import", "Quantity", "Quantity", "", MetaDataType.Money, 8, true, false, false, false, false), fillTypes));
            array.Add(new ColumnInfo(MetaField.CreateVirtual(this.Context, "eCF.50.Import", "GroupName", "Group Name", "", MetaDataType.NVarChar, 100, true, false, false, false, false), fillTypes));
            array.Add(new ColumnInfo(MetaField.CreateVirtual(this.Context, "eCF.50.Import", "SortOrder", "Sort Order", "", MetaDataType.Int, 4, true, false, false, false, false), fillTypes));
        }

		protected override int CreateSystemRow(FillDataMode Mode, int RowIndex, params object[] Item)
        {
            int i = 0;
			object objSysRowAction	 = Item[i++];
            object objParentCode	 = Item[i++];
            object objChildCode		 = Item[i++];
            object objQuantity		 = Item[i++];
            object objGroupName		 = Item[i++];
            object objSortOrder		 = Item[i++];

            CatalogRelationDto.CatalogEntryRelationRow catalogEntryRelationRow = null;
            CatalogRelationDto catalogRelationDto = new CatalogRelationDto();

            try
            {
				RowAction sysRowAction = RowAction.Default;

				if (objSysRowAction != null)
					sysRowAction = GetRowActionEnum((string)objSysRowAction);

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
                CatalogEntryDto catalogEntryDto = CatalogEntryManager.GetCatalogEntryDto(parentCode, new CatalogEntryResponseGroup(CatalogEntryResponseGroup.ResponseGroup.CatalogEntryInfo));
                if(catalogEntryDto.CatalogEntry.Count > 0)
                {
                    CatalogEntryDto.CatalogEntryRow entry = catalogEntryDto.CatalogEntry[0];
                    if (entry.ClassTypeId.Equals(EntryType.Product, StringComparison.OrdinalIgnoreCase) ||
                        entry.ClassTypeId.Equals(EntryType.Bundle, StringComparison.OrdinalIgnoreCase) ||
                        entry.ClassTypeId.Equals(EntryType.Package, StringComparison.OrdinalIgnoreCase) ||
                        entry.ClassTypeId.Equals(EntryType.DynamicPackage, StringComparison.OrdinalIgnoreCase))
                    {

                        CatalogEntryDto childEntryDto = CatalogEntryManager.GetCatalogEntryDto(childCode, new CatalogEntryResponseGroup(CatalogEntryResponseGroup.ResponseGroup.CatalogEntryInfo));
                        if (childEntryDto.CatalogEntry.Count > 0)
                        {
                            CatalogEntryDto.CatalogEntryRow childEntry = childEntryDto.CatalogEntry[0];

                            catalogRelationDto = CatalogRelationManager.GetCatalogRelationDto(this._CatalogId, 0, entry.CatalogEntryId, String.Empty, new CatalogRelationResponseGroup(CatalogRelationResponseGroup.ResponseGroup.CatalogEntry));
                            if (catalogRelationDto.CatalogEntryRelation.Count > 0)
                            {
                                DataRow[] drs = catalogRelationDto.CatalogEntryRelation.Select(String.Format("ParentEntryId = {0} AND ChildEntryId = {1}", entry.CatalogEntryId, childEntry.CatalogEntryId));
                                if (drs.Length > 0)
                                {
                                    catalogEntryRelationRow = (CatalogRelationDto.CatalogEntryRelationRow)drs[0];

									if (sysRowAction == RowAction.Insert)
										throw new MDPImportException(String.Format("The Relation with Parent Entry code '{0}' and Child Entry code '{1}' already exists.", parentCode, childCode));

									if (sysRowAction == RowAction.Delete)
									{
										catalogEntryRelationRow.Delete();
										CatalogRelationManager.SaveCatalogRelation(catalogRelationDto);
										return 0;
									}
                                }
                            }

                            if (catalogEntryRelationRow == null)
                            {
								if (sysRowAction == RowAction.Update)
									throw new MDPImportException(String.Format("The Relation with Parent Entry code '{0}' and Child Entry code '{1}' does not exists.", parentCode, childCode));

								if (sysRowAction == RowAction.Delete)
									throw new MDPImportException(String.Format("The Relation with Parent Entry code '{0}' and Child Entry code '{1}' does not exists.", parentCode, childCode));

                                catalogEntryRelationRow = catalogRelationDto.CatalogEntryRelation.NewCatalogEntryRelationRow();
                                catalogEntryRelationRow.ParentEntryId = entry.CatalogEntryId;
                                catalogEntryRelationRow.ChildEntryId = childEntry.CatalogEntryId;
                                catalogEntryRelationRow.Quantity = 1;
                                catalogEntryRelationRow.GroupName = String.Empty;
                                catalogEntryRelationRow.SortOrder = 0;

                                switch (entry.ClassTypeId)
                                {
                                    case EntryType.Product:
                                        catalogEntryRelationRow.RelationTypeId = EntryRelationType.ProductVariation;
                                        break;
                                    case EntryType.Package:
                                        catalogEntryRelationRow.RelationTypeId = EntryRelationType.PackageEntry;
                                        break;
                                    case EntryType.Bundle:
                                    case EntryType.DynamicPackage:
                                        catalogEntryRelationRow.RelationTypeId = EntryRelationType.BundleEntry;
                                        break;
                                }

                                bIsNew = true;
                            }
                        }
                        else
                        {
                            throw new MDPImportException(String.Format("The Child Entry with code '{0}' does not exists.", childCode));
                        }
                    }
                    else
                    {
                        throw new MDPImportException(String.Format("The Parent Entry with code '{0}' has wrong type ('{1}').", parentCode, entry.ClassTypeId));
                    }
                }
                else
                {
                    throw new MDPImportException(String.Format("The Parent Entry with code '{0}' does not exists.", parentCode));
                }

                //SalePrice
                if (objQuantity != null)
                {
                    catalogEntryRelationRow.Quantity = (decimal)objQuantity;
                }

                if (objGroupName != null)
                {
                    catalogEntryRelationRow.GroupName = (string)objGroupName;
                }

                if (objSortOrder != null)
                {
                    catalogEntryRelationRow.SortOrder = (int)objSortOrder;
                }

                if (bIsNew)
                    catalogRelationDto.CatalogEntryRelation.AddCatalogEntryRelationRow(catalogEntryRelationRow);

                using (TransactionScope tx = new TransactionScope())
                {
                    // Save modifications
                    if (catalogRelationDto.HasChanges())
                        CatalogContext.Current.SaveCatalogRelationDto(catalogRelationDto);

                    tx.Complete();
                }
            }
			catch (Exception ex)
			{
				throw new MDPImportException(ex.Message, null, RowIndex, null, null, Item);
			}

            return catalogEntryRelationRow.ParentEntryId;
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
