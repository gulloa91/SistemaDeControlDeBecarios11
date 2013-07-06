using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

public class ControladoraCuentas
{
    ControladoraBDCuentas controladoraBDCuentas;
    CommonServices cs;
	public ControladoraCuentas()
	{
        controladoraBDCuentas = new ControladoraBDCuentas();
        cs = new CommonServices(null);
	}

    public String ejecutar(int accion, Object[] datos, Object[] datosViejos)
    {
        String resultado = "-1";
        Cuentas cuenta;
        Cuentas cuentaVieja;
        switch(accion){
            case 1: {
                cuenta = new Cuentas(datos);
                resultado = controladoraBDCuentas.insertarCuenta(cuenta);
            }break;
            case 2: {
                cuenta = new Cuentas(datos);
                cuentaVieja = new Cuentas(datosViejos);
                resultado = controladoraBDCuentas.modificarCuenta(cuenta, cuentaVieja);
            } break;
            case 3: {
                cuenta = new Cuentas(datos);
                resultado = controladoraBDCuentas.eliminarCuenta(cuenta);
            }break;
        }

        return resultado;
    }


    public String ejecutarAsociacion(int accion, Object[] datos, Object[] datosNuevos)
    {
        String resultado = "-1";
        switch (accion)
        {
            case 1:
                {
                    resultado = controladoraBDCuentas.insertarAsociacionCuentaPerfil(cs.procesarStringDeUI(datos[0].ToString()), cs.procesarStringDeUI(datos[1].ToString()));
                } break;
            case 2:
                {
                    //resultado = controladoraBDCuentas.modificarCuenta(cuenta, cuentaVieja);
                } break;
            case 3:
                {
                    resultado = controladoraBDCuentas.eliminarAsociacionCuentaPerfil(cs.procesarStringDeUI(datos[0].ToString()), cs.procesarStringDeUI(datos[1].ToString()));
                } break;
        }

        return resultado;
    }

    public Boolean validarUsuario(String nombreUsuario, String contrasenna) {
        return controladoraBDCuentas.validarUsuario(nombreUsuario, contrasenna);
    }

    public String getCedulaByUsuario(String nombreUsuario)
    {
        return controladoraBDCuentas.getCedulaByUsuario(nombreUsuario);
    }

    public DataTable consultarCuentas() {
        return controladoraBDCuentas.consultarCuentas();
    }

    public DataTable consultarPorNombreContr(String nombre, String contrasena)
    {
        return controladoraBDCuentas.consultarPorNombreContr(nombre, contrasena);
    }

    public DataTable obtenerDatosCuenta(String nombre)
    {
        return controladoraBDCuentas.obtenerDatosCuenta(nombre);
    }

    public DataTable consultarPorBusqueda(String Abuscar)
    {
        return controladoraBDCuentas.consultarPorBusqueda(Abuscar);
    }

    public DataTable consultarPorNombreCuenta(String nombre)
    {
        return controladoraBDCuentas.consultarPorNombreCuenta(nombre);
    }

    public String getPerfilByCuenta(String nombreCuenta)
    {
        return controladoraBDCuentas.getPerfilByCuenta(nombreCuenta);
    }

    public void actualizarFechaIngresoCuenta(DateTime fecha, String cuenta)
    {
        controladoraBDCuentas.actualizarFechaIngresoCuenta(fecha, cuenta);
    }

    public DataTable devolverBecariosSinCuenta() { 
        return controladoraBDCuentas.retornarBecariosSinCuenta();
    }

    public DataTable devolverEncargadosSinCuenta() {
        return controladoraBDCuentas.retornarEncargadosSinCuenta();
    }

    public String retornarNombreCuentaPorCedula(String cedula)
    {
        String resultado = "-1";
        DataTable dt = new DataTable();
        dt = controladoraBDCuentas.retornarNombreCuentaPorCedula(cedula);
        if(dt.Rows.Count==1){
            resultado = dt.Rows[0][0].ToString();
        }
        return resultado;
    }

}
