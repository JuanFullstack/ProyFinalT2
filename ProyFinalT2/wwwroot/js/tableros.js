function agregarNuevoTableroAlListado() {
    tableroListadoViewModel.tableros.push(new tableroElementoListadoViewModel({ id: 0, nombre: '' }));
    $("[name=nombre-tablero]").last().focus();
}

async function manejarFocusoutNombreTablero(tablero) {
    const nombre = tablero.nombre();
    if (!nombre) {
        tableroListadoViewModel.tableros.pop();
        return;
    }

    const data = JSON.stringify(nombre);
    const respuesta = await fetch(urlTableros, {
        method: 'POST',
        body: data,
        headers: {
            'Content-Type': 'application/json'
        }
    });

    if (respuesta.ok) {
        const json = await respuesta.json();
        tablero.id(json.id);
    } else {
        manejarErrorApi(respuesta);
    }
}

async function obtenerTableros() {
    tableroListadoViewModel.cargando(true);

    const respuesta = await fetch(urlTableros, {
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

async function manejarClickTablero(tablero) {
    const respuesta = await fetch(`${urlTableros}/${tablero.id()}`, {
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
    // Cargar las tareas del tablero aquí
}

function intentarBorrarTablero(tablero) {
    confirmarAccion({
        callBackAceptar: () => borrarTablero(tablero),
        titulo: `¿Desea borrar el tablero ${tablero.nombre()}?`
    });
}

async function borrarTablero(tablero) {
    const idTablero = tablero.id;

    const respuesta = await fetch(`${urlTableros}/${idTablero}`, {
        method: 'DELETE',
        headers: {
            'Content-Type': 'application/json'
        }
    });

    if (respuesta.ok) {
        tableroListadoViewModel.tableros.remove(tablero);
    }
}

$(function () {
    obtenerTableros();
});
