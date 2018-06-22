using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using ComponentArt.Web.UI;
using Mediachase.Cms.Controls;
using Mediachase.Cms;
using Mediachase.Cms.Web;
using Mediachase.Cms.Web.UI.Controls;

public partial class ToolBox : System.Web.UI.UserControl, IScriptControl
{
	private int _counter = 0;

	#region OldSnapInfo
	public struct OldSnapInfo
	{
		public Snap _snap;
		public string _oldContainerId;
		public int _oldIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="OldSnapInfo"/> struct.
        /// </summary>
        /// <param name="snap">The snap.</param>
        /// <param name="oldContainerId">The old container id.</param>
        /// <param name="oldIndex">The old index.</param>
		public OldSnapInfo(Snap snap, string oldContainerId, int oldIndex)
		{
			this._snap = snap;
			this._oldContainerId = oldContainerId;
			this._oldIndex = oldIndex;
		}
	}

	ArrayList _oldSnapArray = new ArrayList();
	bool _rebindSnapFlag = false;
	#endregion

    #region GeneratedId

    private string _generatedId;

    /// <summary>
    /// Gets or sets the generated id.
    /// </summary>
    /// <value>The generated id.</value>
    public string generatedId
    {
        get { return _generatedId; }
        set { _generatedId = value; }
    }
    #endregion

	#region Page_Load
    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void Page_Load(object sender, EventArgs e)
    {
		if (!this.Visible)
			return;
        
        if (Request.Cookies["showimageT"] != null && !String.IsNullOrEmpty(Request.Cookies["showimageT"].Value))
            tboxLocation.Value = Request.Cookies["showimageT"].Value;

		/*string scriptKey = "initTboxLocation" + this.ClientID;
		if (!this.Page.ClientScript.IsStartupScriptRegistered(scriptKey))
			this.Page.ClientScript.RegisterStartupScript(this.GetType(), scriptKey, "<script language='javascript'>initTb('showimageT','" + tboxLocation.ClientID + "');initTbLocation('showimageT');</script>");*/
		
        LoadControlInfo2();
		tdToolboxHeader.Attributes.Add("onclick", "var _tb=$find('" + this.ClientID + "_Toolbox" + "'); if(_tb!=null) {_tb.MinimizeToolBox('showimageT');}");
    }
    #endregion

    /// <summary>
    /// Handles the PreRender event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
	protected void Page_PreRender(object sender, EventArgs e)
	{
		if (_rebindSnapFlag)
		{
			foreach (OldSnapInfo snapInfo in _oldSnapArray)
			{
				snapInfo._snap.CurrentDockingContainer = snapInfo._oldContainerId;
				snapInfo._snap.CurrentDockingIndex = snapInfo._oldIndex;
			}

			_rebindSnapFlag = false;
		}
	}

    /// <summary>
    /// Sends server control content to a provided <see cref="T:System.Web.UI.HtmlTextWriter"/> object, which writes the content to be rendered on the client.
    /// </summary>
    /// <param name="writer">The <see cref="T:System.Web.UI.HtmlTextWriter"/> object that receives the server control content.</param>
    protected override void Render(HtmlTextWriter writer)
    {
        if (this.Visible)
            base.Render(writer);
    }

	#region LoadControlInfo2 (new)
	/// <summary>
	/// Loads the control info2.
	/// </summary>
	private void LoadControlInfo2()
	{
		Table table = new Table();
		table.CellPadding = 0;
		table.CellSpacing = 0;
		table.CssClass = "NavBarInner";

		ContentPlaceholder.Controls.Add(table);
		int i = 0;

		foreach (Mediachase.Cms.Controls.DynamicControlCategory category in Mediachase.Cms.Controls.DynamicControlFactory.GetControlInfosByCategory())
		{
			TableRow trContent = new TableRow();
			TableCell tcContent = new TableCell();

			trContent.Cells.Add(tcContent);

			#region Header table row
			Table tHeader = new Table();
			table.Width = Unit.Percentage(100);

			TableRow trHeader = new TableRow();
			TableRow tr = new TableRow();
			TableCell tc = new TableCell();
			TableCell tcHeader = new TableCell();
			TableCell tcMinimize = new TableCell();


			trHeader.Cells.Add(tc);

			tc.Controls.Add(tHeader);
			tHeader.Rows.Add(tr);
			tr.Cells.Add(tcHeader);
			tr.Cells.Add(tcMinimize);

			table.Rows.Add(trHeader);
			table.Rows.Add(trContent);

			trHeader.Attributes.Add("onclick", /*"minimizeBox('" + trContent.ClientID + "'); "+*/"var _tb=$find('"+this.ClientID+"_Toolbox"+"'); if(_tb!=null) {_tb.MinimizeBox('" + trContent.ClientID + "');}");

			tcMinimize.CssClass = "btnMinimizeNavBar";
			tcHeader.Width = Unit.Percentage(91);

			tcHeader.Attributes.Add("onmouseover", "var obj_1 = document.getElementById('" + tcHeader.ClientID + "'); obj_1.className='TopItemHover';");
			tcHeader.Attributes.Add("onmouseout", "var obj_1 = document.getElementById('" + tcHeader.ClientID + "'); obj_1.className='TopItem';");


			Image image = new Image();
			//image.ImageUrl = Mediachase.Commerce.Shared.CommerceHelper.GetAbsolutePath("Controls/ToolBox/ImageLoader.aspx") + "?IconId=" + category.IconId;
			//image.ImageUrl = category.

			//image.ToolTip = category.Description;
			//image.Height = 14;
			//image.Width = 14;
			Label label = new Label();

			label.Text = category.Name;
			//label.ToolTip = category.Name;

			//tcHeader.Controls.Add(image);
			tcHeader.Controls.Add(label);

			tc.CssClass = "Level1Item";
			#endregion

			#region Content table row
			tcContent.CssClass = "Level2Item";

			Table tableContent = new Table();
			tcContent.Controls.Add(tableContent);

			IList<DynamicControlInfo> controlCollection = category.DynamicControlInfos;

			if (controlCollection != null)
			{
				foreach (DynamicControlInfo controlInfo in controlCollection)
				{
					_counter++;
					GenerateAddableSnap2(tcContent.ClientID, controlInfo);
				}
			}
			i++;
			#endregion
		}
	}
	#endregion

	#region GenerateAddableSnap2(new)
    /// <summary>
    /// Generates the addable snap2.
    /// </summary>
    /// <param name="dockingId">The docking id.</param>
    /// <param name="controlInfo">The control info.</param>
    /// <returns></returns>
	private Snap GenerateAddableSnap2(string dockingId, Mediachase.Cms.Controls.DynamicControlInfo controlInfo)
	{
		if (CMSContext.Current.ControlPlaces == string.Empty)
			return null;

		Snap snap = new Snap();
		snap.DockingContainers = CMSContext.Current.ControlPlaces;

		snap.DockingStyle = SnapDockingStyleType.TransparentRectangle;
		snap.MustBeDocked = true;
		snap.CollapseDuration = 300;
		snap.ExpandDuration = 300;
		snap.AutoPostBackOnDock = true;
		snap.CurrentDockingContainer = dockingId;
		snap.Dock += new Snap.DockEventHandler(Snap_Dock);

		//Control control = Mediachase.Cms.Controls.DynamicControlFactory.Create(this.Page, controlInfo.UID);

		Panel panel = new Panel();
		Label label = new Label();
		Image image = new Image();
		if (!string.IsNullOrEmpty(controlInfo.IconPath))
		{
			image.ImageUrl = Mediachase.Commerce.Shared.CommerceHelper.GetAbsolutePath(controlInfo.IconPath);
			image.Height = 16;
			image.Width = 16;
			panel.Controls.Add(image);
		}
		label.Text = controlInfo.Title;

		panel.ID = "cntrl" + (Guid.NewGuid().ToString()).Replace("-", string.Empty);
		panel.Controls.Add(label);

        SnapContent content = new SnapContent();
        content.Controls.Add(panel);
        snap.Content = content;
        //snap.Content = panel;
		
		//DV: 2007-09-12
		snap.Attributes.Add("ControlUID", controlInfo.Uid);

		_oldSnapArray.Add(new OldSnapInfo(snap, snap.CurrentDockingContainer, snap.CurrentDockingIndex));

		DockingTemp.Controls.Add(snap);

		string javaString = "";
		//javaString += "RecordAddedControl('add','" + isModified.ClientID + "','" + snap.ClientID;
		//javaString += "','" + panel.ClientID + "','" + controlInfo.Uid + "','" + controlInfo.AdapterPath + "');";
		javaString += " var __sh=$find('SnapHolder1_Snap'); if(__sh!=null) {__sh.RecordAddedControl('add','" + isModified.ClientID + "','" + snap.ClientID + "','" + panel.ClientID + "','" + controlInfo.Uid + "','" + controlInfo.AdapterPath + "');}";
		//    snap.Attributes.Add("onmouseup", "alert('begin');" + javaString + ";RecordNewDN();alert('end')");
		panel.Attributes.Add("onmousedown", snap.ClientID + ".StartDragging(event);" + javaString + " if(__sh!=null) __sh.RecordNewDN();");
		
		return snap;

	}


    /// <summary>
    /// Handles the Dock event of the Snap control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="ComponentArt.Web.UI.SnapDockEventArgs"/> instance containing the event data.</param>
	void Snap_Dock(object sender, SnapDockEventArgs e)
	{
		PlaceHolderManager.GetCurrent(this.Page).AddControl(e.SnapObject.Attributes["ControlUID"], e.Dock, e.DockIndex);
		_rebindSnapFlag = true;
	}
	#endregion

	#region OnPrerender
	protected override void OnPreRender(EventArgs e)
	{
		if (!this.DesignMode)
		{
			ScriptManager sm = ScriptManager.GetCurrent(this.Page);
			if (sm != null)
			{
				sm.RegisterScriptControl(this);
				sm.RegisterScriptDescriptors(this);
			}
		}
		base.OnPreRender(e);
	} 
	#endregion

	#region IScriptControl Members

	public IEnumerable<ScriptDescriptor> GetScriptDescriptors()
	{
		ScriptComponentDescriptor sd = new ScriptComponentDescriptor("Mediachase.Cms.Toolbox");
		sd.ID = this.ClientID + "_Toolbox";
		sd.AddProperty("ToolboxId", "showimageT");
		sd.AddProperty("ToolboxLocation", tboxLocation.ClientID);
		return new ScriptDescriptor[] { sd };
	}

	public IEnumerable<ScriptReference> GetScriptReferences()
	{
		ScriptReference sr = new ScriptReference(Mediachase.Commerce.Shared.CommerceHelper.GetAbsolutePath("~/Structure/Base/Controls/Toolbar/Scripts/CMS_Toolbox.js"));
		return new ScriptReference[] { sr };
	}

	#endregion
}
