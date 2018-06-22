using System;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Cms;
using Mediachase.Cms.Dto;
using Mediachase.Cms.ImportExport;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Manager.Core;
using Mediachase.Web.Console.Common;

namespace Mediachase.Commerce.Manager.Content.Site
{
    public partial class SiteEdit : BaseUserControl
    {
		private const string _SiteIdString = "SiteId";
		private const string _SiteDtoSourceString = "SiteDtoSrc";
		private const string _SiteDtoDestinationString = "SiteDtoDest";
		private const string _SiteEditSessionKey = "ECF.Site.Edit";

        /// <summary>
        /// Gets the site id.
        /// </summary>
        /// <value>The site id.</value>
        public Guid SiteId
        {
            get
            {
				return ManagementHelper.GetGuidFromQueryString(_SiteIdString);
            }
        }

		/// <summary>
		/// True, if the site needs to be copied. Otherwise, an existing site will be edited or a new one created.
		/// </summary>
		/// <value></value>
		public bool NeedCopy
		{
			get
			{
				bool needCopy = false;
				if (Parameters["cmd"] != null)
					needCopy = String.Compare(Parameters["cmd"].ToString(), "COPY", StringComparison.InvariantCultureIgnoreCase) == 0;
				return needCopy;
			}
		}


        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadContext();

            if (!this.IsPostBack)
                DataBind();
        }

        /// <summary>
        /// Loads the fresh.
        /// </summary>
        /// <returns></returns>
        private SiteDto LoadFresh()
        {
			SiteDto dto = null;

			if (SiteId != Guid.Empty && NeedCopy)
				dto = new SiteDto();
			else
				dto = CMSContext.Current.GetSiteDto(SiteId, true);

            // persist in session
            Session[_SiteEditSessionKey] = dto;

            return dto;
        }

        /// <summary>
        /// Loads the context.
        /// </summary>
        private void LoadContext()
        {
            //if (SiteId != Guid.Empty)
            {
                SiteDto dto = null;
                if (!this.IsPostBack && (!this.Request.QueryString.ToString().Contains("Callback=yes"))) // load fresh on initial load
                {
                    dto = LoadFresh();
                }
                else // load from session
                {
                    dto = (SiteDto)Session[_SiteEditSessionKey];

                    if (dto == null)
                        dto = LoadFresh();
                }

                // Put a dictionary key that can be used by other tabs
                IDictionary dic = new ListDictionary();

				if (SiteId != Guid.Empty && NeedCopy)
					// add existing site with specified SiteId
					dic.Add(_SiteDtoSourceString, CMSContext.Current.GetSiteDto(SiteId, true));
				else
					dic.Add(_SiteDtoSourceString, dto);

				// add current site (new or existing)
                dic.Add(_SiteDtoDestinationString, dto);

                // Call tabs load context
                ViewControl.LoadContext(dic);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            EditSaveControl.SaveChanges += new EventHandler<SaveControl.SaveEventArgs>(EditSaveControl_SaveChanges);
        }

        /// <summary>
        /// Handles the SaveChanges event of the EditSaveControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void EditSaveControl_SaveChanges(object sender, SaveControl.SaveEventArgs e)
        {
			// Validate form
			if (!this.Page.IsValid)
			{
				e.RunScript = false;
				return;
			}

			try
			{
				// Always load, since we need application data
				SiteDto dto = (SiteDto)Session[_SiteEditSessionKey];

				if (dto == null && SiteId != Guid.Empty)
				{
                    if (NeedCopy)
                    {
                        dto = new SiteDto();
                    }
                    else
                        dto = CMSContext.Current.GetSiteDto(SiteId, true);
				}
				else if (dto == null && SiteId == Guid.Empty)
					dto = new SiteDto();

				// Put a dictionary key that can be used by other tabs
				IDictionary dic = new ListDictionary();
				dic.Add(_SiteDtoDestinationString, dto);

				dto.EnforceConstraints = false;

				ViewControl.SaveChanges(dic);

				// get saveddto from the dictionary
				dto = (SiteDto)dic[_SiteDtoDestinationString];

				// Save modifications
				if (dto.HasChanges())
				{
					if (dto.Site.Count > 0)
					{
						// update siteId for the GlobalVariable rows that have just been addded
						SiteDto.main_GlobalVariablesRow[] variableRows = (SiteDto.main_GlobalVariablesRow[])dto.main_GlobalVariables.Select("", "", DataViewRowState.Added);
						if (variableRows != null && variableRows.Length > 0)
						{
							foreach (SiteDto.main_GlobalVariablesRow row in variableRows)
								row.SiteId = dto.Site[0].SiteId;
						}
					}

					dto.EnforceConstraints = true;

					CMSContext.Current.SaveSite(dto);

                    if (NeedCopy && SiteId != Guid.Empty)
                    {
                        CopyHelper helper = new CopyHelper();

                        //Menu copy
                        helper.CopySiteMenu(SiteId, dto.Site[0].SiteId);

                        //Folders and pages copy
                        helper.CopySiteContent(SiteId, dto.Site[0].SiteId);
                    }
				}

				// Call commit changes
				ViewControl.CommitChanges(dic);
			}
			catch (SiteImportExportException ex)
			{
				e.RunScript = false;
				DisplayErrorMessage(ex.Message);
			}
			finally
			{
				// we don't need to store Dto in session any more
				Session.Remove(_SiteEditSessionKey);
			}
        }
    }
}