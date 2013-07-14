using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for ControladoraAsignaciones
/// </summary>
public class ControladoraAsignaciones
{

    private ControladoraBecarios controladoraBecario;
    private ControladoraEncargado controladoraEncargado;
    private ControladoraCuentas controladoraCuentas;
         
    private ControladoraBDAsignaciones controladoraBDAsignaciones;
    private CommonServices cs;

	public ControladoraAsignaciones()
	{
        cs = new CommonServices(null);
        controladoraBecario = new ControladoraBecarios();
        controladoraEncargado = new ControladoraEncargado();
        controladoraCuentas = new ControladoraCuentas();

        controladoraBDAsignaciones = new ControladoraBDAsignaciones();
	}


    /* Requiere: Un arreglo con los datos completos de una asignación.
    * 
    *  Efectúa: Controla la inserción y eliminación de asignaciones, y la inserción de comentarios de la dirección .
    *           Crea una nueva asignación con los datos recibidos por parámetro y luego le pide a la controladora de la BD realizar la operación correspondiente.
    *           Devuelve una hilera de caracteres que indica si la operación realizada tuvo éxito.
    * 
    *  Modifica: n/a.
    */
    public String ejecutar(int accion, Object[] datos, String otrosDatos) //modo --> 1:insertar, 2:eliminar, 3:insertarComentarioDireccion (a una asignacion existente)
    {
        
        string mensajeResultado = "-1";
        
        
        switch (accion)
        {
            case 1: //Insertar 
                {
                    Asignacion asignacionNueva = new Asignacion(datos);
                    mensajeResultado = controladoraBDAsignaciones.insertarAsignacion(asignacionNueva);
                } break;
            case 2: //Eliminar
                {
                    Asignacion asignacion = new Asignacion(datos);
                    mensajeResultado = controladoraBDAsignaciones.eliminarAsignacion(asignacion);
                } break;
            case 3: //insertarComentarioDireccion (a una asignacion existente)
               {
                   Asignacion asignacion = new Asignacion(datos);
                   mensajeResultado = controladoraBDAsignaciones.insertarComentarioDireccion(otrosDatos, asignacion);
               }break;
        }

        return mensajeResultado;
    }




    /* Requiere: n/a.
    * 
    *  Efectúa: Consulta todas las asignaciones existentes y devuelve una lista con estas.
    *           Se llena una tabla con el resultado de la consulta devuelta por la controladora de la BD y se llena una lista con las tuplas del resultado.
    * 
    *  Modifica: Crea una nueva lista de asignaciones, la llena y la retorna.
    */
    public List<Asignacion> consultarTablaAsignacionesCompleta()
    {
        
        List<Asignacion> listaAs = new List<Asignacion>();

        AsignacionesDataSet.AsignadoADataTable tabla = controladoraBDAsignaciones.consultarAsignaciones();

        foreach (DataRow r in tabla.Rows)
        {

            Asignacion asignacion = new Asignacion();

            asignacion.CedulaBecario = cs.procesarStringDeUI(r["CedulaBecario"].ToString());
            asignacion.CedulaEncargado = cs.procesarStringDeUI(r["CedulaEncargado"].ToString());
            asignacion.Periodo = Convert.ToInt32(r["Periodo"]);
            asignacion.Año = Convert.ToInt32(r["Año"]);
            asignacion.TotalHoras = Convert.ToInt32(r["TotalHoras"]);
            asignacion.SiglasUA = cs.procesarStringDeUI(r["SiglasUA"].ToString());
            asignacion.InfoUbicacion = cs.procesarStringDeUI(r["InfoUbicacion"].ToString());
            asignacion.Estado = Convert.ToInt32(r["Estado"]);
            asignacion.Activo = Convert.ToBoolean(r["Activo"]);
            asignacion.ComentarioBecario = cs.procesarStringDeUI(r["ComentarioDeBecario"].ToString());
            asignacion.ComentarioEncargado = cs.procesarStringDeUI(r["ComentarioDeEncargado"].ToString());
            asignacion.ComentarioDireccion = cs.procesarStringDeUI(r["ComentarioDeDireccion"].ToString());

            listaAs.Add(asignacion);
        }
         
 
        return listaAs;
    }




    /* Requiere: Una cédula válida de un becario que exista en la base datos.
    * 
    *  Efectúa: Pide a la controladora de becarios el nombre del becario con cédula "ced".
    *           
    *  Modifica: n/a.  
    */
    public String getNombreBecario(String ced) {

        String nombre = "";

        Becario becario = controladoraBecario.obtenerBecarioPorCedula(ced);
        nombre = becario.nombre + " " + becario.apellido1 + " " + becario.apellido2;
        return nombre;
    
    }



    /* Requiere: Una cédula válida de un encargado que exista en la base datos.
    * 
    *  Efectúa: Pide a la controladora de encargado el nombre del encargado con cédula "ced".
    *           
    *  Modifica: n/a.  
    */
    public String getNombreEncargado(String ced)
    {

        String nombre = "";

        Encargado encargado = controladoraEncargado.obtenerEncargadoPorCedula(ced);
        nombre = encargado.Nombre + " "+ encargado.Apellido1 + " "+ encargado.Apellido2;
        return nombre;
    }



    /* Requiere: Una nombre de usuario válido.
    * 
    *  Efectúa: Pide a la controladora de cuentas por la cédula del usuario con nombre de usuario "usuario".
    *           
    *  Modifica: n/a.  
    */
    public String obtieneCedulaDeUsuario(String usuario)
    {
      return controladoraCuentas.getCedulaByUsuario(usuario);
    }



    /* Requiere: Una periodo y año válidos.
    * 
    *  Efectúa: Pide a la controladora de BD buscar los becarios que no tienen una asignación en el periodo y año 
    *          indicado por el valor de los parámetros.
    *           
    *  Modifica: n/a.  
    */
    public AsignacionesDataSet.BecarioSinAsignacionDataTable consultaBecariosSinAsignacion(int periodo, int año)
    {
        return controladoraBDAsignaciones.consultarBecariosSinAsignacion(periodo, año);
   }



    /* Requiere: Una periodo y año válidos. 
    *  Efectúa: Pide a la controladora de encargados la lista completa de encargados.          
    *  Modifica: n/a.  
    */
    public EncargadoDataSet.EncargadoDataTable obtenerEncargadosCompletos() {
        return controladoraEncargado.obtenerEncargadosCompletos();
    }


    /* Requiere: Una periodo y año válidos.
    * 
    *  Efectúa: Pide a la controladora de BD contar cuantos becarios tiene asignados el encargado con cédula "ced" 
    *           en el periodo y año indicado por el valor de los parámetros.
    *           
    *  Modifica: n/a.  
    */
    public int contarBecariosAsignados(string ced, int año, int perido )
    { 
      return controladoraBDAsignaciones.contarBecariosAsignados(ced,año,perido);
    }



    /* Requiere: Una periodo y año válidos.
    * 
    *  Efectúa: Pide a la controladora de BD los becarios que tiene asignados el encargado con cédula "ced" 
    *           en el periodo y año indicado por el valor de los parámetros.
    *           
    *  Modifica: Se crea y devuelve una lista de las asignaciones devueltas por la controladora de la BD.  
    */
    public List<Object[]> consultarBecariosAsignadosAEncargado(string cedEncargado, int año, int perido)
    {


        List<Object[]> listaB =  new List<Object[]>();
              
        AsignacionesDataSet.BecariosAsignadosAEncargadoDataTable tabla = controladoraBDAsignaciones.consultarBecariosAsignadosAEncargado(cedEncargado, año, perido);

        foreach (DataRow r in tabla.Rows)
        {


            Object[] objeto = new Object[9];
            objeto[0] = cs.procesarStringDeUI(r["Nombre"].ToString());
            objeto[1] = cs.procesarStringDeUI(r["Apellido1"].ToString());
            objeto[2] = cs.procesarStringDeUI(r["Apellido2"].ToString());
            objeto[3] = r["Carne"].ToString();
            objeto[4] = cs.procesarStringDeUI(r["Correo"].ToString());
            objeto[5] = r["Celular"].ToString();
            objeto[6] = r["Estado"].ToString();
            objeto[7] = r["TotalHoras"].ToString();
            objeto[8] = r["CedulaBecario"].ToString();
            listaB.Add(objeto);

        }

        return listaB;
    }




    /* Requiere: Una cédula, periodo y año válidos.
    * 
    *  Efectúa: Solicita a la controladora de BD los datos de la asignación del becario con cédula "cedBecario" 
    *           en el periodo y año indicado por el valor de los parámetros.
    *           
    *  Modifica: Se crea y devuelve una lista con la asignación devuelta por la controladora de la BD.  
    */
    public List<Object[]> consultarAsignacionDeBecario(string cedBecario, int año, int perido)
    {


        List<Object[]> retorno = new List<Object[]>();

        AsignacionesDataSet.EncargadoDeBecarioDataTable tabla = controladoraBDAsignaciones.buscarEncargadoDeBecario(cedBecario, año, perido);

        if (tabla.Rows.Count != 0)
        {

            Object[] objeto = new Object[8];
            objeto[0] = cs.procesarStringDeUI(tabla.Rows[0]["Nombre"].ToString());
            objeto[1] = cs.procesarStringDeUI(tabla.Rows[0]["Apellido1"].ToString());
            objeto[2] = cs.procesarStringDeUI(tabla.Rows[0]["Apellido2"].ToString());
            objeto[3] = tabla.Rows[0]["Estado"].ToString();
            objeto[4] = tabla.Rows[0]["TotalHoras"].ToString();
            objeto[5] = tabla.Rows[0]["CedulaBecario"].ToString();
            objeto[6] = tabla.Rows[0]["CedulaEncargado"].ToString();
            objeto[7] = tabla.Rows[0]["Correo"].ToString();
            retorno.Add(objeto);
        }

         return retorno;
    }




    /* Requiere: Cédulas válidas , igual para el periodo y año.
    * 
    *  Efectúa: Solicita a la controladora de BD dejar una asignación inactiva.
    *           La asignación que se debe dejar inactiva es la que tiene el becario con cédula "cedBecario" y el encargado 
     *          con cédula "cedEncargado" en el periodo "periodo" y año "año".
    *           
    *  Modifica: n/a.  
    */
    public void dejarAsignacionInactiva(string cedBecario, string cedEncargado, int año, int periodo)
    {
        controladoraBDAsignaciones.dejarAsignacionInactiva(cedBecario, cedEncargado, año, periodo);
    }



    /* Requiere: Cédulas válidas , igual para el periodo y año.
    * 
    *  Efectúa: Solicita a la controladora de BD actualizar el estado de una asignación .
    *           La asignación que se debe cambiar de estado es la que tiene el becario con cédula "cedBecario" y el encargado 
    *           con cédula "cedEncargado" en el periodo "periodo" y año "año".
    *           
    *  Modifica: n/a.  
    */
    public String actualizarEstadoDeAsignacion(int nuevoEstado, String cedBecario, String cedEncargado, int periodo, int año)
    { 
      return controladoraBDAsignaciones.actualizarEstadoDeAsignacion(nuevoEstado,cedBecario,cedEncargado,periodo,año);
    }



    /* Requiere: Una cédula de becario válida.
    * 
    *  Efectúa: Solicta a la controladora de becarios por el correo de un becario.
    *           
    *  Modifica: n/a.  
    */
    public String buscarCorreoBecario(String cedulaBecario) {

        return controladoraBecario.obtenerCorreoBecario(cedulaBecario); 
    
    }



    /* Requiere: Una cédula de encargado válida.
    * 
    *  Efectúa: Solicta a la controladora de encargados por el correo de un encargado.
    *           
    *  Modifica: n/a.  
    */
    public String buscarCorreoEncargado(String cedulaEncargado)
    {

        return controladoraEncargado.obtenerCorreoEncargado(cedulaEncargado);
    }



    /* Requiere: n/a.
    * 
    *  Efectúa: Solicta a la controladora de BD eliminar la asignación de determinado becario en un periodo y año determinado.
    *           
    *  Modifica: n/a.  
    */
    public String eliminaAsignacionDeBecario(string cedBecario, int ped, int año){

      return controladoraBDAsignaciones.eliminaAsignacionDeBecario(cedBecario, ped, año);

    }



    /* Requiere: n/a.
    * 
    *  Efectúa: Solicta a la controladora de BD eliminar las asignaciones de determinado encargado en un periodo y año determinado.
    *           
    *  Modifica: n/a.  
    */
    public String eliminaAsignacionesDeEncargado(string cedEncargado, int ped, int año)
    {
       return controladoraBDAsignaciones.eliminaAsignacionesDeEncargado(cedEncargado, ped, año);
    }


}