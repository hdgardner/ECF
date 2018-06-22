<%@ Page Language="C#" AutoEventWireup="true" Inherits="ContentFrame" EnableEventValidation="false" Theme="Main" CodeBehind="ContentFrame.aspx.cs" %>
<%@ Register Src="~/Apps/Core/ErrorModule.ascx" TagName="ErrorModule" TagPrefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title><asp:Literal ID="Literal1" runat="server" Text="<%$ Resources:SharedStrings, Mediachase_Commerce_Manager_5_0 %>"/></title>
    
    <script type="text/javascript">
        // this page should be inside frame
        if (top == self) top.location.href = 'default.aspx';

        function pageLoad(sender, args) {
            var obj = document.getElementById('ibnMain_divWithLoadingRss');
            if (obj) {
                obj.style.display = 'none';
            }

            if (Sys.Browser.agent == Sys.Browser.Firefox) {
                initFixTables();
            }
            
            if (window.childPageLoad) {
                window.childPageLoad(sender, args);
            }
        }

        function initFixTables() {
            setTimeout(fixTables, 200);
            return true;
        }

        function fixTables() {
            var colTables = document.getElementsByTagName('TABLE');
            for (var i = 0; i < colTables.length; i++)
                if (colTables[i].style.display == 'inline-block')
                colTables[i].style.display = '';
        }
    </script>

</head>
<body scroll="auto" class="view">
    <form id="form1" runat="server" style="height: 100%;">
    <div id='ibnMain_divWithLoadingRss' style="position: absolute; left: 0px; top: 0px;
        height: 100%; width: 100%; z-index: 10000">
        <div style="left: 40%; top: 40%; height: 30px; width: 200px; position: absolute;
            z-index: 100001">
            <div style="position: relative; z-index: 100002">
                <img alt="" style="position: absolute; left: 30%; top: 40%; z-index: 100003; border: 0"
                    src='<%= ResolveClientUrl("~/App_Themes/Default/Images/Shell/loading_rss.gif") %>' />
            </div>
        </div>
    </div>
    <asp:ScriptManager runat="server" ID="ScriptManager1" EnablePageMethods="true" EnablePartialRendering="true" 
        EnableScriptGlobalization="true" EnableScriptLocalization="true" LoadScriptsBeforeUI="true" ScriptMode="Auto">
        <Services>
 		    <asp:ServiceReference Path="~/Apps/Core/Controls/WebServices/EcfListViewExtenderService.asmx" InlineScript="true" />
 		</Services>
    </asp:ScriptManager>       

    <uc1:ErrorModule ID="ErrorModule1" runat="server"></uc1:ErrorModule>
    <asp:HiddenField runat="server" ID="_action" Value="" />
    <asp:HiddenField runat="server" ID="_params" Value="" />
    <IbnWebControls:LayoutExtender ID="LayoutExtender1" runat="server" TargetControlID="IbnMainLayout">
    </IbnWebControls:LayoutExtender>
    <IbnWebControls:McLayout runat="server" ID="IbnMainLayout">
        <Items>
            <asp:PlaceHolder EnableViewState="true" ID="ContentHolderControl" runat="server">
            </asp:PlaceHolder>
        </Items>
    </IbnWebControls:McLayout>
    <asp:UpdateProgress ID="UpdateProgress2" runat="server" AssociatedUpdatePanelID="cmPanel"
        EnableViewState="true" DynamicLayout="true">
        <ProgressTemplate> 
        <div style="left: 40%; top: 40%; height: 30px; width: 200px; position: absolute;
            z-index: 100001">
            <div style="position: relative; z-index: 100002; ">
                <img alt="" style="position: absolute; left: 30%; top: 40%; z-index: 100003; border: 0"
                    src='<%= ResolveClientUrl("~/App_Themes/Default/Images/Shell/loading_rss.gif") %>' />
            </div>
        </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <asp:UpdatePanel runat="server" ID="cmPanel" UpdateMode="Conditional">
        <ContentTemplate>
            <div style="height: 0px;">
                <IbnWebControls:CommandManager ID="cmContent" runat="server" ContainerId="containerDiv" />
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    <div id="containerDiv" runat="server" style="height: 0px">
    </div>
    </form>
    
        <!--[if IE 8]>
        <script type="text/javascript">
        // get rid of redundant vertical scrolling in IE8
        if ((Sys.Browser.agent == Sys.Browser.InternetExplorer) && (Sys.Browser.version == 8)) {
            var divLayout = $get('IbnMainLayout_divLayoutBase');
            if (divLayout != null) {
                divLayout.style.height = document.body.clientHeight - 1; /* 1 here is padding-top for .LayoutBase */
            }
        }
        </script>
        <![endif]-->
       
</body>
</html>
