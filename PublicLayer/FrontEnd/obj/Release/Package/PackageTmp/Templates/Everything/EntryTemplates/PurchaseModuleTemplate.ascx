<%@ Control Language="C#" ClassName="NodeInfo" AutoEventWireup="true" Inherits="Mediachase.Cms.WebUtility.BaseStoreUserControl, Mediachase.Cms.WebUtility" %>
<%@ Register Src="Modules/FlashImageModule.ascx" TagName="FlashImageModule" TagPrefix="uc2" %>
<%@ Register Src="Modules/PurchaseModule.ascx" TagName="PurchaseModule" TagPrefix="uc1" %>
<%@ Register Src="Modules/ImagesModule.ascx" TagName="ImagesModule" TagPrefix="uc1" %>
<%@ Register Src="Modules/DocsModule.ascx" TagName="DocsModule" TagPrefix="uc1" %>
<%@ Register Src="Modules/CompareButtonModule.ascx" TagName="CompareButtonModule" TagPrefix="uc1" %>
<%@ Register Src="~/Structure/Base/Controls/Catalog/SharedModules/PriceModule.ascx" TagName="PriceModule" TagPrefix="catalog" %>
<%@ Register Src="~/Structure/Base/Controls/Cart/SharedModules/BuyButtonModule.ascx" TagName="BuyButtonModule" TagPrefix="cart" %>
<%@ Register TagPrefix="componentart" Namespace="ComponentArt.Web.UI" Assembly="ComponentArt.Web.UI" %>
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

        if (Entry != null)
        {
            SkuInfo.Entry = this.Entry;
            SkuInfo.CatalogName = CatalogName;
        }

        this.Page.Title = Entry.ItemAttributes["DisplayName"].Value[0];

        //if(!this.IsPostBack)
        DataBind();

        CreateControls();
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


    private void CreateControls()
    {
        TabStripTab tab = new TabStripTab();
        tab.Text = RM.GetString("GENERAL_OVERVIEW_LABEL");//"Overview";
        Tabs.Tabs.Add(tab);

        TabStripTab tab2 = new TabStripTab();
        tab2.Text = RM.GetString("GENERAL_SPECIFICATIONS_LABEL");//"Specifications";
        Tabs.Tabs.Add(tab2);


        if (Entry.Assets != null && Entry.Assets.Length > 0)
        {
            TabStripTab tab6 = new TabStripTab();
            tab6.Text = RM.GetString("PRODUCT_DOWNLOADS_HEADER");//Downloads/Files";
            Tabs.Tabs.Add(tab6);
        }
        else
        {
            Product_MultiPage.PageViews.Remove(ProductDownloadsView);
        }

    }
    
</script>
<table cellpadding="3" cellspacing="0" border="0" id="Table1">
    <tr>
        <td colspan="3"><h1><%#Entry.ItemAttributes["DisplayName"] %></h1></td>
    </tr>
	<tr valign="top">
		<td><!-- images -->
            <cms:MetaImage OpenFullImage="true" AlternateText='<%#Entry.ItemAttributes["DisplayName"] %>' ShowThumbImage="false" ID="PrimaryImage" PropertyName="PrimaryImage" DataSource="<%#Entry.ItemAttributes.Images%>" runat="server" />
         <%--   <uc1:ImagesModule GroupName="image" DataSource="<%#Entry.Assets%>" runat="server"></uc1:ImagesModule>--%>
		</td>
		<td>&nbsp;&nbsp;&nbsp;</td>
		<td style="width: 100%">
			<!-- purchase module -->
			<uc1:PurchaseModule ID="SkuInfo" runat="server" />		
			<p><%#Entry.ItemAttributes["Description"] %></p>
			
		</td>
	</tr>
	<tr>
		<td colspan="3">
			<div class="line_separator">
			    <uc1:CompareButtonModule ID="CompareButtonModule1" runat="server" Product='<%# Entry %>' />
			</div>
		</td>
	</tr>
	<tr>
		<td colspan="3">
			<div class="line_separator">
			<%--<cms:ThemedImage ID="Image2" runat="server" Width="1" Height="1" ImageUrl="images/spacer.gif" />--%>
			</div>
		</td>
	</tr>
    <tr>
        <td></td>
        <td colspan="2">
            
        </td>
    </tr>	
</table>

<ComponentArt:TabStrip id="Tabs" MultiPageId="Product_MultiPage" 
	CssClass="ecf-Product-TopGroup"
	DefaultItemLookId="DefaultTabLook"
	DefaultSelectedItemLookId="SelectedTabLook"
	DefaultDisabledItemLookId="DisabledTabLook"
	DefaultGroupTabSpacing="0" 
	EnableViewState="False"
	style="width: 100%" runat="server">
	<ITEMLOOKS>
		<ComponentArt:ItemLook LookId="DefaultTabLook" CssClass="ecf-Product-DefaultTab" HoverCssClass="ecf-Product-DefaultTabHover" LabelPaddingLeft="10" LabelPaddingRight="10" LabelPaddingTop="3" LabelPaddingBottom="3" LeftIconUrl="tab_left_icon.gif" RightIconUrl="tab_right_icon.gif" HoverLeftIconUrl="hover_tab_left_icon.gif" HoverRightIconUrl="hover_tab_right_icon.gif" LeftIconWidth="5" LeftIconHeight="19" RightIconWidth="5" RightIconHeight="19" />
		<ComponentArt:ItemLook LookId="SelectedTabLook" CssClass="ecf-Product-SelectedTab" LabelPaddingLeft="10" LabelPaddingRight="10" LabelPaddingTop="3" LabelPaddingBottom="3" LeftIconUrl="selected_tab_left_icon.gif" RightIconUrl="selected_tab_right_icon.gif" LeftIconWidth="5" LeftIconHeight="19" RightIconWidth="5" RightIconHeight="19" />
		<ComponentArt:ItemLook LookId="DisabledTabLook" CssClass="ecf-Product-DisabledTab" LabelPaddingLeft="10" LabelPaddingRight="10" LabelPaddingTop="3" LabelPaddingBottom="3" LeftIconUrl="tab_left_icon.gif" RightIconUrl="tab_right_icon.gif" LeftIconWidth="5" LeftIconHeight="19" RightIconWidth="5" RightIconHeight="19" />					
	</ITEMLOOKS>
</ComponentArt:TabStrip>

<ComponentArt:MultiPage EnableViewState="False" id="Product_MultiPage" CssClass="ecf-Product-MultiPage" runat="server" SelectedIndex="0" style="width: 100%">
	<componentart:PageView ID="ProductOverviewView" CssClass="ecf-PageContent" >
	</componentart:PageView>
	<componentart:PageView ID="ProductSpecsView" CssClass="ecf-PageContent">

	</componentart:PageView>
	<componentart:PageView ID="ProductDownloadsView" CssClass="ecf-PageContent">
	    <table cellpadding="2" cellspacing="4" border="0">		
			<tr>
				<td>
		            <uc1:DocsModule ID="DocsModule1" GroupName="documents" DataSource="<%#Entry.Assets%>" runat="server"></uc1:DocsModule>
		        </td>
			</tr>	
		</table>    
	</componentart:PageView>
</ComponentArt:MultiPage>
