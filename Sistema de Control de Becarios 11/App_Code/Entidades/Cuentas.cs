using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class Cuentas
{
    /*
     * Atributos correspondientes a la entidad de cuenta
     */ 
    private String nombreUsuario;
    private String contrasenna;
    Nullable<DateTime> ultimoAcceso;
    private String cedulaUsuario;
    CommonServices cs;

    // Constructor para inicializar las variables
    public Cuentas(Object [] datos) {
        cs = new CommonServices(null);
        this.nombreUsuario = cs.procesarStringDeUI(datos[0].ToString());
        this.contrasenna = cs.procesarStringDeUI(datos[1].ToString());
        this.ultimoAcceso = (Nullable<DateTime>) datos[2];
        this.cedulaUsuario = cs.procesarStringDeUI(datos[3].ToString());
    }


    /*
     * Metodos set y get para acceder a los datos desde otras clases
     */
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