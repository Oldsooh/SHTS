function loadSubResourceTypes(resourceTypeOptions, subTypeOptions) {
    $(subTypeOptions).html('<option value="">--加载中--</option>');

    var resourceTypeId = $(resourceTypeOptions).val();
    var nowTime = new Date().getTime();
    $.get('/resource/LoadSubResourceTypes?resourceTypeId=' + resourceTypeId + "&time=" + nowTime,
        function (data, status) {
            if (data != null && data.Result != null && data.Result.length > 0) {
                var html = '<option value="">请选择具体类型</option>';
                for (var i = 0; i < data.Result.length; i++) {
                    html += '<option value=' + data.Result[i]['Id'] + '>' + data.Result[i]['Text'] + '</option>';
                };

                if (html != '') {
                    $(subTypeOptions).html(html);
                }
                else {
                    $(subTypeOptions).html('<option value="">--加载失败--</option>');
                }
            }
            else {
                $(subTypeOptions).html('<option value="">请选择具体类型</option>');
            }
        });
}