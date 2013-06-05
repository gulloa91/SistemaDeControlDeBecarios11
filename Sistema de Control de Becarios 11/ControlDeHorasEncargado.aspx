<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="ControlDeHorasEncargado.aspx.cs" Inherits="ControlDeHorasEncargado" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
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
                        CssClass="btnInvisibleEnviarRevision invisible" runat="server" Text="" />

                    <!-- Cuerpo -->
                    <div style="width: 100%; float: left;">

                        <!-- Título -->
                        <span style="width: 100%; font-weight: bold; font-size: 24px; float: left; margin: 20px 0 5px 0; text-align:center;">Módulo de Control de Horas para Encargado</span>
                        <span style="width: 100%; font-weight: normal; font-style:italic; font-size: 16px; float: left; margin: 5px 0 20px 0; text-align:center; border-bottom: 2px solid #414141; padding-bottom: 5px;">Revise la actividad de los becarios que tenga a su cargo. Acepte o rechace las horas insertadas por los becarios.</span>
                        <span style="width: 100%; font-weight: bold; font-size: 16px; float: left; margin: 0px 0 5px 0; text-align:left;">Becarios con horas pendientes de revisar. Por favor seleccione uno para empezar a revisar sus horas.</span>

                        <!-- Grid con Becarios con horas pendientes de revisar -->
                        <asp:GridView ID="GridBecariosConHorasPendientes" CssClass="table_css centerText" runat="server">
                            <columns>
                                <asp:ButtonField CommandName="btnSeleccionarTupla_Click" CausesValidation="false" ButtonType="Button" Visible="true" ImageUrl="~/Images/arrow-right.png" ItemStyle-HorizontalAlign="Center" Text="Aceptar/Rechazar" ItemStyle-VerticalAlign="Middle"/> 
                            </columns>
                        </asp:GridView>
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

