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
                    this.lblCriterio2.Text = "Semestre";
                    this.lblCriterio3.Text = "Año"; 
                    commonService.correrJavascript("$('#criterio4').css('display', 'none');"); // Esconder el segundo criterio
                    commonService.correrJavascript("$('#criterio5').css('display', 'none');");
                    llenarGridReportes(0);
                } break;

            // REPORTE 2
            case "No asignados":
                {
                    this.lblReporteActivo.Text = "Reporte de becarios no asignados en un Semestre y Año, que si fueron asignados en el semestre anterior o tras-anterior";
                    this.lblCriterio1.Text = "Semestre";
                    this.lblCriterio2.Text = "Año";
                    this.lblCriterio3.Text = "Última Asignación";                     
                    commonService.correrJavascript("$('#criterio4').css('display', 'none');"); 
                    commonService.correrJavascript("$('#criterio5').css('display', 'none');");
                    llenarGridReportes(1);
                } break;

            // REPORTE 3
            case "Asignados por Unidad Académica":

                {
                    this.lblReporteActivo.Text = "Reporte de Estudiantes Asignados, por Unidad Académica";
                    this.lblCriterio1.Text = "Semestre";
                    this.lblCriterio2.Text = "Año";
                    this.lblCriterio3.Text = "Unidad Académica";
                    commonService.correrJavascript("$('#criterio4').css('display', 'none');");
                    commonService.correrJavascript("$('#criterio5').css('display', 'none');");
                    llenarGridReportes(2);
                } break;

            // REPORTE 4 Y 5
            case "Reporte de Actividad":
                {
                    // Reporte de Actividad de BECARIO
                    if (e.Item.ValuePath == "Becarios/Reporte de Actividad")
                    {
                        this.lblReporteActivo.Text = "Reporte de estudiantes que no han reportado horas en un lapso, con fecha de último reporte";
                        this.lblCriterio1.Text = "Semestre";
                        this.lblCriterio2.Text = "Año";
                        this.lblCriterio3.Text = "Fecha Último Reporte";
                        commonService.correrJavascript("$('#criterio4').css('display', 'none');");
                        commonService.correrJavascript("$('#criterio5').css('display', 'none');");
                        llenarGridReportes(3);
                    }
                    // Reporte de Actividad de ENCARGADO
                    else
                    {
                        this.lblReporteActivo.Text = "Reporte de encargados con aprobaciones pendientes con más de un mes de atraso";
                        this.lblCriterio1.Text = "Semestre";
                        this.lblCriterio2.Text = "Año";
                        this.lblCriterio3.Text = "Fecha Último Reporte";
                        commonService.correrJavascript("$('#criterio4').css('display', 'none');");
                        commonService.correrJavascript("$('#criterio5').css('display', 'none');");
                        llenarGridReportes(4);
                    }

                } break;

            // REPORTE 6
            case "Asignaciones de un Becario":
                {
                    this.lblReporteActivo.Text = "Reporte de Historial de Asignaciones de un Becario";
                    commonService.correrJavascript("$('#criterio1').css('display', 'none');");
                    commonService.correrJavascript("$('#criterio2').css('display', 'none');");
                    commonService.correrJavascript("$('#criterio3').css('display', 'none');");
                    commonService.correrJavascript("$('#criterio4').css('display', 'none');");
                    commonService.correrJavascript("$('#criterio5').css('display', 'none');");
                    llenarGridReportes(5);
                } break;

            // REPORTE 7
            case "Anotaciones de un Encargado":
                {
                    this.lblReporteActivo.Text = "Reporte de Historial de Anotaciones que hace un Encargado";
                    commonService.correrJavascript("$('#criterio1').css('display', 'none');");
                    commonService.correrJavascript("$('#criterio2').css('display', 'none');");
                    commonService.correrJavascript("$('#criterio3').css('display', 'none');");
                    commonService.correrJavascript("$('#criterio4').css('display', 'none');");
                    commonService.correrJavascript("$('#criterio5').css('display', 'none');");
                    llenarGridReportes(6);
                } break;
        }
    }

    protected void llenarGridReportes(int caso)
    {
        DataTable tablaReporte;
        DataRow newRow;

        switch(caso){

            // Reporte 1
            case 0:
                {
                    tablaReporte = crearTabla(0);

                    // Ciclo para llenar tabla
                    newRow = tablaReporte.NewRow();
                    newRow["Nombre"] = "-";
                    newRow["Carné"] = "-";
                    newRow["Horas Asignadas"] = "-";
                    newRow["Estado"] = "-";                    
                    newRow["Fecha de Finalización"] = "-";
                    newRow["Encargado"] = "-";

                    tablaReporte.Rows.InsertAt(newRow, 0);

                    this.GridViewReporte.DataSource = tablaReporte;
                    this.GridViewReporte.DataBind();

                } break;

            // Reporte 2
            case 1:
                {
                    tablaReporte = crearTabla(1);
                    
                    // Ciclo para llenar tabla
                    newRow = tablaReporte.NewRow();
                    newRow["Nombre"] = "-";
                    newRow["Carné"] = "-";
                    newRow["Horas Asignadas"] = "-";
                    newRow["Correo"] = "-";
                    newRow["Teléfono"] = "-";
                    tablaReporte.Rows.InsertAt(newRow, 0);

                    this.GridViewReporte.DataSource = tablaReporte;
                    this.GridViewReporte.DataBind();
                } break;
            // Reporte 3
            case 2:
                {
                    tablaReporte = crearTabla(2);

                    // Ciclo para llenar tabla
                    newRow = tablaReporte.NewRow();
                    newRow["Nombre"] = "-";
                    newRow["Carné"] = "-";
                    newRow["Horas Asignadas"] = "-";
                    newRow["Estado"] = "-";
                    newRow["Encargado"] = "-";

                    tablaReporte.Rows.InsertAt(newRow, 0);

                    this.GridViewReporte.DataSource = tablaReporte;
                    this.GridViewReporte.DataBind();
                } break;
            // Reporte 4
            case 3:
                {
                    tablaReporte = crearTabla(3);

                    // Ciclo para llenar tabla
                    newRow = tablaReporte.NewRow();
                    newRow["Nombre"] = "-";
                    newRow["Carné"] = "-";
                    newRow["Horas Asignadas"] = "-";
                    newRow["Horas Completadas"] = "-";
                    newRow["Encargado"] = "-";

                    tablaReporte.Rows.InsertAt(newRow, 0);

                    this.GridViewReporte.DataSource = tablaReporte;
                    this.GridViewReporte.DataBind();

                } break;
            // Reporte 5
            case 4:
                {
                    tablaReporte = crearTabla(4);

                    // Ciclo para llenar tabla
                    newRow = tablaReporte.NewRow();
                    newRow["Nombre"] = "-";
                    newRow["Cédula"] = "-";
                    newRow["Cnt. Becarios Asignados"] = "-";
                    newRow["Fecha de Última Actividad"] = "-";

                    tablaReporte.Rows.InsertAt(newRow, 0);

                    this.GridViewReporte.DataSource = tablaReporte;
                    this.GridViewReporte.DataBind();
                    
                } break;
            // Reporte 6
            case 5:
                {
                    tablaReporte = crearTabla(5);

                    // Ciclo para llenar tabla
                    newRow = tablaReporte.NewRow();
                    newRow["Encargado"] = "-";
                    newRow["Ciclo"] = "-";
                    newRow["Año"] = "-";
                    newRow["Estado"] = "-";

                    tablaReporte.Rows.InsertAt(newRow, 0);

                    this.GridViewReporte.DataSource = tablaReporte;
                    this.GridViewReporte.DataBind();

                } break;
            // Reporte 7
            case 6:
                {
                    tablaReporte = crearTabla(6);
                    
                    // Ciclo para llenar tabla
                    newRow = tablaReporte.NewRow();
                    newRow["Becario"] = "-";
                    newRow["Ciclo"] = "-";
                    newRow["Año"] = "-";
                    newRow["Estado"] = "-";

                    tablaReporte.Rows.InsertAt(newRow, 0);

                    this.GridViewReporte.DataSource = tablaReporte;
                    this.GridViewReporte.DataBind();

                } break;
        }

        this.headersCorrectosTablaReporte();
    }

    protected DataTable crearTabla(int reporte)
    {
        DataTable dt = new DataTable();
        DataColumn column;

        switch (reporte)
        {
            //Tabla becarios horas finalizadas
            case 0:
                {
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

                    column = new DataColumn();
                    column.DataType = System.Type.GetType("System.String");
                    column.ColumnName = "Encargado";
                    dt.Columns.Add(column);

                } break;
           
            //tabla becarios no asignados
            case 1:
                {
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
                    column.ColumnName = "Correo";
                    dt.Columns.Add(column);

                    column = new DataColumn();
                    column.DataType = System.Type.GetType("System.String");
                    column.ColumnName = "Teléfono";
                    dt.Columns.Add(column);

                } break;

            //tabla becarios por unidad academica
            case 2:
                {
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
                    column.ColumnName = "Encargado";
                    dt.Columns.Add(column);
                } break;

            //tabla reporte de actividad becarios
            case 3:
                {
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
                    column.ColumnName = "Horas Completadas";
                    dt.Columns.Add(column);

                    column = new DataColumn();
                    column.DataType = System.Type.GetType("System.String");
                    column.ColumnName = "Encargado";
                    dt.Columns.Add(column);
                } break;

            //tabla reporte de actividad encargados
            case 4:
                {
                    column = new DataColumn();
                    column.DataType = System.Type.GetType("System.String");
                    column.ColumnName = "Nombre";
                    dt.Columns.Add(column);

                    column = new DataColumn();
                    column.DataType = System.Type.GetType("System.String");
                    column.ColumnName = "Cédula";
                    dt.Columns.Add(column);

                    column = new DataColumn();
                    column.DataType = System.Type.GetType("System.String");
                    column.ColumnName = "Cnt. Becarios Asignados";
                    dt.Columns.Add(column);

                    column = new DataColumn();
                    column.DataType = System.Type.GetType("System.String");
                    column.ColumnName = "Fecha de Última Actividad";
                    dt.Columns.Add(column);

                } break;

            //tabla historial de asignaciones becario
            case 5:
                {
                    column = new DataColumn();
                    column.DataType = System.Type.GetType("System.String");
                    column.ColumnName = "Encargado";
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

                } break;

            //tabla historial de asignaciones encargado
            case 6:
                {
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

                } break;
        }

        return dt;
    }

    protected void headersCorrectosTablaReporte()
    {
        this.GridViewReporte.HeaderRow.BackColor = System.Drawing.Color.FromArgb(4562432);
        this.GridViewReporte.HeaderRow.ForeColor = System.Drawing.Color.White;
    }
}