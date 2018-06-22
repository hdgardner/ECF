jQuery(document).ready(function($) {

	//if someone selects a district from one dropdown, we should re-set the other
	var districtSelectors = $('.nwtd-districtSelector');
	districtSelectors.live('change', function() {
	if ($(this).val()) { //but only if they selected a value
			$('.nwtd-districtSelector').not(this).val('');
		}
	});

});