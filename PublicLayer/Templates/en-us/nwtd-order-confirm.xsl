<?xml version="1.0" encoding="utf-8"?>
<?altova_samplexml file:///C:/Users/James/Desktop/Sample.xml?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="html"/>
	<xsl:include href="nwtd-order-shared.xsl"/>
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
					NWTD Order Notification
				</title>
			</head>
			<body>

			<xsl:apply-templates select="//CartMetaDataInfo"></xsl:apply-templates>
				

			</body>
		</html>
	</xsl:template>
	<xsl:template match="CartMetaDataInfo">
		<div id="PurchaseOrder">
			<h1>Thank You For Your Order</h1>
                                <xsl:call-template name="OrderHeader"/>

<p>A copy of your order is attached to this email.
You will be able to get status information regarding your order by logging into our website and selecting “Order Status” from the Quick Links section. Order status is typically available 24-48 hours after you place your order. If you have any questions regarding this order or need to make changes, please contact us by phone or email.</p>
Customer Service<br />
Northwest Textbook Depository<br/>
503-906-1100 <br />
800-676-6630 <br />
<a href="mailto:customer.service@nwtd.com">customer.service@nwtd.com</a>
		</div>
	</xsl:template>
</xsl:stylesheet>
