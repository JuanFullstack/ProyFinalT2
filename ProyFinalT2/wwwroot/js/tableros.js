function agregarNuevoTableroAlListado() {
    tableroListadoViewModel.tableros.push(new tableroElementoListadoViewModel({ id: 0, titulo: '' }));

    $("[name=titulo-tablero]").last().focus();
}

async function manejarFocusoutTituloTablero(tablero) {
    const titulo = tablero.titulo();
    if (!titulo) {
        tableroListadoViewModel.tableros.pop();
        return;
    }

    const data = JSON.stringify(titulo);
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
    })

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
    const ids = $("[name=titulo-tablero]").map(function () {
        return $(this).attr("data-id");
    }).get();
    return ids;
}

async function enviarIdsTablerosAlBackend(ids) {
    var data = JSON.stringify(ids);
    await fetch(`${urlTableros}/ordenar`, {
        method: 'POST',
        body: data,
        headers: {
            'Content-Type': 'application/json'
        }
    });
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
    tableroEditarVM.titulo(json.titulo);
    tableroEditarVM.descripcion(json.descripcion);

    tableroEditarVM.pasos([]);

    json.pasos.forEach(paso => {
        tableroEditarVM.pasos.push(
            new pasoViewModel({ ...paso, modoEdicion: false })
        )
    })

    modalEditarTableroBootstrap.show();
}

async function manejarCambioEditarTablero() {
    const obj = {
        id: tableroEditarVM.id,
        titulo: tableroEditarVM.titulo(),
        descripcion: tableroEditarVM.descripcion()
    };

    if (!obj.titulo) {
        return;
    }

    await editarTableroCompleto(obj);

    const indice = tableroListadoViewModel.tableros().findIndex(t => t.id() === obj.id);
    const tablero = tableroListadoViewModel.tableros()[indice];
    tablero.titulo(obj.titulo);
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
        callbackAceptar: () => {
            borrarTablero(tablero);
        },
        callbackCancelar: () => {
            modalEditarTableroBootstrap.show();
        },
        titulo: `¿Desea borrar el tablero ${tablero.titulo()}?`
    })
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

function obtenerTableroEnEdicion() {
    const indice = obtenerIndiceTableroEnEdicion();
    return tableroListadoViewModel.tableros()[indice];
}

$(function () {
    $("#reordenable").sortable({
        axis: 'y',
        stop: async function () {
            await actualizarOrdenTableros();
        }
    })
})
