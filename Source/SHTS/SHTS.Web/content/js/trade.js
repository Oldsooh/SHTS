//验证
$(function () {
    $("#form_trade_new").validate({
        rules: {
            qq: {
                required: true
            },
            phone: {
                required: true,
                isMobile: true
            },
            email: {
                required: true,
                isEmail: true
            },
            username: {
                required: true
            },
            amount: {
                required: true,
                isAmount: true
            },
            bankname: {
                required: true
            },
            bankaccount: {
                required: true
            },
            bankusername: {
                required: true
            },
            agreerule: {
                required: true
            }
            ,
            address: {
                required: true
            }
            ,
            tradebody: {
                required: true
            }
            ,
            tradesubject: {
                required: true
            }
            ,
            resourceurl: {
                required: true
            }
            ,
            bankaddress: {
                required: true
            }
        },
        messages: {
            qq: {
                required: "请输入QQ或微信帐号"
            },
            phone: {
                required: "请输入电话号码"
            },
            email: {
                required: "请输入邮箱帐号",
                email: "邮箱格式不正确。"
            },
            username: {
                required: "请输入对方活动在线用户名"
            },
            amount: {
                required: "请输入交易金额",
                amount: "金额格式不正确。最低交易金额不能少于最低手续费"
            },
            bankname: {
                required: "请输入开户银行名称"
            },
            bankaccount: {
                required: "请输入银行账号"
            },
            bankusername: {
                required: "请输入银行账户用户姓名"
            }
            ,
            agreerule: {
                required: "请确认同意活动在线中介交易规则"
            }
            ,
            address: {
                required: "请输如联系地址"
            }
            ,
            tradebody: {
                required: "请填写交易详情"
            }
            ,
            tradesubject: {
                required: "请填写中介交易标题"
            }
            ,
            resourceurl: {
                required: "请输入进行中介申请的资源连接地址"
            }
            ,
            bankaddress: {
                required: "请输入开户银行地址"
            }
        }
    });

    // 手机号码验证       
    jQuery.validator.addMethod("isMobile", function (value, element) {
        var length = value.length;
        var mobile = /^(((13[0-9]{1})|(15[0-9]{1}))+\d{8})$/;
        return this.optional(element) || (length == 11 && mobile.test(value));
    }, "电话号码格式不正确");

    // 邮箱验证
    jQuery.validator.addMethod("isEmail", function (value, element) {
        var mobile = /^[a-z0-9]+([._\\-]*[a-z0-9])*@([a-z0-9]+[-a-z0-9]*[a-z0-9]+.){1,63}[a-z0-9]+$/;
        return this.optional(element) || (mobile.test(value));
    }, "邮箱格式不正确。例：zhangsan@qq.com");

    // 输入金额验证
    jQuery.validator.addMethod("isAmount", function (value, element) {
        var minCommission = Number($('#minpaycommission').val());
        return validateAmount(value, minCommission);
    }, "金额格式不正确。最低交易金额不能少于最低手续费");
});

// 银行账号信息变更
function selectBankInfoChanged(ddlBankInfo) {
    //console.log(ddlBankInfo.value);
    var bankId = $('#bankid');
    var bankname = $('#bankname');
    var bankaccount = $('#bankaccount');
    var bankusername = $('#bankusername');
    var bankaddress = $('#bankaddress');

    var bankinfo = $(ddlBankInfo).val();
    var isNewBank = true;

    if (bankinfo != 'newbank') {
        var infos = bankinfo.split('&');
        if (infos != null && infos.length == 5) {
            isNewBank = false;

            bankname.val(infos[0]);
            bankaccount.val(infos[1]);
            bankusername.val(infos[2]);
            bankId.val(infos[3]);
            bankaddress.val(infos[4]);

            bankname.addClass('lblText');
            bankaccount.addClass('lblText');
            bankusername.addClass('lblText');
            bankaddress.addClass('lblText');

            bankname.attr('readonly', 'readonly');
            bankaccount.attr('readonly', 'readonly');
            bankusername.attr('readonly', 'readonly');
            bankaddress.attr('readonly', 'readonly');
        }
    }

    if (isNewBank) {
        $('.bank').removeClass('hd');
        bankId.val('-1');
        bankname.val('');
        bankaccount.val('');
        bankusername.val('');
        bankaddress.val('');

        bankname.removeClass('lbltxt');
        bankaccount.removeClass('lbltxt');
        bankusername.removeClass('lbltxt');
        bankaddress.removeClass('lbltxt');

        bankname.removeAttr('readonly');
        bankaccount.removeAttr('readonly');
        bankusername.removeAttr('readonly');
        bankaddress.removeAttr('readonly');

        bankname.focus();
    }
    else {
        $('.bank').addClass('hd');
    }

    $('#bankname-error').remove();
    $('#bankaccount-error').remove();
    $('#bankusername-error').remove();
}

// 检查金额输入
function validateAmount(value, minValue) {
    if (value == '') {
        return false;
    }

    if (Number(value) != value ||
        (value.indexOf('.') > -1 && value.substring(value.indexOf('.') + 1).length > 2)) {
        return false;
    }

    if (Number(value) < minValue) {
        return false;
    }

    return true;
}

function onTradeAmountLostFocusEvent() {
    var minCommission = Number($('#minpaycommission').val());
    if (!validateAmount($('#tradeamount').val(), minCommission)) {
        return false;
    }

    var amount = Number($('#tradeamount').val());
    var percent = Number($('#paycommissionpercent').val());
    //var relationship = $('input[name="relationship"]:checked').val();
    //var payer = $('input[name="payer"]:checked').val();
    var sellerpay = $('#sellerpay');
    //var buyerpay = $('#buyerpay');
    var commission = amount * percent / 100;

    if (commission < minCommission) {
        commission = minCommission;
    }

    sellerpay.html('￥&nbsp;' + commission);

    //var half = commission / 2;

    //switch (payer) {
    //    case 'seller':
    //        //buyerpay.html('￥&nbsp;' + amount);
    //        sellerpay.html('￥&nbsp;' + (amount - commission));
    //        break;
    //    case 'buyer':
    //        //buyerpay.html('￥&nbsp;' + (amount + commission));
    //        sellerpay.html('￥&nbsp;' + amount);
    //        break;
    //    case 'both':
    //        //buyerpay.html('￥&nbsp;' + (amount + half));
    //        sellerpay.html('￥&nbsp;' + (amount - half));
    //        break;
    //    default:
    //        break;
    //}
}

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
                    window.location.href = window.location.href;
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

function confirmTradeOperation() {
    var operation = $('#tradeOperation').val();

    switch (operation) {
        case 'reviewed':
            operation = '通过审核';
            break;
        case 'delete':
            operation = '删除';
            break;
        case 'completed':
            operation = '完成';
            break;
        case 'finished':
            operation = '终止';
            break;
        case 'invalid':
            operation = '违规';
            break;
        case 'paid':
            operation = '买家已付款';
            break;
        case 'update':
        default:
            operation = '仅回复';
            break;
    }
    if (!confirm('确认' + operation + '吗')) {
        var lastOperation = $('#lastTradeOperation').val();
        $('#tradeOperation').val(lastOperation);
    }
}