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
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Marketing.Dto;
using Mediachase.Commerce.Marketing.Managers;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Common;
using Mediachase.Commerce.Manager.Core;

namespace Mediachase.Commerce.Manager.Marketing
{
    public partial class PolicyEdit : MarketingBaseUserControl
    {
		private const string _PolicyDtoEditSessionKey = "ECF.PolicyDto.Edit";
		private const string _PolicyIdString = "PolicyId";
		private const string _PolicyDtoString = "PolicyDto";

		private PolicyDto _Policy = null;

        /// <summary>
        /// Gets the policy id.
        /// </summary>
        /// <value>The policy id.</value>
        public int PolicyId
        {
            get
            {
                return ManagementHelper.GetIntFromQueryString(_PolicyIdString);
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

			// need to pass "group"=Policy Group in QueryString
			// if Policy is new, this parameter will be taken from the current QS; otherwise, get it from the Policy being edited
			if (PolicyId > 0 && _Policy != null && _Policy.GroupPolicy != null && _Policy.GroupPolicy.Count > 0)
			{
				EditSaveControl.SavedClientScript = String.Format("CSManagementClient.ChangeView('Marketing','Policy-List', 'group={0}');", _Policy.GroupPolicy[0].GroupName);
				EditSaveControl.CancelClientScript = String.Format("CSManagementClient.ChangeView('Marketing','Policy-List', 'group={0}');", _Policy.GroupPolicy[0].GroupName);
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
        /// Loads the fresh.
        /// </summary>
        /// <returns></returns>
        private PolicyDto LoadFresh()
        {
			PolicyDto policy = PolicyManager.GetPolicyDto(PolicyId);

            // persist in session
            Session[_PolicyDtoEditSessionKey] = policy;

            return policy;
        }

        /// <summary>
        /// Loads the context.
        /// </summary>
        private void LoadContext()
        {
            if (PolicyId > 0)
            {
				PolicyDto policy = null;
                if (!this.IsPostBack && (!this.Request.QueryString.ToString().Contains("Callback=yes"))) // load fresh on initial load
                {
                    policy = LoadFresh();
                }
                else // load from session
                {
					policy = (PolicyDto)Session[_PolicyDtoEditSessionKey];

                    if (policy == null)
                    {
                        policy = LoadFresh();
                    }
                }

                // Put a dictionary key that can be used by other tabs
                IDictionary dic = new ListDictionary();
				dic.Add(_PolicyDtoString, policy);

                // Call tabs load context
                ViewControl.LoadContext(dic);

				_Policy = policy;
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

			PolicyDto policy = (PolicyDto)Session[_PolicyDtoEditSessionKey];

			if (PolicyId > 0 && policy == null)
				policy = PolicyManager.GetPolicyDto(PolicyId);
			else if (PolicyId == 0)
				policy = new PolicyDto();

            IDictionary context = new ListDictionary();
			context.Add(_PolicyDtoString, policy);

            ViewControl.SaveChanges(context);

            if (policy.HasChanges())
                PolicyManager.SavePolicy(policy);

			// we don't need to store Dto in session any more
			Session.Remove(_PolicyDtoEditSessionKey);
        }
    }
}