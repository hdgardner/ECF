using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace Mediachase.Ibn.Web.UI.Layout
{
    public partial class IbnLayout : System.Web.UI.UserControl
    {
        private const int DEFAULT_BORDER_SIZE = 0;

        #region prop: ContainerId
        /// <summary>
        /// Gets the container id.
        /// </summary>
        /// <value>The container id.</value>
        public string ContainerId
        {
            get
            {
                return MainLayout.ClientID;
            }
        }
        #endregion

        #region prop: Items
        /// <summary>
        /// Gets the items.
        /// </summary>
        /// <value>The items.</value>
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public ControlCollection Items
        {
            get
            {
                return MainLayout.Controls;
            }
        }
        #endregion

        #region prop: DockedItems
        private ArrayList _dockedItems = null;
        /// <summary>
        /// Gets the docked items.
        /// </summary>
        /// <value>The docked items.</value>
        public ArrayList DockedItems
        {
            get
            {
                if (_dockedItems == null)
                    _dockedItems = new ArrayList();

                return _dockedItems;
            }
        }
        #endregion

        #region prop: ClientOnResize
        /// <summary>
        /// Gets or sets the client on resize.
        /// </summary>
        /// <value>The client on resize.</value>
        public string ClientOnResize
        {
            get
            {
                if (ViewState[this.ID + "_ClientOnResize"] != null)
                    return ViewState[this.ID + "_ClientOnResize"].ToString();

                return string.Empty;
            }
            set
            {
                ViewState[this.ID + "_ClientOnResize"] = value;
            }
        }
        #endregion

        #region prop: BorderSize
        /// <summary>
        /// Gets or sets the size of the border.
        /// </summary>
        /// <value>The size of the border.</value>
        public int BorderSize
        {
            get
            {
                if (ViewState[this.ID + "BorderSize"] != null)
                    return int.Parse(ViewState[this.ID + "BorderSize"].ToString());

                return DEFAULT_BORDER_SIZE;
            }
            set
            {
                ViewState[this.ID + "BorderSize"] = value;
            }
        }
        #endregion

        public int marginLeft = 0;
        public int marginRight = 0;
        public int marginTop = 0;
        public int marginBottom = 0;

        private int dockIndex = 0;

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Page.LoadComplete += new EventHandler(Page_LoadComplete);
        }

        /// <summary>
        /// Handles the LoadComplete event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void Page_LoadComplete(object sender, EventArgs e)
        {
            for (int i = 0; i < this.DockedItems.Count; i++)
            {
                if (this.DockedItems[i] is IbnDock)
                {
                    IbnDock ctrl = (IbnDock)this.DockedItems[i];
                    InitSizes(ctrl);

                    if (ctrl.Anchor == IbnDockType.Bottom)
                    {
                        marginBottom += ctrl.DefaultSize + ctrl.SplitterSizer;
                        if (ctrl.EnableSplitter)
                            marginBottom += this.BorderSize;
                    }
                    if (ctrl.Anchor == IbnDockType.Top)
                    {
                        marginTop += ctrl.DefaultSize + ctrl.SplitterSizer;
                        if (ctrl.EnableSplitter)
                            marginTop += this.BorderSize;
                    }
                    if (ctrl.Anchor == IbnDockType.Right)
                    {
                        marginRight += ctrl.DefaultSize + ctrl.SplitterSizer;
                        if (ctrl.EnableSplitter)
                            marginRight += this.BorderSize;
                    }
                    if (ctrl.Anchor == IbnDockType.Left)
                    {
                        marginLeft += ctrl.DefaultSize + ctrl.SplitterSizer;
                        if (ctrl.EnableSplitter)
                            marginLeft += this.BorderSize;
                    }

                    ctrl.DockContainer.Attributes.Add("ibnindex", dockIndex.ToString());
                    dockIndex++;
                    ctrl.DockSplitter.Attributes.Add("ibnindex", dockIndex.ToString());
                    dockIndex++;
                }
            }

            MainLayout.Style.Add("margin-left", string.Format("{0}px", marginLeft));
            MainLayout.Style.Add("margin-right", string.Format("{0}px", marginRight));
            MainLayout.Style.Add("margin-top", string.Format("{0}px", marginTop));
            MainLayout.Style.Add("margin-bottom", string.Format("{0}px", marginBottom));

        }

        /// <summary>
        /// Inits the sizes.
        /// </summary>
        /// <param name="Control">The control.</param>
        private void InitSizes(IbnDock Control)
        {
            Control.Left = marginLeft;
            Control.Right = marginRight;
            Control.Top = marginTop;
            Control.Bottom = marginBottom;
            Control.LayoutId = this.ContainerId;
        }

        /// <summary>
        /// Handles the PreRender event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_PreRender(object sender, EventArgs e)
        {
        }

    }
}
