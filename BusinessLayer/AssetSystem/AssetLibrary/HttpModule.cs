using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Mediachase.Ibn.Library
{
    class AssetHttpModule: IHttpModule
    {
        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        public AssetHttpModule()
        {
        }

        /// <summary>
        /// Disposes of any resources used.
        /// </summary>
        public void Dispose()
        {
            // No resources were used.
        }

        /// <summary>
        /// Initializes the module by hooking the application's BeginRequest event if indicated by the config settings.
        /// </summary>
        /// <param name="application">The HttpApplication this module is bound to.</param>
        public void Init(HttpApplication application)
        {
            application.BeginRequest += (new EventHandler(this.Application_BeginRequest));
        }


        /// <summary>
        /// Handles the BeginRequest event of the Application control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void Application_BeginRequest(object sender, EventArgs e)
        {
            string mdCsKey = System.Configuration.ConfigurationManager.AppSettings["MetaDataConnection"];
            if (System.Configuration.ConfigurationManager.ConnectionStrings[mdCsKey] != null)
                Mediachase.Ibn.Data.DataContext.Current = new Mediachase.Ibn.Data.DataContext(System.Configuration.ConfigurationManager.ConnectionStrings[mdCsKey].ConnectionString);

            Mediachase.Ibn.Data.Meta.Management.MetaClassManager metaClassManager = Mediachase.Ibn.Data.DataContext.Current.MetaModel;						
        }

    }
}
