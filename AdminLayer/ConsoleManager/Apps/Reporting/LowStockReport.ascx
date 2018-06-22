<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LowStockReport.ascx.cs"
    Inherits="Mediachase.Commerce.Manager.Apps.Reporting.LowStockReport" %>
<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<div class="report-view">
    <div class="report-content">
        <rsweb:ReportViewer OnBookmarkNavigation="MyReportViewer_BookmarkNavigation" ZoomMode="Percent" 
            SizeToReportContent="true" AsyncRendering="false" ID="MyReportViewer" runat="server" ShowDocumentMapButton="False"
            ShowExportControls="True" ShowFindControls="False" ShowPageNavigationControls="True"
            ShowPrintButton="True" ShowPromptAreaButton="False" ShowRefreshButton="True"
            ShowZoomControl="True" Font-Names="Verdana" Font-Size="8pt" Width="100%" Height="90%" HyperlinkTarget="_blank">
            <LocalReport ReportPath="Apps\Reporting\Reports\LowStock.rdlc" EnableHyperlinks="True">
            </LocalReport>
        </rsweb:ReportViewer>
    </div>
</div>
