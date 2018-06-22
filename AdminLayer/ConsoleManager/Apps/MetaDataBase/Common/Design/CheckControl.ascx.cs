using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace Mediachase.Ibn.Web.UI
{
	public partial class CheckControl : System.Web.UI.UserControl
	{
		#region var
		protected string[] _urls = { "../../Images/none.gif", "../../Images/allow.gif", "../../Images/forbid.gif"};
		private int defaultValue = 1;
		#endregion

		#region Text
        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
		public string Text
		{
			get
			{
				return text.Text;
			}
			set
			{
				text.Text = value;
			}
		}
		#endregion

		#region Value
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
		public int Value
		{
			get
			{
				if (curValue.Value.Length > 0)
					return Convert.ToInt32(curValue.Value);
				return -1;
			}
			set
			{
				if (value < 1 || value > 3) 
					return;
				curValue.Value = value.ToString();
			}
		}
		#endregion

		#region Page_Load
        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, EventArgs e)
		{
			_urls[0] = CHelper.GetAbsolutePath(_urls[0]);
			_urls[1] = CHelper.GetAbsolutePath(_urls[1]);
			_urls[2] = CHelper.GetAbsolutePath(_urls[2]);

			if (!IsPostBack && Value == -1)
				Value = defaultValue;

			if (!Page.ClientScript.IsClientScriptBlockRegistered(this.GetType(), "CheckControlclientScript"))
			{
				Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "CheckControlclientScript", GetClientScript());
			}

			
		}
		#endregion

        /// <summary>
        /// Handles the PreRender event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_PreRender(object sender, EventArgs e)
		{
			if (curValue.Value.Length <= 0) 
				return;
			int _img = Convert.ToInt32(curValue.Value) - 1;
			curImg.ImageUrl = _urls[_img];
			tblMain.Attributes.Add("onclick", String.Format("OnClickCheckControl(\"{0}\",\"{1}\")", curValue.ClientID, curImg.ClientID));
		}

		#region GetClientScript
        /// <summary>
        /// Gets the client script.
        /// </summary>
        /// <returns></returns>
		private string GetClientScript()
		{
			string script_fmt = 
				"<script language=\"javascript\">" +
				"\n var CheckControlImages = new Array();" +
				"\n var a = new Image();" +
				"\n a.src = '" + _urls[0] + "';" +
				"\n CheckControlImages[0]= a.src;" +

				"\n a = new Image();" +
				"\n a.src = '" + _urls[1] + "';" +
				"\n CheckControlImages[1]= a.src;" +

				"\n a = new Image();" +
				"\n a.src = '" + _urls[2] + "';" +
				"\n CheckControlImages[2]= a.src;" +

				"\n function OnClickCheckControl(curValueID,curImgID)" +
				"\n {" +
				"\n 	var val = document.getElementById(curValueID);" +
					"\n var img = document.getElementById(curImgID);" +
					"\n if(val == null || img == null) return;" +
					"\n if(val.value == '') val.value = '1';" +
					"\n switch(val.value)" +
					"\n {" +
						"\n case '1':" +
							"\n val.value = '2'" +
							"\n img.src = CheckControlImages[1];" +
							"\n break;" +
						"\n case '2':" +
							"\n val.value = '3'" +
							"\n img.src = CheckControlImages[2];" +
							"\n break;" +
						"\n case '3':" +
							"\n val.value = '1'" +
							"\n img.src = CheckControlImages[0];" +
							"\n break;" +
					"\n }" +
				"\n }" +
				"\n </script>\n ";
			return script_fmt;
		}
		#endregion
	}
}