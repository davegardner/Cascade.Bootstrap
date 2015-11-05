/* Dave Gardner October 2015
 * Support for duplicating Bootstrap Swatches
 * */

(function ($) {
	var baseUrl = 'CascadeBootstrapTheme';

	$('#cbduplicate').click(function (ev) {
		var img = $('.swatchContainer.active img');
		var swatch = img.parent().data("swatch");

		var toSwatch = prompt("Duplicate swatch '" + swatch + "' and call it:");
		if (toSwatch == null || toSwatch == "")
			return;
		var legalName = /^[A-Za-z0-9_-]+$/;
		if (!legalName.test(toSwatch)) {
			alert("'" + toSwatch + "' is an invalid name. Please use letters, digits, _ and - only. Case is not important.");
			return;
		}

		// ask server to do it please
		$.ajax({
			url: baseUrl + '/DuplicateSwatch',
			data: { fromSwatch: swatch, toSwatch: toSwatch },
			method: 'POST',
			success: function (data) {
				if (!data) {
					// success -- create a theme too?
					if (confirm("Do you want to create a new theme called '" + toSwatch + "' as well?")) {
						DuplicateTheme(swatch, toSwatch);
					}

					// reload so they can see new swatch
					location.reload(true);
				}
				else
					alert(data);
			},
			error: function () { alert('Unable to contact server'); }
		});
	});

	// duplicates the theme, assuming the theme has the same name as the swatch
	var DuplicateTheme = function (fromTheme, toTheme) {
		$.ajax({
			url: baseUrl + '/DuplicateTheme',
			data: { fromTheme: fromTheme, toTheme: toTheme },
			method: 'POST',
			success: function (data) {
				if (!data) {
					// success 
				}
				else
					alert(data);
			},
			error: function () { alert('Unable to contact server'); }
		});
	}


	$('.swatchContainer').click(function () {
		var swatch = $(this).data("swatch");
		$('#CascadeBootstrapThemeSettings_Swatch').val(swatch);
		$('.swatchContainer').removeClass('active');
		$('.' + swatch).addClass('active');

		GetLessValue($('#navColorContainer'), ".navbar-default .navbar-nav > li > a", swatch, "color");
		GetLessValue($('#navColorContainer'), ".navbar-default", swatch, "background-color");
		GetLessValue($('#inverseColorContainer'), ".navbar-inverse .navbar-nav > li > a", swatch, "color");
		GetLessValue($('#inverseColorContainer'), ".navbar-inverse", swatch, "background-color");

		function GetLessValue(el, style, swatch, attribute) {
			$.ajax({
				url: baseUrl + '/GetCssValue',
				data: { Swatch: swatch, Style: style, Attribute: attribute },
				method: 'GET',
				success: function (data)
				{
					el.css(attribute, data );
				},
				error: function () { alert('Unable to contact server'); }
			});
		}

	});

})(jQuery);

