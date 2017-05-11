var addHandler = window.addEventListener ?
    function (elem, event, handler) { elem.addEventListener(event, handler); } :
    function (elem, event, handler) { elem.attachEvent("on" + event, handler); };

var ele = function (id) { return document.getElementById(id); }


function autoHeight(elemid) {
    if (!ele("_textareacopy")) {
        var t = document.createElement("textarea");
        t.id = "_textareacopy";
        t.style.position = "absolute";
        t.style.left = "-9999px";
        document.body.appendChild(t);
    }
    function change() {
        var text = ele(elemid).value;
        var linesCount = text.split('\n').length;

        if (linesCount > 1 || text.length > 12) {
            ele("_textareacopy").value = ele(elemid).value;
            ele(elemid).style.height = ele("_textareacopy").scrollHeight + ele("_textareacopy").style.height + "px";
        }
        else {
            ele(elemid).style.height = "40px";
        }
    }
    addHandler(ele(elemid), "propertychange", change);//for IE  
    addHandler(ele(elemid), "input", change);// for !IE  
    ele(elemid).style.overflow = "hidden";//һأġ  
    ele(elemid).style.resize = "none";//ȥtextareaקŴ/С߶/ȹ  
}


function autoTextarea(elemId) {
    addHandler(window, "load", function () {
        autoHeight(elemId);
    });
}