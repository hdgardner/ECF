// JScript File
function Mediachase_CompareProducts()
{
    // Properties
    this.CompareViewPageUrl = "";
  
    this.OpenCompareView = function(metaClassName)
    {
        var url = this.CompareViewPageUrl;
        if(metaClassName != null && metaClassName != '') url += '?mc=' + metaClassName;
        var win = window.open(url,'compare','width=1000,height=800,resizable=yes,scrollbars=yes');    
        win.focus();
    };
     
    this.ReceiveServerData = function(resultString)
    {
        var res = Sys.Serialization.JavaScriptSerializer.deserialize(resultString);
        var checkObj = $get(res.chbId);
        if (checkObj != null)
            checkObj.checked = res.check;
        
        var color = res.isError ? "red" : "#797878";
        
        if(res.refId != '')
        {
          __doPostBack(res.refId,'');
        } 
        CSCompareProducts.createMessage(res.resultMsg, color, 2000);
    };
    
    this.createMessage = function(text, color, time_to_show)
    {
        with(document.getElementById('AjaxMessage'))
        {
            style.visibility = "visible";
            innerHTML        = text;
            style.color      = color;
        }
        if ( time_to_show > 0 )
            setTimeout("CSCompareProducts.hideMessage()", time_to_show);
    }

     this.hideMessage = function()
     {
        with (document.getElementById('AjaxMessage'))
        {
		      innerHTML = "";
		      style.visibility = "hidden";
		      style.color = "#797878";
        }
     }
};

var CSCompareProducts = new Mediachase_CompareProducts();

