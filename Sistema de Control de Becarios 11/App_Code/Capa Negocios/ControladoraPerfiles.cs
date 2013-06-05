using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

/// <summary>
/// Summary description for ControladoraPerfiles
/// </summary>
public class ControladoraPerfiles
{
    CommonServices cs;//servicios comunes
    ControladoraPerfilesDB cp;//controladora de base de datos
    public ControladoraPerfiles()
    {
        //
        // TODO: Add constructor logic here
        //
        //instacio los objetos
        cs = new CommonServices(null);
        cp = new ControladoraPerfilesDB();
    }

    //retornar todos los nombres de los perfiles en el sistema
    public DataTable consultar() {
        DataTable dt = cp.consultar();//se realiza la consulta
        return dt;//se retorna el table
    }

    //retorna el tipo de perfil del perfil nombre
    public Object tipoPerfil(String nombre){
        return cp.tipoPerfil(nombre);//re realiza la consulta del tipo
    }

    //ejecuta la accion
    public String ejecutar(int accion, Object[] datos)
    {
        String retorno = "";
        Perfil p;
        switch (accion)
        {
            case 1: //insertar
                //agrego un perfil en la tabla Perfiles
                cp.agregarPerfil(datos[0].ToString(), Convert.ToInt32(datos[1].ToString()));
                //recorro los permisos
                for (int i = 3; i < datos.Length; ++i)
                {
                    if (!datos[i].Equals("0"))
                    {//si tiene permiso en el perfil i
                        //el datos[12] es el tipo de perfil, administrador, encargado o becario
                        p = new Perfil(datos[0].ToString(), datos[i].ToString(), datos[1].ToString());
                        retorno = cp.agregarPermisos(p);//envio a insertar el permiso para el perfil p
                    }
                }
                break;


            case 2://modificar
                for (int i = 3; i < datos.Length; ++i)
                {//primero las inserciones
                    if (!datos[i].Equals("0"))
                    {//hay que hacer la insercion
                        //modifico los permisos para el nombre anterior
                        p = new Perfil(datos[2].ToString(), datos[i].ToString(), datos[1].ToString());
                        retorno = cp.modificaPerfil(1, p);
                    }
                }
                for (int i = 3; i < datos.Length; ++i)
                {//borrar los que no se necesitan
                    if (datos[i].Equals("0"))
                    {//el permiso i no se necesita
                        p = new Perfil(datos[2].ToString(), datos[i].ToString(), datos[1].ToString());
                        retorno = cp.modificaPerfil(2, p);
                    }
                }
                retorno = cp.modNom(datos[0].ToString(),datos[2].ToString());
                retorno = cp.modificarTipo(datos[0].ToString(), datos[1].ToString());
                break;


            case 3://eliminar
                for (int i = 3; i < datos.Length; ++i)
                {
                    if (!datos[i].Equals("0"))
                    {//tiene perfil, hay que eliminarlo
                        //primero elimino los permisos del perfil en la tabla Perfil_permiso
                        retorno = cp.eliminarPermisos(cs.procesarStringDeUI(datos[0].ToString()), Convert.ToInt32(datos[i].ToString()));
                    }
                }
                //de ultimo elimino el perfil de la tabla perfiles
                cp.eliminaPerfil(cs.procesarStringDeUI(datos[0].ToString()));
                break;

        }
        return retorno;
    }

    //retorna los permisos del perfil nombre
    public DataTable consultarPerfil(String nombre)
    {
        return cp.consultarPerfil(nombre);//consulto un perfil en especifico
    }

    //retorna un DataTable con los perfiles que contienen el patron que el usuario digito
    public DataTable buscarPerfiles(String nom) {
        return cp.buscarPerfiles(nom);
    }

    public List<int> obtenerPermisosUsuario(String nombre)
    {
        return cp.obtenerPermisosUsuario(nombre);
    }
}