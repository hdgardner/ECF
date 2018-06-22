jQuery(document).ready(function($) {

//although this functionality was cool, we have to ditch it
	/*
	$('.nwtd-SearchByPublisher').each(function() {

		var searchBox = $(this).find('.nwtd-search-keyword');
		var submitButton = $(this).find('input[type=submit]');


		var originalValue = searchBox.val(); //store the original value in the closure
		if (!originalValue) {
			originalValue = 'ISBN or Keyword';
			searchBox.val(originalValue);
		}
		//if someone clicks it or enters it with the tab key, and the value is the original value, clear it out
		searchBox.bind('click', function() {
			if (searchBox.val() == originalValue) searchBox.val('');
		});
		//when they leave the input, if they have left it blank, put it back to the stored original value
		searchBox.bind('blur', function() {
			originalValue = searchBox.val();
			if (!searchBox.val()) searchBox.val(originalValue);
		});

		submitButton.click(function() {
			//if (searchBox.val() == originalValue) searchBox.val('');
		});

	});
*/
	$('.nwtd-SearchByPublisher').each(function() {

		var searchBox = $(this).find('.nwtd-search-keyword');
		var submitButton = $(this).find('input[type=submit]');

		var originalValue = 'Enter ISBN or Keyword';

		//if someone clicks it or enters it with the tab key, and the value is the original value, clear it out
		searchBox.bind('click', function() {
			if (searchBox.val() == originalValue) searchBox.val('');
		});
		searchBox.bind('blur', function() {
			if (!searchBox.val()) searchBox.val(originalValue);
		});

	});
});