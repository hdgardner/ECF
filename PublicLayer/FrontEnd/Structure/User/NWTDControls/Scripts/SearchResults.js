
//because sometimes the search results are in an update panel, we need to break this
//into a function that can be called from other sources, such as a page with this control in an update panel
//an example is the book subsearch control in the Templates

//alert(OakTree.Web.UI.WebControls);

OakTree.Web.UI.WebControls.SearchResults = OakTree.Web.UI.WebControls.SearchResults || {};

OakTree.Web.UI.WebControls.SearchResults.update = function () {

	var cartSelected = false; //a flag so we don't pop up the cart slection modal more than once during a page load

	var selectNewCartDiv = $('.nwtd-selectNewCart-sr');
	var selectCartList = $('.nwtd-selectCartList-sr');

	selectCartList.click(function () { showOrHideCartName(); });

	var showOrHideCartName = function () {
		if (!selectCartList.val()) { selectNewCartDiv.show() }
		else { selectNewCartDiv.hide() }

	};
	showOrHideCartName();

	var selectCartDialog = $('.selectCartDialog').modalDialog({
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
				addItemsToCart(); ; //add the items to the cart
				UpdateShoppingCart(); //this is a global function that updates the cart in the top right corner
			};
			var cartChangeFailure = function (data) {
				selectCartDialog.modal.showMessage(data);
			};


			var selectedCart = $(this).parent().find('select').val();
			if (!selectedCart) { //we're creating a new cart				
				var newCartName = $(this).parent().find('.nwtd-selectNewCartName').val();
				if (!newCartName) {
					selectCartDialog.modal.showMessage("You must enter a cart name.");
					return;
				}
				selectCartDialog.modal.showMessage("creating cart:" + newCartName);
				NWTD.Cart.createCart(
					newCartName,
					cartChangeSuccess,
					cartChangeFailure,
					true //activate the cart
				);
			}
			else {
				NWTD.Cart.selectCart(
					selectedCart,
					cartChangeSuccess,
					cartChangeFailure
				);
			}
		}
	});



	//set up a bunch of variables to store in the closure
	var successModalDialog = $('.nwtd-addToCartSuccess').modalDialog({ title: 'Items Added to Wish List' });
	successModalDialog.find('a[href="#"]').click(function () { successModalDialog.modal.close(); return false; });
	var itemsAddedList = successModalDialog.find('.nwtd-dialogItemsAdded');
	var errorList = successModalDialog.find('.nwtd-dialogErrors');
	var searchResultsForm = $('.nwtd-searchResults');

	$('.nwtd-dialogBody').css('display', 'block'); //they were hidden using css to avoid the peekaboo. now we can show them

	var submitButton = $('.nwtd-searchResultsContainer').find('.nwtd-addSelectedToCart').attr('disabled', 'disabled');
	var entryCheckBoxes = $('.nwtd-selectEntry');

	var clearSelections = function () {
		$('.nwtd-selectEntry input[type="checkbox"]:checked').removeAttr("checked");
		submitButton.attr('disabled', 'disabled');
	};

	//handle clicking the 'clear selections' button
	$('.nwtd-clear-selections').live('click', function (e) {
		e.preventDefault();
		clearSelections();
		return false;
	});

	//only make the submit button availble if a checkbox is checked
	$('.nwtd-searchResultsContainer').live('click', function (e) {
		if ($(e.target).is('input[type="checkbox"]')) {
			if (searchResultsForm.find(':checked').length) {
				submitButton.removeAttr('disabled');
			} else {
				submitButton.attr('disabled', 'disabled');
			}
		}
	});

	//handle submission


	var addItemsToCart = function () {

		if (!searchResultsForm.find(':checked').length) return; //make sure something's actually checked.

		var lineItemRows = $('.nwtd-searchResults').find('.nwtd-table-row, .nwtd-table-row-alt, .nwtd-table-row-selected');

		var valid = true;
		var lineItemData = []; //this will be the data we submit to our entry adding service
		var insertErrorMessage = function (el) { el.insertAfter($(this)); }; //function to add a validation error
		var removeErrorMessage = function () { $(this).parent().find('.warning').remove(); }; //functiont to remove a validation error
		itemsAddedList.empty(); //clear out the list of items added
		lineItemRows.each(function () { //loop through each row of the search results table
			var quantityInput = $(this).find('input[type="text"]');
			var quantity = 0;

			//if we have quantity boxes
			if (quantityInput.length > 0) {
				//reset validation
				quantityInput.valid({
					removeMessage: removeErrorMessage
				});
				quantity = quantityInput.val();
			}


			//find all checked items and validate that they have quantity added
			if ($(this).find(':checked').length) {
				//ADD THIS BACK IF THEY DECIDE TO HAVE QUANTITIES
				//				if (quantity == ) {
				//					valid = false;
				//					quantityInput.invalid('You must supply a quantity', {
				//						insertMessage: insertErrorMessage
				//					});
				//				};
				//make sure that the quantities are numbers
				quantityInput.validate({
					integer: true,
					failure: function (message) {
						valid = false;
						$(this).invalid(message, { insertMessage: insertErrorMessage });
					}
				});
				if (valid) {
					lineItemData.push({ 'Code': $(this).find('input[type="hidden"]').val(), 'Quantity': quantity });

				}
			}
		});

		//if everything is valid, run the code to insert the new items via an XHR to our WCF service:
		if (valid) {

			NWTD.Cart.addItems(
				lineItemData,
				function (data) {
					//clear out all the checkboxes and quantities
					lineItemRows.find('input[type="text"]').val('');
					lineItemRows.find('input[type="checkbox"]').removeAttr('checked');

					itemsAddedList.empty();
					errorList.empty();
					//tell the user what got added
					$.each(data.ItemsAdded, function (i, val) { //the items that were sucessfully added
						//itemsAddedList.append($('<li></li>').prepend(lineItemRows.find('input[value="' + val.Code + '"]').parent().parent().find('a').clone()));
						itemsAddedList.append($('<li>' + val.Code + '</li>'));
					});
					//tell the user what didn't get added
					if (data.Status == 2) { //there was a warning
						errorList.append('<li>' + data.Message + '</li>');
					}
					//show the modal dialog
					successModalDialog.modal.open();
				},
				function (data) {
					//console.log(data);
				}
			);
		}
		submitButton.attr('disabled', 'disabled');
		return false;
	}

	submitButton.click(function (e) {
		e.preventDefault();
		if (selectCartDialog.length && !cartSelected) {
			selectCartDialog.modal.open();
		}
		else {
			addItemsToCart();
		}
		return false;
	});
};

jQuery(document).ready(function($) {
	OakTree.Web.UI.WebControls.SearchResults.update();
});

