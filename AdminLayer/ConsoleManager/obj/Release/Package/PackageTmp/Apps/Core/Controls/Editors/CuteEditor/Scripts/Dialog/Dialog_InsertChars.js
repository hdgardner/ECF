var OxO91ed=["Verdana","innerHTML","Unicode","innerText","\x3Cspan style=\x27font-family:","\x27\x3E","\x3C/span\x3E","selfont","length","checked","value","charstable1","charstable2","fontFamily","style","display","block","none"];var editor=Window_GetDialogArguments(window); function getchar(obj){var Ox2b;var Ox2c=getFontValue()||OxO91ed[0x0];if(!obj[OxO91ed[0x1]]){return ;} ; Ox2b=obj[OxO91ed[0x1]] ;if(Ox2c==OxO91ed[0x2]){ Ox2b=obj[OxO91ed[0x3]] ;} else {if(Ox2c!=OxO91ed[0x0]){ Ox2b=OxO91ed[0x4]+Ox2c+OxO91ed[0x5]+obj[OxO91ed[0x1]]+OxO91ed[0x6] ;} ;} ; editor.PasteHTML(Ox2b) ; Window_CloseDialog(window) ;}  ; function cancel(){ Window_CloseDialog(window) ;}  ; function getFontValue(){var Ox2f=document.getElementsByName(OxO91ed[0x7]);for(var i=0x0;i<Ox2f[OxO91ed[0x8]];i++){if(Ox2f.item(i)[OxO91ed[0x9]]){return Ox2f.item(i)[OxO91ed[0xa]];} ;} ;}  ; function sel_font_change(){var Ox31=getFontValue()||OxO91ed[0x0];var Ox2f4=Window_GetElement(window,OxO91ed[0xb],true);var Ox2f5=Window_GetElement(window,OxO91ed[0xc],true); Ox2f4[OxO91ed[0xe]][OxO91ed[0xd]]=Ox31 ; Ox2f4[OxO91ed[0xe]][OxO91ed[0xf]]=(Ox31!=OxO91ed[0x2]?OxO91ed[0x10]:OxO91ed[0x11]) ; Ox2f5[OxO91ed[0xe]][OxO91ed[0xf]]=(Ox31==OxO91ed[0x2]?OxO91ed[0x10]:OxO91ed[0x11]) ;}  ;