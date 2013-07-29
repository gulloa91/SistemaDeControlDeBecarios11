$(document).ready(function () {
    setPopUp("PopUpCtrlBecario", "btnInvisibleEnviarReporte");
    setPopUp("PopUpAyudaParcialBecario");

    $("#PopUpCtrlBecario").dialog("option", "width", 600);
    $(".dateText").datepicker();
    $("#siguienteAsig").dialog({
        autoOpen: false,
        modal: true,
        width: 430,
        appendTo: "form",
        open: function () {
            
            $(".ui-widget-overlay.ui-front").css('z-index', '1899');
        },
        close: function () {
            $(".ui-widget-overlay.ui-front").css('z-index', '100');
            
        },
        buttons: {
            "Aceptar": function () {
                $(".btnInvisibleAsig").click();
            }

        }
    }).parent().css('z-index', '1900');

});


function abrir() {
    $("#quickfix").remove();
    $("body").append('<div id="quickfix" class="ui-widget-overlay ui-front"></div>');
    $("#siguienteAsig").dialog({ title: "Asignacion" });
    $("#siguienteAsig").dialog("open");
    
}
function cerrar() {
    $("#siguienteAsig").dialog("close");
    $("#quickfix").remove();
    $("#quickfix").remove();s
}