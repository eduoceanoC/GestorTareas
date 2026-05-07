using NUnit.Framework;
using System;
using GestorTareas;
using GestorTareas.Domain;

namespace TestProject1
{
    public class Tests
    {    

        [Test]
        public void CrearTareaSimple_ValoresCorrectos()
        {
            var fecha = DateTime.Today.AddDays(5);

            var tarea = new TareaSimple(
                "Estudiar",
                "Repasar NUnit",
                fecha,
                PrioridadTarea.Media);

            Assert.AreEqual("Estudiar", tarea.Titulo);
            Assert.AreEqual("Repasar NUnit", tarea.Descripcion);
            Assert.AreEqual(PrioridadTarea.Media, tarea.Prioridad);
            Assert.AreEqual(EstadoTarea.Pendiente, tarea.Estado);
        }


        [Test]
        public void CrearTareaSimple_TituloVacio_LanzaExcepcion()
        {
            var fecha = DateTime.Today.AddDays(1);

            Assert.Throws<ArgumentException>(() =>
            {
                var tarea = new TareaSimple(
                    "",
                    "Descripcion",
                    fecha,
                    PrioridadTarea.Alta);
            });
        }

        [Test]
        public void IniciarTarea_CambiaEstadoAEnProgreso()
        {
            var tarea = new TareaSimple(
                "Test",
                "Descripcion",
                DateTime.Today.AddDays(2),
                PrioridadTarea.Media);

            bool resultado = tarea.Iniciar();

            Assert.IsTrue(resultado);
            Assert.AreEqual(EstadoTarea.EnProgreso, tarea.Estado);
        }


        [Test]
        public void CompletarTarea_CambiaEstadoACompletada()
        {
            var tarea = new TareaSimple(
                "Test",
                "Descripcion",
                DateTime.Today.AddDays(2),
                PrioridadTarea.Media);

            tarea.Iniciar();

            bool resultado = tarea.Completar();

            Assert.IsTrue(resultado);
            Assert.AreEqual(EstadoTarea.Completada, tarea.Estado);
        }


        [Test]
        public void CancelarTarea_GuardaMotivo()
        {
            var tarea = new TareaSimple(
                "Test",
                "Descripcion",
                DateTime.Today.AddDays(2),
                PrioridadTarea.Media);

            bool resultado = tarea.Cancelar("No necesaria");

            Assert.IsTrue(resultado);
            Assert.AreEqual(EstadoTarea.Cancelada, tarea.Estado);
            Assert.AreEqual("No necesaria", tarea.MotivoCancelacion);
        }



        [Test]
        public void CrearTareaRecurrente_IntervaloCorrecto()
        {
            var tarea = new TareaRecurrente(
                "Backup",
                "Backup semanal",
                DateTime.Today.AddDays(7),
                PrioridadTarea.Alta,
                7);

            Assert.AreEqual(7, tarea.IntervaloEnDias);
        }


        [Test]
        public void GenerarSiguiente_Recurrente_CreaNuevaFecha()
        {
            var tarea = new TareaRecurrente(
                "Backup",
                "Backup semanal",
                DateTime.Today.AddDays(7),
                PrioridadTarea.Alta,
                7);

            var siguiente = tarea.GenerarSiguiente();

            Assert.AreEqual(
                tarea.FechaLimite.AddDays(7),
                siguiente.FechaLimite);
        }


        [Test]
        public void CrearTareaRecurrente_IntervaloInvalido_LanzaExcepcion()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                var tarea = new TareaRecurrente(
                    "Backup",
                    "Desc",
                    DateTime.Today.AddDays(7),
                    PrioridadTarea.Media,
                    0);
            });
        }

        [Test]
        public void CrearTareaUrgente_ResponsableCorrecto()
        {
            var fecha = DateTime.Now.AddHours(5);

            var tarea = new TareaUrgente(
                "Servidor caido",
                "Revisar urgente",
                fecha,
                PrioridadTarea.Alta,
                "Juan");

            Assert.AreEqual("Juan", tarea.Responsable);
        }


        [Test]
        public void CrearTareaUrgente_SinResponsable_LanzaExcepcion()
        {
            var fecha = DateTime.Now.AddHours(2);

            Assert.Throws<ArgumentException>(() =>
            {
                var tarea = new TareaUrgente(
                    "Error",
                    "Desc",
                    fecha,
                    PrioridadTarea.Alta,
                    "");
            });
        }


        [Test]
        public void TareaVencida_FechaPasada_ReturnTrue()
        {
            var tarea = new TareaSimple(
                "Test",
                "Desc",
                DateTime.Today.AddDays(1),
                PrioridadTarea.Media);

            tarea.FechaLimite = DateTime.Now.AddSeconds(-1);

            bool vencida = tarea.EstaVencida;

            Assert.IsTrue(vencida);
        }


        [Test]
        public void TareaCompletada_NoEstaVencida()
        {
            var tarea = new TareaSimple(
                "Test",
                "Desc",
                DateTime.Today.AddDays(1),
                PrioridadTarea.Media);

            tarea.Iniciar();
            tarea.Completar();

            bool vencida = tarea.EstaVencida;

            Assert.IsFalse(vencida);
        }


        [Test]
        public void TareaTieneGuid_AlCrearse()
        {
            var tarea = new TareaSimple(
                "Test",
                "Desc",
                DateTime.Today.AddDays(1),
                PrioridadTarea.Media);

            Assert.AreNotEqual(Guid.Empty, tarea.Id);
        }

    }
}