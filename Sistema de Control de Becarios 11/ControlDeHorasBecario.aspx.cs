using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

public partial class ControlDeHoras : System.Web.UI.Page
{
    private static CommonServices commonService;//variable de servicios
    private DataTable infoActual;//CedulaBecario, CedulaEncargado, CantidadHoras, Fecha, Estado, CuerpoComentarioBecario, CuerpoComentarioEncargado
    private ControladoraControlBecario cb;//variable de controladora de becario
    private ControladoraAsignaciones ca;//variable controladora de asignaciones
    public static int modo = 0;//inserta o modifica
    public static int indice = 0;//indice del grid


    protected void Page_Load(object sender, EventArgs e)
    {
        cb = new ControladoraControlBecario();
        //cedula del encargado del becario logueado
        String encargado = this.cb.getCedulaEncargado(Session["Cedula"].ToString(),Convert.ToInt32(Session["Periodo"].ToString()));
        //recupero el estado de dicha asignacion
        int estado = cb.getEstado(Session["Cedula"].ToString(),encargado,Convert.ToInt32(Session["Periodo"].ToString()));
        //comentrario final del becario
        String comentario = cb.getComentarioBecarioFinal(Session["Cedula"].ToString(), encargado);

        if (estado == 1)
        {//asignacion aceptada
            this.MultiViewBecario.ActiveViewIndex = 1;
            commonService = new CommonServices(UpdateInfo);//inicializo variables
            ca = new ControladoraAsignaciones();//inicializo variables
            llenarGridHorasReportadas();//se llena el grid
        }
        else
        {
            this.MultiViewBecario.ActiveViewIndex = 0;
            if(estado == 7 && comentario == null){//esta finalizada y no ha hecho el comentario final
                commonService = new CommonServices(panelVacio);//inicializo vairables
                commonService.correrJavascript("abrir();");//abre ventana para ultimo comentario y aceptar sig asignacion
            }
            //no se muestra nada pues no hay asignacion activa
        }
    }

    protected void btnReportarHoras_Click(object sender, EventArgs e)
    {
        modo = 0;//reportar nuevas horas
        habilitar();//habilito los campos para la insercion
        vaciarCampos();//limpia los capos
        //abre la ventana emergente
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
                {//selecciona tupla
                    vaciarCampos();//vacio los campos
                    indice = Convert.ToInt32(e.CommandArgument);//cual fila selecciono
                    //se cargan los datos de la informacion actual
                    this.txtCantidadHoras.Text = infoActual.Rows[indice + gridControlHorasBecario.PageIndex * gridControlHorasBecario.PageSize].ItemArray[2].ToString();
                    this.txtFecha.Text = infoActual.Rows[indice + gridControlHorasBecario.PageIndex * gridControlHorasBecario.PageSize].ItemArray[3].ToString();
                    this.txtComentario.Text = infoActual.Rows[indice + gridControlHorasBecario.PageIndex * gridControlHorasBecario.PageSize].ItemArray[5].ToString();
                    this.txtComentarioEncargado.Text = infoActual.Rows[indice + gridControlHorasBecario.PageIndex * gridControlHorasBecario.PageSize].ItemArray[6].ToString();
                    this.txtComentarioEncargado.Enabled = true;
                    if (Convert.ToInt32(infoActual.Rows[indice + gridControlHorasBecario.PageIndex * gridControlHorasBecario.PageSize].ItemArray[4].ToString()) == 1)
                    {//reporte rechazado
                        habilitarModificacion(true);
                        modo = 1;
                    }
                    else {//reporte aceptado o pendiente
                        habilitarModificacion(false);
                        modo = -1;//no se hace nada en esta
                    }
                    //abre la ventana emergente
                    commonService.correrJavascript("$('#comentarioDeEncargado').show();");
                    commonService.abrirPopUp("PopUpCtrlBecario", "Nuevo Reporte de Horas");
                    
                } break;
        }
    }

    //para saber si puede o no modificar
    private void habilitarModificacion(Boolean estado) {
        if (estado)
        {//para la modificacion del becario se habilitan los campos
            this.txtCantidadHoras.Enabled = true;
            this.txtComentario.Enabled = true;
            this.txtFecha.Enabled = false;
            this.txtComentarioEncargado.Enabled = false;
        }
        else { //no puede modificar, no se habilitan los campos
            this.txtCantidadHoras.Enabled = false;
            this.txtComentario.Enabled = false;
            this.txtFecha.Enabled = false;
            this.txtComentarioEncargado.Enabled = false;
        }
    }

    //habilita los campos
    private void habilitar() {
        this.txtCantidadHoras.Enabled = true;
        this.txtComentario.Enabled = true;
        this.txtFecha.Enabled = true;
    }

    // BUSCAR CLICK
    protected void btnBuscar_Click(object sender, EventArgs e)
    {
        
    }

    // AYUDA CLICK
    /* Efectúa: Carga la ventana emergente de ayuda.
    * Requiere: N/A
    * Modifica: N/A
    * */
    protected void btnAyuda_Click(object sender, EventArgs e)
    {
        commonService.abrirPopUp("PopUpAyuda", "Ayuda");
        commonService.esconderPrimerBotonDePopUp("PopUpAyuda");
    }

    /* llena tabla */
    protected void llenarGridHorasReportadas()
    {
        DataTable tablaHorasReportadas = crearTablaHorasReportadas();//crea la tabla
        //recupera el encargado
        String cedEncargado = this.cb.getCedulaEncargado(Session["Cedula"].ToString(), Convert.ToInt32(Session["Periodo"].ToString()));
        infoActual = cb.horasReportadas(Session["Cedula"].ToString(), cedEncargado);//consulta las horas reportadas
        DataRow newRow;//nueva fila
        //cantidad de horas restantes
        int cantidadHoras = this.cb.getHoras(Session["Cedula"].ToString(), this.cb.getCedulaEncargado(Session["Cedula"].ToString(), Convert.ToInt32(Session["Periodo"].ToString())),Convert.ToInt32(Session["Periodo"].ToString()));
        if (infoActual.Rows.Count > 0)
        {//si hay reportes
            foreach (DataRow r in infoActual.Rows)
            {//se recorre cada reporte de horas
                newRow = tablaHorasReportadas.NewRow();
                newRow["Fecha"] = r[3].ToString();//columna fecha
                newRow["Cantidad Horas"] = r[2].ToString();//columna cantiadd de horas
                switch (Convert.ToInt32(r[4].ToString())) {//estado del reporte
                    case 0: newRow["Estado"] = "Pendiente";
                        break;
                    case 1: newRow["Estado"] = "Rechazada"; 
                        break;
                    case 2: newRow["Estado"] = "Aceptada";
                        break;
                }
                
                if (r[4].ToString().Equals("2")) { //horas aceptadas
                    cantidadHoras -= Convert.ToInt32(r[2].ToString());//resta la cantidad de horas si estan aceptadas
                }
                tablaHorasReportadas.Rows.Add(newRow);//agrega la fila
            }
        }
        this.gridControlHorasBecario.DataSource = tablaHorasReportadas;//los datos del grid
        this.gridControlHorasBecario.DataBind();//se agregan los datos
        this.lblHorasRestantes.Text = cantidadHoras.ToString();//se fija la cantidad de horas restantes
        headersCorrectosHorasReportadas();
    }

    //crea la tabla para el grid
    protected DataTable crearTablaHorasReportadas()
    {
        DataTable dt = new DataTable();
        DataColumn column;
        //nueva columna Fecha
        column = new DataColumn();
        column.DataType = System.Type.GetType("System.String");
        column.ColumnName = "Fecha";
        dt.Columns.Add(column);
        //nueva Columna Cantidad de horas
        column = new DataColumn();
        column.DataType = System.Type.GetType("System.String");
        column.ColumnName = "Cantidad Horas";
        dt.Columns.Add(column);
        //Nueva columna Estado del reporte
        column = new DataColumn();
        column.DataType = System.Type.GetType("System.String");
        column.ColumnName = "Estado";
        dt.Columns.Add(column);

        return dt;
    }

    //crea el header para el grid
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
            case 0: insertarReporte();//va insertar nuevo reporte
                break;
            case 1: modificarReporte();//va modificar un reporte existente
                break;
            default://no se hace nada
                break;
        }
    }

    //limpia todos los campos de texto
    private void vaciarCampos() {
        this.txtCantidadHoras.Text = "";
        this.txtComentario.Text = "";
        this.txtComentarioEncargado.Text = "";
    }

    //se inserta un nuevo reporte de horas
    public void insertarReporte() {
        if (Convert.ToInt32(txtCantidadHoras.Text) <= Convert.ToInt32(lblHorasRestantes.Text))
        {// cantidad de horas que reporto son menores o iguales a las que me hacen falta
            Object[] datos = new Object[9];//se crea el objeto de datos
            datos[0] = this.cb.getCedulaEncargado(Session["Cedula"].ToString(), Convert.ToInt32(Session["Periodo"].ToString()));//recupera la cedula del encargado para el becario logueado en el sistema
            datos[1] = Session["Cedula"].ToString();//el becario que esta logueado
            datos[2] = 0;//0 pendiente, 1 rechazada y 2 aceptada
            datos[3] = Convert.ToInt32(this.txtCantidadHoras.Text);//horas digitadas por el usuario
            datos[4] = txtFecha.Text;//fecha actual del sistema
            datos[5] = "";
            datos[6] = this.txtComentario.Text;//comentario del becario
            datos[7] = Session["Periodo"].ToString();//periodo
            datos[8] = DateTime.Now.Year;
            String resultado = "";
            resultado = this.cb.enviarReporte(datos);//inserta el reporte de horas
            commonService.mensajeJavascript(resultado, "Reporte de Horas");//muestra un mensaje de exito o error
            //cierra la ventana emergente
            commonService.correrJavascript("$('#PopUpCtrlBecario').dialog('close');");
            vaciarCampos();//limpia los campos
            llenarGridHorasReportadas();//llena nuevamente el grid
        }
        else {
            commonService.mensajeJavascript("La cantidad de horas reportadas excede la cantidad de horas restantes", "Error");
            commonService.correrJavascript("$('#comentarioDeEncargado').hide();");
            commonService.correrJavascript("$('.dateText').datepicker({ dateFormat: 'dd-MM-yy' });");
        }
        
    }

    //para modificar un reporte existente
    public void modificarReporte() {
        if (Convert.ToInt32(txtCantidadHoras.Text) <= Convert.ToInt32(lblHorasRestantes.Text))
        {// cantidad de horas que reporto son menores o iguales a las que me hacen falta
            Object[] datos = new Object[9];//crea el arreglo de datos
            datos[0] = this.cb.getCedulaEncargado(Session["Cedula"].ToString(), Convert.ToInt32(Session["Periodo"].ToString()));//recupera la cedula del encargado para el becario logueado en el sistema
            datos[1] = Session["Cedula"].ToString();//el becario que esta logueado
            datos[2] = 0;//0 pendiente, 1 rechazada y 2 aceptada
            datos[3] = Convert.ToInt32(this.txtCantidadHoras.Text);//horas digitadas por el usuario
            datos[4] = infoActual.Rows[indice].ItemArray[3].ToString();
            datos[5] = this.txtComentarioEncargado.Text;
            datos[6] = this.txtComentario.Text;//comentario del becario
            datos[7] = Session["Periodo"].ToString();//periodo
            datos[8] = DateTime.Now.Year;
            int resultado;//resultado de la modificacion
            resultado = this.cb.modificarReporte(datos);//modifico el reporte
            if (resultado == 1) commonService.mensajeJavascript("Modificacion Exitosa", "Reporte de Horas");//exitoso
            else commonService.mensajeJavascript("Modificacion Fallida", "Reporte de Horas"); //fallido
            //cierra la ventana emergente
            commonService.correrJavascript("$('#PopUpCtrlBecario').dialog('close');");
            vaciarCampos();//limpia los campos
            llenarGridHorasReportadas();//llena nuevamente el grid
        }
        else {
            commonService.mensajeJavascript("La cantidad de horas reportadas excede la cantidad de horas restantes", "Error");
            commonService.correrJavascript("$('#comentarioDeEncargado').hide();");
            commonService.correrJavascript("$('.dateText').datepicker({ dateFormat: 'dd-MM-yy' });");
        }
        
    }

    //obtiene el siguiente periodo al actual
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
            //obtiene cedula del encargado
            String encargado = this.cb.getCedulaEncargado(Session["Cedula"].ToString(), Convert.ToInt32(Session["Periodo"].ToString()));
            if (radioSi.Checked)
            {//si ha seleccionado si, hay que aceptar siguiente asignacion
                Object[] datos = new Object[4];//crea objeto de datos
                datos[0] = Session["Cedula"].ToString();//cedula del becaio
                datos[1] = encargado;//cedula encargado
                datos[2] = siguientePeriodo(1);//siguiente periodo
                datos[3] = retornarAnno(Convert.ToInt32(Session["Periodo"].ToString()), DateTime.Now.Year);//anno
                //modificar la asignacion
                cb.aceptarSiguienteAsignacion(datos);
            }
            //agrego el comentario final del becario
            cb.comentarioFinal(Session["Cedula"].ToString(), encargado, txtComentFin.Text);
            commonService.cerrarPopUp("siguienteAsig");//cierra la ventana emergente
        }
    }

    //retorna el a;o  para la siguiente asignacion dependiendo del periodo
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

    //cuando se cambia de pagina en el grid
    protected void gridControlHorasBecario_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        DataTable tablaHorasReportadas = crearTablaHorasReportadas();//se crea la tabla
        //obtiene el encargado
        String cedEncargado = this.cb.getCedulaEncargado(Session["Cedula"].ToString(), Convert.ToInt32(Session["Periodo"].ToString()));
        infoActual = cb.horasReportadas(Session["Cedula"].ToString(), cedEncargado);//guarda los reportes
        DataRow newRow;//nueva fila
        //cantidad de horas que faltan
        int cantidadHoras = this.cb.getHoras(Session["Cedula"].ToString(), this.cb.getCedulaEncargado(Session["Cedula"].ToString(), Convert.ToInt32(Session["Periodo"].ToString())), Convert.ToInt32(Session["Periodo"].ToString()));
        if (infoActual.Rows.Count > 0)
        {//si hay reportes
            foreach (DataRow r in infoActual.Rows)
            {//por cada reporte de horas
                newRow = tablaHorasReportadas.NewRow();
                newRow["Fecha"] = r[3].ToString();//obtiene la fecha
                newRow["Cantidad Horas"] = r[2].ToString();//obtiene la cantidad de horas
                switch (Convert.ToInt32(r[4].ToString()))
                {//el estado del reporte
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
                tablaHorasReportadas.Rows.Add(newRow);//agrega al grid
            }
        }
        this.gridControlHorasBecario.DataSource = tablaHorasReportadas;
        this.gridControlHorasBecario.PageIndex = e.NewPageIndex;//siguiente pagina del grid
        this.gridControlHorasBecario.DataBind();
        this.lblHorasRestantes.Text = cantidadHoras.ToString();
        headersCorrectosHorasReportadas();
    }
}
