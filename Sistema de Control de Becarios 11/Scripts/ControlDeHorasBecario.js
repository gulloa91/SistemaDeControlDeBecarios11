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
        buttons: {
            "Aceptar": function () {
                $(".btnInvisibleAsig").click();
            }

        }
    }).parent().css('z-index', '1010');

});


function abrir() {
    $("#siguienteAsig").dialog({ title: "Asignacion" });
    $("#siguienteAsig").dialog("open");
}
function cerrar() {
    $("#siguienteAsig").dialog("close");
}