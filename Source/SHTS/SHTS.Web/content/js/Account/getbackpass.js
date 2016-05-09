//验证
$(function () {
    $("#js-form-mobile").validate({
        rules: {
            Email: {
                required: true,
                email: true
            },
            CellPhoneVCode: {
                required: true
            },
            VCode: {
                required: true
            },
            Cellphone: {
                required: true,
                isMobile: true
            },
            EncryptedPassword: {
                required: true,
                minlength: 6
            },
            EncryptedPasswordRepter: {
                required: true,
                equalTo: "#js_mobile_pwd_ipt"
            }
        },
        messages: {
            Email: {
                required: "请输入注册邮箱",
                email: "邮箱格式不正确"
            },
            CellPhoneVCode: {
                required: "请输入手机验证码"
            },
            VCode: {
                required: "请输入验证码",
            },
            Cellphone: {
                required: "请输入电话号码"
            },
            EncryptedPassword: {
                required: "请输入密码",
                minlength: "密码不能小于6个字符"
            },
            EncryptedPasswordRepter: {
                required: "请输入确认密码",
                equalTo: "两次输入密码不一致不一致"
            }
        }
    });

    // 手机号码验证       
    jQuery.validator.addMethod("isMobile", function (value, element) {
        var length = value.length;
        var mobile = /^(((13[0-9]{1})|(15[0-9]{1})|(18[0-9]{1}))+\d{8})$/;
        return this.optional(element) || (length == 11 && mobile.test(value));
    }, "电话号码格式不正确");
});


function validbeforevcode(vcode) {
    $.post("/account/VerifyBeforeVcode",
        { "vcode": vcode },
        function (result) {
            return result.IsSuccess;
        });
}

//发送短信验证码
var seccond = 60;
var regInterval;
function CreatePhoneCode(ele, item) {
    if (seccond < 60) {
        return;
    }
    var tel = $("#" + item).val();
    if (tel == "") {
        alert("请先输入手机号码！");
        return;
    }
    if (VerifyInfo($("#js-mobile_ipt")[0], "Cellphone", tel)) {
        return false;
    }
}

function RequestSendVCode(ele, tel) {
    if ($(ele).hasClass('btn-disabled')) {
        return;
    }
    $(ele).addClass("btn-disabled");
    $.post("/account/VerifyCellphone",
    { "cellphone": tel },
    function (result) {
        if (result.IsSuccess) {
            $(ele).html("60秒后再获取");
            $(".cell").removeClass("nextshow");
            $("#mobile_btn").removeClass("nextshow");
            regInterval = setInterval(function () {
                if (seccond == 60) {
                    $(ele).css({ "cursor": "" });
                    $(ele).addClass("btn-disabled");
                    seccond = 59;
                }
                else if (seccond == 0) {
                    $(ele).removeAttr("disabled");
                    $(ele).css({ "background": "#329d15" });
                    seccond = 60;
                    clearInterval(regInterval);
                    $(ele).html("获取手机验证码");
                    $(ele).css({ "cursor": "pointer" });
                    $(ele).removeClass("btn-disabled");
                }
                else {
                    $(ele).html(seccond + "秒后再获取");
                    seccond = seccond - 1;
                }
            }, 1000);
            $("#wrapblock").height(450);
        }
        else {
            clearInterval(regInterval);
            $(ele).html("重新获取手机验证码");
            alert(result.ExceptionInfo);
            $(ele).removeAttr("disabled");
            $(ele).removeClass("btn-disabled");
        }
    });
}

function VerifyInfo(ele, column, tel) {
    var result = false;
    $.post("/account/VerifyUserName",
    {
        "field": column,
        "value": $(ele).val()
    }, function (json) {
        if (json.IsSuccess) {
            alert("该手机号码未注册!");
            result = true;
        } else {
            RequestSendVCode($("#js-get_mobile_vcode")[0], tel);
        }
    });
    return result;
}
