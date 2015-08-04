//弹出上传窗口
function uploadimage(obj) {
    //$('#uploadDiv').css('display', 'inherit');
    var inputId = obj.id;
    var actionUrl = obj.getAttribute("url");

    var uploadDiv = document.createElement("div");
    var div400 = document.createElement("div");
    var div380 = document.createElement("div");
    var div360 = document.createElement("div");
    var div301 = document.createElement("div");
    var div302 = document.createElement("div");

    var imageForm = document.createElement("form");
    var upImage = document.createElement("input");
    var btnUpload = document.createElement("input");
    var uploading = document.createElement("span");
    var btnSure = document.createElement("input");
    var showImage = document.createElement("img");

    upImage.type = "file";
    upImage.name = "upImage";
    upImage.id = "upImage";
    upImage.style.width = "180px";
    upImage.style.height = "22px";
    upImage.style.border = "1px solid #cdcdcd";

    btnUpload.type = "button";
    btnUpload.id = "btnUpload";
    btnUpload.value = "上传";
    btnUpload.style.height = "24px";
    btnUpload.style.marginLeft = "10px";
    btnUpload.style.border = "1px solid #cdcdcd";
    btnUpload.style.cursor = "pointer";
    btnUpload.setAttribute("onclick", "uploadSelected()");

    uploading.id = "uploading";
    uploading.style.color = "#ff0000";
    uploading.style.display = "none";
    uploading.style.fontSize = 14 + "px";
    uploading.style.fontWeight = "bold";
    uploading.innerHTML = "上传中......";

    btnSure.type = "button";
    btnSure.id = "btnSure";
    btnSure.value = "确定";
    btnSure.style.height = "24px";
    btnSure.style.marginLeft = "10px";
    btnSure.style.border = "1px solid #cdcdcd";
    btnSure.style.cursor = "pointer";
    btnSure.style.display = "none";
    btnSure.style.backgroundColor = "#9EBEFF";
    btnSure.setAttribute("onclick", "makeSure(this)");
    btnSure.setAttribute("inputId", inputId);

    div360.style.width = "360px";
    div360.style.height = "26px";
    div360.style.paddingTop = "4px";
    div360.style.paddingLeft = "10px";
    div360.style.float = "left";
    div360.style.overflow = "hidden";

    div302.style.width = "30px";
    div302.style.height = "30px";
    div302.style.backgroundColor = "#9EBEFF";
    div302.style.fontSize = 26 + "px";
    div302.style.color = "#FFFFFF";
    div302.style.textAlign = "center";
    div302.style.lineHeight = "30px";
    div302.style.cursor = "pointer";
    div302.onclick = "closeDiv()";
    div302.innerHTML = "X";
    div302.setAttribute("onclick", "closeDiv()");

    div301.style.width = "30px";
    div301.style.height = "30px";
    div301.style.float = "right";

    div400.style.width = "400px";
    div400.style.height = "30px";

    div380.style.width = "380px";
    div380.style.height = "250px";
    div380.style.margin = "10px";
    div380.style.textAlign = "center";
    div380.style.overflow = "hidden";

    showImage.id = "showImage";
    showImage.setAttribute("alt", "预览图片");
    showImage.style.maxWidth = "380px";
    showImage.style.display = "none";
    showImage.setAttribute("src", "/image.jpg");

    uploadDiv.id = "uploadDiv";
    uploadDiv.style.width = "400px";
    uploadDiv.style.height = "300px";
    uploadDiv.style.border = "3px solid #9EBEFF";
    uploadDiv.style.backgroundColor = "#F5FDFD";
    uploadDiv.style.display = "inherit";
    uploadDiv.style.textAlign = "left";
    uploadDiv.style.zIndex = "10001";

    imageForm.id = "imageForm";
    imageForm.setAttribute("method", "post");
    var nowTime = new Date().getTime();
    imageForm.setAttribute("action", actionUrl + "?time=" + nowTime);
    imageForm.setAttribute("enctype", "multipart/form-data");
    imageForm.appendChild(upImage);
    imageForm.appendChild(btnUpload);
    imageForm.appendChild(uploading);
    imageForm.appendChild(btnSure);

    div360.appendChild(imageForm);
    div301.appendChild(div302);

    div400.appendChild(div360);
    div400.appendChild(div301);

    div380.appendChild(showImage);

    uploadDiv.appendChild(div400);
    uploadDiv.appendChild(div380);

    var divShow = document.createElement('div');
    divShow.setAttribute('id', 'showDiv');
    divShow.style.display = 'block';
    divShow.style.left = '50%';
    divShow.style.top = '50%';
    divShow.style.marginLeft = '-205px';
    divShow.style.marginTop = '-155px';
    divShow.style.position = 'fixed';
    divShow.style.zIndex = "10001";
    divShow.style.filter = "progid:DXImageTransform.Microsoft.Alpha(startX=20, startY=20, finishX=100, finishY=100,style=1,opacity=100,finishOpacity=100);";
    divShow.style.width = 1060;
    divShow.style.height = 600;
    divShow.style.border = '2px solid #FFFFFF';
    divShow.style.overflow = 'hidden';

    var bgObj = document.createElement("div");
    bgObj.setAttribute('id', 'bgDiv');
    bgObj.style.position = "absolute";
    bgObj.style.top = "0";
    //bgObj.style.background = "#F3F3F3";
    bgObj.style.filter = "progid:DXImageTransform.Microsoft.Alpha(style=3,opacity=25,finishOpacity=75)";
    bgObj.style.left = "0";
    bgObj.style.width = '100%';
    bgObj.style.height = '100%';
    bgObj.style.zIndex = "10000";

    divShow.appendChild(uploadDiv);
    document.body.appendChild(divShow);
    document.body.appendChild(bgObj);
}

//上传图片
function uploadSelected() {
    if ($('#upImage').val() == '') {
        alert('请选择一张图片文件');
        return;
    }
    $('#uploading').css('display', 'inline');
    $('#imageForm').ajaxSubmit({
        success: function (msg) {
            var strJSON = msg; //得到的JSON
            var image = eval("(" + strJSON + ")"); //转换后的JSON对象
            if (image.action == "True") {
                $('#uploading').css('display', 'none');
                $('#showImage').css('display', 'inline');
                $('#showImage').attr('src', image.big);
                $('#showImage').attr('big', image.big);
                $('#showImage').attr('small', image.small);
                $('#btnSure').css('display', 'inline');
            }
            else {
                $('#uploading').css('display', 'none');
                alert(image.message);
            }
        }
    });
}

//确定图片
function makeSure(obj) {
    var inputId = obj.getAttribute('inputId');
    $("#" + inputId).val($("#showImage").attr('big'));
    $("#" + inputId).attr('src', $("#showImage").attr("small"));


    //这是为了可以编辑缩略图，后来加上的，如果不希望用户编辑，可以直接删掉。
    //$("#thumbnailUrl").val($("#showImage").attr("small"));

    $("#displayimage").attr('src', $("#showImage").attr("big"));


    $('#btnSure').css('display', 'none');
    $('#showImage').css('display', 'none');
    $('#uploadDiv').css('display', 'none');
    var showDiv = document.getElementById("showDiv");
    var bgDiv = document.getElementById("bgDiv");
    document.body.removeChild(showDiv); //移除消息层
    document.body.removeChild(bgDiv); //移除覆盖整个窗口的div层
}

//关闭窗口
function closeDiv() {
    $('#btnSure').css('display', 'none');
    $('#showImage').css('display', 'none');
    $('#uploadDiv').css('display', 'none');
    var showDiv = document.getElementById("showDiv");
    var bgDiv = document.getElementById("bgDiv");
    document.body.removeChild(showDiv); //移除消息层
    document.body.removeChild(bgDiv); //移除覆盖整个窗口的div层
}