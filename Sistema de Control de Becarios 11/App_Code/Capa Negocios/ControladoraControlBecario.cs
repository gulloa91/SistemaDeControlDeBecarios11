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

    public DataTable horasReportadas(String becario) {
        return cb.horasReportadas(becario);
    }
}