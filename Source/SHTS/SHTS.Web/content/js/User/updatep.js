//验证
$(function () {
    $("#updatepassword").validate({
        rules: {
            oldpassword: {
                required: true
            },
            newpassword: {
                required: true,
                minlength: 6
            },
            confirmnewpassword: {
                required: true,
                equalTo: "#newpassword"
            }
        },
        messages: {
            oldpassword: {
                required: "请输入原密码"
            },
            newpassword: {
                required: "请输入新密码",
                minlength: "密码不能小于6个字符"
            },
            confirmnewpassword: {
                required: "请输入确认密码",
                equalTo: "两次输入密码不一致不一致"
            }
        }
    });
});
$(function () {
    $("#updatepasswordbtn").click(function () {
        $("#updatepassword").validate({
            submitHandler: function (form) {
                form.submit();
            }
        });
    });
    if ($("#errormsg").val() != "") {
        alert($("#errormsg").val());
    }
});