jQuery(document).ready(function ($) {

	var isDirty = false;

	Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(function (sender, args) {
		var updatedPanels = $.map(args.get_panelsUpdated(), function (el, i) { return el.id });
		var update = false;
		$.each(OakTree.Web.UI.WebControls.Carts, function (i, Cart) {
			if ($.inArray(Cart.updatePanelID, updatedPanels) != -1) {
				checkForErrors();
				isDirty = false;
			}
		});
	});


	var submitButton = $('.nwtd-submitCart');
	var allSubmitButtons = $('.dontcheckdirty');

	var message = $('<div class="error">At least one quantity greater than zero must be entered for each item.</div>').insertAfter(submitButton).hide();

	var checkForErrors = function () {
		var errors = false;
		$('.nwtd-cartview .nwtd-table-row, .nwtd-cartview .nwtd-table-row-alt').each(function () {
			var row = $(this);
			var quantity = parseInt(row.find('.quantity input[type="text"]').val());
			var gratis = parseInt(row.find('.gratis input[type="text"]').val());
			if (gratis + quantity < 1) {
				errors = true;
				row.addClass('nwtd-cartrow-zero-quanity');
			}
			else
				row.removeClass('nwtd-cartrow-zero-quanity');
		});
		if (errors) {
			message.show();
		}
		else { message.hide() };
		return errors;
	};

	$('.gratis input[type="text"], .quantity input[type="text"]').live('blur', function () {
		checkForErrors();
	});


	$('.gratis input[type="text"], .quantity input[type="text"]').live('focus', function () {
		var input = $(this);
		var originalValue = input.val();
		input.one('blur', function () {
			if ($(this).val() != originalValue) {
				isDirty = true;
			}
		});
	});

	window.onbeforeunload = function (e) {
		//alert(isDirty);
		e = e || window.event;
		if (isDirty) {
			// For IE and Firefox
			if (e) {
				e.returnValue = "You have unsaved changes.";
			}
			// For Safari
			return "You have unsaved changes.";
		}
	};



	allSubmitButtons.live('click', function () {
		window.onbeforeunload = null;
	});

        submitButton.live('click', function() {
		return !checkForErrors();
        });

	checkForErrors();
});

