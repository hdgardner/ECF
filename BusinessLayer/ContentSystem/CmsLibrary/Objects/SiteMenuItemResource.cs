using System;
using System.Collections.Generic;
using System.Text;

namespace Mediachase.Cms.Objects
{
    public partial class SiteMenuItemResource
    {
        private string _Text;

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        public string Text
        {
            get { return _Text; }
            set { _Text = value; }
        }

        private string _Tooltip;

        /// <summary>
        /// Gets or sets the tooltip.
        /// </summary>
        /// <value>The tooltip.</value>
        public string Tooltip
        {
            get { return _Tooltip; }
            set { _Tooltip = value; }
        }

        private string _LanguageCode;

        /// <summary>
        /// Gets or sets the language code.
        /// </summary>
        /// <value>The language code.</value>
        public string LanguageCode
        {
            get { return _LanguageCode; }
            set { _LanguageCode = value; }
        }
    }
}
