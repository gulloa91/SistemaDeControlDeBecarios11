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

            /*asignacion.CedulaBecario = controladoraBDAsignaciones.

            becario.foto = cs.procesarStringDeUI(r["Foto"].ToString());
            becario.nombre = cs.procesarStringDeUI(r["Nombre"].ToString());
            becario.apellido1 = cs.procesarStringDeUI(r["Apellido1"].ToString());
            becario.apellido2 = cs.procesarStringDeUI(r["Apellido2"].ToString());
            becario.carne = cs.procesarStringDeUI(r["Carne"].ToString());
            becario.cedula = cs.procesarStringDeUI(r["Cedula"].ToString());
            becario.telefonoFijo = cs.procesarStringDeUI(r["Telefono"].ToString());
            becario.telefonoCelular = cs.procesarStringDeUI(r["Celular"].ToString());
            becario.telefonoOtro = cs.procesarStringDeUI(r["OtroTel"].ToString());
            becario.correo = cs.procesarStringDeUI(r["Correo"].ToString());
            listaB.Add(becario);*/
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

}