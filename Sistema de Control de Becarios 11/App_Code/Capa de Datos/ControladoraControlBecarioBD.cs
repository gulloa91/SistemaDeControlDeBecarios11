using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using ControlDeHorasDataSetTableAdapters;

/// <summary>
/// Summary description for ControladoraControlBecarioBD
/// </summary>
public class ControladoraControlBecarioBD
{
    ControlDeHorasTableAdapter ch;
    AsignadoATableAdapter a;
    ComentarioTableAdapter c;
	public ControladoraControlBecarioBD()
	{
		//
		// TODO: Add constructor logic here
		//
        //instancias de los tableAdapters
        ch = new ControlDeHorasTableAdapter();
        a = new AsignadoATableAdapter();
        c = new ComentarioTableAdapter();
	}

    //retorna las horas un becario
    public DataTable horasReportadas(String becario,String encargado)
    {
        DataTable retorno;
        try
        {//intenta recuperar las horas
            retorno = ch.getReportesByBecario(becario,encargado,1,DateTime.Now.Year) ;//recupera las horas
        }
        catch (Exception e)
        {//error
            retorno = null;//no retorna nada
        }
        return retorno;
    }

    //retorna la cedula del encargado para cierto becario
    public String getCedEncargado(String becario,int periodo) {
        String resultado = "";
        try
        {//intenta recuperar cedula
            resultado = a.getCedulaEncargado(becario,periodo,DateTime.Now.Year).ToString();//recupera cedula
        }catch(Exception ex){//error
            resultado = "";//no retorna nada
        }
        return resultado;
    }

    public String enviarReporte(ControlDeHoras c)
    {//se envia un reporte de horas
        String resultado = "Envío Exitoso";
        try
        {//intenta insertar el reporte
            //inserta el reporte en la base de datos
            int result = ch.Insert(c.cedulaBecario, c.cedulaEncargado, c.cantidadHoras, c.fecha, c.estado, c.comentarioBecario, c.comentarioEncargado, c.periodo, DateTime.Now.Year);
            //si el comentario no esta vacio, lo agrego
            if (!c.comentarioBecario.Equals("")) agregarComentario(c.cedulaBecario, c.cedulaEncargado, c.comentarioBecario);//agrego el comentario
        }
        catch (Exception ex)
        {//error
            resultado = "Envío Fallido";//mensaje de fallo
        }
        return resultado;
    }
    
    //modificacion de un reporte existente
    public int modificarReporte(ControlDeHoras c) { 
        int resultado = -1;
        try
        {//intenta la modificacion
            //realiza la modificacion con los nuevos datos
            resultado = ch.updateReporte(c.cantidadHoras, c.estado, c.comentarioBecario, c.cedulaBecario, c.cedulaEncargado, c.fecha, c.periodo, DateTime.Now.Year);
            //si el comentario no esta vacio lo agrego
            if(!c.comentarioBecario.Equals(""))agregarComentario(c.cedulaBecario, c.cedulaEncargado, c.comentarioBecario);//agrego el comentario
        }
        catch (Exception ex)
        {//error
            resultado = 0;//resultado fallido
        }
        return 1;
    }

    //retorna la cantidad de horas totales para una asignacion
    public int getHoras(String becario, String encargado,int periodo)
    {
        AsignadoATableAdapter a = new AsignadoATableAdapter();//se inicializa la instancia
        return (int)(a.getTotalHoras(becario, encargado, periodo, DateTime.Now.Year));//retorna la cantidad de horas
    }

    //agrega el comentario en la tabla de comentarios
    private void agregarComentario(String autor, String destino, String comentario) {
        c.Insert(autor, DateTime.Now, destino, comentario);//inserta el comentario en la tabla de comentario
    }

    //comantario final de la asignacion que concluye
    public String agregarComentarioFinal(String becario,String encargado,String comentario) {
        String resultado = "Exito";
        try {
            a.comentarioFinalBecario(comentario, becario, 1, DateTime.Now.Year, encargado);//inserta el comentario final
            return resultado;
        }catch(Exception ex){
            return "Error";
        }
    }

    //retorna el estado de una asignacion
    public int getAsignacion(String becario,String encargado, int periodo) {
        try {
            return (int)(a.getEstado(becario, periodo, DateTime.Now.Year, encargado));//retorna el estado

        }catch(Exception ex){
            return -1;//error o no existe
        }
    }

    //para aceptar la asignacion del siguiente semestre
    public int aceptarSiguienteAsignacion(Object [] datos)
    {
        try {
            //realiza la insercion en la base de datos
            a.aceptarAsignacion(datos[0].ToString(), Convert.ToInt32(datos[2].ToString()), Convert.ToInt32(datos[3].ToString()), datos[1].ToString());
            return 1;
        }catch(Exception ex){
            return 0;
        }
    }

    //retorna el comentario final del becario para saber si ya finalizo la asignacion
    public String getComentarioBecarioFinal(String becario, String encargado)
    {
        String resultado = null;
        try {
            //busca el comentario
            resultado = (a.getComentarioFinalBecario(becario, 1, DateTime.Now.Year, encargado)).ToString();
            return resultado;//retorna el comentario
        }catch(Exception ex){
            return resultado;//no habia comentario, retorna nulo
        }
    }

}