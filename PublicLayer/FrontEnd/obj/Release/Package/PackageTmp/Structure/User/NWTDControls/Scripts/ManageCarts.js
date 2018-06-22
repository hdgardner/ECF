jQuery(document).ready(function ($) {


	Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(function (sender, args) {
		var updatedPanels = $.map(args.get_panelsUpdated(), function (el, i) { return el.id });
		var update = false;
		$.each(OakTree.Web.UI.WebControls.ManageCarts, function (i, ManageCart) {
			if ($.inArray(ManageCart.updatePanelID, updatedPanels) != -1) {
				update = true;
			}
		});
		if (update) UpdateShoppingCart();
	}
	);


	var refreshCarts = function () {
		for (var i = 0; i < OakTree.Web.UI.WebControls.ManageCarts.length; i++) {
			__doPostBack(OakTree.Web.UI.WebControls.ManageCarts[i].updatePanelID, '');
		}
	};

	var editDialog = $('.nwtd-editCartDialog').modalDialog({
		title: 'Rename Wish List',
		onSubmit: function (e, data) {
			e.preventDefault();
			data = data[0];
			if (data.newName == data.cartName) {
				editDialog.modal.close();
				return;
			}
			NWTD.Cart.updateCart(
					data.cartName,
					data.newName,
					function (data) {
						if (data.Status != 0) {
							editDialog.modal.showMessage(data.Message);
							return;
						}
						editDialog.modal.close();
						refreshCarts();
					},
					function () {

					}
				);
		}
	});

	var deleteDialog = $('.nwtd-deleteCartDialog').modalDialog({
		title: 'Delete Cart',
		focus: 'input[value="Cancel"]',
		onSubmit: function (e, data) {
			e.preventDefault();
			data = data[0]
			NWTD.Cart.deleteCart(
					data.cartName,
					function (data) {
						if (data.Status != 0) {
							deleteDialog.modal.showMessage(data.Message, true);
							return;
						}
						deleteDialog.modal.close();
						refreshCarts();
					},
					function (data) {
						deleteDialog.modal.showMessage(data);
					}
				);

		}
	});

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
						refreshCarts();
					},
					function (data) {
						createDialog.modal.showMessage(data);
					},
					true //make the cart active
				);

		}
	});

	var copyDialog = $('.nwtd-copyCartDialog').modalDialog({
		title: 'Copy Cart',
		onSubmit: function (e, data) {
			e.preventDefault();
			data = data[0]
			NWTD.Cart.copyCart(
					data.sourceCartName,
					data.copyCartName,
					function (data) {
						if (data.Status != 0) {
							copyDialog.modal.showMessage(data.Message, false, true);
							return;
						}
						copyDialog.modal.close();
						refreshCarts();
					},
					function (data) {
						copyDialog.modal.showMessage(data);
					},
					true //make the cart active
				);

		}
	});

	var editButtons = $('.nwtd-editCart');
	var deleteButtons = $('.nwtd-deleteCart');
	var copyButtons = $('.nwtd-copyCart');
	var createButtons = $('.nwtd-createCart');

	editButtons.live('click', function () {
		var cartName = $(this).parent().find('input[type="hidden"]').val();
		editDialog.find('input[name="cartName"]').val(cartName);
		editDialog.find('input[name="newName"]').val(cartName);
		editDialog.modal.open();
		return false;

	});

	deleteButtons.live('click', function () {
		var cartToDelete = $(this).parent().find('input[type="hidden"]').val();
		deleteDialog.find('input[name="cartName"]').val(cartToDelete);
		deleteDialog.modal.showMessage('Are you sure you want to delete the cart named <b>' + cartToDelete + '</b>?');
		deleteDialog.modal.open();
		return false;
	});

	createButtons.live('click', function () {
		createDialog.modal.open();
		return false;
	});


	copyButtons.live('click', function () {
		var sourceCartName = $(this).parent().find('input[type="hidden"]').val();
		copyDialog.find('label').html('Enter a name for the copy of <b>' + sourceCartName + '</b>:');
		copyDialog.find('input[name="sourceCartName"]').val(sourceCartName);
		copyDialog.find('input[name="copyCartName"]').val('Copy of ' + sourceCartName);
		copyDialog.modal.open();
		return false;

	});

	$('.nwtd-dialogs').show();
});