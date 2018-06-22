using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web.Caching;
using System.Collections.Specialized;

namespace Mediachase.Cms.Controls
{
	#region public class FileResolverItem
	public class FileResolverItem
	{
		private string _key1;
		private long _key2;
		private string _filePath;
		private string _extensionName;

        /// <summary>
        /// Gets or sets the file path.
        /// </summary>
        /// <value>The file path.</value>
		public string FilePath
		{
			get
			{
				return _filePath;
			}
			set
			{
				_filePath = value;
			}
		}

        /// <summary>
        /// Gets or sets the name of the extension.
        /// </summary>
        /// <value>The name of the extension.</value>
		public string ExtensionName
		{
			get
			{
				return _extensionName;
			}
			set
			{
				_extensionName = value;
			}
		}

        /// <summary>
        /// Gets or sets the key1.
        /// </summary>
        /// <value>The key1.</value>
		public string Key1
		{
			get
			{
				return _key1;
			}
			set
			{
				_key1 = value;
			}
		}

        /// <summary>
        /// Gets or sets the key2.
        /// </summary>
        /// <value>The key2.</value>
		public long Key2
		{
			get
			{
				return _key2;
			}
			set
			{
				_key2 = value;
			}
		}

        /// <summary>
        /// Compares the specified x.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns></returns>
		internal static int Compare(FileResolverItem x, FileResolverItem y)
		{
			int ret = 0;

			if (x == null)
			{
				if (y != null)
					ret = -1;
			}
			else
			{
				if (y == null)
					ret = 1;
				else
				{
					ret = string.Compare(x._key1, y._key1, true, CultureInfo.InvariantCulture);
					if (ret == 0)
						ret = x._key2.CompareTo(y._key2);
				}
			}
			return ret;
		}
	}
	#endregion

	#region public class FileResolver
	public sealed class FileResolver
	{
		public const string NameAny = "@";
		public const char NameSplitter = '.';
		public const string ExtensionsDir = "Extensions";
		public const string BaseDir = "Base";
        public const string UserDir = "User";
        private static List<CacheDependency> _Dependencies = new List<CacheDependency>();

		//private static Dictionary<string, IList<FileResolverItem>> _collections = new Dictionary<string, IList<FileResolverItem>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="FileResolver"/> class.
        /// </summary>
		private FileResolver()
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
                if (_Dependencies == null)
                {
                    return true;
                }
                else
                {
                    foreach (CacheDependency dep in _Dependencies)
                    {
                        if (dep.HasChanged)
                            return true;
                    }
                }

                return false;
            }
        }

		#region GetFiles(...)
        /// <summary>
        /// Gets the files.
        /// </summary>
        /// <param name="structureDir">The structure dir.</param>
        /// <param name="structureName">Name of the structure.</param>
        /// <param name="searchPattern">The search pattern.</param>
        /// <param name="selector">The selector.</param>
        /// <returns></returns>
		public static IList<FileResolverItem> GetFiles(string structureDir, string structureName, string searchPattern, string[] selector)
		{
			// 1. Collect all files in given folder conforming to search pattern
			IList<FileResolverItem> files;

			string key = string.Format(CultureInfo.InvariantCulture, "{0}:{1}:{2}", structureDir, structureName, searchPattern);
			//lock (_collections)
			{
                /* no need to cache here since we already cache on the higher level
                 * if (_collections.ContainsKey(key) && !IsChanged)
					files = _collections[key];
				else
                 * */
				{
                    // Clear old dependencies
                    _Dependencies.Clear();

					List<FileResolverItem> temporaryList = new List<FileResolverItem>();

                    // Get System controls (base)
					GetFiles2(temporaryList, structureName, searchPattern, string.Concat(structureDir, Path.DirectorySeparatorChar, BaseDir), false);

                    // Get Custom controls (user)
                    string userDir = string.Concat(structureDir, Path.DirectorySeparatorChar, UserDir);
                    if (Directory.Exists(userDir))
                    {
                        foreach (string path in Directory.GetDirectories(userDir))
                            GetFiles2(temporaryList, structureName, searchPattern, path, false);
                    }

					string extensionsDir = string.Concat(structureDir, Path.DirectorySeparatorChar, ExtensionsDir);
					if (Directory.Exists(extensionsDir))
					{
						foreach (string path in Directory.GetDirectories(extensionsDir))
							GetFiles2(temporaryList, structureName, searchPattern, path, true);
					}

					temporaryList.Sort(FileResolverItem.Compare);

					//_collections.Add(key, temporaryList);
					files = temporaryList;
				}
			}

			// 2. Apply filter using selector
			List<FileResolverItem> list = new List<FileResolverItem>();
			if (selector == null || selector.Length == 0)
			{
				list.AddRange(files);
			}
			else
			{
				foreach (FileResolverItem file in files)
				{
					bool add = true;
					string name = Path.GetFileNameWithoutExtension(file.FilePath);
					string[] parts = name.Split(NameSplitter);
					int count = Math.Min(parts.Length, selector.Length);

					for (int i = 0; i < count; i++)
					{
						if (parts[i] != NameAny && 0 != string.Compare(parts[i], selector[i], true, CultureInfo.InvariantCulture))
						{
							add = false;
							break;
						}
					}

					if (add)
						list.Add(file);
				}
			}

			return list;
		}
		#endregion

		#region GetFiles2(...)
        /// <summary>
        /// Gets the files2.
        /// </summary>
        /// <param name="files">The files.</param>
        /// <param name="structureName">Name of the structure.</param>
        /// <param name="searchPattern">The search pattern.</param>
        /// <param name="path">The path.</param>
        /// <param name="isExtension">if set to <c>true</c> [is extension].</param>
		private static void GetFiles2(ICollection<FileResolverItem> files, string structureName, string searchPattern, string path, bool isExtension)
		{
			string dir = string.Concat(path, Path.DirectorySeparatorChar, structureName);
			if (Directory.Exists(dir))
			{
                // Add dependencies here
                _Dependencies.Add(new CacheDependency(dir));

				foreach (string filePath in Directory.GetFiles(dir, searchPattern))
				{
					FileResolverItem f = new FileResolverItem();

					f.FilePath = filePath;
					f.ExtensionName = isExtension ? Path.GetFileName(path) : string.Empty;
					f.Key1 = Path.GetFileNameWithoutExtension(filePath);
					f.Key2 = isExtension ? File.GetCreationTime(filePath).Ticks : 0;

					files.Add(f);
				}
			}
		}
		#endregion
	}
	#endregion
}
