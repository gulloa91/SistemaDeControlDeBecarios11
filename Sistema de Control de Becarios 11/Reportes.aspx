<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Reportes.aspx.cs" Inherits="Reportes" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
    <link href="Styles/Reportes.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <asp:ScriptManager ID="ScriptManager" runat="server">
    </asp:ScriptManager>
    <asp:MultiView ID="MultiViewReportes" runat="server">

        <!-- Consulta de Encargados relacionados con ese Becario -->
        <asp:View ID="VistaReportes" runat="server">

            <asp:UpdatePanel ID="UpdateInfo" runat="server">
                <Triggers>
            
                </Triggers>

                <ContentTemplate>
                    <!-- Botones Invisibles -->
                    <asp:Button ID="btnInvisible1" 
                        CssClass="btnInvisible1 invisible" runat="server" Text="" />

                    <!-- Cuerpo -->
                    <div style="width: 100%; float: left;">

                        <!-- Título -->
                        <span style="width: 100%; font-weight: bold; font-size: 24px; float: left; margin: 20px 0 5px 0; text-align:center;">Módulo de Reportes</span>
                        <span style="width: 100%; font-weight: normal; font-style:italic; font-size: 16px; float: left; margin: 5px 0 20px 0; text-align:center; border-bottom: 2px solid #414141; padding-bottom: 5px;">Por favor seleccione del menu el reporte que desee ver.</span>
                    
                        <!-- Menu reportes -->
                        <div id="Menu_Container" style="width: 15%; float: left;">
                            <asp:Menu ID="MenuListaReportes" CssClass="menu_reportes" runat="server" 
                                onmenuitemclick="MenuListaReportes_MenuItemClick">
                                <Items>
                                    <asp:MenuItem Text="Becarios">
                                        <asp:MenuItem Text="Con horas finalizadas"></asp:MenuItem>
                                        <asp:MenuItem Text="No asignados"></asp:MenuItem>
                                        <asp:MenuItem Text="Asignados por Unidad Académica"></asp:MenuItem>
                                        <asp:MenuItem Text="Reporte de Actividad"></asp:MenuItem>
                                    </asp:MenuItem>
                                    <asp:MenuItem Text="Encargados">
                                        <asp:MenuItem Text="Reporte de Actividad"></asp:MenuItem>
                                    </asp:MenuItem>
                                    <asp:MenuItem Text="Historiales">
                                        <asp:MenuItem Text="Asignaciones de un Becario"></asp:MenuItem>
                                        <asp:MenuItem Text="Anotaciones de un Encargado"></asp:MenuItem>
                                    </asp:MenuItem>
                                </Items>
                            </asp:Menu>
                        </div>

                        <!-- Ventana reportes -->
                        <div style="margin-left: 5%; width: 85%; float: left;"></div>
                    </div>

                </ContentTemplate>
            </asp:UpdatePanel>            
        </asp:View>

        <!-- Sin acceso al módulo -->
        <asp:View ID="VistaSinPermiso" runat="server">
            <h2 style="color: Red; text-align:center;">Lo sentimos. Usted no tiene acceso a esta sección.</h2>
        </asp:View>

    </asp:MultiView>
</asp:Content>

