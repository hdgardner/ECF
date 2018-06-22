<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="HelpModule.ascx.cs"
    Inherits="Mediachase.Commerce.Manager.Apps.Dashboard.Modules.HelpModule" %>
<script language="javascript">
    function help_search() {
        var value = document.getElementById('query').value;
        window.open('http://docs.mediachase.com/doku.php?do=search&id=' + value + '+@ecf%3A50', 'help');
    }
</script>
<div class="db-panel-outer">
    <div class="db-panel">
        <div class="explanation">
            <asp:Literal ID="Literal2" runat="server" Text="<%$ Resources:DashboardStrings, Help_In_Separate_Window %>"/><br />
        </div>
        <asp:Literal ID="Literal3" runat="server" Text="<%$ Resources:DashboardStrings, Help_Search_Description %>"/><br />
        <input type="text" name="query" id="query" size="35">
        <input type="submit" onclick="help_search();" name="Action.Search" value="Search Help" class="buttonbar"><br />
        <asp:Literal ID="Literal1" runat="server" Text="<%$ Resources:DashboardStrings, Examples_Campaigns_And_Promotions %>"/>  
        <ul>
            <li>
                <asp:HyperLink runat="server" Target="_blank" 
                    NavigateUrl="http://docs.mediachase.com/doku.php?id=ecf:50:cmenduser:using:mgmtcust:campaigns"
                    Text="<%$ Resources:DashboardStrings, How_To_Discount_Promotion %>"></asp:HyperLink>
            </li>
            <li>
                <asp:HyperLink ID="HyperLink1" runat="server" Target="_blank" 
                    NavigateUrl="http://docs.mediachase.com/doku.php?id=ecf:50:cmenduser:using:mgmtcat:productedit"
                    Text="<%$ Resources:DashboardStrings, How_To_New_Product %>"></asp:HyperLink>
            </li>
            <li>
                <asp:HyperLink ID="HyperLink2" runat="server" Target="_blank" 
                    NavigateUrl="http://docs.mediachase.com/doku.php?id=ecf:50:start:release"
                    Text="<%$ Resources:DashboardStrings, How_To_Release_History %>"></asp:HyperLink>
            </li>
        </ul>
    </div>
</div>