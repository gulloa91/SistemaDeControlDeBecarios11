<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="ControlDeHorasBecario.aspx.cs" Inherits="ControlDeHoras" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <asp:ScriptManager ID="ScriptManager" runat="server">	
    </asp:ScriptManager>

    <asp:MultiView ID="MultiViewEncargado" runat="server">

        <!-- IMEC de todas las asignaciones -->
        <asp:View ID="VistaAdmin" runat="server">
            <asp:UpdatePanel ID="UpdateInfo" runat="server">
                <Triggers>
                
                </Triggers>

                <ContentTemplate>
                    <!-- Botones Invisibles -->
                    <asp:Button ID="btnInvisibleAceptarAsignacion" 
                        CssClass="btnInvisible1 invisible" runat="server" Text="" 
                        onclick="btnInvisibleAceptarAsignacion_Click" />
                    <asp:Button ID="btnInvisibleEliminarAsignacion" 
                        CssClass="btnInvisible2 invisible" runat="server" Text="" 
                        onclick="btnInvisibleEliminarAsignacion_Click" />

                    <!-- Cuerpo -->
                    <div style="min-height: 500px;">

                        <!-- Título -->
                        <span style="width: 100%; font-weight: bold; font-size: 24px; float: left; margin: 20px 0 5px 0; text-align:center;">Módulo para administración de Asignaciones</span>
                        <span style="width: 100%; font-weight: normal; font-style:italic; font-size: 16px; float: left; margin: 5px 0 20px 0; text-align:center; border-bottom: 2px solid #414141; padding-bottom: 5px;">Por medio de este módulo se pueden insertar, modificar, eliminar y consultar todas las asignaciones entre un encargado y un becario.</span>

                        <!-- Buscador -->
                        <div class="buscador">
                            <div style="width: 100%; float:left; font-weight: bold; font-size: 16px; border-bottom: 1px solid #fff; margin-bottom: 5px;">Buscar:</div>
                            <div style="width: 61%; float: left; margin-right: 4%;">
                                <asp:TextBox ID="txtBuscarAsignacion"  onkeydown = "enterBuscar(event, 'MainContent_btnBuscar');" CssClass="txtAsignacion" runat="server"></asp:TextBox>
                            </div>
                            <div style="width: 35%; float: right">
                                <asp:Button ID="btnBuscar" runat="server" Text="Buscar" CausesValidation="false" CssClass="boton ui-widget ui-state-default ui-corner-all ui-button-text-only" OnClick="btnBuscar_Click" />
                            </div>
                        </div>

                        <!-- Grid -->
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>

            <div id="PopUpNuevaAsignacion">
            </div>

            <div id="PopUpConsultarAsignacion">
            </div>

            <div id="PopUpEliminarAsignacion">
                ¿Seguro que desea eliminar esta asignación?
            </div>
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
            <h2 style="color: Red; text-align:center;">Lo sentimos. Usted no tiene acceso a esta sección.</h2>
        </asp:View>

    </asp:MultiView>
</asp:Content>

