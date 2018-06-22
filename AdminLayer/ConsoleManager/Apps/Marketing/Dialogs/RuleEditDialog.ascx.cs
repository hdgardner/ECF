using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Interfaces;
using Mediachase.Web.Console.Common;
using Mediachase.Commerce.Marketing.Managers;
using Mediachase.Commerce.Marketing.Dto;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Commerce.Marketing;
using System.IO;

namespace Mediachase.Commerce.Manager.Apps.Marketing.Dialogs
{
    public partial class RuleEditDialog : BaseUserControl
    {
        private const string _RulesPrefix = "x-ecf-design-1.0";
		private const string _ExpressionDtoEditSessionKey = "ECF.ExpressionDto.Edit";
		private const string _RuleEditCommandString = "cmdRuleEdit";
		private const string _SegmentDtoEditSessionKey = "ECF.SegmentDto.Edit";
		private const string _FilterExpression = "FilterExpression";

        private ExpressionDto _ExpressionDto = null;
		private SegmentDto _SegmentDto;
		
        /// <summary>
        /// Gets the expression id.
        /// </summary>
        /// <value>The expression id.</value>
        public int ExpressionId
        {
            get
            {
                return ManagementHelper.GetIntFromQueryString("id");
            }
        }

		/// <summary>
		/// Gets a value indicating whether this instance is create expression.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is create expression; otherwise, <c>false</c>.
		/// </value>
		public bool IsCreateExpression
		{
			get
			{
				return ExpressionIndex == -1;
			}
		}

		/// <summary>
		/// Gets the index of the expression.
		/// </summary>
		/// <value>The index of the expression.</value>
		public int ExpressionIndex
		{
			get
			{
				int retVal = -1;
				object storedIndex = ViewState["ExpresionIndex"];
				if (storedIndex == null)
				{
					retVal = ManagementHelper.GetIntFromQueryString("index", -1);
				}
				else
				{
					retVal = Convert.ToInt32(storedIndex);
				}
				return retVal;
			}
			set
			{
				ViewState["ExpresionIndex"] = value;
			}
		}


		private List<FilterExpressionNodeCollection> ExpressionFilters
		{
			get
			{
				return HttpContext.Current.Session[_FilterExpression] as List<FilterExpressionNodeCollection>;
			}
		}

		private FilterExpressionNodeCollection ExpressionFilter
		{
			get
			{
				return ExpressionFilters.ElementAt(ExpressionIndex);
			}
			set
			{
				ExpressionFilters[ExpressionIndex] = value;
			}
		}


        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            _ExpressionDto = (ExpressionDto)HttpContext.Current.Session[_ExpressionDtoEditSessionKey];
			_SegmentDto = HttpContext.Current.Session[_SegmentDtoEditSessionKey] as SegmentDto;
            if (_ExpressionDto == null ||  _SegmentDto == null) // close the window
            {
                CommandParameters cp = new CommandParameters(_RuleEditCommandString);
                CommandHandler.RegisterCloseOpenedFrameScript(this.Page, cp.ToString());
                ExprFilter.Visible = false;
            }

		
            if (!this.IsPostBack)
            {
                if (_ExpressionDto != null)
                {
					//Request for create expresion
					if (IsCreateExpression)
					{
						//Add new ExpressionCollection to session
						ExpressionFilters.Add(new FilterExpressionNodeCollection());
						//Change iterator position to new collections
						ExpressionIndex = ExpressionFilters.Count - 1;

					}
					else
					{
						BindForm();
					}
                    BindExpressionEditor(ExpressionFilter);
                  
                }
            }
        }

        /// <summary>
        /// Handles the PreRender event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (Request["closeFramePopup"] != null)
            {
                CancelButton.OnClientClick = String.Format("javascript:try{{window.parent.{0}();}}catch(ex){{}}", Request["closeFramePopup"]);
            }
        }

        /// <summary>
        /// Handles the Click event of the SaveButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void SaveButton_Click(object sender, EventArgs e)
        {
            ExpressionDto dto = (ExpressionDto)HttpContext.Current.Session[_ExpressionDtoEditSessionKey];
            if (dto != null)
            {
				ExpressionFilter = ExprFilter.NodeCollection[0].ChildNodes;

                ExprFilter.Visible = true;
                ExprFilter.DataSource = dto;
				ExprFilter.Provider.SaveFilters(_RulesPrefix, ExpressionId.ToString(), ExpressionFilter);
                ExpressionDto.ExpressionRow row = null;
                if (ExpressionId == 0) // get the last row added
                    row = dto.Expression[dto.Expression.Count - 1];
                else
                    row = dto.Expression.FindByExpressionId(ExpressionId);

                row.Name = ExpressionName.Text;
            }
            else
            {
                ExprFilter.Visible = false;
            }
			////Serialize FilterExpression in segment table
			//if (_SegmentDto != null && _SegmentDto.Segment.Count > 0)
			//{
			//    SegmentDto.SegmentRow row = _SegmentDto.Segment[0];
			//    row.ExpressionFilter = SerializeFilterExpressions(ExpressionFilters);
			//}

			CommandParameters cp = new CommandParameters(_RuleEditCommandString);
            CommandHandler.RegisterCloseOpenedFrameScript(this.Page, cp.ToString());
        }

		/// <summary>
		/// Binds the form.
		/// </summary>
		private void BindForm()
		{
			ExpressionDto.ExpressionRow row = null;
			if (_ExpressionDto != null && _ExpressionDto.Expression.Count() != 0)
			{
				if (ExpressionId == 0 || ExpressionId == -1)// get the last row added
				{
					row = _ExpressionDto.Expression[_ExpressionDto.Expression.Count - 1];
				}
				else
				{
					row = _ExpressionDto.Expression.FindByExpressionId(ExpressionId);
				}
			}
			if (row != null)
			{
				ExpressionName.Text = row.Name;
			}
		}

        /// <summary>
        /// Binds the expression editor.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        private void BindExpressionEditor(object dataSource)
        {
            ExprFilter.ProviderName = "CustomerDataProvider";
            ExprFilter.ExpressionKey = ExpressionId.ToString();
            ExprFilter.ExpressionPlace = "Customer";
            ExprFilter.DataSource = dataSource;
            ExprFilter.DataBind();
        }
    }
}