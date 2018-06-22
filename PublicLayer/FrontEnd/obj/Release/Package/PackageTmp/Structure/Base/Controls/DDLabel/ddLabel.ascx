<%@ Control Language="C#" AutoEventWireup="true" Inherits="Controls_DDLabel_ddLabel" Codebehind="ddLabel.ascx.cs" %>

<%--<link href='<%= Mediachase.Cms.AppStart.GetAbsolutePath("~/Structure/Base/Controls/ddTextBox/editableWrapper.css", Request.ServerVariables["SERVER_PORT"])%>' rel="stylesheet" type="text/css" />--%>
<script type="text/javascript" >
    var MouseEvent=0;
    function GetTotalOffset1(eSrc)
    {

     this.Top = 0;
     this.Left =0;
    //alert(eSrc.currentStyle.Top)
    //alert(eSrc);
     while (eSrc)
     {
       if (eSrc.style.position=='absolute')
       {
           return this;
       }
         this.Top += eSrc.offsetTop;
         this.Left += eSrc.offsetLeft;
       
       
      eSrc = eSrc.offsetParent;

     }
     return this;
    }
    function ExchangeText(from, to)
    {
        var objFrom = document.getElementById(from);
        var objTo = document.getElementById(to);
        objTo.innerHTML = objFrom.value;
        //objTo.style.background="#EBE7D3";
        
        objCont = document.getElementById('divTextBoxLabel');
        var totaloffset=GetTotalOffset1(objCont);
     
        objTo.style.top = parseInt(totaloffset.Top)+parseInt(MouseEvent+15)+'px';
        objTo.style.display="block";
    }
    function HideDiv(toHide)
    {
        
        //objToShow = document.getElementById(toShow);
        var objToHide = document.getElementById(toHide);
        //alert(GetTotalOffset(objToHide).Top);
        //objToShow.style.display = "inline";
        objToHide.style.display = "none";
        
        //obj.style.display="none";
    }
</script>
<input type=hidden id="commentStorage" runat="server" value=""/>
 <div id="divTextBoxLabel" class="normalDivLabel">
       <div  id="ddTargetLabel" runat="server" style="position:absolute; border: solid 1px Black; padding: 3px; margin: 3px; display:none; background-color: #ACA899;" ></div>
       <textarea class="normalDivLabel" readonly=readonly id='textTargetLabel' onkeydown="event.returnValue=false;" style='width:200px;overflow:hidden;border:0;background-color:#f1ede7;' rows="2" cols="15" runat="server"></textarea>
 </div>
