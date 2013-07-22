using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using DataSetTableAdapters;

/// <summary>
/// Summary description for ControladoraPerfilesDB
/// </summary>
public class ControladoraPerfilesDB
{
    private PerfilTableAdapter pt;//los table adapters
    private Perfil_PermisoTableAdapter pp;//los table adapters
    public ControladoraPerfilesDB()
    {
        //
        // TODO: Add constructor logic here
        //
        //instancio los objetos
        pt = new PerfilTableAdapter();
        pp = new Perfil_PermisoTableAdapter();
    }

    //hce la consulta para buscar los perfiles que tengan cierto patron
    public DataTable buscarPerfiles(String nom) {
        return pt.getNombres(nom);//consulta a la base de datos
    }

    //agrega un perfil en la tabla de Perfil
    public String agregarPerfil(String nombre,int tipo) {
        string returnValue = "";

        try
        {//intenta hacer la insercion
            pt.Insert(nombre, tipo);//se inserta el perfil
        }
        catch (SqlException e)
        {
            int r = e.Number;
            if (r == 2627)
            {//ya existia ese perfil
                returnValue = "Ya existe un perfil con el nombre digitado";
            }
            else
            {//error de base de datos
                returnValue = "Se ha producido un error al insertar el perfil";
            }

        }

        return returnValue;
    }

    //para modificar el tipo del perfil
    public String modificarTipo(String nom,String tipo) {
        string returnValue = "";

        try
        {//intenta hacer la modificacion
            pt.updateTipo(Convert.ToInt32(tipo),nom);//hace la modificacion
        }
        catch (SqlException e)
        {//error muestra mensaje
            returnValue = "Ha ocurrido un error al modificar el perfil";

        }

        return returnValue;
    }

    //modifica el nombre del perfil
    public String modNom(String nom,String nomAnt) {
        string returnValue = "";

        try
        {//intenta hacer la modificacion
            pt.updateNombre(nom, nomAnt);//hace la modificacion
        }
        catch (SqlException e)
        {//error, muestra mensaje de error
            returnValue = "Ha ocurrido un error al modificar el perfil";

        }

        return returnValue;
    }

    //agrega 1 permiso para el perfil p en la tabla Perfil_Permiso
    //este metodo se llama varias veces para un mismo perfil
    public String agregarPermisos(Perfil p)
    {
        string returnValue = "";

        try
        {//intenta agregar permisos
            pp.Insert(p.Nombre, p.IDPermiso);//agrega permisos a un perfil
        }
        catch (SqlException e)
        {//error, muestra mensaje de error
            returnValue = "Ha ocurrido un error al Agregar el perfil";

        }

        return returnValue;
    }

    //retorna el tipo(Administrador,becario,Encargado) del perfil nombre
    public Object tipoPerfil(String nombre) {
        return pt.getTipoByNombre(nombre);
    }

    //retorna los permisos del perfil nombre
    public DataTable consultarPerfil(String nombre) {
        return pp.consultarPerfil(nombre);
    }

    //retorna los nombres de los perfiles que estan en el sistema
    public DataTable consultar() {
        DataTable dt = pt.GetData();
        return dt;
    }

    //elimina todo el perfil del sistema
    public String eliminaPerfil(String nombre)
    {
        string returnValue = "";

        try
        {//intenta la eliminacion
            pt.Delete(nombre);//elimina el perfil de la tabla Perfil
        }
        catch (SqlException e)
        {
            returnValue = "Ha ocurrido un error al eliminar el perfil";

        }

        return returnValue;
    }

    //elimina los permisos de un perfil para luego eliminar el mismo
    public String eliminarPermisos(String nombre,int permiso) {
        string returnValue = "";

        try
        {//intenta la eliminacion
            pp.Delete(nombre, permiso);//elimina un permiso de la tabla Perfil_Permiso
        }
        catch (SqlException e)
        {
            returnValue = "Ha ocurrido un error al eliminar el perfil";

        }

        return returnValue;
    }

    

    //le mando un perfil con su permiso, si no aparece en la base de datos lo agrego,
    //si aparece lo borro
    public String modificaPerfil(int tipo, Perfil perfil)
    {
        String returnValue = "";
        int r;
        try
        {
            switch (tipo)
            {//tipo es para saber si borro o agrego permisos a un perfil
                case 1: //para las inserciones
                    agregarPermisos(perfil);
                    break;

                case 2://para eliminar
                    eliminarPermisos(perfil.Nombre,perfil.IDPermiso);
                    break;
            }

        }
        catch (SqlException e)
        {
            r = e.Number;
            if (r == 2627)
            {//ya existia
                returnValue = "Ya existe un perfil con el nombre digitado";
            }
            else
            {//error de base de datos
                returnValue = "Se ha producido un error al modificar el perfil";
            }
        }
        return returnValue;
    }

    //lista de permisos de un perfil
    public List<int> obtenerPermisosUsuario(String nombre)
    {
        DataTable dt = pp.consultarPerfil(nombre);//consulta perfil
        List<int> lista = new List<int>();//crea la lista
        for (int i = 0; i < dt.Rows.Count; ++i)
        {//recorro el data Table
            lista.Add(Convert.ToInt32(dt.Rows[i].ItemArray[1]));//agrego permisos
        }
        return lista;
    }
}