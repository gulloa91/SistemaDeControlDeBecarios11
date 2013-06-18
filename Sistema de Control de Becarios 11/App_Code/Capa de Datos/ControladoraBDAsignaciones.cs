using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using AsignacionesDataSetTableAdapters;
using System.Data;

/// <summary>
/// Summary description for ControladoraBDAsignaciones
/// </summary>
public class ControladoraBDAsignaciones
{

    AsignadoATableAdapter adapterAsignaciones;


	public ControladoraBDAsignaciones()
	{
        adapterAsignaciones = new AsignadoATableAdapter();
	}



    public AsignacionesDataSet.AsignadoADataTable consultarAsignaciones()
    {

        AsignacionesDataSet.AsignadoADataTable dt = new AsignacionesDataSet.AsignadoADataTable();
        this.adapterAsignaciones.Fill(dt);
        return dt;
    }


}