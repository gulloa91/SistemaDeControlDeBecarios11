using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ControlDeHorasDataSetTableAdapters;
using System.Data;
using System.Data.SqlClient;

public class ControladoraBDControlDeHoras
{
    private ControlDeHorasTableAdapter adapterControlDeHoras;
    private ControlDeHoras1TableAdapter adapterAuxiliar;
    private ComentarioTableAdapter adapterComentario;
    private AsignadoATableAdapter adapterAsignadoA;
    public ControladoraBDControlDeHoras()
    {
        adapterControlDeHoras = new ControlDeHorasTableAdapter();
        adapterAuxiliar = new ControlDeHoras1TableAdapter();
        adapterComentario = new ComentarioTableAdapter();
        adapterAsignadoA = new AsignadoATableAdapter();
    }

    public String insertarReporte(ControlDeHoras controlDeHoras)
    {
        String resultado = "";
        try
        {
            //            adapterControlDeHoras.Insert(controlDeHoras.cedulaBecario, controlDeHoras.cedulaEncargado, controlDeHoras.cantidadHoras, controlDeHoras.fecha, controlDeHoras.estado, controlDeHoras.comentarioBecario, controlDeHoras.comentarioEncargado);
        }
        catch (SqlException e)
        {
            resultado = "Error al insertar el control de horas";
        }
        return resultado;
    }

    public String modificarReporte(ControlDeHoras controlDeHorasViejo, ControlDeHoras controlDeHorasNuevo)
    {
        String resultado = "";
        try
        {
            adapterControlDeHoras.updateReporte(controlDeHorasNuevo.cantidadHoras, controlDeHorasNuevo.estado, controlDeHorasNuevo.comentarioBecario, controlDeHorasNuevo.cedulaBecario, controlDeHorasNuevo.cedulaEncargado, controlDeHorasNuevo.fecha, 1, DateTime.Now.Year);
        }
        catch (SqlException e)
        {
            resultado = "Error al modificar el control de horas";
        }
        return resultado;
    }

    // Se encarga de modificar un reporte de horas en la base de datos. Es decir, cambia un reporte de horas pendiente a aceptado o rechazado.
    public String modificarReporteEncargado(ControlDeHoras controlDeHorasViejo, ControlDeHoras controlDeHorasNuevo)
    {
        String resultado = "";
        try
        {
            adapterControlDeHoras.Update(controlDeHorasNuevo.cedulaBecario, controlDeHorasNuevo.cedulaEncargado, controlDeHorasNuevo.cantidadHoras, controlDeHorasNuevo.fecha, controlDeHorasNuevo.estado, controlDeHorasNuevo.comentarioBecario, controlDeHorasNuevo.comentarioEncargado, controlDeHorasNuevo.periodo, controlDeHorasNuevo.año, controlDeHorasViejo.cedulaBecario, controlDeHorasViejo.cedulaEncargado, controlDeHorasViejo.fecha);
        }
        catch (SqlException e)
        {
            resultado = "Error al modificar el control de horas";
        }
        return resultado;
    }

    // Retorna todos los becarios que hayan hecho reportes hacia un encargado. El listado se ve determinado por el tipo de horas a consultar (rechazados, aceptados o pendientes) indicados en la variable tipo
    public DataTable consultarReportesBecarios(string idEncargado, int tipo)
    {
        DataTable dt = new DataTable();
        dt = adapterAuxiliar.getBecariosPorEncargadoAndTipo(idEncargado, tipo);
        return dt;
    }

    // Retorna todos los reportes de horas realizados por un becario hacia un encargado. Estos reportes pueden ser pendientes, rechazados o aceptados (indicados en la variable tipo)
    public DataTable consultarReportesHorasBecarios(string idEncargado, string idBecario, int tipo)
    {
        DataTable dt = new DataTable();
        dt = adapterAuxiliar.getDataByBecarioEncargadoEstado(idEncargado, idBecario, tipo);
        return dt;
    }

    // Se encarga de insertar en la base de datos un comentario que el encargado efectua hacia un becario cuando le rechaza un reporte de horas.
    public String insertarComentarioEncargado(Comentario comentario)
    {
        String resultado = "";
        try
        {
            adapterComentario.Insert(comentario.cedulaEncargado, comentario.fecha, comentario.cedulaBecario, comentario.comentario);
        }
        catch (SqlException e)
        {
            resultado = "Error al insertar el comentario";
        }
        return resultado;
    }

    //Obtiene el total de horas que un encargado le ha aceptado a un becario.
    public int obtenerTotalHoras(String cedulaEncargado, String cedulaBecario, int estado)
    {
        int resultado = 0;
        try
        {
            resultado = (int)(adapterAuxiliar.totalHoras(cedulaEncargado, cedulaBecario, estado));
        }
        catch (Exception e)
        {

        }
        return resultado;
    }

    // Retorna el total de horas que le fueron asignadas a un becario en un periodo de un determinado año.
    public int horasAsignadasBecario(String cedulaEncargado, String cedulaBecario, int periodo, int año)
    {
        int resultado = 0;
        try
        {
            resultado = (int)(adapterAsignadoA.getTotalHoras(cedulaBecario, cedulaEncargado, periodo, año));
        }
        catch (Exception e)
        {

        }
        return resultado;
    }
    
    // Se encarga de finalizar la asignacion entre un becario y un encargado, cuando el becario cumple con las horas asignadas
    public String finalizarAsignacion(Object[] datos)
    {
        String resultado = "";
        try
        {
            adapterAsignadoA.finalizarAsignacion(datos[0].ToString(), datos[1].ToString(), Convert.ToInt32(datos[2].ToString()), Convert.ToInt32(datos[3].ToString()), datos[4].ToString());
        }
        catch (SqlException e)
        {
            resultado = "Error al modificar la asignación";
        }
        return resultado;
    }

    // Se encarga de crear una nueva asignacion entre un becario y un encargado.
    public String crearAsignacion(Object[] datos)
    {
        String resultado = "";
        try
        {
            adapterAsignadoA.Insert(datos[0].ToString(), Convert.ToInt32(datos[1].ToString()), Convert.ToInt32(datos[2].ToString()), datos[3].ToString(), Convert.ToInt32(datos[4].ToString()), null, null, 3, false, null, null, null);
        }
        catch (SqlException e)
        {
            resultado = "Error al crear la asignación";
        }
        return resultado;
    }

    // Se encarga de retornar todos los becarios que tengan horas pendientes de revisar por un encargado especifico.
    public int totalBecariosPendientes(String cedulaEncargado, int estado)
    {
        int resultado = -1;
        try
        {
            resultado = (int)(adapterAuxiliar.becariosPendientes(cedulaEncargado, estado));
        }
        catch (SqlException e)
        {

        }
        return resultado;
    }

}
