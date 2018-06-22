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
using System.Collections.Generic;
using System.Collections.Specialized;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Interfaces;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Web.Console.Common;

namespace Mediachase.Commerce.Manager.Catalog.Modules
{
    public partial class TemplateTab : BaseUserControl, IAdminTabControl
    {
        private DataRow _DataRow;
        /// <summary>
        /// Gets or sets the row.
        /// </summary>
        /// <value>The row.</value>
        public DataRow Row
        {
            get
            {
                return _DataRow;
            }
            set
            {
                _DataRow = value;
            }
        }

        private string _LanguageCode;
        /// <summary>
        /// Gets or sets the language code.
        /// </summary>
        /// <value>The language code.</value>
        public string LanguageCode
        {
            get
            {
                return _LanguageCode;
            }
            set
            {
                _LanguageCode = value;
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
        /// Binds the form.
        /// </summary>
        private void BindForm()
        {
            if (Row != null)
            {
                Title.Text = Row["Title"].ToString();
                Keywords.Text = Row["Keywords"].ToString();
                Description.Text = Row["Description"].ToString();
                UrlText.Text = Row["Uri"].ToString();
            }
        }

        /// <summary>
        /// Binds a data source to the invoked server control and all its child controls.
        /// </summary>
        public override void DataBind()
        {
            base.DataBind();
            BindForm();
        }


        #region IAdminTabControl Members
        /// <summary>
        /// Saves the changes.
        /// </summary>
        /// <param name="context">The context.</param>
        public void SaveChanges(IDictionary context)
        {
            DataRow row = (DataRow)context["CatalogItemSeoRow"];

            row["Keywords"] = Keywords.Text;
            row["LanguageCode"] = LanguageCode;
            row["Description"] = Description.Text;
            row["Uri"] = UrlText.Text;
            row["Title"] = Title.Text;
        }
        #endregion
    }
}