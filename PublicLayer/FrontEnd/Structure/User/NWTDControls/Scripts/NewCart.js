jQuery(document).ready(function ($) {

	var createButtons = $('.nwtd-createCart');
	var createDialog = $('.nwtd-createCartDialog').modalDialog({
		title: 'New Wish List',
		onSubmit: function (e, data) {
			e.preventDefault();
			data = data[0]
			NWTD.Cart.createCart(
					data.createCartName,
					function (data) {
						if (data.Status != 0) {
							createDialog.modal.showMessage(data.Message);
							return;
						}
						createDialog.modal.close();
						window.location = unescape(NWTD.BaseURL) + 'Cart/Manage.aspx';
					},
					function (data) {
						createDialog.modal.showMessage(data);
					},
					true //make the cart active
				);

		}
	});

	createButtons.live('click', function () {
		createDialog.modal.open();
		return false;
	});

	createDialog.show();

});