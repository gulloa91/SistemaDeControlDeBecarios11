using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class Cuentas
{
    private String nombreUsuario;
    private String contrasenna;
    Nullable<DateTime> ultimoAcceso;
    private String cedulaUsuario;
    CommonServices cs;

    public Cuentas(Object [] datos) {
        cs = new CommonServices(null);
        this.nombreUsuario = cs.procesarStringDeUI(datos[0].ToString());
        this.contrasenna = cs.procesarStringDeUI(datos[1].ToString());
        if (datos[2].ToString() != String.Empty)
        {
            this.ultimoAcceso = Convert.ToDateTime(datos[2].ToString());
        }
        else
        {
            this.ultimoAcceso = null;
        }
        this.cedulaUsuario = cs.procesarStringDeUI(datos[3].ToString());
    }

    public String NombreUsuario
    {
        get { return nombreUsuario; }
        set { nombreUsuario = value; }
    }

    public String Contrasenna
    {
        get { return contrasenna; }
        set { contrasenna = value; }
    }

    public Nullable<DateTime> UltimoAcceso
    {
        get { return ultimoAcceso; }
        set { ultimoAcceso = value; }
    }

    public String CedulaUsuario
    {
        get { return cedulaUsuario; }
        set { cedulaUsuario = value; }
    }
}