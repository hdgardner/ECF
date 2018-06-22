if(Type.isNamespace("Mediachase.Cms")==false) Type.registerNamespace("Mediachase.Cms");

var __currentCmsUtil = null;

///<summary>
///Util CMS class for positioning and highlighting dynamic and static site nodes.
///</summary>
Mediachase.Cms.Util = function()
{
	Mediachase.Cms.Util.initializeBase(this);
	this._currentObj = null;
	this._disableHighlight = false;
	this._wndPopUp = null;
	this._editorContainerPopup = null;
	this._mainDivInline = null;
	this._mainDIvEditor = null;
	this._timer = null;
	this._allowClose = true;
	this._flagToEdit = false;
	this._snap = null;
	this._isDesignMode = false;
}

Mediachase.Cms.Util.prototype =
{
    //getters and setters for properties
    get_IsDesignMode: function() {
        return this._isDesignMode;
    },
    set_IsDesignMode: function(value) {
        this._isDesignMode = value;
    },
    
    ///<summary>
	///Gets DHTML node width
	///</summary>
    GetObjectWidth: function(obj) {
        var elem = obj;
        var result = 0;
        if (elem.offsetWidth) {
            result = elem.offsetWidth;
        }
        else if (elem.clip && elem.clip.width) {
            result = elem.clip.width;

        }
        else if (elem.style && elem.style.pixelWidth) {

            result = elem.style.pixelWidth;
        }
        return parseInt(result);
    },

	///<summary>
	///Gets DHTML node height
	///</summary>
    GetObjectHeight: function(obj) {
        var elem = obj;
        var result = 0;
        if (elem.offsetHeight) {
            result = elem.offsetHeight;
        }
        else if (elem.clip && elem.clip.height) {
            result = elem.clip.height;

        }
        else if (elem.style && elem.style.pixelHeight) {

            result = elem.style.pixelHeight;
        }
        return parseInt(result);
    },

	///<summary>
	///Highlights dynamic or static site node
	///</summary>
    HighlightOn: function(obj) {
        if (this._disableHighlight) return;
        this._currentObj = obj;
        var divs = obj.getElementsByTagName('DIV');
        for (var i = 0; i < divs.length; i++) {
            if (divs[i].getAttribute('IsFrame') != null) {
                //resize frame
                //top
                if (divs[i].getAttribute('IsFrame') == 'top') {
                    divs[i].style.top = '0px';
                    divs[i].style.left = '0px';
                    divs[i].style.height = '1px';
                    divs[i].style.width = this.GetObjectWidth(obj) + 'px';
                }
                //bottom
                if (divs[i].getAttribute('IsFrame') == 'bottom') {
                    divs[i].style.top = this.GetObjectHeight(obj) + 'px';
                    divs[i].style.left = '0px';
                    divs[i].style.height = '1px';
                    divs[i].style.width = this.GetObjectWidth(obj) + 'px';
                }
                //right
                if (divs[i].getAttribute('IsFrame') == 'right') {
                    divs[i].style.top = '0px';
                    divs[i].style.left = this.GetObjectWidth(obj) + 'px';
                    divs[i].style.height = this.GetObjectHeight(obj) + 'px';
                    divs[i].style.width = '1px';
                }

                //left
                if (divs[i].getAttribute('IsFrame') == 'left') {
                    divs[i].style.top = '0px';
                    divs[i].style.left = '0px';
                    divs[i].style.height = this.GetObjectHeight(obj) + 'px';
                    divs[i].style.width = '1px';
                }

                divs[i].className = divs[i].getAttribute('ON');
            }
            if (divs[i].getAttribute('IsMenu') != null) {
                divs[i].className = 'MenuOn';
            }
        }
        var divs = obj.getElementsByTagName('IMG');
        for (var i = 0; i < divs.length; i++) {
            if (divs[i].getAttribute('ActionBtn') != null) {
                divs[i].style.display = 'inline';
                divs[i].style.top = '0px';
                if (divs[i].getAttribute('ActionBtn') == 'Move') {
                    divs[i].style.left = this.GetObjectWidth(obj) - 16 + 'px';
                }
                if (divs[i].getAttribute('ActionBtn') == 'Delete') {
                    divs[i].style.left = this.GetObjectWidth(obj) - 32 + 'px';
                }
                if (divs[i].getAttribute('ActionBtn') == 'Props') {
                    divs[i].style.left = 0 + 'px';
                    divs[i].style.top = -20 + 'px';
                }
            }
        }
    },

	///<summary>
	///Opens pop-up window with html editor inside
	///</summary>
    EditableFCK: function(obj, path) {
        this.ModalDivActivate();
        var _obj = document.getElementById("ContainerTB");
        if (_obj != null) {
            return false;
        }
        var str = obj.innerHTML;

        //obj.innerHTML='<input type="text" id="ContainerTB" value="" onchange="Readable(\''+obj.id+'\')"/>';
        //document.getElementById('ContainerTB').value=str;
        var h = 650;
        var w = screen.width * 0.7;
        var l = (screen.width - w) / 2;
        var t = (screen.height - h) / 2;
        var winprop = 'resizable=1,scrollbars=1, height=' + h + ', width=' + w + ', top=' + t + ', left=' + l;

        this._editorContainerPopUp = obj;
        //	if (BrowserDetect()=="IE")
        //	    wndPopUp = window.showModalDialog(path+'?Object='+obj.id,'mc',winprop);
        //	else
        if ((this._wndPopUp == null) || (this._wndPopUp.closed)) {
            this._wndPopUp = window.open(path + '&Object=' + obj.id, 'mc', winprop);
            //enable div watcher 
            this._timer = window.setInterval(this.ModalDivWatcher, 1000);
        }
    },

	///<summary>
	///Unhighlights dynamic or static node
	///</summary>
    HighlightOff: function(obj) {
        this._currentObj = null;
        var divs = obj.getElementsByTagName('div');
        for (var i = 0; i < divs.length; i++) {
            if (divs[i].getAttribute('IsFrame') != null) {
                divs[i].className = divs[i].getAttribute('OFF');
            }
            if (divs[i].getAttribute('IsMenu') != null) {
                divs[i].className = 'MenuOff';
            }
        }

        var divs = obj.getElementsByTagName('IMG');
        for (var i = 0; i < divs.length; i++) {
            if (divs[i].getAttribute('ActionBtn') != null) {
                divs[i].style.display = 'none';
            }
        }
    },

	///<summary>
	///Detects client browser.
	///</summary>
    BrowserDetect: function() {
        var sUserAgent = navigator.userAgent;
        var isOpera = sUserAgent.indexOf("Opera") > -1;
        var isKHTML = sUserAgent.indexOf("KHTML") > -1
				|| sUserAgent.indexOf("Konqueror") > -1
				|| sUserAgent.indexOf("AppleWebKit") > -1;
        var isIE = sUserAgent.indexOf("compatible") > -1
				&& sUserAgent.indexOf("MSIE") > -1
				&& !isOpera;
        var isMoz = sUserAgent.indexOf("Gecko") > -1
				&& !isKHTML;
        if (isIE) return "IE";
        if (isMoz) return "Mozilla";
        return null;
    },
    
    ///<summary>
	///Watches is opened pop-up window was closed and removes modal div from page if necessary
	///</summary>
    ModalDivWatcher: function() {
        var _uh = $find('MediachaseCmsUtil');
        if (_uh == null)
            return;

        if (!_uh._allowClose) {
            return;
        }

        if (_uh != null) {
            if ((_uh._wndPopUp != null) && (_uh._wndPopUp.closed)) {
                var obj = document.getElementById("TestMainWrapper");
                if (obj != null) {
                    obj.style.visibility = "hidden";
                    window.clearInterval(this._timer);
                }
            }
        }
    },

    ///<summary>
	///Inits action sets array
	///</summary>
    StartInitScript: function(browser) {
        if (browser == null) return null;
        if (browser == "IE") {
            setTimeout('InitActionSet()', 1000);
        }
        else
            setTimeout('InitActionSet()', 500);
    },


    ///<summary>
	///Closes opened pop-up if necessary
	///</summary>
    ModalPopUp: function() {
        var obj = document.getElementById("TestMainWrapper");
        if (obj == null) return false;
        if (obj.style.visibility == "hidden")
            this.ClosePopUp();
    },

    ///<summary>
	///Shows modal div
	///</summary>
    ModalDivActivate: function() {
        var obj = document.getElementById("TestMainWrapper");
        if (obj == null) return false;
        obj.style.visibility = "visible";
        obj.className = "PopUpActive";
    },

	///<summary>
	///Hides modal div
	///</summary>
    ModalDivDeactivate: function() {
        /*if (this._wndPopUp == null) return;
        try 
        {
        var testOnce = this._wndPopUp.closed;
        }
        catch (e) 
        {
        return;
        }
        if (!this._wndPopUp.closed) return;*/
        var obj = document.getElementById("TestMainWrapper");
        if (obj == null) return false;
        obj.style.visibility = "hidden";
    },

	///<summary>
	///Saves changes in site nodes and closes opened pop-up window
	///</summary>
    ClosePopUp: function() {
        //check for snap global variables
        if (this._snap == null || typeof (this._snap._editedFlag) == "undefined" || typeof (this._wndPopUp) == "undefined")
            return;
        if ((this._wndPopUp != null) & (this._snap._editedFlag == 1)) {

            if (this.BrowserDetect() == "IE") {

                //if ((this._editorContainerPopUp.innerContainer != null) && (this._editorContainerPopUp.innerContainer != "")) 
                if ((this._editorContainerPopUp.getAttribute("innerContainer") != null) && (this._editorContainerPopUp.getAttribute("innerContainer") != "")) {
                    //var obj = document.getElementById(this._editorContainerPopUp.innerContainer);
                    var obj = document.getElementById(this._editorContainerPopUp.getAttribute("innerContainer"));
                    if (obj != null && this._snap != null) {
                        if (obj.value != "") {
                            if (this._snap._recordEditedPopupHiddenField != null) {
                                this._snap._recordEditedPopupHiddenField.value = obj.value;
                                return;
                            }
                            this._snap._sbTempEdited.append(obj.value);
                            if ((this._snap._sbTempEdited.length() == 5) || (this._snap._sbTempEdited.length() == 4))
                                this._snap._sbEdited.append(this._snap._sbTempEdited.toString("$^|,|^$"));
                        }
                        this._snap._sbTempEdited.clear();
                        window.status = "Component content has been changed!";
                        this._snap._editedFlag = -1;
                        this._wndPopUp.close();
                    }
                }
            }
            else {
                if ((this._editorContainerPopUp.getAttribute("innerContainer") != null) && (this._editorContainerPopUp.getAttribute("innerContainer") != "")) {
                    var obj = document.getElementById(this._editorContainerPopUp.getAttribute("innerContainer"));
                    if (obj != null && this._snap != null) {
                        if (obj.value != "") {
                            if (this._snap._recordEditedPopupHiddenField != null) {
                                this._snap._recordEditedPopupHiddenField.value = obj.value;
                            }
                            this._snap._sbTempEdited.append(obj.value);
                            if ((this._snap._sbTempEdited.length() == 5) || (this._snap._sbTempEdited.length() == 4))
                                this._snap._sbEdited.append(this._snap._sbTempEdited.toString("$^|,|^$"));
                        }
                        this._snap._editedFlag = -1;
                        this._snap._sbTempEdited.clear();
                        window.status = "Component content has been changed!";
                        this._wndPopUp.close();
                    }
                }
            }
        }

    },

    ///<summary>
	///onkeyup event handler
	///</summary>
    OnKeyUp: function(source, e) {
        this.ApplyChanges();
    },

    ///<summary>
	///Shows inline editor
	///</summary>
    Editable: function(obj) {
        if (this._flagToEdit) {
            return false;
        }

        this._disableHighlight = true;

        var str = obj.getAttribute("innerText").replace(/<br>/g, "\r\n");

        var objHeight = this.GetObjectHeight(obj);
        var objWidth = this.GetObjectWidth(obj);
        var divHidden = "<span style='Valign:bottom' id=\"lblContainer\" style='visibility:hidden;overflow:auto;'>" + str + "</span>"
        obj.innerHTML = divHidden;
        this._mainDivInline = obj;

        var lblContainer = document.getElementById('lblContainer');
        lblContainer.style.width = objWidth + 'px';
        lblContainer.style.visibility = 'hidden';
        lblContainer.innerHTML = str.replace(/\n/gi, '<br/>');
        var lblHeight = this.GetObjectHeight(lblContainer);
        var lblWidth = this.GetObjectWidth(lblContainer);


        obj.innerHTML += "<textarea cols=\"100\" id=" + obj.id + '_' + "  style=\"border:0px;position:absolute;z-index:500;margin:0px;padding:0px;width:100%;word-break:break-all;overflow:hidden; background-color :transparent;\" onkeyup=\"var _uh=$find('MediachaseCmsUtil'); if(_uh!=null) _uh.OnKeyUp(this,event);\" onmousemove=\"focus()\"   onblur=\"var _uh=$find(\'MediachaseCmsUtil\'); if(_uh!=null) return _uh.EventBlur('" + obj.id + "',this);\"    />" + str + "</textarea>";

        var editor = document.getElementById(obj.id + '_');
        this._mainDivEditor = editor;
        editor.style.top = "0px";
        editor.style.left = "0px";
        editor.innerText = str;
        editor.style.height = lblHeight + "px";

        editor.style.width = objWidth + "px";

        if (this.BrowserDetect() == "Mozilla") {

            var objStyle = window.getComputedStyle(obj, null);

            editor.style.fontFamily = objStyle.fontFamily;
            editor.style.fontSize = objStyle.fontSize;

            editor.style.fontStyle = objStyle.fontStyle;
            editor.style.fontWeight = objStyle.fontWeight;
            editor.style.fontVariant = objStyle.fontVariant;
            editor.style.color = objStyle.color;
            editor.style.textTransform = objStyle.textTransform;

        }
        else {

            editor.style.fontFamily = obj.currentStyle.fontFamily;
            editor.style.fontSize = obj.currentStyle.fontSize;

            editor.style.fontStyle = obj.currentStyle.fontStyle;
            editor.style.fontWeight = obj.currentStyle.fontWeight;
            editor.style.fontVariant = obj.currentStyle.fontVariant;

            editor.style.color = obj.currentStyle.color;
        }
        this._flagToEdit = true;
    },

    ///<summary>
	///Saves inline editor changes
	///</summary>
    ApplyChanges: function() {
        var obj = document.getElementById("lblContainer");
        obj.innerHTML = this._mainDivEditor.value.replace(/\n/gi, '<br/>');
        var source = document.getElementById("lblContainer");
        var target = document.getElementById(this._mainDivInline.id + "_");
        var objHeight = this.GetObjectHeight(source);
        target.style.height = objHeight + "px";
    },

	///<summary>
	///Makes node readable
	///</summary>
    Readable: function(labelId, text) {
        var obj = document.getElementById(labelId);
        var str1 = "";
        if (text.value == "")
            str1 = "click for edit";
        else
            str1 = text.value;
        obj.innerHTML = str1.replace(/\n/gi, '<br/>');
        obj.setAttribute("innerText", str1, 0);
        this._flagToEdit = false;
        if (this._snap != null)
            this._snap.RecordEditedValue(str1);
        this._disableHighlight = false;
    },
	
	///<summary>
	///onblur event handler
	///</summary>
    EventBlur: function(idfld, ifield) {
        if (ifield.value != "") {
            this.Readable(idfld, ifield);
            return false;
        }
    },

	///<summary>
	///enter click handler (not used now)
	///</summary>
    EventEnter: function(evt, idfld, ifield) {
        evt = (evt) ? evt : window.event;
        if (evt.keyCode == 13 && idfld.id != "") {
            this.Readable(idfld, ifield);
            return false;
        } else {
            return true;
        }
    },

	///<summary>
	///Opens pop-up window with control property page
	///</summary>
    PopUpWindow: function(URL) {
        window.HideSelect();
        this.ModalDivActivate();
        var h = 400;
        var w = 500; //screen.width * 0.7;
        var l = (screen.width - w) / 2;
        var t = (screen.height - h) / 2;
        var winprop = 'scrollbars= 1, height=' + h + ', width=' + w + ', top=' + t + ', left=' + l;
        if ((this._wndPopUp == null) || (this._wndPopUp.closed)) {
            this._wndPopUp = window.open(URL, 'mc', winprop);
            //enable div watcher 
            this._timer = window.setInterval(this.ModalDivWatcher, 500);
        }
    },

    ///<summary>
	///Constructor
	///</summary>
    initialize: function() {
        Mediachase.Cms.Util.callBaseMethod(this, 'initialize');
        this._focusHandler = Function.createDelegate(this, this.ModalPopUp);
        $addHandler(window, "focus", this._focusHandler);
        if (this._isDesignMode == true)
            this.StartInitScript(this.BrowserDetect());
        __currentCmsUtil = this;
    },

	///<summary>
	///Destructor
	///</summary>
    dispose: function() {
        $removeHandler(window, "focus", this._focusHandler);
        this._focusHandler = null;
        Mediachase.Cms.Util.callBaseMethod(this, 'dispose');
    }
}

Mediachase.Cms.Util.registerClass('Mediachase.Cms.Util', Sys.Component);
