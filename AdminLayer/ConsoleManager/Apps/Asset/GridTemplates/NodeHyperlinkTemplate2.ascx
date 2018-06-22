<%@ Control Language="C#" AutoEventWireup="true" Inherits="Apps_Asset_GridTemplates_NodeHyperlinkTemplate2" Codebehind="NodeHyperlinkTemplate2.ascx.cs" %>
<asp:HyperLink ID="HyperLink1" runat="server" Target="_blank" NavigateUrl='<%# DataBinder.Eval(DataBinder.GetDataItem(Container), "[Url]")%>' ImageUrl='<%# String.Format("{0}", DataBinder.Eval(DataBinder.GetDataItem(Container), "[Icon]")) %>'/>
<asp:HyperLink ID="HyperLink2" runat="server" 
 NavigateUrl='<%# String.Format("javascript:CSAssetClient.ViewItem(\"{0}\",\"{1}\");", DataBinder.Eval(DataItem,"[Type]"), DataBinder.Eval(DataBinder.GetDataItem(Container), "[ID]")) %>' 
 Text='<%# DataBinder.Eval(DataBinder.GetDataItem(Container), "[FileName]") %>'/>
 <asp:HyperLink ID="HyperLink3" runat="server" 
NavigateUrl='<%# String.Format("javascript:CSAssetClient.ViewItem(\"Folder\", \"{0}\");", DataBinder.Eval(DataBinder.GetDataItem(Container), "[GrandParentId]")) %>' 
Text='<%#DataBinder.Eval(DataBinder.GetDataItem(Container), "[Name]") %>' 
Visible='<%# String.Compare((string)DataBinder.Eval(DataItem, "[Type]"), "LevelUp", true)==0 %>'></asp:HyperLink>
<%--<asp:Image ID="Image1" runat="server" ImageUrl='<%# DataBinder.Eval(DataBinder.GetDataItem(Container), "[Type]", "~/app_themes/Default/images/icons/{0}.gif")%>' />
<asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl='<%# String.Format("javascript:CSCatalogClient.OpenItem(\"{0}\",\"{1}\");", DataBinder.Eval(DataItem,"[Type]"), DataBinder.Eval(DataBinder.GetDataItem(Container), "[ID]")) %>' Text='<%# DataBinder.Eval(DataBinder.GetDataItem(Container),"[Name]") %>'>
</asp:HyperLink>--%>
