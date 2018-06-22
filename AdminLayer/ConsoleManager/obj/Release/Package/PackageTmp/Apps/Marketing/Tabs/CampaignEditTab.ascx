<%@ Control Language="C#" AutoEventWireup="true" Inherits="Mediachase.Commerce.Manager.Marketing.Tabs.CampaignEditTab" Codebehind="CampaignEditTab.ascx.cs" %>
<%@ Register Src="~/Apps/Core/Controls/BooleanEditControl.ascx" TagName="BooleanEditControl"
    TagPrefix="ecf" %>
<%@ Register Src="~/Apps/Core/Controls/CalendarDatePicker.ascx" TagName="CalendarDatePicker"
    TagPrefix="ecf" %>
<div id="DataForm">
  <table class="DataForm"> 
   <tr>  
     <td class="FormLabelCell"><asp:Label runat="server" Text="<%$ Resources:MarketingStrings, Campaign_Name %>"></asp:Label>:</td> 
     <td class="FormFieldCell">
        <asp:TextBox runat="server" Width="250" ID="CampaignName" MaxLength="50"></asp:TextBox><br />
        <asp:Label CssClass="FormFieldDescription" runat="server" Text="<%$ Resources:MarketingStrings, Campaign_Name_Enter %>"></asp:Label>
        <asp:RequiredFieldValidator runat="server" ID="CampaignNameRequired" ControlToValidate="CampaignName" Display="None" ErrorMessage="<%$ Resources:MarketingStrings, Campaign_Name_Required %>" />
        <ajaxToolkit:ValidatorCalloutExtender runat="Server" ID="CampaignNameRequiredE" Width="220" TargetControlID="CampaignNameRequired" HighlightCssClass="FormHighlight"/>
     </td> 
   </tr> 
   <tr>  
     <td colspan="2" class="FormSpacerCell"></td> 
   </tr>    
   <tr>  
     <td class="FormLabelCell"><asp:Label ID="Label6" runat="server" Text="<%$ Resources:SharedStrings, Comments %>"></asp:Label>:</td> 
     <td class="FormFieldCell">
        <asp:TextBox runat="server" ID="Comments" Rows="4" TextMode="MultiLine" Columns="50" MaxLength="1024"></asp:TextBox>
     </td>  
   </tr>                
   <tr>  
     <td colspan="2" class="FormSpacerCell"></td> 
   </tr>
   <tr>  
     <td class="FormLabelCell"><asp:Label ID="Label1" runat="server" Text="<%$ Resources:SharedStrings, Available_From %>"></asp:Label>:</td> 
     <td class="FormFieldCell">
        <ecf:CalendarDatePicker runat="server" ID="AvailableFrom"  />
     </td> 
   </tr>   
   <tr>  
     <td colspan="2" class="FormSpacerCell"></td>
   </tr>     
   <tr>  
     <td class="FormLabelCell"><asp:Label ID="Label2" runat="server" Text="<%$ Resources:SharedStrings, Expires_On %>"></asp:Label>:</td> 
     <td class="FormFieldCell"><ecf:CalendarDatePicker runat="server" ID="ExpiresOn" /></td> 
   </tr>    
   <tr>  
     <td colspan="2" class="FormSpacerCell"></td> 
   </tr>     
   <tr>  
     <td class="FormLabelCell"><asp:Label ID="Label4" runat="server" Text="<%$ Resources:MarketingStrings, Campaign_Target_Segments %>"></asp:Label>:</td> 
     <td class="FormFieldCell">
        <asp:ListBox runat="server" SelectionMode="Multiple" Height="150" Width="250" ID="TargetSegments" DataMember="Segment" DataTextField="DisplayName" DataValueField="SegmentId"></asp:ListBox>
     </td>  
   </tr>    
   <tr>  
     <td colspan="2" class="FormSpacerCell"></td> 
   </tr>     
   <tr>  
     <td class="FormLabelCell"><asp:Label ID="Label5" runat="server" Text="<%$ Resources:SharedStrings, Active %>"></asp:Label>:</td> 
     <td class="FormFieldCell">
        <ecf:BooleanEditControl id="IsActive" runat="server"></ecf:BooleanEditControl>
     </td> 
   </tr>
   <tr>  
     <td colspan="2" class="FormSpacerCell"></td> 
   </tr>     
   <tr>  
     <td class="FormLabelCell"><asp:Label ID="Label3" runat="server" Text="<%$ Resources:SharedStrings, Archived %>"></asp:Label>:</td> 
     <td class="FormFieldCell">
        <ecf:BooleanEditControl id="IsArchived" runat="server"></ecf:BooleanEditControl>
     </td> 
   </tr>
   
 </table>
</div>