<?xml version="1.0" encoding="utf-8"?>

<xsl:stylesheet version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:ms="urn:schemas-microsoft-com:xslt">
	<xsl:output method="html" />

	<xsl:template name="PaymentPlanSchedule">
		<div class="schedule">
			Payment Schedule: starting&#160;<xsl:value-of select="ms:format-date(StartDate, 'MMM dd, yyyy')"/>&#160;every&#160;<xsl:value-of select="CycleLength"/>&#160;<xsl:call-template name="PlanCycle" />&#160;for&#160;<xsl:value-of select="MaxCyclesCount"/>&#160;<xsl:call-template name="PlanCycle" />&#160;till&#160;<xsl:value-of select="ms:format-date(EndDate, 'MMM dd, yyyy')"/>
		</div>
	</xsl:template>

	<xsl:template name="PlanCycle">
		<xsl:value-of select="CycleMode"/>(s)
	</xsl:template>
	

	<xsl:template name="OrderHeader">
		Your order confirmation number is : <xsl:value-of select="WebConfirmation"/><br/>
	</xsl:template>

	<xsl:template name="OrderFooter">
		Phone:<br/>	
		801-773-3200 <br />
		800-995-1444 <br /><br />
		
		Email:	<a href="mailto:customer.service@nwtd.com">customer.service@mssd.com</a>
			
		<p>We appreciate your business and look forward to serving you again.</p>

	</xsl:template>

	<xsl:template match="OrderForm">
		<div class="OrderForm">
			<div class="OrderForms">
				<h3>Line Items</h3>
				<xsl:apply-templates select="LineItems/LineItem"></xsl:apply-templates>
				<h3>Payments</h3>
				<xsl:apply-templates select="Payments/Payment"></xsl:apply-templates>
			</div>
			<div class="OrderSummary">
				<!--
				Sub Total: <xsl:value-of select="SubTotal"/><br/>
				Handling Total: <xsl:value-of select="HandlingTotal"/><br/>
				Shipping Total: <xsl:value-of select="ShippingTotal"/><br/>
				Total Tax: <xsl:value-of select="TaxTotal"/><br/>
				Discount: <xsl:value-of select="DiscoutnAmount"/><br/>
				TOTAL: <xsl:value-of select="Total"/><br/>
				-->
			</div>
		</div>
	</xsl:template>

	<xsl:template match="LineItem">
		<div class="LineItem">
			<xsl:value-of select="format-number(Quantity, '###,###.##')"/>&#160;<xsl:value-of select="DisplayName"/> - <xsl:value-of select="//BillingCurrency"/>&#160;<xsl:value-of select="format-number(ListPrice, '###,###.00')"/> each
		</div>
	</xsl:template>

	<xsl:template match="Payment">
		<div class="Payment">
			Payment Method: <xsl:value-of select="PaymentMethodName"/><br/>
			Amount: <xsl:value-of select="//BillingCurrency"/>&#160;<xsl:value-of select="format-number(Amount, '###,###.00')"/>
		</div>
	</xsl:template>


</xsl:stylesheet>


