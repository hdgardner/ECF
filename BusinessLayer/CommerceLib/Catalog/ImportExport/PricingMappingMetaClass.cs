using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
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
using System.Configuration;
using System.Web.Script.Serialization;

namespace Mediachase.Commerce.Catalog.CSVImport
{
    /// <summary>
    /// Summary description for SkuMappingMetaClass.
    /// </summary>
    public class PricingMappingMetaClass : MappingMetaClass
    {
        private int _metaClassId;
        private int _CatalogId;
        private bool _isSystemClass = false;

        public PricingMappingMetaClass(MetaDataContext context, int CatalogId)
            : base(context)
        {
            InnerMetaClassName = "CatalogEntry";

            MetaClass mc = MetaClass.Load(context, InnerMetaClassName);

            _isSystemClass = mc.IsSystem;
            _metaClassId = mc.Id;
            _CatalogId = CatalogId;
        }

        public PricingMappingMetaClass(MetaDataContext context, int metaClassId, int CatalogId)
            : base(context)
        {
            MetaClass mc = MetaClass.Load(context, metaClassId);

            InnerMetaClassName = mc.Name;
            _metaClassId = metaClassId;
            _CatalogId = CatalogId;
        }

		public PricingMappingMetaClass(MetaDataContext context, string metaClassName, int CatalogId)
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
            array.Add(new ColumnInfo(MetaField.CreateVirtual(this.Context, "eCF.50.Import", "Code", "Entry Code<sup>1,2</sup>", "", MetaDataType.NVarChar, 100, false, false, false, false, false), fillTypes));

            //SalePrice
            array.Add(new ColumnInfo(MetaField.CreateVirtual(this.Context, "eCF.50.Import", "SaleType", "Sale Type (Name/Id)", "", MetaDataType.ShortString, 255, true, false, false, false, false), fillTypes, true));
            array.Add(new ColumnInfo(MetaField.CreateVirtual(this.Context, "eCF.50.Import", "SaleCode", "Sale Code", "", MetaDataType.NVarChar, 100, true, false, false, false, false), fillTypes));
            array.Add(new ColumnInfo(MetaField.CreateVirtual(this.Context, "eCF.50.Import", "UnitPrice", "Unit Price", "", MetaDataType.Money, 8, true, false, false, false, false), fillTypes));
            array.Add(new ColumnInfo(MetaField.CreateVirtual(this.Context, "eCF.50.Import", "Currency", "Currency (Name/Code)", "", MetaDataType.ShortString, 128, true, false, false, false, false), fillTypes, true));
            array.Add(new ColumnInfo(MetaField.CreateVirtual(this.Context, "eCF.50.Import", "MinQuantity", "Min. Quantity", "", MetaDataType.Money, 8, true, false, false, false, false), fillTypes));
            array.Add(new ColumnInfo(MetaField.CreateVirtual(this.Context, "eCF.50.Import", "StartDate", "Start Date", "", MetaDataType.DateTime, 8, true, false, false, false, false), fillTypes));
            array.Add(new ColumnInfo(MetaField.CreateVirtual(this.Context, "eCF.50.Import", "EndDate", "Start Date", "", MetaDataType.DateTime, 8, true, false, false, false, false), fillTypes));
        }

        /// <summary>
        /// Gets the sale type id.
        /// </summary>
        /// <param name="saleType">Type of the sale.</param>
        /// <returns></returns>
        private int GetSaleTypeId(string saleType)
        {
            int saleTypeId = 0;
            if (Int32.TryParse(saleType, out saleTypeId))
            {
                // no need to check the sale type id
                //if (!String.IsNullOrEmpty(SaleType.SaleTypes[saleTypeId]))
                {
                    return saleTypeId;
                }
            }
            else
            {
                foreach (KeyValueConfigurationElement element in CatalogConfiguration.Instance.SalePriceTypes)
                {
                    if (element.Key.Equals(saleType))
                        return Int32.Parse(element.Value);
                }
            }

            return 0;
        }

        /// <summary>
        /// Gets the catalog default currency.
        /// </summary>
        /// <returns></returns>
        private string GetCatalogDefaultCurrency()
        {
            CatalogDto dto = CatalogManager.GetCatalogDto(this._CatalogId, new CatalogResponseGroup(CatalogResponseGroup.ResponseGroup.CatalogInfo));
            if (dto.Catalog.Count > 0)
            {
                return dto.Catalog[0].DefaultCurrency;
            }

            return String.Empty;
        }

        /// <summary>
        /// Gets the currency code.
        /// </summary>
        /// <param name="currency">The currency.</param>
        /// <returns></returns>
        private string GetCurrencyCode(string currency)
        {
            CurrencyDto dto = CatalogContext.Current.GetCurrencyDto();
            if (dto.Currency.Count > 0)
            {
                DataRow[] drws = dto.Currency.Select(String.Format("Name LIKE '{0}' OR CurrencyCode LIKE '{0}'", currency.Replace("'", "''")));
                if (drws.Length > 0)
                {
                    return ((CurrencyDto.CurrencyRow)drws[0]).CurrencyCode;
                }
            }
            return String.Empty;
        }

        /// <summary>
        /// Creates the system row.
        /// </summary>
        /// <param name="Mode">The mode.</param>
        /// <param name="RowIndex">Index of the row.</param>
        /// <param name="Item">The item.</param>
        /// <returns></returns>
		protected override int CreateSystemRow(FillDataMode Mode, int RowIndex, params object[] Item)
        {
            int i = 0;
			object objSysRowAction	 = Item[i++];
            object objCode			 = Item[i++];
            //SaleType
            object objSaleType		 = Item[i++];
            object objSaleCode		 = Item[i++];
            object objUnitPrice		 = Item[i++];
            object objCurrency		 = Item[i++];
            object objMinQuantity	 = Item[i++];
            object objStartDate		 = Item[i++];
            object objEndDate		 = Item[i++];

			int salePriceId = 0;
            CatalogEntryDto.SalePriceRow newSalePriceRow = null;

            try
            {
				RowAction sysRowAction = RowAction.Default;

				if (objSysRowAction != null)
					sysRowAction = GetRowActionEnum((string)objSysRowAction);

                string Code;
                if (objCode != null) Code = (string)objCode;
                else throw new AbsentValue("Code");

                bool bSalePriceIsNew = false;
                CatalogEntryDto catalogEntryDto = CatalogEntryManager.GetCatalogEntryDto(Code, new CatalogEntryResponseGroup(CatalogEntryResponseGroup.ResponseGroup.CatalogEntryFull));
                if(catalogEntryDto.CatalogEntry.Count > 0)
                {
                    CatalogEntryDto.CatalogEntryRow entry = catalogEntryDto.CatalogEntry[0];
                    if (entry.ClassTypeId.Equals(EntryType.Variation, StringComparison.OrdinalIgnoreCase) ||
                        entry.ClassTypeId.Equals(EntryType.Package, StringComparison.OrdinalIgnoreCase))
                    {
						if (catalogEntryDto.SalePrice.Count == 0)
						{
							if (sysRowAction == RowAction.Update)
								throw new MDPImportException(String.Format("The Sales Price for Entry code '{0}' does not exists.", Code));

							if (sysRowAction == RowAction.Delete)
								throw new MDPImportException(String.Format("The Sales Price for Entry code '{0}' does not exists.", Code));

							bSalePriceIsNew = true;
						}

						newSalePriceRow = catalogEntryDto.SalePrice.NewSalePriceRow();
						newSalePriceRow.ItemCode = entry.Code;
						newSalePriceRow.SaleType = 0;
						newSalePriceRow.SaleCode = String.Empty;
						newSalePriceRow.UnitPrice = 0;
						newSalePriceRow.Currency = GetCatalogDefaultCurrency();
						newSalePriceRow.MinQuantity = 0;
						newSalePriceRow.StartDate = DateTime.UtcNow;
						newSalePriceRow.EndDate = DateTime.UtcNow.AddMonths(1);
                    }
                    else
                        throw new MDPImportException(String.Format("The Entry with code '{0}' has wrong type ('{1}') for Variation/Inventory import.", Code, entry.ClassTypeId));
				}
                else
                {
                    throw new MDPImportException(String.Format("The Entry with code '{0}' does not exists.", Code));
                }

                //SalePrice
                if (objSaleType != null)
                    newSalePriceRow.SaleType = (int)GetSaleTypeId((string)objSaleType);

                if (objSaleCode != null)
                    newSalePriceRow.SaleCode = (string)objSaleCode;

                if (objUnitPrice != null)
                    newSalePriceRow.UnitPrice = (decimal)objUnitPrice;

                if (objCurrency != null)
                    newSalePriceRow.Currency = GetCurrencyCode((string)objCurrency);

                if (objMinQuantity != null)
                    newSalePriceRow.MinQuantity = (decimal)objMinQuantity;

                if (objStartDate != null)
                    newSalePriceRow.StartDate = ((DateTime)objStartDate).ToUniversalTime();

                if (objEndDate != null)
                    newSalePriceRow.EndDate = ((DateTime)objEndDate).ToUniversalTime();

				if (bSalePriceIsNew)
					catalogEntryDto.SalePrice.AddSalePriceRow(newSalePriceRow);
				else
				{
					IEnumerable<int> result = from SalePriceTable in catalogEntryDto.SalePrice
					where SalePriceTable.SaleType == newSalePriceRow.SaleType && 
							SalePriceTable.SaleCode == newSalePriceRow.SaleCode &&
							SalePriceTable.Currency == newSalePriceRow.Currency &&
							SalePriceTable.StartDate == newSalePriceRow.StartDate &&
							SalePriceTable.EndDate == newSalePriceRow.EndDate
					select SalePriceTable.SalePriceId;

					if (result.Count() == 0)
					{
						if (sysRowAction == RowAction.Update)
							throw new MDPImportException(String.Format("The Sales Price for Entry code '{0}' does not exists.", Code));

						if (sysRowAction == RowAction.Delete)
							throw new MDPImportException(String.Format("The Sales Price for Entry code '{0}' does not exists.", Code));

						catalogEntryDto.SalePrice.AddSalePriceRow(newSalePriceRow);
					}
					else
					{
						if (sysRowAction == RowAction.Insert)
							throw new MDPImportException(String.Format("The Sales Price for Entry code '{0}' already exists.", Code));

						CatalogEntryDto.SalePriceRow salePriceRow = catalogEntryDto.SalePrice.FindBySalePriceId(result.First());

						if (sysRowAction == RowAction.Delete)
							salePriceRow.Delete();

						if (sysRowAction == RowAction.Update)
						{
							salePriceId = salePriceRow.SalePriceId;
							salePriceRow.UnitPrice = newSalePriceRow.UnitPrice;
							salePriceRow.MinQuantity = newSalePriceRow.MinQuantity;
						}
					}

				}

                using (TransactionScope tx = new TransactionScope())
                {
                    // Save modifications
                    if (catalogEntryDto.HasChanges())
                        CatalogContext.Current.SaveCatalogEntry(catalogEntryDto);

                    tx.Complete();
                }
            }
			catch (Exception ex)
			{
				throw new MDPImportException(ex.Message, null, RowIndex, null, null, Item);
			}

            return salePriceId;
        }

        /// <summary>
        /// Finds the dictionary item.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="DestFieldName">Name of the dest field.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Converts to dictionary.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="DestType">Type of the dest.</param>
        /// <param name="DestFieldName">Name of the dest field.</param>
        /// <param name="RowIndex">Index of the row.</param>
        /// <param name="warnings">The warnings.</param>
        /// <returns></returns>
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
