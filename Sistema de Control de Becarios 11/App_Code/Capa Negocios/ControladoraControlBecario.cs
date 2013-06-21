using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

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
    public String getCedulaEncargado(String becario) {
        return cb.getCedEncargado(becario);
    }

    public String enviarReporte(Object[] datos)
    {
        ControlDeHoras c = new ControlDeHoras(datos);
        return this.cb.enviarReporte(c);
    }

    public int modificarReporte(Object [] datos) {
        return this.cb.modificarReporte(new ControlDeHoras(datos));
    }
}

