<%@ Control Language="C#" AutoEventWireup="true" 
    CodeBehind="OverviewModule.ascx.cs" 
    Inherits="Templates_Everything_Entry_Modules_OverviewModule" EnableViewState="false" %>
    
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register Src="DocsModule.ascx" TagName="DocsModule" TagPrefix="catalog" %>

<div id="entry-overview">
    <div style="width:100%;position:relative">
        <div style="position:relative">
            <ajaxToolkit:TabContainer ID="DefaultTabContainer" EnableViewState="false" runat="server" CssClass="gray">
                <ajaxToolkit:TabPanel ID="OverviewTab" HeaderText="Overview" runat="server" EnableViewState="false">
                    <ContentTemplate>
                        <%#Entry.ItemAttributes["Description"].ToString() %>
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>
                <ajaxToolkit:TabPanel ID="SpecificationsTab" HeaderText="Specifications" runat="server" EnableViewState="false">
                    <ContentTemplate>
                        <catalog:DocsModule EnableViewState="false" ID="DocsModule1" GroupName="Specifications"  
                            DataSource="<%#Entry.Assets%>" runat="server"></catalog:DocsModule>
                        <catalog:DocsModule ID="DocsModule3" GroupName="UserGuides" 
                            DataSource="<%#Entry.Assets%>" runat="server"></catalog:DocsModule>
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>
                <ajaxToolkit:TabPanel ID="DownloadsTab" HeaderText="Downloads" runat="server" EnableViewState="false">
                    <ContentTemplate>
                        <catalog:DocsModule ID="DocsModule2" GroupName="Downloads" 
                            DataSource="<%#Entry.Assets%>" runat="server"></catalog:DocsModule>
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>
            </ajaxToolkit:TabContainer>
        </div>                    
    </div>
</div>