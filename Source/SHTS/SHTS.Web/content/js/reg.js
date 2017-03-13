$(function () {
    $('#switch_qlogin').click(function () {
        $('#switch_login').removeClass("switch_btn_focus").addClass('switch_btn');
        $('#switch_qlogin').removeClass("switch_btn").addClass('switch_btn_focus');
        $('#switch_bottom').animate({ left: '0px', width: '70px' });
        $('#js-form-mail').css('display', 'none');
        $('#js-form-mobile').css('display', 'block');
    });
    $('#switch_login').click(function () {
        $('#switch_login').removeClass("switch_btn").addClass('switch_btn_focus');
        $('#switch_qlogin').removeClass("switch_btn_focus").addClass('switch_btn');
        $('#switch_bottom').animate({ left: '154px', width: '70px' });
        $('#js-form-mail').css('display', 'block');
        $('#js-form-mobile').css('display', 'none');
    });
    $('#switch_qlogin').trigger('click');

    $('#js_mobile_Usertype').change(function () {
        if ($(this).val() == 0) {
            $('.usercard').attr('placeholder', '填写身份证号');
        }
        else {
            $('.usercard').attr('placeholder', '填写企业注册号');
        }
    });

});

//验证
$(function () {
    $("#js-form-mobile").validate({
        rules: {
            UserName: {
                required: true
            },
            Cellphone: {
                required: true,
                isMobile: true
            },
            Email: {
                required: true,
                email: true
            },
            EncryptedPassword: {
                required: true,
                minlength: 6
            },
            UCard: {
                required: true
            },
            EncryptedPasswordRepter: {
                required: true,
                equalTo: "#js_mobile_pwd_ipt"
            },
            code: {
                required: true
            }
        },
        messages: {
            UserName: {
                required: "请输入账户名称"
            },
            Cellphone: {
                required: "请输入电话号码"
            },
            Email: {
                required: "请输入邮箱",
                email: "邮箱格式不正确"
            },
            EncryptedPassword: {
                required: "请输入密码",
                minlength: "密码不能小于6个字符"
            },
            EncryptedPasswordRepter: {
                required: "请输入确认密码",
                equalTo: "两次输入密码不一致"
            },
            UCard: {
                required: "请输入身份证号码"
            },
            code: {
                required: "请输入手机验证码"
            }
        }
    });

    // 手机号码验证       
    jQuery.validator.addMethod("isMobile", function (value, element) {
        var length = value.length;
        var mobile = /^(((13[0-9]{1})|(15[0-9]{1})|(18[0-9]{1}))+\d{8})$/;
        return this.optional(element) || (length == 11 && mobile.test(value));
    }, "手机号码格式不正确");

    $("#js-form-mail").validate({
        rules: {
            UserName: {
                required: true
            },
            Cellphone: {
                required: true,
                isMobile: true
            },
            Email: {
                required: true,
                email: true
            },
            EncryptedPassword: {
                required: true,
                minlength: 6
            },
            UCard: {
                required: true
            },
            EncryptedPasswordRepter: {
                required: true,
                equalTo: "#js_mobile_pwd_ipt"
            },
            code: {
                required: true
            }
        },
        messages: {
            UserName: {
                required: "请输入账户名称"
            },
            Cellphone: {
                required: "请输入电话号码"
            },
            Email: {
                required: "请输入企业邮箱",
                email: "邮箱格式不正确"
            },
            EncryptedPassword: {
                required: "请输入密码",
                minlength: "密码不能小于6个字符"
            },
            EncryptedPasswordRepter: {
                required: "请输入确认密码",
                equalTo: "两次输入密码不一致"
            },
            UCard: {
                required: "请输入企业名称"
            },
            code: {
                required: "请输入手机验证码"
            }
        }
    });
});

//上传图片实时预览
$(function () {
    $("#js_mobile_IdentiyImg").change(function () {
        uploadfile(this);
    })
    $("#js_email_IdentiyImg").change(function () {
        uploadfile(this);
    })
    function uploadfile(ele) {
        var objUrl = getObjectURL(ele.files[0]);
        var url = "background-image:url('" + objUrl + "')";
        $(ele).parent().attr("style", url);
    }
    //建立一個可存取到該file的url
    function getObjectURL(file) {
        var url = null;
        if (window.createObjectURL != undefined) { // basic
            url = window.createObjectURL(file);
        } else if (window.URL != undefined) { // mozilla(firefox)
            url = window.URL.createObjectURL(file);
        } else if (window.webkitURL != undefined) { // webkit or chrome
            url = window.webkitURL.createObjectURL(file);
        }
        return url;
    }
});

//发送短信验证码
var seccond = 60;
var regInterval;
function CreatePhoneCode(ele, item,vcodeitem) {
    if (seccond < 60) {
        return;
    }
    var tel = $("#" + item).val();
    if (tel == "") {
        alert("请先输入手机号码！");
        return;
    }
    if ($("#" + item).next().hasClass('haserror')) {
        alert("该手机号码已经被注册，请重新输入！");
        return;
    }

    // 发送手机验证码之前验证码
    var bvcode = $("#" + vcodeitem).val();
    if (bvcode == "") {
        alert("请先输入验证码！");
        return;
    }
    if ($(ele).hasClass('btn-disabled')) {
        return;
    }
    RequestSendVCode(ele, tel);
}

function validbeforevcode(item,btnsend) {
    var bvcode = $(item).val();
    if (bvcode == "") {
        alert("请先输入验证码！");
        return;
    }
    $.post("/account/VerifyBeforeVcode",
        { "vcode": bvcode },
        function (result) {
            if (result.IsSuccess) {
                $("#" + btnsend).removeClass("btn-disabled");
                $("#" + btnsend).removeAttr("disabled");
            }
            else {
                alert("验证码输入错误！");
            }
        });
}

function RequestSendVCode(ele, tel) {
    $(ele).addClass("btn-disabled");
    $.post("/account/VerifyCellphone",
    { "cellphone": tel },
    function (result) {
        console.log(result);
        if (result.IsSuccess) {
            $(ele).html("60秒后再获取");
            regInterval = setInterval(function () {
                if (seccond == 60) {
                    $(ele).addClass("btn-disabled");
                    seccond = 59;
                }
                else if (seccond == 0) {
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
        }
        else {
            clearInterval(regInterval);
            $(ele).html("重新获取手机验证码");
            alert(result.ExceptionInfo);
            $(ele).removeClass("btn-disabled");
        }
    });
}

function VerifyInfo(ele, column) {
    if ($(ele).val() == "") {
        return;
    }
    if ($(ele).parent().parent().css("display") == "none") {
        return;
    }
    var vdiv = $(ele).parent().find(".verifyblock")[0];
    $(vdiv).addClass("showverifyblock");
    var VerifyInfoImg = $(vdiv).find("img:first");

    $.post("/account/VerifyUserName",
    {
        "field": column,
        "value": $(ele).val()
    }, function (json) {
        if (json.IsSuccess) {
            $(VerifyInfoImg).attr("src", "/Content/images/ok.gif");
            $(vdiv).removeClass("haserror");
            $(vdiv).removeAttr("column_name");
        } else {
            $(VerifyInfoImg).attr("src", "/Content/images/no.gif");
            $(vdiv).addClass("haserror");
            $(vdiv).attr("column_name", column);
        }

        //var errors = $("#js-form-mobile").find(".haserror");
        //var errors2 = $("#js-form-mail").find(".haserror");
        //if (errors.length > 0 || errors2.length > 0) {

        //    if (typeof ($('#js_mobile_btn')) != undefined) {
        //        $('#js_mobile_btn').attr('disabled', 'disabled');
        //        $('#js_mobile_btn').addClass('btn-disabled');
        //    }
        //    if (typeof ($('#js_mail_btn')) != undefined) {
        //        $('#js_mail_btn').attr('disabled', 'disabled');
        //        $('#js_mail_btn').addClass('btn-disabled');
        //    }
        //}
        //else {

        //    if (typeof ($('#js_mobile_btn')) != undefined) {
        //        $('#js_mobile_btn').removeAttr('disabled');
        //        $('#js_mobile_btn').removeClass('btn-disabled');
        //    }
        //    if (typeof ($('#js_mail_btn')) != undefined) {
        //        $('#js_mail_btn').removeAttr('disabled');
        //        $('#js_mail_btn').removeClass('btn-disabled');
        //    }
        //}
    });
}

function getValidatiionErrorMessage(column_name) {
    var msg = "";
    switch (column_name.toLowerCase()) {
        case "ucard":
            msg = "您当前注册认证的身份证号已被使用，请仔细确认！";
            break;
        case "username":
            msg = "您当前使用的用户名已被使用，请重新输入！";
            break;
        case "email":
            msg = "您当前注册的邮箱号已被使用，请仔细确认！如您的输入无误，建议您使用邮箱找回密码功能进行密码重置！";
            break;
        case "cellphone":
            msg = "您当前注册的手机号已被使用，请仔细确认！";
            break;
        default:
            break;
    }

    return msg;

}