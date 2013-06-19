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

    private ControladoraBDAsignaciones controladoraBDAsignaciones;
    private CommonServices cs;

	public ControladoraAsignaciones()
	{
        cs = new CommonServices(null);
        controladoraBecario = new ControladoraBecarios();
        controladoraEncargado = new ControladoraEncargado();
        controladoraBDAsignaciones = new ControladoraBDAsignaciones();
	}



    public String ejecutar(int accion, Object[] datos, Object[] datosViejos) //1 insertar 2 modificar 3 eliminar
    {
        
        string mensajeResultado = "-1";
        
        
        /*switch (accion)
        {
            case 1: //Insertar 
                {
                    
                } break;
            case 2: //Modificar
                {
                   
                } break;
            case 3: //Eliminar
                {
                    
                } break;
        }*/

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
            asignacion.Periodo = cs.procesarStringDeUI(r["Periodo"].ToString());
            asignacion.Año = cs.procesarStringDeUI(r["Año"].ToString());
            asignacion.TotalHoras = cs.procesarStringDeUI(r["TotalHoras"].ToString());
            asignacion.SiglasUA = cs.procesarStringDeUI(r["SiglasUA"].ToString());
            asignacion.InfoUbicacion = cs.procesarStringDeUI(r["InfoUbicacion"].ToString());
            asignacion.Estado = cs.procesarStringDeUI(r["Estado"].ToString());

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



    public AsignacionesDataSet.AsignadoADataTable consultaBecariosSinAsignacion(int periodo, int año)
    {
        return controladoraBDAsignaciones.consultarBecariosSinAsignacion(periodo, año);
   }


}