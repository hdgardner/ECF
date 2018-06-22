using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Mediachase.Ibn.Core.Layout;
using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Ibn.Web.UI.Controls.Util;

namespace Mediachase.Ibn.Web.UI.MetaUI
{
	public partial class FormDocumentView : Mediachase.Ibn.Web.UI.WebControls.MCDataBoundControl
	{
		private readonly string placeNameKey = "PlaceName";

		#region FormName
		public string FormName
		{
			get
			{
				return (ViewState["FormName"] != null) ? ViewState["FormName"].ToString() : "";
			}
			set
			{
				ViewState["FormName"] = value;
			}
		}
		#endregion

		#region FormType
		public FormType FormType
		{
			get
			{
				return (ViewState["FormType"] != null) ? (FormType)ViewState["FormType"] : FormType.NotAssigned;
			}
			set
			{
				ViewState["FormType"] = value;
			}
		}
		#endregion

		#region PlaceName
		public string PlaceName
		{
			get
			{
				if (ViewState[placeNameKey] != null)
					return (string)ViewState[placeNameKey];
				else
					return String.Empty;
			}
			set
			{
				ViewState[placeNameKey] = value;
			}
		}
		#endregion

		#region FormExists
		private bool _formExists = false;
		public bool FormExists
		{
			get { return _formExists; }
		}
		#endregion

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, EventArgs e)
		{
		}

		#region DataBind
        /// <summary>
        /// Binds a data source to the invoked server control and all its child controls.
        /// </summary>
		public override void  DataBind()
		{
			string className = "";
			if (DataItem != null)
			{
				MetaObject mo = null;

				if (DataItem is MetaObject)
					mo = (MetaObject)DataItem;
				else if (DataItem is BusinessObjectRequest)
					mo = ((BusinessObjectRequest)DataItem).MetaObject;

				className = mo.GetMetaType().Name;
				if (mo.GetCardMetaType() != null)
					className = mo.GetCardMetaType().Name;
				ViewState["ClassName"] = className;

				if (String.IsNullOrEmpty(FormName))
				{
					if (DataItem is MetaObject)
						FormName = FormController.BaseFormType;
					else if (DataItem is BusinessObjectRequest)
						FormName = FormController.CreateFormType;
				}
			}
			else if (ViewState["ClassName"] != null)
				className = ViewState["ClassName"].ToString();

			FormDocument doc = FormController.LoadFormDocument(className, FormName);
			if (doc != null)
			{
				_formExists = true;
				fRenderer.FormDocumentData = doc;
				fRenderer.FormType = FormType;
				fRenderer.PlaceName = PlaceName;
				if (DataItem != null)
					fRenderer.DataItem = DataItem;
				fRenderer.DataBind();
			}
		}
		#endregion
	}
}