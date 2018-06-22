using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Mediachase.Cms;
using Mediachase.Cms.Pages;

namespace Mediachase.Cms.Web.UI.Controls
{
	/// <summary>
	/// Summary description for StaticWrapper
	/// </summary>
	public class BaseStaticWrapper : WebControl
	{
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseStaticWrapper"/> class.
        /// </summary>
		public BaseStaticWrapper()
		{
		}

		#region prop: CtrlSettings
		private ControlSettings cSettings = null;
        /// <summary>
        /// Gets the CTRL settings.
        /// </summary>
        /// <value>The CTRL settings.</value>
		public ControlSettings CtrlSettings
		{
			get
			{
				return cSettings;
			}
		} 
		#endregion


        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load"></see> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"></see> object that contains the event data.</param>
		protected override void OnLoad(EventArgs e)
		{
			this.Attributes.Add("ActionSet", "NoneMenu");
			this.Style.Add("position", "relative");
			this.Style.Add("z-index", "1");

			base.OnLoad(e);
		}

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.PreRender"></see> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"></see> object that contains the event data.</param>
		protected override void OnPreRender(EventArgs e)
		{
			if (PageDocument.Current == null)
				throw new ArgumentNullException("PageDocument.Current");

			if (PageDocument.Current.StaticNode != null)
			{
				cSettings = new ControlSettings();

				if (PageDocument.Current.StaticNode.GetSettings(this.ID) != null)
				{
					cSettings = PageDocument.Current.StaticNode.GetSettings(this.ID);
				}
				else
				{
					cSettings = new ControlSettings();
					cSettings.IsModified = true;
					Param param = new Param();
					cSettings.Params = param;

					PageDocument.Current.StaticNode.Controls.Add(this.ID, cSettings);
				}

			}

			base.OnPreRender(e);

			//hardcoded for inlineeditor wrapper
			

		}

        /// <summary>
        /// Renders the control to the specified HTML writer.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Web.UI.HtmlTextWriter"></see> object that receives the control content.</param>
		protected override void Render(HtmlTextWriter writer)
		{
			//writer.WriteBeginTag("div");
			//writer.AddAttribute("ActionSet", "NoneMenu");

			//writer.AddStyleAttribute("position", "relative");
			//writer.AddStyleAttribute("z-index", "1");

			base.Render(writer);

			//writer.WriteEndTag("div");
		}

	}
}