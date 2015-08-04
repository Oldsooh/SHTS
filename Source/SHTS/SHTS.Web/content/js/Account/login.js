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
        ds.dialog({
            title: '消息提示',
            content: $("#errormsg").val(),
            icon: "/Content/dialog/images/info.png"
        });
    }
});
