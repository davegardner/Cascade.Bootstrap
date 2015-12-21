// HOVER MENUS
// don't do anything if touch is supported
// OR if collapsed (DAG)
var collapsed = window.innerWidth <= 768; // @grid-float-breakpoint
if (!('ontouchstart' in document) && !collapsed) {
    $('.navbar .dropdown').hover(function () {
        $(this).find('.dropdown-menu').first().stop(true, true).slideDown(0);
    }, function () {
        var na = $(this);
        na.find('.dropdown-menu').first().stop(true, true).slideUp(0);
    });

    $('.navbar .dropdown > a').click(function () {
        if ($(this).attr('href').substr(-1) == '/') {
            window.location.href = $(this).attr('href').substr(0, $(this).attr('href').length - 1);
        }
        else {
            window.location.href = $(this).attr('href');
        }
    });
}