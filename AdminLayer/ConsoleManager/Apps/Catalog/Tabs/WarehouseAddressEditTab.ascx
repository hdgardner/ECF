<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WarehouseAddressEditTab.ascx.cs" Inherits="Mediachase.Commerce.Manager.Catalog.Tabs.WarehouseAddressEditTab" %>
<div id="DataForm">
    <table class="DataForm">
        <tr>
            <td class="FormLabelCell">
                <asp:Label ID="Label1" runat="server" Text="<%$ Resources:SharedStrings, First_Name %>"></asp:Label>:
            </td>
            <td class="FormFieldCell">
                <asp:TextBox runat="server" ID="FirstName" ValidationGroup="AddressMetaData"></asp:TextBox>
                <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator0" ValidationGroup="AddressMetaData" ControlToValidate="FirstName" Display="Dynamic" ErrorMessage="*" />
            </td>
        </tr>
        <tr>  
            <td colspan="2" class="FormSpacerCell"></td> 
        </tr>
        <tr>
            <td class="FormLabelCell">
                <asp:Label ID="Label2" runat="server" Text="<%$ Resources:SharedStrings, Last_Name %>"></asp:Label>:
            </td>
            <td class="FormFieldCell">
                <asp:TextBox runat="server" ID="LastName" ValidationGroup="AddressMetaData"></asp:TextBox>
                <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator1" ValidationGroup="AddressMetaData" ControlToValidate="LastName" Display="Dynamic" ErrorMessage="*" />
            </td>
        </tr>
        <tr>  
            <td colspan="2" class="FormSpacerCell"></td> 
        </tr>        
        <tr>
            <td class="FormLabelCell">
                <asp:Label ID="Label3" runat="server" Text="<%$ Resources:SharedStrings, Organization %>"></asp:Label>:
            </td>
            <td class="FormFieldCell">
                <asp:TextBox runat="server" ID="Organization"></asp:TextBox>
            </td>
        </tr>
        <tr>  
            <td colspan="2" class="FormSpacerCell"></td> 
        </tr>        
        <tr>
            <td class="FormLabelCell">
                <asp:Label ID="Label6" runat="server" Text="<%$ Resources:SharedStrings, Line_1 %>"></asp:Label>:
            </td>
            <td class="FormFieldCell">
                <asp:TextBox runat="server" ID="Line1" ValidationGroup="AddressMetaData"></asp:TextBox>
                <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator2" ValidationGroup="AddressMetaData" ControlToValidate="Line1" Display="Dynamic" ErrorMessage="*" />
            </td>
        </tr>
        <tr>  
            <td colspan="2" class="FormSpacerCell"></td> 
        </tr>        
        <tr>
            <td class="FormLabelCell">
                <asp:Label ID="Label7" runat="server" Text="<%$ Resources:SharedStrings, Line_2 %>"></asp:Label>:
            </td>
            <td class="FormFieldCell">
                <asp:TextBox runat="server" ID="Line2"></asp:TextBox>
            </td>
        </tr>
        <tr>  
            <td colspan="2" class="FormSpacerCell"></td> 
        </tr>        
        <tr>
            <td class="FormLabelCell">
                <asp:Label ID="Label8" runat="server" Text="<%$ Resources:SharedStrings, City %>"></asp:Label>:
            </td>
            <td class="FormFieldCell">
                <asp:TextBox runat="server" ID="City" ValidationGroup="AddressMetaData"></asp:TextBox>
                <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator3" ValidationGroup="AddressMetaData" ControlToValidate="City" Display="Dynamic" ErrorMessage="*" />
            </td>
        </tr>
        <tr>  
            <td colspan="2" class="FormSpacerCell"></td> 
        </tr>        
        <tr>
            <td class="FormLabelCell">
                <asp:Label ID="Label9" runat="server" Text="<%$ Resources:SharedStrings, State %>"></asp:Label>:
            </td>
            <td class="FormFieldCell">
                <asp:TextBox runat="server" ID="State" ValidationGroup="AddressMetaData"></asp:TextBox>
                <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator4" ValidationGroup="AddressMetaData" ControlToValidate="State" Display="Dynamic" ErrorMessage="*" />
            </td>
        </tr>
        <tr>  
            <td colspan="2" class="FormSpacerCell"></td> 
        </tr>        
        <tr>
            <td class="FormLabelCell">
                <asp:Label ID="Label10" runat="server" Text="<%$ Resources:SharedStrings, Country_Code %>"></asp:Label>:
            </td>
            <td class="FormFieldCell">
                <asp:TextBox runat="server" ID="CountryCode" ValidationGroup="AddressMetaData"></asp:TextBox>
                <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator5" ValidationGroup="AddressMetaData" ControlToValidate="CountryCode" Display="Dynamic" ErrorMessage="*" />
            </td>
        </tr>
        <tr>  
            <td colspan="2" class="FormSpacerCell"></td> 
        </tr>        
        <tr>
            <td class="FormLabelCell">
                <asp:Label ID="Label11" runat="server" Text="<%$ Resources:SharedStrings, Country_Name %>"></asp:Label>:
            </td>
            <td class="FormFieldCell">
                <asp:TextBox runat="server" ID="CountryName"></asp:TextBox>
            </td>
        </tr>
        <tr>  
            <td colspan="2" class="FormSpacerCell"></td> 
        </tr>        
        <tr>
            <td class="FormLabelCell">
                <asp:Label ID="Label12" runat="server" Text="<%$ Resources:SharedStrings, Postal_Code %>"></asp:Label>:
            </td>
            <td class="FormFieldCell">
                <asp:TextBox runat="server" ID="PostalCode" ValidationGroup="AddressMetaData"></asp:TextBox>
                <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator6" ValidationGroup="AddressMetaData" ControlToValidate="PostalCode" Display="Dynamic" ErrorMessage="*" />
            </td>
        </tr>
        <tr>  
            <td colspan="2" class="FormSpacerCell"></td> 
        </tr>        
        <tr>
            <td class="FormLabelCell">
                <asp:Label ID="Label13" runat="server" Text="<%$ Resources:SharedStrings, Region_Code %>"></asp:Label>:
            </td>
            <td class="FormFieldCell">
                <asp:TextBox runat="server" ID="RegionCode"></asp:TextBox>
            </td>
        </tr>
        <tr>  
            <td colspan="2" class="FormSpacerCell"></td> 
        </tr>        
        <tr>
            <td class="FormLabelCell">
                <asp:Label ID="Label14" runat="server" Text="<%$ Resources:SharedStrings, Region_Name %>"></asp:Label>:
            </td>
            <td class="FormFieldCell">
                <asp:TextBox runat="server" ID="RegionName"></asp:TextBox>
            </td>
        </tr>
        <tr>  
            <td colspan="2" class="FormSpacerCell"></td> 
        </tr>
        <tr>
            <td class="FormLabelCell">
                <asp:Label ID="Label15" runat="server" Text="<%$ Resources:SharedStrings, Day_Phone %>"></asp:Label>:
            </td>
            <td class="FormFieldCell">
                <asp:TextBox runat="server" ID="DayTimePhone"></asp:TextBox>
            </td>
        </tr>
                <tr>  
            <td colspan="2" class="FormSpacerCell"></td> 
        </tr>
        <tr>
            <td class="FormLabelCell">
                <asp:Label ID="Label16" runat="server" Text="<%$ Resources:SharedStrings, Evening_Phone %>"></asp:Label>:
            </td>
            <td class="FormFieldCell">
                <asp:TextBox runat="server" ID="EveningPhone"></asp:TextBox>
            </td>
        </tr>
        <tr>  
            <td colspan="2" class="FormSpacerCell"></td> 
        </tr>
        <tr>
            <td class="FormLabelCell">
                <asp:Label ID="Label17" runat="server" Text="<%$ Resources:SharedStrings, Fax_Number %>"></asp:Label>:
            </td>
            <td class="FormFieldCell">
                <asp:TextBox runat="server" ID="FaxNumber"></asp:TextBox>
            </td>
        </tr>
        <tr>  
            <td colspan="2" class="FormSpacerCell"></td> 
        </tr>        
        <tr>
            <td class="FormLabelCell">
                <asp:Label ID="Label18" runat="server" Text="<%$ Resources:SharedStrings, Email %>"></asp:Label>:
            </td>
            <td class="FormFieldCell">
                <asp:TextBox runat="server" ID="Email" ValidationGroup="AddressMetaData"></asp:TextBox>
                <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator7" ValidationGroup="AddressMetaData" ControlToValidate="Email" Display="Dynamic" ErrorMessage="*" />
                <asp:regularexpressionvalidator id="RegularExpressionValidator1" runat="server" ValidationGroup="AddressMetaData" ControlToValidate="Email" Display="Dynamic" ErrorMessage="*" ValidationExpression="[\w\.-]+(\+[\w-]*)?@([\w-]+\.)+[\w-]+"></asp:regularexpressionvalidator>
            </td>
        </tr>
    </table>
</div>