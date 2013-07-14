using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;


public partial class Cuentas : System.Web.UI.Page
{
    private static CommonServices commonService;
    private ControladoraCuentas controladoraCuentas = new ControladoraCuentas();
    private ControladoraPerfiles controladoraPerfiles = new ControladoraPerfiles();
    private ControladoraBecarios cb = new ControladoraBecarios();
    private static List<int> lsTipoCuentasDrp = new List<int>();
    private static Object [] datosOriginales = new Object[4]; // datos usados para modificar la cuenta, contienen los valores viejos de la cuenta
    private static Object [] datosOriginalesAsociacion = new Object[2]; // datos usados para modificar la asociacion de una cuenta a un perfil, contiene los datos viejos de la asociacion
    private int drpIndex = 0;

    private static int modo = 0; // variable utilizada para controlar la ejecución de las acciones en la controladora (eliminar, modificar, insertar)

    protected void Page_Load(object sender, EventArgs e)
    {
        commonService = new CommonServices(UpdateInfo);

        List<int> permisos = new List<int>();
        permisos = Session["ListaPermisos"] as List<int>;

        if (permisos == null)
        {
            Session["Nombre"] = "";
            Response.Redirect("~/Default.aspx");
        }
        else
        {

            int permiso = 0; /* Query to user validation */
            if (permisos.Contains(11))
            {
                permiso = 11;
            }
            else
            {
                if (permisos.Contains(12))
                {
                    permiso = 12;
                }
            }

            switch (permiso)
            {
                case 11:
                    { // Vista completa
                        multiViewCuentas.SetActiveView(vistaAdmin);
                        if (!IsPostBack)
                        {
                            llenarDrpDown();
                            llenarGridCuentas();
                            //llenarDrpPersona();
                        }
                    } break;

                case 12:
                    { // Vista Parcial
                        multiViewCuentas.SetActiveView(viewEncBec);
                        if (!IsPostBack)
                        {
                            llenarCamposPersonal();
                            controlarACCAPers(false);
                        }
                        controlarCamposPersonales(false);
                    } break;

                default:
                    { // Vista sin permiso
                        multiViewCuentas.ActiveViewIndex = 2;
                    } break;


            }

        }
    }


    /*
     * Efectúa: Se encarga de insertar o modificar una cuenta. Primero guarda los datos de la cuenta y la asociación en dos arreglos y luego llama a 
     * la controladora para que efectúe la acción que corresponde (insertar o modificar)
     * Requiere: N/A
     * Modifica: La tabla de cuentas y cuenta_perfil en la base de datos
     * */
    protected void btnInvisible1_Click(object sender, EventArgs e) // insertar o modificar una cuenta
    {
        string mensaje = "-1";
        Object[] datos = new Object[4]; // datos que se enviaran a la controladora para insertar la cuenta
        if(modo==1 || modo==2){
            datos[0] = this.txtUsuario.Text;
            datos[1] = this.cntUsuario.Text;
            datos[2] = this.txtFechaAux.Text;
            if (lsTipoCuentasDrp[this.drpDownPerfiles.SelectedIndex] != 1 && lsTipoCuentasDrp[this.drpDownPerfiles.SelectedIndex] != 2)
            {
                datos[3] = "000000000"; // si es administrador no necesita cedula, se usa el default
            }
            else
            {
                datos[3] = this.drpPersona.SelectedValue; //en caso de ser becario o encargado se le asigna la cedula correspondiente
            }
        }
        if(modo==1){ // inserta cuenta
           mensaje = controladoraCuentas.ejecutar(modo, datos, null);
           if(mensaje==""){ // si se inserto correctamente la cuenta, se crea la asignacion con el perfil
              Object [] datosAsoc = new Object[2];
              datosAsoc[0] = this.txtUsuario.Text;
              datosAsoc[1] = this.drpDownPerfiles.SelectedItem.Text;
              controladoraCuentas.ejecutarAsociacion(1, datosAsoc, null);
              if(mensaje==""){
                  mensaje = "Se ha insertado correctamente la cuenta";
              }
           }
        }
        if(modo==2){
            //borro la asociacion de la cuenta vieja para poder modificar
            mensaje = controladoraCuentas.ejecutarAsociacion(3, datosOriginalesAsociacion, null);
            if(mensaje==""){
                mensaje = controladoraCuentas.ejecutar(modo, datos, datosOriginales);
                if(mensaje==""){
                    Object[] datosAsoc = new Object[2];
                    datosAsoc[0] = this.txtUsuario.Text;
                    datosAsoc[1] = this.drpDownPerfiles.SelectedItem.Text;
                    controladoraCuentas.ejecutarAsociacion(1, datosAsoc, null); //vuelvo a crear la nueva asociacion
                    mensaje = "Se ha modificado correctamente la cuenta";
                }
            }
        }
        if(mensaje!="-1"){
            commonService.mensajeJavascript(mensaje, "Aviso");
        }
        llenarGridCuentas(); // actualizo el grid
        commonService.cerrarPopUp("PopUp");
    }


    /*
     * Efectúa: Se encarga de eliminar una cuenta. Primero guarda los datos de la cuenta en un arreglo y luego llama a 
     * la controladora para que efectúe la acción que corresponde.
     * Requiere: N/A
     * Modifica: La tabla de cuentas y cuenta_perfil en la base de datos
     * */
    protected void btnInvisible2_Click(object sender, EventArgs e) //eliminar una cuenta
    {
        if ((string)(Session["Cuenta"]) != this.txtUsuario.Text) // condicion para evitar borrar la cuenta activa
        {
            String mensaje = "-1";
            if (modo != 2)
            {
                commonService.cerrarPopUp("PopUp");//cierro el popUp con los datos
                datosOriginales[0] = this.txtUsuario.Text;
                datosOriginales[1] = this.cntUsuario.Text;
                datosOriginales[2] = this.txtFechaAux.Text;
                int drpIndex = this.drpDownPerfiles.SelectedIndex;
                if (lsTipoCuentasDrp[drpIndex] == 1 || lsTipoCuentasDrp[drpIndex] == 2)
                {
                    datosOriginales[3] = cb.obtieneCedulaDeUsuario(this.txtUsuario.Text); // en caso de ser becario o encargado
                }
                else {
                    datosOriginales[3] = "000000000"; // en caso de ser administrador
                }
                datosOriginalesAsociacion[0] = this.txtUsuario.Text;
                datosOriginalesAsociacion[1] = this.drpDownPerfiles.SelectedItem.Text;
                mensaje = controladoraCuentas.ejecutarAsociacion(3, datosOriginalesAsociacion, null); //borro primero la asociacion
                if (mensaje == "")
                {
                    mensaje = controladoraCuentas.ejecutar(3, datosOriginales, null); // elimino la cuenta
                    commonService.mensajeJavascript(mensaje, "Atención");
                }
            }
            else {
                mensaje = controladoraCuentas.ejecutarAsociacion(3, datosOriginalesAsociacion, null);
                if (mensaje == "")
                {
                    mensaje = controladoraCuentas.ejecutar(3, datosOriginales, null);
                    commonService.mensajeJavascript(mensaje, "Atención");
                }
            }
            llenarGridCuentas(); // actualizo el grid de cuentas
        }
        else {
            commonService.mensajeJavascript("No se puede eliminar la cuenta activa.", "Atención");
        }
    }


    /*
    * -----------------------------------------------------------------------
    * BUTTON: CLICKS
    * -----------------------------------------------------------------------
    */
    // INSERTAR CLICK
    /* Efectúa: Se encarga de limpiar, cargar y habilitar los campos de texto, además prepara las ventanas emergentes donde se insertarán 
     * los datos de la cuenta.
     * Requiere: N/A
     * Modifica: N/A
     * */
    protected void btnInsertarCuenta_Click(object sender, EventArgs e)
    {
        vaciarCampos(true); // borro los campos
        controlarCampos(true); // activo los campos
        controlarCedula(false); 
        llenarDrpDown(); // lleno el dropdown con los perfiles
        modo = 1; // activo el modo para insertar
        commonService.abrirPopUp("PopUp", "Insertar Nueva Cuenta"); // abro el pop up para insertar
        commonService.mostrarPrimerBotonDePopUp("PopUp");
        mostrarBotonesPrincipales(false);
    }

    // MODIFICAR CLICK
    /* Efectúa: Se encarga de cargar y habilitar los campos de texto, además prepara las ventanas emergentes donde se modificarán 
     * los datos de la cuenta. Guarda los datos de la cuenta antes de modificarse.
     * Requiere: N/A
     * Modifica: N/A
     * */
    protected void btnModificarCuenta_Click(object sender, EventArgs e)
    {
        commonService.mostrarPrimerBotonDePopUp("PopUp");
        // guardo los datos antes de modificar
        datosOriginales[0] = this.txtUsuario.Text;
        datosOriginales[1] = this.cntUsuario.Text;
        datosOriginales[2] = this.txtFechaAux.Text;
        datosOriginales[3] = cb.obtieneCedulaDeUsuario(this.txtUsuario.Text); 
        // guardo los datos de la asociacion vieja
        datosOriginalesAsociacion[0] = this.txtUsuario.Text;
        datosOriginalesAsociacion[1] = this.drpDownPerfiles.SelectedItem.Text;
        drpIndex = this.drpDownPerfiles.SelectedIndex;
        if (lsTipoCuentasDrp[drpIndex] == 1 || lsTipoCuentasDrp[drpIndex] == 2)
        {
            switch (lsTipoCuentasDrp[drpIndex])
            {
                case 1:
                    {
                        llenarDrpPersonaModificar(1);
                    } break;
                case 2:
                    {
                        llenarDrpPersonaModificar(0);
                    } break;
            }
            this.drpPersona.Visible = true;
        }
        controlarCampos(true);
        // activo el modo en modificar
        modo = 2;
    }

    // ELIMINAR CLICK
    /* Efectúa: Carga la ventana emergente para eliminar la cuenta.
     * Requiere: N/A
     * Modifica: N/A
     * */
    protected void btnEliminarCuenta_Click(object sender, EventArgs e)
    {
        commonService.abrirPopUp("PopUpEliminar", "Eliminar Cuenta"); // activo la ventana emergente del eliminar
    }

//--------------------------------------------

    /* Efectúa: Se encarga de llenar el dropdown de los perfiles. Además guarda en una lista el permiso de cada perfil,  
     * para efectos de controlar el campo de nombre, ya que el perfil de administrador no lo ocupa.
     * Requiere: Que existan perfiles en la base de datos. (Siempre deben existir Becario, Encargado y Administrador)
     * Modifica: La lista en donde se guarda el permiso de cada perfil.
     * */
    protected void llenarDrpDown() {

        DataTable dtPerfiles = controladoraPerfiles.consultar(); // consulto todos los perfiles disponibles
        lsTipoCuentasDrp.Clear(); // limpio la lista
        this.drpDownPerfiles.SelectedIndex = -1;
        this.drpDownPerfiles.Items.Clear(); // limpio el dropdown antes de llenarlo
        int i = 0;
        foreach(DataRow r in dtPerfiles.Rows){
            ListItem item = new ListItem(commonService.procesarStringDeUI(r[0].ToString()), i+"");
            this.drpDownPerfiles.Items.Add(item); // inserto en el dropdown y en la lista los perfiles y su correspondiente permiso
            lsTipoCuentasDrp.Add(Convert.ToInt32(r[1]));
            i++;
        }
        this.drpDownPerfiles.DataBind();
        this.drpDownPerfiles.SelectedIndex = drpIndex;

        if (lsTipoCuentasDrp[0] == 0)
        {
            ListItem aux = this.drpDownPerfiles.Items.FindByValue("0");
            this.drpDownPerfiles.SelectedValue = aux.Value; // en caso de ser administrador no se muestra la cedula
            this.drpDownPerfiles.SelectedIndex = 0;
            controlarCedula(false);
        }
         
    }


    /* Efectúa: Controla el evento cuando se cambia la selección de un elemento en el dropdown de perfiles.
     * Requiere: N/A
     * Modifica: N/A
     * */
    protected void drpDownPerfiles_SelectedIndexChanged(object sender, EventArgs e)
    {
        drpIndex = this.drpDownPerfiles.SelectedIndex; // guardo el indice de la posicion del dropdown que activo el evento
        if (lsTipoCuentasDrp[drpIndex] == 1 || lsTipoCuentasDrp[drpIndex] == 2)
        {
            switch (lsTipoCuentasDrp[drpIndex]) // en caso de ser becario o encargado
            {
                case 1:
                    {
                        llenarDrpPersona(1); // lleno con los datos de los encargados sin cuentas
                    } break;
                case 2:
                    {
                        llenarDrpPersona(0); // lleno con los datos de los becarios sin cuentas
                    } break;
            }
            controlarCedula(true); // activo el campo de cedula
        }
        else{
            controlarCedula(false); // en caso de ser administrador no muestro cedula
        }
    }

    /* Efectúa: Carga el nombre de la persona cuando se selecciona un elemento en el dropdown de personas.
     * Requiere: N/A
     * Modifica: N/A
     * */
    protected void drpDownPersona_SelectedIndexChanged(object sender, EventArgs e)
    {
        this.txtNombrePersona.Text = this.drpPersona.SelectedItem.ToString(); // cargo el nombre de la persona en el campo de texto cuando el evento se activa
    }

    /* Efectúa: Muestra u oculta los botones de eliminar o modificar, de acuerdo con la opcion que indique la variable "accion"
     * Requiere: N/A
     * Modifica: N/A
     * */
    protected void mostrarBotonesPrincipales(Boolean accion) { 
        if(accion){
            this.btnEliminarCuenta.Visible = true; // activo el boton de eliminar cuenta
            this.btnModificarCuenta.Visible = true; // activo el boton para modificar cuenta
        }
        else{
            this.btnEliminarCuenta.Visible = false; // descativo el boton para eliminar cuenta
            this.btnModificarCuenta.Visible = false; // desactivo el boton para modificar cuenta
        }
    }

    /* Efectúa: Se encarga de limpiar los campos de texto, cuando la variable "accion" lo indique, para la vista administrador.
     * Requiere: N/A
     * Modifica: N/A
     * */
    protected void vaciarCampos(Boolean accion) { 
        // limpio los campos de las cajas de texto en caso de que accion sea verdadero
        if(accion){
            this.txtUsuario.Text = "";
            this.cntUsuario.Text = "";
            this.cofCntUsuario.Text = "";
            this.txtFechaAux.Text = "";
            this.txtNombrePersona.Text = "";
        }
    }

    /* Efectúa: Se encarga de limpiar los campos de texto, cuando la variable "accion" lo indique, para la vista que no es administrador.
     * Requiere: N/A
     * Modifica: N/A
     * */
    protected void vaciarCamposPersonal(Boolean accion)
    {
        // limpio los campos de las cajas de texto en caso de que accion sea verdadero
        if (accion)
        {
            this.txtUsuarioPers.Text = "";
            this.txtContrasenaPers.Text = "";
            this.txtConfContrasenaPers.Text = "";
            this.txtCedulaPers.Text = "";
        }
    }

    /* Efectúa: Muestra u oculta los campos de los datos, dependiendo del valor de la variable accion, para la vista del perfil administrador.
     * Requiere: N/A
     * Modifica: N/A
     * */
    protected void controlarCampos(Boolean accion) {
        // en caso de que accion sea verdadera habilito los campos de texto
        if (accion)
        {
            this.txtUsuario.Enabled = true;
            this.cntUsuario.Enabled = true;
            this.cofCntUsuario.Enabled = true;
            this.drpDownPerfiles.Enabled = true;
        }
        // en caso de que accion no sea verdadera deshabilito los campos de texto
        else {
            this.txtUsuario.Enabled = false;
            this.cntUsuario.Enabled = false;
            this.cofCntUsuario.Enabled = false;
            this.drpDownPerfiles.Enabled = false;
        }
    }

    /* Efectúa: Muestra u oculta los campos de los datos, dependiendo del valor de la variable accion, para la vista del perfil que no es administrador.
     * Requiere: N/A
     * Modifica: N/A
     * */
    protected void controlarCamposPersonales(Boolean accion)
    {
        // en caso de que accion sea verdadera habilito los campos de texto
        if (accion)
        {
            this.txtUsuarioPers.Enabled = false;
            this.txtContrasenaPers.Enabled = true;
            this.txtConfContrasenaPers.Enabled = true;
            this.txtCedulaPers.Enabled = false;
            this.txtPerfil.Enabled = false;
        }
        // en caso de que accion no sea verdadera deshabilito los campos de texto
        else
        {
            this.txtUsuarioPers.Enabled = false;
            this.txtContrasenaPers.Enabled = false;
            this.txtConfContrasenaPers.Enabled = false;
            this.txtCedulaPers.Enabled = false;
            this.txtPerfil.Enabled = false;
        }
    }

    /* Efectúa: Muestra u oculta el campo correspondiente al nombre de la persona en la vista administrador.
     * Requiere: N/A
     * Modifica: N/A
     * */
    protected void controlarCedula(Boolean accion) {
        if (accion)
        {
            // en caso de que accion sea verdadera, activo los campos referentes al nombre de la persona
            this.drpPersona.Visible = true;
            this.lblCedula.Visible = true;
            this.txtNombrePersona.Visible = true;
        }
        else {
            // en caso de que accion sea falsa, los desactivo
            this.drpPersona.Visible = false;
            this.lblCedula.Visible = false;
            this.txtNombrePersona.Visible = false;
        }
    }

    /* Efectúa: Muestra u oculta el campo correspondiente al nombre de la persona en la vista que no es administrador.
     * Requiere: N/A
     * Modifica: N/A
     * */
    protected void controlarCedulaPersonal(Boolean accion)
    {
        
        if (accion)
        {
            this.txtCedulaPers.Visible = true;
            this.lblCedulaPers.Visible = true;
        }
        else
        {
            this.txtCedulaPers.Visible = false;
            this.lblCedulaPers.Visible = false;
        }
    }

    /* Efectúa: Activa o desactiva los botones de aceptar y cancelar, dependiendo de la opción que indique la variable accion.
     * Requiere: N/A
     * Modifica: N/A
     * */
    protected void controlarACCAPers(Boolean accion){
        // activo aceptar y cancelar cuando la variable sea true
        if(accion){
            btnAcepPers.Enabled = true;
            btnCancPers.Enabled = true;
        }
            // desactivo aceptar y cancelar cuando la variable sea false
        else{
            btnAcepPers.Enabled = false;
            btnCancPers.Enabled = false;
        }
    }

    /* Efectúa: Se encarga de cargar los datos correspondientes a la cuenta seleccionada en el grid. Además muestra la ventana emergente correspondiente
     * a consultar cuenta.
     * Requiere: N/A
     * Modifica: N/A
     * */
    protected void GridViewCuentas_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        switch(e.CommandName){
            case "seleccionarPosibleCuenta": { // cuando se selecciona una tupla del grid de todas las cuentas
                GridViewRow filaSeleccionda = this.GridViewCuentas.Rows[Convert.ToInt32(e.CommandArgument)]; // obtengo el indice de la fila seleccionada
                TextBox txtU = filaSeleccionda.FindControl("txtUsuarioG") as TextBox; // obtengo el usuario seleccionado de la tupla
                TextBox txtC = filaSeleccionda.FindControl("txtContrasenna") as TextBox; // obtengo la clave de la tupla seleccionada
                DataTable dt = controladoraCuentas.consultarPorNombreContr(txtU.Text, txtC.Text); // obtengo de la base de datos la fila correspondiente a la tupla seleccionada del grid
                if(dt.Rows.Count==1){
                   foreach(DataRow r in dt.Rows){
                       // lleno los campos de texto con los datos de la tupla seleccionada
                       this.txtUsuario.Text = commonService.procesarStringDeUI(r[0].ToString());
                       this.txtFechaAux.Text = r[2].ToString();
                       this.cntUsuario.Text = commonService.procesarStringDeUI(r[1].ToString());
                       this.cofCntUsuario.Text = commonService.procesarStringDeUI(r[1].ToString());
                       this.txtNombrePersona.Text = controladoraCuentas.retornarNombreCuentaPorCedula(commonService.procesarStringDeUI(r[3].ToString()));
                       DataTable dtPerfil = controladoraCuentas.consultarPorNombreCuenta(this.txtUsuario.Text);
                       // cargo el valor seleccionado en el dropdown de perfiles
                       if (dtPerfil.Rows.Count==1 && this.drpDownPerfiles.Items.FindByText(commonService.procesarStringDeUI(dtPerfil.Rows[0][1].ToString())) != null)
                       {
                           ListItem aux = this.drpDownPerfiles.Items.FindByText(commonService.procesarStringDeUI(dtPerfil.Rows[0][1].ToString()));
                           this.drpDownPerfiles.SelectedValue = aux.Value;
                           if (lsTipoCuentasDrp[Convert.ToInt32(aux.Value)] == 1 || lsTipoCuentasDrp[Convert.ToInt32(aux.Value)] == 2)
                           {
                               controlarCedula(true); // si es becario o encargado activo el campo del nombre
                           }
                           else {
                               controlarCedula(false); // si es administrador desactivo el campo del nombre
                           }
                       }
                   }
                   mostrarBotonesPrincipales(true); //muestro los botones de control
                   controlarCampos(false); // desactivo los campos de texto
                   modo = 0; // desactivo el modo
                   this.drpPersona.Visible = false;
                   commonService.abrirPopUp("PopUp", "Consultar cuenta"); // activo el pop up con los datos previamente cargados
                   commonService.esconderPrimerBotonDePopUp("PopUp");
                }
            }break;
        }
    }

    /* Efectúa: Se encarga de llenar el grid de cuentas con los datos que se encuentren en la base de datos.
     * Requiere: N/A
     * Modifica: El grid de cuentas cuando lo llena.
     * */
    protected void llenarGridCuentas() {
        // traigo de la base todas las cuentas creadas
        DataTable dt = controladoraCuentas.consultarCuentas();
        DataTable aux = crearTablaCuentas(); // creo la tabla de cuentas
        if (dt.Rows.Count > 0)
        {

        }
        else { // en caso de no existir tuplas lleno con valores por defecto
            Object[] datos = new Object[3];
            datos[0] = "-";
            datos[1] = "-";
            datos[2] = "-";
        }
        this.GridViewCuentas.DataSource = dt; // lleno el grid con los valores de la vase
        this.GridViewCuentas.DataBind();
        this.GridViewCuentas.HeaderRow.BackColor = System.Drawing.Color.FromArgb(4562432); // headers correctos
        this.GridViewCuentas.HeaderRow.ForeColor = System.Drawing.Color.White;        

    }

    /* Efectúa: Se encarga de crear los encabezados de la tabla de cuentas.
     * Requiere: N/A
     * Modifica: N/A
     * */
    protected DataTable crearTablaCuentas() {
        DataTable retorno = new DataTable();
        DataColumn columna;
        // creo la tabla de cuentas con las columnas correspondientes
        columna = new DataColumn();
        columna.DataType = System.Type.GetType("System.String");
        columna.ColumnName = "Nombre";
        retorno.Columns.Add(columna);

        columna = new DataColumn();
        columna.DataType = System.Type.GetType("System.String");
        columna.ColumnName = "Contraseña";
        retorno.Columns.Add(columna);

        columna = new DataColumn();
        columna.DataType = System.Type.GetType("System.String");
        columna.ColumnName = "UltimoAcceso";
        retorno.Columns.Add(columna);

        return retorno;
    }

    /* Efectúa: Se encarga de buscar en la base de datos todas las cuentas que coincidan con el valor insertado en el campo de texto de buscar.
     * Además llena el grid de cuentas con las tuplas encontradas. En caso de no encontrar coincidencias, muestra un mensaje.
     * Requiere: N/A
     * Modifica: El grid de cuentas cuando lo llena.
     * */
    protected void btnBuscar_Click(object sender, EventArgs e)
    {
        // lleno la tabla con el resultado insertado en la caja de texto de buscar
        DataTable dt = controladoraCuentas.consultarPorBusqueda(this.txtBuscarCuenta.Text);
        DataTable aux = crearTablaCuentas(); // creo la tabla correspondiente a cuentas
        Object [] datos = new Object[3];
        if (dt.Rows.Count > 0) // lleno la tabla con los valores que tienen coincidencia con el valor insertado en la caja de texto de buscar
        {
            foreach (DataRow r in dt.Rows)
            {
                datos[0] = commonService.procesarStringDeUI(r[0].ToString());
                datos[1] = commonService.procesarStringDeUI(r[1].ToString());
                datos[2] = commonService.procesarStringDeUI(r[2].ToString());
                aux.Rows.Add(datos);
            }
            this.GridViewCuentas.DataSource = aux;
            this.GridViewCuentas.DataBind();
        }
        else {
            commonService.mensajeJavascript("Lo sentimos, no encotramos ninguna cuenta asociada a los caracteres digitados","Atención"); // en caso de no existir coincidencias con los valores digitados, muestro un mensaje
        }
    }

    /* Efectúa: Se encarga de llenar los datos personales de la cuenta correspondiente a la sesion activa.
     * Requiere: N/A
     * Modifica: N/A
     * */
    protected void llenarCamposPersonal() {
        DataTable dt = new DataTable();
        dt = controladoraCuentas.obtenerDatosCuenta(Session["Cuenta"].ToString()); // llenar los campos de la sesion actual
        if(dt.Rows.Count==1){
           foreach(DataRow r in dt.Rows){ // lleno las cajas de texto con los valores traidos de la base de datos
               this.txtUsuarioPers.Text = commonService.procesarStringDeUI(r[0].ToString());
               this.txtContrasenaPers.Text = commonService.procesarStringDeUI(r[1].ToString());
               this.txtConfContrasenaPers.Text = commonService.procesarStringDeUI(r[1].ToString());
               this.txtCedulaPers.Text = commonService.procesarStringDeUI(r[3].ToString());
               DataTable dtPerfil = controladoraCuentas.consultarPorNombreCuenta(this.txtUsuarioPers.Text);
               if (dtPerfil.Rows.Count == 1)
               {
                   this.txtPerfil.Text = commonService.procesarStringDeUI(dtPerfil.Rows[0][1].ToString());
                   Object dtTipo = controladoraPerfiles.tipoPerfil(txtPerfil.Text);
                   if(dtTipo!=null){
                       int tipoPerfil = Convert.ToInt32(dtTipo);
                       if (tipoPerfil == 1 || tipoPerfil == 2)
                       {
                           controlarCedulaPersonal(true); // en caso de ser becario o encargado activo el nombre
                       }
                       else
                       {
                           controlarCedulaPersonal(false); // en caso de ser administrador no activo el nombre
                       }
                   }
               }
           }
        }
         
    }

    /* Efectúa: Se encarga de guardar los datos de la cuenta activa y su asociación con el perfil para su posterior modificación. 
     * Además activa los campos para que se puedan modificar (contrasenna solamente) y habilita los botones de aceptar y cancelar.
     * Requiere: N/A
     * Modifica: N/A
     * */
    protected void btnModificarCuentaPers_Click(object sender, EventArgs e)
    {
        // guardo los datos antes de modificar la cuenta
        datosOriginales[0] = this.txtUsuarioPers.Text;
        datosOriginales[1] = this.txtContrasenaPers.Text;
        datosOriginales[2] = Session["UltimoAcceso"];
        datosOriginales[3] = cb.obtieneCedulaDeUsuario(this.txtUsuarioPers.Text);
        datosOriginalesAsociacion[0] = this.txtUsuarioPers.Text;
        datosOriginalesAsociacion[1] = this.txtPerfil.Text;
        controlarCamposPersonales(true); // activo los campos de texto cuando presiona modificar
        controlarACCAPers(true); // controlo los botones aceptar y cancelar
        modo = 2;
    }

    /* Efectúa: Se encarga de modificar la cuenta activa en la base de datos. Guarda los datos de la cuenta y ejecuta la correspondiente accion de 
     * la controladora para modificar la cuenta. Muestra un mensaje de éxito o de error de la operacion.
     * Requiere: N/A
     * Modifica: La cuenta modificada en la base de datos.
     * */
    protected void btnAceptarCuentaPers_Click(object sender, EventArgs e)
    {
        string mensaje = "-1";
        string a = this.txtContrasenaPers.Text;
        Object[] datos = new Object[4];
        if(modo==2){ // cargo los datos actuales
            datos[0] = this.txtUsuarioPers.Text;
            datos[1] = this.txtContrasenaPers.Text;
            datos[2] = Session["UltimoAcceso"];
            datos[3] = this.txtCedulaPers.Text;
            mensaje = controladoraCuentas.ejecutarAsociacion(3, datosOriginalesAsociacion, null); // elimino la asociacion para poder modificar
            if (mensaje == "")
            {
                mensaje = controladoraCuentas.ejecutar(modo, datos, datosOriginales); // modifico la cuenta
                if (mensaje == "")
                {
                    Object[] datosAsoc = new Object[2];
                    datosAsoc[0] = this.txtUsuarioPers.Text;
                    datosAsoc[1] = this.txtPerfil.Text;
                    controladoraCuentas.ejecutarAsociacion(1, datosAsoc, null); // vuelvo a crear la asociacion
                    mensaje = "Se ha modificado correctamente la cuenta";
                }
            }
            commonService.mensajeJavascript(mensaje, "Ateción");
        }
    }

    /* Efectúa: Deshace los cambios realizados.
     * Requiere: N/A
     * Modifica: N/A
     * */
    protected void btnCancelarCuentaPers_Click(object sender, EventArgs e)
    {
        llenarCamposPersonal(); // cargo los campos con los datos de la cuenta activa
        controlarACCAPers(false); // desactivo los campos de aceptar y cancelars
    }

    /* Efectúa: Se encarga de cargar el grid de cuentas en la pagina seleccionada.
     * Requiere: N/A
     * Modifica: El grid cuando carga los datos de la nueva pagina.
     * */
    protected void GridViewCuentas_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        this.GridViewCuentas.PageIndex = e.NewPageIndex;
        this.GridViewCuentas.DataBind();
        llenarGridCuentas(); // lleno de nuevo el grid cuando la pagina del grid cambia, para evitar que desaparezca
    }

    /* Efectúa: Llena el dropdown con el nombre de las personas que no tienen cuentas asociadas. Pueden ser becarios o encargados.
     * Requiere: Que existan datos en la base de datos.
     * Modifica: El dropdown cuando lo llena.
     * */
    protected void llenarDrpPersona(int tipo) { // lleno el drpdown de las personas que no tengan cuenta
        DataTable dt = new DataTable();
        switch(tipo){
            case 0: { 
                dt = controladoraCuentas.devolverBecariosSinCuenta(); // en caso de que tipo valga 0 lleno el drpdown con los becarios sin cuenta
                this.drpPersona.DataSource = dt;
                this.drpPersona.DataValueField = "Cedula";
                this.drpPersona.DataTextField = "nombreCompleto";
                this.drpPersona.DataBind();
            }break;
            case 1:{
                dt = controladoraCuentas.devolverEncargadosSinCuenta(); // en caso de que tipo valga 1 lleno con los encargados sin cuenta
                this.drpPersona.DataSource = dt;
                this.drpPersona.DataValueField = "Cedula";
                this.drpPersona.DataTextField = "nombreCompleto";
                this.drpPersona.DataBind();
            }break;
        }
        if(dt.Rows.Count>0){ // si hay alguna persona sin cuenta, selecciono el primer elemento
            this.drpPersona.SelectedIndex = 0;
            this.txtNombrePersona.Text = this.drpPersona.SelectedItem.Text;
        }
    }

    /* Efectúa: Llena el dropdown con el nombre de las personas que no tienen cuentas asociadas. Pueden ser becarios o encargados. Además agrega
     * el nombre de la persona correspondiente a la cuenta seleccionada, para una posible modificacion de la cuenta.
     * Requiere: Que existan datos en la base de datos.
     * Modifica: El dropdown cuando lo llena.
     * */
    protected void llenarDrpPersonaModificar(int tipo)
    { // lleno el drpdown de las personas que no tengan cuenta
        this.drpPersona.Items.Clear();
        DataTable dt = new DataTable();
        switch (tipo)
        {
            case 0:
                {
                    dt = controladoraCuentas.devolverBecariosSinCuenta(); // en caso de que tipo valga 0 lleno el drpdown con los becarios sin cuenta
                    ListItem actual = new ListItem(this.txtNombrePersona.Text, datosOriginales[3].ToString()); // Nombre y cedula de la persona seleccionada
                    this.drpPersona.Items.Add(actual); 
                    foreach(DataRow r in dt.Rows){
                        ListItem item = new ListItem(commonService.procesarStringDeUI(r["nombreCompleto"].ToString()), commonService.procesarStringDeUI(r["Cedula"].ToString()));
                        this.drpPersona.Items.Add(item); 
                    }
                    this.drpPersona.DataBind();
                } break;
            case 1:
                {
                    dt = controladoraCuentas.devolverEncargadosSinCuenta(); // en caso de que tipo valga 1 lleno con los encargados sin cuenta
                    ListItem actual = new ListItem(this.txtNombrePersona.Text, datosOriginales[3].ToString());
                    this.drpPersona.Items.Add(actual); 
                    foreach (DataRow r in dt.Rows)
                    {
                        ListItem item = new ListItem(commonService.procesarStringDeUI(r["nombreCompleto"].ToString()), commonService.procesarStringDeUI(r["Cedula"].ToString()));
                        this.drpPersona.Items.Add(item);
                    }
                    this.drpPersona.DataBind();
                } break;
        }
        if (dt.Rows.Count > 0)
        { // si hay alguna persona sin cuenta, selecciono el primer elemento
            this.drpPersona.SelectedIndex = 0;
            this.txtNombrePersona.Text = this.drpPersona.SelectedItem.Text;
        }
    }

}
