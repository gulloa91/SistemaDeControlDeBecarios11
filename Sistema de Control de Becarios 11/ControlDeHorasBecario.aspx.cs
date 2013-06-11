using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

public partial class ControlDeHoras : System.Web.UI.Page
{

    private static CommonServices commonService;

	protected void Page_Load(object sender, EventArgs e)
	{
        this.MultiViewBecario.ActiveViewIndex = 0;
        commonService = new CommonServices(UpdateInfo);

        // TEMP
        llenarGridHorasReportadas();
        // TEMP
	}

	protected void btnReportarHoras_Click(object sender, EventArgs e)
	{
        commonService.abrirPopUp("PopUpCtrlBecario", "Nuevo Reporte de Horas");
        commonService.correrJavascript("$('#comentarioDeEncargado').hide();");
	}

    // Selecciona tupla del grid
    protected void gridControlHorasBecario_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        
        switch (e.CommandName)
        {
            // Abrir Pop Up aceptar/rechazar horas
            case "btnSeleccionarTupla_Click":
                {
                    commonService.abrirPopUp("PopUpCtrlBecario", "Nuevo Reporte de Horas");
                    commonService.correrJavascript("$('#comentarioDeEncargado').show();");
                } break;

        }
    }

	protected void gridControlHorasBecario_SelectedIndexChanging(object sender, EventArgs e)
	{

	}

	protected void gridControlHorasBecario_PageIndexChanging(object sender, EventArgs e)
	{

	}

	protected void btnInvisibleAceptarAsignacion_Click(object sender, EventArgs e)
	{

	}

	protected void btnInvisibleEliminarAsignacion_Click(object sender, EventArgs e)
	{

	}

	// BUSCAR CLICK
	protected void btnBuscar_Click(object sender, EventArgs e)
	{

	}


    /* Crear tabla */
    protected void llenarGridHorasReportadas()
    {
        DataTable tablaHorasReportadas = crearTablaHorasReportadas();
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
        newRow = tablaHorasReportadas.NewRow();
        newRow["Fecha"] = "06/06/2013";
        newRow["Estado"] = "Revisada";

        tablaHorasReportadas.Rows.InsertAt(newRow, 0);

        newRow = tablaHorasReportadas.NewRow();
        newRow["Fecha"] = "04/06/2013";
        newRow["Estado"] = "Revisada";

        tablaHorasReportadas.Rows.InsertAt(newRow, 0);

        newRow = tablaHorasReportadas.NewRow();
        newRow["Fecha"] = "28/05/2013";
        newRow["Estado"] = "Rechazada";

        tablaHorasReportadas.Rows.InsertAt(newRow, 0);

        newRow = tablaHorasReportadas.NewRow();
        newRow["Fecha"] = "25/05/2013";
        newRow["Estado"] = "En Revisión";

        tablaHorasReportadas.Rows.InsertAt(newRow, 0);
        //}
        this.gridControlHorasBecario.DataSource = tablaHorasReportadas;
        this.gridControlHorasBecario.DataBind();
        headersCorrectosHorasReportadas();
    }

    protected DataTable crearTablaHorasReportadas()
    {
        DataTable dt = new DataTable();
        DataColumn column;

        column = new DataColumn();
        column.DataType = System.Type.GetType("System.String");
        column.ColumnName = "Fecha";
        dt.Columns.Add(column);

        column = new DataColumn();
        column.DataType = System.Type.GetType("System.String");
        column.ColumnName = "Estado";
        dt.Columns.Add(column);

        return dt;
    }

    protected void headersCorrectosHorasReportadas()
    {
        this.gridControlHorasBecario.HeaderRow.BackColor = System.Drawing.Color.FromArgb(4562432);
        this.gridControlHorasBecario.HeaderRow.ForeColor = System.Drawing.Color.White;
    }
}