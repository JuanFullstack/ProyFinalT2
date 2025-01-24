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

    const data = JSON.stringify({ nombre });
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
    if (tablero.esNuevo()) {
        return;
    }

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

    tableroEditarVM.id = json.id;
    tableroEditarVM.nombre(json.nombre);
    tableroEditarVM.descripcion(json.descripcion);

    modalEditarTableroBootstrap.show();
}

async function manejarCambioEditarTablero() {
    const obj = {
        id: tableroEditarVM.id,
        nombre: tableroEditarVM.nombre(),
        descripcion: tableroEditarVM.descripcion()
    };

    if (!obj.nombre) {
        return;
    }

    await editarTableroCompleto(obj);

    const indice = tableroListadoViewModel.tableros().findIndex(t => t.id() === obj.id);
    const tablero = tableroListadoViewModel.tableros()[indice];
    tablero.nombre(obj.nombre);
}

async function editarTableroCompleto(tablero) {
    const data = JSON.stringify(tablero);

    const respuesta = await fetch(`${urlTableros}/${tablero.id}`, {
        method: 'PUT',
        body: data,
        headers: {
            'Content-Type': 'application/json'
        }
    });

    if (!respuesta.ok) {
        manejarErrorApi(respuesta);
        throw "error";
    }
}

function intentarBorrarTablero(tablero) {
    modalEditarTableroBootstrap.hide();

    confirmarAccion({
        callBackAceptar: () => {
            borrarTablero(tablero);
        },
        callbackCancelar: () => {
            modalEditarTableroBootstrap.show();
        },
        titulo: `¿Desea borrar el tablero "${tablero.nombre()}"?`
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
        const indice = obtenerIndiceTableroEnEdicion();
        tableroListadoViewModel.tableros.splice(indice, 1);
    }
}

function obtenerIndiceTableroEnEdicion() {
    return tableroListadoViewModel.tableros().findIndex(t => t.id() == tableroEditarVM.id);
}

$(function () {
    $("#reordenableTableros").sortable({
        axis: 'y',
        stop: async function () {
            await actualizarOrdenTableros();
        }
    });
});

async function actualizarOrdenTableros() {
    const ids = obtenerIdsTableros();
    await enviarIdsTablerosAlBackend(ids);

    const arregloOrdenado = tableroListadoViewModel.tableros.sorted(function (a, b) {
        return ids.indexOf(a.id().toString()) - ids.indexOf(b.id().toString());
    });

    tableroListadoViewModel.tableros([]);
    tableroListadoViewModel.tableros(arregloOrdenado);
}

function obtenerIdsTableros() {
    const ids = $("[name=nombre-tablero]").map(function () {
        return $(this).attr("data-id");
    }).get();
    return ids;
}

async function enviarIdsTablerosAlBackend(ids) {
    const data = JSON.stringify(ids);
    await fetch(`${urlTableros}/ordenar`, {
        method: 'POST',
        body: data,
        headers: {
            'Content-Type': 'application/json'
        }
    });
}
