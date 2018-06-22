<?xml version="1.0" encoding="utf-8"?>

<xsl:stylesheet version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="xml" />

	<xsl:template match="/">
		<cycler>
			<slides>
				<xsl:apply-templates select="/ArrayOfImageNode/ImageNode" />
			</slides>
		</cycler>
	</xsl:template>

	<xsl:template match="ImageNode">
		<item>
			<xsl:attribute name="name">
				<xsl:value-of select="Title" />
			</xsl:attribute>
			<xsl:attribute name="caption">
				<xsl:value-of select="Description" />
			</xsl:attribute>
			<xsl:attribute name="img">
				<xsl:value-of select="ImageUrl" />
			</xsl:attribute>
			<xsl:attribute name="url">
				<xsl:value-of select="Url" />
			</xsl:attribute>			
		</item>
	</xsl:template>

</xsl:stylesheet>