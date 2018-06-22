<%@ Control Language="C#" AutoEventWireup="true" Inherits="Mediachase.Cms.Controls.ToolbarControl" Codebehind="ToolBar.ascx.cs" %>
<%@ Register TagPrefix="ComponentArt" Namespace="ComponentArt.Web.UI" Assembly="ComponentArt.Web.UI" %>
<%@ Register TagPrefix="design" TagName="LangBar" Src="~/Structure/Base/Controls/ToolBar/LanguageBar.ascx" %>
<%@ Register TagPrefix="design" TagName="LangMenu" Src="~/Structure/Base/Controls/ToolBar/LanguageMenu.ascx" %>
<%@ Register TagPrefix="design" TagName="ddTextBox" Src="~/Structure/Base/Controls/ddTextBox/ddTextBox.ascx" %>
<%@ Register TagPrefix="cms" TagName="ddLabel" Src="~/Structure/Base/Controls/ddLabel/ddLabel.ascx" %>
<%@ Register TagPrefix="cms" TagName="DesignView" Src="ToolbarDesignView.ascx" %>
<%@ Register TagPrefix="cms" TagName="PublicView" Src="ToolbarPublicView.ascx" %>
<%--STYLES--%>
<link rel="stylesheet" href='<%=Mediachase.Commerce.Shared.CommerceHelper.GetAbsolutePath("/Styles/Common.css")%>' />
<%--STYLES--%>
<span id="ErrorCtrl" runat="server" class="cms-error-status" visible="false"></span>
<%-- TOOLBAR --%>
<div class="cms-toolbar">
        <table style="width: 100%; height: 40px; text-align: left;" cellpadding="0" cellspacing="0">
            <tr>
                <td style="height: 40px; vertical-align: top; font-family: Verdana; font-size: 11px;">
                <table width="100%" cellpadding="0" cellspacing="0">
                    <tr>
                        <td>&nbsp;&nbsp;<asp:Image ID="Image1" ImageUrl="~/images/logo.gif" BorderWidth="0" runat="server" /></td>
                        <td style="text-align: right; vertical-align: top;">
                        <ComponentArt:Menu ID="LoginMenu" Orientation="Horizontal" CssClass="Toolbar-MenuTopGroup"
                            DefaultGroupCssClass="Toolbar-MenuGroup" DefaultItemLookId="DefaultItemLook" DefaultGroupItemSpacing="1"
                            ImagesBaseUrl="~/Structure/Base/Controls/ToolBar/Images/" EnableViewState="true" ExpandDelay="100"
                            ExpandOnClick="true" runat="server">
                            <Items>
                                <ComponentArt:MenuItem ServerTemplateId="RootTemplate" LookId="TopItemLook">
                                    <ComponentArt:MenuItem NavigateUrl='<%#ResolveUrl("~/Login.aspx") %>' Text="Sign in as Different User"
                                        Description="Login with a different account." ClientTemplateId="DescriptionTemplate">
                                    </ComponentArt:MenuItem>
                                    <ComponentArt:MenuItem NavigateUrl='<%#ResolveUrl("~/Logout.aspx") %>' Text="Sign Out" Description="Logout of this site."
                                        ClientTemplateId="DescriptionTemplate">
                                    </ComponentArt:MenuItem>
                                </ComponentArt:MenuItem>
                                <ComponentArt:MenuItem Text="Feedback" LookId="TopDefaultItemLook">
                                </ComponentArt:MenuItem>
                                <ComponentArt:MenuItem Text="Help" LookId="TopDefaultItemLook">
                                </ComponentArt:MenuItem>
                            </Items>
                            <ItemLooks>
                                <ComponentArt:ItemLook LookId="TopItemLook" RightIconUrl="menu/arrow.gif" CssClass="Toolbar-TopMenuItem"
                                    HoverCssClass="Toolbar-TopMenuItemHover" ExpandedCssClass="Toolbar-TopMenuItemExpanded" LabelPaddingLeft="10"
                                    LabelPaddingRight="5" LabelPaddingTop="2" LabelPaddingBottom="2" />
                                <ComponentArt:ItemLook LookId="DefaultItemLook" CssClass="Toolbar-MenuItem" HoverCssClass="Toolbar-MenuItemHover"
                                    ExpandedCssClass="Toolbar-MenuItemHover" LeftIconWidth="20" LeftIconHeight="18" LabelPaddingLeft="10"
                                    LabelPaddingRight="10" LabelPaddingTop="3" LabelPaddingBottom="4" />
                                <ComponentArt:ItemLook LookId="BreakItem" CssClass="Toolbar-MenuBreak" />
                                <ComponentArt:ItemLook LookId="TopDefaultItemLook" CssClass="Toolbar-TopMenuItem" HoverCssClass="Toolbar-TopMenuItemHover"
                                    LabelPaddingLeft="5" LabelPaddingRight="5" LabelPaddingTop="2" LabelPaddingBottom="2" />
                            </ItemLooks>
                            <ClientTemplates>
                                <ComponentArt:ClientTemplate ID="DescriptionTemplate">
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
                                <ComponentArt:NavigationCustomTemplate ID="RootTemplate">
                                    <Template>
                                        Welcome
                                        <%# Page.User.Identity.Name%>
                                    </Template>
                                </ComponentArt:NavigationCustomTemplate>
                                <ComponentArt:NavigationCustomTemplate ID="DescriptionServerTemplate">
                                    <Template>
                                        Welcome
                                        <%# DataBinder.Eval(Container.DataItem, "Text")%>
                                    </Template>
                                </ComponentArt:NavigationCustomTemplate>
                            </ServerTemplates>
                        </ComponentArt:Menu><br />
                        Version: <%=Mediachase.Commerce.FrameworkContext.ProductVersionDesc%>        
                        </td>
                    </tr>
                </table>
                </td>
            </tr>
        </table>
                        
<div id="RibbonContainer">
    <div id="TopContainer">
        <div id="TabStripContainer">
            <table width="100%" cellpadding="0" cellspacing="0">
                <tr>
                    <td align="left">
                        <ComponentArt:TabStrip ID="Tabs" runat="server" ImagesBaseUrl="~/Structure/Base/Controls/Toolbar/Images/TabStrip/"
                            DefaultGroupCssClass="TabStrip" DefaultItemLookId="TabLook" DefaultSelectedItemLookId="SelectedTabLook"
                            MultiPageId="TopToolBar" AutoPostBackOnSelect="false">
                            <ClientEvents>
                                <TabSelect EventHandler="CSToolbarClient.OnTabChanged" />
                            </ClientEvents>
                            <ItemLooks>
                                <ComponentArt:ItemLook LookId="TabLook" CssClass="Tab" HoverCssClass="TabHover" LabelPaddingTop="3"
                                    LabelPaddingBottom="5" LeftIconWidth="15" LeftIconHeight="23" LeftIconUrl="Left.gif"
                                    HoverLeftIconUrl="LeftHover.png" RightIconWidth="15" RightIconHeight="23" RightIconUrl="Right.gif"
                                    HoverRightIconUrl="RightHover.png" />
                                <ComponentArt:ItemLook LookId="SelectedTabLook" CssClass="SelectedTab" HoverCssClass="SelectedTabHover"
                                    LabelPaddingTop="3" LabelPaddingBottom="5" LeftIconWidth="15" LeftIconHeight="23"
                                    LeftIconUrl="LeftSelected.png" HoverLeftIconUrl="LeftSelectedHover.png" RightIconWidth="15"
                                    RightIconHeight="23" RightIconUrl="RightSelected.png" HoverRightIconUrl="RightSelectedHover.png" />
                            </ItemLooks>
                            <Tabs>
                                <ComponentArt:TabStripTab ID="PublicView" Value="PublicView" Text="Public View">
                                </ComponentArt:TabStripTab>
                                <ComponentArt:TabStripTab ID="DesignView" Value="DesignView" Text="Design View">
                                </ComponentArt:TabStripTab>
                                <ComponentArt:TabStripTab ID="ManagementView" Value="ManagementView" Text="Management View">
                                </ComponentArt:TabStripTab>
                            </Tabs>
                        </ComponentArt:TabStrip>
                    </td>
                    <td style="text-align: right; vertical-align: top;">
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <!-- Container for MultiPage / ToolBar controls: The bulk of the Ribbon -->
    <div id="ToolBarContainer">
        <!-- Left edge of the toolbar group -->
        <div id="ToolBarMid" style="text-align: left">
            <ComponentArt:MultiPage RenderSelectedPageOnly="true" ID="TopToolBar" runat="server">
                <ComponentArt:PageView ID="ToolBarPublicView">
                    <cms:PublicView ID="DesignView1" runat="server"></cms:PublicView>
                </ComponentArt:PageView>
                <ComponentArt:PageView ID="ToolBarDesignView">
                    <cms:DesignView runat="server"></cms:DesignView>
                </ComponentArt:PageView>
                <ComponentArt:PageView ID="ToolBarPageLayout">
                </ComponentArt:PageView>
            </ComponentArt:MultiPage>
        </div>
        <!-- Right edge of the toolbar group -->
        <div id="ToolBarRight">
        </div>
    </div>
</div>
</div>
<%-- /TOOLBAR --%>
