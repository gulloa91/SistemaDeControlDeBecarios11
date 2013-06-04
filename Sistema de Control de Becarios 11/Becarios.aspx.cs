using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

public partial class Becarios : System.Web.UI.Page
{

    private static CommonServices commonService;
    private static EmailServices servicioCorreo;

    int permiso = 0;
    static int modoEjecucion=0;
    private List<Becario> listaBecarios;
    private static Object[] datosViejos;
    private static int rowIndex; 
    private static ControladoraBecarios controladoraBecarios = new ControladoraBecarios();
    private static ControladoraCuentas controladoraCuentas = new ControladoraCuentas();
  
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
             if (permisos != null)
             {
                 if (permisos.Contains(1))
                 {
                     permiso = 1;
                 }
                 else
                 {
                     if (permisos.Contains(2))
                     {
                         permiso = 2;
                     }
                 }
             }

             /* Query to user validation */
             switch (permiso)
             {
                 case 1: // Vista Completa
                     {
                         MultiViewBecario.ActiveViewIndex = 0;
                         llenarGridBecarios(1);
                         if (!Page.IsPostBack)
                         {
                             this.gridLenguajesProgC.DataSource = crearListaLenguajes();
                             this.gridLenguajesProgC.DataBind();


                             this.gridIdiomasC.DataSource = crearListaIdiomas();
                             this.gridIdiomasC.DataBind();

                             this.gridAreasInteresC.DataSource = crearListaAreas();
                             this.gridAreasInteresC.DataBind();

                             this.gridCualidadesC.DataSource = crearListaCualidades();
                             this.gridCualidadesC.DataBind();

                         }

                     } break;

                 case 2: // Vista Parcial
                     {
                         MultiViewBecario.ActiveViewIndex = 1;
                         correrJavascript("crearTabsP();");

                         //cargarCamposBecarioActual
                         habilitarCampos(false, 1);
                         this.btnAceptarP.Enabled = false;
                         this.btnCancelarP.Enabled = false;

                         if (!Page.IsPostBack)
                         {
                             this.gridLenguajesProg.DataSource = crearListaLenguajes();
                             this.gridLenguajesProg.DataBind();

                             this.gridIdiomas.DataSource = crearListaIdiomas();
                             this.gridIdiomas.DataBind();

                             this.gridAreasInteres.DataSource = crearListaAreas();
                             this.gridAreasInteres.DataBind();

                             this.gridCualidades.DataSource = crearListaCualidades();
                             this.gridCualidades.DataBind();

                         }

                     } break;

                 default: // Vista sin permiso
                     {
                         MultiViewBecario.ActiveViewIndex = 2;
                     } break;
             }
         }

    }



    protected void correrJavascript(String funcion)
    {
        Guid gMessage = Guid.NewGuid();
        string sMessage = funcion;

        ScriptManager.RegisterStartupScript(UpdateInfo, UpdateInfo.GetType(), gMessage.ToString(), sMessage, true);
    }



  /*
   * -----------------------------------------------------------------------
   * BUTTON: CLICKS
   * -----------------------------------------------------------------------
   */


    //Metodo que se invoca al dar click en el botón aceptar del 'popUp'
    protected void btnInvisible1_Click(object sender, EventArgs e)
    {

        if ((modoEjecucion == 1) || (modoEjecucion == 2))
        {

            TextInfo miTexto = CultureInfo.CurrentCulture.TextInfo;

            Object[] datos;

            datos = new Object[10];
            datos[0] = "";
            datos[1] = miTexto.ToTitleCase(this.txtNombre.Text.ToLower());
            datos[2] = miTexto.ToTitleCase(this.txtApellido1.Text.ToLower());
            datos[3] = miTexto.ToTitleCase(this.txtApellido2.Text.ToLower());
            datos[4] = this.txtCarne.Text;
            datos[5] = this.txtCedula.Text;
            datos[6] = this.txtTelFijo.Text;
            datos[7] = this.txtCel.Text;
            datos[8] = this.txtOtroTel.Text;
            datos[9] = this.txtCorreo.Text;

            string resultado = "";
            if(modoEjecucion==1){
                 resultado = controladoraBecarios.ejecutar(modoEjecucion, datos, null);
            }else{
                 resultado = controladoraBecarios.ejecutar(modoEjecucion, datos, datosViejos);
            }

            if (resultado.Equals("Exito")&&(modoEjecucion==1))
            {

                //luego de insertar un becario se crea una cuenta para este
                string r = cearCuenta(this.txtCedula.Text, this.txtNombre.Text.ToLower(), this.txtApellido1.Text.ToLower(), this.txtApellido2.Text, this.txtCorreo.Text);
                if (r.Equals("Exito"))
                {
                   commonService.mensajeJavascript("El becario ha sido ingresado correctamente y su cuenta ha sido creada", "Éxito");
                   //string mensaje = "Bienvenido al Sistema de Control de Becarios 11 de la ECCI. Favor revisar y completar sus datos personales lo más prnto posible. También le recordamos cambiar su contraseña por su propia seguridad";
                   //servicioCorreo.enviarCorreo(this.txtCorreo.Text,"Bienvenido",mensaje);
                }
                else {
                   commonService.mensajeJavascript("El becario ha sido ingresado correctamente pero hubo un problema al crear la cuenta", "Aviso");
                }
                
                
            }
            else {

                if (resultado.Equals("Exito") && (modoEjecucion == 2))
                {

                    commonService.mensajeJavascript("Se ha modificado correctamente la información solcitada", "Éxito");
                }
                else
                {

                    if (resultado.Equals("Error1"))
                    {
                        commonService.mensajeJavascript("Ya existe un becario con la cédula digitada", "Error");
                    }
                    else
                    {
                        commonService.mensajeJavascript("Se ha producido un error. Favor intentar más tarde", "Error");
                    }
                }
            }
            
            llenarGridBecarios(1);
            habilitarBotonesPrincipales(true);
        }

        correrJavascript("cerrarPopUp();");
    }


    //Método que se invoca al confirmar la eliminación de un becario
    protected void btnInvisible2_Click(object sender, EventArgs e)
    {

        
        Object[] datos = new Object[1];

        datos[0] = this.txtCedula.Text;

        string resultado = controladoraBecarios.ejecutar(3,datos,null);
        this.gridBecarios.SelectedIndex = -1;

        correrJavascript("cerrarPopUp();");
        commonService.mensajeJavascript(resultado, "Aviso");
        llenarGridBecarios(1);

    }


    //Método que se invoca el dar click al botón 'cancelar' del 'popUp'
    protected void btnInvisible3_Click(object sender, EventArgs e)
    {

        habilitarBotonesPrincipales(true);
    }



    // Método que se invoca al dar click al botón 'insertar'
    // Abre el popUp que permite ingresar el nuevo becario , limpia y habilita los campos
    protected void btnInsertarEncargado_Click(object sender, EventArgs e)
    {
        correrJavascript("abrirPopUp();");
        mostrarBotonesPrincipales(false);
        vaciarCampos(0);
        habilitarCampos(true,0);
        modoEjecucion = 1;
    }


    //Metodo que pide llenar el grid pero bajo un criterio de selección (búsqueda) expresado por el usuario
    protected void btnBuscarBecario_Click(object sender, EventArgs e)
    {

        llenarGridBecarios(2);    
    }




    // Método que se invoca al dar click al botón 'modificar'
    // Toma los datos actuales y luego habilita los campos para editarlos
    protected void btnModificarBecario_Click(object sender, EventArgs e)
    {

        TextInfo miTexto = CultureInfo.CurrentCulture.TextInfo;

        correrJavascript("crearTabs();");
        modoEjecucion = 2;
        datosViejos = new Object[10];
        datosViejos[0] = "";
        datosViejos[1] = commonService.procesarStringDeUI(miTexto.ToTitleCase(this.txtNombre.Text.ToLower()));
        datosViejos[2] = commonService.procesarStringDeUI(miTexto.ToTitleCase(this.txtApellido1.Text.ToLower()));
        datosViejos[3] = commonService.procesarStringDeUI(miTexto.ToTitleCase(this.txtApellido2.Text.ToLower()));
        datosViejos[4] = this.txtCarne.Text;
        datosViejos[5] = this.txtCedula.Text;
        datosViejos[6] = this.txtTelFijo.Text;
        datosViejos[7] = this.txtCel.Text;
        datosViejos[8] = this.txtOtroTel.Text;
        datosViejos[9] = commonService.procesarStringDeUI(this.txtCorreo.Text);
        habilitarBotonesPrincipales(false);
        habilitarCampos(true, 0); 
    }


    // Activa el 'popUp' para confirmar la eliminación
    protected void btnEliminarBecario_Click(object sender, EventArgs e)
    {
        correrJavascript("abrirPopUpEliminar();");
    }



    // Método que se invoca al dar click al botón 'modificar' en la vista parcial 
    // Toma los datos actuales y luego habilita los campos para editarlos
    protected void btnModificarDatosBecarioP_Click(object sender, EventArgs e)
    {

        TextInfo miTexto = CultureInfo.CurrentCulture.TextInfo;

        correrJavascript("destruyeTabsP();");
        correrJavascript("crearTabsP();");

        datosViejos = new Object[10];
        datosViejos[0] = "";
        datosViejos[1] = commonService.procesarStringDeUI(miTexto.ToTitleCase(this.txtNombreP.Text.ToLower()));
        datosViejos[2] = commonService.procesarStringDeUI(miTexto.ToTitleCase(this.txtApellido1P.Text.ToLower()));
        datosViejos[3] = commonService.procesarStringDeUI(miTexto.ToTitleCase(this.txtApellido2P.Text.ToLower()));
        datosViejos[4] = this.txtCarneP.Text;
        datosViejos[5] = this.txtCedulaP.Text;
        datosViejos[6] = this.txtTelFijoP.Text;
        datosViejos[7] = this.txtCelP.Text;
        datosViejos[8] = this.txtOtroTelP.Text;
        datosViejos[9] = commonService.procesarStringDeUI(this.txtCorreoP.Text);
        habilitarCampos(true, 1);
        this.btnAceptarP.Enabled = true;
        this.btnCancelarP.Enabled = true;
    }


    //Método que se invoca al dar click al botón 'CANCELAR' en la vista parcial 
    //Vacia campos
    protected void btnCancelarP_Click(object sender, EventArgs e) 
    {

        correrJavascript("destruyeTabsP();");
        correrJavascript("crearTabsP();");
        vaciarCampos(1);
        habilitarCampos(false, 1);
        this.btnAceptarP.Enabled = false;
        this.btnCancelarP.Enabled = false;
    }


    //Método que se invoca al dar click al botón 'ACEPTAR' en la vista parcial 
    //Toma los datos ingresados y pide modificar la información del becario correspondiente
    protected void btnAceptarP_Click()
    {

        TextInfo miTexto = CultureInfo.CurrentCulture.TextInfo;

        Object[] datos;

        datos = new Object[10];
        datos[0] = "";
        datos[1] = miTexto.ToTitleCase(this.txtNombreP.Text.ToLower());
        datos[2] = miTexto.ToTitleCase(this.txtApellido1P.Text.ToLower());
        datos[3] = miTexto.ToTitleCase(this.txtApellido2P.Text.ToLower());
        datos[4] = this.txtCarneP.Text;
        datos[5] = this.txtCedulaP.Text;
        datos[6] = this.txtTelFijoP.Text;
        datos[7] = this.txtCelP.Text;
        datos[8] = this.txtOtroTelP.Text;
        datos[9] = this.txtCorreoP.Text;

        
        String resultado = controladoraBecarios.ejecutar(2, datos, null); 

        commonService.mensajeJavascript(resultado, "Aviso");
        this.btnAceptarP.Enabled = false;
        this.btnCancelarP.Enabled = false;

    }


    /*
    * -----------------------------------------------------------------------
    * AUXILIAR: ENABLE/DISABLE
    * -----------------------------------------------------------------------
    */


    //Muestra u oculta botones
    protected void mostrarBotonesPrincipales(Boolean mostrar)
    {
        if (mostrar)
        {
            this.btnModificarBecarioDatos.Visible = true;
            this.btnEliminarBecarioDatos.Visible = true;
            this.btnModificarBecarioPerfil.Visible = true;
            this.btnEliminarBecarioPerfil.Visible = true;
        }
        else
        {
            this.btnModificarBecarioDatos.Visible = false;
            this.btnEliminarBecarioDatos.Visible = false;
            this.btnModificarBecarioPerfil.Visible = false;
            this.btnEliminarBecarioPerfil.Visible = false;
        }
    }


    //Habilita o deshabiita botones
    protected void habilitarBotonesPrincipales(Boolean habilitar)
    {

         if (habilitar)
         {
             this.btnModificarBecarioDatos.Enabled = true;
             this.btnEliminarBecarioDatos.Enabled = true;
             this.btnModificarBecarioPerfil.Enabled = true;
             this.btnEliminarBecarioPerfil.Enabled = true;
          }
          else
          {
              this.btnModificarBecarioDatos.Enabled = false;
              this.btnEliminarBecarioDatos.Enabled = false;
              this.btnModificarBecarioPerfil.Enabled = false;
              this.btnEliminarBecarioPerfil.Enabled = false;
          }
     

    }


    //Habilita o deshabiita campos
    protected void habilitarCampos(Boolean habilitar,int p)
    {

        if (p == 0)
        {
            if (habilitar)
            {

                this.txtNombre.Enabled = true;
                this.txtApellido1.Enabled = true;
                this.txtApellido2.Enabled = true;
                this.txtCarne.Enabled = true;
                this.txtCedula.Enabled = true;
                this.txtTelFijo.Enabled = true;
                this.txtCel.Enabled = true;
                this.txtOtroTel.Enabled = true;
                this.txtCorreo.Enabled = true;

            }
            else
            {

                this.txtNombre.Enabled = false;
                this.txtApellido1.Enabled = false;
                this.txtApellido2.Enabled = false;
                this.txtCarne.Enabled = false;
                this.txtCedula.Enabled = false;
                this.txtTelFijo.Enabled = false;
                this.txtCel.Enabled = false;
                this.txtOtroTel.Enabled = false;
                this.txtCorreo.Enabled = false;
            }
        }
        else
        {

            if (habilitar)
            {

                this.txtNombreP.Enabled = true;
                this.txtApellido1P.Enabled = true;
                this.txtApellido2P.Enabled = true;
                this.txtCarneP.Enabled = true;
                this.txtCedulaP.Enabled = true;
                this.txtTelFijoP.Enabled = true;
                this.txtCelP.Enabled = true;
                this.txtOtroTelP.Enabled = true;
                this.txtCorreoP.Enabled = true;

            }
            else
            {

                this.txtNombreP.Enabled = false;
                this.txtApellido1P.Enabled = false;
                this.txtApellido2P.Enabled = false;
                this.txtCarneP.Enabled = false;
                this.txtCedulaP.Enabled = false;
                this.txtTelFijoP.Enabled = false;
                this.txtCelP.Enabled = false;
                this.txtOtroTelP.Enabled = false;
                this.txtCorreoP.Enabled = false;
            }
        }

    }


    protected void vaciarCampos(int p)
    {

        if (p == 0)
        {
            this.txtNombre.Text = "";
            this.txtApellido1.Text = "";
            this.txtApellido2.Text = "";
            this.txtCarne.Text = "";
            this.txtCedula.Text = "";
            this.txtTelFijo.Text = "";
            this.txtCel.Text = "";
            this.txtOtroTel.Text = "";
            this.txtCorreo.Text = "";
        }
        else {

            this.txtNombreP.Text = "";
            this.txtApellido1P.Text = "";
            this.txtApellido2P.Text = "";
            this.txtCarneP.Text = "";
            this.txtCedulaP.Text = "";
            this.txtTelFijoP.Text = "";
            this.txtCelP.Text = "";
            this.txtOtroTelP.Text = "";
            this.txtCorreoP.Text = "";   
        }
    }



 /*
 * -----------------------------------------------------------------------
 * METODOS DE BECARIO
 * -----------------------------------------------------------------------
 */


    //Pide la lista de becarios existentes ( los que cumplen con un criterio de búsqueda) y
    // asigna dicha lista como fuente de datos del grid
    protected void llenarGridBecarios(int modo) 
    {

        if (modo == 1) // 1:todos los becarios  2:busqueda por criterio
        {
            listaBecarios = controladoraBecarios.consultarTablaBecario();
        }
        else
        {
            string criterioDeBusqueda = "%" + this.txtBuscarBecarios.Text + "%";
            listaBecarios = controladoraBecarios.consultarPorBusqueda(criterioDeBusqueda);
        }

        DataTable tablaBecarios = llenarTablaBecarios();
        gridBecarios.DataSource = tablaBecarios;
        gridBecarios.Enabled = true;
        gridBecarios.DataBind();
        headersCorrectosGridBecarios();
    }


    //Crea una tabla con los campos que se van a desplegar
    protected DataTable crearTablaBecarios()
    {

        DataTable dt = new DataTable();
        DataColumn column;

        column = new DataColumn();
        column.DataType = System.Type.GetType("System.String");
        column.ColumnName = "Nombre Completo";
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
        column.ColumnName = "Teléfono Celular";
        dt.Columns.Add(column);

        return dt;

    }


    //Pide una tabla y la llena con los datos de la lista de becarios previamente completada
    private DataTable llenarTablaBecarios()
    {

        DataTable tabla = crearTablaBecarios();
        DataRow newRow;

        if (listaBecarios.Count != 0)
        {

            for (int i = 0; i < listaBecarios.Count; ++i)
            {

                 newRow = tabla.NewRow();
                 newRow["Nombre Completo"] = listaBecarios[i].nombre + " " + listaBecarios[i].apellido1 + " " + listaBecarios[i].apellido2;
                 newRow["Carné"] = listaBecarios[i].carne;
                 newRow["Correo"] = listaBecarios[i].correo;
                 newRow["Teléfono Celular"] = listaBecarios[i].telefonoCelular;

                 tabla.Rows.InsertAt(newRow, i);

            }

            this.gridBecarios.Columns[0].Visible = true;
        }
        else
        {

            newRow = tabla.NewRow();
            newRow["Nombre Completo"] = "-";
            newRow["Carné"] = "-";
            newRow["Correo"] = "-";
            newRow["Teléfono Celular"] = "-";
            tabla.Rows.InsertAt(newRow, 0);
            this.gridBecarios.Columns[0].Visible = false;
        }

        return tabla;

    }



    //Carga los campos de texto con los datos de determinado becario
    protected void cargarCamposBecario() {


        this.txtNombre.Text = commonService.procesarStringDeUI(listaBecarios[rowIndex].nombre);
        this.txtApellido1.Text = commonService.procesarStringDeUI(listaBecarios[rowIndex].apellido1);
        this.txtApellido2.Text = commonService.procesarStringDeUI(listaBecarios[rowIndex].apellido2);
        this.txtCarne.Text = listaBecarios[rowIndex].carne;
        this.txtCedula.Text = listaBecarios[rowIndex].cedula;
        this.txtCorreo.Text = commonService.procesarStringDeUI(listaBecarios[rowIndex].correo);
        this.txtTelFijo.Text = listaBecarios[rowIndex].telefonoFijo;
        this.txtCel.Text = listaBecarios[rowIndex].telefonoCelular;
        this.txtOtroTel.Text = listaBecarios[rowIndex].telefonoOtro;
    }



    protected void gridBecarios_SelectedIndexChanged(object sender, GridViewSelectEventArgs e)
    {
        /*this.gridBecarios.SelectedIndex = e.NewSelectedIndex;
        this.gridBecarios.DataBind();*/
    }


    //Arregla los encabezados del grid
    private void headersCorrectosGridBecarios()
    {
        this.gridBecarios.HeaderRow.BackColor = System.Drawing.Color.FromArgb(4562432);
        this.gridBecarios.HeaderRow.ForeColor = System.Drawing.Color.White;
        this.gridBecarios.HeaderRow.Cells[1].Text = "Nombre Completo";
        this.gridBecarios.HeaderRow.Cells[2].Text = "Carné";
        this.gridBecarios.HeaderRow.Cells[3].Text = "Correo";
        this.gridBecarios.HeaderRow.Cells[4].Text = "Teléfono Celular";
    }


    //Controla la paginación del grid
    protected void gridBecarios_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        this.gridBecarios.PageIndex = e.NewPageIndex;
        this.gridBecarios.DataBind();
        this.headersCorrectosGridBecarios();
    }


    //Controla la selección de becarios en el grid
    protected void gridBecarios_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        switch(e.CommandName){
            case "btnSeleccionarTupla_Click":
                {
                    rowIndex = Convert.ToInt32(e.CommandArgument);
                    int pageIndex = this.gridBecarios.PageIndex;
                    int pageSize = this.gridBecarios.PageSize;
                    rowIndex += (pageIndex * pageSize);

                    llenarGridBecarios(1);

                    //this.gridBecarios.Rows[ Convert.ToInt32(e.CommandArgument)].BackColor= System.Drawing.Color.DodgerBlue;
                    //this.gridBecarios.Rows[ Convert.ToInt32(e.CommandArgument)].ForeColor = System.Drawing.Color.White;

                    mostrarBotonesPrincipales(true);
                    cargarCamposBecario();
                    habilitarCampos(false, 0);
                    correrJavascript("abrirPopUp();");
                    modoEjecucion = -1;
                } break;
        }
    }



    //Pide crear una cuenta para un becario 
    protected string cearCuenta( string cedula,string nombre,string apellido, string apellido2, string correo){

        string resultado = "Exito";
        CultureInfo currentCulture = System.Threading.Thread.CurrentThread.CurrentCulture; 
        TextInfo currentInfo = currentCulture.TextInfo; 

        int lg = cedula.Length-3;
        string ced = cedula.Substring(lg,3);
        string usuario = nombre + "." + apellido + ced;
        string pass = nombre.Substring(0, 2) + apellido.Substring(0, 2) + ced;
        string nombreCompleto = (currentInfo.ToTitleCase(nombre) + " " + currentInfo.ToTitleCase(apellido) + " " + currentInfo.ToTitleCase(apellido2)).Trim();

        Object[] datos = new Object[4];
        datos[0] = usuario;//this.txtUsuario.Text;
        datos[1] = pass;  //this.cntUsuario.Text;
        datos[2] = "";
        datos[3] = cedula;

        Object[] datosPerfil = new Object[2];
        datosPerfil[0] = nombre + "." + apellido + ced;
        datosPerfil[1]="Becario"; // REVISAR !!!

        string r1 = controladoraCuentas.ejecutar(1,datos,null);
        string r2;
        if (r1.Equals(""))
        { // Exito

            r2 = controladoraCuentas.ejecutarAsociacion(1, datosPerfil, null);
            if (!r2.Equals(""))
            {
                resultado = "error";
            }

        }else{
          resultado="error";
        }

        if (resultado != "error" && servicioCorreo.enviarCorreoCuentaCreada(correo, nombreCompleto, pass, usuario))
        {
            String mensaje = "Se le ha enviado un correo a: \b Nombre: " + nombreCompleto + "\b Usuario: " + usuario + "\b Contraseña: " + pass + "\b Al correo: " + correo;
            commonService.mensajeJavascript(mensaje, "Mensaje enviado");
        }
        return resultado;

    }
   
    /*
     * -----------------------------------------------------------------------
     * METODOS PARA PERFIL DE BECARIO
     * -----------------------------------------------------------------------
     */



    protected List<Lenguaje> obtenerListaLenguajes() {


        if (Session["listaNueva"] == null)
            return this.crearListaLenguajes();
        else
            return (List<Lenguaje>)Session["listaNueva"];
    }


    protected void btnNuevoLenguaje_click(object sender, EventArgs e)
    {

       
        if (permiso==0)
        {
            TextBox tbLenguajeC = (TextBox)gridLenguajesProgC.FooterRow.Cells[0].FindControl("txtNuevoLenguajeC");
            Lenguaje lg = new Lenguaje(tbLenguajeC.Text);
            agregaLenguaje(lg);
            this.gridLenguajesProgC.DataSource = crearListaLenguajes();
            this.gridLenguajesProgC.DataBind();
            correrJavascript("crearTabsP();");
        }
        else {

            TextBox tbLenguaje = (TextBox)gridLenguajesProg.FooterRow.Cells[0].FindControl("txtNuevoLenguaje");
            Lenguaje lg = new Lenguaje(tbLenguaje.Text);
            agregaLenguaje(lg);
            this.gridLenguajesProg.DataSource = this.obtenerListaLenguajes();
            this.gridLenguajesProg.DataBind();
        }



    }



    protected void agregaLenguaje(Lenguaje ln) {


        if( Session["listaNueva"]==null){

            List<Lenguaje> lt = this.crearListaLenguajes();
            lt.Add(ln);
            Session["listaNueva"] = lt;
        
        }else{

            List<Lenguaje> lt = (List<Lenguaje>)Session["listaNueva"];
            lt.Add(ln);
            Session["listaNueva"] = lt;
        }
        
    }


    protected List<Lenguaje> crearListaLenguajes()
    {


         List<Lenguaje> listaNueva = new List<Lenguaje>();

         Lenguaje len1 = new Lenguaje();
         len1.NombreLenguaje = "Java";

         Lenguaje len2 = new Lenguaje();
         len2.NombreLenguaje = "C++";

         listaNueva.Add(len1);
         listaNueva.Add(len2);

         return listaNueva;
    }



    //


    protected List<Idioma> obtenerListaIdiomas()
    {


        if (Session["listaIdiomas"] == null)
            return this.crearListaIdiomas();
        else
            return (List<Idioma>)Session["listaIdiomas"];
    }


    protected void btnNuevoIdioma_click(object sender, EventArgs e)
    {


        if (permiso == 0)
        {

            TextBox tbLenguaje = (TextBox)gridIdiomasC.FooterRow.Cells[0].FindControl("txtNuevoIdiomaC");

            Idioma ido = new Idioma(tbLenguaje.Text);

            agregaLenguaje(ido);


            this.gridIdiomasC.DataSource = this.obtenerListaIdiomas();
            this.gridIdiomasC.DataBind();

        }
        else {


            TextBox tbLenguaje = (TextBox)gridIdiomas.FooterRow.Cells[0].FindControl("txtNuevoIdioma");

            Idioma ido = new Idioma(tbLenguaje.Text);

            agregaLenguaje(ido);

            this.gridIdiomas.DataSource = this.obtenerListaIdiomas();
            this.gridIdiomas.DataBind();
        }
       
    }



    protected void agregaLenguaje(Idioma ld)
    {


        if (Session["listaIdiomas"] == null)
        {

            List<Idioma> lt = this.crearListaIdiomas();
            lt.Add(ld);
            Session["listaIdiomas"] = lt;

        }
        else
        {

            List<Idioma> lt = (List<Idioma>)Session["listaIdiomas"];
            lt.Add(ld);
            Session["listaIdiomas"] = lt;
        }

    }


    protected List<Idioma> crearListaIdiomas()
    {


        List<Idioma> listaIdiomas = new List<Idioma>();

        Idioma id = new Idioma();
        id.NombreIdioma = "Inglés";

        Idioma id2 = new Idioma();
        id2.NombreIdioma = "Alemán";

        listaIdiomas.Add(id);
        listaIdiomas.Add(id2);

        return listaIdiomas;
    }


    //


    protected List<AreaInteres> obtenerListaAreas()
    {


        if (Session["listaAreas"] == null)
            return this.crearListaAreas();
        else
            return (List<AreaInteres>)Session["listaAreas"];
    }


    protected void btnNuevaArea_click(object sender, EventArgs e)
    {



        if (permiso == 0)
        {
            TextBox tb = (TextBox)gridAreasInteresC.FooterRow.Cells[0].FindControl("txtNuevaAreaC");
            AreaInteres lg = new AreaInteres(tb.Text);
            agregaArea(lg);
            this.gridAreasInteresC.DataSource = this.obtenerListaAreas();
            this.gridAreasInteresC.DataBind();
        }
        else {

            TextBox tb = (TextBox)gridAreasInteres.FooterRow.Cells[0].FindControl("txtNuevaArea");
            AreaInteres lg = new AreaInteres(tb.Text);
            agregaArea(lg);
            this.gridAreasInteres.DataSource = this.obtenerListaAreas();
            this.gridAreasInteres.DataBind();
        }

    }



    protected void agregaArea(AreaInteres ln)
    {


        if (Session["listaAreas"] == null)
        {

            List<AreaInteres> lt = this.crearListaAreas();
            lt.Add(ln);
            Session["listaAreas"] = lt;

        }
        else
        {

            List<AreaInteres> lt = (List<AreaInteres>)Session["listaAreas"];
            lt.Add(ln);
            Session["listaAreas"] = lt;
        }

    }


    protected List<AreaInteres> crearListaAreas()
    {


        List<AreaInteres> listaAreas = new List<AreaInteres>();

        AreaInteres ar1 = new AreaInteres();
        ar1.NombreArea="Bases de Datos";

        AreaInteres ar2 = new AreaInteres();
        ar2.NombreArea = "Redes";

        listaAreas.Add(ar1);
        listaAreas.Add(ar2);

        return listaAreas;
    }


    //


    protected List<Cualidades_Personales> obtenerListaCualidades()
    {


        if (Session["listaCualidades"] == null)
            return this.crearListaCualidades();
        else
            return (List<Cualidades_Personales>)Session["listaCualidades"];
    }


    protected void btnNuevaCualidad_click(object sender, EventArgs e)
    {

        if(permiso==0){
            TextBox tbC = (TextBox)gridCualidadesC.FooterRow.Cells[0].FindControl("txtNuevaCualidadC");
            Cualidades_Personales cp = new Cualidades_Personales(tbC.Text);
            agregaCualidad(cp);
            this.gridCualidadesC.DataSource = this.obtenerListaCualidades();
            this.gridCualidadesC.DataBind();
        
       }else{

           TextBox tbC = (TextBox)gridCualidades.FooterRow.Cells[0].FindControl("txtNuevaCualidad");
           Cualidades_Personales cp = new Cualidades_Personales(tbC.Text);
           agregaCualidad(cp);
           this.gridCualidades.DataSource = this.obtenerListaCualidades();
           this.gridCualidades.DataBind();

       }

    }



    protected void agregaCualidad(Cualidades_Personales lc)
    {


        if (Session["listaCualidades"] == null)
        {

            List<Cualidades_Personales> cp = this.crearListaCualidades();
            cp.Add(lc);
            Session["listaCualidades"] = cp;

        }
        else
        {

            List<Cualidades_Personales> cp = (List<Cualidades_Personales>)Session["listaCualidades"];
            cp.Add(lc);
            Session["listaCualidades"] = cp;
        }

    }


    protected List<Cualidades_Personales> crearListaCualidades()
    {


        List<Cualidades_Personales> listaCualidades = new List<Cualidades_Personales>();

        Cualidades_Personales nuevaCp = new Cualidades_Personales();
        nuevaCp.NombreCualidad = "Puntual";

        Cualidades_Personales nuevaCp2 = new Cualidades_Personales();
        nuevaCp2.NombreCualidad = "Responsable";

        listaCualidades.Add(nuevaCp);
        listaCualidades.Add(nuevaCp2);

        return listaCualidades;
    }



    /*
     * -----------------------------------------------------------------------
     * METODOS PARA VISTA PARCIAL DE BECARIO
     * -----------------------------------------------------------------------
     */


    protected void cargarCamposBecarioActual()
    { 
        
        //este metodo deberia cargar los campos de becario logueado
    }


    protected void AsyncFileUpload1_UploadedComplete(object sender, AjaxControlToolkit.AsyncFileUploadEventArgs e)
    {
        System.Threading.Thread.Sleep(5000);
        if (AsyncFileUpload1.HasFile)
        {
            string extension = getFileExtension(e.FileName);
            //string strPath = MapPath("~/Images/Becarios/") + Session["Cuenta"] + "." +extension;//Path.GetFileName(e.FileName);
            string strPath = MapPath("~/Images/Becarios/") + Path.GetFileName(e.FileName);
            Session["ImgUsuario"] = strPath;
            AsyncFileUpload1.SaveAs(strPath);
        }
    }

    protected string getFileExtension(String fileName)
    {
        String extension = "";
        for (int i = ( fileName.Length - 1); i > 0 ; i-- )
        {
            if (fileName[i] == '.')
            {
                break;
            }
            extension.Insert(extension.Length, fileName[i].ToString());
        }
        return extension;
    }
}
