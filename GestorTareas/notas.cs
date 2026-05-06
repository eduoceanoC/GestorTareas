//--Crear base de datos solo si no existe
//IF DB_ID('GestorTareas') IS NULL
//BEGIN
//    CREATE DATABASE GestorTareas;
//END
//GO

//USE GestorTareas;
//GO

//-- Usuarios
//CREATE TABLE Usuarios (
//    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
//    Nombre NVARCHAR(100) NOT NULL,
//    Email NVARCHAR(150) UNIQUE NOT NULL
//);
//GO

//-- Tareas
//CREATE TABLE Tareas (
//    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),

//    UsuarioId UNIQUEIDENTIFIER NULL,

//    Titulo NVARCHAR(200) NOT NULL,
//    Descripcion NVARCHAR(MAX) NULL,

//    FechaCreacion DATETIME2 NOT NULL DEFAULT GETDATE(),
//    FechaLimite DATETIME2 NOT NULL,

//    Prioridad TINYINT NOT NULL,
//    Estado TINYINT NOT NULL,

//    MotivoCancelacion NVARCHAR(500) NULL,

//    TipoTarea TINYINT NOT NULL,

//    IntervaloEnDias INT NULL,
//    Responsable NVARCHAR(100) NULL,
//    FechaLimiteHora DATETIME2 NULL,

//    CONSTRAINT FK_Tareas_Usuarios 
//        FOREIGN KEY (UsuarioId) REFERENCES Usuarios(Id),

//    CONSTRAINT CHK_Prioridad CHECK (Prioridad IN (0,1,2)),
//    CONSTRAINT CHK_Estado CHECK (Estado IN (0,1,2,3)),
//    CONSTRAINT CHK_Tipo CHECK (TipoTarea IN (1,2,3)),

//    CONSTRAINT CHK_Recurrente CHECK (
//        (TipoTarea != 2) OR (IntervaloEnDias IS NOT NULL AND IntervaloEnDias > 0)
//    ),

//    CONSTRAINT CHK_Urgente CHECK (
//        (TipoTarea != 3) OR (Responsable IS NOT NULL AND FechaLimiteHora IS NOT NULL)
//    )
//);
//GO

//-- Estadísticas
//CREATE TABLE Estadisticas (
//    Clave NVARCHAR(50) PRIMARY KEY,
//    Valor INT NOT NULL
//);
//GO

//INSERT INTO Estadisticas (Clave, Valor) 
//VALUES ('TareasEliminadas', 0);
//GO








//--Crear usuarios
//DECLARE @User1 UNIQUEIDENTIFIER = NEWID();
//DECLARE @User2 UNIQUEIDENTIFIER = NEWID();

//INSERT INTO Usuarios(Id, Nombre, Email)
//VALUES 
//(@User1, 'Edu', 'edu@email.com'),
//(@User2, 'Ana', 'ana@email.com');


//-- ========================
//--TAREAS USUARIO 1 (Edu)
//-- ========================

//-- Simple
//INSERT INTO Tareas (UsuarioId, Titulo, Descripcion, FechaLimite, Prioridad, Estado, TipoTarea)
//VALUES 
//(@User1, 'Estudiar SQL', 'Practicar consultas', '2026-05-01', 1, 0, 1),
//(@User1, 'Hacer ejercicio', 'Ir al gimnasio', '2026-05-02', 0, 2, 1);

//--Recurrente
//INSERT INTO Tareas(UsuarioId, Titulo, Descripcion, FechaLimite, Prioridad, Estado, TipoTarea, IntervaloEnDias)
//VALUES 
//(@User1, 'Backup sistema', 'Copia de seguridad', '2026-05-03', 2, 0, 2, 7);

//--Urgente
//INSERT INTO Tareas(UsuarioId, Titulo, Descripcion, FechaLimite, Prioridad, Estado, TipoTarea, Responsable, FechaLimiteHora)
//VALUES 
//(@User1, 'Bug producción', 'Error crítico', '2026-04-25', 2, 1, 3, 'Edu', '2026-04-25 18:00'),
//(@User1, 'Deploy app', 'Subir cambios', '2026-04-26', 1, 0, 3, 'Edu', '2026-04-26 12:00');


//-- ========================
//--TAREAS USUARIO 2 (Ana)
//-- ========================

//-- Simple
//INSERT INTO Tareas (UsuarioId, Titulo, Descripcion, FechaLimite, Prioridad, Estado, TipoTarea)
//VALUES 
//(@User2, 'Leer libro', 'Capítulos 1-3', '2026-05-04', 0, 0, 1),
//(@User2, 'Comprar comida', 'Supermercado', '2026-04-28', 1, 2, 1);

//--Recurrente
//INSERT INTO Tareas(UsuarioId, Titulo, Descripcion, FechaLimite, Prioridad, Estado, TipoTarea, IntervaloEnDias)
//VALUES 
//(@User2, 'Limpiar casa', 'Limpieza semanal', '2026-05-05', 1, 1, 2, 7);

//--Urgente
//INSERT INTO Tareas(UsuarioId, Titulo, Descripcion, FechaLimite, Prioridad, Estado, TipoTarea, Responsable, FechaLimiteHora)
//VALUES 
//(@User2, 'Entregar informe', 'Trabajo final', '2026-04-24', 2, 0, 3, 'Ana', '2026-04-24 20:00'),
//(@User2, 'Reunión jefe', 'Revisión semanal', '2026-04-23', 2, 1, 3, 'Ana', '2026-04-23 10:00');



