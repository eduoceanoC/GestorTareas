# MEMORY BANK - REGLA OBLIGATORIA PARA CLINE

**DEBO seguir estas reglas SIEMPRE, sin excepción:**

### Regla 1: Leer memoria antes de TODO
Al inicio de CADA tarea, DEBO leer el archivo `MEMORIA.md` de la raíz del proyecto ANTES de hacer cualquier otra cosa. Esto no es opcional.

### Regla 2: Mostrar confirmación
Al empezar cada respuesta, DEBO mostrar: `[MEMORIA CARGADA]`

### Regla 3: Actualizar después de cambios
Después de CADA modificación al código que sea significativa, DEBO actualizar `MEMORIA.md` con:
- La nueva fecha y hora
- Los cambios que hice en la sección "CAMBIOS RECIENTES"
- Si detecté un bug, registrarlo en "BUGS CONOCIDOS"

### Regla 4: Actualizar por comando
Cuando el usuario escriba "actualiza memoria", DEBO revisar TODO el código y actualizar `MEMORIA.md` completamente.

### Regla 5: Prioridad del código real
Si hay discrepancia entre `MEMORIA.md` y el código real, SIEMPRE debo priorizar el código real y luego actualizar la memoria.

---

## 📁 ESTRUCTURA QUE DEBO MANTENER EN MEMORIA.md

El archivo `MEMORIA.md` debe contener estas secciones:

1. Última actualización (fecha/hora)
2. Estructura del proyecto
3. Endpoints del backend
4. Estructura de base de datos
5. Frontend (ventanas)
6. Estados de tarea
7. Tipos de tarea
8. Prioridades
9. Bugs conocidos
10. Cambios recientes
11. Tareas pendientes

---

## 🔄 FLUJO DE TRABAJO

1. Usuario abre nueva sesión o pide una tarea
2. **YO LEO MEMORIA.md primero**
3. Respondo según el contexto guardado
4. Si hago cambios, actualizo memoria
5. Repito para cada tarea


### Regla de caché de memoria
- Lee MEMORIA.md UNA SOLA VEZ al inicio de la sesión
- Guarda su contenido en tu contexto interno
- NO la vuelvas a leer en la misma sesión a menos que:
  a) El usuario diga "actualiza memoria"
  b) Hayas hecho cambios en el código