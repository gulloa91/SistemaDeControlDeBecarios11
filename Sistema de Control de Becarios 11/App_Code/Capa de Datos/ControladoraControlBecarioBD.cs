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
        ch = new ControlDeHorasTableAdapter();
        a = new AsignadoATableAdapter();
        c = new ComentarioTableAdapter();
	}

    public DataTable horasReportadas(String becario,String encargado)
    {
        DataTable retorno;
        try
        {
            retorno = ch.getReportesByBecario(becario,encargado,1,DateTime.Now.Year) ;
        }
        catch (Exception e)
        {
            retorno = null;
        }
        return retorno;
    }

    //retorna la cedula del encargado para cierto becario
    public String getCedEncargado(String becario,int periodo) {
        String resultado = "";
        try
        {
            resultado = a.getCedulaEncargado(becario,periodo,DateTime.Now.Year).ToString();
        }catch(Exception ex){
            resultado = "";
        }
        return resultado;
    }

    public String enviarReporte(ControlDeHoras c)
    {
        String resultado = "Envío Exitoso";
        try
        {
            int result = ch.Insert(c.cedulaBecario, c.cedulaEncargado, c.cantidadHoras, c.fecha, c.estado, c.comentarioBecario, c.comentarioEncargado, c.periodo, DateTime.Now.Year);
            //si el comentario no esta vacio, lo agrego
            if (!c.comentarioBecario.Equals("")) agregarComentario(c.cedulaBecario, c.cedulaEncargado, c.comentarioBecario);//agrego el comentario
        }
        catch (Exception ex)
        {
            resultado = "Envío Fallido";
        }
        return resultado;
    }

    public int modificarReporte(ControlDeHoras c) { 
        int resultado = -1;
        try
        {
            resultado = ch.updateReporte(c.cantidadHoras, c.estado, c.comentarioBecario, c.cedulaBecario, c.cedulaEncargado, c.fecha, c.periodo, DateTime.Now.Year);
            //si el comentario no esta vacio lo agrego
            if(!c.comentarioBecario.Equals(""))agregarComentario(c.cedulaBecario, c.cedulaEncargado, c.comentarioBecario);//agrego el comentario
        }
        catch (Exception ex)
        {
            resultado = 0;
        }
        return 1;
    }

    public int getHoras(String becario, String encargado,int periodo)
    {
        AsignadoATableAdapter a = new AsignadoATableAdapter();
        return (int)(a.getTotalHoras(becario, encargado, periodo, DateTime.Now.Year));
    }

    //agrega el comentario en la tabla de comentarios
    private void agregarComentario(String autor, String destino, String comentario) {
        c.Insert(autor, DateTime.Now, destino, comentario);
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

    public int aceptarSiguienteAsignacion(Object [] datos)
    {
        try {
            a.aceptarAsignacion(datos[0].ToString(), Convert.ToInt32(datos[2].ToString()), Convert.ToInt32(datos[3].ToString()), datos[1].ToString());
            return 1;
        }catch(Exception ex){
            return 0;
        }
    }

    public String getComentarioBecarioFinal(String becario, String encargado)
    {
        String resultado = null;
        try {
            resultado = (a.getComentarioFinalBecario(becario, 1, DateTime.Now.Year, encargado)).ToString();
            return resultado;
        }catch(Exception ex){
            return resultado;
        }
    }

}