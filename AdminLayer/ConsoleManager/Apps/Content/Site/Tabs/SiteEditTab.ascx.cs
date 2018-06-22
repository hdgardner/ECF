using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Web.UI.WebControls;
using Mediachase.Cms.Dto;
using Mediachase.Cms.ImportExport;
using Mediachase.Cms.Managers;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Common;
using Mediachase.Web.Console.Interfaces;
using mc = Mediachase.Cms;

namespace Mediachase.Commerce.Manager.Site.Tabs
{
    public partial class SiteEditTab : BaseUserControl, IAdminTabControl, IAdminContextControl, IPreCommit
    {
		private const string _SiteIdString = "SiteId";
		private const string _SiteDtoSourceString = "SiteDtoSrc";
		private const string _SiteDtoDestinationString = "SiteDtoDest";
		private const string _SiteEditSessionKey = "ECF.Site.Edit";

		private readonly string _SiteTemplatesFolder = "~/App_Data/SiteTemplates";

		private SiteDto _SiteDtoSrc = null;
		private SiteDto _SiteDtoDest = null;

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
			SiteTemplateTableRow.Visible = SiteId == Guid.Empty ? true : false;
        }

        /// <summary>
        /// Binds the form.
        /// </summary>
        private void BindForm()
        {
			if (SiteTemplateTableRow.Visible)
				BindSiteTemplatesDropdown();

            BindDropDown();

			if (SiteId == Guid.Empty && !NeedCopy)
			{
				BindLanguages(null);
				return;
			}

			SiteDto dto = _SiteDtoSrc;
			
            if (dto.Site.Count > 0)
            {
				SiteName.Text = dto.Site[0].Name;
                SiteDescription.Text = dto.Site[0].Description;
                IsSiteActive.IsSelected = dto.Site[0].IsActive;
                IsSiteDefault.IsSelected = dto.Site[0].IsDefault;
                SiteFolder.Text = dto.Site[0].Folder;
                SiteDomains.Text = dto.Site[0].Domain;
            }

            BindLanguages(dto);

            txtURL.Text = mc.GlobalVariable.GetVariable("url", SiteId);
			txtEmail.Text = mc.GlobalVariable.GetVariable("email", SiteId);
			txtPhone.Text = mc.GlobalVariable.GetVariable("phone", SiteId);
			txtAddress.Text = mc.GlobalVariable.GetVariable("address", SiteId);
			txtKeywords.Text = mc.GlobalVariable.GetVariable("meta_keywords", SiteId);
			txtDescription.Text = mc.GlobalVariable.GetVariable("meta_description", SiteId);
			txtTitle.Text = mc.GlobalVariable.GetVariable("title", SiteId);
			PageInclude.Text = mc.GlobalVariable.GetVariable("page_include", SiteId);
			ddTemplate.SelectedValue = mc.GlobalVariable.GetVariable("default_template", SiteId);
			SiteTheme.Text = mc.GlobalVariable.GetVariable("sitetheme", SiteId);
			AdminUrl.Text = mc.GlobalVariable.GetVariable("cm_url", SiteId);
        }

        /// <summary>
        /// Binds the languages.
        /// </summary>
        /// <param name="dto">The dto.</param>
        private void BindLanguages(SiteDto dto)
        {
            List<CultureInfo> leftSource = new List<CultureInfo>();
            List<CultureInfo> rightSource = new List<CultureInfo>();

            foreach (DataRow dataRow in mc.Language.GetAllLanguagesDT().Rows)
            {
                bool found = false;

                if (dto != null)
                {
                    if (dto.SiteLanguage.Count > 0)
                    {
                        foreach (SiteDto.SiteLanguageRow row in dto.SiteLanguage.Rows)
                        {
                            if (row.LanguageCode.Equals(dataRow["LangName"].ToString()))
                            {
                                found = true;
                                break;
                            }
                        }
                    }
                }

                if (found)
                    rightSource.Add(CultureInfo.CreateSpecificCulture(dataRow["LangName"].ToString()));
                else
                    leftSource.Add(CultureInfo.CreateSpecificCulture(dataRow["LangName"].ToString()));
            }

            LanguageList.LeftDataSource = leftSource;
            LanguageList.RightDataSource = rightSource;
            LanguageList.DataBind();
        }

        /// <summary>
        /// Binds the drop down.
        /// </summary>
        private void BindDropDown()
        {
            TemplateDto templates = DictionaryManager.GetTemplateDto();
            DataView templateView = templates.main_Templates.DefaultView;

            templateView.RowFilter = String.Format("TemplateType = '{0}'", "page");

            ddTemplate.DataSource = templateView;
            ddTemplate.DataTextField = "FriendlyName";
            ddTemplate.DataValueField = "TemplateId";
            ddTemplate.DataBind();            
        }

        /// <summary>
        /// Binds the site templates dropdown.
        /// </summary>
		private void BindSiteTemplatesDropdown()
		{
			SiteTemplatesList.Items.Clear();
            SiteTemplatesList.Items.Add(new ListItem("(empty)", String.Empty));
            string templateDir = MapPath(_SiteTemplatesFolder);
            if (Directory.Exists(templateDir))
            {
                DirectoryInfo dir = new DirectoryInfo(MapPath(_SiteTemplatesFolder));
                FileInfo[] files = dir.GetFiles("*.xml");
                foreach (FileInfo fi in files)
                    SiteTemplatesList.Items.Add(new ListItem(Path.GetFileNameWithoutExtension(fi.FullName), fi.FullName));
            }
			SiteTemplatesList.DataBind();
		}

        /// <summary>
        /// Binds a data source to the invoked server control and all its child controls.
        /// </summary>
        public override void DataBind()
        {
            base.DataBind();
            BindForm();
        }

		#region IAdminContextControl Members
        /// <summary>
        /// Loads the context.
        /// </summary>
        /// <param name="context">The context.</param>
		public void LoadContext(IDictionary context)
		{
			_SiteDtoSrc = (SiteDto)context[_SiteDtoSourceString];
			_SiteDtoDest = (SiteDto)context[_SiteDtoDestinationString];
		}
		#endregion

        #region IAdminTabControl Members

        /// <summary>
        /// Saves the changes.
        /// </summary>
        /// <param name="context">The context.</param>
        public void SaveChanges(IDictionary context)
        {
            SiteDto dto = (SiteDto)context[_SiteDtoDestinationString];
            SiteDto.SiteRow row = null;

            if (dto.Site != null && dto.Site.Count > 0)
            {
                row = dto.Site[0];
            }
            else
            {
				if (!String.IsNullOrEmpty(SiteTemplatesList.SelectedValue))
				{
					// create site from template
					FileStream fs = new FileStream(SiteTemplatesList.SelectedValue, FileMode.Open, FileAccess.Read);
					ImportExportHelper helper = new ImportExportHelper();
					Guid[] ids = helper.ImportSite(fs, mc.CmsConfiguration.Instance.ApplicationId, Guid.Empty, true);
					if (ids.Length > 0)
					{
						context[_SiteDtoDestinationString] = mc.CMSContext.Current.GetSiteDto(ids[0], true);
						dto = (SiteDto)context[_SiteDtoDestinationString];
						if (dto.Site.Count > 0)
							row = dto.Site[0];
						else
							throw new Exception(String.Format("Could not load the site after importing. id={0}", ids[0]));
					}
					else
						throw new Exception("Import function did not return siteId.");
				}
				else
				{
					row = dto.Site.NewSiteRow();
					row.SiteId = Guid.NewGuid();
					row.ApplicationId = mc.CmsConfiguration.Instance.ApplicationId;
				}
            }

            row.Name = SiteName.Text;
            row.Description = SiteDescription.Text;
            row.IsActive = IsSiteActive.IsSelected;
            row.IsDefault = IsSiteDefault.IsSelected;
            row.Folder = SiteFolder.Text;
            row.Domain = SiteDomains.Text;

            if (row.RowState == DataRowState.Detached)
            {
                dto.Site.Rows.Add(row);
                /*

                SiteDto.ApplicationSiteRow appRow = dto.ApplicationSite.NewApplicationSiteRow();
                appRow.SiteId = row.SiteId;
                appRow.ApplicationId = CatalogConfiguration.ApplicationId;
                dto.ApplicationSite.Rows.Add(appRow);
                 * */
            }

            // Populate languages
            // Remove existing languages
            foreach(SiteDto.SiteLanguageRow langRow in dto.SiteLanguage.Rows)
            {
                if (langRow.RowState == DataRowState.Deleted)
                    continue;

                bool found = false;
                foreach (ListItem item in LanguageList.RightItems)
                {
                    if (item.Value.Equals(langRow.LanguageCode))
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                    langRow.Delete();
            }

            foreach (ListItem item in LanguageList.RightItems)
            {
                bool exists = false;
                foreach (SiteDto.SiteLanguageRow langRow in dto.SiteLanguage.Rows)
                {
                    if (langRow.RowState != DataRowState.Deleted)
                    {
                        if (langRow.LanguageCode.Equals(item.Value))
                        {
                            exists = true;
                        }
                    }
                }

                if (!exists)
                {
                    SiteDto.SiteLanguageRow langRow = dto.SiteLanguage.NewSiteLanguageRow();
                    langRow.SiteId = row.SiteId;
                    langRow.LanguageCode = item.Value;
                    dto.SiteLanguage.Rows.Add(langRow);
                }
            }
        }
        #endregion

		#region IPreCommit Members

		public void PreCommitChanges(IDictionary context)
		{
			SiteDto dto = (SiteDto)context[_SiteDtoDestinationString];
			if (dto != null && dto.Site.Count > 0)
			{
				SiteDto.SiteRow row = dto.Site[0];

				// save Global variables for the site (afterthe site is saved)
				while (txtURL.Text.EndsWith("/"))
					txtURL.Text = txtURL.Text.Substring(0, txtURL.Text.Length - 1);
				mc.GlobalVariable.SetVariable("url", txtURL.Text, row.SiteId);
				mc.GlobalVariable.SetVariable("email", txtEmail.Text, row.SiteId);
				mc.GlobalVariable.SetVariable("phone", txtPhone.Text, row.SiteId);
				mc.GlobalVariable.SetVariable("address", txtAddress.Text, row.SiteId);
				mc.GlobalVariable.SetVariable("meta_keywords", txtKeywords.Text, row.SiteId);
				mc.GlobalVariable.SetVariable("meta_description", txtDescription.Text, row.SiteId);
				mc.GlobalVariable.SetVariable("default_template", ddTemplate.SelectedValue, row.SiteId);
				mc.GlobalVariable.SetVariable("page_include", PageInclude.Text, row.SiteId);
				mc.GlobalVariable.SetVariable("title", txtTitle.Text, row.SiteId);
				mc.GlobalVariable.SetVariable("sitetheme", SiteTheme.Text, row.SiteId);
				mc.GlobalVariable.SetVariable("cm_url", AdminUrl.Text, row.SiteId);
			}
		}

		#endregion
	}
}