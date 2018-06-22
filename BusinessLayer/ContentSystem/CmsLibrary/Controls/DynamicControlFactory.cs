using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Collections;
using System.Globalization;

using Mediachase.Data;


namespace Mediachase.Cms.Controls
{
	public static class DynamicControlFactory
	{
		private static ArrayList _controls;

		private static string _controlsFolderPath = string.Empty;

		/// <summary>
		/// Gets or sets the controls folder path.
		/// </summary>
		/// <value>The controls folder path.</value>
		public static string ControlsFolderPath
		{
			get { return _controlsFolderPath; }
			set 
			{ 
				_controlsFolderPath = value; 

				lock (typeof(DynamicControlFactory))
				{
					_controls = null;
				}
			}
		}

        /// <summary>
        /// Initializes this instance.
        /// </summary>
		private static void Initialize()
		{
            if (_controls == null || DynamicControlInfoLoader.HasChanged)
			{
				lock (typeof(DynamicControlFactory))
				{
                    if (_controls == null || DynamicControlInfoLoader.HasChanged)
					{
						_controls = ArrayList.Synchronized(new ArrayList());

						_controls.AddRange(DynamicControlInfoLoader.Load(ControlsFolderPath));
					}
				}
			}
		}

		/// <summary>
		/// Creates the specified page.
		/// </summary>
		/// <param name="page">The page.</param>
		/// <param name="controlUid">The control uid.</param>
		/// <returns></returns>
		public static Control Create(TemplateControl page, string controlUid)
		{
			if (page == null)
				throw new ArgumentNullException("page");

			if (controlUid == null)
				throw new ArgumentNullException("controlUid");

			Initialize();
			Control retVal = null;

			// Load Control Info
			DynamicControlInfo info = GetControlInfo(controlUid);
			if (info == null)
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Invalid control Uid '{0}'", controlUid), "controlUid");

			// Try Load Control
			if (!string.IsNullOrEmpty(info.Path))
				retVal = page.LoadControl(CreateControlVirtualPath(info.Path));
			else if (!string.IsNullOrEmpty(info.Type))
				retVal = (Control)AssemblyUtil.LoadObject(info.Type);

			// Try Load Adapter
			if (retVal!=null && (!string.IsNullOrEmpty(info.AdapterPath) || !string.IsNullOrEmpty(info.AdapterType)))
			{
				Control adapter = null;

				if (!string.IsNullOrEmpty(info.Path))
					adapter = page.LoadControl(CreateControlVirtualPath(info.AdapterPath));
				else if (!string.IsNullOrEmpty(info.Type))
					adapter = (Control)AssemblyUtil.LoadObject(info.AdapterType);

				if (adapter != null)
				{
					// Add Control to adapter
					adapter.Controls.Add(retVal);

					// Return adapter
					retVal = adapter;
				}
			}

			return retVal;
		}

		/// <summary>
		/// Creates the control virtual path.
		/// </summary>
		/// <param name="controlPath">The control path.</param>
		/// <returns></returns>
		private static string CreateControlVirtualPath(string controlPath)
		{
			return controlPath;
		}

		/// <summary>
		/// Gets the control info.
		/// </summary>
		/// <param name="controlUid">The control uid.</param>
		/// <returns></returns>
		public static DynamicControlInfo GetControlInfo(string controlUid)
		{
			if (controlUid == null)
				throw new ArgumentNullException("controlUid");

			Initialize();

			foreach(DynamicControlInfo info in _controls)
			{
				if (info.Uid == controlUid)
					return info;
			}

			return null;
		}

		/// <summary>
		/// Gets the control infos.
		/// </summary>
		/// <returns></returns>
		public static DynamicControlInfo[] GetControlInfos()
		{
			Initialize();
			return (DynamicControlInfo[])_controls.ToArray(typeof(DynamicControlInfo));
		}

		/// <summary>
		/// Gets the control infos by category.
		/// </summary>
		/// <returns></returns>
		public static DynamicControlCategory[] GetControlInfosByCategory()
		{
			Initialize();


			List<DynamicControlCategory> retVal = new List<DynamicControlCategory>();
			Dictionary<string, int> categoryHash = new Dictionary<string, int>();

			foreach (DynamicControlInfo info in _controls)
			{
				if (categoryHash.ContainsKey(info.Category))
				{
					retVal[categoryHash[info.Category]].DynamicControlInfos.Add(info);
				}
				else
				{
					DynamicControlCategory category = new DynamicControlCategory();
					category.Name = info.Category;
					category.DynamicControlInfos.Add(info);

					retVal.Add(category);
					categoryHash.Add(category.Name, retVal.Count-1);
				}
			}
			retVal.Sort();

			return retVal.ToArray();
		}


	}
}
