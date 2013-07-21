<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Reportes.aspx.cs" Inherits="Reportes" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
	<script src="Scripts/Reportes.js" type="text/javascript"></script>
	<link href="Styles/Reportes.css" rel="stylesheet" type="text/css" />
	<script type="text/javascript">
		$(document).ready(function () {
			$("a.level1:contains('Reportes')").addClass("item_active");
		});
	</script>
	<script type="text/javascript">
		function IniciarSolicitud() {
			var iframe = document.createElement("iframe");

			//Aquí se le envía la carpeta donde está corriendo el servicio actualmente. Debe tener permisos de escritura
			var ruta = "C:/Users/Tino/GitHub/SistemaDeControlDeBecarios11/Sistema de Control de Becarios 11/PDFs/";
			var periodo = $("[id*='DropDownListCriterio2'] :selected").val();
			var ciclo = 0;
			switch (periodo) {
				case "0":
					ciclo = 3;
					break;
				case "1":
					ciclo = 2;
					break;
				case "2":
					ciclo = 1;
					break;
			}

			var destinatario = $("[id*='txtDestinatario']").val();
			var remitente = $("[id*='txtRemitente']").val();
			var iniciales = $("[id*='txtIniciales']").val();
			var cantHoras = $("[id*='ddlCantHoras'] :selected").text();
			var año = $("[id*='lblAño']").text();

			iframe.src = "DescargarPDF.aspx?ruta=" + ruta + "&destinatario=" + destinatario + "&remitente=" + remitente + "&iniciales=" + iniciales + "&cantHoras=" + cantHoras + "&ciclo=" + ciclo + "&periodo=" + periodo + "&año=" + año;
			iframe.style.display = "none";
			document.body.appendChild(iframe);
		}
	</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
	<asp:ScriptManager ID="ScriptManager" runat="server">
	</asp:ScriptManager>
	<asp:MultiView ID="MultiViewReportes" runat="server">

		<!-- Consulta de Encargados relacionados con ese Becario -->
		<asp:View ID="VistaReportes" runat="server">

			<asp:UpdatePanel ID="UpdateInfo" runat="server">
				<Triggers>
					<asp:AsyncPostBackTrigger ControlID="btnPopUpGenerarPDF" EventName="Click" />
				</Triggers>

				<ContentTemplate>
					<!-- Botones Invisibles -->
					<asp:Button ID="btnInvisible1"
						CssClass="btnInvisible1 invisible" runat="server" Text="" />
					<asp:Button ID="btnInvisGenerarPDF" CssClass="btnInvisGenerarPDF invisible" runat="server"
						OnClientClick="IniciarSolicitud()" CausesValidation="true" ValidationGroup="vldPopUpPDF" />

					<!-- Cuerpo -->
					<div style="width: 100%; float: left;">

						<!-- Título -->
						<span style="width: 100%; font-weight: bold; font-size: 24px; float: left; margin: 20px 0 5px 0; text-align: center;">Módulo de Reportes</span>
						<span style="width: 100%; font-weight: normal; font-style: italic; font-size: 16px; float: left; margin: 5px 0 20px 0; text-align: center; border-bottom: 2px solid #414141; padding-bottom: 5px;">Le permite ver diferentes reportes de la actividad entre encargados y becarios.</span>

						<!-- Menu reportes -->
						<div id="Menu_Container" style="width: 15%; float: left;">
							<asp:Menu ID="MenuListaReportes" CssClass="menu_reportes" runat="server"
								OnMenuItemClick="MenuListaReportes_MenuItemClick" SkipLinkText="">
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

						<!-- Ventana reportes -->
						<asp:UpdatePanel ID="UpdatePanelInformacionReportes" runat="server">
							<Triggers>
								<asp:AsyncPostBackTrigger ControlID="MenuListaReportes" EventName="menuitemclick" />
							</Triggers>
							<ContentTemplate>
								<div style="margin-left: 2%; width: 81%; float: left; border: 2px solid #414141; border-radius: 5px; background: #D8D8BF; padding-bottom: 2%;">
									<!-- Tipo de reporte -->
									<asp:Label ID="lblReporteActivo" runat="server" CssClass="lblReporteActivo" Text="Por favor seleccione del menu el reporte que desee ver."></asp:Label>

									<div id="wrapperDeLaInfo" style="display: none;">

										<!-- Criterios de Búsqueda -->
										<div style="float: left; width: 96%; padding: 2%;">
											<div style="float: left;">

												<div style="width: 100%; float: left; font-weight: bold; font-size: 16px; border-bottom: 1px solid #fff; margin-bottom: 5px;">Buscar:</div>
												<!-- TXT Buscar -->
												<div style="float: left; margin-right: 10px; width: 120px;">
													<br />
													<asp:TextBox ID="txtBuscarGeneral" onkeydown="enterBuscar(event, 'MainContent_btnBuscar');" CssClass="inputElement" runat="server"></asp:TextBox>
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

												<!-- BTN Generar PDF -->
												<div style="float: left; margin-right: 1%;">
													<br />
													<asp:Button ID="btnPopUpGenerarPDF" runat="server" Text="Generar PDF" CausesValidation="false"
														OnClick="btnPopUpGenerarPDF_Click" CssClass="boton ui-widget ui-state-default ui-corner-all ui-button-text-only" Visible="false" />
												</div>
											</div>
										</div>

										<!-- Grid -->
										<div style="float: left; width: 96%; margin: 0 2%;">
											<asp:GridView ID="GridViewReporte" CssClass="table_css"
												GridLines="Both" AllowPaging="True" RowStyle-HorizontalAlign="Center" OnPageIndexChanging="GridReportes_PageIndexChanging"
												RowStyle-VerticalAlign="Middle" PageSize="15"
												PagerStyle-CssClass="pagerGlobal" runat="server">
											</asp:GridView>
										</div>
									</div>
								</div>
							</ContentTemplate>
						</asp:UpdatePanel>
					</div>

				</ContentTemplate>
			</asp:UpdatePanel>

			<!-- PopUp para generación de PDF -->
			<div id="popUpPDF">
				<asp:UpdatePanel runat="server" ID="updatePopUp">
					<Triggers>
						<asp:AsyncPostBackTrigger ControlID="btnInvisGenerarPDF" EventName="Click" />
					</Triggers>
					<ContentTemplate>
						<!-- cuerpo del PopUp -->
						<div style="width: 96%; padding: 2%; float: left; background: #D8D8BF; border-radius: 5px;">
							<div>
								<span>Destinatario:*</span>
								<asp:TextBox runat="server" ID="txtDestinatario" Width="100%"></asp:TextBox>
								<asp:RequiredFieldValidator Display="Dynamic" ControlToValidate="txtDestinatario" ID="RequiredFieldValidator1" runat="server" ErrorMessage="*Destinatario requerido"></asp:RequiredFieldValidator>
								<asp:RegularExpressionValidator ValidationExpression="^[a-zA-ZñÑáéíóúÁÉÍÓÚüÜ\ |., ]{1,50}$" Display="Dynamic" ControlToValidate="txtDestinatario" ID="RegularExpressionValidator1" runat="server" ErrorMessage="*Se han escrito caracteres inválidos"></asp:RegularExpressionValidator>
							</div>
							<div>
								<span>Remitente:*</span>
								<asp:TextBox runat="server" ID="txtRemitente" Width="100%"></asp:TextBox>
								<asp:RequiredFieldValidator Display="Dynamic" ControlToValidate="txtRemitente" ID="RequiredFieldValidator2" runat="server" ErrorMessage="*Remitente requerido"></asp:RequiredFieldValidator>
								<asp:RegularExpressionValidator Display="Dynamic" ControlToValidate="txtRemitente" ValidationExpression="^[a-zA-ZñÑáéíóúÁÉÍÓÚüÜ\ |., ]{1,50}$" ID="RegularExpressionValidator3" runat="server" ErrorMessage="*Se han escrito caracteres inválidos"></asp:RegularExpressionValidator>
							</div>
							<div style="width: 50%; float: left;">
								<div>
									<span style="display: block">Iniciales:*</span>
									<asp:TextBox runat="server" ID="txtIniciales"></asp:TextBox>
									<asp:RequiredFieldValidator Display="Dynamic" ControlToValidate="txtIniciales" ID="RequiredFieldValidator3" runat="server" ErrorMessage="*Iniciales requeridas"></asp:RequiredFieldValidator>
									<asp:RegularExpressionValidator  ValidationExpression="^[A-ZÑÁÉÍÓÚÜ]{1,50}$" Display="Dynamic" ControlToValidate="txtIniciales" ID="RegularExpressionValidator2" runat="server" ErrorMessage="*Sólo mayúsculas"></asp:RegularExpressionValidator>
								</div>
								<div>
									<span style="display: block">Cantidad de horas:</span>
									<!-- Llenado por programación -->
									<asp:DropDownList ID="ddlCantHoras" runat="server"></asp:DropDownList>
								</div>
							</div>
							<div>
								<div style="width: 50%; float: left;">
									<span style="display: block">Período:</span>
									<asp:Label ID="lblPeriodo" runat="server" Text=""></asp:Label>
								</div>
								<div style="width: 50%; float: left;">
									<span style="display: block">Año:</span>
									<asp:Label ID="lblAño" runat="server" Text=""></asp:Label>
								</div>
							</div>
						</div>
					</ContentTemplate>
				</asp:UpdatePanel>
			</div>
		</asp:View>

		<!-- Sin acceso al módulo -->
		<asp:View ID="VistaSinPermiso" runat="server">
			<h2 style="color: Red; text-align: center;">Lo sentimos. Usted no tiene acceso a esta sección.</h2>
		</asp:View>

	</asp:MultiView>
</asp:Content>

