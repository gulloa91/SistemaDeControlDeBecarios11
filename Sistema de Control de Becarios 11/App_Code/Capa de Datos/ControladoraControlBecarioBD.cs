using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using ControlDeHorasDataSetTableAdapters;

/// <summary>
/// Summary description for ControladoraControlBecarioBD
/// </summary>
public class ControladoraControlBecarioBD
{
    ControlDeHorasTableAdapter ch;
    AsignadoATableAdapter a;
	public ControladoraControlBecarioBD()
	{
		//
		// TODO: Add constructor logic here
		//
        ch = new ControlDeHorasTableAdapter();
        a = new AsignadoATableAdapter();
	}

    public DataTable horasReportadas(String becario,String encargado)
    {
        DataTable retorno;
        try
        {
            retorno = ch.getReportesByBecario(becario,encargado) ;
        }
        catch (Exception e)
        {
            retorno = null;
        }
        return retorno;
    }

    //retorna la cedula del encargado para cierto becario
    public String getCedEncargado(String becario) {
        String resultado = "";
        try
        {
            resultado = a.getEncargadoByBecario(becario,1,DateTime.Now.Year).ToString();
        }catch(Exception ex){
            resultado = "";
        }
        return resultado;
    }

    public String enviarReporte(ControlDeHoras c)
    {
        String resultado = "Envío Exitoso";
        try
        {
            int result = ch.Insert(c.cedulaBecario, c.cedulaEncargado, c.cantidadHoras, c.fecha, c.estado, c.comentarioBecario, c.comentarioEncargado);
        }
        catch (Exception ex)
        {
            resultado = "Envío Fallido";
        }
        return resultado;
    }

    public int modificarReporte(ControlDeHoras c) { 
        int resultado = -1;
        try
        {
            resultado = ch.updateReporte(c.cantidadHoras, c.estado, c.comentarioBecario, c.cedulaBecario, c.cedulaEncargado, c.fecha);
        }
        catch (Exception ex)
        {
            resultado = 0;
        }
        return 1;
    }

}