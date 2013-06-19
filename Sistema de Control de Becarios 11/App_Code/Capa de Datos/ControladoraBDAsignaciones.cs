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

      

         AsignacionesDataSet.AsignadoADataTable dt = new AsignacionesDataSet.AsignadoADataTable();

         //AsignacionesDataSet.BecariosSinAsignarDataTable nueva;

         dt.Clear();

         this.adapterAsignaciones.consultarBecariosSinAsignacion(dt, periodo, año);
 
         //AsignacionesDataSet dataSetA = new AsignacionesDataSet();

         //dataSetA.EnforceConstraints = false;

         //dt.Constraints.Clear();

         //dataSetA.Tables["AsignadoA"];

         //dt.Constraints.Clear();
         //dataSetA.EnforceConstraints = false;

         return dt;

    }


    public int contarBecariosAsignados(string ced, int año, int perido)
    {

       int i = Convert.ToInt32(this.adapterAsignaciones.contarBecariosAsignados(ced, año, perido) );
       return i;
    }


}