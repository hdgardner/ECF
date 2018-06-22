using System;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Text;
using System.Collections.Generic;
using System.Globalization;

namespace Mediachase.Ibn.Web.UI.Layout
{
	#region class: WsButton
	public class WsButton
	{
		#region prop: ClientId
		private string _clientId;
		public string ClientId
		{
			get { return _clientId; }
			set { _clientId = value; }
		}
		#endregion

		#region prop: ButtonType
		private string _buttonType;
		public string ButtonType
		{
			get { return _buttonType; }
			set { _buttonType = value; }
		}
		#endregion

		#region .ctor
		public WsButton()
		{
		}

		public WsButton(string ClientId)
			: this()
		{
			this.ClientId = ClientId;
		}

		public WsButton(string ClientId, string ButtonType)
			: this(ClientId)
		{
			this.ButtonType = ButtonType;
		}
		#endregion
	}
	#endregion

	public class IbnControlPlace : CompositeDataBoundControl
	{
		#region prop: controlsJson
        /// <summary>
        /// Gets or sets the controls json.
        /// </summary>
        /// <value>The controls json.</value>
		private string controlsJson
		{
			get
			{
				if (ViewState["__controlsJson"] != null)
					return ViewState["__controlsJson"].ToString();

				return string.Empty;
			}
			set
			{
				ViewState["__controlsJson"] = value;
			}
		} 
		#endregion

		#region prop: WidthPercentage
        /// <summary>
        /// Gets or sets the width percentage.
        /// </summary>
        /// <value>The width percentage.</value>
		public int WidthPercentage
		{
			get
			{
				if (ViewState["_WidthPercentage"] != null)
					return Convert.ToInt32(ViewState["_WidthPercentage"].ToString());

				return 50;
			}
			set { ViewState["_WidthPercentage"] = value; }
		}
		#endregion

		#region prop: ControlPlaceId
        /// <summary>
        /// Gets or sets the control place id.
        /// </summary>
        /// <value>The control place id.</value>
		public string ControlPlaceId
		{
			get
			{
				if (ViewState["__ControlPlaceId"] != null)
					return ViewState["__ControlPlaceId"].ToString();

				return string.Empty;
			}
			set
			{
				ViewState["__ControlPlaceId"] = value;
			}
		}
		#endregion

		#region bindControl
		/// <summary>
        /// Binds the control.
        /// </summary>
        /// <param name="dci">The dci.</param>
        /// <param name="collapsed">The collapsed.</param>
        /// <param name="counter">The counter.</param>
        /// <param name="sourceInfo">The source info.</param>
        /// <returns></returns>
		private string bindControl(DynamicControlInfo dci, string collapsed, int counter, string instanceUid, ref string sourceInfo)
		{
			StringBuilder sb = new StringBuilder();
			Control ctrl;

			if (dci != null)
				ctrl = DynamicControlFactory.Create(this.Page, dci.Uid);
			else
				ctrl = this.Page.LoadControl("~/" + dci.Uid.Replace("..\\", string.Empty).Replace("\\", "/"));

			// if control has been removed, return
			if (ctrl == null)
				return string.Empty;

			HtmlGenericControl divContainer = new HtmlGenericControl("div");
			HtmlGenericControl divHeader = new HtmlGenericControl("div");

			#region Image ExpandCollapse
			ImageButton imgOpen = new ImageButton();
			imgOpen.ID = String.Format("imgOpen_{0}", counter);
			if (!Convert.ToBoolean(collapsed, CultureInfo.InvariantCulture))
			{
				imgOpen.ImageUrl = this.ResolveUrl("~/Apps/Core/Images/btn_up.gif");
				imgOpen.Attributes.Add("changeUrl", this.ResolveUrl("~/Apps/Core/Images/btn_down.gif"));
			}
			else
			{
				imgOpen.ImageUrl = this.ResolveUrl("~/Apps/Core/Images/btn_down.gif");
				imgOpen.Attributes.Add("changeUrl", this.ResolveUrl("~/Apps/Core/Images/btn_up.gif"));
			}
			imgOpen.Attributes.Add("class", "IbnHeaderWidgetButton");

			imgOpen.Style.Add("right", "25px");
			imgOpen.OnClientClick = "return false;";
			#endregion

			#region Image Close
			ImageButton imgClose = new ImageButton();
			imgClose.ID = String.Format("imgClose_{0}", counter);
			imgClose.ImageUrl = this.ResolveUrl("~/Apps/Core/Images/btn_close.gif");
			imgClose.Attributes.Add("class", "IbnHeaderWidgetButton");
			imgClose.Style.Add("right", "5px");
			imgClose.OnClientClick = "return false;";
			#endregion

			#region Image PropertyPage
			ImageButton imgProperty = new ImageButton();
			imgProperty.ID = String.Format("imgProperty_{0}", counter);
			imgProperty.ImageUrl = this.ResolveUrl("~/Apps/Core/Images/btn_prop.gif");
			imgProperty.Attributes.Add("class", "IbnHeaderWidgetButton");
			imgProperty.Style.Add("right", "45px");
			imgProperty.OnClientClick = "return false;";
			#endregion

			#region Label Title
			Label lblTitle = new Label();
			lblTitle.CssClass = "x-panel-header IbnHeaderWidgetButton";
			lblTitle.Style.Add("left", "2px");
			lblTitle.Style.Add("top", "1px");
			lblTitle.Style.Add("right", "75px");
			lblTitle.Style.Add(HtmlTextWriterStyle.BorderWidth, "0px");
			#endregion

			List<WsButton> buttonList = new List<WsButton>();

			divHeader.Attributes.Add("class", "IbnWidgetHeader");
			divHeader.Attributes.Add("dragObj", "0");

			ctrl.ID = String.Format("wrapControl{0}_{1}", dci.Uid.Replace("-", ""), instanceUid);
			IbnWidgetContainer c = new IbnWidgetContainer(ctrl, Convert.ToBoolean(collapsed, CultureInfo.InvariantCulture));
			c.ID = String.Format("id{0}{1}", dci.Uid.Replace("-", ""), counter);
			//this.Controls.Add(c);
			//c.DataBind();

			divContainer.Controls.Add(divHeader);
			divContainer.Controls.Add(c);
			this.Controls.Add(divContainer);

			c.DataBind();

			sourceInfo += String.Format("{0}^{1}^{2}:", dci.Uid, collapsed, instanceUid); // _uid + ":";

			System.Web.Script.Serialization.JavaScriptSerializer jss = new System.Web.Script.Serialization.JavaScriptSerializer();
			if (dci != null)
			{
				divHeader.Controls.Add(imgClose);
				divHeader.Controls.Add(imgOpen);
				divHeader.Controls.Add(lblTitle);
				buttonList.Add(new WsButton(imgClose.ClientID, "close"));
				buttonList.Add(new WsButton(imgOpen.ClientID, "expand"));
				lblTitle.Text = CHelper.GetResFileString(dci.Title);

				if (!string.IsNullOrEmpty(dci.PropertyPagePath ) || !string.IsNullOrEmpty(dci.PropertyPageType))
				{
					divHeader.Controls.Add(imgProperty);
					buttonList.Add(new WsButton(imgProperty.ClientID, "property"));
					sb.AppendFormat("{{ title: '{2}', tools: layoutExtender_tools2, contentEl: '{0}', id:'{4}_{1}', collapsed:{3}, buttons:{5} }},", c.ClientID, instanceUid, CHelper.GetResFileString(dci.Title), collapsed, dci.Uid, jss.Serialize(buttonList));
				}
				else
				{
					sb.AppendFormat("{{ title: '{2}', tools: layoutExtender_tools, contentEl: '{0}', id:'{4}_{1}', collapsed:{3}, buttons:{5} }},", c.ClientID, instanceUid, CHelper.GetResFileString(dci.Title), collapsed, dci.Uid, jss.Serialize(buttonList));
				}
			}
			else
			{
				sb.AppendFormat("{{ title: '', tools: layoutExtender_tools, contentEl: '{0}', id:'{3}_{1}', collapsed:{2} }},", c.ClientID, instanceUid, collapsed, dci.Uid.Replace("..\\", string.Empty).Replace("\\", "/"));
			}

			return sb.ToString();
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
			controlsJson = string.Empty;

			string _sourceInfo = string.Empty;
			int cpPartsCount = 3;

			if (dataBinding)
			{
				foreach (string val in dataSource)
				{
					if (val == string.Empty || val.Split('^').Length != cpPartsCount)
						continue;

					string _uid = val.Split('^')[0];
					string _collapsed = val.Split('^')[1].ToLowerInvariant();
					string _instanceUid = val.Split('^')[2].ToLowerInvariant();

					counter++;

					DynamicControlInfo dci = DynamicControlFactory.GetControlInfo(_uid);
					
					//fix when user has deleted controls
					if (dci == null)
						continue;

					controlsJson += this.bindControl(dci, _collapsed, counter, _instanceUid, ref _sourceInfo);
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

				foreach (string val in _sourceInfo.Split(':'))
				{
					if (val == string.Empty || val.Split('^').Length != cpPartsCount)
						continue;

					string _uid = val.Split('^')[0];
					string _collapsed = val.Split('^')[1].ToLowerInvariant();
					string _instanceUid = val.Split('^')[2].ToLowerInvariant();

					counter++;

					DynamicControlInfo dci = DynamicControlFactory.GetControlInfo(_uid);
					controlsJson += this.bindControl(dci, _collapsed, counter, _instanceUid, ref _sourceInfo);
				}

			}

			if (controlsJson.Length > 0)
				controlsJson = controlsJson.Remove(controlsJson.Length - 1);

			return counter;
		} 
		#endregion

		#region OnInit
        /// <summary>
        /// Handles the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
		protected override void OnInit(EventArgs e)
		{
			IbnControlPlaceManager.GetCurrent(this.Page).ControlPlaces.Add(this);
			base.OnInit(e);
		}
		#endregion

		#region GetItemsJson
        /// <summary>
        /// Gets the items json.
        /// </summary>
        /// <returns></returns>
		public string GetItemsJson()
		{
			StringBuilder sb = new StringBuilder();

			if (this.Page.Request.Browser.Browser.Contains("IE"))
			{
				if (this.WidthPercentage < 100)
					sb.AppendFormat("{{ columnWidth:.{0}, style:'padding:0px 0px 0px 0px', id: '{1}', clientId: '{2}' ", this.WidthPercentage, this.ID, this.ClientID);
				else
					sb.AppendFormat("{{ columnWidth:1, style:'padding:5px 0px 5px 10px', id: '{0}', clientId: '{1}' ", this.ID, this.ClientID);
			}
			else
			{
				if (this.WidthPercentage < 100)
					sb.AppendFormat("{{ columnWidth:.{0}, style:'padding:5px 0px 5px 10px', id: '{1}', clientId: '{2}' ", this.WidthPercentage, this.ID, this.ClientID);
				else
					sb.AppendFormat("{{ columnWidth:1, style:'padding:5px 0px 5px 10px', id: '{0}', clientId: '{1}' ", this.ID, this.ClientID);
			}

			if (!String.IsNullOrEmpty(controlsJson))
				sb.AppendFormat(", items: [{0}]", controlsJson);
			sb.Append("}");

			return sb.ToString();
		}
		#endregion

		#region OnLoad
        /// <summary>
        /// Handles the <see cref="E:System.Web.UI.Control.Load"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains event data.</param>
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			//this.EnsureChildControls();
		} 
		#endregion

		protected override void Render(HtmlTextWriter writer)
		{
			writer.AddAttribute("id", this.ClientID);
			writer.AddStyleAttribute(HtmlTextWriterStyle.Width, this.WidthPercentage + "%");
			writer.AddStyleAttribute("float", "left");
			writer.AddAttribute(HtmlTextWriterAttribute.Class, "columnContainer");
			writer.RenderBeginTag("div");

			base.RenderChildren(writer);

			writer.RenderEndTag();
		}
	}
}
