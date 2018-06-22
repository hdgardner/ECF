using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Commerce.Catalog.Dto;
using System.Data;
using Mediachase.Commerce.Catalog.Managers;
using System.Collections;
using System.Runtime.Serialization;
using Mediachase.MetaDataPlus.Configurator;
using Mediachase.MetaDataPlus;
using Mediachase.Commerce.Storage;

namespace Mediachase.Commerce.Catalog.Objects
{
    /// <summary>
    /// The Entry parameter serves as a container element that is a child of the Entries element, and 
    /// represents Catalog Entry element, which can be Product, Variation, Bundle or any other type of the product.
    /// </summary>
    [DataContract]
    public partial class Entry
    {
		/// <summary>EntryId.</summary>
		private int _CatalogEntryId;

        /// <summary>
        /// Gets or sets the catalog entry id.
        /// </summary>
        /// <value>The catalog entry id.</value>
        public int CatalogEntryId
		{
            get { return _CatalogEntryId; }
            set { _CatalogEntryId = value; }
		}

        /// <summary>Entry Id (=Code).</summary>
        private string _Id;

        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        /// <value>The ID.</value>
        public string ID
        {
            get { return _Id; }
            set { _Id = value; }
        }

        private string _Name;

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        private Entry _ParentEntry;

        /// <summary>
        /// Gets or sets the parent entry.
        /// </summary>
        /// <value>The parent entry.</value>
        public Entry ParentEntry
        {
            get { return _ParentEntry; }
            set { _ParentEntry = value; }
        }


        private DateTime _StartDate;

        /// <summary>
        /// Gets or sets the start date.
        /// </summary>
        /// <value>The start date.</value>
        public DateTime StartDate
        {
            get { return _StartDate; }
            set { _StartDate = value; }
        }
        private DateTime _EndDate;

        /// <summary>
        /// Gets or sets the end date.
        /// </summary>
        /// <value>The end date.</value>
        public DateTime EndDate
        {
            get { return _EndDate; }
            set { _EndDate = value; }
        }
        private bool _IsActive;

        /// <summary>
        /// Gets or sets a value indicating whether this instance is active.
        /// </summary>
        /// <value><c>true</c> if this instance is active; otherwise, <c>false</c>.</value>
        public bool IsActive
        {
            get { return _IsActive; }
            set { _IsActive = value; }
        }

        private string _DisplayTemplate;

        /// <summary>
        /// Gets or sets the display template.
        /// </summary>
        /// <value>The display template.</value>
        public string DisplayTemplate
        {
            get { return _DisplayTemplate; }
            set { _DisplayTemplate = value; }
        }

		/// <summary>Entry MetaClassId.</summary>
		private int _MetaClassId;

		/// <summary>
		/// Gets or sets the MetaClassId.
		/// </summary>
		/// <value>The MetaClassId.</value>
		public int MetaClassId
		{
			get { return _MetaClassId; }
			set { _MetaClassId = value; }
		}

        /// <summary>Collection of Item attributes.</summary>
        private ItemAttributes _ItemAttributes;

        /// <summary>
        /// Gets or sets the item attributes.
        /// </summary>
        /// <value>The item attributes.</value>
        public ItemAttributes ItemAttributes
        {
            get { return _ItemAttributes; }
            set { _ItemAttributes = value; }
        }

        private RelationInfo _RelationInfo;

        /// <summary>
        /// Gets or sets the relation info.
        /// </summary>
        /// <value>The relation info.</value>
        public RelationInfo RelationInfo
        {
            get { return _RelationInfo; }
            set { _RelationInfo = value; }
        }

        private Seo[] _Seo;

        /// <summary>
        /// Gets or sets the seo info.
        /// </summary>
        /// <value>The seo info.</value>
        public Seo[] SeoInfo
        {
            get { return _Seo; }
            set { _Seo = value; }
        }

        /// <remarks/>
        private SalePrices _SalePrices;
        /// <summary>
        /// Gets or sets the sale prices.
        /// </summary>
        /// <value>The sale prices.</value>
        public SalePrices SalePrices
        {
            get { return _SalePrices; }
            set { _SalePrices = value; }
        }

        private string _EntryType;
        /// <summary>
        /// Gets or sets the type of the entry.
        /// </summary>
        /// <value>The type of the entry.</value>
        public string EntryType
        {
            get { return _EntryType; }
            set { _EntryType = value; }
        }

        private Entries _Entries;
        /// <summary>
        /// Gets or sets the entries.
        /// </summary>
        /// <value>The entries.</value>
        public Entries Entries
        {
            get { return _Entries; }
            set { _Entries = value; }
        }

        private Inventory _Inventory;
        /// <summary>
        /// Gets or sets the inventory.
        /// </summary>
        /// <value>The inventory.</value>
        public Inventory Inventory
        {
            get
            {
                return _Inventory;
            }
            set
            {
                _Inventory = value;
            }
        }

        private Association[] _Associations;
        /// <summary>
        /// Gets or sets the associations.
        /// </summary>
        /// <value>The associations.</value>
        public Association[] Associations
        {
            get
            {
                return _Associations;
            }
            set
            {
                _Associations = value;
            }
        }

        private ItemAsset[] _Assets;

        /// <summary>
        /// Gets or sets the assets.
        /// </summary>
        /// <value>The assets.</value>
        public ItemAsset[] Assets
        {
            get
            {
                return _Assets;
            }
            set
            {
                _Assets = value;
            }
        }

        /// <remarks/>
        private CatalogNodes _Nodes;
        /// <summary>
        /// Gets or sets the nodes.
        /// </summary>
        /// <value>The nodes.</value>
        public CatalogNodes Nodes
        {
            get { return _Nodes; }
            set { _Nodes = value; }
        }

		private MetaClass _MetaClass;

		/// <summary>
		/// Returns the MetaClass.
		/// </summary>
		public MetaClass GetEntryMetaClass()
		{
			if (_MetaClass == null)
			{
                _MetaClass = MetaHelper.LoadMetaClassCached(CatalogContext.MetaDataContext, MetaClassId);
			}
			return _MetaClass;
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="Entry"/> class.
        /// </summary>
        public Entry()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Entry"/> class.
        /// </summary>
        /// <param name="input">The input.</param>
        public Entry(CatalogEntryDto.CatalogEntryRow input)
        {
            // Populate basic parameters
			this.CatalogEntryId = input.CatalogEntryId;
            this.ID = input.Code;
            this.EntryType = input.ClassTypeId;
            this.Name = input.Name;
            this.IsActive = input.IsActive;
            this.StartDate = input.StartDate;
            this.EndDate = input.EndDate;
            this.DisplayTemplate = input.TemplateName;
			this.MetaClassId = input.MetaClassId;

            // Populate attributes
            this.ItemAttributes = new ItemAttributes();

            // Cycle through columns
            ObjectHelper.CreateAttributes(this.ItemAttributes, input);

            // Populate variations
            CatalogEntryDto.VariationRow[] variationRows = input.GetVariationRows();
            if (variationRows.Length > 0)
            {
                string currencyCode = "USD";
                string weightCode = "LBS";

                // Get catalog info
                CatalogDto catalogDto = CatalogContext.Current.GetCatalogDto(input.CatalogId);
                if (catalogDto.Catalog.Count != 0)
                {
                    currencyCode = catalogDto.Catalog[0].DefaultCurrency;
                    weightCode = catalogDto.Catalog[0].WeightBase;
                }

                foreach (CatalogEntryDto.VariationRow variationRow in variationRows)
                {
                    this.ItemAttributes.ListPrice = ObjectHelper.CreatePrice(variationRow.ListPrice, currencyCode);
                    this.ItemAttributes.Weight = ObjectHelper.CreateUnitsAttribute(weightCode, variationRow.Weight.ToString());
                    this.ItemAttributes.MinQuantity = variationRow.MinQuantity;
                    this.ItemAttributes.MaxQuantity = variationRow.MaxQuantity;
                }
            }

            // Populate sale prices
            CatalogEntryDto.SalePriceRow[] priceRows = input.GetSalePriceRows();
            if (priceRows.Length > 0)
            {
                List<SalePrice> priceList = new List<SalePrice>();
                foreach (CatalogEntryDto.SalePriceRow priceRow in priceRows)
                {
                    SalePrice price = new SalePrice();
                    if (!priceRow.IsEndDateNull())
                        price.EndDate = priceRow.EndDate;

                    price.MinQuantity = priceRow.MinQuantity;
                    if(!priceRow.IsSaleCodeNull())
                        price.SaleCode = priceRow.SaleCode;

                    price.SaleType = SaleType.GetKey(priceRow.SaleType);
                    price.StartDate = priceRow.StartDate;
                    price.UnitPrice = ObjectHelper.CreatePrice(priceRow.UnitPrice, priceRow.Currency);
                    priceList.Add(price);
                }

                this.SalePrices = new SalePrices();
                this.SalePrices.SalePrice = priceList.ToArray();
            }

            // Populate Inventory
            if (input.InventoryRow != null)
            {
                this.Inventory = new Inventory();
                this.Inventory.AllowBackorder = input.InventoryRow.AllowBackorder;
                this.Inventory.BackorderAvailabilityDate = input.InventoryRow.BackorderAvailabilityDate;
                this.Inventory.BackorderQuantity = input.InventoryRow.BackorderQuantity;
                this.Inventory.InStockQuantity = input.InventoryRow.InStockQuantity;

                if(input.InventoryRow.InventoryStatus == 0)
                    this.Inventory.InventoryStatus = "Disabled";
                else if(input.InventoryRow.InventoryStatus == 1)
                    this.Inventory.InventoryStatus = "Enabled";
                else if(input.InventoryRow.InventoryStatus == 2)
                    this.Inventory.InventoryStatus = "Ignored";

                this.Inventory.AllowPreorder = input.InventoryRow.AllowPreorder;
                this.Inventory.PreorderAvailabilityDate = input.InventoryRow.PreorderAvailabilityDate;
                this.Inventory.PreorderQuantity = input.InventoryRow.PreorderQuantity;
                this.Inventory.ReorderMinQuantity = input.InventoryRow.ReorderMinQuantity;
                this.Inventory.ReservedQuantity = input.InventoryRow.ReservedQuantity;
            }

            // Populate Associations (basic names)
            CatalogEntryDto.CatalogAssociationRow[] associationRows = input.GetCatalogAssociationRows();
            if(associationRows.Length > 0)
            {
                List<Association> associationList = new List<Association>();

                foreach (CatalogEntryDto.CatalogAssociationRow associationRow in associationRows)
                {
                    Association association = new Association();
                    
                    if(!associationRow.IsAssociationDescriptionNull())
                        association.Description = associationRow.AssociationDescription;

                    association.Name = associationRow.AssociationName;
                    associationList.Add(association);
                }

                this.Associations = associationList.ToArray();
            }

            // Populate SEO
            CatalogEntryDto.CatalogItemSeoRow[] seoRows = input.GetCatalogItemSeoRows();
            if (seoRows.Length > 0)
            {
                ArrayList seoList = new ArrayList();
                foreach (CatalogEntryDto.CatalogItemSeoRow seoRow in seoRows)
                {
                    Seo seo = new Seo();
                    if (!seoRow.IsDescriptionNull())
                        seo.Description = seoRow.Description;

                    if (!seoRow.IsKeywordsNull())
                        seo.Keywords = seoRow.Keywords;

                    seo.LanguageCode = seoRow.LanguageCode;

                    if (!seoRow.IsTitleNull())
                        seo.Title = seoRow.Title;

                    seo.Uri = seoRow.Uri;
                    seoList.Add(seo);
                }

                this.SeoInfo = (Seo[])seoList.ToArray(typeof(Seo));
            }

            // Populate Assets
            CatalogEntryDto.CatalogItemAssetRow[] assetRows = input.GetCatalogItemAssetRows();
            if (assetRows.Length > 0)
            {
                List<ItemAsset> assets = new List<ItemAsset>();
                foreach (CatalogEntryDto.CatalogItemAssetRow assetRow in assetRows)
                {
                    ItemAsset asset = new ItemAsset();
                    asset.AssetKey = assetRow.AssetKey;
                    asset.AssetType = assetRow.AssetType;
                    
                    if(!assetRow.IsGroupNameNull())
                        asset.GroupName = assetRow.GroupName;

                    asset.SortOrder = assetRow.SortOrder;
                    assets.Add(asset);
                }

                this.Assets = assets.ToArray();
            }

            // Populate Relation Info
            if (input.Table.Columns.Contains("RelationTypeId"))
            {
                this.RelationInfo = new RelationInfo();
                this.RelationInfo.Quantity = Decimal.Parse(input["Quantity"].ToString());
                this.RelationInfo.RelationType = input["RelationTypeId"].ToString();
                this.RelationInfo.SortOrder = Int32.Parse(input["SortOrder"].ToString());
                RelationInfo.GroupName = input["GroupName"].ToString();
            }
        }
    }
}
