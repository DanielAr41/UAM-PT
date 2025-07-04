var UsuarioId = localStorage.getItem("usuarioId");
$(document).ready(function () {
    $(document).on('click', '#regresar', function () {
        window.location.href = "/Home/Inicio";
    });

    
    traeInfoPersonal(UsuarioId);
    traeDireccionUsuario(UsuarioId);
});

function traeInfoPersonal(uid) {
    //debugger;
    $.ajax({
        url: '/Cuenta/InfoUsuarioPorID',
        type: 'GET',
        data: { usuarioId: uid },
        success: function (data) {
            //debugger;
            if (data != null) {
                $('#Nombres').val(data.data.nombre);
                $('#Apaterno').val(data.data.apaterno);
                $('#Amaterno').val(data.data.amaterno);
                $('#Telefono').val(data.data.telefono);
                $('#Correo').val(data.data.correo);
            } else {
                alert('No se encontró la información del usuario.');
            }
        },
        error: function (xhr, status, error) {
            console.error('Error en la solicitud:', error);
            alert('Ocurrió un error inesperado.');
        }
    });
}


$('#AgregarDireccion').on('click', function () {
    $('#mdlAddDireccion').modal('show');
});

$('#guardarDireccion').on('click', function () {
    AgregaDireccion(UsuarioId);
});

$('#codigoPostal').on('input', function () {
    this.value = this.value.replace(/\D/g, '').slice(0, 5);
});


function AgregaDireccion(usuarioId) {
    debugger;
    let valido = true;

    const camposObligatorios = ['#calle', '#numExterior', '#localidad', '#municipio', '#codigoPostal', '#estado', '#pais'];

    camposObligatorios.forEach(function (selector) {
        const $campo = $(selector);
        if ($campo.val().trim() === '') {
            $campo.addClass('is-invalid');
            valido = false;
        } else {
            $campo.removeClass('is-invalid');
        }
    });

    if (!valido) return;

    var datos = {
        Calle: $('#calle').val(),
        Numero: $('#numExterior').val(),
        NumeroInt: $('#numInterior').val(),
        Localidad: $('#localidad').val(),
        Municipio: $('#municipio').val(),
        Estado: $('#estado').val(),
        CodigoPostal: $('#codigoPostal').val(),
        Pais: $('#pais').val(),
        uid: usuarioId 
    };

    $.ajax({
        url: '/Cuenta/AgregarDireccion',
        data: datos,
        type: 'POST',
        success: function (response) {
            //debugger;
            if (response.success) {
                alert('Direccion guardada correctamente.');
                $('#mdlAddDireccion').modal('hide');
                $('#formAddDireccion')[0].reset();
            } else {
                alert('Error al guardar el producto: ' + response.response_msg);
            }
        },
        error: function (xhr, status, error) {
            debugger;
            console.error('Error en la solicitud:', error);
            alert('Ocurrió un error inesperado.');
        }
    });
}


function traeDireccionUsuario(uid) {
    //debugger;
    $.ajax({
        url: '/Cuenta/CargaDireccion',
        type: 'GET',
        data: { usuarioId: uid },
        success: function (data) {
            //debugger;
            if (data != null) {
                let calle = data.data.calle;
                let numero = data.data.numero;
                let calleCompleta = calle + '' + numero;

                $('#calleNum').val(calleCompleta);

                $('#Colonia').val(data.data.localidad);
                $('#Estado').val(data.data.estado);
                $('#CodigoPostal').val(data.data.codigoPostal);
            } else {
                alert('No se encontró la información del usuario.');
            }
        },
        error: function (xhr, status, error) {
            console.error('Error en la solicitud:', error);
            alert('Ocurrió un error inesperado.');
        }
    });
}

$('#guardaInfoUsuario').on('click', function () {
    guardaInfoUsuario(UsuarioId);
});

function guardaInfoUsuario(usuarioId) {
    var datos = {
        Nombres: $('#Nombres').val(),
        Apaterno: $('#Apaterno').val(),
        Amaterno: $('#Amaterno').val(),
        Telefono: $('#Telefono').val(),
        Correo: $('#Correo').val(),
        uid: usuarioId
    };
    var nombres = $('#Nombres').val();

    $.ajax({
        url: '/Cuenta/guardaInformacionUsuario',
        data: datos,
        type: 'POST',
        success: function (response) {
            //debugger;
            if (response.success) {
                //alert('Información guardada correctamente.');
                Swal.fire({
                    title: '¡Guardado!',
                    text: 'Información actualizada correctamente.',
                    width: '400px',
                    customClass: {
                        popup: 'swal-small-modal'
                    },
                    confirmButtonText: 'Entendido',
                    confirmButtonColor: '#FF8800',
                });

            } else {
                alert('Error al guardar la información: ' + response.response_msg);
            }
        },
        error: function (xhr, status, error) {
            console.error('Error en la solicitud:', error);
            alert('Ocurrió un error inesperado.');
        }
    });
}

$('#misDirecciones').on('click', function () {
    $('#modalDirecciones').modal('show');
});

$('#modalDirecciones').on('show.bs.modal', function () {
    traeDireccionesPorIdUsuario(UsuarioId);
});

function traeDireccionesPorIdUsuario(uid) {
    //debugger;
    $.ajax({
        url: '/Cuenta/TraeDireccionesPorIdUsuario',
        type: 'GET',
        data: { usuarioId: uid },
        success: function (data) {
            //debugger;
            if (data && data.direcciones.length > 0) {
                cargarDirecciones(data.direcciones);
                $('#modalDirecciones').modal('show');
            } else {
                Swal.fire({
                    icon: 'info',
                    title: 'Sin direcciones',
                    text: 'Este usuario no tiene direcciones registradas.'
                });
            }
        },
        error: function (xhr, status, error) {
            console.error('Error en la solicitud:', error);
            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: 'Ocurrió un error al obtener las direcciones.'
            });
        }
    });
}

function cargarDirecciones(direcciones) {
    //debugger;
    const contenedor = $('#contenedorDirecciones');
    contenedor.empty();

    direcciones.forEach((dir) => {
        const item = `
        <div class="direccion-item py-2">
            <div class="form-check d-flex align-items-start">
                <input class="form-check-input mt-1 mr-2" type="radio" name="direccionPredeterminada" value="${dir.id}" id="radio-${dir.id}" ${dir.predeterminada ? 'checked' : ''}>
                <label class="form-check-label w-100" for="radio-${dir.id}">
                    ${dir.texto}
                </label>
            </div>
            <hr class="my-2" />
        </div>`;
        contenedor.append(item);
    });
}


$('#guardarDireccionPredeterminada').on('click', function () {
    const direccion = $('input[name="direccionPredeterminada"]:checked').val();
    //debugger;
    $.ajax({
        url: '/Cuenta/MarcarComoPredeterminada',
        type: 'GET',
        data: {
            usuarioId: UsuarioId,
            direccionId: direccion
        },
        success: function (response) {
            //debugger;
            if (response.success) {
                Swal.fire({
                    title: '¡Guardado!',
                    text: 'Información actualizada correctamente.',
                    width: '400px',
                    customClass: {
                        popup: 'swal-small-modal'
                    },
                    confirmButtonText: 'Entendido',
                    confirmButtonColor: '#FF8800',
                }).then(() => {
                    location.reload();
                });
            } else {
                Swal.fire({
                    icon: 'info',
                    title: 'Sin direcciones',
                    text: 'Este usuario no tiene direcciones registradas.'
                });
            }
        },
        error: function (xhr, status, error) {
            console.error('Error en la solicitud:', error);
            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: 'Ocurrió un error al obtener las direcciones.'
            });
        }
    });
});