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
    ControlDeHorasTableAdapter cb;
    AsignadoATableAdapter a;
	public ControladoraControlBecarioBD()
	{
		//
		// TODO: Add constructor logic here
		//
        cb = new ControlDeHorasTableAdapter();
        a = new AsignadoATableAdapter();
	}

    public DataTable horasReportadas(String becario)
    {
        DataTable retorno;
        try
        {
            retorno = cb.getReportesByBecario(becario) ;
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
            resultado = a.getEncargadoByBecario(becario,1,2013);
        }catch(Exception ex){
            resultado = "";
        }
        return resultado;
    }
}