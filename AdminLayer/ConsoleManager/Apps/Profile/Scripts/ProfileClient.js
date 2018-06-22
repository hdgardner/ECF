// JScript File
function Mediachase_ProfileClient()
{
    // Properties
    
    // Method Mappings   
    this.NewAccount = function(source)
    {
        CSManagementClient.ChangeView('Profile', 'Account-View');
    };
    
    this.NewOrganization = function(source)
    {
        CSManagementClient.ChangeView('Profile', 'Organization-View');
    };
    
    // Roles
    this.NewRole = function()
    {
        var type = CSManagementClient.QueryString("");
        CSManagementClient.ChangeView('Profile', 'Role-Edit', '');
    };
    
    this.EditRole = function(roleId)
    {
        CSManagementClient.ChangeView('Profile', 'Role-Edit', 'RoleId='+encodeURI(roleId));
    };
    
    this.EditRole2 = function(params)
    {
        var roleId = '';
        try
        {
            var cmdObj = Sys.Serialization.JavaScriptSerializer.deserialize(params);
            roleId = cmdObj.CommandArguments.Name;
        }
        catch(e)
        {
            alert('A problem occured with retrieving parameters for method EditRole2');
            return;
        }
        this.EditRole(roleId);
    };
};

var CSProfileClient = new Mediachase_ProfileClient();

