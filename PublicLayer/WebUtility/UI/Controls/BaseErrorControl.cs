using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI;
using Mediachase.Cms;


namespace Mediachase.Cms.WebUtility.UI.Controls
{
    public class BaseErrorControl : UserControl
    {
        /// <summary>
        /// Errors the control handler.
        /// </summary>
        /// <param name="e">The e.</param>
        /// <param name="place">The place.</param>
        private void ErrorControlHandler(Exception e, string place)
        {
            this.Controls.Clear();

            if (CMSContext.Current.IsDesignMode)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("<span style='color: Red'><b>Error phase:</b></span> {0}", place);
                sb.AppendFormat("<br/> <span style='color: Red'><b>Error message:</b></span> {0}", e.Message);
                sb.AppendFormat("<br/> <span style='color: Red'><b>Error stack:</b></span> {0}", e.StackTrace);
                this.Controls.Add(this.Page.LoadControl(typeof(SysErrorControl), new object[] { sb.ToString() }));
            }
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="BaseErrorControl"/> class.
        /// </summary>
        public BaseErrorControl()
        {
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"></see> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"></see> object that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            try
            {
                //this.Controls.Add(Mediachase.Cms.Controls.DynamicControlFactory.Create(this.Page, this.Uid));
                base.OnInit(e);
            }
            catch(Exception ex)
            {
                ErrorControlHandler(ex, "OnInit");
            }
        }

        /// <summary>
        /// Called by the ASP.NET page framework to notify server controls that use composition-based implementation to create any child controls they contain in preparation for posting back or rendering.
        /// </summary>
        protected override void CreateChildControls()
        {
            try
            {
                base.CreateChildControls();
            }
            catch (Exception e)
            {
                ErrorControlHandler(e, "CreateChildControls");
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load"></see> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"></see> object that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            try
            {
                base.OnLoad(e);
            }
            catch (Exception ex)
            {
                ErrorControlHandler(ex, "OnLoad");
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.PreRender"></see> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"></see> object that contains the event data.</param>
        protected override void OnPreRender(EventArgs e)
        {
            try
            {
                base.OnPreRender(e);
            }
            catch (Exception ex)
            {
                ErrorControlHandler(ex, "OnPreRender");
            }
        }

        /// <summary>
        /// Binds a data source to the invoked server control and all its child controls.
        /// </summary>
        public override void DataBind()
        {
            try
            {
                base.DataBind();
            }
            catch (Exception e)
            {
                ErrorControlHandler(e, "DataBind");
            }
        }

        /// <summary>
        /// Determines whether the server control contains child controls. If it does not, it creates child controls.
        /// </summary>
        protected override void EnsureChildControls()
        {
            try
            {
                base.EnsureChildControls();
            }
            catch (Exception e)
            {
                ErrorControlHandler(e, "EnsureChildControls");
            }
        }

        /// <summary>
        /// Sends server control content to a provided <see cref="T:System.Web.UI.HtmlTextWriter"></see> object, which writes the content to be rendered on the client.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Web.UI.HtmlTextWriter"></see> object that receives the server control content.</param>
        protected override void Render(HtmlTextWriter writer)
        {
            try
            {
                base.Render(writer);
            }
            catch (Exception e)
            {
                ErrorControlHandler(e, "Render");
            }
        }

        /// <summary>
        /// Binds a data source to the invoked server control and all its child controls with an option to raise the <see cref="E:System.Web.UI.Control.DataBinding"></see> event.
        /// </summary>
        /// <param name="raiseOnDataBinding">true if the <see cref="E:System.Web.UI.Control.DataBinding"></see> event is raised; otherwise, false. The default is false.</param>
        protected override void DataBind(bool raiseOnDataBinding)
        {
            try
            {
                base.DataBind(raiseOnDataBinding);
            }
            catch (Exception e)
            {
                ErrorControlHandler(e, "DataBind");
            }
        }

        /// <summary>
        /// Binds a data source to the server control's child controls.
        /// </summary>
        protected override void DataBindChildren()
        {
            try
            {
                base.DataBindChildren();
            }
            catch (Exception e)
            {
                ErrorControlHandler(e, "DataBindChildren");
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.DataBinding"></see> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"></see> object that contains the event data.</param>
        protected override void OnDataBinding(EventArgs e)
        {
            try
            {
                base.OnDataBinding(e);
            }
            catch (Exception ex)
            {
                ErrorControlHandler(ex, "OnDataBinding");
            }
        }

        /// <summary>
        /// Outputs the content of a server control's children to a provided <see cref="T:System.Web.UI.HtmlTextWriter"></see> object, which writes the content to be rendered on the client.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Web.UI.HtmlTextWriter"></see> object that receives the rendered content.</param>
        protected override void RenderChildren(HtmlTextWriter writer)
        {
            try
            {
                base.RenderChildren(writer);
            }
            catch (Exception e)
            {
                ErrorControlHandler(e, "RenderChildren");
            }
        }

    }
}
