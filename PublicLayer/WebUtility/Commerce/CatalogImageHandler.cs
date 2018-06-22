using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Mediachase.MetaDataPlus;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Shared;
using Mediachase.MetaDataPlus.Configurator;
using Mediachase.Commerce.Catalog.Dto;

namespace Mediachase.Cms.WebUtility.Commerce
{
    public class CatalogImageHandler : IHttpHandler
    {
        #region IHttpHandler Members

        /// <summary>
        /// Gets a value indicating whether another request can use the <see cref="T:System.Web.IHttpHandler"/> instance.
        /// </summary>
        /// <value></value>
        /// <returns>true if the <see cref="T:System.Web.IHttpHandler"/> instance is reusable; otherwise, false.
        /// </returns>
        public bool IsReusable
        {
            get { return false; }
        }


        /// <summary>
        /// _s the cached version is okay.
        /// </summary>
        /// <param name="Request">The request.</param>
        /// <returns></returns>
        private bool _CachedVersionIsOkay(HttpRequest Request)
        {
            string ifModified = Request.Headers["If-Modified-Since"];

            if (ifModified != null)
            {
				try 
				{
	                DateTime lastModified = DateTime.Parse(ifModified);
		            return DateTime.Now.AddHours(-12) < lastModified;
                }
                catch {}
            }

            return false;
        }

        /// <summary>
        /// Enables processing of HTTP Web requests by a custom HttpHandler that implements the <see cref="T:System.Web.IHttpHandler"/> interface.
        /// </summary>
        /// <param name="context">An <see cref="T:System.Web.HttpContext"/> object that provides references to the intrinsic server objects (for example, Request, Response, Session, and Server) used to service HTTP requests.</param>
        public void ProcessRequest(HttpContext context)
        {
            if (_CachedVersionIsOkay(context.Request))
            {
                context.Response.StatusCode = 304;
                context.Response.SuppressContent = true;
                return;
            }

            string file = context.Request.FilePath;
            file = file.Substring(file.LastIndexOf("/") + 1);
            file = file.Substring(0, file.IndexOf("."));
            string[] fileParams = file.Split(new char[] { '-' });

            if (fileParams.Length < 3)
                return;

            int metaClassId = Int32.Parse(fileParams[0]);
            int objectId = Int32.Parse(fileParams[1]);
            string metaFieldName = fileParams[2];

            bool thumbNail = false;
            if (fileParams.Length > 3)
            {
                if (fileParams[3] == "thumb")
                    thumbNail = true;
            }

            MetaObject o = MetaObject.Load(CatalogContext.MetaDataContext, objectId, metaClassId);
            if (o != null)
            {
                MetaField mf = MetaField.Load(CatalogContext.MetaDataContext, metaFieldName);

                if(mf == null)
                    return;

                MetaFile metafile = (MetaFile)o[mf];
                if (metafile != null && metafile.Buffer != null && metafile.Buffer.Length > 0)
                {
                    context.Response.Cache.SetExpires(DateTime.Now.Add(new TimeSpan(0, 1, 0)));
                    context.Response.Cache.SetCacheability(HttpCacheability.Public);
                    context.Response.Cache.SetLastModified(DateTime.Now);


                    context.Response.AddHeader("content-disposition", "inline; filename=" + metafile.Name);
                    context.Response.ContentType = metafile.ContentType;

                    if (!thumbNail)
                    {
                        context.Response.BinaryWrite(metafile.Buffer);
                    }
                    else // create thumbnail
                    {
                        if (!metafile.ContentType.Contains("gif")) // dont create thumbnails for gifs
                        {
                            int thumbWidth = 0;
                            int thumbHeight = 0;
                            bool thumbStretch = false;

                            object var = mf.Attributes["CreateThumbnail"];

                            if (var != null && Boolean.Parse(var.ToString()))
                            {
                                var = mf.Attributes["ThumbnailHeight"];
                                if (var != null)
                                    thumbHeight = Int32.Parse(var.ToString());

                                var = mf.Attributes["ThumbnailWidth"];
                                if (var != null)
                                    thumbWidth = Int32.Parse(var.ToString());

                                var = mf.Attributes["StretchThumbnail"];
                                if (var != null && Boolean.Parse(var.ToString()))
                                    thumbStretch = true;
                            }

                            byte[] data = ImageGenerator.CreateImageThumbnail(metafile.Buffer, metafile.ContentType, thumbHeight, thumbWidth, thumbStretch);
                            context.Response.BinaryWrite(data);
                        }
                        else
                        {
                            context.Response.BinaryWrite(metafile.Buffer);
                        }
                    }
                }
                else
                {
                    context.Response.Cache.SetExpires(DateTime.Now.Add(new TimeSpan(1, 0, 0)));
                    context.Response.Cache.SetCacheability(HttpCacheability.Public);
                    context.Response.Cache.SetValidUntilExpires(false);

                    context.Response.AddHeader("content-disposition", "inline; filename=" + "nopic.png");
                    context.Response.ContentType = "image/png";
                    context.Response.WriteFile(context.Server.MapPath("~/images/nopic.png"));
                }
            }
        }
        #endregion
    }
}
