using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using AsignacionesDataSetTableAdapters;
using System.Data;

/// <summary>
/// Summary description for ControladoraBDAsignaciones
/// </summary>
public class ControladoraBDAsignaciones
{

    AsignadoATableAdapter adapterAsignaciones;
    BecarioSinAsignacionTableAdapter adapterBecarioSinAsignacion;
    BecariosAsignadosAEncargadoTableAdapter adapterBecariosAsignadosEncargado;
    EncargadoDeBecarioTableAdapter adapterEncargadoDeBecario;

	public ControladoraBDAsignaciones()
	{
        adapterAsignaciones = new AsignadoATableAdapter();
        adapterBecarioSinAsignacion = new BecarioSinAsignacionTableAdapter();
        adapterBecariosAsignadosEncargado = new BecariosAsignadosAEncargadoTableAdapter();
        adapterEncargadoDeBecario = new EncargadoDeBecarioTableAdapter();
	}



    public String insertarAsignacion( Asignacion asignacion){

        String returnValue = "Exito";
        int r;
        try
        {
            this.adapterAsignaciones.Insert(asignacion.CedulaBecario, asignacion.Periodo, asignacion.Año, asignacion.CedulaEncargado, asignacion.TotalHoras, asignacion.SiglasUA, asignacion.InfoUbicacion, asignacion.Estado,asignacion.Activo,asignacion.ComentarioBecario,asignacion.ComentarioEncargado,asignacion.ComentarioDireccion);
        }
        catch (SqlException e)
        {
            r = e.Number;
            if (r == 2627)
            {
                returnValue = "Error1"; //"Ya existe asignación";
            }
            else
            {
                returnValue = "Error2";
            }
        }
        return returnValue;
    }



    public String eliminarAsignacion(Asignacion asig)
    {
        string returnValue = "Exito";

        try
        {
            this.adapterAsignaciones.Delete(asig.CedulaBecario, asig.Periodo, asig.Año, asig.CedulaEncargado, asig.TotalHoras, asig.SiglasUA, asig.InfoUbicacion, asig.Estado, asig.Activo,asig.ComentarioBecario,asig.ComentarioEncargado, asig.ComentarioDireccion);
        }
        catch (SqlException e)
        {
            returnValue = "Error";

        }

        return returnValue;
    }



    public String actualizarEstadoDeAsignacion(int nuevoEstado, String cedBecario, String cedEncargado, int periodo, int año)
    {
        string returnValue = "Exito";

        try
        {
            this.adapterAsignaciones.actualizarEstado(nuevoEstado, cedBecario, periodo, año, cedEncargado);
        }
        catch (SqlException e)
        {
            returnValue = "Error";

        }

        return returnValue;
    }



    public String insertarComentarioDireccion(String comentario, Asignacion asignacion)
    {
        string returnValue = "Exito";

        try
        {
          this.adapterAsignaciones.insertarComentarioDireccion(comentario, asignacion.CedulaBecario, asignacion.Periodo, asignacion.Año, asignacion.CedulaEncargado, asignacion.TotalHoras, 0, asignacion.SiglasUA, 0, asignacion.InfoUbicacion, 0, asignacion.Estado, asignacion.Activo, 0, asignacion.ComentarioBecario, 0, asignacion.ComentarioEncargado, 0, asignacion.ComentarioDireccion);
        }
        catch (SqlException e)
        {
            returnValue = "Error";

        }

        return returnValue;
    }


    public AsignacionesDataSet.AsignadoADataTable consultarAsignaciones()
    {

        AsignacionesDataSet.AsignadoADataTable dt = new AsignacionesDataSet.AsignadoADataTable();
        this.adapterAsignaciones.Fill(dt);
        return dt;
    }


    public AsignacionesDataSet.BecarioSinAsignacionDataTable consultarBecariosSinAsignacion(int periodo, int año)
    {


        AsignacionesDataSet.BecarioSinAsignacionDataTable dt = new AsignacionesDataSet.BecarioSinAsignacionDataTable();

        this.adapterBecarioSinAsignacion.Fill(dt, periodo, año);

        return dt;

    }


    public int contarBecariosAsignados(string ced, int año, int perido)
    {

       int i = Convert.ToInt32(this.adapterAsignaciones.contarBecariosAsignados(ced, año, perido) );
       return i;
    }


    public AsignacionesDataSet.BecariosAsignadosAEncargadoDataTable consultarBecariosAsignadosAEncargado(string cedEncargado, int año, int perido)
    {

        AsignacionesDataSet.BecariosAsignadosAEncargadoDataTable dt = new AsignacionesDataSet.BecariosAsignadosAEncargadoDataTable();
        adapterBecariosAsignadosEncargado.Fill(dt, cedEncargado, perido, año);

        return dt;
    }


    public void dejarAsignacionInactiva(string cedBecario, string cedEncargado, int año, int periodo)
    {

       adapterAsignaciones.asignacionActiva(false, cedBecario, periodo, año, cedEncargado);
    }


    public AsignacionesDataSet.EncargadoDeBecarioDataTable buscarEncargadoDeBecario(string cedBecario, int año, int periodo)
    {

        AsignacionesDataSet.EncargadoDeBecarioDataTable dt = new AsignacionesDataSet.EncargadoDeBecarioDataTable();
        adapterEncargadoDeBecario.Fill(dt, cedBecario, periodo, año);

        return dt;
    
    }


}