<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchLink.ascx.cs" Inherits="Mediachase.Cms.Website.Templates.NWTD.SearchLink" %>
<div id="nwtd-searchLink">
    <asp:HyperLink Visible='<%# (NWTD.Profile.CurrentUserLevel.Equals(NWTD.UserLevel.A)) %>' ID="HyperLink1" runat="server" NavigateUrl="~/catalog/searchresults.aspx">Search our Catalog</asp:HyperLink>
</div>