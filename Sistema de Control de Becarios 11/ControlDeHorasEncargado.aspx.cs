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
        if (!IsPostBack)
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

    /* Crear tabla */
    protected void llenarGridViewHoraYFechaBecario(int estado)
    {
        DataTable tablaBecariosConHorasPorRevisar = crearTablaHoraYFechaBecario();
        DataTable dt = controladora.consultarReportesBecarios((string)(Session["Cedula"]), estado);

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
        else
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

    /* Crear tabla */
    protected void llenarGridBecariosConHorasPorRevisar(String cedula, int estado)
    {
        comentariosEncargado.Clear();
        comentariosBecario.Clear();
        DataTable tablaHorasBecario = crearTablaBecariosConHorasPorRevisar();
        DataTable totalHoras = controladora.consultarReportesHorasBecarios((string)(Session["Cedula"]), cedula, estado);
        if (totalHoras.Rows.Count > 0)
        {
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

    protected void headersCorrectosBecariosConHorasPendientes()
    {
        this.GridBecariosConHorasPendientes.HeaderRow.BackColor = System.Drawing.Color.FromArgb(4562432);
        this.GridBecariosConHorasPendientes.HeaderRow.ForeColor = System.Drawing.Color.White;
    }

    protected void headersCorrectosHoraYFechaBecario()
    {
        this.GridViewHoraYFechaBecario.HeaderRow.BackColor = System.Drawing.Color.FromArgb(4562432);
        this.GridViewHoraYFechaBecario.HeaderRow.ForeColor = System.Drawing.Color.White;
    }

    // Selecciona tupla del grid
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

    protected void btnInvisibleCancelarRevision_Click(object sender, EventArgs e)
    {
        commonService.cerrarPopUp("PopUpControlDeHorasEncargado");
        llenarGridViewHoraYFechaBecario(this.drpDownOpc.SelectedIndex);
    }

    protected void btnInvisibleAsignacion_Click(object sender, EventArgs e)
    {
        //finalizo la asignacion
        finalizarAsignacion(1, DateTime.Now.Year);
        crearAsignacion(1, DateTime.Now.Year);
        commonService.cerrarPopUp("popUpConfirmar");
        llenarGridViewHoraYFechaBecario(this.drpDownOpc.SelectedIndex);
    }

    protected void btnInvisibleAsignacion2_Click(object sender, EventArgs e)
    {
        finalizarAsignacion(1, DateTime.Now.Year);
        commonService.cerrarPopUp("popUpConfirmar");
        llenarGridViewHoraYFechaBecario(this.drpDownOpc.SelectedIndex);
    }

    // AYUDA CLICK
    protected void btnAyuda_Click(object sender, EventArgs e)
    {
        commonService.abrirPopUp("PopUpAyuda", "Ayuda");
        commonService.esconderPrimerBotonDePopUp("PopUpAyuda");
    }

    protected void crearAsignacion(int periodo, int anno)
    {
        Object[] datos = new Object[5]; // becario periodo a;o encargado totalHoras
        datos[0] = controlHorasViejo[1].ToString();
        int proxPer = retornarProximoPeriodo(periodo);
        datos[1] = proxPer;
        datos[2] = retornarAnno(periodo, anno);
        datos[3] = (string)(Session["Cedula"]);
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

    protected int retornarProximoPeriodo(int periodo)
    {
        int resultado = -1;
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

    protected void finalizarAsignacion(int periodo, int año)
    {
        Object[] asignacionFinalizada = new Object[5];
        asignacionFinalizada[0] = this.comentFinalEncargado.Text;
        asignacionFinalizada[1] = controlHorasViejo[1].ToString();
        asignacionFinalizada[2] = periodo;
        asignacionFinalizada[3] = año;
        asignacionFinalizada[4] = (string)(Session["Cedula"]);
        String resul = controladora.finalizarAsignacion(asignacionFinalizada);
    }

    protected void desactivarCamposPrueba()
    {
        this.txtComentarioBecario.Enabled = false;
        this.txtComentarioEncargado.Enabled = false;
    }

    protected void activarCampoBecario()
    {
        this.txtComentarioBecario.Enabled = false;
        this.txtComentarioEncargado.Enabled = true;
    }
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
                    controlHorasViejo[0] = (string)(Session["Cedula"]);
                    controlHorasViejo[2] = 0;
                    controlHorasViejo[3] = filaSel.Cells[2].Text;
                    controlHorasViejo[4] = filaSel.Cells[1].Text;
                    controlHorasViejo[5] = comentariosEncargado[auxiliar];
                    controlHorasViejo[6] = comentariosBecario[auxiliar];
                    controlHorasViejo[7] = 1;
                    controlHorasViejo[8] = 2013;
                    // termino de llenar los datos viejos
                    this.txtComentarioEncargado.Enabled = false;
                    vaciarRadioButton(true);
                    this.txtComentarioBecario.Text = comentariosBecario[auxiliar];
                    this.txtComentarioEncargado.Text = comentariosEncargado[auxiliar];
                } break;

        }
    }

    protected void vaciarFilas()
    {
        foreach (GridViewRow r in this.GridViewHoraYFechaBecario.Rows)
        {
            r.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
            r.ForeColor = System.Drawing.Color.Black;
        }
    }

    protected void btnEnviar_Click(object sender, EventArgs e)
    {
        if (auxiliar != -1 && this.drpDownOpc.SelectedIndex == 0)
        {
            controlHorasNuevo[0] = controlHorasViejo[0];
            controlHorasNuevo[1] = controlHorasViejo[1];
            controlHorasNuevo[2] = retornarSeleccionRadioButon();
            controlHorasNuevo[3] = controlHorasViejo[3];
            controlHorasNuevo[4] = controlHorasViejo[4];
            controlHorasNuevo[6] = controlHorasViejo[6];
            controlHorasNuevo[5] = this.txtComentarioEncargado.Text;
            controlHorasNuevo[7] = controlHorasViejo[7];
            controlHorasNuevo[8] = controlHorasViejo[8];
            if ((this.RadioButtonAceptarHoras.Checked) == true || (this.RadioButtonRechazarHoras.Checked) == true)
            {
                String mensaje = controladora.modificarReporteEncargado(controlHorasNuevo, controlHorasViejo);
                if (mensaje == "")
                {
                    mensaje = "Operación realizada con éxito";
                    if (this.txtComentarioEncargado.Text != "")
                    {
                        crearComentario(this.txtComentarioEncargado.Text, controlHorasNuevo[1].ToString());
                    }
                }
                commonService.mensajeJavascript(mensaje, "Atención");
                vaciarCampos();
                llenarGridBecariosConHorasPorRevisar(controlHorasViejo[1].ToString(), this.drpDownOpc.SelectedIndex);
                this.GridBecariosConHorasPendientes.SelectedIndex = -1;
                this.txtComentarioEncargado.Enabled = false;
                if (controladora.obtenerTotalHoras((string)(Session["Cedula"]), controlHorasViejo[1].ToString(), 2) == controladora.horasAsignadasBecario((string)(Session["Cedula"]), controlHorasViejo[1].ToString(), 1, DateTime.Now.Year))
                {
                    this.lblTexto.Text = "El estudiante: " + controladora.obtenerNombrePorCedula(controlHorasViejo[1].ToString()) + " ha cumplido con el total de horas asignadas.";
                    commonService.cerrarPopUp("PopUpControlDeHorasEncargado");
                    commonService.abrirPopUp("popUpConfirmar", "Atención");
                }
            }
            else
            {
                commonService.mensajeJavascript("Por favor, seleccione una opcion (sí o no).", "Atención");
            }
        }
        else
        {
            if (this.drpDownOpc.SelectedIndex != 0)
            {
                commonService.mensajeJavascript("Lo sentimos, solo es posible revisar horas pendientes", "Atención");
            }
            else
            {
                if (auxiliar == -1)
                {
                    commonService.mensajeJavascript("Por favor, seleccione una fila para revisar", "Atención");
                }
            }
        }
    }

    protected void crearComentario(String comentarioEncargado, String cedulaBecario)
    {
        Object[] datos = new Object[4];
        datos[0] = (string)(Session["Cedula"]);
        datos[1] = cedulaBecario;
        datos[2] = DateTime.Now;
        datos[3] = comentarioEncargado;
        String resultado = controladora.insertarComentarioEncargado(datos);
    }

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

    protected void vaciarCampos()
    {
        this.txtComentarioBecario.Text = "";
        this.txtComentarioEncargado.Text = "";
        vaciarRadioButton(true);
    }

    protected void RadioButtonRechazarHoras_CheckedChanged(object sender, EventArgs e)
    {
        if (auxiliar != -1)
        {
            this.txtComentarioEncargado.Enabled = true;
        }
    }
    protected void RadioButtonAceptarHoras_CheckedChanged(object sender, EventArgs e)
    {
        this.txtComentarioEncargado.Text = "";
        this.txtComentarioEncargado.Enabled = false;
    }

    protected void vaciarRadioButton(Boolean decision)
    {
        if (decision)
        {
            this.RadioButtonAceptarHoras.Checked = false;
            this.RadioButtonRechazarHoras.Checked = false;
        }
    }

    protected void llenarDrp()
    {
        this.drpDownOpc.Items.Add("Pendientes");
        this.drpDownOpc.Items.Add("Rechazadas");
        this.drpDownOpc.Items.Add("Aceptadas");
        this.drpDownOpc.SelectedIndex = 0;
    }
    protected void drpDownOpc_SelectedIndexChanged(object sender, EventArgs e)
    {
        llenarGridViewHoraYFechaBecario(this.drpDownOpc.SelectedIndex);
    }

    protected void GridBecariosConHorasPendientes_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        this.GridBecariosConHorasPendientes.PageIndex = e.NewPageIndex;
        this.GridBecariosConHorasPendientes.DataBind();
        headersCorrectosHoraYFechaBecario();
        llenarGridViewHoraYFechaBecario(this.drpDownOpc.SelectedIndex);
    }

    protected void GridViewHoraYFechaBecario_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        this.GridViewHoraYFechaBecario.PageIndex = e.NewPageIndex;
        this.GridViewHoraYFechaBecario.DataBind();
        headersCorrectosBecariosConHorasPendientes();
        llenarGridBecariosConHorasPorRevisar(controlHorasViejo[1].ToString(), this.drpDownOpc.SelectedIndex);
    }
}
