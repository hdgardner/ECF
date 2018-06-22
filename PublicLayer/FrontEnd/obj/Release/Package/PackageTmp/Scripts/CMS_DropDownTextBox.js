if(Type.isNamespace("Mediachase.Cms")==false) Type.registerNamespace("Mediachase.Cms");

Mediachase.Cms.DropDownTextBox = function()
{
	Mediachase.Cms.DropDownTextBox.initializeBase(this);
	this._textTargetObj = null;
	this._ddTargetObj = null;
	this._flag = true;
}

Mediachase.Cms.Util.prototype = 
{
	InitTaObjscts : function(_textTargetId, _ddTargetId)
	{

		this._textTargetObj=document.getElementById(_textTargetId);
		this._ddTargetObj = document.getElementById(_ddTargetId);
	 
	},
	GetTotalOffset : function(eSrc)
	{
		var obj = new Object();
		obj.Top = 0;
		obj.Left =0;
		while (eSrc)
		{
		  if (eSrc.style.position=='absolute')
		  {
		   return this;
		  }
		 obj.Top += eSrc.offsetTop;
		 obj.Left += eSrc.offsetLeft;
		  
		  
		 eSrc = eSrc.offsetParent;
		}
		return obj;
	},
	
	GetObjectWidth : function(obj)
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
	},
	InitTA : function(picObj, imgPath)
	{
		if (picObj != null)
		{
			if (this._flag)
			{
				picObj.src= imgPath+"scrollup.gif";
			}
			else
			{
				picObj.src=imgPath+"scrolldown.gif";
			}
		}
	 

	 
	  var objTarget= this._textTargetObj;
	  var objddTarget=this._ddTargetObj;
	  // var objTA=newTAObj;
	  if (this._flag)
		{ 
	      
			objddTarget.innerHTML="<textarea id='newTA' style='position:absolute; background-color: #f1ede7;' rows='10' cols='15'>"+objTarget.innerHTML+"</textarea>";
			objddTarget.style.display=""
			this.Redraw();
			this._flag=false;
		}
		else
		{
			objTA=document.getElementById('newTA');
			this._objTarget.innerHTML=objTA.innerHTML;
			this._objddTarget.style.display="none"
			this._objddTarget.innerHTML="";
			this._flag=true;
		}
	},
	
	Redraw : function()
	{  
		var objTarget= this._textTargetObj;
		var totaloffset=this.GetTotalOffset(objTarget);
		var objTA=document.getElementById('newTA');
		   
		if (objTA!=null)
		{    
			objTA.style.top= parseInt(totaloffset.Top) +'px';
			objTA.style.left= parseInt(totaloffset.Left)+'px';
			objTA.style.width= parseInt(this.GetObjectWidth(objTarget)-5)+'px';
		}
	},
	
	initialize : function()
	{
		Mediachase.Cms.DropDownTextBox.callBaseMethod(this, 'initialize');
		
	},
	dispose : function()
	{
		Mediachase.Cms.DropDownTextBox.callBaseMethod(this, 'dispose');
	}
}

Mediachase.Cms.DropDownTextBox.registerClass('Mediachase.Cms.DropDownTextBox', Sys.Component); 
 
 

 
