<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Reportes.aspx.cs" Inherits="Reportes" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
    <script src="Scripts/Reportes.js" type="text/javascript"></script>
    <link href="Styles/Reportes.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript">
        $(document).ready(function () {
            $("a.level1:contains('Reportes')").addClass("item_active");
        });
    </script>
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
                                        <asp:MenuItem Text="Anotaciones de un Encargado" ToolTip="Reporte de Historial de Asiganciones de un Encargado"></asp:MenuItem>
                                    </asp:MenuItem>
                                </Items>
                            </asp:Menu>
                        </div>

                         <!-- Ayuda -->
                        <div class="ayuda" style="float:right; width:10%; margin-bottom:1%">                        
                            <input type="button" value="Ayuda" class="boton ui-widget ui-state-default ui-corner-all ui-button-text-only" id="Button1" onclick="$('#PopUpAyudaAdmin').dialog('open');" />
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
                                                <div style="float: left; margin-right: 10px; width: 120px;">
                                                    <br />
                                                    <asp:TextBox ID="txtBuscarGeneral" onkeydown = "enterBuscar(event, 'MainContent_btnBuscar');" CssClass="inputElement" runat="server"></asp:TextBox>
                                                </div>

                                                <!-- DRP Criterio 1 -->
                                                <div id="criterio1" style="float: left; margin-right: 10px; width: 180px;">
                                                    <asp:Label ID="lblCriterio1" CssClass="inputElement" runat="server" Text=""></asp:Label>
                                                    <asp:DropDownList CssClass="inputElement" ID="DropDownListCriterio1" runat="server">
                                                    </asp:DropDownList>
                                                </div>

                                                <!-- DRP Criterio 2 -->
                                                <div id="criterio2" style="float: left; margin-right: 10px; width: 180px;">
                                                    <asp:Label ID="lblCriterio2" CssClass="inputElement" runat="server" Text=""></asp:Label>
                                                    <asp:DropDownList CssClass="inputElement" ID="DropDownListCriterio2" runat="server">
                                                    </asp:DropDownList>
                                                </div>

                                                <!-- DRP Criterio 3 -->
                                                <div id="criterio3" style="float: left; margin-right: 10px; width: 180px;">
                                                    <asp:Label ID="lblCriterio3" CssClass="inputElement" runat="server" Text=""></asp:Label>
                                                    <asp:DropDownList CssClass="inputElement" ID="DropDownListCriterio3" runat="server">
                                                    </asp:DropDownList>
                                                </div>

                                                <!-- DRP Criterio 4 -->
                                                <div id="criterio4" style="float: left; margin-right: 10px; width: 180px;">
                                                    <asp:Label ID="lblCriterio4" CssClass="inputElement" runat="server" Text=""></asp:Label>
                                                    <asp:DropDownList CssClass="inputElement" ID="DropDownListCriterio4" runat="server">
                                                    </asp:DropDownList>
                                                </div>

                                                 <!-- DRP Criterio 5 -->
                                                <div id="criterio5" style="float: left; margin-right: 10px; width: 180px;">
                                                    <asp:Label ID="lblCriterio5" CssClass="inputElement" runat="server" Text=""></asp:Label>
                                                    <asp:DropDownList CssClass="inputElement" ID="DropDownListCriterio5" runat="server">
                                                    </asp:DropDownList>
                                                </div>
                                                <!-- Se puede hasta Criterio 5 más o menos ... -->
                                                
                                                <!-- BTN Buscar -->
                                                <div style="float: left; margin-right: 1%;">
                                                    <br />
                                                    <asp:Button ID="btnBuscar" runat="server" Text="Buscar" OnClick="btnBuscar_Click" CausesValidation="false" CssClass="boton ui-widget ui-state-default ui-corner-all ui-button-text-only" />
                                                </div>

                                            </div>
                                        </div>

                                        <!-- Grid -->
                                        <div style="float: left; width: 96%; margin: 0 2%;">
                                            <asp:GridView ID="GridViewReporte" CssClass="table_css" 
                                                GridLines="Both" AllowPaging="True" RowStyle-HorizontalAlign="Center" onpageindexchanging="GridReportes_PageIndexChanging"
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

    <div id="PopUpAyudaAdmin">
        <asp:UpdatePanel runat="server" ID="UpdatePanelAyuda">
            <ContentTemplate>
                <iframe style="width: 99%; height: 500px;" src="HTMLS%20Ayuda/Perfil%20Admin/Reportes/Admin%20-%20Reportes.htm"></iframe>
            </ContentTemplate>
        </asp:UpdatePanel>                
    </div>
</asp:Content>

