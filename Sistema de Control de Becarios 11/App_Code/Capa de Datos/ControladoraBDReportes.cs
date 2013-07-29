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
    private AsignadoATableAdapter adapterAsignaciones;
	
    //EFECTO: Constructor de la clase. Inicializa los adaptadores de base de datos
    //REQUIERE: N/A
    //RETORNA: N/A
    public ControladoraBDReportes()
	{
        adapterReporte1 = new AsignacionBecarioEncargadoTableAdapter();
        adapterReporte2 = new BecarioTableAdapter();
        adapterSiglaUA = new SiglaUATableAdapter();
        adapterReporte5 = new ActividadEncargadoTableAdapter();
        adapterReporte4 = new ActividadBecariosTableAdapter();
		adapterAsignaciones = new AsignadoATableAdapter();
	}

    //Consultas para Reporte1

    //EFECTO: Genera la consulta a la base de datos del reporte de los becarios que han finalizado sus horas
    //REQUIERE: N/A
    //RETORNA: Un DataTable con la columnas correspondientes al reporte
    public DataTable reportarBecariosConHorasFinalizadas(string criterioDeBusqueda, int periodo, string año, int criterioLibre)
    {
        DataTable dt = new DataTable();
        dt = this.adapterReporte1.reportarBecariosConHorasFinalizadas(criterioDeBusqueda, periodo, Convert.ToInt32(año), criterioLibre.ToString());
        return dt;
    }

    //EFECTO: Genera la consulta a la base de datos del reporte de los becarios pendientes de horas
    //REQUIERE: N/A
    //RETORNA: Un DataTable con la columnas correspondientes al reporte
    public DataTable reportarBecariosPendientesDeHoras(string criterioDeBusqueda, int periodo, string año, int criterioLibre)
    {
        DataTable dt = new DataTable();
        dt = this.adapterReporte1.reportarBecariosPendientesDeHoras(criterioDeBusqueda, periodo, Convert.ToInt32(año), criterioLibre.ToString());
        return dt;
    }

    //Consultas para Reporte2

    //EFECTO: Genera la consulta a la base de datos del reporte de los becarios que no han sido asignados pero que si fueron asignados en el periodo anterior
    //REQUIERE: N/A
    //RETORNA: Un DataTable con la columnas correspondientes al reporte
    public DataTable reportarBecariosNoAsignados(string criterioBusquedaGeneral, int periodo, string año, int añoAnterior, int periodoAnterior)
    {
        DataTable dt = new DataTable();
        dt = this.adapterReporte2.reportarBecariosNoAsignados(criterioBusquedaGeneral, Convert.ToInt32(año), periodo, añoAnterior, periodoAnterior); 
        return dt;
    }

    //EFECTO: Genera la consulta a la base de datos del reporte de los becarios que no han sido asignados pero que si fueron asignados en el periodo anterior ni tras anterior
    //REQUIERE: N/A
    //RETORNA: Un DataTable con la columnas correspondientes al reporte
    public DataTable reportarBecariosNoAsignados2(string criterioBusquedaGeneral, int periodo, string año, int añoAnterior, int periodoAnterior, int añoTrasAnterior, int periodoTrasAnterior)
    {
        DataTable dt = new DataTable();
        dt = this.adapterReporte2.reportarBecariosNoAsignados2(criterioBusquedaGeneral, Convert.ToInt32(año), periodo, añoAnterior, periodoAnterior, añoTrasAnterior, periodoTrasAnterior);
        return dt;
    }

    //EFECTO: Genera la consulta a la base de datos del reporte de los becarios que no han sido asignados pero que si fueron asignados en el periodo anterior, tras anterior ni tras tras anterior
    //REQUIERE: N/A
    //RETORNA: Un DataTable con la columnas correspondientes al reporte
    public DataTable reportarBecariosNoAsignados3(string criterioBusquedaGeneral, int periodo, string año, int añoAnterior, int periodoAnterior, int añoTrasAnterior, int periodoTrasAnterior, int añoTrasTrasAnterior, int periodoTrasTrasAnterior)
    {
        DataTable dt = new DataTable();
        dt = this.adapterReporte2.reportarBecariosNoAsignados3(criterioBusquedaGeneral, Convert.ToInt32(año), periodo, añoAnterior, periodoAnterior, añoTrasAnterior, periodoTrasAnterior, añoTrasTrasAnterior, periodoTrasTrasAnterior);
        return dt;
    }

    //EFECTO: Genera la consulta a la base de datos del reporte de los becarios que no han sido asignados y que no han sido asignados nunca
    //REQUIERE: N/A
    //RETORNA: Un DataTable con la columnas correspondientes al reporte
    public DataTable reportarBecariosNoAsignados4(string criterioBusquedaGeneral)
    {
        DataTable dt = new DataTable();
        dt = this.adapterReporte2.reportarBecariosNoAsignados4(criterioBusquedaGeneral);
        return dt;
    }

    //EFECTO: Genera la consulta a la base de datos del reporte de los becarios que no han sido asignados para el periodo dado
    //REQUIERE: N/A
    //RETORNA: Un DataTable con la columnas correspondientes al reporte
    public DataTable reportarBecariosNoAsignados5(string criterioBusquedaGeneral, int criterioLibre, string añoActual, int periodoActual)
    {
        DataTable dt = new DataTable();
        dt = this.adapterReporte2.reportarBecariosNoAsignados5(criterioBusquedaGeneral, criterioLibre.ToString(), Convert.ToInt32(añoActual), periodoActual);
        return dt;
    }

    //Consultas para Reorte3

    //EFECTO: Genera la consulta a la base de datos de las unidades académicas existentes
    //REQUIERE: N/A
    //RETORNA: Un DataTable con la columnas correspondientes al reporte
    public DataTable optenerUnidadesAcademicas()
    {
        DataTable dt = new DataTable();
        dt = this.adapterSiglaUA.optenerUnidadesAcademicas();
        return dt;
    }

    //EFECTO: Genera la consulta a la base de datos del reporte de los becarios asigandos a una unidad académica específica
    //REQUIERE: N/A
    //RETORNA: Un DataTable con la columnas correspondientes al reporte
    public DataTable reportarBecariosPorUnidadAcademica(string criterioBusquedaGeneral, int periodo, string año, string siglaUA, int criterioLibre)
    {
        DataTable dt = new DataTable();
        dt = this.adapterReporte1.reportarBecariosPorUnidadAcademica(criterioBusquedaGeneral, criterioLibre.ToString(), periodo, Convert.ToInt32(año), siglaUA);
        return dt;
    }

    //Consultas para Reporte4

    //EFECTO: Genera la consulta a la base de datos del reporte de los becarios Inactivos
    //REQUIERE: N/A
    //RETORNA: Un DataTable con la columnas correspondientes al reporte
    public DataTable llenarBecariosInactivos(string criterioBusqueda, int año, int periodo, DateTime fechaUltimoReporte, int criterioLibre)
    {
        DataTable dt = new DataTable();
        dt = adapterReporte4.obtenerBecariosInactivos(criterioLibre.ToString(), fechaUltimoReporte, periodo, año, criterioBusqueda);
        return dt;
    }

    //Consultas para Reporte5

    //EFECTO: Genera la consulta a la base de datos del reporte de los encargados que están atrasados con sus reportes de horas
    //REQUIERE: N/A
    //RETORNA: Un DataTable con la columnas correspondientes al reporte
    public DataTable llenarEncargadosAtrasados(string criterioBusqueda, int año, int semestre, DateTime fechaUltimoReporte, int criterioLibre)
    {
        DataTable dt = new DataTable();
        dt = adapterReporte5.obtenerEncargadosAtrasados(fechaUltimoReporte, semestre, año, criterioBusqueda, criterioLibre.ToString());
        return dt;
    }

    //Consultas para Reporte6

    //EFECTO: Genera la consulta a la base de datos del reporte del historial de asignaciones de un becario
    //REQUIERE: N/A
    //RETORNA: Un DataTable con la columnas correspondientes al reporte
    public DataTable reportarHistorialDeAsignacionesBecario(string criterioBusqueda, string cedula, int tipo) 
    {
        DataTable dt = new DataTable();
        if (tipo == 0)
        {
            dt = adapterReporte1.reportarHistorialDeAsignacionesBecario(criterioBusqueda, cedula);
        }
        else 
        {
            dt = adapterReporte1.reportarHistorialDeAsignacionesBecarioTodos(criterioBusqueda);
        }
        return dt;
    }


     //Consultas para Reporte7

    //EFECTO: Genera la consulta a la base de datos del reporte del historial de anotaciones de un encargado
    //REQUIERE: N/A
    //RETORNA: Un DataTable con la columnas correspondientes al reporte
    public DataTable reportarHistorialDeAsignacionesEncargado(string criterioBusqueda, string cedula, int tipo) 
    {
        DataTable dt = new DataTable();
        if (tipo == 0)
        {
            dt = adapterReporte1.reportarHistorialDeAsignacionesEncargado(criterioBusqueda, cedula);
        }
        else 
        {
            dt = adapterReporte1.reportarHistorialDeAsignacionesEncargadoTodos(criterioBusqueda);
        }

        return dt;
    }


	public DataTable obtenerCantidadesDeHorasCompletadas(string periodo, string año)
	{
		DataTable dt = new DataTable();
		dt = adapterAsignaciones.obtenerCantidadesDeHorasFinalizadas(Int32.Parse(año), Int32.Parse(periodo));
		return dt;
	}
}
