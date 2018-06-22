using System;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Mediachase.Commerce.Manager.Core;
using Mediachase.Commerce.Orders.Dto;
using Mediachase.Commerce.Orders.Managers;
using Mediachase.Commerce.Shared;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Common;

namespace Mediachase.Commerce.Manager.Order
{
	public partial class JurisdictionEdit : OrderBaseUserControl
	{
		private const string _JurisdictionDtoEditSessionKey = "ECF.JurisdictionDto.Edit";
		private const string _JurisdictionIdString = "jid";
		private const string _JurisdictionDtoString = "JurisdictionDto";
        private const string _JurisdictionTypeString = "Jurisdiction-List&Type";

		private JurisdictionDto _Jurisdiction = null;

		/// <summary>
		/// Gets the Jurisdiction id.
		/// </summary>
		/// <value>The Jurisdiction id.</value>
		public int JurisdictionId
		{
			get
			{
				return ManagementHelper.GetIntFromQueryString(_JurisdictionIdString);
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
            //first check for permissions for create or edit permissions. If JurisdictionId > 0, its an edit
            if (JurisdictionId > 0)
            {
                if (JurisdictionType == JurisdictionManager.JurisdictionType.Tax)
                    SecurityManager.CheckRolePermission("order:admin:taxes:mng:edit");
                else
                    SecurityManager.CheckRolePermission("order:admin:shipping:jur:mng:edit");
            }
            else
            {
                if (JurisdictionType == JurisdictionManager.JurisdictionType.Tax)
                    SecurityManager.CheckRolePermission("order:admin:taxes:mng:create");
                else
                    SecurityManager.CheckRolePermission("order:admin:shipping:jur:mng:create");
            }
            
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
		private JurisdictionDto LoadFresh()
		{
			JurisdictionDto jurisdiction = JurisdictionManager.GetJurisdiction(JurisdictionId);

			// persist in session
			Session[_JurisdictionDtoEditSessionKey] = jurisdiction;

			return jurisdiction;
		}

		/// <summary>
		/// Loads the context.
		/// </summary>
		private void LoadContext()
		{
			if (JurisdictionId > 0)
			{
				JurisdictionDto jurisdiction = null;
				if (!this.IsPostBack && (!this.Request.QueryString.ToString().Contains("Callback=yes"))) // load fresh on initial load
					jurisdiction = LoadFresh();
				else // load from session
				{
					jurisdiction = (JurisdictionDto)Session[_JurisdictionDtoEditSessionKey];

					if (jurisdiction == null)
						jurisdiction = LoadFresh();
				}

				// Put a dictionary key that can be used by other tabs
				IDictionary dic = new ListDictionary();
				dic.Add(_JurisdictionDtoString, jurisdiction);

				// Call tabs load context
				ViewControl.LoadContext(dic);

				_Jurisdiction = jurisdiction;
			}
		}

		/// <summary>
		/// Handles the SaveChanges event of the EditSaveControl control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="Mediachase.Commerce.Manager.Core.SaveControl.SaveEventArgs"/> instance containing the event data.</param>
		void EditSaveControl_SaveChanges(object sender, SaveControl.SaveEventArgs e)
		{
			if (!this.Page.IsValid)
			{
				e.RunScript = false;
				return;
			}

			JurisdictionDto jurisdiction = (JurisdictionDto)Session[_JurisdictionDtoEditSessionKey];

			if (JurisdictionId > 0 && jurisdiction == null)
				jurisdiction = JurisdictionManager.GetJurisdiction(JurisdictionId);
			else if (JurisdictionId == 0)
				jurisdiction = new JurisdictionDto();

			IDictionary context = new ListDictionary();
			context.Add(_JurisdictionDtoString, jurisdiction);

			ViewControl.SaveChanges(context);

			if (jurisdiction.HasChanges())
				JurisdictionManager.SaveJurisdiction(jurisdiction);

			// we don't need to store Dto in session any more
			Session.Remove(_JurisdictionDtoEditSessionKey);
		}
	}
}