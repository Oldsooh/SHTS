$(function () {
    var cu = $.cookie("UserAccount");
    console.log(cu);
    if (cu) {
        var ua = $.parseJSON(cu);

        $("#js-mobile_ipt").val(ua.UserName);
        
        if (ua.Password) {
            $("#js-mobile_pwd_ipt").val(ua.Password);
            $("#IsAutoLogin").val("1");
            $("#js-mobile_btn").disabled = true;
            $("#js-mobile_btn").text("登陆中.......");
            $("#js-form-mobile").submit();
        }
    } else {
        validatelogin();
    }
});
//验证
function validatelogin() {
    $("#js-form-mobile").validate({
        rules: {
            username: {
                required: true
            },
            password: {
                required: true
            },
            code: {
                required: true
            }
        },
        messages: {
            username: {
                required: "请输入账户名/手机号/邮箱"
            },
            password: {
                required: "请输入密码"
            },
            code: {
                required: "请输入验证码"
            }
        }
    });
}
$(function () {
    $("#js-mobile_btn").click(function () {
        $("#js-form-mobile").validate({
            submitHandler: function (form) {
                form.submit();
            }
        });
    });
    if ($("#errormsg").val() != "") {

        //微信端判断解除绑定使用
        if ($('#errormsg').val() == 'logoutinvalid') {
            ds.dialog({
                title: '消息提示',
                content: '您还未绑定会员账号！请先绑定会员账号。',
                icon: "/Content/dialog/images/info.png"
            });
            window.location.href = "/wechat/account/login";
        }
        else if ($('#errormsg').val() == 'logoutsuccess') {
            ds.dialog({
                title: '消息提示',
                content: '解除绑定成功。',
                icon: "/Content/dialog/images/info.png"
            });
            window.location.href = "/wechat/account/login";
        }
        else {
            ds.dialog({
                title: '消息提示',
                content: $("#errormsg").val(),
                icon: "/Content/dialog/images/info.png"
            });
        }
    }
});
