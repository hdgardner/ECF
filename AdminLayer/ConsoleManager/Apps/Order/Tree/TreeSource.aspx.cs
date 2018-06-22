using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Data;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Web.Console.Common;
using Mediachase.Commerce.Orders.Dto;
using Mediachase.Commerce.Orders.Managers;
using Mediachase.Commerce.Shared;
using Mediachase.Cms;
using System.Globalization;
using Mediachase.Web.Console.BaseClasses;

public partial class Apps_Order_Tree_TreeSource : BasePage
{
	private const string ModuleName = "Order";

    public enum TreeListType
    {
		None,
		Root,
		OrderSearch,
        PurchaseOrders,
		PurchaseOrdersByStatus,
		Carts,
		PaymentPlans,

		PaymentMethods,
		ShippingMethods
    }

    /// <summary>
    /// Gets the type of the list.
    /// </summary>
    /// <value>The type of the list.</value>
    public TreeListType ListType
    {
        get
        {
			string nodeType = Request.Form["type"];

			if (String.IsNullOrEmpty(nodeType))
				return TreeListType.Root;

			TreeListType type = TreeListType.None;

			try
			{
				type = (TreeListType)Enum.Parse(typeof(TreeListType), nodeType, true);
			}
			catch
			{
				type = TreeListType.None;
			}

			return type;
        }
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        BindList();
    }

    /// <summary>
    /// Binds the list.
    /// </summary>
    private void BindList()
    {
        switch (ListType)
        {
			//case TreeListType.Root:
			//    BindRoot();
			//    break;
			case TreeListType.PurchaseOrdersByStatus:
				BindPurchaseOrdersByStatus();
				break;
			case TreeListType.PaymentMethods:
				BindPaymentMethods();
				break;
			case TreeListType.ShippingMethods:
				BindShippingMethods();
				break;
        }
    }

    /// <summary>
    /// Binds the root.
    /// </summary>
	private void BindRoot()
	{
		List<JsonTreeNode> nodes = new List<JsonTreeNode>();
		nodes.Add(JsonTreeNode.CreateNode("OrderSearch", String.Empty, "Order Search", ModuleName,
			"OrderSearch-List", String.Empty, TreeListType.OrderSearch.ToString(), true));

		// PurchaseOrders node
		JsonTreeNode poNode = JsonTreeNode.CreateNode("PurchaseOrders", String.Empty, "Purchase Orders", ModuleName, "Orders-List", String.Empty, TreeListType.PurchaseOrders.ToString());
		poNode.icon = Mediachase.Commerce.Shared.CommerceHelper.GetAbsolutePath("~/Apps/Order/images/PurchaseOrders.png");
		poNode.children = new List<JsonTreeNode>();
		poNode.children.Add(JsonTreeNode.CreateNode("PO-TodayOrders", "Today", ModuleName, "Orders-List", "filter=today&class=PurchaseOrder", true));
		poNode.children.Add(JsonTreeNode.CreateNode("PO-WeekOrders", "This Week", ModuleName, "Orders-List", "filter=thisweek&class=PurchaseOrder", true));
		poNode.children.Add(JsonTreeNode.CreateNode("PO-MonthOrders", "This Month", ModuleName, "Orders-List", "filter=thismonth&class=PurchaseOrder", true));
		poNode.children.Add(JsonTreeNode.CreateNode("PO-AllOrders", "All", ModuleName, "Orders-List", "filter=all&class=PurchaseOrder", true));
		nodes.Add(poNode);

		// PurchaseOrdersByStatus node
		JsonTreeNode posNode = JsonTreeNode.CreateNode("PurchaseOrdersByStatus", String.Empty, "Purchase Orders By Status", ModuleName, "Orders-List", String.Empty, TreeListType.PurchaseOrdersByStatus.ToString());
		posNode.children = new List<JsonTreeNode>();

		OrderStatusDto statusDto = OrderStatusManager.GetOrderStatus();

		foreach (OrderStatusDto.OrderStatusRow statusRow in statusDto.OrderStatus.Rows)
			posNode.children.Add(JsonTreeNode.CreateNode("PO-Status-" + statusRow.OrderStatusId, statusRow.Name, ModuleName, "Orders-List", String.Format("status={0}", statusRow.Name), true));
		nodes.Add(posNode);

		// Carts node
		JsonTreeNode cartsNode = JsonTreeNode.CreateNode("Carts", "Carts", ModuleName, "Orders-List", "filter=all&class=ShoppingCart", false);
		cartsNode.children = new List<JsonTreeNode>();
		cartsNode.children.Add(JsonTreeNode.CreateNode("CART-TodayOrders", "Today", ModuleName, "Orders-List", "filter=today&class=ShoppingCart", true));
		cartsNode.children.Add(JsonTreeNode.CreateNode("CART-WeekOrders", "This Week", ModuleName, "Orders-List", "filter=thisweek&class=ShoppingCart", true));
		cartsNode.children.Add(JsonTreeNode.CreateNode("CART-MonthOrders", "This Month", ModuleName, "Orders-List", "filter=thismonth&class=ShoppingCart", true));
		cartsNode.children.Add(JsonTreeNode.CreateNode("CART-AllOrders", "All", ModuleName, "Orders-List", "filter=all&class=ShoppingCart", true));
		nodes.Add(cartsNode);

		// PaymentPlans node
		JsonTreeNode ppNode = JsonTreeNode.CreateNode("PaymentPlans", "Payment Plans (recurring)", ModuleName, "Orders-List", "filter=all&class=PaymentPlan", false);
		ppNode.children = new List<JsonTreeNode>();
		ppNode.children.Add(JsonTreeNode.CreateNode("PP-TodayOrders", "Today", ModuleName, "Orders-List", "filter=today&class=PaymentPlan", true));
		ppNode.children.Add(JsonTreeNode.CreateNode("PP-WeekOrders", "This Week", ModuleName, "Orders-List", "filter=thisweek&class=PaymentPlan", true));
		ppNode.children.Add(JsonTreeNode.CreateNode("PP-MonthOrders", "This Month", ModuleName, "Orders-List", "filter=thismonth&class=PaymentPlan", true));
		ppNode.children.Add(JsonTreeNode.CreateNode("PP-AllOrders", "All", ModuleName, "Orders-List", "filter=all&class=PaymentPlan", true));
		nodes.Add(ppNode);

		WriteArray(nodes);
	}

    /// <summary>
    /// Binds the purchase orders by status.
    /// </summary>
	private void BindPurchaseOrdersByStatus()
	{
		List<JsonTreeNode> nodes = new List<JsonTreeNode>();
		
		// PurchaseOrdersByStatus node
		OrderStatusDto statusDto = OrderStatusManager.GetOrderStatus();

		foreach (OrderStatusDto.OrderStatusRow statusRow in statusDto.OrderStatus.Rows)
			nodes.Add(JsonTreeNode.CreateNode("PO-Status-" + statusRow.OrderStatusId, statusRow.Name, ModuleName, "Orders-List", String.Format("status={0}", statusRow.Name), true));

		WriteArray(nodes);
	}

    /// <summary>
    /// Binds the payment methods.
    /// </summary>
	private void BindPaymentMethods()
	{
        SecurityManager.CheckRolePermission("order:admin:payments:mng:view");

		// add payment gateway languages
		List<JsonTreeNode> nodes = LoadLanguages("PaymentLanguage", "PaymentMethods-List");

		WriteArray(nodes);
	}

    /// <summary>
    /// Binds the shipping methods.
    /// </summary>
	private void BindShippingMethods()
	{
        SecurityManager.CheckRolePermission("order:admin:shipping:methods:mng:view");
        
        // add shipping gateway languages
		List<JsonTreeNode> nodes = LoadLanguages("ShippingMethodLanguage", "ShippingMethodLanguage-List");

		WriteArray(nodes);
	}

    /// <summary>
    /// Loads the languages.
    /// </summary>
    /// <param name="baseNodeId">The base node id.</param>
    /// <param name="viewId">The view id.</param>
    /// <returns></returns>
	private List<JsonTreeNode> LoadLanguages(string baseNodeId, string viewId)
	{
		List<JsonTreeNode> nodes = new List<JsonTreeNode>();

		// add nodes with languages
		using (IDataReader langReader = Language.GetAllLanguages())
		{
			// add nodes with languages
			while (langReader.Read())
			{
				int langId = Int32.Parse(langReader["LangId"].ToString());
				CultureInfo culture = CultureInfo.CreateSpecificCulture(langReader["LangName"].ToString());
				string langName = culture.Name;
				
				nodes.Add(JsonTreeNode.CreateNode(baseNodeId + "-" + langId, culture.DisplayName, ModuleName, viewId, String.Format("lang={0}", langName.ToLower()), true));
			}

            langReader.Close();
		}

		return nodes;
	}

    /// <summary>
    /// Writes the array.
    /// </summary>
    /// <param name="nodes">The nodes.</param>
	private void WriteArray(List<JsonTreeNode> nodes)
    {
        string json = JsonSerializer.Serialize(nodes);
        Response.Write(json);
    }
}