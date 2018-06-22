using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Catalog.Managers;
using System.Collections;
using System.Runtime.Serialization;

namespace Mediachase.Commerce.Catalog.Objects
{
    /// <summary>
    /// Contains information about a category within catalog.
    /// </summary>
    [DataContract]
    public partial class CatalogNode
    {
        /// <summary>Node Id.</summary>
        private string _Id;

        /// <summary>
        /// Gets or sets the Catalog Node ID, which corresponds to the Code field in the database.
        /// </summary>
        /// <value>The ID.</value>
        public string ID
        {
            get { return _Id; }
            set { _Id = value; }
        }

        /// <summary>Catalog Id.</summary>
        private int _CatalogId;

        /// <summary>
        /// Gets or sets the Catalog ID, which corresponds to the CatalogId field in the database.
        /// </summary>
        /// <value>The ID.</value>
        public int CatalogId
        {
            get { return _CatalogId; }
            set { _CatalogId = value; }
        }


        /// <summary>Parent Node Id.</summary>
        private int _ParentNodeId;

        /// <summary>
        /// Gets or sets the Parent Node ID, which corresponds to the ParentNodeId field in the database.
        /// </summary>
        /// <value>The ID.</value>
        public int ParentNodeId
        {
            get { return _ParentNodeId; }
            set { _ParentNodeId = value; }
        }

        /// <summary>Catalog Node Id.</summary>
        private int _CatalogNodeId;

        /// <summary>
        /// Gets or sets the Catalog Node ID, which corresponds to the CatalogNodeId field in the database.
        /// </summary>
        /// <value>The ID.</value>
        public int CatalogNodeId
        {
            get { return _CatalogNodeId; }
            set { _CatalogNodeId = value; }
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

        private CatalogNode _ParentNode;

        /// <summary>
        /// Gets or sets the parent node.
        /// </summary>
        /// <value>The parent node.</value>
        public CatalogNode ParentNode
        {
            get { return _ParentNode; }
            set { _ParentNode = value; }
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

        /// <summary>Child categories</summary>
        private CatalogNodes _Children;

        /// <summary>
        /// Gets or sets the children.
        /// </summary>
        /// <value>The children.</value>
        public CatalogNodes Children
        {
            get { return _Children; }
            set { _Children = value; }
        }

        /// <summary>Parent categories</summary>
        private CatalogNodes _Ancestors;

        /// <summary>
        /// Gets or sets the ancestors.
        /// </summary>
        /// <value>The ancestors.</value>
        public CatalogNodes Ancestors
        {
            get { return _Ancestors; }
            set { _Ancestors = value; }
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

        /// <summary>
        /// Initializes a new instance of the <see cref="CatalogNode"/> class.
        /// </summary>
        public CatalogNode()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CatalogNode"/> class.
        /// </summary>
        /// <param name="input">The input.</param>
        public CatalogNode(CatalogNodeDto.CatalogNodeRow input)
        {
            // Populate basic parameters
            this.ID = input.Code;
            this.Name = input.Name;
            this.IsActive = input.IsActive;
            this.StartDate = input.StartDate;
            this.EndDate = input.EndDate;
            this.DisplayTemplate = input.TemplateName;
            this.ParentNodeId = input.ParentNodeId;
            this.CatalogId = input.CatalogId;
            this.CatalogNodeId = input.CatalogNodeId;

            // Populate attributes
            this.ItemAttributes = new ItemAttributes();

            // Cycle through columns
            ObjectHelper.CreateAttributes(this.ItemAttributes, input);

            // Populate SEO
            CatalogNodeDto.CatalogItemSeoRow[] seoRows = input.GetCatalogItemSeoRows();
            if (seoRows.Length > 0)
            {
                ArrayList seoList = new ArrayList();
                foreach (CatalogNodeDto.CatalogItemSeoRow seoRow in seoRows)
                {
                    Seo seo = new Seo();
                    seo.Description = seoRow.IsDescriptionNull() ? "" : seoRow.Description;
                    seo.Keywords = seoRow.IsKeywordsNull() ? "" : seoRow.Keywords;
                    seo.LanguageCode = seoRow.LanguageCode;
                    seo.Title = seoRow.IsTitleNull() ? "" : seoRow.Title;
                    seo.Uri = seoRow.Uri;
                    seoList.Add(seo);
                }

                this.SeoInfo = (Seo[])seoList.ToArray(typeof(Seo));
            }

            // Populate Assets
            CatalogNodeDto.CatalogItemAssetRow[] assetRows = input.GetCatalogItemAssetRows();
            if (assetRows.Length > 0)
            {
                List<ItemAsset> assets = new List<ItemAsset>();
                foreach (CatalogNodeDto.CatalogItemAssetRow assetRow in assetRows)
                {
                    ItemAsset asset = new ItemAsset();
                    asset.AssetKey = assetRow.AssetKey;
                    asset.AssetType = assetRow.AssetType;
                    asset.GroupName = assetRow.GroupName;
                    asset.SortOrder = assetRow.SortOrder;
                    assets.Add(asset);
                }

                this.Assets = assets.ToArray();
            }
        }
    } 
}
