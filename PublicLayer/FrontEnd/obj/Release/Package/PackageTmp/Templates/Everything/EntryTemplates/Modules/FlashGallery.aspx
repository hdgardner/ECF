<%@ Page Language="C#" AutoEventWireup="true" Inherits="Templates_Everything_Entry_Modules_FlashGallery" Codebehind="FlashGallery.aspx.cs" %>

<%@ Register Src="FlashGalleryModule.ascx" TagName="FlashGalleryModule" TagPrefix="uc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <uc1:FlashGalleryModule ID="FlashGalleryModule1" runat="server" />
    
    </div>
    </form>
</body>
</html>
