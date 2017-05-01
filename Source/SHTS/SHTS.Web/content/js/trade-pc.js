
$(function () {
    $("#btn_submit_new_trade").click(function () {
        $('#tradedetail').val(tradedetail.html());
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
        cssData: 'body{font-size:14px;font-family:Tahoma;}',
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
            'fontname', 'fontsize', 'lineheight', '|', 'undo', 'redo', '|', 'forecolor', 'hilitecolor', 'bold', 'italic', 'underline',
            '|', 'justifyleft', 'justifycenter', 'justifyright', '|', 'emoticons', 'image', 'link', 'table']
    });
});

var tradeReply;
KindEditor.ready(function (K) {
    tradeReply = K.create('#tradeReply', {
        cssData: 'body{font-size:14px;font-family:Tahoma;}',
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
            'fontname', 'fontsize', 'lineheight', '|', 'undo', 'redo', '|', 'forecolor', 'hilitecolor', 'bold', 'italic', 'underline',
            '|', 'justifyleft', 'justifycenter', 'justifyright', '|', 'emoticons', 'image', 'link', 'table']
    });
});

function doTradeReplyWithOperation(currentTradeId) {
    var operation = 'update';
    var content = tradeReply.html();

    // Parameters checking here.
    if (checkParameter(operation, content)) {
        $.ajax({
            url: '/trade/ReplyTradeWithOperation',
            data: {
                id: currentTradeId,
                operation: operation,
                content: content
            },
            type: 'POST',
            success: function (msg) {
                if (msg == "success") {
                    alert("回复成功");
                    //window.location.href = window.location.href;
                    window.location.reload();
                }
                else {
                    alert(msg);
                }
            },
            error: function (msg) {
                alert("网络异常，回复中介交易失败");
            }
        });
    }
}

function checkParameter(operation, content) {
    if (operation != 'update' //&&
        //    operation != 'reviewed' &&
        //    operation != 'delete' &&
        //    operation != 'completed' &&
        //    operation != 'finished' &&
        //    operation != 'invalid' &&
        //    operation != 'paid'
        ) {
        alert('操作动作错误，请重新选择');
        return false;
    }
    else if (content == '' || content.replace(' ', '') == '') {
        alert('回复内容不能为空');
        return false;
    }
    else {
        // Nothing to do.
    }

    return true;

}