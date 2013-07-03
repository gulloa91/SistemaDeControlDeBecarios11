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
    private LenguajesProgTableAdapter adapterLenguajes;
    private IdiomasTableAdapter adapterIdiomas;
    private AreasInteresTableAdapter adapterAreas;
    private CualidadesPersonalesTableAdapter adapterCualidades;
    private Becario1TableAdapter adapterAux;

    public ControladoraBDBecario()
    {
        adapterBecarios = new BecarioTableAdapter();
        adapterLenguajes = new LenguajesProgTableAdapter();
        adapterIdiomas = new IdiomasTableAdapter();
        adapterAreas = new AreasInteresTableAdapter();
        adapterCualidades = new CualidadesPersonalesTableAdapter();
        adapterAux = new Becario1TableAdapter();
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
                adapterBecarios.UpdateQuery(becario.cedula, becario.nombre, becario.apellido1, becario.apellido2, becario.correo, becario.carne, becario.telefonoFijo, becario.telefonoCelular, becario.telefonoOtro, becario.foto, true, becario.cedula);
                returnValue = "Exito"; 
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

          this.adapterBecarios.UpdateQuery(becarioNuevo.cedula, becarioNuevo.nombre, becarioNuevo.apellido1, becarioNuevo.apellido2, becarioNuevo.correo, becarioNuevo.carne, becarioNuevo.telefonoFijo, becarioNuevo.telefonoCelular, becarioNuevo.telefonoOtro, becarioNuevo.foto, becarioNuevo.activo, becarioViejo.cedula);
          
        }
        catch (SqlException e)
        {
            r = e.Number;
            if (r == 2627)
            {
                returnValue = "Error1";//"Ya existe un becario con la cedula digitada";
            }
            else
            {
                returnValue = "Error2";//"Se ha producido un error al modificar el becario";
            }
        }
        return returnValue;
    }

    public String eliminarBecario(string cedula)
    {
        string returnValue = "Exito";

        try
        {
            this.adapterBecarios.actualizaEstado(false, cedula);
        }
        catch (SqlException e)
        {
            returnValue = "Error";

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


    /***METODOS PARA MENEJO DE PERFIL DE BECARIO **/


    public String insertarLenguajeProg(String nuevoLenguaje, String cedBecario)
    {
        String returnValue = "Exito";
        int r;
        try
        {
            this.adapterLenguajes.insertarLenguaje(cedBecario, nuevoLenguaje);
        }
        catch (SqlException e)
        {
            returnValue = "Error";
        }
        return returnValue;
    }


    public String insertarIdioma(String nuevoIdioma, String cedBecario)
    {
        String returnValue = "Exito";//"Se ha insertado correctamente al nuevo becario";
        int r;
        try
        {
            this.adapterIdiomas.insertarIdioma(cedBecario, nuevoIdioma);
        }
        catch (SqlException e)
        {
            returnValue = "Error";
        }
        return returnValue;
    }


    public String insertarAreaInteres(String nuevoInteres, String cedBecario)
    {
        String returnValue = "Exito";//"Se ha insertado correctamente al nuevo becario";
        int r;
        try
        {
            this.adapterAreas.insertarAreaInteres(cedBecario, nuevoInteres);
        }
        catch (SqlException e)
        {
            returnValue = "Error";
        }
        return returnValue;
    }

    public String insertarCualidad(String nuevoCualidad, String cedBecario)
    {
        String returnValue = "Exito";//"Se ha insertado correctamente al nuevo becario";
        int r;
        try
        {
            this.adapterCualidades.insertarCualidad(cedBecario, nuevoCualidad);
        }
        catch (SqlException e)
        {
            returnValue = "Error";
        }
        return returnValue;
    }


    public BecariosDataSet.LenguajesProgDataTable consultarLenguajes(String cedula)
    {

        BecariosDataSet.LenguajesProgDataTable dt = new BecariosDataSet.LenguajesProgDataTable();
        this.adapterLenguajes.consultarLenguajes(dt, cedula);
        return dt;
    }

    public BecariosDataSet.IdiomasDataTable consultarIdiomas(String cedula)
    {
        BecariosDataSet.IdiomasDataTable dt = new BecariosDataSet.IdiomasDataTable();
        this.adapterIdiomas.consultarIdiomas(dt, cedula);
        return dt;

    }

    public BecariosDataSet.AreasInteresDataTable consultarAreasInteres(String cedula)
    {

        BecariosDataSet.AreasInteresDataTable dt = new BecariosDataSet.AreasInteresDataTable();
        this.adapterAreas.consultarAreasInteres(dt, cedula);
        return dt;

    }

    public BecariosDataSet.CualidadesPersonalesDataTable consultarCualidades(String cedula)
    {

        BecariosDataSet.CualidadesPersonalesDataTable dt = new BecariosDataSet.CualidadesPersonalesDataTable();
        this.adapterCualidades.consultarCualidades(dt, cedula);
        return dt;
    }


    public String eliminaPerfilBecario(String cedBecario)
    {

        string mensaje = "Exito";

        try
        {
            this.adapterLenguajes.eliminaLenguajes(cedBecario);
            this.adapterIdiomas.eliminaIdioma(cedBecario);
            this.adapterAreas.eliminaInteres(cedBecario);
            this.adapterCualidades.eliminaCualidad(cedBecario);
        }
        catch (SqlException e)
        {
            mensaje = "Error";

        }

        return mensaje;
    }

    //-----Agrego Beto------
    public DataTable consultarCedulaByCarne(String carne)
    {
        DataTable dt = new DataTable();
        dt = adapterAux.getCedulaByCarne(carne);
        return dt;
    }

    public String obtenerNombrePorCedula(String cedula)
    {
        String resultado = "-1";
        try
        {
            resultado = (String)(adapterAux.obtenerNombrePorCedula(cedula));
        }
        catch (SqlException e)
        {

        }
        return resultado;
    }

}
