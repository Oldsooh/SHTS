function loadCities(provinceOptions, cityOptions, areaOptions, defaultOptionHtml) {
    $(cityOptions).html();
    $(cityOptions).html('<option value="">--加载中--</option>');
    if (areaOptions) {
        $(areaOptions).html('<option value="">---</option>');
    }

    var provinceId = $(provinceOptions).val();
    var nowTime = new Date().getTime();
    $.post('/city/cities',
        {
            provinceId: provinceId,
            time: nowTime
        },
        function (data, status) {
            var html = defaultOptionHtml;
            if (data != null && data.rows != null && data.rows.length > 0) {
                for (var i = 0; i < data.rows.length; i++) {
                    html += '<option value=' + data.rows[i]['Id'] + '>' + data.rows[i]['Name'] + '</option>';
                };
            }

            $(cityOptions).html(html);
        });
}

function loadAreas(cityOptions, areaOptions, defaultOptionHtml) {
    $(areaOptions).html();
    $(areaOptions).html('<option value="">--加载中--</option>');

    var cityId = $(cityOptions).val();
    var nowTime = new Date().getTime();
    $.post('/city/areas',
        {
            cityId: cityId,
            time: nowTime
        },
        function (data, status) {
            var html = defaultOptionHtml;

            if (data != null && data.rows != null && data.rows.length > 0) {
                for (var i = 0; i < data.rows.length; i++) {
                    html += '<option value=' + data.rows[i]['Id'] + '>' + data.rows[i]['Name'] + '</option>';
                };
            }

            $(areaOptions).html(html);
        });
}