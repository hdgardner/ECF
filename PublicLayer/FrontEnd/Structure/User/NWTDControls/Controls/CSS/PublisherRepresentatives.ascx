<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PublisherRepresentatives.ascx.cs" Inherits="Mediachase.Cms.Website.Structure.User.NWTDControls.Controls.CSS.PublisherRepresentatives" %>
<h2>Local Representatives for <asp:Literal runat="server" ID="litPublisherName" /></h2>
<dl class="nwtd-publisherReps">
	<asp:Repeater runat="server" ID="rpRepresentatives" 
		onitemdatabound="rpRepresentatives_ItemDataBound">
		<ItemTemplate>
			<dt><h3><%#Eval("territory") %></h3></dt>
			<dd>
				<h4><%#Eval("firstname") %> <%#Eval("lastname") %> </h4>
<%--				<address>
					<%#Eval("firstname") %> <%#Eval("lastname") %> 
					<%#((NWTD.InfoManager.RepAddress)Eval("address")).line1 %> <%#((NWTD.InfoManager.RepAddress)Eval("address")).line2 %><br />
					<%#((NWTD.InfoManager.RepAddress)Eval("address")).city %><%# (string.IsNullOrEmpty(((NWTD.InfoManager.RepAddress)Eval("address")).state) ? string.Empty : ", ") %>
					<%#((NWTD.InfoManager.RepAddress)Eval("address")).state %> <%#((NWTD.InfoManager.RepAddress)Eval("address")).zipCode %><br />
				</address>--%>
				
				<asp:Repeater runat="server" ID="rpRepPhones">
					<HeaderTemplate>
						<ul>
					</HeaderTemplate>
					<ItemTemplate>
							<li><%#Eval("name") %>: (<%#Eval("areaCode") %>) <%#Eval("prefix") %>-<%#Eval("postfix") %><%# string.IsNullOrEmpty( Eval("extension").ToString() )? string.Empty : string.Format(" ex {0}",Eval("extension")) %></li>
					</ItemTemplate>
					<FooterTemplate>
						</ul>
					</FooterTemplate>
				</asp:Repeater>
				
				<asp:Repeater runat="server" ID="rpRepEmails">
					<HeaderTemplate>
						<ul>
					</HeaderTemplate>
					<ItemTemplate>
<%--							<%# ((bool)Eval("typespecified")) ? string.Format("{0} Email: ", Eval("type").ToString()) : string.Empty %>
--%>							
						Email: <asp:HyperLink runat="server" NavigateUrl='<%# string.Format("mailto:{0}", Eval("address")) %>'><%#Eval("address") %></asp:HyperLink>
					</ItemTemplate>
					<FooterTemplate>
						</ul>
					</FooterTemplate>
				</asp:Repeater>
			</dd>
		</ItemTemplate>
</asp:Repeater>
</dl>