<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Becarios.aspx.cs" Inherits="Becarios" %>


<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
   <script src="Scripts/Becarios.js" type="text/javascript"></script>
   <link href="Styles/Becarios.css" rel="stylesheet" type="text/css" />
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">

    <asp:ScriptManager ID="ScriptManager" runat="server">	
    </asp:ScriptManager>


    <asp:MultiView ID="MultiViewBecario" runat="server">

        <asp:View ID="VistaCompleta" runat="server">

       <asp:UpdatePanel ID="UpdateInfo" runat="server">

            <Triggers>
                 <asp:AsyncPostBackTrigger ControlID="btnInvisible1" EventName="Click" />
                 <asp:AsyncPostBackTrigger ControlID="btnInvisible3" EventName="Click" />
                 <asp:AsyncPostBackTrigger ControlID="btnEliminarBecarioDatos" EventName="Click" />
                 <asp:AsyncPostBackTrigger ControlID="btnModificarBecarioDatos" EventName="Click" />
             </Triggers>

             <ContentTemplate>

                    <!-- Botones Invisibles -->
                    <asp:Button ID="btnInvisible1" CssClass="btnInvisible1 invisible" runat="server" Text="" 
                        onclick="btnInvisible1_Click" CausesValidation="true" />
                        <asp:Button ID="btnInvisible2" CssClass="btnInvisible2 invisible" runat="server" Text="" 
                        onclick="btnInvisible2_Click" />
                    <asp:Button ID="btnInvisible3" CssClass="btnInvisible3 invisible" runat="server" Text="" 
                        onclick="btnInvisible3_Click" CausesValidation="false" />
                    <asp:Button ID="btnInvisibleEnviarCorreo" CssClass="btnInvisibleEnviarCorreo invisible" runat="server" Text="" 
                        onclick="btnInvisibleEnviarCorreo_Click" CausesValidation="false" />
            
                    <!-- Botones INSERTAR-BUSCAR -->
                    <div style="min-height: 500px;">
                        <span style="width: 100%; font-weight: bold; font-size: 24px; float: left; margin: 20px 0 5px 0; text-align:center;">Módulo para administración de Becarios</span>
                        <span style="width: 100%; font-weight: normal; font-style:italic; font-size: 16px; float: left; margin: 5px 0 20px 0; text-align:center; border-bottom: 2px solid #414141; padding-bottom: 5px;">Por medio de este módulo se pueden insertar, modificar, eliminar y consultar todos los becarios que se encuentran en la base de datos.</span>
                        <div class="buscador">
                            <div style="width: 100%; float:left; font-weight: bold; font-size: 16px; border-bottom: 1px solid #fff; margin-bottom: 5px;">Buscar:</div>
                            <div style="width: 61%; float: left; margin-right: 4%;">
                                <asp:TextBox ID="txtBuscarBecarios" onkeydown = "enterBuscar(event, 'MainContent_btnBuscar');" CssClass="txtEncargado" runat="server"></asp:TextBox>
                            </div>
                            <div style="width: 35%; float: right">
                                <asp:Button ID="btnBuscar" runat="server" Text="Buscar" CssClass="boton ui-widget ui-state-default ui-corner-all ui-button-text-only" 
                                 onclick="btnBuscarBecario_Click" CausesValidation="false" />
                            </div>
                        </div>
        
                        <div class="insertar">
                            <div style="width: 100%; float:left; font-weight: bold; font-size: 16px; border-bottom: 1px solid #fff; margin-bottom: 5px;">Nuevo becario</div>
                            <asp:Button ID="btnInsertarBecarios" runat="server" Text="Nuevo" 
                                CssClass="boton ui-widget ui-state-default ui-corner-all ui-button-text-only" 
                                onclick="btnInsertarEncargado_Click" CausesValidation="false" />
                        </div>
                         
                         <!-- GRID BECARIOS -->
                        <div id="divGridBecarios">

                        <asp:GridView ID="gridBecarios" runat="server" GridLines="Both" RowStyle-HorizontalAlign="Center" RowStyle-VerticalAlign="Middle"
                          AllowPaging="True" onselectedindexchanging="gridBecarios_SelectedIndexChanged"
                           onpageindexchanging="gridBecarios_PageIndexChanging" PageSize="15"
                           onrowcommand="gridBecarios_RowCommand"  CssClass="gridBecario"  PagerStyle-CssClass="pagerGlobal">
                        <Columns>
                         <asp:ButtonField CommandName="btnSeleccionarTupla_Click" CausesValidation="false" ButtonType="Image" Visible="true" ImageUrl="~/Images/arrow-right.png" ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle"/> 
                     </Columns>
                     </asp:GridView>

                    </div>
                    </div>

             </ContentTemplate>

       </asp:UpdatePanel>  

       <!-- POP UP -->
      <div id="PopUp" style="min-height: 375px;">
        
        <div id="tabs" style="width:96%; margin-left:1%">

           <asp:UpdatePanel runat="server" ID="UpdatePopUp">
             <Triggers>
                <asp:AsyncPostBackTrigger ControlID="btnInvisible1" EventName="Click" />
                <asp:AsyncPostBackTrigger ControlID="btnInvisible2" EventName="Click" />
                <asp:AsyncPostBackTrigger ControlID="btnInvisible3" EventName="Click" />
                <asp:AsyncPostBackTrigger ControlID="btnAgregaLenguaje" EventName="Click" />
                <asp:AsyncPostBackTrigger ControlID="btnAgregaIdioma" EventName="Click" />
                <asp:AsyncPostBackTrigger ControlID="btnAgregaAreaInteres" EventName="Click" />
                 <asp:AsyncPostBackTrigger ControlID="btnAgregaCualidad" EventName="Click" />
             </Triggers>
           <ContentTemplate>

                <ul>
		          <li><a href="#tabs-1">Datos Personales</a></li>
		          <li><a href="#tabs-2">Perfil</a></li>
	            </ul>

                    <!-- DATOS PERSONALES -->
                	<div id="tabs-1" style= "padding: 5px 2%; width: 96%; min-height:445px" >
		                 
                         <div style="width: 96%; padding: 2%; float: left;" >
                             <div style="width: 20%; float: right; margin-left: 10px;">
                                   <asp:Button ID="btnEliminarBecarioDatos" runat="server" Text="Eliminar" 
                                    CssClass="boton ui-widget ui-state-default ui-corner-all ui-button-text-only" 
                                    onclick="btnEliminarBecario_Click" />
                              </div>

                              <div style="width: 20%; float: right;">
                                   <asp:Button ID="btnModificarBecarioDatos" runat="server" Text="Modificar" 
                                     CssClass="boton ui-widget ui-state-default ui-corner-all ui-button-text-only" 
                                    onclick="btnModificarBecario_Click" />
                              </div>
                              
                         </div>


                          <div style="width: 96%; padding: 0 2%; float: left; background: #D8D8BF; border-radius: 5px;">

                              <div style="font-family:Segoe UI,Verdana,Helvetica,Sans-Serif;font-size:15px;margin-top: 15px;margin-bottom: 15px">
                                <asp:Label ID="lblInstTab1" runat="server"
                                  Text="" Visible="false"></asp:Label>
                             </div>

                             <div class="wrap_row_becario" style="display:none; border: 1px solid #414141;
border-radius: 5px; background: #F9E5B1;">

                                <div style=" width: 20%; min-height: 80px; max-height: 100px; float: left; text-align: center; background: #FFF; border-radius: 5px; padding: 5px 0;">
                                    <asp:UpdatePanel ID="UpdateImage" runat="server">
                                        <ContentTemplate>
                                            <div style="height:80px">
                                            <asp:Image ID="imgBecario" ImageUrl="Images/avatar.png" runat="server" CssClass="img_becario" />
                                            </div>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                    

                                    <div style="width: 100%; float: left;">
                                        <asp:Label ID="Throbber" runat="server" Style="display: none; width: 100%;">
                                            <img src="Images/indicator.gif" align="absmiddle" alt="loading" />
                                        </asp:Label>
                                    </div>
                                </div>
                                <div style="width: 75%; padding-left: 5%; float: right; text-align: right; margin-top: 55px; font-size: 13px;">
                                    <ajaxToolkit:AsyncFileUpload ID="AsyncFileUpload1" Width="400px"  runat="server" 
                                        OnClientUploadError="uploadError" OnClientUploadStarted="StartUpload" 
                                        OnClientUploadComplete="UploadComplete" 
                                        CompleteBackColor="Lime" UploaderStyle="Traditional" 
                                        ErrorBackColor="Red" ThrobberID="Throbber" 
                                        onuploadedcomplete="AsyncFileUpload1_UploadedComplete" 
                                        UploadingBackColor="#66CCFF" />
                                    <asp:Label ID="lblStatus" runat="server" Style="font-family: Arial; font-size: small;"></asp:Label>
                                </div>
                            </div>

                             <div class="wrap_row_becario">

                                <div class="wrap_becario">

                                   <div>
                                    <span class="lblBecario">Nombre<span style="color: Red;">*</span></span>
                                    <asp:TextBox ID="txtNombre" runat="server" CssClass="txtBecario"></asp:TextBox>
                                   </div>

                                  <div style="width: 89%; font-size:12px">

                                    <asp:RequiredFieldValidator ControlToValidate="txtNombre" CssClass="error" ID="obligatorio_nombre" runat="server" ErrorMessage="* Campo obligatorio"  ForeColor="#FF3300" Display="Dynamic" Font-Bold="true" ></asp:RequiredFieldValidator>                                   
                                    
                                    <asp:RegularExpressionValidator ID="formato_nombre" runat="server" 
                                     ErrorMessage ="Se han escrito caracteres inválidos" Display="Dynamic"
                                     ValidationExpression = "[a-zA-Z\u00c1\u00c9\u00cd\u00d3 
                                     \u00da\u00e1\u00e9\u00ed\u00f3\u00fa\u00d1\u00f1\u00c0\u00c2\u00c3\u00c4\u00c5\u00c8
                                     \u00ca\u00cb\u00cc\u00ce\u00cf\u00d2\u00d4\u00d5\u00d6\u00db\u00dc\u00e0\u00e2
                                     \u00e3\u00e4\u00e7\u00e8\u00eb\u00ec\u00ee\u00ef\u00f2\u00f4\u00f5\u00f6\u00f9\u00fb\u00fc\u00fd\u00ff]+" 
                                      ControlToValidate="txtNombre" ForeColor="#FF3300">
                                    </asp:RegularExpressionValidator>
                                 </div>

                                </div>

                                <div class="wrap_becario">

                                    <div>
                                       <span class="lblBecario">Primer Apellido<span style="color: Red;">*</span></span>
                                       <asp:TextBox ID="txtApellido1" runat="server" CssClass="txtBecario"></asp:TextBox>
                                    </div>
                                     
                                    <div style="width: 89%; font-size:12px">                       
                                       <asp:RequiredFieldValidator ControlToValidate="txtApellido1" CssClass="error" ID="obligatorio_apellido1" runat="server" ErrorMessage="* Campo obligatorio"  ForeColor="#FF3300" Display="Dynamic" Font-Bold="true" ></asp:RequiredFieldValidator>
                                    
                                       <asp:RegularExpressionValidator ID="formato_apellido1" runat="server" 
                                       ErrorMessage ="Se ha escrito caracteres inválidos"  Display="Dynamic"
                                       ValidationExpression = "[a-zA-Z\u00c1\u00c9\u00cd\u00d3 
                                         \u00da\u00e1\u00e9\u00ed\u00f3\u00fa\u00d1\u00f1\u00c0\u00c2\u00c3\u00c4\u00c5\u00c8
                                         \u00ca\u00cb\u00cc\u00ce\u00cf\u00d2\u00d4\u00d5\u00d6\u00db\u00dc\u00e0\u00e2
                                         \u00e3\u00e4\u00e7\u00e8\u00eb\u00ec\u00ee\u00ef\u00f2\u00f4\u00f5\u00f6\u00f9\u00fb\u00fc\u00fd\u00ff]+"
                                       ControlToValidate="txtApellido1" ForeColor="#FF3300">
                                       </asp:RegularExpressionValidator>
                                    </div>

                                </div>
                    
                                <div class="wrap_becario">
                                    
                                    <div>
                                      <span class="lblBecario">Segundo Apellido</span>
                                      <asp:TextBox ID="txtApellido2" runat="server" CssClass="txtBecario"></asp:TextBox>
                                    </div>

                                    <div style="width: 89%; font-size:12px">                                                                                    
                                       <asp:RegularExpressionValidator ID="formato_apellido2" runat="server" 
                                       ErrorMessage ="Se ha escrito caracteres inválidos"  Display="Dynamic"
                                       ValidationExpression = "[a-zA-Z\u00c1\u00c9\u00cd\u00d3 
                                         \u00da\u00e1\u00e9\u00ed\u00f3\u00fa\u00d1\u00f1\u00c0\u00c2\u00c3\u00c4\u00c5\u00c8
                                         \u00ca\u00cb\u00cc\u00ce\u00cf\u00d2\u00d4\u00d5\u00d6\u00db\u00dc\u00e0\u00e2
                                         \u00e3\u00e4\u00e7\u00e8\u00eb\u00ec\u00ee\u00ef\u00f2\u00f4\u00f5\u00f6\u00f9\u00fb\u00fc\u00fd\u00ff]+"
                                       ControlToValidate="txtApellido2" ForeColor="#FF3300">
                                       </asp:RegularExpressionValidator>
                                    </div>

                                </div>

                            </div>

                            <div class="wrap_row_becario">

                                <div class="wrap_becario">

                                    <div>
                                       <span class="lblBecario">Carné<span style="color: Red;">*</span></span>
                                       <asp:TextBox ID="txtCarne" runat="server" CssClass="txtBecario"></asp:TextBox>
                                        <span style="font-size:11px; display:block">(Ejm:a00000)</span>
                                    </div>

                                    <div style="width: 89%; font-size:12px">                       
                                       <asp:RequiredFieldValidator ControlToValidate="txtCarne" CssClass="error" ID="obligatorio_carné" runat="server" ErrorMessage="* Campo obligatorio"  ForeColor="#FF3300" Display="Dynamic" Font-Bold="true" ></asp:RequiredFieldValidator>
                                    
                                       <asp:RegularExpressionValidator ID="formato_carné" runat="server" 
                                       ErrorMessage ="Formato de carné inválido"  Display="Dynamic"
                                       ValidationExpression = "[a-z|A-Z|0-9][0-9]{5}$" ControlToValidate="txtCarne" ForeColor="#FF3300">
                                       </asp:RegularExpressionValidator>
                                    </div>                                
                                
                                </div>

                                <div class="wrap_becario">

                                    <div>
                                      <span class="lblBecario">Cédula<span style="color: Red;">*</span></span>
                                      <asp:TextBox ID="txtCedula" runat="server" CssClass="txtBecario"></asp:TextBox>                               
                                    </div>

                                    <div style="width: 89%; font-size:12px">                       
                                       <asp:RequiredFieldValidator ControlToValidate="txtCedula" CssClass="error" ID="obligatorio_cedula" runat="server" ErrorMessage="* Campo obligatorio"  ForeColor="#FF3300" Display="Dynamic" Font-Bold="true" ></asp:RequiredFieldValidator>
                                    
                                       <asp:RegularExpressionValidator ID="fomato_cedula" runat="server" 
                                       ErrorMessage ="Se ha escrito caracteres inválidos"  Display="Dynamic"
                                       ValidationExpression = "^([0-9]){0,15}$" ControlToValidate="txtCedula" ForeColor="#FF3300">
                                       </asp:RegularExpressionValidator>
                                    </div>    

                                </div>
                    
                                 <div class="wrap_becario">

                                    <div>
                                      <span class="lblBecario">Correo Electrónico<span style="color: Red;">*</span></span>
                                       <asp:TextBox ID="txtCorreo" runat="server" CssClass="txtBecario"></asp:TextBox>
                                        <span style="font-size:11px; display:block">(Ejm:miNombre@gmail.com)</span>
                                    </div>
                                    
                                    <div style="width: 89%; font-size:12px">                       
                                       <asp:RequiredFieldValidator ControlToValidate="txtCorreo" CssClass="error" ID="RequiredFieldValidator1" runat="server" ErrorMessage="* Campo obligatorio"  ForeColor="#FF3300" Display="Dynamic" Font-Bold="true" ></asp:RequiredFieldValidator>
                                    
                                       <asp:RegularExpressionValidator ID="formato_correo" runat="server" 
                                       ErrorMessage ="Formato de correo inválido"  Display="Dynamic"
                                       ValidationExpression = "^[_a-z0-9-]+(\.[_a-z0-9-]+)*@[a-z0-9-]+(\.[a-z0-9-]+)*(\.[a-z]{2,3})$" ControlToValidate="txtCorreo" ForeColor="#FF3300">
                                       </asp:RegularExpressionValidator>
                                    </div>   
                                 
                                 </div>

                            </div>

                            <div class="wrap_row_becario">

                                <div class="wrap_becario">

                                    <div>
                                      <span class="lblBecario">Teléfono Fijo</span>
                                      <asp:TextBox ID="txtTelFijo" runat="server" CssClass="txtBecario"></asp:TextBox>
                                      <span style="font-size:11px; display:block">(Ejm:2211-8000)</span>
                                    </div>

                                    <div style="width: 89%; font-size:12px">       
                                     <asp:RegularExpressionValidator ID="formato_telFijo" runat="server" 
                                      ErrorMessage ="Formato de teléfono inválido"  Display="Dynamic"
                                      ValidationExpression = "^([0-9]|-){8,11}$" ControlToValidate="txtTelFijo" ForeColor="#FF3300">
                                     </asp:RegularExpressionValidator>
                                   </div>

                                </div>
                    
                                <div class="wrap_becario">

                                    <div>       
                                      <span class="lblBecario">Teléfono Móvil</span>
                                      <asp:TextBox ID="txtCel" runat="server" CssClass="txtBecario"></asp:TextBox>
                                     <span style="font-size:11px; display:block">(Ejm:8800-0000)</span>
                                   </div>

                                    <div style="width: 89%; font-size:12px">
                                      <asp:RegularExpressionValidator ID="formato_cel" runat="server" 
                                       ErrorMessage ="Formato de teléfono inválido"  Display="Dynamic"
                                      ValidationExpression  = "^([0-9]|-){8,11}$" ControlToValidate="txtCel" ForeColor="#FF3300">
                                      </asp:RegularExpressionValidator>
                                   </div>
                                
                                </div>

                                <div class="wrap_becario">

                                    <div>
                                      <span class="lblBecario">Otro Teléfono</span>
                                      <asp:TextBox ID="txtOtroTel" runat="server" CssClass="txtBecario"></asp:TextBox>
                                      <span style="font-size:11px; display:block">(Ejm:2211-0000)</span>
                                    </div>

                                    <div style="width: 89%; font-size:12px">       
                                      <asp:RegularExpressionValidator ID="formato_otroTel" runat="server" 
                                       ErrorMessage ="Formato de teléfono inválido"  Display="Dynamic"
                                      ValidationExpression  = "^([0-9]|-){8,11}$" ControlToValidate="txtOtroTel" ForeColor="#FF3300">
                                      </asp:RegularExpressionValidator>
                                    </div>

                                </div>
                                <span style="color: Red; margin: 10px 0; float: left;">* Campos obligatorios</span>
                            </div>

                            

                          </div>

	                </div>

                    <!--PERFIL DEL BECARIO-->
	                <div id="tabs-2" style= "width:96%; min-height:560px">

                       <div style="width: 96%; padding: 2%; float: left;" >

                            <div style="width: 20%; float: right;">
                                <asp:Button ID="btnModificarBecarioPerfil" runat="server" Text="Modificar" 
                                 CssClass="boton ui-widget ui-state-default ui-corner-all ui-button-text-only" 
                                 onclick="btnModificarBecario_Click" />
                            </div>
                        </div>                

                     <div style="width: 95%; padding: 0 2%; float: left; background: #D8D8BF; border-radius: 5px;">


                       <div style="font-family:Segoe UI,Verdana,Helvetica,Sans-Serif;font-size:13px;margin-top: 15px;margin-bottom: 15px">
                           <asp:Label ID="lblInstTab2" runat="server" Text="" Visible="false"> </asp:Label>
                       </div>
              
                       <asp:UpdatePanel runat="server" ID="UpdatePanel1">

                         <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="btnAgregaLenguaje" EventName="Click" />
                             <asp:AsyncPostBackTrigger ControlID="btnAgregaIdioma" EventName="Click" />
                             <asp:AsyncPostBackTrigger ControlID="btnAgregaAreaInteres" EventName="Click" />
                             <asp:AsyncPostBackTrigger ControlID="btnAgregaCualidad" EventName="Click" />
                             <asp:AsyncPostBackTrigger ControlID="btnInvisible3" EventName="Click" />
                             <asp:AsyncPostBackTrigger ControlID="btnModificarBecarioPerfil" EventName="Click" />
                         </Triggers>
                           
                         <ContentTemplate>
                             
                           <div class="wrap_row_becario">

                              <!--Grid Lenguajes Programación-->
                             <div class="wrap_perfil">

                               <span class="lbl1" style="margin-left: 15px;font-family:Segoe UI,Verdana,Helvetica,Sans-Serif">a) Lenguajes de programación:</span>

                               <div style= "margin-top: 20px;margin-left: 20px;">

                                <div class="botonMas">
                                   <asp:ImageButton ID="btnAgregaLenguaje" runat="server"
                                    ImageUrl="~/Images/signoMas.png" OnClick="nuevoAtributoDePerfil_click" CausesValidation="False" />
                                </div>

                                <asp:GridView ID="gridLenguajesProg" runat="server" AutoGenerateColumns="False" ShowFooter="True"
                                  CellPadding="7" onrowcommand="eliminaDatosPerfil_RowCommand"
                                  ForeColor="#333333" GridLines="Vertical" >

                                  <AlternatingRowStyle BackColor="White" />
                                  <Columns>

                                     <asp:ButtonField CommandName="btnEliminaLenguaje_Click" CausesValidation="false" ImageUrl="Images/signoMenos.png" ButtonType="Image" />

                                     <asp:TemplateField HeaderText="Lenguaje de Programación" SortExpression="Lenguaje">
                                       <ItemTemplate>
                                         <asp:Label ID="lblLeng" runat="server" Text='<%# Bind("LenguajeProgramacion") %>'></asp:Label>
                                       </ItemTemplate>
                                       <FooterTemplate>
                                         <asp:TextBox ID="txtNuevoLenguaje" runat="server"></asp:TextBox>
                                       </FooterTemplate>

                                     </asp:TemplateField>
                                   </Columns>
                                   <EditRowStyle BackColor="#7C6F57" />
                                   <FooterStyle BackColor="#1C5E55" Font-Bold="True" ForeColor="White" />
                                   <HeaderStyle BackColor="#1C5E55" Font-Bold="True" ForeColor="White" />
                                   <PagerStyle BackColor="#666666" ForeColor="White" HorizontalAlign="Center" />
                                   <RowStyle BackColor="#E3EAEB" />
                                   <SelectedRowStyle BackColor="#C5BBAF" Font-Bold="True" ForeColor="#333333" />
                                   <SortedAscendingCellStyle BackColor="#F8FAFA" />
                                   <SortedAscendingHeaderStyle BackColor="#246B61" />
                                   <SortedDescendingCellStyle BackColor="#D4DFE1" />
                                   <SortedDescendingHeaderStyle BackColor="#15524A" />
                                </asp:GridView>
                                                                                              
                               </div>                                       
                            </div>
                                  
                             <!--Grid Idiomas-->
                            <div class="wrap_perfil">

                               <span class="lbl1" style="margin-left: 15px;font-family:Segoe UI,Verdana,Helvetica,Sans-Serif">b) Idiomas:</span>

                                 <div style= "margin-top: 20px;margin-left: 20px;">
                        
                                    <div class="botonMas">
                                       <asp:ImageButton ID="btnAgregaIdioma" runat="server" ImageUrl="~/Images/signoMas.png" 
                                       OnClick="nuevoAtributoDePerfil_click" CausesValidation="False" />
                                    </div>

                                    <asp:GridView ID="gridIdiomas" runat="server" AutoGenerateColumns="False" ShowFooter="True"
                                      CellPadding="7"  onrowcommand="eliminaDatosPerfil_RowCommand"
                                      ForeColor="#333333" GridLines="Vertical">

                                      <AlternatingRowStyle BackColor="White" />

                                      <Columns>

                                         <asp:ButtonField CommandName="btnEliminaIdioma_Click" CausesValidation="false" ImageUrl="Images/signoMenos.png" ButtonType="Image" />

                                         <asp:TemplateField HeaderText="Idioma" SortExpression="Lenguaje">
                                           <ItemTemplate>
                                             <asp:Label ID="lblIdioma" runat="server" Text='<%# Bind("Idioma") %>'></asp:Label>
                                           </ItemTemplate>
                                           <FooterTemplate>
                                             <asp:TextBox ID="txtNuevoIdioma" runat="server"></asp:TextBox>
                                           </FooterTemplate>
                               
                                         </asp:TemplateField>

                                      </Columns>
                                      <EditRowStyle BackColor="#7C6F57" />
                                       <FooterStyle BackColor="#1C5E55" Font-Bold="True" ForeColor="White" />
                                       <HeaderStyle BackColor="#1C5E55" Font-Bold="True" ForeColor="White" />
                                       <PagerStyle BackColor="#666666" ForeColor="White" HorizontalAlign="Center" />
                                       <RowStyle BackColor="#E3EAEB" />
                                       <SelectedRowStyle BackColor="#C5BBAF" Font-Bold="True" ForeColor="#333333" />
                                       <SortedAscendingCellStyle BackColor="#F8FAFA" />
                                       <SortedAscendingHeaderStyle BackColor="#246B61" />
                                       <SortedDescendingCellStyle BackColor="#D4DFE1" />
                                       <SortedDescendingHeaderStyle BackColor="#15524A" />
                                    </asp:GridView>
                    
                                 </div>

                            </div>                                                         
                                  
                           </div>


                           <div class="wrap_row_becario">
                           
                            <!--Grid Áreas de Interés-->                        
                             <div class="wrap_perfil">

                               <span class="lbl1" style="margin-left:15px; font-family:Segoe UI,Verdana,Helvetica,Sans-Serif">c) Áreas de Interés:</span>

                               <div style= "margin-top: 20px;margin-left: 20px;">

                                 <div class="botonMas">
                                   <asp:ImageButton ID="btnAgregaAreaInteres" runat="server" ImageUrl="~/Images/signoMas.png"
                                   OnClick="nuevoAtributoDePerfil_click" CausesValidation="False" />
                                 </div>

                                <asp:GridView ID="gridAreasInteres" runat="server" AutoGenerateColumns="False" ShowFooter="True"
                                 CellPadding="7" onrowcommand="eliminaDatosPerfil_RowCommand"
                                 ForeColor="#333333" GridLines="Vertical">

                                  <AlternatingRowStyle BackColor="White" />
                                  <Columns>

                                     <asp:ButtonField CommandName="btnEliminaInteres_Click" CausesValidation="false" ImageUrl="Images/signoMenos.png" ButtonType="Image" />

                                     <asp:TemplateField HeaderText="Áreas de Interés" SortExpression="Lenguaje">
                                       <ItemTemplate>
                                         <asp:Label ID="lbArea" runat="server" Text='<%# Bind("AreaInteres") %>'></asp:Label>
                                       </ItemTemplate>
                                       <FooterTemplate>
                                         <asp:TextBox ID="txtNuevaAreaInteres" runat="server"></asp:TextBox>
                                       </FooterTemplate>

                                     </asp:TemplateField>
                                   </Columns>
                                   <EditRowStyle BackColor="#7C6F57" />
                                   <FooterStyle BackColor="#1C5E55" Font-Bold="True" ForeColor="White" />
                                   <HeaderStyle BackColor="#1C5E55" Font-Bold="True" ForeColor="White" />
                                   <PagerStyle BackColor="#666666" ForeColor="White" HorizontalAlign="Center" />
                                   <RowStyle BackColor="#E3EAEB" />
                                   <SelectedRowStyle BackColor="#C5BBAF" Font-Bold="True" ForeColor="#333333" />
                                   <SortedAscendingCellStyle BackColor="#F8FAFA" />
                                   <SortedAscendingHeaderStyle BackColor="#246B61" />
                                   <SortedDescendingCellStyle BackColor="#D4DFE1" />
                                   <SortedDescendingHeaderStyle BackColor="#15524A" />
                                 </asp:GridView>

                               </div>

                            </div>


                            <!--Grid Cualidades-->
                             <div class="wrap_perfil">

                               <span class="lbl1" style="margin-left: 15px;font-family:Segoe UI,Verdana,Helvetica,Sans-Serif">d) Aptitudes:</span>

                               <div style= "margin-top: 20px;margin-left: 20px;">

                                 <div class="botonMas">
                                   <asp:ImageButton ID="btnAgregaCualidad" runat="server" ImageUrl="~/Images/signoMas.png"
                                   OnClick="nuevoAtributoDePerfil_click" CausesValidation="False" />
                                 </div>

                                <asp:GridView ID="gridCualidades" runat="server" AutoGenerateColumns="False" ShowFooter="True"
                                 CellPadding="7" onrowcommand="eliminaDatosPerfil_RowCommand"
                                 ForeColor="#333333" GridLines="Vertical">

                                  <AlternatingRowStyle BackColor="White" />
                                  <Columns>

                                     <asp:ButtonField CommandName="btnEliminaCualidad_Click" CausesValidation="false" ImageUrl="Images/signoMenos.png" ButtonType="Image" />

                                     <asp:TemplateField HeaderText="Aptitudes" SortExpression="Lenguaje">
                                       <ItemTemplate>
                                         <asp:Label ID="lblCualidad" runat="server" Text='<%# Bind("Cualidad") %>'></asp:Label>
                                       </ItemTemplate>
                                       <FooterTemplate>
                                         <asp:TextBox ID="txtNuevaCualidad" runat="server"></asp:TextBox>
                                       </FooterTemplate>

                                     </asp:TemplateField>
                                   </Columns>
                                   <EditRowStyle BackColor="#7C6F57" />
                                   <FooterStyle BackColor="#1C5E55" Font-Bold="True" ForeColor="White" />
                                   <HeaderStyle BackColor="#1C5E55" Font-Bold="True" ForeColor="White" />
                                   <PagerStyle BackColor="#666666" ForeColor="White" HorizontalAlign="Center" />
                                   <RowStyle BackColor="#E3EAEB" />
                                   <SelectedRowStyle BackColor="#C5BBAF" Font-Bold="True" ForeColor="#333333" />
                                   <SortedAscendingCellStyle BackColor="#F8FAFA" />
                                   <SortedAscendingHeaderStyle BackColor="#246B61" />
                                   <SortedDescendingCellStyle BackColor="#D4DFE1" />
                                   <SortedDescendingHeaderStyle BackColor="#15524A" />
                                 </asp:GridView>
                                   
                               </div>

                            </div>                            

                           </div>

                             <div class="wrap_row_becario">
                                <div style="color:red;font-size:12px;font-size:13px;font-weight:bold;margin-top:10px;margin-left:20px">
                                  <asp:Label ID="lblAvisoPerfil" runat="server"
                                  Text="*Debe proveer un nombre, apellido y una cédula antes de completar el perfil"
                                  Visible="false"></asp:Label>
                                </div>  
                           </div>  

                        </ContentTemplate>
                       </asp:UpdatePanel>
                    </div>

                 </div>
            </ContentTemplate>
          </asp:UpdatePanel>
        </div>

      </div>


       <div id="PopUpEliminar">
        ¿Seguro que desea eliminar?
      </div>

   </asp:View>


   <!-- ****VISTA PARCIAL **** -->
   <asp:View ID="VistaParcial" runat="server">

      <div id="tabsP" style="width: 85%; margin-left:5% ; margin-top:2%" >

        <asp:UpdatePanel runat="server" ID="UpdatePanelParcial">

            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="btnModificarBecarioDatosP" EventName="Click" />
                 <asp:AsyncPostBackTrigger ControlID="btnCancelarP" EventName="Click" />
                 <asp:AsyncPostBackTrigger ControlID="btnAceptarP" EventName="Click" />
             </Triggers>

         <ContentTemplate>

           <ul>
		     <li><a href="#tabsP-1">Datos Personales</a></li>
		      <li><a href="#tabsP-2">Perfil</a></li>
	       </ul>

            <!--Datos Personales-->
            <div id="tabsP-1" style= "width:96% ; min-height:445px" >

                    <div style="width: 12%; float: right; margin-bottom:5% ; margin-top:2% ">
                    <asp:Button ID="btnModificarBecarioDatosP" runat="server" Text="Modificar" 
                            CssClass="boton ui-widget ui-state-default ui-corner-all ui-button-text-only" 
                            onclick="btnModificarBecarioParcial_Click" CausesValidation="false" />
                    </div>


                    <div style="width: 96%; padding: 0 2%; float: left; background: #D8D8BF; border-radius: 5px;">

        
              <div style="font-family:Segoe UI,Verdana,Helvetica,Sans-Serif;font-size:15px;margin-bottom:20px">
                <p>Para editar su información personal debe presionar el botón modificar ubicado arriba.</p>
              </div>                   

                             <div class="wrap_row_becario">

                                <div class="wrap_becario">

                                   <div>
                                    <span class="lblBecario">* Nombre</span>
                                    <asp:TextBox ID="txtNombreP" runat="server" CssClass="txtBecario"></asp:TextBox>
                                   </div>

                                  <div style="width: 88%; font-size:14px">

                                    <asp:RequiredFieldValidator ControlToValidate="txtNombreP" CssClass="error" ID="obligatorio_nombreP" runat="server" ErrorMessage="* Campo obligatorio"  ForeColor="#FF3300" Display="Dynamic" Font-Bold="true" ></asp:RequiredFieldValidator>                                   
                                    
                                    <asp:RegularExpressionValidator ID="formato_nombreP" runat="server" 
                                     ErrorMessage ="Se ha escrito caracteres inválidos" Display="Dynamic"
                                     ValidationExpression = "[a-zA-Z\u00c1\u00c9\u00cd\u00d3 
                                     \u00da\u00e1\u00e9\u00ed\u00f3\u00fa\u00d1\u00f1\u00c0\u00c2\u00c3\u00c4\u00c5\u00c8
                                     \u00ca\u00cb\u00cc\u00ce\u00cf\u00d2\u00d4\u00d5\u00d6\u00db\u00dc\u00e0\u00e2
                                     \u00e3\u00e4\u00e7\u00e8\u00eb\u00ec\u00ee\u00ef\u00f2\u00f4\u00f5\u00f6\u00f9\u00fb\u00fc\u00fd\u00ff]+"                                
                                    ControlToValidate="txtNombreP" ForeColor="#FF3300">
                                    </asp:RegularExpressionValidator>
                                 </div>

                                </div>

                                <div class="wrap_becario">

                                    <div>
                                       <span class="lblBecario">* Primer Apellido</span>
                                       <asp:TextBox ID="txtApellido1P" runat="server" CssClass="txtBecario"></asp:TextBox>
                                    </div>
                                     
                                    <div style="width: 88%; font-size:14px">                       
                                       <asp:RequiredFieldValidator ControlToValidate="txtApellido1P" CssClass="error" ID="obligatorio_apellido1P" runat="server" ErrorMessage="* Campo obligatorio"  ForeColor="#FF3300" Display="Dynamic" Font-Bold="true" ></asp:RequiredFieldValidator>
                                    
                                       <asp:RegularExpressionValidator ID="formato_apellido1P" runat="server" 
                                       ErrorMessage ="Se ha escrito caracteres inválidos"  Display="Dynamic"
                                       ValidationExpression = "[a-zA-Z\u00c1\u00c9\u00cd\u00d3 
                                     \u00da\u00e1\u00e9\u00ed\u00f3\u00fa\u00d1\u00f1\u00c0\u00c2\u00c3\u00c4\u00c5\u00c8
                                     \u00ca\u00cb\u00cc\u00ce\u00cf\u00d2\u00d4\u00d5\u00d6\u00db\u00dc\u00e0\u00e2
                                     \u00e3\u00e4\u00e7\u00e8\u00eb\u00ec\u00ee\u00ef\u00f2\u00f4\u00f5\u00f6\u00f9\u00fb\u00fc\u00fd\u00ff]+"
                                     ControlToValidate="txtApellido1P" ForeColor="#FF3300">
                                       </asp:RegularExpressionValidator>
                                    </div>

                                </div>
                    
                                <div class="wrap_becario">
                                    
                                    <div>
                                      <span class="lblBecario">Segundo Apellido</span>
                                      <asp:TextBox ID="txtApellido2P" runat="server" CssClass="txtBecario"></asp:TextBox>
                                    </div>

                                    <div style="width: 88%; font-size:14px">                                                                                    
                                       <asp:RegularExpressionValidator ID="formato_apellido2P" runat="server" 
                                       ErrorMessage ="Se ha escrito caracteres inválidos"  Display="Dynamic"
                                       ValidationExpression = "[a-zA-Z\u00c1\u00c9\u00cd\u00d3 
                                         \u00da\u00e1\u00e9\u00ed\u00f3\u00fa\u00d1\u00f1\u00c0\u00c2\u00c3\u00c4\u00c5\u00c8
                                         \u00ca\u00cb\u00cc\u00ce\u00cf\u00d2\u00d4\u00d5\u00d6\u00db\u00dc\u00e0\u00e2
                                         \u00e3\u00e4\u00e7\u00e8\u00eb\u00ec\u00ee\u00ef\u00f2\u00f4\u00f5\u00f6\u00f9\u00fb\u00fc\u00fd\u00ff]+"
                                       ControlToValidate="txtApellido2P" ForeColor="#FF3300">
                                       </asp:RegularExpressionValidator>
                                    </div>

                                </div>

                            </div>

                            <div class="wrap_row_becario">

                                <div class="wrap_becario">

                                    <div>
                                       <span class="lblBecario">* Carné</span>
                                       <asp:TextBox ID="txtCarneP" runat="server" CssClass="txtBecario"></asp:TextBox>
                                    </div>

                                    <div style="width: 88%; font-size:14px">                       
                                       <asp:RequiredFieldValidator ControlToValidate="txtCarneP" CssClass="error" ID="obligatorio_carneP" runat="server" ErrorMessage="* Campo obligatorio"  ForeColor="#FF3300" Display="Dynamic" Font-Bold="true" ></asp:RequiredFieldValidator>
                                    
                                       <asp:RegularExpressionValidator ID="formato_carneP" runat="server" 
                                       ErrorMessage ="Formato de carné inválido"  Display="Dynamic"
                                       ValidationExpression = "[a-z|A-Z|0-9][0-9]{5}$" ControlToValidate="txtCarneP" ForeColor="#FF3300">
                                       </asp:RegularExpressionValidator>
                                    </div>                                
                                
                                </div>

                                <div class="wrap_becario">

                                    <div>
                                      <span class="lblBecario">* Cédula</span>
                                      <asp:TextBox ID="txtCedulaP" runat="server" CssClass="txtBecario"></asp:TextBox>
                                    </div>

                                    <div style="width: 88%; font-size:14px">                       
                                       <asp:RequiredFieldValidator ControlToValidate="txtCedulaP" CssClass="error" ID="obligatorio_cedP" runat="server" ErrorMessage="* Campo obligatorio"  ForeColor="#FF3300" Display="Dynamic" Font-Bold="true" ></asp:RequiredFieldValidator>
                                    
                                       <asp:RegularExpressionValidator ID="formato_cedP" runat="server" 
                                       ErrorMessage ="Se ha escrito caracteres inválidos"  Display="Dynamic"
                                       ValidationExpression = "^([0-9]|- ){0,15}$" ControlToValidate="txtCedulaP" ForeColor="#FF3300">
                                       </asp:RegularExpressionValidator>
                                    </div>    

                                </div>
                    
                                 <div class="wrap_becario">

                                    <div>
                                      <span class="lblBecario">* Correo Electrónico</span>
                                       <asp:TextBox ID="txtCorreoP" runat="server" CssClass="txtBecario"></asp:TextBox>
                                    </div>
                                    
                                    <div style="width: 88%; font-size:14px">                       
                                       <asp:RequiredFieldValidator ControlToValidate="txtCorreoP" CssClass="error" ID="obligatorio_correoP" runat="server" ErrorMessage="* Campo obligatorio"  ForeColor="#FF3300" Display="Dynamic" Font-Bold="true" ></asp:RequiredFieldValidator>
                                    
                                       <asp:RegularExpressionValidator ID="formato_correoP" runat="server" 
                                       ErrorMessage ="Formato de correo inválido"  Display="Dynamic"
                                       ValidationExpression = "[a-zA-Z0-9-_\.]+@[a-zA-Z]+\.[a-z]+" ControlToValidate="txtCorreoP" 
                                           ForeColor="#FF3300">
                                       </asp:RegularExpressionValidator>
                                    </div>   
                                 
                                 </div>

                            </div>

                            <div class="wrap_row_becario">

                                <div class="wrap_becario">

                                    <div>
                                      <span class="lblBecario">Teléfono Fijo</span>
                                      <asp:TextBox ID="txtTelFijoP" runat="server" CssClass="txtBecario"></asp:TextBox>
                                    </div>

                                    <div style="width: 88%; font-size:14px">       
                                     <asp:RegularExpressionValidator ID="formato_telFijoP" runat="server" 
                                      ErrorMessage ="Formato de teléfono inválido"  Display="Dynamic"
                                      ValidationExpression = "^([0-9]|- ){8,11}$" ControlToValidate="txtTelFijoP" ForeColor="#FF3300">
                                     </asp:RegularExpressionValidator>
                                   </div>

                                </div>
                    
                                <div class="wrap_becario">

                                    <div>       
                                      <span class="lblBecario">Teléfono Móvil</span>
                                      <asp:TextBox ID="txtCelularP" runat="server" CssClass="txtBecario"></asp:TextBox>
                                   </div>

                                    <div style="width: 88%; font-size:14px">
                                      <asp:RegularExpressionValidator ID="formato_celP" runat="server" 
                                       ErrorMessage ="Formato de teléfono inválido"  Display="Dynamic"
                                      ValidationExpression  = "^([0-9]|- ){8,11}$" ControlToValidate="txtCelularP" ForeColor="#FF3300">
                                      </asp:RegularExpressionValidator>
                                   </div>
                                
                                </div>

                                <div class="wrap_becario">

                                    <div>
                                      <span class="lblBecario">Otro Teléfono</span>
                                      <asp:TextBox ID="txtOtroTelP" runat="server" CssClass="txtBecario"></asp:TextBox>
                                    </div>

                                    <div style="width: 88%; font-size:14px">       
                                      <asp:RegularExpressionValidator ID="formato_otroTelP" runat="server" 
                                       ErrorMessage ="Formato de teléfono inválido"  Display="Dynamic"
                                      ValidationExpression  = "^([0-9]|- ){8,11}$" ControlToValidate="txtOtroTelP" ForeColor="#FF3300">
                                      </asp:RegularExpressionValidator>
                                    </div>

                                </div>

                            </div>
                    </div>

                   <div style="width: 96%; padding: 2%; float: left;" >
                       
                       <div style="width: 12%; float: right; margin-bottom:5% ; margin-top:4% ">
                         <asp:Button ID="btnCancelarP" runat="server" Text="Cancelar" 
                         CssClass="boton ui-widget ui-state-default ui-corner-all ui-button-text-only" 
                         onClick="btnCancelarP_Click"  CausesValidation="false"/>
                       </div>

                       <div style="width: 12%; float: right; margin-bottom:5% ; margin-top:4%;margin-right:10px">    
                        <asp:Button ID="btnAceptarP" runat="server" Text="Aceptar"  onClick="btnAceptarP_Click"
                         CssClass="boton ui-widget ui-state-default ui-corner-all ui-button-text-only" />
                       </div>
                   </div> 
           </div>
     
           <!--PERFIL DEL BECARIO : vista parcial-->
           <div id="tabsP-2" style= "width:100% ; min-height:580px" >

              <div style="width: 15%; float: right; margin-bottom:2% ; margin-top:2%; margin-right:8%">
                 <asp:Button ID="btnModificarBecarioPerfilP" runat="server" Text="Modificar" 
                  CssClass="boton ui-widget ui-state-default ui-corner-all ui-button-text-only" 
                    onclick="btnModificarBecarioParcial_Click" />
               </div>

           <div style="width: 86%; padding: 0 2%; float: left; background: #D8D8BF; border-radius: 5px; margin-left:3%">
   
                          
              <div style="font-family:Segoe UI,Verdana,Helvetica,Sans-Serif;font-size:14px">
                <p>A continuación se le presentan unas tablas que resumen algunos aspectos
                    de importancia para el proceso de becas. Se le solicita completar los datos de la forma más precisa posible.
                    Para editar esta información debe presionar el botón modificar ubicado arriba.
                  </p>
              </div>    
                     
             <asp:UpdatePanel runat="server" ID="UpdatePanelPerfilParcial">

                <Triggers>
                   <asp:AsyncPostBackTrigger ControlID="btnAgregaLenguajeParcial" EventName="Click" />
                   <asp:AsyncPostBackTrigger ControlID="btnAgregaIdiomaParcial" EventName="Click" />
                   <asp:AsyncPostBackTrigger ControlID="btnAgregaAreaInteresParcial" EventName="Click" />
                   <asp:AsyncPostBackTrigger ControlID="btnAgregaCualidadParcial" EventName="Click" />
                </Triggers>
                <ContentTemplate>
                
                  <div class="wrap_row_becario">

                     <!--Grid de lenguajes de Programación-->
                     <div class="wrap_perfil">

                        <span class="lblGrid1" style="margin-left: 15px">a) Lenguajes de programación:</span>

                        <div style= "margin-top: 20px;margin-left: 20px;">

                           <div class="botonMasParcial">
                            <asp:ImageButton ID="btnAgregaLenguajeParcial" runat="server"
                            ImageUrl="~/Images/signoMas.png" OnClick="nuevoAtributoDePerfilParcial_click" CausesValidation="False"  />
                           </div>

                            <asp:GridView ID="gridLenguajesProgP" runat="server" AutoGenerateColumns="False" ShowFooter="True"
                              CellPadding ="7" onrowcommand="eliminaDatosPerfilParcial_RowCommand"
                              ForeColor="#333333" GridLines="Vertical">

                             <AlternatingRowStyle BackColor="White" />
                             <Columns>

                              <asp:ButtonField CommandName="btnEliminaLenguajeP_Click" CausesValidation="false" ImageUrl="Images/signoMenos.png" ButtonType="Image" />

                                 <asp:TemplateField HeaderText="Lenguaje de Programación" SortExpression="Lenguaje">
                                   <ItemTemplate>
                                     <asp:Label ID="lblLengP" runat="server" Text='<%# Bind("LenguajeProgramacion") %>'></asp:Label>
                                   </ItemTemplate>
                                   <FooterTemplate>
                                     <asp:TextBox ID="txtNuevoLenguajeParcial" runat="server"></asp:TextBox>
                                   </FooterTemplate>

                                 </asp:TemplateField>
                                 </Columns>
                                 <EditRowStyle BackColor="#7C6F57" />
                                 <FooterStyle BackColor="#1C5E55" Font-Bold="True" ForeColor="White" />
                                 <HeaderStyle BackColor="#1C5E55" Font-Bold="True" ForeColor="White" />
                                 <PagerStyle BackColor="#666666" ForeColor="White" HorizontalAlign="Center" />
                                 <RowStyle BackColor="#E3EAEB" />
                                 <SelectedRowStyle BackColor="#C5BBAF" Font-Bold="True" ForeColor="#333333" />
                                 <SortedAscendingCellStyle BackColor="#F8FAFA" />
                                 <SortedAscendingHeaderStyle BackColor="#246B61" />
                                 <SortedDescendingCellStyle BackColor="#D4DFE1" />
                                 <SortedDescendingHeaderStyle BackColor="#15524A" />
                               </asp:GridView>
                       </div>
                     </div>
 
                     <!--Grid Idiomas-->
                     <div class="wrap_perfil">

                       <span class="lblGrid2" style="margin-left: 15px">b) Idiomas:</span>

                       <div style= "margin-top: 20px;margin-left: 20px;">
                        
                        <div class="botonMasParcial">                       
                          <asp:ImageButton ID="btnAgregaIdiomaParcial" runat="server" ImageUrl="~/Images/signoMas.png" 
                           OnClick="nuevoAtributoDePerfilParcial_click" CausesValidation="False" />
                        </div>

                         <asp:GridView ID="gridIdiomasP" runat="server" AutoGenerateColumns="False" ShowFooter="True"
                           CellPadding="7"  onrowcommand="eliminaDatosPerfilParcial_RowCommand"
                           ForeColor="#333333" GridLines="Vertical">

                          <AlternatingRowStyle BackColor="White" />
                           <Columns>

                              <asp:ButtonField CommandName="btnEliminaIdiomaP_Click" CausesValidation="false" ImageUrl="Images/signoMenos.png" ButtonType="Image" />

                                <asp:TemplateField HeaderText="Idioma" SortExpression="Idioma">
                                  <ItemTemplate>
                                     <asp:Label ID="lblIdiomaParcial" runat="server" Text='<%# Bind("Idioma") %>'></asp:Label>
                                  </ItemTemplate>
                                  <FooterTemplate>
                                     <asp:TextBox ID="txtNuevoIdiomaParcial" runat="server"></asp:TextBox>
                                  </FooterTemplate>
                                </asp:TemplateField>

                             </Columns>
                             <EditRowStyle BackColor="#7C6F57" />
                             <FooterStyle BackColor="#1C5E55" Font-Bold="True" ForeColor="White" />
                              <HeaderStyle BackColor="#1C5E55" Font-Bold="True" ForeColor="White" />
                              <PagerStyle BackColor="#666666" ForeColor="White" HorizontalAlign="Center" />
                              <RowStyle BackColor="#E3EAEB" />
                              <SelectedRowStyle BackColor="#C5BBAF" Font-Bold="True" ForeColor="#333333" />
                              <SortedAscendingCellStyle BackColor="#F8FAFA" />
                              <SortedAscendingHeaderStyle BackColor="#246B61" />
                              <SortedDescendingCellStyle BackColor="#D4DFE1" />
                              <SortedDescendingHeaderStyle BackColor="#15524A" />
                          </asp:GridView>
                    
                       </div>

                     </div>   
                     
                 </div>
                 
                 <div class="wrap_row_becario">

                    <!--Grid Áreas de Interés--> 
                    <div class="wrap_perfil">

                     <span class="lblGrid1" style="margin-left: 15px">c) Áreas de Interés:</span>

                       <div style= "margin-top: 20px;margin-left: 20px;">
                      
                          <div class="botonMasParcial">
                            <asp:ImageButton ID="btnAgregaAreaInteresParcial" runat="server"
                            ImageUrl="~/Images/signoMas.png" OnClick="nuevoAtributoDePerfilParcial_click" CausesValidation="False"  />
                          </div>

                          <asp:GridView ID="gridAreasInteresP" runat="server" AutoGenerateColumns="False" ShowFooter="True"
                            CellPadding="7" onrowcommand="eliminaDatosPerfilParcial_RowCommand"
                            ForeColor="#333333" GridLines="Vertical">

                            <AlternatingRowStyle BackColor="White" />
                            <Columns>

                              <asp:ButtonField CommandName="btnEliminaInteresP_Click" CausesValidation="false" ImageUrl="Images/signoMenos.png" ButtonType="Image" />

                               <asp:TemplateField HeaderText="Área de Interés" SortExpression="Área">
                                 <ItemTemplate>
                                   <asp:Label ID="lbArea" runat="server" Text='<%# Bind("AreaInteres") %>'></asp:Label>
                                 </ItemTemplate>
                                 <FooterTemplate>
                                   <asp:TextBox ID="txtNuevaAreaInteresParcial" runat="server"></asp:TextBox>
                                 </FooterTemplate>
                               </asp:TemplateField>
                             </Columns>
                             <EditRowStyle BackColor="#7C6F57" />
                             <FooterStyle BackColor="#1C5E55" Font-Bold="True" ForeColor="White" />
                             <HeaderStyle BackColor="#1C5E55" Font-Bold="True" ForeColor="White" />
                             <PagerStyle BackColor="#666666" ForeColor="White" HorizontalAlign="Center" />
                             <RowStyle BackColor="#E3EAEB" />
                             <SelectedRowStyle BackColor="#C5BBAF" Font-Bold="True" ForeColor="#333333" />
                             <SortedAscendingCellStyle BackColor="#F8FAFA" />
                             <SortedAscendingHeaderStyle BackColor="#246B61" />
                             <SortedDescendingCellStyle BackColor="#D4DFE1" />
                             <SortedDescendingHeaderStyle BackColor="#15524A" />
                          </asp:GridView>
                        
                       </div>

                    </div>

                    <!--Grid Cualidades--> 
                    <div class="wrap_perfil">

                      <span class="lblGrid1" style="margin-left: 15px">d) Aptitudes:</span>

                      <div style= "margin-top: 20px;margin-left: 20px;">

                         <div class="botonMasParcial">
                           <asp:ImageButton ID="btnAgregaCualidadParcial" runat="server"
                            ImageUrl ="~/Images/signoMas.png" OnClick="nuevoAtributoDePerfilParcial_click" CausesValidation="False"  />
                         </div>

                         <asp:GridView ID="gridCualidadesP" runat="server" AutoGenerateColumns="False" ShowFooter="True"
                          CellPadding="7" onrowcommand="eliminaDatosPerfilParcial_RowCommand"
                          ForeColor="#333333" GridLines="Vertical">

                         <AlternatingRowStyle BackColor="White" />
                         <Columns>

                         <asp:ButtonField CommandName="btnEliminaCualidadP_Click" CausesValidation="false" ImageUrl="Images/signoMenos.png" ButtonType="Image" />

                          <asp:TemplateField HeaderText="Aptitudes" SortExpression="Lenguaje">
                            <ItemTemplate>
                              <asp:Label ID="lbCualidad" runat="server" Text='<%# Bind("Cualidad") %>'></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                              <asp:TextBox ID="txtNuevaCualidadParcial" runat="server"></asp:TextBox>
                            </FooterTemplate>
                          </asp:TemplateField>
                          </Columns>
                          <EditRowStyle BackColor="#7C6F57" />
                          <FooterStyle BackColor="#1C5E55" Font-Bold="True" ForeColor="White" />
                          <HeaderStyle BackColor="#1C5E55" Font-Bold="True" ForeColor="White" />
                          <PagerStyle BackColor="#666666" ForeColor="White" HorizontalAlign="Center" />
                          <RowStyle BackColor="#E3EAEB" />
                          <SelectedRowStyle BackColor="#C5BBAF" Font-Bold="True" ForeColor="#333333" />
                          <SortedAscendingCellStyle BackColor="#F8FAFA" />
                          <SortedAscendingHeaderStyle BackColor="#246B61" />
                          <SortedDescendingCellStyle BackColor="#D4DFE1" />
                          <SortedDescendingHeaderStyle BackColor="#15524A" />
                          </asp:GridView>
                      </div>

                    </div>

                </div>

             </ContentTemplate>
           </asp:UpdatePanel>
          </div>

            <div style="width: 90%; padding: 2%; float: left;" >
                       
               <div style="width: 12%; float: right; margin-bottom:5% ; margin-top:4% ">
                 <asp:Button ID="btnCancelarPerfilParcial" runat="server" Text="Cancelar" 
                  CssClass="boton ui-widget ui-state-default ui-corner-all ui-button-text-only" 
                  onClick="btnCancelarP_Click"  CausesValidation="false"/>
               </div>

               <div style="width: 12%; float: right; margin-bottom:5% ; margin-top:4%;margin-right:10px">    
                 <asp:Button ID="btnAceptarPerfilParcial" runat="server" Text="Aceptar"  onClick="btnAceptarP_Click"
                 CssClass ="boton ui-widget ui-state-default ui-corner-all ui-button-text-only" />
               </div>
            </div>          

          </div>

        </ContentTemplate>
        </asp:UpdatePanel>

      </div>



   </asp:View>
   <asp:View ID="BecarioSinPermiso" runat="server">
            <h2 style="color: Red; text-align:center;">Lo sentimos. Usted no tiene acceso a esta sección.</h2>      
        </asp:View>

  </asp:MultiView>


</asp:Content>

