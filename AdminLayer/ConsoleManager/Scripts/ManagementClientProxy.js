var CSManagementClient = null;
try
{
    CSManagementClient = top.GetManagementClient();
    CSManagementClient.IsPageDirty = false;
}
catch(ex)
{
}