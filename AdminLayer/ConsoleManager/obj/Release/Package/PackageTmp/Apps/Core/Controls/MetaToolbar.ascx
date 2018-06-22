<%@ Control Language="C#" AutoEventWireup="true" Inherits="Mediachase.Commerce.Manager.Core.Controls.MetaToolbar" Codebehind="MetaToolbar.ascx.cs" %>
<%@ Register Namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" TagPrefix="mc" %>
<script type="text/javascript">
function Toolbar_GridHasItemsSelected(params)
{
    var cmdObj = null;
    var gridId = '';
    try
    {
        cmdObj = Sys.Serialization.JavaScriptSerializer.deserialize(params);
        gridId = cmdObj.CommandArguments.GridClientId;
    }
    catch(e)
    {
        alert('A problem occured with retrieving GridId');
        return;
    }
    
    var grid = null;
    try
    {
        grid = $find(gridId);
    }
    catch(e) {  }
    
    if(grid == null)
    {
        alert('Grid not found!');
        return false;
    }
    
    return CSManagementClient.ListHasItemsSelected2(grid);
}

function Toolbar_GetSelectedGridItems(params)
{
    var cmdObj = null;
    var gridId = '';
    try
    {
        cmdObj = Sys.Serialization.JavaScriptSerializer.deserialize(params);
        gridId = cmdObj.CommandArguments.GridClientId;
    }
    catch(e)
    {
        alert('A problem occured with retrieving GridId');
        return;
    }
    
    var grid = null;
    try
    {
        grid = $find(gridId);
    }
    catch(e) {  }
    
    if(grid == null)
    {
        alert('Grid not found!');
        return false;
    }
    
    return CSManagementClient.GetSelectedGridItems(grid);
}
</script>
<div id="toolbarContainer" style="height: 24px">
	<mc:JsToolbar runat="server" ID="MainToolbar" />
</div>