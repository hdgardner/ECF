using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Marketing.Dto;
using Mediachase.Commerce.Marketing.Managers;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Common;
using Mediachase.Commerce.Manager.Core;

namespace Mediachase.Commerce.Manager.Marketing
{
    public partial class CampaignEdit : MarketingBaseUserControl
    {
		private const string _CampaignDtoEditSessionKey = "ECF.CampaignDto.Edit";
		private const string _CampaignIdString = "CampaignId";
		private const string _CampaignDtoString = "CampaignDto";

        /// <summary>
        /// Gets the campaign id.
        /// </summary>
        /// <value>The campaign id.</value>
        public int CampaignId
        {
            get
            {
                return ManagementHelper.GetIntFromQueryString(_CampaignIdString);
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
        /// Loads the fresh.
        /// </summary>
        /// <returns></returns>
        private CampaignDto LoadFresh()
        {
            CampaignDto campaign = CampaignManager.GetCampaignDto(CampaignId);

            // persist in session
            Session[_CampaignDtoEditSessionKey] = campaign;

            return campaign;
        }

        /// <summary>
        /// Loads the context.
        /// </summary>
        private void LoadContext()
        {
            if (CampaignId > 0)
            {
                CampaignDto campaign = null;
                if (!this.IsPostBack && (!this.Request.QueryString.ToString().Contains("Callback=yes"))) // load fresh on initial load
                {
                    campaign = LoadFresh();
                }
                else // load from session
                {
                    campaign = (CampaignDto)Session[_CampaignDtoEditSessionKey];

                    if (campaign == null)
                    {
                        campaign = LoadFresh();
                    }
                }

                // Put a dictionary key that can be used by other tabs
                IDictionary dic = new ListDictionary();
                dic.Add(_CampaignDtoString, campaign);

                // Call tabs load context
                ViewControl.LoadContext(dic);
            }
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

			CampaignDto campaign = (CampaignDto)Session[_CampaignDtoEditSessionKey]; //null;
			if (CampaignId > 0 && campaign == null)
				campaign = CampaignManager.GetCampaignDto(CampaignId); //Int32.Parse(Parameters[_CampaignIdString].ToString()));
			else if (CampaignId == 0)
				campaign = new CampaignDto();

            IDictionary context = new ListDictionary();
            context.Add(_CampaignDtoString, campaign);

            ViewControl.SaveChanges(context);

            if (campaign.HasChanges())
                CampaignManager.SaveCampaign(campaign);

			// we don't need to store Dto in session any more
			Session.Remove(_CampaignDtoEditSessionKey);
        }
    }
}