jQuery(document).ready(function($) {
	var url = new NWTD.Utilities.url(document.URL);
	var selectedIndex = url.parameters.step - 1;
	var sections = $('.nwtd-checkoutSection');
	var nextButton = $('.nwtd-nextStepButton');
	var prevButton = $('.nwtd-prevStepButton');
	var submitButton = $('.nwtd-checkout input[type="submit"]');

	var showSection = function(index) {
		sections.hide().eq(index).show();
		nextButton.show();
		prevButton.show();
		submitButton.hide();
		if (index == 0) {
			prevButton.hide();
		}
		if (index == sections.length - 1) {
			nextButton.hide();
			submitButton.show();
		}
	};

	prevButton.click(function() {
		showSection(--selectedIndex);
		return false;
	});

	nextButton.click(function() {
		showSection(++selectedIndex);
		return false;
	});

	showSection(selectedIndex);
});