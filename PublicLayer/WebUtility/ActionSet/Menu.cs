using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Mediachase.Cms.WebActionSet
{
    public class Menu
    {
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

        #region ScriptStart
        /// <summary>
        /// Gets the script.
        /// </summary>
        /// <value>The script.</value>
        public string ScriptStart
        {
            get
            {
                StringBuilder s = new StringBuilder();
                s.Append("var newDIV = document.createElement('div');");
                s.Append("newDIV.setAttribute('IsMenu','true');");
                s.Append("newDIV.className = 'MenuOff';");
                s.Append("newDIV.innerHTML = \"");

                //s.Append(String.Format(@"<div class='DesignMenu'><ul><li><a title='{3}' class='hide' href='{0}'><img src='{1}' border='0' />{2}</a><!--[if lte IE 6]> <A title='{3}' href='{0}'><img src='{1}' border='0' />{2}<TABLE border='0' cellspacing='0' cellpadding='0'><TR><TD><![endif]--><ul>", Code, ImageUrl, Text, Tooltip));
                s.Append(String.Format(@"<div class='Mediachase_ActionSet_Menu'><ul><li><a title='{3}' class='hide' href='{0}'><img src='{1}' border='0' />{2}<!--[if IE 7]><!--></a><!--<![endif]--><table><tr><td><ul style='padding:1px;border:1px solid black;'>", Code, ImageUrl, Text, Tooltip));
                return s.ToString();
            }
        }
        #endregion

        #region ScriptEnd
        /// <summary>
        /// Gets the script.
        /// </summary>
        /// <value>The script.</value>
        public string ScriptEnd
        {
            get
            {
                //return @"</ul><!--[if lte IE 6]></TD></TR></TABLE></A><![endif]--></li></ul>" +"\"" + ";document.body.appendChild(newDIV);";
                ////return "</ul><!--[if lte IE 6]></TD></TR></TBODY></TABLE></A><![endif]--></li></ul>\";obj.appendChild(newDIV);";
                return @"</ul></td></tr></table></li></ul></div>" + "\"" + ";document.body.appendChild(newDIV);";
            }
        }
        #endregion

        #region MenuItems
        private ArrayList menuItems;
        /// <summary>
        /// Gets or sets the action items.
        /// </summary>
        /// <value>The action items.</value>
        public ArrayList MenuItems
        {
            get { return menuItems; }
            set { menuItems = value; }
        }
        #endregion

        #region ctr:Menu()
        /// <summary>
        /// Initializes a new instance of the <see cref="Menu"/> class.
        /// </summary>
        public Menu()
        {
            MenuItems = new ArrayList();
            Text = String.Empty;
            Code = String.Empty;
            ImageUrl = String.Empty;
            Tooltip = String.Empty;
        }
        #endregion
    }
}
