NWTD = NWTD || {};
NWTD.CartList = {
    controlIDs: [],
    refresh: function() {
        for (var i = 0; i < this.controlIDs.length; i++) {
            __doPostBack(this.controlIDs[i], '');
        }
    },
    hideInactive: function() {
        var cartHeadings = $('.nwtd-cartlist-cartheader h3');
        cartHeadings.not('.selected').parent().next().toggle(false);
    }
};



jQuery(document).ready(function($) {

    var prm = Sys.WebForms.PageRequestManager.getInstance();
    NWTD.CartList.hideInactive();
    //event to fire when the cart viewer re-loads via ajax
    //TODO: this will actually fire during all ASP.NET AJAX requests. Needs to be fixed.
    prm.add_endRequest(function(sender, args) {
        NWTD.CartList.hideInactive();
    });

    var cartHeadings = $('.nwtd-cartlist-cartheader h3');
    cartHeadings.live('click', function() {
        $(this).parent().next().slideToggle('fast');
    });

});