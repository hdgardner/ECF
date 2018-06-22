using System;
using System.Collections;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mediachase.Commerce.Marketing;
using Mediachase.Commerce.Marketing.Dto;
using Mediachase.Commerce.Marketing.Managers;
using Mediachase.Commerce.Shared;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Common;
using Mediachase.Web.Console.Interfaces;

namespace Mediachase.Commerce.Manager.Marketing.Tabs
{
    public partial class PolicyEditTab : BaseUserControl, IAdminTabControl, IAdminContextControl
    {
		private const string _PolicyDtoString = "PolicyDto";

        PolicyDto _Policy = null;

        /// <summary>
        /// Gets the current policy group.
        /// </summary>
        /// <value>The current policy group.</value>
		public string CurrentPolicyGroup
		{
			get
			{
				return ManagementHelper.GetValueFromQueryString("group", null);
			}
		}

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
				BindForm();
        }

        /// <summary>
        /// Loads the expressions.
        /// </summary>
        private void LoadExpressions()
        {
            ExpressionDto dto = ExpressionManager.GetExpressionDto(ExpressionCategory.GetExpressionCategory(ExpressionCategory.CategoryKey.Policy).Key);

            ddlExpression.DataSource = dto;
            ddlExpression.DataBind();
        }

        /// <summary>
        /// Loads the policy groups.
        /// </summary>
		private void LoadPolicyGroups()
		{
			GroupsListBox.Items.Clear();
			foreach (string key in PromotionGroup.Groups.Keys)
				GroupsListBox.Items.Add(new ListItem(PromotionGroup.Groups[key], key));
		}

        /// <summary>
        /// Binds the form.
        /// </summary>
        private void BindForm()
        {
			LoadExpressions();

			if (_Policy != null)
            {
                //first check permissions
                //if permissions not present, deny
                SecurityManager.CheckRolePermission("marketing:policies:mng:edit");

                PolicyDto.PolicyRow row = _Policy.Policy[0];
                PolicyName.Text = row.Name;
				tbStatus.Text = row.Status;
				IsLocal.IsSelected = row.IsLocal;
				ManagementHelper.SelectListItem(ddlExpression, row.ExpressionId);

				/*if (row.IsLocal)
					PolicyGroupRow.Visible = false;
				else
				{*/
					LoadPolicyGroups();
					foreach (PolicyDto.GroupPolicyRow gpRow in row.GetGroupPolicyRows())
						ManagementHelper.SelectListItem(GroupsListBox, gpRow.GroupName, false);
				//}
            }
            else
            {
                //first check permissions
                //if permissions not present, deny
                SecurityManager.CheckRolePermission("marketing:policies:mng:create");
                
                //PolicyGroupRow.Visible = true;
				LoadPolicyGroups();
				ManagementHelper.SelectListItem(GroupsListBox, CurrentPolicyGroup, true);
            }
        }

        #region IAdminTabControl Members
        /// <summary>
        /// Saves the changes.
        /// </summary>
        /// <param name="context">The context.</param>
        public void SaveChanges(IDictionary context)
        {
			PolicyDto dto = (PolicyDto)context[_PolicyDtoString];
            PolicyDto.PolicyRow row = null;

            if (dto.Policy.Count > 0)
                row = dto.Policy[0];
            else
            {
                row = dto.Policy.NewPolicyRow();
                row.ApplicationId = MarketingConfiguration.Instance.ApplicationId;
            }

            row.Name = PolicyName.Text;
			row.Status = tbStatus.Text;
			row.IsLocal = IsLocal.IsSelected;
			if (ddlExpression.Items.Count > 0)
				row.ExpressionId = Int32.Parse(ddlExpression.SelectedValue);

            if (row.RowState == DataRowState.Detached)
                dto.Policy.Rows.Add(row);

			// save selected groups
			if (row.IsLocal)
			{
				// remove all policy groups if it's a local policy
				PolicyDto.GroupPolicyRow[] gpRowsCollection = (PolicyDto.GroupPolicyRow[])dto.GroupPolicy.Select();
				foreach (PolicyDto.GroupPolicyRow gpRow in gpRowsCollection)
					gpRow.Delete();
			}
			else
			{
				foreach (ListItem li in GroupsListBox.Items)
				{
					if (li.Selected)
					{
						// if current policy group is not associated with policy, add it
						PolicyDto.GroupPolicyRow gpRow = null;

						PolicyDto.GroupPolicyRow[] gpRows = (PolicyDto.GroupPolicyRow[])dto.GroupPolicy.Select(String.Format("GroupName='{0}'", li.Value));
						if (gpRows.Length > 0)
							gpRow = gpRows[0];
						else
							gpRow = dto.GroupPolicy.NewGroupPolicyRow();

						gpRow.GroupName = li.Value;
						gpRow.PolicyId = row.PolicyId;

						if (gpRow.RowState == DataRowState.Detached)
							dto.GroupPolicy.Rows.Add(gpRow);
					}
					else
					{
						// remove unselected groups
						PolicyDto.GroupPolicyRow[] gpRows = (PolicyDto.GroupPolicyRow[])dto.GroupPolicy.Select(String.Format("GroupName='{0}'", li.Value));
						if (gpRows.Length > 0)
							gpRows[0].Delete();
					}
				}
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
            _Policy = (PolicyDto)context[_PolicyDtoString];
        }
        #endregion
    }
}