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



    public String ejecutar(int accion, Object[] datos, String otrosDatos) //1 insertar 2 eliminar 3 insertarComentarioDireccion (a una asignacion existente)
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


    public String getNombreBecario(String ced) {

        String nombre = "";

        Becario becario = controladoraBecario.obtenerBecarioPorCedula(ced);
        nombre = becario.nombre + " " + becario.apellido1 + " " + becario.apellido2;
        return nombre;
    
    }



    public String getNombreEncargado(String ced)
    {

        String nombre = "";

        Encargado encargado = controladoraEncargado.obtenerEncargadoPorCedula(ced);
        nombre = encargado.Nombre + " "+ encargado.Apellido1 + " "+ encargado.Apellido2;
        return nombre;
    }


    public String obtieneCedulaDeUsuario(String usuario)
    {
      return controladoraCuentas.getCedulaByUsuario(usuario);
    }


    public AsignacionesDataSet.BecarioSinAsignacionDataTable consultaBecariosSinAsignacion(int periodo, int año)
    {
        return controladoraBDAsignaciones.consultarBecariosSinAsignacion(periodo, año);
   }


    public EncargadoDataSet.EncargadoDataTable obtenerEncargadosCompletos() {
        return controladoraEncargado.obtenerEncargadosCompletos();
    }


    public int contarBecariosAsignados(string ced, int año, int perido )
    { 
      return controladoraBDAsignaciones.contarBecariosAsignados(ced,año,perido);
    }


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



    public List<Object[]> consultarAsignacionDeBecario(string cedBecario, int año, int perido)
    {


        List<Object[]> retorno = new List<Object[]>();

        AsignacionesDataSet.EncargadoDeBecarioDataTable tabla = controladoraBDAsignaciones.buscarEncargadoDeBecario(cedBecario, año, perido);

        if (tabla.Rows.Count != 0)
        {

            Object[] objeto = new Object[7];
            objeto[0] = cs.procesarStringDeUI(tabla.Rows[0]["Nombre"].ToString());
            objeto[1] = cs.procesarStringDeUI(tabla.Rows[0]["Apellido1"].ToString());
            objeto[2] = cs.procesarStringDeUI(tabla.Rows[0]["Apellido2"].ToString());
            objeto[3] = tabla.Rows[0]["Estado"].ToString();
            objeto[4] = tabla.Rows[0]["TotalHoras"].ToString();
            objeto[5] = tabla.Rows[0]["CedulaBecario"].ToString();
            objeto[6] = tabla.Rows[0]["CedulaEncargado"].ToString();
            retorno.Add(objeto);
        }

         return retorno;
    }



    public void dejarAsignacionInactiva(string cedBecario, string cedEncargado, int año, int periodo)
    {
        controladoraBDAsignaciones.dejarAsignacionInactiva(cedBecario, cedEncargado, año, periodo);
    }



    public String actualizarEstadoDeAsignacion(int nuevoEstado, String cedBecario, String cedEncargado, int periodo, int año)
    { 
      return controladoraBDAsignaciones.actualizarEstadoDeAsignacion(nuevoEstado,cedBecario,cedEncargado,periodo,año);
    }

    

}