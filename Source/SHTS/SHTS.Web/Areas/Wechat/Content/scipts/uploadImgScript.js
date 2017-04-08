$(function () {
    $('#file_upload').uploadifive({
        'auto': true,
        'uploadScript': '/Content/upload/uploadimage.ashx',
        'buttonText': '',//empty
        'fileType': 'image/*',
        'multi': true,
        'queueSizeLimit': 12,
        'uploadLimit': 12,
        'fileSizeLimit': 12 * 1024 * 1024,
        'onSelect': function (file) {
            if (file.size >= 12 * 1024 * 1024) {
                alert('每张图片不能超过5M');
                $("#file_upload").uploadify('cancel');
                return false;
            }
        },
        'onCancel': function (file) {
            //alert('The file ' + file.name + ' was cancelled!');
        },
        'onUploadComplete': function (file, data) {
            var postfile = JSON.parse(data);
            var imageUrl = '';
            var originalImg = '';
            var itemid = $(file.queueItem);
            if (postfile.Action) {
                imageUrl = postfile.Small;
                originalImg = postfile.OriginalImage;
                var imgurl = "background-image:url('" + imageUrl + "')";
                $(itemid).attr("style", imgurl);
            }
            else {
                showMessage('图片' + file[0].name + '上传失败，请重新尝试', false);
                $("#file_upload").uploadify('cancel');
                $(itemid).remove();
            }
            $(itemid).attr("rurl", originalImg);
        },
        'onFallback': function () {
            alert("该浏览器无法使用!");
        },
        'onUpload': function (file) {
        }
    });
});
function clearItem(obj) {
    $(obj).parent().fadeOut(500, function () {
        $(this).remove();
    });
}