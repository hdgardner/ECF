// Run command to fix the postback functions for asp.net ajax
Sys.Application.add_load(function()
{
    var form = Sys.WebForms.PageRequestManager.getInstance()._form;
    form._initialAction = form.action = window.location.href;
});

function openWindow(url,win,para) {
  var win = window.open(url,win,para);
  win.focus();
}