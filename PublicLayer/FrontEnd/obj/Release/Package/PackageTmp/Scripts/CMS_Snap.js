if(Type.isNamespace("Mediachase.Cms")==false) Type.registerNamespace("Mediachase.Cms");

///<summary>
///Snap is responsible for adding/editing/saving/deleting site nodes and 
///storing information about added/edited/deleted site nodes
///</summary>
Mediachase.Cms.Snap = function()
{
	Mediachase.Cms.Snap.initializeBase(this);
	this._containerValue = "";
	this._containerId = "";
	this._editedContainer = "";
	this._editedContainerValue = "";
	this._popUpTempContainer = "";
	this._editedFlag = -1;
	this._containerDeleted = "";
	this._sbDeleted = new StringBuilder();
	this._sbEdited = new StringBuilder();
	this._sbTempEdited = new StringBuilder();
	this._editInfo = null;
	//DV: global html objects for inline edit
	this._recordEditedHiddenField = null;
	//DV: global html objects for popup edit
	this._recordEditedPopupHiddenField = null;
}

Mediachase.Cms.Snap.prototype = 
{
	//properties begin
	get_ContainerId : function()
	{
		return this._containerId;
	},	
	set_ContainerId : function(value)
	{
		this._containerId = value;
	},	
	get_EditedContainerId : function()
	{
		return this._editedContainer;
	},	
	set_EditedContainerId : function(value)
	{
		this._editedContainer = value;
	},
	get_PopUpTempContainerId : function()
	{
		return this._popUpTempContainer;
	},	
	set_PopUpTempContainerId : function(value)
	{
		this._popUpTempContainer = value;
	},
	//properties end
	
	EditControlHandler : function(id, dNode) 
	{
		var obj = document.getElementById(id);
		this._editInfo = obj;

		if (obj != null) {
			obj.value = obj.value + 'edit,' + dNode + ';';
		}
		else {
			alert("Cant find hfEditInfo: " + id);
		}
	},
	EditInlineWrapper : function(id) 
	{
		var obj = document.getElementById(id);
		if (obj == null)
			alert("EditInlineWrapper @ Cant find: " + id);

		this._recordEditedHiddenField = obj;
	},
	EditPopupWrapper : function(id) 
	{
		var obj = document.getElementById(id);
		if (obj == null)
			alert("EditPopupWrapper @ Cant find: " + id);
	       
		this._recordEditedPopupHiddenField = obj;
	},
	///<summary>
	///Saves info about deleted nodes
	///</summary>
	FillDeleteSnapInfo : function(target, containerId) 
	{
		var hfDeleted = document.getElementById(target);
		if (hfDeleted == null) {
			alert("Cant find hiddenField: " + target);
			return;
		}
		hfDeleted.value = hfDeleted.value + containerId + ",";
	},
	///<summary>
	///Saves all info about modified nodes
	///</summary>
	RecordAllInfo : function() 
	{
		var containerDeleted = document.getElementById(this._containerDeleted);
		if (containerDeleted != null)
			containerDeleted.value = this._sbDeleted.toString("$^|^$");

		var containerEdited = document.getElementById(this._editedContainer);

		if (containerEdited != null)
			containerEdited.value = this._sbEdited.toString("$^|^$");
	},
	
	MakeModified : function(value, target, containerId) 
	{
		this._containerDeleted = target
		var sbTempDeleted = new StringBuilder();
		sbTempDeleted.append(value);
		sbTempDeleted.append(containerId);

		this._sbDeleted.append(sbTempDeleted.toString(","));
		window.status = "Components removed!";
	},
	InitTempAddingContainer : function(target) 
	{
		this._containerId = target;
	},
	InitStaticEditedContainer : function(target) 
	{
		this._editedContainer = target;
	},
	InitPopUpTempContainer : function(target) 
	{
		this._popUpTempContainer = target;
	},
	///<summary>
	///Saves info about added node
	///</summary>
	RecordAddedControl : function(value, target, containerId, controlId, controlFactoryId, FactoryUID) 
	{
		this._containerValue = value + ',' + containerId + ',' + controlId + ',' + controlFactoryId + ',' + FactoryUID + '^';
	},
	///<summary>
	///Saves info about added dynamic node
	///</summary>
	RecordNewDN : function() 
	{
		var obj = document.getElementById(this._containerId);
		if (obj != null) 
		{
			obj.value = this._containerValue;
		}
	},
	//формируем строку с информацией об отредактированных контролов edit+ snapId+ cntrlId+cntrlKey+value
	RecordEditedSnapCntrl : function(value, versionId, target, snapId, cntrlId) 
	{
		this._editedContainer = target;
		if ((this._editedFlag == -1) && (this._sbTempEdited.length() == 0)) 
		{
			this._sbTempEdited.append(value);
			this._sbTempEdited.append(snapId);
			this._sbTempEdited.append(cntrlId);
			this._editedFlag = 0;
		}
	},
	///<summary>
	///Saves info about edited node key
	///</summary>
	RecordEditedKey : function(key) 
	{
		if ((this._sbTempEdited.length() > 0) && (this._editedFlag == 0)) 
		{
			this._sbTempEdited.append(key);
			this._editedFlag = 1;
		}
	},
	
	RecordStaticEditedInfo : function(value, cntrlId, keyId) 
	{
		if ((this._sbTempEdited.length() == 0) && (this._editedFlag == -1)) 
		{
			this._sbTempEdited.append(value);
			this._sbTempEdited.append(cntrlId);
			this._sbTempEdited.append(keyId);
			this._editedFlag = 1;
		}
	},
	///<summary>
	///Saves info about edited node value
	///</summary>
	RecordEditedValue : function(value) 
	{
		if (this._recordEditedHiddenField != null) 
		{
			this._recordEditedHiddenField.value = value;
			return;
		}
		if ((this._sbTempEdited.length() > 0) && (this._editedFlag == 1)) 
		{
			this._sbTempEdited.append(value);
			if ((this._sbTempEdited.length() == 5) || (this._sbTempEdited.length() == 4))
				this._sbEdited.append(this._sbTempEdited.toString("$^|,|^$"));
			this._sbTempEdited.clear();
			this._editedFlag = -1;
			window.status = "Component content has been changed!";
		}
	},
	InitEditedContainer : function(target) 
	{
		var objWrapper = document.getElementById(target);
		if (objWrapper != null) {
			objWrapper.setAttribute("innerContainer", this._popUpTempContainer);
			//alert(objWrapper.innerHTML);
			var objContainer = document.getElementById(this._popUpTempContainer);
			//alert(objContainer.value);
			if (objContainer != null) {
				if (this._recordEditedPopupHiddenField != null)
					objContainer.value = this._recordEditedPopupHiddenField.value;
				else    
					objContainer.value = "";            
			}
		}
	},
	
	InitUtilSnap : function()
	{
		var ut = $find("MediachaseCmsUtil");
		if(ut!=null)
		{
			ut._snap = this;
			return true;
		}
		else 
		{
			return false;
		}
	},
	
	initialize : function()
	{
		Mediachase.Cms.Snap.callBaseMethod(this, 'initialize');
		this._recAllInfo = Function.createDelegate(this, this.RecordAllInfo);
		var form = document.getElementsByTagName("form");
		if(form!=null && form.length>0)
			$addHandler(form[0],"submit", this._recAllInfo);
		this.RecordAllInfo();	
		setTimeout(Function.createDelegate(this, this.InitUtilSnap), 1000);	
			
	},
	dispose : function()
	{
		var form = document.getElementsByTagName("form");
		if(form!=null && form.length>0)
			$removeHandler(form[0],"submit", this._recAllInfo);
		Mediachase.Cms.Snap.callBaseMethod(this, 'dispose');
	}
}

Mediachase.Cms.Snap.registerClass('Mediachase.Cms.Snap', Sys.Component); 