using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EncargadoDataSetTableAdapters;
using System.Data;
using System.Data.SqlClient;

//Comentario agregado para jugar con el Merge de Git :3

public class ControladoraBDEncargado
{

    /**  Atributos de la Clase **/
    private EncargadoTableAdapter adapter;

    /** EFECTO: Constructor de la clase. Inicializa el "TableAdapter"
     ** REQUIERE: Nada
     ** MODIFICA: Nueva clase ControladoraBDEncargado **/
    public ControladoraBDEncargado()
    {
        adapter = new EncargadoTableAdapter();
    }

    /** EFECTO: Inserta el Encargado "encargado" en la BD con el id "idEncargado"
     ** REQUIERE: "encargado" un Encargado válido, idEncargado un id único
     ** Modifica: Tabla ENCARGADOS de la BD **/
    public string insertarEncargado(Encargado encargado)
    {
        string mensajeError = "Exito";

        try
        {
            this.adapter.Insert(encargado.Cedula, encargado.Nombre, encargado.Apellido1, encargado.Apellido2, encargado.Correo, encargado.TelefonoFijo, encargado.TelefonoCelular, encargado.OtroTelefono, encargado.Puesto, encargado.Activo);
        }
        catch (SqlException e)
        {
            try
            {

                this.adapter.UpdateQuery(encargado.Cedula, encargado.Nombre, encargado.Apellido1, encargado.Apellido2, encargado.Correo, encargado.TelefonoFijo, encargado.TelefonoCelular, encargado.OtroTelefono, encargado.Puesto, true, encargado.Cedula);
            } 
            catch (SqlException ee)
            {
                mensajeError = "Se ha producido un error al insertar el encargado";
            }
        }
        return mensajeError;
    }

    /** EFECTO: Modifica el ENCARGADO con id "idEncargado" y cédula "cedVieja" en la BD
     ** REQUIERE: "idEncargado, cedVieja" una llave de algún encargado en la BD
     ** Modifica: Tabla ENCARGADO de la BD **/
    public string modificaEncargado(Encargado encargado, Encargado encargadoOriginal)
    {
        string mensajeError = "Exito";

        try
        {

            this.adapter.Update(encargado.Cedula, encargado.Nombre, encargado.Apellido1, encargado.Apellido2, encargado.Correo, encargado.TelefonoFijo, encargado.TelefonoCelular, encargado.OtroTelefono, encargado.Puesto, encargado.Activo, encargadoOriginal.Cedula, encargadoOriginal.Nombre, encargadoOriginal.Apellido1, encargadoOriginal.Apellido2, encargadoOriginal.Correo, encargadoOriginal.TelefonoFijo, encargadoOriginal.TelefonoCelular, encargadoOriginal.OtroTelefono,encargadoOriginal.Activo);
        } 
        catch (SqlException e)
        {
            mensajeError = "Se ha producido un error al insertar el encargado";
        }

        return mensajeError;
    }

    /** EFECTO: Elimina el Encargdap con cédula "ced" en la BD
     ** REQUIERE: "ced" una llave e la BD
     ** Modifica: Tabla ENCARGADO de la BD **/
    public string eliminarEncargado(Encargado encargado)
    {
        string mensajeError = "Exito";

        try
        {
            this.adapter.Update(encargado.Cedula, encargado.Nombre, encargado.Apellido1, encargado.Apellido2, encargado.Correo, encargado.TelefonoFijo, encargado.TelefonoCelular, encargado.OtroTelefono, encargado.Puesto, false, encargado.Cedula, encargado.Nombre, encargado.Apellido1, encargado.Apellido2, encargado.Correo, encargado.TelefonoFijo, encargado.TelefonoCelular, encargado.OtroTelefono, encargado.Activo);
        }
        catch (SqlException e)
        {
            mensajeError = "Ha ocurrido un error al eliminar el encargado";
        }

        return mensajeError;
    }

    /** EFECTO: Devuelve todas la tuplas contenidas en la tabla ENCARGADO de la BD
     ** REQUIERE: Nada
     ** MODIFICA: Nada **/
    public EncargadoDataSet.EncargadoDataTable consultarEncargados()
    {
        EncargadoDataSet.EncargadoDataTable dt = new EncargadoDataSet.EncargadoDataTable();

        this.adapter.Fill(dt);

        return dt;
    }

    /** EFECTO: Devuelve todas la tuplas contenidas en la tabla ENCARGADO de la BD que coinsiden con los patrones de busqueda solicitados
     ** REQUIERE: Nada
     ** MODIFICA: Nada **/
    public EncargadoDataSet.EncargadoDataTable ObtenerTablaEncargadosPorBusquedaSelectiva(string criterioDeBusqueda)
    {
        EncargadoDataSet.EncargadoDataTable dt = new EncargadoDataSet.EncargadoDataTable();

        dt = this.adapter.ObtenerTablaEncargadosPorBusquedaSelectiva(criterioDeBusqueda);

        return dt;
    }

    public EncargadoDataSet.EncargadoDataTable obtenerEncargadoPorCedula(String cedula)
    {
        return this.adapter.obtenerEncargadoPorCedula(cedula);

    }
}