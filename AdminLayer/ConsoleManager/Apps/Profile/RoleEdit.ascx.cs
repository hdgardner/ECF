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
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Commerce.Manager.Core;
using Mediachase.Commerce.Profile.Dto;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Common;
using Mediachase.Commerce.Profile.Managers;
using System.Collections.Specialized;

namespace Mediachase.Commerce.Manager.Profile
{
	public partial class RoleEdit : ProfileBaseUserControl
	{
		private const string _PermissionDtoEditSessionKey = "ECF.PermissionDto.Edit";
		private const string _RoleIdString = "RoleId";
		private const string _PermissionDtoString = "PermissionDto";

		private PermissionDto _Permission = null;

		/// <summary>
		/// Gets the role id.
		/// </summary>
		/// <value>The role id.</value>
		public string RoleId
		{
			get
			{
				return ManagementHelper.GetValueFromQueryString(_RoleIdString, String.Empty);
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
		private PermissionDto LoadFresh()
		{
			PermissionDto permission = PermissionManager.GetPermissionDto(RoleId);

			// persist in session
			Session[_PermissionDtoEditSessionKey] = permission;

			return permission;
		}

		/// <summary>
		/// Loads the context.
		/// </summary>
		private void LoadContext()
		{
			if (!String.IsNullOrEmpty(RoleId))
			{
				PermissionDto permission = null;
				if (!this.IsPostBack && (!this.Request.QueryString.ToString().Contains("Callback=yes"))) // load fresh on initial load
					permission = LoadFresh();
				else // load from session
				{
					permission = (PermissionDto)Session[_PermissionDtoEditSessionKey];

					if (permission == null)
						permission = LoadFresh();
				}

				// Put a dictionary key that can be used by other tabs
				IDictionary dic = new ListDictionary();
				dic.Add(_PermissionDtoString, permission);

				// Call tabs load context
				ViewControl.LoadContext(dic);

				_Permission = permission;
			}
		}

		/// <summary>
		/// Handles the SaveChanges event of the EditSaveControl control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void EditSaveControl_SaveChanges(object sender, Mediachase.Commerce.Manager.Core.SaveControl.SaveEventArgs e)
		{
			// Validate form
			if (!this.Page.IsValid)
			{
				e.RunScript = false;
				return;
			}

			try
			{
				PermissionDto permission = (PermissionDto)Session[_PermissionDtoEditSessionKey];

				if (!String.IsNullOrEmpty(RoleId))
					permission = PermissionManager.GetPermissionDto(RoleId);
				else
					permission = new PermissionDto();

				IDictionary context = new ListDictionary();
				context.Add(_PermissionDtoString, permission);

				ViewControl.SaveChanges(context);

				if (permission.HasChanges())
					PermissionManager.SavePermission(permission);
			}
			catch (MembershipCreateUserException ex)
			{
				e.RunScript = false;
				DisplayErrorMessage(ex.Message);
			}
			finally
			{
				// we don't need to store Dto in session any more
				Session.Remove(_PermissionDtoEditSessionKey);
			}
		}
	}
}