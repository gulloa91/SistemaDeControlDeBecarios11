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
    BecarioSinAsignacionTableAdapter adapterBecarioSinAsignacion;

	public ControladoraBDAsignaciones()
	{
        adapterAsignaciones = new AsignadoATableAdapter();
        adapterBecarioSinAsignacion = new BecarioSinAsignacionTableAdapter();
	}



    public String insertarAsignacion( Asignacion asignacion){

        String returnValue = "Exito";
        int r;
        try
        {
            this.adapterAsignaciones.Insert(asignacion.CedulaBecario, asignacion.Periodo, asignacion.Año, asignacion.CedulaEncargado, asignacion.TotalHoras, asignacion.SiglasUA, asignacion.InfoUbicacion, asignacion.Estado);
        }
        catch (SqlException e)
        {
            r = e.Number;
            if (r == 2627)
            {
                returnValue = "Error1"; //"Ya existe asignación";
            }
            else
            {
                returnValue = "Error2";
            }
        }
        return returnValue;
    }


    public AsignacionesDataSet.AsignadoADataTable consultarAsignaciones()
    {

        AsignacionesDataSet.AsignadoADataTable dt = new AsignacionesDataSet.AsignadoADataTable();
        this.adapterAsignaciones.Fill(dt);
        return dt;
    }


    public AsignacionesDataSet.BecarioSinAsignacionDataTable consultarBecariosSinAsignacion(int periodo, int año)
    {


        AsignacionesDataSet.BecarioSinAsignacionDataTable dt = new AsignacionesDataSet.BecarioSinAsignacionDataTable();

        this.adapterBecarioSinAsignacion.Fill(dt, periodo, año);

        return dt;

    }


    public int contarBecariosAsignados(string ced, int año, int perido)
    {

       int i = Convert.ToInt32(this.adapterAsignaciones.contarBecariosAsignados(ced, año, perido) );
       return i;
    }


}