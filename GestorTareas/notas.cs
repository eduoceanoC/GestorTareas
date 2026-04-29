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




//_____PROGRAM.CS INICIAL_____________________________
//    //using System;
//    //using Microsoft.EntityFrameworkCore;
//    //using GestorTareas.Data;
//    //using GestorTareas.Services;
//    //using GestorTareas.UI;

////namespace GestorTareas
////{
////    class Programzvzdfbzdhbsf
////    {
////        static void Main(string[] args)
////        {
////            Console.Title = "Gestor de Tareas";

////            string connectionString =
////                "Server=(localdb)\\MSSQLLocalDB;Database=GestorTareas;Trusted_Connection=True;TrustServerCertificate=True;";

////            var options = new DbContextOptionsBuilder<AppDbContext>()
////                .UseSqlServer(connectionString)
////                .Options;

////            var context = new AppDbContext(options);

////            ITareaRepository repositorio = new EfTareaRepository(context);

////            TareaService servicio = new TareaService(repositorio);

////            ConsoleUI ui = new ConsoleUI(servicio);

////            ui.Ejecutar();
////        }
////    }
////}





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





//ejercicios


//    SELECT 
//    TipoTarea,
//    COUNT(*) AS TotalPendientes
//FROM Tareas
//WHERE Estado = 0
//GROUP BY TipoTarea
//HAVING COUNT(*) > 1;

//SELECT
//    u.Nombre
//FROM Usuarios u
//WHERE EXISTS (
//    SELECT 1
//    FROM Tareas t
//    WHERE t.UsuarioId = u.Id
//      AND t.FechaLimite < GETDATE()
//);


//WITH ConteoTareas AS (
//    SELECT 
//        u.Nombre,
//        COUNT(CASE WHEN t.Estado = 2 THEN 1 ELSE 0 END) AS Completadas,
//        COUNT(CASE WHEN t.Estado = 0 THEN 1 ELSE 0 END) AS Pendientes
//    FROM Usuarios u
//    LEFT JOIN Tareas t ON t.UsuarioId = u.Id
//    GROUP BY u.Nombre
//)
//SELECT *
////FROM ConteoTareas;
//______________________________________

////PROCEDIMIENTOS ALMACENADOS

//CREATE OR ALTER PROCEDURE CompletarTarea
//    @TareaId UNIQUEIDENTIFIER
//AS
//BEGIN
//    -- comprobar existencia
//    IF NOT EXISTS (SELECT 1 FROM Tareas WHERE Id = @TareaId)
//    BEGIN
//        PRINT 'Error: la tarea no existe';
//RETURN;
//END

//-- actualizar estado a completada (2)
//    UPDATE Tareas
//    SET Estado = 2
//    WHERE Id = @TareaId;

//PRINT 'Tarea marcada como completada';
//END;


//CREATE OR ALTER PROCEDURE ResumenUsuario
//    @UsuarioId UNIQUEIDENTIFIER,
//    @Pendientes INT OUTPUT,
//    @Completadas INT OUTPUT
//AS
//BEGIN
//    SELECT 
//        @Pendientes = COUNT(CASE WHEN Estado = 0 THEN 1 END),
//        @Completadas = COUNT(CASE WHEN Estado = 2 THEN 1 END)
//    FROM Tareas
//    WHERE UsuarioId = @UsuarioId;
//END;