using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mediachase.Cms.Util;
using System.ComponentModel;

namespace Mediachase.Cms.Website.Structure.Base.Controls.Cart.SharedModules
{
    /// <summary>
    ///  Displays quantity field for an item
    /// </summary>
    public partial class EntryQuantityControl : System.Web.UI.UserControl
    {
        decimal? _Quantity = null;
        /// <summary>
        /// Gets the quantity.
        /// </summary>
        /// <value>The quantity.</value>
        public decimal? Quantity
        {
            get
            {
                if (QauntityTextBox.Visible && !String.IsNullOrEmpty(QauntityTextBox.Text))
                    return Decimal.Parse(QauntityTextBox.Text);
                else if (QauntityDropDown.Visible && !String.IsNullOrEmpty(QauntityDropDown.SelectedValue))
                    return Decimal.Parse(QauntityDropDown.SelectedValue);

                return _Quantity;
            }
            set
            {
                _Quantity = value;
                EnsureChildControls();
            }
        }

        decimal _MinQuantity = 1;
        /// <summary>
        /// Gets or sets the min quantity.
        /// </summary>
        /// <value>The min quantity.</value>
        [Bindable(true)]
        public decimal MinQuantity
        {
            get
            {
                return _MinQuantity;
            }
            set
            {
                _MinQuantity = value;
                EnsureChildControls();
            }
        }

        decimal _MaxQuantity = 10;
        /// <summary>
        /// Gets or sets the min quantity.
        /// </summary>
        /// <value>The min quantity.</value>
        [Bindable(true)]
        public decimal MaxQuantity
        {
            get
            {
                return _MaxQuantity;
            }
            set
            {
                _MaxQuantity = value;
                EnsureChildControls();
            }
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Determines whether the server control contains child controls. If it does not, it creates child controls.
        /// </summary>
        protected override void EnsureChildControls()
        {
            BindQuantity();
            base.EnsureChildControls();
        }

        /// <summary>
        /// Binds the quantity.
        /// </summary>
        private void BindQuantity()
        {
            QauntityDropDown.Items.Clear();
            if (MaxQuantity <= 50)
            {
                for (decimal index = MinQuantity; index <= MaxQuantity; index++)
                {
                    QauntityDropDown.Items.Add(new ListItem(index.ToString("#"), index.ToString("#")));
                }
            }

            if (QauntityDropDown.Items.Count > 1)
            {
                QauntityDropDown.Visible = true;
                QauntityTextBox.Visible = false;
                QuantityLabel.Visible = false;
            }
            else if (QauntityDropDown.Items.Count == 1)
            {
                QauntityDropDown.Visible = false;
                QauntityTextBox.Visible = false;
                QuantityLabel.Visible = true;
            }
            else
            {
                QauntityDropDown.Visible = false;
                QauntityTextBox.Visible = true;
                QuantityLabel.Visible = false;
            }

            if (this._Quantity != null && QauntityTextBox.Visible)
                QauntityTextBox.Text = ((decimal)this._Quantity).ToString("#.##");
            if (this._Quantity != null && QauntityDropDown.Visible)
                CommonHelper.SelectListItem(QauntityDropDown, (Int32.Parse(((decimal)this._Quantity).ToString("#"))).ToString());
            if (this._Quantity != null && QuantityLabel.Visible)
                QuantityLabel.Text = ((decimal)this._Quantity).ToString("#.##");
        }
    }
}