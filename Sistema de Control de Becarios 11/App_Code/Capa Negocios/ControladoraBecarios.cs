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
    private ControladoraBDBecario controladoraBDBecario;
    private CommonServices cs;
    private ControladoraCuentas controladoraCuentas;

    
    public ControladoraBecarios()
    {
        cs = new CommonServices(null);
        controladoraBDBecario = new ControladoraBDBecario();
        controladoraCuentas = new ControladoraCuentas();
    }

    public String ejecutar(int accion, Object[] datos, Object[] datosViejos) //1 insertar 2 modificar 3 eliminar
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


    public String obtieneCedulaDeUsuario(String usuario) {

      return controladoraCuentas.getCedulaByUsuario(usuario);
  
    }

    public String guardarPerfilBecario(List<String> listaLenguajesProg, List<String> listaIdiomas, List<String> listaIntereses, List<String> listaCualidades, String cedBecario)
    {

        string mensajeResultado = "-1";

        for (int i = 0; i < listaLenguajesProg.Count; i++)
        {
            mensajeResultado = controladoraBDBecario.insertarLenguajeProg(listaLenguajesProg[i], cedBecario);
        }

        for (int i = 0; i < listaIdiomas.Count; i++)
        {
           mensajeResultado = controladoraBDBecario.insertarIdioma(listaIdiomas[i], cedBecario);
        }
        

        for (int i = 0; i < listaIntereses.Count; i++)
        {
           mensajeResultado = controladoraBDBecario.insertarAreaInteres(listaIntereses[i], cedBecario);
        }
        

        for (int i = 0; i < listaCualidades.Count; i++)
        {
            mensajeResultado = controladoraBDBecario.insertarCualidad(listaCualidades[i], cedBecario);
        }
       
        return mensajeResultado;
    }



    public String eliminarPerfilBecario(String cedBecario)
    {

        string mensajeResultado = "";

        mensajeResultado = controladoraBDBecario.eliminaPerfilBecario(cedBecario);

        return mensajeResultado;
    
    }


    public List<String> consultarLenguajes(string ced){

       BecariosDataSet.LenguajesProgDataTable tabla = controladoraBDBecario.consultarLenguajes(ced);
       List<String> listaLeng = new List<String>();

       foreach (DataRow r in tabla.Rows)
       {

           string texto = cs.procesarStringDeUI(r["Lenguaje"].ToString());
           listaLeng.Add(texto);
       }
       return listaLeng;
   }


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



}