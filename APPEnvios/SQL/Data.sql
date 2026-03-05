USE dbAPPenvio;
GO

-- ROLES Y USUARIO ADMIN
INSERT INTO Roles (NombreRol) VALUES ('Administrador'), ('Operador');
INSERT INTO Usuarios (Correo, Password, RolId) 
VALUES ('admin@smallbox.com', 'admin123', 1);

-- CATÁLOGOS NECESARIOS
INSERT INTO EstadosEnvio (NombreEstado) VALUES ('Pendiente'), ('En Tránsito'), ('Entregado');
INSERT INTO EstadosPago (NombreEstado) VALUES ('Pendiente'), ('Pagado'), ('Cancelado');
INSERT INTO Sucursales (NombreSucursal) VALUES ('Sucursal Central'), ('San Salvador'), ('La Unión');

-- CLIENTES Y DESTINATARIOS DE PRUEBA
INSERT INTO Clientes (Nombre, Telefono) VALUES ('Orlando Zúniga', '7777-8888');
INSERT INTO Destinatarios (Nombre, Direccion) VALUES ('Empresa Roja', 'Av. Las Magnolias #12');

-- UN ENVÍO INICIAL PARA QUE EL INDEX NO ESTÉ VACÍO
INSERT INTO Envios (ClienteId, DestinatarioId, SucursalOrigenId, EstadoId, EstadoPagoId, Costo, Activo)
VALUES (1, 1, 1, 1, 2, 15.50, 1);
GO