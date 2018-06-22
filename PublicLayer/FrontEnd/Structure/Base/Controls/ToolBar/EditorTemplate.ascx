<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="Mediachase.Cms.Controls.Toolbar.EditorTemplate" Codebehind="EditorTemplate.ascx.cs" %>
<%@ Register TagPrefix="cms" TagName="ddLabel" Src="~/Structure/Base/Controls/ddLabel/ddLabel.ascx" %>
<%@ Register TagPrefix="design" TagName="ddTextBox" Src="~/Structure/Base/Controls/ddTextBox/ddTextBox.ascx" %>    
<asp:Label runat="server" ID="lblComment" /><cms:ddLabel runat="server" ID="lblCommentValue" />
<design:ddtextbox runat="server" id="ddText" />