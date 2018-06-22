<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PackageEditBaseTab.ascx.cs" Inherits="Mediachase.Commerce.Manager.Order.Shipping.Tabs.PackageEditBaseTab" %>
<script language="javascript" type="text/javascript">
function ToggleDimensionsTable()
{
    var fs = document.getElementById('fsDimensions');
    var chb = $get('<%= chbDimensions.ClientID %>');
    
    var oWidth = $get('<%= tbWidth.ClientID %>');
    var oLength = $get('<%= tbLength.ClientID %>');
    var oHeight = $get('<%= tbHeight.ClientID %>');
    
    if(fs && chb)
    {
        if(chb.checked)
            fs.disabled = false;
        else
            fs.disabled = true;

        if(oWidth)
            oWidth.disabled = !chb.checked;
            
        if(oLength)
            oLength.disabled = !chb.checked;
            
        if(oHeight)
            oHeight.disabled = !chb.checked;
    }
}
</script>
<div id="DataForm">
    <table class="DataForm">
        <tr>
            <td class="FormLabelCell">
                <asp:Label ID="lblName" runat="server" Text="<%$ Resources:SharedStrings, Name %>"></asp:Label>:
            </td>
            <td class="FormFieldCell">
                <asp:TextBox runat="server" ID="tbName" Width="200px"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfName" runat="server" ErrorMessage="<%$ Resources:SharedStrings, Name_Required %>"
                    Font-Size="9pt" Font-Names="verdana" Display="None" ControlToValidate="tbName" />
                <ajaxToolkit:ValidatorCalloutExtender runat="server" ID="vce" Width="220" TargetControlID="rfName"
                    HighlightCssClass="FormHighlight" />
                <asp:CustomValidator runat="server" ID="NameCustomValidator" ControlToValidate="tbName" OnServerValidate="ShippingPackageNameCheck" Display="Dynamic" ErrorMessage="<%$ Resources:OrderStrings, Package_Name_Exists %>" />
            </td>
        </tr>
        <tr>
            <td colspan="2" class="FormSpacerCell"></td>
        </tr>
        <tr>
            <td class="FormLabelCell">
                <asp:Label ID="lblDescription" runat="server" Text="<%$ Resources:SharedStrings, Description %>"></asp:Label>:
            </td>
            <td class="FormFieldCell">
                <asp:TextBox runat="server" ID="tbDescription" Rows="4" TextMode="MultiLine" Width="350px"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td colspan="2" class="FormSpacerCell"></td>
        </tr>
        <tr>
            <td class="FormLabelCell" style="vertical-align: middle;" colspan="2">
                <input type="checkbox" runat="server" id="chbDimensions" onclick="javascript:ToggleDimensionsTable();" />
                <label for="<%= chbDimensions.ClientID %>">&nbsp;<asp:Literal ID="Literal1" runat="server" Text="<%$ Resources:SharedStrings, Set_Dimensions %>"/></label>
            </td>
        </tr>
        <tr>
            <td class="FormFieldCell" colspan="2">
                <fieldset id="fsDimensions">
                    <legend>
                        <asp:Literal ID="Literal2" runat="server" Text="<%$ Resources:SharedStrings, Dimensions %>"/>
                    </legend>
                    <table id="tblDimensions" runat="server">
                        <tr>
                            <td class="FormLabelCell">
                                <asp:Label ID="lblWidth" runat="server" Text="<%$ Resources:SharedStrings, Width %>"></asp:Label>:
                            </td>
                            <td class="FormFieldCell">
                                <asp:TextBox runat="server" ID="tbWidth" Width="150px"></asp:TextBox>
                                <asp:CustomValidator id="WidthValidator" Runat="server" OnServerValidate="WidthValidate" ControlToValidate="tbWidth" Display="Dynamic">*</asp:CustomValidator>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" class="FormSpacerCell"></td>
                        </tr>
                        <tr>
                            <td class="FormLabelCell">
                                <asp:Label ID="Label6" runat="server" Text="<%$ Resources:SharedStrings, Length %>"></asp:Label>:
                            </td>
                            <td class="FormFieldCell">
                                <asp:TextBox runat="server" ID="tbLength" Width="150px"></asp:TextBox>
                                <asp:CustomValidator id="LengthValidator" Runat="server" OnServerValidate="LengthValidate" ControlToValidate="tbLength" Display="Dynamic">*</asp:CustomValidator>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" class="FormSpacerCell"></td>
                        </tr>
                        <tr>
                            <td class="FormLabelCell">
                                <asp:Label ID="Label8" runat="server" Text="<%$ Resources:SharedStrings, Height %>"></asp:Label>:
                            </td>
                            <td class="FormFieldCell">
                                <asp:TextBox runat="server" ID="tbHeight" Width="150px"></asp:TextBox>
                                <asp:CustomValidator id="HeightValidator" Runat="server" OnServerValidate="HeightValidate" ControlToValidate="tbHeight" Display="Dynamic">*</asp:CustomValidator>
                            </td>
                        </tr>
                    </table>
                </fieldset>
            </td>
        </tr>
        
    </table>
</div>