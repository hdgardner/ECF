<%@ Control Language="C#" AutoEventWireup="true" Inherits="Templates_Everything_Entry_Modules_CompareProductModule"
    CodeBehind="CompareProductModule.ascx.cs" %>
<%@ Import Namespace="Mediachase.Commerce.Catalog.Objects" %>
<%@ Import Namespace="Mediachase.Cms.Util" %>


<asp:HiddenField ID="hfCurrentComparisonGroup" runat="server" />
<div id="ComparisonContainer">
</div>
<div id="entry-box">
    <div class="header">
        <div class="text">
            Compare Products
        </div>
        <br class="clearfloat" />
    </div>
    <br class="clearfloat" />
    <div class="desc">
        <asp:UpdatePanel runat="server" ID="upCompare" UpdateMode="Conditional">
            <ContentTemplate>
                <asp:HiddenField runat="server" ID="hfRefreshCompareList" />
                <asp:Repeater runat="server" ID="CompareGroupsRepeater" OnItemCreated="CompareGroupsRepeater_ItemCreated" >
                    <ItemTemplate>
                        <div id="CompareProductGroup">
                            <asp:Label ID="lblCompareGroup" Text='<%# DataBinder.Eval(Container.DataItem, "MetaClassFriendlyName") %>'
                                runat="server" CssClass="ecf-compareproduct-groupheader"></asp:Label>
                            <asp:Repeater runat="server" ID="CompareEntriesRepeater" DataSource='<%# DataBinder.Eval(Container.DataItem, "Entries") %>'
                                OnItemCreated="CompareEntriesRepeater_ItemCreated">
                                <ItemTemplate>
                                    <div class="row">
                                        <div class="image">
                                            <!-- product image-->
                                            <asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl='<%# StoreHelper.GetEntryUrl((Entry)Container.DataItem)%>'>
                                                <cms:MetaImage runat="server" OpenFullImage="false" DataSource='<%#((Entry)Container.DataItem).ItemAttributes.Images%>'
                                                    Width="47" ShowThumbImage="true" ID="PrimaryImage" PropertyName="PrimaryImage" /></asp:HyperLink>
                                        </div>
                                        <div class="desc">
                                            <div class="brand-name">
                                                <asp:ImageButton ImageUrl='<%#CommonHelper.GetImageUrl("images/list_remove_btn.gif", this.Page) %>'
                                                    runat="server" ID="ibRemove" AlternateText='<%# RM.GetString("COMPAREPRODUCTSMODULE_REMOVE") %>'
                                                    ToolTip='<%# RM.GetString("COMPAREPRODUCTSMODULE_REMOVE") %>' CausesValidation="false"
                                                    CssClass="ecf-compareproduct_removebutton" />
                                                <%# ((Entry)Container.DataItem).ItemAttributes["Brand"]%>
                                            </div>
                                            <div class="link">
                                                <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl='<%# StoreHelper.GetEntryUrl((Entry)Container.DataItem)%>'
                                                    Text='<%#StoreHelper.GetEntryDisplayName((Entry)Container.DataItem)%>' CssClass="ecf-compareproduct-itemlink" />
                                            </div>
                                        </div>
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>
                            <div style="text-align: right; padding-right: 8px; padding-bottom: 10px; padding-top: 5px;">
                                <asp:Button runat="server" Text="Compare" ID="btnCompare" CssClass="ecf-compareproduct-buttoncompare"/>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
                <asp:Label runat="server" ID="lblInfo" CssClass="ecf-compareproduct-info" />
                <asp:LinkButton OnClick="ClearCompareButton_Click" ID="ClearCompareButton" runat="server"
                    CssClass="ecf-compareproduct-clear">Clear All</asp:LinkButton>
                <%//=RM.GetString("COMPAREPRODUCTSMODULE_CLEAR")%>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</div>
