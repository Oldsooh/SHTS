function loadCities() {
    $("#ddlProvince").click(function () {
        $("#ddlCity").html();
        $("#ddlCity").html('<option value="">一级城市</option>');

        var provinceId = $("#ddlProvince").val();
        var nowTime = new Date().getTime();
        $.post('/city/cities',
        {
            provinceId: provinceId,
            time: nowTime
        },
        function (data, status) {
            if (data != null && data.rows != null && data.rows.length > 0) {
                var html = '<option value="">一级城市</option>';
                for (var i = 0; i < data.rows.length; i++) {
                    html += '<option value=' + data.rows[i]['Id'] + '>' + data.rows[i]['Name'] + '</option>';
                };

                if (html != '') {
                    $("#ddlCity").html(html);
                }
                else {
                    $("#ddlCity").html('<option value="">一级城市</option>');
                }
            }
            else {
                $("#ddlCity").html('<option value="">一级城市</option>');
            }
        });
    });
}
