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
using Mediachase.Commerce.Shared;
using System.Web.Script.Serialization;

namespace Mediachase.Commerce.Catalog.CSVImport
{
    /// <summary>
    /// Summary description for SkuMappingMetaClass.
    /// </summary>
    public class EntryMappingMetaClass : MappingMetaClass
    {
        private int _metaClassId;
        private int _CatalogId;
        private bool _isSystemClass = false;

        public EntryMappingMetaClass(MetaDataContext context, int CatalogId)
            : base(context)
        {
            InnerMetaClassName = "CatalogEntry";

            MetaClass mc = MetaClass.Load(context, InnerMetaClassName);

            _isSystemClass = mc.IsSystem;
            _metaClassId = mc.Id;
            _CatalogId = CatalogId;
        }

        public EntryMappingMetaClass(MetaDataContext context, int metaClassId, int CatalogId)
            : base(context)
        {
            MetaClass mc = MetaClass.Load(context, metaClassId);

            InnerMetaClassName = mc.Name;
            _metaClassId = metaClassId;
            _CatalogId = CatalogId;
        }

		public EntryMappingMetaClass(MetaDataContext context, string metaClassName, int CatalogId)
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

            //Entry
            array.Add(new ColumnInfo(MetaField.CreateVirtual(this.Context, "eCF.50.Import", "Code", "Code<sup>1,2</sup>", "", MetaDataType.NVarChar, 100, false, false, false, false, false), fillTypes));
            array.Add(new ColumnInfo(MetaField.CreateVirtual(this.Context, "eCF.50.Import", "Name", "Name<sup>1</sup>", "", MetaDataType.NVarChar, 100, true, false, false, false, false), fillTypes));
            array.Add(new ColumnInfo(MetaField.CreateVirtual(this.Context, "eCF.50.Import", "ClassTypeId", "Entry Type<sup>1</sup>", "", MetaDataType.NVarChar, 50, true, false, false, false, false), fillTypes, true));
            array.Add(new ColumnInfo(MetaField.CreateVirtual(this.Context, "eCF.50.Import", "StartDate", "Available from", "", MetaDataType.DateTime, 8, true, false, false, false, false), fillTypes));
            array.Add(new ColumnInfo(MetaField.CreateVirtual(this.Context, "eCF.50.Import", "EndDate", "Expires on", "", MetaDataType.DateTime, 8, true, false, false, false, false), fillTypes));
            array.Add(new ColumnInfo(MetaField.CreateVirtual(this.Context, "eCF.50.Import", "TemplateName", "Display Template", "", MetaDataType.NVarChar, 50, true, false, false, false, false), fillTypes, true));
            array.Add(new ColumnInfo(MetaField.CreateVirtual(this.Context, "eCF.50.Import", "IsActive", "Available (True/False)", "", MetaDataType.Bit, 1, true, false, false, false, false), fillTypes));
            array.Add(new ColumnInfo(MetaField.CreateVirtual(this.Context, "eCF.50.Import", "CategoryCode", "Category Code (by comma)", "", MetaDataType.ShortString, 255, true, false, false, false, false), fillTypes));
			array.Add(new ColumnInfo(MetaField.CreateVirtual(this.Context, "eCF.50.Import", "SortOrder", "Sort Order", "", MetaDataType.Int, 4, true, false, false, false, false), fillTypes));

            //SEO
            array.Add(new ColumnInfo(MetaField.CreateVirtual(this.Context, "eCF.50.Import", "SEO Title", "SeoTitle", "", MetaDataType.NVarChar, 150, true, false, true, false, false), fillTypes));
            array.Add(new ColumnInfo(MetaField.CreateVirtual(this.Context, "eCF.50.Import", "SEO Url", "SeoUrl", "", MetaDataType.NVarChar, 255, true, false, true, false, false), fillTypes));
            array.Add(new ColumnInfo(MetaField.CreateVirtual(this.Context, "eCF.50.Import", "SEO Description", "SeoDescription", "", MetaDataType.NVarChar, 355, true, false, true, false, false), fillTypes));
            array.Add(new ColumnInfo(MetaField.CreateVirtual(this.Context, "eCF.50.Import", "SEO Keywords", "SeoKeywords", "", MetaDataType.NVarChar, 355, true, false, true, false, false), fillTypes));
        }

		protected override int CreateSystemRow(FillDataMode mode, int RowIndex, params object[] item)
        {
            int i = 0;
			object objSysRowAction	 = item[i++];
            //Entry
			object objCode			 = item[i++];
            object objName           = item[i++];
            object objClassTypeId    = item[i++];
            object objStartDate      = item[i++];
            object objEndDate        = item[i++];
            object objTemplateName   = item[i++];
            object objIsActive       = item[i++];
            object objCategoryCodes  = item[i++];
			object objSortOrder		 = item[i++];
            //SEO
            object objSeoTitle       = item[i++];
            object objSeoUrl         = item[i++];
            object objSeoDescription = item[i++];
            object objSeoKeywords    = item[i++];

            CatalogEntryDto.CatalogEntryRow entryRow = null;

            try
            {
				RowAction sysRowAction = RowAction.Default;

				if (objSysRowAction != null)
					sysRowAction = GetRowActionEnum((string)objSysRowAction);

                string code;
                if (objCode != null) 
					code = (string)objCode;
                else 
					throw new AbsentValue("Code");

                bool bIsNew = false;
                CatalogEntryDto catalogEntryDto = CatalogEntryManager.GetCatalogEntryDto(code, new CatalogEntryResponseGroup(CatalogEntryResponseGroup.ResponseGroup.CatalogEntryFull));
                if(catalogEntryDto.CatalogEntry.Count > 0)
                {
					if (sysRowAction == RowAction.Insert)
						throw new MDPImportException(String.Format("The Entry with code '{0}' already exists.", code));

                    entryRow = catalogEntryDto.CatalogEntry[0];

					if (sysRowAction == RowAction.Delete)
					{
						CatalogContext.Current.DeleteCatalogEntry(entryRow.CatalogEntryId, true);
						return 0;
					}
                }
                else
                {
					if (sysRowAction == RowAction.Update)
						throw new MDPImportException(String.Format("The Entry with code '{0}' does not exists.", code));

					if (sysRowAction == RowAction.Delete)
						throw new MDPImportException(String.Format("The Entry with code '{0}' does not exists.", code));

                    entryRow = catalogEntryDto.CatalogEntry.NewCatalogEntryRow();
                    entryRow.ApplicationId = CatalogConfiguration.Instance.ApplicationId;
                    entryRow.CatalogId = _CatalogId;
                    entryRow.Code = code;
                    bIsNew = true;
                }

                //Entry
                if (objName != null)
                {
                    string Name = (string)objName;
                    entryRow.Name = Name;
                }
                else if (bIsNew)
                    throw new AbsentValue("Name");

                if (objClassTypeId != null)
                {
                    string classTypeId = (string)objClassTypeId;
                    entryRow.ClassTypeId = classTypeId;
                }
                else if (bIsNew)
                    entryRow.ClassTypeId = EntryType.Product;

                if (objStartDate != null)
                {
                    DateTime startDate = (DateTime)objStartDate;
                    entryRow.StartDate = startDate.ToUniversalTime();
                }
                else if (bIsNew)
                    entryRow.StartDate = DateTime.UtcNow;

                if (objEndDate != null)
                {
                    DateTime endDate = (DateTime)objEndDate;
                    entryRow.EndDate = endDate.ToUniversalTime();
                }
                else if (bIsNew)
                    entryRow.EndDate = DateTime.UtcNow.AddYears(1);

                if (objTemplateName != null)
                {
                    string templateName = (string)objTemplateName;
                    entryRow.TemplateName = templateName;
                }
                else if (bIsNew)
                    entryRow.TemplateName = String.Empty;

                if (objIsActive != null)
                {
                    bool IsActive = (bool)objIsActive;
                    entryRow.IsActive = IsActive;
                }
                else if (bIsNew)
                    entryRow.IsActive = false;

                int oldMetaClassId = 0;
				if (!_isSystemClass && _metaClassId > 0)
				{
					if (!bIsNew)
						oldMetaClassId = entryRow.MetaClassId;

					entryRow.MetaClassId = _metaClassId;
				}
				else if (bIsNew)
					throw new MDPImportException("The new entry cannot be created without metaclass definition.");

				if (bIsNew)
					catalogEntryDto.CatalogEntry.AddCatalogEntryRow(entryRow);
				else
					entryRow.SerializedData = null;

                //SEO
                CatalogEntryDto.CatalogItemSeoRow catalogItemSeoRow = null;
                bool bSeoIsNew = false;
                if (!String.IsNullOrEmpty(this.Context.Language))
                {
                    if (catalogEntryDto.CatalogItemSeo.Count > 0)
                    {
                        DataRow[] drs = catalogEntryDto.CatalogItemSeo.Select(String.Format("LanguageCode LIKE '{0}' AND CatalogEntryId = {1}", this.Context.Language, entryRow.CatalogEntryId));
                        if (drs.Length > 0)
                        {
                            catalogItemSeoRow = (CatalogEntryDto.CatalogItemSeoRow)drs[0];
                        }
                    }

                    if (catalogItemSeoRow == null)
                    {
                        catalogItemSeoRow = catalogEntryDto.CatalogItemSeo.NewCatalogItemSeoRow();
                        catalogItemSeoRow.ApplicationId = CatalogConfiguration.Instance.ApplicationId;
                        catalogItemSeoRow.LanguageCode = this.Context.Language.ToLower();
                        catalogItemSeoRow.CatalogEntryId = entryRow.CatalogEntryId;
                        bSeoIsNew = true;
                    }

                    if (objSeoTitle != null)
                    {
                        catalogItemSeoRow.Title = (string)objSeoTitle;
                    }

                    if (objSeoUrl != null)
                    {
                        catalogItemSeoRow.Uri = (string)objSeoUrl;
                    }
                    else if (bSeoIsNew)
                    {
                        // Auto generate the URL if empty
                        string name = catalogEntryDto.CatalogEntry.Count > 0 ? catalogEntryDto.CatalogEntry[0].Name : "";
                        string url = String.Format("{0}.aspx", CommerceHelper.CleanUrlField(name));

                        int index = 1;
                        while (CatalogContext.Current.GetCatalogEntryByUriDto(url, this.Context.Language).CatalogEntry.Count != 0 || CatalogContext.Current.GetCatalogNodeDto(url, this.Context.Language).CatalogNode.Count != 0)
                        {
                            url = String.Format("{0}-{1}.aspx", CommerceHelper.CleanUrlField(name), index.ToString());
                            index++;
                        }

                        catalogItemSeoRow.Uri = url;
                    }

                    if (objSeoDescription != null)
                    {
                        catalogItemSeoRow.Description = (string)objSeoDescription;
                    }

                    if (objSeoKeywords != null)
                    {
                        catalogItemSeoRow.Keywords = (string)objSeoKeywords;
                    }

                    if(bSeoIsNew)
                        catalogEntryDto.CatalogItemSeo.AddCatalogItemSeoRow(catalogItemSeoRow);
                }

                using (TransactionScope tx = new TransactionScope())
                {
                    // Save modifications
                    if (catalogEntryDto.HasChanges())
                        CatalogContext.Current.SaveCatalogEntry(catalogEntryDto);

					int sortOrder = -1;
					if (objSortOrder != null)
					{
						sortOrder = (int)objSortOrder;
					}

                    if (objCategoryCodes != null)
                    {
                        //NodeEntryRelation
                        
                        string[] categoryCodes = ((string)objCategoryCodes).Split(',');

                        Catalog.Dto.CatalogRelationDto catalogRelationDto = FrameworkContext.Current.CatalogSystem.GetCatalogRelationDto(this._CatalogId, 0, entryRow.CatalogEntryId, String.Empty, new CatalogRelationResponseGroup(CatalogRelationResponseGroup.ResponseGroup.NodeEntry));

                        Catalog.Dto.CatalogNodeDto catalogNodeDto = FrameworkContext.Current.CatalogSystem.GetCatalogNodesDto(this._CatalogId);
                        if(catalogNodeDto.CatalogNode.Count > 0)
                        {
                            //remove product from category
                            if (catalogRelationDto.NodeEntryRelation.Count > 0)
                            {
                                foreach (CatalogRelationDto.NodeEntryRelationRow nodeEntryRelationRow in catalogRelationDto.NodeEntryRelation)
                                {
                                    DataRow[] catalogNodeDataRows = catalogNodeDto.CatalogNode.Select(String.Format("CatalogNodeId = {0}", nodeEntryRelationRow.CatalogNodeId));
                                    if (catalogNodeDataRows.Length > 0)
                                    {
                                        Catalog.Dto.CatalogNodeDto.CatalogNodeRow catalogNode = (Catalog.Dto.CatalogNodeDto.CatalogNodeRow)catalogNodeDataRows[0];

                                        bool bExist = false;
                                        foreach (string categoryCode in categoryCodes)
                                        {
                                            if (catalogNode.Code.Equals(categoryCode))
                                            {
												if(sortOrder >= 0)
													nodeEntryRelationRow.SortOrder = sortOrder;
		
                                                bExist = true;
                                                break;
                                            }
                                        }
                                        if (!bExist)
                                            nodeEntryRelationRow.Delete();
                                    }
                                }
                            }

                            //add entry to category
                            foreach (string categoryCode in categoryCodes)
                            {
                                DataRow[] catalogNodeDataRows = catalogNodeDto.CatalogNode.Select(String.Format("Code = '{0}'", categoryCode.Replace("'", "''")));
                                if (catalogNodeDataRows.Length > 0)
                                {
                                    Catalog.Dto.CatalogNodeDto.CatalogNodeRow catalogNode = (Catalog.Dto.CatalogNodeDto.CatalogNodeRow)catalogNodeDataRows[0];

                                    DataRow[] nodeEntryRelationDataRows = catalogRelationDto.NodeEntryRelation.Select(String.Format("CatalogNodeId = {0}", catalogNode.CatalogNodeId));
                                    if (nodeEntryRelationDataRows.Length == 0)
                                    {
                                        Catalog.Dto.CatalogRelationDto.NodeEntryRelationRow row = catalogRelationDto.NodeEntryRelation.NewNodeEntryRelationRow();
                                        row.CatalogId = this._CatalogId;
                                        row.CatalogEntryId = entryRow.CatalogEntryId;
                                        row.CatalogNodeId = catalogNode.CatalogNodeId;

										if (sortOrder >= 0)
											row.SortOrder = sortOrder;
										else
											row.SortOrder = 0;

                                        catalogRelationDto.NodeEntryRelation.AddNodeEntryRelationRow(row);
                                    }
                                }
                            }
                        }

                        if(catalogRelationDto.HasChanges())
                            CatalogContext.Current.SaveCatalogRelationDto(catalogRelationDto);
                    }

                    if (!bIsNew && !_isSystemClass && oldMetaClassId != entryRow.MetaClassId)
                    {
                        MetaObject.Delete(this.Context, entryRow.CatalogEntryId, oldMetaClassId);
                        MetaObject obj = MetaObject.NewObject(this.Context, entryRow.CatalogEntryId, entryRow.MetaClassId);
                        obj.AcceptChanges(this.Context);
                    }

                    tx.Complete();
                }
            }
            catch (Exception ex)
            {
				throw new MDPImportException(ex.Message, null, RowIndex, null, null, item);
            }

            return entryRow.CatalogEntryId;
        }

        private MetaDictionaryItem findDictionaryItem(string value, string destFieldName)
        {
            MetaField field = MetaField.Load(this.Context, destFieldName);
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

            string path = value.ToString();

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
