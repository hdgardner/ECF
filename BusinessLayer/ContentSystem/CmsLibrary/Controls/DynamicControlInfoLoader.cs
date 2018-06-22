using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web.Hosting;
using Mediachase.Data;


namespace Mediachase.Cms.Controls
{
	public sealed class DynamicControlInfoLoader
	{
		public const string ControlsDir = "Controls";
		public const string ConfigDir = "Configs";

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicControlInfoLoader"/> class.
        /// </summary>
		private DynamicControlInfoLoader()
		{
		}

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
                return FileResolver.HasChanged;
            }
        }

		#region Methods
        /// <summary>
        /// Loads all controls from given directory and its subdirectories.
        /// </summary>
        /// <param name="structureVirtualPath">The structure virtual path.</param>
        /// <returns></returns>
		public static DynamicControlInfo[] Load(string structureVirtualPath)
		{
			List<DynamicControlInfo> list = new List<DynamicControlInfo>();

			string structurePath = HostingEnvironment.MapPath(structureVirtualPath);
			IList<FileResolverItem> files = FileResolver.GetFiles(structurePath, ControlsDir + Path.DirectorySeparatorChar + "Configs", "*.config", null);
			foreach (FileResolverItem file in files)
			{
				string controlDir = Path.GetDirectoryName(file.FilePath);

				if (!String.IsNullOrEmpty(controlDir))
				{
					string configsDir = Path.DirectorySeparatorChar + ConfigDir;
					string tempControlDirString = controlDir.EndsWith(Convert.ToString(Path.DirectorySeparatorChar)) ? controlDir.Substring(0, controlDir.Length - 1) : controlDir;
					if (controlDir.EndsWith(configsDir, true, CultureInfo.InvariantCulture))
						controlDir = controlDir.Substring(0, controlDir.LastIndexOf(configsDir));
				}

				DynamicControlInfo dci = McXmlSerializer.GetObjectFromFile<DynamicControlInfo>(file.FilePath);

				if (string.IsNullOrEmpty(dci.Uid))
					dci.Uid = Path.GetFileNameWithoutExtension(file.FilePath);
				dci.AdapterPath = MakeVirtualPath(structurePath, structureVirtualPath, controlDir, dci.AdapterPath);
				dci.IconPath = MakeVirtualPath(structurePath, structureVirtualPath, controlDir, dci.IconPath);
				dci.Path = MakeVirtualPath(structurePath, structureVirtualPath, controlDir, dci.Path);
				dci.PropertyPagePath = MakeVirtualPath(structurePath, structureVirtualPath, controlDir, dci.PropertyPagePath);

				list.Add(dci);
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
