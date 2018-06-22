jQuery(document).ready(function($) {

	$.each(OakTree.Web.UI.WebControls.Overlay, function(i, overlay) {
		var triggers = $(overlay.triggers);

		var overlayOptions = overlay.overlayOptions || {};

		//require the API
		overlayOptions.api = true;

		var overlayDiv = $('#' + overlay.controlID).overlay(overlayOptions);

		overlay.api = overlayDiv;

		//for some reason this option isn't working, so we'll handle it ourselves
		//if (overlayOptions.closeOnClick) $('body').click(function() { overlayDiv.close(); });

		triggers.click(function(e) {
			e.preventDefault();
			overlayDiv.load();
		});
	});

});