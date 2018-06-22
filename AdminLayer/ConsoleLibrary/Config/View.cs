using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Xml;
using System.Reflection;
using Mediachase.Web.Console.Interfaces;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.Web.Console.Config
{
	[Serializable]
    public class AdminView
    {
        private string _ControlUrl = String.Empty;

        /// <summary>
        /// Gets or sets the control URL.
        /// </summary>
        /// <value>The control URL.</value>
        public string ControlUrl
        {
            get { return _ControlUrl; }
            set { _ControlUrl = value; }
        }

        private string _Name = String.Empty;

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        Hashtable _Attributes = new Hashtable();

        /// <summary>
        /// Gets or sets the attributes.
        /// </summary>
        /// <value>The attributes.</value>
        public Hashtable Attributes
        {
            get { return _Attributes; }
            set { _Attributes = value; }
        }

        private string _ViewId = String.Empty;

        /// <summary>
        /// Gets the list id.
        /// </summary>
        /// <value>The list id.</value>
        public string ViewId
        {
            get { return _ViewId; }
        }

		private bool _IsNameDynamic = false;

		/// <summary>
		/// If true, Name will be set in code which uses the view.
		/// </summary>
		/// <value></value>
		public bool IsNameDynamic
		{
			get { return _IsNameDynamic; }
			set { _IsNameDynamic = value; }
		}

        private ViewColumnCollection _Columns = null;

        /// <summary>
        /// Gets or sets the columns.
        /// </summary>
        /// <value>The columns.</value>
        public ViewColumnCollection Columns
        {
            get { return _Columns; }
            set { _Columns = value; }
        }

		//private ViewActionCollection _Actions = null;

		///// <summary>
		///// Gets or sets the actions.
		///// </summary>
		///// <value>The actions.</value>
		//public ViewActionCollection Actions
		//{
		//    get { return _Actions; }
		//    set { _Actions = value; }
		//}

        private ViewTabCollection _Tabs = null;

        /// <summary>
        /// Gets or sets the tabs.
        /// </summary>
        /// <value>The tabs.</value>
        public ViewTabCollection Tabs
        {
            get { return _Tabs; }
            set { _Tabs = value; }
        }

        private ViewTransitionCollection _Transitions = null;

        /// <summary>
        /// Gets or sets the transitions.
        /// </summary>
        /// <value>The transitions.</value>
        public ViewTransitionCollection Transitions
        {
            get { return _Transitions; }
            set { _Transitions = value; }
        }

        ModuleConfig _Module = null;

        /// <summary>
        /// Gets the module.
        /// </summary>
        /// <value>The module.</value>
        public ModuleConfig Module
        {
            get { return _Module; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="List"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="listId">The list id.</param>
        /// <param name="name">The name.</param>
        /// <param name="controlUrl">The control URL.</param>
        public AdminView(ModuleConfig parent, string listId, string name, string controlUrl)
        {
            _Module = parent;
            _ViewId = listId;
            _ControlUrl = controlUrl;
            _Name = name;
            _Columns = new ViewColumnCollection();
            //_Actions = new ViewActionCollection();
            _Tabs = new ViewTabCollection();
            _Transitions = new ViewTransitionCollection();
        }

		public string GetLocalizedName()
		{
			return UtilHelper.GetResFileString(this.Name);
		}
    }
}
