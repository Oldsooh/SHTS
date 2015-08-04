function expand(targetid, objN) {
    var d = $('#'+targetid);
    var sb = $('#' + objN);
    if (d.hasClass('hide')) {
        d.removeClass('hide');
        sb.html('收缩');
    }
    else {
        d.addClass('hide');
        sb.html('展开');
    }
}

function expandByClassName(className, objN){
    var sb = $('#' + objN);
    
    $('.'+ className).each(function (){
    if ($(this).hasClass('hide')) {
        $(this).removeClass('hide');
        sb.html('收缩');
    }
    else {
        $(this).addClass('hide');
        sb.html('展开');
    }
    });
    
}