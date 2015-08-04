/* #region 后台资源列表页面全选 */
var allselected = false;
function toggleselectall() {
    if (allselected) {
        $('input[id^=chk_]').removeAttr('checked', 'checked');
        allselected = false;
    }
    else {
        $('input[id^=chk_]').attr('checked', 'checked');
        allselected = true;
    }
}

/* #endregion 后台资源列表页面全选 */

/* #region 删除全部所选资源 */
var ids = "";
function deleteall() {
    var hasselected = false, isfirst = true;
    ids = "";
    $('input[id^=chk_]').each(function () {
        if (this.checked) {
            if (!isfirst) {
                ids = ids + ",";
            }
            ids = ids + this.attributes.rid.value;
            isfirst = false;
            hasselected = true;
        }
    })
    if (hasselected) {
        msg.confirm("删除确认", "是否确定要删除选择的资源吗 ？", "是请点击确定 ，否则点取消。无法撤销。", 1);
    }
    else {
        msg.ok("删除出错", "没有选择要删除的资源", "请选中要删除的资源的复选框");
    }
}
/* #endregion 删除全部所选资源 */

/* #region 删除指定资源 */
var id = "";
$('.del').click(function () {
    id = $(this).attr('pid');
    msg.confirm("删除确认", "是否确定要删除改资源吗 ？", "是请点击确定 ，否则点取消", 2);
});
/* #endregion 删除指定资源 */

/* #region 审核通过全部所选资源 */
var ids = "";
function approall() {
    var hasselected = false, isfirst = true;
    ids = "";
    $('input[id^=chk_]').each(function () {
        if (this.checked) {
            if (!isfirst) {
                ids = ids + ",";
            }
            ids = ids + this.attributes.rid.value;
            isfirst = false;
            hasselected = true;
        }
    })
    if (hasselected) {
        msg.confirm("审核确认", "该资源是否已经通过审核？", "是请点击确定 ，否则点取消", 3);
    }
    else {
        msg.ok("审核出错", "没有选择要审核的资源", "请选中审核通过的资源的复选框");
    }
}
/* #endregion 审核通过全部所选资源 */

/* #region 审核通过指定资源 */
var id = "";
$('.appro').click(function () {
    id = $(this).attr('pid');
    msg.confirm("审核确认", "是否确定要通过该资源的审批 ？", "是请点击确定 ，否则点取消", 4);
});
/* #endregion 审核通过指定资源 */

/* #region messagebox */
$(function () {
    $('.msgconfirm .msgconfirmcancel').click(function () {
        $('.msgconfirm').fadeOut(200);
    })
})
var msg = new function MsgBox() {

    this.ok = function (title, msg, detail, reload) {
        $('.msgok .msgtitle').text(title);
        $('.msgok .msgmsg').text(msg);
        $('.msgok .msgdetail').text(detail);
        $('.msgok .msgokok').click(function () {
            $('.msgok').fadeOut(200);
            if (reload) {
                window.location.href = window.location.href;
            }
        });
        $('.msgok').fadeIn();
    }

    this.confirm = function (title, msg, detail, func) {
        $('.msgconfirm .msgtitle').text(title);
        $('.msgconfirm .msgmsg').text(msg);
        $('.msgconfirm .msgdetail').text(detail);
        $('.msgconfirm .msgconfirmok').unbind('click');
        switch (func) {//在此处注册事件
            case 1://删除所有资源
                $('.msgconfirm .msgconfirmok').click(function () {
                    $('.msgconfirm').fadeOut(200);
                    dodeleteall();
                })
                break;
            case 2://删除指定资源
                $('.msgconfirm .msgconfirmok').click(function () {
                    $('.msgconfirm').fadeOut(200);
                    dodeletebyid();
                })
                break;
            case 3://审核通过所有资源
                $('.msgconfirm .msgconfirmok').click(function () {
                    $('.msgconfirm').fadeOut(200);
                    doapproall();
                })
                break;
            case 4://审核通过指定资源
                $('.msgconfirm .msgconfirmok').click(function () {
                    $('.msgconfirm').fadeOut(200);
                    doapprobyid();
                })
                break;
            default:
                break;
        }
        $('.msgconfirm').fadeIn();
    }
}
//删除全部资源
function dodeleteall() {
    $.post('/admin/resource/deleteresources', { ids: ids }, function (data) {
        if (data.Status == 0) {
            msg.ok("删除成功", "删除成功", "您选择已成功删除所选资源", true);
        } else {
            msg.ok("删除失败", "删除失败", msg.Message);
        }
    })
}
//删除指定资源
function dodeletebyid() {
    $.post('/admin/resource/deleteresourcebyid', { id: id }, function (data) {
        if (data.Status == 0) {
            msg.ok("删除成功", "删除成功", "您选择已成功删除该资源", true);
        } else {
            msg.ok("删除失败", "删除失败", msg.Message);
        }
    })
}
//删除全部资源
function doapproall() {
    $.post('/admin/resource/approresources', { ids: ids }, function (data) {
        if (data.Status == 0) {
            msg.ok("审核成功", "审核成功", "您选择的资源已经通过审核", true);
        } else {
            msg.ok("审核失败", "审核失败", msg.Message);
        }
    })
}
//删除指定资源
function doapprobyid() {
    $.post('/admin/resource/approresourcebyid', { id: id }, function (data) {
        if (data.Status == 0) {
            msg.ok("审核成功", "审核成功", "该资源已通过审核", true);
        } else {
            msg.ok("审核失败", "审核失败", msg.Message);
        }
    })
}
/* #endregion messagebox*/