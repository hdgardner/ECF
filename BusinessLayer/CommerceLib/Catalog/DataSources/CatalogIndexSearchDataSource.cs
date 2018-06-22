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
    public class CatalogIndexSearchDataSource : DataSourceControl
    {
        #region IDataSource Members
        /// <summary>
        /// Gets or sets the total results.
        /// </summary>
        /// <value>The total results.</value>
        [Category("Data")]
        [Browsable(false)]
        [Bindable(false)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public int TotalResults
        {
            get
            {
                return ((CatalogSearchDataSourceView)this.GetView(String.Empty)).TotalResults;
            }
            set
            {
                ((CatalogSearchDataSourceView)this.GetView(String.Empty)).TotalResults = value;
            }
        }

        /// <summary>
        /// Gets or sets the catalog entries.
        /// </summary>
        /// <value>The catalog entries.</value>
        [Category("Data")]
        [Browsable(false)]
        [Bindable(false)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public Entries CatalogEntries
        {
            get 
            {
                return ((CatalogSearchDataSourceView)this.GetView(String.Empty)).CatalogEntries;
            }
            set
            {
                ((CatalogSearchDataSourceView)this.GetView(String.Empty)).CatalogEntries = value;
            }
        }

        /// <summary>
        /// Gets or sets the catalog entries.
        /// </summary>
        /// <value>The catalog entries.</value>
        [Category("Data")]
        [Browsable(false)]
        [Bindable(false)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public CatalogEntryDto CatalogEntriesDto
        {
            get
            {
                return ((CatalogSearchDataSourceView)this.GetView(String.Empty)).CatalogEntriesDto;
            }
            set
            {
                ((CatalogSearchDataSourceView)this.GetView(String.Empty)).CatalogEntriesDto = value;
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

            Entries _CatalogEntries = new Entries();

            /// <summary>
            /// Gets or sets the catalog entries.
            /// </summary>
            /// <value>The catalog entries.</value>
            public Entries CatalogEntries
            {
                get { return _CatalogEntries; }
                set { _CatalogEntries = value; }
            }

            CatalogEntryDto _CatalogEntriesDto = null;
            /// <summary>
            /// Gets or sets the catalog entries.
            /// </summary>
            /// <value>The catalog entries.</value>
            public CatalogEntryDto CatalogEntriesDto
            {
                get { return _CatalogEntriesDto; }
                set { _CatalogEntriesDto = value; }
            }


            int _TotalResults = 0;
            /// <summary>
            /// Gets or sets the total results.
            /// </summary>
            /// <value>The total results.</value>
            public int TotalResults
            {
                get { return _TotalResults; }
                set { _TotalResults = value; }
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
                arguments.TotalRowCount = TotalResults;
                if (DataMode == CatalogSearchDataMode.Objects)
                {
                    return CatalogEntries.Entry;
                    //Entries entries = CatalogContext.Current.GetCatalogEntries(CatalogEntries, CacheResults, CacheTimeout, ResponseGroup);
                    //return entries.Entry;
                }
                else
                {
                    //CatalogEntryDto entries = CatalogContext.Current.GetCatalogEntriesDto(CatalogEntries, CacheResults, CacheTimeout, ResponseGroup);

                    if (CatalogEntriesDto != null && CatalogEntriesDto.CatalogEntry.Count > 0)
                    {
                        return CatalogEntriesDto.CatalogEntry.Rows;
                    }
                    else
                    {
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
