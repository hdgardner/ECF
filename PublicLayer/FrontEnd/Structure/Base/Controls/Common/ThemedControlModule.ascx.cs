namespace Mediachase.eCF.PublicStore.SharedModules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
    using System.Web.UI;

	/// <summary>
	///	Used to load themed control. The control will first check if specified control exists in
    /// the current master template directory. If it does not, the default directory will be used. This allows 
    /// for paths to dynamically change based on the master template specified.
    /// 
    /// The control adds all the child controls to the collection before the viewstate is restored.
	/// </summary>
    public partial class ThemedControlModule : UserControl
	{
		private string _ThemePath = "";
		private System.Web.UI.Control ctrl = null;

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, System.EventArgs e)
		{
            // Make sure controls are created before the viewstate is initialized
            // SASHA: fix, this control does not need to keep viewstate so init it in page_load
            CreateChildControlsTree();
		}

        /// <summary>
        /// Gets or sets the theme path.
        /// </summary>
        /// <value>The theme path.</value>
		public string ThemePath
		{
			get
			{
				return _ThemePath;
			}
			set
			{
				_ThemePath = value;
			}
		}

        /// <summary>
        /// Gets the control.
        /// </summary>
        /// <value>The control.</value>
		public System.Web.UI.Control Control
		{
			get
			{
				return ctrl;
			}
		}

        /// <summary>
        /// Creates child controls before viewstate is initialized.
        /// </summary>
		private void CreateChildControlsTree()
		{
			string id = "Theme"+this.ID;
			ctrl = ThemeControlHolder.FindControl(id);
            string StoreMasterHome = this.Page.Theme;
			if(ctrl == null)
			{
				//this.ThemeControlHolder.Controls.Clear();
				if(System.IO.File.Exists(this.MapPath("~/Templates/" + StoreMasterHome + "/"+_ThemePath)))
                    ctrl = this.LoadControl("~/Templates/" + StoreMasterHome + "/" + _ThemePath);
				else
                    ctrl = this.LoadControl("~/Templates/Default/" + _ThemePath);
				
                ctrl.ID = id;
				this.ThemeControlHolder.Controls.Add(ctrl);
			}

            // we should not call databind since we just adding controls here
            // allow time to restore the viewstate if needed
			// ctrl.DataBind();
		}

		#region Web Form Designer generated code
        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"></see> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"></see> object that contains the event data.</param>
		override protected void OnInit(EventArgs e)
		{
            // Proceed with the rest
			InitializeComponent();
			base.OnInit(e);
		}

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
		private void InitializeComponent()
		{

		}
		#endregion
	}
}