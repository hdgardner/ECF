<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CatalogEntrySubsetSelector.ascx.cs" Inherits="Mediachase.Commerce.Manager.Marketing.EntrySearch" %>
<%@ Register Assembly="Mediachase.Ibn.Web.UI.WebControls" Namespace="Mediachase.Ibn.Web.UI.WebControls" TagPrefix="mc" %>

<div style="background-color: #F8F8F8; height:100%">
	<table cellpadding="2" style="background-color: #F8F8F8; height: 100%;">
        <tr>
            <td class="FormLabelCell" style="width:150px">
                <b><asp:Literal ID="Literal1" runat="server" Text="<%$ Resources:CatalogStrings, Catalog_Search_By_Keyword %>" />:</b>
            </td>
            <td class="FormFieldCell" colspan="2">
                <asp:TextBox ID="tbKeywords" Width="240" runat="server"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="FormLabelCell">
                <asp:Literal ID="Literal3" runat="server" Text="<%$ Resources:CatalogStrings, Catalog_Filter_By_Language %>" />:
            </td>
            <td class="FormFieldCell" colspan="2">
                <asp:DropDownList runat="server" ID="ListLanguages" Width="250" DataValueField="LanguageId"
                    DataTextField="Name">
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td class="FormLabelCell">
                <asp:Literal ID="Literal5" runat="server" Text="<%$ Resources:CatalogStrings, Catalog_Filter_By_Catalog %>" />:
            </td>
            <td class="FormFieldCell">
                <asp:DropDownList runat="server" ID="ListCatalogs" Width="250" DataValueField="CatalogId"
                    DataTextField="Name">
                </asp:DropDownList>
            </td>
            <td>
                <asp:Button ID="btnSearch" runat="server" Width="100" Text="<%$ Resources:SharedStrings, Search %>" />
            </td>
        </tr>
        <tr>
			<td colspan="3">
			    <mc:ListToListSelector runat="server" ID="ltlSelector" OneItemToTargetButtonId="btnMoveSelectedToTarget" OneItemToSourceButtonId="btnMoveSelectedToSource" SourceListId="lbSource" TargetListId="lbTarget">
		        </mc:ListToListSelector>
				<table border="0" cellspacing="0">
					<tr>
						<td style="padding:10px;">
							<asp:Literal runat="server" Text="<%$Resources:CatalogStrings, Catalog_Found_Entries %>"></asp:Literal><br />
							<asp:ListBox runat="server" ID="lbSource" SelectionMode="Multiple" 
								DataTextField="Name" DataValueField="CatalogEntryId" Width="200" Height="125" >
							</asp:ListBox>
						</td>
						<td  style="vertical-align:middle; padding:10px; text-align:center;">
							<asp:Button runat="server" ID="btnMoveSelectedToTarget" Text=">" OnClientClick="return false;" /><br />
							<asp:Button runat="server" ID="btnMoveSelectedToSource" Text="<" OnClientClick="return false;"  />
						</td>
						<td style="padding:10 10 0 10">
							<asp:Literal  runat="server" Text="<%$Resources:CatalogStrings, Catalog_Selected_Entries %>"></asp:Literal><br />
							<asp:ListBox runat="server" ID="lbTarget" SelectionMode="Multiple" Width="200" Height="125"></asp:ListBox>
						</td>
					</tr>
				</table>
			</td>
        </tr>
        <tr>
			<td colspan="3" style="text-align:right; padding:0 10 10 0">
				<asp:Button runat="server" ID="btnSave" Text="<%$Resources:CatalogStrings, Catalog_Save_Selection %>" Width="100" />
			</td>
        </tr>
    </table>
</div>