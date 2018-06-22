//FCKConfig.DocType = '<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">' ;
FCKConfig.SkinPath = FCKConfig.BasePath + 'skins/office2003/' ;

FCKConfig.ProtectedSource.Add( /<script[\s\S]*?\/script>/gi ) ;	// <SCRIPT> tags.

FCKConfig.PreserveSessionOnFileBrowser = true;

FCKConfig.ToolbarSets["ECF"] = [
	['Source','DocProps','-','NewPage','Preview','-','Templates'],
	['Cut','Copy','Paste','PasteText','PasteWord','-','Print','SpellCheck'],
	['Undo','Redo','-','Find','Replace','-','SelectAll','RemoveFormat'],
	['Bold','Italic','Underline','StrikeThrough','-','Subscript','Superscript'],
	['OrderedList','UnorderedList','-','Outdent','Indent'],
	['JustifyLeft','JustifyCenter','JustifyRight','JustifyFull'],
	['Link','Unlink','Anchor'],
	['Image','Flash','Table','Rule','Smiley','SpecialChar','PageBreak'],
	'/',
	['Style','FontFormat','FontName','FontSize'],
	['TextColor','BGColor'],
	['About']
] ;

var _FileBrowserLanguage	= 'aspx';
var _QuickUploadLanguage	= 'aspx';

//FCKConfig.LinkBrowserURL = FCKConfig.BasePath + 'filemanager/browser/default/browser.html?Connector=http://localhost/eCommerce/v41/CommerceManager/plugins/fileservice/connector.aspx' ;
//FCKConfig.ImageBrowserURL = FCKConfig.BasePath + 'filemanager/browser/default/browser.html?Type=Image&Connector=http://localhost/eCommerce/v41/CommerceManager/plugins/fileservice/connector.aspx' ;
//FCKConfig.FlashBrowserURL = FCKConfig.BasePath + 'filemanager/browser/default/browser.html?Type=Flash&Connector=http://localhost/eCommerce/v41/CommerceManager/plugins/fileservice/connector.aspx' ;
FCKConfig.LinkUpload = false ;
//FCKConfig.LinkUploadURL = 'http://localhost/eCommerce/v41/CommerceManager/plugins/fileservice/upload.aspx' ;
FCKConfig.ImageUpload = false ;
//FCKConfig.ImageUploadURL = 'http://localhost/eCommerce/v41/CommerceManager/plugins/fileservice/upload.aspx?Type=Image' ;
FCKConfig.FlashUpload = false ;
//FCKConfig.FlashUploadURL = 'http://localhost/eCommerce/v41/CommerceManager/plugins/fileservice/upload.aspx?Type=Flash' ;

//if( window.console ) window.console.log( 'Config is loaded!' ) ;	// @Packager.Compactor.RemoveLine