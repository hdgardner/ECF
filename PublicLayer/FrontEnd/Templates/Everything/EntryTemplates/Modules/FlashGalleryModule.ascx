<%@ Control Language="C#" AutoEventWireup="true" Inherits="Templates_Everything_Entry_Modules_FlashGalleryModule" Codebehind="FlashGalleryModule.ascx.cs" %>
<object classid="clsid:d27cdb6e-ae6d-11cf-96b8-444553540000" codebase="http://download.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=9,0,0,0" width="100%" height="100%" id="gallery" align="middle">
	<param name="allowScriptAccess" value="sameDomain" />
	<param name="allowFullScreen" value="false" />
	<param name="FlashVars" value="xmlSource=<%#this.XmlSourceUrl %>">
	<param name="movie" value="<%#this.FlashUrl %>" />
	<param name="quality" value="high" />
	<param name="bgcolor" value="#FFFFFF" />	
	<embed src="<%#this.FlashUrl %>" quality="high" bgcolor="#000000" width="100%" height="100%" name="gallery" 
	align="middle" allowScriptAccess="sameDomain" allowFullScreen="false" 
	type="application/x-shockwave-flash" pluginspage="http://www.macromedia.com/go/getflashplayer" />
</object>
