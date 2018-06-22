jQuery(document).ready(function($) {


	var deleteDialog = $('.nwtd-deleteCartDialog').modalDialog({
		title: 'Delete Cart',
		focus: 'input[value="Cancel"]'
	});

	var deleteCartButton = $('.nwtd-deleteCart');
	
	deleteCartButton.click(function() {
		deleteDialog.modal.open();
		return false;
	});


});