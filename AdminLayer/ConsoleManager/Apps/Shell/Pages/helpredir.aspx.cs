using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mediachase.Web.Console.Config;
using Mediachase.Web.Console;

namespace Mediachase.Commerce.Manager
{
    public partial class helpredir : System.Web.UI.Page
    {
        /// <summary>
        /// Gets the view id.
        /// </summary>
        /// <value>The view id.</value>
        public string ViewId
        {
            get
            {
                if (!String.IsNullOrEmpty(this.Request.QueryString["_v"]))
                    return this.Request.QueryString["_v"].ToString();

                return String.Empty;
            }
        }

        /// <summary>
        /// Gets the app id.
        /// </summary>
        /// <value>The app id.</value>
        public string AppId
        {
            get
            {
                if (!String.IsNullOrEmpty(this.Request.QueryString["_a"]))
                    return this.Request.QueryString["_a"].ToString();

                return String.Empty;
            }
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            AdminView view = ManagementContext.Current.FindView(AppId, ViewId);

            string prefix = String.Empty;
            
            ModuleSetting setting = FindModuleSetting(view.Module, "HelpPrefix");

            if(setting != null)
                prefix = setting.Value;

            if (view.Attributes.ContainsKey("help"))
                Response.Redirect(String.Format("http://docs.mediachase.com/doku.php?id=ecf:50:cmenduser:using:{0}:{1}", prefix, view.Attributes["help"]));
            else
            {
                if(String.IsNullOrEmpty(prefix))
                    Response.Redirect(String.Format("http://docs.mediachase.com/doku.php?id=ecf:50:cmenduser:using:intro"));
                else
                    Response.Redirect(String.Format("http://docs.mediachase.com/doku.php?id=ecf:50:cmenduser:using:{0}:{1}", prefix, view.ViewId));
            }
        }

        /// <summary>
        /// Finds the module setting.
        /// </summary>
        /// <param name="module">The module.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        private ModuleSetting FindModuleSetting(ModuleConfig module, string name)
        {
            if (module != null && module.Settings != null)
            {
                foreach (ModuleSetting setting in module.Settings)
                {
                    if (setting.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                        return setting;
                }
            }

            return null;
        }
    }
}
