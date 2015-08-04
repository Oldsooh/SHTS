//验证
$(function () {
    $(".editform").validate({
        rules: {
            UserName: {
                required: true
            },
            EncryptedPassword: {
                required: true
            },
            EncryptedPasswordRepter: {
                required: true
            }
        },
        messages: {
            UserName: {
                required: "请输入用户名"
            },
            EncryptedPassword: {
                required: "请输入密码"
            },
            EncryptedPasswordRepter: {
                required: "请输入确认密码",
                equalTo: "两次输入密码不一致不一致"
            }
        }
    });

    $(".submitbtn").click(function () {
        var formid = $(this).attr("formid");
        $("#" + formid)[0].validate({
            submitHandler: function (form) {
                form.submit();
            }
        });
    });
});