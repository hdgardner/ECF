<%@ Page Language="C#" AutoEventWireup="true" Inherits="Mediachase.Commerce.Manager.Core.Controls.DialogPage" Codebehind="DialogPage.aspx.cs" Theme="Main" %>
<%@ register TagPrefix="ibn" namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="headTag" runat="server">
    <title></title>
    <script type="text/javascript">
        // this page should be inside frame
        if (top == self) top.location.href = 'default.aspx';
    </script>   
</head>
<body style="overflow: hidden;">
    <form id="DialogForm1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true" EnablePartialRendering="true" EnableScriptGlobalization="true" LoadScriptsBeforeUI="true" ScriptMode="debug"></asp:ScriptManager>
        <ibn:CommandManager runat="server" ContainerId="divContainer" ID="CM" />
        <div id="divContainer" runat="server" style="height: 0px" />
        <asp:Panel runat="server" ID="contentPanel"></asp:Panel>
        <script language="javascript">Ext.BLANK_IMAGE_URL = '../../shell/pages/images/s.gif';</script>
    </form>
</body>
</html>