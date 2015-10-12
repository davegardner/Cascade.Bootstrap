/* Dave Gardner October 2015
 * Support for duplicating Bootstrap Swatches
 * */

(function ($) {
	$('#cbduplicate').click(function (ev) {
		var img = $('.swatchContainer.active img');
		var swatch = $('.swatchContainer.active img').data("swatch");
		
		var toSwatch = prompt("Duplicate swatch '" + swatch + "' and call it:");
		if (toSwatch == null || toSwatch == "")
			return;
		var legalName = /^[A-Za-z0-9_-]+$/;
		if (!legalName.test(toSwatch))
		{
			alert("'" + toSwatch + "' is an invalid name. Please use letters, digits, _ and - only. Case is not important.");
			return;
		}

		// ask server to do it please
		$.ajax({
			url: '/Admin/Settings/CascadeBootstrapTheme/DuplicateSwatch',
			data: { fromSwatch: swatch, toSwatch: toSwatch },
			method: 'POST',
			success: function (data) {
				if (!data){
					// success -- reload so new swatch is displayed
					location.reload(true);
				}
				else
					alert(data);
			},
			error: function () { alert('Unable to contact server');}
		});
	});

})(jQuery);

