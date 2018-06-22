function WebForm_CallbackComplete_SyncFixed() {
  // SyncFix: the original version uses "i" as global thereby resulting in javascript errors when "i" is used elsewhere in consuming pages
  for (var i = 0; i < __pendingCallbacks.length; i++) {
   callbackObject = __pendingCallbacks[ i ];
  if (callbackObject && callbackObject.xmlRequest && (callbackObject.xmlRequest.readyState == 4)) {
   // the callback should be executed after releasing all resources 
   // associated with this request. 
   // Originally if the callback gets executed here and the callback 
   // routine makes another ASP.NET ajax request then the pending slots and
   // pending callbacks array gets messed up since the slot is not released
   // before the next ASP.NET request comes.
   // FIX: This statement has been moved below
   // WebForm_ExecuteCallback(callbackObject);
   if (!__pendingCallbacks[ i ].async) {
     __synchronousCallBackIndex = -1;
   }
   __pendingCallbacks[i] = null;

   var callbackFrameID = "__CALLBACKFRAME" + i;
   var xmlRequestFrame = document.getElementById(callbackFrameID);
   if (xmlRequestFrame) {
     xmlRequestFrame.parentNode.removeChild(xmlRequestFrame);
   }

   // SyncFix: the following statement has been moved down from above;
   WebForm_ExecuteCallback(callbackObject);
  }
 }
}
 
if (typeof (WebForm_CallbackComplete) == "function") {
  // set the original version with fixed version
  WebForm_CallbackComplete = WebForm_CallbackComplete_SyncFixed;
}

