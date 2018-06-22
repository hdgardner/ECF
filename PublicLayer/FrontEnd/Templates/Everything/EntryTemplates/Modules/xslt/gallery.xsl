<?xml version="1.0" encoding="utf-8"?>

<xsl:stylesheet version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="xml" />

	<xsl:template match="/">
		<images>
				<xsl:apply-templates select="/ArrayOfImageNode/ImageNode" />
		</images>
	</xsl:template>

	<xsl:template match="ImageNode">
		<pic>
			<image>
				<xsl:value-of select="ImageUrl" />
			</image>
			<thumbnail>
				<xsl:value-of select="ImageUrl" />
			</thumbnail>
			<caption>
				<xsl:value-of select="Description" />
			</caption>
		</pic>
	</xsl:template>

</xsl:stylesheet>