<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Encargados.aspx.cs" Inherits="Encargados" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
    <script src="Scripts/Encargados.js" type="text/javascript"></script>
    <link href="Styles/Encargados.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript">
        $(document).ready(function () {
            $("a.level1:contains('Encargados')").addClass("item_active");
            $("a.level1:contains('Información Personal')").addClass("item_active");
        });
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <asp:ScriptManager ID="ScriptManager" runat="server">	
    </asp:ScriptManager>

    <asp:MultiView ID="MultiViewEncargado" runat="server">

        <asp:View ID="VistaCompleta" runat="server">

            <asp:UpdatePanel ID="UpdateInfo" runat="server">
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="btnInvisible1" EventName="Click" />
                    <asp:AsyncPostBackTrigger ControlID="btnEliminarEncargado" EventName="Click" />
                    <asp:AsyncPostBackTrigger ControlID="btnModificarEncargado" EventName="Click" />
                </Triggers>
                <ContentTemplate>
                    <!-- Botones Invisibles -->
                    <asp:Button ID="btnInvisible1" CssClass="btnInvisible1 invisible" runat="server" Text="" 
                        onclick="btnInvisible1_Click" />
                        <asp:Button ID="btnInvisible2" CssClass="btnInvisible2 invisible" runat="server" Text="" 
                        onclick="btnInvisible2_Click" CausesValidation="false" />
                    <asp:Button ID="btnInvisibleEnviarCorreo" CssClass="btnInvisibleEnviarCorreo invisible" runat="server" Text="" 
                        onclick="btnInvisibleEnviarCorreo_Click" CausesValidation="false" />
            
                    <!-- Cuerpo con la tabla -->
                    <div style="min-height: 500px;">
                        <span style="width: 100%; font-weight: bold; font-size: 24px; float: left; margin: 20px 0 5px 0; text-align:center;">Módulo para administración de Encargados</span>
                        <span style="width: 100%; font-weight: normal; font-style:italic; font-size: 16px; float: left; margin: 5px 0 20px 0; text-align:center; border-bottom: 2px solid #414141; padding-bottom: 5px;">Por medio de este módulo se pueden insertar, modificar, eliminar y consultar todos los encargados que se encuentran en la base de datos. El encargado es el responsable por un becario, tiene como funciones llevar un control sobre las horas de los becarios bajo su tutela.</span>
                        <div class="buscador">
                            <div style="width: 100%; float:left; font-weight: bold; font-size: 16px; border-bottom: 1px solid #fff; margin-bottom: 5px;">Buscar:</div>
                            <div style="width: 61%; float: left; margin-right: 4%;">
                                <asp:TextBox ID="txtBuscarEncargado"  onkeydown = "enterBuscar(event, 'MainContent_btnBuscar');" CssClass="txtEncargado" runat="server"></asp:TextBox>
                            </div>
                            <div style="width: 35%; float: right">
                                <asp:Button ID="btnBuscar" runat="server" Text="Buscar" CausesValidation="false" CssClass="boton ui-widget ui-state-default ui-corner-all ui-button-text-only" OnClick="btnBuscar_Click" />
                            </div>
                        </div>
                
                        <div class="insertar">
                            <div style="width: 100%; float:left; font-weight: bold; font-size: 16px; border-bottom: 1px solid #fff; margin-bottom: 5px;">Nuevo encargado</div>
                            <asp:Button ID="btnInsertarEncargado" runat="server" Text="Nuevo" 
                                CssClass="boton ui-widget ui-state-default ui-corner-all ui-button-text-only" 
                                onclick="btnInsertarEncargado_Click" CausesValidation="false"/>
                        </div>
                        <asp:GridView ID="GridEncargados"  runat="server" CssClass="table_css" 
                            GridLines="Both" AllowPaging="True" RowStyle-HorizontalAlign="Center" RowStyle-VerticalAlign="Middle"
                                    PageSize="15" 
                            onpageindexchanging="GridEncargados_PageIndexChanging" 
                            onrowcommand="GridEncargados_RowCommand"  
                            onselectedindexchanging="GridEncargados_SelectedIndexChanging" PagerStyle-CssClass="pagerGlobal">
                               <columns>
                                   <asp:ButtonField CommandName="btnSeleccionarTupla_Click" CausesValidation="false" ButtonType="Image" Visible="true" ImageUrl="~/Images/arrow-right.png" ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle"/> 
                               </columns>
                        </asp:GridView>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>  
            <div id="PopUpEncargado">
                <asp:UpdatePanel runat="server" ID="UpdatePopUp">
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="btnInvisible2" EventName="Click" />
                    </Triggers>
                    <ContentTemplate>
                        <div style="width: 96%; padding: 2%; float: left;" >
                            <div style="width: 20%; float: right;">
                                <asp:Button ID="btnEliminarEncargado" runat="server" Text="Eliminar" 
                                        CssClass="boton ui-widget ui-state-default ui-corner-all ui-button-text-only" 
                                        onclick="btnEliminarEncargado_Click" CausesValidation="false" />
                            </div>
                            <div style="width: 20%; float: right; margin-right: 5px;">
                                <asp:Button ID="btnModificarEncargado" runat="server" Text="Modificar" 
                                        CssClass="boton ui-widget ui-state-default ui-corner-all ui-button-text-only" 
                                        onclick="btnModificarEncargado_Click" CausesValidation="false" />
                            </div>
                        </div>

                        <div id="popUpContent" style="width: 96%; padding: 0 2%; float: left; background: #D8D8BF; border-radius: 5px;">
                            <div class="wrap_row_encargado">
                                <div class="wrap_encargado">
                                    <div>
                                        <span class="lblEncargado">Nombre<span style="color:red">*</span></span>
                                        <asp:TextBox ID="txtNombre" runat="server" CssClass="txtEncargado"></asp:TextBox> 
                                    </div>
                                    <div>
                                        <asp:RequiredFieldValidator ControlToValidate="txtNombre" CssClass="error" ID="RequiredFieldValidatorNombre" runat="server" ErrorMessage="*Nombre requerido"  ForeColor="#FF3300" Display="Dynamic" Font-Bold="true" font-size="Small"></asp:RequiredFieldValidator>
                                        <asp:RegularExpressionValidator ID="formato_nombreP" CssClass="error"  runat="server" ErrorMessage="**Se han escrito caracteres inválidos."
                                                            ValidationExpression="^[a-zA-ZñÑáéíóúÁÉÍÓÚüÜ\ |., ]{1,50}$" ControlToValidate="txtNombre" ForeColor="#FF3300" Display="Dynamic" font-size="Small"></asp:RegularExpressionValidator>
                                    </div>
                                </div>                            

                                <div class="wrap_encargado">
                                    <div>
                                        <span class="lblEncargado">Primer Apellido<span style="color:red">*</span></span>
                                        <asp:TextBox ID="txtPrimerApellido" runat="server" CssClass="txtEncargado"></asp:TextBox>
                                    </div>
                                    <div>
                                        <asp:RequiredFieldValidator ControlToValidate="txtPrimerApellido" CssClass="error" ID="RequiredFieldValidator2" runat="server" ErrorMessage="*Primer apellido requerido"  ForeColor="#FF3300" Display="Dynamic" font-size="Small" Font-Bold="true" ></asp:RequiredFieldValidator>
                                        <asp:RegularExpressionValidator ID="RegularExpressionValidatorApellido1" CssClass="error"  runat="server" ErrorMessage="**Se han escrito caracteres inválidos."
                                                            ValidationExpression="^[a-zA-ZñÑáéíóúÁÉÍÓÚüÜ\ |., ]{1,50}$" ControlToValidate="txtPrimerApellido" ForeColor="#FF3300" Display="Dynamic" font-size="Small"></asp:RegularExpressionValidator>
                                    </div>
                                </div>
                            </div>
                            <div class="wrap_row_encargado">
                                <div class="wrap_encargado">
                                    <div>
                                        <span class="lblEncargado">Segundo Apellido</span>
                                        <asp:TextBox ID="txtSegundoApellido" runat="server" CssClass="txtEncargado"></asp:TextBox>
                                    </div>
                                    <div>
                                        <asp:RegularExpressionValidator ID="RegularExpressionValidatorApellido2" CssClass="error"  runat="server" ErrorMessage="**Se han escrito caracteres inválidos."
                                                            ValidationExpression="^[a-zA-ZñÑáéíóúÁÉÍÓÚüÜ\ |., ]{1,50}$" ControlToValidate="txtSegundoApellido" ForeColor="#FF3300" Display="Dynamic" font-size="Small"></asp:RegularExpressionValidator>
                                    </div>
                                </div>

                                <div class="wrap_encargado">
                                    <div>
                                        <span class="lblEncargado">Cédula<span style="color:red">*</span></span>
                                        <asp:TextBox ID="txtCedula" runat="server" CssClass="txtEncargado"></asp:TextBox>
                                    </div>
                                    <div>
                                        <span class="lblEncargado" style="color:gray; font-size:smaller;">Ej: 204440444</>
                                    </div>
                                    <div>
                                        <asp:RequiredFieldValidator ControlToValidate="txtCedula" CssClass="error" ID="RequiredFieldValidatorCedula" runat="server" ErrorMessage="*Cédula requerida"  ForeColor="#FF3300" Display="Dynamic" font-size="Small" Font-Bold="true"></asp:RequiredFieldValidator>
                                        <asp:RegularExpressionValidator ID="RegularExpressionValidatorCedula" CssClass="error"  runat="server" ErrorMessage="*Se han escrito caracteres inválidos. Ingrese únicamente caracteres numéricos"
                                                            ValidationExpression="[0-9]+" ControlToValidate="txtCedula" ForeColor="#FF3300" Display="Dynamic" font-size="Small"></asp:RegularExpressionValidator>
                                        <asp:RegularExpressionValidator ID="RegularExpressionValidatorCedulaTam" CssClass="error"  runat="server" ErrorMessage="*Valor de la cédula entre 9 y 15 números"
                                                            ValidationExpression=".{9,15}" ControlToValidate="txtCedula" ForeColor="#FF3300" Display="Dynamic" font-size="Small"></asp:RegularExpressionValidator>
                                    </div>
                                </div>
                            </div>
                            <div class="wrap_row_encargado">
                                <div class="wrap_encargado">
                                    <div>
                                        <span class="lblEncargado">Correo<span style="color:red">*</span></span>
                                        <asp:TextBox ID="txtCorreo" runat="server" CssClass="txtEncargado"></asp:TextBox>
                                    </div>
                                    <div>
                                        <asp:RequiredFieldValidator ControlToValidate="txtCorreo" CssClass="error" ID="RequiredFieldValidator3" runat="server" ErrorMessage="*Correo requerido"  ForeColor="#FF3300" Display="Dynamic" font-size="Small" Font-Bold="true" ></asp:RequiredFieldValidator>
                                        <span class="lblEncargado" style="color:gray; font-size:smaller;">Ej: ejemplo@email.com</>
                                    </div>
                                    <div>
                                        <asp:RegularExpressionValidator ID="RegularExpressionValidatorCorreo" CssClass="error"  runat="server" ErrorMessage="**Formato de correo incorrecto"
                                                            ValidationExpression="^[_a-z0-9-]+(\.[_a-z0-9-]+)*@[a-z0-9-]+(\.[a-z0-9-]+)*(\.[a-z]{2,3})$" ControlToValidate="txtCorreo" ForeColor="#FF3300" Display="Dynamic" font-size="Small"></asp:RegularExpressionValidator>
                                    </div>
                                </div>

                                <div class="wrap_encargado">
                                    <div>
                                        <span class="lblEncargado">Teléfono de habitación</span>
                                        <asp:TextBox ID="txtTelFijo" runat="server" CssClass="txtEncargado"></asp:TextBox>
                                    </div>
                                    <div>
                                        <span class="lblEncargado" style="color:gray; font-size:smaller;">Ej: 4444-4444</>
                                    </div>
                                    <div>
                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator1" CssClass="error"  runat="server" ErrorMessage="**Se han escrito caracteres inválidos. Ingrese únicamente caracteres numéricos"
                                                                ValidationExpression="([0-9]|-)+" ControlToValidate="txtTelFijo" ForeColor="#FF3300" Display="Dynamic" font-size="Small"></asp:RegularExpressionValidator>
                                        <asp:RegularExpressionValidator ID="RegularExpressionValidatorTelHabCnt" CssClass="error"  runat="server" ErrorMessage="*Valor del teléfono entre 8 y 15 caracteres"
                                                            ValidationExpression=".{8,15}" ControlToValidate="txtTelFijo" ForeColor="#FF3300" Display="Dynamic" font-size="Small"></asp:RegularExpressionValidator>
                                    </div>
                                </div>
                            </div>
                            <div class="wrap_row_encargado">
                                <div class="wrap_encargado">
                                    <div>
                                        <span class="lblEncargado">Teléfono celular</span>
                                        <asp:TextBox ID="txtCel" runat="server" CssClass="txtEncargado"></asp:TextBox>
                                    </div>
                                    <div>
                                        <span class="lblEncargado" style="color:gray; font-size:smaller;">Ej: 4444-4444</>
                                    </div>
                                    <div>
                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" ErrorMessage="**Se han escrito caracteres inválidos. Ingrese únicamente caracteres numéricos"
                                                                ValidationExpression="([0-9]|-)+" CssClass="error"  ControlToValidate="txtCel" ForeColor="#FF3300" Display="Dynamic" font-size="Small"></asp:RegularExpressionValidator>
                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator4" CssClass="error"  runat="server" ErrorMessage="*Valor del teléfono entre 8 y 15 caracteres"
                                                            ValidationExpression=".{8,15}" ControlToValidate="txtCel" ForeColor="#FF3300" Display="Dynamic" font-size="Small"></asp:RegularExpressionValidator>
                                    </div>
                                </div>

                                <div class="wrap_encargado">
                                    <div>
                                        <span class="lblEncargado">Otro teléfono</span>
                                        <asp:TextBox ID="txtOtroTel" runat="server" CssClass="txtEncargado"></asp:TextBox>
                                    </div>
                                    <div>
                                        <span class="lblEncargado" style="color:gray; font-size:smaller;">Ej: 4444-4444</>
                                    </div>
                                    <div>
                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator3" CssClass="error"  runat="server" ErrorMessage="**Se han escrito caracteres inválidos. Ingrese únicamente caracteres numéricos"
                                                                ValidationExpression="([0-9]|-)+" ControlToValidate="txtOtroTel" ForeColor="#FF3300" Display="Dynamic" font-size="Small"></asp:RegularExpressionValidator>
                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator5" CssClass="error"  runat="server" ErrorMessage="*Valor del teléfono entre 8 y 15 caracteres"
                                                            ValidationExpression=".{8,15}" ControlToValidate="txtOtroTel" ForeColor="#FF3300" Display="Dynamic" font-size="Small"></asp:RegularExpressionValidator>
                                    </div>
                                </div>
                            </div>
                            <div class="wrap_row_encargadoPuesto">
                                <div class="wrap_encargadoPuesto">
                                    <span class="lblEncargado">Puesto</span>
                                    <asp:TextBox ID="txtPuesto" runat="server" TextMode="multiline" CssClass="txtEncargadoPuesto"></asp:TextBox>
                                </div>
                            </div>
                            <div class="wrap_row_encargadoPuesto">
                                <div class="wrap_encargadoPuesto">
                                    <span style="color: Red; margin: 10px 0; float: left; font-size:smaller;">* Campos obligatorios</span>
                                </div>
                            </div>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
    
            <div id="PopUpEliminarEncargado">
                ¿Seguro que desea eliminar?
            </div>
        </asp:View>

        <asp:View ID="VistaParcial" runat="server">
            <span style="width: 60%; font-weight: bold; font-size: 24px; float: left; margin: 20px 0;">Información Personal</span>
            
            <asp:UpdatePanel ID="UpdatePanelParcial" runat="server">
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="btnModificarEncargadoP" EventName="Click" />
                    <asp:AsyncPostBackTrigger ControlID="btnAceptar" EventName="Click" />
                </Triggers>
                <ContentTemplate>
                <div style="width: 40%; padding: 2% 0%; float: left;" >
                    <div style="width: 35%; float: right;">
                        <asp:Button ID="btnModificarEncargadoP" runat="server" Text="Modificar" Visible="true"
                            CssClass="boton ui-widget ui-state-default ui-corner-all ui-button-text-only" 
                            onclick="btnModificarEncargadoParcial_Click" />
                    </div>
                </div>
                <div id="ParcialContent" style="width: 96%; padding: 2%; float: left; background: #D8D8BF; border-radius: 5px;">
                    
                    <div class="wrap_row_encargado">
						<div class="wrap_encargado">
							<div>
								<span class="lblEncargado">Nombre<span style="color:red">*</span></span>
								<asp:TextBox ID="txtNombreP" Enabled="false" runat="server" CssClass="txtEncargado"></asp:TextBox>
							</div>
							<div>
								<asp:RequiredFieldValidator ControlToValidate="txtNombreP" ValidationGroup="EncargadoParcial" CssClass="error" ID="RequiredFieldValidatorNombreP" runat="server" ErrorMessage="*Nombre requerido"  ForeColor="#FF3300" Display="Dynamic" font-size="Small" Font-Bold="true" ></asp:RequiredFieldValidator>
								<asp:RegularExpressionValidator ID="formato_nombre" ValidationGroup="EncargadoParcial" CssClass="error"  runat="server" ErrorMessage="**Se han escrito caracteres inválidos."
													ValidationExpression="^[a-zA-ZñÑáéíóúÁÉÍÓÚüÜ\ |., ]{1,50}$" ControlToValidate="txtNombreP" ForeColor="#FF3300" Display="Dynamic" font-size="Small"></asp:RegularExpressionValidator>
							</div>
						</div>                            

						<div class="wrap_encargado">
							<div>
								<span class="lblEncargado">Primer Apellido<span style="color:red">*</span></span>
								<asp:TextBox ID="txtPrimerApellidoP" Enabled="false" runat="server" CssClass="txtEncargado"></asp:TextBox>
							</div>
							<div>
                                <asp:RequiredFieldValidator ControlToValidate="txtPrimerApellidoP" ValidationGroup="EncargadoParcial" CssClass="error" ID="RequiredFieldValidator1" runat="server" ErrorMessage="*Primer apellido requerido"  ForeColor="#FF3300" Display="Dynamic" font-size="Small" Font-Bold="true" ></asp:RequiredFieldValidator>
								<asp:RegularExpressionValidator ID="RegularExpressionValidatorApellido1P" CssClass="error"  ValidationGroup="EncargadoParcial" runat="server" ErrorMessage="**Se han escrito caracteres inválidos."
													ValidationExpression="^[a-zA-ZñÑáéíóúÁÉÍÓÚüÜ\ |., ]{1,50}$" ControlToValidate="txtPrimerApellidoP" ForeColor="#FF3300" Display="Dynamic" font-size="Small"></asp:RegularExpressionValidator>
							</div>
						</div>
					</div>
					<div class="wrap_row_encargado">
						<div class="wrap_encargado">
							<div>
								<span class="lblEncargado">Segundo Apellido</span>
								<asp:TextBox ID="txtSegundoApellidoP" Enabled="false" runat="server" CssClass="txtEncargado"></asp:TextBox>
							</div>
							<div>
								<asp:RegularExpressionValidator ID="RegularExpressionValidatorApellido2P" CssClass="error"  ValidationGroup="EncargadoParcial" runat="server" ErrorMessage="**Se han escrito caracteres inválidos."
													ValidationExpression="^[a-zA-ZñÑáéíóúÁÉÍÓÚüÜ\ |., ]{1,50}$" ControlToValidate="txtSegundoApellidoP" ForeColor="#FF3300" Display="Dynamic" font-size="Small"></asp:RegularExpressionValidator>
							</div>
						</div>

						<div class="wrap_encargado">
							<div>
								<span class="lblEncargado">Cédula<span style="color:red">*</span></span>
								<asp:TextBox ID="txtCedulaP" runat="server" Enabled="false" CssClass="txtEncargado"></asp:TextBox>
							</div>  
                            <div>
                                <span class="lblEncargado" style="color:gray; font-size:smaller;">Ej: 204440444</>
                            </div>  
                            <div>
                                <asp:RequiredFieldValidator ControlToValidate="txtCedulaP" ValidationGroup="EncargadoParcial" CssClass="error" ID="RequiredFieldValidatorCedulaP" runat="server" ErrorMessage="*Cédula requerida"  ForeColor="#FF3300" font-size="Small" Display="Dynamic" Font-Bold="true"></asp:RequiredFieldValidator>
								<asp:RegularExpressionValidator ID="RegularExpressionValidatorCedulaP" ValidationGroup="EncargadoParcial" CssClass="error"  runat="server" ErrorMessage="**Se han escrito caracteres inválidos. Ingrese únicamente caracteres numéricos"
													ValidationExpression="^([0-9]){0,15}$" ControlToValidate="txtCedulaP" ForeColor="#FF3300" Display="Dynamic" font-size="Small"></asp:RegularExpressionValidator>
					        </div>
					    </div>
					</div>
					<div class="wrap_row_encargado">
						<div class="wrap_encargado">
							<div>
								<span class="lblEncargado">Correo<span style="color:red">*</span></span>
								<asp:TextBox ID="txtCorreoP" runat="server" Enabled="false" CssClass="txtEncargado"></asp:TextBox>
							</div>
                            <div>
                                <span class="lblEncargado" style="color:gray; font-size:smaller;">Ej: ejemplo@email.com</>
                            </div>
							<div>
                                <asp:RequiredFieldValidator ControlToValidate="txtCorreoP" ValidationGroup="EncargadoParcial" CssClass="error" ID="RequiredFieldValidator4" runat="server" ErrorMessage="*Correo requerido"  ForeColor="#FF3300" font-size="Small" Display="Dynamic" Font-Bold="true"></asp:RequiredFieldValidator>
								<asp:RegularExpressionValidator CssClass="error"  ID="RegularExpressionValidatorCorreoP" ValidationGroup="EncargadoParcial" runat="server" ErrorMessage="**Formato de correo incorrecto"
													ValidationExpression="^[_a-z0-9-]+(\.[_a-z0-9-]+)*@[a-z0-9-]+(\.[a-z0-9-]+)*(\.[a-z]{2,3})$" ControlToValidate="txtCorreoP" ForeColor="#FF3300" Display="Dynamic" font-size="Small"></asp:RegularExpressionValidator>
							</div>
						</div>

						<div class="wrap_encargado">
							<div>
								<span class="lblEncargado">Teléfono de habitación</span>
								<asp:TextBox ID="txtTelFijoP" runat="server" Enabled="false" CssClass="txtEncargado"></asp:TextBox>
							</div>
                            <div>
                                <span class="lblEncargado" style="color:gray; font-size:smaller;">Ej: 4444-4444</>
                            </div>
							<div>
								<asp:RegularExpressionValidator ID="RegularExpressionValidator6" CssClass="error"  runat="server" ErrorMessage="**Se han escrito caracteres inválidos. Ingrese únicamente caracteres numéricos"
                                                                ValidationExpression="([0-9]|-)+" ControlToValidate="txtTelFijoP" ForeColor="#FF3300" Display="Dynamic" font-size="Small"></asp:RegularExpressionValidator>
                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator7" CssClass="error"  runat="server" ErrorMessage="*Valor del teléfono entre 8 y 15 caracteres"
                                                            ValidationExpression=".{8,15}" ControlToValidate="txtTelFijoP" ForeColor="#FF3300" Display="Dynamic" font-size="Small"></asp:RegularExpressionValidator>
							</div>
						</div>
					</div>
					<div class="wrap_row_encargado">
						<div class="wrap_encargado">
							<div>
								<span class="lblEncargado">Teléfono celular</span>
								<asp:TextBox ID="txtCelP" runat="server" Enabled="false" CssClass="txtEncargado"></asp:TextBox>
							</div>
                            <div>
                                <span class="lblEncargado" style="color:gray; font-size:smaller;">Ej: 4444-4444</>
                            </div>
							<div>
								<asp:RegularExpressionValidator ID="RegularExpressionValidator8" CssClass="error"  runat="server" ErrorMessage="**Se han escrito caracteres inválidos. Ingrese únicamente caracteres numéricos"
                                                                ValidationExpression="([0-9]|-)+" ControlToValidate="txtCelP" ForeColor="#FF3300" Display="Dynamic" font-size="Small"></asp:RegularExpressionValidator>
                                <asp:RegularExpressionValidator ID="RegularExpressionValidator9" CssClass="error"  runat="server" ErrorMessage="*Valor del teléfono entre 8 y 15 caracteres"
                                                            ValidationExpression=".{8,15}" ControlToValidate="txtCelP" ForeColor="#FF3300" Display="Dynamic" font-size="Small"></asp:RegularExpressionValidator>
							</div>
						</div>

						<div class="wrap_encargado">
							<div>
								<span class="lblEncargado">Otro teléfono</span>
								<asp:TextBox ID="txtOtroTelP" runat="server" Enabled="false" CssClass="txtEncargado"></asp:TextBox>
							</div>
                            <div>
                                <span class="lblEncargado" style="color:gray; font-size:smaller;">Ej: 4444-4444</>
                            </div>
							<div>
								<asp:RegularExpressionValidator ID="RegularExpressionValidator10" CssClass="error"  runat="server" ErrorMessage="**Se han escrito caracteres inválidos. Ingrese únicamente caracteres numéricos"
                                                                ValidationExpression="([0-9]|-)+" ControlToValidate="txtOtroTelP" ForeColor="#FF3300" Display="Dynamic" font-size="Small"></asp:RegularExpressionValidator>
                                <asp:RegularExpressionValidator ID="RegularExpressionValidator11" CssClass="error"  runat="server" ErrorMessage="*Valor del teléfono entre 8 y 15 caracteres"
                                                            ValidationExpression=".{8,15}" ControlToValidate="txtOtroTelP" ForeColor="#FF3300" Display="Dynamic" font-size="Small"></asp:RegularExpressionValidator>
							</div>
						</div>
					</div>
					<div class="wrap_row_encargadoPuesto">
						<div class="wrap_encargadoPuesto">
							<span class="lblEncargado">Puesto</span>
							<asp:TextBox ID="txtPuestoP" runat="server" TextMode="multiline" CssClass="txtEncargadoPuesto" Enabled="false"></asp:TextBox>
						</div>
					</div>
                    <div class="wrap_row_encargadoPuesto">
                                <div class="wrap_encargadoPuesto">
                                    <span style="color: Red; margin: 10px 0; float: left; font-size:smaller;">* Campos obligatorios</span>
                                </div>
                            </div>
                   
                    <div style="width: 90%; padding: 2% 0%; float: left;" >
                        <div style="width: 30%; float: right;">
                            <asp:Button ID="btnAceptar" runat="server" Text="Aceptar" 
                            CssClass="botonParcial ui-widget ui-state-default ui-corner-all ui-button-text-only" 
                            onclick="btnModificarEncargadoParcialAceptar_Click" Visible="false" CausesValidation="true" ValidationGroup="EncargadoParcial" />
                            
                            <asp:Button ID="btnCancelar" runat="server" Text="Cancelar" 
                            CssClass="botonParcial ui-widget ui-state-default ui-corner-all ui-button-text-only" 
                            onclick="btnModificarEncargadoParcialCancelar_Click" Visible="false" CausesValidation="false"/>
                </div>
            </div>

                </div>
                </ContentTemplate>
            </asp:UpdatePanel>
            
                              
        </asp:View>

        <asp:View ID="VistaSinPermiso" runat="server">
            <h2 style="color: Red; text-align:center;">Lo sentimos. Usted no tiene acceso a esta sección.</h2>      
        </asp:View>
    </asp:MultiView>
    
    
    
</asp:Content>

