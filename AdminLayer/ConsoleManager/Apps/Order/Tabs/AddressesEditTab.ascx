<%@ Control Language="C#" EnableViewState="true" AutoEventWireup="true" Inherits="Mediachase.Commerce.Manager.Order.Tabs.AddressesEditTab" Codebehind="AddressesEditTab.ascx.cs" %>
<%@ Register Src="~/Apps/Core/Controls/DialogControl.ascx" TagName="DialogControl" TagPrefix="uc1" %>
<%@ Register Src="~/Apps/Core/MetaData/EditTab.ascx" TagName="MetaData" TagPrefix="ecf" %>
<script type="text/javascript">
    function Address_CloseDialog() {
        AddressViewDialog.close();
        AddressList.callback();
        CSManagementClient.MarkDirty();
    }

    function Address_OpenDialog() {
        AddressViewDialog.show(null, 'Edit Address Information');
    }

    function Address_Edit(id) {
        ecf_UpdateAddressDialogControl(id);
        Address_OpenDialog();
    }

    function Address_Delete(id) {
        AddressList.deleteItem(id);
        AddressList.callback();
    }

    function AddressList_onCallbackComplete() {
        ecf_UpdateOrderAddressDropDown();
    }
</script>
<div id="DataForm">
 <table width="100%" class="DataForm"> 
 <tr>
    <td>
        <table>
            <tr>
                <td>
                    <a href="#" onclick="Address_Edit(0)"><asp:Literal ID="Literal4" runat="server" Text="<%$ Resources:SharedStrings, New_Address %>"/></a>
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
                        
<ComponentArt:Grid Debug="false" AllowEditing="false" RunningMode="Client" AutoFocusSearchBox="false" ShowHeader="false" ShowFooter="false" SkinID="Inline" runat="server" ID="AddressList" AutoPostBackOnInsert="false" AutoPostBackOnDelete="false" AutoCallBackOnUpdate="false">
    <Levels>
        <ComponentArt:GridLevel DataKeyField="OrderGroupAddressId"></ComponentArt:GridLevel>
    </Levels>
    <ClientTemplates>
          <ComponentArt:ClientTemplate Id="EditTemplate">
            <a href="javascript:Address_Edit(AddressList.getItemFromClientId('## DataItem.ClientId ##').getMember('OrderGroupAddressId').get_text());"><img alt="edit" src="../../../App_Themes/Default/Images/edit.gif" /></a> | <a href="javascript:Address_Delete(AddressList.getItemFromClientId('## DataItem.ClientId ##'));"><img alt="delete" src="../../../App_Themes/Main/Images/toolbar/delete.gif" /></a>
          </ComponentArt:ClientTemplate>
        <ComponentArt:ClientTemplate ID="LoadingFeedbackTemplate">
            <table cellspacing="0" cellpadding="0" border="0">
                <tr>
                    <td style="font-size: 10px;">
                        Loading...&nbsp;</td>
                    <td>
                        <img src="../../../App_Themes/Main/images/grid/spinner.gif" width="16" height="16" border="0"></td>
                </tr>
            </table>
        </ComponentArt:ClientTemplate>          
    </ClientTemplates>
    <ClientEvents>
        <CallbackComplete EventHandler="AddressList_onCallbackComplete" />
    </ClientEvents>
</ComponentArt:Grid>

    </td>
 </tr>
 </table>
</div>
<uc1:DialogControl id="AddressViewDialog" AppId="Order" Width="700" ViewId="View-Address" runat="server">
</uc1:DialogControl>