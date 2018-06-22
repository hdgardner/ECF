<%@ Control Language="C#" AutoEventWireup="true" Inherits="Mediachase.Commerce.Manager.Catalog.Tabs.EntryPricingEditTab" Codebehind="EntryPricingEditTab.ascx.cs" %>
<%@ Register Src="~/Apps/Core/Controls/BooleanEditControl.ascx" TagName="BooleanEditControl" TagPrefix="ecf" %>
<%@ Register Src="~/Apps/Core/Controls/CalendarDatePicker.ascx" TagName="CalendarDatePicker" TagPrefix="ecf" %>
<%@ Register Src="~/Apps/Core/Controls/DialogControl.ascx" TagName="DialogControl" TagPrefix="uc1" %>
<script type="text/javascript">
    function SalePriceEdit_CloseDialog()
    {
        SalePriceEditDialog.close();
        SalePricesGrid.callback();
        CSManagementClient.MarkDirty();
    }
    
    function SalePriceEdit_OpenDialog()
    {
        SalePriceEditDialog.show(null, 'Edit Sale Price Information');
    }
    
    function SalePriceItem_Edit(id)
    {
        ecf_UpdateSalePriceEditDialogControl(id);
            
        // show popup for editing/creating SalePrice
        SalePriceEdit_OpenDialog();
    }
</script>
<div id="DataForm">
    <table class="DataForm">
        <tr>
            <td class="FormLabelCell">
                <asp:Label ID="Label1" runat="server" Text="<%$ Resources:CatalogStrings, Entry_Display_Price %>"></asp:Label>:&nbsp;
                (<asp:Label ID="DisplayPriceCurrency" runat="server"></asp:Label>)</td>
            <td class="FormFieldCell">
                <asp:TextBox runat="server" Width="250" ID="ListPrice"></asp:TextBox><br />
                <ajaxToolkit:MaskedEditExtender Enabled="false" ID="MaskedEditExtender1" runat="server" TargetControlID="ListPrice"
                    Mask="9,999,999.99" MessageValidatorTip="true" OnFocusCssClass="MaskedEditFocus"
                    OnInvalidCssClass="MaskedEditError" MaskType="Number" InputDirection="RightToLeft"
                    AcceptNegative="Left" DisplayMoney="Left" />
                <asp:RequiredFieldValidator runat="server" ID="ListPriceRequired" ControlToValidate="ListPrice" Display="None" 
                        ErrorMessage="<%$ Resources:CatalogStrings, Entry_Price_Required %>" />
                <asp:RangeValidator runat="server" ID="RangeValidator1" ControlToValidate="ListPrice" MinimumValue="-100000000" MaximumValue="1000000000" 
                    Type="Currency" Display="Dynamic" ErrorMessage="<%$ Resources:CatalogStrings, Entry_Enter_Valid_Display_Price %>"></asp:RangeValidator>
                <ajaxToolkit:ValidatorCalloutExtender runat="Server" ID="vceListPriceRequired" Width="220" TargetControlID="ListPriceRequired" HighlightCssClass="FormHighlight"/>
            </td>
            <td class="FormLabelCell">&nbsp;&nbsp;</td>
            <td class="FormLabelCell">
                <asp:Label ID="Label12" runat="server" Text="<%$ Resources:CatalogStrings, Entry_In_Stock %>"></asp:Label>:</td>
            <td class="FormFieldCell">
                <asp:TextBox runat="server" Width="50" ID="InStockQty"></asp:TextBox>
                <asp:RequiredFieldValidator runat="server" ID="InStockRequired" ControlToValidate="InStockQty" Display="None" 
                    ErrorMessage="<%$ Resources:CatalogStrings, Entry_In_Stock_Quantity_Required %>" />
                <ajaxToolkit:ValidatorCalloutExtender runat="Server" ID="vceInStockQty1" Width="220" TargetControlID="InStockRequired" HighlightCssClass="FormHighlight"/>
                <asp:RangeValidator runat="server" ID="InStockRange" ControlToValidate="InStockQty" MinimumValue="0" MaximumValue="1000000000" Type="Double" 
                    Display="Dynamic" ErrorMessage="<%$ Resources:CatalogStrings, Entry_Enter_Valid_Quantity %>"></asp:RangeValidator>
                <ajaxToolkit:ValidatorCalloutExtender runat="Server" ID="vceInStockQty2" Width="220" TargetControlID="InStockRange" HighlightCssClass="FormHighlight"/>
            </td>
        </tr>
        <tr>
            <td colspan="5" class="FormSpacerCell">
            </td>
        </tr>
        <tr>
            <td class="FormLabelCell">
                <asp:Label ID="Label2" runat="server" Text="<%$ Resources:CatalogStrings, Entry_Min_Quantity %>"></asp:Label>:</td>
            <td class="FormFieldCell">
                <asp:TextBox runat="server" Width="250" ID="MinQty"></asp:TextBox>
                <asp:RequiredFieldValidator runat="server" ID="MinQtyRequired" ControlToValidate="MinQty" Display="Dynamic" 
                    ErrorMessage="<%$ Resources:CatalogStrings, Entry_Min_Quantity_Required %>" />
                <asp:RangeValidator runat="server" ID="MinQtyRange" ControlToValidate="MinQty" MinimumValue="0" MaximumValue="1000000000" Type="Double" 
                    Display="Dynamic" ErrorMessage="<%$ Resources:CatalogStrings, Entry_Enter_Valid_Quantity %>"></asp:RangeValidator>
            </td>
            <td class="FormLabelCell">&nbsp;&nbsp;</td>
            <td class="FormLabelCell">
                <asp:Label ID="Label13" runat="server" Text="<%$ Resources:SharedStrings, Reserved %>"></asp:Label>:</td>
            <td class="FormFieldCell">
                <asp:TextBox runat="server" Width="50" ID="ReservedQty"></asp:TextBox>
                <asp:RequiredFieldValidator runat="server" ID="ReservedQtyRequired" ControlToValidate="ReservedQty" Display="None" 
                    ErrorMessage="<%$ Resources:CatalogStrings, Entry_Reserved_Quantity_Required %>" />
                <ajaxToolkit:ValidatorCalloutExtender runat="Server" ID="vceReservedQtyRequired" Width="220" TargetControlID="ReservedQtyRequired" HighlightCssClass="FormHighlight"/>
                <asp:RangeValidator runat="server" ID="ReservedQtyRange" ControlToValidate="ReservedQty" MinimumValue="0" MaximumValue="1000000000" Type="Double" 
                    Display="Dynamic" ErrorMessage="<%$ Resources:CatalogStrings, Entry_Enter_Valid_Quantity %>"></asp:RangeValidator>
                <ajaxToolkit:ValidatorCalloutExtender runat="Server" ID="vceReservedQtyRange" Width="220" TargetControlID="ReservedQtyRange" HighlightCssClass="FormHighlight"/>
            </td>
        </tr>
        <tr>
            <td colspan="5" class="FormSpacerCell">
            </td>
        </tr>
        <tr>
            <td class="FormLabelCell">
                <asp:Label ID="Label3" runat="server" Text="<%$ Resources:CatalogStrings, Entry_Max_Quantity %>"></asp:Label>:</td>
            <td class="FormFieldCell">
                <asp:TextBox runat="server" Width="250" ID="MaxQty"></asp:TextBox>
                <asp:RequiredFieldValidator runat="server" ID="MaxQtyRequired" ControlToValidate="MaxQty" Display="Dynamic" 
                    ErrorMessage="<%$ Resources:CatalogStrings, Entry_Max_Quantity_Required %>" />
                <asp:RangeValidator runat="server" ID="MaxQtyRange" ControlToValidate="MaxQty" MinimumValue="0" MaximumValue="1000000000" Type="Double" Display="Dynamic" ErrorMessage="<%$ Resources:CatalogStrings, Entry_Enter_Valid_Quantity %>"></asp:RangeValidator>
            </td>
            <td class="FormLabelCell">&nbsp;&nbsp;</td>
            <td class="FormLabelCell">
                <asp:Label ID="Label10" runat="server" Text="<%$ Resources:CatalogStrings, Entry_Reorder_Min_Qty %>"></asp:Label>:</td>
            <td class="FormFieldCell">
                <asp:TextBox runat="server" Width="50" ID="ReorderMinQty"></asp:TextBox>
                <asp:RequiredFieldValidator runat="server" ID="ReorderMinQtyRequired" ControlToValidate="ReorderMinQty" Display="None" 
                    ErrorMessage="<%$ Resources:CatalogStrings, Entry_Reorder_Min_Qty_Required %>" />
                <ajaxToolkit:ValidatorCalloutExtender runat="Server" ID="vceReorderMinQtyRequired" Width="220" TargetControlID="ReorderMinQtyRequired" HighlightCssClass="FormHighlight"/>
                <asp:RangeValidator runat="server" ID="ReorderMinQtyRange" ControlToValidate="ReorderMinQty" MinimumValue="0" MaximumValue="1000000000" Type="Double" 
                    Display="Dynamic" ErrorMessage="<%$ Resources:CatalogStrings, Entry_Enter_Valid_Quantity %>"></asp:RangeValidator>
                <ajaxToolkit:ValidatorCalloutExtender runat="Server" ID="vceReorderMinQtyRange" Width="220" TargetControlID="ReorderMinQtyRange" HighlightCssClass="FormHighlight"/>
            </td>
        </tr>
        <tr>
            <td colspan="5" class="FormSpacerCell">
            </td>
        </tr>
        <tr>
            <td class="FormLabelCell">
                <asp:Label ID="Label4" runat="server" Text="<%$ Resources:CatalogStrings, Entry_Merchant %>"></asp:Label>:</td>
            <td class="FormFieldCell">
                <asp:DropDownList runat="server" ID="MerchantList" DataTextField="FriendlyName" DataValueField="Name">
                    <asp:ListItem Value="" Text="<%$ Resources:CatalogStrings, Entry_Select_Merchant %>"></asp:ListItem>
                </asp:DropDownList>
            </td>
            <td class="FormLabelCell">&nbsp;&nbsp;</td>
            <td class="FormLabelCell">
                <asp:Label ID="Label11" runat="server" Text="<%$ Resources:CatalogStrings, Entry_Allow_Preorder %>"></asp:Label>:</td>
            <td class="FormFieldCell">
                <ecf:BooleanEditControl ID="AllowPreorder" runat="server" MDContext="<%# Mediachase.Commerce.Catalog.CatalogContext.MetaDataContext %>"></ecf:BooleanEditControl>
            </td>
        </tr>
        <tr>
            <td colspan="5" class="FormSpacerCell">
            </td>
        </tr>
        <tr>
            <td class="FormLabelCell">
                <asp:Label ID="Label9" runat="server" Text="<%$ Resources:SharedStrings, Weight %>"></asp:Label>:</td>
            <td class="FormFieldCell">
                <asp:TextBox runat="server" Width="250" ID="Weight"></asp:TextBox><br />
                <asp:RequiredFieldValidator runat="server" ID="WeightRequired" ControlToValidate="Weight" Display="Dynamic" ErrorMessage="<%$ Resources:CatalogStrings, Entry_Weight_Required %>" />
                <asp:RangeValidator runat="server" ID="WeightRange" ControlToValidate="Weight" MinimumValue="0" MaximumValue="1000000000" Type="Double" Display="Dynamic" ErrorMessage="<%$ Resources:CatalogStrings, Entry_Enter_Valid_Quantity %>"></asp:RangeValidator>
            </td>
            <td class="FormLabelCell">&nbsp;&nbsp;</td>
            <td class="FormLabelCell">
                <asp:Label ID="Label14" runat="server" Text="<%$ Resources:CatalogStrings, Entry_Preorder_Qty %>"></asp:Label>:</td>
            <td class="FormFieldCell">
                <asp:TextBox runat="server" Width="50" ID="PreorderQty"></asp:TextBox>
                <asp:RequiredFieldValidator runat="server" ID="PreorderQtyRequired" ControlToValidate="PreorderQty" Display="None" ErrorMessage="<%$ Resources:CatalogStrings, Entry_Preorder_Qty_Required %>" />
                <ajaxToolkit:ValidatorCalloutExtender runat="Server" ID="vcePreorderQtyRequired" Width="220" TargetControlID="PreorderQtyRequired" HighlightCssClass="FormHighlight"/>
                <asp:RangeValidator runat="server" ID="PreorderQtyRange" ControlToValidate="PreorderQty" MinimumValue="0" MaximumValue="1000000000" Type="Double" Display="Dynamic" ErrorMessage="<%$ Resources:CatalogStrings, Entry_Enter_Valid_Quantity %>"></asp:RangeValidator>
                <ajaxToolkit:ValidatorCalloutExtender runat="Server" ID="vcePreorderQtyRange" Width="220" TargetControlID="PreorderQtyRange" HighlightCssClass="FormHighlight"/>
            </td>
        </tr>
        <tr>
            <td colspan="5" class="FormSpacerCell">
            </td>
        </tr>
        <tr>
            <td class="FormLabelCell">
                <asp:Label ID="Label5" runat="server" Text="<%$ Resources:SharedStrings, Package %>"></asp:Label>:</td>
            <td class="FormFieldCell">
                <asp:DropDownList runat="server" ID="PackageList" DataTextField="FriendlyName" DataValueField="Name">
                    <asp:ListItem Value="0" Text="<%$ Resources:CatalogStrings, Entry_Select_Package %>"></asp:ListItem>
                </asp:DropDownList>
            </td>
            <td class="FormLabelCell">&nbsp;&nbsp;</td>
            <td class="FormLabelCell">
                <asp:Label ID="Label15" runat="server" Text="<%$ Resources:CatalogStrings, Entry_Preorder_Available %>"></asp:Label>:</td>
            <td class="FormFieldCell">
                <ecf:CalendarDatePicker runat="server" ID="PreorderAvail" />
            </td>
        </tr>
        <tr>
            <td colspan="5" class="FormSpacerCell">
            </td>
        </tr>
        <tr>
            <td class="FormLabelCell">
                <asp:Label ID="Label6" runat="server" Text="<%$ Resources:CatalogStrings, Entry_Tax_Category %>"></asp:Label>:</td>
            <td class="FormFieldCell">
                <asp:DropDownList runat="server" ID="TaxList" DataTextField="FriendlyName" DataValueField="Id">
                    <asp:ListItem Value="0" Text="<%$ Resources:CatalogStrings, Entry_Select_Tax_Category %>"></asp:ListItem>
                </asp:DropDownList>
            </td>
            <td class="FormLabelCell">&nbsp;&nbsp;</td>
            <td class="FormLabelCell">
                <asp:Label ID="Label16" runat="server" Text="<%$ Resources:CatalogStrings, Entry_Allow_Backorder %>"></asp:Label>:</td>
            <td class="FormFieldCell">
                <ecf:BooleanEditControl ID="AllowBackorder" runat="server"></ecf:BooleanEditControl>
            </td>
        </tr>
        <tr>
            <td colspan="5" class="FormSpacerCell">
            </td>
        </tr>
        <tr>
            <td class="FormLabelCell">
                <asp:Label ID="Label7" runat="server" Text="<%$ Resources:SharedStrings, Warehouse %>"></asp:Label>:</td>
            <td class="FormFieldCell">
                <asp:DropDownList runat="server" ID="WarehouseList" DataTextField="FriendlyName"
                    DataValueField="Id">
                    <asp:ListItem Value="0" Text="<%$ Resources:CatalogStrings, Entry_Select_Warehouse %>"></asp:ListItem>
                </asp:DropDownList>
            </td>
            <td class="FormLabelCell">&nbsp;&nbsp;</td>
            <td class="FormLabelCell">
                <asp:Label ID="Label17" runat="server" Text="<%$ Resources:CatalogStrings, Entry_Backorder_Qty %>"></asp:Label>:</td>
            <td class="FormFieldCell">
                <asp:TextBox runat="server" Width="50" ID="BackorderQty"></asp:TextBox>
                <asp:RequiredFieldValidator runat="server" ID="BackorderQtyRequired" ControlToValidate="BackorderQty" Display="None" ErrorMessage="<%$ Resources:CatalogStrings, Entry_Backorder_Qty_Required %>" />
                <ajaxToolkit:ValidatorCalloutExtender runat="Server" ID="vceBackorderQtyRequired" Width="220" TargetControlID="BackorderQtyRequired" HighlightCssClass="FormHighlight"/>
                <asp:RangeValidator runat="server" ID="BackorderQtyRange" ControlToValidate="BackorderQty" MinimumValue="0" MaximumValue="1000000000" Type="Double" Display="Dynamic" ErrorMessage="<%$ Resources:CatalogStrings, Entry_Enter_Valid_Quantity %>"></asp:RangeValidator>
                <ajaxToolkit:ValidatorCalloutExtender runat="Server" ID="vceBackorderQtyRange" Width="220" TargetControlID="BackorderQtyRange" HighlightCssClass="FormHighlight"/>
            </td>
        </tr>
        <tr>
            <td colspan="5" class="FormSpacerCell">
            </td>
        </tr>
        <tr>
            <td class="FormLabelCell">
                <asp:Label ID="Label8" runat="server" Text="<%$ Resources:CatalogStrings, Entry_Track_Inventory %>"></asp:Label>:</td>
            <td class="FormFieldCell">
                <ecf:BooleanEditControl ID="TrackInventory" runat="server"></ecf:BooleanEditControl>
            </td>
            <td class="FormLabelCell">&nbsp;&nbsp;</td>
            <td class="FormLabelCell">
                <asp:Label ID="Label18" runat="server" Text="<%$ Resources:CatalogStrings, Entry_Backorder_Available %>"></asp:Label>:</td>
            <td class="FormFieldCell">
                <ecf:CalendarDatePicker runat="server" ID="BackorderAvail" />
            </td>
        </tr>
        <tr>
            <td colspan="5" class="FormSpacerCell">
            </td>
        </tr>        
        <!-- Price section -->
        <tr>
            <td class="FormLabelCell">
                <asp:Label ID="Label19" runat="server" Text="<%$ Resources:CatalogStrings, Entry_Inventory_Status %>"></asp:Label>:
            </td>
            <td class="FormFieldCell">
                <asp:DropDownList runat="server" ID="InventoryStatusList" DataTextField="FriendlyName"
                    DataValueField="Name">
                    <asp:ListItem Value="0" Text="<%$ Resources:SharedStrings, Disabled %>"></asp:ListItem>
                    <asp:ListItem Value="1" Text="<%$ Resources:SharedStrings, Enabled %>"></asp:ListItem>
                    <asp:ListItem Value="2" Text="<%$ Resources:SharedStrings, Ignored %>"></asp:ListItem>
                </asp:DropDownList>
            </td>
            <td colspan="3" class="FormSpacerCell">
            </td>
        </tr>
        <tr>
            <td colspan="5" class="FormSpacerCell">
                <fieldset>
                    <legend>
                        <asp:Literal ID="Literal1" runat="server" Text="<%$ Resources:CatalogStrings, Entry_Pricing %>" />
                    </legend>
                    <table>
                        <tr>
                            <td>
                                <a href="#" onclick="SalePriceItem_Edit('0')"><asp:Literal ID="Literal2" runat="server" Text="<%$ Resources:CatalogStrings, Entry_Add_Item %>" /></a>
                            </td>
                        </tr>
                        <tr>
                            <td class="wh100">
                                <ComponentArt:Grid Debug="false" AllowEditing="true" RunningMode="Callback" AutoFocusSearchBox="false" ShowHeader="false" ShowFooter="false" Width="100%" SkinID="Inline" runat="server" ID="SalePricesGrid" 
                                    AutoPostBackOnInsert="false" AutoPostBackOnDelete="false" AutoPostBackOnUpdate="false" 
                                    AutoCallBackOnInsert="false" AutoCallBackOnDelete="false" AutoCallBackOnUpdate="false">
                                    <ClientTemplates>
                                          <ComponentArt:ClientTemplate Id="EditTemplate">
                                            <a href="javascript:SalePriceItem_Edit(SalePricesGrid.getItemFromClientId('## DataItem.ClientId ##').getMember('SalePriceId').get_text());"><img alt="edit" src="../../../App_Themes/Default/Images/edit.gif" /></a> | <a href="javascript:SalePricesGrid.deleteItem(SalePricesGrid.getItemFromClientId('## DataItem.ClientId ##'));"><img alt="delete" src="../../../App_Themes/Main/Images/toolbar/delete.gif" /></a>
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
                </fieldset>
            </td>
        </tr>
    </table>
</div>
<uc1:DialogControl id="SalePriceEditDialog" AppId="Catalog" Width="700" ViewId="View-SalePrice" runat="server" />
