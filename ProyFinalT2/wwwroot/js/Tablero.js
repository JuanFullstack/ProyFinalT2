async function obtenerTableros() {
    tableroListadoViewModel.cargando(true);

    const respuesta = await fetch('/api/tableros', {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json'
        }
    });

    if (!respuesta.ok) {
        manejarErrorApi(respuesta);
        return;
    }

    const json = await respuesta.json();
    tableroListadoViewModel.tableros([]);

    json.forEach(valor => {
        tableroListadoViewModel.tableros.push(new tableroElementoListadoViewModel(valor));
    });

    tableroListadoViewModel.cargando(false);
}

async function obtenerTableroConTareas(id) {
    const respuesta = await fetch(`/api/tableros/${id}`, {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json'
        }
    });

    if (!respuesta.ok) {
        manejarErrorApi(respuesta);
        return;
    }

    const json = await respuesta.json();

    tareasTableroViewModel.tareas([]);
    json.tareas.forEach(tarea => {
        tareasTableroViewModel.tareas.push(new tareaElementoListadoViewModel(tarea));
    });

    tareasTableroViewModel.tablero(json);
}
