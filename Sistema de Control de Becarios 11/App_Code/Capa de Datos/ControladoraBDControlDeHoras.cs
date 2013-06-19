using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ControlDeHorasDataSetTableAdapters;
using System.Data;
using System.Data.SqlClient;

public class ControladoraBDControlDeHoras
{
    private ControlDeHorasTableAdapter adapterControlDeHoras;
	public ControladoraBDControlDeHoras()
	{
        adapterControlDeHoras = new ControlDeHorasTableAdapter();
	}

    //-----------------------------------------
    //Inicia parte de Beto

    public String insertarReporte(ControlDeHoras controlDeHoras) {
        String resultado = "";
        try {
            adapterControlDeHoras.Insert(controlDeHoras.cedulaBecario, controlDeHoras.cedulaEncargado, controlDeHoras.cantidadHoras, controlDeHoras.fecha, controlDeHoras.estado, controlDeHoras.comentarioBecario, controlDeHoras.comentarioEncargado);
        }
        catch (SqlException e) {
            resultado = "Error al insertar el control de horas";
        }
        return resultado;
    }

    public DataTable consultarReportesBecarios(string idEncargado, int tipo)
    {
        DataTable dt = new DataTable();
        dt = adapterControlDeHoras.getDataByEncargadoAndTipo(idEncargado, tipo);
        return dt;
    }

    public DataTable consultarReportesHorasBecarios(string idEncargado, string idBecario, int tipo)
    {
        DataTable dt = new DataTable();
        dt = adapterControlDeHoras.getDataByBecarioEncargadoEstado(idEncargado, idBecario, tipo);
        return dt;
    }

}