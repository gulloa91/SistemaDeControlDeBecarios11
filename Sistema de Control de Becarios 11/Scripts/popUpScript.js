/** Mensaje de información **/
$(document).ready(function () {
    $("#popUpMensaje").dialog({
        autoOpen: false,
        modal: true,
        width: 500,
        open: function () {
            $(".ui-widget-overlay.ui-front").css('z-index', '1049');
        },
        close: function () {
            $(".ui-widget-overlay.ui-front").css('z-index', '100');
        },
        buttons: {
            "Aceptar": function () {
                $(this).dialog("close");
            }
        }
    }).parent().css('z-index', '1050');
});

/** Función que se encarga de crear un Pop Up **/
function setPopUp(popUpId, aceptarClick) {

    $("#" + popUpId).dialog({
        autoOpen: false,
        modal: true,
        appendTo: "form",
        width: 500,
        buttons: {
            "Aceptar": function () {
                //$(this).dialog("close");
                $("." + aceptarClick).click();
            },

            "Cancelar": function () {
                $(this).dialog("close");
            }

        }
    }).parent().css('z-index', '1005');
}

/** Función que se encarga de crear un Pop Up de Ayuda**/
function setPopUpAyuda(popUpId) {

    $("#" + popUpId).dialog({
        autoOpen: false,
        modal: true,
        appendTo: "form",
        width: 650,
        buttons: {
            "Cerrar": function () {
                $(this).dialog("close");
            }

        }
    }).parent().css('z-index', '1005');
}

/** Función para iteracción base del popUp de eliminar **/
function setDeletePopUp( popUpId, aceptarClick ){
    $("#" + popUpId).dialog({
        autoOpen: false,
        modal: false,
        appendTo: "form",
        open: function () {
            $(".ui-widget-overlay.ui-front").css('z-index', '1009');
        },
        close: function () {
            $(".ui-widget-overlay.ui-front").css('z-index', '100');
        },
        buttons: {
            "Sí": function () {
                $(this).dialog("close");
                $("." + aceptarClick).click();
            },

            "No": function () {
                $(this).dialog("close");
            }

        }
    }).parent().css('z-index', '1010');
}

function abrirPopUp(popUpId, titulo) {
    $("#" + popUpId).dialog("open");
    $("#" + popUpId).dialog({ title: titulo });
}


function abrirPopUpPersonalizado(popUpId, titulo, mensaje) {
    $("#" + popUpId).dialog("open");
    $("#" + popUpId).text(mensaje);
    $("#" + popUpId).dialog({ title: titulo });
}


function cerrarPopUp(popUpId) {
    $("#" + popUpId).dialog("close");
}

/** Mensaje de espera **/
$(document).ready(function () {
    $("#popUpEspera").dialog({
        autoOpen: false,
        modal: true,
        width: 350,
        open: function () {
            $(".ui-widget-overlay.ui-front").css('z-index', '1499');
        },
        close: function () {
            $(".ui-widget-overlay.ui-front").css('z-index', '150');
        },
    }).parent().css('z-index', '1500');
});