function ResizeImage(o)
{	op = o.parentNode;
if (op.nodeName =='A')
	op = op.parentNode;
if ((o.width!=0) && (o.height!= 0)&&(op!= null)) 
{ dy = (op.clientHeight +1 -o.pv) / o.height;
	dx = (op.clientWidth + 1 - o.ph) / o.width;
	dz = Math.min(dx, dy);
	o.height = o.height * dz; } }
function ResizeImages(){for (i = 0; i < window.document.images.length; i++) {o = window.document.images[i];
	if (o.fitproportional)
		ResizeImage(o);}} function OpenPopup(state, id, id1)
{	window.event.cancelBubble=true;
if (null != currPopup)
		ClosePopup();	currPopup = id1;
	var body = document.getElementById(bodyId);	var top = window.event.clientY + body.scrollTop + body.offsetTop;
	var left = window.event.clientX + body.scrollLeft + body.offsetLeft;	var elem = document.getElementById(id);
	var elemAction = document.getElementById(id1);
	elemAction.style.display = 'block';
if (state > 0) 
	{		elemAction.style.posLeft = left - window.event.offsetX;
		elemAction.style.posTop = top - window.event.offsetY + elem.offsetHeight;
	}	else 
	{		elemAction.style.posLeft = left;		elemAction.style.posTop = top;	}
} function fHERE()
	{	var base1 = document.baseURI || document.URL;	if (base1 && base1.match(/(.*)\/([^\/]+)/)) 
	{		base1 = RegExp.$1 + "/" + "\"\"";	}
	var baseEditor = "";
	var temp
	temp = base1.match(/http:\/\/(.*)wysiwyg[\/]/);	if (temp) 
	{		baseEditor = temp[0];	}	else 
	{		temp = base1.match(/http:\/\/(.*)\/\?/);		if (temp)
			baseEditor = temp[0].substr(0, temp[0].length - 1) + "wysiwyg/";
	}
	return baseEditor;};

function fSet(html, AURL)
{
	editor.url = AURL;	editor.base = "";
	editor.mvHeaderSB = this.mvHeaderSB;	editor.mvFooterSB = this.mvFooterSB;
	editor.mvLeftSB = this.mvLeftSB;	editor.mvRightSB = this.mvRightSB;
	editor.mvWithSB = this.mvWithSB;	editor.mvContAll = this.mvContAll;
	editor.mvRep = this.mvRep;	editor.mvPSID = this.mvPSID;	editor.mvTITLE = this.mvTITLE;
	editor.mvFSSB = this.mvFSSB;	editor.mvFST = this.mvFST;	editor.mvNST = this.mvNST;
	editor.mvSave = this.mvSave;	editor.mvPType = this.mvPType;	editor.mvMASTAG = this.mvMASTAG;	editor.FSOP = this.FSOP;
	editor.mvModules = this.mvModules;
	editor.mvMPath = this.mvMPath;	editor.listT = false;
	if (this.mvFS == "true") 
	{		editor.fullscreen = true;	}
	else 
	{		editor.fullscreen = false;
	}
	try
	{
		var test = html.match(/<base([\s\S]*?)>/);
	}
	catch(e)
	{
		var test = false;
	}
	if (test) 
	{
		editor.base = test[0];
	}
	else 
	{
		if (AURL && AURL != "" && typeof AURL != "undefined") 
		{
			editor.base = '<BASE href="' + AURL + '">';
		}
		else 
		{
			editor.base = '';
		}
	}
	var url = editor.base.match(/href=["|']([\s\S]*?)["|']/);
	if (url) 
	{
		editor.URLBASE = url[0].substr(6, url[0].length - 7);
	}
	var cont, first, endd;
	first = "";
	endd = "";
	if (typeof html == "undefined" || html == "" || html == "undefined") 
	{
		cont = "<html>\n<head>\n</head>\n<body></body>\n</html>"
	}
	else 
	{
		if (!html.match(/<html(.*?)>/gim))
			first = "<html>\n";
		if (!html.match(/<\/html>/gim))
			endd = "</html>";
		if (!html.match(/<head(.*?)>/gim))
			first = first + "<head>\n" + "</head>\n";
		if (!html.match(/<body(.*?)>/gim))
			first = first + "<body>";
		if (!html.match(/<\/body>/gim))
			endd = "</body>\n" + endd;
		cont = html;
	}
	cont = first + cont + endd;
	editor.bod = getBody(cont);
	editor.first = vFirst;
	vFirst = "";
	editor.showborders = true;
	editor.showscripts = true;
	editor.lang = mvLang;
	editor.generate();
	editor.setHTML(cont, AURL);
};
