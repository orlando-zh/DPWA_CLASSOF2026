<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Habitaciones.aspx.cs" Inherits="CapaPresentacion.Habitaciones" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <link href="CSS/Estilo.css" rel="stylesheet"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
<div>
    <h2>Agregar habitaciones</h2>
    <asp:Label ID="lblNumero" runat="server" Text="Número de habitación" />   <br />
    <asp:TextBox  ID="txtNumero"  runat="server" /><br /><br />
    <asp:Label ID="lblDescripcion" runat="server" Text="Descripción general" /> <br />
    <asp:TextBox  ID="txtDescripcion"    runat="server" /><br /><br />
    <asp:Label  ID="lblCant" runat="server"  Text="Cantidad de huéspedes permitidos" />  <br />
    <asp:TextBox ID="txtCant" runat="server" /> <br /><br />
    <asp:Button ID="btnGuardar" runat="server" Text="Guardar" OnClick="btnGuardar_Click" />
</div>
        <hr />

        <asp:GridView ID="dgvHabitaciones" runat="server" AutoGenerateColumns="false" DataKeyNames="id_habitaciones"
            DataUpdating="dvgHabitaciones_RowUpdating"
            DataCancelingEdit="dvgHabitaciones_RowCancelingEdit"
            OnRowDeleting="dvgHabitaciones_RowDeleting" OnRowCancelingEdit="dgvHabitaciones_RowCancelingEdit" OnRowEditing="dgvHabitaciones_RowEditing" OnRowUpdating="dgvHabitaciones_RowUpdating">
            <Columns>
                <asp:BoundField DataField="id_habitaciones" HeaderText="ID" />
                <asp:BoundField DataField="numero" HeaderText="#" />
                <asp:BoundField DataField="descripcion" HeaderText="Descipción" />
                <asp:BoundField DataField="cant_huespedes" HeaderText="Max-Personas" />
                <asp:CommandField ShowEditButton="true" EditText="Editar" />
                <asp:CommandField ShowDeleteButton="true" DeleteText="Eliminar" />
            </Columns>
        </asp:GridView>
    </form>
</body>
</html>
