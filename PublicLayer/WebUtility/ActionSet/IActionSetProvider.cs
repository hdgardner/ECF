using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Xml;

namespace Mediachase.Cms.WebActionSet
{
	public interface IActionSetProvider
	{
        /// <summary>
        /// Controls the runner.
        /// </summary>
        /// <param name="ctrl">The CTRL.</param>
        /// <param name="array">The array.</param>
        /// <param name="i">The i.</param>
		void ControlRunner(Control ctrl, ref string array, ref int i);

        /// <summary>
        /// Fills the items.
        /// </summary>
		void FillItems();

        /// <summary>
        /// Fills the action set.
        /// </summary>
        /// <param name="ActionSetNode">The action set node.</param>
        /// <returns></returns>
		ActionSet FillActionSet(XmlNode ActionSetNode);

        /// <summary>
        /// Fills the action.
        /// </summary>
        /// <param name="ActionNode">The action node.</param>
        /// <returns></returns>
		Action FillAction(XmlNode ActionNode);

        /// <summary>
        /// Fills the menu.
        /// </summary>
        /// <param name="MenuNode">The menu node.</param>
        /// <returns></returns>
		Menu FillMenu(XmlNode MenuNode);

        /// <summary>
        /// Fills the menu item.
        /// </summary>
        /// <param name="MenuItemNode">The menu item node.</param>
        /// <returns></returns>
		MenuItem FillMenuItem(XmlNode MenuItemNode);
	}
}
