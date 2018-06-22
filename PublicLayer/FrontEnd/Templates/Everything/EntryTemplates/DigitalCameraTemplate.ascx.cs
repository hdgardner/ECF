using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mediachase.Cms.WebUtility;
using Mediachase.Commerce.Catalog.Managers;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Objects;
using System.Collections;
using Mediachase.Cms.WebUtility.Commerce;
using System.Web.UI.HtmlControls;
using Mediachase.Cms.WebUtility.BaseControls;

namespace Mediachase.Cms.Website.Templates.Everything.EntryTemplates
{
    /// <summary>
    /// This templates renders digital camera type of product on the front end.
    /// </summary>
    public partial class DigitalCameraTemplate : BaseEntryTemplate
    {
        #region Private Variables
        private CatalogNode _BrandNode = null;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the brand node.
        /// </summary>
        /// <value>The brand node.</value>
        public CatalogNode BrandNode
        {
            get
            {
                if (_BrandNode == null)
                {
                    string brandName = Entry.ItemAttributes["BrandName"].ToString();
                    CatalogNode brandNode = CatalogContext.Current.GetCatalogNode(brandName);
                }

                return _BrandNode;
            }
        }
        #endregion

        /// <summary>
        /// Loads the new entry.
        /// </summary>
        /// <returns></returns>
        protected override Entry LoadNewEntry()
        {
            // Load using base implementation
            Entry entry = base.LoadNewEntry();

            // Add additional association to accomodate service plan and demonstrate dynamic products
            List<Association> associationList = new List<Association>();

            Association optionsAssoc = null;
            if (entry.Associations != null)
            {
                foreach (Association assoc in entry.Associations)
                {
                    if (assoc.Name.Equals("AdditionalOptions", StringComparison.OrdinalIgnoreCase))
                    {
                        optionsAssoc = assoc;
                    }

                    associationList.Add(assoc);
                }
            }

            if (optionsAssoc == null)
            {
                optionsAssoc = new Association();
                optionsAssoc.Name = "AdditionalOptions";
                optionsAssoc.Description = "Additional Options";
                associationList.Add(optionsAssoc);
            }

            List<EntryAssociation> entryAssociationList = new List<EntryAssociation>();

            if (optionsAssoc.EntryAssociations == null)
                optionsAssoc.EntryAssociations = new EntryAssociations();

            if (optionsAssoc.EntryAssociations.Association != null)
            {
                foreach (EntryAssociation assoc in optionsAssoc.EntryAssociations.Association)
                {
                    entryAssociationList.Add(assoc);
                }
            }

            // Add new association and made up entry
            Entry newEntry = new Entry();
            newEntry.ID = "@2YEARSERVICEPLAN-" + entry.ID;
            newEntry.Name = "2 Years Service Plan";
            newEntry.ItemAttributes = new ItemAttributes();
            newEntry.ItemAttributes.MinQuantity = 1;
            newEntry.ItemAttributes.MaxQuantity = 1;
            newEntry.ParentEntry = entry;

            Price listPrice = StoreHelper.GetSalePrice(entry, 1);
            newEntry.ItemAttributes.ListPrice = ObjectHelper.CreatePrice(decimal.Multiply(listPrice.Amount, (decimal)0.10), listPrice.CurrencyCode);

            EntryAssociation newAssociation = new EntryAssociation();
            newAssociation.AssociationType = "OPTIONAL";
            newAssociation.SortOrder = 0;
            newAssociation.AssociationDesc = "";
            newAssociation.Entry = newEntry;
            entryAssociationList.Add(newAssociation);

            optionsAssoc.EntryAssociations.Association = entryAssociationList.ToArray();
            entry.Associations = associationList.ToArray();

            return entry;
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            DataBind();
        }
    }
}