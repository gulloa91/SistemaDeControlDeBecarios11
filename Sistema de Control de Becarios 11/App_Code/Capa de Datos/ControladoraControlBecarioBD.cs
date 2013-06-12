using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using ControlHorasBecarioTableAdapters;
/// <summary>
/// Summary description for ControladoraControlBecarioBD
/// </summary>
public class ControladoraControlBecarioBD
{
    ControlDeHorasTableAdapter cb;
	public ControladoraControlBecarioBD()
	{
		//
		// TODO: Add constructor logic here
		//
        cb = new ControlDeHorasTableAdapter();
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
}