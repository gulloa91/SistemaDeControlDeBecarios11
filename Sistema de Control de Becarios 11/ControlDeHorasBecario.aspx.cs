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
    private DataTable infoActual;
    private ControladoraControlBecario cb;
	protected void Page_Load(object sender, EventArgs e)
	{
        this.MultiViewBecario.ActiveViewIndex = 0;
        commonService = new CommonServices(UpdateInfo);
        cb = new ControladoraControlBecario();
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
        infoActual = cb.horasReportadas(Session["Cedula"].ToString());
        DataRow newRow;
        if (infoActual.Rows.Count > 0)
        {
            foreach (DataRow r in infoActual.Rows) {
                newRow = tablaHorasReportadas.NewRow();
                newRow["Fecha"] = r[3].ToString();
                newRow["Cantidad Hora"] = r[2].ToString();
                newRow["Estado"] = r[4].ToString();
                tablaHorasReportadas.Rows.Add(newRow);
            }

            /*SELECT        CedulaBecario, CedulaEncargado, CantidadHoras, Fecha, Estado, CuerpoComentario, NombreAutor, PrimerApellidoAutor, SegundoApellidoAutor, Timestamp
FROM            ControlDeHoras
WHERE        (CedulaBecario = @Cedula)*/

        }
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
        column.ColumnName = "Cantidad Horas";
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