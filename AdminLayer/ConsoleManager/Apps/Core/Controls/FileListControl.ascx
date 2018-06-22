<%@ Control Language="c#" Inherits="Mediachase.Commerce.Manager.Core.Controls.FileListControl" Codebehind="FileListControl.ascx.cs" %>
<script type="text/javascript">
    function FilesListDefaultGrid_onCallbackError(sender, eventArgs)
    {
      alert('Error during callback. Details: ' + eventArgs.get_errorMessage());
    }

</script>
<table cellpadding="0">
    <tr class="h100">
        <td id="tdLV" style="vertical-align: top">
            <asp:HiddenField ID="hfSelectedItems" runat="server" />
            <ComponentArt:Grid SkinID="Default" runat="server" ID="DefaultGrid" Width="800" 
                AutoPostBackOnInsert="false" AutoPostBackOnDelete="false" AutoCallBackOnUpdate="true" RunningMode="Callback">
                <ClientTemplates>
                    <ComponentArt:ClientTemplate ID="LoadingFeedbackTemplate">
                        <table cellspacing="0" cellpadding="0" border="0">
                            <tr>
                                <td style="font-size: 10px;">
                                    <asp:Literal ID="Literal1" runat="server" Text="<%$ Resources:SharedStrings, Loading %>"/>...&nbsp;</td>
                                <td>
                                <img src="../../../App_Themes/Main/images/grid/spinner.gif" width="16" height="16" border="0"></td>
                            </tr>
                        </table>
                    </ComponentArt:ClientTemplate>
                </ClientTemplates>
                <ClientEvents>
                    <CallbackError EventHandler="FilesListDefaultGrid_onCallbackError" />
                    <ItemSelect EventHandler="FilesListDefaultGrid_onItemSelect" />
                </ClientEvents>
            </ComponentArt:Grid>
        </td>
    </tr>
</table>