<%@ Control Language="C#" AutoEventWireup="true" Inherits="Mediachase.Commerce.Manager.Order.OrderSearchList" Codebehind="OrderSearch.ascx.cs" %>
<%@ Register Src="../Core/Controls/EcfListViewControl.ascx" TagName="EcfListViewControl" TagPrefix="core" %>
<IbnWebControls:McDock ID="DockTop" runat="server" Anchor="Top" EnableSplitter="False" DefaultSize="75">
    <DockItems>
        <asp:Panel runat="server" ID="pnlMain" DefaultButton="btnSearch" Height="75px" BackColor="#F8F8F8" BorderColor="Gray" BorderWidth="0">
            <div id="DataForm">
                <table cellpadding="4" style="background-color: #F8F8F8;">
                    <tr>
                        <td><asp:Literal ID="Literal1" runat="server" Text="<%$ Resources:SharedStrings, Class_Type %>" />:</td><td>
                            <asp:DropDownList ID="ClassType" Width="140" runat="server">
                                <asp:ListItem Value="PurchaseOrder" Text="<%$ Resources:OrderStrings, Order_Purchase_Order %>" Selected="True"/>
                                <asp:ListItem Value="ShoppingCart" Text="<%$ Resources:OrderStrings, Order_Shopping_Cart %>"/>
                                <asp:ListItem Value="PaymentPlan" Text="<%$ Resources:OrderStrings, Order_Payment_Plan %>"/>
                            </asp:DropDownList>
                        </td>
                        <td style="width: 20px;">&nbsp;</td>
                        <td></td><td></td>
                        <td style="width: 20px;">&nbsp;</td>
                        <td></td>
                    </tr>
                    <tr>
                        <td><asp:Literal ID="Literal2" runat="server" Text="<%$ Resources:SharedStrings, Status %>" />:</td><td><asp:DropDownList ID="OrderStatus" Width="140" runat="server"></asp:DropDownList></td>
                        <td style="width: 20px;">&nbsp;</td>
                        <td><asp:Literal ID="Literal3" runat="server" Text="<%$ Resources:SharedStrings, ID %>" />:</td><td><asp:TextBox ID="OrderNumber" Width="140" runat="server"></asp:TextBox></td>
                        <td style="width: 20px;">&nbsp;</td>
                        <td></td>
                    </tr>
                    <tr>
                        <td><asp:Literal ID="Literal4" runat="server" Text="<%$ Resources:SharedStrings, Date_Range %>" />:</td><td><asp:DropDownList ID="DataRange" Width="140" runat="server"></asp:DropDownList></td>
                        <td style="width: 20px;">&nbsp;</td>
                        <td><asp:Literal ID="Literal5" runat="server" Text="<%$ Resources:SharedStrings, Customer %>" />:</td><td><asp:TextBox ID="CustomerKeyword" Width="140" runat="server"></asp:TextBox></td>
                        <td style="width: 20px;">&nbsp;</td>
                        <td>
                            <asp:UpdatePanel ID="upSearchButton" ChildrenAsTriggers="true" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:Button ID="btnSearch" runat="server" Width="100" Text="<%$ Resources:SharedStrings, Search %>" />
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </td>
                    </tr>
                </table>
            </div>
        </asp:Panel>
        </DockItems>
</IbnWebControls:McDock>

<core:EcfListViewControl id="MyListView" runat="server" AppId="Order" ViewId="OrderSearch-List" ShowTopToolbar="false"></core:EcfListViewControl>
<orders:OrderDataSource runat="server" ID="OrderListDataSource"></orders:OrderDataSource>