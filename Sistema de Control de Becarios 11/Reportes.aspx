﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Reportes.aspx.cs" Inherits="Reportes" %>

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
                        <span style="width: 100%; font-weight: normal; font-style:italic; font-size: 16px; float: left; margin: 5px 0 20px 0; text-align:center; border-bottom: 2px solid #414141; padding-bottom: 5px;">Le permite ver diferentes reportes de la actividad entre encargados y becarios.</span>
                    
                        <!-- Menu reportes -->
                        <div id="Menu_Container" style="width: 15%; float: left;">
                            <asp:Menu ID="MenuListaReportes" CssClass="menu_reportes" runat="server" 
                                onmenuitemclick="MenuListaReportes_MenuItemClick" SkipLinkText="">
                                <Items>
                                    <asp:MenuItem Text="Becarios">
                                        <asp:MenuItem Text="Con horas finalizadas" ToolTip="Consultar Becarios que han finalizado sus horas"></asp:MenuItem>
                                        <asp:MenuItem Text="No asignados" ToolTip="Reporte de becarios no asignados en un Semestre y Año que si fueron asignados en el semestre anterior o tras-anterior"></asp:MenuItem>
                                        <asp:MenuItem Text="Asignados por Unidad Académica" ToolTip="Reporte de Estudiantes Asignados, por Unidad Académica"></asp:MenuItem>
                                        <asp:MenuItem Text="Reporte de Actividad" ToolTip="Reporte de estudiantes que no han reportado horas en un lapso, con fecha de último reporte"></asp:MenuItem>
                                    </asp:MenuItem>
                                    <asp:MenuItem Text="Encargados">
                                        <asp:MenuItem Text="Reporte de Actividad" ToolTip="Reporte de encargados con aprobaciones pendientes con más de un mes de atraso"></asp:MenuItem>
                                    </asp:MenuItem>
                                    <asp:MenuItem Text="Historiales">
                                        <asp:MenuItem Text="Asignaciones de un Becario" ToolTip="Reporte de Historial de Asignaciones de un Becario"></asp:MenuItem>
                                        <asp:MenuItem Text="Anotaciones de un Encargado" ToolTip="Reporte de Historial de Anotaciones que hace un Encargado"></asp:MenuItem>
                                    </asp:MenuItem>
                                </Items>
                            </asp:Menu>
                        </div>

                        <!-- Ventana reportes -->
                        <asp:UpdatePanel ID="UpdatePanelInformacionReportes" runat="server">
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="MenuListaReportes" EventName="menuitemclick" />
                            </Triggers>
                            <ContentTemplate>
                                <div style="margin-left: 2%; width: 81%; float: left; border: 2px solid #414141; border-radius: 5px; background: #D8D8BF; padding-bottom: 2%;">
                                    <!-- Tipo de reporte -->
                                    <asp:Label ID="lblReporteActivo" runat="server" CssClass="lblReporteActivo" Text="Por favor seleccione del menu el reporte que desee ver."></asp:Label>

                                    <div id="wrapperDeLaInfo" style="display:none;">

                                        <!-- Criterios de Búsqueda -->
                                        <div style="float: left; width: 96%; padding: 2%;">
                                            <div style="float: left;">

                                                <div style="width: 100%; float:left; font-weight: bold; font-size: 16px; border-bottom: 1px solid #fff; margin-bottom: 5px;">Buscar:</div>
                                                <!-- TXT Buscar -->
                                                <div style="float: left; margin-right: 1%;">
                                                    <asp:TextBox ID="txtBuscarGeneral"  onkeydown = "enterBuscar(event, 'MainContent_txtBuscarGeneral');" CssClass="txtAsignacion" runat="server"></asp:TextBox>
                                                </div>

                                                <!-- BTN Buscar -->
                                                <div style="float: left; margin-right: 1%;">
                                                    <asp:Button ID="btnBuscar" runat="server" Text="Buscar" CausesValidation="false" CssClass="boton ui-widget ui-state-default ui-corner-all ui-button-text-only" />
                                                </div>

                                            </div>
                                        </div>

                                        <!-- Grid -->
                                        <div style="float: left; width: 96%; margin: 0 2%;">
                                            <asp:GridView ID="GridViewReporte" CssClass="table_css" 
                                            GridLines="Both" AllowPaging="True" RowStyle-HorizontalAlign="Center" 
                                            RowStyle-VerticalAlign="Middle" PageSize="15" 
                                            PagerStyle-CssClass="pagerGlobal"  runat="server">
                                            </asp:GridView>
                                        </div>
                                    </div>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
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

