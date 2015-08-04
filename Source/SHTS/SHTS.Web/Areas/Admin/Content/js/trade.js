function doUpdateTradeRemindingEvent() {
    $("#editorContent").val(editorContent.html());
    $.ajax({
        url: '/admin/trade/UpdateTradeReminding',
        data: $("#tradeRemindingForm").serialize(),
        type: 'POST',
        success: function (msg) {
            if (msg == "success") {
                alert("中介交易提醒更新成功");
            }
            else {
                alert(msg);
            }
        },
        error: function (msg) {
            alert("网络异常，更新失败");
        }
    });
}

function doUpdatePayCommissionPercentEvent() {
    $.ajax({
        url: '/admin/trade/UpdatePayCommissionPercent',
        data: $("#payCommissionPercentForm").serialize(),
        type: 'POST',
        success: function (msg) {
            if (msg == "success") {
                alert("中介手续费设置成功");
            }
            else {
                alert(msg);
            }
        },
        error: function (msg) {
            alert("网络异常，中介手续费设置失败");
        }
    });
}

function doUpdateMinPayCommissionEvent() {
    $.ajax({
        url: '/admin/trade/UpdateMinPayCommission',
        data: $("#minPayCommissionForm").serialize(),
        type: 'POST',
        success: function (msg) {
            if (msg == "success") {
                alert("最低中介手续费设置成功");
            }
            else {
                alert(msg);
            }
        },
        error: function (msg) {
            alert("网络异常，最低中介手续费设置失败");
        }
    });
}

function deleteTradeBankInfo(configId) {
    if (!confirm('确认删除该中介付款银行信息吗')) {
        return false;
    }
    $.ajax({
        url: '/admin/trade/DeleteTradeBankInfo',
        data:
        {
            configId: configId
        },
        type: 'POST',
        success: function (msg) {
            if (msg == "success") {
                alert("删除中介手续费线下支付银行帐号成功");
                window.location.href = window.location.href;
            }
            else {
                alert(msg);
            }
        },
        error: function (msg) {
            alert("网络异常，删除中介手续费线下支付银行帐号失败");
        }
    });
}

function addTradeBankInfo() {
    var newbankName = $('#newbankname').val();
    var newbankaccount = $('#newbankaccount').val();
    var newbankusername = $('#newbankusername').val();
    var newbankaddress = $('#newbankaddress').val();

    if (newbankName == '' || newbankaccount == '' || newbankusername == '' || newbankaddress == '') {
        alert('银行信息不能为空，并保证其准确性');
        return false;
    }

    if (!confirm('确认添加该中介付款银行信息吗')) {
        return false;
    }
    $.ajax({
        url: '/admin/trade/AddTradeBankInfo',
        data:
        {
            bankName: newbankName,
            bankAccount: newbankaccount,
            bankUserName: newbankusername,
            bankAddress: newbankaddress
        },
        type: 'POST',
        success: function (msg) {
            if (msg == "success") {
                alert("添加中介手续费线下支付银行帐号成功");
                window.location.href = window.location.href;
            }
            else {
                alert(msg);
            }
        },
        error: function (msg) {
            alert("网络异常，删除中介手续费线下支付银行帐号失败");
        }
    });
}