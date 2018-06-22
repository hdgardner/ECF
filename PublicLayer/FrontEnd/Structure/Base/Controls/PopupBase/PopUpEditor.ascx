<%@ Control Language="C#" AutoEventWireup="true" EnableViewState="false" Inherits="PopUpEditor2" Codebehind="PopUpEditor.ascx.cs" %>
<asp:Panel ID="wrapperInline" Style="position: relative; z-index: 1;" ActionSet="NoneMenu" runat="server">
    <cms:ActiveRegion runat="Server" ID="label" EditWrapper="1" AllowEdit="False">
        <innerText>Double click to edit</innerText>
    </cms:ActiveRegion>
    <input type="hidden" id="tmpContainerValue" runat="server" style="width:0px;height:0px;line-height:0px;border:0px none black;display:none;visibility:hidden;" />
    <asp:HiddenField runat="server" ID="hfEditInfo" Value="" />
</asp:Panel>