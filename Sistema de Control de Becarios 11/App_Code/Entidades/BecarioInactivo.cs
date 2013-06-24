using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class BecarioInactivo
{

	private string nombre;

	private string apellido1;

	private string apellido2;

	private string cedula;

	private int horasAsignadas;

	private int horasCompletadas;

	private string encargado;

	private string fechaUltimoReporte;

	public string Nombre {
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

	public int HorasAsignadas
	{
		get { return horasAsignadas; }
		set { horasAsignadas = value; }
	}

	public int HorasCompletadas
	{
		get { return horasCompletadas; }
		set { horasCompletadas = value; }
	}

	public string Encargado
	{
		get { return encargado; }
		set { encargado = value; }
	}

	public string FechaUltimoReporte
	{
		get { return fechaUltimoReporte; }
		set { fechaUltimoReporte = value; }
	}
}
