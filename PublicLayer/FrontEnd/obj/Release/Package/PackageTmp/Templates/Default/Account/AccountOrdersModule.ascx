<%@ Control Language="C#" AutoEventWireup="true" Inherits="Mediachase.Cms.Website.Templates.Default.Modules.AccountOrdersModule" Codebehind="AccountOrdersModule.ascx.cs" %>
<%@ Import Namespace="Mediachase.Commerce.Shared" %>
<table style="width: 100%; text-align: left" border="0" cellspacing="1" cellpadding="5">
	<tr>
		<td style="width: 100%"><h1><%=RM.GetString("ACCOUNT_ORDERS_TITLE")%></h1>
		</td>
	</tr>
	<tr>
		<td>
			<p><%=RM.GetString("ACCOUNT_ORDERS_INFO_LABEL")%></p>
			<asp:datagrid id="OrdersList" runat="server" allowpaging="False" pagesize="15" allowsorting="True"
				cellpadding="3" gridlines="Horizontal" cellspacing="1" borderwidth="0px" autogeneratecolumns="False"
				style="width: 100%">
				<HeaderStyle CssClass="ecf-table-header"></HeaderStyle>
				<ItemStyle CssClass="ecf-table-item"></ItemStyle>
				<AlternatingItemStyle CssClass="ecf-table-item-alt"></AlternatingItemStyle>
				<Columns>
					<asp:TemplateColumn HeaderText="OrderId">
						<ItemTemplate>
						    <asp:HyperLink runat="server" NavigateUrl='<%# String.Format("~/Profile/secure/OrderDetail.aspx?order={0}", Eval("OrderGroupId")) %>' Font-Bold="true" Text='<%#Eval("TrackingNumber")%>'></asp:HyperLink>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="Total">
						<ItemTemplate>
							<asp:Label runat="server" Text='<%# CurrencyFormatter.FormatCurrency((decimal)Eval("Total"), (string)Eval("BillingCurrency")) %>' ID="Label3" NAME="Label1">
							</asp:Label>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:BoundColumn DataField="Status" HeaderText="Status">
					</asp:BoundColumn>					
					<asp:BoundColumn DataField="Created" HeaderText="Created">
					</asp:BoundColumn>
				</Columns>
			</asp:datagrid>
		</td>
	</tr>
</table>