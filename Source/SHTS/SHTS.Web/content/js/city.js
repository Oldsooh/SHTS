function loadCities() {
    $("#ddlCity").html();
    $("#ddlCity").html('<option value="">加载中</option>');
    $("#ddlArea").html('<option value="">----</option>');

    var provinceId = $("#ddlProvince").val();
    var nowTime = new Date().getTime();
    $.post('/city/cities',
    {
        provinceId: provinceId,
        time: nowTime
    },
    function (data, status) {
        if (data != null && data.rows != null && data.rows.length > 0) {
            var html = '<option value="">全省/市</option>';
            for (var i = 0; i < data.rows.length; i++) {
                html += '<option value=' + data.rows[i]['Id'] + '>' + data.rows[i]['Name'] + '</option>';
            };

            if (html != '') {
                $("#ddlCity").html(html);
            }
            else {
                $("#ddlCity").html('<option value="">----</option>');
            }
        }
        else {
            $("#ddlCity").html('<option value="">----</option>');
        }
    });
}

function loadAreas() {
    $("#ddlArea").html();
    $("#ddlArea").html('<option value="">加载中</option>');

    var cityId = $("#ddlCity").val();
    var nowTime = new Date().getTime();
    $.post('/city/areas',
    {
        cityId: cityId,
        time: nowTime
    },
    function (data, status) {
        if (data != null && data.rows != null && data.rows.length > 0) {
            var html = '<option value="">全县/区</option>';
            for (var i = 0; i < data.rows.length; i++) {
                html += '<option value=' + data.rows[i]['Id'] + '>' + data.rows[i]['Name'] + '</option>';
            };

            if (html != '') {
                $("#ddlArea").html(html);
            }
            else {
                $("#ddlArea").html('<option value="">----</option>');
            }
        }
        else {
            $("#ddlArea").html('<option value="">----</option>');
        }
    });
}

function setLocalCity() {
    // 新浪根据ip获取地址  
    var localprovince = '';
    var localcity = '';
    localprovince = remote_ip_info["province"];
    localcity = remote_ip_info["city"];
    var nowTime = new Date().getTime();
    var url = encodeURI("/city/setlocalcity?cityName=" + localcity + "&time" + nowTime);
    $.get(url, function (data, status) {
        if (status == "success") {
            if (data != "" && data != "no") {
                document.getElementById("currentCityName").innerHTML = data;
            }
        }
    });
}