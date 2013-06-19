using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

public class ControladoraControlEncargado
{
    ControladoraBDControlDeHoras controladoraBDEncargado;
	public ControladoraControlEncargado()
	{
        controladoraBDEncargado = new ControladoraBDControlDeHoras();
	}

    public DataTable consultarReportesBecarios(string idEncargado, int tipo) {
        return controladoraBDEncargado.consultarReportesBecarios(idEncargado, tipo);
    }

    public DataTable consultarReportesHorasBecarios(String cedulaEncargado, String cedulaBecario, int estado)
    {
        return controladoraBDEncargado.consultarReportesHorasBecarios(cedulaEncargado, cedulaBecario, estado);
    }
}