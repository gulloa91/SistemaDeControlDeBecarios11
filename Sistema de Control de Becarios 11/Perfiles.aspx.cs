using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

public partial class Perfiles : System.Web.UI.Page
{

    private static CommonServices commonService;//variable de servicios comunes
    private ControladoraPerfiles cp;//variable de controladora
    private static int modo;//modo de operacion (insretar o modificar)
    private static String nombreAnterior;//para almacenar el nombre anterior en caso de cambio


    protected void Page_Load(object sender, EventArgs e)
    {
        List<int> permisos = new List<int>();//crea una lista para los permisos
        permisos = Session["ListaPermisos"] as List<int>;//obtiene los permisos del usuario logueado

        if (permisos == null)
        {//no tiene permisos
            Session["Nombre"] = "";
            Response.Redirect("~/Default.aspx");
        }
        else
        {

             int permiso = 0; /* Query to user validation */
             if (permisos.Contains(13))//permiso para perfiles
             {
                 permiso = 13;
             }

             switch (permiso)
             {
                 case 13:
                     {//perfiles
                         multiViewPerfiles.SetActiveView(vistaAdmin);//se hace visible la vista del administrador
                         cp = new ControladoraPerfiles();//se crea el objeto de la controlador para solicitar servicios
                         if (!IsPostBack)
                         {
                             llenarGridPerfiles();//se llena el grid con los perfiles de la base de datos
                         }
                         habilitarBotones(false);//se deshabilitan los botones
                         commonService = new CommonServices(UpdateInfo);//instancia de servicios comunes
                     } break;

                 default:
                     {//no tiene permiso por lo tanto se le muestra un mensaje
                         multiViewPerfiles.SetActiveView(VistaSinPermiso);
                     } break;
             }
         }
    }

    //clic del boton para crear un modificar un perfil
    protected void clicBotonAceptar(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {//datos de la pagina validos
            Object[] datos = new Object[16];//crea el objeto para almacenar los datos del perfil
            datos[0] = this.txtNombrePerfil.Text;//nombre nuevo del permiso
            //guardo cual de los tipos de perfil se selecciono
            if (radioAdministrador.Checked)
            {//administrador
                datos[1] = "0";
            }
            else if (radioEncargado.Checked)
            {//encargado
                datos[1] = "1";
            }
            else if (radioBecario.Checked)
            {//becario
                datos[1] = "2";
            }
            else
            {
                datos[1] = "3";
            }
            datos[2] = nombreAnterior;//para modificar el nombre de ser necesario
            //Guardo los perfiles que se han seleccionado para un perfil
            datos[3] = (this.radioBecarioCompleto.Checked) ? "1" : "0";//seleccion becario completo
            datos[4] = (this.radioBecarioParcial.Checked) ? "2" : "0";//seleccion becario parcial
            datos[5] = (this.radioEncargadoCompleto.Checked) ? "3" : "0";//seleccion encargado completo
            datos[6] = (this.radioEncargadoParcial.Checked) ? "4" : "0";//seleccion encargado parcial
            datos[7] = (this.radioControlBecario.Checked) ? "5" : "0";//seleccion control becario
            datos[8] = (this.radioControlEncargado.Checked) ? "6" : "0";//seleccion control encargado
            datos[9] = (this.checkReportes.Checked) ? "7" : "0";//seleccion reportes
            datos[10] = (this.radioAsignacionCompleta.Checked) ? "8" : "0";//seleccion asignacion completa
            datos[11] = (this.radioAsignacionEncargado.Checked) ? "9" : "0";//seleccion asignacion encargado
            datos[12] = (this.radioAsignacionBecario.Checked) ? "10" : "0";//seleccion asignacion becario
            datos[13] = (this.radioCuentaCompleta.Checked) ? "11" : "0";//seleccion cuentas completo
            datos[14] = (this.radioCuentaParcial.Checked) ? "12" : "0";//seleccion cuentas parcial
            datos[15] = (this.checkPerfiles.Checked) ? "13" : "0";//seleccion perfiles completo
            //para saber el tipo de perfil revizo los radioButtons            
            
            String resultado = cp.ejecutar(modo, datos);//se realiza la accion y se retorna el resultado
            if (resultado.Equals(""))
            {//exito al realizar la accion, mensaje de exito
                if (modo == 1) commonService.mensajeJavascript("Se ha ingresado el Perfil satisfactoriamente", "Ingreso de Perfil");
                else if (modo == 2) commonService.mensajeJavascript("Se ha modificado el perfil satisfactoriamente", "Modificación de Perfil");
            }
            else
            {//ocurrio un problema, se muestra al usuario cual fue el problema
                commonService.mensajeJavascript(resultado, "¡Error en la acción!");
            }
            llenarGridPerfiles();//se llena nuevamente el grid con los perfiles
            habilitarBotones(false);//se desabilitan los botones que pueden estar habilitados
            commonService.cerrarPopUp("PopUp");
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
        //this.radioAdministrador.Checked = true;
        habilitarCampos(true);//habilita los para ingresarlos
        commonService.abrirPopUp("PopUp", "Insertar Nuevo Perfil");//abre la ventana emergente
    }

    // AYUDA CLICK
    protected void btnAyuda_Click(object sender, EventArgs e)
    {
        commonService.abrirPopUp("PopUpAyuda", "Ayuda");
        commonService.esconderPrimerBotonDePopUp("PopUpAyuda");
    }

    //llena el grid
    public void llenarGridPerfiles()
    {//se llena el grid con los perfiles en la base de datos
        this.gridPerfiles.DataSource = llenarTablaPerfiles();//creo la tabla
        this.gridPerfiles.DataBind();//se inserto la tabla al grid
        this.gridPerfiles.AllowPaging = true;//paginacion del grid
        if (this.gridPerfiles.Rows.Count > 0)//si hay filas
        {//asigno los headers
            this.gridPerfiles.HeaderRow.BackColor = System.Drawing.Color.FromArgb(4562432);
            this.gridPerfiles.HeaderRow.ForeColor = System.Drawing.Color.White;
        }
    }

    //llena la tabla para el grid
    public DataTable llenarTablaPerfiles()
    {
        DataTable tablaPerfiles = cp.consultar(); //consulto los perfiles en el sistema
        DataTable tablaPerfilesAMostrar = tablaPerfilAMostrar();
        //recorro la consulta
        foreach( DataRow row in tablaPerfiles.Rows) {

            DataRow fila = tablaPerfilesAMostrar.NewRow();//creo una fila para el grid
            fila["Nombre de Perfil"] = row[0].ToString();//nombre del perfil

            switch (Convert.ToInt32( row[1] )){//tipo de perfil
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
            tablaPerfilesAMostrar.Rows.Add(fila);//agrego la fila
        }
        if(tablaPerfiles.Rows.Count == 0){//para cuando no hay nada
            DataRow fila = tablaPerfilesAMostrar.NewRow();
            fila["Tipo"] = "-"; fila["Nombre de Perfil"] = "-";
        }
        return tablaPerfilesAMostrar;
    }

    //crea la tabla que se va mostrar
    private DataTable tablaPerfilAMostrar()
    {
        DataTable tabla = new DataTable();

        // Nombre de Perfil
        DataColumn idColumn = new DataColumn();//nueva columna
        idColumn.DataType = System.Type.GetType("System.String");
        idColumn.ColumnName = "Nombre de Perfil";//nombre de la columna
        tabla.Columns.Add(idColumn);//agrego

        // Nombre de Tipo
        DataColumn typeColumn = new DataColumn();//nueva columna
        typeColumn.DataType = System.Type.GetType("System.String");
        typeColumn.ColumnName = "Tipo";//nombre de la columna
        tabla.Columns.Add(typeColumn);//agrego

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
            this.checkReportes.Enabled = true;
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
            this.checkReportes.Enabled = false;
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
        this.radioBecarioCompleto.Checked = false;
        this.radioBecarioParcial.Checked = false;
        this.radioEncargadoCompleto.Checked = false;
        this.radioEncargadoParcial.Checked = false;
        this.radioControlBecario.Checked = false;
        this.radioControlEncargado.Checked = false;
        this.checkReportes.Checked = false;
        this.radioAsignacionCompleta.Checked = false;
        this.radioAsignacionEncargado.Checked = false;
        this.radioAsignacionBecario.Checked = false;
        this.radioCuentaCompleta.Checked = false;
        this.radioCuentaParcial.Checked = false;
        this.checkPerfiles.Checked = false;
        this.radioSinAccesoBecario.Checked = false;
        this.radioSinAccesoEncargado.Checked = false;
        this.radioSinCuenta.Checked = false;
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

    public void cargarCamposPerfil(DataTable dt, Object tipo,int indice)
    {//carga los datos del perfil cuando se selecciona una tupla del grid
        if (dt.Rows.Count > 0)
        {
            for (int i = 0; i < dt.Rows.Count; ++i)
            {//recorro el data Table
                marcarRadio(Convert.ToInt32(dt.Rows[i].ItemArray[1]));//mando a marcar los permisos para dicho perfil
            }
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
        else { 
            //marco el tipo
            if(this.gridPerfiles.Rows[indice].Cells[2].Text.Equals("Administrador")){
                this.radioAdministrador.Checked = true;//se marca el administrador
            }
            else if (this.gridPerfiles.Rows[indice].Cells[2].Text.Equals("Becario"))
            {
                this.radioBecario.Checked = true;//marca el becario
            }
            else {
                this.radioEncargado.Checked = true;//marca el encargado
            }
        }
        this.txtNombrePerfil.Text = this.gridPerfiles.Rows[indice].Cells[1].Text;//pone el nombre del perfil
        nombreAnterior = this.txtNombrePerfil.Text;//guarda el nombre anterior
    }

    public void siElimina(object sender, EventArgs e)
    {//confirmacion del usuario para eliminar un perfil
        if (!nombreAnterior.Equals("Administrador"))
        {
            commonService.cerrarPopUp("PopUp");//cierro el popUp con los datos
            Object[] datos = new Object[16];//creo el objeto de datos
            datos[0] = this.txtNombrePerfil.Text;//guardo el nombre
            //guardo los permisos que se van a eliminar
            datos[3] = (this.radioBecarioCompleto.Checked) ? "1" : "0";
            datos[4] = (this.radioBecarioParcial.Checked) ? "2" : "0";
            datos[5] = (this.radioEncargadoCompleto.Checked) ? "3" : "0";
            datos[6] = (this.radioEncargadoParcial.Checked) ? "4" : "0";
            datos[7] = (this.radioControlBecario.Checked) ? "5" : "0";
            datos[8] = (this.radioControlEncargado.Checked) ? "6" : "0";
            datos[9] = (this.checkReportes.Checked) ? "7" : "0";
            datos[10] = (this.radioAsignacionCompleta.Checked) ? "8" : "0";
            datos[11] = (this.radioAsignacionEncargado.Checked) ? "9" : "0";
            datos[12] = (this.radioAsignacionBecario.Checked) ? "10" : "0";
            datos[13] = (this.radioCuentaCompleta.Checked) ? "11" : "0";
            datos[15] = (this.radioCuentaParcial.Checked) ? "12" : "0";
            datos[14] = (this.checkPerfiles.Checked) ? "13" : "0";
            String result = cp.ejecutar(3, datos);//ejectuo la accion de eliminar el perfil
            if (result.Equals(""))
            {//mesaje de exito
                commonService.mensajeJavascript("Se ha eliminado el perfil correctamente", "Eliminacion");
            }
            else
            {//mensaje de error
                commonService.mensajeJavascript(result, "Error en la eliminacion");
            }
            llenarGridPerfiles();//lana nuevamente el grid con los nuevos perfiles
        }
        else {
            commonService.mensajeJavascript("No se puede eliminar el perfil de administrador","Error");
        }
    }

    protected void gridPerfiles_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {//para cuando se cambie de pagina en el grid
        DataTable dt = cp.consultar();//se llama a la controladora
        DataTable tablaPerfilesAMostrar = tablaPerfilAMostrar();//crea la tabla a mostrar
        foreach (DataRow row in dt.Rows)
        {//recorre la consulta

            DataRow fila = tablaPerfilesAMostrar.NewRow();
            fila["Nombre de Perfil"] = row[0].ToString();

            switch (Convert.ToInt32(row[1]))
            {//marca administrador, becario o encargado
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
        gridPerfiles.DataBind();//agrego los datos

        if (this.gridPerfiles.Rows.Count > 0)
        {//si hay filas entonces muestra la tabla
            this.gridPerfiles.HeaderRow.BackColor = System.Drawing.Color.FromArgb(4562432);
            this.gridPerfiles.HeaderRow.ForeColor = System.Drawing.Color.White;
        }
    }

    //para cuando se consulta un perfil
    private void marcarRadio(int cual)
    {//se marcan los radioButtons o checkBoxes para dicho perfil
        switch (cual)
        {
            case 1://becario
                this.radioSinAccesoBecario.Checked = false;
                this.radioBecarioCompleto.Checked = true;
                break;
            case 2://becario
                this.radioSinAccesoBecario.Checked = false;
                this.radioBecarioParcial.Checked = true;
                break;
            case 3://encargado
                this.radioSinAccesoEncargado.Checked = false;
                this.radioEncargadoCompleto.Checked = true;
                break;
            case 4://encargado
                this.radioSinAccesoEncargado.Checked = false;
                this.radioEncargadoParcial.Checked = true;
                break;
            case 5://control de horas
                this.radioControlBecario.Checked = true;
                break;
            case 6://control de horas
                this.radioControlEncargado.Checked = true;
                break;
            case 7://reportes
                this.checkReportes.Checked = true;
                break;
            case 8://asignacion
                this.radioAsignacionCompleta.Checked = true;
                break;
            case 9://asignacion
                this.radioAsignacionEncargado.Checked = true;
                break;
            case 10://asignacion
                this.radioAsignacionBecario.Checked = true;
                break;
            case 11://cuentas
                this.radioSinCuenta.Checked = false;
                this.radioCuentaCompleta.Checked = true;
                break;
            case 12://cuentas
                this.radioSinCuenta.Checked = false;
                this.radioCuentaParcial.Checked = true;
                break;
            case 13://perfiles
                {
                    this.checkPerfiles.Checked = true;
                } break;
        }
    }

    //buscar por un filtro
    protected void btnBuscar_Click(object sender, EventArgs e)
    {
        String patron = this.txtBuscarPerfil.Text;//patron para buscar entre los nombres
        DataTable dt = cp.buscarPerfiles("%"+patron+"%");//para buscar patron
        DataTable tablaPerfilesAMostrar = tablaPerfilAMostrar();

        foreach (DataRow row in dt.Rows)
        {//recorro la consulta

            DataRow fila = tablaPerfilesAMostrar.NewRow();
            fila["Nombre de Perfil"] = row[0].ToString();

            switch (Convert.ToInt32(row[1]))
            {//marco el tipo
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
            tablaPerfilesAMostrar.Rows.Add(fila);//agrego la fila
        }
        this.gridPerfiles.DataSource = tablaPerfilesAMostrar;//creo la tabla
        this.gridPerfiles.DataBind();//se inserto la tabla al grid
        this.gridPerfiles.AllowPaging = true;//paginacion
        if (this.gridPerfiles.Rows.Count > 0)
        {
            this.gridPerfiles.HeaderRow.BackColor = System.Drawing.Color.FromArgb(4562432);
            this.gridPerfiles.HeaderRow.ForeColor = System.Drawing.Color.White;
        }
    }

    protected void gridPerfiles_RowCommand(object sender, GridViewCommandEventArgs e)
    {

        switch(e.CommandName){//seleccion
            case "seleccionarPosiblePerfil": {//seleccion de fila
                modo = 4;
                vaciarCampos();//vaciamos los campos del popUp
                habilitarCampos(false);//se deshabilitan los campos, asi al inicio no puede modificar
                habilitarBotones(true);//habilito botones de eliminar y modificar
                int indice = Convert.ToInt32(e.CommandArgument);
                String nom = this.gridPerfiles.Rows[indice].Cells[1].Text;//recupero el nombre del perfil
                DataTable dt = cp.consultarPerfil(nom);//consulto un perfil en especifico de la base
                Object tipo = cp.tipoPerfil(nom);//recupero el tipo de perfil
                cargarCamposPerfil(dt, tipo,indice);//cargo los campos del perfil
                commonService.abrirPopUp("PopUp", "Consulta de Perfil");//abro el porUp con los datos
            } break;
        
        }
    
    }
    protected void radioAdministrador_CheckedChanged(object sender, EventArgs e)
    {
        String temp = txtNombrePerfil.Text;
        vaciarCampos();
        txtNombrePerfil.Text = temp;
        habilitarCampos(true);
        //se marcan los permisos por defecto
        radioSinAccesoBecario.Checked = false;
        radioBecarioCompleto.Checked = true;
        radioEncargadoCompleto.Checked = true;
        radioAsignacionCompleta.Checked = true;
        checkPerfiles.Checked = true;
        checkReportes.Checked = true;
        radioCuentaCompleta.Checked = true;
        //se deshabilitan los permisos no validos
        radioCuentaParcial.Enabled = false;
        radioSinCuenta.Enabled = false;
        radioControlBecario.Enabled = false;
        radioControlEncargado.Enabled = false;
        radioBecarioParcial.Enabled = false;
        radioSinAccesoBecario.Enabled = false;
        radioEncargadoParcial.Enabled = false;
        radioSinAccesoEncargado.Enabled = false;
        radioAsignacionBecario.Enabled = false;
        radioAsignacionEncargado.Enabled = false;
        noControlHoras.Checked = true;
    }

    protected void radioEncargado_CheckedChanged(object sender, EventArgs e)
    {
        String temp = txtNombrePerfil.Text;
        vaciarCampos();
        txtNombrePerfil.Text = temp;
        habilitarCampos(true);
        //permisos por defecto
        radioSinAccesoBecario.Checked = true;
        radioEncargadoParcial.Checked = true;
        radioControlEncargado.Checked = true;
        radioAsignacionEncargado.Checked = true;
        radioCuentaParcial.Checked = true;
        //permisos no admitidos
        radioBecarioParcial.Enabled = false;
        radioControlBecario.Enabled = false;
        radioAsignacionBecario.Enabled = false;
    }

    protected void radioBecario_CheckedChanged(object sender, EventArgs e)
    {
        String temp = txtNombrePerfil.Text;
        vaciarCampos();
        txtNombrePerfil.Text = temp;
        habilitarCampos(true);
        radioSinAccesoEncargado.Checked = true;
        radioBecarioParcial.Checked = true;
        radioControlBecario.Checked = true;
        radioAsignacionBecario.Checked = true;
        radioCuentaParcial.Checked = true;
            //permisos no admitidos
        radioEncargadoParcial.Enabled = false;
        radioControlEncargado.Enabled = false;
        radioAsignacionEncargado.Enabled = false;
    }
}
