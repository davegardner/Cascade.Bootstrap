$(document).ready(function () {
    // NAVIGATION
    // change submenu arrow
    $('.dropdown-menu i').removeClass('fa-angle-down').addClass('fa-angle-right').addClass('pull-right');

    // make sure submenu item links work
    $('.dropdown-menu > .dropdown > a').click(function () {
        window.location.href = $(this).attr('href');
    });

    // add top margin to first container below menu when using fixed navbar
    //$('.navbar-wrapper .navbar-fixed-top').parent('div').next('div').css('margin-top', ($('.navbar-fixed-top').height() + 20) + 'px');
    //$(window).resize(function () {
        //$('.navbar-wrapper .navbar-fixed-top').parent('div').next('div').css('margin-top', ($('.navbar-fixed-top').height() + 20) + 'px');
    //});

    // add bottom margin to body equal to the footer height when using sticky footer
    if ($('html').hasClass('sticky-footer')) {
        var footerHeight = $('#layout-footer').height();

        $('body').css('margin-bottom', footerHeight + 'px');
    }

    // ALERTS
    $('.alert').alert();

    // FORMS
    $('.input-validation-error').addClass('form-control');
    $('.input-validation-error').prev('label').addClass('control-label');
    $('.input-validation-error').closest('.form-group').addClass('has-error');

    // PAGINATION
    $('#pagination ul').removeClass('pager').addClass('pagination');
    $('#pager-current').parent('li').addClass('active');

    // TO TOP
    $(window).scroll(function () {
        if ($(this).scrollTop() !== 0) {
            $("#toTop").fadeIn();
        } else {
            $("#toTop").fadeOut();
        }
    });

    $("#toTop").click(function () {
        $("body,html").animate({ scrollTop: 0 }, 2e3);
    });

    // PASSWORD CAPS LOCK ON ALERT
    $('input[type=password]').keypress(function (e) {
        kc = e.keyCode ? e.keyCode : e.which;
        sk = e.shiftKey ? e.shiftKey : ((kc == 16) ? true : false);
        if (((kc >= 65 && kc <= 90) && !sk) || ((kc >= 97 && kc <= 122) && sk)) {
            if (!$(this).next('.capsAlert').length) {
                $(this).after('<span class="capsAlert text-warning"><i class="fa fa-warning"></i>&nbsp;Caps Lock is on</span>');
            }
        }
        else {
            $(this).next('.capsAlert').remove();
        }
    });

	////////////////// REVEAL IMAGES //////////////////////////////////////////////////////

	// Immediately remove the src attribute from all images to be 'revealed'
	// if an image has already started loading it will be cancelled
	$('img.cascade-reveal').each(function () {
		var t = $(this);
		t.attr('data-src', t.attr('src'));
		t.removeAttr('src');
	});

	var fadeInImage = function (e) {
		e.stopPropagation();
		e.data.removeClass('cascade-reveal').addClass('cascade-reveal-fade-in');
	}

	// load and reveal images when they appear in the viewport
	var loadImage = function () {
		$('img[data-src].cascade-reveal').each(function () {
			if (isElementInViewport(this)) {
				var t = $(this);

				// fade-in the image after it has loaded
				t.one('load', t, fadeInImage);

				// load the image
				t.attr('src', t.attr('data-src'));
				t.removeAttr('data-src'); // only process this image once
			}
		});
	}

	function isElementInViewport(el) {

		var rect = el.getBoundingClientRect();

		return (rect.right >= 0
			&& rect.bottom >= 0
			&& rect.left <= (window.innerWidth || document.documentElement.clientWidth)
			&& rect.top <= (window.innerHeight || document.documentElement.clientHeight));
	}

	$(window).on('load resize scroll', loadImage);

});