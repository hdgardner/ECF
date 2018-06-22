using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Mediachase.MetaDataPlus.Configurator;
using Mediachase.MetaDataPlus;

namespace Mediachase.Web.Console.Interfaces
{
    /// <summary>
    /// Summary description for IMetaControl
    /// </summary>
    public interface IMetaControl
    {
        /// <summary>
        /// Gets or sets the meta field.
        /// </summary>
        /// <value>The meta field.</value>
        MetaField MetaField { get;set;}
        /// <summary>
        /// Gets or sets the meta object.
        /// </summary>
        /// <value>The meta object.</value>
        MetaObject MetaObject { get;set;}
        /// <summary>
        /// Gets or sets the language code.
        /// </summary>
        /// <value>The language code.</value>
        string LanguageCode { get;set;}
        /// <summary>
        /// Gets or sets the validation group.
        /// </summary>
        /// <value>The validation group.</value>
        string ValidationGroup { get;set;}
        /// <summary>
        /// Updates this instance.
        /// </summary>
        void Update();
    }
}