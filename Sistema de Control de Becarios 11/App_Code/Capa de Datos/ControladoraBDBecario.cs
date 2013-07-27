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
    private CorreosBecariosTableAdapter adapterCorreos;

    public ControladoraBDBecario()
    {
        adapterBecarios = new BecarioTableAdapter();
        adapterLenguajes = new LenguajesProgTableAdapter();
        adapterIdiomas = new IdiomasTableAdapter();
        adapterAreas = new AreasInteresTableAdapter();
        adapterCualidades = new CualidadesPersonalesTableAdapter();
        adapterAux = new Becario1TableAdapter();
        adapterCorreos = new CorreosBecariosTableAdapter();
    }



    /* Requiere: Un objeto tipo "Becario" debidamente creado y no nulo.
    * 
    *  Efectúa: Inserta una nuevo becario en la base de datos.
    *           Si obtiene un error de violación de llave primario entonces se significa que el becario ya existía en la 
    *           la base de datos y por lo tanto solo se debe volver a poner en estado "activo". 
    * 
    *  Modifica: n/a.
    */
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
                BecariosDataSet.BecarioDataTable becarioRepetido = adapterBecarios.obtenerBecarioPorCedula(becario.cedula);

                if (!(bool)becarioRepetido[0][10])
                {
                    adapterBecarios.UpdateQuery(becario.cedula, becario.nombre, becario.apellido1, becario.apellido2, becario.correo, becario.carne, becario.telefonoFijo, becario.telefonoCelular, becario.telefonoOtro, becario.foto, true, becario.cedula);
                    returnValue = "Exito";
                }
                else
                {
                    returnValue = "Error1";
                } 
            }
            else
            {
                returnValue = "Error2";
            }
        }
        return returnValue;
    }



    /* Requiere: Dos objetos tipo "Becario" debidamente creados y no nulos, con los datos nuevos y anteriores del becario
    * 
    *  Efectúa: Modifica todos los datos de un becario.
    * 
    *  Modifica: n/a.
    */
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



    /* Requiere: La cédula del becario a eliminar.
    * 
    *  Efectúa: Cambia el estado del becario a "inactivo".
    * 
    *  Modifica: n/a.
    */
    public String eliminarBecario(string cedula)
    {
        string returnValue = "Exito";

        try
        {
            this.adapterBecarios.actualizaEstado(false, cedula);
        }
        catch (SqlException e)
        {
            returnValue = "ErrorB";

        }

        return returnValue;
    }


    /* Requiere: n/a.
     *  Efectúa: Consulta todos los becarios existentes en la base de datos.
     *  Modifica: n/a.
     */
    public BecariosDataSet.BecarioDataTable consultarBecarios()
    {


        BecariosDataSet.BecarioDataTable dt = new BecariosDataSet.BecarioDataTable();
        this.adapterBecarios.consultarBecariosCompletos(dt);
        return dt;
    }



    /* Requiere: n/a.
    *  Efectúa: Consulta cuales becarios existentes en la base de datos cumplen con determinado criterio de búsqueda.
    *  Modifica: n/a.
    */
    public BecariosDataSet.BecarioDataTable consultarPorBusqueda(String criterioBusqueda)
    {

        BecariosDataSet.BecarioDataTable dt = new BecariosDataSet.BecarioDataTable();
        this.adapterBecarios.buscarBecarios(dt, criterioBusqueda);
        return dt;
    }


    /* Requiere: n/a.
    *  Efectúa: Consulta cual el becario con cédula "cedula".
    *  Modifica: n/a.
    */
    public BecariosDataSet.BecarioDataTable obtenerBecarioPorCedula(String cedula)
    {
        return this.adapterBecarios.obtenerBecarioPorCedula(cedula);
    }



    /* Requiere: n/a.
    *  Efectúa: Consulta cual el correo del becario con cédula "ced".
    *  Modifica: n/a.
    */
    public String obtenerCorreoBecario( String ced ) {

        String resultado = "-1";
        
        try
        {
            resultado = (String)(  this.adapterCorreos.obtenerCorreo(ced) );
        }
        catch (SqlException e)
        {

        }

       return resultado;
    }



    /* Requiere: n/a.
     *  Efectúa: Consulta cuál es la cédula del becario con determinado carné.
     *  Modifica: n/a.
     */
    public DataTable consultarCedulaByCarne(String carne)
    {
        DataTable dt = new DataTable();
        dt = adapterAux.getCedulaByCarne(carne);
        return dt;
    }



    /* Requiere: n/a.
     *  Efectúa: Consulta cuál es el nombre del becario con determinada cédula.
     *  Modifica: n/a.
     */
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



    /***METODOS PARA MENEJO DE PERFIL DE BECARIO **/


   /* Requiere: n/a.
    *  Efectúa: Inserta un tupla en la tabla de "LenguajesProg" asociando a un becario con un lenguaje de programación.
    *  Modifica: n/a.
    */
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



    /* Requiere: n/a.
    *  Efectúa: Inserta un tupla en la tabla de "Idiomas" asociando a un becario con un idioma.
    *  Modifica: n/a.
    */
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


    /* Requiere: n/a.
    *  Efectúa: Inserta un tupla en la tabla de "AreasInteres" asociando a un becario con una área de interés.
    *  Modifica: n/a.
    */
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


    /* Requiere: n/a.
    *  Efectúa: Inserta un tupla en la tabla de "CualidadesPersonales" asociando a un becario con una cualidad.
    *  Modifica: n/a.
    */
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


    /* Requiere: n/a.
    *  Efectúa: Consulta por los lenguajes de programación asociados a determinado becario.
    *  Modifica: n/a.
    */
    public BecariosDataSet.LenguajesProgDataTable consultarLenguajes(String cedula)
    {

        BecariosDataSet.LenguajesProgDataTable dt = new BecariosDataSet.LenguajesProgDataTable();
        this.adapterLenguajes.consultarLenguajes(dt, cedula);
        return dt;
    }


    /* Requiere: n/a.
    *  Efectúa: Consulta por los idiomas asociados a determinado becario.
    *  Modifica: n/a.
    */
    public BecariosDataSet.IdiomasDataTable consultarIdiomas(String cedula)
    {
        BecariosDataSet.IdiomasDataTable dt = new BecariosDataSet.IdiomasDataTable();
        this.adapterIdiomas.consultarIdiomas(dt, cedula);
        return dt;

    }


    /* Requiere: n/a.
    *  Efectúa: Consulta por las áreas de interés asociadas a determinado becario.
    *  Modifica: n/a.
    */
    public BecariosDataSet.AreasInteresDataTable consultarAreasInteres(String cedula)
    {

        BecariosDataSet.AreasInteresDataTable dt = new BecariosDataSet.AreasInteresDataTable();
        this.adapterAreas.consultarAreasInteres(dt, cedula);
        return dt;

    }


    /* Requiere: n/a.
     *  Efectúa: Consulta por cualidades asociadas a determinado becario.
     *  Modifica: n/a.
     */
    public BecariosDataSet.CualidadesPersonalesDataTable consultarCualidades(String cedula)
    {

        BecariosDataSet.CualidadesPersonalesDataTable dt = new BecariosDataSet.CualidadesPersonalesDataTable();
        this.adapterCualidades.consultarCualidades(dt, cedula);
        return dt;
    }



    /* Requiere: n/a.
     *  Efectúa: Elimina todo el perfil de un becario.
     *  Modifica: n/a.
     */
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

   
}
