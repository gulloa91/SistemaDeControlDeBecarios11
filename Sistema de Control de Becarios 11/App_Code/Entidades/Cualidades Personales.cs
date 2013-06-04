using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for Cualidades_Personales
/// </summary>
public class Cualidades_Personales
{

    string nombreCualidad; 

	public Cualidades_Personales()
	{
       
	}


    public Cualidades_Personales(string n)
    {
        nombreCualidad = n;
    }

    public string NombreCualidad
    {
        get { return nombreCualidad; }
        set { nombreCualidad = value; }
    }


}