using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

/// <summary>
/// Summary description for CommonServices
/// </summary>
public class CommonServices
{
    private UpdatePanel panel;

	public CommonServices(UpdatePanel panel)
	{
        this.panel = panel;
	}

    public void correrJavascript(String funcion)
    {
        Guid gMessage = Guid.NewGuid();
        string sMessage = funcion;

        ScriptManager.RegisterStartupScript(panel, panel.GetType(), gMessage.ToString(), sMessage, true);
    }

    public void mensajeJavascript(String mensaje, String titulo)
    {
        correrJavascript("$('#popUpMensaje').text('"+ mensaje +"');");
        correrJavascript("$('#popUpMensaje').dialog({ title: '"+ titulo +"' });");
        correrJavascript("$('#popUpMensaje').dialog('open');");
    }

    public void abrirPopUp(String popUpId, String titulo)
    {
        correrJavascript("abrirPopUp('" + popUpId + "', '" + titulo + "');");
    }

    public void cerrarPopUp(String popUpId)
    {
        correrJavascript("cerrarPopUp('" + popUpId + "');");
    }

    public String procesarStringDeUI(String linea)
    {
        return HttpUtility.HtmlDecode(linea).Trim();
    }

    public void esconderPrimerBotonDePopUp(String popUpId)
    {
        this.correrJavascript("$('#"+ popUpId +"').next().find('.ui-dialog-buttonset button:first').hide();");
    }

    public void mostrarPrimerBotonDePopUp(String popUpId)
    {
        this.correrJavascript("$('#" + popUpId + "').next().find('.ui-dialog-buttonset button:first').show();");
    }
}