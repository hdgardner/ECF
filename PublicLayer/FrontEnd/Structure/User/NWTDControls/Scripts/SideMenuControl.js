jQuery(document).ready(function ($) {
	var cookie = new OakTree.Web.UI.CookieObject('NWTD');
	var facetKey = function (facetName) {
		return 'facet-' + facetName;
	}


	var facetNames = $('.nwtd-searchFacetCategoryName');
	facetNames.each(function () {

		var target = $(this).next();
		var trigger = $(this).wrapInner('<a href="#"></a>');
		var triggerLink = $(this).parent();
		trigger.click(function (e) {
			e.preventDefault();

			var facetName = facetKey($(this).text());
			var facets = $(this).next().slideToggle('fast', function (e, t) {
				var target = $(this);
				if (!target.is(':visible')) {
					cookie.setValue(facetName, false);
					trigger.addClass('nwtd-facetHidden');
					trigger.removeClass('nwtd-facetShown');
				} else {
					cookie.setValue(facetName, true);
					//cookie.deleteValue(facetName);
					trigger.addClass('nwtd-facetShown');
					trigger.removeClass('nwtd-facetHidden');
				}
				return false;
			});
		});

		if (cookie.getValue(facetKey(trigger.text())) === true) {
			trigger.next().show();
			trigger.addClass('nwtd-facetShown');
		} else {
			trigger.next().hide();
			trigger.addClass('nwtd-facetHidden');
		}
	});

});