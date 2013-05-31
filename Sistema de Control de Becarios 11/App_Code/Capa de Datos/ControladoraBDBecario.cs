using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using BecariosDataSetTableAdapters;
using System.Data;

/// <summary>
/// Summary description for ControladoraBDBecario
/// </summary>
public class ControladoraBDBecario
{
    private BecarioTableAdapter adapterBecarios;

    public ControladoraBDBecario()
    {
        adapterBecarios = new BecarioTableAdapter();
    }

    public String insertarBecario(Becario becario)
    {
        String returnValue = "Exito";//"Se ha insertado correctamente al nuevo becario";
        int r;
        try
        {

           this.adapterBecarios.Insert(becario.cedula, becario.nombre, becario.apellido1, becario.apellido2, becario.correo, becario.carne, becario.telefonoFijo, becario.telefonoCelular, becario.telefonoOtro, "", becario.activo);
    
        }
        catch (SqlException e)
        {
            r = e.Number;
            if (r == 2627)
            {
                returnValue = "Error1";
            }
            else
            {
                returnValue = "Error2";
            }
        }
        return returnValue;
    }

    public String modificarBecario(Becario becarioNuevo, Becario becarioViejo)
    {
        String returnValue = "Exito";
        int r;
        try
        {
            this.adapterBecarios.Update(becarioNuevo.cedula, becarioNuevo.nombre, becarioNuevo.apellido1, becarioNuevo.apellido2, becarioNuevo.correo, becarioNuevo.carne, becarioNuevo.telefonoFijo, becarioNuevo.telefonoCelular, becarioNuevo.telefonoOtro, "", becarioNuevo.activo, becarioViejo.cedula, becarioViejo.nombre, becarioViejo.apellido1, becarioViejo.apellido2, becarioViejo.correo, becarioViejo.carne, becarioViejo.telefonoFijo, becarioViejo.telefonoCelular, becarioViejo.telefonoOtro, "", becarioViejo.activo);

        }
        catch (SqlException e)
        {
            r = e.Number;
            if (r == 2627)
            {
                returnValue = "Erro1";//"Ya existe un becario con la cedula digitada";
            }
            else
            {
                returnValue ="Erro2";//"Se ha producido un error al modificar el becario";
            }
        }
        return returnValue;
    }

    public String eliminarBecario(string cedula)
    {
        string returnValue = "Se ha eliminado correctamente al becario";

        try
        {
            this.adapterBecarios.actualizaEstado(false, cedula);
        }
        catch (SqlException e)
        {
            returnValue = "Ha ocurrido un error al eliminar el becario";

        }

        return returnValue;
    }


    public BecariosDataSet.BecarioDataTable consultarBecarios()
    {


        BecariosDataSet.BecarioDataTable dt = new BecariosDataSet.BecarioDataTable();
        this.adapterBecarios.consultarBecariosCompletos(dt);
        return dt;
    }



    public BecariosDataSet.BecarioDataTable consultarPorBusqueda(String criterioBusqueda)
    {

        BecariosDataSet.BecarioDataTable dt = new BecariosDataSet.BecarioDataTable();
        this.adapterBecarios.buscarBecarios(dt, criterioBusqueda);
        return dt;
    }

    public BecariosDataSet.BecarioDataTable obtenerBecarioPorCedula(String cedula)
    {
        return this.adapterBecarios.obtenerBecarioPorCedula(cedula);
    }

}