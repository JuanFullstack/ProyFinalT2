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

    modalEditarTableroBootstrap.show();
}

async function manejarCambioEditarTablero() {
    const obj = {
        id: tableroEditarVM.id,
        nombre: tableroEditarVM.nombre()
    };

    if (!obj.nombre) {
        return;
    }

    const data = JSON.stringify(obj);

    const respuesta = await fetch(`${urlTableros}/${obj.id}`, {
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

    const indice = tableroListadoViewModel.tableros().findIndex(t => t.id() === obj.id);
    const tablero = tableroListadoViewModel.tableros()[indice];
    tablero.nombre(obj.nombre);
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
        titulo: `¿Desea borrar el tablero ${tablero.nombre()}?`
    });
}

async function borrarTablero(tablero) {
    const idTablero = tablero.id();

    const respuesta = await fetch(`${urlTableros}/${idTablero}`, {
        method: 'DELETE',
        headers: {
            'Content-Type': 'application/json'
        }
    });

    if (respuesta.ok) {
        const indice = tableroListadoViewModel.tableros().findIndex(t => t.id() === tablero.id());
        tableroListadoViewModel.tableros.splice(indice, 1);
    }
}

$(function () {
    obtenerTableros();
});

function tableroListadoViewModelFn() {
    var self = this;
    self.tableros = ko.observableArray([]);
    self.cargando = ko.observable(true);

    self.noHayTableros = ko.pureComputed(function () {
        if (self.cargando()) {
            return false;
        }
        return self.tableros().length === 0;
    });
}

function tableroElementoListadoViewModel({ id, nombre }) {
    var self = this;
    self.id = ko.observable(id);
    self.nombre = ko.observable(nombre);
    self.esNuevo = ko.pureComputed(function () {
        return self.id() === 0;
    });
}

const tableroEditarVM = {
    id: 0,
    nombre: ko.observable('')
};

const tableroListadoViewModel = new tableroListadoViewModelFn();
ko.applyBindings(tableroListadoViewModel, document.getElementById('contenedor-listado-tableros'));
ko.applyBindings(tableroEditarVM, document.getElementById('modal-editar-tablero'));
