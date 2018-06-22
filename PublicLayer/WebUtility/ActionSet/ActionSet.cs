using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Mediachase.Cms.WebActionSet
{
    public class ActionSet
    {
        #region CSS
        private string css;

        /// <summary>
        /// Gets or sets the CSS class name.
        /// </summary>
        /// <value>The CSS class name.</value>
        public string CSS
        {
            get { return css; }
            set { css = value; }
        } 
        #endregion

        #region CSSHover
        private string cssHover;

        /// <summary>
        /// Gets or sets the CSS class name.
        /// </summary>
        /// <value>The CSS class name.</value>
        public string CSSHover
        {
            get { return cssHover; }
            set { cssHover = value; }
        }
        #endregion

        #region Name
        private string name;
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get { return name; }
            set { name = value; }
        } 
        #endregion

        #region ActionItems
        private ArrayList actionItems;
        /// <summary>
        /// Gets or sets the action items.
        /// </summary>
        /// <value>The action items.</value>
        public ArrayList ActionItems
        {
            get { return actionItems; }
            set { actionItems = value; }
        } 
        #endregion

        #region Menu
        private Menu menu;

        /// <summary>
        /// Gets or sets the menu.
        /// </summary>
        /// <value>The menu.</value>
        public Menu Menu
        {
            get { return menu; }
            set { menu = value; }
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
                StringBuilder script = new StringBuilder();
                //top
                script.Append("var newDIV = document.createElement('div');");
                script.Append("newDIV.id = 'ActionSetDiv_top_'+obj.id;");
                script.Append("newDIV.setAttribute('IsFrame','top');");
                script.Append("newDIV.setAttribute('OFF','" + CSS + "');");
                script.Append("newDIV.setAttribute('ON','" + CSSHover + "');");
                script.Append("newDIV.className = '" + CSS + "';");
                script.Append("obj.appendChild(newDIV);");
                //bottom
                script.Append("var newDIV = document.createElement('div');");
                script.Append("newDIV.id = 'ActionSetDiv_bottom_'+obj.id;");
                script.Append("newDIV.setAttribute('IsFrame','bottom');");
                script.Append("newDIV.setAttribute('OFF','" + CSS + "');");
                script.Append("newDIV.setAttribute('ON','" + CSSHover + "');");
                script.Append("newDIV.className = '" + CSS + "';");
                script.Append("obj.appendChild(newDIV);");
                //right
                script.Append("var newDIV = document.createElement('div');");
                script.Append("newDIV.id = 'ActionSetDiv_right_'+obj.id;");
                script.Append("newDIV.setAttribute('IsFrame','right');");
                script.Append("newDIV.setAttribute('OFF','" + CSS + "');");
                script.Append("newDIV.setAttribute('ON','" + CSSHover + "');");
                script.Append("newDIV.className = '" + CSS + "';");
                script.Append("obj.appendChild(newDIV);");
                //left
                script.Append("var newDIV = document.createElement('div');");
                script.Append("newDIV.id = 'ActionSetDiv_left_'+obj.id;");
                script.Append("newDIV.setAttribute('IsFrame','left');");
                script.Append("newDIV.setAttribute('OFF','" + CSS + "');");
                script.Append("newDIV.setAttribute('ON','" + CSSHover + "');");
                script.Append("newDIV.className = '" + CSS + "';");
                script.Append("obj.appendChild(newDIV);");

                return script.ToString();
            }
        } 
        #endregion

        #region ctr:ActionSet()
        /// <summary>
        /// Initializes a new instance of the <see cref="ActionSet"/> class.
        /// </summary>
        public ActionSet()
        {
            actionItems = new ArrayList();
            menu = new Menu();
        } 
        #endregion
    }
}

