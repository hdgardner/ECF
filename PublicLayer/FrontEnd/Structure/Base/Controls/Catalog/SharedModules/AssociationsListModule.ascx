<%@ Control Language="C#" AutoEventWireup="true" Inherits="Structure_Base_Controls_Catalog_SharedModules_AssociationsListModule" Codebehind="AssociationsListModule.ascx.cs" %>
<%@ Register Src="~/Structure/Base/Controls/Catalog/SharedModules/PriceModule.ascx" TagName="PriceModule" TagPrefix="catalog" %>
<%@ Import Namespace="Mediachase.Commerce.Catalog.Objects" %>
<%@ Import Namespace="Mediachase.Cms.WebUtility.Commerce" %>
<span><asp:Literal runat="server" ID="AssociationHeader"></asp:Literal></span>
  <asp:GridView AutoGenerateColumns="false" cellpadding="0" BorderWidth="0" style="width: 100%" cellspacing="0"
            ShowHeader="false" ID="EntryList" BorderStyle="None" GridLines="None" runat="server" SkinID="SimpleGrid">
    <Columns>
        <asp:TemplateField>
            <ItemTemplate>
            <asp:CheckBox ID="chk" runat="server" /> 
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField>
            <ItemTemplate>
                <cms:MetaImage runat="server" OpenFullImage="true" Width="30" ShowThumbImage="false" ID="PrimaryImage" PropertyName="PrimaryImage" DataSource='<%#((Entry)Eval("Entry")).ItemAttributes.Images%>' />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField>
            <ItemTemplate>
                <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl='<%# StoreHelper.GetEntryUrl((Entry)Eval("Entry"))%>'><%#((Entry)Eval("Entry")).ItemAttributes["DisplayName"]%></asp:HyperLink>              
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField>
            <ItemTemplate>
                <asp:Label ID="PriceLabel" Cssclass="ecf-price" Text='<%# StoreHelper.GetDiscountPrice((Entry)Eval("Entry"), CatalogName).FormattedPrice%>' runat="server"></asp:Label>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>
