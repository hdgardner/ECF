var OxO7d4a=["inp_width","eenheid","alignment","hrcolor","hrcolorpreview","shade","sel_size","width","style","value","px","%","size","align","color","backgroundColor","noShade","noshade","","onclick"];var inp_width=Window_GetElement(window,OxO7d4a[0x0],true);var eenheid=Window_GetElement(window,OxO7d4a[0x1],true);var alignment=Window_GetElement(window,OxO7d4a[0x2],true);var hrcolor=Window_GetElement(window,OxO7d4a[0x3],true);var hrcolorpreview=Window_GetElement(window,OxO7d4a[0x4],true);var shade=Window_GetElement(window,OxO7d4a[0x5],true);var sel_size=Window_GetElement(window,OxO7d4a[0x6],true); UpdateState=function UpdateState_Hr(){}  ; SyncToView=function SyncToView_Hr(){if(element[OxO7d4a[0x8]][OxO7d4a[0x7]]){if(element[OxO7d4a[0x8]][OxO7d4a[0x7]].search(/%/)<0x0){ eenheid[OxO7d4a[0x9]]=OxO7d4a[0xa] ; inp_width[OxO7d4a[0x9]]=element[OxO7d4a[0x8]][OxO7d4a[0x7]].split(OxO7d4a[0xa])[0x0] ;} else { eenheid[OxO7d4a[0x9]]=OxO7d4a[0xb] ; inp_width[OxO7d4a[0x9]]=element[OxO7d4a[0x8]][OxO7d4a[0x7]].split(OxO7d4a[0xb])[0x0] ;} ;} ; sel_size[OxO7d4a[0x9]]=element[OxO7d4a[0xc]] ; alignment[OxO7d4a[0x9]]=element[OxO7d4a[0xd]] ; hrcolor[OxO7d4a[0x9]]=element[OxO7d4a[0xe]] ;if(element[OxO7d4a[0xe]]){ hrcolor[OxO7d4a[0x8]][OxO7d4a[0xf]]=element[OxO7d4a[0xe]] ;} ;if(element[OxO7d4a[0x10]]){ shade[OxO7d4a[0x9]]=OxO7d4a[0x11] ;} else { shade[OxO7d4a[0x9]]=OxO7d4a[0x12] ;} ;}  ; SyncTo=function SyncTo_Hr(element){if(sel_size[OxO7d4a[0x9]]){ element[OxO7d4a[0xc]]=sel_size[OxO7d4a[0x9]] ;} ;if(hrcolor[OxO7d4a[0x9]]){ element[OxO7d4a[0xe]]=hrcolor[OxO7d4a[0x9]] ;} ;if(alignment[OxO7d4a[0x9]]){ element[OxO7d4a[0xd]]=alignment[OxO7d4a[0x9]] ;} ;if(shade[OxO7d4a[0x9]]==OxO7d4a[0x11]){ element[OxO7d4a[0x10]]=true ;} else { element[OxO7d4a[0x10]]=false ;} ;if(inp_width[OxO7d4a[0x9]]){ element[OxO7d4a[0x8]][OxO7d4a[0x7]]=inp_width[OxO7d4a[0x9]]+eenheid[OxO7d4a[0x9]] ;} ;}  ; hrcolor[OxO7d4a[0x13]]=hrcolorpreview[OxO7d4a[0x13]]=function hrcolor_onclick(){ SelectColor(hrcolor,hrcolorpreview) ;}  ;