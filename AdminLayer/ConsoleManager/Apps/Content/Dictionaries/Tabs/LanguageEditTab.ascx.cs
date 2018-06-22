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
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Interfaces;
using Mediachase.Web.Console.Common;
using Mediachase.Cms;

namespace Mediachase.Commerce.Manager.Content.Dictionaries.Tabs
{
	public partial class LanguageEditTab : BaseUserControl, IAdminTabControl, IAdminContextControl
	{
		private const string _LanguageIdString = "langid";

		/// <summary>
		/// Gets the Language id.
		/// </summary>
		/// <value>The Language id.</value>
		public int LanguageId
		{
			get
			{
				return ManagementHelper.GetIntFromQueryString(_LanguageIdString);
			}
		}

		/// <summary>
		/// Handles the Load event of the Page control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (!this.IsPostBack)
				BindForm();
		}

		/// <summary>
		/// Binds the form.
		/// </summary>
		private void BindForm()
		{
			if (LanguageId > 0)
			{
				bool found = false;
				using (IDataReader dr = Language.LoadLanguage(LanguageId))
				{
					if (dr.Read())
					{
						found = true;
						this.tbLanguageName.Text = dr["LangName"].ToString();
						this.FriendlyNameText.Text = dr["FriendlyName"] != DBNull.Value ? dr["FriendlyName"].ToString() : String.Empty;
						this.IsDefault.IsSelected = dr["IsDefault"] != DBNull.Value ? (bool)dr["IsDefault"] : false;
					}
                    dr.Close();
				}

				if (!found)
				{
					DisplayErrorMessage(String.Format("Language with id={0} not found.", LanguageId));
					return;
				}
			}
		}

		/// <summary>
		/// Checks if entered language code is unique.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="args">The <see cref="System.Web.UI.WebControls.ServerValidateEventArgs"/> instance containing the event data.</param>
		public void LanguageCodeCheck(object sender, ServerValidateEventArgs args)
		{
			bool exists = false;

			// load language by code
			using (IDataReader dr = Language.GetLangByName(args.Value))
			{
				if (dr.Read())
				{
					if ((int)dr["LangId"] != LanguageId)
						exists = true;
				}
                dr.Close();
			}

			if(exists)
			{
				args.IsValid = false;
				return;
			}

			args.IsValid = true;
		}

		#region IAdminContextControl Members
		/// <summary>
		/// Loads the context.
		/// </summary>
		/// <param name="context">The context.</param>
		public void LoadContext(IDictionary context)
		{
		}
		#endregion

		#region IAdminTabControl Members
		/// <summary>
		/// Saves the changes.
		/// </summary>
		/// <param name="context">The context.</param>
		public void SaveChanges(IDictionary context)
		{
			if (LanguageId > 0)
				Language.UpdateLanguage(LanguageId, tbLanguageName.Text, FriendlyNameText.Text, IsDefault.IsSelected);
			else
			{
				// add new language
				Language.AddLanguage(tbLanguageName.Text, FriendlyNameText.Text, IsDefault.IsSelected);
			}
		}
		#endregion
	}
}