<%@ Control Language="C#" AutoEventWireup="true" Inherits="Apps_MetaDataBase_FrameControl" Codebehind="FrameControl.ascx.cs" %>
<iframe runat="server" id="FrameCtrl" scrolling="auto" frameborder="0" width="100%" marginheight="0" marginwidth="0"></iframe>
<script type="text/javascript">
    if (Sys.Browser.agent == Sys.Browser.Firefox) {
        var fr = $get('<%= FrameCtrl.ClientID %>');
        if (fr != null) {
            fr.height = document.body.clientHeight;
        }
    }
    else if (Sys.Browser.agent == Sys.Browser.InternetExplorer) {
        var fr = $get('<%= FrameCtrl.ClientID %>');
        if (fr != null) {
            fr.height = document.body.clientHeight - 2;
        }
    }
</script>