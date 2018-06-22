using System;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.ComponentModel;
using System.Resources;

namespace Mediachase.Web.Console.Controls
{

	/// <summary>
	/// A ListBox which can retrieve and persist any client-side changes to its Items collection.
	/// </summary>
	/// <remarks>
	/// <p>
	/// In order for your client-side changes to be persisted, 
	/// you must call new javascript methods on the ListBox.
	/// This Select element now has "Add" and "Remove" methods.
	/// The Add method takes the value, text, and optional index of the new ListItem.
	/// The Remove method takes the index of the ListItem to remove.
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
	/// The following is an example of how to combine the DynamicListBox with client-script in your page.
	/// <code><![CDATA[
	/// <%@ Page Language="C#" %>
	/// <%@ Register TagPrefix="mb" Namespace="MetaBuilders.WebControls" Assembly="MetaBuilders.WebControls.DynamicListBox" %>
	/// <script runat="server">
	/// 
	/// 	protected void Page_Load( Object sender, EventArgs e ) {
	///			RegisterClientScriptBlock( MyList.ClientID, "<script>\r\nwindow.MyListId='" + MyList.ClientID + "';\r\n</" + "script>");
	///		}
	///     protected void MyList_ListChanged( Object sender, EventArgs e ) {
	///         if( MyList.SelectedIndex == -1 ) {
	///             Result.Text = "The SelectedIndex changed and now nothing on the list is selected";
	///         } else {
	///             Result.Text = "The SelectedIndex changed and is now '" + MyList.SelectedIndex + "'<br>The selected item is: " + MyList.SelectedItem.Value + "/" + MyList.SelectedItem.Text;
	///         }
	///     }
	/// 
	/// </script>
	/// <html><body><form runat="server">
	/// 	
	///     <mb:DynamicListBox id="MyList" runat="server" OnSelectedIndexChanged="MyList_ListChanged" SelectionMode="Multiple" >
	///         <asp:ListItem value="normalItem1" text="normalItem1"></asp:ListItem>
	///         <asp:ListItem value="normalItem2" text="normalItem2"></asp:ListItem>
	///     </mb:DynamicListBox>
	///     
	///     <br><br>
	///     <a href="javascript:remove();" >Remove</a>
	///     <a href="javascript:add();" >Add</a>
	///     <script>
	/// 		function remove() {
	/// 			var list = DynamicListBox_FindControl(window.MyListId);
	/// 			var keepLooking = true;
	/// 			while ( keepLooking ) {
	/// 				list.Remove( list.options.selectedIndex );
	/// 				if ( list.options.selectedIndex < 0 ) {
	/// 					keepLooking = false;
	/// 				}
	/// 			}
	/// 		}
	/// 		
	/// 		function add() {
	/// 			var list = DynamicListBox_FindControl(window.MyListId);
	/// 			var generatedName = "newItem" + ( list.options.length + 1 );
	/// 			list.Add(generatedName,generatedName);
	/// 		}
	///     </script>
	///     
	///     <br><br>
	///     <asp:Button runat="server" text="Smack"/>
	///     
	///     <br><br>
	///     <asp:Label runat="server" id="Result" />
	///     
	/// </form></body></html>
	/// ]]></code></example>
	public class DynamicListBox : System.Web.UI.WebControls.ListBox, System.Web.UI.IPostBackDataHandler, INamingContainer
	{

		#region Render Overrides
        /// <summary>
        /// Overridden to call EnsureChildControls.
        /// </summary>
        /// <value></value>
        /// <remarks>Supposedly should be done for all controls which override CreateChildControls</remarks>
		public override System.Web.UI.ControlCollection Controls
		{
			get
			{
				EnsureChildControls();
				return base.Controls;
			}
		}

        /// <summary>
        /// Overridden to include the hidden input which tracks the client side changes.
        /// </summary>
		protected override void CreateChildControls()
		{
			base.CreateChildControls();
			this.itemTracker = new HtmlInputHidden();
			this.itemTracker.ID = "itemTracker";
			this.itemTracker.EnableViewState = false;
			this.Controls.Add(itemTracker);
		}

        /// <summary>
        /// Overridden to register client script.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
		protected override void OnPreRender(System.EventArgs e)
		{
			base.OnPreRender(e);
			if (this.Page != null)
			{
				this.Page.RegisterRequiresPostBack(this);
				this.itemTracker.Value = "";
				this.RegisterClientScript();
			}
		}

        /// <summary>
        /// Overridden to render children after the end tag.
        /// </summary>
        /// <param name="writer">A <see cref="T:System.Web.UI.HtmlTextWriter"/> that represents the output stream to render HTML content on the client.</param>
		public override void RenderEndTag(System.Web.UI.HtmlTextWriter writer)
		{
			base.RenderEndTag(writer);
			this.RenderChildren(writer);
		}
		#endregion

		#region Events
		/// <summary>
		/// The event that is raised when the user has changed the listbox's items collection..
		/// </summary>
		public event EventHandler ItemsChanged;
        /// <summary>
        /// Raises the <see cref="ItemsChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected virtual void OnItemsChanged(EventArgs e)
		{
			if (this.ItemsChanged != null)
			{
				this.ItemsChanged(this, e);
			}
		}
		#endregion

		#region Implementation of IPostBackDataHandler
        /// <summary>
        /// Implements <see cref="IPostBackDataHandler.RaisePostDataChangedEvent"/>.
        /// </summary>
		void IPostBackDataHandler.RaisePostDataChangedEvent()
		{
			if (this.raiseItemsChanged)
			{
				OnItemsChanged(EventArgs.Empty);
			}
			if (this.selectedItemsChanged || this.newSelectionPosted)
			{
				this.OnSelectedIndexChanged(EventArgs.Empty);
			}
		}
        /// <summary>
        /// Implements <see cref="IPostBackDataHandler.LoadPostData"/>.
        /// </summary>
        /// <param name="postDataKey">The key identifier for the control, used to index the <paramref name="postCollection"/>.</param>
        /// <param name="postCollection">A <see cref="T:System.Collections.Specialized.NameValueCollection"/> that contains value information indexed by control identifiers.</param>
        /// <returns>
        /// true if the posted content is different from the last posting; otherwise, false.
        /// </returns>
		Boolean IPostBackDataHandler.LoadPostData(string postDataKey, System.Collections.Specialized.NameValueCollection postCollection)
		{
			if (!dataLoaded)
			{
				dataLoaded = true;
				selectedItemsChanged = loadNewItems(postDataKey, postCollection);

				// This is the only way I know of to call the baseclass implementation of an explicit interface implementation.
				System.Reflection.InterfaceMapping interfaceMap = typeof(ListBox).GetInterfaceMap(typeof(IPostBackDataHandler));
				for (Int32 i = 0; i < interfaceMap.InterfaceMethods.Length; i++)
				{
					if (interfaceMap.InterfaceMethods[i].Name == "LoadPostData")
					{
						newSelectionPosted = (Boolean)interfaceMap.TargetMethods[i].Invoke(this, new Object[] { postDataKey, postCollection });
						break;
					}
				}

				this.raiseItemsChanged = postCollection[this.itemTracker.UniqueID] != null && postCollection[this.itemTracker.UniqueID].Length > 0;

				return this.raiseItemsChanged || selectedItemsChanged || newSelectionPosted;
			}
			else
			{
				return false;
			}
		}
		private Boolean dataLoaded = false;
		private Boolean raiseItemsChanged = false;
		private Boolean selectedItemsChanged = false;
		private Boolean newSelectionPosted = false;

        /// <summary>
        /// Edits the ListItems that were added or removed on the clientside
        /// </summary>
        /// <param name="postDataKey">The post data key.</param>
        /// <param name="postCollection">The post collection.</param>
        /// <returns>True if the list of items has changed.</returns>
		private Boolean loadNewItems(string postDataKey, System.Collections.Specialized.NameValueCollection postCollection)
		{
			this.EnsureChildControls();
			String postedValue = postCollection[this.itemTracker.UniqueID];

			Boolean result = false;

			ArrayList currentSelection = this.SelectedIndicesInternal;

			if (postedValue != null && postedValue.Length != 0)
			{
				ListCommand[] commands = ListCommand.Split(postedValue.Trim());
				foreach (ListCommand command in commands)
				{
					if (command.Operator == "+")
					{
						ListItem newItem = new ListItem(command.Text, command.Value);
						if (command.Index >= 0 && command.Index <= this.Items.Count - 1)
						{
							this.Items.Insert(command.Index, newItem);
						}
						else
						{
							this.Items.Add(newItem);
						}
					}
					else if (command.Operator == "-")
					{
						if (command.Index >= 0 && command.Index <= this.Items.Count - 1)
						{
							if (this.Items[command.Index].Selected)
							{
								result = true;
							}
							this.Items.RemoveAt(command.Index);
						}
					}
				}
			}

			if (!result)
			{
				ArrayList newSelection = this.SelectedIndicesInternal;
				if (newSelection == null)
				{
					result = (currentSelection != null);
				}
				else
				{
					if (newSelection.Count != currentSelection.Count)
					{
						result = true;
					}
					else
					{
						for (Int32 i = 0; i < currentSelection.Count; i++)
						{
							if ((Int32)newSelection[i] != (Int32)currentSelection[i])
							{
								result = true;
								break;
							}
						}
					}
				}
			}

			return result;
		}

        /// <summary>
        /// Gets the selected indices internal.
        /// </summary>
        /// <value>The selected indices internal.</value>
		private ArrayList SelectedIndicesInternal
		{
			get
			{
				ArrayList selections;

				selections = null;
				for (Int32 i = 0; i < this.Items.Count; i++)
				{
					if (this.Items[i].Selected)
					{
						if (selections == null)
						{
							selections = new ArrayList(3);
						}
						selections.Add(i);
					}
				}
				return selections;
			}
		}
		#endregion

		#region Client Script
        /// <summary>
        /// Registers all the client script for the DynamicListBox
        /// </summary>
		protected virtual void RegisterClientScript()
		{
			this.RegisterLibraryScript();
			this.RegisterArrayScript();
			this.RegisterStartupScript();
		}

        /// <summary>
        /// Registers the library script for the DynamicListBox
        /// </summary>
		protected virtual void RegisterLibraryScript()
		{
			DynamicListBoxResourceHandler.RegisterScript(this.Page, "DynamicListBoxes_Library", "DynamicListBox.js");
		}

        /// <summary>
        /// Register this DynamicListBox with the Page
        /// </summary>
		protected virtual void RegisterArrayScript()
		{
			String ScriptArrayName = "DynamicListBoxes";
			Page.ClientScript.RegisterArrayDeclaration(ScriptArrayName, "'" + this.UniqueID + "'");
		}

        /// <summary>
        /// Registers the DynamicListBox startup script
        /// </summary>
		protected virtual void RegisterStartupScript()
		{
			String StartupScriptName = "DynamicListBoxes_Init";
			Page.ClientScript.RegisterStartupScript(typeof(DynamicListBox), StartupScriptName, "<script language='javascript'>\r\n<!--\r\nDynamicListBox_Init();\r\n//-->\r\n</script>");
		}


		#endregion

		private HtmlInputHidden itemTracker;

		/// <summary>
		/// Represents a command stored on the clientside which alters the Items collection of the <see cref="DynamicListBox"/>.
		/// </summary>
		/// <remarks>
		/// You don't need to use this in your code.
		/// </remarks>
		private class ListCommand
		{
			// Don't call this
			// use the static Parse and Split methods
			private ListCommand() { }

            /// <summary>
            /// Parses a single command string and returns the ListCommand created
            /// </summary>
            /// <param name="command">The string command to parse.</param>
            /// <returns>The created ListCommand</returns>
			public static ListCommand Parse(String command)
			{
				ListCommand newCommand = new ListCommand();

				if (command.StartsWith("+"))
				{
					newCommand.Operator = "+";
					String[] newItemText = command.Substring(1).Split(textSeperator);
					newCommand.Value = newItemText[0];
					newCommand.Text = newItemText[1];
					if (newItemText.Length == 3)
					{
						String IndexText = newItemText[2];
						try
						{
							newCommand.Index = Int32.Parse(IndexText, System.Globalization.CultureInfo.InvariantCulture);
						}
						catch { }
					}
				}
				else if (command.StartsWith("-"))
				{
					newCommand.Operator = "-";
					String removalIndexString = command.Substring(1);
					try
					{
						newCommand.Index = Int32.Parse(removalIndexString, System.Globalization.CultureInfo.InvariantCulture);
					}
					catch { }
				}

				return newCommand;
			}

            /// <summary>
            /// Splits a full command string set into an array of ListCommands.
            /// </summary>
            /// <param name="postedCommands">The set of commands to split.</param>
            /// <returns>The created array of ListCommands.</returns>
            /// <remarks>You don't need to use this from your code.</remarks>
			public static ListCommand[] Split(String postedCommands)
			{
				String[] commandsText = postedCommands.Split(commandSeperator);
				ListCommand[] commands = new ListCommand[commandsText.Length];
				for (Int32 i = 0; i < commandsText.Length; i++)
				{
					commands[i] = ListCommand.Parse(commandsText[i]);
				}
				return commands;
			}

			private static readonly Char commandSeperator = '\x1F';
			private static readonly Char textSeperator = '\x03';

			/// <summary>
			/// Gets or sets the operator of the command. + or -.
			/// </summary>
			public String Operator = "";
			/// <summary>
			/// Gets or sets the text of the added ListItem.
			/// </summary>
			public String Text = "";
			/// <summary>
			/// Gets or sets the value of the added ListItem.
			/// </summary>
			public String Value = "";
			/// <summary>
			/// Gets or sets the index of the ListItem to add or remove.
			/// </summary>
			public Int32 Index = -1;
		}
	}
}
