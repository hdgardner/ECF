<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="Mediachase.Commerce.Manager.Catalog.Tabs.EntryRelationEditTab" Codebehind="EntryRelationEditTab.ascx.cs" %>

<script type="text/javascript">
    function Variations_AddRow()
    {
        var searchbox = ItemsFilter;        
        var selectedItem = null;
        
        try 
        {
            selectedItem = searchbox.getSelectedItem();
        }
        catch(e)
        {
            alert('Must select an item');
            return;
        }
        
        var id = selectedItem.get_value();
        var name = selectedItem.get_text();
        
        // check if item with this id already exists
        for(var index = 0; index < EntryRelationDefaultGrid.Table.getRowCount(); index++)
        {
            var row = EntryRelationDefaultGrid.Table.getRow(index);

            if(row.getMemberAt(1).get_text() == id)
            {
                alert('Record with "'+id+'" id already exists');
                return;
            }
        }

        var row = EntryRelationDefaultGrid.Table.addEmptyRow(); 
        
        EntryRelationDefaultGrid.beginUpdate();
        row.SetValue(1, id, true, true); 
        row.SetValue(2, name, true, true); 
        row.SetValue(3, 1, true, true); 
        row.SetValue(4, 'default', true, true); 
        row.SetValue(5, 0, false, false); 
        EntryRelationDefaultGrid.endUpdate();
        
        return false;
    }
</script>

<div id="DataForm">
    <table class="DataForm">
        <tr>
            <td>
                <table>
                    <tr>
                        <td>
                            <asp:Literal ID="Literal1" runat="server" Text="<%$ Resources:CatalogStrings, Entry_Find_Item %>" />:</td>
                        <td>
                            <ComponentArt:ComboBox ID="ItemsFilter" runat="server" AutoHighlight="false" AutoComplete="true"
                                AutoFilter="true" CssClass="comboBox" HoverCssClass="comboBoxHover" FocusedCssClass="comboBoxHover"
                                TextBoxCssClass="comboTextBox" TextBoxHoverCssClass="comboBoxHover" DropDownCssClass="comboDropDown"
                                ItemCssClass="comboItem" ItemHoverCssClass="comboItemHover" SelectedItemCssClass="comboItemHover"
                                DropHoverImageUrl="App_Themes/Main/images/combobox/drop_hover.gif" DropImageUrl="App_Themes/Main/images/combobox/drop.gif"
                                DropDownPageSize="10" ItemClientTemplateId="itemTemplate" Width="350">
                                <ClientTemplates>
                                    <ComponentArt:ClientTemplate ID="itemTemplate"><img src="## DataItem.getProperty('icon') ##" />
                                        ## DataItem.getProperty('Text') ##</ComponentArt:ClientTemplate>
                                </ClientTemplates>
                            </ComponentArt:ComboBox>
                        </td>
                        <td>
                            <asp:Button runat="server" ID="VariationAddButton" UseSubmitBehavior="false" OnClientClick="Variations_AddRow();return;"
                                Text="<%$ Resources:CatalogStrings, Entry_Add_Item %>" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;</td>
        </tr>
        <tr>
            <td>
                <ComponentArt:Grid AllowEditing="true" RunningMode="Client" AutoFocusSearchBox="false"
                    ShowHeader="false" ShowFooter="false" Width="700" SkinID="Inline" runat="server"
                    ID="EntryRelationDefaultGrid" AutoPostBackOnInsert="false" AutoPostBackOnDelete="false"
                    AutoCallBackOnUpdate="false">
                    <ClientTemplates>
                        <ComponentArt:ClientTemplate ID="CheckHeaderTemplate">
                            <input type="checkbox" name="HeaderCheck" />
                        </ComponentArt:ClientTemplate>
                        <ComponentArt:ClientTemplate ID="HyperlinkTemplate">
                            <a href="javascript:EntryRelationDefaultGrid.edit(EntryRelationDefaultGrid.getItemFromClientId('## DataItem.Arguments ##'));">
                                <img alt="edit" src="../../../App_Themes/Default/Images/edit.gif" /></a> | <a href="javascript:EntryRelationDefaultGrid.deleteItem(EntryRelationDefaultGrid.getItemFromClientId('## DataItem.ClientId ##'))">
                                    <img alt="delete" src="../../../App_Themes/Main/Images/toolbar/delete.gif" /></a>
                        </ComponentArt:ClientTemplate>
                        <ComponentArt:ClientTemplate ID="LoadingFeedbackTemplate">
                            <table cellspacing="0" cellpadding="0" border="0">
                                <tr>
                                    <td style="font-size: 10px;">
                                        Loading...&nbsp;</td>
                                    <td>
                                        <img src="App_Themes/Main/images/grid/spinner.gif" width="16" height="16" border="0"></td>
                                </tr>
                            </table>
                        </ComponentArt:ClientTemplate>
                        <ComponentArt:ClientTemplate ID="EditTemplate">
                            <a href="javascript:EntryRelationDefaultGrid.edit(EntryRelationDefaultGrid.getItemFromClientId('## DataItem.ClientId ##'));">
                                <img alt="edit" src="../../../App_Themes/Default/Images/edit.gif" /></a> | <a href="javascript:EntryRelationDefaultGrid.deleteItem(EntryRelationDefaultGrid.getItemFromClientId('## DataItem.ClientId ##'))">
                                    <img alt="delete" src="../../../App_Themes/Main/Images/toolbar/delete.gif" /></a>
                        </ComponentArt:ClientTemplate>
                        <ComponentArt:ClientTemplate ID="EditCommandTemplate">
                            <a href="javascript:EntryRelationDefaultGrid.editComplete();">Update</a> | <a href="javascript:EntryRelationDefaultGrid.editCancel();">Cancel</a>
                        </ComponentArt:ClientTemplate>
                        <ComponentArt:ClientTemplate ID="InsertCommandTemplate">
                            <a href="javascript:EntryRelationDefaultGrid.editComplete();">Insert</a> | <a href="javascript:EntryRelationDefaultGrid.editCancel();">Cancel</a>
                        </ComponentArt:ClientTemplate>
                    </ClientTemplates>
                </ComponentArt:Grid>
            </td>
        </tr>
    </table>
</div>
