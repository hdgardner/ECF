<?xml version="1.0" encoding="utf-8"?>

<xsl:stylesheet version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="html" />
	<xsl:include href="mssd-order-shared.xsl"/>

	<xsl:template match="/">
		<html>
			<head id="Head1">
				<style type="text/css">
					#PurchaseOrder {}
					h1 {font-size: 20px;}
					h2 {font-size: 18px;}
					h3 {font-size: 16px; background-color: #cccccc; padding: 2px 2px 2px 2px}
					.introduction {padding: 5px 0 0 0}
				</style>
				<title>
					MSSD Order Notification
				</title>
			</head>
			<body>
				<xsl:apply-templates select="/OrderGroup/Total"></xsl:apply-templates>
				<xsl:apply-templates select="//CartMetaDataInfo"></xsl:apply-templates>
			</body>
		</html>
	</xsl:template>

	<xsl:template match="CartMetaDataInfo">
		<div id="PurchaseOrder">
			
			<!--<xsl:value-of select="CustomerName"/>,<br /><br />-->
			
			<h1>THANK YOU FOR YOUR ORDER</h1>
			
			<xsl:call-template name="OrderHeader"></xsl:call-template>
<!--
			<div class="OrderForms">
				<h2>Products Purchased:</h2>
				<xsl:apply-templates select="OrderForms/OrderForm"></xsl:apply-templates>
			</div>

			
-->
			<p>You can get status information regarding your order by logging into our website and selecting “Order Status” from the Quick Links section.  Order status is typically available 24-48 hours after you place your order.  If you have any questions regarding this order or need to make changes, please contact us.</p>
			<div class="Footer">
				<xsl:call-template name="OrderFooter"></xsl:call-template>
			</div>
		</div>
	</xsl:template>
</xsl:stylesheet>


