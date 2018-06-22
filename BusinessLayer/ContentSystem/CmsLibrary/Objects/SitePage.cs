using System;
using System.Collections.Generic;
using System.Text;

namespace Mediachase.Cms.Objects
{
    public class SitePage : SiteNode
    {
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
        private bool _IsDefault;

        /// <summary>
        /// Gets or sets a value indicating whether this instance is default.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is default; otherwise, <c>false</c>.
        /// </value>
        public bool IsDefault
        {
            get { return _IsDefault; }
            set { _IsDefault = value; }
        }
        private string _Keywords;

        /// <summary>
        /// Gets or sets the keywords.
        /// </summary>
        /// <value>The keywords.</value>
        public string Keywords
        {
            get { return _Keywords; }
            set { _Keywords = value; }
        }
        private string _MasterPage;

        /// <summary>
        /// Gets or sets the master page.
        /// </summary>
        /// <value>The master page.</value>
        public string MasterPage
        {
            get { return _MasterPage; }
            set { _MasterPage = value; }
        }
        //private string _Name;
        //private string _Title;
    }
}
