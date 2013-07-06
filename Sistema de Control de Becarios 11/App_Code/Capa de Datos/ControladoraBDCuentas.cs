using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using DataSetCuentasTableAdapters;
using DataSetCuentasPerfilesQuitarTableAdapters;

public class ControladoraBDCuentas
{
    CuentaTableAdapter adapterCuentas;
    BecarioTableAdapter adapterBecario;
    EncargadoTableAdapter adapterEncargado;
    Cuenta_PerfilTableAdapter adapterCuenta_Perfil;
    DataTable1TableAdapter adapterNombreCuenta;
	public ControladoraBDCuentas()
    {
        adapterCuenta_Perfil = new Cuenta_PerfilTableAdapter();
        adapterCuentas = new CuentaTableAdapter();
        adapterBecario = new BecarioTableAdapter();
        adapterEncargado = new EncargadoTableAdapter();
        adapterNombreCuenta = new DataTable1TableAdapter();
	}

    public String insertarCuenta(Cuentas cuenta) {
        String returnValue = "";
        int r;
        try { 
           adapterCuentas.Insert(cuenta.NombreUsuario, cuenta.Contrasenna, cuenta.UltimoAcceso, cuenta.CedulaUsuario);
        }
        catch(SqlException e){
            r = e.Number;
            if (r == 2627)
            {
                returnValue = "Ya existe una cuenta con el nombre de usuario digitado";
            }
            else
            {
                returnValue = "Se ha producido un error al insertar la cuenta";
            }           
        }
        return returnValue;
    }

    public String modificarCuenta(Cuentas cuentaNueva, Cuentas cuentaVieja)
    {
        String returnValue = "";
        int r;
        try
        {
            adapterCuentas.Update(cuentaNueva.NombreUsuario, cuentaNueva.Contrasenna, cuentaNueva.UltimoAcceso, cuentaNueva.CedulaUsuario, cuentaVieja.NombreUsuario, cuentaVieja.Contrasenna, cuentaVieja.UltimoAcceso, cuentaVieja.CedulaUsuario);
        }
        catch (SqlException e)
        {
            r = e.Number;
            if (r == 2627)
            {
                returnValue = "Ya existe una cuenta con el nombre de usuario digitado";
            }
            else
            {
                returnValue = "Se ha producido un error al modificar la cuenta";
            }
        }
        return returnValue;
    }

    public String eliminarCuenta(Cuentas cuenta)
    {
        String returnValue = "Se ha eliminado correctamente la cuenta";
        try
        {
            adapterCuentas.Delete(cuenta.NombreUsuario, cuenta.Contrasenna, cuenta.UltimoAcceso, cuenta.CedulaUsuario);
        }
        catch (SqlException e)
        {
           returnValue = "Error al eliminar la cuenta";
        }
        return returnValue;
    }

    public Boolean validarUsuario(String nombreUsuario, String contrasenna) {
        Boolean resultado = false;
        int total = 0;
        try
        {
            total = (int)(adapterCuentas.validarUsuario(nombreUsuario, contrasenna));
        }
        catch(Exception e){
        
        }
        if(total>0){
            resultado = true;
        }
        return resultado;
    }


    public DataTable consultarCuentas() //cambiar el retorno
    {

        DataSetCuentas.CuentaDataTable dt = new DataSetCuentas.CuentaDataTable();
        dt = adapterCuentas.GetData();
        return dt;
    }

    public DataTable consultarPorNombreContr(String nombre, String contrasena) {
        DataSetCuentas.CuentaDataTable dt = new DataSetCuentas.CuentaDataTable();
        dt =  adapterCuentas.GetDataByNombreContr(nombre, contrasena);
        return dt;
    }

    public DataTable obtenerDatosCuenta(String nombre)
    {
        DataSetCuentas.CuentaDataTable dt = new DataSetCuentas.CuentaDataTable();
        dt = adapterCuentas.obtenerDatosNombre(nombre);
        return dt;
    }

    public DataTable consultarPorNombreCuenta(String nombre)
    {
        DataTable dt = adapterCuenta_Perfil.GetDataByNombreCuenta(nombre);
        return dt;
    }

    public String getCedulaByUsuario(string nombreUsuario)
    {
        return (string)adapterCuentas.getCedulaByUsuario( nombreUsuario );
    }

    public String getPerfilByCuenta(string nombreUsuario)
    {
        return (string)adapterCuenta_Perfil.getPerfilByCuenta(nombreUsuario);
    }

    public void actualizarFechaIngresoCuenta(DateTime fecha, String cuenta)
    {
        adapterCuentas.actualizarFechaIngresoCuenta(fecha, cuenta);
    }

    public DataTable consultarPorBusqueda(String Abuscar) {
        String aux = "%" + Abuscar + "%";
        DataTable dt = new DataTable();
        dt = adapterCuentas.GetDataByBusqueda(aux);
        return dt;
    }

    public String insertarAsociacionCuentaPerfil(String nombreCuenta, String nombrePerfil) {
        String returnValue = "";
        int r;
        try
        {
            adapterCuenta_Perfil.Insert(nombreCuenta, nombrePerfil);
        }
        catch (SqlException e)
        {
            r = e.Number;
            if (r == 2627)
            {
                returnValue = "Error al asociar la cuenta con el perfil";
            }
            else
            {
                returnValue = "Error al asociar la cuenta con el perfil";
            }
        }
        return returnValue;    
    }

    public String eliminarAsociacionCuentaPerfil(String nombreCuenta, String nombrePerfil)
    {
        String returnValue = "";
        try
        {
            adapterCuenta_Perfil.Delete(nombreCuenta, nombrePerfil);
        }
        catch (SqlException e)
        {
            returnValue = "Error al eliminar la asociacion";
        }
        return returnValue;
    }

    public DataTable retornarBecariosSinCuenta() {
        DataTable dt = new DataTable();
        dt = adapterBecario.devolverBecariosSinCuenta();
        return dt;
    }

    public DataTable retornarEncargadosSinCuenta()
    {
        DataTable dt = new DataTable();
        dt = adapterEncargado.devolverEncargadosSinCuenta();
        return dt;
    }

    public DataTable retornarNombreCuentaPorCedula(String cedula) {
        DataTable dt = new DataTable();
        dt = adapterNombreCuenta.obtenerNombrePorCedulaCuenta(cedula);
        return dt;
    }

}