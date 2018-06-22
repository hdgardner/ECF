using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using Mediachase.Web.Console.Common;
using System.Web;

namespace Mediachase.Web.Console.BaseClasses
{
	public class BasePage : Page
	{
        /// <summary>
        /// Gets the RM.
        /// </summary>
        /// <value>The RM.</value>
		public ConsoleResourceManager RM
		{
			get
			{
				return new ConsoleResourceManager();
			}
		}
        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache); 
            Response.Cache.SetExpires(DateTime.Now.AddDays(-1)); 
            Response.Cache.SetNoStore();
            base.OnLoad(e);
        }
	}
}
