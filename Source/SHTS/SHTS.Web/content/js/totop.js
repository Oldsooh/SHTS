(function ($) {
    $.fn.UItoTop = function (options) {

        var defaults = {
            text: 'To Top',
            min: 200,
            inDelay: 400,
            outDelay: 400,
            containerID: 'toTop',
            containerHoverID: 'toTopHover',
            erweimaID: 'erweima',
            erweimaHoverID: 'erweimaHover',
            scrollSpeed: 1000,
            easingType: 'linear'
        };

        var settings = $.extend(defaults, options);
        var containerIDhash = '#' + settings.containerID;
        var containerHoverIDHash = '#' + settings.containerHoverID;

        $('body').append('<a href="#" id="' + settings.containerID + '">' + settings.text + '</a>');
        $(containerIDhash).hide().click(function () {
            $('html, body').animate({ scrollTop: 0 }, settings.scrollSpeed, settings.easingType);
            $('#' + settings.containerHoverID, this).stop().animate({ 'opacity': 0 }, settings.inDelay, settings.easingType);
            return false;
        })
		.prepend('<span id="' + settings.containerHoverID + '"></span>')
		.hover(function () {
		    $(containerHoverIDHash, this).stop().animate({
		        'opacity': 1
		    }, 600, 'linear');
		}, function () {
		    $(containerHoverIDHash, this).stop().animate({
		        'opacity': 0
		    }, 700, 'linear');
		});

        $("#" + settings.erweimaID).hover(function() {
            $("#ewm_pic").fadeIn(settings.inDelay);
            //setTimeout("$('#ewm_pic').fadeOut(500)",5000);
        }, function() {
            $("#ewm_pic").fadeOut(settings.Outdelay);
        });


        $(window).scroll(function () {
            var sd = $(window).scrollTop();
            if (typeof document.body.style.maxHeight === "undefined") {
                $(containerIDhash).css({
                    'position': 'absolute',
                    'top': $(window).scrollTop() + $(window).height() - 50
                });
            }
            if (sd > settings.min) {
                $("#" + settings.erweimaID).fadeIn(settings.inDelay);
                $(containerIDhash).fadeIn(settings.inDelay);
            }
            else {
                $("#" + settings.erweimaID).fadeOut(settings.Outdelay);
                $(containerIDhash).fadeOut(settings.Outdelay);
            }
        });

    };
    jQuery().UItoTop({ scrollSpeed: 200 });
})(jQuery);

