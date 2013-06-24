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

    static int modoEjecucion=0;

    private static Object[] datosViejos;
    private static int rowIndex;
    private static string cedulaBecarioActual;
    private static ControladoraBecarios controladoraBecarios = new ControladoraBecarios();
    private static ControladoraCuentas controladoraCuentas = new ControladoraCuentas();

    private static List<Becario> listaBecarios = new List<Becario>();
    private static List<String> listaLocalLenguajes = new List<String>();
    private static List<String> listaLocalIdiomas = new List<String>();
    private static List<String> listaLocalAreasInteres = new List<String>();
    private static List<String> listaLocalCualidades = new List<String>();

    public string ImageUrl
    {
        get { return Session["ImageUrl"] as string; }
        set { Session["ImageUrl"] = value; }
    }
  
    protected void Page_Load(object sender, EventArgs e)
    {

        commonService = new CommonServices(UpdateInfo);
        servicioCorreo = new EmailServices();
        imgBecario.ImageUrl = this.ImageUrl;
       

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
                           
                           llenarGridsPerfil();
                           if (Request["__EVENTTARGET"] == UpdateImage.ClientID)
                           {
                               // Subir foto
                               if (string.IsNullOrEmpty(this.ImageUrl))
                               {
                                   imgBecario.Visible = false;
                               }
                               else
                               {
                                   imgBecario.Visible = true;
                               }
                           }
                           
                         }
                     } break;

                 case 2: // Vista Parcial
                     {
                         MultiViewBecario.ActiveViewIndex = 1;
                         correrJavascript("crearTabsP();");
                         

                         if (!Page.IsPostBack)
                         {
                             consultarDatosBecarioLogueado();

                             llenarListasPerfil(cedulaBecarioActual);
                             llenarGridsPerfil_vistaParcial();
                             
                             habilitarEdicionPefil(false,1);
                             mostrarBotonesSecundariosP(false);
                             if (Request["__EVENTTARGET"] == UpdateImage.ClientID)
                             {
                                 // Subir foto
                                 if (string.IsNullOrEmpty(this.ImageUrl))
                                 {
                                     imgBecario.Visible = false;
                                 }
                                 else
                                 {
                                     imgBecario.Visible = true;
                                 }
                             }

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
            string resultadoPerfil = "";
            string resultadoCreacionCuenta = "";

            CultureInfo currentCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
            TextInfo currentInfo = currentCulture.TextInfo;
            string cedula = this.txtCedula.Text;
            string nombre = this.txtNombre.Text.ToLower();
            string apellido = this.txtApellido1.Text.ToLower();
            string apellido2 = this.txtApellido2.Text;
            string correo = this.txtCorreo.Text;

            int lg = cedula.Length - 3;
            string ced = cedula.Substring(lg, 3);
            string usuario = nombre + "." + apellido + ced;
            string pass = nombre.Substring(0, 2) + apellido.Substring(0, 2) + ced;
            string nombreCompleto = (currentInfo.ToTitleCase(nombre) + " " + currentInfo.ToTitleCase(apellido) + " " + currentInfo.ToTitleCase(apellido2)).Trim();

            switch (modoEjecucion)
            {

                case 1: //Insertar Nuevo Becario
                    {

                        resultado = controladoraBecarios.ejecutar(modoEjecucion, datos, null);
                        resultadoPerfil = controladoraBecarios.guardarPerfilBecario(listaLocalLenguajes, listaLocalIdiomas, listaLocalAreasInteres, listaLocalCualidades, this.txtCedula.Text);
                        resultadoCreacionCuenta = crearCuenta(cedula, ced, nombre, apellido, correo, usuario, pass, nombreCompleto);

                    
                    } break;
                case 2: //Modificar Datos Personales y Perfil
                    {
                        resultado = controladoraBecarios.ejecutar(modoEjecucion, datos, datosViejos);
                        string mensaje = controladoraBecarios.eliminarPerfilBecario(this.txtCedula.Text);
                        if (mensaje.Equals("Exito"))
                        {
                          resultadoPerfil = controladoraBecarios.guardarPerfilBecario(listaLocalLenguajes, listaLocalIdiomas, listaLocalAreasInteres, listaLocalCualidades, this.txtCedula.Text);
                        }
                    } break;
            }


            switch (resultado) {


                case "Exito": 
                    {

                        if (modoEjecucion == 1) // un insertar
                        {

                          if ((resultadoCreacionCuenta.Equals("Exito")) && ((resultadoPerfil.Equals("Exito")) || (resultadoPerfil.Equals("-1"))))
                          {                          
                              commonService.mensajeJavascript("El becario ha sido ingresado correctamente y su cuenta ha sido creada","Éxito");
                          }
                          else if (!(resultadoCreacionCuenta.Equals("Exito")))
                          {
                             commonService.mensajeJavascript("El becario ha sido ingresado correctamente pero hubo un problema al crear la cuenta", "Aviso");                     
                          }
                          else if (!(resultadoPerfil.Equals("Exito")))
                          {
                              commonService.mensajeJavascript("El becario ha sido ingresado correctamente pero hubo un problema al guardar la información del perfil", "Aviso");                                  
                          }
                     
                        }
                        else if (((resultadoPerfil.Equals("Exito")) || (resultadoPerfil.Equals("-1")))) // un modificar
                        {
                            commonService.mensajeJavascript("Se ha modificado correctamente la información solcitada", "Éxito");
                        }
                        else {
                          commonService.mensajeJavascript("Se ha modificado correctamente la información personal pero ha habido un problema al actualizar el perfil del becario", "Aviso");
                        }
                
                    }break;
                case "Error1": 
                    {
                      commonService.mensajeJavascript("Ya existe un becario con la cédula digitada", "Error");           
                    }break;     
            }

           
        llenarGridBecarios(1);
        habilitarBotonesPrincipales(true);
        correrJavascript("cerrarPopUp();");

        if (resultado == "Exito")
        {

            // Abrir mensaje de mandar correo
            commonService.mensajeEspera("Enviando correo de confirmación al becari@", "Enviando correo");
             
            this.btnInvisibleEnviarCorreo.CommandArgument = correo + "," + nombreCompleto + "," + pass + "," + usuario;
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

    //Método que se invoca al confirmar la eliminación de un becario
    protected void btnInvisible2_Click(object sender, EventArgs e)
    {

        
        Object[] datos = new Object[1];

        datos[0] = this.txtCedula.Text;

        string resultado = controladoraBecarios.ejecutar(3,datos,null);
        this.gridBecarios.SelectedIndex = -1;

        correrJavascript("cerrarPopUp();");

        if (resultado.Equals("Exito"))
        {
            commonService.mensajeJavascript("Se ha eliminado correctamente al becario", "Éxito");
        }
        else {
           commonService.mensajeJavascript("No se ha podido al eliminar el becario", "Error");       
        }
       
        llenarGridBecarios(1);

    }


    //Método que se invoca el dar click al botón 'cancelar' del 'popUp'
    protected void btnInvisible3_Click(object sender, EventArgs e)
    {

        habilitarBotonesPrincipales(true);
        cancelarEdicionPerfil();

    }



    // Método que se invoca al dar click al botón 'insertar'
    // Abre el popUp que permite ingresar el nuevo becario , limpia y habilita los campos
    protected void btnInsertarEncargado_Click(object sender, EventArgs e)
    {

        lblInstTab1.Text = "Favor completar la información solicitada y presionar el botón aceptar para completar la operación.";
        lblInstTab1.Visible=true;

        lblInstTab2.Text = "A continuación se le presentan unas tablas que resumen algunos aspectos de importancia para el proceso de becas. Se le solicita completar los datos de la forma más precisa posible.";
        lblInstTab2.Visible = true;


        correrJavascript("abrirPopUp();");
        mostrarBotonesPrincipales(false);
        vaciarCampos(0);
        habilitarCampos(true,0);
        habilitarEdicionPefil(true, 0);
        modoEjecucion = 1;
    }


    //Metodo que pide llenar el grid pero bajo un criterio de selección (búsqueda) expresado por el usuario
    protected void btnBuscarBecario_Click(object sender, EventArgs e)
    {

        llenarGridBecarios(2);    
    }




    // Método que se invoca al dar click al botón 'modificar' en la vista completa
    // Toma los datos actuales y luego habilita los campos para editarlos
    protected void btnModificarBecario_Click(object sender, EventArgs e)
    {

      
        correrJavascript("destruirTabsVistaCompleta();");
        correrJavascript("crearTabsVistaCompleta();");


        guardarDatosActuales();

        modoEjecucion = 2;

        mostrarBotonesPrincipales(false);

        habilitarCampos(true, 0);
        habilitarEdicionPefil(true, 0);

        Button aux = (Button)sender;
        Label lb = new Label();
        lb.Text = aux.ID;

        if (lb.Text.Equals("btnModificarBecarioPerfil"))
        {
            correrJavascript("seleccionarTabs();");
        }


    }



    // Activa el 'popUp' para confirmar la eliminación
    protected void btnEliminarBecario_Click(object sender, EventArgs e)
    {
        correrJavascript("abrirPopUpEliminar();");
    }






    /*
    * -----------------------------------------------------------------------
    * AUXILIARES
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

            this.btnModificarBecarioDatosP.Visible = true;
            this.btnModificarBecarioPerfilP.Visible = true;
        }
        else
        {
            this.btnModificarBecarioDatos.Visible = false;
            this.btnEliminarBecarioDatos.Visible = false;
            this.btnModificarBecarioPerfil.Visible = false;

            this.btnModificarBecarioDatosP.Visible = false;
            this.btnModificarBecarioPerfilP.Visible = false;
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

             this.btnModificarBecarioDatosP.Enabled = true;
             this.btnModificarBecarioPerfilP.Enabled = true;
          }
          else
          {
              this.btnModificarBecarioDatos.Enabled = false;
              this.btnEliminarBecarioDatos.Enabled = false;
              this.btnModificarBecarioPerfil.Enabled = false;

              this.btnModificarBecarioDatosP.Enabled = false;
              this.btnModificarBecarioPerfilP.Enabled = false;
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
                this.txtTelFijoP.Enabled = true;
                this.txtCelularP.Enabled = true;
                this.txtOtroTelP.Enabled = true;
                this.txtCorreoP.Enabled = true;

            }
            else
            {

                this.txtNombreP.Enabled = false;
                this.txtApellido1P.Enabled = false;
                this.txtApellido2P.Enabled = false;
                this.txtCedulaP.Enabled = false;
                this.txtCarneP.Enabled = false;
                this.txtTelFijoP.Enabled = false;
                this.txtCelularP.Enabled = false;
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
            this.txtCelularP.Text = "";
            this.txtOtroTelP.Text = "";
            this.txtCorreoP.Text = "";   
        }
    }



 /*
 * -----------------------------------------------------------------------
 * METODOS DE BECARIO
 * -----------------------------------------------------------------------
 */


    protected void guardarDatosActuales() {


        TextInfo miTexto = CultureInfo.CurrentCulture.TextInfo;

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
    
    }


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
   // Modo =  0 : campos de la vista completa , en cualquier otro caso campos de la vista parcial 
    protected void cargarCamposBecario(int modo) {


        if (modo == 0)
        {

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
        else {
       
            txtNombreP.Text = commonService.procesarStringDeUI(listaBecarios[0].nombre);
            txtApellido1P.Text = commonService.procesarStringDeUI(listaBecarios[0].apellido1);
            txtApellido2P.Text = commonService.procesarStringDeUI(listaBecarios[0].apellido2);
            txtCarneP.Text = listaBecarios[0].carne;
            txtCedulaP.Text = listaBecarios[0].cedula;
            txtTelFijoP.Text = listaBecarios[0].telefonoFijo;
            txtCelularP.Text = listaBecarios[0].telefonoCelular;
            txtOtroTelP.Text = listaBecarios[0].telefonoOtro;
            txtCorreoP.Text = commonService.procesarStringDeUI(listaBecarios[0].correo);     
        }
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

                    cargarCamposBecario(0);
                    llenarGridBecarios(1);
                    
                    llenarListasPerfil(listaBecarios[rowIndex].cedula);
                    llenarGridsPerfil();
                    
                    mostrarBotonesPrincipales(true);
                    habilitarBotonesPrincipales(true);                   
                    habilitarCampos(false, 0);
                    habilitarEdicionPefil(false, 0);

                    lblInstTab1.Text = "Para editar la información debe presionar el botón modificar y para completar la operación presionar aceptar.";
                    lblInstTab1.Visible = true;

                    lblInstTab2.Text = "A continuación se le presentan unas tablas que resumen algunos aspectos de importancia para el proceso de becas. Se le solicita completar los datos de la forma más precisa posible. Para editar esta información debe presionar el botón modificar ubicado arriba.";
                    lblInstTab2.Visible = true;

                    correrJavascript("abrirPopUp();");
                    modoEjecucion = -1;

                } break;
        }
    }


    
    //Pide crear una cuenta para un becario 
    protected string crearCuenta( string cedula, string ced, string nombre, string apellido, string correo, string usuario, string pass, string nombreCompleto){

        string resultado = "Exito";

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

        return resultado;

    }
   


    /*
     * -------------------------------------------
     *  PERFIL DE BECARIO VISTA COMPLETA
     * -------------------------------------------
     */


    //limpia las las lista locales para evitar conflictos y quita avisos
    protected void cancelarEdicionPerfil()
    {

        listaLocalLenguajes.Clear();
        listaLocalIdiomas.Clear();
        listaLocalAreasInteres.Clear();
        listaLocalCualidades.Clear();
        lblAvisoPerfil.Visible = false;
    }


    //llena las listas locales donde se guarda la información del perfil del becario
    protected void llenarListasPerfil(String cedBecario)
    {

        listaLocalLenguajes = controladoraBecarios.consultarLenguajes(cedBecario);
        listaLocalIdiomas = controladoraBecarios.consultarIdiomas(cedBecario);     
        listaLocalAreasInteres = controladoraBecarios.consultarAreasInteres(cedBecario);      
        listaLocalCualidades = controladoraBecarios.consultarCualidades(cedBecario);
        
    }


    //llena los grid del perfil del becario
    protected void llenarGridsPerfil()
    {

        llenarGridLenguajes();
        llenarGridIdiomas();
        llenarGridAreasInteres();
        llenarGridCualidades();
    }


    //verifica si ya se insertaron los datos personales necesarios para poder ingresar información del perfil
    protected bool datosPersonalesVacios(){ 
    
       bool retorno = false;

       if ((txtCedula.Text.ToString().Equals("")) || (txtNombre.ToString().Equals("")) || (txtApellido1.Text.ToString().Equals("")))
       {
           retorno = true;    
       }


       return retorno;
    }


    //habilita los campos de texto del los grids y los botones de agregar para editar el perfil
    protected void habilitarEdicionPefil(Boolean habilitar, int p)
    {

        TextBox txtBoxAux;

        if (p == 0)
        {
            if (habilitar)
            {

               txtBoxAux = (TextBox)gridLenguajesProg.FooterRow.Cells[0].FindControl("txtNuevoLenguaje");
               txtBoxAux.Enabled = true;
               txtBoxAux = (TextBox)gridIdiomas.FooterRow.Cells[0].FindControl("txtNuevoIdioma");
               txtBoxAux.Enabled = true;
               txtBoxAux = (TextBox)gridAreasInteres.FooterRow.Cells[0].FindControl("txtNuevaAreaInteres");
               txtBoxAux.Enabled = true;
               txtBoxAux = (TextBox)gridCualidades.FooterRow.Cells[0].FindControl("txtNuevaCualidad");
               txtBoxAux.Enabled = true;
               btnAgregaLenguaje.Visible = true;
               btnAgregaIdioma.Visible = true;
               btnAgregaCualidad.Visible = true;
               btnAgregaAreaInteres.Visible = true;

            }
            else
            {

               txtBoxAux = (TextBox)gridLenguajesProg.FooterRow.Cells[0].FindControl("txtNuevoLenguaje");
               txtBoxAux.Enabled = false;
               txtBoxAux = (TextBox)gridIdiomas.FooterRow.Cells[0].FindControl("txtNuevoIdioma");
               txtBoxAux.Enabled = false;
               txtBoxAux = (TextBox)gridAreasInteres.FooterRow.Cells[0].FindControl("txtNuevaAreaInteres");
               txtBoxAux.Enabled = false;
               txtBoxAux = (TextBox)gridCualidades.FooterRow.Cells[0].FindControl("txtNuevaCualidad");
               txtBoxAux.Enabled = false;
               btnAgregaLenguaje.Visible = false;
               btnAgregaIdioma.Visible = false;
               btnAgregaCualidad.Visible = false;
               btnAgregaAreaInteres.Visible = false;

            }
        }
        else
        {

            if (habilitar)
            {

                txtBoxAux = (TextBox)gridLenguajesProgP.FooterRow.Cells[0].FindControl("txtNuevoLenguajeParcial");
                txtBoxAux.Enabled = true;
                txtBoxAux = (TextBox)gridIdiomasP.FooterRow.Cells[0].FindControl("txtNuevoIdiomaParcial");
                txtBoxAux.Enabled = true;
                txtBoxAux = (TextBox)gridAreasInteresP.FooterRow.Cells[0].FindControl("txtNuevaAreaInteresParcial");
                txtBoxAux.Enabled = true;
                txtBoxAux = (TextBox)gridCualidadesP.FooterRow.Cells[0].FindControl("txtNuevaCualidadParcial");
                txtBoxAux.Enabled = true;
                btnAgregaLenguajeParcial.Visible = true;
                btnAgregaIdiomaParcial.Visible = true;
                btnAgregaAreaInteresParcial.Visible = true;
                btnAgregaCualidadParcial.Visible = true;
            }
            else
            {

                txtBoxAux = (TextBox)gridLenguajesProgP.FooterRow.Cells[0].FindControl("txtNuevoLenguajeParcial");
                txtBoxAux.Enabled = false;
                txtBoxAux = (TextBox)gridIdiomasP.FooterRow.Cells[0].FindControl("txtNuevoIdiomaParcial");
                txtBoxAux.Enabled = false;
                txtBoxAux = (TextBox)gridAreasInteresP.FooterRow.Cells[0].FindControl("txtNuevaAreaInteresParcial");
                txtBoxAux.Enabled = false;
                txtBoxAux = (TextBox)gridCualidadesP.FooterRow.Cells[0].FindControl("txtNuevaCualidadParcial");
                txtBoxAux.Enabled = false;
                btnAgregaLenguajeParcial.Visible = false;
                btnAgregaIdiomaParcial.Visible = false;
                btnAgregaAreaInteresParcial.Visible = false;
                btnAgregaCualidadParcial.Visible = false;
            }
        }

    }


    protected void eliminaDatosPerfil_RowCommand(object sender, GridViewCommandEventArgs e)
    {

        correrJavascript("destruirTabsVistaCompleta();");
        correrJavascript("crearTabsVistaCompleta();");
        correrJavascript("seleccionarTabs();");

         if (modoEjecucion == 2)
         {

            int indiceFila;
            switch (e.CommandName)
            {
                case "btnEliminaLenguaje_Click":
                    {
                        indiceFila = Convert.ToInt32(e.CommandArgument);
                        listaLocalLenguajes.RemoveAt(indiceFila);
                        llenarGridLenguajes();
                    }
                    break;
                case "btnEliminaIdioma_Click":
                    {
                        indiceFila = Convert.ToInt32(e.CommandArgument);
                        listaLocalIdiomas.RemoveAt(indiceFila);
                        llenarGridIdiomas();
                    }
                    break;
                case "btnEliminaInteres_Click":
                    {
                        indiceFila = Convert.ToInt32(e.CommandArgument);
                        listaLocalAreasInteres.RemoveAt(indiceFila);
                        llenarGridAreasInteres();
                    }
                    break;
                case "btnEliminaCualidad_Click":
                    {
                        indiceFila = Convert.ToInt32(e.CommandArgument);
                        listaLocalCualidades.RemoveAt(indiceFila);
                        llenarGridCualidades();
                    }
                    break;
            }
        }
    }


    //controla la inserción de nuevos atributos a los grids del perfil
    protected void nuevoAtributoDePerfil_click(object sender, EventArgs e)
    {

        correrJavascript("destruirTabsVistaCompleta();");
        correrJavascript("crearTabsVistaCompleta();");
        correrJavascript("seleccionarTabs();");

        TextInfo miTexto = CultureInfo.CurrentCulture.TextInfo;
        TextBox txtBoxAux = new TextBox();

        ImageButton aux = (ImageButton)sender;
        Label lb = new Label();
        lb.Text = aux.ID;

        if (datosPersonalesVacios() == false)
        {
            lblAvisoPerfil.Visible = false;
            switch (lb.Text)
            {

                case "btnAgregaLenguaje":
                    {
                         
                        txtBoxAux = (TextBox)gridLenguajesProg.FooterRow.Cells[0].FindControl("txtNuevoLenguaje");
                        if (!(txtBoxAux.Text.Equals("")))
                        {
                            listaLocalLenguajes.Add(miTexto.ToTitleCase(txtBoxAux.Text.ToLower()));
                            llenarGridLenguajes();
                        }

                    } break;
                case "btnAgregaIdioma":
                    {
                        txtBoxAux = (TextBox)gridIdiomas.FooterRow.Cells[0].FindControl("txtNuevoIdioma");
                        if (!(txtBoxAux.Text.Equals("")))
                        {
                            listaLocalIdiomas.Add(miTexto.ToTitleCase(txtBoxAux.Text.ToLower()));
                            llenarGridIdiomas();
                        }
                    } break;
                case "btnAgregaAreaInteres":
                    {
                        txtBoxAux = (TextBox)gridAreasInteres.FooterRow.Cells[0].FindControl("txtNuevaAreaInteres");
                        if (!(txtBoxAux.Text.Equals("")))
                        {
                            listaLocalAreasInteres.Add(miTexto.ToTitleCase(txtBoxAux.Text.ToLower()));
                            llenarGridAreasInteres();
                        }
                    } break;
                case "btnAgregaCualidad":
                    {
                        txtBoxAux = (TextBox)gridCualidades.FooterRow.Cells[0].FindControl("txtNuevaCualidad");
                        if (!(txtBoxAux.Text.Equals("")))
                        {
                            listaLocalCualidades.Add(miTexto.ToTitleCase(txtBoxAux.Text.ToLower()));
                            llenarGridCualidades();
                        }
                    } break;

            }
        }
        else {
            lblAvisoPerfil.Visible = true; 
        }

    }


    /**GRID DE LENGUAJES DE PROGRAMACION**/
  
    protected void llenarGridLenguajes()
    {


        DataTable dt = new DataTable();
        DataColumn column = new DataColumn();
        column.DataType = System.Type.GetType("System.String");
        column.ColumnName = "LenguajeProgramacion";
        dt.Columns.Add(column);

        DataRow newRow;
        if (listaLocalLenguajes.Count != 0)
        {

            for (int i = 0; i < listaLocalLenguajes.Count; ++i)
            {
                newRow = dt.NewRow();
                newRow["LenguajeProgramacion"] = listaLocalLenguajes[i];
                dt.Rows.InsertAt(newRow, i);
                gridLenguajesProg.Columns[0].Visible = true;

            }
        }
        else
        {

            newRow = dt.NewRow();
            newRow["LenguajeProgramacion"] = "--";
            dt.Rows.InsertAt(newRow, 0);
            gridLenguajesProg.Columns[0].Visible = false;
        }

        gridLenguajesProg.DataSource = dt;
        gridLenguajesProg.DataBind();
    }



    /*protected void gridLenguajes_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {

        this.gridLenguajesProg.PageIndex = e.NewPageIndex;
        this.gridLenguajesProg.DataBind();

        this.gridLenguajesProg.HeaderRow.BackColor = System.Drawing.Color.FromArgb(4562432);
        this.gridLenguajesProg.ForeColor = System.Drawing.Color.White;
        this.gridLenguajesProg.HeaderRow.Cells[1].Text = "Lenguaje de Programación";
        //this.headersCorrectosGridBecarios();
    }*/



    /**GRID DE IDIOMAS**/


    protected void llenarGridIdiomas()
    {

        DataTable dt = new DataTable();
        DataColumn column = new DataColumn();
        column.DataType = System.Type.GetType("System.String");
        column.ColumnName = "Idioma";
        dt.Columns.Add(column);

        DataRow newRow;
        if (listaLocalIdiomas.Count != 0)
        {

            for (int i = 0; i < listaLocalIdiomas.Count; ++i)
            {
                newRow = dt.NewRow();
                newRow["Idioma"] = listaLocalIdiomas[i];
                dt.Rows.InsertAt(newRow, i);
                gridIdiomas.Columns[0].Visible = true;

            }
        }
        else
        {

            newRow = dt.NewRow();
            newRow["Idioma"] = "--";
            dt.Rows.InsertAt(newRow, 0);
            gridIdiomas.Columns[0].Visible = false;
        }

        gridIdiomas.DataSource = dt;
        gridIdiomas.DataBind();
    }



    /**GRID DE AREAS DE INTERÉS**/


    protected void llenarGridAreasInteres()
    {

        DataTable dt = new DataTable();
        DataColumn column = new DataColumn();
        column.DataType = System.Type.GetType("System.String");
        column.ColumnName = "AreaInteres";
        dt.Columns.Add(column);

        DataRow newRow;
        if (listaLocalAreasInteres.Count != 0)
        {

            for (int i = 0; i < listaLocalAreasInteres.Count; ++i)
            {
                newRow = dt.NewRow();
                newRow["AreaInteres"] = listaLocalAreasInteres[i];
                dt.Rows.InsertAt(newRow, i);
                gridAreasInteres.Columns[0].Visible = true;

            }
        }
        else
        {

            newRow = dt.NewRow();
            newRow["AreaInteres"] = "--";
            dt.Rows.InsertAt(newRow, 0);
            gridAreasInteres.Columns[0].Visible = false;
        }

        gridAreasInteres.DataSource = dt;
        gridAreasInteres.DataBind();
    }


    /**GRID DE CUALIDADES PERSONALES**/

    protected void llenarGridCualidades()
    {

        DataTable dt = new DataTable();
        DataColumn column = new DataColumn();
        column.DataType = System.Type.GetType("System.String");
        column.ColumnName = "Cualidad";
        dt.Columns.Add(column);

        DataRow newRow;
        if (listaLocalCualidades.Count != 0)
        {

            for (int i = 0; i < listaLocalCualidades.Count; ++i)
            {
                newRow = dt.NewRow();
                newRow["Cualidad"] = listaLocalCualidades[i];
                dt.Rows.InsertAt(newRow, i);
                gridCualidades.Columns[0].Visible = true;

            }
        }
        else
        {

            newRow = dt.NewRow();
            newRow["Cualidad"] = "--";
            dt.Rows.InsertAt(newRow, 0);
            gridCualidades.Columns[0].Visible = false;
        }

        gridCualidades.DataSource = dt;
        gridCualidades.DataBind();
    }




    protected void AsyncFileUpload1_UploadedComplete(object sender, AjaxControlToolkit.AsyncFileUploadEventArgs e)
    {
        //System.Threading.Thread.Sleep(5000);
        if (AsyncFileUpload1.HasFile)
        {
            var scheme = Request.Url.Scheme; // will get http, https, etc.
            var host = Request.Url.Host; // will get www.mywebsite.com
            var port = Request.Url.Port; // will get the port
            var path = Request.Url.AbsolutePath;
            string picpath = scheme.ToString() + "://" + host.ToString() + ":" + port.ToString() + Request.ApplicationPath + "/" + "images/Becarios/";

            string extension = getFileExtension(e.FileName);
            //string strPath = MapPath("~/Images/Becarios/") + Session["Cuenta"] + "." +extension;//Path.GetFileName(e.FileName);
            string strPath = MapPath("~/Images/Becarios/") + Path.GetFileName(e.FileName);
            string webPath = picpath + Path.GetFileName(e.FileName);
            AsyncFileUpload1.SaveAs(strPath);

            //string filePath = "~/images/" + System.IO.Path.GetFileName(e.filename);
            //AsyncFileUpload1.SaveAs(Server.MapPath(filePath));
            ImageUrl = webPath;
        }
    }

    protected string getFileExtension(String fileName)
    {
        String extension = "";
        for (int i = (fileName.Length - 1); i > 0; i--)
        {
            if (fileName[i] == '.')
            {
                break;
            }
            extension.Insert(extension.Length, fileName[i].ToString());
        }
        return extension;
    }




   /*
   * -----------------------------------------------------------------------
   *              METODOS PARA VISTA PARCIAL DE BECARIO
   * -----------------------------------------------------------------------
   */



/*
* -----------------------------
*    CLICKS
* -----------------------------
*/


    // Método que se invoca al dar click al botón 'modificar' en la vista parcial 
    // Toma los datos actuales y luego habilita los campos para editarlos
    protected void btnModificarBecarioParcial_Click(object sender, EventArgs e)
    {

        TextInfo miTexto = CultureInfo.CurrentCulture.TextInfo;

        correrJavascript("destruyeTabsP();");
        correrJavascript("crearTabsP();");


        mostrarBotonesSecundariosP(true);

        guardarDatosActuales();

        modoEjecucion = 2;

        mostrarBotonesPrincipales(false);
        habilitarCampos(true, 1);
        habilitarEdicionPefil(true, 1);

        Button aux = (Button)sender;
        Label lb = new Label();
        lb.Text = aux.ID;

        if (lb.Text.Equals("btnModificarBecarioPerfilP"))
        {
            correrJavascript("seleccionarTabPerfilParcial();");
        }

    }


    //Método que se invoca al dar click al botón 'CANCELAR' en la vista parcial 
    //Vacia campos
    protected void btnCancelarP_Click(object sender, EventArgs e)
    {

        correrJavascript("destruyeTabsP();");
        correrJavascript("crearTabsP();");

        cargarCamposBecario(1);
        
        llenarListasPerfil(cedulaBecarioActual);
        llenarGridsPerfil_vistaParcial();      

        habilitarCampos(false, 1);
        habilitarEdicionPefil(false, 1);

        mostrarBotonesPrincipales(true);
        mostrarBotonesSecundariosP(false);

        Button aux = (Button)sender;
        Label lb = new Label();
        lb.Text = aux.ID;

        if (lb.Text.Equals("btnCancelarPerfilParcial"))
        {
            correrJavascript("seleccionarTabPerfilParcial();");
        }

    }


    //Método que se invoca al dar click al botón 'ACEPTAR' en la vista parcial 
    //Toma los datos ingresados y pide modificar la información del becario correspondiente
    protected void btnAceptarP_Click(object sender, EventArgs e)
    {

        correrJavascript("destruyeTabsP();");
        correrJavascript("crearTabsP();");

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
        datos[7] = this.txtCelularP.Text;
        datos[8] = this.txtOtroTelP.Text;
        datos[9] = this.txtCorreoP.Text;



        string resultado = controladoraBecarios.ejecutar(2, datos, datosViejos);
        string resultadoPerfil="";

        if (resultado.Equals("Exito"))
        {
            string mensaje = controladoraBecarios.eliminarPerfilBecario(cedulaBecarioActual);
            resultadoPerfil = controladoraBecarios.guardarPerfilBecario(listaLocalLenguajes, listaLocalIdiomas, listaLocalAreasInteres, listaLocalCualidades, cedulaBecarioActual);              
        }
        else
        {
            commonService.mensajeJavascript("Se producido un error. Favor intentar más tarde", "Error");
        }

        if (resultadoPerfil.Equals("Exito")){
            commonService.mensajeJavascript("Se ha modificado correctamente la información", "Éxito");
        }
        
      
        habilitarCampos(false, 1);
        habilitarEdicionPefil(false, 1);
        mostrarBotonesSecundariosP(false);
        mostrarBotonesPrincipales(true);

    }



/*
* -----------------------------
*   VARIOS
* -----------------------------
*/


    protected void mostrarBotonesSecundariosP(Boolean mostrar){

        if (mostrar)
        {

            this.btnAceptarP.Visible = true;
            this.btnCancelarP.Visible = true;
            this.btnAceptarPerfilParcial.Visible = true;
            this.btnCancelarPerfilParcial.Visible = true;
        }
        else {

            this.btnAceptarP.Visible = false;
            this.btnCancelarP.Visible = false;
            this.btnAceptarPerfilParcial.Visible = false;
            this.btnCancelarPerfilParcial.Visible = false;    
        }
    }


    protected void consultarDatosBecarioLogueado()
    {

        string usuario = Session["Cuenta"].ToString();
        cedulaBecarioActual = controladoraBecarios.obtieneCedulaDeUsuario(usuario);
        Becario becarioActual = controladoraBecarios.obtenerBecarioPorCedula(cedulaBecarioActual);

        
        listaBecarios.Clear();
        listaBecarios.Add(becarioActual);

        cargarCamposBecario(1);

        habilitarCampos(false, 1);

    }



  /*
   * ---------------------------------
   *   PERFIL BECARIO - vista parcial
   * --------------------------------
   */



    protected void llenarGridsPerfil_vistaParcial()
    {

        llenarGridLenguajesParcial();
        llenarGridIdiomasParcial();
        llenarGridAreasInteresParcial();
        llenarGridCualidadesParcial();
    }


    protected void nuevoAtributoDePerfilParcial_click(object sender, EventArgs e)
    {

        correrJavascript("destruyeTabsP();");
        correrJavascript("crearTabsP();");
        correrJavascript("seleccionarTabPerfilParcial();");

        TextInfo miTexto = CultureInfo.CurrentCulture.TextInfo;
        TextBox txtBoxAux = new TextBox();

        ImageButton aux = (ImageButton)sender;
        Label lb =  new Label();
        lb.Text = aux.ID;

       

        switch (lb.Text) {

            case "btnAgregaLenguajeParcial":
                {

                  txtBoxAux = (TextBox)gridLenguajesProgP.FooterRow.Cells[0].FindControl("txtNuevoLenguajeParcial");
                  if (!(txtBoxAux.Text.Equals("")))
                  {
                      listaLocalLenguajes.Add(miTexto.ToTitleCase(txtBoxAux.Text.ToLower()));
                      llenarGridLenguajesParcial();
                  }

                }break;
            case "btnAgregaIdiomaParcial":
                {
                   txtBoxAux = (TextBox)gridIdiomasP.FooterRow.Cells[0].FindControl("txtNuevoIdiomaParcial");
                   if (!(txtBoxAux.Text.Equals("")))
                   {
                       listaLocalIdiomas.Add(miTexto.ToTitleCase(txtBoxAux.Text.ToLower()));
                       llenarGridIdiomasParcial();
                   }
                }break;
            case "btnAgregaAreaInteresParcial":
                {
                    txtBoxAux = (TextBox)gridAreasInteresP.FooterRow.Cells[0].FindControl("txtNuevaAreaInteresParcial");
                    if (!(txtBoxAux.Text.Equals("")))
                    {
                        listaLocalAreasInteres.Add(miTexto.ToTitleCase(txtBoxAux.Text.ToLower()));
                        llenarGridAreasInteresParcial();
                    }
                } break;
            case "btnAgregaCualidadParcial":
                {
                    txtBoxAux = (TextBox)gridCualidadesP.FooterRow.Cells[0].FindControl("txtNuevaCualidadParcial");
                    if (!(txtBoxAux.Text.Equals("")))
                    {
                        listaLocalCualidades.Add(miTexto.ToTitleCase(txtBoxAux.Text.ToLower()));
                        llenarGridCualidadesParcial();
                    }
                } break;
            
        }

    }


    protected void eliminaDatosPerfilParcial_RowCommand(object sender, GridViewCommandEventArgs e)
    {

        correrJavascript("destruyeTabsP();");
        correrJavascript("crearTabsP();");
        correrJavascript("seleccionarTabPerfilParcial();");

        

        int indiceFila;
        switch (e.CommandName)
        {
            case "btnEliminaLenguajeP_Click":
                {
                    indiceFila = Convert.ToInt32(e.CommandArgument);
                    listaLocalLenguajes.RemoveAt(indiceFila);
                    llenarGridLenguajesParcial();
                }
                break;
            case "btnEliminaIdiomaP_Click":
                {
                    indiceFila = Convert.ToInt32(e.CommandArgument);
                    listaLocalIdiomas.RemoveAt(indiceFila);
                    llenarGridIdiomasParcial();
                }
                break;
            case "btnEliminaInteresP_Click":
                {
                    indiceFila = Convert.ToInt32(e.CommandArgument);
                    listaLocalAreasInteres.RemoveAt(indiceFila);
                    llenarGridAreasInteresParcial();
                }
                break;
            case "btnEliminaCualidadP_Click":
                {
                    indiceFila = Convert.ToInt32(e.CommandArgument);
                    listaLocalCualidades.RemoveAt(indiceFila);
                    llenarGridCualidadesParcial();
                }
                break;
        }   

    }


    /**GRID DE LENGUAJES DE PROGRAMACION**/
    protected void llenarGridLenguajesParcial()
    {


        DataTable dt = new DataTable();
        DataColumn column = new DataColumn();
        column.DataType = System.Type.GetType("System.String");
        column.ColumnName = "LenguajeProgramacion";
        dt.Columns.Add(column);

        DataRow newRow;
        if (listaLocalLenguajes.Count != 0)
        {

            for (int i = 0; i < listaLocalLenguajes.Count; ++i)
            {
                newRow = dt.NewRow();
                newRow["LenguajeProgramacion"] = listaLocalLenguajes[i];
                dt.Rows.InsertAt(newRow, i);
                gridLenguajesProgP.Columns[0].Visible = true;

            }
        }
        else
        {

            newRow = dt.NewRow();
            newRow["LenguajeProgramacion"] = "--";
            dt.Rows.InsertAt(newRow, 0);
            gridLenguajesProgP.Columns[0].Visible = false;
        }


        gridLenguajesProgP.DataSource = dt;
        gridLenguajesProgP.DataBind();
    }



    /**GRID DE IDIOMAS**/
    protected void llenarGridIdiomasParcial()
    {

        DataTable dt = new DataTable();
        DataColumn column = new DataColumn();
        column.DataType = System.Type.GetType("System.String");
        column.ColumnName = "Idioma";
        dt.Columns.Add(column);

        DataRow newRow;
        if (listaLocalIdiomas.Count != 0)
        {

            for (int i = 0; i < listaLocalIdiomas.Count; ++i)
            {
                newRow = dt.NewRow();
                newRow["Idioma"] = listaLocalIdiomas[i];
                dt.Rows.InsertAt(newRow, i);
                gridIdiomasP.Columns[0].Visible = true;

            }
        }
        else
        {

            newRow = dt.NewRow();
            newRow["Idioma"] = "--";
            dt.Rows.InsertAt(newRow, 0);
            gridIdiomasP.Columns[0].Visible = false;
        }

        gridIdiomasP.DataSource = dt;
        gridIdiomasP.DataBind();
    }


    protected void llenarGridAreasInteresParcial() { 
    
        DataTable dt = new DataTable();
        DataColumn column = new DataColumn();
        column.DataType = System.Type.GetType("System.String");
        column.ColumnName = "AreaInteres";
        dt.Columns.Add(column);
        
        DataRow newRow;
        if (listaLocalAreasInteres.Count != 0)
        {

            for (int i = 0; i < listaLocalAreasInteres.Count; ++i)
            {
                newRow = dt.NewRow();
                newRow["AreaInteres"] = listaLocalAreasInteres[i];
                dt.Rows.InsertAt(newRow, i);
                gridAreasInteresP.Columns[0].Visible = true;

            }
        }
        else
        {

            newRow = dt.NewRow();
            newRow["AreaInteres"] = "--";
            dt.Rows.InsertAt(newRow, 0);
            gridAreasInteresP.Columns[0].Visible = false;
        }

        gridAreasInteresP.DataSource = dt;
        gridAreasInteresP.DataBind(); 
    }



    protected void llenarGridCualidadesParcial(){


        DataTable dt = new DataTable();
        DataColumn column = new DataColumn();
        column.DataType = System.Type.GetType("System.String");
        column.ColumnName = "Cualidad";
        dt.Columns.Add(column);
        
        DataRow newRow;
        if (listaLocalCualidades.Count != 0)
        {

            for (int i = 0; i < listaLocalCualidades.Count; ++i)
            {
                newRow = dt.NewRow();
                newRow["Cualidad"] = listaLocalCualidades[i];
                dt.Rows.InsertAt(newRow, i);
                gridCualidadesP.Columns[0].Visible = true;

            }
        }
        else
        {

            newRow = dt.NewRow();
            newRow["Cualidad"] = "--";
            dt.Rows.InsertAt(newRow, 0);
            gridCualidadesP.Columns[0].Visible = false;
        }

        gridCualidadesP.DataSource = dt;
        gridCualidadesP.DataBind();    
    
    }


   


}
