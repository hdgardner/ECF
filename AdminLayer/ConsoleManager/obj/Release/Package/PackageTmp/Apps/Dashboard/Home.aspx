<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="Mediachase.Commerce.Manager.Dashboard.Home" Theme="Main" %>
<%@ Register Assembly="Mediachase.ConsoleManager" Namespace="Mediachase.Ibn.Web.UI.Layout.Extender" TagPrefix="ibn" %>
<%@ Register Assembly="Mediachase.ConsoleManager" Namespace="Mediachase.Ibn.Web.UI.Layout" TagPrefix="ibn" %>
<%--<%@ Register Src="~/Apps/Core/Layout/PropertyPageContainer.ascx" TagPrefix="ibn" TagName="PropertyPage" %>--%>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title><asp:Literal runat="server" Text="<%$ Resources:SharedStrings, Mediachase_Commerce_Manager_5_0 %>"/></title>
    <link href="../../App_Themes/Default/css/ext-all.css" rel="stylesheet" type="text/css" />
    <link href="../Core/Layout/Styles/ext-all2-workspace.css" rel="stylesheet" type="text/css" />
    <link href="../Core/Layout/Styles/workspace.css" rel="stylesheet" type="text/css" />
    
    <script type="text/javascript">
        // this page should be inside frame
        if(top == self) top.location.href = 'default.aspx';
        
        function pageLoad()
	    {
	        var obj = document.getElementById('ibnMain_divWithLoadingRss');
		    if (obj)
		    {
			    obj.style.display = 'none';
			}

			if (Sys.Browser.agent == Sys.Browser.Firefox) {
			    var obj1 = document.getElementById('cpManager');
			    if (obj1) {
			        obj1.style.display = 'inline';
			    }
			}

			if (window.childPageLoad) {
			    window.childPageLoad(sender, args);
			}
		}
    </script>

</head>
<body scroll="auto" class="view"; style="background-color:white;">
    <form id="form1" runat="server">
        <div id='ibnMain_divWithLoadingRss' style="position: absolute; left: 0px; top: 0px; height: 100%;width: 100%; z-index: 10000">
			<div style="left: 40%; top: 40%; height: 30px; width: 200px; position: absolute;
				z-index: 100001">
				<div style="position: relative; z-index: 100002">
					<img alt="" style="position: absolute; left: 30%; top: 40%; z-index: 100003; border:0" src='<%= ResolveClientUrl("~/App_Themes/Default/Images/Shell/loading_rss.gif") %>' />
				</div>
			</div>
		</div>
   		<asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true" EnablePartialRendering="true" EnableScriptGlobalization="true" LoadScriptsBeforeUI="true" ScriptMode="debug">
   		    <Services>
     		    <asp:ServiceReference Path="~/Apps/Core/Layout/WebServices/LayoutCustomizationService.asmx" InlineScript="true" />
     		</Services>
   		</asp:ScriptManager>
       
        <ibn:IbnControlPlaceManager runat="server" ID="cpManager" />
        <ibn:WsLayoutExtender runat="server" TargetControlID="cpManager" ID="cpManagerExtender"/>
        <asp:UpdatePanel runat="server" ID="cmPanel1" UpdateMode="Conditional">
            <ContentTemplate>
                <div style="height: 0px;width:0px;">
                    <IbnWebControls:CommandManager ID="cmContent1" runat="server" ContainerId="containerDiv" />
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
        <div id="containerDiv" runat="server" style="height: 0px">
        </div>
    </form>
</body>
</html>