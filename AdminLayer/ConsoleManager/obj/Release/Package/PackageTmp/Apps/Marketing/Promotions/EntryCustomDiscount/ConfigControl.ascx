<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ConfigControl.ascx.cs"
    Inherits="Apps_Marketing_Promotions_EntryCustomDiscount" %>
<%@ Register TagPrefix="ibn" Assembly="Mediachase.Ibn.Web.UI.WebControls" Namespace="Mediachase.Ibn.Web.UI.WebControls" %>
<div id="DataForm">
    <asp:HiddenField runat="server" ID="SelectedEntries" />
    <table class="DataForm"> <!--style="table-layout: fixed; width: 100%;">-->
		<tr>
			<td style="width: 250px;"></td>
			<td style="width: 100%;"></td>
		</tr>
        <tr>
            <td colspan="2" class="FormFieldCell">
                <b><i><asp:Label runat="server" ID="Description"></asp:Label></i></b><br /><br />
            </td>
        </tr>
        <tr>
            <td colspan="2" class="FormSpacerCell">
            </td>
        </tr>
        <tr>
            <td colspan="2" class="FormFieldCell">
                Use the condition below to restrict how the promotion will be applied.
            </td>
        </tr>
        <tr>
            <td class="FormLabelCell" colspan="2"><asp:Label ID="Label1" runat="server" Text="Purchase Condition"></asp:Label>:</td>
        </tr>        
        <tr>
            <td colspan="2" class="FormFieldCell" style="width: 100%;">          
                <asp:UpdateProgress runat="server" ID="UpdateProgress1" AssociatedUpdatePanelID="ConditionFilterPanel" DynamicLayout="false" DisplayAfter="100">
					<ProgressTemplate>
						<div style="position: relative; width: 100%; ">
						<div style="position: absolute; width: 100%; background-color: white;">
							<img alt="" style="vertical-align:middle; border: 0;" height="32" width="32" src='<%= this.ResolveClientUrl("~/App_Themes/Default/images/Shell/loading_rss.gif") %>' />
						</div>
						</div>
					</ProgressTemplate>
                </asp:UpdateProgress>
                <asp:UpdatePanel runat="server" ID="ConditionFilterPanel" ChildrenAsTriggers="true" UpdateMode="Conditional">
                    <ContentTemplate>
                        <ibn:FilterExpressionBuilder runat="server" ID="ConditionFilter"/>
                    </ContentTemplate>
                </asp:UpdatePanel>
                <br />
            </td>
        </tr>
        <tr>
            <td colspan="2" class="FormSpacerCell">
            </td>
        </tr>      
        <tr>
            <td colspan="2" class="FormFieldCell">
                Use the actions below to assign rewards.
            </td>
        </tr>        
        <tr>
            <td class="FormLabelCell" colspan="2"><asp:Label ID="Label11" runat="server" Text="Rewards"></asp:Label>:</td>
        </tr>        
        <tr>
            <td colspan="2" class="FormFieldCell" style="width: 100%;">
                <asp:UpdateProgress runat="server" ID="progressUP1" AssociatedUpdatePanelID="UpdatePanel1" DisplayAfter="100" DynamicLayout="false">
					<ProgressTemplate>
						<div style="position: relative; width: 100%; ">
						<div style="position: absolute; width: 100%; background-color: white;">
							<img alt="" style="vertical-align:middle; border: 0;" height="32" width="32" src='<%= this.ResolveClientUrl("~/App_Themes/Default/images/Shell/loading_rss.gif") %>' />
						</div>
						</div>
					</ProgressTemplate>
                </asp:UpdateProgress>
               <asp:UpdatePanel runat="server" ID="UpdatePanel1" ChildrenAsTriggers="true" UpdateMode="Conditional">
                    <ContentTemplate>
                        <ibn:FilterExpressionBuilder runat="server" ID="ApplyConditionFilter" BlockEnable="false"/>
                    </ContentTemplate>
                </asp:UpdatePanel>
                <br />
            </td>
        </tr>
        <tr>
            <td colspan="2" class="FormSpacerCell">
            </td>
        </tr>
    </table>
</div>
<script language="javascript">
function childPageLoad()
{
	Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(beginRequestHandler_ConfigControls);
	//Sys.WebForms.PageRequestManager.getInstance().add_endRequest(endRequestHandler_ConfigControls);
	var obj = document.getElementById('ibnMain_divWithLoadingRss');
	if (obj)
		obj.style.display = 'none';
}

function beginRequestHandler_ConfigControls(sender, args)
{
	//var s = '';
	for (var i = 0; i < sender._updatePanelClientIDs.length; i++)
	{
		var _id = sender._postBackSettings.panelID.split('|')[0].replace(/\$/g, '_');
		if (sender._updatePanelClientIDs[i] == _id)
		{
			var obj = document.getElementById(sender._updatePanelClientIDs[i]);
			var objProgress = obj;

			while (objProgress.previousSibling != null)
			{
				objProgress = objProgress.previousSibling;
				if (objProgress.nodeType == 1)
					break;
			}
			
			if (objProgress && objProgress.tagName == 'DIV' && objProgress.style.visibility == 'hidden')
			{
				var mas = objProgress.getElementsByTagName('DIV');
				for (var j = 0; j < mas.length; j++)
				{
					if (mas[j].style.position == 'relative')
					{
						obj.style.visibility = 'visible';
						//s += '|' + i + ' _ ' + obj.offsetHeight;
						mas[j].style.height = obj.offsetHeight + 'px';
						obj.style.visibility = 'hidden';
					}
				}
			}
		}
	}
	//window.status = s;
}
</script>
