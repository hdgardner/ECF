var NWTD = NWTD || {};

//Ajax proxy functions for WCF services
NWTD.Ajax = {
	callService: function(serviceName, method, data, success, failure) {
		success = success || OakTree.Utilities.emptyFunction;
		failure = failure || OakTree.Utilities.emptyFunction;
		$.ajax({
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json',
			data: OakTree.Utilities.serialize(data),
			url: unescape(NWTD.BaseURL) + 'Services/' + serviceName + '.svc/' + method,
			success: function(data) {
				success.call(this, data.d);
			},
			error: function(XMLHttpRequest, textStatus, errorThrown) {
				failure.call(this, textStatus);
			}
		});
	}
};

//Cart related functions
NWTD.Cart = {
	addItem: function(code, quantity, success, failure) {
		var data = { item: { Code: code, Quantity: quantity} };
		return NWTD.Ajax.callService('Cart', 'AddItem', data, success, failure);
	},

	addItems: function(items, success, failure) {
		var data = { items: items };
		return NWTD.Ajax.callService('Cart', 'AddItems', data, success, failure);
	},
	updateCart: function(cartName, newName, success, failure) {
		var data = { cartName: cartName, newName: newName };
		return NWTD.Ajax.callService('Cart', 'Update', data, success, failure);
	},
	deleteCart: function(cartName, success, failure) {
		var data = { cartName: cartName };
		return NWTD.Ajax.callService('Cart', 'Delete', data, success, failure);
	},
	createCart: function(cartName, success, failure, activate) {
		activate = activate || false;
		var data = { cartName: cartName, activate:activate };
		return NWTD.Ajax.callService('Cart', 'Create', data, success, failure);
	},
	copyCart: function(cartName, newName, success, failure, activate) {
		activate = activate || false;
		var data = { cartName: cartName, newName: newName, activate:activate };
		return NWTD.Ajax.callService('Cart', 'Copy', data, success, failure);
	},
	selectCart: function(cartName, success, failure) {
		var data = { cartName: cartName };
		return NWTD.Ajax.callService('Cart', 'Select', data, success, failure);
	}
}