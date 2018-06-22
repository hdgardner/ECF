<%@ Control Language="C#" ClassName="NodeInfo" AutoEventWireup="true" Inherits="Mediachase.Cms.WebUtility.BaseControls.BaseNodeTemplate, Mediachase.Cms.WebUtility"%>
<%@ Import Namespace="Mediachase.Commerce.Catalog.Objects" %>
<%@ Import Namespace="Mediachase.Commerce.Catalog" %>
<%@ Import Namespace="Mediachase.Commerce.Catalog.Managers" %>
<%@ Import Namespace="Mediachase.Cms.WebUtility.Commerce" %>

<script runat="server">
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        BindFields();
    }

    private void BindFields()
    {
        DataBind();
    }
</script>


<table width="100%" cellpadding="0" cellspacing="0" id="Table1">
	<tr valign="top">
		<td style="width: 100%">
		    <table cellspacing="0" cellpadding="0" style="width: 100%" border="0">
			    <tbody>
				    <tr>
					    <td style="width:287; height:217" rowspan="3">
					        <cms:MetaImage PropertyName="PrimaryImage" DataSource="<%#Node.ItemAttributes.Images%>" runat="server" />
					    </td>
				    </tr>
				    <tr>
					    <td style="width:100%;height:195" valign="top">
					        <div style="PADDING-LEFT: 30px;">
					            <%# Node.ItemAttributes["Description"]%>
					        </div>
					    </td>
				    </tr>
			    </tbody>
		    </table>
		</td>
	</tr>
	<tr>
		<td style="width: 100%" align="left">
			<div id="pagination" style="text-align:left"><br/>
				<asp:placeholder id="PageLinks" runat="server"></asp:placeholder></div>
			<asp:DataList EnableViewState="False" RepeatDirection="Horizontal" DataSource="<%#Node.Children.CatalogNode %>" RepeatColumns="4" cellpadding="0" cellspacing="5" Runat="server"
				ID="CategoryList">
				<ItemStyle VerticalAlign="Top" Width="123" HorizontalAlign="Left" CssClass="ecf-category-item"></ItemStyle>
				<ItemTemplate>
				    <asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl='<%# StoreHelper.GetNodeUrl((CatalogNode)Container.DataItem)%>'>
                        <cms:MetaImage PropertyName="PrimaryImage" DataSource='<%#DataBinder.Eval(Container.DataItem, "ItemAttributes.Images")%>' runat="server" />
						<h3><%#((ItemAttributes)DataBinder.Eval(Container.DataItem, "ItemAttributes"))["DisplayName"]%></h3>
				    </asp:HyperLink>
				</ItemTemplate>
				<SeparatorTemplate>
				    <cms:ThemedImage runat="server" Width="2" Height="1" ImageUrl="images/spacer.gif"/>
				</SeparatorTemplate>
				<FooterTemplate>
				    <cms:ThemedImage runat="server" Width="123" Height="20" ImageUrl="images/spacer.gif"/>
				</FooterTemplate>
			</asp:DataList>
		</td>
	</tr>
</table>
