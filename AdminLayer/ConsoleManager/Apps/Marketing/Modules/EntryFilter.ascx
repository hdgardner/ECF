<%@ Control Language="C#" AutoEventWireup="true" Inherits="Apps_Marketing_Modules_EntryFilter" Codebehind="EntryFilter.ascx.cs" %>
<ComponentArt:ComboBox ID="EntriesFilter" runat="server" AutoHighlight="false" AutoFilter="true" AutoComplete="true" ItemClientTemplateId="itemTemplate" Width="350" DropDownPageSize="10">
    <ClientTemplates>
        <ComponentArt:ClientTemplate ID="itemTemplate">
            <img src="## DataItem.getProperty('icon') ##" />
            ## DataItem.getProperty('Text') ##</ComponentArt:ClientTemplate>
    </ClientTemplates>
</ComponentArt:ComboBox>
<asp:RequiredFieldValidator ID="RequiredValidator" runat="server" ControlToValidate="EntriesFilter" Display="Dynamic" ErrorMessage="*"></asp:RequiredFieldValidator>