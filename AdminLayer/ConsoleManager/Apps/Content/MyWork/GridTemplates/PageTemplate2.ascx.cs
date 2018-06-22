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
using Mediachase.Web.Console.Interfaces;
using Mediachase.Commerce.Profile;
using Mediachase.Cms;
using System.Text.RegularExpressions;

namespace Mediachase.Commerce.Manager.Apps.Content.MyWork.GridTemplates
{
    public partial class PageTemplate2 : System.Web.UI.UserControl, IEcfListViewTemplate
    {
        private object _DataItem;

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Gets the preview URL.
        /// </summary>
        /// <param name="siteId">The site id.</param>
        /// <param name="PageId">The page id.</param>
        /// <param name="versionId">The version id.</param>
        /// <param name="WithUserId">if set to <c>true</c> [with user id].</param>
        /// <returns></returns>
        protected string GetPreviewUrl(Guid siteId, int PageId, int versionId, bool WithUserId)
        {
            string url = GlobalVariable.GetVariable("url", siteId);
            string path = GetPageUrlInternal(PageId, WithUserId);
            return String.Format("javascript:CSManagementClient.OpenExternal('{0}?_mode=edit&VersionId={1}');", url + path, versionId.ToString());
        }

        /// <summary>
        /// Gets the page URL.
        /// </summary>
        /// <param name="PageId">The page id.</param>
        /// <param name="WithUserId">if set to <c>true</c> [with user id].</param>
        /// <returns></returns>
        protected string GetPageUrl(Guid siteId, int PageId, bool WithUserId)
        {
            string url = GlobalVariable.GetVariable("url", siteId);
            string path = GetPageUrlInternal(PageId, WithUserId);
            return String.Format("{0}{1}", url, path);
        }

        /// <summary>
        /// Gets the page URL internal.
        /// </summary>
        /// <param name="PageId">The page id.</param>
        /// <param name="WithUserId">if set to <c>true</c> [with user id].</param>
        /// <returns></returns>
        protected string GetPageUrlInternal(int PageId, bool WithUserId)
        {
            string path = String.Empty;
            using (System.Data.IDataReader reader = Mediachase.Cms.FileTreeItem.GetItemById(PageId))
            {
                if (reader.Read())
                {
                    if ((bool)reader["IsFolder"])
                        path += "/" + reader["Name"].ToString() + "/";
                    else
                        path += reader["Outline"].ToString();
                }
                reader.Close();
            }
            if (WithUserId)
            {
                //path += "?_mode=edit";
            }
            else
            {
                //trunc path
                if (path.Length > 100)
                {
                    string begin = string.Empty;
                    string end = string.Empty;
                    if (Regex.IsMatch(path, "/[\\w\\d]+\\u002Easpx"))
                    {
                        end = Regex.Match(path, "/[\\w\\d]+\\u002Easpx").Value;
                    }
                    if (Regex.IsMatch(path, "^/[\\w\\d]*"))
                    {
                        end = Regex.Match(path, "^/[\\w\\d]*").Value;
                    }
                    path = begin + "..." + end;
                }
            }
            return path.Trim();
        }

        #region IEcfListViewTemplate Members

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
    }
}