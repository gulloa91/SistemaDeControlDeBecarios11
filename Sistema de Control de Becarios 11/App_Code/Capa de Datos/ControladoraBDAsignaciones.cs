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




    /* Requiere: Un objeto tipo asignación debidamente creado y no nulo.
    * 
    *  Efectúa: Inserta una nueva asignación en la base de datos.
    * 
    *  Modifica: n/a.
    */
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



    /* Requiere: Un objeto tipo asignación debidamente creado y no nulo.
    * 
    *  Efectúa: Elimina una asignación de la base de datos.
    * 
    *  Modifica: n/a.
    */
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



    /* Requiere: Los datos "llave" de una asignación existente.
    * 
    *  Efectúa: Modifica el estado de una asignación existente.
    * 
    *  Modifica: n/a.
    */
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



    /* Requiere: Un objeto tipo asignación debidamente creado y no nulo, y el comentario a insertar.
    * 
    *  Efectúa: Inserta el comentario de la dirección de una asignación existente.
    * 
    *  Modifica: n/a.
    */
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





    /* Requiere: n/a.
    * 
    *  Efectúa: Consulta todas las asignaciones existente en la base de datos.
    * 
    *  Modifica: n/a.
    */
    public AsignacionesDataSet.AsignadoADataTable consultarAsignaciones()
    {

        AsignacionesDataSet.AsignadoADataTable dt = new AsignacionesDataSet.AsignadoADataTable();
        this.adapterAsignaciones.Fill(dt);
        return dt;
    }



    /* Requiere: n/a.
    * 
    *  Efectúa: Consulta cuales becarios no tienen una asignación en determinado perido y año 
    * 
    *  Modifica: n/a.
    */
    public AsignacionesDataSet.BecarioSinAsignacionDataTable consultarBecariosSinAsignacion(int periodo, int año)
    {


        AsignacionesDataSet.BecarioSinAsignacionDataTable dt = new AsignacionesDataSet.BecarioSinAsignacionDataTable();

        this.adapterBecarioSinAsignacion.Fill(dt, periodo, año);

        return dt;

    }



    /* Requiere: n/a.
    * 
    *  Efectúa: Cuenta cuantos becarios tiene asignados el encargado con cédula "ced" en un determinado periodo y año 
    * 
    *  Modifica: n/a.
    */
    public int contarBecariosAsignados(string ced, int año, int perido)
    {

       int i = Convert.ToInt32(this.adapterAsignaciones.contarBecariosAsignados(ced, año, perido) );
       return i;
    }



    /* Requiere: n/a.
     * 
     *  Efectúa: Consulta cuales son los becarios que tiene asignados el encargado con cédula "cedEncargado" en un determinado periodo y año 
     * 
     *  Modifica: n/a.
     */
    public AsignacionesDataSet.BecariosAsignadosAEncargadoDataTable consultarBecariosAsignadosAEncargado(string cedEncargado, int año, int perido)
    {

        AsignacionesDataSet.BecariosAsignadosAEncargadoDataTable dt = new AsignacionesDataSet.BecariosAsignadosAEncargadoDataTable();
        adapterBecariosAsignadosEncargado.Fill(dt, cedEncargado, perido, año);

        return dt;
    }


    /* Requiere: n/a.
     * 
     *  Efectúa:  Pone una asignación en estado "inactiva". 
     * 
     *  Modifica: n/a.
     */
    public void dejarAsignacionInactiva(string cedBecario, string cedEncargado, int año, int periodo)
    {

       adapterAsignaciones.asignacionActiva(false, cedBecario, periodo, año, cedEncargado);
    }



    /* Requiere: n/a.
     * 
     *  Efectúa: Consulta cual es el encargado a cargo del becario con cédula "cedBecario" en un determinado periodo y año . 
     * 
     *  Modifica: n/a.
     */
    public AsignacionesDataSet.EncargadoDeBecarioDataTable buscarEncargadoDeBecario(string cedBecario, int año, int periodo)
    {

        AsignacionesDataSet.EncargadoDeBecarioDataTable dt = new AsignacionesDataSet.EncargadoDeBecarioDataTable();
        adapterEncargadoDeBecario.Fill(dt, cedBecario, periodo, año);
        return dt; 
    }



    /* Requiere: n/a.
     * 
     *  Efectúa: Elimina la asignación del becario con cédula "cedBecario" en un determinado periodo y año . 
     * 
     *  Modifica: n/a.
     */
    public String eliminaAsignacionDeBecario(string cedBecario, int ped, int año)
    {

        string resultado = "Exito";

        try
        {
            this.adapterAsignaciones.eliminaAsignacionDeBecario(cedBecario, ped, año);         
        }
        catch (SqlException e)
        {
            resultado = "Error";

        }

        return resultado;
    }



    /*  Requiere: n/a.
     * 
     *  Efectúa: Elimina todas las asignaciones del encargado con cédula "cedEncargado" en un determinado periodo y año . 
     * 
     *  Modifica: n/a.
     */
    public String eliminaAsignacionesDeEncargado(string cedEncargado, int ped, int año)
    {

        string resultado = "Exito";

        try
        {
            this.adapterAsignaciones.eliminaAsignacionesEncargado(cedEncargado,ped,año);
        }
        catch (SqlException e)
        {
            resultado = "Error";

        }

        return resultado;
    }


}