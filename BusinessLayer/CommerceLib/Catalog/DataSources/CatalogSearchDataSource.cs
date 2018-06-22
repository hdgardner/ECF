using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Collections;
using Mediachase.Commerce.Catalog.Search;
using Mediachase.Commerce.Catalog.Managers;
using Mediachase.Commerce.Catalog.Objects;
using Mediachase.Commerce.Catalog;
using System.Web;
using System.Security.Permissions;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using Mediachase.Commerce.Catalog.Dto;

namespace Mediachase.Commerce.Catalog.DataSources
{
    /// <summary>
    /// Represent catalog search data source to data-bound controls.
    /// </summary>
    [AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
    [ParseChildren(true)]
    public class CatalogSearchDataSource : DataSourceControl
    {
        #region IDataSource Members

        /// <summary>
        /// Gets or sets the parameters.
        /// </summary>
        /// <value>The parameters.</value>
        [Category("Data")]
        [Browsable(true)]
        [Bindable(false)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public CatalogSearchParameters Parameters
        {
            get
            {
                return ((CatalogSearchDataSourceView)this.GetView(String.Empty)).Parameters;
            }
        }
            
        /// <summary>
        /// Gets or sets the options.
        /// </summary>
        /// <value>The options.</value>
        [Category("Data")]
        [Browsable(true)]
        [Bindable(false)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public CatalogSearchOptions Options
        {
            get 
            {
                return ((CatalogSearchDataSourceView)this.GetView(String.Empty)).Options;
            }
        }

        /// <summary>
        /// Gets or sets the response group.
        /// </summary>
        /// <value>The response group.</value>
        [Category("Data")]
        [Browsable(true)]
        [Bindable(false)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public CatalogEntryResponseGroup ResponseGroup
        {
            get
            {
                return ((CatalogSearchDataSourceView)this.GetView(String.Empty)).ResponseGroup;
            }
            set
            {
                ((CatalogSearchDataSourceView)this.GetView(String.Empty)).ResponseGroup = value;
            }
        }

        /// <summary>
        /// Gets or sets the data mode.
        /// </summary>
        /// <value>The data mode.</value>
        [Category("Data")]
        [Browsable(true)]
        [Bindable(false)]
        [PersistenceMode(PersistenceMode.Attribute)]
        public CatalogSearchDataMode DataMode
        {
            get
            {
                return ((CatalogSearchDataSourceView)this.GetView(String.Empty)).DataMode;
            }
            set
            {
                ((CatalogSearchDataSourceView)this.GetView(String.Empty)).DataMode = value;
            }
        }


        private CatalogSearchDataSourceView view = null;
        /// <summary>
        /// Gets the named data source view associated with the data source control.
        /// </summary>
        /// <param name="viewName">The name of the view to retrieve.</param>
        /// <returns>
        /// Returns the named <see cref="T:System.Web.UI.DataSourceView"/> associated with the <see cref="T:System.Web.UI.IDataSource"/>.
        /// </returns>
        protected override DataSourceView GetView(string viewName)
        {
            if (null == view)
            {
                view = new CatalogSearchDataSourceView(this, String.Empty);
            }
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
            ArrayList al = new ArrayList(1);
            al.Add(CatalogSearchDataSourceView.DefaultViewName);
            return al as ICollection;
        }

        #endregion

        /// <summary>
        /// Defines the catalog search data mode.
        /// </summary>
        public enum CatalogSearchDataMode
        {
            /// <summary>
            /// Represents the Objects catalog search mode
            /// </summary>
            Objects,
            /// <summary>
            /// Represents the DataSet catalog search mode
            /// </summary>
            DataSet
        }

        /// <summary>
        /// Represents the catalog search data source view.
        /// </summary>
        public class CatalogSearchDataSourceView : DataSourceView
        {
            /// <summary>
            /// Represents the default catalog search view name
            /// </summary>
            public static string DefaultViewName = "CatalogEntriesView";

            /// <summary>
            /// Initializes a new instance of the <see cref="CatalogSearchDataSourceView"/> class.
            /// </summary>
            /// <param name="owner">The owner.</param>
            /// <param name="name">The name.</param>
            public CatalogSearchDataSourceView(IDataSource owner, string name)
                : base(owner, DefaultViewName)
            {
            }

            CatalogSearchParameters _Parameters = new CatalogSearchParameters();

            /// <summary>
            /// Gets or sets the parameters.
            /// </summary>
            /// <value>The parameters.</value>
            public CatalogSearchParameters Parameters
            {
                get { return _Parameters; }
                set { _Parameters = value; }
            }

            CatalogSearchOptions _Options = new CatalogSearchOptions();
            /// <summary>
            /// Gets or sets the options.
            /// </summary>
            /// <value>The options.</value>
            public CatalogSearchOptions Options
            {
                get { return _Options; }
                set { _Options = value; }
            }

            CatalogEntryResponseGroup _ResponseGroup = new CatalogEntryResponseGroup();

            /// <summary>
            /// Gets or sets the response group.
            /// </summary>
            /// <value>The response group.</value>
            public CatalogEntryResponseGroup ResponseGroup
            {
                get { return _ResponseGroup; }
                set { _ResponseGroup = value; }
            }

            CatalogSearchDataMode _DataMode = CatalogSearchDataMode.Objects;

            /// <summary>
            /// Gets or sets the data mode.
            /// </summary>
            /// <value>The data mode.</value>
            public CatalogSearchDataMode DataMode
            {
                get { return _DataMode; }
                set { _DataMode = value; }
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

                if (DataMode == CatalogSearchDataMode.Objects)
                {
                    Entries entries = CatalogContext.Current.FindItems(Parameters, Options, ResponseGroup);

                    arguments.TotalRowCount = entries.TotalResults;
                    return entries.Entry;
                }
                else
                {
                    int totalRecordsCount = 0;
                    CatalogEntryDto entries = CatalogContext.Current.FindItemsDto(Parameters, Options, ref totalRecordsCount, ResponseGroup);

                    if (totalRecordsCount > 0)
                    {
                        arguments.TotalRowCount = totalRecordsCount;
                        return entries.CatalogEntry.Rows;
                    }
                    else
                    {
                        arguments.TotalRowCount = 0;
                        return null;
                    }
                    
                }
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
