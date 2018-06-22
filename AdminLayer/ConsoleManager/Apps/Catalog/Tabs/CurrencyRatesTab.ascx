<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CurrencyRatesTab.ascx.cs" Inherits="Mediachase.Commerce.Manager.Catalog.Tabs.CurrencyRatesTab" %>
<%@ Register Src="~/Apps/Core/Controls/DialogControl.ascx" TagName="DialogControl" TagPrefix="uc1" %>
<script type="text/javascript">
    function CurrencyRateDialog_CloseDialog()
    {
        CurrencyRateDialog.close();
        CurrencyRatesGrid.callback();
        CSManagementClient.MarkDirty();
    }
    
    function CurrencyRateDialog_OpenDialog()
    {
        CurrencyRateDialog.show(null, 'Edit Currency Rate Information');
    }
    
    function CurrencyRate_Edit(id)
    {
        ecf_UpdateCurrencyRateDialogControl(id);
            
        // show popup for editing/creating CurrencyRate
        CurrencyRateDialog_OpenDialog();
    }
</script>
<div id="DataForm">
 <table width="100%" class="DataForm"> 
     <tr>
        <td>
            <a href="#" onclick="CurrencyRate_Edit(0)"><asp:Literal ID="Literal1" runat="server" Text="<%$ Resources:CatalogStrings, Currency_Add_Rate %>" /></a>
        </td>
     </tr>
     <tr>
        <td>&nbsp;</td>
     </tr>
     <tr>
        <td class="wh100">
            <ComponentArt:Grid Debug="false" AllowEditing="true" RunningMode="Callback" AutoFocusSearchBox="false" 
                ShowHeader="false" ShowFooter="false" Width="700" SkinID="Inline" runat="server" ID="CurrencyRatesGrid" 
                AutoPostBackOnInsert="false" AutoPostBackOnDelete="false" AutoPostBackOnUpdate="false" 
                AutoCallBackOnInsert="false" AutoCallBackOnDelete="false" AutoCallBackOnUpdate="false">
                <ClientTemplates>
                      <ComponentArt:ClientTemplate Id="EditTemplate">
                        <a href="javascript:CurrencyRate_Edit(CurrencyRatesGrid.getItemFromClientId('## DataItem.ClientId ##').getMember('CurrencyRateId').get_text());"><img alt="edit" src="../../../App_Themes/Default/Images/edit.gif" /></a> | <a href="javascript:CurrencyRatesGrid.deleteItem(CurrencyRatesGrid.getItemFromClientId('## DataItem.ClientId ##'));"><img alt="delete" src="../../../App_Themes/Main/Images/toolbar/delete.gif" /></a>
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
<uc1:DialogControl id="CurrencyRateDialog" AppId="Catalog" Width="700" ViewId="View-CurrencyRateEdit" runat="server" />