<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeFile="Default.aspx.cs" Inherits="_Default" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <script type="text/javascript">
        $(document).ready(function () {
            $("a.level1:contains('Inicio')").addClass("item_active");
        });
    </script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <h2>
       ¡Bienvenidos al Sistema de Control de Becarios 11!
    </h2>
    <p>
        <i>De la Escuela de Ciencias de la Computación e Informática</i>.
    </p>
    <p>
        Aplicación diseñada para el curso de <b>Ingeniería de Software 2</b> por:
    </p>
    <ul>
        <li>Constatino Bolaños Araya - B00962</li>
        <li>Manuel Hidalgo Murillo - B03143</li>
        <li>Christopher Sánchez Coto - B05829</li>
        <li>Gabriel Ulloa Murillo - A96320</li>
        <li>Heriberto Ureña Madrigal - B06432</li>        
        <li>Hugo Villalta Mena - A86985</li>
    </ul>
	<p>
		Bajo la supervisión de:
	</p>
	<ul><li>MSc. Gabriela Salazar Bermúdez</li></ul>
</asp:Content>

