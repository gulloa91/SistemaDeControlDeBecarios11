using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class DescargarPDF : System.Web.UI.Page
{
	protected void Page_Load(object sender, EventArgs e)
	{
		string ruta = Server.MapPath("~");
		string destinatario = Request.QueryString["destinatario"];
		string remitente = Request.QueryString["remitente"];
		string iniciales = Request.QueryString["iniciales"];
		int cantHoras = Int32.Parse(Request.QueryString["cantHoras"]);
		int ciclo = Int32.Parse(Request.QueryString["ciclo"]);
		string periodo = Request.QueryString["periodo"];
		int año = Int32.Parse(Request.QueryString["año"]);

		GeneradorPDF generadorPDF = new GeneradorPDF(ruta, destinatario, remitente, iniciales, cantHoras, ciclo, periodo, año);
		string nombreArchivo = generadorPDF.generarInforme();

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