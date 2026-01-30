<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="Principal.aspx.cs"
    Inherits="ProyectoReservas.Principal" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>Principal</title>
    <link href="CSS/Estilo.css" rel="stylesheet" />
</head>
<body>

    <form id="form1" runat="server">

        <div class="navbar">
            <h1>Sistema</h1>

            <div class="user-info">
                <span class="username">Usuario:</span>
                <asp:Label ID="lblusuario" runat="server" CssClass="username" />
            </div>

            <asp:Button ID="out" runat="server" Text="Cerrar Sesión"  OnClick="out_Click" />
        </div>

        <div class="content">
            <h2>Bienvenidos</h2>
            <p>Has iniciado sesión correctamente.</p>
        </div>

    </form>

</body>
</html>
