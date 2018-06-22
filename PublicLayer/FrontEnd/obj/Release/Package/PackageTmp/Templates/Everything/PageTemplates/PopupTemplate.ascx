<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PopupTemplate.ascx.cs"
    Inherits="Mediachase.Cms.Website.Templates.Everything.PageTemplates.PopupTemplate" %>
<%@ Register Src="~/Structure/Base/Controls/Common/ErrorModule.ascx" TagPrefix="cms"
    TagName="ErrorModule" %>
<%@ Register Src="~/Structure/Base/Controls/Image/ImageView.ascx" TagPrefix="cms"
    TagName="Image" %>
    
<style type="text/css">
    .mainHeader
    {
        height: 60px;
        border-bottom-color: #CCCCCC;
        border-bottom-style: solid;
        border-bottom-width: 1px;
        text-align: right;
    }
    .mainTitle
    {
        font-family: Arial, Helvetica, sans-serif;
        color: #808080;
        font-size: 2em;
        font-weight: bold;
        padding-top: 30px;
    }
    .mainFooter
    {
        border-top-color: #CCCCCC;
        border-top-style: solid;
        border-top-width: 1px;
        text-align: right;
        margin-top: 8px;
        padding-top: 8px;
    }
    .buttonCloseWindow
    {
        font-family: Arial,Helvetica,sans-serif;
        font-size: 11px;
        border: solid 1px #9e9e9e;
        background-color: #F5F5F5;
        color: #6a686c;
        width: 120px;
    }
</style>
<div style="padding: 20px 25px;">
    <div class="mainHeader">
        <div id="sitetitle" style="text-align: left;"><cms:Image runat="server" ID="SiteLogo" ImageUrl="~/images/title.gif" />&nbsp;</asp:HyperLink></div>
        <div class="mainTitle">
            <asp:Literal ID="Literal2" runat="server" Text="<%# Page.Title%>" />
        </div>
    </div>
    <cms:ErrorModule ID="ErrorModule1" runat="server" EnableViewState="false"></cms:ErrorModule>
    <!-- START: Content -->
    <cms:CmsPlaceHolder runat="server" ID="MainContentArea" />
    <!-- END: Content -->
    <div class="mainFooter">
        <button class="buttonCloseWindow" onclick="window.close();">
            Close Window</button>
    </div>
</div>
