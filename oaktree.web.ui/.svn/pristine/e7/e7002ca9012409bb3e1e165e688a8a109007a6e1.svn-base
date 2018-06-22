var OakTree = OakTree || {};
OakTree.Web = OakTree.Web || {};
OakTree.Web.UI = OakTree.Web.UI || {};

/**
* public static Cookie
* A singleton that sets/gets/deletes HTTP cookies
* @example 
* 	OakTree.Web.UI.Cookie.setValue('foo','bar'); //sets the cookie value 'foo' to 'bar'
* 	OakTree.Web.UI.Cookie.getValue('foo'); //returns 'bar'
*/
OakTree.Web.UI.Cookie = (function() {
	/**
	* public static string getValue(string name);
	* @param {String} name
	*/ 
	var _getValue = function(name) {
		var start = document.cookie.indexOf(name + "=");
		var len = start + name.length + 1;
		if ((!start) && (name != document.cookie.substring(0, name.length))) {
			return null;
		}
		if (start == -1) return null;
		var end = document.cookie.indexOf(';', len);
		if (end == -1) end = document.cookie.length;
		return unescape(document.cookie.substring(len, end));
	};
	/**
	* public static void setValue(string name,string value [[[[,date expires],string path],string domain],boolean secure])
	* @param {String} name
	* @param {String} value
	* @param {Date} expires
	* @param {String} path
	* @param {String} domain
	* @param {Boolean} secure
	*/
	var _setValue = function(name, value, expires, path, domain, secure) {
		var today = new Date();
		today.setTime(today.getTime());
		if (expires) {
			expires = expires * 1000 * 60 * 60 * 24;
		}
		var expires_date = new Date(today.getTime() + (expires));
		document.cookie = name + '=' + escape(value) +
			((expires) ? ';expires=' + expires_date.toGMTString() : '') + //expires.toGMTString()
			((path) ? ';path=' + path : '') +
			((domain) ? ';domain=' + domain : '') +
			((secure) ? ';secure' : '');
	};
	/**
	* pulic static void deleteValue(string name[[,string path],string domain])
	* @param {String} name
	* @param {String} path
	* @param {String} domain
	*/
	var _deleteValue = function(name, path, domain) {
		if (_getValue(name)) document.cookie = name + '=' +
		((path) ? ';path=' + path : '') +
		((domain) ? ';domain=' + domain : '') +
		';expires=Thu, 01-Jan-1970 00:00:01 GMT';
	};
	return {
		getValue: _getValue,
		setValue: _setValue,
		deleteValue: _deleteValue
	};
})();

/**
* An object that can store key/value pairs in a single HTTP cookie
* Accepts strings as keys and JavaScript objects as values.
* @param {Object} name The name of the cookie
* @param {Object} options
* @option {Date} expires The exiration date of the cookie
* 	Defaults to the current session.
* @option {String} path 
* @option {String} domain
* @option {Boolean} secure
* @constructor
* @example
* 	var myCookie = new OakTree.Web.UI.CookieObject('myCookie');
* 	myCookie.setValue('foo','bar'); //sets the key/value pair
* 	myCookie.setValue('anArray',['this','is','an','array',{
* 		'with':'anObject!'} ]; //sets another key/value pair
* 	myCookie.getValue('foo'); //returns 'foo';
*  myCookie.getValue('anArray'); //returns an Array
*/
OakTree.Web.UI.CookieObject = function(name, options) {
	this.name = name;
	this.settings = OakTree.Utilities.combineObj({
		autoSave: true,
		expires: false,
		path: false,
		domain: false,
		secure: false
	}, options);
	this.load();
};
OakTree.Web.UI.CookieObject.prototype = {
	/**
	* Loads the cookie into the object

	*/
	load: function() {
		this.obj = OakTree.Utilities.deserialize(
			OakTree.Web.UI.Cookie.getValue(this.name)) || {};
	},
	/**
	* Saves the object into the cookie
	*/
	save: function() {
		var cookiestr = OakTree.Utilities.serialize(this.obj);
		if (cookiestr.length > 4096) return false;
		//OakTree.Web.UI.Cookie.deleteValue(this.name,this.settings.path,this.settings.domain);
		OakTree.Web.UI.Cookie.setValue(
			this.name,
			cookiestr,
			this.settings.expires,
			this.settings.path,
			this.settings.domain,
			this.settings.secure
		);
	},
	/**
	* Adds a key/value pair to the cookie object
	* @param {String} key
	* @param {Object} value
	*/
	setValue: function(key, value) {
		this.obj[key] = value;
		if (this.settings.autoSave) this.save();
	},
	/**
	* Retrieves a value from the cookie object
	* @param {String} key
	*/
	getValue: function(key) {
		return this.obj[key];
	},
	/**
	* Deletes a value from the cookie object
	* @param {String} key
	*/
	deleteValue: function(key) {
		delete this.obj[key];
		if (this.settings.autoSave) this.save();
	}

};

