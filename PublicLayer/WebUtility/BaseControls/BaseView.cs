using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Mediachase.Cms;

namespace Mediachase.Web.UI
{
    /// <summary>
    /// Summary description for BaseView
    /// </summary>
    public class BaseView : Mediachase.Web.UI.BaseLocalizableControl
    {

        #region ObjectId 
        /// <summary>
        /// Gets the object id.
        /// </summary>
        /// <value>The object id.</value>
        public int ObjectId
        {
            get 
            {
                if (CMSContext.Current.QueryString.Contains("ObjectId"))
                {
                    return int.Parse(CMSContext.Current.QueryString["ObjectId"].ToString());
                }
                return -1;
            }
        }
        #endregion

        #region GetValue()
        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="FieldName">Name of the field.</param>
        /// <returns></returns>
        protected object GetValue(string FieldName)
        {
            //using (IDataReader reader = DynamicObject.VersionLocalGetByDynObjectIdAndLangId(ObjectId, LanguageId))
            //{
            //    if (reader.Read())
            //    {
            //        //SET METADATA LANGUAGE
            //        MetaDataContext.Current.UseCurrentIUCulture = false;
            //        MetaDataContext.Current.Language = LanguageName;
            //        //LOAD METAOBJECT
            //        MetaObject obj = MetaObject.Load(int.Parse(reader["LocalVersionId"].ToString()), int.Parse(reader["MetaClassId"].ToString()));
            //        //TRY READ FIELD FROM METAOBJECT
            //        if (obj != null)
            //        {
            //            if(obj[FieldName] != null)
            //            {
            //                return obj[FieldName];
            //            }
            //        }
            //        //TRY READ FIELD FROM DATAREADER
            //        try
            //        {
            //            return reader[FieldName];
            //        }
            //        catch
            //        {
            //            return string.Empty;
            //        }
            //    }
            //}
            return string.Empty;
        }
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseView"/> class.
        /// </summary>
        public BaseView()
        {
        }
    }
}