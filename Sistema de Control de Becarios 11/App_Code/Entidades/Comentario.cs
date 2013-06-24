using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Descripción breve de Comentario
/// </summary>
public class Comentario
{
    private String CedulaEncargado;
    private String CedulaBecario;
    private DateTime Fecha;
    private String Coment;
    public Comentario(Object[] datos)
    {
        this.CedulaEncargado = datos[0].ToString();
        this.CedulaBecario = datos[1].ToString();
        this.Fecha = Convert.ToDateTime(datos[2].ToString());
        this.Coment = datos[3].ToString();
    }

    public String cedulaEncargado
    {
        set { CedulaEncargado = value; }
        get { return CedulaEncargado; }
    }

    public String cedulaBecario
    {
        set { CedulaBecario = value; }
        get { return CedulaBecario; }
    }

    public DateTime fecha
    {
        set { Fecha = value; }
        get { return Fecha; }
    }

    public String comentario
    {
        set { Coment = value; }
        get { return Coment; }
    }
}