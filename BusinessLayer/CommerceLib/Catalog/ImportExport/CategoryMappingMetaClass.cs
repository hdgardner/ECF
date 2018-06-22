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
    public class CategoryMappingMetaClass : MappingMetaClass
    {
        private int _metaClassId;
        private int _CatalogId;
        private bool _isSystemClass = false;

        public CategoryMappingMetaClass(MetaDataContext context, int CatalogId)
            : base(context)
        {
            InnerMetaClassName = "CatalogNode";

            MetaClass mc = MetaClass.Load(context, InnerMetaClassName);

            _isSystemClass = mc.IsSystem;
            _metaClassId = mc.Id;
            _CatalogId = CatalogId;
        }

        public CategoryMappingMetaClass(MetaDataContext context, int metaClassId, int CatalogId)
            : base(context)
        {
            MetaClass mc = MetaClass.Load(context, metaClassId);

            InnerMetaClassName = mc.Name;
            _metaClassId = metaClassId;
            _CatalogId = CatalogId;
        }

		public CategoryMappingMetaClass(MetaDataContext context, string metaClassName, int CatalogId)
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

            //CatalogNode
            array.Add(new ColumnInfo(MetaField.CreateVirtual(this.Context, "eCF.50.Import", "Code", "Code<sup>1,2</sup>", "", MetaDataType.NVarChar, 100, false, false, false, false, false), fillTypes));
            array.Add(new ColumnInfo(MetaField.CreateVirtual(this.Context, "eCF.50.Import", "ParentCode", "Parent Code", "", MetaDataType.NVarChar, 100, true, false, false, false, false), fillTypes));
            array.Add(new ColumnInfo(MetaField.CreateVirtual(this.Context, "eCF.50.Import", "Name", "Name<sup>1</sup>", "", MetaDataType.NVarChar, 100, true, false, false, false, false), fillTypes));
            array.Add(new ColumnInfo(MetaField.CreateVirtual(this.Context, "eCF.50.Import", "StartDate", "Available from", "", MetaDataType.DateTime, 8, true, false, false, false, false), fillTypes));
            array.Add(new ColumnInfo(MetaField.CreateVirtual(this.Context, "eCF.50.Import", "EndDate", "Expires on", "", MetaDataType.DateTime, 8, true, false, false, false, false), fillTypes));
            array.Add(new ColumnInfo(MetaField.CreateVirtual(this.Context, "eCF.50.Import", "TemplateName", "Display Template", "", MetaDataType.NVarChar, 50, true, false, false, false, false), fillTypes, true));
            array.Add(new ColumnInfo(MetaField.CreateVirtual(this.Context, "eCF.50.Import", "IsActive", "Available (True/False)", "", MetaDataType.Bit, 1, true, false, false, false, false), fillTypes));
            array.Add(new ColumnInfo(MetaField.CreateVirtual(this.Context, "eCF.50.Import", "SortOrder", "Sort Order", "", MetaDataType.Int, 4, true, false, false, false, false), fillTypes));

            //SEO
            array.Add(new ColumnInfo(MetaField.CreateVirtual(this.Context, "eCF.50.Import", "SEO Title", "SeoTitle", "", MetaDataType.NVarChar, 150, true, false, true, false, false), fillTypes));
            array.Add(new ColumnInfo(MetaField.CreateVirtual(this.Context, "eCF.50.Import", "SEO Url", "SeoUrl", "", MetaDataType.NVarChar, 255, true, false, true, false, false), fillTypes));
            array.Add(new ColumnInfo(MetaField.CreateVirtual(this.Context, "eCF.50.Import", "SEO Description", "SeoDescription", "", MetaDataType.NVarChar, 355, true, false, true, false, false), fillTypes));
            array.Add(new ColumnInfo(MetaField.CreateVirtual(this.Context, "eCF.50.Import", "SEO Keywords", "SeoKeywords", "", MetaDataType.NVarChar, 355, true, false, true, false, false), fillTypes));
        }

        protected override int CreateSystemRow(FillDataMode Mode, int RowIndex, params object[] Item)
        {
            int i = 0;
			object objSysRowAction = Item[i++];
            //CatalogNode
            object objCode           = Item[i++];
            object objParentCode     = Item[i++];
            object objName           = Item[i++];
            object objStartDate      = Item[i++];
            object objEndDate        = Item[i++];
            object objTemplateName   = Item[i++];
            object objIsActive       = Item[i++];
            object objSortOrder      = Item[i++];
            //SEO
            object objSeoTitle = Item[i++];
            object objSeoUrl = Item[i++];
            object objSeoDescription = Item[i++];
            object objSeoKeywords = Item[i++];
            
            CatalogNodeDto.CatalogNodeRow nodeRow = null;

            try
            {
				RowAction sysRowAction = RowAction.Default;

				if (objSysRowAction != null)
					sysRowAction = GetRowActionEnum((string)objSysRowAction);

                string Code;
                if (!String.IsNullOrEmpty((string)objCode))
                    Code = (string)objCode;
                else
                    throw new AbsentValue("Code");

                int parentNodeId = 0;
                if (objParentCode != null)
                {
                    if (!objParentCode.Equals(String.Empty))
                    {
                        CatalogNodeDto parentNodeDto = CatalogNodeManager.GetCatalogNodeDto((string)objParentCode, new CatalogNodeResponseGroup(CatalogNodeResponseGroup.ResponseGroup.CatalogNodeInfo));
                        if (parentNodeDto.CatalogNode.Count > 0)
                        {
                            parentNodeId = parentNodeDto.CatalogNode[0].CatalogNodeId;
                        }
                    }
                }

                bool bIsNew = false;
                CatalogNodeDto catalogNodeDto = CatalogNodeManager.GetCatalogNodeDto(Code, new CatalogNodeResponseGroup(CatalogNodeResponseGroup.ResponseGroup.CatalogNodeFull));
                if(catalogNodeDto.CatalogNode.Count > 0)
                {
					if (sysRowAction == RowAction.Insert)
						throw new MDPImportException(String.Format("The Catalog Node with Code '{0}' already exists.", Code));

                    nodeRow = catalogNodeDto.CatalogNode[0];

					if (sysRowAction == RowAction.Delete)
					{
						CatalogContext.Current.DeleteCatalogNode(nodeRow.CatalogNodeId, nodeRow.CatalogId);
						return 0;
					}

					if (objParentCode != null && parentNodeId > -1)
                        nodeRow.ParentNodeId = parentNodeId;
                }
                else
                {
					if (sysRowAction == RowAction.Update)
						throw new MDPImportException(String.Format("The Catalog Node with code '{0}' does not exists.", Code));

					if (sysRowAction == RowAction.Delete)
						throw new MDPImportException(String.Format("The Catalog Node with code '{0}' does not exists.", Code));

                    nodeRow = catalogNodeDto.CatalogNode.NewCatalogNodeRow();
                    nodeRow.ApplicationId = CatalogConfiguration.Instance.ApplicationId;
                    nodeRow.CatalogId = _CatalogId;
                    nodeRow.Code = Code;
                    nodeRow.ParentNodeId = parentNodeId;
                    nodeRow.Name = String.Empty;
                    nodeRow.StartDate = DateTime.UtcNow;
                    nodeRow.EndDate = DateTime.UtcNow.AddYears(3);
                    nodeRow.TemplateName = String.Empty;
                    nodeRow.IsActive = false;
                    nodeRow.SortOrder = 0;
                    bIsNew = true;
                }

				if (objName != null)
					nodeRow.Name = (string)objName;

				if (objStartDate != null)
					nodeRow.StartDate = ((DateTime)objStartDate).ToUniversalTime();

				if (objEndDate != null)
					nodeRow.EndDate = ((DateTime)objEndDate).ToUniversalTime();

				if (objTemplateName != null)
					nodeRow.TemplateName = (string)objTemplateName;

				if (objIsActive != null)
					nodeRow.IsActive = (bool)objIsActive;

				if (objSortOrder != null)
					nodeRow.SortOrder = (int)objSortOrder;

				int oldMetaClassId = 0;
				if (!_isSystemClass && _metaClassId > 0)
				{
					if (!bIsNew)
						oldMetaClassId = nodeRow.MetaClassId;

					nodeRow.MetaClassId = _metaClassId;
				}
				else if (bIsNew)
					throw new MDPImportException("The new category cannot be created without metaclass definition.");

				if (bIsNew)
					catalogNodeDto.CatalogNode.AddCatalogNodeRow(nodeRow);

				//SEO
				CatalogNodeDto.CatalogItemSeoRow catalogItemSeoRow = null;
				bool bSeoIsNew = false;
				if (!String.IsNullOrEmpty(this.Context.Language))
				{
					if (catalogNodeDto.CatalogItemSeo.Count > 0)
					{
						DataRow[] drs = catalogNodeDto.CatalogItemSeo.Select(String.Format("LanguageCode LIKE '{0}' AND CatalogNodeId = {1}", this.Context.Language, nodeRow.CatalogNodeId));
						if (drs.Length > 0)
							catalogItemSeoRow = (CatalogNodeDto.CatalogItemSeoRow)drs[0];
					}

					if (catalogItemSeoRow == null)
					{
						catalogItemSeoRow = catalogNodeDto.CatalogItemSeo.NewCatalogItemSeoRow();
						catalogItemSeoRow.ApplicationId = CatalogConfiguration.Instance.ApplicationId;
						catalogItemSeoRow.LanguageCode = this.Context.Language.ToLower();
						catalogItemSeoRow.CatalogNodeId = nodeRow.CatalogNodeId;
						catalogItemSeoRow.Description = String.Empty;
						catalogItemSeoRow.Keywords = String.Empty;
						bSeoIsNew = true;
					}

					if (objSeoTitle != null)
						catalogItemSeoRow.Title = (string)objSeoTitle;

					if (objSeoUrl != null)
						catalogItemSeoRow.Uri = (string)objSeoUrl;
					else if (bSeoIsNew)
					{
						// Auto generate the URL if empty
						string name = catalogNodeDto.CatalogNode[0].Name;
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
						catalogItemSeoRow.Description = (string)objSeoDescription;

					if (objSeoKeywords != null)
						catalogItemSeoRow.Keywords = (string)objSeoKeywords;

					if (bSeoIsNew)
						catalogNodeDto.CatalogItemSeo.AddCatalogItemSeoRow(catalogItemSeoRow);
				}

                using (TransactionScope tx = new TransactionScope())
                {
                    // Save modifications
                    if (catalogNodeDto.HasChanges())
                        CatalogContext.Current.SaveCatalogNode(catalogNodeDto);

                    if (!bIsNew && !_isSystemClass && oldMetaClassId != nodeRow.MetaClassId)
                    {
                        MetaObject.Delete(this.Context, nodeRow.CatalogNodeId, oldMetaClassId);
                        MetaObject obj = MetaObject.NewObject(this.Context, nodeRow.CatalogNodeId, nodeRow.MetaClassId);
                        obj.AcceptChanges(this.Context);
                    }

                    tx.Complete();
                }
            }
			catch (Exception ex)
			{
				throw new MDPImportException(ex.Message, null, RowIndex, null, null, Item);
			}

            return nodeRow.CatalogNodeId;
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
                value = new MetaDictionaryItem[values.Length];
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
