<%@ Page Language="C#" AutoEventWireup="true" Inherits="Login" CodeBehind="Login.aspx.cs" %>

<html>
<head runat="server">
    <title>Mediachase CMS - Login</title>
</head>
<body style="background-image: url('images/bg1.gif'); background-repeat: repeat;">
    <form id="form1" runat="server">
    <div>
        <table width="100%" height="100%" cellpadding="0" cellspacing="0">
            <tr valign="middle">
                <td align="center" valign="middle">
                    <asp:Login ID="LogOn" runat="server" BackColor="#F7F7DE" BorderColor="#CCCC99" BorderStyle="Solid"
                        BorderWidth="1px" Font-Names="Verdana" Font-Size="10pt" LoginButtonText="<%$ Resources:Public, LoginEnter %>"
                        PasswordLabelText="<%$ Resources:Public, LoginPass %>" RememberMeText="<%$ Resources:Public, LoginRemember %>"
                        TitleText="<%$ Resources:Public, LoginTitle %>" UserNameLabelText="<%$ Resources:Public, LoginName %>"
                        FailureText="<%$ Resources:Public, LoginFail %>">
                        <TitleTextStyle BackColor="#6B696B" Font-Bold="True" ForeColor="#FFFFFF" />
                        <TextBoxStyle Width="150px" />
                    </asp:Login>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
