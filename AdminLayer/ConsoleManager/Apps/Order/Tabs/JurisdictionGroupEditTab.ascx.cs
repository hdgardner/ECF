using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Orders.Dto;
using Mediachase.Commerce.Orders.Managers;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Common;
using Mediachase.Web.Console.Interfaces;
using System.Collections.Generic;

namespace Mediachase.Commerce.Manager.Order.Tabs
{
	public partial class JurisdictionGroupEditTab : OrderBaseUserControl, IAdminTabControl, IAdminContextControl
	{
		private const string _JurisdictionGroupIdString = "jid";
		private const string _JurisdictionDtoString = "JurisdictionDto";

		private JurisdictionDto _JurisdictionDto = null;

		/// <summary>
		/// Gets the JurisdictionGroup id.
		/// </summary>
		/// <value>The JurisdictionGroup id.</value>
		public int JurisdictionGroupId
		{
			get
			{
				return ManagementHelper.GetIntFromQueryString(_JurisdictionGroupIdString);
			}
		}

		/// <summary>
		/// Gets the JurisdictionType.
		/// </summary>
		/// <value>The JurisdictionType.</value>
		public JurisdictionManager.JurisdictionType JurisdictionType
		{
			get
			{
				JurisdictionManager.JurisdictionType jType = JurisdictionManager.JurisdictionType.Shipping;

				string type = ManagementHelper.GetStringValue(Request.QueryString["type"], String.Empty);
				if (!String.IsNullOrEmpty(type))
				{
					try
					{
						jType = (JurisdictionManager.JurisdictionType)Enum.Parse(typeof(JurisdictionManager.JurisdictionType), type);
					}
					catch { }
				}

				return jType;
			}
		}

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!this.IsPostBack)
				BindForm();
		}

		/// <summary>
		/// Binds the form.
		/// </summary>
		private void BindForm()
		{
			if (JurisdictionGroupId > 0)
			{
				if (_JurisdictionDto.JurisdictionGroup.Count > 0)
				{
					this.DisplayName.Text = _JurisdictionDto.JurisdictionGroup[0].DisplayName;
					this.Code.Text = _JurisdictionDto.JurisdictionGroup[0].Code;
					BindJurisdictionsList(_JurisdictionDto.JurisdictionGroup[0]);
				}
				else
					DisplayErrorMessage(String.Format("JurisdictionGroup with id={0} not found.", JurisdictionGroupId));
			}
			else
			{
				BindJurisdictionsList(null);
			}
		}

		/// <summary>
		/// Binds the jurisdictions list.
		/// </summary>
		/// <param name="jurisdictionGroupRow">The jurisdiction group row.</param>
		private void BindJurisdictionsList(JurisdictionDto.JurisdictionGroupRow jurisdictionGroupRow)
		{
			List<JurisdictionDto.JurisdictionRow> leftJurisdictions = new List<JurisdictionDto.JurisdictionRow>();
			List<JurisdictionDto.JurisdictionRow> rightJurisdictions = new List<JurisdictionDto.JurisdictionRow>();

            JurisdictionManager.JurisdictionType jType = JurisdictionType;
            if(jurisdictionGroupRow != null)
                jType = (JurisdictionManager.JurisdictionType)Enum.Parse(typeof(JurisdictionManager.JurisdictionType), jurisdictionGroupRow.JurisdictionType.ToString());

            JurisdictionDto dto = JurisdictionManager.GetJurisdictions(jType);

			bool allToLeft = false; // if true, then add all jurisdictions to the left list

			if (jurisdictionGroupRow != null)
			{
				JurisdictionDto.JurisdictionRelationRow[] selectedJurisdictionRows = jurisdictionGroupRow.GetJurisdictionRelationRows();
				if (selectedJurisdictionRows != null && selectedJurisdictionRows.Length > 0)
				{
					foreach (JurisdictionDto.JurisdictionRow jRow in dto.Jurisdiction)
					{
						bool found = false;
						foreach (JurisdictionDto.JurisdictionRelationRow selectedJurisdictionRow in selectedJurisdictionRows)
						{
							if (jRow.JurisdictionId == selectedJurisdictionRow.JurisdictionId)
							{
								found = true;
								break;
							}
						}

						if (found)
							rightJurisdictions.Add(jRow);
						else
							leftJurisdictions.Add(jRow);
					}

					JurisdictionsList.LeftDataSource = leftJurisdictions;
					JurisdictionsList.RightDataSource = rightJurisdictions;
				}
				else
					// add all jurisdictions to the left list
					allToLeft = true;
			}
			else
				allToLeft = true;

			if (allToLeft)
				// add all jurisdictions to the left list
				JurisdictionsList.LeftDataSource = dto.Jurisdiction;

			JurisdictionsList.DataBind();
		}

		/// <summary>
		/// Checks if entered code is unique.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="args">The <see cref="System.Web.UI.WebControls.ServerValidateEventArgs"/> instance containing the event data.</param>
		public void CodeCheck(object sender, ServerValidateEventArgs args)
		{
			// load jurisdiction group by code
			JurisdictionDto dto = JurisdictionManager.GetJurisdictionGroup(args.Value);

			// check if jurisdiction group with specified code is loaded
			if (dto != null && dto.JurisdictionGroup.Count > 0 &&
				dto.JurisdictionGroup[0].JurisdictionGroupId != JurisdictionGroupId &&
				String.Compare(dto.JurisdictionGroup[0].Code, args.Value, StringComparison.CurrentCultureIgnoreCase) == 0)
			{
				args.IsValid = false;
				return;
			}

			args.IsValid = true;
		}

		#region IAdminTabControl Members
		public void SaveChanges(IDictionary context)
		{
			JurisdictionDto dto = (JurisdictionDto)context[_JurisdictionDtoString];
			JurisdictionDto.JurisdictionGroupRow row = null;

			if (dto.JurisdictionGroup.Count > 0)
			{
				row = dto.JurisdictionGroup[0];
			}
			else
			{
				row = dto.JurisdictionGroup.NewJurisdictionGroupRow();
				row.JurisdictionType = JurisdictionType.GetHashCode();
			}

			row.ApplicationId = OrderConfiguration.Instance.ApplicationId;
			row.DisplayName = DisplayName.Text;
			row.Code = Code.Text;

			if (row.RowState == DataRowState.Detached)
				dto.JurisdictionGroup.Rows.Add(row);

			// update jurisdictions in the group

			// a). delete rows from dto that are not selected
			bool found = false;
			foreach (JurisdictionDto.JurisdictionRelationRow rowTmp in row.GetJurisdictionRelationRows())
			{
				found = false;
				ListItem lItem = JurisdictionsList.LeftItems.FindByValue(rowTmp.JurisdictionId.ToString());
				if (lItem != null)
					rowTmp.Delete();
			}

			// b). add selected rows to dto
			foreach (ListItem item in JurisdictionsList.RightItems)
			{
				bool exists = false;
				foreach (JurisdictionDto.JurisdictionRelationRow rowTmp in row.GetJurisdictionRelationRows())
				{
					if (String.Compare(item.Value, rowTmp.JurisdictionId.ToString(), true) == 0)
					{
						exists = true;
						break;
					}
				}

				// if jurisdiction not in the group, add it
				if (!exists)
				{
					// add jurisdiction to the group
					JurisdictionDto.JurisdictionRelationRow newRow = dto.JurisdictionRelation.NewJurisdictionRelationRow();
					newRow.JurisdictionId = Int32.Parse(item.Value);
					newRow.JurisdictionGroupId = row.JurisdictionGroupId;

					// add the row to the dto
					dto.JurisdictionRelation.Rows.Add(newRow);
				}
			}
		}
		#endregion

		#region IAdminContextControl Members
		public void LoadContext(IDictionary context)
		{
			_JurisdictionDto = (JurisdictionDto)context[_JurisdictionDtoString];
		}
		#endregion
	}
}