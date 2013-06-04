using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for Idioma
/// </summary>
public class Idioma
{

    private string nombreIdioma;

	public Idioma()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public Idioma(string n)
    {
        nombreIdioma = n;
    }

    public string NombreIdioma
    {
        get { return nombreIdioma; }
        set { nombreIdioma = value; }
    }

}