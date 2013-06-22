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
     * 7 : Finalizada
     * */

    private static CommonServices commonService;
    private static List<Asignacion> listaAsignaciones = new List<Asignacion>();
    private static List<Object[]> lstBecariosAsignadosEncargado = new List<Object[]>();

    //variable usada solo en la vista de becario para guardar la información de la asignación del becario actual
    private static List<Object[]> datosDeAsignacionDeBecario = new List<Object[]>();
    
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
                        consultaDatosDeAsignacion();
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
*     CLICKS DE BOTONES
* ------------------------------
*/


    // Click del botón Insertar Asignacion
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


    //Click del botón aceptar del popUp
    protected void btnInvisibleAceptarAsignacion_Click(object sender, EventArgs e)
    {

       
        TextInfo miTexto = CultureInfo.CurrentCulture.TextInfo;

        Object[] datos = new Object[9];

        datos[0] = dropDownBecariosPopUp.SelectedValue;
        datos[1] = dropDownEncargadosPopUp.SelectedValue;
        datos[2] = periodoActual;
        datos[3] = añoActual;
        datos[4] = txtTotalHoras.Text;
        datos[5] = txtUnidAcademica.Text;
        datos[6] = txtInfoDeUbicacion.Text;
        datos[7] = 2;
        datos[8] = true;
        
        if (modoEjecucion == 2) // es una modificación
        {
 
            //cuando se modifica una asignación en realidad se debe crear una nueva ya que la asignacion actual se debe mantener almacenada
            //para posibles consultas históricas. Sin embargo, a nivel de BD se realiza la distinción usando el atributo "Activo"
            controladoraAsignaciones.dejarAsignacionInactiva(datosViejos[0].ToString(), datosViejos[1].ToString(), añoActual, periodoActual);
        }
        
        string mensajeResultado = controladoraAsignaciones.ejecutar(1, datos, null);
       
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


    // Click del botón eliminar asignación
    protected void btnEliminarAsignacion_Click(object sender, EventArgs e)
    {

        datosViejos = new Object[9];

        datosViejos[0] = dropDownBecariosPopUp.SelectedValue;
        datosViejos[1] = dropDownEncargadosPopUp.SelectedValue;
        datosViejos[2] = periodoActual;
        datosViejos[3] = añoActual;
        datosViejos[4] = txtTotalHoras.Text;
        datosViejos[5] = txtUnidAcademica.Text;
        datosViejos[6] = txtInfoDeUbicacion.Text;
        datosViejos[7] = listaAsignaciones[rowIndex].Estado;
        datosViejos[8] = listaAsignaciones[rowIndex].Activo;
        commonService.abrirPopUp("PopUpEliminarAsignacion", "Eliminar Asignación");
    }



    // Click de confirmación de la eliminación ( botón invisible)
    protected void btnInvisibleEliminarAsignacion_Click(object sender, EventArgs e)
    {

        string resultado = controladoraAsignaciones.ejecutar(2, datosViejos, null);
        commonService.cerrarPopUp("PopUpAsignacion");

        if (resultado.Equals("Exito"))
        {
            commonService.mensajeJavascript("La asignación se eliminó correctamente", "Eliminado"); // Obviamente se tiene que cambiar con el resultado de vd
        }
        else {
            commonService.mensajeJavascript("Se ha producido un error en la eliminación", "Error"); // Obviamente se tiene que cambiar con el resultado de vd
        }
        llenarGridAsignaciones();
    }



    // Click del botón modificar asignación
    protected void btnModificarAsignacion_Click(object sender, EventArgs e)
    {

        if (listaAsignaciones[rowIndex].Activo == true)
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
                else
                {
                    this.dropDownBecariosPopUp.Enabled = false;   // la asignación fue rechazada por el encargado entonces solo se habilita el dropdown de encargados
                }

                datosViejos = new Object[3];
                datosViejos[0] = dropDownBecariosPopUp.SelectedValue;
                datosViejos[1] = dropDownEncargadosPopUp.SelectedValue;
                datosViejos[2] = listaAsignaciones[rowIndex].Estado;

                modoEjecucion = 2;

                commonService.correrJavascript("$('#PopUpAsignacion').dialog('option', 'title', 'Modificar Asignación');");
                commonService.mostrarPrimerBotonDePopUp("PopUpAsignacion");
            }
            else
            {
                commonService.mensajeJavascript("Solo se puede modificar una asignacón cuando esta ha sido rechazada por el becario o el encargado", "Aviso");
            }
        }
        else {
          commonService.mensajeJavascript("La asignación que quiere modificar ya fue reemplazada por otra por lo tanto no se puede modificar", "Aviso");
        }

        mostrarBotonesPrincipales(false);
    }

    // Click del botón para ver los becarios asignados a determinado encargado
    protected void btnCantidadBecariosDeEncargado_Click(object sender, EventArgs e)
    {
        llenarGridBecariosAsigandosAEncargado();
        String nombreDeEncargado = "Becarios asignados a: " + controladoraAsignaciones.getNombreEncargado(dropDownEncargadosPopUp.SelectedValue) ;  // Get nombre de encargado
        commonService.abrirPopUp("PopUpVerBecariosAsignados", nombreDeEncargado);        
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


    // Llenar tabla con todas las asignaciones existentes
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


    // Seleccionar tupla del grid de asignaciones con la flecha
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



    //Carga los campos correspondientes de una asignación seleccionada por el usuario
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
        string becarioSeleccionado = controladoraAsignaciones.getNombreBecario(cedBecario);

        ListItem item = new ListItem(becarioSeleccionado, cedBecario);
        this.dropDownBecariosPopUp.Items.Add(item);
        this.dropDownBecariosPopUp.DataBind();
       
        dropDownBecariosPopUp.SelectedValue = cedBecario;
       
        
        int cantidadBecariosAsignados = controladoraAsignaciones.contarBecariosAsignados(cedEncargado, añoActual, periodoActual);

        this.btnCantidadBecariosDeEncargado.Text = "Becarios asignados : " + cantidadBecariosAsignados;
        this.btnCantidadBecariosDeEncargado.Enabled = false;


    }



    // Llenar el grid que muestra los becarios asignados actualmente a determinado encargado
    protected void llenarGridBecariosAsigandosAEncargado()
    {

        string cedEncargado = dropDownEncargadosPopUp.SelectedValue;
        List<Object[]> listaBecarios = controladoraAsignaciones.consultarBecariosAsignadosAEncargado(cedEncargado, añoActual, periodoActual);

        

        DataTable tablaBecariosAsigandosAEncargado = crearTablaBecariosAsigandosAEncargado();
        DataRow newRow;

        if (listaBecarios.Count > 0)
        {
            for (int i = 0; i < listaBecarios.Count; ++i)
            {
                newRow = tablaBecariosAsigandosAEncargado.NewRow();
                newRow["Nombre"] = listaBecarios[i][0].ToString() + " " + listaBecarios[i][1].ToString() + " " + listaBecarios[i][2].ToString();
                newRow["Carné"] = listaBecarios[i][3].ToString();
                newRow["Correo"] = listaBecarios[i][4].ToString();
                newRow["Celular"] = listaBecarios[i][5].ToString();
                tablaBecariosAsigandosAEncargado.Rows.InsertAt(newRow, i);
            }
        }
        else
        {
         
            newRow = tablaBecariosAsigandosAEncargado.NewRow();
            newRow["Nombre"] = "-";
            newRow["Carné"] = "-";
            newRow["Correo"] = "-";
            newRow["Celular"] = "-";

            tablaBecariosAsigandosAEncargado.Rows.InsertAt(newRow, 0);

        }

       
        gridBecariosAsignadosAEncargado.DataSource = tablaBecariosAsigandosAEncargado;
        gridBecariosAsignadosAEncargado.DataBind();
        this.HeadersCorrectosBecariosAsigandosAEncargado();
    }



    // Le da formato a las columnas del grid que muestra los becarios asignados actualmente a determinado encargado
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
        gridBecariosAsignadosAEncargado.HeaderRow.BackColor = System.Drawing.Color.FromArgb(4562432);
        gridBecariosAsignadosAEncargado.HeaderRow.ForeColor = System.Drawing.Color.White;
        gridBecariosAsignadosAEncargado.HeaderRow.Cells[0].Text = "Nombre";
        gridBecariosAsignadosAEncargado.HeaderRow.Cells[1].Text = "Carné";
        gridBecariosAsignadosAEncargado.HeaderRow.Cells[2].Text = "Correo";
        gridBecariosAsignadosAEncargado.HeaderRow.Cells[3].Text = "Celular";
    }




    /***************************************************************************
     * 
     *                       VISTA BECARIO
     * 
     * **************************************************************************/


    //consulta en la BD los datos que se ocupan sobre la asignación del becario logueado actualmente
    protected void consultaDatosDeAsignacion() {

       string usuario = Session["Cuenta"].ToString();
       string cedBecario = controladoraAsignaciones.obtieneCedulaDeUsuario(usuario);
       datosDeAsignacionDeBecario = controladoraAsignaciones.consultarAsignacionDeBecario(cedBecario,añoActual,periodoActual);    
    }


    protected void actualizarEstadoAsignacionVistaBecario(int estadoAsignacion)
    {
       
        string mensajeAMostrar = "";

        switch (estadoAsignacion) {

            case 4: 
                {
                   mensajeAMostrar = "La asignación aún no esta completa ya que falta la confirmación del encargado.";          
                }break;
            case 5:
                {
                    mensajeAMostrar = "Usted ha rechazado esta asignación. Su decisión fue notificada a dirección y pronto se le asignará un nuevo encargado";  
                }break;
            case 6:
                {
                   mensajeAMostrar =  "El encargado ha rechazado esta asignación. Debe esperar a que la dirección le asigne un nuevo encargado";
                }break;
        
        }

        if (!(mensajeAMostrar.Equals(""))) {
            lblEstadoAsignacionVistaBecario.Text = mensajeAMostrar;
            lblEstadoAsignacionVistaBecario.Visible = true;
        }


    }

    protected void llenarInfoVistaBecario()
    {

        this.lblAnioVistaBecario.Text = añoActual.ToString();
        this.lblCicloVistaBecario.Text = convertirANumeroRomano(periodoActual);

        if (datosDeAsignacionDeBecario.Count != 0)
        {

            this.lblEncargadoVistaBecario.Text = datosDeAsignacionDeBecario[0][0].ToString() + " " + datosDeAsignacionDeBecario[0][1].ToString() + " " + datosDeAsignacionDeBecario[0][2].ToString();
            this.lblHorasVistaBecario.Text = datosDeAsignacionDeBecario[0][4].ToString();
           
            int estadoAsignacion = Convert.ToInt32(datosDeAsignacionDeBecario[0][3]);

            if (estadoAsignacion != 2 && estadoAsignacion != 3)
            {
                esconderBotonesVistaBecario(true);
            }

            actualizarEstadoAsignacionVistaBecario(estadoAsignacion);
        }
        else {

            this.lblEncargadoVistaBecario.Visible=false;
            this.lblHorasVistaBecario.Visible = false;
            this.lblTituloEncargadoVistaBecario.Visible = false;
            this.lblTituloHorasVistaBecario.Visible = false;

            lblEstadoAsignacionVistaBecario.Text = "Usted aún no tiene ningún encargado asignado.";
            lblEstadoAsignacionVistaBecario.Visible = true;
            esconderBotonesVistaBecario(true);
        }

    }


   // Aceptar asignación
   protected void btnAceptarAsignacionBecario_Click(object sender, EventArgs e)
   {


       int estadoActual = Convert.ToInt32(datosDeAsignacionDeBecario[0][3]);
       int nuevoEstado;
       if (estadoActual == 2)
       {
           nuevoEstado = 4;
       }
       else
       {
           nuevoEstado = 1;
       }

       string mensajeResultado = controladoraAsignaciones.actualizarEstadoDeAsignacion(nuevoEstado, datosDeAsignacionDeBecario[0][5].ToString(), datosDeAsignacionDeBecario[0][6].ToString() , periodoActual, añoActual);

       if (mensajeResultado.Equals("Exito"))
       {
           if (nuevoEstado == 1)
           {
               commonService.mensajeJavascript("Usted ha aceptado la asignación satisfactoriamente.La asignación ha quedado completa por lo que ya puede empezar a realizar sus horas ", "Aviso");
               
           }
           else {
               commonService.mensajeJavascript("Usted ha aceptado la asignación satisfactoriamente. Sin embargo la asignación no ha quedado completada ya que debe esperar la confirmación del encargado", "Aviso");
           }

       }
       else
       {
           commonService.mensajeJavascript("No se pudo completar la operación. Debe intentarlo de nuevo", "Error");
       }

       actualizarEstadoAsignacionVistaBecario(nuevoEstado);
       esconderBotonesVistaBecario(true);
   }

   // Abrir confirmación de rechazo de asignación
   protected void btnCancelarAsignacionBecario_Click(object sender, EventArgs e)
   {
       commonService.abrirPopUp("PopUpConfirmarRechazoBecario", "Rechazar Asignación");
   }


   // Click de rechazar asignación para el becario
   protected void btnInvisibleConfirmarRechazo_Click(object sender, EventArgs e)
   {
       commonService.cerrarPopUp("PopUpConfirmarRechazoBecario");

       int nuevoEstado = 5; //rechazado por becario

       string mensajeResultado = controladoraAsignaciones.actualizarEstadoDeAsignacion(nuevoEstado, datosDeAsignacionDeBecario[0][5].ToString(), datosDeAsignacionDeBecario[0][6].ToString(), periodoActual, añoActual);

       if (mensajeResultado.Equals("Exito"))
       {
          commonService.mensajeJavascript("¡Su rechazo ha sido procesado satisfactoriamente!", "Rechazo procesado");
       }
       else
       {
           commonService.mensajeJavascript("No se pudo completar la operación. Debe intentarlo de nuevo", "Error");
       }

       actualizarEstadoAsignacionVistaBecario(nuevoEstado);
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


     /*
     * ------------------------------
     *   CLICKS - BOTONES
     * ------------------------------
     */


    // Aceptar Asignación - Vista Encargado
    protected void btnInvisibleAceptarAsignacionEncargado_Click(object sender, EventArgs e)
    {

        string usuario = Session["Cuenta"].ToString();
        string cedEncargado = controladoraAsignaciones.obtieneCedulaDeUsuario(usuario);
        string cedBecario = lstBecariosAsignadosEncargado[rowIndex][8].ToString();

        int estadoActual = Convert.ToInt32(lstBecariosAsignadosEncargado[rowIndex][6]);
        int nuevoEstado;
        if(estadoActual==2){
          nuevoEstado=3;
        }else{
          nuevoEstado=1;        
        }

        string mensajeResultado = controladoraAsignaciones.actualizarEstadoDeAsignacion(nuevoEstado,cedBecario,cedEncargado,periodoActual,añoActual);
       
        if(mensajeResultado.Equals("Exito") ){
           commonService.mensajeJavascript("Se ha aceptado la asignación.", "Aviso"); // Obviamente se tiene que cambiar con el resultado de vd
            //si nuevoEstado es 1 : manda correo a encargado y becario
        }else{
           commonService.mensajeJavascript("La asignación no ha quedado aceptada porque se produjo error. Debe intentarlo de nuevo", "Error");
        }

        commonService.cerrarPopUp("PopUpAsignacionEncargado");
       
        llenarGridaBecariosAsignadosVistaEncargado();
        
    }

    // Rechazar Asignación - Vista Encargado
    protected void btnInvisibleRechazarAsignacionEncargado_Click(object sender, EventArgs e)
    {

        string usuario = Session["Cuenta"].ToString();
        string cedEncargado = controladoraAsignaciones.obtieneCedulaDeUsuario(usuario);
        string cedBecario = lstBecariosAsignadosEncargado[rowIndex][8].ToString();

        int estadoActual = Convert.ToInt32(lstBecariosAsignadosEncargado[rowIndex][6]);
        int nuevoEstado=6;
      
    
        string mensajeResultado = controladoraAsignaciones.actualizarEstadoDeAsignacion(nuevoEstado, cedBecario, cedEncargado, periodoActual, añoActual);

        if (mensajeResultado.Equals("Exito"))
        {
           commonService.mensajeJavascript("Se ha rechazado la asignación.", "Aviso"); 
           //manda correo a dirección
     
        }
        else
        {
            commonService.mensajeJavascript("La asignación no fue rechazada porque se produjo error. Debe intentarlo de nuevo", "Error");
        }

        commonService.cerrarPopUp("PopUpAsignacionEncargado");

        llenarGridaBecariosAsignadosVistaEncargado();

        commonService.cerrarPopUp("PopUpAsignacionEncargado");
        commonService.mensajeJavascript("Se ha rechazado la asignación. Un mensaje se enviará la dirección de la ECCI.", "Rechazada"); // Obviamente se tiene que cambiar con el resultado de vd
    }



    /*
    * ------------------------------
    *   VARIOS
    * ------------------------------
    */


    protected void llenarCicloYAnioVistaEncargados()
    {
        this.lblCicloPrincipalVistaEncargado.Text = convertirANumeroRomano(periodoActual);
        this.lblAnioPrincipalVistaEncargado.Text = añoActual.ToString();
    }


    // Llenar tabla con todas las asignaciones del encargado logueado actualmente
    protected void llenarGridaBecariosAsignadosVistaEncargado()
    {

        string usuario = Session["Cuenta"].ToString();
        string cedEncargado = controladoraAsignaciones.obtieneCedulaDeUsuario(usuario);

        lstBecariosAsignadosEncargado = controladoraAsignaciones.consultarBecariosAsignadosAEncargado(cedEncargado, añoActual, periodoActual);


        DataTable tablaBecariosAsigandosAEncargado = crearTablaBecariosAsignadosVistaEncargado();
        DataRow newRow; 

        if (lstBecariosAsignadosEncargado.Count > 0)
        {
            for (int i = 0; i < lstBecariosAsignadosEncargado.Count; ++i)
            {

                newRow = tablaBecariosAsigandosAEncargado.NewRow();
                newRow["Nombre"] = lstBecariosAsignadosEncargado[i][0].ToString() + " " + lstBecariosAsignadosEncargado[i][1].ToString() + " " + lstBecariosAsignadosEncargado[i][2].ToString();
                newRow["Carné"] = lstBecariosAsignadosEncargado[i][3].ToString();
                newRow["Correo"] = lstBecariosAsignadosEncargado[i][4].ToString();
                newRow["Celular"] = lstBecariosAsignadosEncargado[i][5].ToString();
                newRow["Estado"] =  interpretaEstado( Convert.ToInt32(lstBecariosAsignadosEncargado[i][6]));
                tablaBecariosAsigandosAEncargado.Rows.InsertAt(newRow, i);
            }
        }
        else
        {

            newRow = tablaBecariosAsigandosAEncargado.NewRow();
            newRow["Nombre"] = "-";
            newRow["Carné"] = "-";
            newRow["Correo"] = "-";
            newRow["Celular"] = "-";
            newRow["Estado"] = "-";
            tablaBecariosAsigandosAEncargado.Rows.InsertAt(newRow, 0);

        }

       
        gridBecariosAsignadosVistaEncargado.DataSource = tablaBecariosAsigandosAEncargado;
        gridBecariosAsignadosVistaEncargado.DataBind();
        this.headersCorrectosBecariosAsignadosVistaEncargado();
    }

    // Le da formato a las columnas del Grid




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
    private void headersCorrectosBecariosAsignadosVistaEncargado()
    {
        gridBecariosAsignadosVistaEncargado.HeaderRow.BackColor = System.Drawing.Color.FromArgb(4562432);
        gridBecariosAsignadosVistaEncargado.HeaderRow.ForeColor = System.Drawing.Color.White;
        gridBecariosAsignadosVistaEncargado.HeaderRow.Cells[1].Text = "Nombre";
        gridBecariosAsignadosVistaEncargado.HeaderRow.Cells[2].Text = "Carné";
        gridBecariosAsignadosVistaEncargado.HeaderRow.Cells[3].Text = "Correo";
        gridBecariosAsignadosVistaEncargado.HeaderRow.Cells[4].Text = "Celular";
        gridBecariosAsignadosVistaEncargado.HeaderRow.Cells[5].Text = "Estado";
    }



    protected void GridBecariosAsignadosVistaEncargado_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        switch (e.CommandName)
        {
            // Abrir Pop Up aceptar/rechazar asignación vista encargado
            case "btnSeleccionarTupla_Click":
                {


                    rowIndex = Convert.ToInt32(e.CommandArgument);
                    int pageIndex = this.GridAsignaciones.PageIndex;
                    int pageSize = this.GridAsignaciones.PageSize;
                    rowIndex += (pageIndex * pageSize);

                    if(   (Convert.ToInt32(lstBecariosAsignadosEncargado[rowIndex][6])==2 ) || (Convert.ToInt32(lstBecariosAsignadosEncargado[rowIndex][6])==4 ) ){  

                        commonService.abrirPopUp("PopUpAsignacionEncargado", "Aceptar/Rechazar Asignación");
                        string nombreBecario =  lstBecariosAsignadosEncargado[rowIndex][0].ToString() + " " + lstBecariosAsignadosEncargado[rowIndex][1].ToString() + " " + lstBecariosAsignadosEncargado[rowIndex][2].ToString();
                        this.lblNombreBecarioPopUpVistaEncargado.Text = nombreBecario;
                        this.lblCicloBecarioPopUpVistaEncargado.Text = convertirANumeroRomano(periodoActual);
                        this.lblAnioBecarioPopUpVistaEncargado.Text = añoActual.ToString();
                        this.lblHorasBecarioPopUpVistaEncargado.Text = lstBecariosAsignadosEncargado[rowIndex][7].ToString();
                    }else{
                      commonService.mensajeJavascript("Esta asignación no está pendiente","Aviso");
                    }
                    
                } break;
        }
    }



}





