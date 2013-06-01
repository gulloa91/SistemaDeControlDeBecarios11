﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Asignaciones.aspx.cs" Inherits="Asignaciones" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
    <script src="Scripts/Asignaciones.js" type="text/javascript"></script>
    <link href="Styles/Asignaciones.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <asp:ScriptManager ID="ScriptManager" runat="server">	
    </asp:ScriptManager>

    <asp:MultiView ID="MultiViewEncargado" runat="server">

        <!-- IMEC de todas las asignaciones -->
        <asp:View ID="VistaAdmin" runat="server">
            <asp:UpdatePanel ID="UpdateInfo" runat="server">
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="btnInvisibleAceptarAsignacion" EventName="Click" />
                </Triggers>

                <!-- Content template -->
                <ContentTemplate>
                    <!-- Botones Invisibles -->
                    <asp:Button ID="btnInvisibleAceptarAsignacion" 
                        CssClass="btnInvisible1 invisible" runat="server" Text="" 
                        onclick="btnInvisibleAceptarAsignacion_Click" />
                    <asp:Button ID="btnInvisibleEliminarAsignacion" 
                        CssClass="btnInvisible2 invisible" runat="server" Text="" 
                        onclick="btnInvisibleEliminarAsignacion_Click" CausesValidation="false" />

                    <!-- Cuerpo -->
                    <div style="min-height: 500px;">

                        <!-- Título -->
                        <span style="width: 100%; font-weight: bold; font-size: 24px; float: left; margin: 20px 0 5px 0; text-align:center;">Módulo para administración de Asignaciones</span>
                        <span style="width: 100%; font-weight: normal; font-style:italic; font-size: 16px; float: left; margin: 5px 0 20px 0; text-align:center; border-bottom: 2px solid #414141; padding-bottom: 5px;">Por medio de este módulo se pueden insertar, modificar, eliminar y consultar todas las asignaciones entre un encargado y un becario.</span>

                        <!-- Buscador -->
                        <div class="buscadorAsignacion">
                            <div style="width: 16%; float:left; font-weight: bold; font-size: 16px; border-bottom: 1px solid #fff; margin-bottom: 5px;">Buscar:</div>
                            <div style="width: 84%; float:left; font-weight: bold; font-size: 16px; border-bottom: 1px solid #fff;  margin-bottom: 5px;">Filtros:</div>
                            <!-- TXT Buscar -->
                            <div style="width: 15.66%; float: left; margin-right: 1%;">
                                <span style="float:left; width:100%;">&nbsp;</span>
                                <asp:TextBox ID="txtBuscarAsignacion"  onkeydown = "enterBuscar(event, 'MainContent_btnBuscar');" CssClass="txtAsignacion" runat="server"></asp:TextBox>
                            </div>

                            <!-- DRP Año -->
                            <div style="width: 15.66%; float: left; margin-right: 1%;">
                                <span style="float:left; width:100%;">Año:</span>
                                <asp:DropDownList ID="DropDownAnio" CssClass="txtAsignacion" runat="server"> 
                                </asp:DropDownList>
                            </div>

                            <!-- DRP Ciclo -->
                            <div style="width: 15.66%; float: left; margin-right: 1%;">
                                <span style="float:left; width:100%;">Ciclo:</span>
                                <asp:DropDownList ID="DropDownCiclo" CssClass="txtAsignacion" runat="server">
                                </asp:DropDownList>
                            </div>

                            <!-- DRP Estado -->
                            <div style="width: 15.66%; float: left; margin-right: 1%;">
                                <span style="float:left; width:100%;">Estado:</span>
                                <asp:DropDownList ID="DropDownEstado" CssClass="txtAsignacion" runat="server">
                                </asp:DropDownList>
                            </div>

                            <!-- DRP Encargado -->
                            <div style="width: 15.66%; float: left; margin-right: 1%;">
                                <span style="float:left; width:100%;">Encargado:</span>
                                <asp:DropDownList ID="DropDownList1" CssClass="txtAsignacion" runat="server">
                                </asp:DropDownList>
                            </div>

                            <!-- BTN Buscar -->
                            <div style="width: 15%; float: left; margin-right: 1%;">
                                <span style="float:left; width:100%;">&nbsp;</span>
                                <asp:Button ID="btnBuscar" runat="server" Text="Buscar" CausesValidation="false" CssClass="boton ui-widget ui-state-default ui-corner-all ui-button-text-only" OnClick="btnBuscar_Click" />
                            </div>
                        </div>

                        <!-- Insertar -->
                        <div class="insertar" style="margin-bottom: 20px;">
                            <div style="width: 100%; float:left; font-weight: bold; font-size: 16px; border-bottom: 1px solid #fff; margin-bottom: 5px;">Nuevo encargado</div>
                            <span style="float:left; width:100%;">&nbsp;</span>
                            <asp:Button ID="btnInsertarAsignacion" runat="server" Text="Nuevo" 
                                CssClass="boton ui-widget ui-state-default ui-corner-all ui-button-text-only" 
                                CausesValidation="false" onclick="btnInsertarAsignacion_Click"/>
                        </div>

                        <!-- Grid -->
                        <asp:GridView ID="GridAsignaciones"  runat="server" CssClass="table_css" 
                            GridLines="Both" AllowPaging="True" RowStyle-HorizontalAlign="Center" 
                            RowStyle-VerticalAlign="Middle" PageSize="15" 
                            PagerStyle-CssClass="pagerGlobal" 
                            onrowcommand="GridAsignaciones_RowCommand">
                               <columns>
                                   <asp:ButtonField CommandName="btnSeleccionarTupla_Click" CausesValidation="false" ButtonType="Image" Visible="true" ImageUrl="~/Images/arrow-right.png" ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle"/> 
                               </columns>
                        </asp:GridView>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>

            <!-- Pop Ups -->
            <div id="PopUpAsignacion">
                
                <asp:UpdatePanel runat="server" ID="UpdatePopUp">
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="btnInvisibleEliminarAsignacion" EventName="Click" />
                    </Triggers>

                    <ContentTemplate>
                        <!-- Botones Modificar y Eliminar -->
                        <div style="width: 96%; padding: 2%; float: left;" >
                            <div style="width: 20%; float: right;">
                                <asp:Button ID="btnEliminarAsignacion" CausesValidation="false" runat="server" Text="Eliminar" 
                                        
                                    CssClass="boton ui-widget ui-state-default ui-corner-all ui-button-text-only" 
                                    onclick="btnEliminarAsignacion_Click" />
                            </div>
                            <div style="width: 20%; float: right; margin-right: 5px;">
                                <asp:Button ID="btnModificarAsignacion" CausesValidation="false" runat="server" Text="Modificar" 
                                        
                                    CssClass="boton ui-widget ui-state-default ui-corner-all ui-button-text-only" 
                                    onclick="btnModificarAsignacion_Click" />
                            </div>
                        </div>

                        <!-- Contenido Pop Up -->
                        <div id="popUpContent" style="width: 96%; padding: 0 2%; float: left; background: #D8D8BF; border-radius: 5px;">
                            
                            <!-- Año y ciclo -->
                            <div style="width: 50%; float: left; text-align: center; margin-top: 10px;">
                                <span style="width:100%; float: left; font-weight: bold; text-align: left;">Periodo: </span>
                                <div style="width:90%; float: left; margin: 4px 0 0 0; border-radius: 5px; font-size: 1em;" >
                                    <asp:Label ID="lblCiclo" runat="server" Text=""></asp:Label> -
                                    <asp:Label ID="lblAnio" runat="server" Text=""></asp:Label>
                                </div>
                            </div>
                            
                            <!-- Total de horas -->
                            <div style="width: 50%; float: left; margin-top: 10px; text-align: center;">
                                <span style="width:100%; float: left; font-weight: bold; text-align: left;">Total de Horas:<span style="color:red">*</span></span>
                                <asp:TextBox ID="txtTotalHoras" CssClass="txtAsignacion centerText" runat="server"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidatorTotalDeHoras" ControlToValidate="txtTotalHoras" Display="Dynamic" runat="server" ErrorMessage="Cantidad de horas requerida"  ForeColor="#FF3300"></asp:RequiredFieldValidator>
                            </div>

                            <!-- Becario y Encargado -->
                            <div style="width: 100%; float: left; margin-top: 10px;">
                                <div style="width: 50%; float: left;">
                                    <span style="width:37%; float: left; font-weight: bold;">Encargado:<span style="color:red">*</span></span>
                                    <div style="width:63%; float: left;">
                                        <asp:Button ID="btnCantidadBecariosDeEncargado" runat="server" 
                                            CssClass="BtnCntBecarios ui-state-default ui-button" 
                                            Text="" onclick="btnCantidadBecariosDeEncargado_Click" CausesValidation="false" />
                                    </div>
                                    <asp:DropDownList ID="DropDownEncargadosPopUp" CssClass="txtAsignacion" runat="server">
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidatorEncargadosPopUp" ControlToValidate="DropDownEncargadosPopUp" Display="Dynamic" runat="server" ErrorMessage="Por favor seleccione un encargado" CssClass="txtAsignacion"  ForeColor="#FF3300"></asp:RequiredFieldValidator>
                                </div>
                                <div style="width: 50%; float: left;">
                                    <span style="width:100%; float: right; font-weight: bold;">Becario:<span style="color:red">*</span></span>
                                    <asp:DropDownList ID="DropDownBecariosPopUp" CssClass="txtAsignacion" runat="server">
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidatorBecarioPopUp" ControlToValidate="DropDownEncargadosPopUp" Display="Dynamic" runat="server" ErrorMessage="Por favor seleccione un becario" CssClass="txtAsignacion" ForeColor="#FF3300"></asp:RequiredFieldValidator>
                                </div>
                            </div>

                            <!-- Unidad Academica, Información de Ubicación -->
                            <div style="width: 100%; float: left; margin-top: 20px; margin-bottom: 10px;">
                                <div style="width: 50%; float: left;">
                                    <span style="width:100%; float: left; font-weight: bold;">Unidad Académica:</span>
                                    <asp:TextBox ID="txtUnidAcademica" CssClass="txtAsignacion" TextMode="MultiLine" runat="server"></asp:TextBox>
                                </div>
                                <div style="width: 50%; float: left;">
                                    <span style="width:100%; float: left; font-weight: bold;">Info. De Ubicación:</span>
                                    <asp:TextBox ID="txtInfoDeUbicacion" CssClass="txtAsignacion" TextMode="MultiLine" runat="server"></asp:TextBox>
                                </div>
                                <span style="width:100%; float: left; color: Red; margin-top: 10px; font-size: 12px;">* Campos obligatorios</span>
                            </div>

                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>

                
            </div>

            <div id="PopUpEliminarAsignacion">
                ¿Seguro que desea eliminar esta asignación?
            </div>

            <div id="PopUpVerBecariosAsignados">
                <asp:UpdatePanel ID="UpdatePanelListaDeAsignados" runat="server">
                    <Triggers></Triggers>
                    <ContentTemplate>
                        <asp:GridView ID="GridBecariosAsignadosAEncargado" CssClass="table_css centerText" runat="server">
                        </asp:GridView>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </asp:View>

        <!-- Consulta de Becarios relacionados con el Encargado -->
        <asp:View ID="VistaEncargado" runat="server">
            <asp:UpdatePanel ID="UpdatePanelEncargado" runat="server">
                <Triggers></Triggers>

                <ContentTemplate>
                    <!-- Botones Invisibles -->
                    <asp:Button ID="btnInvisibleAceptarAsignacionEncargado" runat="server" 
                        CssClass="invisible btnInvisibleAceptarAsignacionEncargado" Text="Button" 
                        onclick="btnInvisibleAceptarAsignacionEncargado_Click" />
                    <asp:Button ID="btnInvisibleRechazarAsignacionEncargado" runat="server" 
                        CssClass="invisible btnInvisibleRechazarAsignacionEncargado" Text="Button" 
                        onclick="btnInvisibleRechazarAsignacionEncargado_Click" />

                    <!-- Título -->
                    <span style="width: 100%; font-weight: bold; font-size: 24px; float: left; margin: 20px 0 5px 0; text-align:center;">Becarios Asignados</span>
                    <span style="width: 100%; font-weight: normal; font-style:italic; font-size: 16px; float: left; margin: 5px 0 20px 0; text-align:center; border-bottom: 2px solid #414141; padding-bottom: 5px;">Consulte los becarios que tiene bajo tutela. Acepte o rechace las asignaciones que tenga pendientes. </span>
                    
                    <!-- Buscador -->
                    <div class="buscadorAsignacion" style="width: 20%; margin-bottom: 20px;">
                        <div style="width: 100%; float:left; font-weight: bold; font-size: 16px; border-bottom: 1px solid #fff; margin-bottom: 5px;">Buscar:</div>
                        <!-- TXT Buscar -->
                        <div style="width: 59%; float: left; margin-right: 1%;">
                            <asp:TextBox ID="txtBuscarVistaEncargado"  onkeydown = "enterBuscar(event, 'MainContent_txtBuscarVistaEncargado');" CssClass="txtAsignacion" runat="server"></asp:TextBox>
                        </div>

                        <!-- BTN Buscar -->
                        <div style="width: 39%; float: left; margin-right: 1%;">
                            <asp:Button ID="btnBuscarVistaEncargado" runat="server" Text="Buscar" CausesValidation="false" CssClass="boton ui-widget ui-state-default ui-corner-all ui-button-text-only" />
                        </div>
                    </div>

                    <!-- Periodo -->
                    <div class="buscadorAsignacion" style="width: 15%; margin-bottom: 20px;">
                        <div style="width: 100%; float:left; font-weight: bold; font-size: 16px; border-bottom: 1px solid #fff; margin-bottom: 5px;">Periodo:</div>
                        <div style="text-align:center;"><b>
                            <asp:Label ID="lblCicloPrincipalVistaEncargado" runat="server" Text=""></asp:Label> - <asp:Label ID="lblAnioPrincipalVistaEncargado" runat="server" Text="Label"></asp:Label></b>
                        </div>
                    </div>

                    <!-- Grid con Becarios asignados y por asignar -->
                    <asp:GridView ID="GridBecariosAsignadosVistaEncargado" CssClass="table_css centerText" runat="server" 
                        onrowcommand="GridBecariosAsignadosVistaEncargado_RowCommand">
                        <columns>
                            <asp:ButtonField CommandName="btnSeleccionarTupla_Click" CausesValidation="false" ButtonType="Button" Visible="true" ImageUrl="~/Images/arrow-right.png" ItemStyle-HorizontalAlign="Center" Text="Aceptar/Rechazar" ItemStyle-VerticalAlign="Middle"/> 
                        </columns>
                    </asp:GridView>
                </ContentTemplate>
            </asp:UpdatePanel>

            <div id="PopUpAsignacionEncargado">
                <asp:UpdatePanel ID="UpdatePanelPopUpAsignacionEncargado" runat="server">
                    <Triggers ></Triggers>
                    <ContentTemplate>
                        <p>
                        <span style="float: left; width: 100%;">
                            Usted ha sido elegido para ser encargado del becario: 
                            <b><asp:Label ID="lblNombreBecarioPopUpVistaEncargado" runat="server" Text=""></asp:Label></b>
                        </span>
                        <span style="float: left; width: 100%;">
                            Se le han asignado un total de:
                            <b><asp:Label ID="lblHorasBecarioPopUpVistaEncargado" runat="server" Text=""></asp:Label></b>
                            horas al becario.
                        </span>
                        <span style="float: left; width: 100%;">
                            Esta asignación está provista para el: <b><asp:Label ID="lblCicloBecarioPopUpVistaEncargado" runat="server" Text=""></asp:Label> - 
                            <asp:Label ID="lblAnioBecarioPopUpVistaEncargado" runat="server" Text=""></asp:Label></b>
                        </span>
                        ¿Acepta la asignación?</p>
                    </ContentTemplate>
                </asp:UpdatePanel>
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

