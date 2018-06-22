<%@ Control Language="C#" AutoEventWireup="true" Inherits="Controls_DDTextBox_ddTextBox" Codebehind="ddTextBox.ascx.cs" %>
<link href='<%= Mediachase.Commerce.Shared.CommerceHelper.GetAbsolutePath("/Styles/DropDownTextBox.css")%>' rel="stylesheet" type="text/css" />

 <script type="text/javascript" language="javascript"  src='<%= Mediachase.Commerce.Shared.CommerceHelper.GetAbsolutePath("/Scripts/DropDownTextBox.js")%>' ></script>
 <script type="text/javascript" >
 function HighLightWrapper(obj)
{
     obj.style.cursor='text'
     //obj.style.background='#afeafe';
}
function UnHighLightWrapper(obj)
{
        obj.style.cursor='default'; 
        //obj.style.background='white';
}
 </script>

<div id="ddTextBox" runat="server">
 
<div id="divTextBox" class="normalDiv" onmouseover="HighLightWrapper(this)" onmouseout="UnHighLightWrapper(this)" style="background-color: Transparent;">
<textarea id='textTarget' style='width:200px;overflow:hidden; background-color: #f1ede7' rows="1" cols="15" runat="server" ></textarea><img id="arrowImg" alt="" src='<%= Mediachase.Commerce.Shared.CommerceHelper.GetAbsolutePath("/Images/DropDownTextBox/scrollDown.gif")%>' onclick="initTA(this, '<%= Mediachase.Commerce.Shared.CommerceHelper.GetAbsolutePath("/Images/DropDownTextBox/")%>');" />
<div id="ddTarget" runat="server"></div>
</div>
</div>
