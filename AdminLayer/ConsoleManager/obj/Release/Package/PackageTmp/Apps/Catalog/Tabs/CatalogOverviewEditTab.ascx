<%@ Control Language="C#" AutoEventWireup="true" Inherits="Mediachase.Commerce.Manager.Catalog.Tabs.CatalogOverviewEditTab" Codebehind="CatalogOverviewEditTab.ascx.cs" %>
<%@ Register Src="~/Apps/Core/Controls/BooleanEditControl.ascx" TagName="BooleanEditControl" TagPrefix="ecf" %>
<%@ Register Src="~/Apps/Core/Controls/CalendarDatePicker.ascx" TagName="CalendarDatePicker" TagPrefix="ecf" %>
<div id="DataForm">
  <table class="DataForm"> 
   <tr>  
     <td class="FormLabelCell"><asp:Label runat="server" Text="<%$ Resources:CatalogStrings, Catalog_Catalog_Name %>"></asp:Label>:</td> 
     <td class="FormFieldCell">
        <asp:TextBox runat="server" Width="250" ID="CatalogName"></asp:TextBox>
        <asp:RequiredFieldValidator runat="server" ID="CatalogNameRequired" ControlToValidate="CatalogName" Display="Dynamic" ErrorMessage="<%$ Resources:CatalogStrings, Catalog_Catalog_Name_Required %>" />
        <ajaxToolkit:ValidatorCalloutExtender runat="Server" ID="CatalogNameRequiredE" Width="100" TargetControlID="CatalogNameRequired" HighlightCssClass="FormHighlight"/>
        <asp:CustomValidator runat="server" ID="NameUniqueCustomValidator" ControlToValidate="CatalogName"
            OnServerValidate="NameCheck" Display="Dynamic" ErrorMessage="<%$ Resources:CatalogStrings, Catalog_With_Name_AlreadyExists %>" />
        <br />
        <asp:Label ID="Label9" CssClass="FormFieldDescription" runat="server" Text="<%$ Resources:CatalogStrings, Catalog_Name_Description %>"></asp:Label>
     </td>
   </tr>
   <tr>
     <td colspan="2" class="FormSpacerCell"></td> 
   </tr>
   <tr>
     <td class="FormLabelCell"><asp:Label runat="server" Text="<%$ Resources:SharedStrings, Available_From %>"></asp:Label>:</td> 
     <td class="FormFieldCell"><ecf:CalendarDatePicker runat="server" ID="AvailableFrom" /></td> 
   </tr>
   <tr>  
     <td colspan="2" class="FormSpacerCell"></td>
   </tr>
   <tr>  
     <td class="FormLabelCell"><asp:Label runat="server" ID="Label6" Text="<%$ Resources:SharedStrings, Expires_On %>"></asp:Label>:</td> 
     <td class="FormFieldCell"><ecf:CalendarDatePicker runat="server" ID="ExpiresOn" /></td> 
   </tr>    
   <tr>  
     <td colspan="2" class="FormSpacerCell"></td> 
   </tr>     
   <tr>  
     <td class="FormLabelCell"><asp:Label ID="Label1" runat="server" Text="<%$ Resources:CatalogStrings, Catalog_Default_Currency %>"></asp:Label>:</td> 
     <td class="FormFieldCell">
        <asp:DropDownList runat="server" id="DefaultCurrency">
            <asp:ListItem Value="" Text="<%$ Resources:CatalogStrings, Catalog_Select_Primary_Currency %>"></asp:ListItem>
        </asp:DropDownList>
        <asp:RequiredFieldValidator runat="server" ID="DefaultCurrencyRequired" ControlToValidate="DefaultCurrency"
            Display="None" ErrorMessage="<%$ Resources:CatalogStrings, Catalog_Default_Currency_Required %>" 
            InitialValue="" />
        <ajaxToolkit:ValidatorCalloutExtender runat="Server" ID="DefaultCurrencyRequiredE" Width="200" TargetControlID="DefaultCurrencyRequired" HighlightCssClass="FormHighlight"/>
     </td>
   </tr>
   <tr>
     <td colspan="2" class="FormSpacerCell"></td> 
   </tr>   
   <tr>  
     <td class="FormLabelCell"><asp:Label ID="Label2" runat="server" Text="<%$ Resources:CatalogStrings, Catalog_Default_Language %>"></asp:Label>:</td>
     <td class="FormFieldCell">
        <asp:DropDownList runat="server" ID="DefaultLanguage">
            <asp:ListItem Value="" Text="<%$ Resources:CatalogStrings, Catalog_Select_Primary_Language %>"></asp:ListItem>
        </asp:DropDownList><br/>
        <asp:RequiredFieldValidator runat="server" ID="DefaultLanguageRequired" ControlToValidate="DefaultLanguage"
            Display="None" ErrorMessage="<%$ Resources:CatalogStrings, Catalog_Default_Language_Required %>" 
            InitialValue="" />
        <ajaxToolkit:ValidatorCalloutExtender runat="Server" ID="DefaultLanguageRequiredE" Width="200" TargetControlID="DefaultLanguageRequired" HighlightCssClass="FormHighlight"/>
     </td>
   </tr>
   <tr>
     <td colspan="2" class="FormSpacerCell"></td> 
   </tr>     
   <tr>
     <td class="FormLabelCell"><asp:Label ID="Label3" runat="server" Text="<%$ Resources:CatalogStrings, Catalog_Base_Weight %>"></asp:Label>:</td> 
     <td class="FormFieldCell">
        <asp:DropDownList runat="server" id="BaseWeight">
            <asp:ListItem Value="lbs" Text="<%$ Resources:CatalogStrings, Catalog_In_Pounds %>"></asp:ListItem>
            <asp:ListItem Value="kgs" Text="<%$ Resources:CatalogStrings, Catalog_In_Kilograms %>"></asp:ListItem>
        </asp:DropDownList>
     </td> 
   </tr>   
   <tr>  
     <td colspan="2" class="FormSpacerCell"></td> 
   </tr>     
   <tr>  
     <td class="FormLabelCell"><asp:Label ID="Label4" runat="server" Text="<%$ Resources:CatalogStrings, Catalog_Other_Languages %>"></asp:Label>:</td> 
     <td class="FormFieldCell">
        <asp:ListBox runat="server" ID="OtherLanguagesList" Width="250" SelectionMode="Multiple" DataTextField="LanguageCode" DataValueField="LanguageCode">
        </asp:ListBox>
     </td> 
   </tr>   
   <tr>  
     <td colspan="2" class="FormSpacerCell"></td> 
   </tr> 
   <tr>  
     <td class="FormLabelCell"><asp:Label ID="Label7" runat="server" Text="<%$ Resources:SharedStrings, Sites %>"></asp:Label>:</td> 
     <td class="FormFieldCell">
        <asp:ListBox runat="server" ID="SiteList" Width="250" SelectionMode="Multiple" DataTextField="Name" DataValueField="SiteId">
        </asp:ListBox>
     </td> 
   </tr>   
   <tr>  
     <td colspan="2" class="FormSpacerCell"></td> 
   </tr>     

   <tr>  
     <td class="FormLabelCell"><asp:Label ID="Label8" runat="server" Text="<%$ Resources:SharedStrings, Sort_Order %>"></asp:Label>:</td> 
     <td class="FormFieldCell">
        <asp:TextBox runat="server" ID="SortOrder"></asp:TextBox>
     </td> 
   </tr>
   <tr>  
     <td colspan="2" class="FormSpacerCell"></td> 
   </tr>              
   <tr>  
     <td class="FormLabelCell"><asp:Label ID="Label5" runat="server" Text="<%$ Resources:SharedStrings, Available %>"></asp:Label>:</td> 
     <td class="FormFieldCell">
        <ecf:BooleanEditControl id="IsCatalogActive" runat="server" MDContext="<%# Mediachase.Commerce.Catalog.CatalogContext.MetaDataContext %>"></ecf:BooleanEditControl>
     </td> 
   </tr>
 </table>
</div>