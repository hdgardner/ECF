using System;
using System.Data;
using System.Configuration;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using ComponentArt.Web.UI;
using Mediachase.Cms;
using Mediachase.Cms.DataAdapter;
using Mediachase.Cms.Pages;
using Mediachase.Cms.WebActionSet;
using Mediachase.Cms.WebUtility.UI.Controls;

namespace Mediachase.Cms.Web.UI.Controls
{
    /// <summary>
    /// Summary description for PlaceHolderWrapper
    /// </summary>
    public class CmsPlaceHolder : CompositeDataBoundControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CmsPlaceHolder"/> class.
        /// </summary>
        public CmsPlaceHolder()
        {
        }

        /// <summary>
        /// Gets the <see cref="T:System.Web.UI.HtmlTextWriterTag"/> value that corresponds to this Web server control. This property is used primarily by control developers.
        /// </summary>
        /// <value></value>
        /// <returns>One of the <see cref="T:System.Web.UI.HtmlTextWriterTag"/> enumeration values.</returns>
        protected override HtmlTextWriterTag TagKey
        {
            get
            {
                return HtmlTextWriterTag.Div;
            }
        }

        #region SetRequireDataBind (public EnsureDataBind())
        /// <summary>
        /// Sets the require data bind.
        /// </summary>
        public void SetRequireDataBind()
        {
            this.RequiresDataBinding = true;
        }
        #endregion

        #region CreateChildControls
        /// <summary>
        /// When overridden in an abstract class, creates the control hierarchy that is used to render the composite data-bound control based on the values from the specified data source.
        /// </summary>
        /// <param name="dataSource">An <see cref="T:System.Collections.IEnumerable"></see> that contains the values to bind to the control.</param>
        /// <param name="dataBinding">true to indicate that the <see cref="M:System.Web.UI.WebControls.CompositeDataBoundControl.CreateChildControls(System.Collections.IEnumerable,System.Boolean)"></see> is called during data binding; otherwise, false.</param>
        /// <returns>
        /// The number of items created by the <see cref="M:System.Web.UI.WebControls.CompositeDataBoundControl.CreateChildControls(System.Collections.IEnumerable,System.Boolean)"></see>.
        /// </returns>
        protected override int CreateChildControls(System.Collections.IEnumerable dataSource, bool dataBinding)
        {
            int counter = 0;

            string _sourceInfo = string.Empty;

            if (dataBinding)
            {
                foreach (DynamicNode dn in dataSource)
                {
                    counter++;
                    this.Controls.Add(this.CreateDynamicControl(dn.NodeUID));
                    //ControlInfo ci = ciProvider.LoadControl(dn.FactoryControlUID);

                    _sourceInfo += dn.NodeUID + "^";
                }

                if (_sourceInfo.Length > 0)
                    _sourceInfo = _sourceInfo.Remove(_sourceInfo.Length - 1);

                this.ViewState[this.ID + "_sourceInfo"] = _sourceInfo;
            }
            else
            {
                if (this.ViewState[this.ID + "_sourceInfo"] == null)
                    throw new ArgumentNullException("SourceInfo");

                _sourceInfo = this.ViewState[this.ID + "_sourceInfo"].ToString();

                if (_sourceInfo.Length == 0)
                    return 0;

                foreach (string s in _sourceInfo.Split('^'))
                {
                    DynamicNode dn = Mediachase.Cms.Pages.PageDocument.Current.DynamicNodes.LoadByUID(s);
                    if (dn == null)
                    {
                        if (_sourceInfo.Contains(s + "^"))
                            _sourceInfo = _sourceInfo.Replace(s + "^", string.Empty);
                        else
                            _sourceInfo = _sourceInfo.Replace(s, string.Empty);

                        continue;
                    }

                    this.Controls.Add(this.CreateDynamicControl(s));



                    counter++;
                }

                this.ViewState[this.ID + "_sourceInfo"] = _sourceInfo;
            }


            return counter;
        }
        #endregion

        #region CreateDesignWrapper
        /// <summary>
        /// Creates the design wrapper.
        /// </summary>
        /// <param name="ctrl">The CTRL.</param>
        /// <param name="dn">The dn.</param>
        /// <returns></returns>
        protected virtual Control CreateDesignWrapper(Control ctrl, DynamicNode dn)
        {
            ComponentArt.Web.UI.Snap snap = new ComponentArt.Web.UI.Snap();

            snap.DockingContainers = CMSContext.Current.ControlPlaces;
            snap.DockingStyle = SnapDockingStyleType.TransparentRectangle;

            snap.MustBeDocked = true;
            snap.CollapseDuration = 300;
            snap.ExpandDuration = 300;

            Panel wControl = new Panel();
            snap.ID = dn.NodeUID;

            Image moveImg = new Image();
            Image moveDelete = new Image();
            Image propertyImg = new Image();

            wControl.Attributes.Add("ActionSet", "NoneMenu");

            wControl.Style.Add("position", "relative");
            wControl.Style.Add("z-index", "1");
            wControl.ID = dn.NodeUID + "_wControl";

            moveImg.Attributes.Add("ActionBtn", "Move");
            moveDelete.Attributes.Add("ActionBtn", "Delete");

            moveImg.CssClass = "imgMove";
            moveDelete.CssClass = "imgDelete";
            propertyImg.CssClass = "imgProperty";

            snap.CurrentDockingContainer = dn.ControlPlaceId;
            snap.CurrentDockingIndex = dn.ControlPlaceIndex;

            //add move button
            moveImg.ID = dn.NodeUID + "_move";
            moveImg.Attributes.Add("onmousedown", this.ClientID + "_" + snap.ClientID + ".StartDragging(event);");
			moveImg.ImageUrl = Mediachase.Commerce.Shared.CommerceHelper.GetAbsolutePath("/Images/move.gif");

            //add delete button
            //moveDelete.Attributes.Add("onclick", snap.ClientID + ".Collapse();MakeModified('del','" + PlaceHolderManager.GetCurrent(this.Page).hfDeletetedId + "','" + snap.ID + "');");
            moveDelete.ID = dn.NodeUID + "_deleted";
            //moveDelete.Attributes.Add("onclick", this.ClientID + "_" + snap.ClientID + ".Collapse(); FillDeleteSnapInfo('" + PlaceHolderManager.GetCurrent(this.Page).hfDeletetedId + "','" + dn.NodeUID + "'); ");
			moveDelete.Attributes.Add("onclick", this.ClientID + "_" + snap.ClientID + ".Collapse(); var _sh=$find('SnapHolder1_Snap'); if(_sh!=null) _sh.FillDeleteSnapInfo('" + PlaceHolderManager.GetCurrent(this.Page).hfDeletetedId + "','" + dn.NodeUID + "'); ");
			moveDelete.ImageUrl = Mediachase.Commerce.Shared.CommerceHelper.GetAbsolutePath("/Images/delete.gif");

            wControl.Controls.Add(moveImg);
            wControl.Controls.Add(moveDelete);

            string command = CreatePropertiesCommand(dn.NodeUID, dn.FactoryControlUID);
            if (!String.IsNullOrEmpty(command) && ctrl.GetType().FullName != "ASP.structure_base_controls_inlinebase_inlineeditor_ascx" && ctrl.GetType().FullName != "ASP.structure_base_controls_popupbase_popupeditor_ascx")
            {
				wControl.Attributes.Add("ondblclick", command);
            }

            SnapContent content = new SnapContent();
            content.Controls.Add(wControl);
            snap.Content = content;

            //add property page call
            //wControl.Attributes.Add("onclick", String.Format("EditControlHandler('{0}', '{1}');", PlaceHolderManager.GetCurrent(this.Page).hfEdited.ClientID, dn.NodeUID));
			wControl.Attributes.Add("onclick", String.Format(" var _sh=$find('SnapHolder1_Snap'); if(_sh!=null) _sh.EditControlHandler('{0}', '{1}');", PlaceHolderManager.GetCurrent(this.Page).hfEdited.ClientID, dn.NodeUID));
            wControl.Controls.Add(ctrl); // <---- crash

            dn.AssignedControl = snap;

            return snap;

        }

        /// <summary>
        /// Creates the command for opening the properties dialog. Returns empty string if no command exists.
        /// </summary>
        /// <param name="nodeUid">The node uid.</param>
        /// <param name="controlUid">The control uid.</param>
        /// <returns></returns>
        public static string CreatePropertiesCommand(string nodeUid, string controlUid)
        {
            string command = String.Empty;
            Mediachase.Cms.Controls.DynamicControlInfo ctrlInfo = Mediachase.Cms.Controls.DynamicControlFactory.GetControlInfo(controlUid);
            StringBuilder propertyString = new StringBuilder();
            if (ctrlInfo != null)
            {
                propertyString.Append(Mediachase.Commerce.Shared.CommerceHelper.GetAbsolutePath("~/Structure/Base/Controls/PropertyPage.aspx"));
                propertyString.Append("?PropertyPagePath=");
                propertyString.Append(ctrlInfo.PropertyPagePath);
                propertyString.Append("&NodeUID=");
                propertyString.Append(nodeUid);
                propertyString.Append("&ControlUID=");
                propertyString.Append(controlUid);
                // Sasha: added versionid so we can get the specific page settings
                propertyString.Append("&VersionId=");
                propertyString.Append(PageDocument.Current.PageVersionId);
                // Sasha: added siteid to better support multi site
                propertyString.Append("&siteid=");
                propertyString.Append(CMSContext.Current.SiteId);
            }

            // [2007/10/01] Nadya: added ctrlInfo.PropertyPagePath != null 
            // 2007/10/2 Sasha: added ctrlInfo != null
            if (ctrlInfo != null && ctrlInfo.PropertyPagePath != null)
            {
                //wControl.Attributes.Add("ondblclick", "PopUpWindow('" + propertyString.ToString() + "');");
                command = "var _uh=$find('MediachaseCmsUtil'); if(_uh!=null) _uh.PopUpWindow('" + propertyString.ToString() + "');";
            }

            return command;
        }

        /// <summary>
        /// Determines whether the control is Dynamic Control or not.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <returns>
        /// 	<c>true</c> if [is dynamic control] [the specified control]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsDynamicControl(Control control)
        {
            if (control.Parent != null && control.Parent.Parent != null && control.Parent.Parent.ID != null)
            {
                DynamicNode dn = PageDocument.Current.DynamicNodes.LoadByUID(control.Parent.Parent.ID.Replace("_wControl", ""));
                if (dn == null)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Registers the static control.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="wrapper">The wrapper.</param>
        /// <param name="controlUid">The control uid.</param>
        public static void RegisterStaticControl(Control control, IAttributeAccessor wrapper, string controlUid)
        {
            if (!IsDynamicControl(control))
            {
                ControlSettings cs = PageDocument.Current.StaticNode.Controls[control.ID];
                if (cs == null)
                {
                    cs = new ControlSettings();
                    PageDocument.Current.StaticNode.Controls.Add(control.ID, cs);
                }
                else
                {
                    if (control is ICmsDataAdapter)
                    {
                        ControlSettings settings = PageDocument.Current.StaticNode.Controls[control.ID];
                        ((ICmsDataAdapter)control).SetParamInfo(settings);
                    }
                }

                // Since it is a static control, add a properties dialog 
                if (CMSContext.Current.IsDesignMode && wrapper != null)
                {
                    string controlId = controlUid;
                    string command = CmsPlaceHolder.CreatePropertiesCommand(control.ID, controlId);
                    if (!String.IsNullOrEmpty(command))
                        wrapper.SetAttribute("ondblclick", command);
                }
            }
        }
        #endregion

        #region CreateDynamicControl
        /// <summary>
        /// Creates the dynamic control.
        /// </summary>
        /// <param name="DNodeUID">The D node UID.</param>
        /// <returns></returns>
        protected virtual Control CreateDynamicControl(string DNodeUID)
        {
            DynamicNode dn = Mediachase.Cms.Pages.PageDocument.Current.DynamicNodes.LoadByUID(DNodeUID);

            if (dn == null)
                throw new ArgumentNullException(String.Format("DynamicNodeUID: {0}", DNodeUID));

            Mediachase.Cms.Controls.DynamicControlInfo dci = Mediachase.Cms.Controls.DynamicControlFactory.GetControlInfo(dn.FactoryControlUID);
            Control c = new Control();

            if (dci != null)
            {
                c = this.Page.LoadControl(typeof(BaseErrorControl), null);  //Mediachase.Cms.Controls.DynamicControlFactory.Create(this.Page, dci.Uid);
                Control dynamicControl = Mediachase.Cms.Controls.DynamicControlFactory.Create(this.Page, dci.Uid);
                
                if (dynamicControl is ICmsDataAdapter)
                {
                    ControlSettings settings = new ControlSettings();
                    settings = dn.GetSettings(dn.NodeUID);
                    ((ICmsDataAdapter)dynamicControl).SetParamInfo(settings);
                }

                c.Controls.Add(dynamicControl);
            }
            else
            {
                c = this.Page.LoadControl("~/Structure/Base/Controls/Common/SysEmptyControl.ascx");
            }

            if (CMSContext.Current.IsDesignMode)
            {
                return this.CreateDesignWrapper(c, dn);
            }

            return c;
        }
        #endregion

        /// <summary>
        /// Handles the <see cref="E:System.Web.UI.Control.Init"></see> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"></see> object that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PlaceHolderManager.GetCurrent(this.Page).PlaceHolders.Add(this);
        }

        /// <summary>
        /// Handles the <see cref="E:System.Web.UI.Control.PreRender"></see> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"></see> object that contains the event data.</param>
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (this.RequiresDataBinding)
            {
                this.DataBind();
                this.RequiresDataBinding = false;
            }
        }

        /// <summary>
        /// Adds HTML attributes and styles that need to be rendered to the specified <see cref="T:System.Web.UI.HtmlTextWriterTag"/>. This method is used primarily by control developers.
        /// </summary>
        /// <param name="writer">A <see cref="T:System.Web.UI.HtmlTextWriter"/> that represents the output stream to render HTML content on the client.</param>
        protected override void AddAttributesToRender(HtmlTextWriter writer)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Id, this.ID);
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "CmsControlPlace");
        }
    }
}