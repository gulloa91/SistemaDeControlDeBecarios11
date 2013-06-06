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
        newRow["Nombre"] = "-";
        newRow["Carné"] = "-";
        newRow["Horas por Revisar"] = "-";

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

    protected void headersCorrectosBecariosConHorasPendientes()
    {
        this.GridBecariosConHorasPendientes.HeaderRow.BackColor = System.Drawing.Color.FromArgb(4562432);
        this.GridBecariosConHorasPendientes.HeaderRow.ForeColor = System.Drawing.Color.White;
    }

    // Selecciona tupla del grid
    protected void GridBecariosConHorasPendientes_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        switch (e.CommandName)
        {
            // Abrir Pop Up aceptar/rechazar horas
            case "btnSeleccionarTupla_Click":
                {
                    String nombreDeBecarioSeleccionado = "Heriberto Ureña";
                    commonService.abrirPopUp("PopUpControlDeHorasEncargado", "Revisar horas de: " + nombreDeBecarioSeleccionado);
                } break;

        }
    }

    // Seleccionar Aceptar (para enviar a revisión las horas)
    protected void btnInvisibleEnviarRevision_Click(object sender, EventArgs e)
    {
        commonService.cerrarPopUp("PopUpControlDeHorasEncargado");
    }
}