-- 1. CREACIÓN DE LA BASE DE DATOS
CREATE DATABASE dbAPPenvio;
GO

USE dbAPPenvio;
GO

-- 2. LOGIN
CREATE TABLE Roles (
    RolId INT PRIMARY KEY IDENTITY(1,1),
    NombreRol VARCHAR(50) NOT NULL
);

CREATE TABLE Usuarios (
    UsuarioId INT PRIMARY KEY IDENTITY(1,1),
    Correo VARCHAR(100) NOT NULL UNIQUE,
    Password VARCHAR(255) NOT NULL,
    RolId INT FOREIGN KEY REFERENCES Roles(RolId)
);

-- 3. CATÁLOGOS
CREATE TABLE Clientes (
    ClienteId INT PRIMARY KEY IDENTITY(1,1),
    Nombre VARCHAR(100) NOT NULL,
    Telefono VARCHAR(20)
);

CREATE TABLE Destinatarios (
    DestinatarioId INT PRIMARY KEY IDENTITY(1,1),
    Nombre VARCHAR(100) NOT NULL,
    Direccion TEXT
);

CREATE TABLE Sucursales (
    SucursalId INT PRIMARY KEY IDENTITY(1,1),
    NombreSucursal VARCHAR(100) NOT NULL
);

CREATE TABLE EstadosEnvio (
    EstadoId INT PRIMARY KEY IDENTITY(1,1),
    NombreEstado VARCHAR(50) NOT NULL
);

CREATE TABLE EstadosPago (
    EstadoPagoId INT PRIMARY KEY IDENTITY(1,1),
    NombreEstado VARCHAR(50) NOT NULL 
);

-- 4. TABLA ENVÍOS
CREATE TABLE Envios (
    EnvioId INT PRIMARY KEY IDENTITY(1,1),
    ClienteId INT NOT NULL FOREIGN KEY REFERENCES Clientes(ClienteId),
    DestinatarioId INT NOT NULL FOREIGN KEY REFERENCES Destinatarios(DestinatarioId),
    SucursalOrigenId INT NOT NULL FOREIGN KEY REFERENCES Sucursales(SucursalId),
    EstadoId INT NOT NULL FOREIGN KEY REFERENCES EstadosEnvio(EstadoId),
    EstadoPagoId INT NOT NULL FOREIGN KEY REFERENCES EstadosPago(EstadoPagoId),
    Costo DECIMAL(10,2) NOT NULL,
    FechaEnvio DATETIME DEFAULT GETDATE(),
    Activo BIT NOT NULL DEFAULT 1 
);

-- 5. TABLA DE AUDITORÍA 
CREATE TABLE Auditoria_Envios (
    AuditoriaId INT PRIMARY KEY IDENTITY(1,1),
    EnvioId INT NOT NULL FOREIGN KEY REFERENCES Envios(EnvioId),
    UsuarioId INT NOT NULL FOREIGN KEY REFERENCES Usuarios(UsuarioId),
    Accion VARCHAR(50) NOT NULL,
    DetalleCambio TEXT,
    FechaModificacion DATETIME DEFAULT GETDATE()
);
GO