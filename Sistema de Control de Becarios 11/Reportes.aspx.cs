using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

public partial class Reportes : System.Web.UI.Page
{

    private static CommonServices commonService;

    protected void Page_Load(object sender, EventArgs e)
    {
        commonService = new CommonServices(UpdateInfo);
        MultiViewReportes.ActiveViewIndex = 0;
    }

    // Click del Menu
    protected void MenuListaReportes_MenuItemClick(object sender, MenuEventArgs e)
    {
        // Mostrar Grid:
        commonService.correrJavascript("$('#wrapperDeLaInfo').fadeIn();");
        switch (e.Item.Text)
        {
            // REPORTE 1
            case "Con horas finalizadas":
                {
                    this.lblReporteActivo.Text = "Consultar Becarios que han finalizado sus horas";
                    this.lblCriterio1.Text = "Estado"; // Dejar el primero
                    commonService.correrJavascript("$('#criterio2').css('display', 'none');"); // Esconder el segundo criterio
                    llenarGridReportes(0);
                } break;

            // REPORTE 2
            case "No asignados":
                {
                    this.lblReporteActivo.Text = "Reporte de becarios no asignados en un Semestre y Año, que si fueron asignados en el semestre anterior o tras-anterior";
                } break;

            // REPORTE 3
            case "Asignados por Unidad Académica":

                {
                    this.lblReporteActivo.Text = "Reporte de Estudiantes Asignados, por Unidad Académica";
                } break;

            // REPORTE 4 Y 5
            case "Reporte de Actividad":
                {
                    // Reporte de Actividad de BECARIO
                    if (e.Item.ValuePath == "Becarios/Reporte de Actividad")
                    {
                        this.lblReporteActivo.Text = "Reporte de estudiantes que no han reportado horas en un lapso, con fecha de último reporte";
                    }
                    // Reporte de Actividad de ENCARGADO
                    else
                    {
                        this.lblReporteActivo.Text = "Reporte de encargados con aprobaciones pendientes con más de un mes de atraso";
                    }

                } break;

            // REPORTE 6
            case "Asignaciones de un Becario":

                {
                    this.lblReporteActivo.Text = "Reporte de Historial de Asignaciones de un Becario";
                } break;

            // REPORTE 7
            case "Anotaciones de un Encargado":
                {
                    this.lblReporteActivo.Text = "Reporte de Historial de Anotaciones que hace un Encargado";
                } break;
        }
    }

    protected void llenarGridReportes(int caso)
    {
        DataTable tablaReporteHorasFinalizadas;
        DataRow newRow;

        switch(caso){

            // Reporte 1
            case 0:
                {
                    tablaReporteHorasFinalizadas = crearTablaHorasFinalizadas();

                    // Ciclo para llenar tabla
                    newRow = tablaReporteHorasFinalizadas.NewRow();
                    newRow["Nombre"] = "-";
                    newRow["Carné"] = "-";
                    newRow["Horas Asignadas"] = "-";
                    newRow["Estado"] = "-";
                    newRow["Fecha de Finalización"] = "-";

                    tablaReporteHorasFinalizadas.Rows.InsertAt(newRow, 0);

                    this.GridViewReporte.DataSource = tablaReporteHorasFinalizadas;
                    this.GridViewReporte.DataBind();

                } break;

            // Reporte 2
            // Reporte 3
            // Reporte 4
            // Reporte 5
            // Reporte 6
            // Reporte 7
        }

        this.headersCorrectosTablaReporte();
    }

    protected DataTable crearTablaHorasFinalizadas()
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
        column.ColumnName = "Horas Asignadas";
        dt.Columns.Add(column);

        column = new DataColumn();
        column.DataType = System.Type.GetType("System.String");
        column.ColumnName = "Estado";
        dt.Columns.Add(column);

        column = new DataColumn();
        column.DataType = System.Type.GetType("System.String");
        column.ColumnName = "Fecha de Finalización";
        dt.Columns.Add(column);

        return dt;
    }

    protected void headersCorrectosTablaReporte()
    {
        this.GridViewReporte.HeaderRow.BackColor = System.Drawing.Color.FromArgb(4562432);
        this.GridViewReporte.HeaderRow.ForeColor = System.Drawing.Color.White;
    }
}