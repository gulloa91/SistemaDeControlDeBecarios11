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
    private static CommonServices commonService;

    protected void Page_Load(object sender, EventArgs e)
    {
        commonService = new CommonServices(UpdateInfo);

        this.MultiViewControlHorasEncargado.ActiveViewIndex = 0;
        llenarGridBecariosConHorasPorRevisar();
        llenarGridViewHoraYFechaBecario();
    }

    /* Crear tabla */
    protected void llenarGridViewHoraYFechaBecario()
    {
        DataTable tablaBecariosConHorasPorRevisar = crearTablaHoraYFechaBecario();
        DataTable dt = controladora.consultarReportesBecarios((string)(Session["Cedula"]), 0);

        if (dt.Rows.Count > 0)
        {
            Object[] datos = new Object[2];
            foreach (DataRow r in tablaBecariosConHorasPorRevisar.Rows)
            {
                datos[0] = r[0].ToString();
                datos[1] = r[1].ToString();
                tablaBecariosConHorasPorRevisar.Rows.Add(datos);
            }
        }
        else
        {
            Object[] datos = new Object[2];
            datos[0] = "-";
            datos[1] = "-";
            tablaBecariosConHorasPorRevisar.Rows.Add(datos);
        }
        this.GridBecariosConHorasPendientes.DataSource = tablaBecariosConHorasPorRevisar;
        this.GridBecariosConHorasPendientes.DataBind();
        headersCorrectosBecariosConHorasPendientes();
    }

    /* Crear tabla */
    protected void llenarGridBecariosConHorasPorRevisar()
    {

    }

    protected DataTable crearTablaBecariosConHorasPorRevisar()
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
        column.ColumnName = "Fecha";
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
                    String nombreDeBecarioSeleccionado = "Heriberto Ureña";
                    commonService.abrirPopUp("PopUpControlDeHorasEncargado", "Revisar horas de: " + nombreDeBecarioSeleccionado);
                    commonService.esconderPrimerBotonDePopUp("PopUpControlDeHorasEncargado");
                } break;

        }
    }

    // Seleccionar Aceptar (para enviar a revisión las horas)
    protected void btnInvisibleEnviarRevision_Click(object sender, EventArgs e)
    {
        commonService.cerrarPopUp("PopUpControlDeHorasEncargado");
    }

    protected void desactivarCamposPrueba() {
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
                    this.txtComentarioBecario.Text = "Trabajo realizado el 23 de mayo del 2013";
                } break;

        }
    }
    protected void RadioButtonAceptarHoras_CheckedChanged(object sender, EventArgs e)
    {
        this.txtComentarioEncargado.Enabled = true;
    }
    protected void btnEnviar_Click(object sender, EventArgs e)
    {
        commonService.mensajeJavascript("Enviado","Atención");
    }

    protected void llenarGridBecariosDeEncargado(String idEncargado,int tipo) { 
    
    }
}