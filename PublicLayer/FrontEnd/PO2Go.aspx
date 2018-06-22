<%@ Page Language="C#" AutoEventWireup="true" Inherits="Po2Go" CodeBehind="PO2Go.aspx.cs" %>

<html>
<head runat="server">
    <title>Redirecting to PO2Go...</title>
    <script type="text/javascript">
        document.getElementById("po2goform").submit();
    </script>
</head>
<body style="background-image: url('images/bg1.gif'); background-repeat: repeat;">
        <form id="po2goform" action="<%=this.ReturnUrl %>" method="POST">
            <!--  api  key  -->
            <input  type="hidden"  name="apikey" value="HE5cc69cdaf42a68.5b24081998d26">
            <!--  version  -->
            <input  type="hidden"  name="version"  value="1.0">
            <!--  params/order  -->
            <input  type="hidden"  name="params"  value="<%=this.EncodedPunchOut2GoParams%>">
        </form>

    </form>
</body>
</html>
