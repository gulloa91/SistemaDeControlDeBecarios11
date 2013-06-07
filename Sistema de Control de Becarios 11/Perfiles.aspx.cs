using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

public partial class Perfiles : System.Web.UI.Page
{
    private static CommonServices commonService;
    private ControladoraPerfiles cp;
    private static int modo;
    private static String nombreAnterior;
    protected void Page_Load(object sender, EventArgs e)
    {
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
             if (permisos.Contains(13))/**Cambiar**/
             {
                 permiso = 13;
             }

             switch (permiso)
             {
                 case 13:
                     {
                         multiViewPerfiles.SetActiveView(vistaAdmin);
                         cp = new ControladoraPerfiles();//se crea el objeto de la controlador para solicitar servicios
                         if (!IsPostBack)
                         {
                             llenarGridPerfiles();//se llena el grid con los perfiles de la base de datos
                         }

                         habilitarBotones(false);//se deshabilitan los botones
                         commonService = new CommonServices(UpdateInfo);//instancia de servicios comunes
                     } break;

                 default:
                     {
                         multiViewPerfiles.SetActiveView(VistaSinPermiso);
                     } break;
             }
         }
    }

    protected void clicBotonAceptar(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            Object[] datos = new Object[16];//crea el objeto para almacenar los datos del perfil
            datos[0] = this.txtNombrePerfil.Text;//nombre nuevo del permiso
            if (radioAdministrador.Checked)
            {
                datos[1] = "0";
            }
            else if (radioEncargado.Checked)
            {
                datos[1] = "1";
            }
            else if (radioBecario.Checked)
            {
                datos[1] = "2";
            }
            else
            {
                datos[1] = "3";
            }
            datos[2] = nombreAnterior;//para modificar el nombre de ser necesario
            datos[3] = (this.radioBecarioCompleto.Checked) ? "1" : "0";
            datos[4] = (this.radioBecarioParcial.Checked) ? "2" : "0";
            datos[5] = (this.radioEncargadoCompleto.Checked) ? "3" : "0";
            datos[6] = (this.radioEncargadoParcial.Checked) ? "4" : "0";
            datos[7] = (this.radioControlBecario.Checked) ? "5" : "0";
            datos[8] = (this.radioControlEncargado.Checked) ? "6" : "0";
            datos[9] = (this.radioControlAdmin.Checked) ? "7" : "0";
            datos[10] = (this.radioAsignacionCompleta.Checked) ? "8" : "0";
            datos[11] = (this.radioAsignacionEncargado.Checked) ? "9" : "0";
            datos[12] = (this.radioAsignacionBecario.Checked) ? "10" : "0";
            datos[13] = (this.radioCuentaCompleta.Checked) ? "11" : "0";
            datos[15] = (this.radioCuentaParcial.Checked) ? "12" : "0";
            datos[14] = (this.checkPerfiles.Checked) ? "13" : "0";
            //para saber el tipo de perfil revizo los radioButtons            
            
            String resultado = cp.ejecutar(modo, datos);//se realiza la accion y se retorna el resultado
            if (resultado.Equals(""))
            {//exito al realizar la accion, mensaje de exito
                commonService.mensajeJavascript("Se ha ingresado el perfil exitosamente", "Perfil");
            }
            else
            {//ocurrio un problema, se muestra al usuario cual fue el problema
                commonService.mensajeJavascript(resultado, "¡Error en la acción!");
            }
            llenarGridPerfiles();//se llena nuevamente el grid con los perfiles
            habilitarBotones(false);//se desabilitan los botones que pueden estar habilitados

            commonService.cerrarPopUp("PopUp");
            if (modo == 1) commonService.mensajeJavascript("Se ha ingresado el Perfil Satisfactoriamente", "Ingreso de Perfil");
            else if (modo == 2) commonService.mensajeJavascript("Se ha modificado el perfil Staisfactoriamente", "Modificación de Perfil");
        }
    }

    protected void clickBotonEliminar(object sender, EventArgs e)
    {//se abre el popUp de confirmacion para la eliminacion de algun perfil
        commonService.abrirPopUp("PopUpEliminar", "Eliminar Perfil");

    }

    protected void clickBotonModificar(object sender, EventArgs e)
    {//se habilitan los campos para poder modificar el perfil
        modo = 2;//modificar
        habilitarCampos(true);
    }

    protected void clickBotonInsertar(object sender, EventArgs e)
    {
        modo = 1;//insertar
        vaciarCampos();//se vacian los campos para insertar un nuevo perfil
        habilitarCampos(true);
        commonService.abrirPopUp("PopUp", "Insertar Nuevo Perfil");
    }

    public void llenarGridPerfiles()
    {//se llena el grid con los perfiles en la base de datos
        this.gridPerfiles.DataSource = llenarTablaPerfiles();//creo la tabla
        this.gridPerfiles.DataBind();//se inserto la tabla al grid
        this.gridPerfiles.AllowPaging = true;
        if (this.gridPerfiles.Rows.Count > 0)
        {
            this.gridPerfiles.HeaderRow.BackColor = System.Drawing.Color.FromArgb(4562432);
            this.gridPerfiles.HeaderRow.ForeColor = System.Drawing.Color.White;
        }
    }

    public DataTable llenarTablaPerfiles()
    {
        DataTable tablaPerfiles = cp.consultar(); //consulto los perfiles en el sistema
        DataTable tablaPerfilesAMostrar = tablaPerfilAMostrar();

        foreach( DataRow row in tablaPerfiles.Rows) {

            DataRow fila = tablaPerfilesAMostrar.NewRow();
            fila["Nombre de Perfil"] = row[0].ToString();

            switch (Convert.ToInt32( row[1] )){
                case 0: // Administrador
                    {
                        fila["Tipo"] = "Administrador";
                    } break;

                case 1: // Encargado
                    {
                        fila["Tipo"] = "Encargado";
                    } break;

                case 2: 
                    { // Becario
                        fila["Tipo"] = "Becario";
                    } break;
                default:
                    {
                        fila["Tipo"] = "Otro";
                    } break;
            }
            tablaPerfilesAMostrar.Rows.Add(fila);
        }
        
        return tablaPerfilesAMostrar;
    }

    private DataTable tablaPerfilAMostrar()
    {
        DataTable tabla = new DataTable();

        // Nombre de Perfil
        DataColumn idColumn = new DataColumn();
        idColumn.DataType = System.Type.GetType("System.String");
        idColumn.ColumnName = "Nombre de Perfil";
        tabla.Columns.Add(idColumn);

        // Nombre de Tipo
        DataColumn typeColumn = new DataColumn();
        typeColumn.DataType = System.Type.GetType("System.String");
        typeColumn.ColumnName = "Tipo";
        tabla.Columns.Add(typeColumn);

        return tabla;
    }

    public void habilitarCampos(bool modo)
    {//se habilitan o deshabilitan los campos del perfillocalhost
        if (modo == true)
        {//habilitar
            this.txtNombrePerfil.Enabled = true;
            this.radioAdministrador.Enabled = true;
            this.radioEncargado.Enabled = true;
            this.radioBecario.Enabled = true;
            this.radioBecarioCompleto.Enabled = true;
            this.radioBecarioParcial.Enabled = true;
            this.radioEncargadoCompleto.Enabled = true;
            this.radioEncargadoParcial.Enabled = true;
            this.radioControlBecario.Enabled = true;
            this.radioControlEncargado.Enabled = true;
            this.radioControlAdmin.Enabled = true;
            this.radioAsignacionCompleta.Enabled = true;
            this.radioAsignacionEncargado.Enabled = true;
            this.radioAsignacionBecario.Enabled = true;
            this.radioCuentaCompleta.Enabled = true;
            this.radioCuentaParcial.Enabled = true;
            this.checkPerfiles.Enabled = true;
            this.radioSinAccesoBecario.Enabled = true;
            this.radioSinAccesoEncargado.Enabled = true;
            this.radioSinCuenta.Enabled = true;
        }
        else
        {//deshabilitar
            this.txtNombrePerfil.Enabled = false;
            this.radioAdministrador.Enabled = false;
            this.radioEncargado.Enabled = false;
            this.radioBecario.Enabled = false;
            this.radioBecarioCompleto.Enabled = false;
            this.radioBecarioParcial.Enabled = false;
            this.radioEncargadoCompleto.Enabled = false;
            this.radioEncargadoParcial.Enabled = false;
            this.radioControlBecario.Enabled = false;
            this.radioControlEncargado.Enabled = false;
            this.radioControlAdmin.Enabled = false;
            this.radioAsignacionCompleta.Enabled = false;
            this.radioAsignacionEncargado.Enabled = false;
            this.radioAsignacionBecario.Enabled = false;
            this.radioCuentaCompleta.Enabled = false;
            this.radioCuentaParcial.Enabled = false;
            this.checkPerfiles.Enabled = false;
            this.radioSinAccesoBecario.Enabled = false;
            this.radioSinAccesoEncargado.Enabled = false;
            this.radioSinCuenta.Enabled = false;
        }

    }

    public void vaciarCampos()
    {//se vacian los campos del perfil para cuando se tenga que hacer una insercion
        this.txtNombrePerfil.Text = "";
        this.radioAdministrador.Checked = false;
        this.radioEncargado.Checked = false;
        this.radioBecario.Checked = false;
        this.radioBecarioCompleto.Checked = false;
        this.radioBecarioParcial.Checked = false;
        this.radioEncargadoCompleto.Checked = false;
        this.radioEncargadoParcial.Checked = false;
        this.radioControlBecario.Checked = false;
        this.radioControlEncargado.Checked = false;
        this.radioControlAdmin.Checked = false;
        this.radioAsignacionCompleta.Checked = false;
        this.radioAsignacionEncargado.Checked = false;
        this.radioAsignacionBecario.Checked = false;
        this.radioCuentaCompleta.Checked = false;
        this.radioCuentaParcial.Checked = false;
        this.checkPerfiles.Checked = false;
        this.radioSinAccesoBecario.Checked = true;
        this.radioSinAccesoEncargado.Checked = true;
        this.radioSinCuenta.Checked = true;
        this.noControlHoras.Checked = false;
    }

    public void habilitarBotones(bool modo)
    {//se habilitan o deshabilitan los botones de insertar o modificar
        if (modo == true)
        {//habilitar
            this.btnEliminarEncargado.Visible = true;
            this.btnModificarEncargado.Visible = true;
        }
        else
        {//deshabilitar
            this.btnEliminarEncargado.Visible = false;
            this.btnModificarEncargado.Visible = false;
        }
    }

    public void cargarCamposPerfil(DataTable dt, Object tipo)
    {//carga los datos del perfil cuando se selecciona una tupla del grid
        for (int i = 0; i < dt.Rows.Count; ++i)
        {//recorro el data Table
            marcarRadio(Convert.ToInt32(dt.Rows[i].ItemArray[1]));//mando a marcar los permisos para dicho perfil
        }
        this.txtNombrePerfil.Text = dt.Rows[0].ItemArray[0].ToString();//guardo el nombre del perfil
        nombreAnterior = this.txtNombrePerfil.Text;
        //marco el radio con el tipo de perfil que se tenga
        if (Convert.ToInt32(tipo.ToString()) == 0)
        {
            this.radioAdministrador.Checked = true;
        }
        else if (Convert.ToInt32(tipo.ToString()) == 1)
        {
            this.radioEncargado.Checked = true;
        }
        else if (Convert.ToInt32(tipo.ToString()) == 2)
        {
            this.radioBecario.Checked = true;
        }
        else
        {
            //no hay ninguno seleccionado entonces se deja asi
        }
    }

    public void siElimina(object sender, EventArgs e)
    {//confirmacion del usuario para eliminar un perfil
        commonService.cerrarPopUp("PopUp");//cierro el popUp con los datos
        Object[] datos = new Object[16];
        datos[0] = this.txtNombrePerfil.Text;
        datos[3] = (this.radioBecarioCompleto.Checked) ? "1" : "0";
        datos[4] = (this.radioBecarioParcial.Checked) ? "2" : "0";
        datos[5] = (this.radioEncargadoCompleto.Checked) ? "3" : "0";
        datos[6] =  (this.radioEncargadoParcial.Checked) ? "4" : "0";
        datos[7] = (this.radioControlBecario.Checked) ? "5" : "0";
        datos[8] = (this.radioControlEncargado.Checked) ? "6" : "0";
        datos[9] = (this.radioControlAdmin.Checked) ? "7" : "0";
        datos[10] = (this.radioAsignacionCompleta.Checked) ? "8" : "0";
        datos[11] = (this.radioAsignacionEncargado.Checked) ? "9" : "0";
        datos[12] = (this.radioAsignacionBecario.Checked) ? "10" : "0";
        datos[13] = (this.radioCuentaCompleta.Checked) ? "11" : "0";
        datos[14] = (this.checkPerfiles.Checked) ? "12" : "0";
        datos[15] = (this.radioCuentaParcial.Checked) ? "13" : "0";
        String result = cp.ejecutar(3, datos);//ejectuo la accion de eliminar el perfil
        if (result.Equals(""))
        {//mesaje de exito
            commonService.mensajeJavascript("Se ha eliminado el perfil correctamente", "Eliminacion");
        }
        else
        {//mensaje de error
            commonService.mensajeJavascript(result, "Error en la eliminacion");
        }
        llenarGridPerfiles();
    }

    protected void gridPerfiles_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {//para cuando se cambie de pagina en el grid
        DataTable dt = cp.consultar();//se llama a la controladora
        DataTable tablaPerfilesAMostrar = tablaPerfilAMostrar();
        foreach (DataRow row in dt.Rows)
        {

            DataRow fila = tablaPerfilesAMostrar.NewRow();
            fila["Nombre de Perfil"] = row[0].ToString();

            switch (Convert.ToInt32(row[1]))
            {
                case 0: // Administrador
                    {
                        fila["Tipo"] = "Administrador";
                    } break;

                case 1: // Encargado
                    {
                        fila["Tipo"] = "Encargado";
                    } break;

                case 2:
                    { // Becario
                        fila["Tipo"] = "Becario";
                    } break;
            }
            tablaPerfilesAMostrar.Rows.Add(fila);
        }
        gridPerfiles.PageIndex = e.NewPageIndex;//siguiente indice del grid
        gridPerfiles.DataSource = tablaPerfilesAMostrar;//se ligan los datos
        gridPerfiles.DataBind();

        if (this.gridPerfiles.Rows.Count > 0)
        {
            this.gridPerfiles.HeaderRow.BackColor = System.Drawing.Color.FromArgb(4562432);
            this.gridPerfiles.HeaderRow.ForeColor = System.Drawing.Color.White;
        }
    }

    private void marcarRadio(int cual)
    {//se marcan los radioButtons o checkBoxes para dicho perfil
        switch (cual)
        {
            case 1:
                this.radioSinAccesoBecario.Checked = false;
                this.radioBecarioCompleto.Checked = true;
                break;
            case 2:
                this.radioSinAccesoBecario.Checked = false;
                this.radioBecarioParcial.Checked = true;
                break;
            case 3:
                this.radioSinAccesoEncargado.Checked = false;
                this.radioEncargadoCompleto.Checked = true;
                break;
            case 4:
                this.radioSinAccesoEncargado.Checked = false;
                this.radioEncargadoParcial.Checked = true;
                break;
            case 5:
                this.radioControlBecario.Checked = true;
                break;
            case 6:
                this.radioControlEncargado.Checked = true;
                break;
            case 7:
                this.radioControlAdmin.Checked = true;
                break;
            case 8:
                this.radioAsignacionCompleta.Checked = true;
                break;
            case 9:
                this.radioAsignacionEncargado.Checked = true;
                break;
            case 10:
                this.radioAsignacionBecario.Checked = true;
                break;
            case 11:
                this.radioSinCuenta.Checked = false;
                this.radioCuentaCompleta.Checked = true;
                break;
            case 12:
                this.checkPerfiles.Checked = true;
                break;
            case 13:{
                this.radioSinCuenta.Checked = false;
                this.radioCuentaParcial.Checked = true;
                } break;
        }
    }

    protected void btnBuscar_Click(object sender, EventArgs e)
    {
        String patron = this.txtBuscarPerfil.Text;//patron para buscar entre los nombres
        DataTable dt = cp.buscarPerfiles("%"+patron+"%");
        DataTable tablaPerfilesAMostrar = tablaPerfilAMostrar();

        foreach (DataRow row in dt.Rows)
        {

            DataRow fila = tablaPerfilesAMostrar.NewRow();
            fila["Nombre de Perfil"] = row[0].ToString();

            switch (Convert.ToInt32(row[1]))
            {
                case 0: // Administrador
                    {
                        fila["Tipo"] = "Administrador";
                    } break;

                case 1: // Encargado
                    {
                        fila["Tipo"] = "Encargado";
                    } break;

                case 2:
                    { // Becario
                        fila["Tipo"] = "Becario";
                    } break;
            }
            tablaPerfilesAMostrar.Rows.Add(fila);
        }
        this.gridPerfiles.DataSource = tablaPerfilesAMostrar;//creo la tabla
        this.gridPerfiles.DataBind();//se inserto la tabla al grid
        this.gridPerfiles.AllowPaging = true;
        if (this.gridPerfiles.Rows.Count > 0)
        {
            this.gridPerfiles.HeaderRow.BackColor = System.Drawing.Color.FromArgb(4562432);
            this.gridPerfiles.HeaderRow.ForeColor = System.Drawing.Color.White;
        }
    }

    protected void gridPerfiles_RowCommand(object sender, GridViewCommandEventArgs e)
    {

        switch(e.CommandName){

            case "seleccionarPosiblePerfil": {
                vaciarCampos();//vaciamos los campos del popUp
                habilitarCampos(false);//se deshabilitan los campos, asi al inicio no puede modificar
                habilitarBotones(true);//habilito botones de eliminar y modificar

                String nom = this.gridPerfiles.Rows[Convert.ToInt32(e.CommandArgument)].Cells[1].Text;//recupero el nombre del perfil
                DataTable dt = cp.consultarPerfil(nom);//consulto un perfil en especifico de la base
                Object tipo = cp.tipoPerfil(nom);//recupero el tipo de perfil
                cargarCamposPerfil(dt, tipo);//cargo los campos del perfil
                commonService.abrirPopUp("PopUp", "Consulta de Perfil");//abro el porUp con los datos
            } break;
        
        }
    
    }
}