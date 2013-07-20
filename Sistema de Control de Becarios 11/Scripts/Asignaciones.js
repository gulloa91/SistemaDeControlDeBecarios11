$(document).ready(function () {
    setPopUp("PopUpAyuda", "btnInvisible1");
    setPopUp("PopUpAsignacion", "btnInvisible1");
    setPopUp("PopUpConfirmarRechazoBecario", "btnInvisibleConfirmarRechazo");
    setPopUpVerBecariosAsignados();
    setPopUpAceptarRechazarBecarios();
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

function setPopUpAceptarRechazarBecarios() {

    $("#PopUpAsignacionEncargado").dialog({
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
            "Aceptar Asignación": function () {
                $(".btnInvisibleAceptarAsignacionEncargado").click();
            },

            "Rechazar Asignación": function () {
                $(".btnInvisibleRechazarAsignacionEncargado").click();
            },

            "Decidir más tarde": function () {
                $(this).dialog("close");
            }

        }
    }).parent().css('z-index', '1025');
}
