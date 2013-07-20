using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using System.Data;


public partial class Encargados : System.Web.UI.Page
{
    private static CommonServices commonService;
    private static EmailServices servicioCorreo;
    private static ControladoraCuentas controladoraCuentas = new ControladoraCuentas();
    private static ControladoraEncargado controladora = new ControladoraEncargado();
    private static int rowIndex; 
    private static List<Encargado> lsEncargados; 

    private static int modo = 0; //1=insertar 2=eliminar 3=modificar
    private static Object[] datosOriginalesEncargado = new Object[9];

    //Establece la vista actual de acuerdo a los permisos que el usuario posee
    protected void Page_Load(object sender, EventArgs e)
    {
        
        commonService = new CommonServices(UpdateInfo);
        servicioCorreo = new EmailServices();

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
            if (permisos.Contains(3))
            {
                permiso = 3;
            }
            else
            {
                if (permisos.Contains(4))
                {
                    permiso = 4;
                }
            }

            switch (permiso)
            {
                case 3: // Vista Completa
                    {
                        MultiViewEncargado.ActiveViewIndex = 0;
                        if (!IsPostBack)
                        {
                            this.llenarGridEncargados(1);
                        }
                    } break;

                case 4: // Vista Parcial
                    {
                        MultiViewEncargado.ActiveViewIndex = 1;
                        if (!IsPostBack)
                        {
                            llenarCamposEncargadoVistaParcial();
                        }

                    } break;

                default: // Vista sin permiso
                    {
                        MultiViewEncargado.ActiveViewIndex = 2;
                    } break;
            }
        }
    }

    //Metodo que se ejecuta al presionar aceptar en el popUp
    protected void btnInvisible1_Click(object sender, EventArgs e)
    {
        TextInfo miTexto = CultureInfo.CurrentCulture.TextInfo;

        //Empaquetar información
        Object[] nuevoEncargado = new Object[9];
        
        nuevoEncargado[0] = commonService.procesarStringDeUI(txtCedula.Text);
        nuevoEncargado[1] = miTexto.ToTitleCase(commonService.procesarStringDeUI(txtNombre.Text));
        nuevoEncargado[2] = miTexto.ToTitleCase(commonService.procesarStringDeUI(txtPrimerApellido.Text));
        nuevoEncargado[3] = miTexto.ToTitleCase(commonService.procesarStringDeUI(txtSegundoApellido.Text));
        nuevoEncargado[4] = commonService.procesarStringDeUI(txtCorreo.Text);
        nuevoEncargado[5] = commonService.procesarStringDeUI(txtTelFijo.Text);
        nuevoEncargado[6] = commonService.procesarStringDeUI(txtCel.Text);
        nuevoEncargado[7] = commonService.procesarStringDeUI(txtOtroTel.Text);
        nuevoEncargado[8] = commonService.procesarStringDeUI(txtPuesto.Text);

        string mensajeResultado = "Exito";
        if (modo == 1)
        {
            mensajeResultado = controladora.ejecutar(modo, nuevoEncargado, null);
            if (mensajeResultado == "Exito")
            {
                //luego de insertar un encargado se crea una cuenta para este
                string r = cearCuenta(this.txtCedula.Text, this.txtNombre.Text.ToLower(), this.txtPrimerApellido.Text.ToLower(), this.txtSegundoApellido.Text, this.txtCorreo.Text);
                if (r == "Exito")
                {
                    commonService.mensajeJavascript("Encargado insertado con éxito y su cuenta ha sido creada correctamente!", "Confirmación");
                    this.llenarGridEncargados(1);
                }
                else
                {
                    commonService.mensajeJavascript("Encargado insertado con éxito, pero se presentó un error al crear la cuenta!", "ERROR");
                    this.llenarGridEncargados(1);
                }
            }
            else
            {
                commonService.mensajeJavascript(mensajeResultado, "ERROR");
            }
        }
        else
        {
            if (modo == 3)
            {
                mensajeResultado = controladora.ejecutar(modo, nuevoEncargado, datosOriginalesEncargado);
                if (mensajeResultado == "Exito")
                {
                    commonService.mensajeJavascript("Encargado modificado con éxito!", "Confirmación");
                    this.llenarGridEncargados(1);
                    mensajeResultado = "ExiteUpdate";
                }
                else
                {
                    commonService.mensajeJavascript("No fue posible modificar el encargado actual!", "ERROR");
                }
            }
        }
        modo = 0;
        commonService.cerrarPopUp("PopUpEncargado");

        if (mensajeResultado == "Exito")
        {
            //correo, nombreCompleto, pass, usuario
            // Abrir mensaje de mandar correo
            commonService.mensajeEspera("Enviando correo de confirmación al/la encargad@", "Enviando correo");
            commonService.correrJavascript("$('.btnInvisibleEnviarCorreo').click();");
        }
    }

    //Enviar correo
    protected void btnInvisibleEnviarCorreo_Click(object sender, EventArgs e)
    {
        Button btn = (Button)sender;
        string[] parametros = (btn.CommandArgument.ToString()).Split(',');
        string correo = parametros[0];
        string nombreCompleto = parametros[1];
        string pass = parametros[2];
        string usuario = parametros[3];
        if (servicioCorreo.enviarCorreoCuentaCreada(correo, nombreCompleto, pass, usuario))
        {

        }
        commonService.cerrarMensajeEspera();
    }

    //Metodo que se ejecuta el eliminar un encargado desde el popUp
    protected void btnInvisible2_Click(object sender, EventArgs e)
    {
        Object[] encargado = new Object[10];
        encargado[0] = lsEncargados[rowIndex].Cedula;
        encargado[1] = lsEncargados[rowIndex].Nombre;
        encargado[2] = lsEncargados[rowIndex].Apellido1;
        encargado[3] = lsEncargados[rowIndex].Apellido2;
        encargado[4] = lsEncargados[rowIndex].Correo;
        encargado[5] = lsEncargados[rowIndex].TelefonoFijo;
        encargado[6] = lsEncargados[rowIndex].TelefonoCelular;
        encargado[7] = lsEncargados[rowIndex].OtroTelefono;
        encargado[8] = lsEncargados[rowIndex].Puesto;
        encargado[9] = lsEncargados[rowIndex].Activo;

        string resultadoEliminarAsignación = "-1";
        if (controladora.tieneAsignaciones(lsEncargados[rowIndex].Cedula) == true)
        {
            resultadoEliminarAsignación = eliminaAsignaciones();
        }


        if ((resultadoEliminarAsignación.Equals("-1")) || (resultadoEliminarAsignación.Equals("Exito")))
        {

            string mensajeResultado = controladora.ejecutar(modo, null, encargado);
            if (mensajeResultado == "Exito")
            {
                commonService.mensajeJavascript("El encargado ha sido eliminado correctamente", "Aviso");
                commonService.correrJavascript("$('#PopUpEncargado').dialog('close');");
                this.llenarGridEncargados(1);
            }
            else {
                commonService.mensajeJavascript("Se ha producido un error al eliminar el encargado", "Error");
            }

        }else{
          
            commonService.mensajeJavascript("No se pudo eliminar al encargado porque hubo un problema al eliminar las asignaciones", "Error");
        }

       
        
    }

    /*
    * -----------------------------------------------------------------------
    * BUTTON: CLICKS
    * -----------------------------------------------------------------------
    */
    // INSERTAR CLICK
    protected void btnInsertarEncargado_Click(object sender, EventArgs e)
    {
        modo = 1;
        commonService.abrirPopUp("PopUpEncargado", "Insertar Nuevo Encargado");
        commonService.mostrarPrimerBotonDePopUp("PopUpEncargado"); 
        mostrarBotonesPrincipales(false);
        activarInputsPrincipales(true);
        borrarInputsPrincipales();
    }

    // MODIFICAR CLICK
    protected void btnModificarEncargado_Click(object sender, EventArgs e)
    {
        commonService.mostrarPrimerBotonDePopUp("PopUpEncargado"); 

        modo = 3;

        Object[] encargadoActual = new Object[9];
        encargadoActual[0] = lsEncargados[rowIndex].Cedula;
        encargadoActual[1] = lsEncargados[rowIndex].Nombre;
        encargadoActual[2] = lsEncargados[rowIndex].Apellido1;
        encargadoActual[3] = lsEncargados[rowIndex].Apellido2;
        encargadoActual[4] = lsEncargados[rowIndex].Correo;
        encargadoActual[5] = lsEncargados[rowIndex].TelefonoFijo;
        encargadoActual[6] = lsEncargados[rowIndex].TelefonoCelular;
        encargadoActual[7] = lsEncargados[rowIndex].OtroTelefono;
        encargadoActual[8] = lsEncargados[rowIndex].Puesto;
        datosOriginalesEncargado = encargadoActual;

        activarInputsPrincipales(true);
        mostrarBotonesPrincipales(false);
    }
    
    // ELIMINAR CLICK
    protected void btnEliminarEncargado_Click(object sender, EventArgs e)
    {
        //commonService.abrirPopUp("PopUpEliminarEncargado", "Eliminar Encargado");
        modo = 2;
        if (controladora.tieneAsignaciones(lsEncargados[rowIndex].Cedula) == true)
        {
          commonService.abrirPopUpPersonalizado("PopUpEliminarEncargado", "Eliminar Encargado", "Este encargado tiene una o más asignaciones en el presente semestre por lo que al eliminarlo se eliminarán también dichas asignaciones , ¿ está seguro de que desea continuar ? ");   
        }
        else {
          commonService.abrirPopUpPersonalizado("PopUpEliminarEncargado", "Eliminar Encargado", "¿ Está seguro de que desea eliminar al encargado seleccionado ? ");
        }
       
    }

    // BUSCAR CLICK
    protected void btnBuscar_Click(object sender, EventArgs e)
    {
        this.llenarGridEncargados(2);
    }

    // AYUDA CLICK
    protected void btnAyuda_Click(object sender, EventArgs e)
    {
        commonService.abrirPopUp("PopUpAyuda", "Ayuda");
        commonService.esconderPrimerBotonDePopUp("PopUpAyuda");
    }
    public String eliminaAsignaciones()
    {
        string ced = lsEncargados[rowIndex].Cedula;
        int año = commonService.getAñoActual();
        int periodo = commonService.getPeriodoActual();
        return controladora.eliminaAsignacionesActuales(ced, periodo, año);
    }


    /*
    * -----------------------------------------------------------------------
    * AUXILIAR: ENABLE/DISABLE/ERASE
    * -----------------------------------------------------------------------
    */
    //Define que botones se van a mostrar
    protected void mostrarBotonesPrincipales(Boolean mostrar)
    {
        if (mostrar)
        {
            this.btnModificarEncargado.Visible = true;
            this.btnEliminarEncargado.Visible = true;
        }
        else
        {
            this.btnModificarEncargado.Visible = false;
            this.btnEliminarEncargado.Visible = false;
        }
    }

    protected void activarInputsPrincipales(Boolean mostrar)
    {
        if (mostrar)
        {
            txtCedula.Enabled = true;
            txtNombre.Enabled = true;
            txtPrimerApellido.Enabled = true;
            txtSegundoApellido.Enabled = true;
            txtTelFijo.Enabled = true;
            txtCel.Enabled = true;
            txtOtroTel.Enabled = true;
            txtCorreo.Enabled = true;
            txtPuesto.Enabled = true;
        }
        else
        {
            txtCedula.Enabled = false;
            txtNombre.Enabled = false;
            txtPrimerApellido.Enabled = false;
            txtSegundoApellido.Enabled = false;
            txtTelFijo.Enabled = false;
            txtCel.Enabled = false;
            txtOtroTel.Enabled = false;
            txtCorreo.Enabled = false;
            txtPuesto.Enabled = false;
        }
    }

    protected void activarInputsPrincipalesParciales(Boolean mostrar)
    {
        if (mostrar)
        {
            txtCedulaP.Enabled = true;
            txtNombreP.Enabled = true;
            txtPrimerApellidoP.Enabled = true;
            txtSegundoApellidoP.Enabled = true;
            txtTelFijoP.Enabled = true;
            txtCelP.Enabled = true;
            txtOtroTelP.Enabled = true;
            txtCorreoP.Enabled = true;
            txtPuestoP.Enabled = true;
        }
        else
        {
            txtCedulaP.Enabled = false;
            txtNombreP.Enabled = false;
            txtPrimerApellidoP.Enabled = false;
            txtSegundoApellidoP.Enabled = false;
            txtTelFijoP.Enabled = false;
            txtCelP.Enabled = false;
            txtOtroTelP.Enabled = false;
            txtCorreoP.Enabled = false;
            txtPuestoP.Enabled = false;
        }
    }

    protected void borrarInputsPrincipales()
    {

        txtCedula.Text = "";
        txtNombre.Text = "";
        txtPrimerApellido.Text = "";
        txtSegundoApellido.Text = "";
        txtTelFijo.Text = "";
        txtCel.Text = "";
        txtOtroTel.Text = "";
        txtCorreo.Text = "";
        txtPuesto.Text = "";
    }

    /*
    * -----------------------------------------------------------------------
    * METODOS: Grid Encargados
    * -----------------------------------------------------------------------
    */
    protected void llenarGridEncargados(int modo) //1=Todos los encargados 2=PorCriteriosDeBusqueda
    {
        if (modo == 1)
        {
            lsEncargados = controladora.consultarTablaEncargados();
        }
        else
        {
            string criterioDeBusqueda = "%" + this.txtBuscarEncargado.Text + "%";
            lsEncargados = controladora.ObtenerTablaEncargadosPorBusquedaSelectiva(criterioDeBusqueda);
        }

        DataTable tablaEncargados = crearTablaEncargados();
        DataRow newRow;
        if (lsEncargados.Count > 0)
        {
            for (int i = 0; i < lsEncargados.Count; ++i)
            {
                newRow = tablaEncargados.NewRow();
                newRow["Nombre"] = lsEncargados[i].Nombre + " " + lsEncargados[i].Apellido1 + " " + lsEncargados[i].Apellido2;
                newRow["Cedula"] = lsEncargados[i].Cedula;
                newRow["Correo"] = lsEncargados[i].Correo;
                newRow["Celular"] = lsEncargados[i].TelefonoCelular;
                if (lsEncargados[i].TelefonoFijo != "")
                {
                    newRow["Telefono"] = lsEncargados[i].TelefonoFijo;
                }
                else
                {
                    if (lsEncargados[i].OtroTelefono != "")
                    {
                        newRow["Telefono"] = lsEncargados[i].OtroTelefono;
                    }
                }
              
                tablaEncargados.Rows.InsertAt(newRow, i);
            }                 
        }else {
            newRow = tablaEncargados.NewRow();
            newRow["Nombre"] = "-";
            newRow["Cedula"] = "-";
            newRow["Correo"] = "-";
            newRow["Celular"] = "-";
            newRow["Telefono"] = "-";

            tablaEncargados.Rows.InsertAt(newRow, 0);
        }
        this.GridEncargados.DataSource = tablaEncargados;
        this.GridEncargados.DataBind();
        this.HeadersCorrectosGridEncargados();
    }


    protected DataTable crearTablaEncargados()
    {

        DataTable dt = new DataTable();
        DataColumn column;

        column = new DataColumn();
        column.DataType = System.Type.GetType("System.String");
        column.ColumnName = "Nombre";
        dt.Columns.Add(column);

        column = new DataColumn();
        column.DataType = System.Type.GetType("System.String");
        column.ColumnName = "Cedula";
        dt.Columns.Add(column);

        column = new DataColumn();
        column.DataType = System.Type.GetType("System.String");
        column.ColumnName = "Correo";
        dt.Columns.Add(column);

        column = new DataColumn();
        column.DataType = System.Type.GetType("System.String");
        column.ColumnName = "Celular";
        dt.Columns.Add(column);

        column = new DataColumn();
        column.DataType = System.Type.GetType("System.String");
        column.ColumnName = "Telefono";
        dt.Columns.Add(column);

               
        return dt;
    }

    private void HeadersCorrectosGridEncargados()
    {
        this.GridEncargados.HeaderRow.BackColor = System.Drawing.Color.FromArgb(4562432);
        this.GridEncargados.HeaderRow.ForeColor = System.Drawing.Color.White;
        this.GridEncargados.HeaderRow.Cells[1].Text = "Nombre";
        this.GridEncargados.HeaderRow.Cells[2].Text = "Cédula";
        this.GridEncargados.HeaderRow.Cells[3].Text = "Correo";
        this.GridEncargados.HeaderRow.Cells[4].Text = "Celular";
        this.GridEncargados.HeaderRow.Cells[5].Text = "Teléfono";
    }

    /*
   * -----------------------------------------------------------------------
   * AUXILIAR: Grid Encargados
   * -----------------------------------------------------------------------
   */
    protected void GridEncargados_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        this.GridEncargados.PageIndex = e.NewPageIndex;
        this.GridEncargados.DataBind();
        this.HeadersCorrectosGridEncargados();
        llenarGridEncargados(1);
    }    

    protected void GridEncargados_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        Guid gMessage = Guid.NewGuid();
        string sMessage = "";
        switch (e.CommandName)
        {
            case "btnSeleccionarTupla_Click":
                {
                    rowIndex = Convert.ToInt32(e.CommandArgument);
                    int pageIndex = this.GridEncargados.PageIndex;
                    int pageSize = this.GridEncargados.PageSize;
                    rowIndex += (pageIndex * pageSize);

                    commonService.abrirPopUp("PopUpEncargado", "Consultar Encargado");
                    commonService.esconderPrimerBotonDePopUp("PopUpEncargado"); 

                    mostrarBotonesPrincipales(true);
                    activarInputsPrincipales(false);

                    txtCedula.Text = lsEncargados[rowIndex].Cedula;
                    txtNombre.Text = lsEncargados[rowIndex].Nombre;
                    txtPrimerApellido.Text = lsEncargados[rowIndex].Apellido1;
                    txtSegundoApellido.Text = lsEncargados[rowIndex].Apellido2;
                    txtTelFijo.Text = lsEncargados[rowIndex].TelefonoFijo;
                    txtCel.Text = lsEncargados[rowIndex].TelefonoCelular;
                    txtOtroTel.Text = lsEncargados[rowIndex].OtroTelefono;
                    txtCorreo.Text = lsEncargados[rowIndex].Correo;
                    txtPuesto.Text = lsEncargados[rowIndex].Puesto;
                } break;
        }
        ScriptManager.RegisterStartupScript(UpdateInfo, UpdateInfo.GetType(), gMessage.ToString(), sMessage, true);
    }

    protected void GridEncargados_SelectedIndexChanging(object sender, GridViewSelectEventArgs e)
    {       
        this.GridEncargados.SelectedIndex = e.NewSelectedIndex;
        this.GridEncargados.DataBind();
    }
     /*
   * -----------------------------------------------------------------------
   * Vista Parcial
   * -----------------------------------------------------------------------
   */
    void llenarCamposEncargadoVistaParcial() 
    {
        
        lsEncargados = controladora.ObtenerDatosCuenta(Session["Cuenta"].ToString());
        
        txtCedulaP.Text = lsEncargados[0].Cedula;
        txtNombreP.Text = lsEncargados[0].Nombre;
        txtPrimerApellidoP.Text = lsEncargados[0].Apellido1;
        txtSegundoApellidoP.Text = lsEncargados[0].Apellido2;
        txtTelFijoP.Text = lsEncargados[0].TelefonoFijo;
        txtCelP.Text = lsEncargados[0].TelefonoCelular;
        txtOtroTelP.Text = lsEncargados[0].OtroTelefono;
        txtCorreoP.Text = lsEncargados[0].Correo;
        txtPuestoP.Text = lsEncargados[0].Puesto;
    }

    //Al presiona el boton para modificar el encargado desde la vista parcial
    protected void btnModificarEncargadoParcial_Click(object sender, EventArgs e)
    {
        modo = 3;

        this.btnAceptar.Visible = true;
        this.btnCancelar.Visible = true;
        activarInputsPrincipalesParciales(true);
        this.btnModificarEncargadoP.Visible = false;
    }

    //Metodo que se ejecuta al precionar aceptar luego de modificar un encargado en la vista parcial
    protected void btnModificarEncargadoParcialAceptar_Click(object sender, EventArgs e)
    {

        Object[] encargadoActual = new Object[9];
        encargadoActual[0] = this.txtCedulaP.Text;
        encargadoActual[1] = this.txtNombreP.Text;
        encargadoActual[2] = this.txtPrimerApellidoP.Text;
        encargadoActual[3] = this.txtSegundoApellidoP.Text;
        encargadoActual[4] = this.txtCorreoP.Text;
        encargadoActual[5] = this.txtTelFijoP.Text;
        encargadoActual[6] = this.txtCelP.Text;
        encargadoActual[7] = this.txtOtroTelP.Text;
        encargadoActual[8] = this.txtPuestoP.Text;
        
        Object[] encargadoOriginal = new Object[9];
        encargadoOriginal[0] = lsEncargados[0].Cedula;
        encargadoOriginal[1] = lsEncargados[0].Nombre;
        encargadoOriginal[2] = lsEncargados[0].Apellido1;
        encargadoOriginal[3] = lsEncargados[0].Apellido2;
        encargadoOriginal[4] = lsEncargados[0].Correo;
        encargadoOriginal[5] = lsEncargados[0].TelefonoFijo;
        encargadoOriginal[6] = lsEncargados[0].TelefonoCelular;
        encargadoOriginal[7] = lsEncargados[0].OtroTelefono;
        encargadoOriginal[8] = lsEncargados[0].Puesto;

        controladora.ejecutar(modo, encargadoActual, encargadoOriginal);

        this.btnAceptar.Visible = false;
        this.btnCancelar.Visible = false;
        this.btnModificarEncargadoP.Visible = true;
        activarInputsPrincipalesParciales(false);

        llenarCamposEncargadoVistaParcial();

        modo = 0;
    }

    //Metodo que se ejecuta al precionar cancelar de haber presionado modificar para un encargado en la vista parcial
    protected void btnModificarEncargadoParcialCancelar_Click(object sender, EventArgs e)
    {
        modo = 0;

        this.btnAceptar.Visible = false;
        this.btnCancelar.Visible = false;
        activarInputsPrincipalesParciales(false);
        this.btnModificarEncargadoP.Visible = true;

        llenarCamposEncargadoVistaParcial();

    }


    //Pide crear una cuenta para un encargado 
    protected string cearCuenta(string cedula, string nombre, string apellido, string apellido2, string correo)
    {

        string resultado = "Exito";
        CultureInfo currentCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
        TextInfo currentInfo = currentCulture.TextInfo;

        int lg = cedula.Length - 3;
        string ced = cedula.Substring(lg, 3);
        string usuario = nombre + "." + apellido + ced;
        string pass = commonService.getContrasena(nombre, apellido, ced);
        string nombreCompleto = (currentInfo.ToTitleCase(nombre) + " " + currentInfo.ToTitleCase(apellido) + " " + currentInfo.ToTitleCase(apellido2)).Trim();

        Object[] datos = new Object[4];
        datos[0] = usuario;//this.txtUsuario.Text;
        datos[1] = pass;  //this.cntUsuario.Text;
        datos[2] = "";
        datos[3] = cedula;

        Object[] datosPerfil = new Object[2];
        datosPerfil[0] = nombre + "." + apellido + ced;
        datosPerfil[1] = "Encargado"; // REVISAR !!!

        string r1 = controladoraCuentas.ejecutar(1, datos, null);
        string r2;
        if (r1.Equals(""))
        { // Exito

            r2 = controladoraCuentas.ejecutarAsociacion(1, datosPerfil, null);
            if (!r2.Equals(""))
            {
                resultado = "error";
            }

        }
        else
        {
            resultado = "error";
        }

        if (resultado != "error")
        {
            
            this.btnInvisibleEnviarCorreo.CommandArgument = correo + "," + nombreCompleto + "," + pass + "," + usuario;
            
        }
        return resultado;

    }
}

