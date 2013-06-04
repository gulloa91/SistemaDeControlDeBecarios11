using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for Becario
/// </summary>
public class Becario
{
    private String Foto;
    private String Nombre;
    private String Apellido1;
    private String Apellido2;
    private String Carne;
    private String Cedula;
    private String TelefonoFijo;
    private String TelefonoCelular;
    private String TelefonoOtro;
    private String Correo;
    private bool Activo;

    public Becario(Object[] datos)
    {
        this.Foto = datos[0].ToString();
        this.Nombre = datos[1].ToString();
        this.Apellido1 = datos[2].ToString();
        this.Apellido2 = datos[3].ToString();
        this.Carne = datos[4].ToString();
        this.Cedula = datos[5].ToString();
        this.TelefonoFijo = datos[6].ToString();
        this.TelefonoCelular = datos[7].ToString();
        this.TelefonoOtro = datos[8].ToString();
        this.Correo = datos[9].ToString();
        this.activo = true;
    }

    public Becario()
    {
        this.activo = true;
    }

    public String foto
    {
        get { return Foto; }
        set { Foto = value; }
    }

    public String nombre
    {
        get { return Nombre; }
        set { Nombre = value; }
    }

    public String apellido1
    {
        get { return Apellido1; }
        set { this.Apellido1 = value; }
    }


    public String apellido2
    {
        get { return Apellido2; }
        set { this.Apellido2 = value; }
    }

    public String carne
    {
        get { return Carne; }
        set { this.Carne = value; }
    }

    public String cedula
    {
        get { return Cedula; }
        set { this.Cedula = value; }
    }

    public String telefonoFijo
    {
        get { return TelefonoFijo; }
        set { this.TelefonoFijo = value; }
    }

    public String telefonoCelular
    {
        get { return TelefonoCelular; }
        set { this.TelefonoCelular = value; }
    }

    public String telefonoOtro
    {
        get { return TelefonoOtro; }
        set { this.TelefonoOtro = value; }
    }

    public String correo
    {
        get { return Correo; }
        set { this.Correo = value; }
    }

    public bool activo
    {
        get { return Activo; }
        set { Activo = value; }
    }

}