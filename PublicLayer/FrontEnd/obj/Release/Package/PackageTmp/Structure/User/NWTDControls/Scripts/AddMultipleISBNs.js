jQuery(document).ready(function() {

	var multipleISBNDialog = $('.nwtd-addMultipleISBNsDialog').modalDialog({ title: 'Add Multiple ISBNs' });
	var multipleISBNButton = $('input[value="Add multiple ISBNs"]');
	var multipleISBNSubmit = multipleISBNDialog.find('input[type="submit"]');
	var multipleISBNTextArea = multipleISBNDialog.find('textarea');

	var successDialog = $('<div class="nwtd-dialogBody"></div>').appendTo('body').modalDialog({ title: 'Add Multiple ISBNs Complete' });

	multipleISBNButton.click(function() {
		multipleISBNDialog.modal.open();

		return false;
	});

	multipleISBNSubmit.click(function() {
		var ISBNs = multipleISBNTextArea.val().split('\n');
		var items = [];

		$.each(ISBNs, function(i, val) {
			if (val != '') {
				items.push({ Code: val, Quantity: 1 });
			}
		});

		NWTD.Cart.addItems(items, function(data) {
			multipleISBNDialog.modal.close();
			multipleISBNTextArea.val('');
			successDialog.empty();
			successDialog.append('<p>' + data.Message + '</p>');
			successDialog.modal.open();
		});

		return false;
	});


});