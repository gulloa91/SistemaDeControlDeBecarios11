using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Descripción breve de ControlDeHoras
/// </summary>
public class ControlDeHoras
{
    private string CedulaEncargado;
    private string CedulaBecario;
    private int Estado;
    private int CantidadHoras;
    private DateTime Fecha;
    private string ComentarioEncargado;
    private string ComentarioBecario;
    private int Periodo;
    private int Año;

    public ControlDeHoras(Object[] datos)
    {
        this.CedulaEncargado = datos[0].ToString();
        this.CedulaBecario = datos[1].ToString();
        this.Estado = Convert.ToInt32(datos[2].ToString());
        this.CantidadHoras = Convert.ToInt32(datos[3].ToString());
        this.Fecha = Convert.ToDateTime(datos[4].ToString());
        this.ComentarioEncargado = datos[5].ToString();
        this.ComentarioBecario = datos[6].ToString();
        this.Periodo = Convert.ToInt32(datos[7].ToString());
        this.Año = Convert.ToInt32(datos[8].ToString());
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

    public int estado
    {
        set { Estado = value; }
        get { return Estado; }
    }

    public int cantidadHoras
    {
        set { CantidadHoras = value; }
        get { return CantidadHoras; }
    }

    public DateTime fecha
    {
        set { Fecha = value; }
        get { return Fecha; }
    }

    public String comentarioEncargado
    {
        set { ComentarioEncargado = value; }
        get { return ComentarioEncargado; }
    }

    public String comentarioBecario
    {
        set { ComentarioBecario = value; }
        get { return ComentarioBecario; }
    }

    public int periodo
    {
        set { Periodo = value; }
        get { return Periodo; }
    }

    public int año
    {
        set { Año = value; }
        get { return Año; }
    }

}