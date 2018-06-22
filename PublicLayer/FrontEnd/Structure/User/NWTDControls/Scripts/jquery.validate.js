//create a jquery plugin for basic form validation
//syntax : myElement.validate({required:true,email:true}) and so on...
(function($) {
	$.fn.validate = function(options) {
		var settings = $.extend({
			required: false,
			email: false,
			phone: false,
			integer: false,
			success: function() { },
			failure: function() { }
		}, options);

		var validateInteger = function(val) {
			var i;
			for (i = 0; i < val.length; i++) {
				var c = val.charAt(i);
				if (((c < "0") || (c > "9"))) return false;
			}
			return true;
		};
		var validateEmail = function(val) {
			var trimmed = val.replace(/^\s+|\s+$/, '');
			var emailFilter = /^[^@]+@[^@.]+\.[^@]*\w\w$/;
			var illegalChars = /[\(\)\<\>\,\;\:\\\"\[\]]/;

			if (!emailFilter.test(trimmed) || val.match(illegalChars)) {
				return false;
			}
			return true;
		};

		var validatePhone = function(val, minDigits) {
			var digits = "0123456789";
			var phoneNumberDelimiters = "()- ";
			var validWorldPhoneChars = phoneNumberDelimiters + "+";
			var minDigitsInIPhoneNumber = minDigits || 10;

			var stripCharsInBag = function(s, bag) {
				var i;
				var returnString = "";
				for (i = 0; i < s.length; i++) {
					var c = s.charAt(i);
					if (bag.indexOf(c) == -1) returnString += c;
				}
				return returnString;
			}
			var s = stripCharsInBag(val, validWorldPhoneChars);
			return (validateInteger(s) && s.length >= minDigitsInIPhoneNumber);

		};
		return this.each(function() {
			var val = (this.nodeName.toLowerCase() == 'input' || this.nodeName.toLowerCase() == 'textarea' || this.nodeName.toLowerCase() == 'select') ? $(this).val() : $(this).text();
			if (val === '') {
				if (settings.required) {
					settings.failure.call(this, 'Required Field');
					return;
				}
				settings.success.call(this);
				return;
			}
			if (settings.integer && !validateInteger(val)) {
				settings.failure.call(this, 'Not a number');
				return;
			}
			if (settings.email && !validateEmail(val)) {
				settings.failure.call(this, 'Invalid Email')
				return;
			}
			if (settings.phone && !validatePhone(val)) {
				settings.failure.call(this, 'Invalid Phone Number')
				return;
			}
			settings.success.call(this);
		});
	};

	$.fn.invalid = function(message, options) {
		var settings = $.extend({
			insertMessage: function(errorMessageElement) {
				var label = $('label[for=' + this.id + ']');
				errorMessageElement.insertAfter(label);
			},
			errorMessageElement: function(message) {
				return $('<div class="warning">' + message + '</div>');
			}
		}, options);

		return this.each(function() {
			var errorMessageElement = settings.errorMessageElement(message);
			settings.insertMessage.call(this, errorMessageElement);
		});
	};
	$.fn.valid = function(options) {
		var settings = $.extend({
			removeMessage: function() {
				var label = $('label[for=' + this.id + ']');
				label.find('.invalid').remove();
			}

		}, options);

		return this.each(function() {
			settings.removeMessage.call(this);
		});
	};
})(jQuery);