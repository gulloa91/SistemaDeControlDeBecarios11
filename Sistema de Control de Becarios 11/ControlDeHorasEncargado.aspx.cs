using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

public partial class ControlDeHorasEncargado : System.Web.UI.Page
{

    private ControladoraControlEncargado controladora = new ControladoraControlEncargado();
    private ControladoraBecarios cb = new ControladoraBecarios();
    private static CommonServices commonService;
    private static List<String> comentariosBecario = new List<String>();
    private static List<String> comentariosEncargado = new List<String>();
    private static int auxiliar = -1;
    private static Object[] controlHorasViejo = new Object[9];
    private static Object[] controlHorasNuevo = new Object[9];
    private ControladoraControlEncargado controladoraControlEncargado = new ControladoraControlEncargado();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack) // si no es refresh  llenar el dropdown  y el grid
        {
            llenarDrp();
            llenarGridViewHoraYFechaBecario(0);
        }

        commonService = new CommonServices(UpdateInfo);

        this.MultiViewControlHorasEncargado.ActiveViewIndex = 0;

        // Actualizar botón del menu
        int horasPendientesRevision = controladoraControlEncargado.totalBecarios((string)(Session["Cedula"]), 0);
        string jscript = "$('a[href*=\"ControlDeHorasEncargado.aspx\"]').text(\"Revisar Horas (" + horasPendientesRevision.ToString() + ")\");";
        // " + horasPendientesRevision.ToString() + "
        commonService.correrJavascript(jscript);
    }

    /* Efectúa: Se encarga de llenar el grid con todos los becarios que tengan horas pendientes, horas revisadas u horas rechazadas. Dependiendo del
     * valor que se indique en la variable estado
    * Requiere: Que la variable estado indique un valor valido para rechazadas, aceptadas o pendientes
    * Modifica: El grid cuando lo actualiza con los datos traidos de la base de datos.
    */
    protected void llenarGridViewHoraYFechaBecario(int estado)
    {
        DataTable tablaBecariosConHorasPorRevisar = crearTablaHoraYFechaBecario();
        DataTable dt = controladora.consultarReportesBecarios((string)(Session["Cedula"]), estado); // traigo de la base todos los becarios del encargado
                                                                                                    // logueado, con el tipo de horas indicado
        if (dt.Rows.Count > 0)
        {
            Object[] datos = new Object[3];
            foreach (DataRow r in dt.Rows)
            {
                Becario bc = cb.obtenerBecarioPorCedula(r[0].ToString());
                datos[0] = bc.nombre + " " + bc.apellido1 + " " + bc.apellido2;
                datos[1] = bc.carne;
                datos[2] = r[1].ToString();
                tablaBecariosConHorasPorRevisar.Rows.Add(datos);
            }
        }
        else // en caso de no existir becarios para ese tipo de reportes (pendientes, rechazadas o aceptadas)
        {
            Object[] datos = new Object[3];
            datos[0] = "-";
            datos[1] = "-";
            datos[2] = "-";
            tablaBecariosConHorasPorRevisar.Rows.Add(datos);
        }
        this.GridBecariosConHorasPendientes.DataSource = tablaBecariosConHorasPorRevisar;
        this.GridBecariosConHorasPendientes.DataBind();
        headersCorrectosBecariosConHorasPendientes();
    }

    /* Efectúa: Se encarga de llenar el grid con todas las horas correspondientes a un becario. Ya sean horas pendientes, horas revisadas u horas 
     * rechazadas. Dependiendo del valor que se indique en la variable estado
    * Requiere: Que la variable estado indique un valor valido para rechazadas, aceptadas o pendientes
    * Modifica: El grid cuando lo actualiza con los datos traidos de la base de datos.
    */
    protected void llenarGridBecariosConHorasPorRevisar(String cedula, int estado)
    {
        comentariosEncargado.Clear(); // vacio la lista para volverla a llenar
        comentariosBecario.Clear(); // vacio la lista para volverla a llenar
        DataTable tablaHorasBecario = crearTablaBecariosConHorasPorRevisar();
        DataTable totalHoras = controladora.consultarReportesHorasBecarios((string)(Session["Cedula"]), cedula, estado); // traigo de la base todas las horas
        if (totalHoras.Rows.Count > 0)                                                                          // correspondientes de un becario asignado
        {                                                                                               // a un encargado, dependiendo de lo que se indique en la variable estado (pendientes, rechazadas o aceptadas)
            Object[] datos = new Object[2];
            foreach (DataRow r in totalHoras.Rows)
            {
                datos[0] = r[3].ToString();
                datos[1] = r[2].ToString();
                comentariosBecario.Add(r[4].ToString());
                comentariosEncargado.Add(r[5].ToString());
                tablaHorasBecario.Rows.Add(datos);
            }
        }
        else
        {
            Object[] datos = new Object[2];
            datos[0] = "-";
            datos[1] = "-";
            tablaHorasBecario.Rows.Add(datos);
        }
        this.GridViewHoraYFechaBecario.DataSource = tablaHorasBecario;
        this.GridViewHoraYFechaBecario.DataBind();
        headersCorrectosHoraYFechaBecario();
    }

    /* Efectúa: Se encarga de crear la tabla con las horas reportadas por un becario. Se crean dos columnas, una para la fecha del reporte y otra para las
     * horas reportadas.
    * Requiere: N/A
    * Modifica: N/A
    */
    protected DataTable crearTablaBecariosConHorasPorRevisar()
    {
        DataTable dt = new DataTable();
        DataColumn column;

        column = new DataColumn();
        column.DataType = System.Type.GetType("System.String");
        column.ColumnName = "Fecha";
        dt.Columns.Add(column);

        column = new DataColumn();
        column.DataType = System.Type.GetType("System.String");
        column.ColumnName = "Horas por Revisar";
        dt.Columns.Add(column);

        return dt;
    }

    /* Efectúa: Se encarga de crear la tabla con los becarios, ya sean con horas aceptadas, rechazadas u pendientes. Se crean tres columnas, una para el 
     * Nombre, otra para el carnet y otra para el total de horas reportadas por ese becario.
    * Requiere: N/A
    * Modifica: N/A
    */
    protected DataTable crearTablaHoraYFechaBecario()
    {
        DataTable dt = new DataTable();
        DataColumn column;

        column = new DataColumn();
        column.DataType = System.Type.GetType("System.String");
        column.ColumnName = "Nombre";
        dt.Columns.Add(column);

        column = new DataColumn();
        column.DataType = System.Type.GetType("System.String");
        column.ColumnName = "Carnet";
        dt.Columns.Add(column);

        column = new DataColumn();
        column.DataType = System.Type.GetType("System.String");
        column.ColumnName = "Cnt. de Horas";
        dt.Columns.Add(column);

        return dt;
    }

    // formato para el encabezado de la tabla de becarios con horas reportadas
    protected void headersCorrectosBecariosConHorasPendientes()
    {
        this.GridBecariosConHorasPendientes.HeaderRow.BackColor = System.Drawing.Color.FromArgb(4562432);
        this.GridBecariosConHorasPendientes.HeaderRow.ForeColor = System.Drawing.Color.White;
    }

    // formato para el encabezado de la tabla de horas reportadas por un becario
    protected void headersCorrectosHoraYFechaBecario()
    {
        this.GridViewHoraYFechaBecario.HeaderRow.BackColor = System.Drawing.Color.FromArgb(4562432);
        this.GridViewHoraYFechaBecario.HeaderRow.ForeColor = System.Drawing.Color.White;
    }

    /* Efectúa: Se encarga de abrir la ventana emergente que contiene todas las horas reportadas de un becario seleccionado del grid principal.
    * Requiere: N/A
    * Modifica: El grid de horas reportadas de un becario.
    */
    protected void GridBecariosConHorasPendientes_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        desactivarCamposPrueba();
        this.txtComentarioBecario.Text = "";
        switch (e.CommandName)
        {
            // Abrir Pop Up aceptar/rechazar horas
            case "btnSeleccionarTupla_Click":
                {
                    auxiliar = -1;
                    vaciarRadioButton(true);
                    GridViewRow filaSeleccionda = this.GridBecariosConHorasPendientes.Rows[Convert.ToInt32(e.CommandArgument)];
                    if (filaSeleccionda.Cells[1].Text != "-")
                    {
                        controlHorasViejo[1] = cb.consultarCedulaByCarne(filaSeleccionda.Cells[2].Text);
                        String nombreDeBecarioSeleccionado = filaSeleccionda.Cells[1].Text;
                        llenarGridBecariosConHorasPorRevisar(controlHorasViejo[1].ToString(), this.drpDownOpc.SelectedIndex);
                        commonService.abrirPopUp("PopUpControlDeHorasEncargado", "Revisar horas de: " + nombreDeBecarioSeleccionado);
                        commonService.esconderPrimerBotonDePopUp("PopUpControlDeHorasEncargado");
                    }
                } break;

        }
    }

    // Seleccionar Aceptar (para enviar a revisión las horas)
    protected void btnInvisibleEnviarRevision_Click(object sender, EventArgs e)
    {
        commonService.cerrarPopUp("PopUpControlDeHorasEncargado");
    }

    //Cerrar la ventana emergente de control de horas para un becario
    protected void btnInvisibleCancelarRevision_Click(object sender, EventArgs e)
    {
        commonService.cerrarPopUp("PopUpControlDeHorasEncargado");
        llenarGridViewHoraYFechaBecario(this.drpDownOpc.SelectedIndex);
    }

    /* Efectúa: Se encarga de llamar al método encargado de finalizar la asignación entre un becario y un encargado cuando el becario ya cumplió el total de horas. Y al método
     * encargado de crear la nueva asignación
     *  en caso de que el encargado acepte.
    * Requiere: N/A
    * Modifica: N/A
    */
    protected void btnInvisibleAsignacion_Click(object sender, EventArgs e)
    {
        //finalizo la asignacion
        finalizarAsignacion(Convert.ToInt32(Session["Periodo"].ToString()), DateTime.Now.Year);
        crearAsignacion(Convert.ToInt32(Session["Periodo"].ToString()), DateTime.Now.Year);
        commonService.cerrarPopUp("popUpConfirmar");
        llenarGridViewHoraYFechaBecario(this.drpDownOpc.SelectedIndex);
    }

    /* Efectúa:  Se encarga de llamar al método encargado de finalizar la asignación entre un becario y un encargado cuando el becario ya cumplió el total de horas.
    * Requiere: N/A
    * Modifica: N/A
    */
    protected void btnInvisibleAsignacion2_Click(object sender, EventArgs e)
    {
        finalizarAsignacion(Convert.ToInt32(Session["Periodo"].ToString()), DateTime.Now.Year);
        commonService.cerrarPopUp("popUpConfirmar");
        llenarGridViewHoraYFechaBecario(this.drpDownOpc.SelectedIndex);
    }

    /* Efectúa: Método encargado de crear una nueva asignacion entre el becario y el encargado en el próximo período. 
    * Requiere: N/A
    * Modifica: La base de datos cuando crea la asignacion.
    */
    protected void crearAsignacion(int periodo, int anno)
    {
        Object[] datos = new Object[5]; // becario periodo a;o encargado totalHoras
        datos[0] = controlHorasViejo[1].ToString(); // cedula becario
        int proxPer = retornarProximoPeriodo(periodo); // siguiente periodo al actual
        datos[1] = proxPer;
        datos[2] = retornarAnno(periodo, anno);
        datos[3] = (string)(Session["Cedula"]); // cedula del encargado
        int totalHoras = -1;
        if (proxPer == 3)
        {
            totalHoras = 36;
        }
        else
        {
            totalHoras = 72;
        }
        datos[4] = totalHoras;
        controladora.crearAsignacion(datos);
    }

    /* Efectúa: Se encarga de retornar el próximo período al período ingresado por parametro.
    * Requiere: N/A
    * Modifica: N/A
    */
    protected int retornarProximoPeriodo(int periodo)
    {
        int resultado = -1; // retorna -1 si no es 1 , 2 o 3 periodo
        switch (periodo)
        {
            case 1:
                {
                    resultado = 2;
                } break;
            case 2:
                {
                    resultado = 3;
                } break;
            case 3:
                {
                    resultado = 1;
                } break;
        }
        return resultado;
    }

    /* Efectúa: Se encarga de retornar el próximo año, tomando como base el período y año ingresados por parámetro.
    * Requiere: N/A
    * Modifica: N/A
    */
    protected int retornarAnno(int periodo, int anno)
    {
        int resultado = -1;
        switch (periodo)
        {
            case 1:
                {
                    resultado = anno;
                } break;
            case 2:
                {
                    resultado = (anno + 1);
                } break;
            case 3:
                {
                    resultado = anno;
                } break;
        }
        return resultado;
    }

    /* Efectúa: Se encarga de finalizar la asignación entre un encargado y un becario, cuadno el becario finalice las horas correspondientes.
    * Requiere: Que periodo y año correspondan a la asignacion entre el becario y el encargado.
    * Modifica: La base de datos cuando realiza el update de la asignacion.
    */
    protected void finalizarAsignacion(int periodo, int año)
    {
        Object[] asignacionFinalizada = new Object[5];
        asignacionFinalizada[0] = this.comentFinalEncargado.Text; // comentario del encargado cuando finaliza asignacion
        asignacionFinalizada[1] = controlHorasViejo[1].ToString(); // cedula becario
        asignacionFinalizada[2] = periodo;
        asignacionFinalizada[3] = año;
        asignacionFinalizada[4] = (string)(Session["Cedula"]); // cedula encargado
        String resul = controladora.finalizarAsignacion(asignacionFinalizada);
    }

    /* Efectúa: Desactiva los campos del comentario del encargado y del becario
    * Requiere: N/A
    * Modifica: N/A
    */
    protected void desactivarCamposPrueba()
    {
        this.txtComentarioBecario.Enabled = false;
        this.txtComentarioEncargado.Enabled = false;
    }

    /* Efectúa: Activa los campos del comentario del encargado y del becario
    * Requiere: N/A
    * Modifica: N/A
    */
    protected void activarCampoBecario()
    {
        this.txtComentarioBecario.Enabled = false;
        this.txtComentarioEncargado.Enabled = true;
    }

    /* Efectúa: Se encarga de cargar los datos correspondientes a un reporte de horas seleccionado y preparar los campos para aceptar o
     *  rechazar dicho reporte de horas.
    * Requiere: N/A
    * Modifica: N/A
    */
    protected void GridViewHoraYFechaBecario_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        switch (e.CommandName)
        {
            // Abrir Pop Up aceptar/rechazar horas
            case "btnSeleccionarTupla_Click":
                {
                    vaciarFilas();
                    auxiliar = Convert.ToInt32(e.CommandArgument);
                    //lleno los datos viejos aca
                    GridViewRow filaSel = this.GridViewHoraYFechaBecario.Rows[auxiliar];
                    filaSel.BackColor = System.Drawing.Color.FromArgb(5, 170, 225);
                    filaSel.ForeColor = System.Drawing.Color.White;
                    controlHorasViejo[0] = (string)(Session["Cedula"]); // cedula encargado
                    controlHorasViejo[2] = 0; // estado de pendientes
                    controlHorasViejo[3] = filaSel.Cells[2].Text;
                    controlHorasViejo[4] = filaSel.Cells[1].Text;
                    controlHorasViejo[5] = comentariosEncargado[auxiliar];
                    controlHorasViejo[6] = comentariosBecario[auxiliar];
                    controlHorasViejo[7] = Convert.ToInt32(Session["Periodo"].ToString()); // periodo actual
                    controlHorasViejo[8] = DateTime.Now.Year; // a;o actual
                    // termino de llenar los datos viejos
                    this.txtComentarioEncargado.Enabled = false;
                    vaciarRadioButton(true);
                    this.txtComentarioBecario.Text = comentariosBecario[auxiliar];
                    this.txtComentarioEncargado.Text = comentariosEncargado[auxiliar];
                } break;

        }
    }

    /* Efectúa: Pinta de color blanco las filas, para evitar que queden seleccionadas multiples filas.
    * Requiere: N/A
    * Modifica: N/A
    */
    protected void vaciarFilas()
    {
        foreach (GridViewRow r in this.GridViewHoraYFechaBecario.Rows)
        {
            r.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
            r.ForeColor = System.Drawing.Color.Black;
        }
    }

    /* Efectúa: Se encarga de aceptar o rechazar un reporte de horas de un becario. En caso de que al aceptar el reporte el becario finalice el total de horas
     * asignadas muestra un mensaje preguntando al encargado si desea seguir trabajando con ese becario, en caso de aceptar crea la nueva asignacion.
    * Requiere: N/A
    * Modifica: La base de datos cuando modifica el reporte de horas. 
    */
    protected void btnEnviar_Click(object sender, EventArgs e)
    {
        if (auxiliar != -1 && this.drpDownOpc.SelectedIndex == 0)
        {
            // lleno el arreglo con los datos correspondientes al reporte de horas aceptado o rechazado por el encargado
            controlHorasNuevo[0] = controlHorasViejo[0]; // cedula encargado
            controlHorasNuevo[1] = controlHorasViejo[1]; // cedula becario
            controlHorasNuevo[2] = retornarSeleccionRadioButon(); // aceptado o rechazado
            controlHorasNuevo[3] = controlHorasViejo[3];
            controlHorasNuevo[4] = controlHorasViejo[4];
            controlHorasNuevo[6] = controlHorasViejo[6];
            controlHorasNuevo[5] = this.txtComentarioEncargado.Text; //en caso de rechazado lleva el coment del encargado
            controlHorasNuevo[7] = controlHorasViejo[7];
            controlHorasNuevo[8] = controlHorasViejo[8];
            if ((this.RadioButtonAceptarHoras.Checked) == true || (this.RadioButtonRechazarHoras.Checked) == true)
            {
                String mensaje = controladora.modificarReporteEncargado(controlHorasNuevo, controlHorasViejo); // acepto o rechazo las horas reportadas
                if (mensaje == "")
                {
                    mensaje = "Operación realizada con éxito";
                    if (this.txtComentarioEncargado.Text != "")
                    {
                        crearComentario(this.txtComentarioEncargado.Text, controlHorasNuevo[1].ToString()); // creo el comentario del encargado para el historial
                    }
                }
                commonService.mensajeJavascript(mensaje, "Atención");
                vaciarCampos();
                llenarGridBecariosConHorasPorRevisar(controlHorasViejo[1].ToString(), this.drpDownOpc.SelectedIndex);
                this.GridBecariosConHorasPendientes.SelectedIndex = -1;
                this.txtComentarioEncargado.Enabled = false;
                // en caso de completar las horas el becario, pregunta si desea seguir trabajando juntos
                if (controladora.obtenerTotalHoras((string)(Session["Cedula"]), controlHorasViejo[1].ToString(), 2) == controladora.horasAsignadasBecario((string)(Session["Cedula"]), controlHorasViejo[1].ToString(), Convert.ToInt32(Session["Periodo"].ToString()), DateTime.Now.Year))
                {
                    this.lblTexto.Text = "El estudiante: " + controladora.obtenerNombrePorCedula(controlHorasViejo[1].ToString()) + " ha cumplido con el total de horas asignadas.";
                    commonService.cerrarPopUp("PopUpControlDeHorasEncargado");
                    commonService.abrirPopUp("popUpConfirmar", "Atención");
                }
            }
            else // si no selecciona aceptar o rechazar
            {
                commonService.mensajeJavascript("Por favor, seleccione una opcion (sí o no).", "Atención");
            }
        }
        else
        {
            if (this.drpDownOpc.SelectedIndex != 0)// si la tupla seleccionada es de rechazadas o aceptadas
            {
                commonService.mensajeJavascript("Lo sentimos, solo es posible revisar horas pendientes", "Atención");
            }
            else
            {
                if (auxiliar == -1) //si no selecciona ninguna fila
                {
                    commonService.mensajeJavascript("Por favor, seleccione una fila para revisar", "Atención");
                }
            }
        }
    }

    /* Efectúa: Crea un comentario en la base de datos del encargado hacia el becario. Para control de historial principalmente.
    * Requiere: N/A
    * Modifica: La base de datos cuando modifica el reporte de horas. 
    */
    protected void crearComentario(String comentarioEncargado, String cedulaBecario)
    {
        Object[] datos = new Object[4];
        datos[0] = (string)(Session["Cedula"]); // cedula encargado
        datos[1] = cedulaBecario;
        datos[2] = DateTime.Now;
        datos[3] = comentarioEncargado;
        String resultado = controladora.insertarComentarioEncargado(datos);
    }

    /* Efectúa: Retorna la selección entre los radio buton de aceptar y rechazar, en caso de ninguno estar seleccionado retorna un -1.
    * Requiere: N/A
    * Modifica: N/A
    */
    protected int retornarSeleccionRadioButon()
    {
        int resultado = -1;
        if (this.RadioButtonAceptarHoras.Checked)
        {
            resultado = 2;
        }
        else
        {
            resultado = 1;
        }
        return resultado;
    }

    /* Efectúa: Vacia los campos de texto de los comentarios del becario y encargado. Ademas llama al metodo encargado de limpiar los radio buton.
    * Requiere: N/A
    * Modifica: N/A
    */
    protected void vaciarCampos()
    {
        this.txtComentarioBecario.Text = "";
        this.txtComentarioEncargado.Text = "";
        vaciarRadioButton(true);
    }

    /* Efectúa: Activa el campo de texto para el comentario del encargado, cuando selecciona rechazar horas.
    * Requiere: N/A
    * Modifica: N/A
    */
    protected void RadioButtonRechazarHoras_CheckedChanged(object sender, EventArgs e)
    {
        if (auxiliar != -1)
        {
            this.txtComentarioEncargado.Enabled = true;
        }
    }

    /* Efectúa: Desactiva el campo de texto para el comentario del encargado, cuando selecciona aceptar horas.
        * Requiere: N/A
        * Modifica: N/A
        */
    protected void RadioButtonAceptarHoras_CheckedChanged(object sender, EventArgs e)
    {
        this.txtComentarioEncargado.Text = "";
        this.txtComentarioEncargado.Enabled = false;
    }

    /* Efectúa: Deselecciona los radio buton.
        * Requiere: N/A
        * Modifica: N/A
        */
    protected void vaciarRadioButton(Boolean decision)
    {
        if (decision)
        {
            this.RadioButtonAceptarHoras.Checked = false;
            this.RadioButtonRechazarHoras.Checked = false;
        }
    }

    /* Efectúa: Llena el dropdown del tipo de horas, ya sean aceptadas, rechazadas o pendientes y deja seleccionada el dropdown en "Pendientes"
        * Requiere: N/A
        * Modifica: N/A
        */
    protected void llenarDrp()
    {
        this.drpDownOpc.Items.Add("Pendientes");
        this.drpDownOpc.Items.Add("Rechazadas");
        this.drpDownOpc.Items.Add("Aceptadas");
        this.drpDownOpc.SelectedIndex = 0;
    }

    /* Efectúa: Llena el grid de becarios cada vez que cambie la seleccion del dropdown a "Pendientes", "Rechazadas" o "Aceptadas". 
        * Requiere: N/A
        * Modifica: N/A
        */
    protected void drpDownOpc_SelectedIndexChanged(object sender, EventArgs e)
    {
        llenarGridViewHoraYFechaBecario(this.drpDownOpc.SelectedIndex);
    }

    /* Efectúa: Llena el grid de becarios con los datos correspondientes a la pagina que se selecciono.
        * Requiere: N/A
        * Modifica: N/A
        */
    protected void GridBecariosConHorasPendientes_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        this.GridBecariosConHorasPendientes.PageIndex = e.NewPageIndex;
        this.GridBecariosConHorasPendientes.DataBind();
        headersCorrectosHoraYFechaBecario();
        llenarGridViewHoraYFechaBecario(this.drpDownOpc.SelectedIndex);
    }

    /* Efectúa: Llena el grid de reporte de horas de un becario con los datos correspondientes a la pagina que se selecciono.
        * Requiere: N/A
        * Modifica: N/A
        */
    protected void GridViewHoraYFechaBecario_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        this.GridViewHoraYFechaBecario.PageIndex = e.NewPageIndex;
        this.GridViewHoraYFechaBecario.DataBind();
        headersCorrectosBecariosConHorasPendientes();
        llenarGridBecariosConHorasPorRevisar(controlHorasViejo[1].ToString(), this.drpDownOpc.SelectedIndex);
    }
}
