jQuery(document).ready(function($) {
	$.each(OakTree.Web.UI.WebControls.TabbedControl, function(i, tab) {
		var tabControl = $('#' + tab.controlID);
		var triggers = tabControl.find('ul.oakTree-tabList a');
		var containers = tabControl.find('div.oakTree-tabContainer');

		triggers.each(function() {
			var trigger = $(this);

			trigger.click(function(e) {
				e.preventDefault();
				triggers.not(trigger).removeClass('selectedTab');
				trigger.addClass('selectedTab');
				containers.eq(triggers.index(trigger)).show();
				containers.not(containers.eq(triggers.index(trigger))).hide();
			});
		});

	});
});