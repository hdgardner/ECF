// JScript File
function Mediachase_Checkout()
{
    this.RedirectFromPopupWindow = function(url){
        if(self.opener != null && !self.opener.closed && url != null){
            self.opener.document.location.href = url;
            self.opener.focus();
        }
    };
};

var CSCheckout = new Mediachase_Checkout();