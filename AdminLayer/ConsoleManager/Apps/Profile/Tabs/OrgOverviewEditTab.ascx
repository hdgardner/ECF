<%@ Control Language="C#" AutoEventWireup="true" Inherits="Mediachase.Commerce.Manager.Profile.Tabs.OrgOverviewEditTab" Codebehind="OrgOverviewEditTab.ascx.cs" %>
<div id="DataForm">
 <table class="DataForm"> 
   <tr>  
     <td class="FormLabelCell"><asp:Label runat="server" Text="<%$ Resources:SharedStrings, Name %>"></asp:Label>:</td> 
     <td class="FormFieldCell">
        <asp:TextBox runat="server" Width="250" ID="Name"></asp:TextBox><br />
        <asp:RequiredFieldValidator runat="server" ID="NameRequired" ControlToValidate="Name" Display="None" ErrorMessage="<%$ Resources:SharedStrings, Name_Required %>" />
        <ajaxToolkit:ValidatorCalloutExtender runat="Server" ID="NameRequiredE" Width="200" TargetControlID="NameRequired" HighlightCssClass="FormHighlight"/>
     </td> 
   </tr> 
   <tr>  
     <td colspan="2" class="FormSpacerCell"></td> 
   </tr>   
   <tr>  
     <td class="FormLabelCell"><asp:Label ID="Label1" runat="server" Text="<%$ Resources:SharedStrings, Description %>"></asp:Label>:</td> 
     <td class="FormFieldCell">
        <asp:TextBox runat="server" Width="350" TextMode="MultiLine" Rows="4" ID="Description"></asp:TextBox><br />
     </td> 
   </tr> 
   <tr>  
     <td colspan="2" class="FormSpacerCell"></td> 
   </tr>      
  
   <tr>  
     <td class="FormLabelCell"><asp:Label ID="Label3" runat="server" Text="<%$ Resources:SharedStrings, Status %>"></asp:Label>:</td> 
     <td class="FormFieldCell">
        <asp:DropDownList runat="server" id="AccountState" DataTextField="FriendlyName" DataValueField="Name">
            <asp:ListItem Value="1" Text="<%$ Resources:SharedStrings, Pending %>"></asp:ListItem>
            <asp:ListItem Value="2" Text="<%$ Resources:SharedStrings, Active %>"></asp:ListItem>
            <asp:ListItem Value="3" Text="<%$ Resources:SharedStrings, Suspended %>"></asp:ListItem>
        </asp:DropDownList>
     </td> 
   </tr>    
   <tr>  
     <td colspan="2" class="FormSpacerCell"></td> 
   </tr>   
   <tr>  
     <td class="FormLabelCell"><asp:Label ID="Label2" runat="server" Text="<%$ Resources:SharedStrings, Members %>"></asp:Label>:</td> 
     <td class="FormFieldCell">
         <asp:DataList runat="server" ID="OrgMemberList">
            <ItemTemplate>
                <asp:ImageButton runat="server" ID="DeleteButton" Visible="true" OnClientClick="isSubmit = true;" OnCommand="DeleteButton_Command" ImageUrl="../images/delete.png" CommandArgument='<%# Eval("PrincipalId")%>' /> 
                <asp:HyperLink runat="server" NavigateUrl=<%# DataBinder.Eval(Container.DataItem, "PrincipalId", "javascript:CSManagementClient.ChangeView('Profile', 'Account-View','id={0}');") %>><%# Eval("Name") %></asp:HyperLink>
            </ItemTemplate>
         </asp:DataList>
     </td> 
   </tr>    
 </table>
</div>


