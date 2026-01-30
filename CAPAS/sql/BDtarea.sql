CREATE DATABASE SistemaLogin;
GO

USE SistemaLogin;
GO

CREATE TABLE Usuario (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Usuario VARCHAR(50) NOT NULL,
    Clave VARCHAR(50) NOT NULL
);
GO

INSERT INTO Usuario (Usuario, Clave) 
VALUES ('admin', '1234');
GO