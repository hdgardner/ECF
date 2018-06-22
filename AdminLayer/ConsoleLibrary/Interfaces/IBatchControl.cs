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
    public interface IBatchUpdateControl
    {
        /// <summary>
        /// Gets or sets the flag is meta field.
        /// </summary>
        /// <value>The flag is meta field.</value>
        bool IsMetaField { get;set;}
        /// <summary>
        /// Gets or sets the field name.
        /// </summary>
        /// <value>The field name.</value>
        string FieldName { get;set;}
        /// <summary>
        /// Gets or sets the language code.
        /// </summary>
        /// <value>The language code.</value>
        string LanguageCode { get;set;}
        /// <summary>
        /// Gets or sets the catalog node ID.
        /// </summary>
        /// <value>The catalog node ID.</value>
        int CatalogNodeId { get; set; }
        /// <summary>
        /// Updates this instance.
        /// </summary>
        void Update();
    }
}