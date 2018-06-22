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
using System.ComponentModel;

namespace Mediachase.Ibn.Web.UI.Layout
{
    public enum IbnDockType { Top, Left, Bottom, Right };

    public partial class IbnDock : System.Web.UI.UserControl
    {
        private const int DEFAULT_SIZE = 50;
        private const int DEFAULT_SPLIT_SIZE = 4;

        #region prop: DockContainer
        /// <summary>
        /// Gets the dock container.
        /// </summary>
        /// <value>The dock container.</value>
        public HtmlGenericControl DockContainer
        {
            get
            {
                return this.Container;
            }
        }
        #endregion

        #region prop: DockSplitter
        /// <summary>
        /// Gets the dock splitter.
        /// </summary>
        /// <value>The dock splitter.</value>
        public HtmlGenericControl DockSplitter
        {
            get
            {
                return this.Splitter;
            }
        }
        #endregion

        #region prop: LayoutId
        /// <summary>
        /// Gets or sets the layout id.
        /// </summary>
        /// <value>The layout id.</value>
        public string LayoutId
        {
            get
            {
                if (ViewState[this.ID + "_LayoutId"] != null)
                    return ViewState[this.ID + "_LayoutId"].ToString();

                return string.Empty;
            }
            set
            {
                ViewState[this.ID + "_LayoutId"] = value;
            }
        }
        #endregion

        #region prop: SplitterId
        /// <summary>
        /// Gets the splitter id.
        /// </summary>
        /// <value>The splitter id.</value>
        public string SplitterId
        {
            get
            {
                return Splitter.ClientID;
            }
        }
        #endregion

        #region prop: SplitterSizer
        /// <summary>
        /// Gets or sets the splitter sizer.
        /// </summary>
        /// <value>The splitter sizer.</value>
        public int SplitterSizer
        {
            get
            {
                if (!this.EnableSplitter)
                    return 0;

                if (ViewState[this.ID + "SplitterSize"] != null)
                    return int.Parse(ViewState[this.ID + "SplitterSize"].ToString());

                return DEFAULT_SPLIT_SIZE;
            }
            set
            {
                ViewState[this.ID + "SplitterSize"] = value;
            }
        }
        #endregion


        #region prop: DefaultSize
        /// <summary>
        /// Gets or sets the default size.
        /// </summary>
        /// <value>The default size.</value>
        public int DefaultSize
        {
            get
            {
                if (ViewState[this.ID + "_DefaultSize"] != null)
                    return int.Parse(ViewState[this.ID + "_DefaultSize"].ToString());

                return DEFAULT_SIZE;
            }
            set
            {
                ViewState[this.ID + "_DefaultSize"] = value;
            }
        }
        #endregion

        #region prop: Anchor
        /// <summary>
        /// Gets or sets the anchor.
        /// </summary>
        /// <value>The anchor.</value>
        [Bindable(true)]
        public IbnDockType Anchor
        {
            get
            {
                if (ViewState[this.ID + "_Anchor"] != null)
                    return (IbnDockType)Enum.Parse(typeof(IbnDockType), ViewState[this.ID + "_Anchor"].ToString());

                return IbnDockType.Left;
            }

            set
            {
                ViewState[this.ID + "_Anchor"] = value;
            }
        }
        #endregion

        #region prop: DockItems
        /// <summary>
        /// Gets the dock items.
        /// </summary>
        /// <value>The dock items.</value>
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public ControlCollection DockItems
        {
            get
            {
                return ControlsWrapper.Controls;
            }
        }
        #endregion

        #region prop: EnableSplitter
        /// <summary>
        /// Gets or sets a value indicating whether [enable splitter].
        /// </summary>
        /// <value><c>true</c> if [enable splitter]; otherwise, <c>false</c>.</value>
        public bool EnableSplitter
        {
            get
            {
                if (ViewState[this.ID + "_EnableSplitter"] != null)
                    return Convert.ToBoolean(ViewState[this.ID + "_EnableSplitter"]);
                return true;
            }
            set
            {
                ViewState[this.ID + "_EnableSplitter"] = value;
            }
        }
        #endregion

        #region prop: Left
        /// <summary>
        /// Gets or sets the left.
        /// </summary>
        /// <value>The left.</value>
        public int Left
        {
            get
            {
                if (ViewState[this.ID + "_Left"] != null)
                    return int.Parse(ViewState[this.ID + "_Left"].ToString());

                return 0;
            }
            set
            {
                ViewState[this.ID + "_Left"] = value;
            }
        }
        #endregion

        #region prop: Right
        /// <summary>
        /// Gets or sets the right.
        /// </summary>
        /// <value>The right.</value>
        public int Right
        {
            get
            {
                if (ViewState[this.ID + "_Right"] != null)
                    return int.Parse(ViewState[this.ID + "_Right"].ToString());

                return 0;
            }
            set
            {
                ViewState[this.ID + "_Right"] = value;
            }
        }
        #endregion

        #region prop: Bottom
        /// <summary>
        /// Gets or sets the bottom.
        /// </summary>
        /// <value>The bottom.</value>
        public int Bottom
        {
            get
            {
                if (ViewState[this.ID + "_Bottom"] != null)
                    return int.Parse(ViewState[this.ID + "_Bottom"].ToString());

                return 0;
            }
            set
            {
                ViewState[this.ID + "_Bottom"] = value;
            }
        }
        #endregion

        #region prop: Top
        /// <summary>
        /// Gets or sets the top.
        /// </summary>
        /// <value>The top.</value>
        public int Top
        {
            get
            {
                if (ViewState[this.ID + "_Top"] != null)
                    return int.Parse(ViewState[this.ID + "_Top"].ToString());

                return 0;
            }
            set
            {
                ViewState[this.ID + "_Top"] = value;
            }
        }
        #endregion

        #region prop: FriendlyName
        /// <summary>
        /// Gets or sets the name of the friendly.
        /// </summary>
        /// <value>The name of the friendly.</value>
        public string FriendlyName
        {
            get
            {
                if (ViewState[this.ID + "_FriendlyName"] != null)
                    return ViewState[this.ID + "_FriendlyName"].ToString();
                return string.Empty;
            }
            set
            {
                ViewState[this.ID + "_FriendlyName"] = value;
            }
        }
        #endregion

        #region prop: ShowScrolling
        /// <summary>
        /// Gets or sets a value indicating whether [show scrolling].
        /// </summary>
        /// <value><c>true</c> if [show scrolling]; otherwise, <c>false</c>.</value>
        public bool ShowScrolling
        {
            get
            {
                if (ViewState[this.ID + "_ShowScrolling"] != null)
                    return Convert.ToBoolean(ViewState[this.ID + "_ShowScrolling"].ToString());

                return false;
            }
            set
            {
                ViewState[this.ID + "_ShowScrolling"] = value;
            }
        }
        #endregion

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            Control ctrl = this;

            while (ctrl.Parent != null)
            {
                if (ctrl is IbnLayout)
                {
                    ((IbnLayout)ctrl).DockedItems.Add(this);
                    return;
                }
                ctrl = ctrl.Parent;
            }

            throw new ArgumentException("IbnDock must be placed inside IbnLayout");
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        //TO DO: set sizes(left/right/bottom/top)
        /// <summary>
        /// Handles the PreRender event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (this.ShowScrolling)
                Container.Style.Add("overflow-y", "scroll");

            if (HttpContext.Current.Request.Browser.Browser == "IE" && HttpContext.Current.Request.Browser.MajorVersion == 6)
            {
                InitSizeIE6();
            }
            else
            {
                InitSizes();
            }
        }

        /// <summary>
        /// Inits the sizes.
        /// </summary>
        private void InitSizes()
        {
            if (this.EnableSplitter)
            {
                Splitter.Attributes.Add("ibntyperegion", Container.ClientID);
                Splitter.Attributes.Add("ibnorientation", this.Anchor.ToString());
                Splitter.Attributes.Add("ibnfriendlyname", this.FriendlyName);
                Splitter.Style.Add("z-index", "100");
            }
            else
            {
                Splitter.Visible = false;
            }

            #region Init div size
            switch (this.Anchor)
            {
                #region IbnDockType.Bottom
                case IbnDockType.Bottom:
                    {
                        Container.Style.Add("left", String.Format("{0}px", this.Left));
                        Container.Style.Add("right", String.Format("{0}px", this.Right));
                        Container.Style.Add("bottom", String.Format("{0}px", this.Bottom));
                        Container.Style.Add("height", String.Format("{0}px", this.DefaultSize));

                        if (this.EnableSplitter)
                        {
                            Splitter.Style.Add("left", String.Format("{0}px", this.Left));
                            Splitter.Style.Add("right", String.Format("{0}px", this.Right));
                            Splitter.Style.Add("bottom", String.Format("{0}px", this.Bottom + this.DefaultSize));

                            Splitter.Style.Add("height", String.Format("{0}px", this.SplitterSizer));
                            //Splitter.Style.Add("cursor", "n-resize");
                            Splitter.Attributes.Add("class", "Splitter HorizontalSplitter");
                        }
                        break;
                    }
                #endregion

                #region IbnDockType.Top
                case IbnDockType.Top:
                    {
                        Container.Style.Add("left", String.Format("{0}px", this.Left));
                        Container.Style.Add("right", String.Format("{0}px", this.Right));
                        Container.Style.Add("top", String.Format("{0}px", this.Top));
                        Container.Style.Add("height", String.Format("{0}px", this.DefaultSize));

                        if (this.EnableSplitter)
                        {
                            Splitter.Style.Add("right", String.Format("{0}px", this.Right));
                            Splitter.Style.Add("top", String.Format("{0}px", this.Top + this.DefaultSize));
                            Splitter.Style.Add("left", String.Format("{0}px", this.Left));

                            Splitter.Style.Add("height", String.Format("{0}px", this.SplitterSizer));
                            //Splitter.Style.Add("cursor", "n-resize");
                            Splitter.Attributes.Add("class", "Splitter HorizontalSplitter");
                        }
                        break;
                    }

                #endregion

                #region IbnDockType.Left
                case IbnDockType.Left:
                    {
                        Container.Style.Add("left", String.Format("{0}px", this.Left));
                        Container.Style.Add("bottom", String.Format("{0}px", this.Bottom));
                        Container.Style.Add("top", String.Format("{0}px", this.Top));
                        Container.Style.Add("width", String.Format("{0}px", this.DefaultSize));

                        if (this.EnableSplitter)
                        {
                            Splitter.Style.Add("left", String.Format("{0}px", this.Left + this.DefaultSize));
                            Splitter.Style.Add("top", String.Format("{0}px", this.Top));
                            Splitter.Style.Add("bottom", String.Format("{0}px", this.Bottom));

                            Splitter.Style.Add("width", String.Format("{0}px", this.SplitterSizer));
                            //Splitter.Style.Add("cursor", "w-resize");
                            Splitter.Attributes.Add("class", "Splitter VerticalSplitter");
                        }
                        break;
                    }
                #endregion

                #region IbnDockType.Right
                case IbnDockType.Right:
                    {
                        Container.Style.Add("right", String.Format("{0}px", this.Right));
                        Container.Style.Add("bottom", String.Format("{0}px", this.Bottom));
                        Container.Style.Add("top", String.Format("{0}px", this.Top));
                        Container.Style.Add("width", String.Format("{0}px", this.DefaultSize));

                        if (this.EnableSplitter)
                        {
                            Splitter.Style.Add("right", String.Format("{0}px", this.Right + this.DefaultSize));
                            Splitter.Style.Add("top", String.Format("{0}px", this.Top));
                            Splitter.Style.Add("bottom", String.Format("{0}px", this.Bottom));

                            Splitter.Style.Add("width", String.Format("{0}px", this.SplitterSizer));
                            //Splitter.Style.Add("cursor", "w-resize");
                            Splitter.Attributes.Add("class", "Splitter VerticalSplitter");
                        }
                        break;
                    }
                #endregion
            }
            #endregion
        }

        /// <summary>
        /// Inits the size I e6.
        /// </summary>
        private void InitSizeIE6()
        {
            Container.Attributes.Add("ibnorientation", this.Anchor.ToString());

            if (this.EnableSplitter)
            {
                Splitter.Attributes.Add("ibntyperegion", Container.ClientID);
                Splitter.Attributes.Add("ibnorientation", this.Anchor.ToString());
                Splitter.Attributes.Add("ibnfriendlyname", this.FriendlyName);
                Splitter.Style.Add("z-index", "100");
            }
            else
            {
                Splitter.Visible = false;
            }

            Splitter.Style.Add("font-size", "0px");

            #region Init div size
            switch (this.Anchor)
            {
                #region IbnDockType.Bottom
                case IbnDockType.Bottom:
                    {
                        Container.Style.Add("left", String.Format("{0}px", this.Left));
                        Container.Style.Add("bottom", String.Format("{0}px", this.Bottom));

                        Container.Style.Add("height", String.Format("{0}px", this.DefaultSize));
                        Container.Attributes.Add("ibnwidth", String.Format("countWidth({0}, {1}, '{2}')", this.Left, this.Right, this.LayoutId));

                        if (this.EnableSplitter)
                        {
                            Splitter.Style.Add("left", String.Format("{0}px", this.Left));
                            Splitter.Style.Add("bottom", String.Format("{0}px", this.Bottom + this.DefaultSize));

                            Splitter.Style.Add("height", String.Format("{0}px", this.SplitterSizer));
                            Splitter.Attributes.Add("ibnwidth", String.Format("countWidth({0}, {1}, '{2}')", this.Left, this.Right, this.LayoutId));
                            Splitter.Style.Add("cursor", "n-resize");
                        }
                        break;
                    }
                #endregion

                #region IbnDockType.Top
                case IbnDockType.Top:
                    {
                        Container.Style.Add("left", String.Format("{0}px", this.Left));
                        Container.Style.Add("top", String.Format("{0}px", this.Top));
                        Container.Style.Add("height", String.Format("{0}px", this.DefaultSize));
                        Container.Attributes.Add("ibnwidth", String.Format("countWidth({0}, {1}, '{2}')", this.Left, this.Right, this.LayoutId));

                        if (this.EnableSplitter)
                        {
                            Splitter.Style.Add("top", String.Format("{0}px", this.Top + this.DefaultSize));
                            Splitter.Style.Add("left", String.Format("{0}px", this.Left));

                            Splitter.Style.Add("height", String.Format("{0}px", this.SplitterSizer));
                            Splitter.Attributes.Add("ibnwidth", String.Format("countWidth({0}, {1}, '{2}')", this.Left, this.Right, this.LayoutId));
                            Splitter.Style.Add("cursor", "n-resize");
                        }
                        break;
                    }

                #endregion

                #region IbnDockType.Left
                case IbnDockType.Left:
                    {
                        Container.Style.Add("left", String.Format("{0}px", this.Left));
                        Container.Style.Add("top", String.Format("{0}px", this.Top));
                        Container.Style.Add("width", String.Format("{0}px", this.DefaultSize));
                        Container.Attributes.Add("ibnheight", String.Format("countHeight({0}, {1}, '{2}')", this.Top, this.Bottom, this.LayoutId));

                        if (this.EnableSplitter)
                        {
                            Splitter.Style.Add("left", String.Format("{0}px", this.Left + this.DefaultSize));
                            Splitter.Style.Add("top", String.Format("{0}px", this.Top));

                            Splitter.Style.Add("width", String.Format("{0}px", this.SplitterSizer));
                            Splitter.Attributes.Add("ibnheight", String.Format("countHeight({0}, {1}, '{2}')", this.Top, this.Bottom, this.LayoutId));
                            //Splitter.Style.Add("cursor", "w-resize");
                            Splitter.Attributes.Add("class", "Splitter HorizontalSplitter");
                        }
                        break;
                    }
                #endregion

                #region IbnDockType.Right
                case IbnDockType.Right:
                    {
                        Container.Style.Add("right", String.Format("{0}px", this.Right));
                        Container.Style.Add("top", String.Format("{0}px", this.Top));
                        Container.Style.Add("width", String.Format("{0}px", this.DefaultSize));
                        Container.Attributes.Add("ibnheight", String.Format("countHeight({0}, {1}, '{2}')", this.Top, this.Bottom, this.LayoutId));

                        if (this.EnableSplitter)
                        {
                            Splitter.Style.Add("right", String.Format("{0}px", this.Right + this.DefaultSize));
                            Splitter.Style.Add("top", String.Format("{0}px", this.Top));

                            Splitter.Style.Add("width", String.Format("{0}px", this.SplitterSizer));
                            Splitter.Attributes.Add("ibnheight", String.Format("countHeight({0}, {1}, '{2}')", this.Top, this.Bottom, this.LayoutId));
                            Splitter.Style.Add("cursor", "w-resize");
                        }
                        break;
                    }
                #endregion
            }
            #endregion
        }

    }
}