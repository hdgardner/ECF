using System;
using System.Data;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace Mediachase.Ibn.Web.UI.Layout
{
	public class IbnWidgetContainer : CompositeDataBoundControl
	{
		//private UpdatePanel _up = null;
		private Control _wrapControl = null;
        /// <summary>
        /// Gets or sets the wrap control.
        /// </summary>
        /// <value>The wrap control.</value>
		public Control WrapControl
		{
			get
			{
				return _wrapControl;
			}
			set
			{
				_wrapControl = value;
			}
		}

		#region prop: Collapsed
		private bool _collapsed = false;
		public bool Collapsed
		{
			get { return _collapsed; }
			set { _collapsed = value; }
		}
		#endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="IbnWidgetContainer"/> class.
        /// </summary>
		public IbnWidgetContainer()
		{
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="IbnWidgetContainer"/> class.
        /// </summary>
        /// <param name="c">The c.</param>
		public IbnWidgetContainer(Control c) : this()
		{
			this.WrapControl = c;
		}

		public IbnWidgetContainer(Control c, bool Collapsed)
			: this(c)
		{
			this.Collapsed = Collapsed;
		}

        /// <summary>
        /// Renders the control to the specified HTML writer.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Web.UI.HtmlTextWriter"/> object that receives the control content.</param>
		protected override void Render(HtmlTextWriter writer)
		{
			writer.AddAttribute("id", this.ClientID);
			writer.AddAttribute("class", "IbnWidgetContainer");
			if (this.Collapsed)
				writer.AddStyleAttribute(HtmlTextWriterStyle.Display, "none");

			writer.RenderBeginTag("div");
			
			this.RenderContents(writer);		
			
			writer.RenderEndTag();
		}

        /// <summary>
        /// When overridden in an abstract class, creates the control hierarchy that is used to render the composite data-bound control based on the values from the specified data source.
        /// </summary>
        /// <param name="dataSource">An <see cref="T:System.Collections.IEnumerable"/> that contains the values to bind to the control.</param>
        /// <param name="dataBinding">true to indicate that the <see cref="M:System.Web.UI.WebControls.CompositeDataBoundControl.CreateChildControls(System.Collections.IEnumerable,System.Boolean)"/> is called during data binding; otherwise, false.</param>
        /// <returns>
        /// The number of items created by the <see cref="M:System.Web.UI.WebControls.CompositeDataBoundControl.CreateChildControls(System.Collections.IEnumerable,System.Boolean)"/>.
        /// </returns>
		protected override int CreateChildControls(System.Collections.IEnumerable dataSource, bool dataBinding)
		{
			if (this.WrapControl != null)
			{
				UpdatePanel up = new UpdatePanel();
				
				up.ID = String.Format("up_{0}", this.WrapControl.ID);
				up.ChildrenAsTriggers = true;
				up.EnableViewState = true;
				up.UpdateMode = UpdatePanelUpdateMode.Conditional;
				up.ContentTemplateContainer.Controls.Add(this.WrapControl);

				this.Controls.Add(up);
				return 1;
			}
			else
			{
				return 0;
			}
		}
	}
}