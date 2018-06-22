var OakTree = OakTree || {};
OakTree.Web = OakTree.Web || {};
OakTree.Web.UI = OakTree.Web.UI || {};

OakTree.Web.UI.WebControls = (function() {
	
	var WebControls = {
		registerControl: function(controlType, controlInfo) {
			this[controlType] = this[controlType] || [];
			switch (typeof (controlInfo)) {
				case 'string':
					this[controlType].push({ controlID: controlInfo });
					break;
				case 'object':
					this[controlType].push(controlInfo);
					break;
			}
		},
		refresh: function(controls) {
			if (OakTree.Utilities.getType(__doPostBack) != 'function') return;
			switch (OakTree.Utilities.getType(controls)) {
				case 'string':
					OakTree.Web.UI.WebControls.refresh(this[controls]);
					break;
				case 'array':
					for (var i = 0; i < controls.length; i++) {
						__doPostBack(controls[i].controlID, '');
					}
					break;
				default:
					return;
			}
		}	
	};
	return WebControls;
})();
