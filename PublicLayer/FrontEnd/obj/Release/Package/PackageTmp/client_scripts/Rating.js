var mc_rating_thankYouImage = mc_ThemeBase + "Images/review/saved.gif";

var mc_rating_starImages = new Array(mc_ThemeBase + "Images/review/not-rated-off.gif",
							mc_ThemeBase + "Images/review/1-star-off.gif",
							mc_ThemeBase + "Images/review/2-star-off.gif",
							mc_ThemeBase + "Images/review/3-star-off.gif",
							mc_ThemeBase + "Images/review/4-star-off.gif",
							mc_ThemeBase + "Images/review/5-star-off.gif");
var mc_rating_nullStarMessage = mc_ThemeBase + "Images/review/not-rated-msg.gif";
var mc_rating_starMap = new Array('0,0,22,20',
						'23,0,36,20',
						'37,0,50,20',
						'51,0,64,20',
						'65,0,78,20',
						'79,0,101,20');
var mc_rating_starMessages = new Array(mc_ThemeBase + "Images/review/not-rated-msg.gif",
							mc_ThemeBase + "Images/review/i-hate-it-msg.gif",
							mc_ThemeBase + "Images/review/i-dont-like-it-msg.gif",
							mc_ThemeBase + "Images/review/its-okay-msg.gif",
							mc_ThemeBase + "Images/review/i-like-it-msg.gif",
							mc_ThemeBase + "Images/review/i-love-it-msg.gif",
							mc_ThemeBase + "Images/review/saved.gif");

var mc_rating_savedRatings = new Array();
var mc_rating_changedRatings = new Array();
var mc_rating_starTwinkler = new Array();
var mc_rating_msgTwinkler = new Array();
var mc_rating_delayTime = 200;
var mc_rating_allImages = new Array();

function mc_js_preloadImages()
{
	for (i=0; i <mc_js_preloadImages.length ;i++)
	{
		mc_rating_allImages[i] = new Image();
		mc_rating_allImages[i].src = mc_js_preloadImages.arguments[i];
	}
}

mc_js_preloadImages(mc_rating_starImages);
mc_js_preloadImages(mc_rating_thankYouImage);
mc_js_preloadImages(mc_rating_starMessages);

function mc_js_sendRating(asin, ratingType, ratingValue)
{
    mc_js_swapStarMsgs(asin, 6); 
    //mc_js_showMessages(asin, 6);
    try
    {
        Mediachase.eCF.PublicStore.SharedModules.ProductRatingModule.UpdateRating(asin, ratingValue);
    }
    catch(ex)
    {
    }
}

function mc_js_swapStars(asin, rating)
{
	if (rating == undefined)
	{
		rating = mc_rating_savedRatings[asin];
	}
	document.images["stars." + asin].src = mc_rating_starImages[rating];
}

function mc_js_swapStarMsgs(asin, rating)
{
	if (rating == undefined)
	{
		if(mc_rating_changedRatings[asin]) 
		{
		    //alert("mc_js_swapStarMsgs changed asin="+asin+"  rating="+rating+" src="+mc_rating_starMessages[6]);
			document.images["messages." + asin].src = mc_rating_starMessages[6];
		} 
		else
		{
		    //alert("mc_js_swapStarMsgs changed asin="+asin+"  rating="+rating+" src="+mc_rating_nullStarMessage);
			document.images["messages." + asin].src = mc_rating_nullStarMessage;
		}
	} 
	else
	{
	    //alert("mc_js_swapStarMsgs rating asin="+asin+"  rating="+rating+" src="+mc_rating_starMessages[rating]);
		document.images["messages." + asin].src = mc_rating_starMessages[rating];
	}
}

function mc_js_sendStars(asin, rating)
{
	mc_rating_savedRatings[asin] = rating;
	mc_rating_changedRatings[asin] = 1;
	mc_js_sendRating(asin, 'onetofive', rating);
	
	mc_rating_msgTwinkler[asin] = window.setTimeout("mc_js_swapStarMsgs('"+asin+"'," + rating + ")", mc_rating_delayTime);
	//mc_js_swapStarMsgs(asin, rating);
}

function mc_js_starMouseOver(asin, rating)
{
	if (mc_rating_starTwinkler[asin] != 0)
	{
		window.clearTimeout(mc_rating_starTwinkler[asin]);
		mc_rating_starTwinkler[asin] = 0;
	}
	if (mc_rating_msgTwinkler[asin] != 0)
	{
		window.clearTimeout(mc_rating_msgTwinkler[asin]);
		mc_rating_msgTwinkler[asin] = 0;
	}
	mc_js_swapStars(asin, rating);
	mc_js_swapStarMsgs(asin, rating); 
}

function mc_js_starMouseOut(asin, rating)
{
	mc_rating_starTwinkler[asin] = window.setTimeout("mc_js_swapStars('"+asin+"')", mc_rating_delayTime);
	mc_rating_msgTwinkler[asin] = window.setTimeout("mc_js_swapStarMsgs('"+asin+"'," + mc_rating_savedRatings[asin] + ")", mc_rating_delayTime);
}

function mc_js_showStars(asin, rating, url)
{
    var starID = "stars." + asin;
	mc_rating_starTwinkler[asin] = 0;
	mc_rating_msgTwinkler[asin] = 0;
	document.write("<map name='starmap" + asin +"'>");
	var i = 0;
	for (i = 0; i < 6; i++) 
	{
		document.write("<area shape=rect " + 
		"coords='" + mc_rating_starMap[i] + "' " +
		"onMouseOver=\"mc_js_starMouseOver('" + asin + "'," + i + ");\" " +
		"onMouseOut=\"mc_js_starMouseOut('" + asin + "'," + i + ");\" " +
		"onClick=\"mc_js_sendStars('" + asin + "'," + i + ");" +
			"\" >");
	}
	document.write("</map>");
	document.write("<img vspace=2 src='" + mc_rating_starImages[rating] + "'");
	document.write(" border=0 usemap='#starmap" + asin);
	document.write("' id='" + starID + "'>");
}

function mc_js_showMessages(asin, rating)
{
	var msgID = "messages." + asin;
	if ( rating == undefined ) 
	{
		//alert("mc_js_showMessages rating undefined asin="+asin+" rating="+rating+" image location="+mc_rating_nullStarMessage);
		document.write("<img vspace=2 height=11 src='" + mc_rating_nullStarMessage + "'");
	}
	else
	{
		//alert("mc_js_showMessages asin="+asin+" rating="+rating+" image location="+mc_rating_starMessages[rating]);
		document.write("<img vspace=2 height=11 src='" + mc_rating_starMessages[rating] + "'"); 
	}
	document.write("' id='" + msgID + "'>");
}

