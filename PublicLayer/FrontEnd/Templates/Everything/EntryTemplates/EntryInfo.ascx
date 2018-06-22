<%@ Control Language="C#" ClassName="NodeInfo" AutoEventWireup="true" Inherits="Mediachase.Cms.WebUtility.BaseStoreUserControl, Mediachase.Cms.WebUtility" %>
<%@ Register Src="~/Structure/Base/Controls/Catalog/SharedModules/PriceLineModule.ascx" TagName="PriceLineModule" TagPrefix="catalog" %>
<%@ Register Src="~/Structure/Base/Controls/Cart/SharedModules/BuyButtonModule.ascx" TagName="BuyButtonModule" TagPrefix="cart" %>
<%@ Register Src="Modules/CompareButtonModule.ascx" TagName="CompareButtonModule" TagPrefix="uc1" %>
<%@ Import Namespace="Mediachase.Commerce.Catalog.Dto" %>
<%@ Import Namespace="Mediachase.Commerce.Catalog" %>
<%@ Import Namespace="Mediachase.Commerce.Catalog.Managers" %>
<%@ Import Namespace="Mediachase.Commerce.Catalog.Objects" %>
<%@ Import Namespace="Mediachase.Cms.WebUtility.Commerce" %>

<script runat="server">
    private Entry _CurrentEntry = null;
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        //if(!this.IsPostBack)
            DataBind();
    }

    public Entry Entry
    {
        get
        {
            if (_CurrentEntry == null)
                _CurrentEntry = CatalogContext.Current.GetCatalogEntry(Code, new CatalogEntryResponseGroup(CatalogEntryResponseGroup.ResponseGroup.CatalogEntryFull));
            
            return _CurrentEntry;
        }
    }

    string _Code;
    public string Code
    {
        get
        {
            return _Code;
        }
    }

    string _CatalogName;
    public string CatalogName
    {
        get
        {
            return _CatalogName;
        }
    }

    public override void LoadContext(IDictionary context)
    {
        if (context["Code"] != null)
            _Code = context["Code"].ToString();

        if (context["CatalogName"] != null)
            _CatalogName = context["CatalogName"].ToString();
    }
    
</script>
<table cellpadding="3" cellspacing="0" border="0" id="Table1">
    <tr>
        <td colspan="3"><h1><%#Entry.Name %></h1></td>
    </tr>
	<tr valign="top">
		<td><!-- images -->
            <cms:MetaImage OpenFullImage="true" AlternateText='<%#Entry.ItemAttributes["DisplayName"] %>' ShowThumbImage="false" ID="PrimaryImage" PropertyName="PrimaryImage" DataSource="<%#Entry.ItemAttributes.Images%>" runat="server" />
		</td>
		<td>&nbsp;&nbsp;&nbsp;</td>
		<td style="width: 100%">
			<!-- purchase module -->
			
			<p>
			<%#Entry.ItemAttributes["Description"] %>
			</p>
			
			<br />
			
			<div class="line_separator">
			    <uc1:CompareButtonModule ID="cmpBtnModule1" runat="server" Product='<%# Entry %>' />
			</div>
			
			<br />
			
			<!-- SKU List -->
                <asp:DataGrid DataSource='<%# Entry.Entries.Entry %>' DataKeyField="ID" CssClass="ecf-table" ID="SkuList" style="width: 100%" BorderStyle="Solid"
                    AutoGenerateColumns="False" BorderWidth="1px" cellspacing="0" GridLines="Horizontal"
                    cellpadding="2" AllowSorting="True" PageSize="15" runat="server">
                    <ItemStyle CssClass="ecf-table-item"></ItemStyle>
                    <HeaderStyle CssClass="ecf-table-header"></HeaderStyle>
                    <Columns>
                        <asp:TemplateColumn HeaderText="Description">
                            <HeaderStyle Width="150px"></HeaderStyle>
                            <ItemTemplate>
                                <%# Eval("Name") %><br />
                                <!-- price line module -->
                                <catalog:PriceLineModule id="PriceLineModule1" ListPrice='<%# StoreHelper.GetSalePrice((Entry)Container.DataItem, 1)%>' SalePrice='<%# StoreHelper.GetDiscountPrice((Entry)Container.DataItem, CatalogName)%>' runat="server"></catalog:PriceLineModule>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="" ItemStyle-HorizontalAlign="Right">
                            <HeaderStyle Width="30px"></HeaderStyle>
                            <ItemTemplate>
                                <cart:BuyButtonModule runat="server" Entry='<%# Container.DataItem%>' />
                                <nobr><asp:Label CssClass="ecf-outofstock" Visible="False" runat="server" ID="OutOfStock">Out of stock</asp:Label></nobr>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                    </Columns>
                </asp:DataGrid> 			
		</td>
	</tr>
	<tr>
		<td colspan="2">
		</td>
	</tr>
</table>
