using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Mediachase.Cms.WebActionSet
{
    public class MenuItem
    {
        #region Allow
        private ArrayList allow;

        /// <summary>
        /// Gets or sets the allowed roles.
        /// </summary>
        /// <value>The allow.</value>
        public ArrayList Allow
        {
            get { return allow; }
            set { allow = value; }
        }
        #endregion

        #region Deny
        private ArrayList deny;

        /// <summary>
        /// Gets or sets the denyed roles.
        /// </summary>
        /// <value>The deny.</value>
        public ArrayList Deny
        {
            get { return deny; }
            set { deny = value; }
        }
        #endregion

        #region Code
        private string code;

        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        /// <value>The code.</value>
        public string Code
        {
            get { return code; }
            set { code = value; }
        }
        #endregion

        #region Tooltip
        private string tooltip;

        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        /// <value>The code.</value>
        public string Tooltip
        {
            get { return tooltip; }
            set { tooltip = value; }
        }
        #endregion

        #region Text
        private string text;

        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        /// <value>The code.</value>
        public string Text
        {
            get { return text; }
            set { text = value; }
        }
        #endregion

        #region ImageUrl
        private string imageUrl;

        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        /// <value>The code.</value>
        public string ImageUrl
        {
            get { return imageUrl; }
            set { imageUrl = value; }
        }
        #endregion

        #region Script
        /// <summary>
        /// Gets the script.
        /// </summary>
        /// <value>The script.</value>
        public string Script
        {
            get
            {
                StringBuilder s = new StringBuilder();
                s.Append("<li>");
                s.Append("<a title='" + Tooltip + "' href='" + Code.Replace("'", "\\\"") + "'>");                
                s.Append("<table  cellspacing='0' cellpadding='0' border='0'><tr valign='center'><td style='background-color:#aec9f0;'>");
                if (ImageUrl != string.Empty)
                {
                    s.Append("<img border='0' src='" + ImageUrl + "' align='absmiddle'>");
                }
                s.Append("</td><td valign='center' align='left' width='100%'>");
                s.Append("<!--[if lte IE 6]><a title='" + Tooltip + "' href='" + Code.Replace("'", "\\\"") + "'><div style='height:100%;width:100%;'><![endif]-->");
                s.Append("&nbsp;" + Text);
                s.Append("<!--[if lte IE 6]></a></div><![endif]--></td></tr></table></a>");
                //s.Append("</td></tr></table></a>");
                //return String.Format("<li cellspacing='0'><a title='{0}' href='{1}'><img  align='middle' src='{2}' border='0' />&nbsp;{3}</a></li>", Tooltip, Code, ImageUrl, Text);
                s.Append("</li>");
                return s.ToString();
            }
        }
        #endregion

        #region ctr:MenuItem()
        /// <summary>
        /// Initializes a new instance of the <see cref="MenuItem"/> class.
        /// </summary>
        public MenuItem()
        {
            Allow = new ArrayList();
            Deny = new ArrayList();
            Text = String.Empty;
            Code = String.Empty;
            ImageUrl = String.Empty;
            Tooltip = String.Empty;
        }
        #endregion
    }
}
