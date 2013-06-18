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
    private String periodo;
    private String año;  
    private String totalHoras;
    private String siglasUA;
    private String infoUbicacion;
    private String estado;


    public Asignacion()
    {

    }

	public Asignacion(Object[] datos)
	{

        this.cedulaBecario = datos[0].ToString();
        this.cedulaEncargado = datos[1].ToString();
        this.periodo = datos[2].ToString();
        this.año = datos[3].ToString();
        this.totalHoras = datos[4].ToString();
        this.siglasUA = datos[5].ToString();
        this.infoUbicacion = datos[6].ToString();
        this.estado = datos[7].ToString();
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

    public String Periodo
    {
        get { return periodo; }
        set { periodo = value; }
    }

    public String Año
    {
        get { return año; }
        set { año = value; }
    }


    public String TotalHoras
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

    public String Estado
    {
        get { return estado; }
        set { estado = value; }
    }


}