<%@ Control Language="C#" EnableViewState="true" AutoEventWireup="true" Inherits="Mediachase.Commerce.Manager.Order.Tabs.OrderPaymentsEditTab" Codebehind="OrderPaymentsEditTab.ascx.cs" %>
<%@ Register Src="../../Core/Controls/DialogControl.ascx" TagName="DialogControl"
    TagPrefix="uc1" %>
<script type="text/javascript">  
    function Payment_CloseDialog()
    {
        PaymentViewDialog.close();
        PaymentOptionList.callback();
        CSManagementClient.MarkDirty();
    }
    
    function Payment_OpenDialog()
    {
        PaymentViewDialog.show(null, 'Edit Payment Information');
    }
    
    function Payment_Edit(id)
    {
        ecf_UpdatePaymentDialogControl(id);
        Payment_OpenDialog();
    }
</script>
<div id="DataForm">
 <table width="100%" class="DataForm"> 
     <tr>
        <td>
            <table>
                <tr>
                    <td>
                        <a href="#" onclick="Payment_Edit(0)"><asp:Literal ID="Literal4" runat="server" Text="<%$ Resources:OrderStrings, New_Payment %>"/></a>
                    </td>
                </tr>
            </table>
        </td>
     </tr>
     <tr>
        <td>&nbsp;</td>
     </tr>
     <tr>
        <td class="wh100">
            <ComponentArt:Grid Debug="false" AllowEditing="false" RunningMode="Client" AutoFocusSearchBox="false" ShowHeader="false" ShowFooter="false" SkinID="Inline" runat="server" ID="PaymentOptionList" AutoPostBackOnInsert="false" AutoPostBackOnDelete="false" AutoCallBackOnUpdate="false">
                <Levels>
                    <ComponentArt:GridLevel DataKeyField="PaymentId"></ComponentArt:GridLevel>
                </Levels>
                <ClientTemplates>
                      <ComponentArt:ClientTemplate Id="EditTemplate">
                        <a href="javascript:Payment_Edit(PaymentOptionList.getItemFromClientId('## DataItem.ClientId ##').getMember('PaymentId').get_text());"><img alt="edit" src="../../../App_Themes/Default/Images/edit.gif" /></a> | <a href="javascript:PaymentOptionList.deleteItem(PaymentOptionList.getItemFromClientId('## DataItem.ClientId ##'));"><img alt="delete" src="../../../App_Themes/Main/Images/toolbar/delete.gif" /></a>
                      </ComponentArt:ClientTemplate>
                    <ComponentArt:ClientTemplate ID="LoadingFeedbackTemplate">
                        <table cellspacing="0" cellpadding="0" border="0">
                            <tr>
                                <td style="font-size: 10px;">
                                    Loading...&nbsp;</td>
                                <td>
                                    <img src="../../../App_Themes/Main/images/grid/spinner.gif" width="16" height="16" border="0">
                                </td>
                            </tr>
                        </table>
                    </ComponentArt:ClientTemplate>
                </ClientTemplates>
            </ComponentArt:Grid>
        </td>
     </tr>
 </table>
</div>
<uc1:DialogControl id="PaymentViewDialog" AppId="Order" Width="700" ViewId="View-Payment" runat="server">
</uc1:DialogControl>