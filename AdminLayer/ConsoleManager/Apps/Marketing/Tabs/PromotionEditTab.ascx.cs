using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using ComponentArt.Web.UI;
using Mediachase.Cms;
using Mediachase.Commerce.Marketing;
using Mediachase.Commerce.Marketing.Dto;
using Mediachase.Commerce.Marketing.Managers;
using Mediachase.Commerce.Shared;
using Mediachase.Web.Console;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Common;
using Mediachase.Web.Console.Interfaces;

namespace Mediachase.Commerce.Manager.Marketing.Tabs
{
    public partial class PromotionEditTab : BaseUserControl, IAdminTabControl, IAdminContextControl
    {
		private const string _PromotionDtoString = "PromotionDto";
		private const string _ConfigString = "Config";

        PromotionConfig[] _PromotionConfigs = null;
        PromotionDto _promotion = null;

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            //CampaignFilter.DataRequested += new ComboBox.DataRequestedEventHandler(CampaignFilter_DataRequested);
            if (!Page.IsPostBack && !CampaignFilter.CausedCallback)
            {
                LoadCampaignItems(0, CampaignFilter.DropDownPageSize * 2, "");
                BindForm();

                if(CampaignFilter.Items.Count == 0)
                    DisplayErrorMessage("You must create campaign first before creating a promotion");
            }

            if (!CampaignFilter.CausedCallback)
            {
                if (String.IsNullOrEmpty(this.Request.Form["__EVENTTARGET"]) || !this.Request.Form["__EVENTTARGET"].Equals(PromotionTypeList.UniqueID, StringComparison.OrdinalIgnoreCase))
                    BindPromotionConfig();
            }
        }

		protected void CampaignRequiredValidator_ServerValidate(object source, ServerValidateEventArgs args)
		{
			args.IsValid = !String.IsNullOrEmpty(CampaignFilter.SelectedValue);
		}

        /// <summary>
        /// Binds the languages repeater.
        /// </summary>
        /// <returns></returns>
        private void BindPromotionLanguagesList()
        {
            DataTable sourceTable = new DataTable();
            sourceTable.Columns.AddRange(new DataColumn[] { 
				new DataColumn("LanguageCode", typeof(string)),
				new DataColumn("FriendlyName", typeof(string)),
				new DataColumn("DisplayName", typeof(string))
					});

            DataTable dtLanguages = Language.GetAllLanguagesDT();

            if (_promotion != null && _promotion.PromotionLanguage.Count > 0)
            {
                //first check permissions
                //if permissions not present, deny
                SecurityManager.CheckRolePermission("marketing:promotions:mng:edit");

                foreach (DataRow row in dtLanguages.Rows)
                {
                    string langCode = row["LangName"].ToString();

                    PromotionDto.PromotionLanguageRow promoLanguageRow = null;

                    // check if record for the current language already exists in TaxLanguage table
                    PromotionDto.PromotionLanguageRow[] promoLanguageRows = (PromotionDto.PromotionLanguageRow[])_promotion.PromotionLanguage.Select(String.Format("LanguageCode='{0}'", langCode));
                    if (promoLanguageRows != null && promoLanguageRows.Length > 0)
                    {
                        promoLanguageRow = promoLanguageRows[0];
                    }
                    else
                    {
                        promoLanguageRow = _promotion.PromotionLanguage.NewPromotionLanguageRow();
                        promoLanguageRow.LanguageCode = langCode;
                        promoLanguageRow.PromotionId = _promotion.Promotion[0].PromotionId;
                        promoLanguageRow.DisplayName = String.Empty;
                    }

                    // add taxLanguage to the source table
                    DataRow sourceTableRow = sourceTable.NewRow();
                    sourceTableRow["LanguageCode"] = promoLanguageRow.LanguageCode;
                    sourceTableRow["FriendlyName"] = (string)row["FriendlyName"];
                    sourceTableRow["DisplayName"] = promoLanguageRow.DisplayName;
                    sourceTable.Rows.Add(sourceTableRow);
                }
            }
            else
            {
                //first check permissions
                //if permissions not present, deny
                SecurityManager.CheckRolePermission("marketing:promotions:mng:create");

                // this is a new tax, bind empty table
                foreach (DataRow row in dtLanguages.Rows)
                {
                    string langCode = row["LangName"].ToString();

                    DataRow sourceTableRow = sourceTable.NewRow();
                    sourceTableRow["LanguageCode"] = langCode;
                    sourceTableRow["FriendlyName"] = (string)row["FriendlyName"];
                    sourceTableRow["DisplayName"] = String.Empty;
                    sourceTable.Rows.Add(sourceTableRow);
                }
            }

            LanguagesList.DataSource = sourceTable;
            LanguagesList.DataBind();
        }

        /// <summary>
        /// Loads the promotion configs.
        /// </summary>
        /// <returns></returns>
        private PromotionConfig[] LoadPromotionConfigs()
        {
            List<PromotionConfig> configs = new List<PromotionConfig>();
            DirectoryInfo directory = new DirectoryInfo(Server.MapPath("~/Apps/Marketing/Promotions/Configs"));
            FileInfo[] files = directory.GetFiles();
            
            if(files.Length == 0)
                return null;

            foreach (FileInfo file in files)
            {
                XmlDocument doc = new XmlDocument();
                FileStream stream = file.OpenRead();
                doc.Load(stream);
                stream.Close();
                XmlElement configNode = doc.DocumentElement;

                PromotionConfig config = new PromotionConfig();

                config.Name = configNode.SelectSingleNode("Name").InnerText;
                config.Description = configNode.SelectSingleNode("Description").InnerText;
                config.Group = configNode.SelectSingleNode("Group").InnerText;
                config.Path = configNode.SelectSingleNode("Path").InnerText;
                config.Type = configNode.SelectSingleNode("Type").InnerText;
                config.Expression = configNode.SelectSingleNode("Expression").InnerXml;

                if (configNode.HasAttribute("sortorder"))
                    config.SortOrder = Int32.Parse(configNode.Attributes["sortorder"].Value);

                configs.Add(config);
            }

            configs.Sort(delegate(PromotionConfig c1, PromotionConfig c2)
            {
                return c1.SortOrder.CompareTo(c2.SortOrder);
            });

            return configs.ToArray();
        }

        /// <summary>
        /// Handles the DataRequested event of the CampaignFilter control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="ComponentArt.Web.UI.ComboBoxDataRequestedEventArgs"/> instance containing the event data.</param>
        void CampaignFilter_DataRequested(object sender, ComboBoxDataRequestedEventArgs args)
        {
            LoadCampaignItems(args.StartIndex, args.NumItems, args.Filter);
        }

        /// <summary>
        /// Loads the campaign items.
        /// </summary>
        /// <param name="iStartIndex">Start index of the i.</param>
        /// <param name="iNumItems">The i num items.</param>
        /// <param name="sFilter">The s filter.</param>
        private void LoadCampaignItems(int iStartIndex, int iNumItems, string sFilter)
        {
            int total = 0;

            CampaignDto dto = CampaignManager.GetCampaignDto();

            total = dto.Campaign.Count;
            
            CampaignFilter.Items.Clear();

            foreach (CampaignDto.CampaignRow row in dto.Campaign.Rows)
            {
                ComboBoxItem item = new ComboBoxItem(row.Name);
                item.Value = row.CampaignId.ToString();
                CampaignFilter.Items.Add(item);
            }

            CampaignFilter.ItemCount = total;
        }

        /// <summary>
        /// Loads the policy groups.
        /// </summary>
		private void LoadPolicyGroups()
		{
            /*
			PromotionGroupsList.Items.Clear();
			foreach (string key in PromotionGroup.Groups.Keys)
				PromotionGroupsList.Items.Add(new ListItem(PromotionGroup.Groups[key], key));
             * */
		}

        /// <summary>
        /// Binds the form.
        /// </summary>
        private void BindForm()
        {
			LoadPolicyGroups();
            LoadPromotionTypes();

            if (_promotion != null)
            {
                PromotionDto.PromotionRow promo = _promotion.Promotion[0];
                PromotionName.Text = promo.Name;
				this.AvailableFrom.Value = ManagementHelper.GetUserDateTime(promo.StartDate);
				this.ExpiresOn.Value = ManagementHelper.GetUserDateTime(promo.EndDate);
                Priority.Text = promo.Priority.ToString();
                CouponCode.Text = promo.CouponCode;
                MaxTotalRedemptions.Text = promo.ApplicationLimit.ToString();
                MaxCustomerRedemptions.Text = promo.CustomerLimit.ToString();
                MaxOrderRedemptions.Text = promo.PerOrderLimit.ToString();

                // Get existing statistics
                DataTable usageStatistics = PromotionManager.GetPromotionUsageStatistics();

                // Get total used
                int totalUsed = 0;

                System.Data.DataRow[] rows = usageStatistics.Select(String.Format("PromotionId = {0}", promo.PromotionId));
                if (rows != null && rows.Length > 0)
                    totalUsed = (int)rows[0]["TotalUsed"];

                MaxTotalRedemptionsUsed.Text = totalUsed.ToString();

                //OfferAmount.Text = promo.OfferAmount.ToString();
                ComboBoxItem item = CampaignFilter.Items.FindByValue(promo.CampaignId.ToString());
                if (item != null)
                    CampaignFilter.SelectedItem = item;

                //CampaignFilter.SelectedIndex = 0;
                //ManagementHelper.SelectListItem(OfferType, promo.OfferType);
                //ManagementHelper.SelectListItem(PromotionGroupsList, promo.PromotionGroup);
                ManagementHelper.SelectListItem(ExclusivityType, promo.ExclusivityType);
                ManagementHelper.SelectListItem(PromotionStatus, promo.Status);
                ManagementHelper.SelectListItem(PromotionTypeList, promo.PromotionType);
                PromotionTypeList.Enabled = false;
            }
            else
            {
                ManagementHelper.SelectListItem(PromotionTypeList, Request.QueryString["type"]);
                PromotionTypeList.Enabled = true;
				this.AvailableFrom.Value = ManagementHelper.GetUserDateTimeNow();
				this.ExpiresOn.Value = ManagementHelper.GetUserDateTimeNow().AddMonths(1);
                Priority.Text = "1";
                //OfferAmount.Text = "0";
            }

            BindPromotionLanguagesList();
        }

        /// <summary>
        /// Loads the promotion types.
        /// </summary>
        private void LoadPromotionTypes()
        {
            PromotionTypeList.Items.Clear();
            if (_PromotionConfigs == null)
                _PromotionConfigs = LoadPromotionConfigs();

            foreach (PromotionConfig config in _PromotionConfigs)
            {
                PromotionTypeList.Items.Add(new ListItem(GetPromotionTypeName(config), config.Type));
            }

            PromotionTypeList.DataBind();
        }

        private string GetPromotionTypeName(PromotionConfig config)
        {
            return String.Format("{0}: {1}", Mediachase.Commerce.Marketing.PromotionGroup.Groups[config.Group], config.Name);
        }

        /// <summary>
        /// Binds the promotion config.
        /// </summary>
        private void BindPromotionConfig()
        { 
            // Find selected
            PromotionConfig selectedConfig = new PromotionConfig();
            if (_PromotionConfigs == null)
                _PromotionConfigs = LoadPromotionConfigs();

            foreach (PromotionConfig config in _PromotionConfigs)
            {
                if(config.Type == PromotionTypeList.SelectedValue)
                {
                    selectedConfig = config;
                    break;
                }
            }

            // Bind promotion group
            PromotionGroup.Text = selectedConfig.Group;

            string id = selectedConfig.Type;
            Control ctrl = PromotionConfigPlaceholder.FindControl(id);

            if (ctrl == null)
            {
                PromotionConfigPlaceholder.Controls.Clear();

                ctrl = LoadControl(String.Format("~/Apps/Marketing/Promotions/{0}", selectedConfig.Path));
                ctrl.ID = id;
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add(_PromotionDtoString, _promotion);
                dic.Add(_ConfigString, selectedConfig);

                if (ctrl is IAdminContextControl)
                    ((IAdminContextControl)ctrl).LoadContext(dic);

                ((PlaceHolder)PromotionConfigPlaceholder).Controls.Add(ctrl);
            }
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the PromotionTypeList control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void PromotionTypeList_SelectedIndexChanged(object sender, EventArgs e)
        {
            Response.Redirect(String.Format("ContentFrame.aspx?_a=Marketing&_v=Promotion-Edit&type={0}", PromotionTypeList.SelectedValue));
            //BindPromotionConfig();
        }
		
        #region IAdminTabControl Members
        /// <summary>
        /// Saves the changes.
        /// </summary>
        /// <param name="context">The context.</param>
        public void SaveChanges(IDictionary context)
        {
			PromotionDto dto = (PromotionDto)context[_PromotionDtoString];
            PromotionDto.PromotionRow row = null;

            if (dto.Promotion.Count > 0)
            {
                row = dto.Promotion[0];
                row.Modified = DateTime.UtcNow;
                row.ModifiedBy = Page.User.Identity.Name;
            }
            else
            {
                row = dto.Promotion.NewPromotionRow();
                row.ApplicationId = MarketingConfiguration.Instance.ApplicationId;
                row.Created = DateTime.UtcNow;
                row.ModifiedBy = Page.User.Identity.Name;
            }

            row.Name = PromotionName.Text;
            row.StartDate = this.AvailableFrom.Value.ToUniversalTime();
            row.EndDate = this.ExpiresOn.Value.ToUniversalTime();
            row.Priority = Int32.Parse(Priority.Text);
            row.CouponCode = CouponCode.Text;
            row.Status = PromotionStatus.SelectedValue;
            row.ExclusivityType = ExclusivityType.SelectedValue;
            row.CampaignId = Int32.Parse(CampaignFilter.SelectedValue);
            row.ApplicationLimit = Int32.Parse(MaxTotalRedemptions.Text);
            row.CustomerLimit = Int32.Parse(MaxCustomerRedemptions.Text);
            row.PerOrderLimit = Int32.Parse(MaxOrderRedemptions.Text);

            if (row.RowState == DataRowState.Detached)
            {
                row.PromotionGroup = Mediachase.Commerce.Marketing.PromotionGroup.GetPromotionGroupValue(Mediachase.Commerce.Marketing.PromotionGroup.PromotionGroupKey.Entry);
                row.OfferType = 0;
                row.OfferAmount = 0;
                dto.Promotion.Rows.Add(row);
            }

            // save localized values
            foreach (RepeaterItem item in LanguagesList.Items)
            {
                HiddenField hfCtrl = item.FindControl("hfLangCode") as HiddenField;
                TextBox tbCtrl = item.FindControl("tbDisplayName") as TextBox;

                if (hfCtrl != null && tbCtrl != null)
                {
                    PromotionDto.PromotionLanguageRow[] promoLanguageRows = (PromotionDto.PromotionLanguageRow[])dto.PromotionLanguage.Select(String.Format("LanguageCode='{0}'", hfCtrl.Value));
                    PromotionDto.PromotionLanguageRow promoLanguageRow = null;

                    if (promoLanguageRows != null && promoLanguageRows.Length > 0)
                        promoLanguageRow = promoLanguageRows[0];
                    else
                    {
                        // add a new record for the current language
                        promoLanguageRow = dto.PromotionLanguage.NewPromotionLanguageRow();
                        promoLanguageRow.PromotionId = row.PromotionId;
                        promoLanguageRow.LanguageCode = hfCtrl.Value;
                    }

                    promoLanguageRow.DisplayName = tbCtrl.Text;

                    if (promoLanguageRow.RowState == DataRowState.Detached)
                        dto.PromotionLanguage.Rows.Add(promoLanguageRow);
                }
            }

            Control ctrl = PromotionConfigPlaceholder.Controls[0];

            if (ctrl is IAdminTabControl)
            {
                // Find selected
                PromotionConfig selectedConfig = new PromotionConfig();
                if (_PromotionConfigs == null)
                    _PromotionConfigs = LoadPromotionConfigs();

                foreach (PromotionConfig config in _PromotionConfigs)
                {
                    if (config.Type == PromotionTypeList.SelectedValue)
                    {
                        selectedConfig = config;
                        break;
                    }
                }

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add(_PromotionDtoString, dto);
				dic.Add(_ConfigString, selectedConfig);
                ((IAdminTabControl)ctrl).SaveChanges(dic);
            }
        }
        #endregion

        #region IAdminContextControl Members

        /// <summary>
        /// Loads the context.
        /// </summary>
        /// <param name="context">The context.</param>
        public void LoadContext(IDictionary context)
        {
			_promotion = (PromotionDto)context[_PromotionDtoString];
        }
        #endregion
    }
}