namespace Mediachase.Commerce.Manager.Core
{
	using System;
	using System.Collections;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
    using Mediachase.Web.Console.BaseClasses;

	/// <summary>
	///		Summary description for ErrorModule1.
	/// </summary>
	public partial class ErrorModule : System.Web.UI.UserControl
	{
        private ArrayList _Messages = new ArrayList();

        /// <summary>
        /// Gets or sets the messages.
        /// </summary>
        /// <value>The messages.</value>
        public ArrayList Messages
        {
            get { return _Messages; }
            set { _Messages = value; }
        }



		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (_Messages.Count == 0)
				//this.Visible = false;
				ErrorMessages.Visible = false;
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			ErrorManager.Instance.Error += new ErrorEventHandler(Instance_Error);
		}
		#endregion

		private void Instance_Error(object sender, ErrorEventArgs e)
		{
			if(_Messages==null)
				_Messages = new ArrayList();
			_Messages.Add(e.Message);
			ErrorMessages.DataSource = _Messages;
			ErrorMessages.DataBind();
			//this.Visible = true;
			ErrorMessages.Visible = true;
			ErrorsPanel.Update();
		}
	}
}
