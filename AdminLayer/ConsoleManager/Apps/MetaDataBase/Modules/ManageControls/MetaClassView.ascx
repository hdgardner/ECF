<%@ Control Language="C#" AutoEventWireup="true" Inherits="Mediachase.Ibn.Web.UI.Apps.MetaDataBase.Modules.ManageControls.MetaClassView" Codebehind="MetaClassView.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" Src="~/Apps/MetaDataBase/Common/Design/BlockHeader.ascx" %>
<%@ Register TagPrefix="ibn2" Namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<asp:Panel ID="Panel1" ScrollBars="Auto" runat="server">
	<table cellspacing="0" cellpadding="0" border="0" width="100%" class="ibn-stylebox2">
		<tr>
		<td>
		  <ibn:BlockHeader id="secHeader" runat="server" />
		</td>
	  </tr>
	  <tr>
		<td>
			<asp:UpdatePanel runat="server" ID="upMain" ChildrenAsTriggers="true" UpdateMode="Conditional">
				<ContentTemplate>
					<ibn2:XMLFormBuilder ID="xmlStruct" runat="server" />
				</ContentTemplate>
			</asp:UpdatePanel>
			<asp:UpdateProgress ID="uProgress" runat="server" AssociatedUpdatePanelID="upMain" DisplayAfter="1000">
				<ProgressTemplate>
					<div class="upProgressMain">
						<div class="upProgressCenter">
							<img align='absmiddle' border='0' src='<%# Mediachase.Ibn.Web.UI.CHelper.GetAbsolutePath("/Images/loading_rss.gif") %>' />&nbsp;Loading...
						</div>
					</div>
				</ProgressTemplate>
			</asp:UpdateProgress>
		</td>
	  </tr>
	</table>
</asp:Panel>