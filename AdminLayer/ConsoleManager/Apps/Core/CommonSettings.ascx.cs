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
using Mediachase.Commerce.Core.Dto;
using Mediachase.Commerce.Core.Managers;
using Mediachase.Commerce.Manager.Core;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Common;

namespace Mediachase.Commerce.Manager.Core
{
	public partial class CommonSettings : BaseUserControl
	{
		private const string _SettingsDtoEditSessionKey = "ECF.SettingsDto.Edit";
		private const string _SettingsDtoString = "SettingsDto";

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
		private SettingsDto LoadFresh()
		{
			SettingsDto settings = CommonSettingsManager.GetSettingsDto();

			if (settings == null)
				settings = new SettingsDto();

			// persist in session
			Session[_SettingsDtoEditSessionKey] = settings;

			return settings;
		}

		/// <summary>
		/// Loads the context.
		/// </summary>
		private void LoadContext()
		{
			SettingsDto settings = null;
			if (!this.IsPostBack && (!this.Request.QueryString.ToString().Contains("Callback=yes"))) // load fresh on initial load
				settings = LoadFresh();
			else // load from session
			{
				settings = (SettingsDto)Session[_SettingsDtoEditSessionKey];

				if (settings == null)
					settings = LoadFresh();
			}

			// Put a dictionary key that can be used by other tabs
			IDictionary dic = new ListDictionary();
			dic.Add(_SettingsDtoString, settings);

			// Call tabs load context
			ViewControl.LoadContext(dic);
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

			try
			{
				SettingsDto settings = (SettingsDto)Session[_SettingsDtoEditSessionKey];

				if (settings == null)
					settings = new SettingsDto();

				IDictionary context = new ListDictionary();
				context.Add(_SettingsDtoString, settings);

				ViewControl.SaveChanges(context);

				if (settings.HasChanges())
				{
					// commit changes to the db
					CommonSettingsManager.SaveSettings(settings);
				}
			}
			catch (Exception ex)
			{
				e.RunScript = false;
				DisplayErrorMessage(ex.Message);
			}
			finally
			{
				// we don't need to store Dto in session any more
				Session.Remove(_SettingsDtoEditSessionKey);
			}
		}
	}
}