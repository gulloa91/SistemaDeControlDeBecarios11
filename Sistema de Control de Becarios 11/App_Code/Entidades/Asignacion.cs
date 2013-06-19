using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for Asignacion
/// </summary>
public class Asignacion
{

    private String cedulaBecario;
    private String cedulaEncargado;
    private int periodo;
    private int año;  
    private int totalHoras;
    private String siglasUA;
    private String infoUbicacion;
    private int estado;


    public Asignacion()
    {

    }

	public Asignacion(Object[] datos)
	{

        this.cedulaBecario = datos[0].ToString();
        this.cedulaEncargado = datos[1].ToString();
        this.periodo = Convert.ToInt32( datos[2]);
        this.año = Convert.ToInt32(datos[3]);
        this.totalHoras = Convert.ToInt32(datos[4]);
        this.siglasUA = datos[5].ToString();
        this.infoUbicacion = datos[6].ToString();
        this.estado = Convert.ToInt32(datos[7]);
	}



    public String CedulaBecario
    {
        get { return cedulaBecario; }
        set { cedulaBecario = value; }
    }

    public String CedulaEncargado
    {
        get { return cedulaEncargado; }
        set { cedulaEncargado = value; }
    }

    public int Periodo
    {
        get { return periodo; }
        set { periodo = value; }
    }

    public int Año
    {
        get { return año; }
        set { año = value; }
    }


    public int TotalHoras
    {
        get { return totalHoras; }
        set { totalHoras = value; }
    }


    public String SiglasUA
    {
        get { return siglasUA; }
        set { siglasUA = value; }
    }

    public String InfoUbicacion
    {
        get { return infoUbicacion; }
        set { infoUbicacion = value; }
    }

    public int Estado
    {
        get { return estado; }
        set { estado = value; }
    }


}