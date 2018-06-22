var OakTree = OakTree || {};
OakTree.Utilities = (function() {
	//private
	var _specialChars = { '\b': '\\b', '\t': '\\t', '\n': '\\n', '\f': '\\f', '\r': '\\r', '"': '\\"', '\\': '\\\\' };
	var _replaceChars = function(chr) {
		return _specialChars[chr] ||
		 '\\u00' + Math.floor(chr.charCodeAt() / 16).toString(16) + (chr.charCodeAt() % 16).toString(16);
	};

	/**
	* public object combineObj([boolean deep,] object target, object options)
	* Merges the second object into the first, overwriting any shared members.
	* @param {Boolean} deep Causes any member object to be recursively merged
	* @param {Object} target
	* @param {Object} options 
	* @returns {Object} The merged object
	*/
	var _combineObj = function(deep, target, options) {
		var target = arguments[0] || {}, i = 1, length = arguments.length, deep = false, options;
		if (_getType(target) == 'boolean') {
			deep = target;
			target = arguments[1] || {};
			// skip the boolean and the target
			i = 2;
		}
		if (typeof target != "object" && typeof target != "function")
			target = {};

		for (; i < length; i++)
			if ((options = arguments[i]) != null) {
			for (var name in options) {
				// Prevent never-ending loop
				if (target === options[name])
					continue;
				if (deep && options[name] && typeof options[name] == "object" && target[name] && !options[name].nodeType)
					target[name] = _combineObj(target[name], options[name]);
				else
					if (options[name] != undefined)
					target[name] = options[name];

			}
		}
		return target;
	};

	/**
	* public string getType(object obj)
	* And extened versin of JavaScript's typeof operator
	* Includes the types 'array,''arguments,''element,'textnode','whitespace'
	* Gets the type of object
	* @param {Object} obj The object to test
	* @returns {String} The object's type
	*/
	var _getType = function(obj) {
		if (obj == undefined) return false;
		if (obj.nodeName) {
			switch (obj.nodeType) {
				case 1: return 'element';
				case 8: return 'comment';
				case 3: return (/\S/).test(obj.nodeValue) ? 'textnode' : 'whitespace';
			}
		} else if (typeof obj.length == 'number') {
			if (obj.callee) return 'arguments';
			else if (obj.item) return 'collection';
		}
		switch (typeof obj) {
			case 'object':
				if (obj.constructor == Array) {
					return 'array';
				}
			default:
				return typeof obj;
		}
	};

	/**
	* public boolean isDefined(object obj)
	* @param {Object} obj
	* @returns {Boolean}
	*/
	var _isDefined = function(obj) {
		return obj != undefined;
	};

	/**
	* public string serialize(object obj)
	* Serializes a JavaScript object into a string. Does not include undefined values or DOM objects.
	* @param {Object} obj The object to serialize
	* @returns{String} The serialized object as a string
	*/
	var _serialize = function(obj, ignoreNumbers) {
		switch (_getType(obj)) {
			case 'string':
				return '"' + obj.replace(/[\x00-\x1f\\"]/g, _replaceChars) + '"'; ;
			case 'array':
				var len = obj.length;
				var res = new Array();
				for (var i = 0; i < len; i++) {
					if (i in obj) {
						var str = _serialize.call(this, obj[i]);
						if (_isDefined(str))
							res.push(str);
					}
				}
				return '[' + String(res) + ']';
			case 'number':
				if (ignoreNumbers) return obj;
				return String(obj);
			case 'boolean':
				return String(obj);
			case 'object':
				var string = [];
				for (var prop in obj) {
					var val = _serialize(obj[prop]);
					if (val) string.push(_serialize(prop) + ':' + val);
				}
				return '{' + String(string) + '}';
			case false:
				return 'null';
		}
		return null;
	};

	/**
	* public object deserialize(sring string [,boolean secure]);
	* Evaluates a serialized JavaScript object
	* @param {String} string The string to evaluate
	* @param {Boolean} secure Checks the string against a regular expression if true
	* @returns {Object} The evaluated object
	*/
	var _deserialize = function(string, secure) {
		if (_getType(string) != 'string' || !string.length) return null;
		if (secure && !(/^[\],:{}\s]*$/.test(string.replace(/\\["\\\/bfnrtu]/g, '@').
                    replace(/"[^"\\\n\r]*"|true|false|null|-?\d+(?:\.\d*)?(?:[eE][+\-]?\d+)?/g, ']').
                    replace(/(?:^|:|,)(?:\s*\[)+/g, '')))) return null;


		return eval('(' + string + ')');
	};

	var _url = function(url, parameters) {
		parameters = parameters || {};
		var queryStringParameters = {};
		this.hash = '';
		var hashSplit = url.split("#");
		if (hashSplit.length == 2) {
			this.hash = '#' + hashSplit[1];
			url = hashSplit[0];
		}
		var querySplit = url.split('?');
		if (querySplit.length == 2) {
			queryStringParameters = OakTree.Utilities.parseQueryString(url);
			url = querySplit[0];
		}

		for (var parameter in parameters) {
			queryStringParameters[parameter] = parameters[parameter];
		}

		this.parameters = queryStringParameters;
		this.url = url;
	}
	_url.prototype = {
		toString: function() {
			return this.url + this.getQueryString() + this.hash;
		},
		getQueryString: function() {
			return OakTree.Utilities.createQueryString(this.parameters);
		}

	}


	return {
		serialize: _serialize,
		deserialize: _deserialize,
		getType: _getType,
		isDefined: _isDefined,
		combineObj: _combineObj,
		url: _url,
		emptyFunction: function() { },
		createQueryString: function(obj) {
			var pairs = [];
			for (var pair in obj) {
				pairs.push(pair + '=' + obj[pair]);
			}
			return '?' + pairs.join('&');
		},
		parseQueryString: function(querystring) {
			var output = {};
			//make sure there's no question mark
			if (querystring.split("?").length == 2) querystring = querystring.split("?")[1];
			//make sure there's no hash
			querystring = querystring.split("#")[0];
			//add each k/v to the object
			var pairs = querystring.split('&');
			for (var i = 0; i < pairs.length; i++) {
				var pair = pairs[i].split('=');
				output[pair[0]] = pair[1];
			}
			return output;
		}

	};

})();