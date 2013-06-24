using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Globalization;

public partial class Asignaciones : System.Web.UI.Page
{

    /*Estados de la Asignación :
     * 
     * 1 : Aceptada
     * 2 : Pendiente de confirmación
     * 3 : Esperando confirmación de becario
     * 4 : Esperando confirmación de encargado
     * 5 : Rechazada por el becario.
     * 6 : Rechazada por el encargado.
     * 7 : Finalizada
     * */

    private static CommonServices commonService;
    private static EmailServices servicioCorreo;


    private static List<Asignacion> listaAsignaciones = new List<Asignacion>();
    private static List<Object[]> lstBecariosAsignadosEncargado = new List<Object[]>();

    //variable usada solo en la vista de becario para guardar la información de la asignación del becario actual
    private static List<Object[]> datosDeAsignacionDeBecario = new List<Object[]>();
    
    private static ControladoraAsignaciones controladoraAsignaciones =  new ControladoraAsignaciones();
    private static Object[] datosViejos;

    EncargadoDataSet.EncargadoDataTable tablaEncargados; //para guardar la lista de encargados existentes

    //listas para búsquedas
    private static List<Asignacion> listaBusquedaAño = new List<Asignacion>();
    private static List<Asignacion> listaBusquedaCiclo = new List<Asignacion>();
    private static List<Asignacion> listaBusquedaEstado = new List<Asignacion>();
    private static List<Asignacion> listaBusquedaEncargado = new List<Asignacion>();

    private static int añoActual;
    private static int periodoActual;
    static int modoEjecucion = 0;
    static int rowIndex;

    protected void Page_Load(object sender, EventArgs e)
    {

        determinaSemestreActual();
        llenarTablaEncargados();

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
            int permiso = 0; 
            if (permisos.Contains(8))
            {
                permiso = 8;
            }
            else
            {
                if (permisos.Contains(9))
                {
                    permiso = 9;
                }
                else
                {
                    if (permisos.Contains(10))
                    {
                        permiso = 10;
                    }
                }
            }

            switch (permiso)
            {
                case 8: // Vista Completa
                    {
                        MultiViewEncargado.ActiveViewIndex = 0;
                        if (!IsPostBack)
                        {
                            cargarDropDownsBusquedas();
                            llenarListaAsignaciones();
                            llenarGridAsignaciones( listaAsignaciones );
                        }
                    } break;

                case 9: // Vista Parcial Encargado
                    {
                        MultiViewEncargado.ActiveViewIndex = 1;
                        if (!IsPostBack)
                        {
                            llenarListaBecariosAsignados();
                            llenarGridaBecariosAsignadosVistaEncargado(lstBecariosAsignadosEncargado);
                            llenarCicloYAnioVistaEncargados();
                        }

                    } break;

                case 10: // Vista Parcial Becario
                    {
                        consultaDatosDeAsignacion();
                        MultiViewEncargado.ActiveViewIndex = 2;
                        if (!IsPostBack)
                        {
                            llenarInfoVistaBecario();
                        }

                    } break;

                default: // Vista sin permiso
                    {
                        MultiViewEncargado.ActiveViewIndex = 3;
                    } break;
            }
        }
         
    }



   /***************************************************************************
   * 
   *                       VISTA COMPLETA
   * 
   * **************************************************************************/



/*
* ------------------------------
*     CLICKS DE BOTONES
* ------------------------------
*/


    // Click del botón Insertar Asignacion
    protected void btnInsertarAsignacion_Click(object sender, EventArgs e)
    {

        
        mostrarBotonesPrincipales(false);
        habilitarContenidoAsignacion(true);
        limpiarContenidoAsignacion();

        /** Comentarios **/
        commonService.correrJavascript("$('#container_comentarios_encargadoybecario').css('display','none');");
        
        cargarDropDownBecarios();
        cargarDropDownEncargados(1);
        this.btnCantidadBecariosDeEncargado.Text = "Becarios asignados: -- ";
        this.btnCantidadBecariosDeEncargado.Enabled = false;
        
        determinaSemestreActual();

        this.lblCiclo.Text = convertirANumeroRomano(periodoActual); 
        this.lblAnio.Text = añoActual.ToString();

        modoEjecucion = 1;

        commonService.abrirPopUp("PopUpAsignacion", "Insertar Nueva Asignación");
        commonService.mostrarPrimerBotonDePopUp("PopUpAsignacion");


    }


    //Click del botón aceptar del popUp
    protected void btnInvisibleAceptarAsignacion_Click(object sender, EventArgs e)
    {

       
        TextInfo miTexto = CultureInfo.CurrentCulture.TextInfo;

        Object[] datos = new Object[12];

        datos[0] = dropDownBecariosPopUp.SelectedValue;
        datos[1] = dropDownEncargadosPopUp.SelectedValue;
        datos[2] = periodoActual;
        datos[3] = añoActual;
        datos[4] = txtTotalHoras.Text;
        datos[5] = txtUnidAcademica.Text;
        datos[6] = txtInfoDeUbicacion.Text;
        datos[7] = 2;
        datos[8] = true;
        datos[9] = txtComentarioBecario.Text;
        datos[10] = txtComentarioEncargado.Text;
        datos[11] = txtComentarioDireccion.Text;
        
        if (modoEjecucion == 2) // es una modificación
        {
            //cuando se modifica una asignación en realidad se debe crear una nueva ya que la asignacion actual se debe mantener almacenada
            //para posibles consultas históricas. Sin embargo, a nivel de BD se realiza la distinción usando el atributo "Activo"
            controladoraAsignaciones.dejarAsignacionInactiva(datosViejos[0].ToString(), datosViejos[1].ToString(), añoActual, periodoActual);
        }


        if (modoEjecucion == 1 || modoEjecucion == 2)
        {

            string mensajeResultado = controladoraAsignaciones.ejecutar(1, datos, "");

            commonService.cerrarPopUp("PopUpAsignacion");

            if (mensajeResultado.Equals("Exito"))
            {
                commonService.mensajeJavascript("Se ha creado correctamente una nueva asignación", "Éxito");
            }
            else
            {
                commonService.mensajeJavascript("No se pudo crear la inserción", "Error");
            }
        }


        //comentario de la dirección
        if (modoEjecucion == 3) {

           commonService.cerrarPopUp("PopUpAsignacion");
           
            string mensajeResultado = controladoraAsignaciones.ejecutar(3, datosViejos, this.txtComentarioDireccion.Text);
           if (mensajeResultado.Equals("Exito"))
           {
               commonService.mensajeJavascript("Se guardó el comentario correctamente", "Éxito");
           }
           else {
               commonService.mensajeJavascript("No se pudo guardar el comentario.", "Error");
           }

        }

        llenarListaAsignaciones();
        llenarGridAsignaciones(listaAsignaciones);

    }


    // Click del botón eliminar asignación
    protected void btnEliminarAsignacion_Click(object sender, EventArgs e)
    {

        datosViejos = new Object[9];

        datosViejos[0] = dropDownBecariosPopUp.SelectedValue;
        datosViejos[1] = dropDownEncargadosPopUp.SelectedValue;
        datosViejos[2] = periodoActual;
        datosViejos[3] = añoActual;
        datosViejos[4] = txtTotalHoras.Text;
        datosViejos[5] = txtUnidAcademica.Text;
        datosViejos[6] = txtInfoDeUbicacion.Text;
        datosViejos[7] = listaAsignaciones[rowIndex].Estado;
        datosViejos[8] = listaAsignaciones[rowIndex].Activo;
        commonService.abrirPopUp("PopUpEliminarAsignacion", "Eliminar Asignación");
    }



    // Click de confirmación de la eliminación ( botón invisible)
    protected void btnInvisibleEliminarAsignacion_Click(object sender, EventArgs e)
    {

        string resultado = controladoraAsignaciones.ejecutar(2, datosViejos, null);
        commonService.cerrarPopUp("PopUpAsignacion");

        if (resultado.Equals("Exito"))
        {
            commonService.mensajeJavascript("La asignación se eliminó correctamente", "Eliminado"); // Obviamente se tiene que cambiar con el resultado de vd
        }
        else {
            commonService.mensajeJavascript("Se ha producido un error en la eliminación", "Error"); // Obviamente se tiene que cambiar con el resultado de vd
        }

        llenarListaAsignaciones();
        llenarGridAsignaciones(listaAsignaciones);
    }



    // Click del botón modificar asignación
    protected void btnModificarAsignacion_Click(object sender, EventArgs e)
    {

        if (listaAsignaciones[rowIndex].Activo == true)
        {
            //solo si la asignación esta rechazada por el becario o el encargado entonces se puede modificar
            if ((listaAsignaciones[rowIndex].Estado == 5) || (listaAsignaciones[rowIndex].Estado == 6))
            {

                // la asignación fue rechazada por el becario entonces solo se habilita el dropdown de becarios
                if (listaAsignaciones[rowIndex].Estado == 5)
                {
                    this.dropDownEncargadosPopUp.Enabled = false;
                }
                else
                {
                    this.dropDownBecariosPopUp.Enabled = false;   // la asignación fue rechazada por el encargado entonces solo se habilita el dropdown de encargados
                }

                datosViejos = new Object[3];
                datosViejos[0] = dropDownBecariosPopUp.SelectedValue;
                datosViejos[1] = dropDownEncargadosPopUp.SelectedValue;
                datosViejos[2] = listaAsignaciones[rowIndex].Estado;

                modoEjecucion = 2;

                habilitarContenidoAsignacion(true);
                mostrarBotonesPrincipales(false);
                commonService.correrJavascript("$('#PopUpAsignacion').dialog('option', 'title', 'Modificar Asignación');");
                commonService.mostrarPrimerBotonDePopUp("PopUpAsignacion");
            }
            else
            {
                commonService.mensajeJavascript("Solo se puede cambiar una asignacón que ha sido rechazada por el becario o el encargado", "Aviso");
            }
        }
        else {
          commonService.mensajeJavascript("La asignación que quiere modificar ya fue reemplazada por otra por lo tanto no se puede modificar", "Aviso");
        }

       
    }

    // Click del botón para ver los becarios asignados a determinado encargado
    protected void btnCantidadBecariosDeEncargado_Click(object sender, EventArgs e)
    {

        commonService.correrJavascript("$('#container_comentarios_encargadoybecario').css('display','none');");
        llenarGridBecariosAsigandosAEncargado();
        String nombreDeEncargado = "Becarios asignados a: " + controladoraAsignaciones.getNombreEncargado(dropDownEncargadosPopUp.SelectedValue) ;  // Get nombre de encargado
        commonService.abrirPopUp("PopUpVerBecariosAsignados", nombreDeEncargado);        
    }


    //
    protected void btnComentarioDireccion_click(object sender, EventArgs e)
    {

        datosViejos = new Object[12];

        datosViejos[0] = dropDownBecariosPopUp.SelectedValue;
        datosViejos[1] = dropDownEncargadosPopUp.SelectedValue;
        datosViejos[2] = periodoActual;
        datosViejos[3] = añoActual;
        datosViejos[4] = txtTotalHoras.Text;
        datosViejos[5] = txtUnidAcademica.Text;
        datosViejos[6] = txtInfoDeUbicacion.Text;
        datosViejos[7] = listaAsignaciones[rowIndex].Estado;
        datosViejos[8] = listaAsignaciones[rowIndex].Activo;
        datosViejos[9] = txtComentarioBecario.Text;
        datosViejos[10] = txtComentarioEncargado.Text;
        datosViejos[11] = txtComentarioDireccion.Text;

        modoEjecucion = 3;

        this.txtComentarioDireccion.Enabled = true;
        mostrarBotonesPrincipales(false);
        commonService.mostrarPrimerBotonDePopUp("PopUpAsignacion");
    }


    /*
    * ------------------------------
    *    BÚSQUEDAS
    * ------------------------------
    */


    // BUSCAR CLICK
    protected void btnBuscar_Click(object sender, EventArgs e)
    {

        TextInfo miTexto = CultureInfo.CurrentCulture.TextInfo;
        string criterioDeBusqueda = miTexto.ToTitleCase( this.txtBuscarAsignacion.Text );
       

        List<Asignacion> listaAux = new List<Asignacion>();

        for (int i = 0; i < listaAsignaciones.Count; i++) {

            string cedEncargado = listaAsignaciones[i].CedulaEncargado;
            string cedBecario = listaAsignaciones[i].CedulaBecario;
            string nombreEncargardo = controladoraAsignaciones.getNombreEncargado(cedEncargado);
            string nombreBecario = controladoraAsignaciones.getNombreBecario(cedBecario);

            char[] delimiterChars = { ' ' };
            string[] words = nombreEncargardo.Split(delimiterChars);

            foreach (string palabra in words)
            {
                if (palabra.Equals(criterioDeBusqueda) )
                {
                    listaAux.Add(listaAsignaciones[i])  ;
                }
            }

            words = nombreBecario.Split(delimiterChars);
            foreach (string palabra in words)
            {
                if (palabra.Equals(criterioDeBusqueda))
                {
                    listaAux.Add(listaAsignaciones[i]);
                }
            }

        }

        llenarGridAsignaciones(listaAux);  
    }





    protected void cargarDropDownsBusquedas(){


        ListItem item;

        //dropdown año
        item = new ListItem("Seleccionar año", "0");
        this.dropDownAnio.Items.Add(item);       
        for (int i = 2010; i <= añoActual; ++i) {
           item = new ListItem(i.ToString(), i.ToString());
           this.dropDownAnio.Items.Add(item);       
        }
        dropDownAnio.DataBind();


        //dropdown ciclo
        item = new ListItem("Seleccionar ciclo", "0");
        this.dropDownCiclo.Items.Add(item);
        for (int i = 1; i <= 3; ++i)
        {
            item = new ListItem( convertirANumeroRomano(i), i.ToString() );
            this.dropDownCiclo.Items.Add(item);
        }
        this.dropDownCiclo.DataBind();


        //dropdown estado
        item = new ListItem("Seleccionar estado", "0");
        this.dropDownEstado.Items.Add(item);
        for (int i = 1; i <= 7; ++i) {
          string estado = interpretaEstado(i,true);
          item = new ListItem(estado, i.ToString());
          this.dropDownEstado.Items.Add(item);
        }
        dropDownEstado.DataBind();


        //dropdown encargados
        item = new ListItem("Seleccionar un encargado", "0");
        this.dropDownBusquedaEncargado.Items.Add(item);
        cargarDropDownEncargados(2);
    }


    //Método que se invoca a al seleccionar un año para las búsuqedas
    protected void dropDownAnio_SelectedIndexChanged(object sender, EventArgs e)
    {
        busquedaPorAño();
    }

    
    //buscador de año 
    protected void busquedaPorAño()
    {

        List<Asignacion> asignacionesParaRemover = new List<Asignacion>();
        listaBusquedaAño.Clear();

        int añoSeleccionado = Convert.ToInt32(dropDownAnio.SelectedValue);
        int periodoSeleccionado = Convert.ToInt32(dropDownCiclo.SelectedValue);
        int estadoSeleccionado = Convert.ToInt32(dropDownEstado.SelectedValue);
        string encargadoSeleccionado = dropDownBusquedaEncargado.SelectedValue;


        if (añoSeleccionado != 0)
        {
         
            for (int i = 0; i < listaAsignaciones.Count; i++)
            {
                if (listaAsignaciones[i].Año == añoSeleccionado)
                {
                    listaBusquedaAño.Add(listaAsignaciones[i]);
                }
            }

            asignacionesParaRemover.Clear();
            if (periodoSeleccionado != 0)
            {

                for (int i = 0; i < listaBusquedaAño.Count; ++i)
                {
                    if (!(listaBusquedaAño[i].Periodo == periodoSeleccionado))
                    {
                        asignacionesParaRemover.Add(listaBusquedaAño[i]);
                    }
                }

                for (int i = 0; i < asignacionesParaRemover.Count; ++i)
                {
                    listaBusquedaAño.Remove(asignacionesParaRemover[i]);
                }
            }

            asignacionesParaRemover.Clear();
            if (estadoSeleccionado != 0)
            {
               
                for (int i = 0; i < listaBusquedaAño.Count; ++i)
                {
                    if (!(listaBusquedaAño[i].Estado == estadoSeleccionado))
                    {
                        asignacionesParaRemover.Add(listaBusquedaAño[i]);
                    }
                }

                for (int i = 0; i < asignacionesParaRemover.Count; ++i)
                {
                    listaBusquedaAño.Remove(asignacionesParaRemover[i]);
                }
            }


            asignacionesParaRemover.Clear();
            if ( !(encargadoSeleccionado.Equals("0"))  )
            {

                for (int i = 0; i < listaBusquedaAño.Count; ++i)
                {
                    if (!(listaBusquedaAño[i].CedulaEncargado.Equals(encargadoSeleccionado)))
                    {
                       asignacionesParaRemover.Add(listaBusquedaAño[i]);
                    }
                }

                for (int i = 0; i < asignacionesParaRemover.Count; ++i)
                {
                    listaBusquedaAño.Remove(asignacionesParaRemover[i]);
                }
            }


            llenarGridAsignaciones(listaBusquedaAño);
        }
        else {

            if ((periodoSeleccionado == 0) && (estadoSeleccionado == 0) && (encargadoSeleccionado.Equals("0")))
            {
                llenarGridAsignaciones(listaAsignaciones);
            }
            else
            {
                if (periodoSeleccionado != 0)
                {
                    busquedaPorCiclo();
                }
                if (estadoSeleccionado != 0)
                {
                    busquedaPorEstado();
                }
                if (!(encargadoSeleccionado.Equals("0")))
                {
                    busquedaPorEncargado();
                }
            }           
        }

    }


    //Método que se invoca a al seleccionar un ciclo para las búsuqedas
    protected void dropDownCiclo_SelectedIndexChanged(object sender, EventArgs e)
    {
        busquedaPorCiclo();
    }

    //buscador de ciclo
    protected void busquedaPorCiclo()
    {

        List<Asignacion> asignacionesParaRemover = new List<Asignacion>();
        listaBusquedaCiclo.Clear();

        int cicloSeleccinado = Convert.ToInt32( dropDownCiclo.SelectedValue );
        int añoSeleccionado = Convert.ToInt32(dropDownAnio.SelectedValue);
        int estadoSeleccionado = Convert.ToInt32(dropDownEstado.SelectedValue);
        string encargadoSeleccionado = dropDownBusquedaEncargado.SelectedValue;

        if (cicloSeleccinado != 0)
        {
        
            for (int i = 0; i < listaAsignaciones.Count; i++)
            {
                if (listaAsignaciones[i].Periodo == cicloSeleccinado)
                {
                    listaBusquedaCiclo.Add(listaAsignaciones[i]);
                }
            }


            if (añoSeleccionado != 0)
            {
                asignacionesParaRemover.Clear();
                for (int i = 0; i < listaBusquedaCiclo.Count; ++i)
                {
                    if (!(listaBusquedaCiclo[i].Año == añoSeleccionado))
                    {
                        asignacionesParaRemover.Add(listaBusquedaCiclo[i]);
                    }
                }

                for (int i = 0; i < asignacionesParaRemover.Count; ++i)
                {
                    listaBusquedaCiclo.Remove(asignacionesParaRemover[i]);
                }
            }


            if (estadoSeleccionado != 0)
            {
                asignacionesParaRemover.Clear();
                for (int i = 0; i < listaBusquedaCiclo.Count; ++i)
                {
                    if (!(listaBusquedaCiclo[i].Estado == estadoSeleccionado))
                    {
                        asignacionesParaRemover.Add(listaBusquedaCiclo[i]);
                    }
                }

                for (int i = 0; i < asignacionesParaRemover.Count; ++i)
                {
                    listaBusquedaCiclo.Remove(asignacionesParaRemover[i]);
                }
            }


            if (!(encargadoSeleccionado.Equals("0")))
            {

                asignacionesParaRemover.Clear();
                for (int i = 0; i < listaBusquedaCiclo.Count; ++i)
                {
                    if (!(listaBusquedaCiclo[i].CedulaEncargado.Equals(encargadoSeleccionado)))
                    {
                        asignacionesParaRemover.Add(listaBusquedaCiclo[i]);
                    }
                }

                for (int i = 0; i < asignacionesParaRemover.Count; ++i)
                {
                    listaBusquedaCiclo.Remove(asignacionesParaRemover[i]);
                }
            }


            llenarGridAsignaciones(listaBusquedaCiclo);
        }
        else
        {

            if ((añoSeleccionado == 0) && (estadoSeleccionado == 0) && (encargadoSeleccionado.Equals("0")))
            {
                llenarGridAsignaciones(listaAsignaciones);
            }
            else
            {
                if (añoSeleccionado != 0)
                {
                    busquedaPorAño();
                }
                if (estadoSeleccionado != 0)
                {
                    busquedaPorEstado();
                }

                if (!(encargadoSeleccionado.Equals("0")))
                {
                    busquedaPorEncargado();
                }
            }
        }

    }


    //Método que se invoca a al seleccionar un estado para las búsuqedas
    protected void dropDownEstado_SelectedIndexChanged(object sender, EventArgs e)
    {
        busquedaPorEstado();
    }
    

    //buscador de estado
    protected void busquedaPorEstado()
    {

        List<Asignacion> asignacionesParaRemover = new List<Asignacion>();
        listaBusquedaEstado.Clear();

        int estadoSeleccionado = Convert.ToInt32(dropDownEstado.SelectedValue);
        int periodoSeleccionado = Convert.ToInt32(dropDownCiclo.SelectedValue);
        int añoSeleccionado = Convert.ToInt32(dropDownAnio.SelectedValue);
        string encargadoSeleccionado = dropDownBusquedaEncargado.SelectedValue;

        if (estadoSeleccionado != 0)
        {

            for (int i = 0; i < listaAsignaciones.Count; i++)
            {
                if (listaAsignaciones[i].Estado == estadoSeleccionado)
                {
                    listaBusquedaEstado.Add(listaAsignaciones[i]);
                }
            }


            if (periodoSeleccionado != 0)
            {
                asignacionesParaRemover.Clear();
                for (int i = 0; i < listaBusquedaEstado.Count; ++i)
                {
                    if (!(listaBusquedaEstado[i].Periodo == periodoSeleccionado))
                    {
                      asignacionesParaRemover.Add(listaBusquedaEstado[i]);
                    }
                }

                for (int i = 0; i < asignacionesParaRemover.Count; ++i)
                {
                    listaBusquedaEstado.Remove(asignacionesParaRemover[i]);
                }
            }


            if (añoSeleccionado != 0)
            {
                asignacionesParaRemover.Clear();
                for (int i = 0; i < listaBusquedaEstado.Count; ++i)
                {
                    if (!(listaBusquedaEstado[i].Año == añoSeleccionado))
                    {
                        asignacionesParaRemover.Add(listaBusquedaEstado[i]);                     
                    }
                }

                for (int i = 0; i < asignacionesParaRemover.Count; ++i)
                {
                    listaBusquedaEstado.Remove(asignacionesParaRemover[i]);
                }
            }


            if (!(encargadoSeleccionado.Equals("0")))
            {

                asignacionesParaRemover.Clear();
                for (int i = 0; i < listaBusquedaEstado.Count; ++i)
                {
                    if (!(listaBusquedaEstado[i].CedulaEncargado.Equals(encargadoSeleccionado)))
                    {
                        asignacionesParaRemover.Add(listaBusquedaEstado[i]);
                    }
                }

                for (int i = 0; i < asignacionesParaRemover.Count; ++i)
                {
                    listaBusquedaEstado.Remove(asignacionesParaRemover[i]);
                }
            }


            llenarGridAsignaciones(listaBusquedaEstado);
        }
        else
        {

            if ((añoSeleccionado == 0) && (periodoSeleccionado == 0) && (encargadoSeleccionado.Equals("0")))
            {
                llenarGridAsignaciones(listaAsignaciones);
            }
            else
            {
                if (añoSeleccionado != 0)
                {
                    busquedaPorAño();
                }
                if (periodoSeleccionado != 0)
                {
                    busquedaPorCiclo();
                }
                if (!(encargadoSeleccionado.Equals("0")))
                {
                    busquedaPorEncargado();
                }
            }
        }

    }



    protected void dropDownBusquedaEncargado_SelectedIndexChanged(object sender, EventArgs e)
    {
        busquedaPorEncargado();
    }


    //buscador de encargado
    protected void busquedaPorEncargado() {

        List<Asignacion> asignacionesParaRemover = new List<Asignacion>();
     
        listaBusquedaEncargado.Clear();

        int estadoSeleccionado = Convert.ToInt32(dropDownEstado.SelectedValue);
        int periodoSeleccionado = Convert.ToInt32(dropDownCiclo.SelectedValue);
        int añoSeleccionado = Convert.ToInt32(dropDownAnio.SelectedValue);
        string encargadoSeleccionado = dropDownBusquedaEncargado.SelectedValue;

        if ( !(encargadoSeleccionado.Equals("0")))
        {

            for (int i = 0; i < listaAsignaciones.Count; i++)
            {
                if (listaAsignaciones[i].CedulaEncargado.Equals(encargadoSeleccionado))
                {
                    listaBusquedaEncargado.Add(listaAsignaciones[i]);
                }
            }


            if (añoSeleccionado != 0)
            {
                asignacionesParaRemover.Clear();
                for (int i = 0; i < listaBusquedaEncargado.Count; ++i)
                {
                    if (!(listaBusquedaEncargado[i].Año == añoSeleccionado))
                    {
                        asignacionesParaRemover.Add(listaBusquedaEncargado[i]);
                    }
                }

                for (int i = 0; i < asignacionesParaRemover.Count; ++i)
                {
                    listaBusquedaEncargado.Remove(asignacionesParaRemover[i]);
                }
            }


            if (periodoSeleccionado != 0)
            {
                asignacionesParaRemover.Clear();
                for (int i = 0; i < listaBusquedaEncargado.Count; ++i)
                {
                    if (!(listaBusquedaEncargado[i].Periodo == periodoSeleccionado))
                    {
                        asignacionesParaRemover.Add(listaBusquedaEncargado[i]);
                    }
                }

                for (int i = 0; i < asignacionesParaRemover.Count; ++i)
                {
                    listaBusquedaEncargado.Remove(asignacionesParaRemover[i]);
                }
            }


            if (estadoSeleccionado != 0)
            {
                asignacionesParaRemover.Clear();
                for (int i = 0; i < listaBusquedaEncargado.Count; ++i)
                {
                    if (!(listaBusquedaEncargado[i].Estado == estadoSeleccionado))
                    {
                        asignacionesParaRemover.Add(listaBusquedaEncargado[i]);
                    }
                }

                for (int i = 0; i < asignacionesParaRemover.Count; ++i)
                {
                    listaBusquedaEncargado.Remove(asignacionesParaRemover[i]);
                }
            }


            llenarGridAsignaciones(listaBusquedaEncargado);
        }
        else
        {

            if ((añoSeleccionado == 0) && (periodoSeleccionado == 0) && (estadoSeleccionado == 0))
            {
                llenarGridAsignaciones(listaAsignaciones);
            }
            else
            {
                if (añoSeleccionado != 0)
                {
                    busquedaPorAño();
                }
                if (periodoSeleccionado != 0)
                {
                    busquedaPorCiclo();
                }

                if (estadoSeleccionado != 0)
                {
                    busquedaPorEstado();
                }
            }
        }

    }


    //metodo q actualiza el botón de cantidad de becarios cuando se selecciona un encargado
    protected void seleccionaEncargado_dropDownPopUp(object sender, EventArgs e)
    {

        /** Comentarios **/
        commonService.correrJavascript("$('#container_comentarios_encargadoybecario').css('display','none');");
        

        string cedEncargadoSeleccionado = dropDownEncargadosPopUp.SelectedValue;

        int cantidadBecariosAsignados = controladoraAsignaciones.contarBecariosAsignados(cedEncargadoSeleccionado, añoActual, periodoActual);

        this.btnCantidadBecariosDeEncargado.Text = "Becarios asignados : " + cantidadBecariosAsignados;
        this.btnCantidadBecariosDeEncargado.Enabled = true;
    }


 /*
 * ----------------------------------------
 *   AUXILIARES - MOSTRAR BOTONES Y OTROS
 * ----------------------------------------
 */


    //Define que botones se van a mostrar
    protected void mostrarBotonesPrincipales(Boolean mostrar)
    {
        if (mostrar)
        {
            this.btnModificarAsignacion.Visible = true;
            this.btnEliminarAsignacion.Visible = true;
            this.btnComentario.Visible = true;
        }
        else
        {
            this.btnModificarAsignacion.Visible = false;
            this.btnEliminarAsignacion.Visible = false;
            this.btnComentario.Visible = false;
        }
    }

    protected void habilitarContenidoAsignacion(Boolean mostrar)
    {
        if (mostrar)
        {
            this.dropDownBecariosPopUp.Enabled = true;
            this.dropDownEncargadosPopUp.Enabled = true;
            this.txtUnidAcademica.Enabled = true;
            this.txtInfoDeUbicacion.Enabled = true;
            this.txtTotalHoras.Enabled = true;
            this.txtComentarioDireccion.Enabled = true;
        }
        else
        {
            this.dropDownBecariosPopUp.Enabled = false;
            this.dropDownEncargadosPopUp.Enabled = false;
            this.txtUnidAcademica.Enabled = false;
            this.txtInfoDeUbicacion.Enabled = false;
            this.txtTotalHoras.Enabled = false;
            this.txtComentarioDireccion.Enabled = false;
        }
    }

    protected void limpiarContenidoAsignacion()
    {

        this.dropDownBecariosPopUp.Items.Clear();
        this.dropDownEncargadosPopUp.Items.Clear();
        this.txtUnidAcademica.Text = "";
        this.txtInfoDeUbicacion.Text = "";
        this.txtTotalHoras.Text = "";
        this.txtComentarioDireccion.Text = "";
        this.txtComentarioBecario.Text = "";
        this.txtComentarioEncargado.Text = "";
    }




    protected string convertirANumeroRomano(int num){

        string retorno = "";

        if (num == 1) {
            retorno = "I";
        }
        if (num == 2) {
            retorno = "II";
        }
        if (num == 3) {
            retorno = "III";
        }


        return retorno;
    }


    /*Estados de la Asignación :
     * 
     * 1 : Aceptada
     * 2 : Pendiente de confirmación
     * 3 : Esperando confirmación de becario
     * 4 : Esperando confirmación de encargado
     * 5 : Rechazada por el becario.
     * 6 : Rechazada por el encargado.
     * 7 : Finalizada
     * */
    protected String interpretaEstado(int estado, bool activo){

        string respuesta = "";

        switch (estado) {

            case 1:
                {
                    respuesta = "Aceptada";
                }break;
            case 2:
                {
                    respuesta = "Pendiente de confirmación";
                }break;
            case 3:
                {
                    respuesta = "Esperando confirmación de becario";
                }break;
            case 4:
                {
                    respuesta = "Esperando confirmación de encargado";
                } break;
            case 5:
                {
                    respuesta = "Rechazada por el becario";
                    if (activo == false) {
                        respuesta += " (ya reemplazada)";
                    }
                } break;
            case 6:
                {
                    respuesta = "Rechazada por encargado";
                    if (activo == false)
                    {
                        respuesta += " (ya reemplazada)";
                    }
                } break;
            case 7:
                {
                    respuesta = "Finalizada";
                } break;

            default: //desconocido
                {
                    respuesta = "Desconocido";
                } break;
        
        }

        return respuesta;
    
    }


  /*
   * ------------------------------
   *       VARIOS
   * ------------------------------
   */


    protected void determinaSemestreActual() {

        DateTime fecha = DateTime.Now;
        añoActual = fecha.Year;
        int mes = fecha.Month;

        periodoActual = -1;

        if ((mes >= 3) && (mes <= 6))
        {
            periodoActual = 1;
        }
        else {

            if ((mes >= 7) && (mes <= 12))
            {
                periodoActual = 2;
            }
            else {
                periodoActual = 3;
            }
        }
    
    }




    protected void llenarListaAsignaciones()
    {
       listaAsignaciones = controladoraAsignaciones.consultarTablaAsignacionesCompleta();
    }


    // Llenar tabla con todas las asignaciones existentes
    protected void llenarGridAsignaciones( List<Asignacion> listaDatos )
    {


        //listaAsignaciones = controladoraAsignaciones.consultarTablaAsignacionesCompleta();

        DataTable tablaAsignaciones = crearTablaAsignaciones();
        DataRow newRow;

        if (listaDatos.Count > 0)
        {
            for (int i = 0; i < listaDatos.Count; ++i)
            {

                newRow = tablaAsignaciones.NewRow();
                newRow["Encargado"] = controladoraAsignaciones.getNombreEncargado(listaDatos[i].CedulaEncargado);
                newRow["Becario"] = controladoraAsignaciones.getNombreBecario(listaDatos[i].CedulaBecario);
                newRow["Ciclo"] = convertirANumeroRomano(listaDatos[i].Periodo);
                newRow["Año"] = listaDatos[i].Año;
                newRow["Estado"] = interpretaEstado(listaDatos[i].Estado, listaDatos[i].Activo);
                tablaAsignaciones.Rows.InsertAt(newRow, i);
            }
        }
        else
        {

            newRow = tablaAsignaciones.NewRow();
            newRow["Encargado"] = "-";
            newRow["Becario"] = "-";
            newRow["Ciclo"] = "-";
            newRow["Año"] = "-";
            newRow["Estado"] = "-";
            tablaAsignaciones.Rows.InsertAt(newRow, 0);
       }
    

        this.GridAsignaciones.DataSource = tablaAsignaciones;
        this.GridAsignaciones.DataBind();
        this.HeadersCorrectosAsignaciones();
    }



    protected DataTable crearTablaAsignaciones()
    {

        DataTable dt = new DataTable();
        DataColumn column;

        column = new DataColumn();
        column.DataType = System.Type.GetType("System.String");
        column.ColumnName = "Encargado";
        dt.Columns.Add(column);

        column = new DataColumn();
        column.DataType = System.Type.GetType("System.String");
        column.ColumnName = "Becario";
        dt.Columns.Add(column);

        column = new DataColumn();
        column.DataType = System.Type.GetType("System.String");
        column.ColumnName = "Ciclo";
        dt.Columns.Add(column);

        column = new DataColumn();
        column.DataType = System.Type.GetType("System.String");
        column.ColumnName = "Año";
        dt.Columns.Add(column);

        column = new DataColumn();
        column.DataType = System.Type.GetType("System.String");
        column.ColumnName = "Estado";
        dt.Columns.Add(column);

        return dt;
    }


    //carga el dropdown con los becarios que estan pendientes de asignacion en el ciclo lectivo actual
    protected void cargarDropDownBecarios() {


        AsignacionesDataSet.BecarioSinAsignacionDataTable tablaBecarios = controladoraAsignaciones.consultaBecariosSinAsignacion(periodoActual, añoActual);
        ListItem item;


        this.dropDownBecariosPopUp.SelectedIndex = -1;
        this.dropDownBecariosPopUp.SelectedValue = null;

        item = new ListItem("Seleccionar un becario", "0");
        this.dropDownBecariosPopUp.Items.Add(item);

        foreach(DataRow r in tablaBecarios.Rows){
         
           string nombre = commonService.procesarStringDeUI(r["Nombre"].ToString()) + " " + commonService.procesarStringDeUI(r["Apellido1"].ToString()) + " " + commonService.procesarStringDeUI(r["Apellido2"].ToString());
           item = new ListItem(nombre, commonService.procesarStringDeUI(r["Cedula"].ToString()));  
            this.dropDownBecariosPopUp.Items.Add(item);
        }

        this.dropDownBecariosPopUp.DataBind();
       
    }



    protected void llenarTablaEncargados() {

       tablaEncargados = controladoraAsignaciones.obtenerEncargadosCompletos();
    }


    //carga el dropdown de encargados con todos los encargados existentes
    // el parámetro "aux" sirve para saber cual dropdown de encargados llenar: 1-> el del popUp , cualquier otro número -> el de búsquedas
    protected void cargarDropDownEncargados(int aux) 
    {


        //EncargadoDataSet.EncargadoDataTable tablaEncargados = controladoraAsignaciones.obtenerEncargadosCompletos();
        ListItem item;

        this.dropDownEncargadosPopUp.Items.Clear();
        this.dropDownEncargadosPopUp.SelectedIndex = -1;
        this.dropDownEncargadosPopUp.SelectedValue = null;
       
        item = new ListItem("Seleccionar un encargado", "0");
        this.dropDownEncargadosPopUp.Items.Add(item);
        

        foreach (DataRow r in tablaEncargados.Rows)
        {

            string nombre = commonService.procesarStringDeUI(r["Nombre"].ToString()) + " " + commonService.procesarStringDeUI(r["Apellido1"].ToString()) + " " + commonService.procesarStringDeUI(r["Apellido2"].ToString());
            item = new ListItem(nombre, commonService.procesarStringDeUI(r["Cedula"].ToString()));
            if (aux == 1)
            {
                this.dropDownEncargadosPopUp.Items.Add(item);
            }
            else {
                this.dropDownBusquedaEncargado.Items.Add(item);
            }
        }

        this.dropDownEncargadosPopUp.DataBind();
        

    }



    // Seleccionar tupla del grid de asignaciones con la flecha
    protected void GridAsignaciones_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        switch (e.CommandName)
        {
            // Consultar tupla
            case "btnSeleccionarTupla_Click":
                {

                    rowIndex = Convert.ToInt32(e.CommandArgument);
                    int pageIndex = this.GridAsignaciones.PageIndex;
                    int pageSize = this.GridAsignaciones.PageSize;
                    rowIndex += (pageIndex * pageSize);

                    limpiarContenidoAsignacion();
                    cargarCamposAsignacion();

                    
                    mostrarBotonesPrincipales(true);
                    habilitarContenidoAsignacion(false);

                    /** Comentarios **/
                    commonService.correrJavascript("$('#container_comentarios_encargadoybecario').css('display','block');");

                    commonService.abrirPopUp("PopUpAsignacion", "Consultar Asignación");
                    commonService.esconderPrimerBotonDePopUp("PopUpAsignacion");               
                   
                } break;
        }
    }



    //Carga los campos correspondientes de una asignación seleccionada por el usuario
    protected void cargarCamposAsignacion(){


        this.lblCiclo.Text = convertirANumeroRomano( listaAsignaciones[rowIndex].Periodo);
        this.lblAnio.Text = listaAsignaciones[rowIndex].Año.ToString();  

        this.txtTotalHoras.Text = listaAsignaciones[rowIndex].TotalHoras.ToString();
        this.txtUnidAcademica.Text = listaAsignaciones[rowIndex].SiglasUA;
        this.txtInfoDeUbicacion.Text = listaAsignaciones[rowIndex].InfoUbicacion;
        this.txtComentarioBecario.Text = listaAsignaciones[rowIndex].ComentarioBecario;
        this.txtComentarioEncargado.Text = listaAsignaciones[rowIndex].ComentarioEncargado;
        this.txtComentarioDireccion.Text = listaAsignaciones[rowIndex].ComentarioDireccion;
       
        cargarDropDownEncargados(1);
        string cedEncargado = listaAsignaciones[rowIndex].CedulaEncargado;
        dropDownEncargadosPopUp.SelectedValue = cedEncargado;

        cargarDropDownBecarios();

        string cedBecario = listaAsignaciones[rowIndex].CedulaBecario;
        string becarioSeleccionado = controladoraAsignaciones.getNombreBecario(cedBecario);

        ListItem item = new ListItem(becarioSeleccionado, cedBecario);
        this.dropDownBecariosPopUp.Items.Add(item);
        this.dropDownBecariosPopUp.DataBind();
       
        dropDownBecariosPopUp.SelectedValue = cedBecario;
       
        
        int cantidadBecariosAsignados = controladoraAsignaciones.contarBecariosAsignados(cedEncargado, añoActual, periodoActual);

        this.btnCantidadBecariosDeEncargado.Text = "Becarios asignados : " + cantidadBecariosAsignados;
        this.btnCantidadBecariosDeEncargado.Enabled = false;


    }



    // Llenar el grid que muestra los becarios asignados actualmente a determinado encargado
    protected void llenarGridBecariosAsigandosAEncargado()
    {

        string cedEncargado = dropDownEncargadosPopUp.SelectedValue;
        List<Object[]> listaBecarios = controladoraAsignaciones.consultarBecariosAsignadosAEncargado(cedEncargado, añoActual, periodoActual);

        

        DataTable tablaBecariosAsigandosAEncargado = crearTablaBecariosAsigandosAEncargado();
        DataRow newRow;

        if (listaBecarios.Count > 0)
        {
            for (int i = 0; i < listaBecarios.Count; ++i)
            {
                newRow = tablaBecariosAsigandosAEncargado.NewRow();
                newRow["Nombre"] = listaBecarios[i][0].ToString() + " " + listaBecarios[i][1].ToString() + " " + listaBecarios[i][2].ToString();
                newRow["Carné"] = listaBecarios[i][3].ToString();
                newRow["Correo"] = listaBecarios[i][4].ToString();
                newRow["Celular"] = listaBecarios[i][5].ToString();
                tablaBecariosAsigandosAEncargado.Rows.InsertAt(newRow, i);
            }
        }
        else
        {
         
            newRow = tablaBecariosAsigandosAEncargado.NewRow();
            newRow["Nombre"] = "-";
            newRow["Carné"] = "-";
            newRow["Correo"] = "-";
            newRow["Celular"] = "-";

            tablaBecariosAsigandosAEncargado.Rows.InsertAt(newRow, 0);

        }

       
        gridBecariosAsignadosAEncargado.DataSource = tablaBecariosAsigandosAEncargado;
        gridBecariosAsignadosAEncargado.DataBind();
        this.HeadersCorrectosBecariosAsigandosAEncargado();
    }



    // Le da formato a las columnas del grid que muestra los becarios asignados actualmente a determinado encargado
    protected DataTable crearTablaBecariosAsigandosAEncargado()
    {

        DataTable dt = new DataTable();
        DataColumn column;

        column = new DataColumn();
        column.DataType = System.Type.GetType("System.String");
        column.ColumnName = "Nombre";
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
        column.ColumnName = "Celular";
        dt.Columns.Add(column);

        return dt;
    }

    // Aplica nombre a las columnas así como color
    private void HeadersCorrectosAsignaciones()
    {
        this.GridAsignaciones.HeaderRow.BackColor = System.Drawing.Color.FromArgb(4562432);
        this.GridAsignaciones.HeaderRow.ForeColor = System.Drawing.Color.White;
        this.GridAsignaciones.HeaderRow.Cells[1].Text = "Encargado";
        this.GridAsignaciones.HeaderRow.Cells[2].Text = "Becario";
        this.GridAsignaciones.HeaderRow.Cells[3].Text = "Ciclo";
        this.GridAsignaciones.HeaderRow.Cells[4].Text = "Año";
        this.GridAsignaciones.HeaderRow.Cells[5].Text = "Estado";
    }

    // Aplica nombre a las columnas así como color
    private void HeadersCorrectosBecariosAsigandosAEncargado()
    {
        gridBecariosAsignadosAEncargado.HeaderRow.BackColor = System.Drawing.Color.FromArgb(4562432);
        gridBecariosAsignadosAEncargado.HeaderRow.ForeColor = System.Drawing.Color.White;
        gridBecariosAsignadosAEncargado.HeaderRow.Cells[0].Text = "Nombre";
        gridBecariosAsignadosAEncargado.HeaderRow.Cells[1].Text = "Carné";
        gridBecariosAsignadosAEncargado.HeaderRow.Cells[2].Text = "Correo";
        gridBecariosAsignadosAEncargado.HeaderRow.Cells[3].Text = "Celular";
    }

    //Controla la paginación del grid
    protected void gridAsignaciones_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        this.GridAsignaciones.PageIndex = e.NewPageIndex;
        this.GridAsignaciones.DataBind();
        this.HeadersCorrectosAsignaciones();
    }


    /***************************************************************************
     * 
     *                       VISTA BECARIO
     * 
     * **************************************************************************/


    //consulta en la BD los datos que se ocupan sobre la asignación del becario logueado actualmente
    protected void consultaDatosDeAsignacion() {

       string usuario = Session["Cuenta"].ToString();
       string cedBecario = controladoraAsignaciones.obtieneCedulaDeUsuario(usuario);
       datosDeAsignacionDeBecario = controladoraAsignaciones.consultarAsignacionDeBecario(cedBecario,añoActual,periodoActual);    
    }


    protected void actualizarEstadoAsignacionVistaBecario(int estadoAsignacion)
    {
       
        string mensajeAMostrar = "";

        switch (estadoAsignacion) {

            case 4: 
                {
                   mensajeAMostrar = "La asignación aún no esta completa ya que falta la confirmación del encargado.";          
                }break;
            case 5:
                {
                    mensajeAMostrar = "Usted ha rechazado esta asignación. Su decisión fue notificada a dirección y pronto se le asignará un nuevo encargado";  
                }break;
            case 6:
                {
                   mensajeAMostrar =  "El encargado ha rechazado esta asignación. Debe esperar a que la dirección le asigne un nuevo encargado";
                }break;
        
        }

        if (!(mensajeAMostrar.Equals(""))) {
            lblEstadoAsignacionVistaBecario.Text = mensajeAMostrar;
            lblEstadoAsignacionVistaBecario.Visible = true;
        }


    }

    protected void llenarInfoVistaBecario()
    {

        this.lblAnioVistaBecario.Text = añoActual.ToString();
        this.lblCicloVistaBecario.Text = convertirANumeroRomano(periodoActual);

        if (datosDeAsignacionDeBecario.Count != 0)
        {

            this.lblEncargadoVistaBecario.Text = datosDeAsignacionDeBecario[0][0].ToString() + " " + datosDeAsignacionDeBecario[0][1].ToString() + " " + datosDeAsignacionDeBecario[0][2].ToString();
            this.lblHorasVistaBecario.Text = datosDeAsignacionDeBecario[0][4].ToString();
           
            int estadoAsignacion = Convert.ToInt32(datosDeAsignacionDeBecario[0][3]);

            if (estadoAsignacion != 2 && estadoAsignacion != 3)
            {
                esconderBotonesVistaBecario(true);
            }

            actualizarEstadoAsignacionVistaBecario(estadoAsignacion);
        }
        else {

            this.lblEncargadoVistaBecario.Visible=false;
            this.lblHorasVistaBecario.Visible = false;
            this.lblTituloEncargadoVistaBecario.Visible = false;
            this.lblTituloHorasVistaBecario.Visible = false;

            lblEstadoAsignacionVistaBecario.Text = "Usted aún no tiene ningún encargado asignado.";
            lblEstadoAsignacionVistaBecario.Visible = true;
            esconderBotonesVistaBecario(true);
        }

    }


   // Aceptar asignación
   protected void btnAceptarAsignacionBecario_Click(object sender, EventArgs e)
   {

       int estadoActual = Convert.ToInt32(datosDeAsignacionDeBecario[0][3]);
       int nuevoEstado;
       if (estadoActual == 2)  // si ninguno había confirmado 
       {
           nuevoEstado = 4;    // pasa a estado de " esperando confirmación encargado "
       }
       else // si ya el encargado habia aceptado y solo faltaba el becario
       {
           nuevoEstado = 1; //pasa a estado aceptado
       }

       string mensajeResultado = controladoraAsignaciones.actualizarEstadoDeAsignacion(nuevoEstado, datosDeAsignacionDeBecario[0][5].ToString(), datosDeAsignacionDeBecario[0][6].ToString() , periodoActual, añoActual);

       if (mensajeResultado.Equals("Exito"))
       {
           if (nuevoEstado == 1)
           {
               commonService.mensajeJavascript("Usted ha aceptado la asignación satisfactoriamente y su encargado ha sido notficado. La asignación ha quedado completa por lo que ya puede empezar a realizar sus horas.", "Aviso");
             
             //CORREO A ENCARGADO !!
             string correoEncargado = datosDeAsignacionDeBecario[0][7].ToString();
             string nombreBecario = Session["Nombre"].ToString() + " " + Session["Apellido1"].ToString();
             string mensaje = "Se le informa que el estudiante " + nombreBecario + " acaba de aceptar una asignación con usted para el presente semestre. A partir de este momento el estudiante queda bajo su tutela para el control de las horas beca.";
             servicioCorreo.enviarCorreo(correoEncargado, "Mensaje del Sistema de Control de Becarios 11", mensaje);
  
           }
           else {
               commonService.mensajeJavascript("Usted ha aceptado la asignación satisfactoriamente. Sin embargo la asignación no ha quedado completada ya que debe esperar la confirmación del encargado", "Aviso");
           }

       }
       else
       {
           commonService.mensajeJavascript("No se pudo completar la operación. Debe intentarlo de nuevo", "Error");
       }

       actualizarEstadoAsignacionVistaBecario(nuevoEstado);
       esconderBotonesVistaBecario(true);
   }

   // Abrir confirmación de rechazo de asignación
   protected void btnCancelarAsignacionBecario_Click(object sender, EventArgs e)
   {
       commonService.abrirPopUp("PopUpConfirmarRechazoBecario", "Rechazar Asignación");
   }


   // Click de rechazar asignación para el becario
   protected void btnInvisibleConfirmarRechazo_Click(object sender, EventArgs e)
   {
       commonService.cerrarPopUp("PopUpConfirmarRechazoBecario");

       int nuevoEstado = 5; //rechazado por becario

       string mensajeResultado = controladoraAsignaciones.actualizarEstadoDeAsignacion(nuevoEstado, datosDeAsignacionDeBecario[0][5].ToString(), datosDeAsignacionDeBecario[0][6].ToString(), periodoActual, añoActual);

       if (mensajeResultado.Equals("Exito"))
       {
          commonService.mensajeJavascript("¡Su rechazo ha sido procesado satisfactoriamente!", "Rechazo procesado");
       }
       else
       {
           commonService.mensajeJavascript("No se pudo completar la operación. Debe intentarlo de nuevo", "Error");
       }

       actualizarEstadoAsignacionVistaBecario(nuevoEstado);
       esconderBotonesVistaBecario(true);

   }

   protected void esconderBotonesVistaBecario(Boolean esconder)
   {
       if (esconder)
       {
           this.btnAceptarAsignacionBecario.Visible = false;
           this.btnCancelarAsignacionBecario.Visible = false;
       }
       else
       {
           this.btnAceptarAsignacionBecario.Visible = true;
           this.btnCancelarAsignacionBecario.Visible = true;
       }
   }




   /***************************************************************************
    * 
    *                       VISTA ENCARGADO
    * 
    * **************************************************************************/


     /*
     * ------------------------------
     *   CLICKS - BOTONES
     * ------------------------------
     */


    // Aceptar Asignación - Vista Encargado
    protected void btnInvisibleAceptarAsignacionEncargado_Click(object sender, EventArgs e)
    {

        string usuario = Session["Cuenta"].ToString();
        string cedEncargado = controladoraAsignaciones.obtieneCedulaDeUsuario(usuario);
        string cedBecario = lstBecariosAsignadosEncargado[rowIndex][8].ToString();

        int estadoActual = Convert.ToInt32(lstBecariosAsignadosEncargado[rowIndex][6]);
        int nuevoEstado;
        if(estadoActual==2){ // si ninguno habia confirmado 
          nuevoEstado=3;     // la asignación queda en estado "pendiente de confirmacíon becario"
        }else{
          nuevoEstado=1;     //de lo contrario queda aceptada
        }

        string mensajeResultado = controladoraAsignaciones.actualizarEstadoDeAsignacion(nuevoEstado,cedBecario,cedEncargado,periodoActual,añoActual);
       
        if(mensajeResultado.Equals("Exito") ){

            if (nuevoEstado == 3)
            {
              commonService.mensajeJavascript("Usted ha aceptado la asignación.Sin embargo aún hay que esperar la confirmación del becario.", "Aviso");
            }
            else {

                //CORREO AL BECARIO  !!!
              commonService.mensajeJavascript("Se ha aceptado la asignación y esta ya quedó en firme. El becario ha sido notificado", "Aviso");
              string correoBecario = lstBecariosAsignadosEncargado[rowIndex][4].ToString();
              string mensaje = "Se le informa que se acaba de confimar su asignación para el presente semetre. Ya puede empezar a reportar sus horas con la aplicación.";
              servicioCorreo.enviarCorreo(correoBecario, "Mensaje del Sistema de Control de Becarios 11", mensaje);
            }
          
        }else{
           commonService.mensajeJavascript("La asignación no ha quedado aceptada porque se produjo error. Debe intentarlo de nuevo", "Error");
        }

        commonService.cerrarPopUp("PopUpAsignacionEncargado");

        llenarListaBecariosAsignados();
        llenarGridaBecariosAsignadosVistaEncargado(lstBecariosAsignadosEncargado);
        
    }

    // Rechazar Asignación - Vista Encargado
    protected void btnInvisibleRechazarAsignacionEncargado_Click(object sender, EventArgs e)
    {

        string usuario = Session["Cuenta"].ToString();
        string cedEncargado = controladoraAsignaciones.obtieneCedulaDeUsuario(usuario);
        string cedBecario = lstBecariosAsignadosEncargado[rowIndex][8].ToString();

        int estadoActual = Convert.ToInt32(lstBecariosAsignadosEncargado[rowIndex][6]);
        int nuevoEstado=6;
      
    
        string mensajeResultado = controladoraAsignaciones.actualizarEstadoDeAsignacion(nuevoEstado, cedBecario, cedEncargado, periodoActual, añoActual);

        if (mensajeResultado.Equals("Exito"))
        {
           commonService.mensajeJavascript("Se ha rechazado la asignación y su decisión ha sido notifcada a la dirección.", "Aviso");

           //CORREO A DIRECCIÓN !!!

           string correo = "hugo.villalta@ucr.ac.cr";
           string nombreEncargado = Session["Nombre"].ToString() + " " + Session["Apellido1"].ToString() ;
           string mensaje = "Se le informa que el encargado " + nombreEncargado + " acaba de rechazar una asignación. Dirígase al menú de asignaciones para cambiar esta asignación.";
           servicioCorreo.enviarCorreo(correo, "Mensaje del Sistema de Control de Becarios 11", mensaje); 
        }
        else
        {
            commonService.mensajeJavascript("La asignación no fue rechazada porque se produjo error. Debe intentarlo de nuevo", "Error");
        }

        commonService.cerrarPopUp("PopUpAsignacionEncargado");

        llenarListaBecariosAsignados();
        llenarGridaBecariosAsignadosVistaEncargado(lstBecariosAsignadosEncargado);

        commonService.cerrarPopUp("PopUpAsignacionEncargado");
       
    }


    // Botón de buscar
    protected void btnBuscarVistaEncargado_Click(object sender, EventArgs e)
    {

        TextInfo miTexto = CultureInfo.CurrentCulture.TextInfo;
        string criterioDeBusqueda = miTexto.ToTitleCase(this.txtBuscarVistaEncargado.Text);

        if (!(txtBuscarVistaEncargado.Text.Equals("")))
        {

            List<Object[]> listaAux = new List<Object[]>();
            if (lstBecariosAsignadosEncargado.Count > 0)
            {
                for (int i = 0; i < lstBecariosAsignadosEncargado.Count; ++i)
                {

                    string nombre = lstBecariosAsignadosEncargado[i][0].ToString();
                    string apellido1 = lstBecariosAsignadosEncargado[i][1].ToString();
                    string apellido2 = lstBecariosAsignadosEncargado[i][2].ToString();
                    string carne = lstBecariosAsignadosEncargado[i][3].ToString();
                    if ((nombre.Equals(criterioDeBusqueda)) || (apellido1.Equals(criterioDeBusqueda)) || (apellido2.Equals(criterioDeBusqueda)) || (carne.Equals(criterioDeBusqueda)))
                    {
                        listaAux.Add(lstBecariosAsignadosEncargado[i]);
                    }
                }
            }

            if (listaAux.Count != 0)
            {
                llenarGridaBecariosAsignadosVistaEncargado(listaAux);
            }
        }
        else {
            
            llenarGridaBecariosAsignadosVistaEncargado(lstBecariosAsignadosEncargado);
        }
    }



    /*
    * ------------------------------
    *   VARIOS
    * ------------------------------
    */


    protected void llenarCicloYAnioVistaEncargados()
    {
        this.lblCicloPrincipalVistaEncargado.Text = convertirANumeroRomano(periodoActual);
        this.lblAnioPrincipalVistaEncargado.Text = añoActual.ToString();
    }



    protected void llenarListaBecariosAsignados()
    {

        string usuario = Session["Cuenta"].ToString();
        string cedEncargado = controladoraAsignaciones.obtieneCedulaDeUsuario(usuario);

        lstBecariosAsignadosEncargado = controladoraAsignaciones.consultarBecariosAsignadosAEncargado(cedEncargado, añoActual, periodoActual);

    }


    // Llenar tabla con todas las asignaciones del encargado logueado actualmente
    protected void llenarGridaBecariosAsignadosVistaEncargado(List<Object[]> listaDatos)
    {


        DataTable tablaBecariosAsigandosAEncargado = crearTablaBecariosAsignadosVistaEncargado();
        DataRow newRow;

        if (listaDatos.Count > 0)
        {
            for (int i = 0; i < listaDatos.Count; ++i)
            {

                newRow = tablaBecariosAsigandosAEncargado.NewRow();
                newRow["Nombre"] = listaDatos[i][0].ToString() + " " + listaDatos[i][1].ToString() + " " + listaDatos[i][2].ToString();
                newRow["Carné"] = listaDatos[i][3].ToString();
                newRow["Correo"] = listaDatos[i][4].ToString();
                newRow["Celular"] = listaDatos[i][5].ToString();
                newRow["Estado"] = interpretaEstado(Convert.ToInt32(listaDatos[i][6]),true);
                tablaBecariosAsigandosAEncargado.Rows.InsertAt(newRow, i);
            }
        }
        else
        {

            newRow = tablaBecariosAsigandosAEncargado.NewRow();
            newRow["Nombre"] = "-";
            newRow["Carné"] = "-";
            newRow["Correo"] = "-";
            newRow["Celular"] = "-";
            newRow["Estado"] = "-";
            tablaBecariosAsigandosAEncargado.Rows.InsertAt(newRow, 0);

        }

       
        gridBecariosAsignadosVistaEncargado.DataSource = tablaBecariosAsigandosAEncargado;
        gridBecariosAsignadosVistaEncargado.DataBind();
        this.headersCorrectosBecariosAsignadosVistaEncargado();
    }

   


    // Le da formato a las columnas del Grid
    protected DataTable crearTablaBecariosAsignadosVistaEncargado()
    {

        DataTable dt = new DataTable();
        DataColumn column;

        column = new DataColumn();
        column.DataType = System.Type.GetType("System.String");
        column.ColumnName = "Nombre";
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
        column.ColumnName = "Celular";
        dt.Columns.Add(column);

        column = new DataColumn();
        column.DataType = System.Type.GetType("System.String");
        column.ColumnName = "Estado";
        dt.Columns.Add(column);

        return dt;
    }


    // Aplica nombre a las columnas así como color
    private void headersCorrectosBecariosAsignadosVistaEncargado()
    {
        gridBecariosAsignadosVistaEncargado.HeaderRow.BackColor = System.Drawing.Color.FromArgb(4562432);
        gridBecariosAsignadosVistaEncargado.HeaderRow.ForeColor = System.Drawing.Color.White;
        gridBecariosAsignadosVistaEncargado.HeaderRow.Cells[1].Text = "Nombre";
        gridBecariosAsignadosVistaEncargado.HeaderRow.Cells[2].Text = "Carné";
        gridBecariosAsignadosVistaEncargado.HeaderRow.Cells[3].Text = "Correo";
        gridBecariosAsignadosVistaEncargado.HeaderRow.Cells[4].Text = "Celular";
        gridBecariosAsignadosVistaEncargado.HeaderRow.Cells[5].Text = "Estado";
    }



    protected void GridBecariosAsignadosVistaEncargado_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        switch (e.CommandName)
        {
            // Abrir Pop Up aceptar/rechazar asignación vista encargado
            case "btnSeleccionarTupla_Click":
                {


                    rowIndex = Convert.ToInt32(e.CommandArgument);
                    int pageIndex = this.GridAsignaciones.PageIndex;
                    int pageSize = this.GridAsignaciones.PageSize;
                    rowIndex += (pageIndex * pageSize);

                    if(   (Convert.ToInt32(lstBecariosAsignadosEncargado[rowIndex][6])==2 ) || (Convert.ToInt32(lstBecariosAsignadosEncargado[rowIndex][6])==4 ) ){  

                        commonService.abrirPopUp("PopUpAsignacionEncargado", "Aceptar/Rechazar Asignación");
                        string nombreBecario =  lstBecariosAsignadosEncargado[rowIndex][0].ToString() + " " + lstBecariosAsignadosEncargado[rowIndex][1].ToString() + " " + lstBecariosAsignadosEncargado[rowIndex][2].ToString();
                        this.lblNombreBecarioPopUpVistaEncargado.Text = nombreBecario;
                        this.lblCicloBecarioPopUpVistaEncargado.Text = convertirANumeroRomano(periodoActual);
                        this.lblAnioBecarioPopUpVistaEncargado.Text = añoActual.ToString();
                        this.lblHorasBecarioPopUpVistaEncargado.Text = lstBecariosAsignadosEncargado[rowIndex][7].ToString();
                    }else{
                      commonService.mensajeJavascript("Esta asignación no está pendiente","Aviso");
                    }
                    
                } break;
        }
    }



}





