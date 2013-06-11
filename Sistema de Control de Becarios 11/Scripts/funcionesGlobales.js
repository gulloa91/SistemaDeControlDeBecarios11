function enterBuscar(e, idBoton) {
    if (e.keyCode == 13) {
        $("#"+idBoton).click();
        e.preventDefault();
    }

}

/** Mensaje de información **/
$(document).ready(function () {
    $("#popUpEspera").dialog({
        autoOpen: false,
        modal: true,
        width: 350,
        height: 
        open: function () {
            $(".ui-widget-overlay.ui-front").css('z-index', '1099');
        },
        close: function () {
            $(".ui-widget-overlay.ui-front").css('z-index', '150');
        },
    }).parent().css('z-index', '1100');
});