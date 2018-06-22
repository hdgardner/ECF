using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Mediachase.Commerce.Profile;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Interfaces;
using Mediachase.Web.Console;

namespace Mediachase.Commerce.Manager.Apps.Order.GridTemplates
{
    public partial class OrderLinkTemplate : BaseUserControl, IEcfListViewTemplate
    {
        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (ProfileConfiguration.Instance.EnablePermissions)
            {
                if (!ProfileContext.Current.CheckPermission("order:mng:edit"))
                    HyperLink1.Enabled = false;
            }
        }

        #region IEcfListViewTemplate Members
        private object _DataItem;

        /// <summary>
        /// Gets or sets the data item.
        /// </summary>
        /// <value>The data item.</value>
        public object DataItem
        {
            get
            {
                return _DataItem;
            }
            set
            {
                _DataItem = value;
            }
        }

        #endregion

        /// <summary>
        /// Gets the name of the view.
        /// </summary>
        /// <returns></returns>
        protected string GetViewName()
        {
            string cacheKey = ConsoleCache.CreateCacheKey("order-viewname", DataItem.GetType().Name);

            string className = String.Empty;

            // check cache first
            object cachedObject = ConsoleCache.Get(cacheKey);

            if (cachedObject != null)
            {
                className = cachedObject.ToString();
            }

            // Load the object
            if (String.IsNullOrEmpty(className))
            {
                className = (string)DataBinder.Eval(DataItem, "MetaClass.Name");

                // Insert to the cache collection
                ConsoleCache.Insert(cacheKey, className, new TimeSpan(0, 0, 5));
            }

            return className;
        }
    }
}