$(document).ready(function () {

            $("#PopUp").dialog({
                autoOpen: false,
                modal: true,
                appendTo: "form",
                width: 820,
                resizable: false,
                open: function () {
                    $("#tabs").tabs();
                    $("#tabs").css("height", "auto");
                },
                close: function () {
                    $("#tabs").tabs("destroy");
                },
                buttons: {
                    "Aceptar": function () {
                        //$(this).dialog("close");
                        $(".btnInvisible1").click();
                    },

                    "Cancelar": function () {
                        $(this).dialog("close");
                        //$(".btnInvisible3").click();
                    }

                },
                    
            }).parent().css('z-index', '1005');

            $("#PopUpEliminar").dialog({
                autoOpen: false,
                modal: false,
                appendTo: "form",
                open: function () {
                    $("#tabs").tabs("destroy");
                    $("#tabs").tabs();
                    $(".ui-widget-overlay.ui-front").css('z-index', '1009');
                    
                    
                },
                close: function () {
                    $("#tabs").tabs();
                    $(".ui-widget-overlay.ui-front").css('z-index', '100');
                },
                buttons: {
                    "Aceptar": function () {
                        $(this).dialog("close");
                        $(".btnInvisible2").click();
                    },

                    "Cancelar": function () {
                        $(this).dialog("close");
                    }

                }
            }).parent().css('z-index', '1010');

        });


        function abrirPopUp(titulo) {
            $("#PopUp").dialog("open");
            $("#PopUp").dialog({ title: titulo });
        }

        function crearTabsVistaCompleta() {
            $("#tabs").tabs();
        }

        function destruirTabsVistaCompleta() {
            $("#tabs").tabs("destroy");
        }

        function seleccionarTabs() {
            $('#tabs a[href="#tabs-2"]').click();
        }

        function destruyeTabsP() {
            $("#tabsP").tabs("destroy");
        }


        function crearTabsP() {
            $("#tabsP").tabs();
        }


        function seleccionarTabPerfilParcial() {
            $('#tabsP a[href="#tabsP-2"]').click();
        }

        function abrirPopUpEliminar() {
            $("#PopUpEliminar").dialog("open");
        }


        function cerrarPopUp() {
            $("#PopUp").dialog("close");
        }


        function uploadError(sender,args)
        {
        document.getElementById('MainContent_lblStatus').innerText = args.get_fileName(), 
	        "<span style='color:red;'>" + args.get_errorMessage() + "</span>";
        }

        function StartUpload(sender,args)
        {
          //document.getElementById('MainContent_lblStatus').innerText = 'Subiendo imagen.';
        }

        function UploadComplete(sender,args)
        {
         //__doPostBack("UpdateImage", "");
        }
