using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for Encargado
/// </summary>
public class Encargado
{
    private string nombre;   
    private string apellido1;
    private string apellido2;
    private string cedula;
    private string puesto;
    private string correo;
    private string telefonoFijo;
    private string telefonoCelular;
    private string otroTelefono;
    private bool activo;
    
    public Encargado(Object[] datos)
	{
        crearEncargado(datos);
	}


    public Encargado()
    {
        this.activo = true;
    }

    public void crearEncargado(Object[] datos)
    {
        this.cedula = datos[0].ToString();
        this.nombre = datos[1].ToString();
        this.apellido1 = datos[2].ToString();
        this.apellido2 = datos[3].ToString();        
        this.correo = datos[4].ToString(); 
        this.telefonoFijo = datos[5].ToString();
        this.telefonoCelular = datos[6].ToString();
        this.otroTelefono = datos[7].ToString();
        this.puesto = datos[8].ToString();
        this.activo = true;
    }

    public string Nombre
    {
        get { return this.nombre; }
        set { nombre = value; }
    }

    public string Apellido1
    {
        get { return this.apellido1; }
        set { apellido1 = value; }
    }

    public string Apellido2
    {
        get { return this.apellido2; }
        set { apellido2 = value; }
    }

    public string Cedula
    {
        get { return this.cedula; }
        set { cedula = value; }
    }

    public string Puesto
    {
        get { return this.puesto; }
        set { puesto = value; }
    }

    public string Correo
    {
        get { return this.correo; }
        set { correo = value; }
    }

    public string TelefonoFijo
    {
        get { return this.telefonoFijo; }
        set { telefonoFijo = value; }
    }

    public string TelefonoCelular
    {
        get { return this.telefonoCelular; }
        set { telefonoCelular = value; }
    }

    public string OtroTelefono
    {
        get { return this.otroTelefono; }
        set { otroTelefono = value; }
    }

    public bool Activo
    {
        get { return this.activo; }
        set { activo= value; }
    }
}