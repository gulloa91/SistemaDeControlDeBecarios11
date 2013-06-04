using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for Lenguaje
/// </summary>
public class Lenguaje
{


    private string nombreLenguaje;

	public Lenguaje()
	{
        
	}

    public Lenguaje(string n)
    {
        nombreLenguaje = n;
    }

    public string NombreLenguaje
    {
        get { return nombreLenguaje; }
        set { nombreLenguaje = value; }
    }


}