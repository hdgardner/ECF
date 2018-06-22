using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.IO;
using Mediachase.Cms;
using System.Data;
using System.Threading;
using System.Text.RegularExpressions;

namespace Mediachase.Cms.ResourceHandler
{
    public class ResourceHandler : IHttpHandler
    {
        #region ResourceProvider
        private static IResourceProvider _resourceProvider;

        /// <summary>
        /// Gets or sets the resource provider.
        /// </summary>
        /// <value>The resource provider.</value>
        public static IResourceProvider ResourceProvider
        {
            get { return ResourceHandler._resourceProvider; }
            set { ResourceHandler._resourceProvider = value; }
        } 
        #endregion

        #region LanguageId
        private int languageId = -1;
        /// <summary>
        /// Gets the language id.
        /// </summary>
        /// <value>The language id.</value>
        public int LanguageId
        {
            get
            {
                using (IDataReader reader = Language.GetLangByName(Thread.CurrentThread.CurrentCulture.Name))
                {
                    if (reader.Read())
                    {
                        languageId = (int)reader["LangId"];
                    }
                    else
                    {
                        using (IDataReader reader2 = Language.GetAllLanguages())
                        {
                            while (reader2.Read())
                            {
                                if ((bool)reader2["IsDefault"])
                                {
                                    int langId = (int)reader2["LangId"];
                                    reader2.Close();
                                    reader.Close();
                                    return langId;
                                }
                            }

                            reader2.Close();
                        }
                    }

                    reader.Close();
                }
                return languageId;
                //return CMSContext.Current.LanguageId;
            }
        }
        #endregion

        /// <summary>
        /// Fills the context.
        /// </summary>
        /// <param name="absolutePath">The absolute path.</param>
        private void FillContext(string absolutePath)
        {
            CMSContext.Current.Outline = CMSContext.Current.GetQueryString(absolutePath);
            CMSContext.Current.Outline = Regex.Match(CMSContext.Current.Outline, "/[\\w]+(?<outline>.*)").Groups["outline"].Value;

            //GET PAGEID
            using (IDataReader reader = FileTreeItem.GetItemByOutlineAll(CMSContext.Current.Outline, CMSContext.Current.SiteId))
            {
                if (reader.Read())
                {
                    CMSContext.Current.PageId = (int)reader["PageId"];
                }

                reader.Close();
            }

            //SHOW  REQUESTED VERSION
            if (CMSContext.Current.QueryString["VersionId"] != null)
            {
                CMSContext.Current.VersionId = int.Parse(CMSContext.Current.QueryString["VersionId"].ToString());
            }
            //SHOW PUBLISHED VERSION
            else
            {
                //GET PUBLISHED VERSION
                int statusId = WorkflowStatus.GetLast(-1);
                using (IDataReader reader = PageVersion.GetByLangIdAndStatusId(CMSContext.Current.PageId, LanguageId, statusId))
                {
                    if (reader.Read())
                    {
                        CMSContext.Current.VersionId = (int)reader["VersionId"];
                    }

                    reader.Close();
                }
            }
        }

        #region IHttpHandler Members

        /// <summary>
        /// Gets a value indicating whether another request can use the <see cref="T:System.Web.IHttpHandler"/> instance.
        /// </summary>
        /// <value></value>
        /// <returns>true if the <see cref="T:System.Web.IHttpHandler"/> instance is reusable; otherwise, false.</returns>
        public bool IsReusable
        {
            get { return false; }
        }

        /// <summary>
        /// Enables processing of HTTP Web requests by a custom HttpHandler that implements the <see cref="T:System.Web.IHttpHandler"/> interface.
        /// </summary>
        /// <param name="context">An <see cref="T:System.Web.HttpContext"/> object that provides references to the intrinsic server objects (for example, Request, Response, Session, and Server) used to service HTTP requests.</param>
        public void ProcessRequest(HttpContext context)
        {
            if (context.Request.UrlReferrer != null)
            {
                FillContext(context.Request.UrlReferrer.PathAndQuery);
                if (ResourceProvider != null)
                    ResourceProvider.ProcessRequest(context);
            }
            else
            {
                context.Response.Write("");
            }
        }

        #endregion
    }
}
