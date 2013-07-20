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
    private ControladoraAsignaciones ca;
    public static int modo = 0;
    public static int indice = 0;


    protected void Page_Load(object sender, EventArgs e)
    {
        cb = new ControladoraControlBecario();
        //recupero el estado de dicha asignacion
        String encargado = this.cb.getCedulaEncargado(Session["Cedula"].ToString(),Convert.ToInt32(Session["Periodo"].ToString()));
        int estado = cb.getEstado(Session["Cedula"].ToString(),encargado,Convert.ToInt32(Session["Periodo"].ToString()));
        String comentario = cb.getComentarioBecarioFinal(Session["Cedula"].ToString(), encargado);

        if (estado == 1)
        {//asignacion aceptada
            this.MultiViewBecario.ActiveViewIndex = 1;
            commonService = new CommonServices(UpdateInfo);
            ca = new ControladoraAsignaciones();
            llenarGridHorasReportadas();
        }
        else
        {
            this.MultiViewBecario.ActiveViewIndex = 0;
            if(estado == 7 && comentario == null){//esta finalizada y no ha hecho el comentario final
                commonService = new CommonServices(panelVacio);
                commonService.correrJavascript("abrir();");
            }
            //no se muestra nada pues no hay asignacion activa
        }
    }

    protected void btnReportarHoras_Click(object sender, EventArgs e)
    {
        modo = 0;
        habilitar();//habilito los campos para la insercion
        vaciarCampos();
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
                    vaciarCampos();//vacio los campos
                    indice = Convert.ToInt32(e.CommandArgument);
                    this.txtCantidadHoras.Text = infoActual.Rows[indice + gridControlHorasBecario.PageIndex * gridControlHorasBecario.PageSize].ItemArray[2].ToString();
                    this.txtFecha.Text = infoActual.Rows[indice + gridControlHorasBecario.PageIndex * gridControlHorasBecario.PageSize].ItemArray[3].ToString();
                    this.txtComentario.Text = infoActual.Rows[indice + gridControlHorasBecario.PageIndex * gridControlHorasBecario.PageSize].ItemArray[5].ToString();
                    this.txtComentarioEncargado.Text = infoActual.Rows[indice + gridControlHorasBecario.PageIndex * gridControlHorasBecario.PageSize].ItemArray[6].ToString();
                    this.txtComentarioEncargado.Enabled = false;
                    if (Convert.ToInt32(infoActual.Rows[indice + gridControlHorasBecario.PageIndex * gridControlHorasBecario.PageSize].ItemArray[4].ToString()) == 1)
                    {//reporte aceptado
                        habilitarModificacion(true);
                        modo = 1;
                    }
                    else {//reporte rechazado 
                        habilitarModificacion(false);
                        modo = -1;//no se hace nada en esta
                    }
                    commonService.abrirPopUp("PopUpCtrlBecario", "Nuevo Reporte de Horas");
                    commonService.correrJavascript("$('#comentarioDeEncargado').show();");
                    
                } break;
        }
    }

    private void habilitarModificacion(Boolean estado) {
        if (estado)
        {//para la modificacion del becario
            this.txtCantidadHoras.Enabled = true;
            this.txtComentario.Enabled = true;
            this.txtFecha.Enabled = false;
            this.txtComentarioEncargado.Enabled = false;
        }
        else { //no puede modificar
            this.txtCantidadHoras.Enabled = false;
            this.txtComentario.Enabled = false;
            this.txtFecha.Enabled = false;
            this.txtComentarioEncargado.Enabled = false;
        }
    }

    private void habilitar() {
        this.txtCantidadHoras.Enabled = true;
        this.txtComentario.Enabled = true;
        this.txtFecha.Enabled = true;
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

    // AYUDA CLICK
    protected void btnAyuda_Click(object sender, EventArgs e)
    {
        commonService.abrirPopUp("PopUpAyuda", "Ayuda");
        commonService.esconderPrimerBotonDePopUp("PopUpAyuda");
    }

    /* Crear tabla */
    protected void llenarGridHorasReportadas()
    {
        DataTable tablaHorasReportadas = crearTablaHorasReportadas();
        String cedEncargado = this.cb.getCedulaEncargado(Session["Cedula"].ToString(), Convert.ToInt32(Session["Periodo"].ToString()));
        infoActual = cb.horasReportadas(Session["Cedula"].ToString(), cedEncargado);
        DataRow newRow;
        int cantidadHoras = this.cb.getHoras(Session["Cedula"].ToString(), this.cb.getCedulaEncargado(Session["Cedula"].ToString(), Convert.ToInt32(Session["Periodo"].ToString())),Convert.ToInt32(Session["Periodo"].ToString()));
        if (infoActual.Rows.Count > 0)
        {
            foreach (DataRow r in infoActual.Rows)
            {
                newRow = tablaHorasReportadas.NewRow();
                newRow["Fecha"] = r[3].ToString();
                newRow["Cantidad Horas"] = r[2].ToString();
                switch (Convert.ToInt32(r[4].ToString())) {
                    case 0: newRow["Estado"] = "Pendiente";
                        break;
                    case 1: newRow["Estado"] = "Rechazada"; 
                        break;
                    case 2: newRow["Estado"] = "Aceptada";
                        break;
                }
                
                if (r[4].ToString().Equals("2")) { //horas aceptadas
                    cantidadHoras -= Convert.ToInt32(r[2].ToString());
                }
                tablaHorasReportadas.Rows.Add(newRow);
            }
        }
        this.gridControlHorasBecario.DataSource = tablaHorasReportadas;
        this.gridControlHorasBecario.DataBind();
        this.lblHorasRestantes.Text = cantidadHoras.ToString();
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
        switch(modo){
            case 0: insertarReporte();
                break;
            case 1: modificarReporte();
                break;
            default://no se hace nada
                break;
        }
        commonService.correrJavascript("$('#PopUpCtrlBecario').dialog('close');");
        vaciarCampos();
        llenarGridHorasReportadas();
    }

    private void vaciarCampos() {
        this.txtCantidadHoras.Text = "";
        this.txtComentario.Text = "";
        this.txtComentarioEncargado.Text = "";
    }

    public void insertarReporte() {
        Object[] datos = new Object[9];
        datos[0] = this.cb.getCedulaEncargado(Session["Cedula"].ToString(), Convert.ToInt32(Session["Periodo"].ToString()));//recupera la cedula del encargado para el becario logueado en el sistema
        datos[1] = Session["Cedula"].ToString();//el becario que esta logueado
        datos[2] = 0;//0 pendiente, 1 rechazada y 2 aceptada
        datos[3] = Convert.ToInt32(this.txtCantidadHoras.Text);//horas digitadas por el usuario
        datos[4] = DateTime.Now;//fecha actual del sistema
        datos[5] = "";
        datos[6] = this.txtComentario.Text;//comentario del becario
        datos[7] = Session["Periodo"].ToString();//periodo
        datos[8] = DateTime.Now.Year;
        String resultado = "";
        resultado = this.cb.enviarReporte(datos);
        commonService.mensajeJavascript(resultado, "Reporte de Horas");
    }

    public void modificarReporte() {
        Object[] datos = new Object[9];
        datos[0] = this.cb.getCedulaEncargado(Session["Cedula"].ToString(), Convert.ToInt32(Session["Periodo"].ToString()));//recupera la cedula del encargado para el becario logueado en el sistema
        datos[1] = Session["Cedula"].ToString();//el becario que esta logueado
        datos[2] = 0;//0 pendiente, 1 rechazada y 2 aceptada
        datos[3] = Convert.ToInt32(this.txtCantidadHoras.Text);//horas digitadas por el usuario
        datos[4] = infoActual.Rows[indice].ItemArray[3].ToString();
        datos[5] = this.txtComentarioEncargado.Text;
        datos[6] = this.txtComentario.Text;//comentario del becario
        datos[7] = Session["Periodo"].ToString();//periodo
        datos[8] = DateTime.Now.Year;
        int resultado;
        resultado = this.cb.modificarReporte(datos);
        if (resultado == 1) commonService.mensajeJavascript("Modificacion Exitosa", "Reporte de Horas");
        else  commonService.mensajeJavascript("Modificacion Fallida", "Reporte de Horas"); 
        
    }

    protected void gridControlHorasBecario_SelectedIndexChanging(object sender, GridViewSelectEventArgs e)
    {

    }

    protected void gridControlHorasBecario_SelectedIndexChanged(object sender, EventArgs e)
    {

    }

    private int siguientePeriodo(int cual) {
        switch (cual) {
            case 1: return 2;
            case 2: return 3;
            case 3: return 1;
            default: return 0;
        }
    }

    //metodo para la siguiente asignacion
    protected void btnInvisibleAsignacion_Click(object sender, EventArgs e)
    {
        if (radioNo.Checked || radioSi.Checked)
        {//si alguno esta presionado
            String encargado = this.cb.getCedulaEncargado(Session["Cedula"].ToString(), Convert.ToInt32(Session["Periodo"].ToString()));
            if (radioSi.Checked)
            {
                Object[] datos = new Object[4];
                datos[0] = Session["Cedula"].ToString();
                datos[1] = encargado;
                datos[2] = siguientePeriodo(1);
                datos[3] = retornarAnno(Convert.ToInt32(Session["Periodo"].ToString()), DateTime.Now.Year);
                //modificar la asignacion
                cb.aceptarSiguienteAsignacion(datos);
            }
            //agrego el comentario final del becario
            cb.comentarioFinal(Session["Cedula"].ToString(), encargado, txtComentFin.Text);
            commonService.cerrarPopUp("siguienteAsig");
        }
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

    protected void gridControlHorasBecario_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        DataTable tablaHorasReportadas = crearTablaHorasReportadas();
        String cedEncargado = this.cb.getCedulaEncargado(Session["Cedula"].ToString(), Convert.ToInt32(Session["Periodo"].ToString()));
        infoActual = cb.horasReportadas(Session["Cedula"].ToString(), cedEncargado);
        DataRow newRow;
        int cantidadHoras = this.cb.getHoras(Session["Cedula"].ToString(), this.cb.getCedulaEncargado(Session["Cedula"].ToString(), Convert.ToInt32(Session["Periodo"].ToString())), Convert.ToInt32(Session["Periodo"].ToString()));
        if (infoActual.Rows.Count > 0)
        {
            foreach (DataRow r in infoActual.Rows)
            {
                newRow = tablaHorasReportadas.NewRow();
                newRow["Fecha"] = r[3].ToString();
                newRow["Cantidad Horas"] = r[2].ToString();
                switch (Convert.ToInt32(r[4].ToString()))
                {
                    case 0: newRow["Estado"] = "Pendiente";
                        break;
                    case 1: newRow["Estado"] = "Rechazada";
                        break;
                    case 2: newRow["Estado"] = "Aceptada";
                        break;
                }

                if (r[4].ToString().Equals("2"))
                { //horas aceptadas
                    cantidadHoras -= Convert.ToInt32(r[2].ToString());
                }
                tablaHorasReportadas.Rows.Add(newRow);
            }
        }
        this.gridControlHorasBecario.DataSource = tablaHorasReportadas;
        this.gridControlHorasBecario.PageIndex = e.NewPageIndex;//siguiente pagina del grid
        this.gridControlHorasBecario.DataBind();
        this.lblHorasRestantes.Text = cantidadHoras.ToString();
        headersCorrectosHorasReportadas();
    }
}
