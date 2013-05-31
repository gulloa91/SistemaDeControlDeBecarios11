function enterBuscar(e, idBoton) {
    if (e.keyCode == 13) {
        $("#"+idBoton).click();
        e.preventDefault();
    }

}