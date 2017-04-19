$(function () {
    $("#btn_submit_new_trade").click(function () {

        var obj = $(this);
        try {
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

                $(obj).attr('disabled', 'disabled');
                $(obj).html('正在提交...');

                $.ajax({
                    url: '/wechat/trade/new',
                    data: $("#form_trade_new").serialize(),
                    type: 'POST',
                    success: function (data) {
                        if (data.IsSuccessful) {
                            alert("中介申请交易成功，请等待管理员审核");
                            window.location.href = '/wechat/trade/mytradelist';
                        }
                        else {
                            if (data.ErrorMessage == '') {
                                showMessage('中介申请失败，请稍后再试', false);
                            }
                            else {
                                showMessage(data.ErrorMessage, false);
                            }
                        }

                        $(obj).html('提交申请');
                        $(obj).removeAttr("disabled");
                    },
                    error: function (msg) {
                        $(obj).html('提交申请');
                        $(obj).removeAttr("disabled");
                        showMessage("资源创建失败，请检查您的网络是否连接", false);
                    }
                });
            }
        }
        catch (e) {
            $(obj).html('提交申请');
            $(obj).removeAttr("disabled");
        }
    });
});

function showTradeCommissionRule() {
    ds.dialog({
        title: '中介手续费说明',
        content: $('#tradeCommissionRule').html()//,
        //icon: "/Content/dialog/images/info.png"
    });
}

function showMessage(msg, isSuccessfulMsg) {
    if (msg != '') {
        if (isSuccessfulMsg) {
            ds.dialog({
                title: '消息提示',
                content: msg,
                icon: "/Content/dialog/images/success.png"
            });
        }
        else {
            ds.dialog({
                title: '消息提示',
                content: msg,
                icon: "/Content/dialog/images/info.png"
            });
        }
    }
}