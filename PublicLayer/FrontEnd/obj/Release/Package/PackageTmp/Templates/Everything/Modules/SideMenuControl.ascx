<%@ Control Language="C#" AutoEventWireup="true" Inherits="Mediachase.Cms.Templates.Everything.Modules.SideMenuControl"
    CodeBehind="SideMenuControl.ascx.cs" EnableViewState="false" %>
<%@ Register TagPrefix="ComponentArt" Namespace="ComponentArt.Web.UI" Assembly="ComponentArt.Web.UI" %>
<%@ Import Namespace="Mediachase.Cms.WebUtility.Search" %>
<div class="box ecf-nav">
    <asp:Repeater runat="server" ID="ActiveFilterList" EnableViewState="false">
        <HeaderTemplate>
            <h4>
                Currently Shopping By</h4>
            <div class="content">
                <ol>
        </HeaderTemplate>
        <ItemTemplate>
            <li>
            <cms:ThemedHyperLink runat="server" ImageUrl="images/list_remove_btn.gif" AlternateText="Remove This Item" CssClass="remove" NavigateUrl=<%#Eval("RemoveUrl") %>></cms:ThemedHyperLink>
            <span class="label"><%#Eval("Name")%>:</span> <%#Eval("ValueName")%>
            
            </li>
        </ItemTemplate>
        <FooterTemplate>
            </ol></div>
            <div class="actions">
                <a href="<%#SearchHelper.GetCleanUrl() %>">Clear All</a></div>
        </FooterTemplate>
    </asp:Repeater>
    <asp:Repeater runat="server" ID="FilterList" EnableViewState="false">
        <HeaderTemplate>
            <h4>
                Shopping Options</h4>
            <div class="content no-padding">
                <div class="ecf-filter">
                    <dl id="ecf-filter-list">
        </HeaderTemplate>
        <ItemTemplate>
            <dt>
                <%#Eval("Name")%></dt>
            <dd>
                <ol>
                    <asp:Repeater runat="server" ID="ValueList" DataSource='<%#Eval("Facets") %>'>
                        <ItemTemplate>
                            <li><a href='<%#SearchFilterHelper.GetQueryStringNavigateUrl((string)Eval("Group.FieldName"), (string)Eval("Key"))%>'>
                                <%#Eval("Name")%></a> (<%#Eval("Count")%>) </li>
                        </ItemTemplate>
                    </asp:Repeater>
                </ol>
            </dd>
        </ItemTemplate>
        <FooterTemplate>
            </dl> </div> </div>
        </FooterTemplate>
    </asp:Repeater>
</div>