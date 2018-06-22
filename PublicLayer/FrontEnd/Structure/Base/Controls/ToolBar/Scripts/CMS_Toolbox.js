if(Type.isNamespace("Mediachase.Cms")==false) Type.registerNamespace("Mediachase.Cms");

///<summary>
///Toolbox with controls
///</summary>
Mediachase.Cms.Toolbox = function()
{
	Mediachase.Cms.Toolbox.initializeBase(this);
	this._ns4 = document.layers;
	this._ie4 = document.all;
	this._ns6 = document.getElementById && !document.all;

	this._CP_toolBarStorage = "";
	this._CP_toolBarHt = new Array();

	this._toolBoxVisible="False";

	// holds cookie name for saving toolbox visible property
	this._toolBoxVisibleCookieString = "ToolBoxVisibleCookie";

	// for storing pictures
	this._objCount = 0; // amount of images on a web page 
	this._pics = new Array();

	// toolBoxId value. Initialized in initToolBox function
	this._toolBoxVisibleClientId = "";

	// Maximized-minimized-logic value
	this._toolBoxVisible = "False";
	this._storage = null;
	this._toolBoxId = "";
	this._toolBoxLocation = "";

    // constant for storing coordinates delimiter
	this._toolBoxCoordinatesDelimiter = ',';
}

Mediachase.Cms.Toolbox.prototype = 
{
	
	//begin properties
	get_ToolboxId : function()
	{
		return this._toolBoxId;
	},
	set_ToolboxId : function(value)
	{
		this._toolBoxId = value;
	},
	get_ToolboxLocation : function()
	{
		return this._toolBoxLocation;
	},
	set_ToolboxLocation : function(value)
	{
		this._toolBoxLocation = value;
	},
	//end properties
	
	///<summary>
	///Inits toolbox location
	///</summary>	
	InitTbLocation : function(_name)
	{
		var _storage = this._CP_toolBarHt[_name];
		var al = new Array();
		var tmpStr = "";
		var obj = document.getElementById(_storage);
		if (obj!=null)
		{
			tmpStr = obj.value;
			al = tmpStr.split(this._toolBoxCoordinatesDelimiter);
	    
			var _obj = document.getElementById(_name);
			if (_obj!=null)
			{
				if (parseInt(al[0])<parseInt(document.body.clientHeight))
					_obj.style.top = parseInt(al[0])+'px';
				else
					_obj.style.top = (parseInt(document.body.clientHeight)/2)+'px';
	                
	            if (al[1]!=null)
	                _obj.style.left = parseInt(al[1]) + 'px';
	            else
	                _obj.style.left = '0px';
	            
				if(_obj.style.left < 0)
					_obj.style.left = '0px';
				if(_obj.style.top < 0)
					_obj.style.top = '0px';
	            
				_obj.style.display = '';
			}
			else
				 alert('initTbLocation: Problem with Toolbox ID.');
		}
		else
		{
			alert('initTbLocation:  Problem with ToolboxID storage. StorageId='+_name);
		}
	},
	
	///<summary>
	///Saves toolbox location
	///</summary>	
	SaveToolboxState : function()
	{
		this.RecordTbLocation();
	    
		// save toolbar visible property to cookie
		var obj = document.getElementById(this._toolBoxVisibleClientId);
		if (obj != null)
		{
			var date = new Date();
			date.setTime(date.getTime()+(10*24*60*60*1000));
			var expires = "; expires=" + date.toGMTString();
			document.cookie = this._toolBoxVisibleCookieString + "=" + obj.value + expires + "; path=/";
		}
		else
		{
			//alert('SaveToolboxState: Problem with Toolbar ID.');
		}
	},
	
	///<summary>
	///Hides toolbox
	///</summary>	
	MinimizeToolBox : function(contentId)
	{
		var objContent = document.getElementById(contentId);
		if (objContent == null) { alert('Element with ['+contentId+'] not found!'); return; }
		//var objBtn = document.getElementById(btnId);
		//if (objBtn == null) { alert('Element with ['+btnId+'] not found!'); return; }
		objContent.style.display="none";
	    
		if (contentId == 'showimage') { document.getElementById('ToolBarVisible').value = 'False';}
		if (contentId == 'showimageT') 
		{
			document.getElementById('ToolBoxVisible').value = 'False';
		}
	},
	
	RecordTbLocation : function()
	{
		for (var i=0; i<this._CP_toolBarHt.length; i++)
		{
			var _name = this._CP_toolBarHt[i].name;
			var _storage = this._CP_toolBarHt[i].storage;
	        
			var tmpStr = "";
			var obj = document.getElementById(_storage);
	        
			if (obj != null)
			{
				var _obj = document.getElementById(_name);
				if (_obj != null)
				{
					var objTop = parseInt(_obj.style.top);
					var objLeft = parseInt(_obj.style.left);
					tmpStr = objTop >= 0 ? objTop : 0;
					tmpStr += this._toolBoxCoordinatesDelimiter;
					tmpStr += objLeft >= 0 ? objLeft : 0;
					obj.value = tmpStr;
    	    		var date = new Date();
	    			date.setTime(date.getTime()+(10*24*60*60*1000));
					var expires = "; expires="+date.toGMTString();
					document.cookie = _name + "=" + tmpStr + expires + "; path=/";
				}
				else
				{
					alert('recordTbLocation: Problem with Toolbox ID.');
				}
			}
			else
			{
				alert('recordTbLocation: Problem with ToolboxID storage. StorageId='+_storage);
			}
		}
	},
	
	InitTb : function(_name, _storage)
	{
	   this._CP_toolBarHt[this._CP_toolBarHt.length] = {name: _name, storage: _storage, scroll: "-1"};
	   
	   for (var i=0; i<this._CP_toolBarHt.length; i++)
	   {
		  this._CP_toolBarHt[this._CP_toolBarHt[i].name] = this._CP_toolBarHt[i].storage;
	   }
	},
	
	InitToolBox : function(id)
	{
		var obj = document.getElementById(id);
		if (obj != null)
		{
			this._toolBoxVisibleClientId = id;
			var flag = obj.value;
			switch (flag) 
			{
				case "True":
					this.ShowToolBox();
					this._toolBoxVisible = flag;
					break;
				case "False":
					this.HideToolBox();
					this._toolBoxVisible = flag;
					break;
				default:
					break;
			}
		}
	},
	
	HideToolBox : function()
	{
		var obj = document.getElementById('showimageT')
		if (obj!=null)
		{   
			obj.style.display='none';
		}
	},
	
	ShowToolBox : function()
	{
		var obj=document.getElementById('showimageT')
		if (obj!=null)
		{ 
			obj.style.display='';
		}
	},
	
	MinimizeBox : function(contentId)
	{
		var obj=document.getElementById(contentId);
		if (obj!=null)
		{
			if (obj.style.display=='')
			{
				obj.style.display='none';
			}
			else
			{
				obj.style.display='';
			}
		}
	},
	
	initialize : function()
	{
		Mediachase.Cms.Toolbox.callBaseMethod(this, 'initialize');
		this._savestate = Function.createDelegate(this, this.SaveToolboxState);
		$addHandler(window, "beforeunload",this._savestate);
		if(this._toolBoxId!="" && this._toolBoxLocation!="")
		{
			this.InitTb(this._toolBoxId, this._toolBoxLocation);
			this.InitTbLocation(this._toolBoxId);
			this.InitToolBox("ToolBoxVisible");
		}
	},
	dispose : function()
	{
		Mediachase.Cms.Toolbox.callBaseMethod(this, 'dispose');
		$removeHandler(window, "beforeunload",this._savestate);
	}
}

Mediachase.Cms.Toolbox.registerClass('Mediachase.Cms.Toolbox', Sys.Component); 