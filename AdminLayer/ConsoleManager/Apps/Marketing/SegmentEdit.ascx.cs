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
using Mediachase.Commerce.Marketing;

namespace Mediachase.Commerce.Manager.Marketing
{
    public partial class SegmentEdit : MarketingBaseUserControl
    {
		private const string _SegmentDtoEditSessionKey = "ECF.SegmentDto.Edit";
		private const string _SegmentIdString = "SegmentId";
		private const string _SegmentDtoString = "SegmentDto";

        private const string _ExpressionDtoEditSessionKey = "ECF.ExpressionDto.Edit";
        private const string _ExpressionDtoString = "ExpressionDto";

        /// <summary>
        /// Gets the segment id.
        /// </summary>
        /// <value>The segment id.</value>
        public int SegmentId
        {
            get
            {
                return ManagementHelper.GetIntFromQueryString(_SegmentIdString);
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
        private SegmentDto LoadFresh()
        {
            SegmentDto segment = null;

            if (SegmentId == 0)
            {
                // Create an empty segment
                segment = new SegmentDto();
                SegmentDto.SegmentRow row = segment.Segment.NewSegmentRow();
                row.ApplicationId = MarketingConfiguration.Instance.ApplicationId;

                row.Name = "";
                row.DisplayName = "";
                row.Description = "";

                if (row.RowState == DataRowState.Detached)
                    segment.Segment.Rows.Add(row);
            }
            else
            {
                segment = SegmentManager.GetSegmentDto(SegmentId);
            }

            // persist in session
            Session[_SegmentDtoEditSessionKey] = segment;

            return segment;
        }

        private ExpressionDto LoadFreshExpression()
        {
            ExpressionDto expression = null;

            if (SegmentId == 0)
            {
                // Create an empty expression
                expression = new ExpressionDto();
            }
            else
            {
                expression = ExpressionManager.GetExpressionBySegmentDto(SegmentId);
            }

            // persist in session
            Session[_ExpressionDtoEditSessionKey] = expression;

            return expression;
        }

        /// <summary>
        /// Loads the context.
        /// </summary>
        private void LoadContext()
        {
			//if (SegmentId > 0)
			{
				SegmentDto segment = null;
                ExpressionDto expression = null;
				if (!this.IsPostBack && (!this.Request.QueryString.ToString().Contains("Callback=yes"))) // load fresh on initial load
				{
					segment = LoadFresh();
                    expression = LoadFreshExpression();
				}
				else // load from session
				{
					segment = (SegmentDto)Session[_SegmentDtoEditSessionKey];
                    expression = (ExpressionDto)Session[_ExpressionDtoEditSessionKey];

					if (segment == null)
					{
						segment = LoadFresh();
					}

                    if (expression == null)
                    {
                        expression = LoadFreshExpression();
                    }

				}

				// Put a dictionary key that can be used by other tabs
				IDictionary dic = new ListDictionary();
				dic.Add(_SegmentDtoString, segment);
                dic.Add(_ExpressionDtoString, expression);

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

			SegmentDto segment = (SegmentDto)Session[_SegmentDtoEditSessionKey];
            ExpressionDto expression = (ExpressionDto)Session[_ExpressionDtoEditSessionKey];

            if (segment == null && SegmentId > 0)
                segment = SegmentManager.GetSegmentDto(SegmentId);
            else if (segment == null && SegmentId == 0)
				segment = new SegmentDto();


            if (expression == null && SegmentId > 0)
                expression = ExpressionManager.GetExpressionBySegmentDto(SegmentId);
            else if (expression == null && SegmentId == 0)
                expression = new ExpressionDto();

			/*
			// if we add a new segment, remove all other segments from Dto that is passed to control that saves changes
			if (SegmentId == 0 && segment != null && segment.Segment.Count > 0)
			{
				SegmentDto.SegmentRow[] rows2del = (SegmentDto.SegmentRow[])segment.Segment.Select(String.Format("{0} <> {1}", _SegmentIdString, SegmentId));
				if (rows2del != null)
					foreach (SegmentDto.SegmentRow row in rows2del)
						segment.Segment.RemoveSegmentRow(row);
			}*/

            IDictionary context = new ListDictionary();
			context.Add(_SegmentDtoString, segment);
            context.Add(_ExpressionDtoString, expression);            

            ViewControl.SaveChanges(context);

			// save expressionDto
			if (expression.HasChanges())
				ExpressionManager.SaveExpression(expression);

			// update segment conditions
			foreach (ExpressionDto.ExpressionRow tmpRow in expression.Expression.Rows)
			{
				// skip deleted rows
				if (tmpRow.RowState == DataRowState.Deleted)
					continue;

				// add SegmentConditionRow
				SegmentDto.SegmentConditionRow[] segmentConditionRows = (SegmentDto.SegmentConditionRow[])segment.SegmentCondition.Select(String.Format("ExpressionId={0}", tmpRow.ExpressionId));
				if (segmentConditionRows == null || segmentConditionRows.Length==0)
				{
					// add new expression
					SegmentDto.SegmentConditionRow newSCRow = segment.SegmentCondition.NewSegmentConditionRow();
					newSCRow.ExpressionId = tmpRow.ExpressionId;
					newSCRow.SegmentId = segment.Segment[0].SegmentId;

					if (newSCRow.RowState == DataRowState.Detached)
						segment.SegmentCondition.Rows.Add(newSCRow);
				}
			}

			// save segmentDto
            if (segment.HasChanges())
                SegmentManager.SaveSegment(segment);

			// we don't need to store Dto in session any more
			Session.Remove(_SegmentDtoEditSessionKey);
            Session.Remove(_ExpressionDtoEditSessionKey);
        }
    }
}