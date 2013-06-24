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

    //-----------------------------------------
    //Inicia parte de Beto

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

    public DataTable consultarReportesBecarios(string idEncargado, int tipo)
    {
        DataTable dt = new DataTable();
        dt = adapterAuxiliar.getBecariosPorEncargadoAndTipo(idEncargado, tipo);
        return dt;
    }

    public DataTable consultarReportesHorasBecarios(string idEncargado, string idBecario, int tipo)
    {
        DataTable dt = new DataTable();
        dt = adapterAuxiliar.getDataByBecarioEncargadoEstado(idEncargado, idBecario, tipo);
        return dt;
    }

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