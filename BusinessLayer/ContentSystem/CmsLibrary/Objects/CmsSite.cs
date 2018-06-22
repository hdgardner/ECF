using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;

namespace Mediachase.Cms.Objects
{
    public class CmsSite
    {
        private StringDictionary _Attributes;

        /// <summary>
        /// Gets or sets the attributes.
        /// </summary>
        /// <value>The attributes.</value>
        public StringDictionary Attributes
        {
            get { return _Attributes; }
            set { _Attributes = value; }
        }
        private string _Description;

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }
        private string[] _Domains;

        /// <summary>
        /// Gets or sets the domains.
        /// </summary>
        /// <value>The domains.</value>
        public string[] Domains
        {
            get { return _Domains; }
            set { _Domains = value; }
        }
        private Guid _ID;

        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        /// <value>The ID.</value>
        public Guid ID
        {
            get { return _ID; }
            set { _ID = value; }
        }
        private string _Name;

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }
        private string _RootFolder;

        /// <summary>
        /// Gets or sets the root folder.
        /// </summary>
        /// <value>The root folder.</value>
        public string RootFolder
        {
            get { return _RootFolder; }
            set { _RootFolder = value; }
        }

        private SiteMenu[] _Menus;

        /// <summary>
        /// Gets or sets the menus.
        /// </summary>
        /// <value>The menus.</value>
        public SiteMenu[] Menus
        {
            get { return _Menus; }
            set { _Menus = value; }
        }
    }
}
