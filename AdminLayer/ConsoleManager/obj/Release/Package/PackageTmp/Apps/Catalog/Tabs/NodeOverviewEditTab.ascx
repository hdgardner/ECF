<%@ Control Language="C#" AutoEventWireup="true" Inherits="Mediachase.Commerce.Manager.Catalog.Tabs.NodeOverviewEditTab" Codebehind="NodeOverviewEditTab.ascx.cs" %>
<%@ Register Src="~/Apps/Core/Controls/BooleanEditControl.ascx" TagName="BooleanEditControl" TagPrefix="ecf" %>
<%@ Register Src="~/Apps/Core/Controls/CalendarDatePicker.ascx" TagName="CalendarDatePicker" TagPrefix="ecf" %>
<%@ Register Src="~/Apps/Core/MetaData/EditTab.ascx" TagName="MetaData" TagPrefix="ecf" %>

<div id="DataForm">
 <table class="DataForm"> 
   <tr>  
     <td class="FormLabelCell"><asp:Label runat="server" Text="<%$ Resources:SharedStrings, Name %>"></asp:Label>:</td> 
     <td class="FormFieldCell">
        <asp:TextBox runat="server" Width="250" ID="Name"></asp:TextBox><br />
        <asp:Label CssClass="FormFieldDescription" runat="server" Text="<%$ Resources:CatalogStrings, Node_Name_Description %>"></asp:Label>
        <asp:RequiredFieldValidator runat="server" ID="NameRequired" ControlToValidate="Name" Display="None" ErrorMessage="<%$ Resources:CatalogStrings, Catalog_Name_Required %>" />
        <ajaxToolkit:ValidatorCalloutExtender runat="Server" ID="NameRequiredE" Width="220" TargetControlID="NameRequired" HighlightCssClass="FormHighlight"/>
     </td> 
   </tr> 
   <tr>  
     <td colspan="2" class="FormSpacerCell"></td> 
   </tr>   
   <tr>  
     <td class="FormLabelCell"><asp:Label ID="Label1" runat="server" Text="<%$ Resources:SharedStrings, Available_From %>"></asp:Label>:</td> 
     <td class="FormFieldCell">
         <ecf:CalendarDatePicker runat="server" ID="AvailableFrom" />
     </td> 
   </tr>   
   <tr>  
     <td colspan="2" class="FormSpacerCell"></td>
   </tr>     
   <tr>  
     <td class="FormLabelCell"><asp:Label ID="Label2" runat="server" Text="<%$ Resources:SharedStrings, Expires_On %>"></asp:Label>:</td> 
     <td class="FormFieldCell">
        <ecf:CalendarDatePicker runat="server" ID="ExpiresOn" />
     </td> 
   </tr>    
   <tr>  
     <td colspan="2" class="FormSpacerCell"></td> 
   </tr>     
   <tr>  
     <td class="FormLabelCell"><asp:Label ID="Label3" runat="server" Text="<%$ Resources:SharedStrings, Display_Template %>"></asp:Label>:</td> 
     <td class="FormFieldCell">
        <asp:DropDownList runat="server" id="DisplayTemplate" DataTextField="FriendlyName" DataValueField="Name">
            <asp:ListItem Value="" Text="<%$ Resources:CatalogStrings, Select_Display_Template %>"></asp:ListItem>
        </asp:DropDownList>
     </td> 
   </tr>    
   <tr>  
     <td colspan="2" class="FormSpacerCell"></td> 
   </tr>
   <tr>  
     <td class="FormLabelCell"><asp:Label ID="Label9" runat="server" Text="<%$ Resources:SharedStrings, Code %>"></asp:Label>:</td> 
     <td class="FormFieldCell">
        <asp:TextBox runat="server" Width="250" ID="CodeText"></asp:TextBox>
        <asp:RequiredFieldValidator runat="server" ID="CodeTextRequired" ControlToValidate="CodeText" Display="None" ErrorMessage="<%$ Resources:SharedStrings, Code_Required %>" />
        <ajaxToolkit:ValidatorCalloutExtender runat="Server" ID="vceCodeTextRequired" Width="220" TargetControlID="CodeTextRequired" HighlightCssClass="FormHighlight"/>
        <br />
        <asp:CustomValidator runat="server" id="cvCodeCheck"
            ControlToValidate="CodeText"
            OnServerValidate="NodeCodeCheck" Display="Dynamic"
            ErrorMessage="<%$ Resources:CatalogStrings, Catalog_Node_With_Code_Exists %>" />
     </td>
   </tr> 
   <tr>  
     <td colspan="2" class="FormSpacerCell"></td> 
   </tr>   
   <tr>  
     <td class="FormLabelCell"><asp:Label ID="Label11" runat="server" Text="<%$ Resources:SharedStrings, Sort_Order %>"></asp:Label>:</td> 
     <td class="FormFieldCell">
        <asp:TextBox runat="server" Width="50" ID="SortOrder"></asp:TextBox><br />
     </td> 
   </tr> 
   <tr>  
     <td colspan="2" class="FormSpacerCell"></td> 
   </tr>      
   <tr>  
     <td class="FormLabelCell"><asp:Label ID="Label5" runat="server" Text="<%$ Resources:SharedStrings, Available %>"></asp:Label>:</td> 
     <td class="FormFieldCell">
        <ecf:BooleanEditControl id="IsCatalogNodeActive" runat="server"></ecf:BooleanEditControl>
     </td> 
   </tr>   
   <tr>  
     <td colspan="2" class="FormSpacerCell"></td> 
   </tr>    
   <tr>  
     <td class="FormLabelCell"><asp:Label ID="Label6" runat="server" Text="<%$ Resources:CatalogStrings, Meta_Class %>"></asp:Label>:</td> 
     <td class="FormFieldCell">
        <asp:DropDownList runat="server" AutoPostBack="true" id="MetaClassList" DataTextField="FriendlyName" DataValueField="Id">
        </asp:DropDownList>
     </td> 
   </tr>
   <tr>  
     <td colspan="2" class="FormSpacerCell"></td> 
   </tr>      
   <ecf:MetaData runat="server" ID="MetaDataTab" MDContext="<%# Mediachase.Commerce.Catalog.CatalogContext.MetaDataContext %>" />
 </table>
</div>