using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

public class ControladoraControlEncargado
{
    ControladoraBDControlDeHoras controladoraBDEncargado;
    ControladoraAsignaciones contAsig;
    ControladoraBecarios cb;

    // Constructor que inicializa las controladoras de base de datos y otras controladoras necesarias para la comunicacion entre controladoras
    public ControladoraControlEncargado()
    {
        controladoraBDEncargado = new ControladoraBDControlDeHoras();
        contAsig = new ControladoraAsignaciones();
        cb = new ControladoraBecarios();
    }

    //Consulto todos los becarios que tengan reportes de horas hechas hacia un encargado (especificado en el parametro), la consulta depende del tipo de
    // horas (rechazadas, aceptadas o rechazadas) especificadas en el parametro "tipo"
    public DataTable consultarReportesBecarios(string idEncargado, int tipo)
    {
        return controladoraBDEncargado.consultarReportesBecarios(idEncargado, tipo);
    }

    // devuelve el listado de los reportes de horas que haya hecho un becario hacia un encargado. La consulta puede hacerse por horas aceptadas, rechazadas o pendientes.
    public DataTable consultarReportesHorasBecarios(String cedulaEncargado, String cedulaBecario, int estado)
    {
        return controladoraBDEncargado.consultarReportesHorasBecarios(cedulaEncargado, cedulaBecario, estado);
    }

    
    public String modificarReporte(Object[] nuevo, Object[] viejo)
    {
        String resultado = "-1";
        ControlDeHoras cv = new ControlDeHoras(viejo);
        ControlDeHoras cn = new ControlDeHoras(nuevo);
        resultado = controladoraBDEncargado.modificarReporte(cv, cn);
        return resultado;
    }

    // Se encarga de llamar a la controladora de base de datos de control de horas para que actualice un reporte de horas pendientes a aeptadas o rechazadas.
    public String modificarReporteEncargado(Object[] nuevo, Object[] viejo)
    {
        String resultado = "-1";
        ControlDeHoras cv = new ControlDeHoras(viejo);
        ControlDeHoras cn = new ControlDeHoras(nuevo);
        resultado = controladoraBDEncargado.modificarReporteEncargado(cv, cn);
        return resultado;
    }

    // crea una entidad de comentario y llama a la controladora de base de datos para insertar el comentario del encargado hacia el becario
    public String insertarComentarioEncargado(Object[] datos)
    {
        String resultado = "-1";
        Comentario comentario = new Comentario(datos);
        resultado = controladoraBDEncargado.insertarComentarioEncargado(comentario);
        return resultado;
    }

    // Obtiene el total de horas que tiene un becario hacia un encargado. De igual manera las horas pueden ser aceptadas, rechazadas o pendientes.
    public int obtenerTotalHoras(String cedulaEncargado, String cedulaBecario, int estado)
    {
        return controladoraBDEncargado.obtenerTotalHoras(cedulaEncargado, cedulaBecario, estado);
    }

    // Obtiene el total de horas que fueron asignadas a un becario
    public int horasAsignadasBecario(String cedulaEncargado, String cedulaBecario, int periodo, int año)
    {
        return controladoraBDEncargado.horasAsignadasBecario(cedulaEncargado, cedulaBecario, periodo, año);
    }

    // Llama a la controladora de base de datos para que finalice una asignacion entre un becario y un encargado
    public String finalizarAsignacion(Object[] datos)
    {
        String resultado = "-1";
        resultado = controladoraBDEncargado.finalizarAsignacion(datos);
        return resultado;
    }

    // Se encarga de llamar a la controladora de base de datos para crear una asignacion entre un becario y un encargado
    public String crearAsignacion(Object[] datos)
    {
        String resultado = "-1";
        resultado = controladoraBDEncargado.crearAsignacion(datos);
        return resultado;
    }

    // Retorna el reporte con los becarios asignados a un encargado que tengan horas (pendientes, rechazadas o aceptadas) indicadas en la variable estado
    public int totalBecarios(String cedulaEncargado, int estado)
    {
        return controladoraBDEncargado.totalBecariosPendientes(cedulaEncargado, estado);
    }

    // Obtiene el nombre de un becario que tenga la cedula indicada por parametro
    public String obtenerNombrePorCedula(String cedula)
    {
        return cb.obtenerNombrePorCedula(cedula);
    }

}