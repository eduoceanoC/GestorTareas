// =============================================
// GESTORTAREAS v1.0 — app.js
// Tarjetas dinámicas desde array de objetos
// Primer paso hacia Angular
// =============================================

// ─── Array de tareas de ejemplo ───
const tareas = [
    {
        id: 1,
        titulo: 'Estudiar SQL',
        descripcion: 'Practicar consultas avanzadas',
        tipo: 'Simple',
        estaCompletada: false,
        fechaLimite: '2026-05-01',
        nombreUsuario: 'Edu'
    },
    {
        id: 2,
        titulo: 'Hacer ejercicio',
        descripcion: 'Ir al gimnasio por la mañana',
        tipo: 'Recurrente',
        estaCompletada: true,
        fechaLimite: '2026-05-02',
        nombreUsuario: 'Edu'
    },
    {
        id: 3,
        titulo: 'Bug producción',
        descripcion: 'Error crítico en servidor',
        tipo: 'Urgente',
        estaCompletada: false,
        fechaLimite: '2026-04-25',
        nombreUsuario: 'Edu'
    },
    {
        id: 4,
        titulo: 'Leer libro',
        descripcion: 'Capítulos 1 al 3',
        tipo: 'Simple',
        estaCompletada: false,
        fechaLimite: '2026-05-04',
        nombreUsuario: 'Ana'
    },
    {
        id: 5,
        titulo: 'Limpiar casa',
        descripcion: 'Limpieza semanal completa',
        tipo: 'Recurrente',
        estaCompletada: true,
        fechaLimite: '2026-05-05',
        nombreUsuario: 'Ana'
    }
];

// ─── Función crearTarjetaHTML ───
function crearTarjetaHTML(tarea) {
    const badgeClase = tarea.estaCompletada ? 'bg-success' : 'bg-warning text-dark';
    const badgeTexto = tarea.estaCompletada ? 'Completada' : 'Pendiente';

    return `
        <article class="card mb-3" data-id="${tarea.id}">
            <div class="card-body">
                <div class="d-flex justify-content-between align-items-start">
                    <h5 class="card-title">${tarea.titulo}</h5>
                    <span class="badge ${badgeClase}">${badgeTexto}</span>
                </div>
                <p class="card-text text-muted">${tarea.descripcion}</p>
                <div class="d-flex justify-content-between align-items-center">
                    <small class="text-muted">
                        <span class="fw-bold">${tarea.nombreUsuario}</span> &middot;
                        ${tarea.tipo} &middot;
                        ${tarea.fechaLimite}
                    </small>
                </div>
            </div>
        </article>
    `;
}

// ─── Renderizar todas las tareas ───
function renderizarTareas(contenedorId = 'lista-contenido') {
    const contenedor = document.getElementById(contenedorId);
    if (!contenedor) return;

    contenedor.innerHTML = '';
    tareas.forEach(t => {
        contenedor.innerHTML += crearTarjetaHTML(t);
    });
}

// ─── Inicializar cuando el DOM esté listo ───
document.addEventListener('DOMContentLoaded', () => {
    renderizarTareas();
});