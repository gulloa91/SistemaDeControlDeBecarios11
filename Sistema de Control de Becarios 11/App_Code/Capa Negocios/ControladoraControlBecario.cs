using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using ControlDeHorasDataSetTableAdapters;

/// <summary>
/// Summary description for ControladoraControlBecario
/// </summary>
public class ControladoraControlBecario
{
    ControladoraControlBecarioBD cb;
	public ControladoraControlBecario()
	{
		//
		// TODO: Add constructor logic here
		//
        cb = new ControladoraControlBecarioBD();
	}

    public DataTable horasReportadas(String becario,String encargado) {
        return cb.horasReportadas(becario,encargado);
    }

    //retorna la cedula del encargado para el becario en el periodo actual
    public String getCedulaEncargado(String becario,int periodo) {
        return cb.getCedEncargado(becario,periodo);
    }
    //inserta un reporte de horas
    public String enviarReporte(Object[] datos)
    {
        ControlDeHoras c = new ControlDeHoras(datos);
        return this.cb.enviarReporte(c);
    }
    //modifica el reporte de horas
    public int modificarReporte(Object [] datos) {
        return this.cb.modificarReporte(new ControlDeHoras(datos));
    }

    //retorna las horas de un becario
    public int getHoras(String becario, String encargado,int periodo) {
        return cb.getHoras(becario, encargado,periodo);
    }

    //para agrega el comentario final del becario sobre la asignacion
    public String comentarioFinal(String becario, String encargado, String comentario)
    { 
        return cb.agregarComentarioFinal( becario, encargado, comentario);
    }

    //retorna el estado de una asignacion
    public int getEstado(String becario,String encargado, int periodo) {
        return cb.getAsignacion(becario, encargado,periodo);
    }

    public int aceptarSiguienteAsignacion(Object [] datos) {
        return cb.aceptarSiguienteAsignacion(datos);
    }

    public String getComentarioBecarioFinal(String becario, String encargado)
    {
        return cb.getComentarioBecarioFinal(becario, encargado);
    }
}

