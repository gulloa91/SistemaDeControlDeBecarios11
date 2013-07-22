using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using DataSetCuentasTableAdapters;
using DataSetCuentasPerfilesQuitarTableAdapters;
using AsignacionesDataSetTableAdapters;

public class ControladoraBDCuentas
{
    CuentaTableAdapter adapterCuentas;
    BecarioTableAdapter adapterBecario;
    EncargadoTableAdapter adapterEncargado;
    Cuenta_PerfilTableAdapter adapterCuenta_Perfil;
    DataTable1TableAdapter adapterNombreCuenta;
    AsignadoA1TableAdapter adapterAsignaciones;

    //Constructor encargado de inicializar los adapter
	public ControladoraBDCuentas()
    {
        adapterCuenta_Perfil = new Cuenta_PerfilTableAdapter();
        adapterCuentas = new CuentaTableAdapter();
        adapterBecario = new BecarioTableAdapter();
        adapterEncargado = new EncargadoTableAdapter();
        adapterNombreCuenta = new DataTable1TableAdapter();
        adapterAsignaciones = new AsignadoA1TableAdapter();
	}

    /* Efectúa: Se encarga de ejecutar la inserción en la tabla cuentas de la base de datos.
     * Requiere: Que la entidad Cuentas no esté vacía.
     * Modifica: La tabla Cuentas en la base de datos cuando inserta la cuenta.
     */
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

    /* Efectúa: Se encarga de ejecutar el update en la tabla cuentas de la base de datos.
     * Requiere: Que la entidad cuentaVieja contenga los datos de la cuenta que existe en la base de datos y que la entidad cuentaNueva
     * contenga los datos nuevos.
     * Modifica: La tabla Cuentas en la base de datos cuando realiza el update de la cuenta.
     */
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

    /* Efectúa: Se encarga de ejecutar la eliminación de una cuenta en la tabla cuentas de la base de datos.
     * Requiere: Que la entidad Cuentas no esté vacía.
     * Modifica: La tabla Cuentas en la base de datos cuando elimina la cuenta.
     */
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


    /* Efectúa: Se encarga de verificar que exista una cuenta  en la base de datos con el nombre de usuario y contraseña especificados en los parametros.
     * Requiere: N/A.
     * Modifica: N/A.
     */
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


    /* Efectúa: Se encarga de retornar todas las cuentas existentes en la base de datos.
    * Requiere: N/A.
    * Modifica: N/A.
    */
    public DataTable consultarCuentas() 
    {

        DataSetCuentas.CuentaDataTable dt = new DataSetCuentas.CuentaDataTable();
        dt = adapterCuentas.GetData();
        return dt;
    }

    /* Efectúa: Se encarga de retornar los datos de la cuenta que coincidan con el usuario y contraseña especificados.
    * Requiere: N/A.
    * Modifica: N/A.
    */
    public DataTable consultarPorNombreContr(String nombre, String contrasena) {
        DataSetCuentas.CuentaDataTable dt = new DataSetCuentas.CuentaDataTable();
        dt =  adapterCuentas.GetDataByNombreContr(nombre, contrasena);
        return dt;
    }

    /* Efectúa: Se encarga de retornar los datos de la cuenta que coincidan con el usuario especificado.
    * Requiere: N/A.
    * Modifica: N/A.
    */
    public DataTable obtenerDatosCuenta(String nombre)
    {
        DataSetCuentas.CuentaDataTable dt = new DataSetCuentas.CuentaDataTable();
        dt = adapterCuentas.obtenerDatosNombre(nombre);
        return dt;
    }

    /* Efectúa: Se encarga de retornar el perfil y el nombre de usuario de la asociacion con cuenta especificada.
    * Requiere: N/A.
    * Modifica: N/A.
    */
    public DataTable consultarPorNombreCuenta(String nombre)
    {
        DataTable dt = adapterCuenta_Perfil.GetDataByNombreCuenta(nombre);
        return dt;
    }

    /* Efectúa: Se encarga de retornar la cedula correspondiente a un nombre de usuario especificado.
    * Requiere: N/A.
    * Modifica: N/A.
    */
    public String getCedulaByUsuario(string nombreUsuario)
    {
        return (string)adapterCuentas.getCedulaByUsuario( nombreUsuario );
    }

    /* Efectúa: Se encarga de retornar el perfil correspondiente a un nombre de usuario especificado.
    * Requiere: N/A.
    * Modifica: N/A.
    */
    public String getPerfilByCuenta(string nombreUsuario)
    {
        return (string)adapterCuenta_Perfil.getPerfilByCuenta(nombreUsuario);
    }

    /* Efectúa: Se encarga de actualizar una cuenta en la base de datos con una nueva fecha.
    * Requiere: Que los parámetros fecha y cuenta no sean nulos.
    * Modifica: La cuenta después de realizar el update.
    */
    public void actualizarFechaIngresoCuenta(DateTime fecha, String cuenta)
    {
        adapterCuentas.actualizarFechaIngresoCuenta(fecha, cuenta);
    }

    /* Efectúa: Se encarga de retornar todas las cuentas que tengan datos que coincidan con el parametro Abuscar.
    * Requiere: N/A
    * Modifica: N/A
    */
    public DataTable consultarPorBusqueda(String Abuscar) {
        String aux = "%" + Abuscar + "%";
        DataTable dt = new DataTable();
        dt = adapterCuentas.GetDataByBusqueda(aux);
        return dt;
    }

    /* Efectúa: Se encarga de insertar en la base de datos una nueva asociación de una cuenta con un perfil
    * Requiere: Que los parámetros nombreCuenta y nombrePerfil no sean nulos.
    * Modifica: La tabla cuentas_perfil en la base de datos.
    */
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


    /* Efectúa: Se encarga de eliminar de la base de datos una asociación de una cuenta con un perfil
    * Requiere: Que los parámetros nombreCuenta y nombrePerfil no sean nulos.
    * Modifica: La tabla cuentas_perfil en la base de datos.
    */
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

    /* Efectúa: Se encarga de retornar todos los becarios existentes en la base de datos que no tienen una cuenta asociada
    * Requiere: N/A
    * Modifica: N/A
    */
    public DataTable retornarBecariosSinCuenta() {
        DataTable dt = new DataTable();
        dt = adapterBecario.devolverBecariosSinCuenta();
        return dt;
    }

    /* Efectúa: Se encarga de retornar todos los encargados existentes en la base de datos que no tienen una cuenta asociada
    * Requiere: N/A
    * Modifica: N/A
    */
    public DataTable retornarEncargadosSinCuenta()
    {
        DataTable dt = new DataTable();
        dt = adapterEncargado.devolverEncargadosSinCuenta();
        return dt;
    }

    /* Efectúa: Se encarga de retornar el nombre de una cuenta que contenga la cedula especificada por parametro
    * Requiere: N/A
    * Modifica: N/A
    */
    public DataTable retornarNombreCuentaPorCedula(String cedula) {
        DataTable dt = new DataTable();
        dt = adapterNombreCuenta.obtenerNombrePorCedulaCuenta(cedula);
        return dt;
    }

    /* Efectua: Revisa si existen cuentas con asignaciones pendientes
     * Requiere: N/A
     * Modifica: N/A
     */
    public DataTable revisarAsignaciones(String cedula) {
        DataTable dt = new DataTable();
        dt = adapterAsignaciones.revisarAsignaciones(cedula);
        return dt;
    }

}
