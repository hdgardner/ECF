using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections;

using Mediachase.Cms;



namespace Mediachase.Web.UI
{
    /// <summary>
    /// Summary description for BaseList
    /// </summary>
    public class BaseList : Mediachase.Web.UI.BaseLocalizableControl
    {
        #region MetaClassId
        private int metaClassId;
        /// <summary>
        /// Gets or sets the meta class id.
        /// </summary>
        /// <value>The meta class id.</value>
        protected int MetaClassId
        {
            get
            {
                return metaClassId;
            }
            set
            {
                metaClassId = value;
            }
        }
        #endregion

        #region DataSource()
        /// <summary>
        /// Gets the data source.
        /// </summary>
        /// <value>The data source.</value>
        protected object DataSource
        {
            get
            {
                DataTable news = new DataTable();//DynamicObject.ObjectGetListByMetaClassId_DT(MetaClassId, LanguageId);
                ArrayList AlreadyIn = new ArrayList();
                foreach (DataRow row in news.Rows)
                {
                    if (!AlreadyIn.Contains(row["DynObjectId"]))
                    {
                        AlreadyIn.Add(row["DynObjectId"]);
                    }
                    else
                    {
                        row.Delete();
                    }
                }
                return news;
            }
        }
        #endregion
    }
}