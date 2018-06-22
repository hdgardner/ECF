using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Configuration.Provider;

namespace Mediachase.Web.Console.Config
{
    public class ModuleConfig
    {
        private ViewCollection _Views;
        private ModuleSettingCollection _Settings;
        private Acl _Acl;
        private string _Name;
        private string _DisplayName;

        /// <summary>
        /// Initializes a new instance of the <see cref="ModuleConfig"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
		public ModuleConfig(string name)
		{
			_Name = name;
            _DisplayName = name;
		}

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
		public string Name
		{
			get { return _Name; }
		}

        /// <summary>
        /// Gets the display name.
        /// </summary>
        /// <value>The display name.</value>
        public string DisplayName
        {
            get { return _DisplayName; }
        }

        /// <summary>
        /// Gets or sets the acl.
        /// </summary>
        /// <value>The acl.</value>
        public Acl Acl
        {
            get { return _Acl; }
            set { _Acl = value; }
        }

        /// <summary>
        /// Gets or sets the settings.
        /// </summary>
        /// <value>The settings.</value>
        public ModuleSettingCollection Settings
        {
            get { return _Settings; }
            set { _Settings = value; }
        }

        /// <summary>
        /// Gets or sets the views.
        /// </summary>
        /// <value>The views.</value>
        public ViewCollection Views
        {
            get { return _Views; }
            set { _Views = value; }
        }

        /// <summary>
        /// Finds the view by id.
        /// </summary>
        /// <param name="viewId">The view id.</param>
        /// <returns></returns>
        public AdminView FindViewById(string viewId)
        {
            foreach (AdminView view in Views)
            {
                if (view.ViewId.Equals(viewId))
                    return view;
            }

            return null;
        }
    }
}
