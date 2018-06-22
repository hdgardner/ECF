<%@ Control Language="C#" AutoEventWireup="true" Inherits="Mediachase.Commerce.Manager.Core.Controls.CalendarDatePicker" Codebehind="CalendarDatePicker.ascx.cs" %>
<asp:TextBox runat="server" Width="150" ID="Date"></asp:TextBox>
<asp:Image runat="Server" ID="Image1" ImageAlign="Middle" CssClass="CalendarImage" ImageUrl="~/app_themes/default/images/calendar.png" />
<ajaxToolkit:CalendarExtender ID="calendarButtonExtender" runat="server" TargetControlID="Date" PopupButtonID="Image1" />
<asp:RequiredFieldValidator runat="server" ID="rfvDate" ControlToValidate="Date" Display="None" ErrorMessage="<%$ Resources:SharedStrings, Valid_Date_Required %>" />
<ajaxToolkit:ValidatorCalloutExtender runat="Server" ID="vceDate1" Width="220" TargetControlID="rfvDate" HighlightCssClass="FormHighlight"/>
<asp:CompareValidator id="cvDate" runat="Server" Operator="DataTypeCheck" Type="Date" ErrorMessage="<%$ Resources:SharedStrings, Valid_Date_Format_Required %>" Display="None" ControlToValidate="Date" />
<ajaxToolkit:ValidatorCalloutExtender runat="Server" ID="cveDate2" Width="220" TargetControlID="cvDate" HighlightCssClass="FormHighlight"/>
<asp:RangeValidator id="rvDate" runat="Server" Type="Date" MinimumValue="1752/1/1" MaximumValue="9999/12/31" ErrorMessage="<%$ Resources:SharedStrings, Valid_Date_Required %>" Display="None" ControlToValidate="Date" />
<ajaxToolkit:ValidatorCalloutExtender runat="Server" ID="cveDate3" Width="220" TargetControlID="rvDate" HighlightCssClass="FormHighlight"/>

<asp:TextBox runat="server" Width="60" ID="tbTime"></asp:TextBox>
<ajaxToolkit:MaskedEditExtender ID="timeMaskedExtender" runat="server" Mask="99:99" MaskType="Time" MessageValidatorTip="true" TargetControlID="tbTime"></ajaxToolkit:MaskedEditExtender>
<ajaxToolkit:MaskedEditValidator ID="timeMaskedValidator" runat="server" ControlExtender="timeMaskedExtender" ControlToValidate="tbTime" Display="None" EmptyValueMessage="<%$ Resources:SharedStrings, Time_Required %>" InvalidValueMessage="<%$ Resources:SharedStrings, Valid_Time_Required %>" IsValidEmpty="false"></ajaxToolkit:MaskedEditValidator>
<ajaxToolkit:ValidatorCalloutExtender runat="Server" ID="TimeRequiredE" Width="200" TargetControlID="timeMaskedValidator" HighlightCssClass="FormHighlight"/>