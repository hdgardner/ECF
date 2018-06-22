<%@ Control Language="C#" AutoEventWireup="true" Inherits="InlineEditor2" Codebehind="InlineEditor.ascx.cs" %>
<asp:Panel ID="wrapperInline" Style="position: relative; z-index: 1; width: 100%;  height: 100%;" ActionSet="NoneMenu" runat="server">
    <cms:ActiveRegion runat="Server" ID="label" EditWrapper="2" AllowEdit="False">
        <innerText>InlineEdit</innerText>
    </cms:ActiveRegion>
</asp:Panel>
<asp:HiddenField runat="server" ID="hfEditInfo" Value="" />