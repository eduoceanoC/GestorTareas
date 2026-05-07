using Moq;
using GestorTareas.Domain;
using GestorTareas.Data;
using GestorTareas.Services;

namespace TestProject1
{
    [TestFixture]
    public class GestorTareasServiceTests
    {
        [Test]
        public void ObtenerTodas_DevuelveLista()
        {
            // Arrange
            var mockRepo = new Mock<ITareaRepository>();
            var tareas = new List<Tarea>
            {
                new TareaSimple("Tarea 1", "Desc 1", DateTime.Today.AddDays(1), PrioridadTarea.Media),
                new TareaSimple("Tarea 2", "Desc 2", DateTime.Today.AddDays(2), PrioridadTarea.Alta)
            };

            mockRepo.Setup(r => r.ObtenerTodas()).Returns(tareas);

            var servicio = new TareaService(mockRepo.Object);

            // Act
            var resultado = servicio.ObtenerTodas();

            // Assert
            Assert.That(resultado, Has.Count.EqualTo(2));
            Assert.That(resultado[0].Titulo, Is.EqualTo("Tarea 1"));
            Assert.That(resultado[1].Titulo, Is.EqualTo("Tarea 2"));
        }

        [Test]
        public void Crear_LanzaArgumentException()
        {
            // Arrange
            var mockRepo = new Mock<ITareaRepository>();
            var servicio = new TareaService(mockRepo.Object);

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
            {
                var tarea = new TareaSimple("", "Desc", DateTime.Today.AddDays(1), PrioridadTarea.Media);
            });
        }

        [Test]
        public void Crear_LlamaAgregarUnaVez()
        {
            // Arrange
            var mockRepo = new Mock<ITareaRepository>();
            var servicio = new TareaService(mockRepo.Object);
            var tarea = new TareaSimple("Test", "Desc", DateTime.Today.AddDays(1), PrioridadTarea.Media);
            var usuarioId = Guid.NewGuid();

            // Act
            servicio.AgregarTarea(tarea, usuarioId);

            // Assert
            mockRepo.Verify(r => r.Agregar(It.Is<Tarea>(t => t.Titulo == "Test" && t.UsuarioId == usuarioId)), Times.Once);
        }
    }
}