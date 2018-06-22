using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Threading;

using Mediachase.Cms;
/// <summary>
/// Summary description for BaseLocalizableMasterPage
/// </summary>
public class BaseLocalizableMasterPage : MasterPage
{
    #region LanguageId
    private int languageId = -1;
    /// <summary>
    /// Gets the language id.
    /// </summary>
    /// <value>The language id.</value>
    public int LanguageId
    {
        get
        {
            using (IDataReader reader = Language.GetLangByName(Thread.CurrentThread.CurrentCulture.Name))
            {
                if (reader.Read())
                {
                    languageId = (int)reader["LangId"];
                }
                else
                {
                    using (IDataReader reader2 = Language.GetAllLanguages())
                    {
                        while (reader2.Read())
                        {
                            if ((bool)reader2["IsDefault"])
                            {
                                int langId = (int)reader2["LangId"];
                                reader.Close();
                                reader2.Close();
                            }
                        }

                        reader2.Close();
                    }
                }

                reader.Close();
            }
            return languageId;
            //return CMSContext.Current.LanguageId;
        }
    }
    #endregion

    #region LanguageName
    /// <summary>
    /// Gets the name of the language.
    /// </summary>
    /// <value>The name of the language.</value>
    public string LanguageName
    {
        get
        {
            //return Thread.CurrentThread.CurrentCulture.Name;
            using (IDataReader reader = Language.LoadLanguage(LanguageId))
            {
                if (reader.Read())
                {
                    string langName = reader["LangName"].ToString();
                    reader.Close();
                    return langName;
                }

                reader.Close();
            }
            return "en-US";
        }
    }
    #endregion
}
