using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

/// <summary>
/// Summary description for ControladoraEncargado
/// </summary>
public class ControladoraEncargado
{
	/** Atributos de la clase **/
    private Encargado nuevoEncargado;
    private ControladoraBDEncargado controladoraBDEncargado;
    private ControladoraCuentas controladoraCuentas;
    private ControladoraAsignaciones contAsig;
    private CommonServices cs;

	/** EFECTO: Constructor de la clase. Inicializa la controladoraBDEncagado
	 ** REQUIERE: Nada
	 ** MODIFICA: Nueva clase ControladoraEncargado **/
	public ControladoraEncargado()
	{
        controladoraBDEncargado = new ControladoraBDEncargado();
        controladoraCuentas = new ControladoraCuentas();
        cs = new CommonServices(null);
	}

	/** EFECTO: LLama a la controladosra BD de Encargado para Insertar, Modificar, o Eliminar tuplas de la tabla ENCARGADO en la BD 
	 ** REQUIERE: Que se indique en "accion" la operación a realizar. "datos" debe contener la información del Encargado al que se le va a apliacaar la operación
	 ** MODIFICA: Nada **/
    public string ejecutar(int accion, Object[] datos, Object[] datosOriginales)
    {
        
        //bool resultado = true;
        string mensajeResultado="Exito";

        switch (accion)
        {
            case 1://INSERTAR:
                {
                    nuevoEncargado = new Encargado(datos);
                    mensajeResultado = controladoraBDEncargado.insertarEncargado(nuevoEncargado);
                }
                break;
            case 2://ELIMINAR (este caso esta contemplado en el método )
                {

                    Encargado encargado = new Encargado(datosOriginales);
                    int año = cs.getAñoActual();
                    int periodo = cs.getPeriodoActual();
                    contAsig = new ControladoraAsignaciones();
                    int becariosAsignados = contAsig.contarBecariosAsignados(encargado.Cedula, año, periodo);

                    if (becariosAsignados == 0)
                    {
                        mensajeResultado = controladoraBDEncargado.eliminarEncargado(encargado);
                    }
                    else {
                        mensajeResultado = "ErrorAsignacion";
                    }
                }
                break;
            case 3://MODIFICAR
                {
                    nuevoEncargado = new Encargado(datos);
                    Encargado encargadoOriginal = new Encargado(datosOriginales);
                    mensajeResultado = controladoraBDEncargado.modificaEncargado(nuevoEncargado, encargadoOriginal);
                }
                break;
        }

        return mensajeResultado;
    }

	/** EFECTO: Consulta la controladora BD de Encargado y devuelve un listado de todas la tuplas en la tabla ENCARGADO en la BD
	 ** REQUIERE: Nada
	 ** MODIFICA: Nada **/
    public List<Encargado> consultarTablaEncargados()
    {
        EncargadoDataSet.EncargadoDataTable tabla = controladoraBDEncargado.consultarEncargados();
        List<Encargado> lsEncargados = new List<Encargado>();

        foreach (DataRow r in tabla.Rows) {

            Encargado encargado = new Encargado();

            encargado.Nombre = r["Nombre"].ToString();
            encargado.Apellido1 = r["Apellido1"].ToString();
            encargado.Apellido2 = r["Apellido2"].ToString();
            encargado.Cedula = r["Cedula"].ToString();
            encargado.Puesto = r["Puesto"].ToString();
            encargado.Correo = r["Correo"].ToString();
            encargado.TelefonoFijo = r["Telefono"].ToString();
            encargado.TelefonoCelular = r["Celular"].ToString();
            encargado.OtroTelefono = r["OtroTel"].ToString();

            lsEncargados.Add(encargado);
        }
        return lsEncargados;
    }

    /** EFECTO: Consulta la controladora BD de Encargado y devuelve un listado de todas la tuplas en la tabla ENCARGADO en la BD que coinciden con los datos de busqueda
     ** REQUIERE: Nada
     ** MODIFICA: Nada **/
    public List<Encargado> ObtenerTablaEncargadosPorBusquedaSelectiva(string criterioDeBusqueda)
    {
        EncargadoDataSet.EncargadoDataTable tabla = controladoraBDEncargado.ObtenerTablaEncargadosPorBusquedaSelectiva(criterioDeBusqueda);
        List<Encargado> lsEncargados = new List<Encargado>();

        foreach (DataRow r in tabla.Rows)
        {

            Encargado encargado = new Encargado();

            encargado.Nombre = r["Nombre"].ToString();
            encargado.Apellido1 = r["Apellido1"].ToString();
            encargado.Apellido2 = r["Apellido2"].ToString();
            encargado.Cedula = r["Cedula"].ToString();
            encargado.Puesto = r["Puesto"].ToString();
            encargado.Correo = r["Correo"].ToString();
            encargado.TelefonoFijo = r["Telefono"].ToString();
            encargado.TelefonoCelular = r["Celular"].ToString();
            encargado.OtroTelefono = r["OtroTel"].ToString();

            lsEncargados.Add(encargado);
        }
        return lsEncargados;
    } 

    public List<Encargado> ObtenerDatosCuenta(String Usuario)
    {
        List<Encargado> lsEncargados = new List<Encargado>();
        
        String cedula = controladoraCuentas.getCedulaByUsuario(Usuario);
        Encargado encargado = obtenerEncargadoPorCedula(cedula);
        lsEncargados.Add(encargado);
       
        return lsEncargados;
    }

    public Encargado obtenerEncargadoPorCedula(String cedula)
    {
         EncargadoDataSet.EncargadoDataTable tabla = controladoraBDEncargado.obtenerEncargadoPorCedula(cedula);

         Encargado encargado = new Encargado();

         encargado.Nombre = tabla.Rows[0]["Nombre"].ToString();
         encargado.Apellido1 = tabla.Rows[0]["Apellido1"].ToString();
         encargado.Apellido2 = tabla.Rows[0]["Apellido2"].ToString();
         encargado.Cedula = tabla.Rows[0]["Cedula"].ToString();
         encargado.Puesto = tabla.Rows[0]["Puesto"].ToString();
         encargado.Correo = tabla.Rows[0]["Correo"].ToString();
         encargado.TelefonoFijo = tabla.Rows[0]["Telefono"].ToString();
         encargado.TelefonoCelular = tabla.Rows[0]["Celular"].ToString();
         encargado.OtroTelefono = tabla.Rows[0]["OtroTel"].ToString();
         return encargado;
    }


    public EncargadoDataSet.EncargadoDataTable obtenerEncargadosCompletos(){
        return controladoraBDEncargado.consultarEncargados();
    }


    public String obtenerCorreoEncargado(String cedulaEncargado)
    {
        return controladoraBDEncargado.obtenerCorreoEncargado(cedulaEncargado);
    }


    public String eliminaAsignacionesActuales(string cedEncargado, int ped, int año)
    {
        contAsig = new ControladoraAsignaciones();
        return contAsig.eliminaAsignacionesDeEncargado(cedEncargado, ped, año);
    }

    

    public Boolean tieneAsignaciones(string cedEncargado) {

        Boolean resultado;

        int año = cs.getAñoActual();
        int periodo = cs.getPeriodoActual();
        contAsig = new ControladoraAsignaciones();
        int cantidadAsignaciones = contAsig.contarBecariosAsignados(cedEncargado, año, periodo); ;

        if (cantidadAsignaciones == 0)
        {
            resultado = false;
        }
        else
        {
            resultado = true;
        }

        return resultado;
    
    }


}
