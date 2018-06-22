<%@ Control Language="C#" Inherits="Mediachase.eCF.PublicStore.Plugins.PaymentGateways.Authorize.PaymentMethod" Codebehind="PaymentMethod.ascx.cs" %>
<asp:Image Runat="server" id="Image1" AlternateText="Visa" ImageUrl="images/visa.gif"></asp:Image>
<asp:Image Runat="server" id="Image2" AlternateText="Discover" ImageUrl="images/discover.gif"></asp:Image>
<asp:Image Runat="server" id="Image3" AlternateText="Mastercard" ImageUrl="images/mastercard.gif"></asp:Image>
<asp:Image Runat="server" id="Image4" AlternateText="American Express" ImageUrl="images/amex.gif"></asp:Image>
<br/>
<table id="TableCreditCard" cellspacing="2" cellpadding="1" border="0">
	<tr>
		<td width="250"><%=RM.GetString("CHECKOUT_PAYMENT_CARDHOLDER_NAME")%>:<br/>
			<%=RM.GetString("CHECKOUT_PAYMENT_CARDHOLDER_LABEL")%></td>
		<td>
			<asp:TextBox id="creditCardName" runat="server" Width="220px"></asp:TextBox>
			<asp:RequiredFieldValidator ValidationGroup="Authorize" Runat="server" ID="CCNameValidator" ControlToValidate="creditCardName" ErrorMessage="*"
				EnableClientScript="False" Display="Dynamic"></asp:RequiredFieldValidator>				
		</td>
	</tr>
	<tr>
		<td width="250"><%=RM.GetString("CHECKOUT_PAYMENT_CARD_NUMBER") %>:</td>
		<td>
			<asp:TextBox id="creditCardNumber" runat="server" Width="165px" MaxLength="16"></asp:TextBox>
			<asp:RequiredFieldValidator ValidationGroup="Authorize" Runat="server" ID="CCRequiredValidator" ControlToValidate="creditCardNumber" ErrorMessage="*"
					EnableClientScript="False" Display="Dynamic"></asp:RequiredFieldValidator>
			<cms:CreditCardValidator ValidationGroup="Authorize" ControlToValidate="creditCardNumber" Runat="server" ID="CCValidator"
				ErrorMessage="Invalid Credit Card Number" EnableClientScript="False" Display="Dynamic"></cms:CreditCardValidator>
		</td>
	</tr>
	<tr>
		<td width="250"><%=RM.GetString("CHECKOUT_PAYMENT_EXPIRATION_DATE_LABEL")%>:</td>
		<td>
			<asp:DropDownList id="creditCardExpireMonth" runat="server">
				<asp:ListItem Value="1">1</asp:ListItem>
				<asp:ListItem Value="2">2</asp:ListItem>
				<asp:ListItem Value="3">3</asp:ListItem>
				<asp:ListItem Value="4">4</asp:ListItem>
				<asp:ListItem Value="5">5</asp:ListItem>
				<asp:ListItem Value="6">6</asp:ListItem>
				<asp:ListItem Value="7">7</asp:ListItem>
				<asp:ListItem Value="8">8</asp:ListItem>
				<asp:ListItem Value="9">9</asp:ListItem>
				<asp:ListItem Value="10">10</asp:ListItem>
				<asp:ListItem Value="11">11</asp:ListItem>
				<asp:ListItem Value="12">12</asp:ListItem>
			</asp:DropDownList>/
			<asp:DropDownList id="creditCardExpireYear" runat="server">
				<asp:ListItem Value="2009">2009</asp:ListItem>
				<asp:ListItem Value="2010">2010</asp:ListItem>
				<asp:ListItem Value="2011">2011</asp:ListItem>
				<asp:ListItem Value="2012">2012</asp:ListItem>
				<asp:ListItem Value="2013">2013</asp:ListItem>
				<asp:ListItem Value="2014">2014</asp:ListItem>
				<asp:ListItem Value="2011">2015</asp:ListItem>
				<asp:ListItem Value="2012">2016</asp:ListItem>
				<asp:ListItem Value="2013">2017</asp:ListItem>
				<asp:ListItem Value="2014">2018</asp:ListItem>
				<asp:ListItem Value="2013">2019</asp:ListItem>
				<asp:ListItem Value="2014">2020</asp:ListItem>
			</asp:DropDownList>
		</td>
	</tr>
	<tr>
		<td width="250"><%=RM.GetString("CHECKOUT_PAYMENT_CARD_CODE")%>:</td>
		<td><font face="verdana" size="1">
				<asp:TextBox id="creditCardCSC" runat="server" Width="60px" MaxLength="4"></asp:TextBox>
				<asp:RequiredFieldValidator ValidationGroup="Authorize" Runat="server" ID="CIDValidator" ControlToValidate="creditCardCSC" ErrorMessage="*"
					EnableClientScript="False" Display="Dynamic"></asp:RequiredFieldValidator>				
			</font>
		</td>
	</tr>
</table>
