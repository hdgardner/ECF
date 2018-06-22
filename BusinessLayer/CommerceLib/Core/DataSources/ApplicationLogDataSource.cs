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
using Mediachase.Commerce.Core.Log;
using Mediachase.Commerce.Core.Managers;
using Mediachase.Commerce.Core.Dto;

namespace Mediachase.Commerce.Core.DataSources
{
    /// <summary>
    /// Represent application log source to data-bound controls.
    /// </summary>
    [AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
    [ParseChildren(true)]
    public class ApplicationLogDataSource : DataSourceControl
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
        public ApplicationLogParameters Parameters
        {
            get
            {
                return ((ApplicationLogDataSourceView)this.GetView(String.Empty)).Parameters;
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
        public ApplicationLogOptions Options
        {
            get 
            {
                return ((ApplicationLogDataSourceView)this.GetView(String.Empty)).Options;
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
        public ApplicationLogDataMode DataMode
        {
            get
            {
                return ((ApplicationLogDataSourceView)this.GetView(String.Empty)).DataMode;
            }
            set
            {
                ((ApplicationLogDataSourceView)this.GetView(String.Empty)).DataMode = value;
            }
        }

        private ApplicationLogDataSourceView view = null;
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
                view = new ApplicationLogDataSourceView(this, String.Empty);
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
            al.Add(ApplicationLogDataSourceView.DefaultViewName);
            return al as ICollection;
        }

        #endregion

        /// <summary>
        /// Defines the application log data mode.
        /// </summary>
        public enum ApplicationLogDataMode
        {
            /// <summary>
            /// Represents the System log mode
            /// </summary>
            SystemLog,
            /// <summary>
            /// Represents the Application log mode
            /// </summary>
            ApplicationLog
        }

        /// <summary>
        /// Represents the catalog search data source view.
        /// </summary>
        public class ApplicationLogDataSourceView : DataSourceView
        {
            /// <summary>
            /// Represents the default catalog search view name
            /// </summary>
            public static string DefaultViewName = "ApplicationLogView";

            /// <summary>
            /// Initializes a new instance of the <see cref="ApplicationLogDataSourceView"/> class.
            /// </summary>
            /// <param name="owner">The owner.</param>
            /// <param name="name">The name.</param>
            public ApplicationLogDataSourceView(IDataSource owner, string name)
                : base(owner, DefaultViewName)
            {
            }

            ApplicationLogParameters _Parameters = new ApplicationLogParameters();

            /// <summary>
            /// Gets or sets the parameters.
            /// </summary>
            /// <value>The parameters.</value>
            public ApplicationLogParameters Parameters
            {
                get { return _Parameters; }
                set { _Parameters = value; }
            }

            ApplicationLogOptions _Options = new ApplicationLogOptions();
            /// <summary>
            /// Gets or sets the options.
            /// </summary>
            /// <value>The options.</value>
            public ApplicationLogOptions Options
            {
                get { return _Options; }
                set { _Options = value; }
            }

            ApplicationLogDataMode _DataMode = ApplicationLogDataMode.ApplicationLog;

            /// <summary>
            /// Gets or sets the data mode.
            /// </summary>
            /// <value>The data mode.</value>
            public ApplicationLogDataMode DataMode
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

                int totalRecordsCount = 0;
                LogDto dto = new LogDto();

                if (this.DataMode == ApplicationLogDataMode.ApplicationLog)
                {
                    dto = LogManager.GetAppLog(Parameters.SourceKey, Parameters.Operation, Parameters.ObjectType, Parameters.Created, Options.StartingRecord, Options.RecordsToRetrieve, ref totalRecordsCount);
                }
                else
                {
                    dto = LogManager.GetSystemLog(Parameters.Operation, Parameters.ObjectType, Parameters.Created, Options.StartingRecord, Options.RecordsToRetrieve, ref totalRecordsCount);
                }

                if (totalRecordsCount > 0)
                {
                    arguments.TotalRowCount = totalRecordsCount;
                    return dto.ApplicationLog.Rows;
                }
                else
                {
                    arguments.TotalRowCount = 0;
                    return null;
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
