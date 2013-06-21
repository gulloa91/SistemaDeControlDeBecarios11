using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Globalization;

public partial class Asignaciones : System.Web.UI.Page
{

    /*Estados de la Asignación :
     * 
     * 1 : Aceptada
     * 2 : Pendiente de confirmación
     * 3 : Esperando confirmación de becario
     * 4 : Esperando confirmación de encargado
     * 5 : Rechazada por el becario.
     * 6 : Rechazada por el encargado.
     * 7 :  Finalizada
     * */

    private static CommonServices commonService;
    private static List<Asignacion> listaAsignaciones = new List<Asignacion>();
    private static ControladoraAsignaciones controladoraAsignaciones =  new ControladoraAsignaciones();
    private static Object[] datosViejos;

    private static int añoActual;
    private static int periodoActual;
    static int modoEjecucion = 0;
    static int rowIndex;

    protected void Page_Load(object sender, EventArgs e)
    {

        determinaSemestreActual();
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
            int permiso = 0; 
            if (permisos.Contains(8))
            {
                permiso = 8;
            }
            else
            {
                if (permisos.Contains(9))
                {
                    permiso = 9;
                }
                else
                {
                    if (permisos.Contains(10))
                    {
                        permiso = 10;
                    }
                }
            }

            switch (permiso)
            {
                case 8: // Vista Completa
                    {
                        MultiViewEncargado.ActiveViewIndex = 0;
                        if (!IsPostBack)
                        {
                            llenarGridAsignaciones();
                        }
                    } break;

                case 9: // Vista Parcial Encargado
                    {
                        MultiViewEncargado.ActiveViewIndex = 1;
                        if (!IsPostBack)
                        {
                            llenarGridaBecariosAsignadosVistaEncargado();
                            llenarCicloYAnioVistaEncargados();
                        }

                    } break;

                case 10: // Vista Parcial Becario
                    {
                        MultiViewEncargado.ActiveViewIndex = 2;
                        if (!IsPostBack)
                        {
                            llenarInfoVistaBecario();
                        }

                    } break;

                default: // Vista sin permiso
                    {
                        MultiViewEncargado.ActiveViewIndex = 3;
                    } break;
            }
        }
         
    }



   /***************************************************************************
   * 
   *                       VISTA COMPLETA
   * 
   * **************************************************************************/



/*
* ------------------------------
*           CLICKS     
* ------------------------------
*/


    // Abrir PopUp Insertar Asignacion
    protected void btnInsertarAsignacion_Click(object sender, EventArgs e)
    {

        
        mostrarBotonesPrincipales(false);
        habilitarContenidoAsignacion(true);
        limpiarContenidoAsignacion();
        
        cargarDropDownBecarios();
        cargarDropDownEncargados();
        this.btnCantidadBecariosDeEncargado.Text = "Becarios asignados: -- ";
        this.btnCantidadBecariosDeEncargado.Enabled = false;
        
        determinaSemestreActual();

        this.lblCiclo.Text = convertirANumeroRomano(periodoActual); 
        this.lblAnio.Text = añoActual.ToString();

        modoEjecucion = 1;

        commonService.abrirPopUp("PopUpAsignacion", "Insertar Nueva Asignación");
        commonService.mostrarPrimerBotonDePopUp("PopUpAsignacion");


    }


    // Aceptar PopUp
    protected void btnInvisibleAceptarAsignacion_Click(object sender, EventArgs e)
    {

       
        TextInfo miTexto = CultureInfo.CurrentCulture.TextInfo;

        Object[] datos = new Object[10];

        datos[0] = dropDownBecariosPopUp.SelectedValue;
        datos[1] = dropDownEncargadosPopUp.SelectedValue;
        datos[2] = periodoActual;
        datos[3] = añoActual;
        datos[4] = txtTotalHoras.Text;
        datos[5] = txtUnidAcademica.Text;
        datos[6] = txtInfoDeUbicacion.Text;
        
        
        if (modoEjecucion == 1)
        {
          datos[7] = 2; // estado es pendiente cuando se acaba de insetar
        }
        else
        { //es una modificación

            // si la asignación habia sido rechazada por el becario ahora se modificó para asignarle 
           // un nuevo encargado y por lo tanto queda pendiente de confirmación por este
            if (Convert.ToInt32(datosViejos[7]) == 5)
            {
                datos[7] = 3;
            }
            else {
                datos[7] = 4; // si la asignación habia sido rechazada por el encargado (estado 6) ahora se modificó para asignarle 
                              // un nuevo becario y por lo tanto queda pendiente de confirmación por este
            } 
        }


        string mensajeResultado = controladoraAsignaciones.ejecutar(modoEjecucion, datos, null);
       
        commonService.cerrarPopUp("PopUpAsignacion");

        if (mensajeResultado.Equals("Exito"))
        {
            commonService.mensajeJavascript("Se ha creado correctamente una nueva asignación", "Éxito");
        }
        else {
            commonService.mensajeJavascript("No se pudo crear la inserción", "Error");        
        }

        llenarGridAsignaciones();

    }


    // Abrir PopUp Eliminar
    protected void btnEliminarAsignacion_Click(object sender, EventArgs e)
    {
        commonService.abrirPopUp("PopUpEliminarAsignacion", "Eliminar Asignación");
    }



    // Eliminar
    protected void btnInvisibleEliminarAsignacion_Click(object sender, EventArgs e)
    {

        commonService.cerrarPopUp("PopUpAsignacion");
        commonService.mensajeJavascript("La asignación se eliminó correctamente", "Eliminado"); // Obviamente se tiene que cambiar con el resultado de vd
    }


    // Seleccionar Modificar en el PopUp
    protected void btnModificarAsignacion_Click(object sender, EventArgs e)
    {
        

        //solo si la asignación esta rechazada por el becario o el encargado entonces se puede modificar
        if ((listaAsignaciones[rowIndex].Estado == 5) || (listaAsignaciones[rowIndex].Estado == 6))
        {

            habilitarContenidoAsignacion(true);

            // la asignación fue rechazada por el becario entonces solo se habilita el dropdown de becarios
            if (listaAsignaciones[rowIndex].Estado == 5)
            {
                this.dropDownEncargadosPopUp.Enabled = false;
            }
            else {
                this.dropDownBecariosPopUp.Enabled = false;   // la asignación fue rechazada por el encargado entonces solo se habilita el dropdown de encargados
            }


            guardarDatosActuales();
            modoEjecucion = 1;

            commonService.correrJavascript("$('#PopUpAsignacion').dialog('option', 'title', 'Modificar Asignación');");
            commonService.mostrarPrimerBotonDePopUp("PopUpAsignacion");
        }
        else {
            commonService.mensajeJavascript("Solo se puede modificar una asignacón cuando esta ha sido rechazada por el becario o el encargado", "Aviso"); 
        }


    }

    // Seleccionar el de ver becarios asignados a un encargado
    protected void btnCantidadBecariosDeEncargado_Click(object sender, EventArgs e)
    {
        String nombreDeEncargado = "Becarios asignados a: Gabriel Ulloa Murillo"; // Get nombre de encargado
        commonService.abrirPopUp("PopUpVerBecariosAsignados", nombreDeEncargado);
        llenarGridaBecariosAsigandosAEncargado();
    }




    // BUSCAR CLICK
    protected void btnBuscar_Click(object sender, EventArgs e)
    {
       
    }

    

 /*
 * ----------------------------------------
 *   AUXILIARES - MOSTRAR BOTONES Y OTROS
 * ----------------------------------------
 */


    //Define que botones se van a mostrar
    protected void mostrarBotonesPrincipales(Boolean mostrar)
    {
        if (mostrar)
        {
            this.btnModificarAsignacion.Visible = true;
            this.btnEliminarAsignacion.Visible = true;
        }
        else
        {
            this.btnModificarAsignacion.Visible = false;
            this.btnEliminarAsignacion.Visible = false;
        }
    }

    protected void habilitarContenidoAsignacion(Boolean mostrar)
    {
        if (mostrar)
        {
            this.dropDownBecariosPopUp.Enabled = true;
            this.dropDownEncargadosPopUp.Enabled = true;
            this.txtUnidAcademica.Enabled = true;
            this.txtInfoDeUbicacion.Enabled = true;
            this.txtTotalHoras.Enabled = true;
        }
        else
        {
            this.dropDownBecariosPopUp.Enabled = false;
            this.dropDownEncargadosPopUp.Enabled = false;
            this.txtUnidAcademica.Enabled = false;
            this.txtInfoDeUbicacion.Enabled = false;
            this.txtTotalHoras.Enabled = false;
        }
    }

    protected void limpiarContenidoAsignacion()
    {

        this.dropDownBecariosPopUp.Items.Clear();
        this.dropDownEncargadosPopUp.Items.Clear();
        this.txtUnidAcademica.Text = "";
        this.txtInfoDeUbicacion.Text = "";
        this.txtTotalHoras.Text = "";
    }



    protected void guardarDatosActuales() {


        datosViejos = new Object[8];

        datosViejos[0] = dropDownBecariosPopUp.SelectedValue;
        datosViejos[1] = dropDownEncargadosPopUp.SelectedValue;
        datosViejos[2] = periodoActual;
        datosViejos[3] = añoActual;
        datosViejos[4] = txtTotalHoras.Text;
        datosViejos[5] = txtUnidAcademica.Text;
        datosViejos[6] = txtInfoDeUbicacion.Text;
        datosViejos[7] = 2; // estado es pendiente cuando se acaba de insetar*/
    }



    protected string convertirANumeroRomano(int num){

        string retorno = "";

        if (num == 1) {
            retorno = "I";
        }
        if (num == 2) {
            retorno = "II";
        }
        if (num == 3) {
            retorno = "III";
        }


        return retorno;
    }


    /*Estados de la Asignación :
     * 
     * 1 : Aceptada
     * 2 : Pendiente de confirmación
     * 3 : Esperando confirmación de becario
     * 4 : Esperando confirmación de encargado
     * 5 : Rechazada por el becario.
     * 6 : Rechazada por el encargado.
     * 7 : Finalizada
     * */
    protected String interpretaEstado(int estado){

        string respuesta = "";

        switch (estado) {

            case 1:
                {
                    respuesta = "Aceptada";
                }break;
            case 2:
                {
                    respuesta = "Pendiente de confirmación";
                }break;
            case 3:
                {
                    respuesta = "Esperando confirmación de becario";
                }break;
            case 4:
                {
                    respuesta = "Esperando confirmación de encargado";
                } break;
            case 5:
                {
                    respuesta = "Rechazada por el becario";
                } break;
            case 6:
                {
                    respuesta = "Rechazada por encargado";
                } break;
            case 7:
                {
                    respuesta = "Finalizada";
                } break;

            default: //desconocido
                {
                    respuesta = "Desconocido";
                } break;
        
        }

        return respuesta;
    
    }



    //busca en base a la cedula el becario correspondiente es la lista de asignaciones
    private String buscarNombreBecario( string ced) {

        string nombre = "";

       
        return nombre;
    }


  /*
   * ------------------------------
   *       VARIOS
   * ------------------------------
   */


    protected void determinaSemestreActual() {

        DateTime fecha = DateTime.Now;
        añoActual = fecha.Year;
        int mes = fecha.Month;

        periodoActual = -1;

        if ((mes >= 3) && (mes <= 6))
        {
            periodoActual = 1;
        }
        else {

            if ((mes >= 7) && (mes <= 12))
            {
                periodoActual = 2;
            }
            else {
                periodoActual = 3;
            }
        }
    
    }


    // Llenar tabla con todas las asignaciones
    protected void llenarGridAsignaciones()
    {


        listaAsignaciones = controladoraAsignaciones.consultarTablaAsignacionesCompleta();

        DataTable tablaAsignaciones = crearTablaAsignaciones();
        DataRow newRow;

        if (listaAsignaciones.Count > 0)
        {
            for (int i = 0; i < listaAsignaciones.Count; ++i)
            {

                newRow = tablaAsignaciones.NewRow();
                newRow["Encargado"] = controladoraAsignaciones.getNombreEncargado(listaAsignaciones[i].CedulaEncargado);
                newRow["Becario"] = controladoraAsignaciones.getNombreBecario(listaAsignaciones[i].CedulaBecario);            
                newRow["Ciclo"] =  convertirANumeroRomano(listaAsignaciones[i].Periodo);
                newRow["Año"] = listaAsignaciones[i].Año;
                newRow["Estado"] = interpretaEstado(listaAsignaciones[i].Estado);
                tablaAsignaciones.Rows.InsertAt(newRow, i);
            }
        }
        else
        {

            newRow = tablaAsignaciones.NewRow();
            newRow["Encargado"] = "-";
            newRow["Becario"] = "-";
            newRow["Ciclo"] = "-";
            newRow["Año"] = "-";
            newRow["Estado"] = "-";
            tablaAsignaciones.Rows.InsertAt(newRow, 0);
       }
    

        this.GridAsignaciones.DataSource = tablaAsignaciones;
        this.GridAsignaciones.DataBind();
        this.HeadersCorrectosAsignaciones();
    }



    protected DataTable crearTablaAsignaciones()
    {

        DataTable dt = new DataTable();
        DataColumn column;

        column = new DataColumn();
        column.DataType = System.Type.GetType("System.String");
        column.ColumnName = "Encargado";
        dt.Columns.Add(column);

        column = new DataColumn();
        column.DataType = System.Type.GetType("System.String");
        column.ColumnName = "Becario";
        dt.Columns.Add(column);

        column = new DataColumn();
        column.DataType = System.Type.GetType("System.String");
        column.ColumnName = "Ciclo";
        dt.Columns.Add(column);

        column = new DataColumn();
        column.DataType = System.Type.GetType("System.String");
        column.ColumnName = "Año";
        dt.Columns.Add(column);

        column = new DataColumn();
        column.DataType = System.Type.GetType("System.String");
        column.ColumnName = "Estado";
        dt.Columns.Add(column);

        return dt;
    }


    //carga el dropdown con los becarios que estan pendientes de asignacion en el ciclo lectivo actual
    protected void cargarDropDownBecarios() {


        //int periodo = 1;
        //int año = 2013;  

        AsignacionesDataSet.BecarioSinAsignacionDataTable tablaBecarios = controladoraAsignaciones.consultaBecariosSinAsignacion(periodoActual, añoActual);
        ListItem item;


        this.dropDownBecariosPopUp.SelectedIndex = -1;
        this.dropDownBecariosPopUp.SelectedValue = null;

        item = new ListItem("Seleccionar un becario", "0");
        this.dropDownBecariosPopUp.Items.Add(item);

        foreach(DataRow r in tablaBecarios.Rows){
         
           string nombre = commonService.procesarStringDeUI(r["Nombre"].ToString()) + " " + commonService.procesarStringDeUI(r["Apellido1"].ToString()) + " " + commonService.procesarStringDeUI(r["Apellido2"].ToString());
           item = new ListItem(nombre, commonService.procesarStringDeUI(r["Cedula"].ToString()));  
            this.dropDownBecariosPopUp.Items.Add(item);
        }

        this.dropDownBecariosPopUp.DataBind();
       
    }


    //carga el dropdown de encargados con todos los encargados existentes
    protected void cargarDropDownEncargados()
    {


        EncargadoDataSet.EncargadoDataTable tablaEncargados = controladoraAsignaciones.obtenerEncargadosCompletos();
        ListItem item;

        this.dropDownEncargadosPopUp.SelectedIndex = -1;
        this.dropDownEncargadosPopUp.SelectedValue = null;

        item = new ListItem("Seleccionar un encargado", "0");
        this.dropDownEncargadosPopUp.Items.Add(item);

        foreach (DataRow r in tablaEncargados.Rows)
        {

            string nombre = commonService.procesarStringDeUI(r["Nombre"].ToString()) + " " + commonService.procesarStringDeUI(r["Apellido1"].ToString()) + " " + commonService.procesarStringDeUI(r["Apellido2"].ToString());
            item = new ListItem(nombre, commonService.procesarStringDeUI(r["Cedula"].ToString()));
            this.dropDownEncargadosPopUp.Items.Add(item);
        }

        this.dropDownEncargadosPopUp.DataBind();
        

    }


    //metodo q actualiza el botón de cantidad de becarios cuando se selecciona un encargado
    protected void seleccionaEncargado(object sender, EventArgs e)
    {

        string cedEncargadoSeleccionado = dropDownEncargadosPopUp.SelectedValue;

        //int periodo = 1; 
        //int año = 2013; 

        int cantidadBecariosAsignados = controladoraAsignaciones.contarBecariosAsignados(cedEncargadoSeleccionado, añoActual, periodoActual);

        this.btnCantidadBecariosDeEncargado.Text = "Becarios asignados : " + cantidadBecariosAsignados;
        this.btnCantidadBecariosDeEncargado.Enabled = true;


    }


    // Seleccionar tupla del grid con la flecha
    protected void GridAsignaciones_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        switch (e.CommandName)
        {
            // Consultar tupla
            case "btnSeleccionarTupla_Click":
                {

                    rowIndex = Convert.ToInt32(e.CommandArgument);
                    int pageIndex = this.GridAsignaciones.PageIndex;
                    int pageSize = this.GridAsignaciones.PageSize;
                    rowIndex += (pageIndex * pageSize);

                    limpiarContenidoAsignacion();
                    cargarCamposAsignacion();

                    
                    mostrarBotonesPrincipales(true);
                    habilitarContenidoAsignacion(false);

                    commonService.abrirPopUp("PopUpAsignacion", "Consultar Asignación");
                    commonService.esconderPrimerBotonDePopUp("PopUpAsignacion");               
                   
                } break;
        }
    }



    protected void cargarCamposAsignacion(){


        this.lblCiclo.Text = convertirANumeroRomano( listaAsignaciones[rowIndex].Periodo);
        this.lblAnio.Text = listaAsignaciones[rowIndex].Año.ToString();  

        this.txtTotalHoras.Text = listaAsignaciones[rowIndex].TotalHoras.ToString();
        this.txtUnidAcademica.Text = commonService.procesarStringDeUI(listaAsignaciones[rowIndex].SiglasUA);
        this.txtInfoDeUbicacion.Text = commonService.procesarStringDeUI(listaAsignaciones[rowIndex].InfoUbicacion);

        cargarDropDownEncargados();
        string cedEncargado = listaAsignaciones[rowIndex].CedulaEncargado;
        dropDownEncargadosPopUp.SelectedValue = cedEncargado;

        cargarDropDownBecarios();

        string cedBecario = listaAsignaciones[rowIndex].CedulaBecario;
        string becarioSeleccionado = controladoraAsignaciones.buscarNombreBecario(cedBecario);

        ListItem item = new ListItem(becarioSeleccionado, cedBecario);
        this.dropDownBecariosPopUp.Items.Add(item);
        this.dropDownBecariosPopUp.DataBind();
       
        dropDownBecariosPopUp.SelectedValue = cedBecario;
       
        
        int cantidadBecariosAsignados = controladoraAsignaciones.contarBecariosAsignados(cedEncargado, añoActual, periodoActual);

        this.btnCantidadBecariosDeEncargado.Text = "Becarios asignados : " + cantidadBecariosAsignados;
        this.btnCantidadBecariosDeEncargado.Enabled = false;


    }


    ///////******//////


    // Grid vista Encargados
    protected void GridBecariosAsignadosVistaEncargado_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        switch (e.CommandName)
        {
            // Abrir Pop Up aceptar/rechazar asignación vista encargado
            case "btnSeleccionarTupla_Click":
                {
                    commonService.abrirPopUp("PopUpAsignacionEncargado", "Aceptar/Rechazar Asignación");
                    this.lblNombreBecarioPopUpVistaEncargado.Text = "José Perez";
                    this.lblCicloBecarioPopUpVistaEncargado.Text = "I";
                    this.lblAnioBecarioPopUpVistaEncargado.Text = "2013";
                    this.lblHorasBecarioPopUpVistaEncargado.Text = "73";
                } break;
        }
    }



    protected void llenarCicloYAnioVistaEncargados()
    {
        this.lblCicloPrincipalVistaEncargado.Text = "I";
        this.lblAnioPrincipalVistaEncargado.Text = "2013";
    }

   
    // Llenar tabla con todas las asignaciones
    protected void llenarGridaBecariosAsigandosAEncargado()
    {

        DataTable tablaBecariosAsigandosAEncargado = crearTablaBecariosAsigandosAEncargado();
        DataRow newRow;
        /*
        if (lsEncargados.Count > 0)
        {
            for (int i = 0; i < lsEncargados.Count; ++i)
            {
                newRow = tablaAsignaciones.NewRow();
                newRow["Nombre"] = lsEncargados[i].Nombre + " " + lsEncargados[i].Apellido1 + " " + lsEncargados[i].Apellido2;
                newRow["Cedula"] = lsEncargados[i].Cedula;
                newRow["Correo"] = lsEncargados[i].Correo;
                newRow["Celular"] = lsEncargados[i].TelefonoCelular;
                if (lsEncargados[i].TelefonoFijo != "")
                {
                    newRow["Telefono"] = lsEncargados[i].TelefonoFijo;
                }
                else
                {
                    if (lsEncargados[i].OtroTelefono != "")
                    {
                        newRow["Telefono"] = lsEncargados[i].OtroTelefono;
                    }
                }

                tablaAsignaciones.Rows.InsertAt(newRow, i);
            }
        }
        else
        {
         */
        newRow = tablaBecariosAsigandosAEncargado.NewRow();
        newRow["Nombre"] = "-";
        newRow["Carné"] = "-";
        newRow["Correo"] = "-";
        newRow["Celular"] = "-";

        tablaBecariosAsigandosAEncargado.Rows.InsertAt(newRow, 0);

        //}
        GridBecariosAsignadosAEncargado.DataSource = tablaBecariosAsigandosAEncargado;
        GridBecariosAsignadosAEncargado.DataBind();
        this.HeadersCorrectosBecariosAsigandosAEncargado();
    }

    // Llenar tabla con todas las asignaciones
    protected void llenarGridaBecariosAsignadosVistaEncargado()
    {

        DataTable tablaBecariosAsignadosVistaEncargado = crearTablaBecariosAsignadosVistaEncargado();
        DataRow newRow;
        /*
        if (lsEncargados.Count > 0)
        {
            for (int i = 0; i < lsEncargados.Count; ++i)
            {
                newRow = tablaAsignaciones.NewRow();
                newRow["Nombre"] = lsEncargados[i].Nombre + " " + lsEncargados[i].Apellido1 + " " + lsEncargados[i].Apellido2;
                newRow["Cedula"] = lsEncargados[i].Cedula;
                newRow["Correo"] = lsEncargados[i].Correo;
                newRow["Celular"] = lsEncargados[i].TelefonoCelular;
                if (lsEncargados[i].TelefonoFijo != "")
                {
                    newRow["Telefono"] = lsEncargados[i].TelefonoFijo;
                }
                else
                {
                    if (lsEncargados[i].OtroTelefono != "")
                    {
                        newRow["Telefono"] = lsEncargados[i].OtroTelefono;
                    }
                }

                tablaAsignaciones.Rows.InsertAt(newRow, i);
            }
        }
        else
        {
         */
        newRow = tablaBecariosAsignadosVistaEncargado.NewRow();
        newRow["Nombre"] = "-";
        newRow["Carné"] = "-";
        newRow["Correo"] = "-";
        newRow["Celular"] = "-";
        newRow["Estado"] = "-";

        tablaBecariosAsignadosVistaEncargado.Rows.InsertAt(newRow, 0);

        //}
        GridBecariosAsignadosVistaEncargado.DataSource = tablaBecariosAsignadosVistaEncargado;
        GridBecariosAsignadosVistaEncargado.DataBind();
        this.HeadersCorrectosBecariosAsignadosVistaEncargado();
    }

    // Le da formato a las columnas del Grid


    // Le da formato a las columnas del Grid
    protected DataTable crearTablaBecariosAsigandosAEncargado()
    {

        DataTable dt = new DataTable();
        DataColumn column;

        column = new DataColumn();
        column.DataType = System.Type.GetType("System.String");
        column.ColumnName = "Nombre";
        dt.Columns.Add(column);

        column = new DataColumn();
        column.DataType = System.Type.GetType("System.String");
        column.ColumnName = "Carné";
        dt.Columns.Add(column);

        column = new DataColumn();
        column.DataType = System.Type.GetType("System.String");
        column.ColumnName = "Correo";
        dt.Columns.Add(column);

        column = new DataColumn();
        column.DataType = System.Type.GetType("System.String");
        column.ColumnName = "Celular";
        dt.Columns.Add(column);

        return dt;
    }

    // Le da formato a las columnas del Grid
    protected DataTable crearTablaBecariosAsignadosVistaEncargado()
    {

        DataTable dt = new DataTable();
        DataColumn column;

        column = new DataColumn();
        column.DataType = System.Type.GetType("System.String");
        column.ColumnName = "Nombre";
        dt.Columns.Add(column);

        column = new DataColumn();
        column.DataType = System.Type.GetType("System.String");
        column.ColumnName = "Carné";
        dt.Columns.Add(column);

        column = new DataColumn();
        column.DataType = System.Type.GetType("System.String");
        column.ColumnName = "Correo";
        dt.Columns.Add(column);

        column = new DataColumn();
        column.DataType = System.Type.GetType("System.String");
        column.ColumnName = "Celular";
        dt.Columns.Add(column);

        column = new DataColumn();
        column.DataType = System.Type.GetType("System.String");
        column.ColumnName = "Estado";
        dt.Columns.Add(column);

        return dt;
    }
    // Aplica nombre a las columnas así como color
    private void HeadersCorrectosAsignaciones()
    {
        this.GridAsignaciones.HeaderRow.BackColor = System.Drawing.Color.FromArgb(4562432);
        this.GridAsignaciones.HeaderRow.ForeColor = System.Drawing.Color.White;
        this.GridAsignaciones.HeaderRow.Cells[1].Text = "Encargado";
        this.GridAsignaciones.HeaderRow.Cells[2].Text = "Becario";
        this.GridAsignaciones.HeaderRow.Cells[3].Text = "Ciclo";
        this.GridAsignaciones.HeaderRow.Cells[4].Text = "Año";
        this.GridAsignaciones.HeaderRow.Cells[5].Text = "Estado";
    }
    
    // Aplica nombre a las columnas así como color
    private void HeadersCorrectosBecariosAsigandosAEncargado()
    {
        GridBecariosAsignadosAEncargado.HeaderRow.BackColor = System.Drawing.Color.FromArgb(4562432);
        GridBecariosAsignadosAEncargado.HeaderRow.ForeColor = System.Drawing.Color.White;
        GridBecariosAsignadosAEncargado.HeaderRow.Cells[0].Text = "Nombre";
        GridBecariosAsignadosAEncargado.HeaderRow.Cells[1].Text = "Carné";
        GridBecariosAsignadosAEncargado.HeaderRow.Cells[2].Text = "Correo";
        GridBecariosAsignadosAEncargado.HeaderRow.Cells[3].Text = "Celular";
    }

    // Aplica nombre a las columnas así como color
    private void HeadersCorrectosBecariosAsignadosVistaEncargado()
    {
        GridBecariosAsignadosVistaEncargado.HeaderRow.BackColor = System.Drawing.Color.FromArgb(4562432);
        GridBecariosAsignadosVistaEncargado.HeaderRow.ForeColor = System.Drawing.Color.White;
        GridBecariosAsignadosVistaEncargado.HeaderRow.Cells[1].Text = "Nombre";
        GridBecariosAsignadosVistaEncargado.HeaderRow.Cells[2].Text = "Carné";
        GridBecariosAsignadosVistaEncargado.HeaderRow.Cells[3].Text = "Correo";
        GridBecariosAsignadosVistaEncargado.HeaderRow.Cells[4].Text = "Celular";
        GridBecariosAsignadosVistaEncargado.HeaderRow.Cells[5].Text = "Estado";
    }




    /***************************************************************************
     * 
     *                       VISTA BECARIO
     * 
     * **************************************************************************/
   
    protected void llenarInfoVistaBecario()
    {
        this.lblAnioVistaBecario.Text = "2013";
        this.lblCicloVistaBecario.Text = "I";
        this.lblEncargadoVistaBecario.Text = "Gabriel Ulloa Murillo";
        this.lblHorasVistaBecario.Text = "73";
    }


    // Aceptar asignación
    protected void btnAceptarAsignacionBecario_Click(object sender, EventArgs e)
    {
        commonService.mensajeJavascript("Usted ha aceptado la asignación satisfactoriamente", "Aceptado");
        esconderBotonesVistaBecario(true);
    }

    // Abrir confirmación de rechazo de asignación
    protected void btnCancelarAsignacionBecario_Click(object sender, EventArgs e)
    {
        commonService.abrirPopUp("PopUpConfirmarRechazoBecario", "Rechazar Asignación");
    }

    // Confirmar rechazo
    protected void btnInvisibleConfirmarRechazo_Click(object sender, EventArgs e)
    {
        commonService.cerrarPopUp("PopUpConfirmarRechazoBecario");
        commonService.mensajeJavascript("¡Su rechazo ha sido procesado satisfactoriamente!","Rechazo procesado");
        esconderBotonesVistaBecario(true);
    }

    protected void esconderBotonesVistaBecario(Boolean esconder)
    {
        if (esconder)
        {
            this.btnAceptarAsignacionBecario.Visible = false;
            this.btnCancelarAsignacionBecario.Visible = false;
        }
        else
        {
            this.btnAceptarAsignacionBecario.Visible = true;
            this.btnCancelarAsignacionBecario.Visible = true;
        }
    }







    /***************************************************************************
     * 
     *                       VISTA ENCARGADO
     * 
     * **************************************************************************/


    // Aceptar Asignación Vista Encargado
    protected void btnInvisibleAceptarAsignacionEncargado_Click(object sender, EventArgs e)
    {
        commonService.cerrarPopUp("PopUpAsignacionEncargado");
        commonService.mensajeJavascript("Se ha aceptado la asignación. Un mensaje se enviará la dirección de la ECCI.", "Aceptada"); // Obviamente se tiene que cambiar con el resultado de vd
    }

    // Rechazar Asignación Vista Encargado
    protected void btnInvisibleRechazarAsignacionEncargado_Click(object sender, EventArgs e)
    {
        commonService.cerrarPopUp("PopUpAsignacionEncargado");
        commonService.mensajeJavascript("Se ha rechazado la asignación. Un mensaje se enviará la dirección de la ECCI.", "Rechazada"); // Obviamente se tiene que cambiar con el resultado de vd
    }






}





