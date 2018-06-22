using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Web;
using System.Web.Hosting;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Mediachase.Ibn.Core;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.Ibn.Web.UI.Layout
{
	public sealed class WorkspaceTemplateLoader
	{
		public const string ControlsDir = "Templates";
		public const string ConfigDir = "Configs";

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkspaceTemplateLoader"/> class.
        /// </summary>
		private WorkspaceTemplateLoader()
		{
		}

		// TODO:
        /// <summary>
        /// Gets a value indicating whether this instance has changed.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has changed; otherwise, <c>false</c>.
        /// </value>
		public static bool HasChanged
		{
			get
			{
				//return FileResolver.HasChanged;
				return true;
			}
		}

		#region Methods
        /// <summary>
        /// Loads all controls from given directory and its subdirectories.
        /// </summary>
        /// <param name="structureVirtualPath">The structure virtual path.</param>
        /// <returns></returns>
		public static WorkspaceTemplateInfo[] Load(string structureVirtualPath)
		{
			List<WorkspaceTemplateInfo> list = new List<WorkspaceTemplateInfo>();

			string structurePath = HostingEnvironment.MapPath(structureVirtualPath);
			IList<Mediachase.Ibn.Web.UI.WebControls.FileResolverItem> files = Mediachase.Ibn.Web.UI.WebControls.FileResolver.GetFilesSingleSelector(structurePath, ControlsDir + Path.DirectorySeparatorChar + "Configs", "*.xml", new string[] { });
			foreach (Mediachase.Ibn.Web.UI.WebControls.FileResolverItem file in files)
			{
				string controlDir = Path.GetDirectoryName(file.FilePath);

				if (!String.IsNullOrEmpty(controlDir))
				{
					string configsDir = Path.DirectorySeparatorChar + ConfigDir;
					string tempControlDirString = controlDir.EndsWith(Convert.ToString(Path.DirectorySeparatorChar)) ? controlDir.Substring(0, controlDir.Length - 1) : controlDir;
					if (controlDir.EndsWith(configsDir, true, CultureInfo.InvariantCulture))
						controlDir = controlDir.Substring(0, controlDir.LastIndexOf(configsDir));
				}

				WorkspaceTemplateInfo wti = McXmlSerializer.GetObjectFromFile<WorkspaceTemplateInfo>(file.FilePath);

				if (string.IsNullOrEmpty(wti.Uid))
					wti.Uid = Path.GetFileNameWithoutExtension(file.FilePath);

				//wti.Path = wti.Path; //MakeVirtualPath(structurePath, structureVirtualPath, controlDir, dci.Path);

				list.Add(wti);
			}

			return list.ToArray();
		}

        /// <summary>
        /// Makes the virtual path.
        /// </summary>
        /// <param name="structurePath">The structure path.</param>
        /// <param name="structureVirtualPath">The structure virtual path.</param>
        /// <param name="physicalDir">The physical dir.</param>
        /// <param name="relativePath">The relative path.</param>
        /// <returns></returns>
		private static string MakeVirtualPath(string structurePath, string structureVirtualPath, string physicalDir, string relativePath)
		{
			string virtualPath = relativePath;

			if (!string.IsNullOrEmpty(virtualPath))
			{
				virtualPath = string.Concat(physicalDir, Path.DirectorySeparatorChar, relativePath);
				virtualPath = virtualPath.Replace(structurePath, structureVirtualPath);
				virtualPath = virtualPath.Replace(Path.DirectorySeparatorChar, '/');
			}

			return virtualPath;
		}

		#endregion
	}
}
