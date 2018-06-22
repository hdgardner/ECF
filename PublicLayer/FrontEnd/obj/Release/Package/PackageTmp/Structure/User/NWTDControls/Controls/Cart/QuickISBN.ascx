<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="QuickISBN.ascx.cs" Inherits="Mediachase.Cms.Website.Structure.User.NWTDControls.Controls.Cart.QuickISBN" %>
<%@ Import Namespace="Mediachase.Commerce.Catalog" %>
<%@ Import Namespace="Mediachase.Commerce.Catalog.Objects" %>
<h1>Add Multiple ISBNs to <%=this.SelectedCartName %></h1>
<asp:Wizard 
	runat="server"  
	FinishCompleteButtonText="Add Items to Wish List"
	DisplaySideBar="false" 
	DisplayCancelButton="true"  
	ID="wzImportISBNS" 
	onfinishbuttonclick="wzImportISBNS_FinishButtonClick" 
	oncancelbuttonclick="wzImportISBNS_CancelButtonClick">
	<FinishNavigationTemplate>
		<asp:Button ID="btnFinish" runat="server" CssClass="add-isbns-to-cart-btn buttons"
            CausesValidation="true" Text="Add Items to Wish List"
            CommandName="MoveComplete" 
        />
		 <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="cancel-isbn-btn buttons"
            CausesValidation="false" OnClick="wzImportISBNS_CancelButtonClick"
        />
		
	</FinishNavigationTemplate>
	<WizardSteps>
		<asp:WizardStep runat="server" ID="wsAddISBNS" Title="Quick ISBN Entry" >
			<p>To add, please enter multiple <strong>ISBN-13</strong> numbers separated by line breaks. All items that are found will be added to your active Wish List. A confirmation message will be displayed to indicate which items were found and what errors occurred. You will be able to modify or remove items from your Wish List after the import is complete.</p>
			<p><strong>NOTE: If you have ISBN-10 numbers, they will need to be converted to ISBN-13 before importing.  You can use an <a href="http://isbn13converter.pearsoned.com/" target="_blank">online ISBN converter</a> to do this conversion.</strong></p>
			<asp:TextBox runat="server" ID="tbISNBS" TextMode="MultiLine" Height="200px" Width="200px" />
		</asp:WizardStep>
		<asp:WizardStep ID="wsConfirmation" StepType="Complete" AllowReturn="false">
			<asp:Panel runat="server" ID="pnlImportFailure" Visible="false">
				<p>
					The following ISBNs were not imported successfully. Please check their accuracy and contact us if you need help.
				</p>
				<asp:BulletedList runat="server" ID="blFailedISBNS" CssClass="nwtd-failedISBNImport" />
			</asp:Panel>
			<asp:Panel runat="server" ID="pnlImportSuccess" Visible="false">
				<p>
					The following items were imported into your active Wish List. Please review the list - if you need to
					change or remove an item, please make a note of it and you can modify it in your Wish List.
				</p>
				<asp:GridView runat="server" ID="gvImportedEntries" OnRowDataBound="gvImportedEntries_RowDataBound"  AutoGenerateColumns="false">
					<Columns>
						<asp:TemplateField HeaderText="Title">
							<ItemTemplate>
								<asp:HyperLink ID="linkEntryLink" runat="server" NavigateUrl='<%# StoreHelper.GetEntryUrl((Entry)Container.DataItem)%>'><%# StoreHelper.GetEntryDisplayName((Entry)Container.DataItem)%></asp:HyperLink> 
							</ItemTemplate>
						</asp:TemplateField>
						<asp:TemplateField HeaderText="Publisher Number / ISBN13" >
							<ItemTemplate>
								<%# ((Mediachase.Commerce.Catalog.Objects.ItemAttributes)Eval("ItemAttributes"))["ISBN13"]%>
							</ItemTemplate>
						</asp:TemplateField>
						<asp:TemplateField HeaderText="Year">
							<ItemTemplate>
								<%# ((Mediachase.Commerce.Catalog.Objects.ItemAttributes)Eval("ItemAttributes"))["Year"]%>
							</ItemTemplate>
						</asp:TemplateField>
					</Columns>
				</asp:GridView>
			</asp:Panel>
			<br />
			<asp:HyperLink ID="hlVAddMore" Text="" runat="server" CssClass="add-more-isbns-to-cart-btn"  EnableViewState="false" NavigateUrl="~/Cart/QuickISBN.aspx" />
			<asp:HyperLink ID="hlViewCart" Text="" runat="server" CssClass="view-cart-btn" EnableViewState="false" NavigateUrl="~/cart/view.aspx" />
		</asp:WizardStep>
	</WizardSteps>
</asp:Wizard>