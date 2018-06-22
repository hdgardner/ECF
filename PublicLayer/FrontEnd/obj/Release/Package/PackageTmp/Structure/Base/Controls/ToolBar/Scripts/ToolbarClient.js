// JScript File
function Mediachase_ToolbarClient()
{
    // Properties
    
    // Method Mappings   
    this.OnTabChanged = function(sender, eventArgs)
    {
        var tabNewVal = eventArgs.get_tab().get_value();
      
        if(tabNewVal != 'DesignView')
        {
            var question = 'Do you want to save current page?';
            if (confirm(question))
            {
                eventArgs.set_cancel(true);
                sender.selectTabById('DesignView');
                return false;
            }
        }        
        
        if(tabNewVal == 'ManagementView')
        {
            CSToolbarClient.SwitchDesignMode('false');//RunCommand('Cancel');
        }
        else if(tabNewVal == 'DesignView')
        {
            CSToolbarClient.SwitchDesignMode('true');RunCommand('Edit');
        }
        else
        {
            CSToolbarClient.SwitchDesignMode('false');RunCommand('Cancel');
        }        
    };  
    
    this.ToolbarClick = function(contentId)
    {
		var objContent = document.getElementById(contentId);
		if (objContent == null) { alert('Element with ['+contentId+'] not found!'); return; }
		if (objContent.style.display != "none")
		{ 
			objContent.style.display = "none";
			if (contentId == 'showimage') { document.getElementById('ToolBarVisible').value = 'False'; }
			if (contentId == 'showimageT') 
			{   
				document.getElementById('ToolBoxVisible').value = 'False';
			}
		}
		else
		{ 
			objContent.style.display="block"; 
			if (contentId == 'showimage') { document.getElementById('ToolBarVisible').value = 'True'; }
			if (contentId == 'showimageT')
			{
				document.getElementById('ToolBoxVisible').value = 'True';
			}
		}
    };
    
    this.OnToolbarItemChecked = function(sender, eventArgs)
    {
        var val = eventArgs.get_item().get_value();
        if(val == 'Toolbox')
        {
            CSToolbarClient.ToolbarClick('showimageT'); //minimizeToolBar('showimage', 'False');
        }
    /*
        eventArgs.get_item().get_text();

        var tabNewVal = eventArgs.get_tab().get_value();
        var tabSelectedVal = sender.getSelectedTab().get_value();
        
        if(tabSelectedVal == 'DesignView')
        {
            var question = 'Do you want to save current page?';
            if (confirm(question))
            {
                eventArgs.set_cancel(true);
            }
        }
        */
    };  

    this.SwitchDesignMode = function(isDesign)    
    {
        var obj = document.getElementById('IsDesignMode');
        obj.value = isDesign;
    };
    
    this.Session = new MC_SessionHandler;    
};

function MC_SessionHandler()
{
    this.Save = function(name,value,days)
    {
    	if (days) {
	    	var date = new Date();
		    date.setTime(date.getTime()+(days*24*60*60*1000));
		    var expires = "; expires="+date.toGMTString();
	    }
	    else var expires = "";
	    document.cookie = name+"="+value+expires+"; path=/";
    };

    this.Read = function(name) 
    {
    	var nameEQ = name + "=";
	    var ca = document.cookie.split(';');
    	for(var i=0;i < ca.length;i++) {
	    	var c = ca[i];
		    while (c.charAt(0)==' ') c = c.substring(1,c.length);
    		if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length,c.length);
	    }
	    return null;
	};

    this.Remove = function(name) 
    {
	    createCookie(name,"",-1);
    };

}

var CSToolbarClient = new Mediachase_ToolbarClient();



