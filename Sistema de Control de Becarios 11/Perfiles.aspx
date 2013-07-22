<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Perfiles.aspx.cs"
    Inherits="Perfiles" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
    <script src="Scripts/Perfiles.js" type="text/javascript"></script>
    <link href="Styles/Perfiles.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        .txtEncargado {
            width: 128px;
        }
    </style>
    <script type="text/javascript">
        $(document).ready(function () {
            $("a.level1:contains('Perfiles')").addClass("item_active");
        });
    </script>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="Server">
    <asp:ScriptManager ID="ScriptManager" runat="server">
    </asp:ScriptManager>
    <asp:MultiView ID="multiViewPerfiles" runat="server">
       <asp:View ID="vistaAdmin" runat="server">
    <asp:UpdatePanel ID="UpdateInfo" runat="server">
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="btnInvisible1" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="btnEliminarEncargado" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="btnModificarEncargado" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="gridPerfiles" EventName="SelectedIndexChanging" />
        </Triggers>
        <ContentTemplate>
            <!-- Botones Invisibles -->
            <asp:Button ID="btnInvisible1" CssClass="btnInvisible1 invisible" runat="server"
                Text="" OnClick="clicBotonAceptar" />
            <asp:Button ID="btnInvisible2" CssClass="btnInvisible2 invisible" runat="server"
                Text="" OnClick="siElimina" />
            <!-- Cuerpo con la tabla -->
            <div style="min-height: 500px;">
                <span style="width: 100%; font-weight: bold; font-size: 24px; float: left; margin: 20px 0 5px 0; text-align:center;">Módulo para administración de Perfiles</span>
                        <span style="width: 100%; font-weight: normal; font-style:italic; font-size: 16px; float: left; margin: 5px 0 20px 0; text-align:center; border-bottom: 2px solid #414141; padding-bottom: 5px;">Por medio de este módulo se pueden insertar, modificar, eliminar y consultar todos los perfiles que se encuentran en la base de datos. Cada perfil tiene un conjunto de permisos, los cuales dictan qué módulos van a ser poder accesados.</span>
                <div class="buscador">
                    <div style="width: 100%; float: left; font-weight: bold; font-size: 16px; border-bottom: 1px solid #fff; margin-bottom: 5px;">
                        Buscar:
                    </div>
                    <div style="width: 61%; float: left; margin-right: 4%;">
                        <asp:TextBox ID="txtBuscarPerfil" CssClass="txtEncargado" runat="server"></asp:TextBox>
                    </div>
                    <div style="width: 35%; float: right">
                        <asp:Button ID="btnBuscar" runat="server" Text="Buscar" CssClass="boton ui-widget ui-state-default ui-corner-all ui-button-text-only" OnClick="btnBuscar_Click" />
                    </div>
                </div>
                <div class="insertar">
                    <div style="width: 100%; float: left; font-weight: bold; font-size: 16px; border-bottom: 1px solid #fff; margin-bottom: 5px;">
                        Nuevo Perfil
                    </div>
                    <asp:Button ID="btnInsertarEncargado" runat="server" Text="Nuevo" CssClass="boton ui-widget ui-state-default ui-corner-all ui-button-text-only"
                        OnClick="clickBotonInsertar" CausesValidation="False" />
                </div>
                <!-- Ayuda -->
                <div class="insertar" style="float:right; width:10%">     
                    <div style="width: 100%; float:left; font-weight: bold; font-size: 16px; border-bottom: 1px solid #fff; margin-bottom: 5px;">Ayuda</div>                       
                    <input type="button" value="Ayuda" class="boton ui-widget ui-state-default ui-corner-all ui-button-text-only" id="Button1" onclick="$('#PopUpAyuda').dialog('open');" />
                </div>
                <asp:GridView ID="gridPerfiles" CssClass="globalTable" runat="server" 
                    OnPageIndexChanging="gridPerfiles_PageIndexChanging" 
                    onrowcommand="gridPerfiles_RowCommand" AllowPaging="True" PageSize="15" PagerStyle-CssClass="pagerGlobal">
                    <Columns>
                        <asp:ButtonField CommandName="seleccionarPosiblePerfil" CausesValidation="false" ButtonType="Image" Visible="true" ImageUrl="Images/arrow-right.png"/>
                    </Columns>
                </asp:GridView>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    <div id="PopUp" title="Perfiles">
        <asp:UpdatePanel runat="server" ID="UpdatePopUp">
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="btnInvisible2" EventName="Click" />
                <asp:AsyncPostBackTrigger ControlID="btnModificarEncargado" />
                <asp:AsyncPostBackTrigger ControlID="radioAdministrador" />
                <asp:AsyncPostBackTrigger ControlID="radioBecario" />
                <asp:AsyncPostBackTrigger ControlID="radioEncargado" />
            </Triggers>
            <ContentTemplate>
                <div id="perfil_content">
                    <!-- BOTONES -->
                    <div id="perfil_botones">
                        <asp:Button ID="btnModificarEncargado" runat="server" CssClass="btnModPerfil boton ui-widget ui-state-default ui-corner-all ui-button-text-only"
                            OnClick="clickBotonModificar" Text="Modificar" Width="135px" />
                        <asp:Button ID="btnEliminarEncargado" runat="server" CssClass="boton ui-widget ui-state-default ui-corner-all ui-button-text-only"
                            OnClick="clickBotonEliminar" Text="Eliminar" Width="139px" />
                    </div>
                    <!-- NOMBRE -->
                    <div id="perfil_nombre">
                        <div id="LabelNombre">
                            <span>Nombre</span>
                        </div>
                        <div id="txtNombre">
                            <asp:TextBox ID="txtNombrePerfil"  runat="server" Width="323px" Enabled="False" MaxLength="50"></asp:TextBox>
                        </div>
                    </div>
                    <div id="validadores">
                        <div id ="validarores1">
                        </div>
                        <div id="validarores2">
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="Nombre Requerido" ControlToValidate="txtNombrePerfil" Display="Dynamic" EnableClientScript="False" Font-Bold="True" ForeColor="Red"></asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ErrorMessage="Se han escrito caracteres inválidos" ControlToValidate="txtNombrePerfil" Display="Dynamic" EnableClientScript="False" Font-Bold="True" ForeColor="Red" ValidationExpression="[a-zA-ZÀ-ÖØ-öø-ÿ]+\.?(( |\-)[a-zA-ZÀ-ÖØ-öø-ÿ]+\.?)*"></asp:RegularExpressionValidator>
                        </div>
                    </div>
                    <!--RADIO BUTTONS PARA EL TIPO DE PERFIL-->
                    <div id="radioTipo">
                        <span>Tipo de Perfil:</span>
                        <asp:RadioButton ID="radioAdministrador" runat="server" Text="Administrador" GroupName="tipo" Enabled="False" OnCheckedChanged="radioAdministrador_CheckedChanged" AutoPostBack="True"/>
                        <asp:RadioButton ID="radioEncargado" runat="server" Text="Encargado" GroupName="tipo" Enabled="False" OnCheckedChanged="radioEncargado_CheckedChanged" AutoPostBack="True" />
                        <asp:RadioButton ID="radioBecario" runat="server" Text="Becario" GroupName="tipo" Enabled="False" OnCheckedChanged="radioBecario_CheckedChanged" AutoPostBack="True" />
                    </div>
                    <!-- PERMISOS -->
                    <div id="perfil_permisos">
                        <div id="perfil_permisos_col0" class="columna-perfil">
                            <div id="permisos_becario" class="borde_perfil">
                                <asp:RadioButton ID="radioBecarioCompleto" CssClass="perfil_radio" runat="server"
                                    GroupName="Becarios" Text="Becario Completo" Enabled="False" />
                                <span class="perfil_mensaje">Muestra toda la vista del becario, es decir, la lista de
                                    todos los becarios en el sistema. </span>
                                <asp:RadioButton ID="radioBecarioParcial" CssClass="perfil_radio" runat="server"
                                    GroupName="Becarios" Text="Becario Parcial" Enabled="False" />
                                <span class="perfil_mensaje">Muestra sólo la información principal del usuario que está
                                    logueado en el sistema. </span>
                                <asp:RadioButton ID="radioSinAccesoBecario" CssClass="perfil_radio" runat="server"
                                    GroupName="Becarios" Text="Sin acceso a Becario" Enabled="False" />
                            </div>
                            <div id="permisos_encargado" class="borde_perfil">
                                <asp:RadioButton ID="radioEncargadoCompleto" CssClass="perfil_radio" runat="server"
                                    GroupName="Encargados" Text="Encargado Completo" Enabled="False" />
                                <span class="perfil_mensaje">Muestra toda la vista del Encargado, es decir, la lista
                                    de todos los Encargados en el sistema.</span>
                                <asp:RadioButton ID="radioEncargadoParcial" CssClass="perfil_radio" runat="server"
                                    GroupName="Encargados" Text="Encargado Parcial" Enabled="False" />
                                <span class="perfil_mensaje">Muestra solo la información principal del usuario que está
                                    logueado en el sistema.-</span>
                                <asp:RadioButton ID="radioSinAccesoEncargado" CssClass="perfil_radio" runat="server"
                                    GroupName="Encargados" Text="Sin acceso a Encargado" Enabled="False" />
                            </div>
                        </div>
                        <div id="perfil_permisos_col1" class="columna-perfil">
                            <div id="permisos_horas" class="borde_perfil">
                                <asp:RadioButton ID="radioControlBecario" CssClass="perfil_radio" runat="server" Text="Control Horas Estudiante" Enabled="False" GroupName="ControlHoras"/>
                                <span class="perfil_mensaje">Permite al becario las horas que cumplen para ser aprobadas</span>
                                <asp:RadioButton ID="radioControlEncargado" Text="Control Horas Encargado" Enabled="False" CssClass="perfil_radio" runat="server" GroupName="ControlHoras"/>
                                <span class="perfil_mensaje">Permite al encargado revisar las horas de los becarios
                                    asignados a él</span>
                                <asp:RadioButton ID="noControlHoras" Text="Sin Acceso Control de Horas" CssClass="perfil_radio" GroupName="ControlHoras" runat="server" />
                            
                            </div>
                            <div id="asignacion" class="borde_perfil">
                                <asp:RadioButton ID="radioAsignacionCompleta" CssClass="perfil_radio" runat="server"
                                    Text="Asignacion de horas Completa" Enabled="False" GroupName="radioAsignaciones"/>
                                <span class="perfil_mensaje">Muestra la vista completa de asignación, es decir, tanto
                                    de encargados como de Becarios.</span>
                                <asp:RadioButton ID="radioAsignacionEncargado" CssClass="perfil_radio" runat="server"
                                    Text="Asignación de horas Encargado" Enabled="False" GroupName="radioAsignaciones"/>
                                <span class="perfil_mensaje">El Encargado puede aceptar/cancelar las asignaciones de los becarios.</span>
                                <asp:RadioButton ID="radioAsignacionBecario" CssClass="perfil_radio" runat="server"
                                    Text="Asignación de horas Becario" Enabled="False" GroupName="radioAsignaciones"/>
                                <span class="perfil_mensaje">El Becario puede consultar y saber quién es su encargado en el actual semestre.</span>
                            </div>
                        </div>
                        <div id="perfil_permisos_col2" class="columna-perfil">
                            <div id="cuentas_perfiles" class="borde_perfil">
                                <asp:CheckBox ID="checkPerfiles" CssClass="perfil_radio" runat="server" Text="Vista Completa de Perfiles" />
                                <span class="perfil_mensaje">Muestra la vista completa de creación de Perfiles del sistema.</span>
                            </div>
                            <div id="Cuentas_id" class="borde_perfil">
                                <asp:RadioButton ID="radioCuentaCompleta" CssClass="perfil_radio" runat="server"
                                    GroupName="Cuentas" Text="Cuentas Completo" Enabled="False" />
                                <span class="perfil_mensaje">El usuario tiene acceso a todas las cuentas del sistema y puede reiniciar cualquier contraseña.</span>
                                <asp:RadioButton ID="radioCuentaParcial" CssClass="perfil_radio" runat="server"
                                    GroupName="Cuentas" Text="Cuentas Parcial" Enabled="False" />
                                <span class="perfil_mensaje">Muestra la información de la cuenta de sólo el usuario que está utilizando el sistema.</span>
                                <asp:RadioButton ID="radioSinCuenta" CssClass="perfil_radio" runat="server"
                                    GroupName="Cuentas" Text="Sin acceso a Cuenta" Enabled="False" />
                            </div>
                            <div id="reportes" class="borde_perfil">
                                <asp:CheckBox ID="checkReportes" Text="Reportes" runat="server" />
                                <span class="perfil_mensaje">Muestra el módulo de Reportes</span>
                            </div>
                        </div>
                    </div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    <div id="PopUpEliminar">
        ¿Seguro que desea eliminar?
    </div>
    </asp:View>
    <asp:View ID="VistaSinPermiso" runat="server">
            <h2 style="color: Red; text-align:center;">Lo sentimos. Usted no tiene acceso a esta sección.</h2>      
        </asp:View>
        </asp:MultiView>
    <div id="PopUpAyuda">
        <asp:UpdatePanel runat="server" ID="UpdatePanelAyuda">
            <ContentTemplate>
                <iframe style="width: 99%; height: 500px;" src="HTMLS%20Ayuda/Perfil%20Admin/Perfiles/Admin%20-%20Perfil.htm"></iframe>
            </ContentTemplate>
        </asp:UpdatePanel>                
    </div>
</asp:Content>
