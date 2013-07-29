using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Collections;
using System.IO;

public partial class Reportes : System.Web.UI.Page
{
	ControladoraReportes controladoraReportes;
	ControladoraBecarios controladoraBecarios;
	ControladoraEncargado controladoraEncargados;
	private static CommonServices commonService;
	private static int tipoReporte = -1;
	private static List<Object[]> lsObject = new List<Object[]>(); 
	static int indexEstado = 0;
	static int indexPeriodo = 0;
	static int indexAño = 0;
	static int indexUltimaAsignacion = 0;
	static int indexBecario = 0;
	static int indexEncargado = 0;
	private static List<EncargadoAtrasado> lsEncargadosAtrasados = new List<EncargadoAtrasado>();
	private static List<BecarioInactivo> lsBecariosInactivos = new List<BecarioInactivo>();
    private static List<string> lsCedulas = new List<string>();

    //EFECTO: Carga la pagina correctamente
    //REQUIERE: N/A
    //RETORNA: N/A
    protected void Page_Load(object sender, EventArgs e)
    {
        controladoraReportes = new ControladoraReportes();
        commonService = new CommonServices(UpdateInfo);
        controladoraBecarios = new ControladoraBecarios();
        controladoraEncargados = new ControladoraEncargado();
        MultiViewReportes.ActiveViewIndex = 0;
    }

    //EFECTO: Determina el ciclo de ejecución al seleccionar alguna de las entradas del menú de reportes
    //REQUIERE: Haber seleccionado alguna entrada del menú
    //RETORNA: N/A
    protected void MenuListaReportes_MenuItemClick(object sender, MenuEventArgs e)
    {
        lsObject.Clear();
	    btnPopUpGenerarPDF.Visible = false;
        btnGenerarPDFHistorialAsignacionesBecario.Visible = false;
        btnGenerarPDFHistorialAsignacionesEncargado.Visible = false;

        switch (e.Item.Text)
        {
            // REPORTE 1
            case "Con horas finalizadas":
                {
                    indexEstado = 0;
                    indexPeriodo = 0;
                    indexAño = 0;
                    indexUltimaAsignacion = 0;

                    tipoReporte = 1;
                    mostrarGrid();                   

                } break;

            // REPORTE 2
            case "No asignados":
                {
                    indexEstado = 0;
                    indexPeriodo = 0;
                    indexAño = 0;
                    indexUltimaAsignacion = 0;

                    tipoReporte = 2;
                    mostrarGrid();                  

                } break;

            // REPORTE 3
            case "Asignados por Unidad Académica":

                {
                    indexEstado = 0;
                    indexPeriodo = 0;
                    indexAño = 0;
                    indexUltimaAsignacion = 0;

                    tipoReporte = 3;
                    mostrarGrid();                                       

                } break;

            // REPORTE 4 Y 5
            case "Reporte de Actividad":
                {
                    // Reporte de Actividad de BECARIO
                    if (e.Item.ValuePath == "Becarios/Reporte de Actividad")
                    {
                        indexEstado = 0;
                        indexPeriodo = 0;
                        indexAño = 0;
                        indexUltimaAsignacion = 0;

                        tipoReporte = 4;
                        mostrarGrid(); 
                                               
                    }
                    // Reporte de Actividad de ENCARGADO
                    else
                    {
                        indexEstado = 0;
                        indexPeriodo = 0;
                        indexAño = 0;
                        indexUltimaAsignacion = 0;

                        tipoReporte = 5;
                        mostrarGrid();                        
                    }

                } break;

            // REPORTE 6
            case "Asignaciones de un Becario":
                {
                    indexBecario = 0;

                    tipoReporte = 6;
                    mostrarGrid(); 
                    
                } break;

            // REPORTE 7
            case "Asignaciones de un Encargado":
                {
                    indexEncargado = 0;

                    tipoReporte = 7;
                    mostrarGrid();

                } break;
        }
    }

    //EFECTO: LLena el grid de reportes segun el reporte que se haya seleccionado
    //REQUIERE: Haber seleccinado algun reporte del menú de reportes
    //RETORNA: N/A
    protected void llenarGridReportes(int caso)
    {
        DataTable tablaReporte;
        DataRow newRow;

        switch(caso){

            // Reporte 1
            case 1:
                {
                    tablaReporte = crearTabla(0);

                    // Ciclo para llenar tabla
                    if (lsObject.Count() == 0)
                    {
                        newRow = tablaReporte.NewRow();
                        newRow["Nombre"] = "-";
                        newRow["Carné"] = "-";
                        newRow["Correo"] = "-";
                        newRow["Horas Asignadas"] = "-";
                        newRow["Encargado"] = "-";
                        newRow["Correo Encargado"] = "-";

                        tablaReporte.Rows.InsertAt(newRow, 0);
                    }
                    else
                    {
                        for (int i = 0; i < lsObject.Count(); i++)
                        {
                            //Becario becario = lsObject[i][0];
                            newRow = tablaReporte.NewRow();
                            newRow["Nombre"] = lsObject[i][1].ToString() + " " + lsObject[i][2].ToString() + " " + lsObject[i][3].ToString();
                            newRow["Carné"] = lsObject[i][4].ToString();
                            newRow["Correo"] = lsObject[i][9].ToString();
                            newRow["Horas Asignadas"] = lsObject[i][19].ToString();
                            newRow["Encargado"] = lsObject[i][10].ToString() + " " + lsObject[i][11].ToString() + " " + lsObject[i][12].ToString();
                            newRow["Correo Encargado"] = lsObject[i][18].ToString();

                            tablaReporte.Rows.InsertAt(newRow, i);
                        }
                    }
                    this.GridViewReporte.DataSource = tablaReporte;
                    this.GridViewReporte.DataBind();

                } break;

            // Reporte 2
            case 2:
                {
                    tablaReporte = crearTabla(1);

                    if (lsObject.Count() == 0)
                    {
                        // Ciclo para llenar tabla
                        newRow = tablaReporte.NewRow();
                        newRow["Nombre"] = "-";
                        newRow["Carné"] = "-";
                        newRow["Correo"] = "-";
                        newRow["Teléfono"] = "-";
                        tablaReporte.Rows.InsertAt(newRow, 0);
                    }
                    else
                    {
                        for (int i = 0; i < lsObject.Count(); i++)
                        {
                            newRow = tablaReporte.NewRow();
                            newRow["Nombre"] = lsObject[i][1].ToString() + " " + lsObject[i][2].ToString() + " " + lsObject[i][3].ToString();
                            newRow["Carné"] = lsObject[i][4].ToString();
                            newRow["Correo"] = lsObject[i][9].ToString();
                            newRow["Teléfono"] = lsObject[i][7].ToString();

                            tablaReporte.Rows.InsertAt(newRow, i);
                        }
                    }

                    this.GridViewReporte.DataSource = tablaReporte;
                    this.GridViewReporte.DataBind();
                } break;
            // Reporte 3
            case 3:
                {
                    tablaReporte = crearTabla(2);

                    if (lsObject.Count() == 0)
                    {
                        newRow = tablaReporte.NewRow();
                        newRow = tablaReporte.NewRow();
                        newRow["Nombre"] = "-";
                        newRow["Carné"] = "-";
                        newRow["Correo"] = "-";
                        newRow["Horas Asignadas"] = "-";                        
                        newRow["Encargado"] = "-";
                        newRow["Correo Encargado"] = "-";
                        tablaReporte.Rows.InsertAt(newRow, 0);
                    }
                    else
                    {
                        for (int i = 0; i < lsObject.Count(); i++)
                        {                         

                            // Ciclo para llenar tabla
                            newRow = tablaReporte.NewRow();
                            newRow["Nombre"] = lsObject[i][1].ToString() + " " + lsObject[i][2].ToString() + " " + lsObject[i][3].ToString();
                            newRow["Carné"] = lsObject[i][4].ToString();
                            newRow["Correo"] = lsObject[i][9].ToString();
                            newRow["Horas Asignadas"] = lsObject[i][19].ToString();
                            newRow["Encargado"] = lsObject[i][10].ToString() + " " + lsObject[i][11].ToString() + " " + lsObject[i][12].ToString();
                            newRow["Correo Encargado"] = lsObject[i][18].ToString();

                            tablaReporte.Rows.InsertAt(newRow, i);
                        }
                    }

                    this.GridViewReporte.DataSource = tablaReporte;
                    this.GridViewReporte.DataBind();
                } break;
            // Reporte 4
            case 4:
                {
                    tablaReporte = crearTabla(3);


                    if (lsBecariosInactivos.Count == 0)
                    {
                        newRow = tablaReporte.NewRow();
                        newRow["Nombre"] = "-";
                        newRow["Carné"] = "-";
                        newRow["Horas Asignadas"] = "-";
                        newRow["Horas Completadas"] = "-";
                        newRow["Encargado"] = "-";
                        newRow["Fecha último reporte"] = "-";

                        tablaReporte.Rows.InsertAt(newRow, 0);
                    }
                    else
                    {
                        foreach (BecarioInactivo bi in lsBecariosInactivos)
                        {
                            newRow = tablaReporte.NewRow();

                            newRow["Nombre"] = bi.Nombre + " " + bi.Apellido1 + " " + bi.Apellido2;
                            newRow["Carné"] = bi.Cedula;
                            newRow["Horas Asignadas"] = bi.HorasAsignadas;
                            newRow["Horas Completadas"] = bi.HorasCompletadas;
                            newRow["Encargado"] = bi.Encargado;
                            newRow["Fecha último reporte"] = bi.FechaUltimoReporte;

                            tablaReporte.Rows.InsertAt(newRow, 0);
                        }
                    }

                    this.GridViewReporte.DataSource = tablaReporte;
                    this.GridViewReporte.DataBind();

                } break;
            // Reporte 5
            case 5:
                {
                    tablaReporte = crearTabla(4);

                    if (lsEncargadosAtrasados.Count == 0)
                    {
                        newRow = tablaReporte.NewRow();
                        newRow["Nombre"] = "-";
                        newRow["Cédula"] = "-";
                        newRow["Cnt. Becarios Asignados"] = "-";
                        newRow["Fecha de Última Actividad"] = "-";

                        tablaReporte.Rows.InsertAt(newRow, 0);
                    }
                    else
                    {
                        foreach (EncargadoAtrasado ea in lsEncargadosAtrasados)
                        {
                            newRow = tablaReporte.NewRow();
                            newRow["Nombre"] = ea.Nombre + " " + ea.Apellido1 + " " + ea.Apellido2;
                            newRow["Cédula"] = ea.Cedula;
                            newRow["Cnt. Becarios Asignados"] = ea.CantBecariosAsignados.ToString();
                            newRow["Fecha de Última Actividad"] = ea.FechaUltimaActividad;

                            tablaReporte.Rows.InsertAt(newRow, 0);
                        }
                    }

                    this.GridViewReporte.DataSource = tablaReporte;
                    this.GridViewReporte.DataBind();                    
                } break;
            // Reporte 6
            case 6:
                {
                    tablaReporte = crearTabla(5);

                    if (lsObject.Count == 0)
                    {
                        // Ciclo para llenar tabla
                        newRow = tablaReporte.NewRow();
                        newRow["Becario"] = "-";
                        newRow["Carné"] = "-";
                        newRow["Periodo"] = "-";
                        newRow["Año"] = "-";
                        newRow["Horas Asignadas"] = "-";
                        newRow["Encargado"] = "-";
                        newRow["Correo Encargado"] = "-";
                        newRow["Comentario del Encargado"] = "-";

                        tablaReporte.Rows.InsertAt(newRow, 0);
                    }
                    else 
                    {
                        for(int i=0; i<lsObject.Count();i++)
                        {
                            // Ciclo para llenar tabla
                            newRow = tablaReporte.NewRow();
                            newRow["Becario"] = lsObject[i][1].ToString() + " " + lsObject[i][2].ToString() + " " + lsObject[i][3].ToString();
                            newRow["Carné"] = lsObject[i][4].ToString();
                            newRow["Periodo"] = lsObject[i][20].ToString();
                            newRow["Año"] = lsObject[i][21].ToString();
                            newRow["Horas Asignadas"] = lsObject[i][19].ToString();
                            newRow["Encargado"] = lsObject[i][10].ToString() + " " + lsObject[i][11].ToString() + " " + lsObject[i][12].ToString(); 
                            newRow["Correo Encargado"] = lsObject[i][18].ToString();
                            newRow["Comentario del Encargado"] = lsObject[i][22].ToString(); 

                            tablaReporte.Rows.InsertAt(newRow, i); 
                        }
                    }
                    this.GridViewReporte.DataSource = tablaReporte;
                    this.GridViewReporte.DataBind();

                } break;
            // Reporte 7
            case 7:
                {
                    tablaReporte = crearTabla(6);

                    if (lsObject.Count == 0)
                    {
                        // Ciclo para llenar tabla
                        newRow = tablaReporte.NewRow();
                        newRow["Encargado"] = "-";
                        newRow["Cédula"] = "-";
                        newRow["Periodo"] = "-";
                        newRow["Año"] = "-";
                        newRow["Horas Asignadas"] = "-";
                        newRow["Becario"] = "-";
                        newRow["Carné del Becario"] = "-";
                        newRow["Comentario del Becario"] = "-";

                        tablaReporte.Rows.InsertAt(newRow, 0);
                    }
                    else
                    {
                        for (int i = 0; i < lsObject.Count(); i++)
                        {
                            // Ciclo para llenar tabla
                            newRow = tablaReporte.NewRow();
                            newRow["Encargado"] = lsObject[i][10].ToString() + " " + lsObject[i][11].ToString() + " " + lsObject[i][12].ToString();
                            newRow["Cédula"] = lsObject[i][14].ToString();
                            newRow["Periodo"] = lsObject[i][20].ToString();
                            newRow["Año"] = lsObject[i][21].ToString();
                            newRow["Horas Asignadas"] = lsObject[i][19].ToString();
                            newRow["Becario"] = lsObject[i][1].ToString() + " " + lsObject[i][2].ToString() + " " + lsObject[i][3].ToString();
                            newRow["Carné del Becario"] = lsObject[i][4].ToString();
                            newRow["Comentario del Becario"] = lsObject[i][22].ToString();

                            tablaReporte.Rows.InsertAt(newRow, i);
                        }
                    }

                    this.GridViewReporte.DataSource = tablaReporte;
                    this.GridViewReporte.DataBind();

                } break;
        }

        this.headersCorrectosTablaReporte();
    }

    //EFECTO: Crea la tabla correspondiente al reportes, que va a servir de fuente de datos para el grid
    //REQUIERE: Haber seleccionado algun reporte del menú de reportes
    //RETORNA: Un DataTable con las columnas correspondientes al reporte seleccionado
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
                    column.ColumnName = "Correo";
                    dt.Columns.Add(column);

                    column = new DataColumn();
                    column.DataType = System.Type.GetType("System.String");
                    column.ColumnName = "Horas Asignadas";
                    dt.Columns.Add(column);                              
                    
                    column = new DataColumn();
                    column.DataType = System.Type.GetType("System.String");
                    column.ColumnName = "Encargado";
                    dt.Columns.Add(column);

                    column = new DataColumn();
                    column.DataType = System.Type.GetType("System.String");
                    column.ColumnName = "Correo Encargado";
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
                    column.ColumnName = "Correo";
                    dt.Columns.Add(column);

                    column = new DataColumn();
                    column.DataType = System.Type.GetType("System.String");
                    column.ColumnName = "Horas Asignadas";
                    dt.Columns.Add(column);                   

                    column = new DataColumn();
                    column.DataType = System.Type.GetType("System.String");
                    column.ColumnName = "Encargado";
                    dt.Columns.Add(column);

                    column = new DataColumn();
                    column.DataType = System.Type.GetType("System.String");
                    column.ColumnName = "Correo Encargado";
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

                    column = new DataColumn();
                    column.DataType = System.Type.GetType("System.String");
                    column.ColumnName = "Fecha último reporte";
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
                    column.ColumnName = "Becario";
                    dt.Columns.Add(column);

                    column = new DataColumn();
                    column.DataType = System.Type.GetType("System.String");
                    column.ColumnName = "Carné";
                    dt.Columns.Add(column);

                    column = new DataColumn();
                    column.DataType = System.Type.GetType("System.String");
                    column.ColumnName = "Periodo";
                    dt.Columns.Add(column);

                    column = new DataColumn();
                    column.DataType = System.Type.GetType("System.String");
                    column.ColumnName = "Año";
                    dt.Columns.Add(column);

                    column = new DataColumn();
                    column.DataType = System.Type.GetType("System.String");
                    column.ColumnName = "Horas Asignadas";
                    dt.Columns.Add(column);

                    column = new DataColumn();
                    column.DataType = System.Type.GetType("System.String");
                    column.ColumnName = "Encargado";
                    dt.Columns.Add(column);

                    column = new DataColumn();
                    column.DataType = System.Type.GetType("System.String");
                    column.ColumnName = "Correo Encargado";
                    dt.Columns.Add(column);

                    column = new DataColumn();
                    column.DataType = System.Type.GetType("System.String");
                    column.ColumnName = "Comentario del Encargado";
                    dt.Columns.Add(column);

                } break;

            //tabla historial de asignaciones encargado
            case 6:
                {

                    column = new DataColumn();
                    column.DataType = System.Type.GetType("System.String");
                    column.ColumnName = "Encargado";
                    dt.Columns.Add(column);

                    column = new DataColumn();
                    column.DataType = System.Type.GetType("System.String");
                    column.ColumnName = "Cédula";
                    dt.Columns.Add(column);

                    column = new DataColumn();
                    column.DataType = System.Type.GetType("System.String");
                    column.ColumnName = "Periodo";
                    dt.Columns.Add(column);

                    column = new DataColumn();
                    column.DataType = System.Type.GetType("System.String");
                    column.ColumnName = "Año";
                    dt.Columns.Add(column);

                    column = new DataColumn();
                    column.DataType = System.Type.GetType("System.String");
                    column.ColumnName = "Horas Asignadas";
                    dt.Columns.Add(column);

                    column = new DataColumn();
                    column.DataType = System.Type.GetType("System.String");
                    column.ColumnName = "Becario";
                    dt.Columns.Add(column);

                    column = new DataColumn();
                    column.DataType = System.Type.GetType("System.String");
                    column.ColumnName = "Carné del Becario";
                    dt.Columns.Add(column);

                    column = new DataColumn();
                    column.DataType = System.Type.GetType("System.String");
                    column.ColumnName = "Comentario del Becario";
                    dt.Columns.Add(column);

                } break;
        }

        return dt;
    }

    //EFECTO: Establece el formaro correcto para el header del grid
    //REQUIERE: N/A
    //RETORNA: N/A
    protected void headersCorrectosTablaReporte()
    {
        this.GridViewReporte.HeaderRow.BackColor = System.Drawing.Color.FromArgb(4562432);
        this.GridViewReporte.HeaderRow.ForeColor = System.Drawing.Color.White;
    }

    //EFECTO: Establece la paginacion del grid, establecioendo un máximo de 15 filas por página
    //REQUIERE: N/A
    //RETORNA: N/A
    protected void GridReportes_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        this.GridViewReporte.PageIndex = e.NewPageIndex;
        this.GridViewReporte.DataBind();
        this.headersCorrectosTablaReporte();
        mostrarGrid();
    }

    //EFECTO: Define la información a mostrar al seleccionar un reporte del menú de reportes
    //REQUIERE: Haber seleccionado algun reporte del menú de reportes
    //RETORNA: N/A
    protected void mostrarGrid()
    {
        // Mostrar Grid:
        commonService.correrJavascript("$('#wrapperDeLaInfo').fadeIn();");

        this.DropDownListCriterio1.Items.Clear();
        this.DropDownListCriterio2.Items.Clear();
        this.DropDownListCriterio3.Items.Clear();
        this.DropDownListCriterio4.Items.Clear();
        this.DropDownListCriterio5.Items.Clear();

        switch (tipoReporte)
        {
            case 1:
                {
                    this.lblReporteActivo.Text = "Consultar Becarios que han finalizado sus horas";
                    this.lblCriterio1.Text = "Estado"; // Dejar el primero
                    this.lblCriterio2.Text = "Periodo a Consultar";
                    this.lblCriterio3.Text = "Año a Consultar";
                    commonService.correrJavascript("$('#criterio4').css('display', 'none');"); // Esconder el segundo criterio
                    commonService.correrJavascript("$('#criterio5').css('display', 'none');");
                    llenarGridReportes(1);

                    //llenar criterio de selección 1
                    Hashtable estado = new Hashtable();
                    estado.Add(0, "Pendiente de Horas");
                    estado.Add(1, "Horas Finalizadas");
                    estado.Add(2, "Seleccionar Estado");
                    this.DropDownListCriterio1.DataTextField = "Value";
                    this.DropDownListCriterio1.DataValueField = "Key";
                    this.DropDownListCriterio1.DataSource = estado;
                    this.DropDownListCriterio1.DataBind();
                    DropDownListCriterio1.SelectedIndex = indexEstado;

                    //llenar criterio de selección 2
                    Hashtable periodo = new Hashtable();
                    periodo.Add(0, "III - Periodo");
                    periodo.Add(1, "II  - Periodo");
                    periodo.Add(2, "I   - Periodo");
                    periodo.Add(3, "Seleccionar Periodo");
                    this.DropDownListCriterio2.DataTextField = "Value";
                    this.DropDownListCriterio2.DataValueField = "Key";
                    this.DropDownListCriterio2.DataSource = periodo;
                    this.DropDownListCriterio2.DataBind();
                    DropDownListCriterio2.SelectedIndex = indexPeriodo;
                    
                    //llenar criterio de selección 3
                    Hashtable año = new Hashtable();

                    DateTime today = DateTime.Today;                    
                    año.Add(0, (today.Year)+1); año.Add(1, today.Year);
                    año.Add(2, (today.Year) - 1); año.Add(3, (today.Year) - 2); año.Add(4, (today.Year) - 3);
                    año.Add(5, (today.Year) - 4); año.Add(6, (today.Year) - 5); año.Add(7, (today.Year) - 6);
                    año.Add(8, "Seleccionar Año");
                    this.DropDownListCriterio3.DataTextField = "Value";
                    this.DropDownListCriterio3.DataValueField = "Key";
                    this.DropDownListCriterio3.DataSource = año;
                    this.DropDownListCriterio3.DataBind();
                    DropDownListCriterio3.SelectedIndex = indexAño;
                } break;
            case 2:
                {
                    this.lblReporteActivo.Text = "Reporte de becarios no asignados en un Semestre y Año, que si fueron asignados en el semestre anterior o tras-anterior";
                    this.lblCriterio1.Text = "Periodo a Consultar";
                    this.lblCriterio2.Text = "Año a Consultar";
                    this.lblCriterio3.Text = "Última Asignación";
                    commonService.correrJavascript("$('#criterio4').css('display', 'none');");
                    commonService.correrJavascript("$('#criterio5').css('display', 'none');");
                    llenarGridReportes(2);


                    //llenar criterio de selección 1
                    Hashtable periodo = new Hashtable();
                    periodo.Add(0, "III - Periodo");
                    periodo.Add(1, "II  - Periodo");
                    periodo.Add(2, "I   - Periodo");
                    periodo.Add(3, "Seleccionar Periodo");
                    this.DropDownListCriterio1.DataTextField = "Value";
                    this.DropDownListCriterio1.DataValueField = "Key";
                    this.DropDownListCriterio1.DataSource = periodo;
                    this.DropDownListCriterio1.DataBind();
                    DropDownListCriterio1.SelectedIndex = indexPeriodo;

                    //llenar criterio de selección 2
                    Hashtable año = new Hashtable();

                    DateTime today = DateTime.Today;
                    año.Add(0, (today.Year) + 1); año.Add(1, today.Year);
                    año.Add(2, (today.Year) - 1); año.Add(3, (today.Year) - 2); año.Add(4, (today.Year) - 3);
                    año.Add(5, (today.Year) - 4); año.Add(6, (today.Year) - 5); año.Add(7, (today.Year) - 6);
                    año.Add(8, "Seleccionar Año");
                    this.DropDownListCriterio2.DataTextField = "Value";
                    this.DropDownListCriterio2.DataValueField = "Key";
                    this.DropDownListCriterio2.DataSource = año;
                    this.DropDownListCriterio2.DataBind();
                    DropDownListCriterio2.SelectedIndex = indexAño;

                    //llenar criterio de selección 3
                    Hashtable ultimaAsignación = new Hashtable();
                    ultimaAsignación.Add(0, "Hace 3 periodos");
                    ultimaAsignación.Add(1, "Hace 2 periodos");
                    ultimaAsignación.Add(2, "Periodo anterior");
                    ultimaAsignación.Add(3, "Becarios nunca Asignados");
                    ultimaAsignación.Add(4, "Seleccionar Lapso");
                    this.DropDownListCriterio3.DataTextField = "Value";
                    this.DropDownListCriterio3.DataValueField = "Key";
                    this.DropDownListCriterio3.DataSource = ultimaAsignación;
                    this.DropDownListCriterio3.DataBind();
                    DropDownListCriterio3.SelectedIndex = indexUltimaAsignacion;

                } break;
            case 3:
                {
                    this.lblReporteActivo.Text = "Reporte de Estudiantes Asignados, por Unidad Académica";
                    this.lblCriterio1.Text = "Periodo a Consultar";
                    this.lblCriterio2.Text = "Año a Consultar";
                    this.lblCriterio3.Text = "Unidad Académica";
                    commonService.correrJavascript("$('#criterio4').css('display', 'none');");
                    commonService.correrJavascript("$('#criterio5').css('display', 'none');");
                    llenarGridReportes(3);

                    //llenar criterio de selección 1
                    Hashtable periodo = new Hashtable();
                    periodo.Add(0, "III - Periodo");
                    periodo.Add(1, "II  - Periodo");
                    periodo.Add(2, "I   - Periodo");
                    periodo.Add(3, "Seleccionar Periodo");
                    this.DropDownListCriterio1.DataTextField = "Value";
                    this.DropDownListCriterio1.DataValueField = "Key";
                    this.DropDownListCriterio1.DataSource = periodo;
                    this.DropDownListCriterio1.DataBind();
                    DropDownListCriterio1.SelectedIndex = indexPeriodo;

                    //llenar criterio de selección 2
                    Hashtable año = new Hashtable();

                    DateTime today = DateTime.Today;
                    año.Add(0, (today.Year) + 1); año.Add(1, today.Year);
                    año.Add(2, (today.Year) - 1); año.Add(3, (today.Year) - 2); año.Add(4, (today.Year) - 3);
                    año.Add(5, (today.Year) - 4); año.Add(6, (today.Year) - 5); año.Add(7, (today.Year) - 6);
                    año.Add(8, "Seleccionar Año");
                    this.DropDownListCriterio2.DataTextField = "Value";
                    this.DropDownListCriterio2.DataValueField = "Key";
                    this.DropDownListCriterio2.DataSource = año;
                    this.DropDownListCriterio2.DataBind();
                    DropDownListCriterio2.SelectedIndex = indexAño;

                    //llenar criterio de selección 3
                    Hashtable ultimaAsignación = new Hashtable();
                    List<string> lsSiglasUA = controladoraReportes.optenerUnidadesAcademicas();
                    int i;
                    for(i=0;i<lsSiglasUA.Count();i++)
                    {
                    ultimaAsignación.Add(i, lsSiglasUA[i]);
                    }
                    ultimaAsignación.Add(i, "Seleccionar UA");
                    this.DropDownListCriterio3.DataTextField = "Value";
                    this.DropDownListCriterio3.DataValueField = "Key";
                    this.DropDownListCriterio3.DataSource = ultimaAsignación;
                    this.DropDownListCriterio3.DataBind();
                    DropDownListCriterio3.SelectedIndex = indexUltimaAsignacion;
                } break;
            case 4:
                {
                    this.lblReporteActivo.Text = "Reporte de estudiantes que no han reportado horas en un lapso, con fecha de último reporte";
                    this.lblCriterio1.Text = "Periodo a Consultar"; // Dejar el primero
                    this.lblCriterio2.Text = "Año a Consultar";
                    this.lblCriterio3.Text = "Fecha Último Reporte";
                    commonService.correrJavascript("$('#criterio4').css('display', 'none');"); // Esconder el segundo criterio
                    commonService.correrJavascript("$('#criterio5').css('display', 'none');");
                    llenarGridReportes(4);

                    //llenar criterio de selección 1
                    Hashtable semestre = new Hashtable();
                    semestre.Add(0, "III - Periodo");
                    semestre.Add(1, "II  - Periodo");
                    semestre.Add(2, "I   - Periodo");
                    semestre.Add(3, "Seleccionar Periodo");
                    this.DropDownListCriterio1.DataTextField = "Value";
                    this.DropDownListCriterio1.DataValueField = "Key";
                    this.DropDownListCriterio1.DataSource = semestre;
                    this.DropDownListCriterio1.DataBind();
                    this.DropDownListCriterio1.SelectedIndex = indexPeriodo;

                    //llenar criterio de selección 2
                    Hashtable año = new Hashtable();
                    DateTime hoy = DateTime.Now;
                    año.Add(7, "Seleccionar Año");
                    año.Add(6, (hoy.Year + 1).ToString());
                    año.Add(5, hoy.Year.ToString());
                    año.Add(4, (hoy.Year - 1).ToString());
                    año.Add(3, (hoy.Year - 2).ToString());
                    año.Add(2, (hoy.Year - 3).ToString());
                    año.Add(1, (hoy.Year - 4).ToString());
                    año.Add(0, (hoy.Year - 5).ToString());
                    this.DropDownListCriterio2.DataTextField = "Value";
                    this.DropDownListCriterio2.DataValueField = "Key";
                    this.DropDownListCriterio2.DataSource = año;
                    this.DropDownListCriterio2.DataBind();
                    DropDownListCriterio2.SelectedIndex = indexAño;

                    //llenar criterio de selección 3
                    Hashtable ultimaAsigancion = new Hashtable();                    
                    ultimaAsigancion.Add(0, "Más de dos meses");
                    ultimaAsigancion.Add(1, "Más de un mes");
                    ultimaAsigancion.Add(2, "Más de dos semanas");
                    ultimaAsigancion.Add(3, "Más de una semana");
                    ultimaAsigancion.Add(4, "Seleccionar Lapso");


                    this.DropDownListCriterio3.DataTextField = "Value";
                    this.DropDownListCriterio3.DataValueField = "Key";
                    this.DropDownListCriterio3.DataSource = ultimaAsigancion;
                    this.DropDownListCriterio3.DataBind();
                    DropDownListCriterio3.SelectedIndex = indexUltimaAsignacion;
                } break;
            case 5:
                {
                    this.lblReporteActivo.Text = "Reporte de encargados con aprobaciones pendientes con más de un mes de atraso";
                    this.lblCriterio1.Text = "Periodo a Consultar"; // Dejar el primero
                    this.lblCriterio2.Text = "Año a Consultar";
                    this.lblCriterio3.Text = "Fecha Último Reporte";
                    commonService.correrJavascript("$('#criterio4').css('display', 'none');"); // Esconder el segundo criterio
                    commonService.correrJavascript("$('#criterio5').css('display', 'none');");
                    llenarGridReportes(5);

                    //llenar criterio de selección 1
                    Hashtable periodo = new Hashtable();
                    periodo.Add(0, "III - Periodo");
                    periodo.Add(1, "II  - Periodo");
                    periodo.Add(2, "I   - Periodo");
                    periodo.Add(3, "Seleccionar Periodo");
                    this.DropDownListCriterio1.DataTextField = "Value";
                    this.DropDownListCriterio1.DataValueField = "Key";
                    this.DropDownListCriterio1.DataSource = periodo;
                    this.DropDownListCriterio1.DataBind();
                    this.DropDownListCriterio1.SelectedIndex = indexPeriodo;
                                        					                   
					//llenar criterio de selección 2
					Hashtable año = new Hashtable();
					DateTime hoy = DateTime.Now;
                    año.Add(7, "Seleccionar Año");
                    año.Add(6, (hoy.Year + 1).ToString());
                    año.Add(5, hoy.Year.ToString());
					año.Add(4, (hoy.Year - 1).ToString());
					año.Add(3, (hoy.Year - 2).ToString());
					año.Add(2, (hoy.Year - 3).ToString());
					año.Add(1, (hoy.Year - 4).ToString());
					año.Add(0, (hoy.Year - 5).ToString());
					this.DropDownListCriterio2.DataTextField = "Value";
					this.DropDownListCriterio2.DataValueField = "Key";
					this.DropDownListCriterio2.DataSource = año;
					this.DropDownListCriterio2.DataBind();
                    DropDownListCriterio2.SelectedIndex = indexAño;

					//llenar criterio de selección 3
					Hashtable fechaUltimoReporte = new Hashtable();
                    
                    fechaUltimoReporte.Add(4, "Seleccionar Lapso");
					fechaUltimoReporte.Add(3, "Más de una semana");
					fechaUltimoReporte.Add(2, "Más de dos semanas");
					fechaUltimoReporte.Add(1, "Más de un mes");
					fechaUltimoReporte.Add(0, "Más de dos meses");
                    
					
					this.DropDownListCriterio3.DataTextField = "Value";
					this.DropDownListCriterio3.DataValueField = "Key";
					this.DropDownListCriterio3.DataSource = fechaUltimoReporte;
					this.DropDownListCriterio3.DataBind();
					DropDownListCriterio3.SelectedIndex = indexUltimaAsignacion;                              
                } break;
            case 6:
                {
                    this.lblReporteActivo.Text = "Reporte de Historial de Asignaciones de un Becario";
                    this.lblCriterio1.Text = "Seleccione un Becario";
                    commonService.correrJavascript("$('#criterio2').css('display', 'none');");
                    commonService.correrJavascript("$('#criterio3').css('display', 'none');");
                    commonService.correrJavascript("$('#criterio4').css('display', 'none');");
                    commonService.correrJavascript("$('#criterio5').css('display', 'none');");
                    llenarGridReportes(6);

                    List<string> lsCedulasAux = new List<string>();

                    List<Becario> lsBecarios = controladoraBecarios.consultarTablaBecario();
                    lsCedulasAux.Clear();
                    Hashtable becario = new Hashtable();
                    becario.Add(lsBecarios.Count(), "Seleccionar Becario");
                    lsCedulasAux.Add(lsBecarios.Count().ToString());
                    int j = lsBecarios.Count()-1;
                    for (int i = 0; i < lsBecarios.Count(); i++)
                    {
                        becario.Add(j, lsBecarios[i].nombre + " " + lsBecarios[i].apellido1 + " " + lsBecarios[i].apellido2);
                        lsCedulasAux.Add(lsBecarios[i].cedula);
                        j--;
                    }
                    lsCedulas = lsCedulasAux;

                    this.DropDownListCriterio1.DataTextField = "Value";
                    this.DropDownListCriterio1.DataValueField = "Key";
                    this.DropDownListCriterio1.DataSource = becario;
                    this.DropDownListCriterio1.DataBind();
                    DropDownListCriterio1.SelectedIndex = indexBecario; 
                } break;
            case 7:
                {
                    this.lblReporteActivo.Text = "Reporte de Historial de Asignaciones que hace un Encargado";
                    this.lblCriterio1.Text = "Seleccione Encargado";
                    commonService.correrJavascript("$('#criterio2').css('display', 'none');");
                    commonService.correrJavascript("$('#criterio3').css('display', 'none');");
                    commonService.correrJavascript("$('#criterio4').css('display', 'none');");
                    commonService.correrJavascript("$('#criterio5').css('display', 'none');");
                    llenarGridReportes(7);

                    List<string> lsCedulasAux = new List<string>();

                    List<Encargado> lsEncargados = controladoraEncargados.consultarTablaEncargados();
                    lsCedulasAux.Clear();
                    Hashtable encargdo = new Hashtable();
                    encargdo.Add(lsEncargados.Count(), "Seleccionar Encargado");
                    lsCedulasAux.Add(lsEncargados.Count().ToString());
                    int j = lsEncargados.Count() - 1;
                    for (int i = 0; i < lsEncargados.Count(); i++)
                    {
                        encargdo.Add(j, lsEncargados[i].Nombre + " " + lsEncargados[i].Apellido1 + " " + lsEncargados[i].Apellido2);
                        lsCedulasAux.Add(lsEncargados[i].Cedula);
                        j--;
                    }
                    lsCedulas = lsCedulasAux;
                    

                    this.DropDownListCriterio1.DataTextField = "Value";
                    this.DropDownListCriterio1.DataValueField = "Key";
                    this.DropDownListCriterio1.DataSource = encargdo;
                    this.DropDownListCriterio1.DataBind();
                    DropDownListCriterio1.SelectedIndex = indexEncargado; 
                } break;
        }

    }

    //EFECTO: Ciclo de ejecución al presionar el botón buscar
    //REQUIERE: Haber seleccionado algun reporte del menú de reportes
    //RETORNA: N/A
    protected void btnBuscar_Click(object sender, EventArgs e) 
    {
        switch (tipoReporte)
        {
            case 1:
                {
                    indexEstado = this.DropDownListCriterio1.SelectedIndex;
                    indexPeriodo = this.DropDownListCriterio2.SelectedIndex;
                    indexAño = this.DropDownListCriterio3.SelectedIndex;

                    string criterioDeBusqueda = "%" + this.txtBuscarGeneral.Text + "%";
                    string estado = this.DropDownListCriterio1.SelectedItem.ToString();
                    string periodo = this.DropDownListCriterio2.SelectedItem.ToString();
                    string año = this.DropDownListCriterio3.SelectedItem.ToString();                    

                    lsObject = controladoraReportes.reporteBecarios(tipoReporte, criterioDeBusqueda, estado, periodo, año, null, null);

                    mostrarGrid();
                    if (this.GridViewReporte.Rows[0].Cells[0].Text != "-" && Convert.ToInt32(this.DropDownListCriterio1.SelectedIndex) == 1 && Convert.ToInt32(this.DropDownListCriterio2.SelectedIndex) != 0 && Convert.ToInt32(this.DropDownListCriterio3.SelectedIndex) != 0)
                    {
                        btnPopUpGenerarPDF.Visible = false;
                    }
                    else 
                    {
                        btnPopUpGenerarPDF.Visible = false;
                    }

                } break;
            case 2:
                {
                    indexPeriodo = this.DropDownListCriterio1.SelectedIndex;
                    indexAño = this.DropDownListCriterio2.SelectedIndex;
                    indexUltimaAsignacion = this.DropDownListCriterio3.SelectedIndex;

                    if (this.DropDownListCriterio1.SelectedIndex != 0 && this.DropDownListCriterio2.SelectedIndex == 0)
                    {
                        indexAño = 7;
                        this.DropDownListCriterio2.SelectedIndex = 7;
                    }
                    if (this.DropDownListCriterio3.SelectedIndex == 1)
                    {
                        indexPeriodo = 0;
                        this.DropDownListCriterio1.SelectedIndex = 0;
                        indexAño = 0;
                        this.DropDownListCriterio2.SelectedIndex = 0;
                    }
                    else 
                    {
                        if (this.DropDownListCriterio3.SelectedIndex != 1 && this.DropDownListCriterio3.SelectedIndex != 0)
                        {
                            if (this.DropDownListCriterio1.SelectedIndex == 0 && this.DropDownListCriterio2.SelectedIndex == 0)
                            {
                                indexPeriodo = 1;
                                this.DropDownListCriterio1.SelectedIndex = 1;
                                indexAño = 7;
                                this.DropDownListCriterio2.SelectedIndex = 7;
                            }
                            else
                            {
                                if (this.DropDownListCriterio2.SelectedIndex == 0)
                                {
                                    indexAño = 7;
                                    this.DropDownListCriterio2.SelectedIndex = 7;
                                }
                            }
                        }
                    }

                    string criterioDeBusqueda = "%" + this.txtBuscarGeneral.Text + "%";
                    string periodo = this.DropDownListCriterio1.SelectedItem.ToString();
                    string año = this.DropDownListCriterio2.SelectedItem.ToString();
                    string ultimaAsignacion = this.DropDownListCriterio3.SelectedItem.ToString();

                    lsObject = controladoraReportes.reporteBecarios(tipoReporte, criterioDeBusqueda, periodo, año, ultimaAsignacion, null, null);

                    mostrarGrid();
                } break;
            case 3:
                {
                    indexPeriodo = this.DropDownListCriterio1.SelectedIndex;
                    indexAño = this.DropDownListCriterio2.SelectedIndex;
                    indexUltimaAsignacion = this.DropDownListCriterio3.SelectedIndex;

                    string criterioDeBusqueda = "%" + this.txtBuscarGeneral.Text + "%";
                    string periodo = this.DropDownListCriterio1.SelectedItem.ToString();
                    string año = this.DropDownListCriterio2.SelectedItem.ToString();
                    string unidadAcademica = this.DropDownListCriterio3.SelectedItem.ToString();

                    lsObject = controladoraReportes.reporteBecarios(tipoReporte, criterioDeBusqueda, periodo, año, unidadAcademica, null, null);
                     
                    mostrarGrid();
                } break;
            case 4:
                {
                    indexPeriodo = this.DropDownListCriterio1.SelectedIndex;
                    indexAño = this.DropDownListCriterio2.SelectedIndex;
                    indexUltimaAsignacion = this.DropDownListCriterio3.SelectedIndex;

                    string criterioBusqueda = "%" + this.txtBuscarGeneral.Text + "%";
                    string periodo = this.DropDownListCriterio1.SelectedItem.ToString();
                    int año = 0;
                    if (String.Compare(this.DropDownListCriterio2.SelectedItem.ToString(), "Seleccionar Año") != 0)
                    {
                        año = Convert.ToInt32(this.DropDownListCriterio2.SelectedItem.ToString());
                    }
                    DateTime fechaUltimoReporte = DateTime.Now;
                    switch (this.DropDownListCriterio3.SelectedItem.ToString())
                    {
                        case "Más de una semana":
                            fechaUltimoReporte -= (new TimeSpan(7, 0, 0, 0));
                            break;
                        case "Más de dos semanas":
                            fechaUltimoReporte -= (new TimeSpan(14, 0, 0, 0));
                            break;
                        case "Más de un mes":
                            fechaUltimoReporte -= (new TimeSpan(31, 0, 0, 0));
                            break;
                        case "Más de dos meses":
                            fechaUltimoReporte -= (new TimeSpan(62, 0, 0, 0));
                            break;
                        case "Seleccionar Lapso":
                            fechaUltimoReporte += (new TimeSpan(62, 1, 1, 1));
                            break;                            
                    }

                    lsBecariosInactivos = controladoraReportes.llenarBecariosInactivos(criterioBusqueda, año, periodo, fechaUltimoReporte);

                    mostrarGrid();
                } break;
            case 5:
                {
                    indexPeriodo = this.DropDownListCriterio1.SelectedIndex;
                    indexAño = this.DropDownListCriterio2.SelectedIndex;
                    indexUltimaAsignacion = this.DropDownListCriterio3.SelectedIndex;

                    int año;
                    if (Convert.ToInt32(this.DropDownListCriterio2.SelectedIndex) == 0)
                    {
                        año = 0;
                    }
                    else
                    {
                        año = Convert.ToInt32(this.DropDownListCriterio2.SelectedItem.ToString());
                    }

                    string criterioBusqueda = "%" + this.txtBuscarGeneral.Text + "%";
                    string semestre = this.DropDownListCriterio1.SelectedItem.ToString();                     

                    DateTime fechaUltimoReporte = DateTime.Now;
                    switch (this.DropDownListCriterio3.SelectedItem.ToString())
                    {
                        case "Más de una semana":
                            fechaUltimoReporte -= (new TimeSpan(7, 0, 0, 0));
                            break;
                        case "Más de dos semanas":
                            fechaUltimoReporte -= (new TimeSpan(14, 0, 0, 0));
                            break;
                        case "Más de un mes":
                            fechaUltimoReporte -= (new TimeSpan(31, 0, 0, 0));
                            break;
                        case "Más de dos meses":
                            fechaUltimoReporte -= (new TimeSpan(62, 0, 0, 0));
                            break;
                        case "Seleccionar Lapso":
                            fechaUltimoReporte += (new TimeSpan(62, 1, 1, 1));
                            break;
                    }                    
                    lsEncargadosAtrasados = controladoraReportes.llenarEncargadosAtrasados(criterioBusqueda, año, semestre, fechaUltimoReporte);

                    mostrarGrid();
                } break;
            case 6:
                {
                    indexBecario = this.DropDownListCriterio1.SelectedIndex;

                    string criterioBusqueda = "%" + this.txtBuscarGeneral.Text + "%";
                    int index = Convert.ToInt32(this.DropDownListCriterio1.SelectedValue);
                    string cedulaBecario = lsCedulas[(Convert.ToInt32(lsCedulas[0]) - index)];

                    if (Convert.ToInt32(this.DropDownListCriterio1.SelectedValue) != Convert.ToInt32(lsCedulas[0]))
                    {
                        lsObject = controladoraReportes.reportarHistorialDeAsignacionesBecario(criterioBusqueda, cedulaBecario, 0);
                    }
                    else 
                    {
                        lsObject = controladoraReportes.reportarHistorialDeAsignacionesBecario(criterioBusqueda, cedulaBecario, 1);
                    }
                    mostrarGrid();

                    if (this.GridViewReporte.Rows[0].Cells[0].Text != "-")
                    {
                        this.btnGenerarPDFHistorialAsignacionesBecario.Visible = false;
                    }
                    else
                    {
                        this.btnGenerarPDFHistorialAsignacionesBecario.Visible = false;
                    }

                } break;
            case 7:
                {
                    indexEncargado = this.DropDownListCriterio1.SelectedIndex;

                    string criterioBusqueda = "%" + this.txtBuscarGeneral.Text + "%";
                    int index = Convert.ToInt32(this.DropDownListCriterio1.SelectedValue);
                    string cedulaEncargado = lsCedulas[(Convert.ToInt32(lsCedulas[0]) - index)];

                    if (Convert.ToInt32(this.DropDownListCriterio1.SelectedValue) != Convert.ToInt32(lsCedulas[0]))
                    {
                        lsObject = controladoraReportes.reportarHistorialDeAsignacionesEncargado(criterioBusqueda, cedulaEncargado, 0);
                    }
                    else 
                    {
                        lsObject = controladoraReportes.reportarHistorialDeAsignacionesEncargado(criterioBusqueda, cedulaEncargado, 1);
                    }
					
					mostrarGrid();

                    if (this.GridViewReporte.Rows[0].Cells[0].Text != "-")
                    {
                        this.btnGenerarPDFHistorialAsignacionesEncargado.Visible = false;
                    }
                    else
                    {
                        this.btnGenerarPDFHistorialAsignacionesEncargado.Visible = false;
                    }
                    
				} break;
		}
	}

    //EFECTO: Ciclo de ejecución al presionar el botón btnPopUpGenerarPDF. Abre un popUp para completar los datos para generar el pdf
    //REQUIERE: N/A
    //RETORNA: N/A
	protected void btnPopUpGenerarPDF_Click(object sender, EventArgs e)
	{
		if (inicializarCamposPopUpPDF())
			commonService.abrirPopUp("popUpPDF", "Generar PDF");
		else
			commonService.mensajeJavascript("No hay becarios que hayan completado horas para el año y período elegidos", "Atención"); //Reportar que no hay becarios para actualizar
	}
    //EFECTO: Inicializa los campos del popUp para completar los datos para generar el pdf
    //REQUIERE: N/A
    //RETORNA: N/A
	protected bool inicializarCamposPopUpPDF()
	{
		switch (DropDownListCriterio2.SelectedValue)
		{
			case "0":
				lblPeriodo.Text = "3";
				break;
			case "1":
				lblPeriodo.Text = "2";
				break;
			case "2":
				lblPeriodo.Text = "1";
				break;
		}
		List<ListItem> listaCantidadHoras = controladoraReportes.obtenerCantidadesDeHorasCompletadas(lblPeriodo.Text, DropDownListCriterio3.SelectedItem.Text);
		if (listaCantidadHoras.Count > 0)
		{
			lblAño.Text = DropDownListCriterio3.SelectedItem.Text;
            txtDestinatario.Text = "Lic. Andrey Fallas Alvarado";
            txtRemitente.Text = "Licda. Milena Zúñiga Cárdenas";
			txtIniciales.Text = "";
			ddlCantHoras.DataSource = listaCantidadHoras;
			ddlCantHoras.DataTextField = "Text";
			ddlCantHoras.DataValueField = "Value";
			ddlCantHoras.DataBind();
			return true;
		}
		else
			return false;
	}

    //EFECTO: Ciclo de ejecución al presionar el botón  Acaptar enerar el pdf del reporte 
    //REQUIERE: N/A
    //RETORNA: N/A
    protected void btnGenerarPDF_Click(object sender, EventArgs e)
    {
		int? tipo = null;
		string cedula = "nada";

		switch (((Button)sender).ID)
		{
			case "btnInvisGenerarPDF":
				tipo = 1;
                commonService.correrJavascript("$('#popUpPDF').dialog('close');");
                mostrarGrid();
				break;
			case "btnGenerarPDFHistorialAsignacionesBecario":
				{
					tipo = 2;
                    if (this.DropDownListCriterio1.SelectedIndex == lsCedulas.Count)
                    {
                        int index = Convert.ToInt32(this.DropDownListCriterio1.SelectedValue);
                        cedula = lsCedulas[(Convert.ToInt32(lsCedulas[0]) - index)];
                    }
                    mostrarGrid();
				}
				break;
			case "btnGenerarPDFHistorialAsignacionesEncargado":
				{
					tipo = 3;
                    if (this.DropDownListCriterio1.SelectedIndex == lsCedulas.Count)
                    {
                        int index = Convert.ToInt32(this.DropDownListCriterio1.SelectedValue);
                        cedula = lsCedulas[(Convert.ToInt32(lsCedulas[0]) - index)];
                    }
                    mostrarGrid();
				}
				break;
		}
		if (tipo.HasValue)
		{
			commonService.correrJavascript("IniciarSolicitud(" + tipo.Value + ", \'%" + this.txtBuscarGeneral.Text + "%\', \'" + cedula +"\')");
		}
		else
			throw new Exception("Se recibió un tipo de reporte inválido");
    }
}

