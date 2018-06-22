(function($) {
	var overlay = $('<div class="jquery-modal-overlay"  />').css('opacity', .75).hide();

	$.fn.serializeForm = function() {
		var output = [];
		this.each(function() {
			var values = {};
			var el = $(this);
			var inputs = el.find('input[type="text"], input[type="hidden"], textarea, select');

			inputs.each(function() {
				var input = $(this);
				values[input.attr('name')] = input.val();
			});
			output.push(values);
		});
		return output;
	}

	$.fn.modalDialog = function(options) {
		var modals = this;



		overlay.click(function() {
			modals.modal.close();
		});

		overlay.prependTo('body');


		var settings = $.extend({
			title: null,
			height: null,
			width: '400px',
			onSubmit: function() { modals.modal.close(); },
			onCancel: function() { modals.modal.close(); },
			onOK: function() { modals.modal.close(); },
			focus: 'input[type="text"]',
			closeButtonClass: 'close-button'
		}, options);

		this.modal = this.modal || {}
		this.modal.message = $('<p class="nwtd-modal-message" />');

		this.modal.open = function() {
			overlay.height($(document).height());
			modals.modal.enableSubmit();

			//modals.parent().center(false, true).css({ 'height': '' }).show();
			var dialog = modals.parent();

			dialog.css({ 'height': '' });
			dialog.parent().show();
			dialog.css({
				'top': ($(window).height() / 2) - (dialog.height() / 2) + $('html').scrollTop(),
				'left': ($(window).width() / 2) - (dialog.width() / 2) + $('html').scrollLeft()
			});



			modals.find(settings.focus).eq(0).focus().select();
			overlay.show();
		};

		this.modal.close = function() {
			this.message.remove();
			modals.parent().parent().hide();
			overlay.hide();
		};

		this.modal.showMessage = function(message, disableSubmit, error) {
			if (error) {
				modals.modal.message.addClass('nwtd-error');
			}
			else {
				modals.modal.message.removeClass('nwtd-error');
			}
			modals.modal.message.html(message);
			modals.prepend(this.message);
			modals.modal.open();
			if (disableSubmit) modals.modal.disableSubmit();
		};

		this.modal.disableSubmit = function() {
			modals.find('input[type="submit"]').attr('disabled', 'disabled');
		};

		this.modal.enableSubmit = function() {
			modals.find('input[type="submit"]').removeAttr('disabled');
		};

		return this.each(function() {
			var el = $(this);
			var dialogWrapper = $('<div class="jquery-modal-container-wrapper"  />').hide();

			if ($.browser.msie) {
				dialogWrapper.css({
					position: 'absolute',
					'z-index': 1000,
					height: '100%',
					width: '100%',
					//display: 'block',
					top: 0,
					left: 0
				});
			}

			var dialog = $('<div class="jquery-modal-container" />');
			var closebutton = $('').click(function() { modals.modal.close(); return false; });
			if (settings.title) {
				dialog.prepend($('<h1>' + settings.title + '</h1>').append(closebutton));
			}
			else {
				el.prepend(closebutton);
			}
			dialogWrapper.append(dialog);
			var form = $('form').eq(0);
			form.append(dialogWrapper);

			//dialogWrapper.insertAfter(el);
			//dialog.prependTo('body');
			//dialogWrapper.css('z-index', dialog.css('z-index') + 1); //just do this in CSS

			el.appendTo(dialog);

			//dialog.prependTo('body'); //since this is asp.net, we need to add it to the form element, not the body

			if (settings.height) dialog.css('height', settings.height);

			dialog.css({
				'width': settings.width
			});

			el.find('input[type="submit"]').click(function(e) {
				settings.onSubmit.call(this, e, el.serializeForm());
			});

			el.find('input[type="button"][value="OK"]').click(function() {
				settings.onOK.call(this);
			});

			el.find('input[type="button"][value="Cancel"]').click(function() {
				settings.onCancel.call(this);
				return false;
			});

		});

	};

	$.fn.modalTrigger = function(options) {


	};

	jQuery.fn.center = function(options) {

		var settings = $.extend({
			vertical: true,
			horizontal: true
		}, options);

		return this.each(function() {
			//initializing variables
			var $self = jQuery(this);
			//get the dimensions using dimensions plugin
			var width = $self.width();
			var height = $self.height();
			//get the paddings
			var paddingTop = parseInt($self.css("padding-top"));
			var paddingBottom = parseInt($self.css("padding-bottom"));
			//get the borders
			var borderTop = parseInt($self.css("border-top-width"));
			var borderBottom = parseInt($self.css("border-bottom-width"));
			//get the media of padding and borders
			var mediaBorder = (borderTop + borderBottom) / 2;
			var mediaPadding = (paddingTop + paddingBottom) / 2;
			//get the type of positioning
			var positionType = $self.parent().css("position");
			// get the half minus of width and height
			var halfWidth = (width / 2) * (-1);
			var halfHeight = ((height / 2) * (-1)) - mediaPadding - mediaBorder;
			// initializing the css properties
			var cssProp = {
				position: 'absolute'
			};

			if (settings.vertical) {
				cssProp.height = height;
				cssProp.top = '50%';
				cssProp.marginTop = halfHeight;
			}
			if (settings.horizontal) {
				cssProp.width = width;
				cssProp.left = '50%';
				cssProp.marginLeft = halfWidth;
			}
			//check the current position
			if (positionType == 'static') {
				$self.parent().css("position", "relative");
			}
			//aplying the css
			$self.css(cssProp);


		});

	};


})(jQuery);