<%@ Control Language="C#" ClassName="SkuEntryInfo" Inherits="Mediachase.Cms.WebUtility.BaseStoreUserControl, Mediachase.Cms.WebUtility"%>
<%@ Register Src="~/Structure/Base/Controls/Catalog/SharedModules/AssociationsListModule.ascx"
    TagName="AssociationsListModule" TagPrefix="uc1" %>
<%@ Register Src="~/Structure/Base/Controls/Catalog/SharedModules/PriceModule.ascx" TagName="PriceModule" TagPrefix="catalog" %>
<%@ Register Src="~/Structure/Base/Controls/Cart/SharedModules/BuyButtonModule.ascx" TagName="BuyButtonModule" TagPrefix="cart" %>
<%@ Import Namespace="Mediachase.Commerce.Orders" %>
<%@ Import Namespace="Mediachase.Commerce.Catalog.Dto" %>
<%@ Import Namespace="Mediachase.Commerce.Catalog" %>
<%@ Import Namespace="Mediachase.Commerce.Catalog.Managers" %>
<%@ Import Namespace="Mediachase.Commerce.Catalog.Objects" %>
<%@ Import Namespace="Mediachase.Cms.WebUtility.Commerce" %>

<script runat="server">
    private Entry _CurrentEntry = null;
    public Entry Entry
    {
        get
        {
            return _CurrentEntry;
        }
        set
        {
            _CurrentEntry = value;
        }
    }

    string _CatalogName;
    public string CatalogName
    {
        get
        {
            return _CatalogName;
        }
        set
        {
            _CatalogName = value;
        }
    }

    void BindInventory()
    {
        AvailabilityLabel.Text = StoreHelper.GetInventoryStatus(Entry);       
        //AvailabilityLabel.CssClass = "ecf-inventory-available";      
        
        // Disable the inventory button
        if (StoreHelper.IsInStock(Entry) || StoreHelper.IsAvailableForBackorder(Entry) || StoreHelper.IsAvailableForPreorder(Entry))
            PurchaseLink.Enabled = true;
        else
            PurchaseLink.Enabled = false;
    }

    void PurchaseLink_Click(object sender, EventArgs e)
    {
        // Add item to a cart
        new CartHelper(Cart.DefaultName).AddEntry(this.Entry, Int32.Parse(QuantityList.Text));
        
        // Go through the list of associations
        foreach (RepeaterItem item in AssociationList.Items)
        {
        }

        // Redirect to shopping cart
        Response.Redirect(Mediachase.Cms.NavigationManager.GetUrl("ShoppingCart"));
    }

    void WishlistLink_Click(object sender, EventArgs e)
    {
        // Add item to a cart
        CartHelper helper = new CartHelper(CartHelper.WishListName);

        bool alreadyExists = false;
        
        /*
        // Check if item exists
        foreach (LineItem item in helper.LineItems)
        {
            if (item.CatalogEntryId.Equals(this.Entry.ID))
            {
                alreadyExists = true;
            }
        }
         * */
        
        if(!alreadyExists)
            helper.AddEntry(this.Entry);

        // Go through the list of associations
        foreach (RepeaterItem item in AssociationList.Items)
        {
        }

        // Redirect to shopping cart
        Response.Redirect(Mediachase.Cms.NavigationManager.GetUrl("WishList"));
    }
    
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        BindInventory();
        BindPricing();
    }

    private void BindPricing()
    {
        Price listPrice = StoreHelper.GetSalePrice(Entry, 1);
        Price salePrice = StoreHelper.GetDiscountPrice(Entry, CatalogName);
        // Bind Price
        if (listPrice != null && salePrice != null)
        {
            if (listPrice.Amount > salePrice.Amount)
            {
                ListPriceLabel.Text = listPrice.FormattedPrice;
                PriceLabel.Text = salePrice.FormattedPrice;
                DiscountPrice.Text = CurrencyFormatter.FormatCurrency(listPrice.Amount - salePrice.Amount, salePrice.CurrencyCode);
                DiscountPricePanel.Visible = true;
                ListPricePanel.Visible = true;
            }
            else
            {
                DiscountPricePanel.Visible = false;
                ListPricePanel.Visible = false;
               
                PriceLabel.Text = salePrice.FormattedPrice;
            }
        }
        else
        {
            ListPriceLabel.Text = "0";
            PriceLabel.Text = "0";
            DiscountPricePanel.Visible = false;
            ListPricePanel.Visible = false;
        }
    }    
   
</script>
<!--Start Ready to Buy -->
<div id="readyToBuy">
    <div id="readyTitle">
		<%=RM.GetString("PURCHASE_READY_TO_BUY")%>
    </div>

    <table cellpadding="0" cellspacing="0" id="prodPrices" align="center">
        <tr runat="server" id="ListPricePanel">
            <td><%=RM.GetString("GENERAL_LISTPRICE_LABEL")%>:</td>
            <td class="ecf-price">
                <asp:Label runat="server" ID="ListPriceLabel"></asp:Label></td>
        </tr>
        <tr runat="server" id="DiscountPricePanel">
            <td>
                <strong><%=RM.GetString("GENERAL_DISCOUNT_LABEL")%>:</strong>
            </td>
            <td class="ecf-price">
                <asp:Label runat="server" ID="DiscountPrice"></asp:Label>
            </td>
        </tr>
        <tr class="yourPrice">
            <td><%=RM.GetString("GENERAL_YOUR_PRICE_LABEL")%>:</td>
            <td class="ecf-price">
                <asp:Label runat="server" ID="PriceLabel"></asp:Label></td>
        </tr>
        <tr class="shipping">
            <td colspan="2"><asp:Label runat="server" ID="AvailabilityLabel"></asp:Label></td>
        </tr>
    </table>
<asp:Repeater runat="server" ID="AssociationList" DataSource="<%#Entry.Associations %>">
    <ItemTemplate>
        <div id="prodRecomend"><uc1:AssociationsListModule ID="AssociationsListModule1" Association="<%#Container.DataItem%>" CatalogName="<%#CatalogName %>" runat="server" /></div>    
    </ItemTemplate>
</asp:Repeater>

    <!--Start Qty-->
    <div id="prodQty">
        <%=RM.GetString("GENERAL_QTY_LABEL")%>:
        <asp:DropDownList runat="server" ID="QuantityList">
            <asp:ListItem Value="1">1</asp:ListItem>
            <asp:ListItem Value="2">2</asp:ListItem>
            <asp:ListItem Value="3">3</asp:ListItem>
            <asp:ListItem Value="4">4</asp:ListItem>
            <asp:ListItem Value="5">5</asp:ListItem>
            <asp:ListItem Value="6">6</asp:ListItem>
            <asp:ListItem Value="7">7</asp:ListItem>
            <asp:ListItem Value="8">8</asp:ListItem>
            <asp:ListItem Value="9">9</asp:ListItem>
            <asp:ListItem Value="10">10</asp:ListItem>
        </asp:DropDownList>
        <span><%=RM.GetString("PRODUCT_ITEM")%>:
            <asp:Label runat="server" ID="CodeLabel" Text="<%#Entry.ID%>"></asp:Label></span>
    </div>
    <!--Add to cart-->
    <div id="prodButton">
        <div>
            <asp:LinkButton ID="PurchaseLink" OnClick="PurchaseLink_Click" runat="server"><%#RM.GetString("SKU_ADD_TO_CART")%></asp:LinkButton>
        </div>
    </div> 
    <div id="wishButton">
        <div>
            <asp:LinkButton ID="WishListLink" OnClick="WishlistLink_Click" runat="server">Add to Wishlist</asp:LinkButton>
        </div>
    </div>
    <!-- end: Add to cart-->
    <div id="shipAddresses">
        <div>
            &nbsp;
        </div>
    </div>     
</div>