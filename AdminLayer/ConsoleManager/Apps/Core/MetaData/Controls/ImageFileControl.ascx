<%@ Control Language="c#" Inherits="Mediachase.Commerce.Manager.Core.MetaData.MetaControls.ImageFileControl" Codebehind="ImageFileControl.ascx.cs" %>
<%@ Register TagPrefix="mc" Namespace="Mediachase.FileUploader.Web.UI" Assembly="Mediachase.FileUploader" %>

<tr>
	<td class="FormLabelCell"><asp:Label id="MetaLabelCtrl" runat="server" Text="<%$ Resources:SharedStrings, Label %>"></asp:Label>:</td>
	<td class="FormFieldCell">
	    <mc:McHtmlInputFile CssClass="text" ID="MetaValueCtrl" runat="server" Size="60" Width="400" />
		<br><asp:Label id="MetaDescriptionCtrl" runat="server" Text="<%$ Resources:SharedStrings, Label %>"></asp:Label>
		<br>
		<IMG runat="server" id="CurrentPicture" alt="">
		<br />
		<asp:CheckBox Visible="false" runat="server" ID="RemovePicture" Text="<%$ Resources:SharedStrings, Delete_Picture %>" />
		<!--
		<br />
		<asp:TextBox Width="250" runat="server" ID="CaptionText" MaxLength="50"></asp:TextBox>
		<br><asp:Label id="CaptionDescriptionCtrl" runat="server">Please enter image caption text</asp:Label>
		-->
	</td>
</tr>

