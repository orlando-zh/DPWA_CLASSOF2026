<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="ProyectoReservas.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
<div>
        <h1>Inicio de Sesión - Proyecto Reservas</h1>
        
        <p>
            Usuario:<br />
            <asp:TextBox ID="txtUsuario" runat="server" placeholder="Ingrese usuario"></asp:TextBox>
        </p>

        <p>
            Contraseña:<br />
            <asp:TextBox ID="txtClave" runat="server" TextMode="Password" placeholder="Ingrese clave"></asp:TextBox>
        </p>

        <p>
            <asp:Button ID="btnLogin" runat="server" Text="Ingresar" OnClick="btnLogin_Click" />
        </p>

        <p>
            <asp:Label ID="lblMensaje" runat="server" Text="" ForeColor="Red"></asp:Label>
        </p>
    </div>
    </form>
</body>
</html>
