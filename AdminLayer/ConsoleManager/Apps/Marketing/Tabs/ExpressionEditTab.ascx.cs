using System;
using System.Collections;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mediachase.Commerce.Marketing;
using Mediachase.Commerce.Marketing.Dto;
using Mediachase.Commerce.Shared;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Common;
using Mediachase.Web.Console.Interfaces;

namespace Mediachase.Commerce.Manager.Marketing.Tabs
{
	public partial class ExpressionEditTab : BaseUserControl, IAdminTabControl, IAdminContextControl
    {
		private const string _ExpressionDtoString = "ExpressionDto";

		private ExpressionDto _ExpressionDto = null;

        /// <summary>
        /// Gets the current expression category.
        /// </summary>
        /// <value>The current expression category.</value>
		public string CurrentExpressionCategory
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
        /// Loads the expression categories.
        /// </summary>
        private void LoadExpressionCategories()
        {
			foreach(string key in ExpressionCategory.Categories.Keys)
				ddlExpressionCategory.Items.Add(new ListItem(ExpressionCategory.Categories[key], key));
			ManagementHelper.SelectListItem(ddlExpressionCategory, CurrentExpressionCategory);
        }

        /// <summary>
        /// Binds the form.
        /// </summary>
        private void BindForm()
        {
			if (_ExpressionDto != null)
			{
                //first check permissions
                //if permissions not present, deny
                SecurityManager.CheckRolePermission("marketing:expr:mng:edit");
                
                ExpressionDto.ExpressionRow row = _ExpressionDto.Expression[0];
				tbExpressionName.Text = row.Name;
				tbDescription.Text = row.Description;
				tbXml.Text = row.ExpressionXml;

				ExpressionCategoryRow.Visible = false;
			}
			else
			{
                //first check permissions
                //if permissions not present, deny
                SecurityManager.CheckRolePermission("marketing:expr:mng:create");
                
                ExpressionCategoryRow.Visible = true;
				LoadExpressionCategories();
			}
        }

        #region IAdminTabControl Members
        /// <summary>
        /// Saves the changes.
        /// </summary>
        /// <param name="context">The context.</param>
        public void SaveChanges(IDictionary context)
        {
			ExpressionDto dto = (ExpressionDto)context[_ExpressionDtoString];
			ExpressionDto.ExpressionRow row = null;

			if (dto.Expression.Count > 0)
			{
				row = dto.Expression[0];
                row.Modified = DateTime.UtcNow;
			}
			else
			{
				row = dto.Expression.NewExpressionRow();
                row.ApplicationId = MarketingConfiguration.Instance.ApplicationId;
                row.Category = ddlExpressionCategory.SelectedValue;
                row.Created = DateTime.UtcNow;
			}

			row.Name = tbExpressionName.Text;
			row.Description = tbDescription.Text;
			row.ExpressionXml = tbXml.Text;
			row.ModifiedBy = Page.User.Identity.Name;

			if (row.RowState == DataRowState.Detached)
				dto.Expression.Rows.Add(row);
        }
        #endregion
		
        #region IAdminContextControl Members

        /// <summary>
        /// Loads the context.
        /// </summary>
        /// <param name="context">The context.</param>
        public void LoadContext(IDictionary context)
        {
            _ExpressionDto = (ExpressionDto)context[_ExpressionDtoString];
        }
        #endregion
    }
}