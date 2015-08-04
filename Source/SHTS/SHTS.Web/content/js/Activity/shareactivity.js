function deleteactivity(aid) {
    console.log(aid);
    $.post("/user/DeleteActivity", { "id": aid }, function (json) {
        if (json.IsSuccess) {
            window.location.href = "/user/Activitys";
        } else {
            alert(json.Message);
        }
    });
}