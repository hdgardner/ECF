var OxOc020=["hiddenDirectory","hiddenFile","hiddenAlert","hiddenAction","hiddenActionData","This function is disabled in the demo mode.","disabled","[[Disabled]]","[[SpecifyNewFolderName]]","","value","createdir","[[CopyMoveto]]","/","move","copy","[[AreyouSureDelete]]","parentNode","text","isdir","true",".","[[SpecifyNewFileName]]","rename","path","True","False",":","FoldersAndFiles","TR","length","this.bgColor=\x27#eeeeee\x27;","onmouseover","this.bgColor=\x27\x27;","onmouseout","nodeName","INPUT","changedir","url","TargetUrl","htmlcode","onload","getElementsByTagName","table","sortable","className"," ","id","rows","cells","innerHTML","\x3Ca href=\x22#\x22 onclick=\x22ts_resortTable(this);return false;\x22\x3E","\x3Cspan class=\x22sortarrow\x22\x3E\x26nbsp;\x3C/span\x3E\x3C/a\x3E","string","undefined","innerText","childNodes","nodeValue","nodeType","span","cellIndex","TABLE","sortdir","down","\x26uarr;","up","\x26darr;","sortbottom","tBodies","sortarrow","\x26nbsp;","20","19","Form1","Image1","FolderDescription","CreateDir","Copy","Move","Delete","DoRefresh","name_Cell","size_Cell","op_Cell","row0","row0_cb","divpreview","Width","Height","AutoStart","ShowControls","ShowStatusBar","WindowlessVideo","Button1","Button2","btn_zoom_in","btn_zoom_out","btn_Actualsize","checked","\x3Cembed name=\x22MediaPlayer1\x22 src=\x22","\x22 autostart=\x22","\x22 showcontrols=\x22","\x22  windowlessvideo=\x22","\x22 showstatusbar=\x22","\x22 width=\x22","\x22 height=\x22","\x22 type=\x22application/x-mplayer2\x22 pluginspage=\x22http://www.microsoft.com/Windows/MediaPlayer\x22 \x3E\x3C/embed\x3E\x0A","\x3Cobject classid=\x22CLSID:22D6F312-B0F6-11D0-94AB-0080C74C7E95\x22 "," codebase=\x22http://activex.microsoft.com/activex/"," controls/mplayer/en/nsmp2inf.cab#Version=6,0,02,902\x22 "," standby=\x22Loading Microsoft Windows Media Player components...\x22 "," type=\x22application/x-oleobject\x22","  height=\x22","\x22 \x3E","\x3Cparam name=\x22FileName\x22 value=\x22","\x22/\x3E","\x3Cparam name=\x22autoStart\x22 value=\x22","\x3Cparam name=\x22showControls\x22 value=\x22","\x3Cparam name=\x22showstatusbar\x22 value=\x22","\x3Cparam name=\x22windowlessvideo\x22 value=\x22","\x3C/object\x3E","onunload","onbeforeunload","Please choose a Media movie to insert","\x22 windowlessvideo=\x22","zoom","style","wrapupPrompt","iepromptfield","display","none","body","div","IEPromptBox","promptBlackout","border","1px solid #b0bec7","backgroundColor","#f0f0f0","position","absolute","width","330px","zIndex","100","\x3Cdiv style=\x22width: 100%; padding-top:3px;background-color: #DCE7EB; font-family: verdana; font-size: 10pt; font-weight: bold; height: 22px; text-align:center; background:url(Load.ashx?type=image\x26file=formbg2.gif) repeat-x left top;\x22\x3E[[InputRequired]]\x3C/div\x3E","\x3Cdiv style=\x22padding: 10px\x22\x3E","\x3CBR\x3E\x3CBR\x3E","\x3Cform action=\x22\x22 onsubmit=\x22return wrapupPrompt()\x22\x3E","\x3Cinput id=\x22iepromptfield\x22 name=\x22iepromptdata\x22 type=text size=46 value=\x22","\x22\x3E","\x3Cbr\x3E\x3Cbr\x3E\x3Ccenter\x3E","\x3Cinput type=\x22submit\x22 value=\x22\x26nbsp;\x26nbsp;\x26nbsp;[[OK]]\x26nbsp;\x26nbsp;\x26nbsp;\x22\x3E","\x26nbsp;\x26nbsp;\x26nbsp;\x26nbsp;\x26nbsp;\x26nbsp;","\x3Cinput type=\x22button\x22 onclick=\x22wrapupPrompt(true)\x22 value=\x22\x26nbsp;[[Cancel]]\x26nbsp;\x22\x3E","\x3C/form\x3E\x3C/div\x3E","top","100px","offsetWidth","left","px","block","CuteEditor_ColorPicker_ButtonOver(this)"];var hiddenDirectory=Window_GetElement(window,OxOc020[0x0],true);var hiddenFile=Window_GetElement(window,OxOc020[0x1],true);var hiddenAlert=Window_GetElement(window,OxOc020[0x2],true);var hiddenAction=Window_GetElement(window,OxOc020[0x3],true);var hiddenActionData=Window_GetElement(window,OxOc020[0x4],true); function CreateDir_click(){if(isDemoMode){ alert(OxOc020[0x5]) ;return false;} ;if(Event_GetSrcElement()[OxOc020[0x6]]){ alert(OxOc020[0x7]) ;return false;} ;if(Browser_IsIE7()){ IEprompt(Ox194,OxOc020[0x8],OxOc020[0x9]) ; function Ox194(Ox2f9){if(Ox2f9){ hiddenActionData[OxOc020[0xa]]=Ox2f9 ; hiddenAction[OxOc020[0xa]]=OxOc020[0xb] ; window.PostBackAction() ;return true;} else {return false;} ;}  ;return Event_CancelEvent();} else {var Ox2f9=prompt(OxOc020[0x8],OxOc020[0x9]);if(Ox2f9){ hiddenActionData[OxOc020[0xa]]=Ox2f9 ;return true;} else {return false;} ;return false;} ;}  ; function Move_click(){if(isDemoMode){ alert(OxOc020[0x5]) ;return false;} ;if(Event_GetSrcElement()[OxOc020[0x6]]){ alert(OxOc020[0x7]) ;return false;} ;if(Browser_IsIE7()){ IEprompt(Ox194,OxOc020[0xc],OxOc020[0xd]) ; function Ox194(Ox2f9){if(Ox2f9){ hiddenActionData[OxOc020[0xa]]=Ox2f9 ; hiddenAction[OxOc020[0xa]]=OxOc020[0xe] ; window.PostBackAction() ;return true;} else {return false;} ;}  ;return Event_CancelEvent();} else {var Ox2f9=prompt(OxOc020[0xc],OxOc020[0xd]);if(Ox2f9){ hiddenActionData[OxOc020[0xa]]=Ox2f9 ;return true;} else {return false;} ;return false;} ;}  ; function Copy_click(){if(isDemoMode){ alert(OxOc020[0x5]) ;return false;} ;if(Event_GetSrcElement()[OxOc020[0x6]]){ alert(OxOc020[0x7]) ;return false;} ;if(Browser_IsIE7()){ IEprompt(Ox194,OxOc020[0xc],OxOc020[0xd]) ; function Ox194(Ox2f9){if(Ox2f9){ hiddenActionData[OxOc020[0xa]]=Ox2f9 ; hiddenAction[OxOc020[0xa]]=OxOc020[0xf] ; window.PostBackAction() ;return true;} else {return false;} ;}  ;return Event_CancelEvent();} else {var Ox2f9=prompt(OxOc020[0xc],OxOc020[0xd]);if(Ox2f9){ hiddenActionData[OxOc020[0xa]]=Ox2f9 ;return true;} else {return false;} ;return false;} ;}  ; function Delete_click(){if(isDemoMode){ alert(OxOc020[0x5]) ;return false;} ;if(Event_GetSrcElement()[OxOc020[0x6]]){ alert(OxOc020[0x7]) ;return false;} ;return confirm(OxOc020[0x10]);}  ; function EditImg_click(img){if(isDemoMode){ alert(OxOc020[0x5]) ;return false;} ;if(img[OxOc020[0x6]]){ alert(OxOc020[0x7]) ;return false;} ;var Ox2fe=img[OxOc020[0x11]][OxOc020[0x11]];var Ox2ff=Ox2fe[OxOc020[0x12]];var name;var Ox300;if(Browser_IsOpera()){ Ox300=Ox2fe.getAttribute(OxOc020[0x13])==OxOc020[0x14] ;} else { Ox300=Ox2fe[OxOc020[0x13]] ;} ;if(Browser_IsIE7()){var Oxb3;if(Ox300){ IEprompt(Ox194,OxOc020[0x8],Ox2ff) ;} else {var i=Ox2ff.lastIndexOf(OxOc020[0x15]); Oxb3=Ox2ff.substr(i) ;var Ox12=Ox2ff.substr(0x0,Ox2ff.lastIndexOf(OxOc020[0x15])); IEprompt(Ox194,OxOc020[0x16],Ox12) ;} ; function Ox194(Ox2f9){if(Ox2f9&&Ox2f9!=Ox2fe[OxOc020[0x12]]){if(!Ox300){ Ox2f9=Ox2f9+Oxb3 ;} ; hiddenAction[OxOc020[0xa]]=OxOc020[0x17] ; hiddenActionData[OxOc020[0xa]]=(Ox300?OxOc020[0x19]:OxOc020[0x1a])+OxOc020[0x1b]+Ox2fe[OxOc020[0x18]]+OxOc020[0x1b]+Ox2f9 ; window.PostBackAction() ;} ;}  ;} else {if(Ox300){ name=prompt(OxOc020[0x8],Ox2ff) ;} else {var i=Ox2ff.lastIndexOf(OxOc020[0x15]);var Oxb3=Ox2ff.substr(i);var Ox12=Ox2ff.substr(0x0,Ox2ff.lastIndexOf(OxOc020[0x15])); name=prompt(OxOc020[0x16],Ox12) ;if(name){ name=name+Oxb3 ;} ;} ;if(name&&name!=Ox2fe[OxOc020[0x12]]){ hiddenAction[OxOc020[0xa]]=OxOc020[0x17] ; hiddenActionData[OxOc020[0xa]]=(Ox300?OxOc020[0x19]:OxOc020[0x1a])+OxOc020[0x1b]+Ox2fe[OxOc020[0x18]]+OxOc020[0x1b]+name ; window.PostBackAction() ;} ;} ;return Event_CancelEvent();}  ; setMouseOver() ; function setMouseOver(){var FoldersAndFiles=Window_GetElement(window,OxOc020[0x1c],true);var Ox303=FoldersAndFiles.getElementsByTagName(OxOc020[0x1d]);for(var i=0x0;i<Ox303[OxOc020[0x1e]];i++){var Ox2fe=Ox303[i]; Ox2fe[OxOc020[0x20]]= new Function(OxOc020[0x9],OxOc020[0x1f]) ; Ox2fe[OxOc020[0x22]]= new Function(OxOc020[0x9],OxOc020[0x21]) ;} ;}  ; function row_click(Ox2fe){var Ox300;if(Browser_IsOpera()){ Ox300=Ox2fe.getAttribute(OxOc020[0x13])==OxOc020[0x14] ;} else { Ox300=Ox2fe[OxOc020[0x13]] ;} ;if(Ox300){if(Event_GetSrcElement()[OxOc020[0x23]]==OxOc020[0x24]){return ;} ; hiddenAction[OxOc020[0xa]]=OxOc020[0x25] ; hiddenActionData[OxOc020[0xa]]=Ox2fe.getAttribute(OxOc020[0x18]) ; window.PostBackAction() ;} else {var Oxf2=Ox2fe.getAttribute(OxOc020[0x18]); hiddenFile[OxOc020[0xa]]=Oxf2 ;var Ox1fe=Ox2fe.getAttribute(OxOc020[0x26]); Window_GetElement(window,OxOc020[0x27],true)[OxOc020[0xa]]=Ox1fe ;var htmlcode=Ox2fe.getAttribute(OxOc020[0x28]);if(htmlcode!=OxOc020[0x9]&&htmlcode!=null){ do_preview(htmlcode) ;} else {try{ Actualsize() ;} catch(x){ do_preview() ;} ;} ;} ;}  ; function do_preview(){}  ; function reset_hiddens(){if(hiddenAlert[OxOc020[0xa]]){ alert(hiddenAlert.value) ;} ; hiddenAlert[OxOc020[0xa]]=OxOc020[0x9] ; hiddenAction[OxOc020[0xa]]=OxOc020[0x9] ; hiddenActionData[OxOc020[0xa]]=OxOc020[0x9] ;}  ; Event_Attach(window,OxOc020[0x29],reset_hiddens) ; function RequireFileBrowseScript(){}  ; function Actualsize(){}  ; Event_Attach(window,OxOc020[0x29],sortables_init) ;var SORT_COLUMN_INDEX; function sortables_init(){if(!document[OxOc020[0x2a]]){return ;} ;var Ox308=document.getElementsByTagName(OxOc020[0x2b]);for(var Ox309=0x0;Ox309<Ox308[OxOc020[0x1e]];Ox309++){var Ox30a=Ox308[Ox309];if(((OxOc020[0x2e]+Ox30a[OxOc020[0x2d]]+OxOc020[0x2e]).indexOf(OxOc020[0x2c])!=-0x1)&&(Ox30a[OxOc020[0x2f]])){ ts_makeSortable(Ox30a) ;} ;} ;}  ; function ts_makeSortable(Ox30c){if(Ox30c[OxOc020[0x30]]&&Ox30c[OxOc020[0x30]][OxOc020[0x1e]]>0x0){var Ox30d=Ox30c[OxOc020[0x30]][0x0];} ;if(!Ox30d){return ;} ;for(var i=0x2;i<0x4;i++){var Ox30e=Ox30d[OxOc020[0x31]][i];var Ox1f9=ts_getInnerText(Ox30e); Ox30e[OxOc020[0x32]]=OxOc020[0x33]+Ox1f9+OxOc020[0x34] ;} ;}  ; function ts_getInnerText(Ox27){if( typeof Ox27==OxOc020[0x35]){return Ox27;} ;if( typeof Ox27==OxOc020[0x36]){return Ox27;} ;if(Ox27[OxOc020[0x37]]){return Ox27[OxOc020[0x37]];} ;var Ox24=OxOc020[0x9];var Ox2ba=Ox27[OxOc020[0x38]];var Ox11=Ox2ba[OxOc020[0x1e]];for(var i=0x0;i<Ox11;i++){switch(Ox2ba[i][OxOc020[0x3a]]){case 0x1: Ox24+=ts_getInnerText(Ox2ba[i]) ;break ;case 0x3: Ox24+=Ox2ba[i][OxOc020[0x39]] ;break ;;;} ;} ;return Ox24;}  ; function ts_resortTable(Ox311){var Ox21d;for(var Ox312=0x0;Ox312<Ox311[OxOc020[0x38]][OxOc020[0x1e]];Ox312++){if(Ox311[OxOc020[0x38]][Ox312][OxOc020[0x23]]&&Ox311[OxOc020[0x38]][Ox312][OxOc020[0x23]].toLowerCase()==OxOc020[0x3b]){ Ox21d=Ox311[OxOc020[0x38]][Ox312] ;} ;} ;var Ox313=ts_getInnerText(Ox21d);var Ox314=Ox311[OxOc020[0x11]];var Ox315=Ox314[OxOc020[0x3c]];var Ox30c=getParent(Ox314,OxOc020[0x3d]);if(Ox30c[OxOc020[0x30]][OxOc020[0x1e]]<=0x1){return ;} ;var Ox316=ts_getInnerText(Ox30c[OxOc020[0x30]][0x1][OxOc020[0x31]][Ox315]);var Ox317=ts_sort_caseinsensitive;if(Ox316.match(/^\d\d[\/-]\d\d[\/-]\d\d\d\d$/)){ Ox317=ts_sort_date ;} ;if(Ox316.match(/^\d\d[\/-]\d\d[\/-]\d\d$/)){ Ox317=ts_sort_date ;} ;if(Ox316.match(/^[?]/)){ Ox317=ts_sort_currency ;} ;if(Ox316.match(/^[\d\.]+$/)){ Ox317=ts_sort_numeric ;} ; SORT_COLUMN_INDEX=Ox315 ;var Ox30d= new Array();var Ox318= new Array();for(var i=0x0;i<Ox30c[OxOc020[0x30]][0x0][OxOc020[0x1e]];i++){ Ox30d[i]=Ox30c[OxOc020[0x30]][0x0][i] ;} ;for(var j=0x1;j<Ox30c[OxOc020[0x30]][OxOc020[0x1e]];j++){ Ox318[j-0x1]=Ox30c[OxOc020[0x30]][j] ;} ; Ox318.sort(Ox317) ;if(Ox21d.getAttribute(OxOc020[0x3e])==OxOc020[0x3f]){var Ox319=OxOc020[0x40]; Ox318.reverse() ; Ox21d.setAttribute(OxOc020[0x3e],OxOc020[0x41]) ;} else { Ox319=OxOc020[0x42] ; Ox21d.setAttribute(OxOc020[0x3e],OxOc020[0x3f]) ;} ;for( i=0x0 ;i<Ox318[OxOc020[0x1e]];i++){if(!Ox318[i][OxOc020[0x2d]]||(Ox318[i][OxOc020[0x2d]]&&(Ox318[i][OxOc020[0x2d]].indexOf(OxOc020[0x43])==-0x1))){ Ox30c[OxOc020[0x44]][0x0].appendChild(Ox318[i]) ;} ;} ;for( i=0x0 ;i<Ox318[OxOc020[0x1e]];i++){if(Ox318[i][OxOc020[0x2d]]&&(Ox318[i][OxOc020[0x2d]].indexOf(OxOc020[0x43])!=-0x1)){ Ox30c[OxOc020[0x44]][0x0].appendChild(Ox318[i]) ;} ;} ;var Ox31a=document.getElementsByTagName(OxOc020[0x3b]);for(var Ox312=0x0;Ox312<Ox31a[OxOc020[0x1e]];Ox312++){if(Ox31a[Ox312][OxOc020[0x2d]]==OxOc020[0x45]){if(getParent(Ox31a[Ox312],OxOc020[0x2b])==getParent(Ox311,OxOc020[0x2b])){ Ox31a[Ox312][OxOc020[0x32]]=OxOc020[0x46] ;} ;} ;} ; Ox21d[OxOc020[0x32]]=Ox319 ;}  ; function getParent(Ox27,Ox31c){if(Ox27==null){return null;} else {if(Ox27[OxOc020[0x3a]]==0x1&&Ox27[OxOc020[0x23]].toLowerCase()==Ox31c.toLowerCase()){return Ox27;} else {return getParent(Ox27.parentNode,Ox31c);} ;} ;}  ; function ts_sort_date(Oxd7,Oxc){var Ox31e=ts_getInnerText(Oxd7[OxOc020[0x31]][SORT_COLUMN_INDEX]);var Ox31f=ts_getInnerText(Oxc[OxOc020[0x31]][SORT_COLUMN_INDEX]);if(Ox31e[OxOc020[0x1e]]==0xa){var Ox320=Ox31e.substr(0x6,0x4)+Ox31e.substr(0x3,0x2)+Ox31e.substr(0x0,0x2);} else {var Ox321=Ox31e.substr(0x6,0x2);if(parseInt(Ox321)<0x32){ Ox321=OxOc020[0x47]+Ox321 ;} else { Ox321=OxOc020[0x48]+Ox321 ;} ;var Ox320=Ox321+Ox31e.substr(0x3,0x2)+Ox31e.substr(0x0,0x2);} ;if(Ox31f[OxOc020[0x1e]]==0xa){var Ox322=Ox31f.substr(0x6,0x4)+Ox31f.substr(0x3,0x2)+Ox31f.substr(0x0,0x2);} else { Ox321=Ox31f.substr(0x6,0x2) ;if(parseInt(Ox321)<0x32){ Ox321=OxOc020[0x47]+Ox321 ;} else { Ox321=OxOc020[0x48]+Ox321 ;} ;var Ox322=Ox321+Ox31f.substr(0x3,0x2)+Ox31f.substr(0x0,0x2);} ;if(Ox320==Ox322){return 0x0;} ;if(Ox320<Ox322){return -0x1;} ;return 0x1;}  ; function ts_sort_currency(Oxd7,Oxc){var Ox31e=ts_getInnerText(Oxd7[OxOc020[0x31]][SORT_COLUMN_INDEX]).replace(/[^0-9.]/g,OxOc020[0x9]);var Ox31f=ts_getInnerText(Oxc[OxOc020[0x31]][SORT_COLUMN_INDEX]).replace(/[^0-9.]/g,OxOc020[0x9]);return parseFloat(Ox31e)-parseFloat(Ox31f);}  ; function ts_sort_numeric(Oxd7,Oxc){var Ox31e=parseFloat(ts_getInnerText(Oxd7[OxOc020[0x31]][SORT_COLUMN_INDEX]));if(isNaN(Ox31e)){ Ox31e=0x0 ;} ;var Ox31f=parseFloat(ts_getInnerText(Oxc[OxOc020[0x31]][SORT_COLUMN_INDEX]));if(isNaN(Ox31f)){ Ox31f=0x0 ;} ;return Ox31e-Ox31f;}  ; function ts_sort_caseinsensitive(Oxd7,Oxc){var Ox31e=ts_getInnerText(Oxd7[OxOc020[0x31]][SORT_COLUMN_INDEX]).toLowerCase();var Ox31f=ts_getInnerText(Oxc[OxOc020[0x31]][SORT_COLUMN_INDEX]).toLowerCase();if(Ox31e==Ox31f){return 0x0;} ;if(Ox31e<Ox31f){return -0x1;} ;return 0x1;}  ; function ts_sort_default(Oxd7,Oxc){var Ox31e=ts_getInnerText(Oxd7[OxOc020[0x31]][SORT_COLUMN_INDEX]);var Ox31f=ts_getInnerText(Oxc[OxOc020[0x31]][SORT_COLUMN_INDEX]);if(Ox31e==Ox31f){return 0x0;} ;if(Ox31e<Ox31f){return -0x1;} ;return 0x1;} [sortables_init] ; RequireFileBrowseScript() ;var Form1=Window_GetElement(window,OxOc020[0x49],true);var hiddenDirectory=Window_GetElement(window,OxOc020[0x0],true);var hiddenFile=Window_GetElement(window,OxOc020[0x1],true);var hiddenAlert=Window_GetElement(window,OxOc020[0x2],true);var hiddenAction=Window_GetElement(window,OxOc020[0x3],true);var hiddenActionData=Window_GetElement(window,OxOc020[0x4],true);var Image1=Window_GetElement(window,OxOc020[0x4a],true);var FolderDescription=Window_GetElement(window,OxOc020[0x4b],true);var CreateDir=Window_GetElement(window,OxOc020[0x4c],true);var Copy=Window_GetElement(window,OxOc020[0x4d],true);var Move=Window_GetElement(window,OxOc020[0x4e],true);var FoldersAndFiles=Window_GetElement(window,OxOc020[0x1c],true);var Delete=Window_GetElement(window,OxOc020[0x4f],true);var DoRefresh=Window_GetElement(window,OxOc020[0x50],true);var name_Cell=Window_GetElement(window,OxOc020[0x51],true);var size_Cell=Window_GetElement(window,OxOc020[0x52],true);var op_Cell=Window_GetElement(window,OxOc020[0x53],true);var row0=Window_GetElement(window,OxOc020[0x54],true);var row0_cb=Window_GetElement(window,OxOc020[0x55],true);var divpreview=Window_GetElement(window,OxOc020[0x56],true);var Width=Window_GetElement(window,OxOc020[0x57],true);var Height=Window_GetElement(window,OxOc020[0x58],true);var AutoStart=Window_GetElement(window,OxOc020[0x59],true);var ShowControls=Window_GetElement(window,OxOc020[0x5a],true);var ShowStatusBar=Window_GetElement(window,OxOc020[0x5b],true);var WindowlessVideo=Window_GetElement(window,OxOc020[0x5c],true);var TargetUrl=Window_GetElement(window,OxOc020[0x27],true);var Button1=Window_GetElement(window,OxOc020[0x5d],true);var Button2=Window_GetElement(window,OxOc020[0x5e],true);var btn_zoom_in=Window_GetElement(window,OxOc020[0x5f],true);var btn_zoom_out=Window_GetElement(window,OxOc020[0x60],true);var btn_Actualsize=Window_GetElement(window,OxOc020[0x61],true);var editor=Window_GetDialogArguments(window); do_preview() ; function do_preview(){var Ox3a3;var Ox5d;var Ox5c;if(TargetUrl[OxOc020[0xa]]==OxOc020[0x9]){return ;} ;var Ox3a4,Ox3a5,Ox3a6,Ox3a7;if(AutoStart[OxOc020[0x62]]){ Ox3a4=0x1 ;} else { Ox3a4=0x0 ;} ;if(ShowStatusBar[OxOc020[0x62]]){ Ox3a5=0x1 ;} else { Ox3a5=0x0 ;} ;if(ShowControls[OxOc020[0x62]]){ Ox3a6=0x1 ;} else { Ox3a6=0x0 ;} ;if(WindowlessVideo[OxOc020[0x62]]){ Ox3a7=true ;} else { Ox3a7=false ;} ; Ox5d=Width[OxOc020[0xa]] ; Ox5c=Height[OxOc020[0xa]] ; Ox5d=parseInt(Ox5d) ; Ox5c=parseInt(Ox5c) ;var Ox363=OxOc020[0x63]+TargetUrl[OxOc020[0xa]]+OxOc020[0x64]+Ox3a4+OxOc020[0x65]+Ox3a6+OxOc020[0x66]+Ox3a7+OxOc020[0x67]+Ox3a5+OxOc020[0x68]+Ox5d+OxOc020[0x69]+Ox5c+OxOc020[0x6a];var Ox344=OxOc020[0x6b]+OxOc020[0x6c]+OxOc020[0x6d]+OxOc020[0x6e]+OxOc020[0x6f]+OxOc020[0x70]+Ox5c+OxOc020[0x68]+Ox5d+OxOc020[0x71]; Ox344=Ox344+OxOc020[0x72]+TargetUrl[OxOc020[0xa]]+OxOc020[0x73] ; Ox344=Ox344+OxOc020[0x74]+Ox3a4+OxOc020[0x73] ; Ox344=Ox344+OxOc020[0x75]+Ox3a6+OxOc020[0x73] ; Ox344=Ox344+OxOc020[0x76]+Ox3a5+OxOc020[0x73] ; Ox344=Ox344+OxOc020[0x77]+Ox3a7+OxOc020[0x73] ; Ox344=Ox344+Ox363+OxOc020[0x78] ; Ox363=Ox344 ; divpreview[OxOc020[0x32]]=Ox363 ;}  ; window[OxOc020[0x7a]]=window[OxOc020[0x79]]=function (){ divpreview[OxOc020[0x32]]=OxOc020[0x9] ;}  ;var parameters= new Array(); function do_insert(){ divpreview[OxOc020[0x32]]=OxOc020[0x9] ;if(TargetUrl[OxOc020[0xa]]==OxOc020[0x9]){ alert(OxOc020[0x7b]) ;return false;} ;var Ox3a4,Ox3a5,Ox3a6,Ox3a7;if(AutoStart[OxOc020[0x62]]){ Ox3a4=0x1 ;} else { Ox3a4=0x0 ;} ;if(ShowStatusBar[OxOc020[0x62]]){ Ox3a5=0x1 ;} else { Ox3a5=0x0 ;} ;if(ShowControls[OxOc020[0x62]]){ Ox3a6=0x1 ;} else { Ox3a6=0x0 ;} ;if(WindowlessVideo[OxOc020[0x62]]){ Ox3a7=true ;} else { Ox3a7=false ;} ; width=Width[OxOc020[0xa]]+OxOc020[0x9] ; height=Height[OxOc020[0xa]]+OxOc020[0x9] ; width=parseInt(width) ; height=parseInt(height) ;var Ox363=OxOc020[0x63]+TargetUrl[OxOc020[0xa]]+OxOc020[0x64]+Ox3a4+OxOc020[0x65]+Ox3a6+OxOc020[0x67]+Ox3a5+OxOc020[0x7c]+Ox3a7+OxOc020[0x68]+width+OxOc020[0x69]+height+OxOc020[0x6a];var Ox344=OxOc020[0x6b]+OxOc020[0x6c]+OxOc020[0x6d]+OxOc020[0x6e]+OxOc020[0x6f]+OxOc020[0x70]+height+OxOc020[0x68]+width+OxOc020[0x71]; Ox344=Ox344+OxOc020[0x72]+TargetUrl[OxOc020[0xa]]+OxOc020[0x73] ; Ox344=Ox344+OxOc020[0x74]+Ox3a4+OxOc020[0x73] ; Ox344=Ox344+OxOc020[0x75]+Ox3a6+OxOc020[0x73] ; Ox344=Ox344+OxOc020[0x76]+Ox3a5+OxOc020[0x73] ; Ox344=Ox344+OxOc020[0x77]+Ox3a7+OxOc020[0x73] ; Ox344=Ox344+Ox363+OxOc020[0x78] ; Ox363=Ox344 ; editor.PasteHTML(Ox363) ; Window_CloseDialog(window) ;}  ; function do_Close(){ divpreview[OxOc020[0x32]]=OxOc020[0x9] ; Window_CloseDialog(window) ;}  ; function Zoom_In(){if(divpreview[OxOc020[0x7e]][OxOc020[0x7d]]!=0x0){ divpreview[OxOc020[0x7e]][OxOc020[0x7d]]*=1.2 ;} else { divpreview[OxOc020[0x7e]][OxOc020[0x7d]]=1.2 ;} ;}  ; function Zoom_Out(){if(divpreview[OxOc020[0x7e]][OxOc020[0x7d]]!=0x0){ divpreview[OxOc020[0x7e]][OxOc020[0x7d]]*=0.8 ;} else { divpreview[OxOc020[0x7e]][OxOc020[0x7d]]=0.8 ;} ;}  ; function Actualsize(){ divpreview[OxOc020[0x7e]][OxOc020[0x7d]]=0x1 ; do_preview() ;}  ;if(Browser_IsIE7()){var _dialogPromptID=null; function IEprompt(Ox194,Ox195,Ox196){ that=this ; this[OxOc020[0x7f]]=function (Ox197){ val=document.getElementById(OxOc020[0x80])[OxOc020[0xa]] ; _dialogPromptID[OxOc020[0x7e]][OxOc020[0x81]]=OxOc020[0x82] ; document.getElementById(OxOc020[0x80])[OxOc020[0xa]]=OxOc020[0x9] ;if(Ox197){ val=OxOc020[0x9] ;} ; Ox194(val) ;return false;}  ;if(Ox196==undefined){ Ox196=OxOc020[0x9] ;} ;if(_dialogPromptID==null){var Ox198=document.getElementsByTagName(OxOc020[0x83])[0x0]; tnode=document.createElement(OxOc020[0x84]) ; tnode[OxOc020[0x2f]]=OxOc020[0x85] ; Ox198.appendChild(tnode) ; _dialogPromptID=document.getElementById(OxOc020[0x85]) ; tnode=document.createElement(OxOc020[0x84]) ; tnode[OxOc020[0x2f]]=OxOc020[0x86] ; Ox198.appendChild(tnode) ; _dialogPromptID[OxOc020[0x7e]][OxOc020[0x87]]=OxOc020[0x88] ; _dialogPromptID[OxOc020[0x7e]][OxOc020[0x89]]=OxOc020[0x8a] ; _dialogPromptID[OxOc020[0x7e]][OxOc020[0x8b]]=OxOc020[0x8c] ; _dialogPromptID[OxOc020[0x7e]][OxOc020[0x8d]]=OxOc020[0x8e] ; _dialogPromptID[OxOc020[0x7e]][OxOc020[0x8f]]=OxOc020[0x90] ;} ;var Ox199=OxOc020[0x91]; Ox199+=OxOc020[0x92]+Ox195+OxOc020[0x93] ; Ox199+=OxOc020[0x94] ; Ox199+=OxOc020[0x95]+Ox196+OxOc020[0x96] ; Ox199+=OxOc020[0x97] ; Ox199+=OxOc020[0x98] ; Ox199+=OxOc020[0x99] ; Ox199+=OxOc020[0x9a] ; Ox199+=OxOc020[0x9b] ; _dialogPromptID[OxOc020[0x32]]=Ox199 ; _dialogPromptID[OxOc020[0x7e]][OxOc020[0x9c]]=OxOc020[0x9d] ; _dialogPromptID[OxOc020[0x7e]][OxOc020[0x9f]]=parseInt((document[OxOc020[0x83]][OxOc020[0x9e]]-0x13b)/0x2)+OxOc020[0xa0] ; _dialogPromptID[OxOc020[0x7e]][OxOc020[0x81]]=OxOc020[0xa1] ;var Ox19a=document.getElementById(OxOc020[0x80]);try{var Ox19b=Ox19a.createTextRange(); Ox19b.collapse(false) ; Ox19b.select() ;} catch(x){ Ox19a.focus() ;} ;}  ;} ;if(!Browser_IsWinIE()){ btn_zoom_in[OxOc020[0x7e]][OxOc020[0x81]]=btn_zoom_out[OxOc020[0x7e]][OxOc020[0x81]]=btn_Actualsize[OxOc020[0x7e]][OxOc020[0x81]]=OxOc020[0x82] ;} else {} ;if(CreateDir){ CreateDir[OxOc020[0x20]]= new Function(OxOc020[0xa2]) ;} ;if(Copy){ Copy[OxOc020[0x20]]= new Function(OxOc020[0xa2]) ;} ;if(Move){ Move[OxOc020[0x20]]= new Function(OxOc020[0xa2]) ;} ;if(Delete){ Delete[OxOc020[0x20]]= new Function(OxOc020[0xa2]) ;} ;if(DoRefresh){ DoRefresh[OxOc020[0x20]]= new Function(OxOc020[0xa2]) ;} ;if(btn_zoom_in){ btn_zoom_in[OxOc020[0x20]]= new Function(OxOc020[0xa2]) ;} ;if(btn_zoom_out){ btn_zoom_out[OxOc020[0x20]]= new Function(OxOc020[0xa2]) ;} ;if(btn_Actualsize){ btn_Actualsize[OxOc020[0x20]]= new Function(OxOc020[0xa2]) ;} ;