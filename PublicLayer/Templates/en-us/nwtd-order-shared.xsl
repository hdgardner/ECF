<?xml version="1.0" encoding="utf-8"?>
<?altova_samplexml file:///C:/Users/James/Desktop/Sample.xml?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:ms="urn:schemas-microsoft-com:xslt">
	<xsl:output method="html"/>
	
	<xsl:template name="OrderHeader">
		Your order confirmation number is : <xsl:value-of select="WebConfirmation"/>
		<br/>
	</xsl:template>
	<xsl:template name="OrderFooter">
		Phone:<br/>	
		503-906-1100 <br/>
		800-676-6630 <br/>
		<br/>
		
		Email:	<a href="mailto:customer.service@nwtd.com">customer.service@nwtd.com</a>
		<p>We appreciate your business and look forward to serving you again.</p>
	</xsl:template>
	<xsl:template match="OrderForm">
		<div class="OrderForm">
			<div class="OrderForms">
				<h3>Line Items</h3>
				<xsl:apply-templates select="LineItems/LineItem"/>
				<h3>Payments</h3>
				<xsl:apply-templates select="Payments/Payment"/>
			</div>
			<div class="OrderSummary">
			
				Sub Total: <xsl:value-of select="SubTotal"/>
				<br/>
				Handling Total: <xsl:value-of select="HandlingTotal"/>
				<br/>
				Shipping Total: <xsl:value-of select="ShippingTotal"/>
				<br/>
				Total Tax: <xsl:value-of select="TaxTotal"/>
				<br/>
				Discount: <xsl:value-of select="DiscoutnAmount"/>
				<br/>
				TOTAL: <xsl:value-of select="Total"/>
				<br/>
			</div>
		</div>
	</xsl:template>
	
	<xsl:template match="OrderAddress">
		<div class="OrderAddress">
		       <h3>Order Address</h3>
				<xsl:value-of select="FirstName"/>
				<br />
				<xsl:value-of select="Line2"/>
				<br/>
				<xsl:value-of select="City"/>, <xsl:value-of select="State"/> <xsl:value-of select="PostalCode"/>
				<br/>
			<br />
		</div>
	</xsl:template>
	
	<xsl:template match="LineItem">
		<div class="LineItem">
			<xsl:value-of select="format-number(Quantity, '###,###.##')"/>&#160;<xsl:value-of select="DisplayName"/> - <xsl:value-of select="//BillingCurrency"/>&#160;<xsl:value-of select="format-number(ListPrice, '###,###.00')"/> each
		</div>
	</xsl:template>
	<xsl:template match="Payment">
		<div class="Payment">
			Payment Method: <xsl:value-of select="PaymentMethodName"/>
			<br/>
			Amount: <xsl:value-of select="//BillingCurrency"/>&#160;<xsl:value-of select="format-number(Amount, '###,###.00')"/>
		</div>
	</xsl:template>
</xsl:stylesheet>
