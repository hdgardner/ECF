using System;
using System.Web;
using System.Reflection;

namespace Mediachase.Web.Console.Controls
{

	/// <summary>
	/// The DynamicListBoxResourceHandler takes care of script support for the controls in the assembly.
	/// It is not meant to be used by your code.
	/// </summary>
	public class DynamicListBoxResourceHandler : System.Web.IHttpHandler {
		
		#region IHttpHandler Members

		/// <summary>
		/// Processes the request by emitting script embedded in the assembly.
		/// </summary>
		/// <param name="context"></param>
		void IHttpHandler.ProcessRequest(System.Web.HttpContext context)
		{
			String scriptFile = context.Request.QueryString[resourceKey];
			context.Response.ContentType = "text/javascript";

			context.Response.Cache.SetExpires(DateTime.Now.AddMonths(1));
			context.Response.Cache.SetCacheability(HttpCacheability.Public);
			context.Response.Cache.SetValidUntilExpires(false);
			context.Response.Cache.VaryByParams[resourceKey] = true;
			context.Response.Cache.VaryByParams[assemblyIdKey] = true;

			context.Response.Write(GetScript(context.Cache, scriptFile));
		}

        /// <summary>
        /// Gets a value indicating whether another request can use the <see cref="T:System.Web.IHttpHandler"/> instance.
        /// </summary>
        /// <value></value>
        /// <returns>true if the <see cref="T:System.Web.IHttpHandler"/> instance is reusable; otherwise, false.</returns>
		bool IHttpHandler.IsReusable
		{
			get
			{
				return true;
			}
		}

		private static readonly String handlerName = "MetaBuilders_WebControls_DynamicListBoxResourceHandler.axd";
		private static readonly String resourceKey = "r";
		private static readonly String assemblyIdKey = "c";

		#endregion

		#region Script
        /// <summary>
        /// Registers the script.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="scriptKey">The script key.</param>
        /// <param name="scriptFile">The script file.</param>
		public static void RegisterScript( System.Web.UI.Page page, String scriptKey, String scriptFile ) {
			if (!page.ClientScript.IsClientScriptBlockRegistered(typeof(DynamicListBoxResourceHandler), scriptKey))
			{
				String script = "";
				//if ( IsRegistered ) {
				script = "<script language='javascript' type='text/javascript' src='" + page.ResolveUrl("~/scripts/" + scriptFile) + "'></script>";
				//} else {
				//	script = "<script language='javascript' type='text/javascript'>\r\n<!--\r\n" + GetScript(page.Cache, scriptFile) + "\r\n// -->\r\n</script>";
				//}
				page.ClientScript.RegisterClientScriptBlock(typeof(DynamicListBoxResourceHandler), scriptKey, script);
			}
		}

        /// <summary>
        /// Gets the script URL.
        /// </summary>
        /// <param name="scriptFile">The script file.</param>
        /// <returns></returns>
		private static String GetScriptUrl(String scriptFile)
		{
			Assembly a = typeof(DynamicListBoxResourceHandler).Assembly;
			String assemblyId = System.IO.File.GetCreationTime(a.Location).GetHashCode().ToString();
			return handlerName + "?" + assemblyIdKey + "=" + assemblyId + "&" + resourceKey + "=" + scriptFile;
		}

        /// <summary>
        /// Gets the script.
        /// </summary>
        /// <param name="cache">The cache.</param>
        /// <param name="scriptFile">The script file.</param>
        /// <returns></returns>
		private static String GetScript(System.Web.Caching.Cache cache, String scriptFile)
		{
			if (cache == null)
			{
				return GetScriptDirect(scriptFile);
			}

			String key = "MetaBuilders WebControls DynamicListBox Script " + scriptFile;
			if (cache[key] == null)
			{
				cache.Insert(key, GetScriptDirect(scriptFile));
			}
			return (String)cache[key];
		}

        /// <summary>
        /// Gets the script direct.
        /// </summary>
        /// <param name="scriptFile">The script file.</param>
        /// <returns></returns>
		private static String GetScriptDirect(String scriptFile)
		{
			if (scriptFile == "DualList.js" || scriptFile == "DynamicListBox.js")
			{
				using (System.IO.StreamReader reader = new System.IO.StreamReader(typeof(DynamicListBoxResourceHandler).Assembly.GetManifestResourceStream(typeof(DynamicListBoxResourceHandler), scriptFile)))
				{
					return reader.ReadToEnd();
				}
			}
			return "";
		}
		#endregion

		#region Registration
        /// <summary>
        /// Gets a value indicating whether this instance is registered.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is registered; otherwise, <c>false</c>.
        /// </value>
		private static Boolean IsRegistered
		{
			get
			{
				HttpContext context = HttpContext.Current;
				if (context == null)
				{
					return DetermineIsRegistered();
				}
				String cacheKey = "IHttpHandlerFactory Installed " + handlerName;
				if (context.Cache[cacheKey] == null)
				{
					context.Cache.Insert(cacheKey, DetermineIsRegistered());
				}
				return (Boolean)context.Cache[cacheKey];
			}
		}

        /// <summary>
        /// Determines the is registered.
        /// </summary>
        /// <returns></returns>
		private static Boolean DetermineIsRegistered()
		{
			Object handlerMap = System.Web.Configuration.WebConfigurationManager.GetWebApplicationSection("system.web/httpHandlers");
			if (handlerMap == null)
			{
				return false;
			}

			MethodInfo findMapping = handlerMap.GetType().GetMethod("FindMapping", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (findMapping == null)
			{
				return false;
			}

			Object handler = findMapping.Invoke(handlerMap, new Object[] { "GET", handlerName });
			if (handler == null)
			{
				return false;
			}

			PropertyInfo handlerPathProperty = handler.GetType().GetProperty("Path", BindingFlags.NonPublic | BindingFlags.Instance);
			if (handlerPathProperty == null)
			{
				return false;
			}

			String handlerPath = handlerPathProperty.GetValue(handler, null) as String;
			if (handlerPath == null || handlerPath != handlerName)
			{
				return false;
			}

			return true;
		}
		#endregion
	}
}
