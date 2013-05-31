$(document).ready(function () {

    setPopUp("PopUpAsignacion", "btnInvisible1");
    setPopUpVerBecariosAsignados();
    setDeletePopUp("PopUpEliminarAsignacion", "btnInvisible2");
    $("#PopUpAsignacion").dialog("option", "width", 600);
});

function setPopUpVerBecariosAsignados() {

    $("#PopUpVerBecariosAsignados").dialog({
        autoOpen: false,
        modal: true,
        appendTo: "form",
        width: 600,
        open: function () {
            $(".ui-widget-overlay.ui-front").css('z-index', '1020');
        },
        close: function () {
            $(".ui-widget-overlay.ui-front").css('z-index', '100');
        },
        buttons: {
            "Cerrar": function () {
                $(this).dialog("close");
            }

        }
    }).parent().css('z-index', '1025');
}

function abrirPopUpVerBecariosAsignados(titulo) {
    $("#PopUpVerBecariosAsignados").dialog("open");
    $("#PopUpVerBecariosAsignados").dialog("option", "title", ("Becarios asignados a: " + titulo));
}

function cerrarPopUpVerBecariosAsignados() {
    $("#PopUpVerBecariosAsignados").dialog("close");
}