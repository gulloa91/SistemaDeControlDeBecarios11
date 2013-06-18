using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ControlDeHorasDataSetTableAdapters;
using System.Data;

public class ControladoraBDControlDeHoras
{
    private ControlDeHorasTableAdapter adapterControlDeHoras;
	public ControladoraBDControlDeHoras()
	{
        adapterControlDeHoras = new ControlDeHorasTableAdapter();
	}

    public DataTable consultarReportesBecarios(String cedulaEncargado, int estado){
        DataTable dt = new DataTable();
        dt = adapterControlDeHoras.consultarReportesBecarios(cedulaEncargado, estado);
        return dt;
    }

    public DataTable consultarReportesHorasBecarios(String cedulaEncargado, String cedulaBecario, int estado) {
        DataTable dt = new DataTable();
        dt = adapterControlDeHoras.consultarReportesHorasBecarios(cedulaBecario, cedulaEncargado, estado);
        return dt;
    }

    //-----------------------------------------
    //Inicia parte de Beto

    public DataTable consultarReportesBecarios(string idEncargado, int tipo)
    {
        DataTable dt = new DataTable();
        dt = adapterControlDeHoras.getDataByEncargadoAndTipo(idEncargado, tipo);
        return dt;
    }

    public DataTable consultarReportesBecarios(string idEncargado, string idBecario, int tipo)
    {
        DataTable dt = new DataTable();
        dt = adapterControlDeHoras.getDataByBecarioEncargadoEstado(idEncargado, idBecario, tipo);
        return dt;
    }

}