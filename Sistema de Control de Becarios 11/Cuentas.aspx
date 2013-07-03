<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Cuentas.aspx.cs" Inherits="Cuentas" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
    <script src="Scripts/Cuentas.js" type="text/javascript"></script>
    <link href="Styles/Cuentas.css" rel="stylesheet" type="text/css" />
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" Runat="Server">
    <asp:MultiView ID="multiViewCuentas" runat="server">
       <asp:View ID="vistaAdmin" runat="server">
            <asp:ScriptManager ID="ScriptManager" runat="server">	
            </asp:ScriptManager>
            <asp:UpdatePanel ID="UpdateInfo" runat="server">
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="btnInvisible1" EventName="Click" />
                    <asp:AsyncPostBackTrigger ControlID="btnEliminarCuenta" EventName="Click" />
                    <asp:AsyncPostBackTrigger ControlID="btnModificarCuenta" EventName="Click" />
                    <asp:AsyncPostBackTrigger ControlID="GridViewCuentas" EventName="RowCommand" />
                </Triggers>
                <ContentTemplate>
                    <!-- Botones Invisibles -->
                    <asp:Button ID="btnInvisible1" CssClass="btnInvisible1 invisible" runat="server" Text="" 
                        onclick="btnInvisible1_Click" CausesValidation="true" />
                        <asp:Button ID="btnInvisible2" CssClass="btnInvisible2 invisible" runat="server" Text="" 
                        onclick="btnInvisible2_Click" CausesValidation="true" />
            
                    <!-- Cuerpo con la tabla -->
                    <div style="min-height: 500px;">
                        <span style="width: 100%; font-weight: bold; font-size: 24px; float: left; margin: 20px 0 5px 0; text-align:center;">Módulo para administración de Cuentas</span>
                        <span style="width: 100%; font-weight: normal; font-style:italic; font-size: 16px; float: left; margin: 5px 0 20px 0; text-align:center; border-bottom: 2px solid #414141; padding-bottom: 5px;">Por medio de este módulo se pueden insertar, modificar, eliminar y consultar todas las cuentas que se encuentran en la base de datos. Las cuentas son usadas para tener acceso al sistema.</span>
                        <div class="buscador">
                            <div style="width: 100%; float:left; font-weight: bold; font-size: 16px; border-bottom: 1px solid #fff; margin-bottom: 5px;">Buscar:</div>
                            <div style="width: 61%; float: left; margin-right: 4%;">
                                <asp:TextBox ID="txtBuscarCuenta" onkeydown = "enterBuscar(event, 'MainContent_btnBuscar');" CssClass="txtEncargado" runat="server"></asp:TextBox>
                            </div>
                            <div style="width: 35%; float: right">
                                <asp:Button ID="btnBuscar" runat="server" Text="Buscar" CssClass="boton ui-widget ui-state-default ui-corner-all ui-button-text-only" OnClick="btnBuscar_Click" CausesValidation="false"/>
                            </div>
                        </div>
        
                        <div class="insertar">
                            <div style="width: 100%; float:left; font-weight: bold; font-size: 16px; border-bottom: 1px solid #fff; margin-bottom: 5px;">Nueva Cuenta</div>
                            <asp:Button ID="btnInsertarCuenta" runat="server" Text="Nuevo" 
                                CssClass="boton ui-widget ui-state-default ui-corner-all ui-button-text-only" 
                                onclick="btnInsertarCuenta_Click" CausesValidation="false"/>
                        </div>
                        <asp:GridView ID="GridViewCuentas" runat="server" CssClass="tabla_cuentas" AllowPaging="true" PageSize="15" OnRowCommand="GridViewCuentas_RowCommand" OnPageIndexChanging="GridViewCuentas_PageIndexChanging" PagerStyle-CssClass="pagerGlobal" AutoGenerateColumns="false">
                            <Columns>
                                <asp:ButtonField CommandName="seleccionarPosibleCuenta" CausesValidation="false" ButtonType="Image" Visible="true" ImageUrl="Images/arrow-right.png"/>
                                <asp:TemplateField HeaderText="Usuario"> 
                                        <ItemTemplate> 
                                            <asp:TextBox ID="txtUsuarioG" CssClass="textoGrid" runat ="server" Text='<%# Bind("Nombre") %>' Enabled="false"></asp:TextBox>
                                        </ItemTemplate> 
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Contraseña"> 
                                        <ItemTemplate> 
                                            <asp:TextBox ID="txtContrasenna" type="password" CssClass="textoGrid" runat ="server" Text='<%# Bind("Contraseña") %>' Enabled="false"></asp:TextBox>
                                        </ItemTemplate> 
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Ultimo Acceso"> 
                                        <ItemTemplate> 
                                            <asp:TextBox ID="txtUltimoAcceso" CssClass="textoGrid" runat ="server" Text='<%# Bind("UltimoAcceso") %>' Enabled="false"></asp:TextBox>
                                        </ItemTemplate> 
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
    
            <div id="PopUp" title="Perfiles">
                <asp:UpdatePanel runat="server" ID="UpdatePopUp">
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="btnInvisible2" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="drpDownPerfiles" EventName="SelectedIndexChanged" />
                    </Triggers>
                    <ContentTemplate>
                        <table style="width: 100%;">
                            <tr>
                                <td class="auto-style2">&nbsp;</td>
                                <td class="auto-style6">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                    <asp:Button ID="btnEliminarCuenta" runat="server" CssClass="boton ui-widget ui-state-default ui-corner-all ui-button-text-only" onclick="btnEliminarCuenta_Click" Text="Eliminar" Width="139px" />
                                    <asp:Button ID="btnModificarCuenta" runat="server" CssClass="boton ui-widget ui-state-default ui-corner-all ui-button-text-only" onclick="btnModificarCuenta_Click" Text="Modificar" Width="135px" />
                                </td>
                                <td>&nbsp;</td>
                            </tr>
                            <tr>
                                <td class="auto-style2">Usuario</td>
                                <td class="auto-style6">
                                    <asp:TextBox ID="txtUsuario" runat="server" Width="323px" CausesValidation="true" MaxLength="50"></asp:TextBox>
                                    <asp:TextBox ID="txtFechaAux" runat="server" Width="323px" CausesValidation="false" Enabled ="false" Visible ="false"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidatorUser" runat="server" ErrorMessage="* Campo Requerido" ControlToValidate="txtUsuario" Display="Dynamic"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="rxvUsuario" runat="server" ErrorMessage="El nombre de usuario no puede contener caracteres blancos, comillas dobles o sencillas"
						        ValidationExpression="([^\s&quot;' ])+" ControlToValidate="txtUsuario" Display="Dynamic"/>
                                </td>
                                <td>&nbsp;</td>
                            </tr>
                            <tr>
                                <td class="auto-style2">
                                    Contraseña</td>
                                <td class="auto-style6">
                                    <asp:TextBox ID="cntUsuario" runat="server" Width="320px" CausesValidation="true" type="password" MaxLength="50"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidatorCnt" runat="server" ErrorMessage="* Contraseña de usuario requerida" ControlToValidate="cntUsuario" Display="Dynamic"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="rxvContraseña" runat="server" ErrorMessage="La contraseña no puede contener caracteres blancos, comillas dobles o sencillas"
						        ValidationExpression="([^\s&quot;' ])+" ControlToValidate="cntUsuario" Display="Dynamic"/>
                                </td>
                                <td>
                                    &nbsp;</td>
                            </tr>
                            <tr>
                                <td class="auto-style2">Confirmar Contraseña</td>
                                <td class="auto-style6">
                                    <asp:TextBox ID="cofCntUsuario" runat="server" Width="320px" type="password" MaxLength="50"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="* Contraseña de usuario requerida" ControlToValidate="cofCntUsuario" Display="Dynamic"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ErrorMessage="La contraseña no puede contener caracteres blancos, comillas dobles o sencillas"
						        ValidationExpression="([^\s&quot;' ])+" ControlToValidate="cofCntUsuario" Display="Dynamic"/>
                                    <asp:CompareValidator runat="server" id="cmpClaves" ControlToValidate="cntUsuario" ControlToCompare="cofCntUsuario"
                                operator="Equal" type="String" ErrorMessage="Las claves introducidas no son iguales" Display="Dynamic"/>
                                </td>
                                <td>&nbsp;</td>
                            </tr>
                            <tr>
                                <td class="auto-style2">Asignar perfil</td>
                                <td class="auto-style6">
                                    <asp:DropDownList ID="drpDownPerfiles" runat="server" 
                                        onselectedindexchanged="drpDownPerfiles_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                <td>&nbsp;</td>
                            </tr>
                            <tr>
                               <td class="auto-style2">
                                   <asp:Label ID="lblCedula" runat="server" Text="Seleccione una persona:"></asp:Label>
                               </td>
                               <td class="auto-style6"> 
                                  <asp:DropDownList ID="drpPersona" runat="server" AutoPostBack="true" OnSelectedIndexChanged="drpDownPersona_SelectedIndexChanged"></asp:DropDownList>
                                  <asp:TextBox ID="txtNombrePersona" runat="server" Text="" Enabled="false"></asp:TextBox>
                               </td>
                            </tr>
                        </table>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>

            <div id="PopUpEliminar">
                ¿Seguro que desea eliminar?
            </div>
        </asp:View>
        <asp:View ID="viewEncBec" runat="server">
            <div id="containerCuentaPers">
                <div id="pnlBtnMod">
                    <asp:Button ID="btnModPers" runat="server" CssClass="boton ui-widget ui-state-default ui-corner-all ui-button-text-only" onclick="btnModificarCuentaPers_Click" Text="Modificar" Width="135px" CausesValidation="false" />
                </div>
                <asp:UpdatePanel ID="updCuentaPersonal" runat="server">
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="btnModPers" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="btnAcepPers" EventName="Click" />
                    </Triggers>
                    <ContentTemplate>
                        <div id="infCuenta">
                            <div id="pnlUsuarioPers" class="cont">
                                    <asp:Label ID="lblUsuarioPers" runat="server" Text="Usuario" CssClass="etiq"></asp:Label>
                                    <asp:TextBox ID="txtUsuarioPers" runat="server" CssClass="txtBox" MaxLength="50"></asp:TextBox>
                            </div>
                            <div id="pnlContrasenaPers" class="cont">
                                <asp:Label ID="lblContrasenaPer" runat="server" Text="Contraseña" CssClass="etiq"></asp:Label>
                                <asp:TextBox ID="txtContrasenaPers" runat="server" CausesValidation="true" type="password" CssClass="txtBox" MaxLength="50"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ErrorMessage="* Contraseña de usuario requerida" ControlToValidate="txtContrasenaPers" Display="Dynamic"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator3" runat="server" ErrorMessage="La contraseña no puede contener caracteres blancos, comillas dobles o sencillas"
					            ValidationExpression="([^\s&quot;' ])+" ControlToValidate="txtContrasenaPers" Display="Dynamic"/>
                            </div>
                            <div id="pnlConfContrasenaPers" class="cont">
                                <asp:Label ID="lblConfContrasenaPer" runat="server" Text="Confirmar contraseña" CssClass="etiq"></asp:Label>
                                <asp:TextBox ID="txtConfContrasenaPers" runat="server" CausesValidation="true" type="password" CssClass="txtBox" MaxLength="50"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" ErrorMessage="* Contraseña de usuario requerida" ControlToValidate="txtConfContrasenaPers" Display="Dynamic"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator6" runat="server" ErrorMessage="La contraseña no puede contener caracteres blancos, comillas dobles o sencillas"
					            ValidationExpression="([^\s&quot;' ])+" ControlToValidate="txtConfContrasenaPers" Display="Dynamic"/>
                                <asp:CompareValidator runat="server" id="CompareValidator2" ControlToValidate="txtConfContrasenaPers" ControlToCompare="txtContrasenaPers"
                                operator="Equal" type="String" ErrorMessage="Las claves introducidas no son iguales" Display="Dynamic"/>
                            </div>
                            <div id="pnlDrpDown" class="cont">
                                <asp:Label ID="lblAsigPerfil" runat="server" Text="Perfil" CssClass="etiq"></asp:Label>
                                <asp:TextBox ID="txtPerfil" runat="server" CssClass="txtBox"></asp:TextBox>
                            </div>
                            <div id="pnlCedulaPers" class="cont">
                                <asp:Label ID="lblCedulaPers" runat="server" Text="Cédula" CssClass="etiq"></asp:Label>
                                <asp:TextBox ID="txtCedulaPers" runat="server" CssClass="txtBox"></asp:TextBox>
                            </div>
                            <div id="pnlBtnAcCan" class="cont">
                                <asp:Button ID="btnAcepPers" runat="server" CssClass="boton ui-widget ui-state-default ui-corner-all ui-button-text-only" onclick="btnAceptarCuentaPers_Click" Text="Aceptar" Width="135px" />
                                <asp:Button ID="btnCancPers" runat="server" CssClass="boton ui-widget ui-state-default ui-corner-all ui-button-text-only" onclick="btnCancelarCuentaPers_Click" Text="Cancelar" Width="135px" CausesValidation="false"/>
                            </div>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </asp:View>
        <asp:View ID="VistaSinPermiso" runat="server">
            <h2 style="color: Red; text-align:center;">Lo sentimos. Usted no tiene acceso a esta sección.</h2>      
        </asp:View>
    </asp:MultiView>
</asp:Content>


