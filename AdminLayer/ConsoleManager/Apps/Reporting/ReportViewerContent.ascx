<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ReportViewerContent.ascx.cs" Inherits="Mediachase.Commerce.Manager.Apps.Reporting.ReportViewerContent" %>
<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<rsweb:ReportViewer ID="MyReportViewer" runat="server" ShowDocumentMapButton="False"
    ShowExportControls="True" ShowFindControls="False" ShowPageNavigationControls="True"
    ShowPrintButton="True" ShowPromptAreaButton="False" ShowRefreshButton="True"
    ShowZoomControl="True" Font-Names="Verdana" Font-Size="8pt" DocumentMapWidth="100%" 
    Width="100%">
</rsweb:ReportViewer>
