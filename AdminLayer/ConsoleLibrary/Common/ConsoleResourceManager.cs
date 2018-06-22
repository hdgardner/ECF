using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Resources;

namespace Mediachase.Web.Console.Common
{
	public class ConsoleResourceManager
	{
        /// <summary>
        /// Gets the string.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
		public string GetString(string name)
		{
			string str = "";
			try
			{
				str = System.Web.HttpContext.GetGlobalResourceObject("CommerceManager", name, System.Threading.Thread.CurrentThread.CurrentUICulture).ToString();
			}
			catch
			{
				try
				{
					object obj = System.Web.HttpContext.GetGlobalResourceObject("CommerceManager", name);
					if (obj == null)
						throw new MissingManifestResourceException(String.Format("Resource for key {0} was not found.", name), null);

					str = obj.ToString();
				}
				catch (System.Resources.MissingManifestResourceException ex)
				{

					throw new MissingManifestResourceException(String.Format("Resource for key {0} was not found.", name), ex);
				}
			}
			return str;
		}
	}
}
