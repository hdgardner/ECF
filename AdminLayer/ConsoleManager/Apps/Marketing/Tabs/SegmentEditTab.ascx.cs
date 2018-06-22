using System;
using System.Collections;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using ComponentArt.Web.UI;
using Mediachase.Commerce.Marketing;
using Mediachase.Commerce.Marketing.Dto;
using Mediachase.Commerce.Marketing.Managers;
using Mediachase.Commerce.Profile;
using Mediachase.Commerce.Profile.Search;
using Mediachase.Commerce.Shared;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Interfaces;
using Mediachase.Ibn.Web.UI.WebControls;
using System.Collections.Generic;

namespace Mediachase.Commerce.Manager.Marketing.Tabs
{
    public partial class SegmentEditTab : BaseUserControl, IAdminTabControl, IAdminContextControl
    {
		private const string _SegmentDtoString = "SegmentDto";
        private const string _ExpressionDtoString = "ExpressionDto";
		private const string _CommandNameString = "CommandName";
		private const string _RuleEditCommandString = "cmdRuleEdit";

        SegmentDto _segment = null;
        ExpressionDto _expression = null;
        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
           //first check permissions
            if (_segment != null && _segment.Segment.Count > 0)
            {
                //if permissions not present, deny
                SecurityManager.CheckRolePermission("marketing:segments:mng:edit");
            }
            else
            {
                //first check permissions
                //if permissions not present, deny
                SecurityManager.CheckRolePermission("marketing:segments:mng:create");
            }

            MembershipFilter.DataRequested += new ComboBox.DataRequestedEventHandler(MembershipFilter_DataRequested);
            AddMember.Click += new EventHandler(AddMember_Click);
            //AddExpression.Click += new EventHandler(AddExpression_Click);
            if (!Page.IsPostBack && !MembershipFilter.CausedCallback /*&& !ExpressionFilter.CausedCallback*/)
            {
                LoadMemberItems(0, MembershipFilter.DropDownPageSize * 2, "");
                //LoadExpressions(0, ExpressionFilter.DropDownPageSize * 2, "");
                BindForm();
            }

			if (IsPostBack && String.Compare(Request.Form["__EVENTTARGET"], CommandManager.GetCurrent(this.Page).ID, false) == 0)
			{
				object objArgs = Request.Form["__EVENTARGUMENT"];
				if (objArgs != null)
				{
					Dictionary<string, object> cmd = new System.Web.Script.Serialization.JavaScriptSerializer().DeserializeObject(objArgs.ToString()) as Dictionary<string, object>;
					if (cmd != null && cmd.Count > 1)
					{
						object cmdName = cmd[_CommandNameString];
						if (String.Compare((string)cmdName, _RuleEditCommandString, true) == 0)
						{
							// update expression list
							if (_segment.Segment.Count > 0)
							{
								int exprId = 0;
								exprId = FindDesignExpressionId(_segment);
								BindExpressionEditor(String.Format("customer;{0}", exprId));
							}
							else
							{
								BindExpressionEditor(String.Format("customer;0"));
							}
						}
					}
				}

			}
        }

        /*
        /// <summary>
        /// Handles the Click event of the AddExpression control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void AddExpression_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(ExpressionFilter.SelectedValue))
                return;

            foreach (SegmentDto.SegmentConditionRow cond in _segment.SegmentCondition)
            {
                if (cond.PrincipalGroupConditionId == Int32.Parse(ExpressionFilter.SelectedValue))
                {
                    BindForm();
                    return;
                }
            }

            SegmentDto.SegmentConditionRow row = _segment.SegmentCondition.NewSegmentConditionRow();
            row.ExpressionId = Int32.Parse(ExpressionFilter.SelectedValue);
            row.SegmentId = _segment.Segment[0].SegmentId;

            _segment.SegmentCondition.Rows.Add(row);
            BindForm();
        }
         * */

        /*
        /// <summary>
        /// Loads the expressions.
        /// </summary>
        /// <param name="iStartIndex">Start index of the i.</param>
        /// <param name="iNumItems">The i num items.</param>
        /// <param name="sFilter">The s filter.</param>
        private void LoadExpressions(int iStartIndex, int iNumItems, string sFilter)
        {
            ExpressionDto dto = ExpressionManager.GetExpressionDto(ExpressionCategory.GetExpressionCategory(ExpressionCategory.CategoryKey.Segment).Key);

            ExpressionFilter.DataSource = dto;
            ExpressionFilter.DataBind();
        }
         * */

        /// <summary>
        /// Handles the Command event of the DeleteButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.CommandEventArgs"/> instance containing the event data.</param>
        protected void DeleteButton_Command(object sender, CommandEventArgs e)
        {
            foreach (SegmentDto.SegmentMemberRow member in _segment.SegmentMember)
            {
                if (member.RowState != DataRowState.Deleted && 
					member.SegmentMemberId == Int32.Parse(e.CommandArgument.ToString()))
                {
                    member.Delete();
                    BindForm();
                    return;
                }
            }

            BindForm();
        }

        /*
        /// <summary>
        /// Handles the Command event of the ExpressionDeleteButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.CommandEventArgs"/> instance containing the event data.</param>
        protected void ExpressionDeleteButton_Command(object sender, CommandEventArgs e)
        {
            foreach (SegmentDto.SegmentConditionRow row in _segment.SegmentCondition)
            {
                if (row.PrincipalGroupConditionId == Int32.Parse(e.CommandArgument.ToString()))
                {
                    row.Delete();
                    BindForm();
                    return;
                }
            }

            BindForm();
        }
         * */

        /// <summary>
        /// Handles the Click event of the AddMember control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void AddMember_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(MembershipFilter.SelectedValue))
                return;

            Guid principalId = new Guid(MembershipFilter.SelectedValue);

            foreach (SegmentDto.SegmentMemberRow member in _segment.SegmentMember)
            {
                if (member.RowState != DataRowState.Deleted && member.PrincipalId == principalId)
                {
                    BindForm();
                    return;
                }
            }

            SegmentDto.SegmentMemberRow row = _segment.SegmentMember.NewSegmentMemberRow();
            row.PrincipalId = principalId;
            row.SegmentId = _segment.Segment[0].SegmentId;
            row.Exclude = Exclude.Checked;

			_segment.SegmentMember.Rows.Add(row);
            BindForm();
        }

        /// <summary>
        /// Handles the DataRequested event of the MembershipFilter control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="ComponentArt.Web.UI.ComboBoxDataRequestedEventArgs"/> instance containing the event data.</param>
        void MembershipFilter_DataRequested(object sender, ComboBoxDataRequestedEventArgs args)
        {
            LoadMemberItems(args.StartIndex, args.NumItems, args.Filter);
        }

        /// <summary>
        /// Loads the member items.
        /// </summary>
        /// <param name="iStartIndex">Start index of the i.</param>
        /// <param name="iNumItems">The i num items.</param>
        /// <param name="sFilter">The s filter.</param>
        private void LoadMemberItems(int iStartIndex, int iNumItems, string sFilter)
        {
            int total = 0;
            ProfileSearchParameters pars = new ProfileSearchParameters();
            ProfileSearchOptions options = new ProfileSearchOptions();
            options.Namespace = "Mediachase.Commerce.Profile";
            options.Classes.Add("Account");

            int totalAccounts = 0;
            Account[] accounts = ProfileContext.Current.FindAccounts(pars, options, out totalAccounts);
            total += totalAccounts;

            options.Classes.Clear();
            options.Classes.Add("Organization");
            Organization[] orgs = ProfileContext.Current.FindOrganizations(pars, options, out totalAccounts);
            total += totalAccounts;

            MembershipFilter.Items.Clear();

            foreach (Account account in accounts)
            {
                ComboBoxItem item = new ComboBoxItem(account.Name);
                item.Value = account.PrincipalId.ToString();
                MembershipFilter.Items.Add(item);
            }

            foreach (Organization org in orgs)
            {
                ComboBoxItem item = new ComboBoxItem(org.Name);
                item.Value = org.PrincipalId.ToString();
                MembershipFilter.Items.Add(item);
            }

            MembershipFilter.ItemCount = total;
        }

        /// <summary>
        /// Gets the status.
        /// </summary>
        /// <param name="excluded">if set to <c>true</c> [excluded].</param>
        /// <returns></returns>
        protected string GetStatus(bool excluded)
        {
            return excluded ? "(excluded)" : "";
        }

        /// <summary>
        /// Gets the name of the principal.
        /// </summary>
        /// <param name="principalId">The principal id.</param>
        /// <returns></returns>
        protected string GetPrincipalName(Guid principalId)
        {
            Account acc = ProfileContext.Current.GetAccount(principalId);
            if (acc != null)
                return acc.Name;

            Organization org = ProfileContext.Current.GetOrganization(principalId);
            if (org != null)
                return org.Name;

            return principalId.ToString();
        }

        /// <summary>
        /// Binds the form.
        /// </summary>
        private void BindForm()
        {
            if (_segment != null)
            {
                if (_segment.Segment.Count > 0)
                {
                    SegmentDto.SegmentRow row = _segment.Segment[0];
					SegmentName.Text = string.IsNullOrEmpty(SegmentName.Text) ? row.Name : SegmentName.Text;
					DisplayName.Text =  string.IsNullOrEmpty(DisplayName.Text) ? row.DisplayName : DisplayName.Text;
					Description.Text = string.IsNullOrEmpty(Description.Text) ? row.Description : Description.Text;

                    int exprId = 0;
                    exprId = FindDesignExpressionId(_segment);
                    BindExpressionEditor(String.Format("customer;{0}", exprId));
                }
                else
                {
                    BindExpressionEditor(String.Format("customer;0"));
                }

                MemberList.DataSource = _segment.SegmentMember;
                MemberList.DataBind();

                /*
                ExpressionList.DataSource = _segment.SegmentCondition;
                ExpressionList.DataBind();
                 * */
            }
            else
            {
                BindExpressionEditor(String.Format("customer;0"));
            }
        }

        /// <summary>
        /// Binds the expression editor.
        /// </summary>
        private void BindExpressionEditor(string dataSourceString)
        {
            RulesEditorCtrl.DataSource = _expression;
            RulesEditorCtrl.DataBind();
            //ExprFilter.ProviderName = "CustomerDataProvider";
            //ExprFilter.DataSource = dataSourceString; // not actually used anywhere
            //ExprFilter.DataBind();
        }

        /// <summary>
        /// Gets the name of the expression.
        /// </summary>
        /// <param name="expressionId">The expression id.</param>
        /// <returns></returns>
        protected string GetExpressionName(int expressionId)
        {
            ExpressionDto expr = ExpressionManager.GetExpressionDto(expressionId);
            return expr.Expression[0].Name;
        }

        /// <summary>
        /// Finds the design expression id.
        /// </summary>
        /// <param name="segmentDto">The segment dto.</param>
        /// <returns></returns>
        private int FindDesignExpressionId(SegmentDto segmentDto)
        {
            if (segmentDto.SegmentCondition.Count > 0)
            {
                DataView view = segmentDto.SegmentCondition.DefaultView;
                //view.RowFilter = String.Format("name like '{0}*'", _RulesPrefix);
                if (view.Count > 0)
                {
                    return Int32.Parse(view[0]["ExpressionId"].ToString());
                }
            }

            return 0;           
        }

        #region IAdminTabControl Members
        /// <summary>
        /// Saves the changes.
        /// </summary>
        /// <param name="context">The context.</param>
        public void SaveChanges(IDictionary context)
        {
            int expressionId = 0;
            SegmentDto dto = (SegmentDto)context[_SegmentDtoString];
            SegmentDto.SegmentRow row = null;

            if (dto.Segment.Count > 0)
            {
                row = dto.Segment[0];

                expressionId = FindDesignExpressionId(dto);
            }
            else
            {
                row = dto.Segment.NewSegmentRow();
                row.ApplicationId = MarketingConfiguration.Instance.ApplicationId;
            }

            row.Name = SegmentName.Text;
            row.DisplayName = DisplayName.Text;
            row.Description = Description.Text;

			//Tell RulesEditorCtrl save changes
			RulesEditorCtrl.SaveChanges();

            if (row.RowState == DataRowState.Detached)
                dto.Segment.Rows.Add(row);

			/*
            // design-ui denotes that this expression was created using design ui
            if(ExprFilter.Visible)
                ExprFilter.Provider.SaveFilters(_RulesPrefix, String.Format("{0}:{1}", row.SegmentId, expressionId), ExprFilter.NodeCollection[0].ChildNodes);
             * */
        }
        #endregion

        #region IAdminContextControl Members

        /// <summary>
        /// Loads the context.
        /// </summary>
        /// <param name="context">The context.</param>
        public void LoadContext(IDictionary context)
        {
            _segment = (SegmentDto)context[_SegmentDtoString];
            _expression = (ExpressionDto)context[_ExpressionDtoString];

			RulesEditorCtrl.LoadContext(context);
        }
        #endregion
    }
}