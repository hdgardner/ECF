using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Collections;
using System.Web;
using System.Security.Permissions;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using Mediachase.Commerce.Orders.Search;

namespace Mediachase.Commerce.Orders.DataSources
{
    /// <summary>
    /// Implements the operations for the order data source.
    /// </summary>
    [AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
    [ParseChildren(true)]
    public class OrderDataSource : DataSourceControl
    {
        #region IDataSource Members

        OrderSearchParameters _Parameters = new OrderSearchParameters();
        /// <summary>
        /// Gets or sets the parameters.
        /// </summary>
        /// <value>The parameters.</value>
        [Category("Data")]
        [Browsable(true)]
        [Bindable(false)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public OrderSearchParameters Parameters
        {
            get
            {
                return _Parameters;
            }
        }

        OrderSearchOptions _Options = new OrderSearchOptions();
        /// <summary>
        /// Gets or sets the options.
        /// </summary>
        /// <value>The options.</value>
        [Category("Data")]
        [Browsable(true)]
        [Bindable(false)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public OrderSearchOptions Options
        {
            get 
            {
                return _Options;
            }
        }

        /// <summary>
        /// Gets the named data source view associated with the data source control.
        /// </summary>
        /// <param name="viewName">The name of the view to retrieve.</param>
        /// <returns>
        /// Returns the named <see cref="T:System.Web.UI.DataSourceView"/> associated with the <see cref="T:System.Web.UI.IDataSource"/>.
        /// </returns>
        protected override DataSourceView GetView(string viewName)
        {
            OrderDataSourceView view = new OrderDataSourceView(this, viewName);
            view.Parameters = this.Parameters;
            view.Options = this.Options;
            return view;
        }

        /// <summary>
        /// Gets a collection of names representing the list of view objects associated with the <see cref="T:System.Web.UI.IDataSource"/> interface.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.ICollection"/> that contains the names of the views associated with the <see cref="T:System.Web.UI.IDataSource"/>.
        /// </returns>
        protected override ICollection GetViewNames()
        {
            ArrayList al = new ArrayList(3);
            al.Add(OrderDataSourceView.PurchaseOrdersViewName);
            al.Add(OrderDataSourceView.CartsViewName);
            al.Add(OrderDataSourceView.PaymentPlansViewName);
            return al as ICollection;
        }

        #endregion

        /// <summary>
        /// Implements operations for the order data source view and inherits the <see cref="DataSourceView"/> class.
        /// </summary>
        public class OrderDataSourceView : DataSourceView
        {
            /// <summary>
            /// Represents the purchase orders view name.
            /// </summary>
            public static string PurchaseOrdersViewName = "PurchaseOrders";
            /// <summary>
            /// Represents the carts view name.
            /// </summary>
            public static string    CartsViewName = "Carts";
            /// <summary>
            /// Represents the payment plans view name.
            /// </summary>
            public static string PaymentPlansViewName = "PaymentPlans";

            /// <summary>
            /// Initializes a new instance of the <see cref="OrderDataSourceView"/> class.
            /// </summary>
            /// <param name="owner">The owner.</param>
            /// <param name="name">The name.</param>
            public OrderDataSourceView(IDataSource owner, string name)
                : base(owner, name)
            {
            }

            OrderSearchParameters _Parameters = new OrderSearchParameters();

            /// <summary>
            /// Gets or sets the parameters.
            /// </summary>
            /// <value>The parameters.</value>
            public OrderSearchParameters Parameters
            {
                get { return _Parameters; }
                set { _Parameters = value; }
            }

            OrderSearchOptions _Options = new OrderSearchOptions();
            /// <summary>
            /// Gets or sets the options.
            /// </summary>
            /// <value>The options.</value>
            public OrderSearchOptions Options
            {
                get { return _Options; }
                set { _Options = value; }
            }

            /// <summary>
            /// Gets a list of data from the underlying data storage.
            /// </summary>
            /// <param name="arguments">A <see cref="T:System.Web.UI.DataSourceSelectArguments"/> that is used to request operations on the data beyond basic data retrieval.</param>
            /// <returns>
            /// An <see cref="T:System.Collections.IEnumerable"/> list of data from the underlying data storage.
            /// </returns>
            protected override IEnumerable ExecuteSelect(DataSourceSelectArguments arguments)
            {
                Options.RecordsToRetrieve = arguments.MaximumRows;
                Options.StartingRecord = arguments.StartRowIndex;

                if (!String.IsNullOrEmpty(arguments.SortExpression))
                    Parameters.OrderByClause = arguments.SortExpression;

                int totalRecords = 0;

                OrderGroup[] orders = null;
                if (base.Name == CartsViewName)
                    orders = OrderContext.Current.FindCarts(this.Parameters, this.Options, out totalRecords);
                else if (base.Name == PaymentPlansViewName)
                    orders = OrderContext.Current.FindPaymentPlans(this.Parameters, this.Options, out totalRecords);
                else
                    orders = OrderContext.Current.FindPurchaseOrders(this.Parameters, this.Options, out totalRecords);

                arguments.TotalRowCount = totalRecords;
                return orders;
            }

            /// <summary>
            /// Gets a value indicating whether the <see cref="T:System.Web.UI.DataSourceView"/> object associated with the current <see cref="T:System.Web.UI.DataSourceControl"/> object supports retrieving the total number of data rows, instead of the data.
            /// </summary>
            /// <value></value>
            /// <returns>true if the operation is supported; otherwise, false. The base class implementation returns false.</returns>
            public override bool CanRetrieveTotalRowCount
            {
                get
                {
                    return true;
                }
            }

            /// <summary>
            /// Gets a value indicating whether the <see cref="T:System.Web.UI.DataSourceView"/> object associated with the current <see cref="T:System.Web.UI.DataSourceControl"/> object supports paging through the data retrieved by the <see cref="M:System.Web.UI.DataSourceView.ExecuteSelect(System.Web.UI.DataSourceSelectArguments)"/> method.
            /// </summary>
            /// <value></value>
            /// <returns>true if the operation is supported; otherwise, false. The base class implementation returns false.</returns>
            public override bool CanPage
            {
                get
                {
                    return true;
                }
            }

            /// <summary>
            /// Gets a value indicating whether the <see cref="T:System.Web.UI.DataSourceView"/> object associated with the current <see cref="T:System.Web.UI.DataSourceControl"/> object supports the <see cref="M:System.Web.UI.DataSourceView.ExecuteDelete(System.Collections.IDictionary,System.Collections.IDictionary)"/> operation.
            /// </summary>
            /// <value></value>
            /// <returns>true if the operation is supported; otherwise, false. The base class implementation returns false.</returns>
            public override bool CanDelete
            {
                get
                {
                    return false;
                }
            }

            /// <summary>
            /// Gets a value indicating whether the <see cref="T:System.Web.UI.DataSourceView"/> object associated with the current <see cref="T:System.Web.UI.DataSourceControl"/> object supports the <see cref="M:System.Web.UI.DataSourceView.ExecuteInsert(System.Collections.IDictionary)"/> operation.
            /// </summary>
            /// <value></value>
            /// <returns>true if the operation is supported; otherwise, false. The base class implementation returns false.</returns>
            public override bool CanInsert
            {
                get
                {
                    return false;
                }
            }

            /// <summary>
            /// Gets a value indicating whether the <see cref="T:System.Web.UI.DataSourceView"/> object associated with the current <see cref="T:System.Web.UI.DataSourceControl"/> object supports the <see cref="M:System.Web.UI.DataSourceView.ExecuteUpdate(System.Collections.IDictionary,System.Collections.IDictionary,System.Collections.IDictionary)"/> operation.
            /// </summary>
            /// <value></value>
            /// <returns>true if the operation is supported; otherwise, false. The default implementation returns false.</returns>
            public override bool CanUpdate
            {
                get
                {
                    return false;
                }
            }
        }
    }
}
