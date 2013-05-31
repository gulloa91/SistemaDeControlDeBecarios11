using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for Class1
/// </summary>
public class AreaInteres
{

    string nombreArea; 

	public AreaInteres()
	{

	}

    public AreaInteres(string n)
    {
        nombreArea = n;
    }

    public string NombreArea
    {
        get { return nombreArea; }
        set { nombreArea = value; }
    }

}