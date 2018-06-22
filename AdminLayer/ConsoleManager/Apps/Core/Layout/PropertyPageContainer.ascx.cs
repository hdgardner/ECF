using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace Mediachase.Ibn.Web.UI.Layout
{
	public partial class PropertyPageContainer : UserControl
	{
		#region prop: UpdateId
        /// <summary>
        /// Gets the update id.
        /// </summary>
        /// <value>The update id.</value>
		public string UpdateId
		{
			get
			{
				return hfPrimary.ClientID;
			}
		} 
		#endregion

		#region prop: ContainerId
        /// <summary>
        /// Gets the container id.
        /// </summary>
        /// <value>The container id.</value>
		public string ContainerId
		{
			get
			{
				return PropertyPagePanel.ClientID;
			}
		}
		#endregion

		#region prop: CancelElementId
        /// <summary>
        /// Gets the cancel element id.
        /// </summary>
        /// <value>The cancel element id.</value>
		public string CancelElementId
		{
			get
			{
				return btnCancel.ClientID;
			}
		}
		#endregion

		#region prop: SaveElementId
        /// <summary>
        /// Gets the save element id.
        /// </summary>
        /// <value>The save element id.</value>
		public string SaveElementId
		{
			get
			{
				return btnSaveReal.ClientID;
			}
		}
		#endregion

		#region prop: SaveCommand
        /// <summary>
        /// Gets the save command.
        /// </summary>
        /// <value>The save command.</value>
		public string SaveCommand
		{
			get
			{
				return this.Page.ClientScript.GetPostBackEventReference(btnSave, "");
			}
		}
		#endregion

		#region prop: PropertyControlUid
        /// <summary>
        /// Gets or sets the property control uid.
        /// </summary>
        /// <value>The property control uid.</value>
		public string PropertyControlUid
		{
			get
			{
				if (ViewState["__PropertyControlUid"] != null)
					return ViewState["__PropertyControlUid"].ToString();

				return string.Empty;
			}
			set
			{
				ViewState["__PropertyControlUid"] = value;
			}
		}
		#endregion

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, EventArgs e)
		{
			//ScriptManager.GetCurrent(this.Page).RegisterPostBackControl(btnSave);
			hfPrimary.ValueChanged += new EventHandler(hfPrimary_ValueChanged);
			btnSave.Click += new EventHandler(btnSave_Click);
		}

		#region btnSave_Click
        /// <summary>
        /// Handles the Click event of the btnSave control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
		void btnSave_Click(object sender, EventArgs e)
		{
			foreach (Control c in mainContainer.Controls)
			{
				if (c is IPropertyPageControl)
				{
					(c as IPropertyPageControl).Save();
				}
			}
		} 
		#endregion

		#region hfPrimary_ValueChanged
        /// <summary>
        /// Handles the ValueChanged event of the hfPrimary control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
		void hfPrimary_ValueChanged(object sender, EventArgs e)
		{
			if (hfPrimary.Value == string.Empty || hfPrimary.Value.Split('^').Length < 2)
				throw new ArgumentException("hfUpdate");

			string uid = hfPrimary.Value.Split('^')[0];
			mainContainer.Style.Add("display", "block");
			buttonContainer.Style.Add("display", "block");
			backgroundContainer.Style.Add("display", "block");

			this.PropertyControlUid = uid;

			Control c = DynamicControlFactory.CreatePropertyPage(this.Page, uid);
			c.ID = String.Format("wrapControl{0}", uid.Replace("-", String.Empty));
			mainContainer.Controls.Add(c);
			//TO DO: load PropertyPageControl + Bind
		} 
		#endregion

        /// <summary>
        /// Called by the ASP.NET page framework to notify server controls that use composition-based implementation to create any child controls they contain in preparation for posting back or rendering.
        /// </summary>
		protected override void CreateChildControls()
		{
			if (this.PropertyControlUid != string.Empty)
			{
				Control c = DynamicControlFactory.CreatePropertyPage(this.Page, this.PropertyControlUid);
				c.ID = String.Format("wrapControl{0}", this.PropertyControlUid.Replace("-", string.Empty));
				mainContainer.Controls.Add(c);
			}
		}
	}
}