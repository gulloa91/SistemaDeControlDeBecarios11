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


    public AsignacionesDataSet.AsignadoADataTable consultarBecariosSinAsignacion(int periodo, int año)
    {

        /*AsignacionesDataSet dataSet = new AsignacionesDataSet();

        dataSet.AsignadoADataTable.Clear();

        dataSet.EnforceConstraints = false;*/

        AsignacionesDataSet.AsignadoADataTable dt = new AsignacionesDataSet.AsignadoADataTable();
        this.adapterAsignaciones.consultarBecariosSinAsignacion(dt,año,periodo);
        return dt;
    }



}