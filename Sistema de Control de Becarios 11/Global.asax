<%@ Application Language="C#" %>

<script runat="server">

    void Application_Start(object sender, EventArgs e) 
    {
        // Code that runs on application startup

    }
    
    void Application_End(object sender, EventArgs e) 
    {
        //  Code that runs on application shutdown

    }
        
    void Application_Error(object sender, EventArgs e) 
    { 
        // Code that runs when an unhandled error occurs

    }

    void Session_Start(object sender, EventArgs e) 
    {
        // Code that runs when a new session is started
        Session["Nombre"] = "";
        Session["Cedula"] = "";
        Session["Apellido1"] = "";
        Session["ListaPermisos"] = "";
        Session["Cuenta"] = "";
        Session["TipoPerfil"] = -1;
        Session["ImageUrl"] = "";
        int mes = DateTime.Now.Month;
        switch(mes){
            case 1: Session["Periodo"] = "3";
                break;
            case 2: Session["Periodo"] = "3";
                break;
            case 3: Session["Periodo"] = "1";
                break;
            case 4: Session["Periodo"] = "1";
                break;
            case 5: Session["Periodo"] = "1";
                break;
            case 6: Session["Periodo"] = "1";
                break;
            case 7: Session["Periodo"] = "1";
                break;
            case 8: Session["Periodo"] = "2";
                break;
            case 9: Session["Periodo"] = "2";
                break;
            case 10: Session["Periodo"] = "2";
                break;
            case 11: Session["Periodo"] = "2";
                break;
            case 12: Session["Periodo"] = "2";
                break;
            default: break;
        }
    }

    void Session_End(object sender, EventArgs e) 
    {
        // Code that runs when a session ends. 
        // Note: The Session_End event is raised only when the sessionstate mode
        // is set to InProc in the Web.config file. If session mode is set to StateServer 
        // or SQLServer, the event is not raised.

    }
       
</script>
