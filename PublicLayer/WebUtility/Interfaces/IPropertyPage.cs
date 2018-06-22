using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

/// <summary>
/// Summary description for IPropertyPage
/// </summary>
public interface IPropertyPage
{
    /// <summary>
    /// Gets the title.
    /// </summary>
    /// <value>The title.</value>
    string Title { get;}
    /// <summary>
    /// Gets the description.
    /// </summary>
    /// <value>The description.</value>
    string Description { get;}
    /// <summary>
    /// Loads the specified node uid.
    /// </summary>
    /// <param name="NodeUid">The node uid.</param>
    /// <param name="ControlUid">The control uid.</param>
    void Load(string NodeUid, string ControlUid);
    /// <summary>
    /// Saves the specified node uid.
    /// </summary>
    /// <param name="NodeUid">The node uid.</param>
    /// <param name="ControlUid">The control uid.</param>
    void Save(string NodeUid, string ControlUid);
}
