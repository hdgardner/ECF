jQuery(document).ready(function ($) {
	var addToCartDialog = $('.addToCartDialog').modalDialog({ title: 'Add Item to Wish List' });
	addToCartDialog.find('a[href="#"]').click(function () { addToCartDialog.modal.close(); return false; });
	var addToCartButton = $('.nwtd-AddToCart');
	var cartSelected = false; //a flag so we don't pop up the cart slection modal more than once during a page load
	$('.nwtd-dialogBody').css('display', 'block'); //they were hidden using css to avoid the peekaboo. now we can show them

	var selectCartDialog = $('.selectCartDialogItem').modalDialog({
		title: 'Select Active Wish List',
		onSubmit: function (e, data) {
			e.preventDefault();

			//some success and failure functions to fire when cart change is done
			var cartChangeSuccess = function (data) {
				if (data.Status != 0) {
					selectCartDialog.modal.showMessage('There was an error: ' + data.Message);
					return;
				}
				cartSelected = true; //we won't show this again 
				selectCartDialog.modal.close(); //close the modal
				addToCart(); //add the item to the cart
				UpdateShoppingCart(); //this is a global function that updates the cart in the top right corner
			};
			var cartChangeFailure = function (data) {
				selectCartDialog.modal.showMessage(data);
			};

			//the selected cart in the dropdown
			var selectedCart = $(this).parent().find('select').val();
			if (!selectedCart) { //we're creating a new cart				
				var newCartName = $(this).parent().find('.nwtd-selectNewCartName').val();
				if (!newCartName) {
					selectCartDialog.modal.showMessage("You must enter a wish list name.");
					return;
				}
				selectCartDialog.modal.showMessage("creating wish list:" + newCartName);
				NWTD.Cart.createCart(
					newCartName,
					cartChangeSuccess,
					cartChangeFailure,
					true //activate the cart
				);
			}
			else { //we're simply changing carts
				NWTD.Cart.selectCart(
					selectedCart,
					cartChangeSuccess,
					cartChangeFailure
				);
			}
		}
	});

	var selectCartButton = $('.nwtd-selectCart');
	var selectNewCartDiv = $('.nwtd-selectNewCart');
	var selectCartList = $('.nwtd-selectCartList');

	selectCartList.click(function () { showOrHideCartName(); });

	var showOrHideCartName = function () {
		if (!selectCartList.val()) { selectNewCartDiv.show() }
		else { selectNewCartDiv.hide() }

	};
	showOrHideCartName();

	addToCartButton.live('click', function (e) {
		if (selectCartDialog.length && !cartSelected) {
			selectCartDialog.modal.open();
		}
		else {
			addToCart();
		}
		return false;

	});

	var addToCart = function () {
		var code = addToCartButton.parent().find('input[type="hidden"]').val();
		NWTD.Cart.addItem(code, 0,
			function (data) {
				if (data.Status != 0) {
					addToCartDialog.modal.showMessage('There was an error: ' + data.Message, false, true);
					return;
				}
				var message = 'This item has been added to your wish list: ' + data.ItemsAdded[0].Code;
				addToCartDialog.modal.showMessage(message);

			},
			function (data) {
				addToCartDialog.modal.showMessage(data, false, true);
			}
		);
	};

	$('.addToCartDialogs').show(); // these were hidden to prevent a peekaboo
});