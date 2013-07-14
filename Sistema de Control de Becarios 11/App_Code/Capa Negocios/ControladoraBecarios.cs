using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;

/// <summary>
/// Summary description for ControladoraBecarios
/// </summary>
public class ControladoraBecarios
{

    private CommonServices cs;
    private ControladoraBDBecario controladoraBDBecario;
    private ControladoraCuentas controladoraCuentas;
    private ControladoraAsignaciones contAsig;

    public ControladoraBecarios()
    {
        cs = new CommonServices(null);
        controladoraBDBecario = new ControladoraBDBecario();
        controladoraCuentas = new ControladoraCuentas();
        
    }





    /* Requiere: Dos arreglos con los datos completos de un becario. Puede haber uno que sea nulo.
    * 
    *  Efectúa: Controla la inserción, modificación y eliminación de becarios.
    *           Crea un nuevo becario con los datos recibidos por parámetro y luego le pide a la controladora de la BD realizar la operación correspondiente.
    *           Devuelve una hilera de caracteres que indica si la operación realizada tuvo éxito.
    * 
    *  Modifica: n/a.
    */
    public String ejecutar(int accion, Object[] datos, Object[] datosViejos) //acción --> 1: insertar, 2: modificar, 3:eliminar
    {
        Becario becarioNuevo;
        Becario becarioViejo;
        string mensajeResultado = "-1";
        switch (accion)
        {
            case 1: //Insertar 
                {
                    becarioNuevo = new Becario(datos);
                    mensajeResultado = controladoraBDBecario.insertarBecario(becarioNuevo);
                } break;
            case 2: //Modificar
                {
                    becarioNuevo = new Becario(datos);
                    becarioViejo = new Becario(datosViejos);
                    mensajeResultado = controladoraBDBecario.modificarBecario(becarioNuevo, becarioViejo);
                } break;
            case 3: //Eliminar
                {
                 mensajeResultado = controladoraBDBecario.eliminarBecario(datos[0].ToString());
                } break;
        }
        return mensajeResultado;
    }




    /* Requiere: n/a.
    * 
    *  Efectúa: Consulta todos los becarios existentes y devuelve una lista con estos.
    *           Se llena una tabla con el resultado de la consulta devuelta por la controladora de la BD y se llena una lista con las tuplas del resultado.
    * 
    *  Modifica: Crea una nueva lista de becarios, la llena y la retorna.
    */
    public List<Becario> consultarTablaBecario()
    {
        BecariosDataSet.BecarioDataTable tabla = controladoraBDBecario.consultarBecarios();
        List<Becario> listaB = new List<Becario>();
        foreach (DataRow r in tabla.Rows)
        {

            Becario becario = new Becario();

            becario.foto = cs.procesarStringDeUI(r["Foto"].ToString());
            becario.nombre = cs.procesarStringDeUI(r["Nombre"].ToString());
            becario.apellido1 = cs.procesarStringDeUI(r["Apellido1"].ToString());
            becario.apellido2 = cs.procesarStringDeUI(r["Apellido2"].ToString());
            becario.carne = cs.procesarStringDeUI(r["Carne"].ToString());
            becario.cedula = cs.procesarStringDeUI(r["Cedula"].ToString());
            becario.telefonoFijo = cs.procesarStringDeUI(r["Telefono"].ToString());
            becario.telefonoCelular = cs.procesarStringDeUI(r["Celular"].ToString());
            becario.telefonoOtro = cs.procesarStringDeUI(r["OtroTel"].ToString());
            becario.correo = cs.procesarStringDeUI(r["Correo"].ToString());
            listaB.Add(becario);
        }
        return listaB;
    }




    /* Requiere: n/a.
    * 
    *  Efectúa: Consulta cuales becarios cumplen con determinado criterio de búsqueda y devuelve una lista con estos.
    *           Se llena una tabla con el resultado de la consulta devuelta por la controladora de la BD y se llena una lista con las tuplas del resultado.
    * 
    *  Modifica: Crea una nueva lista de becarios, la llena y la retorna.
    */
    public List<Becario> consultarPorBusqueda(String textoABuscar)
    {

        BecariosDataSet.BecarioDataTable tabla = controladoraBDBecario.consultarPorBusqueda(textoABuscar);
        List<Becario> listaB = new List<Becario>();
        foreach (DataRow r in tabla.Rows)
        {

            Becario becario = new Becario();

            becario.foto = cs.procesarStringDeUI(r["Foto"].ToString());
            becario.nombre = cs.procesarStringDeUI(r["Nombre"].ToString());
            becario.apellido1 = cs.procesarStringDeUI(r["Apellido1"].ToString());
            becario.apellido2 = cs.procesarStringDeUI(r["Apellido2"].ToString());
            becario.carne = cs.procesarStringDeUI(r["Carne"].ToString());
            becario.cedula = cs.procesarStringDeUI(r["Cedula"].ToString());
            becario.telefonoFijo = cs.procesarStringDeUI(r["Telefono"].ToString());
            becario.telefonoCelular = cs.procesarStringDeUI(r["Celular"].ToString());
            becario.telefonoOtro = cs.procesarStringDeUI(r["OtroTel"].ToString());
            becario.correo = cs.procesarStringDeUI(r["Correo"].ToString());
           
            listaB.Add(becario);
        }
        return listaB;
    }



    /* Requiere: Una cédula en formato válido.
    * 
    *  Efectúa: Solicita a la controladora de la BD buscar el becario que tiene una cédula determinada.
    *           Se crea un objeto becario con el resultado devuelto por la controladora de la BD y se retorna.
    * 
    *  Modifica: n/a.
    */
    public Becario obtenerBecarioPorCedula(String cedula)
    {
        BecariosDataSet.BecarioDataTable tabla = controladoraBDBecario.obtenerBecarioPorCedula(cedula);
        Becario becario = new Becario();
        becario.foto = cs.procesarStringDeUI(tabla.Rows[0]["Foto"].ToString());
        becario.nombre = cs.procesarStringDeUI(tabla.Rows[0]["Nombre"].ToString());
        becario.apellido1 = cs.procesarStringDeUI(tabla.Rows[0]["Apellido1"].ToString());
        becario.apellido2 = cs.procesarStringDeUI(tabla.Rows[0]["Apellido2"].ToString());
        becario.carne = cs.procesarStringDeUI(tabla.Rows[0]["Carne"].ToString());
        becario.cedula = cs.procesarStringDeUI(tabla.Rows[0]["Cedula"].ToString());
        becario.telefonoFijo = cs.procesarStringDeUI(tabla.Rows[0]["Telefono"].ToString());
        becario.telefonoCelular = cs.procesarStringDeUI(tabla.Rows[0]["Celular"].ToString());
        becario.telefonoOtro = cs.procesarStringDeUI(tabla.Rows[0]["OtroTel"].ToString());
        becario.correo = cs.procesarStringDeUI(tabla.Rows[0]["Correo"].ToString());
        return becario;
    }




    /* Requiere: Una nombre de usuario válido.
    * 
    *  Efectúa: Solicita a la controladora de cuentas por la cédula del becario con determinado nombre de usuario.
    *           
    *  Modifica: n/a.
    */
    public String obtieneCedulaDeUsuario(String usuario)
    {

        return controladoraCuentas.getCedulaByUsuario(usuario);

    }


    /* Requiere: Una cédula válida.
    * 
    *  Efectúa: Solicita a la controladora de BD el correo del becario con determinada cédula.
    *           
    *  Modifica: n/a.
    */
    public String obtenerCorreoBecario(String cedulaBecario){

        return controladoraBDBecario.obtenerCorreoBecario(cedulaBecario);
    }



    /* Requiere: Una carné válido.
    * 
    *  Efectúa: Solicita a la controladora de BD la cédula del becario con determinado carné.
    *           
    *  Modifica: n/a.
    */
    public String consultarCedulaByCarne(String carne)
    {
        String resultado = "";
        DataTable dt = controladoraBDBecario.consultarCedulaByCarne(carne);
        resultado = dt.Rows[0][0].ToString();
        return resultado;
    }



    /* Requiere: Una cédula válida.
    * 
    *  Efectúa: Solicita a la controladora de BD el nombre completo del becario con determinada cédula.
    *           
    *  Modifica: n/a.
    */
    public String obtenerNombrePorCedula(String cedula)
    {
        return controladoraBDBecario.obtenerNombrePorCedula(cedula);
    }




    //**PERFIL de becario**//


    /* Requiere: n/a.
    * 
    *  Efectúa: Solicita a la controladora de la BD guardar cada uno de los datos de cada una de las lista que guardan el perfil del becario.
    *           Se itera por cada lista solicitando guardar dato por dato.
    * 
    *  Modifica: n/a.
    */
    public String guardarPerfilBecario(List<String> listaLenguajesProg, List<String> listaIdiomas, List<String> listaIntereses, List<String> listaCualidades, String cedBecario)
    {

        string mensajeResultado = "-1";

        for (int i = 0; (i < listaLenguajesProg.Count) && (!(mensajeResultado.Equals("Error"))); i++)
        {
            mensajeResultado = controladoraBDBecario.insertarLenguajeProg(listaLenguajesProg[i], cedBecario);
        }

        for (int i = 0; (i < listaIdiomas.Count) && (!(mensajeResultado.Equals("Error"))); i++)
        {
            mensajeResultado = controladoraBDBecario.insertarIdioma(listaIdiomas[i], cedBecario);
        }


        for (int i = 0; (i < listaIntereses.Count) && (!(mensajeResultado.Equals("Error"))); i++)
        {
            mensajeResultado = controladoraBDBecario.insertarAreaInteres(listaIntereses[i], cedBecario);
        }


        for (int i = 0; (i < listaCualidades.Count) && (!(mensajeResultado.Equals("Error"))); i++)
        {
            mensajeResultado = controladoraBDBecario.insertarCualidad(listaCualidades[i], cedBecario);
        }

        return mensajeResultado;
    }



    /* Requiere: Una cédula válido de un becario que exista en la base datos.
     * 
     *  Efectúa: Solicita a la controladora de la BD eliminar el perfil de un becario.
     *           
     *  Modifica: n/a.
     */
    public String eliminarPerfilBecario(String cedBecario)
    {

        string mensajeResultado = "";

        mensajeResultado = controladoraBDBecario.eliminaPerfilBecario(cedBecario);

        return mensajeResultado;

    }



    /* Requiere: n/a.
     * 
     *  Efectúa: Solicita a la controladora de la BD buscar cuales son los lenguajes de programación que pertenecen al perfil de determinado becario.
     *           Primero crea una tabla con los resultado de la consulta para luego llenar una lista con las tuplas del resultado.     
     * 
     *  Modifica: Crea  y retorna una lista con los lenguajes de programación que conoce el becario solicitado.
     */
    public List<String> consultarLenguajes(string ced)
    {

        BecariosDataSet.LenguajesProgDataTable tabla = controladoraBDBecario.consultarLenguajes(ced);
        List<String> listaLeng = new List<String>();

        foreach (DataRow r in tabla.Rows)
        {

            string texto = cs.procesarStringDeUI(r["Lenguaje"].ToString());
            listaLeng.Add(texto);
        }
        return listaLeng;
    }



    /* Requiere: n/a.
     * 
     *  Efectúa: Solicita a la controladora de la BD buscar cuales son los idiomas que pertenecen al perfil de determinado becario.
     *           Primero crea una tabla con los resultado de la consulta para luego llenar una lista con las tuplas del resultado.     
     * 
     *  Modifica: Crea  y retorna una lista con los idiomas que conoce el becario solicitado.
     */
    public List<String> consultarIdiomas(string ced)
    {

        BecariosDataSet.IdiomasDataTable tabla = controladoraBDBecario.consultarIdiomas(ced);
        List<String> listaIdiomas = new List<String>();

        foreach (DataRow r in tabla.Rows)
        {

            string texto = cs.procesarStringDeUI(r["Idioma"].ToString());
            listaIdiomas.Add(texto);
        }
        return listaIdiomas;
    }


    /* Requiere: n/a.
    * 
    *  Efectúa: Solicita a la controladora de la BD buscar cuales son las áreas de interés que pertenecen al perfil de determinado becario.
    *           Primero crea una tabla con los resultado de la consulta para luego llenar una lista con las tuplas del resultado.     
    * 
    *  Modifica: Crea  y retorna una lista con las áreas de interés del becario solicitado.
    */
    public List<String> consultarAreasInteres(string ced)
    {

        BecariosDataSet.AreasInteresDataTable tabla = controladoraBDBecario.consultarAreasInteres(ced);
        List<String> listaAreas = new List<String>();

        foreach (DataRow r in tabla.Rows)
        {

            string texto = cs.procesarStringDeUI(r["Interes"].ToString());
            listaAreas.Add(texto);
        }
        return listaAreas;
    }




    /* Requiere: n/a.
    * 
    *  Efectúa: Solicita a la controladora de la BD buscar cuales son las cualidades ( aptitudes) que pertenecen al perfil de determinado becario.
    *           Primero crea una tabla con los resultado de la consulta para luego llenar una lista con las tuplas del resultado.     
    * 
    *  Modifica: Crea  y retorna una lista con cualidades ( aptitudes) del becario solicitado.
    */
    public List<String> consultarCualidades(string ced)
    {

        BecariosDataSet.CualidadesPersonalesDataTable tabla = controladoraBDBecario.consultarCualidades(ced);
        List<String> listaCualidades = new List<String>();

        foreach (DataRow r in tabla.Rows)
        {

            string texto = cs.procesarStringDeUI(r["Cualidad"].ToString());
            listaCualidades.Add(texto);
        }
        return listaCualidades;
    }




    /* Requiere: n/a.
    * 
    *  Efectúa: Pregunta a la controladora de asignaciones si el becario con cédula "cedBecario" tiene alguna 
     *          asignación en el semestre actual .     
    * 
    *  Modifica: n/a.
    */
    public Boolean tieneAsignacion(string cedBecario){

        Boolean resultado;

        int año = cs.getAñoActual();
        int periodo = cs.getPeriodoActual();
        contAsig = new ControladoraAsignaciones();
        List<Object[]> asignacion = contAsig.consultarAsignacionDeBecario(cedBecario, año, periodo);

        if (asignacion.Count == 0)
        {
            resultado = false;
        }
        else
        {
            resultado = true;
        }

        return resultado;
    }



    /* Requiere: n/a.
    * 
    *  Efectúa: Pide a la controladora de asignaciones eliminar cualquier asignación del becario con cédula "cedBecario" 
    *           en el semestre actual .     
    * 
    *  Modifica: n/a.
    */
    public String eliminarAsignacion(string cedBecario, int ped, int año){

        contAsig = new ControladoraAsignaciones();
        return contAsig.eliminaAsignacionDeBecario(cedBecario,ped,año);
    }


}
