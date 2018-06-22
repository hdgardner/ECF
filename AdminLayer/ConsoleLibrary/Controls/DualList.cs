using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Resources;
using System.ComponentModel;
using Mediachase.Web.Console.Common;

namespace Mediachase.Web.Console.Controls
{

	/// <summary>
	/// Provides the UI for the common requirement of two listboxes with items that move between them.
	/// </summary>
	/// <remarks>
	/// <p>When DataBinding is used, the control will automaticly remove items from the left side if they exist on the right side. This ensures that double items can't be chosen.</p>
	/// <p>
	/// For browsers that support scripting, the movement will be completely clientside. There is also 100% serverside support, for any browsers with script disabled or nonexistant.
	/// </p>
	/// <p>By default, the clientscript used by the control is emited directly into the page.
	/// In order to save bandwidth, it's possible to have the control to use a script reference instead,
	/// but this requires the following handler to be added to the httpHandlers section of web.config.</p>
	/// <code>
	/// &lt;httpHandlers&gt;
	///		&lt;add verb="*"
	///			path="MetaBuilders_WebControls_DynamicListBoxResourceHandler.axd"
	///			type="MetaBuilders.WebControls.DynamicListBoxResourceHandler,MetaBuilders.WebControls.DynamicListBox"
	///			validate="false"
	///		/&gt;
	/// &lt;/httpHandlers&gt;
	/// </code>
	/// </remarks>
	/// <example>
	/// The following is a simple example that uses this control.
	/// <code><![CDATA[
    /// <%@ Register TagPrefix="ecf" Namespace="Mediachase.WebConsoleLib.Controls" Assembly="Mediachase.WebConsoleLib" %>
	/// <script runat="server" language="c#" >
	/// 	protected void DualList1_ItemsMoved( Object sender, EventArgs e ) {
	/// 		DualResult.Text = "The Chosen Items Are:";
	/// 		foreach( ListItem item in DualList1.RightBox.Items ) {
	/// 			DualResult.Text += "<br>" + item.Value + "/" + item.Text;
	///			}
	///		}
	/// </script>
	/// <form runat="server">
	/// <ecf:DualList runat="server" Id="DualList1" OnItemsMoved="DualList1_ItemsMoved" >
	/// 	<LeftItems>
	/// 		<asp:ListItem Value="1" Text="One" />
	/// 		<asp:ListItem Value="2" Text="Two" />
	/// 		<asp:ListItem Value="3" Text="Three" />
	/// 	</LeftItems>
	/// 	<RightItems>
	/// 		<asp:ListItem Value="4" Text="Four" />
	/// 		<asp:ListItem Value="5" Text="Five" />
	/// 		<asp:ListItem Value="6" Text="Six" />
	/// 	</RightItems>
	/// </ecf:DualList>
	/// <br><br><asp:Label runat="server" id="DualResult" />
	/// <hr><asp:Button runat="server" text="Smack"/>
	/// </form>
	/// ]]></code>
	/// </example>
	[
	DefaultProperty("ItemsName"),
	DefaultEvent("ItemsMoved"),
	PersistChildren(false),
	ParseChildren(true),
	]
	public class DualList : System.Web.UI.WebControls.WebControl, INamingContainer
	{
		#region Composite Control Pattern
        /// <summary>
        /// Overrides <see cref="Control.Controls"/> to implement the Composite Control Pattern
        /// </summary>
        /// <value></value>
        /// <returns>The collection of child controls for the specified server control.</returns>
		public override ControlCollection Controls
		{
			get
			{
				this.EnsureChildControls();
				return base.Controls;
			}
		}

        /// <summary>
        /// Overrides <see cref="Control.CreateChildControls"/> to implement the Composite Control Pattern
        /// </summary>
		protected override void CreateChildControls()
		{
			this.Controls.Clear();
			this.InitializeComponent();
		}

		#endregion

		#region Events
		/// <summary>
		/// The event that fires when items have been moved.
		/// </summary>
		public event EventHandler ItemsMoved;
        /// <summary>
        /// Raises the <see cref="ItemsMoved"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected virtual void OnItemsMoved(EventArgs e)
		{
			if (this.ItemsMoved != null)
			{
				this.ItemsMoved(this, e);
			}
		}
		#endregion

		#region Properties
        /// <summary>
        /// Overrides <see cref="WebControl.TagKey"/>
        /// </summary>
        /// <value></value>
        /// <returns>One of the <see cref="T:System.Web.UI.HtmlTextWriterTag"/> enumeration values.</returns>
		protected override HtmlTextWriterTag TagKey
		{
			get
			{
				return HtmlTextWriterTag.Table;
			}
		}


		#region LeftBox
        /// <summary>
        /// Gets or sets the DataSource of the list on the left side of the control.
        /// </summary>
        /// <value>The left data source.</value>
		[
		Description("Gets or sets the DataSource of the list on the left side of the control."),
		Category("Data"),
		Bindable(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		]
		public virtual Object LeftDataSource
		{
			get
			{
				this.EnsureChildControls();
				return this.leftBox.DataSource;
			}
			set
			{
				this.EnsureChildControls();
				this.leftBox.DataSource = value;
			}
		}

        /// <summary>
        /// Gets or sets the DataMember of the list on the left side of the control.
        /// </summary>
        /// <value>The left data member.</value>
		[
		Description("Gets or sets the DataMember of the list on the left side of the control."),
		Category("Data"),
		DefaultValue(""),
		]
		public virtual String LeftDataMember
		{
			get
			{
				this.EnsureChildControls();
				return this.leftBox.DataMember;
			}
			set
			{
				this.EnsureChildControls();
				this.leftBox.DataMember = value;
			}
		}

        /// <summary>
        /// Gets or sets the DataTextField of the list on the left side of the control.
        /// </summary>
        /// <value>The left data text field.</value>
		[
		Description("Gets or sets the DataTextField of the list on the left side of the control."),
		Category("Data"),
		DefaultValue(""),
		]
		public virtual String LeftDataTextField
		{
			get
			{
				this.EnsureChildControls();
				return this.leftBox.DataTextField;
			}
			set
			{
				this.EnsureChildControls();
				this.leftBox.DataTextField = value;
			}
		}

        /// <summary>
        /// Gets or sets the DataValueField of the list on the left side of the control.
        /// </summary>
        /// <value>The left data value field.</value>
		[
		Description("Gets or sets the DataValueField of the list on the left side of the control."),
		Category("Data"),
		DefaultValue(""),
		]
		public virtual String LeftDataValueField
		{
			get
			{
				this.EnsureChildControls();
				return this.leftBox.DataValueField;
			}
			set
			{
				this.EnsureChildControls();
				this.leftBox.DataValueField = value;
			}
		}

        /// <summary>
        /// Gets or sets the DataTextFormatString of the list on the left side of the control.
        /// </summary>
        /// <value>The left data text format string.</value>
		[
		Description("Gets or sets the DataTextFormatString of the list on the left side of the control."),
		Category("Data"),
		DefaultValue(""),
		]
		public virtual String LeftDataTextFormatString
		{
			get
			{
				this.EnsureChildControls();
				return this.leftBox.DataTextFormatString;
			}
			set
			{
				this.EnsureChildControls();
				this.leftBox.DataTextFormatString = value;
			}
		}

        /// <summary>
        /// Gets the items in the list the left side of the control.
        /// </summary>
        /// <value>The left items.</value>
		[
		Description("Gets the items in the list the left side of the control."),
		DefaultValue(null),
		MergableProperty(false),
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NotifyParentProperty(true),
		]
		public virtual ListItemCollection LeftItems
		{
			get
			{
				this.EnsureChildControls();
				return this.leftBox.Items;
			}
		}

        /// <summary>
        /// Gets the <see cref="WebControl.ControlStyle"/> of the list on left side of the control.
        /// </summary>
        /// <value>The left list style.</value>
		[
		Description("Gets the ControlStyle of the list on left side of the control."),
		Category("Appearance"),
		NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty),
		]
		public virtual Style LeftListStyle
		{
			get
			{
				this.EnsureChildControls();
				return this.leftBox.ControlStyle;
			}
		}

		#endregion

		#region RightBox
        /// <summary>
        /// Gets or sets the DataSource of the list on the right side of the control.
        /// </summary>
        /// <value>The right data source.</value>
		[
		Description("Gets or sets the DataSource of the list on the right side of the control."),
		Category("Data"),
		Bindable(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		]
		public virtual Object RightDataSource
		{
			get
			{
				this.EnsureChildControls();
				return this.rightBox.DataSource;
			}
			set
			{
				this.EnsureChildControls();
				this.rightBox.DataSource = value;
			}
		}

        /// <summary>
        /// Gets or sets the DataMember of the list on the right side of the control.
        /// </summary>
        /// <value>The right data member.</value>
		[
		Description("Gets or sets the DataMember of the list on the right side of the control."),
		Category("Data"),
		DefaultValue(""),
		]
		public virtual String RightDataMember
		{
			get
			{
				this.EnsureChildControls();
				return this.rightBox.DataMember;
			}
			set
			{
				this.EnsureChildControls();
				this.rightBox.DataMember = value;
			}
		}

        /// <summary>
        /// Gets or sets the DataTextField of the list on the right side of the control.
        /// </summary>
        /// <value>The right data text field.</value>
		[
		Description("Gets or sets the DataTextField of the list on the right side of the control."),
		Category("Data"),
		DefaultValue(""),
		]
		public virtual String RightDataTextField
		{
			get
			{
				this.EnsureChildControls();
				return this.rightBox.DataTextField;
			}
			set
			{
				this.EnsureChildControls();
				this.rightBox.DataTextField = value;
			}
		}

        /// <summary>
        /// Gets or sets the DataValueField of the list on the right side of the control.
        /// </summary>
        /// <value>The right data value field.</value>
		[
		Description("Gets or sets the DataValueField of the list on the right side of the control."),
		Category("Data"),
		DefaultValue(""),
		]
		public virtual String RightDataValueField
		{
			get
			{
				this.EnsureChildControls();
				return this.rightBox.DataValueField;
			}
			set
			{
				this.EnsureChildControls();
				this.rightBox.DataValueField = value;
			}
		}

        /// <summary>
        /// Gets or sets the DataTextFormatString of the list on the right side of the control.
        /// </summary>
        /// <value>The right data text format string.</value>
		[
		Description("Gets or sets the DataTextFormatString of the list on the right side of the control."),
		Category("Data"),
		DefaultValue(""),
		]
		public virtual String RightDataTextFormatString
		{
			get
			{
				this.EnsureChildControls();
				return this.rightBox.DataTextFormatString;
			}
			set
			{
				this.EnsureChildControls();
				this.rightBox.DataTextFormatString = value;
			}
		}

        /// <summary>
        /// Gets the items in the list the right side of the control.
        /// </summary>
        /// <value>The right items.</value>
		[
		Description("Gets the items in the list the right side of the control."),
		DefaultValue(null),
		MergableProperty(false),
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NotifyParentProperty(true),
		]
		public virtual ListItemCollection RightItems
		{
			get
			{
				this.EnsureChildControls();
				return this.rightBox.Items;
			}
		}

        /// <summary>
        /// Gets the <see cref="WebControl.ControlStyle"/> of the list on right side of the control.
        /// </summary>
        /// <value>The right list style.</value>
		[
		Description("Gets the ControlStyle of the list on right side of the control."),
		Category("Appearance"),
		NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty),
		]
		public virtual Style RightListStyle
		{
			get
			{
				this.EnsureChildControls();
				return this.rightBox.ControlStyle;
			}
		}

		#endregion

        /// <summary>
        /// Gets or sets the number of rows visible in the lists of the control.
        /// </summary>
        /// <value>The list rows.</value>
		[
		Description("Gets or sets the number of rows visible in the lists of the control."),
		Category("Appearance"),
		DefaultValue(8),
		]
		public virtual Int32 ListRows
		{
			get
			{
				this.EnsureChildControls();
				return this.leftBox.Rows;
			}
			set
			{
				this.EnsureChildControls();
				this.leftBox.Rows = value;
				this.rightBox.Rows = value;
			}
		}

        /// <summary>
        /// Gets or sets the text displayed above the lists, the plural name of the items being chosen.
        /// </summary>
        /// <value>The name of the items.</value>
		[
		Description("Gets or sets the text displayed above the lists, the plural name of the items being chosen."),
		Category("Appearance"),
		DefaultValue("Items"),
		]
		public virtual String ItemsName
		{
			get
			{
				Object savedState = this.ViewState["ItemLabel"];
				if (savedState != null)
				{
					return (String)savedState;
				}
				return "Items";
			}
			set
			{
				this.ViewState["ItemLabel"] = value;
				this.EnsureChildControls();
				this.leftBoxLabel.Text = ManagementHelper.GetConsoleResource("DUALLIST_AVAILABLE") + " " + value;
				this.rightBoxLabel.Text = ManagementHelper.GetConsoleResource("DUALLIST_CHOSEN") + " " + value;
			}
		}

        /// <summary>
        /// Gets or sets the visibility of the buttons for moving all items between the lists.
        /// </summary>
        /// <value><c>true</c> if [enable move all]; otherwise, <c>false</c>.</value>
		[
		Description("Gets or sets the visibility of the buttons for moving all items between the lists."),
		Category("Behavior"),
		DefaultValue(false),
		]
		public virtual Boolean EnableMoveAll
		{
			get
			{
				Object savedState = this.ViewState["EnableMoveAll"];
				if (savedState != null)
				{
					return (Boolean)savedState;
				}
				return false;
			}
			set
			{
				this.ViewState["EnableMoveAll"] = value;
			}
		}

        /// <summary>
        /// Gets or sets the visibility of the buttons for moving items up and down within the list on the right side of the control.
        /// </summary>
        /// <value><c>true</c> if [enable move up down]; otherwise, <c>false</c>.</value>
		[
		Description("Gets or sets the visibility of the buttons for moving items up and down within the RightBox."),
		Category("Behavior"),
		DefaultValue(false),
		]
		public virtual Boolean EnableMoveUpDown
		{
			get
			{
				Object savedState = this.ViewState["EnableMoveUpDown"];
				if (savedState != null)
				{
					return (Boolean)savedState;
				}
				return false;
			}
			set
			{
				this.ViewState["EnableMoveUpDown"] = value;
			}
		}

        /// <summary>
        /// Overrides <see cref="WebControl.Enabled"/>
        /// </summary>
        /// <value></value>
        /// <returns>true if control is enabled; otherwise, false. The default is true.</returns>
		public override bool Enabled
		{
			get
			{
				return base.Enabled;
			}
			set
			{
				base.Enabled = value;
				this.EnsureChildControls();
				this.leftBox.Enabled = value;
				this.rightBox.Enabled = value;
				this.moveRight.Enabled = value;
				this.moveAllRight.Enabled = value;
				this.moveLeft.Enabled = value;
				this.moveAllLeft.Enabled = value;
				this.moveUp.Enabled = value;
				this.moveDown.Enabled = value;
			}
		}

        /// <summary>
        /// Gets the <see cref="WebControl.ControlStyle"/> of the buttons contained in the control.
        /// </summary>
        /// <value>The button style.</value>
		[
		Description("Gets the WebControl.ControlStyle of the buttons contained in the control."),
		Category("Appearance"),
		NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty),
		]
		public virtual Style ButtonStyle
		{
			get
			{
				this.EnsureChildControls();
				return this.moveRight.ControlStyle;
			}
		}
		#endregion

		#region Life Cycle

        /// <summary>
        /// Initializes the contained controls.
        /// </summary>
		private void InitializeComponent()
		{
			// Instantiate
			this.leftBox = new DynamicListBox();
			this.rightBox = new DynamicListBox();
			this.moveRight = new Button();
			this.moveLeft = new Button();
			this.moveAllRight = new Button();
			this.moveAllLeft = new Button();
			this.moveUp = new Button();
			this.moveDown = new Button();
			this.leftBoxLabel = new Label();
			this.rightBoxLabel = new Label();
			this.allLeftContainer = new PlaceHolder();
			this.allRightContainer = new PlaceHolder();

			// Customize
			this.leftBox.ID = "LeftBox";
			this.leftBox.SelectionMode = ListSelectionMode.Multiple;
			this.leftBox.Rows = 8;
			this.leftBox.ItemsChanged += new EventHandler(leftBox_ItemsChanged);

			this.rightBox.ID = "RightBox";
			this.rightBox.SelectionMode = ListSelectionMode.Multiple;
			this.rightBox.Rows = 8;
			this.rightBox.ItemsChanged += new EventHandler(rightBox_ItemsChanged);

			this.moveRight.ID = "MoveRight";
			this.moveRight.Text = ManagementHelper.GetConsoleResource("DUALLIST_MOVE_RIGHT") + " ->";
			this.moveRight.CausesValidation = false;
			this.moveRight.Click += new EventHandler(moveRight_Click);

			this.moveAllRight.ID = "MoveAllRight";
			this.moveAllRight.Text = ManagementHelper.GetConsoleResource("DUALLIST_MOVE_ALL_RIGHT") + " ->>";
			this.moveAllRight.CausesValidation = false;
			this.moveAllRight.Click += new EventHandler(moveAllRight_Click);

			this.moveLeft.ID = "MoveLeft";
			this.moveLeft.Text = "<- " + ManagementHelper.GetConsoleResource("DUALLIST_MOVE_LEFT");
			this.moveLeft.CausesValidation = false;
			this.moveLeft.Click += new EventHandler(moveLeft_Click);

			this.moveAllLeft.ID = "MoveAllLeft";
			this.moveAllLeft.Text = "<<- " + ManagementHelper.GetConsoleResource("DUALLIST_MOVE_ALL_LEFT");
			this.moveAllLeft.CausesValidation = false;
			this.moveAllLeft.Click += new EventHandler(moveAllLeft_Click);

			this.moveUp.ID = "MoveUp";
			this.moveUp.Text = ManagementHelper.GetConsoleResource("DUALLIST_MOVE_UP");
			this.moveUp.CausesValidation = false;
			this.moveUp.Width = Unit.Parse("100%", System.Globalization.CultureInfo.InvariantCulture);
			this.moveUp.Click += new EventHandler(moveUp_Click);

			this.moveDown.ID = "MoveDown";
			this.moveDown.Text = ManagementHelper.GetConsoleResource("DUALLIST_MOVE_DOWN");
			this.moveDown.CausesValidation = false;
			this.moveDown.Click += new EventHandler(moveDown_Click);

			this.leftBoxLabel.Text = ManagementHelper.GetConsoleResource("DUALLIST_AVAILABLE") + " " + this.ItemsName;
			this.rightBoxLabel.Text = ManagementHelper.GetConsoleResource("DUALLIST_CHOSEN") + " " + this.ItemsName;

			// Layout
			TableRow topRow = new TableRow();
			this.Controls.Add(topRow);
			topRow.Cells.AddRange(new TableCell[] { new TableCell(), new TableCell() });
			topRow.Cells[0].ColumnSpan = 2;
			topRow.Cells[0].Controls.Add(this.leftBoxLabel);
			topRow.Cells[1].ColumnSpan = 2;
			topRow.Cells[1].Controls.Add(this.rightBoxLabel);

			TableRow mainRow = new TableRow();
			this.Controls.Add(mainRow);
			mainRow.Cells.AddRange(new TableCell[] { new TableCell(), new TableCell(), new TableCell(), new TableCell() });

			TableCell currentCell;

			currentCell = mainRow.Cells[0];
			currentCell.Controls.Add(leftBox);
			currentCell.HorizontalAlign = HorizontalAlign.Center;

			currentCell = mainRow.Cells[1];
			currentCell.Controls.Add(moveRight);
			this.allRightContainer.Controls.Add(new LiteralControl("<br>"));
			this.allRightContainer.Controls.Add(moveAllRight);
			currentCell.Controls.Add(this.allRightContainer);
			currentCell.Controls.Add(new LiteralControl("<br>"));
			currentCell.Controls.Add(new LiteralControl("<br>"));
			currentCell.Controls.Add(moveLeft);
			this.allLeftContainer.Controls.Add(new LiteralControl("<br>"));
			this.allLeftContainer.Controls.Add(this.moveAllLeft);
			currentCell.Controls.Add(this.allLeftContainer);
			currentCell.HorizontalAlign = HorizontalAlign.Center;
			currentCell.VerticalAlign = VerticalAlign.Middle;

			currentCell = mainRow.Cells[2];
			currentCell.Controls.Add(rightBox);
			currentCell.HorizontalAlign = HorizontalAlign.Center;

			currentCell = mainRow.Cells[3];
			currentCell.Controls.Add(this.moveUp);
			currentCell.Controls.Add(new LiteralControl("<br>"));
			currentCell.Controls.Add(this.moveDown);
			currentCell.HorizontalAlign = HorizontalAlign.Left;
			currentCell.VerticalAlign = VerticalAlign.Middle;

		}

        /// <summary>
        /// Overrides <see cref="Control.DataBind"/>.
        /// </summary>
		public override void DataBind()
		{
			base.DataBind();
			this.FixAvailableItems();
		}

        /// <summary>
        /// FixAvailableItems is called after <see cref="DataBind"/> to make sure that none of the items on the right, "chosen", list exist in the left, "available" list.
        /// </summary>
		protected virtual void FixAvailableItems()
		{
			foreach (ListItem item in this.rightBox.Items)
			{
				ListItem leftItem = this.leftBox.Items.FindByValue(item.Value);
				if (leftItem != null && leftItem.Text == item.Text)
				{
					this.leftBox.Items.Remove(leftItem);
				}
			}
		}


        /// <summary>
        /// Overrides <see cref="Control.OnPreRender"/>.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			this.RegisterScript();
		}


        /// <summary>
        /// Overrides <see cref="Control.Render"/>.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Web.UI.HtmlTextWriter"/> object that receives the control content.</param>
		protected override void Render(HtmlTextWriter writer)
		{
			this.EnsureChildControls();
			this.allRightContainer.Visible = this.EnableMoveAll;
			this.allLeftContainer.Visible = this.EnableMoveAll;
			this.moveUp.Parent.Visible = this.EnableMoveUpDown;

			this.moveAllRight.ControlStyle.CopyFrom(this.moveRight.ControlStyle);
			this.moveLeft.ControlStyle.CopyFrom(this.moveRight.ControlStyle);
			this.moveAllLeft.ControlStyle.CopyFrom(this.moveRight.ControlStyle);
			this.moveUp.ControlStyle.CopyFrom(this.moveRight.ControlStyle);
			this.moveDown.ControlStyle.CopyFrom(this.moveRight.ControlStyle);

			base.Render(writer);
		}


        /// <summary>
        /// Overrides <see cref="WebControl.CreateControlStyle"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Web.UI.WebControls.Style"/> that is used to implement all style-related properties of the control.
        /// </returns>
		protected override Style CreateControlStyle()
		{
			return new TableStyle(this.ViewState);
		}


		#endregion

		#region ClientScript
        /// <summary>
        /// Registers the script for this control.
        /// </summary>
		protected virtual void RegisterScript()
		{
			if (this.Page != null)
			{
				this.RegisterScriptLibrary();
				this.RegisterScriptArray();
				this.RegisterScriptStartup();
			}
		}

        /// <summary>
        /// Registers the script library for this control.
        /// </summary>
		protected virtual void RegisterScriptLibrary()
		{
			DynamicListBoxResourceHandler.RegisterScript(this.Page, "MetaBuilders_DualList", "DualList.js");
		}

        /// <summary>
        /// Registers the script array for this control.
        /// </summary>
		protected virtual void RegisterScriptArray()
		{
			this.EnsureChildControls();
			String idPrefix = this.UniqueID + this.leftBox.UniqueID.Substring(this.UniqueID.Length, 1);
			this.Page.ClientScript.RegisterArrayDeclaration("MetaBuilders_DualLists", "'" + idPrefix + "'");
		}

        /// <summary>
        /// Registers the script which initializes this control.
        /// </summary>
		protected virtual void RegisterScriptStartup()
		{
			this.Page.ClientScript.RegisterStartupScript(typeof(DualList), "MetaBuilders_DualList", @"
						<script language='javascript'>
						<!--
							MetaBuilders_DualList_Init();
						//-->
						</script>");
		}

		#endregion

		#region Child Control Event Handlers
		// These handlers will only fire if the client browser doesn't support clientscript

        /// <summary>
        /// Handles the Click event of the moveRight control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void moveRight_Click(object sender, EventArgs e)
		{
			this.MoveSelectedItems(this.leftBox, this.rightBox);
			this.EnsureChangeEvent();
		}

        /// <summary>
        /// Handles the Click event of the moveAllRight control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void moveAllRight_Click(object sender, EventArgs e)
		{
			this.MoveAllItems(this.leftBox, this.rightBox);
			this.EnsureChangeEvent();
		}

        /// <summary>
        /// Handles the Click event of the moveLeft control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void moveLeft_Click(object sender, EventArgs e)
		{
			this.MoveSelectedItems(this.rightBox, this.leftBox);
			this.EnsureChangeEvent();
		}

        /// <summary>
        /// Handles the Click event of the moveAllLeft control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void moveAllLeft_Click(object sender, EventArgs e)
		{
			this.MoveAllItems(this.rightBox, this.leftBox);
			this.EnsureChangeEvent();
		}

        /// <summary>
        /// Handles the Click event of the moveUp control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void moveUp_Click(object sender, EventArgs e)
		{
			Int32 originalIndex = this.rightBox.SelectedIndex;
			if (originalIndex > 0)
			{
				ListItem movedItem = this.rightBox.SelectedItem;
				this.rightBox.Items.Remove(movedItem);
				this.rightBox.Items.Insert(originalIndex - 1, movedItem);
				this.EnsureChangeEvent();
			}
		}

        /// <summary>
        /// Handles the Click event of the moveDown control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void moveDown_Click(object sender, EventArgs e)
		{
			Int32 originalIndex = this.rightBox.SelectedIndex;
			if (originalIndex < this.rightBox.Items.Count - 1)
			{
				ListItem movedItem = this.rightBox.SelectedItem;
				this.rightBox.Items.Remove(movedItem);
				this.rightBox.Items.Insert(originalIndex + 1, movedItem);
				this.EnsureChangeEvent();
			}
		}


        /// <summary>
        /// Moves the selected items.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
		private void MoveSelectedItems(ListBox source, ListBox target)
		{
			while (source.SelectedIndex != -1)
			{
				target.Items.Add(source.SelectedItem);
				source.Items.Remove(source.SelectedItem);
			}
		}
        /// <summary>
        /// Moves all items.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
		private void MoveAllItems(ListBox source, ListBox target)
		{
			while (source.Items.Count != 0)
			{
				target.Items.Add(source.Items[0]);
				source.Items.RemoveAt(0);
			}
		}

        /// <summary>
        /// Handles the ItemsChanged event of the leftBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void leftBox_ItemsChanged(object sender, EventArgs e)
		{
			this.EnsureChangeEvent();
		}

        /// <summary>
        /// Handles the ItemsChanged event of the rightBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void rightBox_ItemsChanged(object sender, EventArgs e)
		{
			this.EnsureChangeEvent();
		}
        /// <summary>
        /// Ensures the change event.
        /// </summary>
		private void EnsureChangeEvent()
		{
			if (!this.hasNotifiedOfChange)
			{
				hasNotifiedOfChange = true;
				this.OnItemsMoved(EventArgs.Empty);
			}
		}
		private Boolean hasNotifiedOfChange = false;
		#endregion

		#region Child Control References
		private DynamicListBox leftBox;
		private DynamicListBox rightBox;

		private Button moveRight;
		private Button moveLeft;
		private Button moveAllRight;
		private Button moveAllLeft;

		private Button moveUp;
		private Button moveDown;

		private Label leftBoxLabel;
		private Label rightBoxLabel;

		private PlaceHolder allRightContainer;
		private PlaceHolder allLeftContainer;
		#endregion
	}
}
