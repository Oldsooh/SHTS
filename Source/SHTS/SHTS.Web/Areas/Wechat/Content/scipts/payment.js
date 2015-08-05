function doPayForOrderEvent() {
    $('#payform').submit();
}

$("#submitVip").click(function () {
    var vipDuration = Number($('#selectVipDuration').val());

    if (vipDuration < 1 || vipDuration > 5) {
        alert('VIP时间选择有误，请重新选择');
        return false;
    }
    if (confirm('您确认充值' + vipDuration + '年VIP会员吗？')) {
        $.ajax({
            url: '/m/user/VipOrder',
            data:
            {
                vipDuration: vipDuration
            },
            type: 'POST',
            success: function (msg) {
                if (msg != null && msg.indexOf('正在跳转') > -1) {
                    document.write(msg);
                }
                else {
                    alert(msg);
                }
            },
            error: function (msg) {
                alert("网络异常，支付失败");
            }
        });
    }
});