using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Net;
using System.Net.Mail;

/// <summary>
/// Summary description for EmailServices
/// </summary>
public class EmailServices
{
    private static String nombreSistema = "Sistema de Control de Becarios 11 de la ECCI";
    private static String correoSistema = "cristopher.sanchez@ucr.ac.cr";
    private static String clavecorreoSistema = "CRSACO829";
    private static SmtpClient SmtpServer;

	public EmailServices()
	{
        SmtpServer = new SmtpClient("smtp.ucr.ac.cr");
        //Configuracion del SMTP
        SmtpServer.Port = 25; //Puerto que utiliza Gmail para sus servicios

        //Especificamos las credenciales con las que enviaremos el mail
        SmtpServer.Credentials = new System.Net.NetworkCredential(correoSistema, clavecorreoSistema);
        SmtpServer.EnableSsl = true;
	}

    public bool enviarCorreoCuentaCreada(String correo_destino, String nombre_completo, String contrasena, String usuario)
    {
        try
        {
            MailMessage mail = new MailMessage();
            mail.Subject = "Bienvenido al Sistema de Control de Becarios 11";
            mail.From = new MailAddress(correoSistema, nombreSistema, Encoding.UTF8);
            mail.Body = "<html><body><div style='width: 80%; margin:0 10%; border-radius: 5px; border: 2px solid #414141; font-size: 16px; background: #2F4F2F;'> <div style='margin: 5%; width: 90%;background: #FFF; border-radius: 5px;'> <div style='padding: 5px 5%; width: 90%; text-align: center;'> <img src='http://www.ecci.ucr.ac.cr/files/bluebreeze_logo.png' /> <p style='font-size: 18px; font-weight: bold;'>¡Bienvenido al Sistema de Control de Becarios 11!</p> <p style='font-size: 16px; font-style: italic;'>Un proyecto de estudiantes para la Escuela de Ciencias de la Computación e Informática.</p> </div> <div style='padding: 5px 5%; width: 90%;'> <p>Sus credenciales han sido creadas satisfactoriamente. A continuación se le muestra su información para poder ingresar al sistema:</p> <p><b>Nombre:</b>  " + nombre_completo + "</p> <p><b>Usuario:</b>  " + usuario + "</p> <p><b>Contraseña:</b>   " + contrasena + "</p> <p>Para empezar a a utilizar el sistema por favor rediríjase a la siguiente página y revise sus datos personales:</p> <p style='text-align: center;'><a href='#'>aplicaciones/ControlBecarios/</a></p> </div> <div style='padding: 5px 5%; width: 90%; text-align: center;'> <p style='font-size: 16px; font-style: italic;'>Proyecto de Ingeniería de Software II | Grupo 2 | 2013</p> </div> </div> </div></body></html>";
            mail.IsBodyHtml = true;
            mail.To.Add(correo_destino);
            SmtpServer.Send(mail);
        }
        catch (Exception ex)
        {
            return false;
        }

        return true;
    }

    public bool enviarCorreo(String correo_destino, String asunto, String mensaje)
    {
        try
        {
            //Configuración del Mensaje
            MailMessage mail = new MailMessage();
            

            //Especificamos el correo desde el que se enviará el Email y el nombre de la persona que lo envía
            mail.From = new MailAddress(correoSistema, nombreSistema, Encoding.UTF8);

            //Aquí ponemos el asunto del correo
            mail.Subject = asunto;

            //Aquí ponemos el mensaje que incluirá el correo
            mail.Body = mail.Body = "<html><body><div style='width: 80%; margin:0 10%; border-radius: 5px; border: 2px solid #414141; font-size: 16px; background: #2F4F2F;'> <div style='margin: 5%; width: 90%;background: #FFF; border-radius: 5px;'> <div style='padding: 5px 5%; width: 90%; text-align: center;'> <img src='http://www.ecci.ucr.ac.cr/files/bluebreeze_logo.png' /> <p style='font-size: 18px; font-weight: bold;'>Sistema de Control de Becarios 11</p> <p style='font-size: 16px; font-style: italic;'>Un proyecto de estudiantes para estudiantes de la Escuela de Ciencias de la Computación e Informática.</p> </div> <div style='padding: 5px 5%; width: 90%;'> <p> </p> " + mensaje + " </div> <div style='padding: 5px 5%; width: 90%; text-align: center;'> <p style='font-size: 16px; font-style: italic;'>Proyecto de Ingeniería de Software II | Grupo 2 | 2013</p> </div> </div> </div></body></html>";

            // Activar el uso de html para decorar el correo
            mail.IsBodyHtml = true;

            //Especificamos a quien enviaremos el Email, no es necesario que sea Gmail, puede ser cualquier otro proveedor
            mail.To.Add(correo_destino);

            //Si queremos enviar archivos adjuntos tenemos que especificar la ruta en donde se encuentran
            //mail.Attachments.Add(new Attachment(@"C:\Documentos\carta.docx"));

            SmtpServer.Send(mail);
        }
        catch (Exception ex)
        {
            return false;
        }
        return true;
    }
}