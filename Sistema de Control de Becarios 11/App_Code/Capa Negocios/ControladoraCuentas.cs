using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

public class ControladoraCuentas
{
    ControladoraBDCuentas controladoraBDCuentas;
    CommonServices cs;

    /* Efectúa: Constructor encargado de inicializar la controladora de base de datos.
     * Requiere: N/A
     * Modifica: N/A
     */ 
	public ControladoraCuentas()
	{
        controladoraBDCuentas = new ControladoraBDCuentas();
        cs = new CommonServices(null);
	}


    /* Efectúa: Se encarga de controlar las acciones de insertar, modificar y eliminar una cuenta de la base de datos, además de llamar a la controladora
     * de base de datos para que realice la acción respectiva.
    * Requiere: Que en acción se especifique la accion a realizar y los arreglos "datos" y "datosViejos" no esten nulos.
    * Modifica: N/A
    */ 
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

    /* Efectúa: Se encarga de controlar las acciones de insertar y eliminar una asociacion entre una cuenta de la base de datos y un peril, además 
     * de llamar a la controladora de base de datos para que realice la acción respectiva.
    * Requiere: Que en acción se especifique la accion a realizar y los arreglos "datos" y "datosNuevos" no esten nulos.
    * Modifica: N/A
    */ 
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

                } break;
            case 3:
                {
                    resultado = controladoraBDCuentas.eliminarAsociacionCuentaPerfil(cs.procesarStringDeUI(datos[0].ToString()), cs.procesarStringDeUI(datos[1].ToString()));
                } break;
        }

        return resultado;
    }

    /* Efectúa: Se encarga de llamar a la controladora de base de datos de cuenta para que verifique si existe una cuenta con el nombre de usuario y
     * contraseña especificados
    * Requiere: N/A
    * Modifica: N/A
    */ 
    public Boolean validarUsuario(String nombreUsuario, String contrasenna) {
        return controladoraBDCuentas.validarUsuario(nombreUsuario, contrasenna);
    }

    /* Efectúa: Se encarga de retornar la cedula de una cuenta que contenga el nombre de usuario especificado.
    * Requiere: N/A
    * Modifica: N/A
    */ 
    public String getCedulaByUsuario(String nombreUsuario)
    {
        return controladoraBDCuentas.getCedulaByUsuario(nombreUsuario);
    }

    /* Efectúa: Se encarga de ejecutar la controladora de base de datos para retornar todas las cuentas existentes en la base de datos.
    * Requiere: N/A
    * Modifica: N/A
    */ 
    public DataTable consultarCuentas() {
        return controladoraBDCuentas.consultarCuentas();
    }

    /* Efectúa: Se encarga de ejecutar la controladora de base de datos para retornar la cuenta que contenga el nombre de usuario y contraseña
     * especificados en los parametros.
    * Requiere: N/A
    * Modifica: N/A
    */
    public DataTable consultarPorNombreContr(String nombre, String contrasena)
    {
        return controladoraBDCuentas.consultarPorNombreContr(nombre, contrasena);
    }

    /* Efectúa: Se encarga de retornar los datos de la cuenta que coincidan con el usuario especificado y llamar a la controladora de base de datos para
     * que realice la consulta.
    * Requiere: N/A.
    * Modifica: N/A.
    */
    public DataTable obtenerDatosCuenta(String nombre)
    {
        return controladoraBDCuentas.obtenerDatosCuenta(nombre);
    }

    /* Efectúa: Se encarga de retornar todas las cuentas que tengan datos que coincidan con el parametro Abuscary llamar a la controladora de base de datos para
     * que realice la consulta.
    * Requiere: N/A
    * Modifica: N/A
    */
    public DataTable consultarPorBusqueda(String Abuscar)
    {
        return controladoraBDCuentas.consultarPorBusqueda(Abuscar);
    }


    /* Efectúa: Se encarga de retornar el perfil y el nombre de usuario de la asociacion con cuenta especificada.
    * Requiere: N/A.
    * Modifica: N/A.
    */
    public DataTable consultarPorNombreCuenta(String nombre)
    {
        return controladoraBDCuentas.consultarPorNombreCuenta(nombre);
    }

    /* Efectúa: Se encarga de retornar el perfil de la cuenta que tiene el nombre de usuario especificada en el parametro.
    * Requiere: N/A.
    * Modifica: N/A.
    */
    public String getPerfilByCuenta(String nombreCuenta)
    {
        return controladoraBDCuentas.getPerfilByCuenta(nombreCuenta);
    }

    /* Efectúa: Se encarga de llamar a la controladora de base de datos de cuentas para que modifique la fecha de ingreso en la cuenta especificada.
    * Requiere: N/A.
    * Modifica: N/A.
    */
    public void actualizarFechaIngresoCuenta(DateTime fecha, String cuenta)
    {
        controladoraBDCuentas.actualizarFechaIngresoCuenta(fecha, cuenta);
    }

    /* Efectúa: Se encarga de llamar a la controladora de base de datos de cuentas para retornar todos los becarios que no tengan una cuenta asociada.
    * Requiere: N/A.
    * Modifica: N/A.
    */
    public DataTable devolverBecariosSinCuenta() { 
        return controladoraBDCuentas.retornarBecariosSinCuenta();
    }

    /* Efectúa: Se encarga de llamar a la controladora de base de datos de cuentas para retornar todos los encargados que no tengan una cuenta asociada.
    * Requiere: N/A.
    * Modifica: N/A.
    */
    public DataTable devolverEncargadosSinCuenta() {
        return controladoraBDCuentas.retornarEncargadosSinCuenta();
    }

    /* Efectúa: Se encarga de retornar el nombre de la cuenta  que contenga la cedula especificada en el parametro
    * Requiere: N/A.
    * Modifica: N/A.
    */
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
