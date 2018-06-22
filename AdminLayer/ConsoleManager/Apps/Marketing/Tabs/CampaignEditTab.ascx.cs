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
    public partial class CampaignEditTab : BaseUserControl, IAdminTabControl, IAdminContextControl
    {
		private const string _CampaignDtoString = "CampaignDto";

        CampaignDto _campaign = null;

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                BindForm();
            }
        }

        /// <summary>
        /// Binds the form.
        /// </summary>
        private void BindForm()
        {
            // Bind segments
            TargetSegments.DataSource = SegmentManager.GetSegmentDto();
            TargetSegments.DataBind();

            if (_campaign != null)
            {
                //first check permissions
                //if permissions not present, deny
                SecurityManager.CheckRolePermission("marketing:campaigns:mng:edit");

                CampaignDto.CampaignRow row = _campaign.Campaign[0];
                CampaignName.Text = row.Name;
				this.AvailableFrom.Value = ManagementHelper.GetUserDateTime(row.StartDate);
				this.ExpiresOn.Value = ManagementHelper.GetUserDateTime(row.EndDate);
                IsActive.IsSelected = row.IsActive;
                IsArchived.IsSelected = row.IsArchived;
                Comments.Text = row.Comments;

                foreach (CampaignDto.CampaignSegmentRow segmentRow in row.GetCampaignSegmentRows())
                {
                    ManagementHelper.SelectListItem(TargetSegments, segmentRow.SegmentId.ToString(), false);
                }
            }
            else
            {
                //first check permissions
                //if permissions not present, deny
                SecurityManager.CheckRolePermission("marketing:campaigns:mng:create");

				this.AvailableFrom.Value = ManagementHelper.GetUserDateTimeNow();
				this.ExpiresOn.Value = ManagementHelper.GetUserDateTimeNow().AddMonths(1);
            }
        }

        #region IAdminTabControl Members
        /// <summary>
        /// Saves the changes.
        /// </summary>
        /// <param name="context">The context.</param>
        public void SaveChanges(IDictionary context)
        {
            CampaignDto dto = (CampaignDto)context[_CampaignDtoString];
            CampaignDto.CampaignRow row = null;

            if (dto.Campaign.Count > 0)
            {
                row = dto.Campaign[0];
                row.Modified = DateTime.UtcNow;
                row.ModifiedBy = Page.User.Identity.Name;
            }
            else
            {
                row = dto.Campaign.NewCampaignRow();
                row.ApplicationId = MarketingConfiguration.Instance.ApplicationId;
                row.Created = DateTime.UtcNow;
                row.ModifiedBy = Page.User.Identity.Name;
            }

            row.Name = CampaignName.Text;
            row.StartDate = this.AvailableFrom.Value.ToUniversalTime();
            row.EndDate = this.ExpiresOn.Value.ToUniversalTime();
            row.IsActive = IsActive.IsSelected;
            row.IsArchived = IsArchived.IsSelected;
            row.Comments = Comments.Text;

            if (row.RowState == DataRowState.Detached)
                dto.Campaign.Rows.Add(row);

            // Attach segments
            // Clear existing segments
            if (dto.CampaignSegment.Count > 0)
            {
                foreach (CampaignDto.CampaignSegmentRow segmentrow in dto.CampaignSegment.Rows)
                    segmentrow.Delete();
            }

            // Attach selected ones
            foreach (ListItem item in TargetSegments.Items)
            {
                if (!item.Selected)
                    continue;

                CampaignDto.CampaignSegmentRow srow = dto.CampaignSegment.NewCampaignSegmentRow();;
                srow.CampaignId = row.CampaignId;
                srow.SegmentId = Int32.Parse(item.Value);
                dto.CampaignSegment.Rows.Add(srow);
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
			_campaign = (CampaignDto)context[_CampaignDtoString];

        }
        #endregion
    }
}