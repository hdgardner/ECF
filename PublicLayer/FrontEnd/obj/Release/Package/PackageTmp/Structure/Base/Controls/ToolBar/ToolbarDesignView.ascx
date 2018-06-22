<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="Mediachase.Cms.Controls.ToolbarDesignView" Codebehind="ToolbarDesignView.ascx.cs" %>
<%@ Register TagPrefix="ComponentArt" Namespace="ComponentArt.Web.UI" Assembly="ComponentArt.Web.UI" %>
<%@ Register TagPrefix="cms" TagName="PageTemplate" Src="~/Structure/Base/Controls/ToolBar/PageTemplate.ascx" %>
<ComponentArt:ToolBar ID="TopToolbar" runat="server" ImagesBaseUrl="~/Structure/Base/Controls/Toolbar/Images/"
    ItemSpacing="1" DefaultItemCssClass="admin-toolbar-item" DefaultItemHoverCssClass="admin-toolbar-item-hover"
    DefaultItemCheckedCssClass="admin-toolbar-item-checked" DefaultItemImageWidth="24"
    DefaultItemImageHeight="24" DefaultItemTextImageSpacing="2" DefaultItemTextImageRelation="ImageBeforeText"
    AutoPostBackOnSelect="true" AutoPostBackOnCheckChanged="false" Orientation="Horizontal">
    <ClientEvents>
        <ItemCheckChange EventHandler="CSToolbarClient.OnToolbarItemChecked" />
    </ClientEvents>
    <Items>
        <ComponentArt:ToolBarItem Value="Toolbox" ID="ToolBarItem1" Text="" ToolTip="Displays controls toolbox"
            ItemType="ToggleCheck" ImageUrl="icons/toolbox.png" HoverImageUrl="icons/toolbox-on.png"
            DisabledImageUrl="icons/toolbox-off.png" />
        <ComponentArt:ToolBarItem ItemType="Separator" ImageUrl="icons/break.gif" ImageHeight="24"
            ImageWidth="2" />
        <ComponentArt:ToolBarItem Text="" ToolTip="Saves current page as a draft" ClientSideCommand="RunCommand('SaveDraft')"
            ItemType="Command" ImageUrl="icons/draft.png" HoverImageUrl="icons/draft-on.png"
            DisabledImageUrl="icons/draft-off.png"></ComponentArt:ToolBarItem>
        <ComponentArt:ToolBarItem Text="" ToolTip="Publishes the current page" ClientSideCommand="RunCommand('Publish')"
            ItemType="Command" ImageUrl="icons/publish.png" HoverImageUrl="icons/publish-on.png"
            DisabledImageUrl="icons/publish-off.png" />
        <ComponentArt:ToolBarItem Value="Cancel" ID="CancelItem" ClientSideCommand="CSToolbarClient.SwitchDesignMode('false');RunCommand('Cancel');"
            Text="" ToolTip="Cancel the pending changes" ImageUrl="icons/cancel.png" />
        <ComponentArt:ToolBarItem ItemType="Separator" ImageUrl="icons/break.gif" ImageHeight="24"
            ImageWidth="2" />
        <ComponentArt:ToolBarItem HoverCssClass="" ServerTemplateId="LanguageTemplateId"
            ImageHeight="24" ImageWidth="2" />
        <ComponentArt:ToolBarItem ItemType="Separator" ImageUrl="icons/break.gif" ImageHeight="24"
            ImageWidth="2" />
        <ComponentArt:ToolBarItem HoverCssClass="" ServerTemplateId="PageTemplateId" AutoPostBackOnSelect="false"
            ImageHeight="24" ImageWidth="2" />
    </Items>
    <ClientTemplates>
        <ComponentArt:ClientTemplate ID="MyDescriptionTemplate">
            <table cellpadding="0" cellspacing="0">
                <tr>
                    <td>
                        <b>## DataItem.GetProperty('Text') ##</b></td>
                </tr>
                <tr>
                    <td>
                        ## DataItem.GetProperty('Description') ##</td>
                </tr>
            </table>
        </ComponentArt:ClientTemplate>
    </ClientTemplates>
    <ServerTemplates>
        <ComponentArt:ToolBarCustomTemplate ID="LanguageTemplateId">
            <Template>
                <table cellpadding="0" cellspacing="0">
                    <tr valign="middle">
                        <td valign="middle">
                            <asp:Image ImageUrl='<%#Mediachase.Cms.Util.CommonHelper.GetFlagIcon(System.Threading.Thread.CurrentThread.CurrentCulture)%>'
                                runat="server" ID="imgLang" Width="18" Height="12" BorderWidth="0" />
                        </td>
                    </tr>
                </table>
            </Template>
        </ComponentArt:ToolBarCustomTemplate>
        <ComponentArt:ToolBarCustomTemplate ID="PageTemplateId">
            <Template>
                <cms:PageTemplate runat="server" />
            </Template>
        </ComponentArt:ToolBarCustomTemplate>
    </ServerTemplates>
</ComponentArt:ToolBar>
