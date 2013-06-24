using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using ReportesDataSetTableAdapters;
using System.Data;

/// <summary>
/// Descripción breve de ControladoraBDReportes
/// </summary>
public class ControladoraBDReportes
{
    private AsignacionBecarioEncargadoTableAdapter adapterReporte1;
    private BecarioTableAdapter adapterReporte2;
    private SiglaUATableAdapter adapterSiglaUA;
    private ActividadEncargadoTableAdapter adapterReporte5;
    private ActividadBecariosTableAdapter adapterReporte4;
	public ControladoraBDReportes()
	{
        adapterReporte1 = new AsignacionBecarioEncargadoTableAdapter();
        adapterReporte2 = new BecarioTableAdapter();
        adapterSiglaUA = new SiglaUATableAdapter();
        adapterReporte5 = new ActividadEncargadoTableAdapter();
        adapterReporte4 = new ActividadBecariosTableAdapter();

	}

    //Consultas para Reporte1
    public DataTable reportarBecariosConHorasFinalizadas(string criterioDeBusqueda, int periodo, string año)
    {
        DataTable dt = new DataTable();
        dt = this.adapterReporte1.reportarBecariosConHorasFinalizadas(criterioDeBusqueda, periodo, Convert.ToInt32(año));
        return dt;
    }

    public DataTable reportarBecariosPendientesDeHoras(string criterioDeBusqueda, int periodo, string año)
    {
        DataTable dt = new DataTable();
        dt = this.adapterReporte1.reportarBecariosPendientesDeHoras(criterioDeBusqueda, periodo, Convert.ToInt32(año));
        return dt;
    }

    //Consultas para Reporte2
    public DataTable reportarBecariosNoAsignados(string criterioBusquedaGeneral, int periodo, string año, int añoAnterior, int periodoAnterior)
    {
        DataTable dt = new DataTable();
        dt = this.adapterReporte2.reportarBecariosNoAsignados(criterioBusquedaGeneral, Convert.ToInt32(año), periodo, añoAnterior, periodoAnterior); 
        return dt;
    }

    public DataTable reportarBecariosNoAsignados2(string criterioBusquedaGeneral, int periodo, string año, int añoAnterior, int periodoAnterior, int añoTrasAnterior, int periodoTrasAnterior)
    {
        DataTable dt = new DataTable();
        dt = this.adapterReporte2.reportarBecariosNoAsignados2(criterioBusquedaGeneral, Convert.ToInt32(año), periodo, añoAnterior, periodoAnterior, añoTrasAnterior, periodoTrasAnterior);
        return dt;
    }

    public DataTable reportarBecariosNoAsignados3(string criterioBusquedaGeneral, int periodo, string año, int añoAnterior, int periodoAnterior, int añoTrasAnterior, int periodoTrasAnterior, int añoTrasTrasAnterior, int periodoTrasTrasAnterior)
    {
        DataTable dt = new DataTable();
        dt = this.adapterReporte2.reportarBecariosNoAsignados3(criterioBusquedaGeneral, Convert.ToInt32(año), periodo, añoAnterior, periodoAnterior, añoTrasAnterior, periodoTrasAnterior, añoTrasTrasAnterior, periodoTrasTrasAnterior);
        return dt;
    }

    //Consultas para Reorte3
    public DataTable optenerUnidadesAcademicas()
    {
        DataTable dt = new DataTable();
        dt = this.adapterSiglaUA.optenerUnidadesAcademicas();
        return dt;
    }

    public DataTable reportarBecariosPorUnidadAcademica(string criterioBusquedaGeneral, int periodo, string año, string siglaUA)
    {
        DataTable dt = new DataTable();
        dt = this.adapterReporte1.reportarBecariosPorUnidadAcademica(criterioBusquedaGeneral, periodo, Convert.ToInt32(año), siglaUA);
        return dt;
    }

    //Consultas para Reporte4
    public DataTable llenarBecariosInactivos(string criterioBusqueda, int año, int periodo, DateTime fechaUltimoReporte)
    {
        DataTable dt = new DataTable();
        dt = adapterReporte4.obtenerBecariosInactivos(fechaUltimoReporte, criterioBusqueda, año, periodo);
        return dt;
    }

    //Consultas para Reporte5
    public DataTable llenarEncargadosAtrasados(string criterioBusqueda, int año, int semestre, DateTime fechaUltimoReporte)
    {
        DataTable dt = new DataTable();
        dt = adapterReporte5.obtenerEncargadosAtrasados(fechaUltimoReporte, semestre, año, criterioBusqueda);
        return dt;
    }

    //Consultas para Reporte6
    public DataTable reportarHistorialDeAsignacionesBecario(string criterioBusqueda, string cedula) 
    {
        DataTable dt = new DataTable();
        dt = adapterReporte1.reportarHistorialDeAsignacionesBecario(criterioBusqueda, cedula);
        return dt;
    }

     //Consultas para Reporte7
    public DataTable reportarHistorialDeAsignacionesEncargado(string criterioBusqueda, string cedula) 
    {
        DataTable dt = new DataTable();
        dt = adapterReporte1.reportarHistorialDeAsignacionesEncargado(criterioBusqueda, cedula);
        return dt;
    }

}