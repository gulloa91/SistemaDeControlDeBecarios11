$(document).ready(function () {

    setPopUpHorasBecario("PopUpControlDeHorasEncargado", "MainContent_btnInvisibleEnviarRevision", "MainContent_btnInvisibleCancelarRevision");
    //setDeletePopUp("PopUpEliminarAsignacion", "btnInvisible2");
    $("#PopUpControlDeHorasEncargado").dialog("option", "width", 600);

    setPopUpAsignarBecario("popUpConfirmar", "MainContent_btnInvisibleAsignacion", "MainContent_btnInvisibleAsignacion2");
    //setDeletePopUp("PopUpEliminarAsignacion", "btnInvisible2");
    $("#popUpConfirmar").dialog("option", "width", 600);
});


/** Función que se encarga de crear un Pop Up para asignar nuevamente becario a encargado **/
function setPopUpAsignarBecario(popUpId, aceptarClick, rechazarClick) {
    $("#" + popUpId).dialog({
        autoOpen: false,
        modal: true,
        appendTo: "form",
        width: 500,
        buttons: {
            "Aceptar": function () {
                //alert("Acepto");
                //$(this).dialog("close");
                $("#" + aceptarClick).click();
            },

            "Rechazar": function () {
                $("#" + rechazarClick).click();
                //alert("Rechazo");
                //$(this).dialog("close");
            }

        }
    }).parent().css('z-index', '1005');
}

function setPopUpHorasBecario(popUpId, aceptarClick, rechazarClick) {
    $("#" + popUpId).dialog({
        autoOpen: false,
        modal: true,
        appendTo: "form",
        width: 500,
        buttons: {
            "Aceptar": function () {
                //alert("Acepto");
                //$(this).dialog("close");
                $("#" + aceptarClick).click();
            },

            "Cancelar": function () {
                $("#" + rechazarClick).click();
                //alert("Rechazo");
                //$(this).dialog("close");
            }

        }
    }).parent().css('z-index', '1005');
}