using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;


public partial class Cuentas : System.Web.UI.Page
{
    private static CommonServices commonService;
    private ControladoraCuentas controladoraCuentas = new ControladoraCuentas();
    private ControladoraPerfiles controladoraPerfiles = new ControladoraPerfiles();
    private static List<int> lsTipoCuentasDrp = new List<int>();
    private static Object [] datosOriginales = new Object[4];
    private static Object [] datosOriginalesAsociacion = new Object[2];
    private int drpIndex = 0;

    private static int modo = 0;

    protected void Page_Load(object sender, EventArgs e)
    {
        commonService = new CommonServices(UpdateInfo);

        List<int> permisos = new List<int>();
        permisos = Session["ListaPermisos"] as List<int>;

        if (permisos == null)
        {
            Session["Nombre"] = "";
            Response.Redirect("~/Default.aspx");
        }
        else
        {

            int permiso = 0; /* Query to user validation */
            if (permisos.Contains(11))
            {
                permiso = 11;
            }
            else
            {
                if (permisos.Contains(12))
                {
                    permiso = 12;
                }
            }

            switch (permiso)
            {
                case 11:
                    { // Vista completa
                        multiViewCuentas.SetActiveView(vistaAdmin);
                        if (!IsPostBack)
                        {
                            llenarDrpDown();
                            llenarGridCuentas();
                        }
                    } break;

                case 12:
                    { // Vista Parcial
                        multiViewCuentas.SetActiveView(viewEncBec);
                        if (!IsPostBack)
                        {
                            llenarCamposPersonal();
                            controlarACCAPers(false);
                        }
                        controlarCamposPersonales(false);
                    } break;

                default:
                    { // Vista sin permiso
                        multiViewCuentas.ActiveViewIndex = 2;
                    } break;


            }

        }
    }

    protected void btnInvisible1_Click(object sender, EventArgs e)
    {
        string mensaje = "-1";
        Object[] datos = new Object[4];
        if(modo==1 || modo==2){
            datos[0] = this.txtUsuario.Text;
            datos[1] = this.cntUsuario.Text;
            datos[2] = this.txtFechaAux.Text;
            if (this.drpDownPerfiles.SelectedValue != "1" && this.drpDownPerfiles.SelectedValue != "2")
            {
                datos[3] = "000000000";
            }
            else
            {
                datos[3] = this.txtCedula.Text;
            }
        }
        if(modo==1){
           mensaje = controladoraCuentas.ejecutar(modo, datos, null);
           if(mensaje==""){
              Object [] datosAsoc = new Object[2];
              datosAsoc[0] = this.txtUsuario.Text;
              datosAsoc[1] = this.drpDownPerfiles.SelectedItem.Text;
              controladoraCuentas.ejecutarAsociacion(1, datosAsoc, null);
              if(mensaje==""){
                  mensaje = "Se ha insertado correctamente la cuenta";
              }
           }
        }
        if(modo==2){
            //borro la asociacion de la cuenta vieja
            mensaje = controladoraCuentas.ejecutarAsociacion(3, datosOriginalesAsociacion, null);
            if(mensaje==""){
                mensaje = controladoraCuentas.ejecutar(modo, datos, datosOriginales);
                if(mensaje==""){
                    Object[] datosAsoc = new Object[2];
                    datosAsoc[0] = this.txtUsuario.Text;
                    datosAsoc[1] = this.drpDownPerfiles.SelectedItem.Text;
                    controladoraCuentas.ejecutarAsociacion(1, datosAsoc, null);
                    mensaje = "Se ha modificado correctamente la cuenta";
                }
            }
        }
        if(mensaje!="-1"){
            commonService.mensajeJavascript(mensaje, "Aviso");
        }
        llenarGridCuentas();
        commonService.cerrarPopUp("PopUp");
        //commonService.mensajeJavascript("hola!", "soy el título!");
    }

    protected void btnInvisible2_Click(object sender, EventArgs e)
    {
        if ((string)(Session["Cuenta"]) != this.txtUsuario.Text)
        {
            String mensaje = "-1";
            if (modo != 2)
            {
                commonService.cerrarPopUp("PopUp");//cierro el popUp con los datos
                datosOriginales[0] = this.txtUsuario.Text;
                datosOriginales[1] = this.cntUsuario.Text;
                datosOriginales[2] = "";
                datosOriginales[3] = this.txtCedula.Text;
                datosOriginalesAsociacion[0] = this.txtUsuario.Text;
                datosOriginalesAsociacion[1] = this.drpDownPerfiles.SelectedItem.Text;
                mensaje = controladoraCuentas.ejecutarAsociacion(3, datosOriginalesAsociacion, null);
                if (mensaje == "")
                {
                    mensaje = controladoraCuentas.ejecutar(3, datosOriginales, null);
                    commonService.mensajeJavascript(mensaje, "Atención");
                }
            }
            else {
                mensaje = controladoraCuentas.ejecutarAsociacion(3, datosOriginalesAsociacion, null);
                if (mensaje == "")
                {
                    mensaje = controladoraCuentas.ejecutar(3, datosOriginales, null);
                    commonService.mensajeJavascript(mensaje, "Atención");
                }
            }
            llenarGridCuentas();
        }
        else {
            commonService.mensajeJavascript("No se puede eliminar la cuenta activa.", "Atención");
        }
    }


    /*
    * -----------------------------------------------------------------------
    * BUTTON: CLICKS
    * -----------------------------------------------------------------------
    */
    // INSERTAR CLICK
    protected void btnInsertarCuenta_Click(object sender, EventArgs e)
    {
        vaciarCampos(true);
        controlarCampos(true);
        modo = 1;
        commonService.abrirPopUp("PopUp", "Insertar Nueva Cuenta");
        commonService.mostrarPrimerBotonDePopUp("PopUp");
        mostrarBotonesPrincipales(false);
    }

    // MODIFICAR CLICK
    protected void btnModificarCuenta_Click(object sender, EventArgs e)
    {
        commonService.mostrarPrimerBotonDePopUp("PopUp");

        datosOriginales[0] = this.txtUsuario.Text;
        datosOriginales[1] = this.cntUsuario.Text;
        datosOriginales[2] = this.txtFechaAux.Text;
        datosOriginales[3] = this.txtCedula.Text;
        datosOriginalesAsociacion[0] = this.txtUsuario.Text;
        datosOriginalesAsociacion[1] = this.drpDownPerfiles.SelectedItem.Text;
        controlarCampos(true);
        modo = 2;
    }

    // ELIMINAR CLICK
    protected void btnEliminarCuenta_Click(object sender, EventArgs e)
    {
        commonService.abrirPopUp("PopUpEliminar", "Eliminar Cuenta");
    }

//--------------------------------------------

    protected void llenarDrpDown() {

        DataTable dtPerfiles = controladoraPerfiles.consultar();
        lsTipoCuentasDrp.Clear();
        int i = 0;
        foreach(DataRow r in dtPerfiles.Rows){
            ListItem item = new ListItem(commonService.procesarStringDeUI(r[0].ToString()), i+"");
            this.drpDownPerfiles.Items.Add(item);
            lsTipoCuentasDrp.Add(Convert.ToInt32(r[1]));
            i++;
        }
        this.drpDownPerfiles.DataBind();
        this.drpDownPerfiles.SelectedIndex = drpIndex;

        if (lsTipoCuentasDrp[0] == 0)
        {
            ListItem aux = this.drpDownPerfiles.Items.FindByValue("0");
            this.drpDownPerfiles.SelectedValue = aux.Value;
            controlarCedula(false);
        }
         
    }


    protected void drpDownPerfiles_SelectedIndexChanged(object sender, EventArgs e)
    {
        drpIndex = this.drpDownPerfiles.SelectedIndex;
        if (lsTipoCuentasDrp[drpIndex] == 1 || lsTipoCuentasDrp[drpIndex] == 2)
        {
            controlarCedula(true);
        }
        else{
            controlarCedula(false);
        }
    }

    protected void mostrarBotonesPrincipales(Boolean accion) { 
        if(accion){
            this.btnEliminarCuenta.Visible = true;
            this.btnModificarCuenta.Visible = true;
        }
        else{
            this.btnEliminarCuenta.Visible = false;
            this.btnModificarCuenta.Visible = false;
        }
    }

    protected void vaciarCampos(Boolean accion) { 
        if(accion){
            this.txtUsuario.Text = "";
            this.cntUsuario.Text = "";
            this.cofCntUsuario.Text = "";
            this.txtCedula.Text = "";
            this.txtFechaAux.Text = "";
        }
    }

    protected void vaciarCamposPersonal(Boolean accion)
    {
        if (accion)
        {
            this.txtUsuarioPers.Text = "";
            this.txtContrasenaPers.Text = "";
            this.txtConfContrasenaPers.Text = "";
            this.txtCedulaPers.Text = "";
        }
    }

    protected void controlarCampos(Boolean accion) {
        if (accion)
        {
            this.txtUsuario.Enabled = true;
            this.cntUsuario.Enabled = true;
            this.cofCntUsuario.Enabled = true;
            this.txtCedula.Enabled = true;
            this.drpDownPerfiles.Enabled = true;
        }
        else {
            this.txtUsuario.Enabled = false;
            this.cntUsuario.Enabled = false;
            this.cofCntUsuario.Enabled = false;
            this.txtCedula.Enabled = false;
            this.drpDownPerfiles.Enabled = false;
        }
    }

    protected void controlarCamposPersonales(Boolean accion)
    {
        if (accion)
        {
            this.txtUsuarioPers.Enabled = false;
            this.txtContrasenaPers.Enabled = true;
            this.txtConfContrasenaPers.Enabled = true;
            this.txtCedulaPers.Enabled = false;
            this.txtPerfil.Enabled = false;
        }
        else
        {
            this.txtUsuarioPers.Enabled = false;
            this.txtContrasenaPers.Enabled = false;
            this.txtConfContrasenaPers.Enabled = false;
            this.txtCedulaPers.Enabled = false;
            this.txtPerfil.Enabled = false;
        }
    }

    protected void controlarCedula(Boolean accion) {
        if (accion)
        {
            this.txtCedula.Visible = true;
            this.lblCedula.Visible = true;
        }
        else {
            this.txtCedula.Visible = false;
            this.lblCedula.Visible = false;
        }
    }

    protected void controlarCedulaPersonal(Boolean accion)
    {
        if (accion)
        {
            this.txtCedulaPers.Visible = true;
            this.lblCedulaPers.Visible = true;
        }
        else
        {
            this.txtCedulaPers.Visible = false;
            this.lblCedulaPers.Visible = false;
        }
    }

    protected void controlarACCAPers(Boolean accion){
        if(accion){
            btnAcepPers.Enabled = true;
            btnCancPers.Enabled = true;
        }
        else{
            btnAcepPers.Enabled = false;
            btnCancPers.Enabled = false;
        }
    }

    protected void GridViewCuentas_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        switch(e.CommandName){
            case "seleccionarPosibleCuenta": {
                GridViewRow filaSeleccionda = this.GridViewCuentas.Rows[Convert.ToInt32(e.CommandArgument)];
                TextBox txtU = filaSeleccionda.FindControl("txtUsuarioG") as TextBox;
                TextBox txtC = filaSeleccionda.FindControl("txtContrasenna") as TextBox;
                DataTable dt = controladoraCuentas.consultarPorNombreContr(txtU.Text, txtC.Text);
                if(dt.Rows.Count==1){
                   foreach(DataRow r in dt.Rows){
                       this.txtUsuario.Text = commonService.procesarStringDeUI(r[0].ToString());
                       this.txtFechaAux.Text = r[2].ToString();
                       this.cntUsuario.Text = commonService.procesarStringDeUI(r[1].ToString());
                       this.cofCntUsuario.Text = commonService.procesarStringDeUI(r[1].ToString());
                       this.txtCedula.Text = commonService.procesarStringDeUI(r[3].ToString());
                       DataTable dtPerfil = controladoraCuentas.consultarPorNombreCuenta(this.txtUsuario.Text);
                       if (dtPerfil.Rows.Count==1 && this.drpDownPerfiles.Items.FindByText(commonService.procesarStringDeUI(dtPerfil.Rows[0][1].ToString())) != null)
                       {
                           ListItem aux = this.drpDownPerfiles.Items.FindByText(commonService.procesarStringDeUI(dtPerfil.Rows[0][1].ToString()));
                           this.drpDownPerfiles.SelectedValue = aux.Value;
                           if (lsTipoCuentasDrp[Convert.ToInt32(aux.Value)] == 1 || lsTipoCuentasDrp[Convert.ToInt32(aux.Value)] == 2)
                           {
                               controlarCedula(true);
                           }
                           else {
                               controlarCedula(false);
                           }
                       }
                   }
                   mostrarBotonesPrincipales(true);
                   controlarCampos(false);
                   modo = 0;
                   commonService.abrirPopUp("PopUp", "Consultar cuenta");
                   commonService.esconderPrimerBotonDePopUp("PopUp");
                }
            }break;
        }
    }

    protected void llenarGridCuentas() {
        DataTable dt = controladoraCuentas.consultarCuentas();
        DataTable aux = crearTablaCuentas();
        if (dt.Rows.Count > 0)
        {

        }
        else {
            Object[] datos = new Object[3];
            datos[0] = "-";
            datos[1] = "-";
            datos[2] = "-";
        }
        this.GridViewCuentas.DataSource = dt;
        this.GridViewCuentas.DataBind();
        this.GridViewCuentas.HeaderRow.BackColor = System.Drawing.Color.FromArgb(4562432);
        this.GridViewCuentas.HeaderRow.ForeColor = System.Drawing.Color.White;        

    }

    protected DataTable crearTablaCuentas() {
        DataTable retorno = new DataTable();
        DataColumn columna;

        columna = new DataColumn();
        columna.DataType = System.Type.GetType("System.String");
        columna.ColumnName = "Nombre";
        retorno.Columns.Add(columna);

        columna = new DataColumn();
        columna.DataType = System.Type.GetType("System.String");
        columna.ColumnName = "Contraseña";
        retorno.Columns.Add(columna);

        columna = new DataColumn();
        columna.DataType = System.Type.GetType("System.String");
        columna.ColumnName = "UltimoAcceso";
        retorno.Columns.Add(columna);

        return retorno;
    }


    protected void btnBuscar_Click(object sender, EventArgs e)
    {
        DataTable dt = controladoraCuentas.consultarPorBusqueda(this.txtBuscarCuenta.Text);
        DataTable aux = crearTablaCuentas();
        Object [] datos = new Object[3];
        if (dt.Rows.Count > 0)
        {
            foreach (DataRow r in dt.Rows)
            {
                datos[0] = commonService.procesarStringDeUI(r[0].ToString());
                datos[1] = commonService.procesarStringDeUI(r[1].ToString());
                datos[2] = commonService.procesarStringDeUI(r[2].ToString());
                aux.Rows.Add(datos);
            }
            this.GridViewCuentas.DataSource = aux;
            this.GridViewCuentas.DataBind();
        }
        else {
            commonService.mensajeJavascript("Lo sentimos, no encotramos ninguna cuenta asociada a los caracteres digitados","Atención");
        }
    }

    protected void llenarCamposPersonal() {
        DataTable dt = new DataTable();
        dt = controladoraCuentas.obtenerDatosCuenta(Session["Cuenta"].ToString());
        if(dt.Rows.Count==1){
           foreach(DataRow r in dt.Rows){
               this.txtUsuarioPers.Text = commonService.procesarStringDeUI(r[0].ToString());
               this.txtContrasenaPers.Text = commonService.procesarStringDeUI(r[1].ToString());
               this.txtConfContrasenaPers.Text = commonService.procesarStringDeUI(r[1].ToString());
               this.txtCedulaPers.Text = commonService.procesarStringDeUI(r[3].ToString());
               DataTable dtPerfil = controladoraCuentas.consultarPorNombreCuenta(this.txtUsuarioPers.Text);
               if (dtPerfil.Rows.Count == 1)
               {
                   this.txtPerfil.Text = commonService.procesarStringDeUI(dtPerfil.Rows[0][1].ToString());
                   Object dtTipo = controladoraPerfiles.tipoPerfil(txtPerfil.Text);
                   if(dtTipo!=null){
                       int tipoPerfil = Convert.ToInt32(dtTipo);
                       if (tipoPerfil == 1 || tipoPerfil == 2)
                       {
                           controlarCedulaPersonal(true);
                       }
                       else
                       {
                           controlarCedulaPersonal(false);
                       }
                   }
               }
           }
        }
         
    }

    protected void btnModificarCuentaPers_Click(object sender, EventArgs e)
    {
        datosOriginales[0] = this.txtUsuarioPers.Text;
        datosOriginales[1] = this.txtContrasenaPers.Text;
        datosOriginales[2] = Session["UltimoAcceso"];
        datosOriginales[3] = this.txtCedulaPers.Text;
        datosOriginalesAsociacion[0] = this.txtUsuarioPers.Text;
        datosOriginalesAsociacion[1] = this.txtPerfil.Text;
        controlarCamposPersonales(true);
        controlarACCAPers(true);
        modo = 2;
    }
    protected void btnAceptarCuentaPers_Click(object sender, EventArgs e)
    {
        string mensaje = "-1";
        string a = this.txtContrasenaPers.Text;
        Object[] datos = new Object[4];
        if(modo==2){
            datos[0] = this.txtUsuarioPers.Text;
            datos[1] = this.txtContrasenaPers.Text;
            datos[2] = Session["UltimoAcceso"];
            datos[3] = this.txtCedulaPers.Text;
            mensaje = controladoraCuentas.ejecutarAsociacion(3, datosOriginalesAsociacion, null);
            if (mensaje == "")
            {
                mensaje = controladoraCuentas.ejecutar(modo, datos, datosOriginales);
                if (mensaje == "")
                {
                    Object[] datosAsoc = new Object[2];
                    datosAsoc[0] = this.txtUsuarioPers.Text;
                    datosAsoc[1] = this.txtPerfil.Text;
                    controladoraCuentas.ejecutarAsociacion(1, datosAsoc, null);
                    mensaje = "Se ha modificado correctamente la cuenta";
                }
            }
            commonService.mensajeJavascript(mensaje, "Ateción");
        }
    }
    protected void btnCancelarCuentaPers_Click(object sender, EventArgs e)
    {
        llenarCamposPersonal();
        controlarACCAPers(false);
    }
    protected void GridViewCuentas_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        this.GridViewCuentas.PageIndex = e.NewPageIndex;
        this.GridViewCuentas.DataBind();
    }

}
