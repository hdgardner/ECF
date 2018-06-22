// JScript File

var textTargetObj;
var ddTargetObj;
var flag=true;
function InitTaObjscts(_textTargetId, _ddTargetId)
{

    textTargetObj=document.getElementById(_textTargetId);
    ddTargetObj = document.getElementById(_ddTargetId);
 
}
 
function getObjectHeight(obj)
{
    var elem=obj;
    var result=0;

    if (elem.offsetHeight)
    {
        result=elem.offsetHeight;
    }
    else if (elem.clip&& elem.clip.height)
        { 
            result=elem.clip.height;
        
        }
        else if (elem.style&&elem.style.pixelHeight)
        {
        
            result=elem.style.pixelHeight;
        }

 return parseInt(result);
 
} 

function GetTotalOffset(eSrc)
{

 this.Top = 0;
 this.Left =0;
//alert(eSrc.currentStyle.Top)
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

function getObjectWidth(obj)
{
    var elem=obj;
    var result=0;
    if (elem.offsetWidth)
    {
        result=elem.offsetWidth;
    }
    else if (elem.clip&& elem.clip.width)
        { 
            result=elem.clip.width;
        
        }
        else if (elem.style&&elem.style.pixelWidth)
        {
        
            result=elem.style.pixelWidth;
        }

 return parseInt(result);
 
}



 function initTA(picObj, imgPath)
 {

    if (picObj != null)
    {
        if (flag)
        {
            picObj.src= imgPath+"scrollup.gif";
        }
        else
        {
            picObj.src=imgPath+"scrolldown.gif";
        }
    }
 

 
  var objTarget= textTargetObj;
  var objddTarget=ddTargetObj;
  // var objTA=newTAObj;
  if (flag)
    { 
      
        objddTarget.innerHTML="<textarea id='newTA' style='position:absolute; background-color: #f1ede7;' rows='10' cols='15'>"+objTarget.innerHTML+"</textarea>";
        objddTarget.style.display=""
        redraw();
        flag=false;
    }
    else
    {
        objTA=document.getElementById('newTA');
        objTarget.innerHTML=objTA.innerHTML;
        objddTarget.style.display="none"
        objddTarget.innerHTML="";
        flag=true;
    }
    

 }
 

 
 
 function redraw()
 {  
    var objTarget= textTargetObj;
    var totaloffset=GetTotalOffset(objTarget);
    var objTA=document.getElementById('newTA');
    
    if (objTA!=null)
    {    

    objTA.style.top= parseInt(totaloffset.Top) +'px';
    objTA.style.left= parseInt(totaloffset.Left)+'px';
    objTA.style.width= parseInt(getObjectWidth(objTarget)-5)+'px';
    }
 }
 
//window.onresize=redraw;
 
