<%@ Control Language="C#" AutoEventWireup="true" Inherits="Mediachase.Cms.Templates.Everything.MainMenuControl" Codebehind="MainMenuControl.ascx.cs" %>
<div class="top-menu" id="menu">
        <asp:Repeater runat="server" ID="SiteMenu" EnableViewState="false">
            <ItemTemplate><div class="top-menu-normal-column"><a class="top-menu-item" href='<%#Page.ResolveUrl(Eval("navigateurl").ToString())%>'><%# Eval("Text") %></a></div></ItemTemplate>
        </asp:Repeater>
        <div runat="server" class="top-menu-last-column"><a runat="server" class="top-menu-item" id="TopMenuLastColumn" href='' /></div>
</div>
