using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class EncargadoAtrasado
{
	private string nombre;
	private string apellido1;
	private string apellido2;
	private string cedula;
	private int cantBecariosAsignados;
	private string fechaUltimaActividad;

	public string Nombre
	{
		get { return nombre; }
		set { nombre = value; }
	}

	public string Apellido1
	{
		get { return apellido1; }
		set { apellido1 = value; }
	}

	public string Apellido2
	{
		get { return apellido2; }
		set { apellido2 = value; }
	}

	public string Cedula
	{
		get { return cedula; }
		set { cedula = value; }
	}

	public int CantBecariosAsignados
	{
		get { return cantBecariosAsignados; }
		set { cantBecariosAsignados = value; }
	}

	public string FechaUltimaActividad
	{
		get { return fechaUltimaActividad; }
		set { fechaUltimaActividad = value; }
	}
}
