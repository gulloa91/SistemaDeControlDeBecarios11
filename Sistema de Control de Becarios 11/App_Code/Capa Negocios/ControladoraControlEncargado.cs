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

    public ControladoraControlEncargado()
    {
        controladoraBDEncargado = new ControladoraBDControlDeHoras();
        contAsig = new ControladoraAsignaciones();
        cb = new ControladoraBecarios();
    }

    //-----------------------------------------
    //Inicia parte de Beto

    public DataTable consultarReportesBecarios(string idEncargado, int tipo)
    {
        return controladoraBDEncargado.consultarReportesBecarios(idEncargado, tipo);
    }

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

    public String modificarReporteEncargado(Object[] nuevo, Object[] viejo)
    {
        String resultado = "-1";
        ControlDeHoras cv = new ControlDeHoras(viejo);
        ControlDeHoras cn = new ControlDeHoras(nuevo);
        resultado = controladoraBDEncargado.modificarReporteEncargado(cv, cn);
        return resultado;
    }

    public String insertarComentarioEncargado(Object[] datos)
    {
        String resultado = "-1";
        Comentario comentario = new Comentario(datos);
        resultado = controladoraBDEncargado.insertarComentarioEncargado(comentario);
        return resultado;
    }

    public int obtenerTotalHoras(String cedulaEncargado, String cedulaBecario, int estado)
    {
        return controladoraBDEncargado.obtenerTotalHoras(cedulaEncargado, cedulaBecario, estado);
    }

    public int horasAsignadasBecario(String cedulaEncargado, String cedulaBecario, int periodo, int año)
    {
        return controladoraBDEncargado.horasAsignadasBecario(cedulaEncargado, cedulaBecario, periodo, año);
    }

    public String finalizarAsignacion(Object[] datos)
    {
        String resultado = "-1";
        resultado = controladoraBDEncargado.finalizarAsignacion(datos);
        return resultado;
    }

    public String crearAsignacion(Object[] datos)
    {
        String resultado = "-1";
        resultado = controladoraBDEncargado.crearAsignacion(datos);
        return resultado;
    }

    public int totalBecarios(String cedulaEncargado, int estado)
    {
        return controladoraBDEncargado.totalBecariosPendientes(cedulaEncargado, estado);
    }

    public String obtenerNombrePorCedula(String cedula)
    {
        return cb.obtenerNombrePorCedula(cedula);
    }

}