namespace Mediachase.Cms.Web.Common
{
    using System;
    using System.Data;
    using System.Configuration;
    using System.Collections;
    using System.Web;
    using System.Web.Security;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;
    using System.Threading;
	using Mediachase.Commerce.Shared;
    using System.Globalization;
    using System.Collections.Generic;
    using Mediachase.Cms.Dto;
    using Mediachase.Cms.Util;
    using System.Collections.Specialized;

    public partial class LanguagePicker : System.Web.UI.UserControl
    {
        #region BindToolBar()
        /// <summary>
        /// Binds the toolbar.
        /// </summary>
        public void BindToolbar()
        {
            List<CultureInfo> dataSource = new List<CultureInfo>();
            SiteDto dto = CMSContext.Current.GetSiteDto(CMSContext.Current.SiteId);
            ComponentArt.Web.UI.MenuItem rootItem = CreateMenuItem(Thread.CurrentThread.CurrentCulture);
            rootItem.LookId = "TopItemLook";
            LanguageMenu.Items.Add(rootItem);
            foreach (SiteDto.SiteLanguageRow row in dto.SiteLanguage.Rows)
            {
                CultureInfo culture = CultureInfo.CreateSpecificCulture(row.LanguageCode);
                if (!Thread.CurrentThread.CurrentCulture.Name.Equals(row.LanguageCode))
                {
                    ComponentArt.Web.UI.MenuItem item = CreateMenuItem(culture);
                    NameValueCollection vals = new NameValueCollection();
                    vals.Add("lang", row.LanguageCode);
                    item.NavigateUrl = CommonHelper.FormatQueryString(CMSContext.Current.CurrentUrl, vals);
                    rootItem.Items.Add(item);
                }
            }

            LanguageMenu.DataBind();
        }

        /// <summary>
        /// Creates the menu item.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <returns></returns>
        private ComponentArt.Web.UI.MenuItem CreateMenuItem(CultureInfo culture)
        {
            ComponentArt.Web.UI.MenuItem item = new ComponentArt.Web.UI.MenuItem();
            item.Text = culture.DisplayName;
            item.Look.LeftIconUrl = CommonHelper.GetFlagIcon(culture);
            return item;
        }
        #endregion

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            BindToolbar();
        }
    }
}