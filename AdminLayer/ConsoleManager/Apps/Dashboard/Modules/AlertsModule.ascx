<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AlertsModule.ascx.cs"
    Inherits="Mediachase.Commerce.Manager.Dashboard.Modules.AlertsModule" %>
    
<div class="db-panel-outer">
    <div class="db-panel">
        <asp:ListView runat="server" ID="AlertsList">
            <EmptyDataTemplate>
                <p>
                <asp:Literal ID="Literal1" runat="server" Text="<%$ Resources:DashboardStrings, No_Alerts %>"/>
                </p>        
            </EmptyDataTemplate>
            <EmptyItemTemplate>
                <asp:Literal ID="Literal2" runat="server" Text="<%$ Resources:DashboardStrings, No_Alerts %>"/>
            </EmptyItemTemplate>
            <LayoutTemplate>
                <ol>
                    <asp:PlaceHolder runat="server" ID="itemPlaceholder"></asp:PlaceHolder>
                </ol>
            </LayoutTemplate>
            <ItemTemplate>
                <li><asp:Image runat="server" ImageUrl=<%# DataBinder.Eval(Container.DataItem, "Level", "~/App_Themes/Default/Images/{0}.png") %> AlternateText='<%#Eval("level") %>' /><%# Eval("Text") %></li>
            </ItemTemplate>
        </asp:ListView>
    </div>
</div>