// JScript File


// Initializes a new instance of the StringBuilder class
// and appends the given value if supplied
function StringBuilder(value)
{
    this.strings = new Array();
    this.append(value);
}

// Appends the given value to the end of this instance.
StringBuilder.prototype.append = function (value)
{
    if (value)
    {
        this.strings.push(value);
    }
}

// Returns the length of this instance.
StringBuilder.prototype.length = function ()
{
   return this.strings.length;
}



// Clears the string buffer
StringBuilder.prototype.clear = function ()
{
   //editedFlag alert("Clear "+this.strings.length);
    this.strings.length = 0;
}

// Converts this instance to a String with separater $^|^$.
StringBuilder.prototype.toString = function (gap)
{
    return this.strings.join(gap);
}

// Convert the String with separater $^|^$ to this Array;
StringBuilder.prototype.fromString = function (strSource)
{
    
  //  alert(strSource.indexOf("$^|^$"));
   // alert(strSource.substr(0,strSource.indexOf("$^|^$")+5));
    var i = 0;
    i = parseInt(strSource.indexOf("$^|^$")+5);
    while (i<parseInt(strSource.length))
    {
       
       
        if ((i<5)&&(strSource.length>0))
              {
                this.strings.push(strSource);
                break;
              }
              
        var s =strSource.substr(0,i);
       
      
        if (s.length>5)
        {
            var str=s.substr(0,parseInt(s.length-6));
              this.strings.push(str);
        }
       
        strSource=strSource.substr(i,parseInt(strSource.length-1));
       
        i = parseInt(strSource.indexOf("$^|^$")+5);
    }
    ///alert(this.toString());
    return true;
}

