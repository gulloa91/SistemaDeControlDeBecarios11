using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for Perfil
/// </summary>
public class Perfil
{
    private String nombre;
    private int idPermiso;
    private int tipo;
    CommonServices cs;
    public Perfil(String nom,String per,String tipo)
    {
        cs = new CommonServices(null);
        this.Nombre = cs.procesarStringDeUI(nom);
        this.IDPermiso = Convert.ToInt32(per);
        this.Tipo = Convert.ToInt32(tipo);
    }

    public String Nombre
    {
        set { this.nombre = value; }
        get { return this.nombre; }
    }

    public int IDPermiso
    {
        set { this.idPermiso = value; }
        get { return this.idPermiso; }
    }

    public int Tipo
    {
        set { this.tipo = value; }
        get { return this.tipo; }
    }
}