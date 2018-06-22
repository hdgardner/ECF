jQuery(document).ready(function($) {

	$.each(OakTree.Web.UI.WebControls.FlowPlayer, function(i, player) {
		var playerOptions = player.playerOptions || {};

		playerOptions.clip = playerOptions.clip || { autoPlay: false, scaling: 'scale' };
		playerOptions.clip.url = player.flv;

		if (player.controls) {
			playerOptions.plugins = playerOptions.plugins || {};
			playerOptions.plugins.controls = playerOptions.plugins.controls || {};
			playerOptions.plugins.controls.url = player.controls;
		}

		var fplayer = flowplayer(player.controlID, player.swf, playerOptions);

		if (player.usemodal) {
			var overlayOptions = player.overlayOptions || {};

			//require the API
			overlayOptions.api = true;

			//Make sure we don't run over any existing event listeners
			var originalOnload = overlayOptions.onLoad;
			overlayOptions.onLoad = function() {
				if ($.isFunction(originalOnload)) originalOnload.apply(this, arguments);
				fplayer.load();
			};
			var originalOnClose = overlayOptions.onClose
			overlayOptions.onClose = function() {
				if ($.isFunction(originalOnClose)) originalOnClose.apply(this, arguments);
				fplayer.unload();
			};

			var playerDiv = $('#' + player.overlayID).overlay(overlayOptions);

			//for some reason this option isn't working, so we'll handle it ourselves
			if (overlayOptions.closeOnClick) $('body').click(function() { playerDiv.close(); });

			if (player.triggers) {
				var triggers = $(player.triggers);
				triggers.click(function(e) {
					e.preventDefault();
					playerDiv.load();
				});
			}
		} else {
			fplayer.load();
		}


	});
});