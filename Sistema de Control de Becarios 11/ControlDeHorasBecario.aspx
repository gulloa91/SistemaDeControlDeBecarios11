<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeFile="ControlDeHorasBecario.aspx.cs" Inherits="ControlDeHoras" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
    <link href="Styles/ControlDeHorasBecario.css" rel="stylesheet" type="text/css" />
    <script src="Scripts/ControlDeHorasBecario.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            $("a.level1:contains('Reportar Horas')").addClass("item_active");
        });
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <asp:ScriptManager ID="ScriptManager" runat="server">
    </asp:ScriptManager>

    <asp:MultiView ID="MultiViewBecario" runat="server">

        <!--Por si la asignacion no esta aceptada o esta finalizada-->
        <asp:View ID="View1" runat="server">
            <asp:UpdatePanel ID="panelVacio" runat="server">
                <Triggers>
                </Triggers>                
                    
                <ContentTemplate>
                    <asp:Button ID="btnInvisibleAsignacion" ValidationGroup="asig" CssClass="invisible btnInvisibleAsig"
                        runat="server" Text="Button" OnClick="btnInvisibleAsignacion_Click" />

                    <!-- Ayuda -->
                    <div class="Ayuda" style="float:right">     
                        <input type="button" value="Ayuda" class="boton ui-widget ui-state-default ui-corner-all ui-button-text-only" id="Button1" onclick="$('#PopUpAyudaParcialBecario').dialog('open');" />
                    </div> 

                    <h2 style="color: Red; text-align: center;">La asignación para el presente periodo no está aceptada aun, o está finalizada.</h2>
                    
                </ContentTemplate>
            </asp:UpdatePanel>
            <div id="siguienteAsig" style="height: 176px; margin: 20px;">
                <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                    <Triggers>
                    </Triggers>
                    <ContentTemplate>
                        <!-- Ayuda -->
                        <div class="Ayuda" style="float:right">     
                            <input type="button" value="Ayuda" class="boton ui-widget ui-state-default ui-corner-all ui-button-text-only" id="Button2" onclick="$('#PopUpAyudaParcialEncargado').dialog('open');" />
                        </div> 
                        <div style="width: 100%; float: left;">
                            ¿Desea continuar con el mismo Encargado el siguiente periodo?
                        </div>
                        <div style="width: 100%; float: left;">
                            <asp:RadioButton ID="radioSi" Text="Si" runat="server" GroupName="radios" />
                            <asp:RadioButton ID="radioNo" Text="No" runat="server" GroupName="radios" />
                        </div>
                        <div style="width: 100%; float: left;">
                            <asp:TextBox ID="txtComentFin" Width="95%" CssClass="comentarioCHB" runat="server" TextMode="MultiLine"></asp:TextBox>
                        </div>
                        <div style="width: 100%; float: left;">
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator3" ValidationGroup="asig" runat="server" ErrorMessage="Comentario Requerido" ControlToValidate="txtComentFin" Font-Bold="True" ForeColor="Red"></asp:RequiredFieldValidator>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </asp:View>

        <!-- IMEC de todas las asignaciones -->
        <asp:View ID="VistaAdmin" runat="server">
            <asp:UpdatePanel ID="UpdateInfo" runat="server">
                <Triggers>
                </Triggers>
                <ContentTemplate>
                    <!-- Botones Invisibles -->
                    <asp:Button ID="btnInvisibleEnviarReporte" ValidationGroup="NuevoReporte" CssClass="invisible btnInvisibleEnviarReporte"
                        runat="server" Text="Button" OnClick="btnInvisibleEnviarReporte_Click" />
                    
                    <!-- Cuerpo -->
                    <div style="min-height: 500px;">
                        <!-- Título -->
                        <span style="width: 100%; font-weight: bold; font-size: 24px; float: left; margin: 20px 0 5px 0; text-align: center;">Módulo de Control de Horas para Becarios</span> <span style="width: 100%; font-weight: normal; font-style: italic; font-size: 16px; float: left; margin: 5px 0 20px 0; text-align: center; border-bottom: 2px solid #414141; padding-bottom: 5px;">Por
                                medio de este módulo puede ver el detalle de todos los reportes de horas hechos
                                hasta el momento, así como agregar nuevos reportes de sus horas laboradas.</span>
                        <!-- Buscador -->
                        <div class="buscador">
                            <div style="width: 100%; float: left; font-weight: bold; font-size: 16px; border-bottom: 1px solid #fff; margin-bottom: 5px;">
                                Buscar:
                            </div>
                            <div style="width: 61%; float: left; margin-right: 4%;">
                                <asp:TextBox ID="txtBuscarBecario" onkeydown="enterBuscar(event, 'MainContent_btnBuscar');"
                                    CssClass="txtEncargado" runat="server"></asp:TextBox>
                            </div>
                            <div style="width: 35%; float: right">
                                <asp:Button ID="btnBuscar" runat="server" Text="Buscar" CausesValidation="false"
                                    CssClass="boton ui-widget ui-state-default ui-corner-all ui-button-text-only"
                                    OnClick="btnBuscar_Click" />
                            </div>
                        </div>
                        <!-- Botón Reportar Horas -->
                        <div class="insertar">
                            <div style="width: 100%; float: left; font-weight: bold; font-size: 16px; border-bottom: 1px solid #fff; margin-bottom: 5px;">
                                Nuevo reporte de horas
                            </div>
                            <asp:Button ID="btnInsertarBecarios" runat="server" Text="Reportar" CssClass="boton ui-widget ui-state-default ui-corner-all ui-button-text-only"
                                OnClick="btnReportarHoras_Click" CausesValidation="false" />
                        </div>
                        <!-- Horas restantes -->
                        <div class="horasRestantes">
                            <div style="width: 100%; float: left; font-weight: bold; font-size: 16px; border-bottom: 1px solid #fff; margin-bottom: 5px;">
                                Horas restantes:
                            </div>
                            <div style="width: 92%; float: left; padding: 0 4%; text-align: center;">
                                <b>
                                    <asp:Label ID="lblHorasRestantes" runat="server" Text=""></asp:Label></b>
                            </div>
                        </div>
                        <!-- Grid -->
                        <div id="divGridControlHorasBecario">
                            <asp:GridView ID="gridControlHorasBecario" runat="server" GridLines="Both" RowStyle-HorizontalAlign="Center"
                                RowStyle-VerticalAlign="Middle" AllowPaging="true" PageSize="15"
                                OnRowCommand="gridControlHorasBecario_RowCommand" CssClass="table_css centerText"
                                PagerStyle-CssClass="pagerGlobal" >
                                <Columns>
                                    <asp:ButtonField CommandName="btnSeleccionarTupla_Click" CausesValidation="false"
                                        ButtonType="Image" ImageUrl="~/Images/arrow-right.png" ItemStyle-HorizontalAlign="Center"
                                        ItemStyle-VerticalAlign="Middle" />
                                </Columns>
                            </asp:GridView>
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
            <!-- Pop-ups -->
            <div id="PopUpCtrlBecario">
                <asp:UpdatePanel runat="server" ID="UpdatePopUp">
                    <Triggers>
                    </Triggers>
                    <ContentTemplate>
                        <!-- Campos PopUp -->
                        <div id="popUpContent" style="width: 84%; padding: 2% 8%; float: left; background: #D8D8BF; border-radius: 5px;">
                        <p>Tome en cuenta que solo puede realizar un reporte de horas trabajadas para un día específico</p>
                            <!-- Cantidad de Horas -->
                            <div style="width: 92%; float: left; margin-bottom: 10px; padding-left: 8%;">
                                <div style="width: 40%; float: left; margin-bottom: 0px;">
                                    Cantidad de Horas:
                                </div>
                                <div style="width: 60%; float: left">
                                    <asp:TextBox ID="txtCantidadHoras" runat="server" MaxLength="2"></asp:TextBox>
                                </div>
                            </div>
                            <!--Para la fecha en que se hicieron las horas-->
                            <div style="width: 92%; float: left; margin-bottom: 10px; padding-left: 8%;">
                                <div style="width: 40%; float: left; margin-bottom: 0px;">
                                    Fecha:
                                </div>
                                <div style="width: 60%; float: left">
                                    <asp:TextBox ID="txtFecha" CssClass="dateText" runat="server"></asp:TextBox>
                                </div>
                            </div>
                            <div style="width: 92%; float: left; margin-bottom: 10px; padding-left: 8%;">
                                <div style="width: 60%; float: left; padding-left: 40%;">
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" ValidationGroup="NuevoReporte"
                                        ControlToValidate="txtCantidadHoras" runat="server"
                                        ErrorMessage="Cantidad de Horas Requeridas" ForeColor="Red" Display="Dynamic"></asp:RequiredFieldValidator>
                                   
                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator1"
                                        ControlToValidate="txtCantidadHoras" runat="server"
                                        ErrorMessage="Horas no válidas" Display="Dynamic"
                                        ForeColor="Red" ValidationExpression="^([1-16])*$" Enabled="true"></asp:RegularExpressionValidator>
                                    <asp:RangeValidator ID="RangeValidator1" Enabled="false" runat="server" 
                                        ErrorMessage="Horas no válidas range" Font-Bold="True" ForeColor="Red" 
                                        ControlToValidate="txtCantidadHoras" MaximumValue="16" MinimumValue="1" 
                                        Type="Integer"></asp:RangeValidator>
                                </div>
                            </div>
                            <div id="comentario" style="width: 84%; padding: 0% 8% 0% 8%; float: left; background: #D8D8BF; border-radius: 5px;">
                                <!-- Textbox para comentario -->
                                <div style="width: 100%; float: left; margin-bottom: 0px;">
                                    <span style="width: 100%; float: left;">Explique de manera breve el trabajo realizado:
                                    </span>
                                    <asp:TextBox ID="txtComentario" CssClass="comentarioCHB" TextMode="MultiLine" Rows="5"
                                        runat="server"></asp:TextBox>
                                </div>
                                <div id="validadorComentario" style="width: 100%; float: left">
                                    <asp:RequiredFieldValidator ValidationGroup="NuevoReporte" Display="Dynamic" ID="RequiredFieldValidator2" ForeColor="Red" runat="server" ControlToValidate="txtComentario" ErrorMessage="Comentario Requerido"></asp:RequiredFieldValidator>
                                </div>
                                <!-- Textbox para comentario respuesta -->
                                <div id="comentarioDeEncargado" style="width: 100%; float: left">
                                    <asp:Label ID="Label2" runat="server" Text="Comentario del encargado"></asp:Label>
                                    <asp:TextBox ID="txtComentarioEncargado" CssClass="comentarioCHB" TextMode="MultiLine"
                                        Rows="5" runat="server"></asp:TextBox>
                                </div>
                                
                            </div>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
            <!--Para preguntar si se desea seguir con la misma asignacion-->

        </asp:View>
        <!-- Consulta de Becarios relacionados con el Encargado -->
        <asp:View ID="VistaEncargado" runat="server">
            <div id="PopUpAceptarAsignacionEncargado">
            </div>
        </asp:View>
        <!-- Consulta de Encargados relacionados con ese Becario -->
        <asp:View ID="VistaBecario" runat="server">
        </asp:View>
        <!-- Sin acceso al módulo -->
        <asp:View ID="VistaSinPermiso" runat="server">
            <h2 style="color: Red; text-align: center;">Lo sentimos. Usted no tiene acceso a esta sección.</h2>
        </asp:View>
    </asp:MultiView>
    <div id="PopUpAyudaParcialBecario">
        <asp:UpdatePanel runat="server" ID="UpdatePanel2">
            <ContentTemplate>
                <iframe style="width: 99%; height: 500px;" src="HTMLS%20Ayuda/Perfil%20Becario/Control%20de%20Horas/Becario%20-%20Control%20de%20Horas.htm"></iframe>
            </ContentTemplate>
        </asp:UpdatePanel>                
    </div>
</asp:Content>
