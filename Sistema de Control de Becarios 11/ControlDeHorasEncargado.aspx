<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="ControlDeHorasEncargado.aspx.cs" Inherits="ControlDeHorasEncargado" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
    <script src="Scripts/ControlDeHorasEncargado.js" type="text/javascript"></script>
    <link href="Styles/ControlDeHorasEncargado.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <asp:ScriptManager ID="ScriptManager" runat="server">
    </asp:ScriptManager>
    <asp:MultiView ID="MultiViewControlHorasEncargado" runat="server">

        <!-- Vista para Encargado de control de horas -->
        <asp:View ID="VistaControlHorasEncargado" runat="server">
            <asp:UpdatePanel ID="UpdateInfo" runat="server">
                <Triggers>
            
                </Triggers>

                <ContentTemplate>
                    <!-- Botones Invisibles -->
                    <asp:Button ID="btnInvisibleEnviarRevision" 
                        CssClass="btnInvisibleEnviarRevision invisible" runat="server" Text="" 
                        onclick="btnInvisibleEnviarRevision_Click" />

                    <!-- Cuerpo -->
                    <div style="width: 100%; float: left;">

                        <!-- Título -->
                        <span style="width: 100%; font-weight: bold; font-size: 24px; float: left; margin: 20px 0 5px 0; text-align:center;">Módulo de Control de Horas para Encargado</span>
                        <span style="width: 100%; font-weight: normal; font-style:italic; font-size: 16px; float: left; margin: 5px 0 20px 0; text-align:center; border-bottom: 2px solid #414141; padding-bottom: 5px;">Revise la actividad de los becarios que tenga a su cargo. Acepte o rechace las horas insertadas por los becarios.</span>
                        <span style="width: 100%; font-weight: bold; font-size: 16px; float: left; margin: 0px 0 5px 0; text-align:left;">Becarios con horas pendientes de revisar. Por favor seleccione uno para empezar a revisar sus horas.</span>

                        <!-- Grid con Becarios con horas pendientes de revisar -->
                        <asp:GridView ID="GridBecariosConHorasPendientes" 
                            CssClass="table_css centerText" runat="server" 
                            onrowcommand="GridBecariosConHorasPendientes_RowCommand">
                            <columns>
                                <asp:ButtonField CommandName="btnSeleccionarTupla_Click" CausesValidation="false" ButtonType="Button" Visible="true" ImageUrl="~/Images/arrow-right.png" ItemStyle-HorizontalAlign="Center" Text="Revisar" ItemStyle-VerticalAlign="Middle"/> 
                            </columns>
                        </asp:GridView>
                    </div>

                </ContentTemplate>

            </asp:UpdatePanel>


            <!-- Pop Up -->
            <div id="PopUpControlDeHorasEncargado">
                <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                    <Triggers>
                        <asp:AsyncPostBackTrigger EventName="CheckedChanged" ControlID="RadioButtonAceptarHoras" />
                    </Triggers>

                    <ContentTemplate>

                        <!-- Título -->
                        <span style="width: 100%; font-weight: bold; font-size: 16px; float: left; margin: 0px 0 10px 0; text-align:left;">Horas pendientes de revisión. Seleccione una línea para revisarla.</span>

                        <!-- Grid con Horas de becario -->
                        <asp:GridView ID="GridViewHoraYFechaBecario" 
                            CssClass="table_css centerText" runat="server" OnRowCommand="GridViewHoraYFechaBecario_RowCommand">
                            <columns>
                                <asp:ButtonField CommandName="btnSeleccionarTupla_Click" CausesValidation="false" ButtonType="Image" Visible="true" ImageUrl="~/Images/arrow-right.png" ItemStyle-HorizontalAlign="Center" Text="Revisar" ItemStyle-VerticalAlign="Middle"/> 
                            </columns>
                        </asp:GridView>

                        <!-- Cuerpo -->
                        <div style="width: 92%; float: left; margin-top: 10px; background: #D8D8BF; padding: 4%; border-radius:5px;">
                            <!-- Comentarios -->
                            <div style="width: 60%; float: left; margin-right: 4%;">
                                <span style="width: 100%; float: left;">Comentario del becario:</span>
                                <asp:TextBox ID="txtComentarioBecario" TextMode="MultiLine" CssClass="multiLine" runat="server"></asp:TextBox>
                                <span style="width: 100%; float: left; margin-top: 5px;">Responder al becario:</span>
                                <asp:TextBox ID="txtComentarioEncargado" TextMode="MultiLine" CssClass="multiLine" runat="server"></asp:TextBox>
                            </div>
                            <!-- Aceptar/Rechazar y Enviar -->
                            <div style="width: 34%; float: right;">
                                <span style="width: 100%; float: left; text-align: center; margin-bottom: 10px;">¿Acepta las horas?</span>
                                <asp:RadioButton ID="RadioButtonAceptarHoras" Text="Sí" GroupName="AceptarRechazar" runat="server" CssClass="radioLeft floatLeft" OnCheckedChanged="RadioButtonAceptarHoras_CheckedChanged" />
                                <asp:RadioButton ID="RadioButtonRechazarHoras" Text="No" GroupName="AceptarRechazar" runat="server" CssClass="radioRight floatRight" /> 
                                <asp:Button ID="btnEnviar" Height="50px" runat="server" Text="Enviar" CssClass="boton ui-widget ui-state-default ui-corner-all ui-button-text-only" OnClick="btnEnviar_Click" />
                            </div>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </asp:View>

        <!-- Sin acceso al módulo -->
        <asp:View ID="VistaSinPermiso" runat="server">
            <h2 style="color: Red; text-align:center;">Lo sentimos. Usted no tiene acceso a esta sección.</h2>
        </asp:View>

    </asp:MultiView>
    
</asp:Content>

