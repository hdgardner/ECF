<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PerformanceModule.ascx.cs"
    Inherits="Mediachase.Commerce.Manager.Apps.Dashboard.Modules.PerformanceModule" %>
<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<rsweb:ReportViewer ID="PerfReportViewer" runat="server" ShowDocumentMapButton="False"
    ShowExportControls="False" ShowFindControls="False" ShowPageNavigationControls="False"
    ShowPrintButton="False" ShowPromptAreaButton="False" ShowRefreshButton="False"
    ShowZoomControl="False" Font-Names="Verdana" Font-Size="8pt" 
    Height="600px" Width="100%">
    <LocalReport ReportPath="Apps\Dashboard\Modules\PerformanceReport.rdlc">
    </LocalReport>
</rsweb:ReportViewer>
