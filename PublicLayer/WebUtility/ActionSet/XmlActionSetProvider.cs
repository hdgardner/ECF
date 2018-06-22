using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Collections;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Mediachase.Cms.WebActionSet
{
    public class XmlActionSetProvider: IActionSetProvider
    {
        #region UserRole
        private string userRole;

        /// <summary>
        /// Gets the user role.
        /// </summary>
        /// <value>The user role.</value>
        public string UserRole
        {
            get { return userRole; }
        } 
        #endregion

        #region FilePath
        private string filePath;

        /// <summary>
        /// Gets the file path.
        /// </summary>
        /// <value>The file path.</value>
        public string FilePath
        {
            get { return filePath; }
        } 
        #endregion

        #region Page
        private Page page;

        /// <summary>
        /// Gets the page.
        /// </summary>
        /// <value>The page.</value>
        public Page Page
        {
            get { return page; }
        }
        #endregion

        #region ActionSetItems
        private ArrayList actionSetItems;

        /// <summary>
        /// Gets the action set items.
        /// </summary>
        /// <value>The action set items.</value>
        public ArrayList ActionSetItems
        {
            get { return actionSetItems; }
        } 
        #endregion

        #region RegisterPage
        /// <summary>
        /// Registers the page.
        /// </summary>
        public void RegisterPage()
        {
			Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "MenuScript", "<script type='text/javascript' language='javascript'>" + "var ActionSetID = new Array();" + Script + "</script>");
        }

        /// <summary>
        /// Registers the page.
        /// </summary>
        /// <param name="AutoInit">if set to <c>true</c> [auto init].</param>
		public void RegisterPage(bool AutoInit)
		{
			/*string s = "<script type='text/javascript' language='javascript'>"
				+ "var s  = BrowserDetect();"
				+ "StartInitScript(s);"
				+ "</script>";*/
			/*string s = "<script type='text/javascript' language='javascript'>"
				+ "var _uh  = $find('MediachaseCmsUtil'); "
				+ " if(_uh!=null) _uh.StartInitScript(_uh.BrowserDetect());"
				+ "</script>";*/
			RegisterPage();
			if (AutoInit)
			{
				//Page.ClientScript.RegisterStartupScript(this.GetType(), "AutoInitScript", s);
			}
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
                return ActionSetID + SetScript + MenuScript + InitScript;
            }
        } 
        #endregion

        #region ActionSetID
        /// <summary>
        /// Gets the action set ID.
        /// </summary>
        /// <value>The action set ID.</value>
        protected string ActionSetID
        {
            get
            {
                int i = 0;
                string array = string.Empty;
                foreach (Control ctrl in Page.Controls)
                {
                    ControlRunner(ctrl, ref array, ref i);
                }
                return array;
            }
        } 
        #endregion

        #region SetScript
        /// <summary>
        /// Gets the set script.
        /// </summary>
        /// <value>The set script.</value>
        protected string SetScript
        {
            get
            {
                StringBuilder s = new StringBuilder();
                foreach (ActionSet set in ActionSetItems)
                {
                    s.Append("function " + set.Name + "ActionSet(obj){");
                    s.Append(set.Script);
                    s.Append("newDIV.MENU = '" + set.Name + "Menu';");
                    foreach (Action act in set.ActionItems)
                    {
                        if (!act.Deny.Contains(UserRole))
                        {
                            s.Append(act.Script);
                        }
                    }
                    s.Append(set.Name + "ActionSetMenu(obj);");
                    s.Append("};");
                }
                return s.ToString();
            }
        } 
        #endregion

        #region MenuScript
        /// <summary>
        /// Gets the menu script.
        /// </summary>
        /// <value>The menu script.</value>
        protected string MenuScript
        {
            get
            {
                StringBuilder s = new StringBuilder();
                foreach (ActionSet set in ActionSetItems)
                {
                    s.Append("function " + set.Name + "ActionSetMenu(obj){");
                    if (set.Menu.MenuItems.Count != 0)
                    {
                        s.Append(set.Menu.ScriptStart);
                        foreach (MenuItem mi in set.Menu.MenuItems)
                        {
                            if (!mi.Deny.Contains(UserRole))
                            {
                                s.Append(mi.Script);
                            }
                        }
                        s.Append(set.Menu.ScriptEnd);
                        s.Append("newDIV.id = '" + set.Name + "Menu';");
                        s.Append("obj.appendChild(newDIV);");
                    }
                    s.Append("};");
                }
                return s.ToString();
            }
        } 
        #endregion

        #region InitScript
        /// <summary>
        /// Gets the init script.
        /// </summary>
        /// <value>The init script.</value>
        protected string InitScript
        {
            get
            {
                /*StringBuilder s = new StringBuilder();
                s.Append("function Init(){");
				//s.Append("alert('Init start');");
                s.Append("for(var i = 0; i < ActionSetID.length ; i++){");
                s.Append("var obj = document.getElementById(ActionSetID[i]);");
                s.Append("if(obj != null){");
                s.Append("var actionSet = obj.getAttribute('ActionSet');");
                s.Append("if(actionSet != null){");
				//s.Append("alert('ActionSet['+i.toString()+']');");
                s.Append("var str = \"\" + actionSet;");
                s.Append("switch(str){");
                foreach (ActionSet set in ActionSetItems)
                {
                    //s.Append("case '" + set.Name + "': " + set.Name + "ActionSet(obj); " + set.Name + "ActionSetMenu(obj);" + "break;");
                    s.Append("case '" + set.Name + "': " + set.Name + "ActionSet(obj); break;");
                }
                s.Append("};");
                s.Append("};");
                s.Append("};");
                s.Append("};");
                //foreach (ActionSet set in ActionSetItems)
                //{
                //    //s.Append(set.Name + "ActionSetMenu();");
                //}
				//s.Append("alert('Init end');");
                s.Append("};");
                return s.ToString();*/
				StringBuilder s = new StringBuilder();
				s.Append("function InitActionSet(){");
				s.Append("for(var i = 0; i < ActionSetID.length ; i++){");
				s.Append("var obj = document.getElementById(ActionSetID[i]);");
				s.Append("if(obj != null){");
				s.Append("var actionSet = obj.getAttribute('ActionSet');");
				s.Append("if(actionSet != null){");
				s.Append("var str = \"\" + actionSet;");
				s.Append("switch(str){");
				foreach (ActionSet set in ActionSetItems)
				{
					s.Append("case '" + set.Name + "': " + set.Name + "ActionSet(obj); break;");
				}
				s.Append("};");
				s.Append("};");
				s.Append("};");
				s.Append("};");
				s.Append("};");
				return s.ToString();
            }
        } 
        #endregion

        #region ctr:XmlActionSetProvider()
        /// <summary>
        /// Initializes a new instance of the <see cref="T:ActionSetManager"/> class.
        /// </summary>
        /// <param name="UserRole">The user role.</param>
        /// <param name="FilePath">The XML file path.</param>
        /// <param name="page">The page.</param>
		public XmlActionSetProvider(string UserRole, string FilePath, Page page)
        {
            this.page = page;
            this.userRole = UserRole;
            this.filePath = FilePath;
            actionSetItems = new ArrayList();
            FillItems();
        }
        #endregion

        #region ControlRunner() private
        /// <summary>
        /// Controls the runner.
        /// </summary>
        /// <param name="ctrl">The CTRL.</param>
        /// <param name="array">The array.</param>
        /// <param name="i">The i.</param>
		public void ControlRunner(Control ctrl, ref string array, ref int i)
        {
            if (ctrl is IAttributeAccessor)
            {
                if (!String.IsNullOrEmpty(((IAttributeAccessor)ctrl).GetAttribute("ActionSet")))
                {
                    array += "ActionSetID[" + i.ToString() + "] = '" + ctrl.ClientID + "';";
                    i += 1;
                }
            }

            foreach (Control c in ctrl.Controls)
            {
                ControlRunner(c, ref array, ref i);
            }
        } 
        #endregion

        #region FillItems() private
        /// <summary>
        /// Fills the items.
        /// </summary>
		public void FillItems()
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(FilePath);
            foreach (XmlNode aSet in xml.ChildNodes[1])
            {
                actionSetItems.Add(FillActionSet(aSet));
            }
        } 
        #endregion

        #region FillActionSet() private
        /// <summary>
        /// Fills the action set.
        /// </summary>
        /// <param name="ActionSetNode">The action set node.</param>
        /// <returns></returns>
        public ActionSet FillActionSet(XmlNode ActionSetNode)
        {
            ActionSet set = new ActionSet();
            set.Name = ActionSetNode.Attributes["Name"].Value.ToString();
            set.CSS = ActionSetNode.Attributes["CSS"].Value.ToString();
            set.CSSHover = ActionSetNode.Attributes["CSS-Hover"].Value.ToString();
            //FILL ACTION
            foreach (XmlNode act in ActionSetNode.ChildNodes[0].ChildNodes)
            {
                set.ActionItems.Add(FillAction(act));
            }
            //FILL MENU if SECTION EXIST
            if (ActionSetNode.ChildNodes[1] != null)
            {
                set.Menu = FillMenu(ActionSetNode.ChildNodes[1]);       
            }
            return set;
        } 
        #endregion

        #region FillAction() private
        /// <summary>
        /// Fills the action.
        /// </summary>
        /// <param name="ActionNode">The action node.</param>
        /// <returns></returns>
        public Action FillAction(XmlNode ActionNode)
        {
            Action act = new Action();
            act.Event = ActionNode.Attributes["Event"].Value.ToString();
            act.Code = ActionNode.Attributes["Code"].Value.ToString();
            //ALLOW DENY
            foreach (XmlNode rule in ActionNode.ChildNodes)
            {
                if (rule.Name.ToUpper() == "DENY")
                {
                    act.Deny.Add(rule.Attributes["role"].Value);
                }
                if (rule.Name.ToUpper() == "ALLOW")
                {
                    act.Allow.Add(rule.Attributes["role"].Value);
                }
            }
            return act;
        } 
        #endregion

        #region FillMenu() private
        /// <summary>
        /// Fills the menu.
        /// </summary>
        /// <param name="MenuNode">The menu node.</param>
        /// <returns></returns>
		public Menu FillMenu(XmlNode MenuNode)
        {
            Menu menu = new Menu();
            if (MenuNode.Attributes["ImageUrl"] != null)
                menu.ImageUrl = MenuNode.Attributes["ImageUrl"].Value.ToString();

            if (MenuNode.Attributes["Tooltip"] != null)
                menu.Tooltip = MenuNode.Attributes["Tooltip"].Value.ToString();

            if (MenuNode.Attributes["Text"] != null)
                menu.Text = MenuNode.Attributes["Text"].Value.ToString();

            if (MenuNode.Attributes["Code"] != null)
                menu.Code = MenuNode.Attributes["Code"].Value.ToString();

            foreach (XmlNode item in MenuNode.FirstChild.ChildNodes)
            {
                menu.MenuItems.Add(FillMenuItem(item));
            }

            return menu;
        } 
        #endregion

        #region FillMenuItem() private
        /// <summary>
        /// Fills the menu item.
        /// </summary>
        /// <param name="MenuItemNode">The menu item node.</param>
        /// <returns></returns>
		public MenuItem FillMenuItem(XmlNode MenuItemNode)
        {
            MenuItem mi = new MenuItem();
            if (MenuItemNode.Attributes["ImageUrl"] != null)
                mi.ImageUrl = MenuItemNode.Attributes["ImageUrl"].Value.ToString();

            if (MenuItemNode.Attributes["Tooltip"] != null)
                mi.Tooltip = MenuItemNode.Attributes["Tooltip"].Value.ToString();

            if (MenuItemNode.Attributes["Text"] != null)
                mi.Text = MenuItemNode.Attributes["Text"].Value.ToString();
            if (MenuItemNode.Attributes["Code"] != null)
                mi.Code = MenuItemNode.Attributes["Code"].Value.ToString();
            //ALLOW DENY
            foreach (XmlNode rule in MenuItemNode.ChildNodes)
            {
                if (rule.Name.ToUpper() == "DENY")
                {
                    mi.Deny.Add(rule.Attributes["role"].Value);
                }
                if (rule.Name.ToUpper() == "ALLOW")
                {
                    mi.Allow.Add(rule.Attributes["role"].Value);
                }
            }
            return mi;
        } 
        #endregion

    }
}