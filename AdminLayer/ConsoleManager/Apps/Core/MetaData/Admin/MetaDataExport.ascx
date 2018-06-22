<%@ Control Language="c#" Inherits="Mediachase.Commerce.Manager.Core.MetaData.Admin.MetaDataExport" Codebehind="MetaDataExport.ascx.cs" %>
<%@ Register TagPrefix="console" Namespace="Mediachase.Web.Console.Controls" Assembly="Mediachase.WebConsoleLib" %>
<div id="FormMultiPage">
    <table class="FormMultiPage">
        <tr>
            <td class="FormLabelCell">
                <table cellpadding="5" width="70%">
    <tr runat="server" id="CatalogRow">
        <td>
            <!-- START: Catalog Meta Classes -->
            <div style="width:100%">
                <asp:Panel runat="server" ID="CatalogMetaClassesPanel1" Height="25px">
                    <div style="padding:5px; cursor: pointer; vertical-align: middle; background-color:#6F8AD2">
                        <div style="float: left;">
                            <asp:Label ID="Label1" runat="server" Font-Bold="true" Text="<%$ Resources:CoreStrings, MetaData_Catalog_Meta_Classes %>"></asp:Label>
                        </div>
                        <div style="float: right; vertical-align: middle;">
                            <asp:Image ID="ImageExpand1" runat="server" ImageUrl="~/App_Themes/Default/images/Expand.jpg"/>
                        </div>
                    </div>
                </asp:Panel>
                <asp:Panel runat="server" ID="CatalogMetaClassesPanel2">
                    <asp:DataGrid id="CatalogItemsGrid" runat="server" CssClass="Grid" AllowPaging="False" Width="100%" ShowFooter="True" DataKeyField="MetaClassId" AutoGenerateColumns="False">
	                    <ItemStyle CssClass="DataCell"></ItemStyle>
	                    <AlternatingItemStyle CssClass="DataCell"></AlternatingItemStyle>
	                    <HeaderStyle CssClass="HeadingCell"></HeaderStyle>
	                    <SelectedItemStyle CssClass="SelectedRow" />
	                    <Columns>
	                        <console:RowSelectorColumn ItemStyle-Width="10%" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"
		                            allowselectall="false" SelectionMode="Multiple" HeaderText="Select" AutoPostBack="False" />
		                    <asp:TemplateColumn HeaderText="Name" HeaderStyle-Width="55%">
			                    <ItemTemplate>
			                        <asp:Label runat="server" ID="MetaClassNameLabel" Text='<%# DataBinder.Eval(Container, "DataItem.FriendlyName")%>'></asp:Label>
			                    </ItemTemplate>
		                    </asp:TemplateColumn>
		                    <asp:TemplateColumn HeaderText="SystemName" HeaderStyle-Width="35%">
			                    <ItemTemplate>
			                        <asp:Label runat="server" ID="MetaClassSystemNameLabel" Text='<%# DataBinder.Eval(Container, "DataItem.SystemName")%>'></asp:Label>
			                    </ItemTemplate>
		                    </asp:TemplateColumn>
	                    </Columns>
                    </asp:DataGrid>
                    
                </asp:Panel>
                <ajaxToolkit:CollapsiblePanelExtender runat="server" ID="CatalogCollapsiblePanelExtender"
                      ExpandControlID="CatalogMetaClassesPanel1" CollapseControlID="CatalogMetaClassesPanel1" TargetControlID="CatalogMetaClassesPanel2"
                      Collapsed="false" ExpandedText="Hide" CollapsedText="Show"
                      SuppressPostBack="true"
                      ImageControlID="ImageExpand1" ExpandedImage="~/App_Themes/Default/images/collapse.jpg" CollapsedImage="~/App_Themes/Default/images/expand.jpg"></ajaxToolkit:CollapsiblePanelExtender>
            </div>
            <!-- END: Catalog Meta Classes -->
        </td>
    </tr>
    <tr runat="server" id="OrderRow">
        <td>
            <!-- START: Order Meta Classes -->
            <div style="width:100%">
                <asp:Panel runat="server" ID="OrderMetaClassesPanel1" Height="25px">
                    <div style="padding:5px; cursor: pointer; vertical-align: middle; background-color:#6F8AD2">
                        <div style="float: left;">
                            <asp:Label ID="Label2" runat="server" Font-Bold="true" Text="<%$ Resources:CoreStrings, MetaData_Order_Meta_Classes %>"></asp:Label>
                        </div>
                        <div style="float: right; vertical-align: middle;">
                            <asp:Image ID="ImageExpand2" runat="server" ImageUrl="~/App_Themes/Default/images/Expand.jpg"/>
                        </div>
                    </div>
                </asp:Panel>
                <asp:Panel runat="server" ID="OrderMetaClassesPanel2">
                    <asp:DataGrid id="OrderItemsGrid" runat="server" CssClass="Grid" AllowPaging="False" Width="100%" ShowFooter="True" DataKeyField="MetaClassId" AutoGenerateColumns="False">
	                    <ItemStyle CssClass="Row DataCell"></ItemStyle>
	                    <AlternatingItemStyle CssClass="HoverRow DataCell"></AlternatingItemStyle>
	                    <HeaderStyle CssClass="HeadingCell"></HeaderStyle>
	                    <SelectedItemStyle CssClass="SelectedRow" />
	                    <Columns>
	                        <console:RowSelectorColumn ItemStyle-Width="10%" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"
		                            allowselectall="false" SelectionMode="Multiple" HeaderText="Select" AutoPostBack="False" />
		                    <asp:TemplateColumn HeaderText="Name" HeaderStyle-Width="55%">
			                    <ItemTemplate>
			                        <asp:Label runat="server" ID="MetaClassNameLabel" Text='<%# DataBinder.Eval(Container, "DataItem.FriendlyName")%>'></asp:Label>
			                    </ItemTemplate>
		                    </asp:TemplateColumn>
		                    <asp:TemplateColumn HeaderText="SystemName" HeaderStyle-Width="35%">
			                    <ItemTemplate>
			                        <asp:Label runat="server" ID="MetaClassSystemNameLabel" Text='<%# DataBinder.Eval(Container, "DataItem.SystemName")%>'></asp:Label>
			                    </ItemTemplate>
		                    </asp:TemplateColumn>
	                    </Columns>
                    </asp:DataGrid>
                </asp:Panel>
                <ajaxToolkit:CollapsiblePanelExtender runat="server" ID="OrderCollapsiblePanelExtender"
                      ExpandControlID="OrderMetaClassesPanel1" CollapseControlID="OrderMetaClassesPanel1" TargetControlID="OrderMetaClassesPanel2"
                      Collapsed="false" ExpandedText="Hide" CollapsedText="Show"
                      SuppressPostBack="true"
                      ImageControlID="ImageExpand2" ExpandedImage="~/App_Themes/Default/images/collapse.jpg" CollapsedImage="~/App_Themes/Default/images/expand.jpg"></ajaxToolkit:CollapsiblePanelExtender>
            </div>
            <!-- END: Order Meta Classes -->
        </td>
    </tr>
    <tr runat="server" id="ProfileRow">
        <td>
            <!-- START: Profile Meta Classes -->
            <div style="width:100%">
                <asp:Panel runat="server" ID="ProfileMetaClassesPanel1" Height="25px">
                    <div style="padding:5px; cursor: pointer; vertical-align: middle; background-color:#6F8AD2">
                        <div style="float: left;">
                            <asp:Label ID="Label3" runat="server" Font-Bold="true" Text="<%$ Resources:CoreStrings, MetaData_Profile_Meta_Classes %>"></asp:Label>
                        </div>
                        <div style="float: right; vertical-align: middle;">
                            <asp:Image ID="Image1" runat="server" ImageUrl="~/App_Themes/Default/images/Expand.jpg"/>
                        </div>
                    </div>
                </asp:Panel>
                <asp:Panel runat="server" ID="ProfileMetaClassesPanel2">
                    <asp:DataGrid id="ProfileItemsGrid" runat="server" CssClass="Grid" AllowPaging="False" Width="100%" ShowFooter="True" DataKeyField="MetaClassId" AutoGenerateColumns="False">
	                    <ItemStyle CssClass="Row DataCell"></ItemStyle>
	                    <AlternatingItemStyle CssClass="HoverRow DataCell"></AlternatingItemStyle>
	                    <HeaderStyle CssClass="HeadingCell"></HeaderStyle>
	                    <SelectedItemStyle CssClass="SelectedRow" />
	                    <Columns>
	                        <console:RowSelectorColumn ItemStyle-Width="10%" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"
		                            allowselectall="false" SelectionMode="Multiple" HeaderText="Select" AutoPostBack="False" />
		                    <asp:TemplateColumn HeaderText="Name" HeaderStyle-Width="55%">
			                    <ItemTemplate>
			                        <asp:Label runat="server" ID="MetaClassNameLabel" Text='<%# DataBinder.Eval(Container, "DataItem.FriendlyName")%>'></asp:Label>
			                    </ItemTemplate>
		                    </asp:TemplateColumn>
		                    <asp:TemplateColumn HeaderText="SystemName" HeaderStyle-Width="35%">
			                    <ItemTemplate>
			                        <asp:Label runat="server" ID="MetaClassSystemNameLabel" Text='<%# DataBinder.Eval(Container, "DataItem.SystemName")%>'></asp:Label>
			                    </ItemTemplate>
		                    </asp:TemplateColumn>
	                    </Columns>
                    </asp:DataGrid>
                </asp:Panel>
                <ajaxToolkit:CollapsiblePanelExtender runat="server" ID="ProfileCollapsiblePanelExtender"
                      ExpandControlID="ProfileMetaClassesPanel1" CollapseControlID="ProfileMetaClassesPanel1" TargetControlID="ProfileMetaClassesPanel2"
                      Collapsed="false" ExpandedText="Hide" CollapsedText="Show"
                      SuppressPostBack="true"
                      ImageControlID="ImageExpand2" ExpandedImage="~/App_Themes/Default/images/collapse.jpg" CollapsedImage="~/App_Themes/Default/images/expand.jpg"></ajaxToolkit:CollapsiblePanelExtender>
            </div>
            <!-- END: Profile Meta Classes -->
        </td>
    </tr>
   
    <tr>
        <td align="left">
            <!-- START: Export Button -->
            <asp:Button runat="server" ID="BtnExport" Text="<%$ Resources:SharedStrings, Export %>" OnClick="BtnExport_Click" />
            <!-- END: Export Button -->
        </td>
    </tr>
</table>
            </td>
        </tr>
    </table>
</div>