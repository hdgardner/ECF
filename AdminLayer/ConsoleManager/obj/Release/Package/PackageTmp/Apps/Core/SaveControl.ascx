<%@ Control Language="C#" AutoEventWireup="true" Inherits="Mediachase.Commerce.Manager.Core.SaveControl" Codebehind="SaveControl.ascx.cs" %>
<div class="ButtonContainer">
<asp:Button runat="server" ID="SaveChangesButton" CssClass="button" OnClientClick="isSubmit = true;" Text="<%$ Resources:SharedStrings, OK %>" Width="72" />
<asp:Button ID="CancelButton" runat="server" CssClass="button" Text="<%$ Resources:SharedStrings, Cancel %>" CausesValidation="false" Width="72"/>&nbsp;
<asp:Button ID="DeleteButton" Visible="false" CssClass="button" runat="server" Text="<%$ Resources:SharedStrings, Delete %>" CausesValidation="false" Width="72"/>
</div>