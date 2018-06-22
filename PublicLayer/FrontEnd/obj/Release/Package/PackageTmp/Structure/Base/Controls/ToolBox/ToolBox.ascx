<%@ Control Language="C#" AutoEventWireup="true" Inherits="ToolBox" Codebehind="ToolBox.ascx.cs" %>

<div id="showimageT" class="showimage" style="position: absolute; z-index: 254; display: none;">
    <input type="text" runat="server" id="tboxLocation" value="150,350" style="display: none;" />
    <input type="hidden" value="" id="isModified" runat="server" />
    <table class="mainToolbox" cellspacing="0" cellpadding="0" height="250px">
        <tr>
            <td>
                <div id="dragbarT" class="dragbar" style="vertical-align: middle;">
                    <table style="width: 100%; height: 100%; text-align: left; padding-left: 4px;" cellpadding="0"
                        cellspacing="0">
                        <tr>
                            <td style="width: 90%">
                                Control Toolbox
                            </td>
                            <td runat="server" id="tdToolboxHeader" style="width: 10%" class="btnMinimize">
                            </td>
                        </tr>
                    </table>
                </div>
            </td>
        </tr>
        <tr>
            <td class="firstCell">
                <table class="innerTable" border="0" cellspacing="0" cellpadding="0">
                    <tr id="contentT" class="content">
                        <td>
                            <!-- PUT YOUR CONTENT BETWEEN HERE -->
                            <asp:PlaceHolder runat="server" ID="ContentPlaceholder"></asp:PlaceHolder>
                            <asp:PlaceHolder runat="server" ID="DockingTemp"></asp:PlaceHolder>
                            <!-- END YOUR CONTENT HERE -->
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</div>

<script type="text/javascript">

	var theHandle = document.getElementById('dragbarT');
	var theRoot   = document.getElementById('showimageT');
	var theLocation = document.getElementById('<%= tboxLocation.ClientID %>');
	Drag.init(theHandle, theRoot, theLocation);

</script>