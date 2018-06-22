using System;
using System.Collections.Generic;
using System.Text;

namespace Mediachase.Cms.Objects
{
    public class SiteFolder : SiteNode
    {
        private SiteFolder[] _Folders;

        /// <summary>
        /// Gets or sets the folders.
        /// </summary>
        /// <value>The folders.</value>
        public SiteFolder[] Folders
        {
            get { return _Folders; }
            set { _Folders = value; }
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
        private SitePage[] _Pages;

        /// <summary>
        /// Gets or sets the pages.
        /// </summary>
        /// <value>The pages.</value>
        public SitePage[] Pages
        {
            get { return _Pages; }
            set { _Pages = value; }
        }
    }
}
