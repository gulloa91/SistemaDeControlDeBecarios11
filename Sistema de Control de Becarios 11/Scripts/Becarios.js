$(document).ready(function () {

            $("#PopUp").dialog({
                autoOpen: false,
                modal: true,
                appendTo: "form",
                width: 690,
                open: function () {
                    $("#tabs").tabs();
                },
                close: function () {
                    $("#tabs").tabs("destroy");
                },
                buttons: {
                    "Aceptar": function () {
                        $(this).dialog("close");
                        $(".btnInvisible1").click();
                    },

                    "Cancelar": function () {
                        $(this).dialog("close");
                        $(".btnInvisible3").click();
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


        function abrirPopUp() {
            $("#PopUp").dialog("open");
        }

        function crearTabs() {
            $("#tabs").tabs("destroy");
            $("#tabs").tabs();
        }

        function destruyeTabsP() {
            $("#tabsP").tabs("destroy");
        }


        function crearTabsP() {
            $("#tabsP").tabs();
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
          document.getElementById('MainContent_lblStatus').innerText = 'Subiendo imagen.';
        }

        function UploadComplete(sender,args)
        {
         var filename = args.get_fileName();
         var contentType = args.get_contentType();
         var text = "Tamaño de " + filename + " son " + args.get_length() + " bytes";
         if (contentType.length > 0)
         {
          text += " es un tipo '" + contentType + "'.";
          }
          document.getElementById('MainContent_lblStatus').innerText = text;
        }