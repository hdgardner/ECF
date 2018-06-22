using System;
using System.IO;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Mediachase.MetaDataPlus;
using Mediachase.MetaDataPlus.Configurator;
using Mediachase.Commerce.Catalog;

namespace Mediachase.Commerce.Manager.Core.MetaData.MetaControls
{
	/// <summary>
	/// Summary description for ShowImage.
	/// </summary>
	public partial class ShowImage : System.Web.UI.Page
	{
        /// <summary>
        /// Gets the object id.
        /// </summary>
        /// <value>The object id.</value>
		public int ObjectId 
		{ 
			get
			{
				if (base.Request["objectid"] != null)
					return int.Parse(base.Request["objectid"].ToString());
				else
					return 0; 
			} 
		}

        /// <summary>
        /// Gets the class id.
        /// </summary>
        /// <value>The class id.</value>
		public int ClassId 
		{ 
			get
			{
				if (base.Request["classid"] != null)
					return int.Parse(base.Request["classid"].ToString());
				else
					return 0; 
			} 
		}

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
		public string Name 
		{ 
			get
			{
				if (base.Request["name"] != null)
					return base.Request["name"].ToString();
				else
					return ""; 
			} 
		}

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (!base.IsPostBack && (this.ClassId > 0))
			{
                MetaObject o = MetaObject.Load(CatalogContext.MetaDataContext, ObjectId, ClassId);
				if (o != null)
				{
					MetaFile metafile = (MetaFile)o[Name];
					if (metafile != null && metafile.Buffer != null && metafile.Buffer.Length > 0)
					{
						base.Response.Clear();
						Response.AddHeader("content-disposition", string.Format("attachment; filename=\"{0}\"", Server.UrlEncode(metafile.Name)));
						Response.ContentType = metafile.ContentType + "; name=\"" + Server.UrlEncode(metafile.Name) + "\"";
						//base.Response.ContentType = metafile.ContentType;
						base.Response.BinaryWrite(metafile.Buffer);
						//base.Response.Flush(); // seem to cause issue on some systems
						base.Response.End();
					}
					else
						this.Visible = false;
				}
				else
					this.Visible = false;
			}
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
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
		}
		#endregion
	}
}
