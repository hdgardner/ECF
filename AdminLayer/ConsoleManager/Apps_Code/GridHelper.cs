using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Mediachase.Web.Console.Config;
using Mediachase.Web.Console;
using ComponentArt.Web.UI;
using Mediachase.Web.Console.Interfaces;
using Mediachase.Web.Console.Common;

namespace Mediachase.Commerce.Manager
{
    /// <summary>
    /// Summary description for GridHelper
    /// </summary>
    public class GridHelper
    {
        /// <summary>
        /// Binds the grid.
        /// </summary>
        /// <param name="grid">The grid.</param>
        /// <param name="appId">The app id.</param>
        /// <param name="viewId">The view id.</param>
        public static void BindGrid(Grid grid, string appId, string viewId)
        {
            new GridHelper().CreateColumns(grid, appId, viewId);
        }

        internal class SimpleTemplate : ITemplate
        {
            string _controlUrl = String.Empty;
            string _ModuleName = String.Empty;
            UserControl _parent = null;
            ViewColumnTemplate _columnTemplate = null;
            /// <summary>
            /// Initializes a new instance of the <see cref="SimpleTemplate"/> class.
            /// </summary>
            /// <param name="parent">The parent.</param>
            /// <param name="moduleName">Name of the module.</param>
            /// <param name="columnTemplate">The column template.</param>
            public SimpleTemplate(UserControl parent, string moduleName, ViewColumnTemplate columnTemplate)
            {
                _ModuleName = moduleName;
                _columnTemplate = columnTemplate;
                _parent = parent;
            }

            #region ITemplate Members

            /// <summary>
            /// When implemented by a class, defines the <see cref="T:System.Web.UI.Control"/> object that child controls and templates belong to. These child controls are in turn defined within an inline template.
            /// </summary>
            /// <param name="container">The <see cref="T:System.Web.UI.Control"/> object to contain the instances of controls from the inline template.</param>
            public void InstantiateIn(Control container)
            {
                string url = String.Empty;
                if (_columnTemplate.ControlUrl.StartsWith("~"))
                    url = _columnTemplate.ControlUrl;
                else
                    url = String.Format("~/apps/{0}/{1}", _ModuleName, _columnTemplate.ControlUrl);

                Control ctrl = _parent.LoadControl(url);

                // Bind all attributes
                foreach (string key in _columnTemplate.Attributes.Keys)
                {
                    ReflectionHelper.BindProperty(ctrl, key, _columnTemplate.Attributes[key]);
                }

                container.Controls.Add(ctrl);
            }

            #endregion

        }

        /// <summary>
        /// Creates the columns.
        /// </summary>
        /// <param name="grid">The grid.</param>
        /// <param name="appId">The app id.</param>
        /// <param name="viewId">The view id.</param>
        protected void CreateColumns(Grid grid, string appId, string viewId)
        {
            AdminView view = ManagementContext.Current.FindView(appId, viewId);

            if (view == null)
                return;

            // Remove columns
            if (grid.Levels[0].Columns.Count > 0)
                grid.Levels[0].Columns.Clear();

            // Add columns
            foreach (ViewColumn node in view.Columns)
            {
                // Check if the node is visible
                if (!IsVisible(node))
                    continue;

                BindTemplate(grid, view.Module, node);

                //if (!this.IsPostBack)
                {
                    GridColumn column = new GridColumn();

                    BindColumnProperties(column, node);
                    grid.Levels[0].Columns.Add(column);
                }
            }
        }

        /// <summary>
        /// Binds the template.
        /// </summary>
        /// <param name="grid">The grid.</param>
        /// <param name="module">The module.</param>
        /// <param name="column">The column.</param>
        private void BindTemplate(Grid grid, ModuleConfig module, ViewColumn column)
        {
            if (column.Template == null)
                return;

            // Remove any template with the same name
            //foreach (GridServerTemplate existingTemplate in grid.ServerTemplates)
			for (int index = 0; index < grid.ServerTemplates.Count; index++)
			{
				if (String.Compare(grid.ServerTemplates[index].ID, column.Template.TemplateId, true) == 0)
					grid.ServerTemplates.RemoveAt(index);
			}

            string url = String.Empty;
            ITemplate template = new SimpleTemplate((UserControl)grid.Parent, module.Name, column.Template);

            // Add server template to grid
            ComponentArt.Web.UI.GridServerTemplate gst = new ComponentArt.Web.UI.GridServerTemplate();
            gst.Template = (ITemplate)template;

            gst.ID = column.Template.TemplateId;

            BindColumnTemplateProperties(gst.Template, column.Template);
            grid.ServerTemplates.Add(gst);
            grid.Controls.Add(gst);
        }

        /// <summary>
        /// Binds the column properties.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <param name="node">The node.</param>
        private void BindColumnProperties(GridColumn column, ViewColumn node)
        {
            // Bind all attributes
            foreach (string key in node.Attributes.Keys)
            {
				if (String.Compare(key, ViewColumn._HeadingTextAttributeName, false) == 0)
					ReflectionHelper.BindProperty(column, key, node.GetLocalizedHeadingString());
				else
					ReflectionHelper.BindProperty(column, key, node.Attributes[key]);
            }
        }

        /// <summary>
        /// Binds the column template properties.
        /// </summary>
        /// <param name="template">The template.</param>
        /// <param name="node">The node.</param>
        private void BindColumnTemplateProperties(object template, ViewColumnTemplate node)
        {
            // Bind all attributes
            foreach (string key in node.Attributes.Keys)
            {
                ReflectionHelper.BindProperty(template, key, node.Attributes[key]);
            }
        }


        /// <summary>
        /// Determines whether the specified node is visible.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>
        /// 	<c>true</c> if the specified node is visible; otherwise, <c>false</c>.
        /// </returns>
        private bool IsVisible(ViewColumn node)
        {
            // TODO: implement personalization here
            return true; // always visible node.Visible;
        }
    }
}