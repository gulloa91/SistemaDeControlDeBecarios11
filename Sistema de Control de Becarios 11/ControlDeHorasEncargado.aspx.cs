using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

public partial class ControlDeHorasEncargado : System.Web.UI.Page
{

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
        DataTable tablaHoraYFechaBecario = crearTablaHoraYFechaBecario();
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
        newRow = tablaHoraYFechaBecario.NewRow();
        newRow["Fecha"] = "06/06/2013";
        newRow["Cnt. de Horas"] = "2";

        tablaHoraYFechaBecario.Rows.InsertAt(newRow, 0);

        newRow = tablaHoraYFechaBecario.NewRow();
        newRow["Fecha"] = "04/06/2013";
        newRow["Cnt. de Horas"] = "4";

        tablaHoraYFechaBecario.Rows.InsertAt(newRow, 0);

        newRow = tablaHoraYFechaBecario.NewRow();
        newRow["Fecha"] = "28/05/2013";
        newRow["Cnt. de Horas"] = "6";

        tablaHoraYFechaBecario.Rows.InsertAt(newRow, 0);

        newRow = tablaHoraYFechaBecario.NewRow();
        newRow["Fecha"] = "25/05/2013";
        newRow["Cnt. de Horas"] = "5";

        tablaHoraYFechaBecario.Rows.InsertAt(newRow, 0);
        //}
        this.GridViewHoraYFechaBecario.DataSource = tablaHoraYFechaBecario;
        this.GridViewHoraYFechaBecario.DataBind();
        headersCorrectosHoraYFechaBecario();
    }

    /* Crear tabla */
    protected void llenarGridBecariosConHorasPorRevisar()
    {
        DataTable tablaBecariosConHorasPorRevisar = crearTablaBecariosConHorasPorRevisar();
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
        newRow = tablaBecariosConHorasPorRevisar.NewRow();
        newRow["Nombre"] = "Constantino Bolaños Araya";
        newRow["Carné"] = "B04512";
        newRow["Horas por Revisar"] = "13";

        tablaBecariosConHorasPorRevisar.Rows.InsertAt(newRow, 0);

        newRow = tablaBecariosConHorasPorRevisar.NewRow();
        newRow["Nombre"] = "Christopher Sánchez Coto";
        newRow["Carné"] = "B01239";
        newRow["Horas por Revisar"] = "14";

        tablaBecariosConHorasPorRevisar.Rows.InsertAt(newRow, 0);

        newRow = tablaBecariosConHorasPorRevisar.NewRow();
        newRow["Nombre"] = "Heriberto Ureña Madrigal";
        newRow["Carné"] = "B08888";
        newRow["Horas por Revisar"] = "17";

        tablaBecariosConHorasPorRevisar.Rows.InsertAt(newRow, 0);
        //}
        this.GridBecariosConHorasPendientes.DataSource = tablaBecariosConHorasPorRevisar;
        this.GridBecariosConHorasPendientes.DataBind();
        headersCorrectosBecariosConHorasPendientes();
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
}