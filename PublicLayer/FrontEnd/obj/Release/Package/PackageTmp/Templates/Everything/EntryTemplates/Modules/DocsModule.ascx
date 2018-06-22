<%@ Control Language="C#" AutoEventWireup="true" Inherits="Templates_Everything_Entry_Modules_DocsModule" Codebehind="DocsModule.ascx.cs" %>
<table cellpadding="0" cellspacing="0" style="width: 100%" class="ecf-download-navframe">
    <tr>
        <th class="ecf-download-header">Documentation&nbsp;</th>
    </tr>
    <tr>
        <td>
            <asp:GridView SkinID="Versions" AutoGenerateColumns="false" ShowHeader="true" runat="server"
                Width="100%" ID="DownloadsList">
                <EmptyDataTemplate>
                    <%#RM.GetString("ACCOUNT_NO_DOWNLOADS_LABEL")%>
                </EmptyDataTemplate>
                <Columns>
                    <asp:TemplateField HeaderText="File" HeaderStyle-Width="400px">
                        <ItemTemplate>
                            <asp:Image runat="server" ID="Image1" BorderWidth="0" Width="16" Height="16" ImageUrl='<%# Eval("Icon")%>' />
                            <asp:HyperLink runat="server" CssClass="ecf-inline-header3" NavigateUrl='<%# Eval("Url")%>'
                                ID="Hyperlink1" NAME="Hyperlink1"><%# DataBinder.Eval(Container.DataItem, "Name")%></asp:HyperLink>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Date Added" HeaderStyle-Width="250px">
                        <ItemTemplate>
                            <%# Eval("Created")%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="" HeaderStyle-Width="100px">
                        <ItemTemplate>
                            <asp:HyperLink CssClass="ecf-image-button ecf-download-button" runat="server" NavigateUrl='<%# Eval("Url")%>'
                                ID="Hyperlink1" NAME="Hyperlink1">Download</asp:HyperLink><br />
                            Size:
                            <%# Eval("Size")%>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </td>
    </tr>
</table>
