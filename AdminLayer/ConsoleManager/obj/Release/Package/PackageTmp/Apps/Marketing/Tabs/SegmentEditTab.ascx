<%@ Control Language="C#" AutoEventWireup="true" Inherits="Mediachase.Commerce.Manager.Marketing.Tabs.SegmentEditTab" Codebehind="SegmentEditTab.ascx.cs" %>
<%@ Register Src="~/Apps/Core/Controls/BooleanEditControl.ascx" TagName="BooleanEditControl"
    TagPrefix="ecf" %>
<%@ Register Src="~/Apps/Core/Controls/CalendarDatePicker.ascx" TagName="CalendarDatePicker"
    TagPrefix="ecf" %>
<%@ Register TagPrefix="ibn" Assembly="Mediachase.Ibn.Web.UI.WebControls" Namespace="Mediachase.Ibn.Web.UI.WebControls" %>    
<%@ Register src="../Modules/RulesEditor.ascx" tagname="RulesEditor" tagprefix="marketing" %>
<div id="DataForm">
  <table class="DataForm"> 
   <tr>  
     <td class="FormLabelCell"><asp:Label runat="server" Text="<%$ Resources:MarketingStrings, Segment_Name %>"></asp:Label>:</td> 
     <td class="FormFieldCell">
        <asp:TextBox runat="server" Width="250" ID="SegmentName"></asp:TextBox><br />
        <asp:Label CssClass="FormFieldDescription" runat="server" Text="<%$ Resources:MarketingStrings, Segment_Enter_Name %>"></asp:Label>
        <asp:RequiredFieldValidator runat="server" ID="SegmentNameRequired" ControlToValidate="SegmentName" Display="None" ErrorMessage="<%$ Resources:MarketingStrings, Segment_Name_Required %>" />
        <ajaxToolkit:ValidatorCalloutExtender runat="Server" ID="SegmentNameRequiredE" Width="220" TargetControlID="SegmentNameRequired" HighlightCssClass="FormHighlight"/>
     </td> 
   </tr>   
   <tr>  
     <td colspan="2" class="FormSpacerCell"></td> 
   </tr>    
   <tr>  
     <td class="FormLabelCell"><asp:Label ID="Label4" runat="server" Text="<%$ Resources:MarketingStrings, Segment_Display_Name %>"></asp:Label>:</td> 
     <td class="FormFieldCell">
        <asp:TextBox runat="server" ID="DisplayName" Width="250"></asp:TextBox>
     </td>  
   </tr> 
   <tr>  
     <td colspan="2" class="FormSpacerCell"></td> 
   </tr>    
   <tr>  
     <td class="FormLabelCell"><asp:Label ID="Label1" runat="server" Text="<%$ Resources:MarketingStrings, Segment_Description %>"></asp:Label>:</td> 
     <td class="FormFieldCell">
        <asp:TextBox runat="server" ID="Description" Rows="5" TextMode="MultiLine" Columns="60"></asp:TextBox>
     </td>  
   </tr>
   <tr>  
     <td colspan="2" class="FormSpacerCell"></td> 
   </tr>
   <tr>  
     <td class="FormLabelCell"><asp:Label ID="Label2" runat="server" Text="<%$ Resources:MarketingStrings, Segment_Members %>"></asp:Label>:</td> 
     <td class="FormFieldCell">
         <ComponentArt:ComboBox id="MembershipFilter" TextBoxEnabled="false" runat="server" RunningMode="Callback" Width="250"></ComponentArt:ComboBox>
         <asp:CheckBox runat="server" ID="Exclude" Text="<%$ Resources:MarketingStrings, Segment_Exclude_Member %>" /><br />
         <asp:LinkButton runat="server" ID="AddMember" Text="<%$ Resources:MarketingStrings, Segment_Add_Member %>" CausesValidation="false"></asp:LinkButton> <br />
         <asp:DataList runat="server" ID="MemberList">
            <ItemTemplate>
                <asp:ImageButton runat="server" ID="DeleteButton" OnClientClick="isSubmit = true;" OnCommand="DeleteButton_Command" ImageUrl="../images/delete.png" CommandArgument='<%# Eval("SegmentMemberId")%>' /> <%# GetPrincipalName((Guid)Eval("PrincipalId"))%> <%#GetStatus((bool)Eval("exclude"))%>
            </ItemTemplate>
         </asp:DataList>
     </td> 
   </tr>   
   <tr>  
     <td colspan="2" class="FormSpacerCell"></td> 
   </tr>
   <tr>  
     <td class="FormLabelCell"><asp:Label ID="Label5" runat="server" Text="<%$ Resources:MarketingStrings, Segment_Conditions %>"></asp:Label>:</td> 
     <td class="FormFieldCell">       
        
        <marketing:RulesEditor ID="RulesEditorCtrl" runat="server" />        
        <asp:Label ID="Label3" CssClass="FormFieldDescription" runat="server" Text="<%$ Resources:MarketingStrings,Segment_Conditions_Description %>"></asp:Label>
     </td> 
   </tr>         
  
 </table>
</div>

