<%@ Control Language="C#" AutoEventWireup="true" Inherits="Templates_Everything_Entry_Modules_FlashImageModule" Codebehind="FlashImageModule.ascx.cs" %>
<object classid="clsid:d27cdb6e-ae6d-11cf-96b8-444553540000" codebase="http://download.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=9,0,0,0" width="400" height="225" id="cycler" align="middle">
	<param name="allowScriptAccess" value="always" />
	<param name="allowFullScreen" value="false" />
	<param name="FlashVars" value="xmlSource=<%#this.XmlSourceUrl %>">
	<param name="movie" value="<%#this.FlashUrl %>" />
	<param name="quality" value="high" />
	<param name="bgcolor" value="#FFFFFF" />	
	<embed src="<%#this.FlashUrl %>" quality="high" bgcolor="#000000" width="400" height="225" name="cycler" 
	align="middle" allowScriptAccess="sameDomain" allowFullScreen="false" 
	type="application/x-shockwave-flash" pluginspage="http://www.macromedia.com/go/getflashplayer" />
</object>
