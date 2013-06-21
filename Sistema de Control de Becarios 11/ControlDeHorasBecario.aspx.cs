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
    private DataTable infoActual;//CedulaBecario, CedulaEncargado, CantidadHoras, Fecha, Estado, CuerpoComentarioBecario, CuerpoComentarioEncargado
    private ControladoraControlBecario cb;
    public static int modo = 0;
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
        modo = 0;
        commonService.abrirPopUp("PopUpCtrlBecario", "Nuevo Reporte de Horas");
        commonService.correrJavascript("$('#comentarioDeEncargado').hide();");
        commonService.correrJavascript("$('.dateText').datepicker({ dateFormat: 'dd-MM-yy' });");
    }

    // Selecciona tupla del grid
    protected void gridControlHorasBecario_RowCommand(object sender, GridViewCommandEventArgs e)
    {

        switch (e.CommandName)
        {
            // Abrir Pop Up aceptar/rechazar horas
            case "btnSeleccionarTupla_Click":
                {
                    modo = 1;
                    commonService.abrirPopUp("PopUpCtrlBecario", "Nuevo Reporte de Horas");
                    commonService.correrJavascript("$('#comentarioDeEncargado').show();");
                    this.txtCantidadHoras.Text = infoActual.Rows[gridControlHorasBecario.SelectedIndex].ItemArray[3].ToString();
                    this.txtComentario.Text = infoActual.Rows[gridControlHorasBecario.SelectedIndex].ItemArray[5].ToString();
                    this.txtComentarioEncargado.Text = infoActual.Rows[gridControlHorasBecario.SelectedIndex].ItemArray[6].ToString();
                    this.txtComentarioEncargado.Enabled = false;
                } break;

        }
    }

    protected void gridControlHorasBecario_SelectedIndexChanging(object sender, EventArgs e)
    {

    }

    protected void gridControlHorasBecario_PageIndexChanging(object sender, EventArgs e)
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
        infoActual = cb.horasReportadas(Session["Cedula"].ToString(), this.cb.getCedulaEncargado(Session["Cedula"].ToString()));
        DataRow newRow;
        if (infoActual.Rows.Count > 0)
        {
            foreach (DataRow r in infoActual.Rows)
            {
                newRow = tablaHorasReportadas.NewRow();
                newRow["Fecha"] = r[3].ToString();
                newRow["Cantidad Hora"] = r[2].ToString();
                newRow["Estado"] = r[4].ToString();
                tablaHorasReportadas.Rows.Add(newRow);
            }

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
        try
        {//para la primera ves que se inicie si no hay horas reportadas entonces no crea la tabla
            this.gridControlHorasBecario.HeaderRow.BackColor = System.Drawing.Color.FromArgb(4562432);
            this.gridControlHorasBecario.HeaderRow.ForeColor = System.Drawing.Color.White;
        }
        catch (Exception ex)
        {

        }
    }

    //cuando se ha aceptar para las horas que inserta el usuario
    protected void btnInvisibleEnviarReporte_Click(object sender, EventArgs e)
    {
        if (modo == 0)
        {//insercion de reporte
            insertarReporte();
        }
        else { //modificacion de reporte
            modificarReporte();
        }
    }

    public void insertarReporte() {
        Object[] datos = new Object[7];
        datos[0] = this.cb.getCedulaEncargado(Session["Cedula"].ToString());//recupera la cedula del encargado para el becario logueado en el sistema
        datos[1] = Session["Cedula"].ToString();//el becario que esta logueado
        datos[2] = 0;//0 pendiente, 1 rechazada y 2 aceptada
        datos[3] = Convert.ToInt32(this.txtCantidadHoras.Text);//horas digitadas por el usuario
        datos[4] = DateTime.Now;//fecha actual del sistema
        datos[5] = "";
        datos[6] = this.txtComentario.Text;//comentario del becario
        String resultado = "";
        resultado = this.cb.enviarReporte(datos);
        commonService.mensajeJavascript(resultado, "Reporte de Horas");
    }

    public void modificarReporte() {
        Object[] datos = new Object[7];
        datos[0] = this.cb.getCedulaEncargado(Session["Cedula"].ToString());//recupera la cedula del encargado para el becario logueado en el sistema
        datos[1] = Session["Cedula"].ToString();//el becario que esta logueado
        datos[2] = 0;//0 pendiente, 1 rechazada y 2 aceptada
        datos[3] = Convert.ToInt32(this.txtCantidadHoras.Text);//horas digitadas por el usuario
        datos[4] = infoActual.Rows[gridControlHorasBecario.SelectedIndex].ItemArray[3].ToString();
        datos[5] = this.txtComentarioEncargado.Text;
        datos[6] = this.txtComentario.Text;//comentario del becario
        int resultado;
        resultado = this.cb.modificarReporte(datos);
        if (resultado == 1) commonService.mensajeJavascript("Modificacion Exitosa", "Reporte de Horas");
        else  commonService.mensajeJavascript("Modificacion Fallida", "Reporte de Horas"); 
        
    }

    protected void gridControlHorasBecario_SelectedIndexChanging(object sender, GridViewSelectEventArgs e)
    {

    }
}