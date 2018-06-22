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
    public class VariationMappingMetaClass : MappingMetaClass
    {
        private int _metaClassId;
        private int _CatalogId;
        private bool _isSystemClass = false;

        public VariationMappingMetaClass(MetaDataContext context, int CatalogId)
            : base(context)
        {
            InnerMetaClassName = "CatalogEntry";

            MetaClass mc = MetaClass.Load(context, InnerMetaClassName);

            _isSystemClass = mc.IsSystem;
            _metaClassId = mc.Id;
            _CatalogId = CatalogId;
        }

        public VariationMappingMetaClass(MetaDataContext context, int metaClassId, int CatalogId)
            : base(context)
        {
            MetaClass mc = MetaClass.Load(context, metaClassId);

            InnerMetaClassName = mc.Name;
            _metaClassId = metaClassId;
            _CatalogId = CatalogId;
        }

		public VariationMappingMetaClass(MetaDataContext context, string metaClassName, int CatalogId)
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

            //Variation      
            string defaultCurrency = GetCatalogDefaultCurrency();
            defaultCurrency = (String.IsNullOrEmpty(defaultCurrency))? "USD": defaultCurrency;

            array.Add(new ColumnInfo(MetaField.CreateVirtual(this.Context, "eCF.50.Import", "ListPrice", String.Format("Display Price ({0})<sup>1</sup>", defaultCurrency), "", MetaDataType.Money, 8, false, false, false, false, false), fillTypes));
            array.Add(new ColumnInfo(MetaField.CreateVirtual(this.Context, "eCF.50.Import", "TaxCategoryId", "Tax Category (Name/Id)", "", MetaDataType.ShortString, 255, true, false, false, false, false), fillTypes, true));
            array.Add(new ColumnInfo(MetaField.CreateVirtual(this.Context, "eCF.50.Import", "TrackInventory", "Track Inventory", "", MetaDataType.Bit, 1, true, false, false, false, false), fillTypes));
            array.Add(new ColumnInfo(MetaField.CreateVirtual(this.Context, "eCF.50.Import", "MerchantId", "Merchant", "", MetaDataType.ShortString, 255, true, false, false, false, false), fillTypes, true));
            array.Add(new ColumnInfo(MetaField.CreateVirtual(this.Context, "eCF.50.Import", "WarehouseId", "Warehouse", "", MetaDataType.ShortString, 255, true, false, false, false, false), fillTypes, true));
            array.Add(new ColumnInfo(MetaField.CreateVirtual(this.Context, "eCF.50.Import", "Weight", "Weight", "", MetaDataType.Float, 8, true, false, false, false, false), fillTypes));
            array.Add(new ColumnInfo(MetaField.CreateVirtual(this.Context, "eCF.50.Import", "PackageId", "Package", "", MetaDataType.ShortString, 255, true, false, false, false, false), fillTypes, true));
            array.Add(new ColumnInfo(MetaField.CreateVirtual(this.Context, "eCF.50.Import", "MinQuantity", "Min. Quantity", "", MetaDataType.Money, 8, true, false, false, false, false), fillTypes));
            array.Add(new ColumnInfo(MetaField.CreateVirtual(this.Context, "eCF.50.Import", "MaxQuantity", "Max. Quantity", "", MetaDataType.Money, 8, true, false, false, false, false), fillTypes));

            //Inventory
            array.Add(new ColumnInfo(MetaField.CreateVirtual(this.Context, "eCF.50.Import", "InStockQuantity", "In Stock", "", MetaDataType.Decimal, 18, true, false, false, false, false), fillTypes));
            array.Add(new ColumnInfo(MetaField.CreateVirtual(this.Context, "eCF.50.Import", "ReservedQuantity", "Reserved", "", MetaDataType.Decimal, 18, true, false, false, false, false), fillTypes));
            array.Add(new ColumnInfo(MetaField.CreateVirtual(this.Context, "eCF.50.Import", "ReorderMinQuantity", "Reorder Min. Qty", "", MetaDataType.Decimal, 18, true, false, false, false, false), fillTypes));
            array.Add(new ColumnInfo(MetaField.CreateVirtual(this.Context, "eCF.50.Import", "PreorderQuantity", "Preorder Qty", "", MetaDataType.Decimal, 18, true, false, false, false, false), fillTypes));
            array.Add(new ColumnInfo(MetaField.CreateVirtual(this.Context, "eCF.50.Import", "BackorderQuantity", "Backorder Qty", "", MetaDataType.Decimal, 18, true, false, false, false, false), fillTypes));
            array.Add(new ColumnInfo(MetaField.CreateVirtual(this.Context, "eCF.50.Import", "AllowBackorder", "Allow Backorder", "", MetaDataType.Bit, 1, true, false, false, false, false), fillTypes));
            array.Add(new ColumnInfo(MetaField.CreateVirtual(this.Context, "eCF.50.Import", "AllowPreorder", "Allow Preorder", "", MetaDataType.Bit, 1, true, false, false, false, false), fillTypes));
            array.Add(new ColumnInfo(MetaField.CreateVirtual(this.Context, "eCF.50.Import", "InventoryStatus", "Inventory Status", "", MetaDataType.Int, 4, true, false, false, false, false), fillTypes));
            array.Add(new ColumnInfo(MetaField.CreateVirtual(this.Context, "eCF.50.Import", "PreorderAvailabilityDate", "Preorder Avail.", "", MetaDataType.DateTime, 8, true, false, false, false, false), fillTypes));
            array.Add(new ColumnInfo(MetaField.CreateVirtual(this.Context, "eCF.50.Import", "BackorderAvailabilityDate", "Backorder Avail.", "", MetaDataType.DateTime, 8, true, false, false, false, false), fillTypes));

        }

        private int GetTaxCategoryId(string taxCategory)
        {
            CatalogTaxDto taxes = CatalogTaxManager.GetTaxCategories();
            if (taxes.TaxCategory != null)
            {   
                DataRow[] drws = null;
                int taxCategoryId = 0;
                if (Int32.TryParse(taxCategory, out taxCategoryId))
                {
                    drws = taxes.TaxCategory.Select(String.Format("TaxCategoryId = '{0}'", taxCategoryId));
                    if (drws.Length > 0)
                    {
                        return taxCategoryId;
                    }
                }
                else
                {
                    drws = taxes.TaxCategory.Select(String.Format("Name LIKE '{0}'", taxCategory.Replace("'", "''")));
                    if (drws.Length > 0)
                    {
                        return ((CatalogTaxDto.TaxCategoryRow)drws[0]).TaxCategoryId;
                    }
                }
            }
            return 0;
        }

        private Guid GetMerchantId(string merchant)
        {
            CatalogEntryDto merchants = CatalogContext.Current.GetMerchantsDto();
            if (merchants.Merchant != null)
            {
                DataRow[] drws = null;
                Guid merchantId = new Guid();
                try
                {
                    merchantId = new Guid(merchant);
                }
                catch{}
                if (merchantId != Guid.Empty)
                {
                    drws = merchants.Merchant.Select(String.Format("MerchantId = '{0}'", merchantId));
                    if (drws.Length > 0)
                    {
                        return merchantId;
                    }
                }
                else
                {
                    drws = merchants.Merchant.Select(String.Format("Name LIKE '{0}'", merchant.Replace("'", "''")));
                    if (drws.Length > 0)
                    {
                        return ((CatalogEntryDto.MerchantRow)drws[0]).MerchantId;
                    }
                }
            }
            return Guid.Empty;
        }

        private int GetWarehouseId(string warehouse)
        {
            WarehouseDto warehouses = WarehouseManager.GetWarehouseDto();
            if (warehouses.Warehouse != null)
            {
                DataRow[] drws = null;
                int warehouseId = 0;
                if (Int32.TryParse(warehouse, out warehouseId))
                {
                    drws = warehouses.Warehouse.Select(String.Format("WarehouseId = '{0}'", warehouseId));
                    if (drws.Length > 0)
                    {
                        return warehouseId;
                    }
                }
                else
                {
                    drws = warehouses.Warehouse.Select(String.Format("Name LIKE '{0}'", warehouse.Replace("'", "''")));
                    if (drws.Length > 0)
                    {
                        return ((WarehouseDto.WarehouseRow)drws[0]).WarehouseId;
                    }
                }
            }
            return 0;
        }

        private int GetPackageId(string package)
        {
            ShippingMethodDto shippingDto = ShippingManager.GetShippingPackages();
            if (shippingDto.Package != null)
            {
                DataRow[] drws = null;
                int packageId = 0;
                if (Int32.TryParse(package, out packageId))
                {
                    drws = shippingDto.Package.Select(String.Format("PackageId = '{0}'", packageId));
                    if (drws.Length > 0)
                    {
                        return packageId;
                    }
                }
                else
                {
                    drws = shippingDto.Package.Select(String.Format("Name LIKE '{0}'", package.Replace("'", "''")));
                    if (drws.Length > 0)
                    {
                        return ((ShippingMethodDto.PackageRow)drws[0]).PackageId;
                    }
                }
            }
            return 0;
        }

        private string GetCatalogDefaultCurrency()
        {
            CatalogDto dto = CatalogManager.GetCatalogDto(this._CatalogId, new CatalogResponseGroup(CatalogResponseGroup.ResponseGroup.CatalogInfo));
            if (dto.Catalog.Count > 0)
            {
                return dto.Catalog[0].DefaultCurrency;
            }

            return String.Empty;
        }

		protected override int CreateSystemRow(FillDataMode Mode, int RowIndex, params object[] Item)
        {
            int i = 0;
			object objSysRowAction				 = Item[i++];
            object objCode						 = Item[i++];
            //Variation
            object objListPrice					 = Item[i++];
            object objTaxCategoryId				 = Item[i++];
            object objTrackInventory			 = Item[i++];
            object objMerchantId				 = Item[i++];
            object objWarehouseId				 = Item[i++];
            object objWeight					 = Item[i++];
            object objPackageId					 = Item[i++];
            object objMinQuantity				 = Item[i++];
            object objMaxQuantity				 = Item[i++];
            //Inventory
            object objInStockQuantity			 = Item[i++];
            object objReservedQuantity			 = Item[i++];
            object objReorderMinQuantity		 = Item[i++];
            object objPreorderQuantity			 = Item[i++];
            object objBackorderQuantity			 = Item[i++];
            object objAllowBackorder			 = Item[i++];
            object objAllowPreorder				 = Item[i++];
            object objInventoryStatus			 = Item[i++];
            object objPreorderAvailabilityDate	 = Item[i++];
            object objBackorderAvailabilityDate	 = Item[i++];
            
            CatalogEntryDto.VariationRow variationRow = null;
            CatalogEntryDto.InventoryRow inventoryRow = null;

            try
            {
				RowAction sysRowAction = RowAction.Default;

				if (objSysRowAction != null)
					sysRowAction = GetRowActionEnum((string)objSysRowAction);

                string Code;
                if (objCode != null) Code = (string)objCode;
                else throw new AbsentValue("Code");

                bool bVariationIsNew = false;
                bool bInventoryIsNew = false;
                CatalogEntryDto catalogEntryDto = CatalogEntryManager.GetCatalogEntryDto(Code, new CatalogEntryResponseGroup(CatalogEntryResponseGroup.ResponseGroup.CatalogEntryFull));
                if(catalogEntryDto.CatalogEntry.Count > 0)
                {
                    CatalogEntryDto.CatalogEntryRow entry = catalogEntryDto.CatalogEntry[0];
                    if (entry.ClassTypeId.Equals(EntryType.Variation, StringComparison.OrdinalIgnoreCase) ||
                        entry.ClassTypeId.Equals(EntryType.Package, StringComparison.OrdinalIgnoreCase))
                    {

                        if (catalogEntryDto.Variation.Count > 0)
                        {
							if (sysRowAction == RowAction.Insert)
								throw new MDPImportException(String.Format("The Variation with with Entry Code '{0}' already exists.", Code));

                            variationRow = catalogEntryDto.Variation[0];

							if (sysRowAction == RowAction.Delete)
								variationRow.Delete();
                        }
                        else
                        {
							if (sysRowAction == RowAction.Update)
								throw new MDPImportException(String.Format("The Variation with with Entry Code '{0}' does not exists.", Code));

							if (sysRowAction == RowAction.Delete)
								throw new MDPImportException(String.Format("The Variation with with Entry Code '{0}' does not exists.", Code));

                            variationRow = catalogEntryDto.Variation.NewVariationRow();
                            variationRow.CatalogEntryId = entry.CatalogEntryId;
                            variationRow.ListPrice = 0;
                            variationRow.TaxCategoryId = 0;
                            variationRow.TrackInventory = false;
                            variationRow.WarehouseId = 0;
                            variationRow.Weight = 1;
                            variationRow.PackageId = 0;
                            variationRow.MinQuantity = 1;
                            variationRow.MaxQuantity = 100;
                            bVariationIsNew = true;
                        }

                        if (catalogEntryDto.Inventory.Count > 0)
                        {
							if (sysRowAction == RowAction.Insert)
								throw new MDPImportException(String.Format("The Inventory with with Entry Code '{0}' already exists.", Code));

                            inventoryRow = catalogEntryDto.Inventory[0];

							if (sysRowAction == RowAction.Delete)
								inventoryRow.Delete();
                        }
                        else
                        {
							if (sysRowAction == RowAction.Update)
								throw new MDPImportException(String.Format("The Inventory with with Entry Code '{0}' does not exists.", Code));

							if (sysRowAction == RowAction.Delete)
								throw new MDPImportException(String.Format("The Inventory with with Entry Code '{0}' does not exists.", Code));

                            inventoryRow = catalogEntryDto.Inventory.NewInventoryRow();
                            inventoryRow.ApplicationId = CatalogConfiguration.Instance.ApplicationId;
                            inventoryRow.SkuId = entry.Code;
                            inventoryRow.InStockQuantity = 10;
                            inventoryRow.ReservedQuantity = 2;
                            inventoryRow.ReorderMinQuantity = 1;
                            inventoryRow.PreorderQuantity = 10;
                            inventoryRow.BackorderQuantity = 10;
                            inventoryRow.AllowBackorder = false;
                            inventoryRow.AllowPreorder = false;
                            inventoryRow.InventoryStatus = 0;
                            inventoryRow.PreorderAvailabilityDate = DateTime.UtcNow;
                            inventoryRow.BackorderAvailabilityDate = DateTime.UtcNow;
                            bInventoryIsNew = true;
                        }
                    }
                    else
                        throw new MDPImportException(String.Format("The Entry with code '{0}' has wrong type ('{1}') for Variation/Inventory import.", Code, entry.ClassTypeId));
                }
                else
                {
                    throw new MDPImportException(String.Format("The Entry with code '{0}' does not exists.", Code));
                }

				if (sysRowAction == RowAction.Delete)
				{
					CatalogContext.Current.SaveCatalogEntry(catalogEntryDto);
					return 0;
				}

                //Variation
                if (objListPrice != null)
                {
                    decimal ListPrice = (decimal)objListPrice;
                    variationRow.ListPrice = ListPrice;
                }

                if (objTaxCategoryId != null)
                    variationRow.TaxCategoryId = GetTaxCategoryId((string)objTaxCategoryId);

                if (objTrackInventory != null)
                    variationRow.TrackInventory = (bool)objTrackInventory;

                if (objMerchantId != null)
                {
                    Guid merchantId = GetMerchantId((string)objMerchantId);
                    if (merchantId != Guid.Empty)
                        variationRow.MerchantId = merchantId;
                }

                if (objWarehouseId != null)
                    variationRow.WarehouseId = GetWarehouseId((string)objWarehouseId);

                if (objWeight != null)
                    variationRow.Weight = (double)objWeight;

                if (objPackageId != null)
                    variationRow.PackageId = GetPackageId((string)objPackageId);

                if (objMinQuantity != null)
                    variationRow.MinQuantity = (decimal)objMinQuantity;

                if (objMaxQuantity != null)
                    variationRow.MaxQuantity = (decimal)objMaxQuantity;

                if (bVariationIsNew)
                    catalogEntryDto.Variation.AddVariationRow(variationRow);

                //Inventory
                if (objInStockQuantity != null)
                    inventoryRow.InStockQuantity = (decimal)objInStockQuantity;

                if (objReservedQuantity != null)
                    inventoryRow.ReservedQuantity = (decimal)objReservedQuantity;

                if (objReorderMinQuantity != null)
                    inventoryRow.ReorderMinQuantity = (decimal)objReorderMinQuantity;

                if (objPreorderQuantity != null)
                    inventoryRow.PreorderQuantity = (decimal)objPreorderQuantity;

                if (objBackorderQuantity != null)
                    inventoryRow.BackorderQuantity = (decimal)objBackorderQuantity;

                if (objAllowBackorder != null)
                    inventoryRow.AllowBackorder = (bool)objAllowBackorder;

                if (objAllowPreorder != null)
                    inventoryRow.AllowPreorder = (bool)objAllowPreorder;

                if (objInventoryStatus != null)
                    inventoryRow.InventoryStatus = (int)objInventoryStatus;

                if (objPreorderAvailabilityDate != null)
                    inventoryRow.PreorderAvailabilityDate = ((DateTime)objPreorderAvailabilityDate).ToUniversalTime();

                if (objBackorderAvailabilityDate != null)
                    inventoryRow.BackorderAvailabilityDate = ((DateTime)objBackorderAvailabilityDate).ToUniversalTime();

                if (bInventoryIsNew)
                    catalogEntryDto.Inventory.AddInventoryRow(inventoryRow);



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

            return variationRow.CatalogEntryId;
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
