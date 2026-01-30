<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs"
    Inherits="ProyectoReservas.Default" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>Login</title>
</head>
<body>

    <form id="form1" runat="server">

        <div class="navbar">
            <h1>Sistema</h1>

            <div class="login-form">
                <asp:TextBox ID="txtUsuario" runat="server" Placeholder="Usuario" />

                <asp:TextBox ID="txtClave" runat="server" TextMode="Password" Placeholder="Contraseña" />

                <asp:Button ID="btnLogin" runat="server" Text="Ingresar" OnClick="btnLogin_Click" />
            </div>
        </div>

        <asp:Label ID="lblMensaje" runat="server" CssClass="mensaje" />

    </form>

</body>
</html>
