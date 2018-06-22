<%@ Control Language="C#" AutoEventWireup="true" Inherits="Mediachase.Cms.Templates.Default.SearchModule" Codebehind="SearchModule.ascx.cs" %>
<asp:Panel ID="SearchPanel" runat="server" DefaultButton="SearchButton"><b><%=RM.GetString("SEARCH_LABEL")%></b>
<asp:DropDownList Runat="server" ID="SearchFilter"></asp:DropDownList>
<b>&nbsp;&nbsp;<%=RM.GetString("SEARCH_FOR_LABEL")%>&nbsp;&nbsp;</b> <asp:TextBox Runat="server" Size="15" ID="Search"></asp:TextBox>&nbsp;&nbsp;
<cms:ThemedImageButton ImageAlign="AbsMiddle" CausesValidation="False" OnClick="OnSearch" Runat="server" ImageUrl="Images/go.gif" ID="SearchButton"></cms:ThemedImageButton></asp:Panel>
