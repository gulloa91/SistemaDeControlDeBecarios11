﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using DataSetCuentasTableAdapters;

public partial class SiteMaster : System.Web.UI.MasterPage
{
	private ControladoraCuentas controladoraCuentas = new ControladoraCuentas();
	private ControladoraPerfiles controladoraPerfiles = new ControladoraPerfiles();
    private ControladoraBecarios controladoraBecarios = new ControladoraBecarios();
    private ControladoraEncargado controladoraEncargados = new ControladoraEncargado();
    private ControladoraControlEncargado controladoraControlEncargado = new ControladoraControlEncargado();

	protected void Page_Load(object sender, EventArgs e)
	{
		//if (!IsPostBack)
		//{
			if (String.Compare("", (string)Session["Nombre"]) == 0)
			{
				MultiViewSiteMaster.SetActiveView(VistaLogin);
			}
			else
			{
				this.lblCerrarSesion.Text = "| Bienvenid@, " + Session["Nombre"].ToString() + " " + Session["Apellido1"].ToString();
				MultiViewSiteMaster.SetActiveView(VistaPrincipal);

                List<int> permisos = new List<int>();
                permisos = Session["ListaPermisos"] as List<int>;


                /** Crear Menu dinamicamente **/
                MenuItem InicioMenuItem = new MenuItem("Inicio", "m0", "", "~/Default.aspx");
                NavigationMenu.Items.Add(InicioMenuItem);

                foreach (int permiso in permisos)
                {
                    switch( permiso )
                    {
                        case 1: // BC 
                            {
                                MenuItem BecariosCompletoMenuItem = new MenuItem("Becarios", "m3", "", "~/Becarios.aspx");
                                NavigationMenu.Items.Add(BecariosCompletoMenuItem);
                            } break;

                        case 2: // BP
                            {
                                MenuItem BecariosParcialMenuItem = new MenuItem("Información Personal", "m4", "", "~/Becarios.aspx");
                                NavigationMenu.Items.Add(BecariosParcialMenuItem);
                            } break;

                        case 3: // EC
                            {
                                MenuItem EncargadosCompletoMenuItem = new MenuItem("Encargados", "m1", "", "~/Encargados.aspx");
                                NavigationMenu.Items.Add(EncargadosCompletoMenuItem);
                            } break;

                        case 4: // EP
                            {
                                MenuItem EncargadosParcialMenuItem = new MenuItem("Información Personal", "m2", "", "~/Encargados.aspx");
                                NavigationMenu.Items.Add(EncargadosParcialMenuItem);
                            } break;

                        case 5: // Cont Becario
                            {
                                MenuItem CtrlBecarioMenuItem = new MenuItem("Reportar Horas", "m0", "", "~/ControlDeHorasBecario.aspx");
                                NavigationMenu.Items.Add(CtrlBecarioMenuItem);
                            } break;

                        case 6: // Cont Encargado
                            {
                                int horasPendientesRevision = controladoraControlEncargado.totalBecarios((string)(Session["Cedula"]),0);
                                MenuItem CtrlEncargadoMenuItem = new MenuItem("Revisar Horas (" + horasPendientesRevision.ToString() + ")", "revisarHorasBecario", "", "~/ControlDeHorasEncargado.aspx");
                                NavigationMenu.Items.Add(CtrlEncargadoMenuItem);
                            } break;

                        case 7: // Reportes
                            {
                                MenuItem ReportesMenuItem = new MenuItem("Reportes", "m0", "", "~/Reportes.aspx");
                                NavigationMenu.Items.Add(ReportesMenuItem); 
                            } break;

                        case 8: // Asig. Comp
                            {
                                MenuItem AsignacionesMenuItem = new MenuItem("Asignaciones", "m0", "", "~/Asignaciones.aspx");
                                NavigationMenu.Items.Add(AsignacionesMenuItem);
                            } break;

                        case 9: // Asig. Encargado
                            {
                                MenuItem AsignacionesMenuItem = new MenuItem("Becarios Asignados", "m0", "", "~/Asignaciones.aspx");
                                NavigationMenu.Items.Add(AsignacionesMenuItem);
                            } break;

                        case 10: // Asig. Becario
                            {
                                MenuItem AsignacionesMenuItem = new MenuItem("Encargado Asignado", "m0", "", "~/Asignaciones.aspx");
                                NavigationMenu.Items.Add(AsignacionesMenuItem);
                            } break;

                        case 11: // CC
                            {
                                MenuItem CuentasCompletoMenuItem = new MenuItem("Cuentas", "m5", "", "~/Cuentas.aspx");
                                NavigationMenu.Items.Add(CuentasCompletoMenuItem);
                            } break;

                        case 12: // CP
                            {
                                MenuItem CuentasParcialMenuItem = new MenuItem("Información de Cuenta", "m6", "", "~/Cuentas.aspx");
                                NavigationMenu.Items.Add(CuentasParcialMenuItem);
                            } break;

                        case 13: // P
                            {
                                MenuItem PerfilesMenuItem = new MenuItem("Perfiles", "m7", "", "~/Perfiles.aspx");
                                NavigationMenu.Items.Add(PerfilesMenuItem);
                            } break;
                        
                    }
                }
            }
		//}
	}

	protected void Click_Aceptar(object sender, EventArgs e)
	{
		//MultiViewSiteMaster.SetActiveView(VistaPrincipal);
		Boolean usuarioValido = controladoraCuentas.validarUsuario(this.txtUsuario.Text, this.txtContrasena.Text);
        Session["UltimoAcceso"] = DateTime.Now;

		if (usuarioValido)
		{
			string nombre = "desconocido";
			string apellido1 = "";
            string cedulaUsuario = controladoraCuentas.getCedulaByUsuario(this.txtUsuario.Text );
            string perfil = controladoraCuentas.getPerfilByCuenta(this.txtUsuario.Text);
            int tipoPerfil = Convert.ToInt32(controladoraPerfiles.tipoPerfil(perfil));

            List<int> listaPermisos = controladoraPerfiles.obtenerPermisosUsuario(perfil);
            int tipoUsuario = Convert.ToInt32(controladoraPerfiles.tipoPerfil(perfil));

			Session["ListaPermisos"] = listaPermisos;
            Session["Cuenta"] = this.txtUsuario.Text;
            Session["TipoPerfil"] = tipoPerfil;
			controladoraCuentas.actualizarFechaIngresoCuenta((DateTime) Session["UltimoAcceso"], this.txtUsuario.Text);

			if ( tipoUsuario == 1)
			{
				Encargado encargado = controladoraEncargados.obtenerEncargadoPorCedula(cedulaUsuario);
				nombre = encargado.Nombre;
				apellido1 = encargado.Apellido1;
			}
                
			else
			{
                if (tipoUsuario == 2)
                {
                    Becario becario = controladoraBecarios.obtenerBecarioPorCedula(cedulaUsuario);
                    nombre = becario.nombre;
                    apellido1 = becario.apellido1;
                }
                else
                {

                    if (tipoUsuario == 0)
                    {
                        nombre = this.txtUsuario.Text;
                        apellido1 = "";
                    }
                }
			}

			Session["Nombre"] = nombre;
			Session["Apellido1"] = apellido1;
            Session["Cedula"] = cedulaUsuario;
			Response.Redirect("~/Default.aspx");
		}
		else {
            this.lblErrorUsuario.Visible = true;
		}
	}

	protected void btnCerrarSesion_Click(object sender, EventArgs e)
	{
        Session["Nombre"] = "";
		Response.Redirect("~/Default.aspx");
	}
}
