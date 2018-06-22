<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TaxValueEditTab.ascx.cs" Inherits="Mediachase.Commerce.Manager.Order.Tabs.TaxValueEditTab" %>
<%@ Register Src="~/Apps/Core/Controls/DialogControl.ascx" TagName="DialogControl" TagPrefix="uc1" %>
<script type="text/javascript">
    function TaxValueDialog_CloseDialog()
    {
        TaxValueDialog.close();
        TaxValuesGrid.callback();
        CSManagementClient.MarkDirty();
    }
    
    function TaxValueDialog_OpenDialog()
    {
        TaxValueDialog.show(null, 'Edit Tax Value Information');
    }
    
    function TaxValue_Edit(id)
    {
        ecf_UpdateTaxValueDialogControl(id);
            
        // show popup for editing/creating TaxValue
        TaxValueDialog_OpenDialog();
    }
</script>
<div id="DataForm">
 <table width="100%" class="DataForm"> 
     <tr>
        <td>
            <a href="#" onclick="TaxValue_Edit(0)"><asp:Literal ID="Literal1" runat="server" Text="<%$ Resources:OrderStrings, Tax_Add_Value %>" /></a>
        </td>
     </tr>
     <tr>
        <td>&nbsp;</td>
     </tr>
     <tr>
        <td class="wh100">
            <ComponentArt:Grid Debug="false" AllowEditing="true" RunningMode="Callback" AutoFocusSearchBox="false" 
                ShowHeader="false" ShowFooter="false" Width="700" SkinID="Inline" runat="server" ID="TaxValuesGrid" 
                AutoPostBackOnInsert="false" AutoPostBackOnDelete="false" AutoPostBackOnUpdate="false" 
                AutoCallBackOnInsert="false" AutoCallBackOnDelete="false" AutoCallBackOnUpdate="false">
                <ClientTemplates>
                      <ComponentArt:ClientTemplate Id="EditTemplate">
                        <a href="javascript:TaxValue_Edit(TaxValuesGrid.getItemFromClientId('## DataItem.ClientId ##').getMember('TaxValueId').get_text());"><img alt="edit" src="../../../App_Themes/Default/Images/edit.gif" /></a> | <a href="javascript:TaxValuesGrid.deleteItem(TaxValuesGrid.getItemFromClientId('## DataItem.ClientId ##'));"><img alt="delete" src="../../../App_Themes/Main/Images/toolbar/delete.gif" /></a>
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
<uc1:DialogControl id="TaxValueDialog" AppId="Order" Width="400" ViewId="View-TaxValueEdit" runat="server" />