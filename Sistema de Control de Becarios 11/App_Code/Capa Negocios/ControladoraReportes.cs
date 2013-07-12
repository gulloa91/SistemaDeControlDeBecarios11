using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

/// <summary>
/// Descripción breve de ControladoraReportes
/// </summary>
public class ControladoraReportes
{
    private ControladoraBDReportes controladoraBDReportes;

    private CommonServices cs;

	public ControladoraReportes()
	{
        cs = new CommonServices(null);
        controladoraBDReportes = new ControladoraBDReportes();
	}

    public List<Object[]> reporteBecarios(int tipoReporte, string criterioBusquedaGeneral, string criterio1, string criterio2, string criterio3, string criterio4, string criterio5)
    {
        List<Object[]> lsOject = new List<Object[]>();

        switch(tipoReporte)
        {
            case 1:
                {
                    int periodo = 0;
                    switch (criterio2)
                    {
                        case "I   - Periodo":
                            {
                                periodo = 1;
                            } break;
                        case "II  - Periodo":
                            {
                                periodo = 2;
                            } break;
                        case "III   - Periodo":
                            {
                                periodo = 3;
                            } break;
                    }

                    DataTable tbBecarios = new DataTable();
                    switch (criterio1)
                    {
                        case "Horas Finalizadas":
                            {
                                tbBecarios = controladoraBDReportes.reportarBecariosConHorasFinalizadas(criterioBusquedaGeneral, periodo, criterio3);
                            } break;
                        case "Pendiente de Horas":
                            {
                                tbBecarios = controladoraBDReportes.reportarBecariosPendientesDeHoras(criterioBusquedaGeneral, periodo, criterio3);
                            } break;
                    }

                    List<object[]> lsObjectAux = new List<object[]>();

        
                    foreach (DataRow r in tbBecarios.Rows)
                    {
            
                        Object[] obj = new Object[20]; 

                        obj[0] = cs.procesarStringDeUI(r["FotoB"].ToString());
                        obj[1] = cs.procesarStringDeUI(r["NombreB"].ToString());
                        obj[2] = cs.procesarStringDeUI(r["Apellido1B"].ToString());
                        obj[3] = cs.procesarStringDeUI(r["Apellido2B"].ToString());
                        obj[4] = cs.procesarStringDeUI(r["CarneB"].ToString());
                        obj[5] = cs.procesarStringDeUI(r["CedulaB"].ToString());
                        obj[6] = cs.procesarStringDeUI(r["TelefonoB"].ToString());
                        obj[7] = cs.procesarStringDeUI(r["CelularB"].ToString());
                        obj[8] = cs.procesarStringDeUI(r["OtroTelB"].ToString());
                        obj[9] = cs.procesarStringDeUI(r["CorreoB"].ToString());

                        obj[10] = cs.procesarStringDeUI(r["NombreE"].ToString());
                        obj[11] = cs.procesarStringDeUI(r["Apellido1E"].ToString());
                        obj[12] = cs.procesarStringDeUI(r["Apellido2E"].ToString());
                        obj[13] = cs.procesarStringDeUI(r["PuestoE"].ToString());
                        obj[14] = cs.procesarStringDeUI(r["CedulaE"].ToString());
                        obj[15] = cs.procesarStringDeUI(r["TelefonoE"].ToString());
                        obj[16] = cs.procesarStringDeUI(r["CelularE"].ToString());
                        obj[17] = cs.procesarStringDeUI(r["OtroTelE"].ToString());
                        obj[18] = cs.procesarStringDeUI(r["CorreoE"].ToString());

                        obj[19] = cs.procesarStringDeUI(r["TotalHorasA"].ToString());
            
                        lsObjectAux.Add(obj);

                    }
                    lsOject = lsObjectAux;
    
                } break;

            case 2:
                {
                    int periodo = 0;
                    switch (criterio1)
                    {
                        case "I   - Periodo":
                            {
                                periodo = 1;
                            } break;
                        case "II  - Periodo":
                            {
                                periodo = 2;
                            } break;
                        case "III - Periodo":
                            {
                                periodo = 3;
                            } break;
                    }

                    DataTable tbBecarios = new DataTable();
                    switch (criterio3)
                    {
                        case "Hace más de 3 periodos":
                            {
                                int periodoAnterior;
                                int añoAnterior;
                                int periodoTrasAnterior;
                                int añoTrasAnterior;
                                int periodoTrasTrasAnterior;
                                int añoTrasTrasAnterior;
                                if (periodo == 1)
                                {
                                    periodoAnterior = 3;
                                    periodoTrasAnterior = 2;
                                    periodoTrasTrasAnterior = 1;
                                    añoAnterior = Convert.ToInt32(criterio2) - 1;
                                    añoTrasAnterior = Convert.ToInt32(criterio2) - 1;
                                    añoTrasTrasAnterior = Convert.ToInt32(criterio2) - 1;
                                }
                                else
                                {
                                    if (periodo == 2)
                                    {
                                        periodoAnterior = 1;
                                        periodoTrasAnterior = 3;
                                        periodoTrasTrasAnterior = 2;
                                        añoAnterior = Convert.ToInt32(criterio2);
                                        añoTrasAnterior = Convert.ToInt32(criterio2) - 1;
                                        añoTrasTrasAnterior = Convert.ToInt32(criterio2) - 1;
                                    }
                                    else
                                    {
                                        periodoAnterior = 2;
                                        periodoTrasAnterior = 1;
                                        periodoTrasTrasAnterior = 3;
                                        añoAnterior = Convert.ToInt32(criterio2);
                                        añoTrasAnterior = Convert.ToInt32(criterio2);
                                        añoTrasTrasAnterior = Convert.ToInt32(criterio2)-1;
                                    }
                                }

                                tbBecarios = controladoraBDReportes.reportarBecariosNoAsignados3(criterioBusquedaGeneral, periodo, criterio2, añoAnterior, periodoAnterior, añoTrasAnterior, periodoTrasAnterior, añoTrasTrasAnterior, periodoTrasTrasAnterior);
                            } break;
                        case "Hace 2 periodos":
                            {
                                int periodoAnterior;
                                int añoAnterior;
                                int periodoTrasAnterior;
                                int añoTrasAnterior;
                                if (periodo == 1)
                                {
                                    periodoAnterior = 3;
                                    periodoTrasAnterior = 2;
                                    añoAnterior = Convert.ToInt32(criterio2) - 1;
                                    añoTrasAnterior = Convert.ToInt32(criterio2) - 1;
                                }
                                else
                                {
                                    if (periodo == 2)
                                    {
                                        periodoAnterior = 1;
                                        periodoTrasAnterior = 3;
                                        añoAnterior = Convert.ToInt32(criterio2);
                                        añoTrasAnterior = Convert.ToInt32(criterio2) - 1;
                                    }
                                    else
                                    {
                                        periodoAnterior = 2;
                                        periodoTrasAnterior = 1;
                                        añoAnterior = Convert.ToInt32(criterio2);
                                        añoTrasAnterior = Convert.ToInt32(criterio2);
                                    }
                                }

                                tbBecarios = controladoraBDReportes.reportarBecariosNoAsignados2(criterioBusquedaGeneral, periodo, criterio2, añoAnterior, periodoAnterior, añoTrasAnterior, periodoTrasAnterior);
                            } break;
                        case "Periodo anterior":
                            {
                                int periodoAnterior;
                                int añoAnterior;
                                if (periodo == 1)
                                {
                                    periodoAnterior = 3;
                                    añoAnterior = Convert.ToInt32(criterio2) - 1;
                                }
                                else {
                                    periodoAnterior = periodo - 1;
                                    añoAnterior = Convert.ToInt32(criterio2);
                                }
                                tbBecarios = controladoraBDReportes.reportarBecariosNoAsignados(criterioBusquedaGeneral, periodo, criterio2, añoAnterior, periodoAnterior);
                            } break;

                        case "Becarios nunca Asignados":
                            {
                                tbBecarios = controladoraBDReportes.reportarBecariosNoAsignados4(criterioBusquedaGeneral);
                            } break;
                    }

                    List<object[]> lsObjectAux = new List<object[]>();


                    foreach (DataRow r in tbBecarios.Rows)
                    {

                        Object[] obj = new Object[10];

                        obj[0] = cs.procesarStringDeUI(r["Foto"].ToString());
                        obj[1] = cs.procesarStringDeUI(r["Nombre"].ToString());
                        obj[2] = cs.procesarStringDeUI(r["Apellido1"].ToString());
                        obj[3] = cs.procesarStringDeUI(r["Apellido2"].ToString());
                        obj[4] = cs.procesarStringDeUI(r["Carne"].ToString());
                        obj[5] = cs.procesarStringDeUI(r["Cedula"].ToString());
                        obj[6] = cs.procesarStringDeUI(r["Telefono"].ToString());
                        obj[7] = cs.procesarStringDeUI(r["Celular"].ToString());
                        obj[8] = cs.procesarStringDeUI(r["OtroTel"].ToString());
                        obj[9] = cs.procesarStringDeUI(r["Correo"].ToString());                       

                        lsObjectAux.Add(obj);

                    }
                    lsOject = lsObjectAux;
                } break;
            case 3:
                {
                    int periodo = 0;
                    switch (criterio1)
                    {
                        case "I   - Periodo":
                            {
                                periodo = 1;
                            } break;
                        case "II  - Periodo":
                            {
                                periodo = 2;
                            } break;
                        case "III - Periodo":
                            {
                                periodo = 3;
                            } break;
                    }

                    DataTable tbBecarios = new DataTable();
                    tbBecarios = controladoraBDReportes.reportarBecariosPorUnidadAcademica(criterioBusquedaGeneral, periodo, criterio2, criterio3);
                    List<object[]> lsObjectAux = new List<object[]>();


                    foreach (DataRow r in tbBecarios.Rows)
                    {

                        Object[] obj = new Object[20];

                        obj[0] = cs.procesarStringDeUI(r["FotoB"].ToString());
                        obj[1] = cs.procesarStringDeUI(r["NombreB"].ToString());
                        obj[2] = cs.procesarStringDeUI(r["Apellido1B"].ToString());
                        obj[3] = cs.procesarStringDeUI(r["Apellido2B"].ToString());
                        obj[4] = cs.procesarStringDeUI(r["CarneB"].ToString());
                        obj[5] = cs.procesarStringDeUI(r["CedulaB"].ToString());
                        obj[6] = cs.procesarStringDeUI(r["TelefonoB"].ToString());
                        obj[7] = cs.procesarStringDeUI(r["CelularB"].ToString());
                        obj[8] = cs.procesarStringDeUI(r["OtroTelB"].ToString());
                        obj[9] = cs.procesarStringDeUI(r["CorreoB"].ToString());

                        obj[10] = cs.procesarStringDeUI(r["NombreE"].ToString());
                        obj[11] = cs.procesarStringDeUI(r["Apellido1E"].ToString());
                        obj[12] = cs.procesarStringDeUI(r["Apellido2E"].ToString());
                        obj[13] = cs.procesarStringDeUI(r["PuestoE"].ToString());
                        obj[14] = cs.procesarStringDeUI(r["CedulaE"].ToString());
                        obj[15] = cs.procesarStringDeUI(r["TelefonoE"].ToString());
                        obj[16] = cs.procesarStringDeUI(r["CelularE"].ToString());
                        obj[17] = cs.procesarStringDeUI(r["OtroTelE"].ToString());
                        obj[18] = cs.procesarStringDeUI(r["CorreoE"].ToString());

                        obj[19] = cs.procesarStringDeUI(r["TotalHorasA"].ToString());

                        lsObjectAux.Add(obj);

                    }
                    lsOject = lsObjectAux;
                } break;
        }

        return lsOject;
    }

    public List<string> optenerUnidadesAcademicas()
    {
        List<string> lsSiglasUA = new List<string>();
        DataTable tbSiglasUA = controladoraBDReportes.optenerUnidadesAcademicas();
        foreach (DataRow r in tbSiglasUA.Rows)
        {
            string sigla = cs.procesarStringDeUI(r.ItemArray[1].ToString());
            lsSiglasUA.Add(sigla);
        }
        return lsSiglasUA;
    }

    public List<EncargadoAtrasado> llenarEncargadosAtrasados(string criterioBusqueda, int año, int semestre, DateTime fechaUltimoReporte)
    {
        List<EncargadoAtrasado> lista = new List<EncargadoAtrasado>();
        DataTable dt = controladoraBDReportes.llenarEncargadosAtrasados(criterioBusqueda, año, semestre, fechaUltimoReporte);
        foreach (DataRow r in dt.Rows)
        {
            EncargadoAtrasado encargado = new EncargadoAtrasado();
            encargado.Nombre = r["Nombre"].ToString();
            encargado.Apellido1 = r["Apellido1"].ToString();
            encargado.Apellido2 = r["Apellido2"].ToString();
            encargado.Cedula = r["Cedula"].ToString();
            encargado.CantBecariosAsignados = (int)r["CantidadBecariosAsignados"];

            DateTime dateFechaUltimoIngreso = (DateTime)r["FechaUltimoIngreso"];
            encargado.FechaUltimaActividad = dateFechaUltimoIngreso.ToString("U");

            lista.Add(encargado);
        }
        return lista;
    }

    public List<Object[]> reportarHistorialDeAsignacionesBecario(string criterioBusqueda, string cedula) 
    {
        List<Object[]> lsObject = new List<Object[]>();
        DataTable dt = controladoraBDReportes.reportarHistorialDeAsignacionesBecario(criterioBusqueda, cedula) ;
        foreach (DataRow r in dt.Rows)
        {
            Object[] obj = new Object[23];

            obj[0] = cs.procesarStringDeUI(r["FotoB"].ToString());
            obj[1] = cs.procesarStringDeUI(r["NombreB"].ToString());
            obj[2] = cs.procesarStringDeUI(r["Apellido1B"].ToString());
            obj[3] = cs.procesarStringDeUI(r["Apellido2B"].ToString());
            obj[4] = cs.procesarStringDeUI(r["CarneB"].ToString());
            obj[5] = cs.procesarStringDeUI(r["CedulaB"].ToString());
            obj[6] = cs.procesarStringDeUI(r["TelefonoB"].ToString());
            obj[7] = cs.procesarStringDeUI(r["CelularB"].ToString());
            obj[8] = cs.procesarStringDeUI(r["OtroTelB"].ToString());
            obj[9] = cs.procesarStringDeUI(r["CorreoB"].ToString());

            obj[10] = cs.procesarStringDeUI(r["NombreE"].ToString());
            obj[11] = cs.procesarStringDeUI(r["Apellido1E"].ToString());
            obj[12] = cs.procesarStringDeUI(r["Apellido2E"].ToString());
            obj[13] = cs.procesarStringDeUI(r["PuestoE"].ToString());
            obj[14] = cs.procesarStringDeUI(r["CedulaE"].ToString());
            obj[15] = cs.procesarStringDeUI(r["TelefonoE"].ToString());
            obj[16] = cs.procesarStringDeUI(r["CelularE"].ToString());
            obj[17] = cs.procesarStringDeUI(r["OtroTelE"].ToString());
            obj[18] = cs.procesarStringDeUI(r["CorreoE"].ToString());

            obj[19] = cs.procesarStringDeUI(r["TotalHorasA"].ToString());
            obj[20]= cs.procesarStringDeUI(r["Periodo"].ToString());
            obj[21] = cs.procesarStringDeUI(r["Año"].ToString());
            obj[22] = cs.procesarStringDeUI(r["ComentarioDeEncargado"].ToString());


            lsObject.Add(obj);
        }
        return lsObject;
    }

    public List<Object[]> reportarHistorialDeAnotacionesEncargado(string criterioBusqueda, string cedula)
    {
        List<Object[]> lsObject = new List<Object[]>();
        DataTable dt = controladoraBDReportes.reportarHistorialDeAnotacionesEncargado(criterioBusqueda, cedula);
        foreach (DataRow r in dt.Rows)
        {
            Object[] obj = new Object[23];

            obj[0] = cs.procesarStringDeUI(r["FotoB"].ToString());
            obj[1] = cs.procesarStringDeUI(r["NombreB"].ToString());
            obj[2] = cs.procesarStringDeUI(r["Apellido1B"].ToString());
            obj[3] = cs.procesarStringDeUI(r["Apellido2B"].ToString());
            obj[4] = cs.procesarStringDeUI(r["CarneB"].ToString());
            obj[5] = cs.procesarStringDeUI(r["CedulaB"].ToString());
            obj[6] = cs.procesarStringDeUI(r["TelefonoB"].ToString());
            obj[7] = cs.procesarStringDeUI(r["CelularB"].ToString());
            obj[8] = cs.procesarStringDeUI(r["OtroTelB"].ToString());
            obj[9] = cs.procesarStringDeUI(r["CorreoB"].ToString());

            obj[10] = cs.procesarStringDeUI(r["NombreE"].ToString());
            obj[11] = cs.procesarStringDeUI(r["Apellido1E"].ToString());
            obj[12] = cs.procesarStringDeUI(r["Apellido2E"].ToString());
            obj[13] = cs.procesarStringDeUI(r["PuestoE"].ToString());
            obj[14] = cs.procesarStringDeUI(r["CedulaE"].ToString());
            obj[15] = cs.procesarStringDeUI(r["TelefonoE"].ToString());
            obj[16] = cs.procesarStringDeUI(r["CelularE"].ToString());
            obj[17] = cs.procesarStringDeUI(r["OtroTelE"].ToString());
            obj[18] = cs.procesarStringDeUI(r["CorreoE"].ToString());

            obj[19] = cs.procesarStringDeUI(r["TotalHorasA"].ToString());
            obj[20] = cs.procesarStringDeUI(r["Periodo"].ToString());
            obj[21] = cs.procesarStringDeUI(r["Año"].ToString());
            obj[22] = cs.procesarStringDeUI(r["ComentarioDeBecario"].ToString());


            lsObject.Add(obj);
        }
        return lsObject;
    }

    public List<BecarioInactivo> llenarBecariosInactivos(string criterioBusqueda, int año, int periodo, DateTime fechaUltimoReporte)
    {
        List<BecarioInactivo> lista = new List<BecarioInactivo>();
        DataTable dt = controladoraBDReportes.llenarBecariosInactivos(criterioBusqueda, año, periodo, fechaUltimoReporte);
        foreach (DataRow r in dt.Rows)
        {
            BecarioInactivo becario = new BecarioInactivo();
            becario.Nombre = r["Nombre"].ToString();
            becario.Apellido1 = r["Apellido1"].ToString();
            becario.Apellido2 = r["Apellido2"].ToString();
            becario.Cedula = r["Cedula"].ToString();
            becario.HorasAsignadas = (int)r["HorasAsignadas"];
            becario.HorasCompletadas = (int)r["HorasCompletadas"];
            becario.Encargado = r["Encargado"].ToString();

            DateTime dateFechaUltimoIngreso = (DateTime)r["FechaUltimoIngreso"];
            becario.FechaUltimoReporte = dateFechaUltimoIngreso.ToString("U");

            lista.Add(becario);
        }
        return lista;
    }
}