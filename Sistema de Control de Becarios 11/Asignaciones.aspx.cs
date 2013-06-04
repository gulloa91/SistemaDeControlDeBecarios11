﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

public partial class Asignaciones : System.Web.UI.Page
{
    private static CommonServices commonService;

    protected void Page_Load(object sender, EventArgs e)
    {
        commonService = new CommonServices(UpdateInfo);
        List<int> permisos = new List<int>();
        permisos = Session["ListaPermisos"] as List<int>;

        // Cuando perfiles esté actualizado con los permisos más recientes entonces vamos a poder
        // mover estos campos a sus respectivos Loads, por el momento todos están juntos, aunque 
        // eso no genera error.
        MultiViewEncargado.ActiveViewIndex = 0;

        if (!IsPostBack)
        {
            llenarGridAsignaciones();
            ListItem noSelectedItem = new ListItem("No seleccionado", "");
            this.DropDownBecariosPopUp.Items.Add(noSelectedItem);
            this.DropDownEncargadosPopUp.Items.Add(noSelectedItem);
            llenarGridaBecariosAsignadosVistaEncargado();
            llenarCicloYAnioVistaEncargados();
            llenarInfoVistaBecario();
        }
        // END TEMP

        /*
        if (permisos == null)
        {
            Session["Nombre"] = "";
            Response.Redirect("~/Default.aspx");
        }
        else
        {
            int permiso = 0; 
            if (permisos.Contains(3))
            {
                permiso = 3;
            }
            else
            {
                if (permisos.Contains(4))
                {
                    permiso = 4;
                }
            }

            switch (permiso)
            {
                case 3: // Vista Completa
                    {
                        MultiViewEncargado.ActiveViewIndex = 0;
                        if (!IsPostBack)
                        {
                            
                        }
                    } break;

                case 4: // Vista Parcial
                    {
                        MultiViewEncargado.ActiveViewIndex = 1;
                        if (!IsPostBack)
                        {
                            
                        }

                    } break;

                default: // Vista sin permiso
                    {
                        MultiViewEncargado.ActiveViewIndex = 2;
                    } break;
            }
        }
         */
    }

    // Aceptar PopUp
    protected void btnInvisibleAceptarAsignacion_Click(object sender, EventArgs e)
    {
        commonService.cerrarPopUp("PopUpAsignacion");

        commonService.mensajeJavascript("La asignación se insertó correctamente", "Insertada");
    }

    // Eliminar
    protected void btnInvisibleEliminarAsignacion_Click(object sender, EventArgs e)
    {
        commonService.cerrarPopUp("PopUpAsignacion");
        commonService.mensajeJavascript("La asignación se eliminó correctamente", "Eliminado"); // Obviamente se tiene que cambiar con el resultado de vd
    }

    // Aceptar Asignación Vista Encargado
    protected void btnInvisibleAceptarAsignacionEncargado_Click(object sender, EventArgs e)
    {
        commonService.cerrarPopUp("PopUpAsignacionEncargado");
        commonService.mensajeJavascript("Se ha aceptado la asignación. Un mensaje se enviará la dirección de la ECCI.", "Aceptada"); // Obviamente se tiene que cambiar con el resultado de vd
    }

    // Rechazar Asignación Vista Encargado
    protected void btnInvisibleRechazarAsignacionEncargado_Click(object sender, EventArgs e)
    {
        commonService.cerrarPopUp("PopUpAsignacionEncargado");
        commonService.mensajeJavascript("Se ha rechazado la asignación. Un mensaje se enviará la dirección de la ECCI.", "Rechazada"); // Obviamente se tiene que cambiar con el resultado de vd
    }

    // BUSCAR CLICK
    protected void btnBuscar_Click(object sender, EventArgs e)
    {
       
    }

    // Abrir PopUp Insertar Asignacion
    protected void btnInsertarAsignacion_Click(object sender, EventArgs e)
    {
        commonService.abrirPopUp("PopUpAsignacion", "Insertar Nueva Asignación");
        mostrarBotonesPrincipales(false);
        habilitarContenidoAsignacion(true);
        limpiarContenidoAsignacion();
        commonService.mostrarPrimerBotonDePopUp("PopUpAsignacion");
        int cantidadDeBecariosAsignados = 3; // Get cantidad de becarios asigandos a encargado
        this.lblCiclo.Text = "I";
        this.lblAnio.Text = "2013";
        this.btnCantidadBecariosDeEncargado.Text = "Becarios asignados: " + cantidadDeBecariosAsignados.ToString();
    }

    // Abrir PopUp Eliminar
    protected void btnEliminarAsignacion_Click(object sender, EventArgs e)
    {
        commonService.abrirPopUp("PopUpEliminarAsignacion", "Eliminar Asignación");
    }

    // Seleccionar Modificar en el PopUp
    protected void btnModificarAsignacion_Click(object sender, EventArgs e)
    {
        habilitarContenidoAsignacion(true);
        this.DropDownBecariosPopUp.Enabled = false; // Tenemos que deshabilitarlo a pata
        commonService.correrJavascript("$('#PopUpAsignacion').dialog('option', 'title', 'Modificar Asignación');");
        commonService.mostrarPrimerBotonDePopUp("PopUpAsignacion");
    }

    // Seleccionar el de ver becarios asignados a un encargado
    protected void btnCantidadBecariosDeEncargado_Click(object sender, EventArgs e)
    {
        String nombreDeEncargado = "Becarios asignados a: Gabriel Ulloa Murillo"; // Get nombre de encargado
        commonService.abrirPopUp("PopUpVerBecariosAsignados", nombreDeEncargado);
        llenarGridaBecariosAsigandosAEncargado();
    }

    // Seleccionar tupla del grid con la flecha
    protected void GridAsignaciones_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        switch (e.CommandName)
        {
            // Consultar tupla
            case "btnSeleccionarTupla_Click":
                {
                    commonService.abrirPopUp("PopUpAsignacion", "Consultar Asignación");
                    mostrarBotonesPrincipales(true);
                    habilitarContenidoAsignacion(false);
                    commonService.esconderPrimerBotonDePopUp("PopUpAsignacion");
                    int cantidadDeBecariosAsignados = 3; // Get cantidad de becarios asigandos a encargado
                    this.lblCiclo.Text = "I";
                    this.lblAnio.Text = "2013";
                    this.btnCantidadBecariosDeEncargado.Text = "Becarios asignados: " + cantidadDeBecariosAsignados.ToString();
                } break;
        }
    }

    // Grid vista Encargados
    protected void GridBecariosAsignadosVistaEncargado_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        switch (e.CommandName)
        {
            // Abrir Pop Up aceptar/rechazar asignación vista encargado
            case "btnSeleccionarTupla_Click":
                {
                    commonService.abrirPopUp("PopUpAsignacionEncargado", "Aceptar/Rechazar Asignación");
                    this.lblNombreBecarioPopUpVistaEncargado.Text = "José Perez";
                    this.lblCicloBecarioPopUpVistaEncargado.Text = "I";
                    this.lblAnioBecarioPopUpVistaEncargado.Text = "2013";
                    this.lblHorasBecarioPopUpVistaEncargado.Text = "73";
                } break;
        }
    }

    //Define que botones se van a mostrar
    protected void mostrarBotonesPrincipales(Boolean mostrar)
    {
        if (mostrar)
        {
            this.btnModificarAsignacion.Visible = true;
            this.btnEliminarAsignacion.Visible = true;
        }
        else
        {
            this.btnModificarAsignacion.Visible = false;
            this.btnEliminarAsignacion.Visible = false;
        }
    }

    protected void habilitarContenidoAsignacion(Boolean mostrar)
    {
        if (mostrar)
        {
            this.DropDownBecariosPopUp.Enabled = true;
            this.DropDownEncargadosPopUp.Enabled = true;
            this.txtUnidAcademica.Enabled = true;
            this.txtInfoDeUbicacion.Enabled = true;
            this.txtTotalHoras.Enabled = true;
        }
        else
        {
            this.DropDownBecariosPopUp.Enabled = false;
            this.DropDownEncargadosPopUp.Enabled = false;
            this.txtUnidAcademica.Enabled = false;
            this.txtInfoDeUbicacion.Enabled = false;
            this.txtTotalHoras.Enabled = false;
        }
    }

    protected void limpiarContenidoAsignacion()
    {
        this.DropDownBecariosPopUp.Text = "";
        this.DropDownEncargadosPopUp.Text = "";
        this.txtUnidAcademica.Text = "";
        this.txtInfoDeUbicacion.Text = "";
        this.txtTotalHoras.Text = "";
    }

    protected void llenarCicloYAnioVistaEncargados()
    {
        this.lblCicloPrincipalVistaEncargado.Text = "I";
        this.lblAnioPrincipalVistaEncargado.Text = "2013";
    }

    // Llenar tabla con todas las asignaciones
    protected void llenarGridAsignaciones() 
    {
    
        DataTable tablaAsignaciones = crearTablaAsignaciones();
        DataRow newRow;
        /*
        if (lsEncargados.Count > 0)
        {
            for (int i = 0; i < lsEncargados.Count; ++i)
            {
                newRow = tablaAsignaciones.NewRow();
                newRow["Nombre"] = lsEncargados[i].Nombre + " " + lsEncargados[i].Apellido1 + " " + lsEncargados[i].Apellido2;
                newRow["Cedula"] = lsEncargados[i].Cedula;
                newRow["Correo"] = lsEncargados[i].Correo;
                newRow["Celular"] = lsEncargados[i].TelefonoCelular;
                if (lsEncargados[i].TelefonoFijo != "")
                {
                    newRow["Telefono"] = lsEncargados[i].TelefonoFijo;
                }
                else
                {
                    if (lsEncargados[i].OtroTelefono != "")
                    {
                        newRow["Telefono"] = lsEncargados[i].OtroTelefono;
                    }
                }

                tablaAsignaciones.Rows.InsertAt(newRow, i);
            }
        }
        else
        {
         */
            newRow = tablaAsignaciones.NewRow();
            newRow["Encargado"] = "-";
            newRow["Becario"] = "-";
            newRow["Ciclo"] = "-";
            newRow["Año"] = "-";
            newRow["Estado"] = "-";

            tablaAsignaciones.Rows.InsertAt(newRow, 0);
        //}
        this.GridAsignaciones.DataSource = tablaAsignaciones;
        this.GridAsignaciones.DataBind();
        this.HeadersCorrectosAsignaciones();
    }

    // Llenar tabla con todas las asignaciones
    protected void llenarGridaBecariosAsigandosAEncargado()
    {

        DataTable tablaBecariosAsigandosAEncargado = crearTablaBecariosAsigandosAEncargado();
        DataRow newRow;
        /*
        if (lsEncargados.Count > 0)
        {
            for (int i = 0; i < lsEncargados.Count; ++i)
            {
                newRow = tablaAsignaciones.NewRow();
                newRow["Nombre"] = lsEncargados[i].Nombre + " " + lsEncargados[i].Apellido1 + " " + lsEncargados[i].Apellido2;
                newRow["Cedula"] = lsEncargados[i].Cedula;
                newRow["Correo"] = lsEncargados[i].Correo;
                newRow["Celular"] = lsEncargados[i].TelefonoCelular;
                if (lsEncargados[i].TelefonoFijo != "")
                {
                    newRow["Telefono"] = lsEncargados[i].TelefonoFijo;
                }
                else
                {
                    if (lsEncargados[i].OtroTelefono != "")
                    {
                        newRow["Telefono"] = lsEncargados[i].OtroTelefono;
                    }
                }

                tablaAsignaciones.Rows.InsertAt(newRow, i);
            }
        }
        else
        {
         */
        newRow = tablaBecariosAsigandosAEncargado.NewRow();
        newRow["Nombre"] = "-";
        newRow["Carné"] = "-";
        newRow["Correo"] = "-";
        newRow["Celular"] = "-";

        tablaBecariosAsigandosAEncargado.Rows.InsertAt(newRow, 0);

        //}
        GridBecariosAsignadosAEncargado.DataSource = tablaBecariosAsigandosAEncargado;
        GridBecariosAsignadosAEncargado.DataBind();
        this.HeadersCorrectosBecariosAsigandosAEncargado();
    }

    // Llenar tabla con todas las asignaciones
    protected void llenarGridaBecariosAsignadosVistaEncargado()
    {

        DataTable tablaBecariosAsignadosVistaEncargado = crearTablaBecariosAsignadosVistaEncargado();
        DataRow newRow;
        /*
        if (lsEncargados.Count > 0)
        {
            for (int i = 0; i < lsEncargados.Count; ++i)
            {
                newRow = tablaAsignaciones.NewRow();
                newRow["Nombre"] = lsEncargados[i].Nombre + " " + lsEncargados[i].Apellido1 + " " + lsEncargados[i].Apellido2;
                newRow["Cedula"] = lsEncargados[i].Cedula;
                newRow["Correo"] = lsEncargados[i].Correo;
                newRow["Celular"] = lsEncargados[i].TelefonoCelular;
                if (lsEncargados[i].TelefonoFijo != "")
                {
                    newRow["Telefono"] = lsEncargados[i].TelefonoFijo;
                }
                else
                {
                    if (lsEncargados[i].OtroTelefono != "")
                    {
                        newRow["Telefono"] = lsEncargados[i].OtroTelefono;
                    }
                }

                tablaAsignaciones.Rows.InsertAt(newRow, i);
            }
        }
        else
        {
         */
        newRow = tablaBecariosAsignadosVistaEncargado.NewRow();
        newRow["Nombre"] = "-";
        newRow["Carné"] = "-";
        newRow["Correo"] = "-";
        newRow["Celular"] = "-";
        newRow["Estado"] = "-";

        tablaBecariosAsignadosVistaEncargado.Rows.InsertAt(newRow, 0);

        //}
        GridBecariosAsignadosVistaEncargado.DataSource = tablaBecariosAsignadosVistaEncargado;
        GridBecariosAsignadosVistaEncargado.DataBind();
        this.HeadersCorrectosBecariosAsignadosVistaEncargado();
    }

    // Le da formato a las columnas del Grid
    protected DataTable crearTablaAsignaciones()
    {

        DataTable dt = new DataTable();
        DataColumn column;

        column = new DataColumn();
        column.DataType = System.Type.GetType("System.String");
        column.ColumnName = "Encargado";
        dt.Columns.Add(column);

        column = new DataColumn();
        column.DataType = System.Type.GetType("System.String");
        column.ColumnName = "Becario";
        dt.Columns.Add(column);

        column = new DataColumn();
        column.DataType = System.Type.GetType("System.String");
        column.ColumnName = "Ciclo";
        dt.Columns.Add(column);

        column = new DataColumn();
        column.DataType = System.Type.GetType("System.String");
        column.ColumnName = "Año";
        dt.Columns.Add(column);

        column = new DataColumn();
        column.DataType = System.Type.GetType("System.String");
        column.ColumnName = "Estado";
        dt.Columns.Add(column);


        return dt;
    }

    // Le da formato a las columnas del Grid
    protected DataTable crearTablaBecariosAsigandosAEncargado()
    {

        DataTable dt = new DataTable();
        DataColumn column;

        column = new DataColumn();
        column.DataType = System.Type.GetType("System.String");
        column.ColumnName = "Nombre";
        dt.Columns.Add(column);

        column = new DataColumn();
        column.DataType = System.Type.GetType("System.String");
        column.ColumnName = "Carné";
        dt.Columns.Add(column);

        column = new DataColumn();
        column.DataType = System.Type.GetType("System.String");
        column.ColumnName = "Correo";
        dt.Columns.Add(column);

        column = new DataColumn();
        column.DataType = System.Type.GetType("System.String");
        column.ColumnName = "Celular";
        dt.Columns.Add(column);

        return dt;
    }

    // Le da formato a las columnas del Grid
    protected DataTable crearTablaBecariosAsignadosVistaEncargado()
    {

        DataTable dt = new DataTable();
        DataColumn column;

        column = new DataColumn();
        column.DataType = System.Type.GetType("System.String");
        column.ColumnName = "Nombre";
        dt.Columns.Add(column);

        column = new DataColumn();
        column.DataType = System.Type.GetType("System.String");
        column.ColumnName = "Carné";
        dt.Columns.Add(column);

        column = new DataColumn();
        column.DataType = System.Type.GetType("System.String");
        column.ColumnName = "Correo";
        dt.Columns.Add(column);

        column = new DataColumn();
        column.DataType = System.Type.GetType("System.String");
        column.ColumnName = "Celular";
        dt.Columns.Add(column);

        column = new DataColumn();
        column.DataType = System.Type.GetType("System.String");
        column.ColumnName = "Estado";
        dt.Columns.Add(column);

        return dt;
    }
    // Aplica nombre a las columnas así como color
    private void HeadersCorrectosAsignaciones()
    {
        this.GridAsignaciones.HeaderRow.BackColor = System.Drawing.Color.FromArgb(4562432);
        this.GridAsignaciones.HeaderRow.ForeColor = System.Drawing.Color.White;
        this.GridAsignaciones.HeaderRow.Cells[1].Text = "Encargado";
        this.GridAsignaciones.HeaderRow.Cells[2].Text = "Becario";
        this.GridAsignaciones.HeaderRow.Cells[3].Text = "Ciclo";
        this.GridAsignaciones.HeaderRow.Cells[4].Text = "Año";
        this.GridAsignaciones.HeaderRow.Cells[5].Text = "Estado";
    }
    
    // Aplica nombre a las columnas así como color
    private void HeadersCorrectosBecariosAsigandosAEncargado()
    {
        GridBecariosAsignadosAEncargado.HeaderRow.BackColor = System.Drawing.Color.FromArgb(4562432);
        GridBecariosAsignadosAEncargado.HeaderRow.ForeColor = System.Drawing.Color.White;
        GridBecariosAsignadosAEncargado.HeaderRow.Cells[0].Text = "Nombre";
        GridBecariosAsignadosAEncargado.HeaderRow.Cells[1].Text = "Carné";
        GridBecariosAsignadosAEncargado.HeaderRow.Cells[2].Text = "Correo";
        GridBecariosAsignadosAEncargado.HeaderRow.Cells[3].Text = "Celular";
    }

    // Aplica nombre a las columnas así como color
    private void HeadersCorrectosBecariosAsignadosVistaEncargado()
    {
        GridBecariosAsignadosVistaEncargado.HeaderRow.BackColor = System.Drawing.Color.FromArgb(4562432);
        GridBecariosAsignadosVistaEncargado.HeaderRow.ForeColor = System.Drawing.Color.White;
        GridBecariosAsignadosVistaEncargado.HeaderRow.Cells[1].Text = "Nombre";
        GridBecariosAsignadosVistaEncargado.HeaderRow.Cells[2].Text = "Carné";
        GridBecariosAsignadosVistaEncargado.HeaderRow.Cells[3].Text = "Correo";
        GridBecariosAsignadosVistaEncargado.HeaderRow.Cells[4].Text = "Celular";
        GridBecariosAsignadosVistaEncargado.HeaderRow.Cells[5].Text = "Estado";
    }

    /**********************************************************************************************************
     * 
     *                                          VISTA BECARIO
     * 
     * ********************************************************************************************************/
    protected void llenarInfoVistaBecario()
    {
        this.lblAnioVistaBecario.Text = "2013";
        this.lblCicloVistaBecario.Text = "I";
        this.lblEncargadoVistaBecario.Text = "Gabriel Ulloa Murillo";
        this.lblHorasVistaBecario.Text = "73";
    }


    // Aceptar asignación
    protected void btnAceptarAsignacionBecario_Click(object sender, EventArgs e)
    {
        commonService.mensajeJavascript("Usted ha aceptado la asignación satisfactoriamente", "Aceptado");
        esconderBotonesVistaBecario(true);
    }

    // Abrir confirmación de rechazo de asignación
    protected void btnCancelarAsignacionBecario_Click(object sender, EventArgs e)
    {
        commonService.abrirPopUp("PopUpConfirmarRechazoBecario", "Rechazar Asignación");
    }

    // Confirmar rechazo
    protected void btnInvisibleConfirmarRechazo_Click(object sender, EventArgs e)
    {
        commonService.cerrarPopUp("PopUpConfirmarRechazoBecario");
        commonService.mensajeJavascript("¡Su rechazo ha sido procesado satisfactoriamente!","Rechazo procesado");
        esconderBotonesVistaBecario(true);
    }

    protected void esconderBotonesVistaBecario(Boolean esconder)
    {
        if (esconder)
        {
            this.btnAceptarAsignacionBecario.Visible = false;
            this.btnCancelarAsignacionBecario.Visible = false;
        }
        else
        {
            this.btnAceptarAsignacionBecario.Visible = true;
            this.btnCancelarAsignacionBecario.Visible = true;
        }
    }
}