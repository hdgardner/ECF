using System;
using System.Collections.Generic;
using System.Text;

namespace Mediachase.Web.Console.Navigation
{
    [Serializable]
    public class NavigationState
    {
        private string _SelectedMenuId = String.Empty;

        /// <summary>
        /// Gets or sets the selected menu id.
        /// </summary>
        /// <value>The selected menu id.</value>
        public string SelectedMenuId
        {
            get { return _SelectedMenuId; }
            set { _SelectedMenuId = value; }
        }
        private string _SelectedContentNameId = String.Empty;

        /// <summary>
        /// Gets or sets the selected content name id.
        /// </summary>
        /// <value>The selected content name id.</value>
        public string SelectedContentNameId
        {
            get { return _SelectedContentNameId; }
            set { _SelectedContentNameId = value; }
        }

        private string _QueryString = String.Empty;

        /// <summary>
        /// Gets or sets the query string.
        /// </summary>
        /// <value>The query string.</value>
        public string QueryString
        {
            get { return _QueryString; }
            set { _QueryString = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationState"/> class.
        /// </summary>
        /// <param name="selectedMenuId">The selected menu id.</param>
        /// <param name="selectedContentNameId">The selected content name id.</param>
        /// <param name="queryString">The query string.</param>
        public NavigationState(string selectedMenuId, string selectedContentNameId, string queryString)
        {
            _SelectedMenuId = selectedMenuId;
            _SelectedContentNameId = selectedContentNameId;
            _QueryString = queryString;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationState"/> class.
        /// </summary>
        public NavigationState() { }
    }
}
