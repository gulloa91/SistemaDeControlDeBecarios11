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
    private static ControladoraBecarios controladoraBecarios;
    private static ControladoraCuentas controladoraCuentas;

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

        controladoraBecarios = new ControladoraBecarios();
        controladoraCuentas = new ControladoraCuentas();

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

                         if (!Page.IsPostBack)
                         {
                           llenarGridBecarios(1);
                           
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



    /* Requiere: n/a
    * 
    *  Efectúa: Metodo que se invoca al dar click en el botón "aceptar" del 'popUp'
    *           Recolecta todos los datos ingresados por el usuario, luego ordena a la controladora insertar o modificar un becario según sea el caso.
    *          En caso de ser nuevo becario , genera el nombre de usuario y contrasena e invoca al método que crea la cuenta.
    *          Luego, muestra los mensajes de aviso correspondientes.
    *          Por último, envía un correo al nuevo becario indicándole cuales son sus credenciales.
    *
    *  Modifica: Crea una nuevo becario en la base de datos y una nueva cuenta de usuario, ó cambia los datos de un becario ya existente.
    */
    protected void btnInvisible1_Click(object sender, EventArgs e)
    {

        if (camposPerfilVacios(0) == true)
        {

            TextInfo miTexto = CultureInfo.CurrentCulture.TextInfo;

            Object[] datos;

            datos = new Object[11];
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


            //para la parte de creación de cuenta y envio de correo

            CultureInfo currentCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
            TextInfo currentInfo = currentCulture.TextInfo;
            string cedula = this.txtCedula.Text;
            string nombre = this.txtNombre.Text.ToLower();
            
            char[] delim = {' '};
            string[] nombreSeparado = nombre.Split(delim);

            string apellido = this.txtApellido1.Text.ToLower();
            string apellido2 = this.txtApellido2.Text;
            string correo = this.txtCorreo.Text;

            int lg = cedula.Length - 3;
            string ced = cedula.Substring(lg, 3);
            string usuario = nombreSeparado[0] + "." + apellido + ced;
            string pass = commonService.getContrasena(nombre, apellido, ced);

            string nombreCompleto = (currentInfo.ToTitleCase(nombre) + " " + currentInfo.ToTitleCase(apellido) + " " + currentInfo.ToTitleCase(apellido2)).Trim();


            //se ejecuta la acción
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


            bool noCerrarVentana = false;
            switch (resultado)
            {


                case "Exito":
                    {

                        if (modoEjecucion == 1) // un insertar
                        {

                            if ((resultadoCreacionCuenta.Equals("Exito")) && ((resultadoPerfil.Equals("Exito")) || (resultadoPerfil.Equals("-1"))))
                            {
                                commonService.mensajeJavascript("El becario ha sido ingresado correctamente y su cuenta ha sido creada", "Éxito");
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
                            resultado = "ExitoUpate";
                        }
                        else
                        {
                            commonService.mensajeJavascript("Se ha modificado correctamente la información personal pero ha habido un problema al actualizar el perfil del becario", "Aviso");
                            resultado = "ExitoUpate";
                        }

                    } break;
                case "Error1":
                    {
                        commonService.mensajeJavascript("Ya existe un becario con la cédula digitada", "ERROR");
                        arreglarTabs();
                        noCerrarVentana = true;

                    } break;
                default:
                    {
                        commonService.mensajeJavascript("No fue posible modificar al becario.", "ERROR");
                    } break;
            }


            llenarGridBecarios(1);
            habilitarBotonesPrincipales(true);


            if (noCerrarVentana == false)
            {
                correrJavascript("cerrarPopUp();");
            }

            if (resultado == "Exito")
            {

                // Abrir mensaje de mandar correo
                commonService.mensajeEspera("Enviando correo de confirmación al becari@", "Enviando correo");

                this.btnInvisibleEnviarCorreo.CommandArgument = correo + "," + nombreCompleto + "," + pass + "," + usuario;
                commonService.correrJavascript("$('.btnInvisibleEnviarCorreo').click();");
            }

        }
        else {

          commonService.mensajeJavascript("En la sección del perfil hay datos que no se han guardado , para hacerlo debe presionar el botón color verde con el signo  \\+ en la correspondiente tabla", "AVISO");
          arreglarTabs();
          correrJavascript("seleccionarTabs();");

        }


    }



    /* Requiere: n/a
       Efectúa: Envía un correo
    *  Modifica: n/a
    */
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




    /* Requiere: Se debe haber seleccionado un becario
       Efectúa: Método que se invoca al confirmar la eliminación de un becario.
    *           Elimina el becario seleccionado y cualquier asignación que tuviera este en el semestre actual.
    *           
    *  Modifica: n/a
    */
    protected void btnInvisible2_Click(object sender, EventArgs e)
    {

        string resultadoEliminarAsignación = "-1";
        if (controladoraBecarios.tieneAsignacion(listaBecarios[rowIndex].cedula) == true)
        {
           resultadoEliminarAsignación =  eliminaAsignacion();
        }

        if ((resultadoEliminarAsignación.Equals("-1"))||(resultadoEliminarAsignación.Equals("Exito")))
        {

            Object[] datos = new Object[1];
            datos[0] = this.txtCedula.Text;

            string resultado = controladoraBecarios.ejecutar(3, datos, null);
            this.gridBecarios.SelectedIndex = -1;

            correrJavascript("cerrarPopUp();");

            if (resultado.Equals("Exito"))
            {
                commonService.mensajeJavascript("Se ha eliminado correctamente el becario", "Éxito");
            }
            else
            {
                commonService.mensajeJavascript("Se ha producido un error al eliminar el becario", "Error");
            }

            llenarGridBecarios(1);
        }
        else {
            commonService.mensajeJavascript("No se pudo eliminar al becario porque hubo un problema al eliminar la asignación", "Error");
        }

    }



    //Método que se invoca el dar click al botón 'cancelar' del 'popUp'
    protected void btnInvisible3_Click(object sender, EventArgs e)
    {

    }


    /* Requiere: n/a.
       Efectúa: Método que se invoca al dar click al botón 'insertar'
    *           Abre el popUp que permite ingresar el nuevo becario , limpia y habilita los campos.
    *           
    *  Modifica: n/a
    */
    protected void btnInsertarEncargado_Click(object sender, EventArgs e)
    {

        lblInstTab1.Text = "Favor completar la información solicitada y presionar el botón aceptar para completar la operación.";
        lblInstTab1.Visible=true;

        lblInstTab2.Text = "A continuación se le presentan unas tablas que resumen algunos aspectos de importancia para el proceso de becas. Se le solicita completar los datos de la forma más precisa posible.";
        lblInstTab2.Visible = true;

       
        mostrarBotonesPrincipales(false);

        vaciarCampos(0);
        habilitarCampos(true,0);

        limpiarListasPerfil();
        llenarGridsPerfil();
       
        modoEjecucion = 1;

        correrJavascript("abrirPopUp('Insertar Becario');");
        commonService.mostrarPrimerBotonDePopUp("PopUp");
    }



    /* Requiere: n/a.
       Efectúa: Metodo que pide llenar el grid pero bajo un criterio de selección (búsqueda) expresado por el usuario
    *           
    *  Modifica: n/a
    */  
    protected void btnBuscarBecario_Click(object sender, EventArgs e)
    {

        llenarGridBecarios(2);    
    }




    /* Requiere: Se debe haber seleccionado un becario del grid principal.
    * 
    *   Efectúa: Método que se invoca al dar click al botón 'modificar' en la vista completa.
    *            Guarda los datos actuales y luego habilita los campos para editarlos.
     *            
    *  Modifica: n/a.
    */
    protected void btnModificarBecario_Click(object sender, EventArgs e)
    {


        arreglarTabs();

        guardarDatosActuales(1);
        modoEjecucion = 2;

        mostrarBotonesPrincipales(false);

        habilitarCampos(true, 0);
       

        commonService.mostrarPrimerBotonDePopUp("PopUp");
    }



    /* Requiere: n/a.
    * 
    *   Efectúa: Activa el 'popUp' para confirmar la eliminación.
    *            Verifica si el becario seleccionado tiene una asignación actualmente ya que en caso afirmartivo debe advertir al usuario.
    *            
    *  Modifica: n/a
    */
    protected void btnEliminarBecario_Click(object sender, EventArgs e)
    {

        if (controladoraBecarios.tieneAsignacion( listaBecarios[rowIndex].cedula ) == true)
        {
            correrJavascript("abrirPopUpEliminar('Este becario becario tiene una asignación en el presente semestre por lo que al eliminarlo se eliminará también la asignación , ¿ está seguro de que desea continuar ? ');");
        }
        else {

          correrJavascript("abrirPopUpEliminar('¿ Está seguro de que desea eliminar el becario seleccionado ? ');");
        }


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

    /*
    * -----------------------------------------------------------------------
    * AUXILIARES
    * -----------------------------------------------------------------------
    */


    /* Requiere: n/a.
    * 
    *   Efectúa: Muestra u oculta botones.
    *            
    *  Modifica: n/a
    */
    protected void mostrarBotonesPrincipales(Boolean mostrar)
    {
        if (mostrar)
        {
            this.btnModificarBecarioDatos.Visible = true;
            this.btnEliminarBecarioDatos.Visible = true;
           
            this.btnModificarBecarioDatosP.Visible = true;
            

        }
        else
        {
            this.btnModificarBecarioDatos.Visible = false;
            this.btnEliminarBecarioDatos.Visible = false;
          
            this.btnModificarBecarioDatosP.Visible = false;
           
        }
    }



    /*  Requiere: n/a.
    * 
    *   Efectúa: Habilita o deshabiita botones.
    *            
    *   Modifica: n/a
    */
    protected void habilitarBotonesPrincipales(Boolean habilitar)
    {

         if (habilitar)
         {
             this.btnModificarBecarioDatos.Enabled = true;
             this.btnEliminarBecarioDatos.Enabled = true;
            
             this.btnModificarBecarioDatosP.Enabled = true;
             
          }
          else
          {
              this.btnModificarBecarioDatos.Enabled = false;
              this.btnEliminarBecarioDatos.Enabled = false;
              
              this.btnModificarBecarioDatosP.Enabled = false;

          }
     

    }



    /*  Requiere: n/a.
    * 
    *   Efectúa: Habilita o deshabiita campos.
    *            El parámetro "p" le indica si usar los campos de la vista completa ( 0 ) o parcial ( cualquier otro número )
    *            
    *   Modifica: n/a
    */
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



    /*  Requiere: n/a.
    * 
    *   Efectúa: Limpia los campos de datos.
    *            El parámetro "p" le indica si usar los campos de la vista completa ( 0 ) o parcial ( cualquier otro número )
    *            
    *   Modifica: n/a
    */
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




    /*  Requiere: n/a.
    * 
    *   Efectúa: Ordena eliminar cualquier asignación del presente semestre donde participe el becario 
    *            
    *   Modifica: n/a
    */
    public String eliminaAsignacion(){

        string ced = listaBecarios[rowIndex].cedula;
        int año = commonService.getAñoActual();
        int periodo = commonService.getPeriodoActual();
        return controladoraBecarios.eliminarAsignacion(ced, periodo, año);
    }




    /*  Requiere: n/a.
    * 
    *   Efectúa: Arregla las tabs , sea de la vista parcial o de la vista completa.
    *            
    *   Modifica: n/a
    */
    public void arreglarTabs(){

       int tipoPerfil =  Convert.ToInt32(Session["TipoPerfil"]);
       if (tipoPerfil == 0) //administrador --> vista completa
       {
          correrJavascript("destruirTabsVistaCompleta();");
          correrJavascript("crearTabsVistaCompleta();");  
       }
       else {  //  becario --> vista parcial

         correrJavascript("destruyeTabsP();");
         correrJavascript("crearTabsP();");
       }

    }


 /*
 * -----------------------------------------------------------------------
 * METODOS DE BECARIO
 * -----------------------------------------------------------------------
 */



    /*  Requiere: n/a.
    * 
    *   Efectúa: Se guardan los datos actuales --> 1 : datos de la vista administrador , otro numero : datos de la vista de becario
    *            
    *   Modifica: n/a.
    */
    protected void guardarDatosActuales(int vista) {


        TextInfo miTexto = CultureInfo.CurrentCulture.TextInfo;


        if (vista == 1)
        {

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
        else {

            datosViejos = new Object[10];
            datosViejos[0] = "";
            datosViejos[1] = commonService.procesarStringDeUI(miTexto.ToTitleCase(this.txtNombreP.Text.ToLower()));
            datosViejos[2] = commonService.procesarStringDeUI(miTexto.ToTitleCase(this.txtApellido1P.Text.ToLower()));
            datosViejos[3] = commonService.procesarStringDeUI(miTexto.ToTitleCase(this.txtApellido2P.Text.ToLower()));
            datosViejos[4] = this.txtCarneP.Text;
            datosViejos[5] = this.txtCedulaP.Text;
            datosViejos[6] = this.txtTelFijoP.Text;
            datosViejos[7] = this.txtCelularP.Text;
            datosViejos[8] = this.txtOtroTelP.Text;
            datosViejos[9] = commonService.procesarStringDeUI(this.txtCorreoP.Text);       
        }
       
    }



    /*  Requiere: n/a.
    * 
    *   Efectúa: Llena el grid principal de becarios ( de la vista completa )
    *            Pide la lista de becarios existentes ( los que cumplen con un criterio de búsqueda) y asigna dicha lista como fuente de datos del grid
    *   Modifica: Cambia todos los datos de "listaBecarios".
    */
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



    /*  Requiere: n/a.
    * 
    *   Efectúa: Crea una tabla con los campos que se van a desplegar en el grid
    *   
    *  Modifica: n/a.
    */
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
        column.ColumnName = "Correo Electrónico";
        dt.Columns.Add(column);

        column = new DataColumn();
        column.DataType = System.Type.GetType("System.String");
        column.ColumnName = "Teléfono Celular";
        dt.Columns.Add(column);

        return dt;

    }



    /*  Requiere: n/a.
    * 
    *   Efectúa: Pide una tabla y la llena con los datos de la lista de becarios previamente llenada.
    *   
    *  Modifica: n/a.
    */
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
                 newRow["Correo Electrónico"] = listaBecarios[i].correo;
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
            newRow["Correo Electrónico"] = "-";
            newRow["Teléfono Celular"] = "-";
            tabla.Rows.InsertAt(newRow, 0);
            this.gridBecarios.Columns[0].Visible = false;
        }

        return tabla;

    }



    /*  Requiere: Se debe haber seleccionado un becario del grid ( "rowIndex" debe representar un valor válido )
    * 
    *   Efectúa: Carga los campos de texto con los datos de determinado becario.
    *            Modo =  0 : campos de la vista completa , en cualquier otro caso campos de la vista parcial.
    *            
    *   Modifica: LLena todos los campos de texto de la interfaz.
    */
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




    /*  Requiere: n/a.
    * 
    *   Efectúa: Arregla los encabezados del grid.
    *            
    *   Modifica: n/a.
    */
    private void headersCorrectosGridBecarios()
    {
        this.gridBecarios.HeaderRow.BackColor = System.Drawing.Color.FromArgb(4562432);
        this.gridBecarios.HeaderRow.ForeColor = System.Drawing.Color.White;
        this.gridBecarios.HeaderRow.Cells[1].Text = "Nombre Completo";
        this.gridBecarios.HeaderRow.Cells[2].Text = "Carné";
        this.gridBecarios.HeaderRow.Cells[3].Text = "Correo Electrónico";
        this.gridBecarios.HeaderRow.Cells[4].Text = "Teléfono Celular";
    }



    /*  Requiere: n/a.
    * 
    *   Efectúa: Controla la paginación del grid.
    *            
    *   Modifica: n/a.
    */
    protected void gridBecarios_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        this.gridBecarios.PageIndex = e.NewPageIndex;
        this.gridBecarios.DataBind();
        this.headersCorrectosGridBecarios();
        llenarGridBecarios(1);
    }


    /*  Requiere: n/a.
    * 
    *   Efectúa: Controla la selección de becarios en el grid.
    *            Carga todos los datos correspondientes tanto datos personales como del perfil.
    *            
    *   Modifica: Asigna a "rowIndex" un valor válido.
    */
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
                   

                    lblInstTab1.Text = "Para editar la información debe presionar el botón modificar y para completar la operación presionar aceptar.";
                    lblInstTab1.Visible = true;

                    lblInstTab2.Text = "A continuación se le presentan unas tablas que resumen algunos aspectos de importancia para el proceso de becas. Se le solicita completar los datos de la forma más precisa posible. Para editar esta información debe presionar el botón modificar ubicado arriba.";
                    lblInstTab2.Visible = true;


                    guardarDatosActuales(1);
                    modoEjecucion = 2;

                    correrJavascript("abrirPopUp('Becarios');");


                } break;
        }
    }




    /*  Requiere: Todos los parámetros deben tener valor y ninguno puede ser nulo.
    * 
    *   Efectúa: Crea una cuenta para un becario y le asocia el perfil correspondiente.
    *            
    *   Modifica: n/a.
    */
    protected string crearCuenta( string cedula, string ced, string nombre, string apellido, string correo, string usuario, string pass, string nombreCompleto){

        string resultado = "Exito";

        Object[] datos = new Object[4];
        datos[0] = usuario;//this.txtUsuario.Text;
        datos[1] = pass;  //this.cntUsuario.Text;
        datos[2] = "";
        datos[3] = cedula;

        Object[] datosPerfil = new Object[2];
        datosPerfil[0] = usuario;
        datosPerfil[1]="Becario"; 

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



    /*  Requiere: n/a.
    * 
    *   Efectúa: Limpia las las lista locales donde se almacena la información del perfil del becario para evitar conflictos, y quita avisos
    *            
    *   Modifica: Borra todos los datos de las listas locales.
    */
    protected void limpiarListasPerfil()
    {

        listaLocalLenguajes.Clear();
        listaLocalIdiomas.Clear();
        listaLocalAreasInteres.Clear();
        listaLocalCualidades.Clear();
        lblAvisoPerfil.Visible = false;
    }



    /*  Requiere: n/a.
    * 
    *   Efectúa: Llena las listas locales donde se guarda la información del perfil del becario.
    *            
    *   Modifica: Todos los datos que tuvieran las lista se borran y se llenan con nuevos.
    */
    protected void llenarListasPerfil(String cedBecario)
    {

        listaLocalLenguajes = controladoraBecarios.consultarLenguajes(cedBecario);
        listaLocalIdiomas = controladoraBecarios.consultarIdiomas(cedBecario);     
        listaLocalAreasInteres = controladoraBecarios.consultarAreasInteres(cedBecario);      
        listaLocalCualidades = controladoraBecarios.consultarCualidades(cedBecario);
        
    }



    /*  Requiere: n/a.
    * 
    *   Efectúa: Llena los grid del perfil del becario
    *            
    *   Modifica: n/a.
    */
    protected void llenarGridsPerfil()
    {
        lblAvisoPerfil.Visible = false;
        llenarGridLenguajes();
        llenarGridIdiomas();
        llenarGridAreasInteres();
        llenarGridCualidades();
    }



    /*  Requiere: n/a.
    * 
    *   Efectúa: Verifica si ya se insertaron los datos personales necesarios para poder ingresar información del perfil
    *            
    *   Modifica: n/a.
    */
    protected bool datosPersonalesVacios(){ 
    
       bool retorno = false;

       if ((txtCedula.Text.ToString().Equals("")) || (txtNombre.ToString().Equals("")) || (txtApellido1.Text.ToString().Equals("")))
       {
           retorno = true;    
       }


       return retorno;
    }




    /*  Requiere: n/a.
    * 
    *   Efectúa: Verifica si se dejaron datos en los campos de texto en los grids del perfil.
    *            
    *   Modifica: n/a.
    */
    public Boolean camposPerfilVacios(int p)
    {


        TextBox txtBoxLeng;
        TextBox txtBoxIdioma;
        TextBox txtBoxIntereses;
        TextBox txtBoxCualidades;

        Boolean retorno = false;
        if (p == 0)
        {

            txtBoxLeng = (TextBox)gridLenguajesProg.FooterRow.Cells[0].FindControl("txtNuevoLenguaje");
            txtBoxIdioma = (TextBox)gridIdiomas.FooterRow.Cells[0].FindControl("txtNuevoIdioma");
            txtBoxIntereses = (TextBox)gridAreasInteres.FooterRow.Cells[0].FindControl("txtNuevaAreaInteres");
            txtBoxCualidades = (TextBox)gridCualidades.FooterRow.Cells[0].FindControl("txtNuevaCualidad");
            
        }
        else {

            txtBoxLeng = (TextBox)gridLenguajesProgP.FooterRow.Cells[0].FindControl("txtNuevoLenguajeParcial");
            txtBoxIdioma = (TextBox)gridIdiomasP.FooterRow.Cells[0].FindControl("txtNuevoIdiomaParcial");
            txtBoxIntereses = (TextBox)gridAreasInteresP.FooterRow.Cells[0].FindControl("txtNuevaAreaInteresParcial");
            txtBoxCualidades = (TextBox)gridCualidadesP.FooterRow.Cells[0].FindControl("txtNuevaCualidadParcial");
        }


        if ((txtBoxLeng.Text.Equals("")) && (txtBoxIdioma.Text.Equals("")) && (txtBoxIntereses.Text.Equals("")) && (txtBoxCualidades.Text.Equals("")))
        {
            retorno = true;
        }


        return retorno;
    }




    /*  Requiere: n/a.
    * 
    *   Efectúa: Controla la eliminación de datos de los grid del perfil.
    *            Primero verifica cual es el grid que se esta modificando, determina el indice de la fila y 
    *            procede a eliminar el dato correspondiente de la lista local, para finalmente actualizar el grid.
    *            
    *   Modifica: Remueve de la lista local correspondiente el dato que está en la posición "indiceFila" .
    */
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




    /*  Requiere: n/a.
    * 
    *   Efectúa: Controla la inserción de nuevos atributos a los grids del perfil.
    *            Primero verifica cual es el grid que se esta modificando, lee el dato ingresado en el campo de texto y 
    *            procede a insertarlo en la lista local correspondiente, para finalmente actualizar el grid.
    *            
    *   Modifica: Inserta nuevos datos en las lista locales de los perfiles .
    */
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

    /*  Requiere: La lista local "listaLocalLenguajes" debe estar debidamente instanciada .
    * 
    *   Efectúa: Llena el grid de "Lenguajes de Programación". 
    *            Primero crea una tabla que llena con los datos de la lista llamada "listaLocalLenguajes" para luego
    *            asignar dicha tabla como fuente de datos del grid.
    *            
    *   Modifica: n/a.
    */
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




    /*  Requiere: n/a.
    *   Efectúa: Controla la paginacion del grid "Lenguajes de Programación".
    *   Modifica: n/a.
    */
    protected void gridLenguajesProg_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
       this.gridLenguajesProg.PageIndex = e.NewPageIndex;
       this.gridLenguajesProg.DataBind();
       llenarGridLenguajes();
    }



    /**GRID DE IDIOMAS**/

    /*  Requiere: La lista local "listaLocalIdiomas" debe estar debidamente instanciada .
    * 
    *   Efectúa: Llena el grid de "Idiomas". 
    *            Primero crea una tabla que llena con los datos de la lista llamada "listaLocalIdiomas" para luego
    *            asignar dicha tabla como fuente de datos del grid.
    *            
    *   Modifica: n/a.
    */
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



    /*  Requiere: n/a.
    *   Efectúa: Controla la paginacion del grid "Idiomas".
    *   Modifica: n/a.
    */
    protected void gridIdiomas_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        this.gridIdiomas.PageIndex = e.NewPageIndex;
        this.gridIdiomas.DataBind();
        llenarGridIdiomas();
    }




    /**GRID DE AREAS DE INTERÉS**/

    /*  Requiere: La lista local "listaLocalAreasInteres" debe estar debidamente instanciada .
    * 
    *   Efectúa: Llena el grid de "Áreas de Interés". 
    *            Primero crea una tabla que llena con los datos de la lista llamada "listaLocalAreasInteres" para luego
    *            asignar dicha tabla como fuente de datos del grid.
    *            
    *   Modifica: n/a.
    */
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



    /*  Requiere: n/a.
    *   Efectúa: Controla la paginacion del grid "Áreas de Interés"
    *   Modifica: n/a.
    */
    protected void gridAreasInteres_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        this.gridAreasInteres.PageIndex = e.NewPageIndex;
        this.gridAreasInteres.DataBind();
        llenarGridAreasInteres();
    }



    /**GRID DE CUALIDADES PERSONALES**/

    /*  Requiere: La lista local "listaLocalCualidades" debe estar debidamente instanciada .
    * 
    *   Efectúa: Llena el grid de "Aptitudes". 
    *            Primero crea una tabla que llena con los datos de la lista llamada "listaLocalCualidades" para luego
    *            asignar dicha tabla como fuente de datos del grid.
    *            
    *   Modifica: n/a.
    */
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



    /*  Requiere: n/a.
    *   Efectúa: Controla la paginacion del grid "Aptitudes"
    *   Modifica: n/a.
    */
    protected void gridCualidades_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        this.gridCualidades.PageIndex = e.NewPageIndex;
        this.gridCualidades.DataBind();
        llenarGridCualidades();
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




    /*  Requiere: n/a.
    *   Efectúa: Método que se invoca al dar click al botón 'modificar' en la vista parcial.
    *             Guarda los datos actuales y luego habilita los campos para editarlos
    *   Modifica: n/a.
    */
    protected void btnModificarBecarioParcial_Click(object sender, EventArgs e)
    {

        TextInfo miTexto = CultureInfo.CurrentCulture.TextInfo;

        correrJavascript("destruyeTabsP();");
        correrJavascript("crearTabsP();");


        mostrarBotonesSecundariosP(true);

        guardarDatosActuales(2);
        modoEjecucion = 2;

        mostrarBotonesPrincipales(false);
        habilitarCampos(true, 1);
        

        
    }



    /*  Requiere: n/a.
    *   Efectúa: Método que se invoca al dar click al botón 'CANCELAR' en la vista parcial.
    *            Vuelve a cargar los datos anteriores para eliminar las modificiones y deshabilita los campos.
    *   Modifica: n/a.
    */
    protected void btnCancelarP_Click(object sender, EventArgs e)
    {

        correrJavascript("destruyeTabsP();");
        correrJavascript("crearTabsP();");

        cargarCamposBecario(1);
        
        llenarListasPerfil(cedulaBecarioActual);
        llenarGridsPerfil_vistaParcial();      

        habilitarCampos(false, 1);
       

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



    /*  Requiere: n/a.
    *   Efectúa: Método que se invoca al dar click al botón 'ACEPTAR' en la vista parcial.
    *            Toma los datos ingresados y pide modificar la información del becario correspondiente
    *   Modifica: n/a.
    */
    protected void btnAceptarP_Click(object sender, EventArgs e)
    {


        if (camposPerfilVacios(1) == true)
        {

            arreglarTabs();

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
            string resultadoPerfil = "";

            if (resultado.Equals("Exito"))
            {
                string mensaje = controladoraBecarios.eliminarPerfilBecario(cedulaBecarioActual);
                resultadoPerfil = controladoraBecarios.guardarPerfilBecario(listaLocalLenguajes, listaLocalIdiomas, listaLocalAreasInteres, listaLocalCualidades, cedulaBecarioActual);
            }
            else
            {
                commonService.mensajeJavascript("Se producido un error. Favor intentar más tarde", "Error");
            }

            if (resultadoPerfil.Equals("Exito"))
            {
                commonService.mensajeJavascript("Se ha modificado correctamente la información", "Éxito");
            }


            habilitarCampos(false, 1);          
           
            mostrarBotonesPrincipales(true);

        }
        else {

          commonService.mensajeJavascript("Aún hay datos escritos en algún grid del perfil, debe borrar esta información o presionar el botón con el signo  \\+  para guardarla ", "AVISO");
          arreglarTabs();
        }

    }



/*
* -----------------------------
*   VARIOS
* -----------------------------
*/


    /*  Requiere: n/a.
    *   Efectúa: Muestra u oculta los botones de la vista parcial.          
    *   Modifica: n/a.
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



    /*  Requiere: n/a.
    *   Efectúa: Consulta cuales son los datos los datos del becario que esta usando el sistema para cargar 
    *            los campos con los datos personales y datos del perfil correspondientes.          
    *   Modifica: n/a.
    */
    protected void consultarDatosBecarioLogueado()
    {

        string usuario = Session["Cuenta"].ToString();
        cedulaBecarioActual = controladoraBecarios.obtieneCedulaDeUsuario(usuario);
        Becario becarioActual = controladoraBecarios.obtenerBecarioPorCedula(cedulaBecarioActual);
        
        listaBecarios.Clear();
        listaBecarios.Add(becarioActual);

        cargarCamposBecario(1);

        habilitarCampos(false, 1);

        guardarDatosActuales(2);
        modoEjecucion = 2;

    }



  /*
   * ---------------------------------
   *   PERFIL BECARIO - vista parcial
   * --------------------------------
   */



    /*  Requiere: n/a.
    *   Efectúa: Llena los grids del perfil del becario en la vista parcial                 
    *   Modifica: n/a.
    */
    protected void llenarGridsPerfil_vistaParcial()
    {

        llenarGridLenguajesParcial();
        llenarGridIdiomasParcial();
        llenarGridAreasInteresParcial();
        llenarGridCualidadesParcial();
    }




    /*  Requiere: n/a.
    * 
    *   Efectúa: Controla la inserción de nuevos atributos a los grids del perfil del becario en la vista parcial .
    *            Primero verifica cual es el grid que se esta modificando, lee el dato ingresado en el campo de texto y 
    *            procede a insertarlo en la lista local correspondiente, para finalmente actualizar el grid.
    *            
    *   Modifica: Inserta nuevos datos en las lista locales de los perfiles .
    */
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



    /*  Requiere: n/a.
    * 
    *   Efectúa: Controla la eliminación de datos de los grid del perfil en la vista parcial.
    *            Primero verifica cual es el grid que se esta modificando, determina el indice de la fila y 
    *            procede a eliminar el dato correspondiente de la lista local, para finalmente actualizar el grid.
    *            
    *   Modifica: Remueve de la lista local correspondiente el dato que está en la posición "indiceFila" .
    */
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

    /*  Requiere: La lista local "listaLocalLenguajes" debe estar debidamente instanciada .
    * 
    *   Efectúa: Llena el grid de "Lenguajes de Programación" de la vista parcial. 
    *            Primero crea una tabla que llena con los datos de la lista llamada "listaLocalLenguajes" para luego
    *            asignar dicha tabla como fuente de datos del grid.
    *            
    *   Modifica: n/a.
    */
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





    /*  Requiere: n/a.
    *   Efectúa: Controla la paginacion del grid "Lenguajes de Programación" de la vista parcial.
    *   Modifica: n/a.
    */
    protected void gridLenguajesProgParcial_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        this.gridLenguajesProgP.PageIndex = e.NewPageIndex;
        this.gridLenguajesProgP.DataBind();
        llenarGridLenguajesParcial();
    }



    /**GRID DE IDIOMAS**/

    /*  Requiere: La lista local "listaLocalIdiomas" debe estar debidamente instanciada .
    * 
    *   Efectúa: Llena el grid de "Idiomas" de la vista parcial. 
    *            Primero crea una tabla que llena con los datos de la lista llamada "listaLocalIdiomas" para luego
    *            asignar dicha tabla como fuente de datos del grid.
    *            
    *   Modifica: n/a.
    */
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





    /*  Requiere: n/a.
    *   Efectúa: Controla la paginacion del grid "Idiomas" de la vista parcial.
    *   Modifica: n/a.
    */
    protected void gridIdiomasParcial_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        this.gridIdiomasP.PageIndex = e.NewPageIndex;
        this.gridIdiomasP.DataBind();
        llenarGridIdiomasParcial();
    }



    /**GRID DE ÁREAS DE INTERÉS**/

    /*  Requiere: La lista local "listaLocalAreasInteres" debe estar debidamente instanciada .
    * 
    *   Efectúa: Llena el grid de "Áreas de Interés" de la vista parcial. 
    *            Primero crea una tabla que llena con los datos de la lista llamada "listaLocalAreasInteres" para luego
    *            asignar dicha tabla como fuente de datos del grid.
    *            
    *   Modifica: n/a.
    */
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



    /*  Requiere: n/a.
    *   Efectúa: Controla la paginacion del grid "Áreas de Interés" de la vista parcial.
    *   Modifica: n/a.
    */
    protected void gridInteresParcial_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        this.gridAreasInteresP.PageIndex = e.NewPageIndex;
        this.gridAreasInteresP.DataBind();
        llenarGridAreasInteresParcial();
    }



    /**GRID DE APTITUDES ( CUALIDADES PERSONALES) **/

    /*  Requiere: La lista local "listaLocalCualidades" debe estar debidamente instanciada .
    * 
    *   Efectúa: Llena el grid de "Cualidades" de la vista parcial. 
    *            Primero crea una tabla que llena con los datos de la lista llamada "listaLocalCualidades" para luego
    *            asignar dicha tabla como fuente de datos del grid.
    *            
    *   Modifica: n/a.
    */
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


    /*  Requiere: n/a.
     *   Efectúa: Controla la paginacion del grid "Aptitudes" de la vista parcial.
     *   Modifica: n/a.
     */
    protected void gridCualidadesParcial_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        this.gridCualidadesP.PageIndex = e.NewPageIndex;
        this.gridCualidadesP.DataBind();
        llenarGridCualidadesParcial();
    }


}
