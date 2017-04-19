
$(function () {
    $("#btn_submit_new_trade").click(function () {
        $('#tradedetail').val($(tradedetail).html());
        var tradedetail_error = $('#tradedetail-error');

        if (tradedetail_error != null) {
            tradedetail_error.remove();
        }

        // special validation for trade detail
        if ($('#tradedetail').val() == '' || $('#tradedetail').val().replace(' ', '') == '') {
            var errorHtml = '<label id="tradedetail-error" style="display: block;">请输入交易详情</label>';
            $('#tradedetail').parent().append(errorHtml);
            return false;
        }

        if ($("#form_trade_new").validate().form()) {

            $.ajax({
                url: '/trade/new',
                data: $("#form_trade_new").serialize(),
                type: 'POST',
                success: function (msg) {
                    if (msg == "success") {
                        alert("中介申请交易成功，请等待管理员审核");
                        window.location.href = '/trade/mytradelist';
                    }
                    else {
                        alert(msg);
                    }
                },
                error: function (msg) {
                    alert("网络异常，中介申请失败");
                }
            });
        }

    });
});
var tradedetail;
KindEditor.ready(function (K) {
    tradedetail = K.create('#tradedetail', {
        width: '680px',
        height: '500px',
        resizeType: 1,
        allowPreviewEmoticons: false,
        allowImageUpload: true,
        allowFileManager: false,
        fillDescAfterUploadImage: true,
        uploadJson: '/content/kindeditor/asp.net/upload_json.ashx',
        fileManagerJson: '/content/kindeditor/asp.net/file_manager_json.ashx',
        items: [
            'source', 'fontname', 'fontsize', 'lineheight', '|', 'undo', 'redo', '|', 'forecolor', 'hilitecolor', 'bold', 'italic', 'underline',
            'removeformat', '|', 'justifyleft', 'justifycenter', 'justifyright', 'insertorderedlist',
            'insertunorderedlist', '|', 'emoticons', 'link', 'image', 'multiimage', 'insertfile']
    });
});

var tradeReply;
KindEditor.ready(function (K) {
    tradeReply = K.create('#tradeReply', {
        width: '730px',
        height: '314px',
        resizeType: 1,
        allowPreviewEmoticons: false,
        allowImageUpload: true,
        allowFileManager: false,
        fillDescAfterUploadImage: true,
        uploadJson: '/content/kindeditor/asp.net/upload_json.ashx',
        fileManagerJson: '/content/kindeditor/asp.net/file_manager_json.ashx',
        items: [
            'source', 'fontname', 'fontsize', 'lineheight', '|', 'undo', 'redo', '|', 'forecolor', 'hilitecolor', 'bold', 'italic', 'underline',
            'removeformat', '|', 'justifyleft', 'justifycenter', 'justifyright', 'insertorderedlist',
            'insertunorderedlist', '|', 'emoticons', 'link', 'image', 'multiimage', 'insertfile']
    });
});