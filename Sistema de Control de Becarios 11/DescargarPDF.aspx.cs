using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class DescargarPDF : System.Web.UI.Page
{
	ControladoraBDReportes controladoraBDReportes = new ControladoraBDReportes();

	protected void Page_Load(object sender, EventArgs e)
	{
		int tipoDescarga = Int32.Parse(Request.QueryString["tipo"]);
		string nombreArchivo = "";
		string ruta = "Aplicaciones\\inetpub\\wwwroot\\gsg2\\PDFs\\";
		switch (tipoDescarga)
		{
			case 1:
				{
					string destinatario = Request.QueryString["destinatario"];
					string remitente = Request.QueryString["remitente"];
					string iniciales = Request.QueryString["iniciales"];
					int cantHoras = Int32.Parse(Request.QueryString["cantHoras"]);
					int ciclo = Int32.Parse(Request.QueryString["ciclo"]);
					string periodo = Request.QueryString["periodo"];
					int año = Int32.Parse(Request.QueryString["año"]);

					GeneradorPDF generadorPDF = new GeneradorPDF(ruta, destinatario, remitente, iniciales, cantHoras, ciclo, periodo, año);
					nombreArchivo = generadorPDF.generarInforme();
				}
				break;
			case 2:
				{
					string cedula = Request.QueryString["cedula"];
					string criterioBusqueda = Request.QueryString["criterioBusqueda"];
					GeneradorPDF generadorPDF = new GeneradorPDF();
					DataTable dt = controladoraBDReportes.reportarHistorialDeAsignacionesBecario(criterioBusqueda, cedula, (cedula == "nada" ? 1 : 0));
					nombreArchivo = generadorPDF.generarReporteAsignacionesBecarioEncargado(ruta + "\\PDFs\\" , 0, dt);
				}
				break;
			case 3:
				{
					string cedula = Request.QueryString["cedula"];
					string criterioBusqueda = Request.QueryString["criterioBusqueda"];
					GeneradorPDF generadorPDF = new GeneradorPDF();
					DataTable dt = controladoraBDReportes.reportarHistorialDeAsignacionesEncargado(criterioBusqueda, cedula, (cedula == "nada" ? 1 : 0));
					nombreArchivo = generadorPDF.generarReporteAsignacionesBecarioEncargado(ruta + "\\PDFs\\", 1, dt);
				}
				break;
		}

		HttpResponse respuesta = HttpContext.Current.Response;
		respuesta.ClearContent();
		respuesta.Clear();
		respuesta.ContentType = "Application/pdf";
		respuesta.AddHeader("Content-Disposition", "attachment; filename=" + nombreArchivo + ";");
		respuesta.TransmitFile(ruta+ "\\PDFs\\" + nombreArchivo);
		respuesta.Flush();

		System.IO.File.Delete(ruta + "\\Pdfs\\" + nombreArchivo);

		respuesta.End();
	}
}