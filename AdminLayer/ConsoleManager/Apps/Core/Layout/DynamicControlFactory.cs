using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Collections;
using System.Globalization;
using Mediachase.Ibn.Data;

namespace Mediachase.Ibn.Web.UI.Layout
{
	public static class DynamicControlFactory
	{
		private static ArrayList _controls;

		private static string _controlsFolderPath = string.Empty;
		//private const string = "~";

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
        /// Creates the property page.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="controlUid">The control uid.</param>
        /// <returns></returns>
		public static Control CreatePropertyPage(TemplateControl page, string controlUid)
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
				return null;
			//    throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Invalid control Uid '{0}'", controlUid), "controlUid");

			// Try Load Control
			if (!string.IsNullOrEmpty(info.PropertyPagePath))
				retVal = page.LoadControl(CreateControlVirtualPath(info.PropertyPagePath));
			else if (!string.IsNullOrEmpty(info.PropertyPageType))
				retVal = (Control)AssemblyUtil.LoadObject(info.PropertyPageType);

			//TODO: Load default PP control
			//if (retVal == null)
			//    retVal = page.LoadControl();

			if (retVal == null)
				throw new InvalidOperationException(String.Format("Cant load PropertyPageControl for control: {0}", controlUid));

			return retVal;
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
				return null;
				//throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Invalid control Uid '{0}'", controlUid), "controlUid");

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
		/// Creates the specified page.
		/// </summary>
		/// <param name="page">The page.</param>
		/// <param name="controlUid">The control uid.</param>
		/// <param name="instanceUid">The instance uid.</param>
		/// <returns></returns>
		public static Control Create(TemplateControl page, string controlUid, string instanceUid)
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
				return null;
			//throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Invalid control Uid '{0}'", controlUid), "controlUid");

			// Try Load Control
			if (!string.IsNullOrEmpty(info.Path))
				retVal = page.LoadControl(CreateControlVirtualPath(info.Path));
			else if (!string.IsNullOrEmpty(info.Type))
				retVal = (Control)AssemblyUtil.LoadObject(info.Type);

			// Try Load Adapter
			if (retVal != null && (!string.IsNullOrEmpty(info.AdapterPath) || !string.IsNullOrEmpty(info.AdapterType)))
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
	}
}