using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Mediachase.Web.Console.Interfaces;
using Mediachase.Commerce.Shared;

namespace Mediachase.Web.Console.Config
{
    public class ViewManager
    {
        /// <summary>
        /// Creates the view collection.
        /// </summary>
        /// <param name="module">The module.</param>
        /// <param name="viewsNode">The views node.</param>
        /// <returns></returns>
        public static ViewCollection CreateViewCollection(ModuleConfig module, XmlNode viewsNode)
        {
            if (viewsNode == null)
                return null;

            // Create list
            ViewCollection viewCol = new ViewCollection();
            XmlNodeList views = viewsNode.SelectNodes("View");
            foreach (XmlNode view in views)
            {
                AdminView viewElement = new AdminView(module, view.Attributes["id"].Value, view.Attributes["name"].Value, view.Attributes["controlUrl"].Value);

				// Bind admin view attributes
                foreach (XmlAttribute attr in view.Attributes)
                    viewElement.Attributes[attr.Name] = attr.Value;

				if (view.Attributes["isNameDynamic"] != null)
				{
					bool boolValue = false;
					Boolean.TryParse(view.Attributes["isNameDynamic"].Value, out boolValue);
					viewElement.IsNameDynamic = boolValue;
				}

                // Bind columns
                XmlNodeList columns = view.SelectNodes("Columns/Column");
				int columnsCount = 0;
				int visibleColumnsCount = 0;
                foreach (XmlNode node in columns)
                {
                    ViewColumn column = new ViewColumn();
                    if(node.Attributes["DataField"] != null)
                        column.DataField = node.Attributes["DataField"].Value;

					// assign ColumnType
					ColumnType type = ColumnType.None;
					if (node.Attributes["ColumnType"] != null)
					{
						try
						{
							type = (ColumnType)Enum.Parse(typeof(ColumnType), node.Attributes["ColumnType"].Value, true);
						}
						catch { }
					}
					column.ColumnType = type;

					// set template
					if ((column.ColumnType == ColumnType.CustomTemplate || column.ColumnType == ColumnType.None) && node.HasChildNodes)
					{
						column.Template = new ViewColumnTemplate();
						// select Template nodes
						XmlNodeList tmpChildNodes = node.SelectNodes("Template");
						if (tmpChildNodes.Count > 0)
							// bind template attributes
							BindColumnTemplateProperties(column.Template, tmpChildNodes[0]);
					}

					// set column actions
					if (column.ColumnType == ColumnType.Action && node.HasChildNodes)
						BindColumnActions(column, node);

					// set attributes
                    BindColumnProperties(column, node);

					column.ColumnIndex = columnsCount++;
					if (column.Visible)
						column.ColumnVisibleIndex = visibleColumnsCount++;

					// add column to the collection
                    viewElement.Columns.Add(column);
                }

                XmlNodeList tabs = view.SelectNodes("Tabs/Tab");
                foreach (XmlNode node in tabs)
                {
                    ViewTab tab = new ViewTab();

                    BindTabProperties(tab, node);
                    viewElement.Tabs.Add(tab);
                }

				//XmlNodeList actions = view.SelectNodes("Actions/Action");
				//foreach (XmlNode node in actions)
				//{
				//    viewElement.Actions.Add(PopulateActionRecursive(node));
				//}

                XmlNodeList transitions = view.SelectNodes("Transitions/Transition");
                foreach (XmlNode node in transitions)
                {
                    ViewTransition transition = new ViewTransition();

                    BindTransitionProperties(transition, node);
                    viewElement.Transitions.Add(transition);
                }

                // Check roles security
                if (viewElement.Attributes.Contains("Roles"))
                {
                    string roles = viewElement.Attributes.Contains("Roles").ToString();
                    if (!String.IsNullOrEmpty(roles))
                    {
                        if (!SecurityManager.CheckPermission(roles.Split(new char[] { ',' })))
                        {
                            // Skip current record
                            continue;
                        }
                    }
                }

                viewCol.Add(viewElement);
            }

            return viewCol;
        }

        /// <summary>
        /// Binds the column actions.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <param name="actionsNode">The actions node.</param>
		private static void BindColumnActions(ViewColumn column, XmlNode actionsNode)
		{
			foreach (XmlNode node in actionsNode.SelectNodes("Item"))
			{
				ViewColumnAction action = new ViewColumnAction();
				foreach (XmlAttribute attr in node.Attributes)
					action.Attributes[attr.Name] = attr.Value;
				column.AddColumnAction(action);
			}
		}

		///// <summary>
		///// Populates the action recursive.
		///// </summary>
		///// <param name="node">The node.</param>
		///// <returns></returns>
		//private static ViewAction PopulateActionRecursive(XmlNode node)
		//{
		//    // Create root action
		//    ViewAction action = new ViewAction(node.Attributes["type"].Value, node.Attributes["name"].Value);
		//    BindActionProperties(action, node);

		//    // Populate children
		//    XmlNodeList actions = node.SelectNodes("Action");
		//    foreach (XmlNode nodeChild in actions)
		//    {
		//        action.Children.Add(PopulateActionRecursive(nodeChild));
		//    }

		//    return action;
		//}

        /// <summary>
        /// Binds the tab properties.
        /// </summary>
        /// <param name="tab">The tab.</param>
        /// <param name="node">The node.</param>
        private static void BindTabProperties(ViewTab tab, XmlNode node)
        {
            // Bind all attributes
            foreach (XmlAttribute attr in node.Attributes)
            {
                tab.Attributes[attr.Name] = attr.Value;
            }
        }

        /// <summary>
        /// Binds the transition properties.
        /// </summary>
        /// <param name="tran">The tran.</param>
        /// <param name="node">The node.</param>
        private static void BindTransitionProperties(ViewTransition tran, XmlNode node)
        {
            // Bind all attributes
            foreach (XmlAttribute attr in node.Attributes)
            {
                tran.Attributes[attr.Name] = attr.Value;
            }
        }

		///// <summary>
		///// Binds the action properties.
		///// </summary>
		///// <param name="action">The action.</param>
		///// <param name="node">The node.</param>
		//private static void BindActionProperties(ViewAction action, XmlNode node)
		//{
		//    // Bind all attributes
		//    foreach (XmlAttribute attr in node.Attributes)
		//    {
		//        action.Attributes[attr.Name] = attr.Value;
		//    }
		//}

        /// <summary>
        /// Binds the column properties.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <param name="node">The node.</param>
        private static void BindColumnProperties(ViewColumn column, XmlNode node)
        {
            // Bind all attributes
            foreach (XmlAttribute attr in node.Attributes)
            {
                column.Attributes[attr.Name] = attr.Value;
            }
        }

        /// <summary>
        /// Binds the column template properties.
        /// </summary>
        /// <param name="template">The template.</param>
        /// <param name="node">The node.</param>
        private static void BindColumnTemplateProperties(ViewColumnTemplate template, XmlNode node)
        {
            // Bind all attributes
            foreach (XmlAttribute attr in node.Attributes)
            {
                template.Attributes[attr.Name] = attr.Value;
            }
        }
    }
}
