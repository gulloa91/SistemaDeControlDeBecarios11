using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
//using System.Web.UI.WebControls;
using System.Data;
using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;

/// <summary>
/// Descripción breve de GeneradorPDF
/// </summary>
public class GeneradorPDF
{
    private String Receptor;
    private String Emisor;
    private String Iniciales;
    private int cntHoras;
    private int Ciclo;
    private String Semestre;
    private int Anno;
    private String UnidadAcademica;
    protected BaseFont basePie = BaseFont.CreateFont(BaseFont.TIMES_ITALIC, BaseFont.WINANSI, BaseFont.NOT_EMBEDDED);
    AsignacionesDataSetTableAdapters.AsignadoA1TableAdapter adapterAs = new AsignacionesDataSetTableAdapters.AsignadoA1TableAdapter();
    ControladoraBecarios cb = new ControladoraBecarios();

    public GeneradorPDF() { }

    public GeneradorPDF(string receptor, string emisor, string iniciales, int cntHoras, int ciclo, string semestre, int anno, string unidadAcadémica)
	{
        this.Receptor = receptor;
        this.Emisor = emisor;
        this.Iniciales = iniciales;
        this.cntHoras = cntHoras;
        this.Ciclo = ciclo;
        this.Semestre = semestre;
        this.Anno = anno;
        this.UnidadAcademica = unidadAcadémica;
	}

    public void generarInforme(){
        Document document = new Document(PageSize.LETTER);
        PdfWriter writer = PdfWriter.GetInstance(document, new FileStream("C:/Users/ryuzaki1792/Desktop/machote.PDF", FileMode.OpenOrCreate));
        document.Open();
        crearEncabezado(document, writer);
        document.Add(new Paragraph(" "));
        Paragraph p = new Paragraph();
        p.Alignment = Element.ALIGN_CENTER;
        p.Font = FontFactory.GetFont("Arial", 13);
        p.Add(DateTime.Now.ToString("dd-MM-yyyy")+"\n");
        p.Font.SetStyle(iTextSharp.text.Font.BOLD);
        p.Add("ECCI-"+this.Iniciales+"-"+DateTime.Now.Year.ToString());
        document.Add(p);
        //dejo un espaciado de 3 renglones
        document.Add(new Paragraph(" "));
        document.Add(new Paragraph(" "));
        document.Add(new Paragraph(" "));
        p.Clear();
        p.Alignment = Element.ALIGN_JUSTIFIED;
        p.Font = FontFactory.GetFont("Arial",12);
        p.Font.SetStyle(iTextSharp.text.Font.NORMAL);
        p.Add("Señor(a)\n"+this.Receptor+"\nEncargado Becario 11\nOficina de Becas");
        document.Add(p);
        // dejo un espaciado de un renglon
        document.Add(new Paragraph(" "));
        p.Clear();
        p.Add("Estimado señor(a):");
        document.Add(p);
        // dejo un espaciado de un renglon
        document.Add(new Paragraph(" "));
        p.Clear();
        p.Add("Me permito informarle que los siguientes estudiantes cumplieron con sus "+this.cntHoras.ToString()+" horas becario 11 en el "+this.Semestre+" semestre del "+this.Anno.ToString()+" en esta Unidad Académica.");
        document.Add(p);
        // dejo un espaciado de un renglon
        document.Add(new Paragraph(" "));
        DataTable dt = adapterAs.obtenerBecariosFinalizados(this.cntHoras,this.Ciclo,this.Anno);
        if (dt.Rows.Count > 0)
        {
            if (dt.Rows.Count <= 12)
            {
                PdfPTable tablaBecarios = retornarTabla();
                foreach (DataRow r in dt.Rows)
                {
                    PdfPCell[] datos = new PdfPCell[2];
                    Becario becario = cb.obtenerBecarioPorCedula(r[0].ToString());
                    datos[0] = new PdfPCell(new Paragraph((becario.nombre + " " + becario.apellido1 + " " + becario.apellido2), FontFactory.GetFont("Arial", 10)));
                    datos[1] = new PdfPCell(new Paragraph((becario.carne), FontFactory.GetFont("Arial", 10)));
                    PdfPRow fila = new PdfPRow(datos);
                    tablaBecarios.Rows.Add(fila);
                }
                centrarDatos(tablaBecarios);
                document.Add(tablaBecarios);
            }
            else {
                int cantPaginas = dt.Rows.Count - 12;
                int auxiliar = 0;
                PdfPTable tabla1 = retornarTabla();
                for (int i = 0; i < 12;++i )
                {
                    PdfPCell[] datos = new PdfPCell[2];
                    Becario becario = cb.obtenerBecarioPorCedula(dt.Rows[auxiliar][0].ToString());
                    datos[0] = new PdfPCell(new Paragraph((becario.nombre + " " + becario.apellido1 + " " + becario.apellido2), FontFactory.GetFont("Arial", 10)));
                    datos[1] = new PdfPCell(new Paragraph((becario.carne), FontFactory.GetFont("Arial", 10)));
                    PdfPRow fila = new PdfPRow(datos);
                    tabla1.Rows.Add(fila);
                    auxiliar++;
                }
                centrarDatos(tabla1);
                Paragraph p1 = new Paragraph();
                p1.Add(tabla1);
                document.Add(p1);
                crearPieDePagina(document, writer);

                int resto = cantPaginas % 24;
                cantPaginas = (int)(cantPaginas/24);
                for (int i = 0; i < cantPaginas;++i )
                {
                    //otras hojas
                    PdfPTable tabla2 = retornarTabla();
                    crearEncabezado(document, writer);
                    for (int j = 0; j < 24; ++j )
                    {
                        PdfPCell[] datos = new PdfPCell[2];
                        Becario becario = cb.obtenerBecarioPorCedula(dt.Rows[auxiliar][0].ToString());
                        datos[0] = new PdfPCell(new Paragraph((becario.nombre + " " + becario.apellido1 + " " + becario.apellido2), FontFactory.GetFont("Arial", 10)));
                        datos[1] = new PdfPCell(new Paragraph((becario.carne), FontFactory.GetFont("Arial", 10)));
                        PdfPRow fila = new PdfPRow(datos);
                        tabla2.Rows.Add(fila);
                        auxiliar++;
                    }
                    centrarDatos(tabla2);
                    Paragraph p2 = new Paragraph();
                    p2.Add("\n\n");
                    p2.Add(tabla2);
                    document.Add(p2);
                    crearPieDePagina(document, writer);

                }
                if (resto > 0)
                {
                    PdfPTable tabla3 = retornarTabla();
                    crearEncabezado(document, writer);
                    for (int i = 0; i < resto; ++i)
                    {
                        PdfPCell[] datos = new PdfPCell[2];
                        Becario becario = cb.obtenerBecarioPorCedula(dt.Rows[auxiliar][0].ToString());
                        datos[0] = new PdfPCell(new Paragraph((becario.nombre + " " + becario.apellido1 + " " + becario.apellido2), FontFactory.GetFont("Arial", 10)));
                        datos[1] = new PdfPCell(new Paragraph((becario.carne), FontFactory.GetFont("Arial", 10)));
                        PdfPRow fila = new PdfPRow(datos);
                        tabla3.Rows.Add(fila);
                        auxiliar++;
                    }
                    centrarDatos(tabla3);
                    Paragraph p3 = new Paragraph();
                    p3.Add("\n\n");
                    p3.Add(tabla3);
                    document.Add(p3);
                }
                else {
                    crearEncabezado(document, writer);
                    Paragraph p3 = new Paragraph();
                    p3.Add("\n\n");
                    document.Add(p3);
                }
            }
        }
        else { 
           // no hay becarios
        }
        crearFirma(document);
        crearPieDePagina(document, writer);
        document.Close();
    }

    public PdfPTable retornarTabla() {
        PdfPTable tablaBecarios = new PdfPTable(2);
        tablaBecarios.SetWidthPercentage(new float[] { 200, 200 }, PageSize.LETTER);
        tablaBecarios.AddCell(new Paragraph("Nombre"));
        tablaBecarios.AddCell(new Paragraph("Carnet"));
        //para darle formato a las columnas del nombre y carnet
        foreach (PdfPCell celda in tablaBecarios.Rows[0].GetCells())
        {
            celda.BackgroundColor = BaseColor.LIGHT_GRAY;
            celda.HorizontalAlignment = 1;
            celda.Padding = 3;
            celda.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
        }
        return tablaBecarios;
    }

    public void centrarDatos(PdfPTable tablaBecarios) {
        foreach (PdfPRow fila in tablaBecarios.Rows)
        {
            foreach (PdfPCell celda in fila.GetCells())
            {
                celda.HorizontalAlignment = 1;
                celda.Padding = 3;
                celda.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            }
        }
    }

    public void crearFirma(Document doc) {
        Paragraph p = new Paragraph();
        p.Add("Atentamente,\n\n\n");
        p.Add("Licda. Milena Zúñiga Cárdenas\n");
        p.Add("Jefe Administrativa");
        p.Font = FontFactory.GetFont(BaseFont.TIMES_ROMAN, 13);
        p.Font.SetStyle(iTextSharp.text.Font.NORMAL);
        p.Alignment = Element.ALIGN_CENTER;
        doc.Add(p);
    }

    public Image colocarImagenUCR(Document doc){
        Image imagen = Image.GetInstance("C:/Users/ryuzaki1792/Desktop/UCR-Escudo-Colores.png");
        imagen.Alignment = Image.TEXTWRAP;
        imagen.ScaleAbsolute(75f, 75f);
        imagen.SetAbsolutePosition(40, doc.PageSize.Height - 100);
        return imagen;
    }

    public Image colocarImagenECCI(Document doc)
    {
        Image imagen = Image.GetInstance("C:/Users/ryuzaki1792/Desktop/logoEcci.jpg");
        imagen.Alignment = Image.TEXTWRAP;
        imagen.ScaleAbsolute(75f, 75f);
        imagen.SetAbsolutePosition(doc.Right - 95, doc.PageSize.Height - 100);
        return imagen;
    }

    public Paragraph GenerarParrafo(String texto)
    {
        Paragraph parrafo = new Paragraph();

        parrafo.Alignment = Element.ALIGN_CENTER;
        parrafo.Font = FontFactory.GetFont(BaseFont.TIMES_ROMAN, 13);
        parrafo.Font.SetStyle(iTextSharp.text.Font.BOLD);

        parrafo.Add(texto);

        return parrafo;
    }

    public void crearEncabezado(Document doc, PdfWriter writer)
    {
        doc.NewPage();
        Image imagen = colocarImagenUCR(doc);
        imagen.Alignment = Image.TEXTWRAP | Image.LEFT_ALIGN;
        doc.Add(imagen);
        imagen = colocarImagenECCI(doc);
        imagen.Alignment = Image.TEXTWRAP | Image.RIGHT_ALIGN;
        doc.Add(imagen);
        Paragraph parrafo = new Paragraph();
        parrafo = GenerarParrafo("Universidad de Costa Rica\nFacultad de Ingeniería\nEscuela de Ciencias de la Computación e\nInformática");
        parrafo.Font.SetColor(0, 0, 255);
        doc.Add(parrafo);
        PdfContentByte cb = writer.DirectContent;
        cb.MoveTo(40, doc.PageSize.Height - 120);
        cb.LineTo(doc.PageSize.Width-40, doc.PageSize.Height - 120);
        cb.Stroke();
    }

    private Image colocarImagenAcreditacion(Document doc) {
        Image imagen = Image.GetInstance("C:/Users/ryuzaki1792/Desktop/acreditacion.png");
        imagen.Alignment = Image.TEXTWRAP;
        imagen.ScaleAbsolute(75f, 75f);
        imagen.SetAbsolutePosition((float)(doc.PageSize.Width / 2),40);
        return imagen;
    }

    public void crearPieDePagina(Document doc, PdfWriter writer) {
        BaseFont bf = basePie;
        PdfContentByte cb = writer.DirectContent;
        cb.MoveTo(40, 120);
        cb.LineTo(doc.PageSize.Width - 40, 120);
        cb.Stroke();
        cb.BeginText();
        cb.SetFontAndSize(bf, 10);
        cb.SetTextMatrix(doc.Left, 80);
        cb.ShowText("Teléfono: (506) 2511-8000");
        cb.SetTextMatrix(doc.Left, 68);
        cb.ShowText("http://www.ecci.ucr.ac.cr");
        cb.SetTextMatrix(doc.Right-95, 80);
        cb.ShowText("Fax: (506) 2511-5527");
        cb.EndText();
        Image imagen = colocarImagenAcreditacion(doc);
        imagen.Alignment = Image.TEXTWRAP | Image.ALIGN_CENTER;
        doc.Add(imagen);
    }

}