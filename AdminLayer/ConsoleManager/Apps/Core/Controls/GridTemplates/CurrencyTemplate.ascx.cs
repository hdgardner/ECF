using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using ComponentArt.Web.UI;
using System.ComponentModel;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Commerce.Shared;

namespace Mediachase.Commerce.Manager.Core.Controls.GridTemplates
{
    public partial class CurrencyTemplate : BaseUserControl
    {
        private string _FormatString = String.Empty;

        /// <summary>
        /// Gets or sets the format string.
        /// </summary>
        /// <value>The format string.</value>
        public string FormatString
        {
            get { return _FormatString; }
            set { _FormatString = value; }
        }

        private string _CurrencyCode = String.Empty;

        /// <summary>
        /// Gets or sets the currency argument.
        /// </summary>
        /// <value>The currency argument.</value>
        public string CurrencyArgument
        {
            get { return _CurrencyCode; }
            set { _CurrencyCode = value; }
        }

        private string _Argument = null;

        /// <summary>
        /// Gets or sets the data argument.
        /// </summary>
        /// <value>The data argument.</value>
        public string DataArgument
        {
            get { return _Argument; }
            set { _Argument = value; }
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(DataArgument) || String.IsNullOrEmpty(CurrencyArgument))
                return;

            string formattedCurrency = CurrencyFormatter.FormatCurrency(Decimal.Parse(((GridServerTemplateContainer)this.Parent).DataItem[DataArgument].ToString()), ((GridServerTemplateContainer)this.Parent).DataItem[CurrencyArgument].ToString());

            if (!String.IsNullOrEmpty(formattedCurrency))
                MyText.Text = String.Format(FormatString, formattedCurrency);
        }
    }
}