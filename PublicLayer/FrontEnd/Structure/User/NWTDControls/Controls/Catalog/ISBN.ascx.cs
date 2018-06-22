using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mediachase.Commerce.Catalog.DataSources;
using Mediachase.Commerce.Catalog.Objects;
using Mediachase.Cms.WebUtility.Commerce;
using System.ComponentModel;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Shared;

namespace Mediachase.Cms.Website.Structure.User.NWTDControls.Controls.Catalog{
    
	/// <summary>
	/// A control used to display an Entry's ISBN numbers. If an ISBN10 is unavailble it's not shown.
	/// This control is intended to be used in search results.
	/// </summary>
	public partial class ISBN : System.Web.UI.UserControl{

		/// <summary>
		/// The Entry whose ISBN info is being displayed.
		/// </summary>
        public Entry Entry { get; set; }

		/// <summary>
		/// All the logic for this control occurs during PreRender. ISBN numbers are bound to controls within this control and displayed appropriately.
		/// </summary>
		/// <param name="e"></param>
        protected override void OnPreRender(EventArgs e)
        {
            if (this.Entry != null)
            {
                //Hide the ISBN10 if it's missing or itentical to the ISBN13
                string isbn10 = this.Entry.ItemAttributes["ISBN10"].ToString().Trim();
                string isbn13 = this.Entry.ItemAttributes["ISBN13"].ToString().Trim();
                this.litISBN10.Text = isbn10;
                this.litISBN13.Text = isbn13;

                if (string.IsNullOrEmpty(isbn10) || isbn10.Equals(isbn13))
                {
                    this.lblISBN10.Visible = false;
                }
            }
            base.OnPreRender(e);
        }

        protected void Page_Load(object sender, EventArgs e){

        }
    }
}