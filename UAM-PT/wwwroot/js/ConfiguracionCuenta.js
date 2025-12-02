var UsuarioId = localStorage.getItem("usuarioId");
$(document).ready(function () {
    $(document).on('click', '#regresar', function () {
        window.location.href = "/Home/Inicio";
    });

    $("#metodoPago").on("change", function () {
        var metodoSeleccionado = $(this).val();
        //debugger;
        // Ocultar todas las secciones
        $(".campos-metodo").addClass("d-none");

        if (metodoSeleccionado) {
            // Mostrar solo las que apliquen (ejemplo: 1 y 2 = tarjetas)
            $(".campos-metodo").each(function () {
                var metodos = $(this).data("metodo").toString().split(",");
                if (metodos.includes(metodoSeleccionado)) {
                    $(this).removeClass("d-none");
                }
            });
        }
    });
    
    traeInfoPersonal(UsuarioId);
    traeDireccionUsuario(UsuarioId);
    cargarMetodoPredeterminado();

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

$("#formAgregarMetodo").on("submit", function (e) {
    e.preventDefault();

    $.ajax({
        url: '/Cuenta/AgregarMetodoPago',
        type: 'POST',
        data: $(this).serialize(),
        success: function (response) {
            if (response.success) {
                alert("✅ Método de pago guardado correctamente.");
                $("#modalAgregarMetodo").modal('hide');
                $("#formAgregarMetodo")[0].reset();
            } else {
                alert("⚠️ " + response.message);
            }
        },
        error: function (xhr, status, error) {
            console.error(error);
            alert("❌ Ocurrió un error al guardar el método de pago.");
        }
    });
});

    $("#btnMisMetodos").on("click", function () {
        // Abrir el modal
        var modal = new bootstrap.Modal(document.getElementById('modalMisMetodos'));
        modal.show();

        // Vaciar lista
        $("#listaMetodosPago").empty();

        // AJAX para traer métodos de pago del usuario
        $.ajax({
            url: '/Cuenta/ObtenerMetodosPago', 
            type: 'GET',
            success: function (response) {
                if (response.length > 0) {
                    response.forEach(function (metodo) {
                        var item = `
                            <li class="list-group-item d-flex justify-content-between align-items-center">
                                ${metodo.descripcion} 
                                <button class="btn btn-sm btn-primary btn-predeterminado" data-id="${metodo.id}">
                                    ${metodo.esPredeterminado ? 'Predeterminado' : 'Elegir'}
                                </button>
                            </li>
                        `;
                        $("#listaMetodosPago").append(item);
                    });
                } else {
                    $("#listaMetodosPago").append('<li class="list-group-item">No tienes métodos de pago</li>');
                }
            },
            error: function () {
                alert("Error al cargar los métodos de pago");
            }
        });
    });

    // Seleccionar método predeterminado
    $(document).on("click", ".btn-predeterminado", function () {
        var metodoId = $(this).data("id");

        $.ajax({
            url: '/Cuenta/EstablecerPredeterminado',
            type: 'POST',
            data: { metodoId: metodoId },
            success: function (response) {
                if (response.success) {
                    alert("✅ Método de pago establecido como predeterminado");

                    // En lugar de cerrar y reabrir el modal, solo recargamos la lista
                    $.ajax({
                        url: '/Cuenta/ObtenerMetodosPago',
                        type: 'GET',
                        success: function (response) {
                            $("#listaMetodosPago").empty();
                            response.forEach(function (metodo) {
                                var item = `
                                <li class="list-group-item d-flex justify-content-between align-items-center">
                                    ${metodo.descripcion} 
                                    <button class="btn btn-sm btn-primary btn-predeterminado" data-id="${metodo.id}">
                                        ${metodo.esPredeterminado ? 'Predeterminado' : 'Elegir'}
                                    </button>
                                </li>
                            `;
                                $("#listaMetodosPago").append(item);
                            });
                        }
                    });

                    // Actualizamos el input del método predeterminado también
                    cargarMetodoPredeterminado();

                } else {
                    alert("⚠️ " + response.message);
                }
            },
            error: function () {
                alert("❌ Error al establecer método predeterminado");
            }
        });
    });


function cargarMetodoPredeterminado() {
    $.ajax({
        url: '/Cuenta/ObtenerMetodoPredeterminado',
        type: 'GET',
        success: function (response) {
            if (response) {
                var texto = "";

                if (response.numeroTarjeta) {
                    // Censurar todos los dígitos excepto los últimos 3
                    var ultimos = response.numeroTarjeta.slice(-3);
                    texto = "**** **** **** " + ultimos;
                } else if (response.cuentaPaypal) {
                    texto = "Paypal: " + response.cuentaPaypal;
                } else if (response.cuentaMercadoPago) {
                    texto = "Mercado Pago: " + response.cuentaMercadoPago;
                } else {
                    texto = "Otro método de pago";
                }

                $("#inputMetodoPredeterminado").val(texto);
            } else {
                $("#inputMetodoPredeterminado").val("Sin métodos de pago");
            }
        },
        error: function () {
            $("#inputMetodoPredeterminado").val("Error al cargar método de pago");
        }
    });
}

$(document).on("click", ".btn-predeterminado", function () {
    setTimeout(cargarMetodoPredeterminado, 500);
});

$("#btnCambiarPassword").on("click", function () {
    var actual = $("#passwordActual").val();
    var nueva = $("#passwordNueva").val();

    if (!actual || !nueva) {
        alert("Debes llenar ambos campos");
        return;
    }

    $.ajax({
        url: '/Cuenta/CambiarPassword',
        type: 'POST',
        data: JSON.stringify({
            PasswordActual: actual,
            PasswordNueva: nueva
        }),
        contentType: "application/json; charset=utf-8",
        success: function (response) {
            if (response.success) {
                alert("✅ Contraseña cambiada correctamente");
                $("#passwordActual, #passwordNueva").val(""); // limpiar inputs
            } else {
                alert("⚠️ " + response.message);
            }
        },
        error: function () {
            alert("❌ Error al cambiar la contraseña");
        }
    });
});


// ----- //
//document.getElementById('identificacion').addEventListener('change', function (event) {
//    let reader = new FileReader();
//    reader.onload = function (e) {
//        let img = document.getElementById('previewIdentificacion');
//        img.src = e.target.result;
//        img.style.display = "block";
//    }
//    reader.readAsDataURL(event.target.files[0]);
//});

//// Previsualización Comprobante
//document.getElementById('comprobante').addEventListener('change', function (event) {
//    let reader = new FileReader();
//    reader.onload = function (e) {
//        let img = document.getElementById('previewComprobante');
//        img.src = e.target.result;
//        img.style.display = "block";
//    }
//    reader.readAsDataURL(event.target.files[0]);
//});

$('#quieroVenderBtn').on('click', function () {
    $('#mdlRegistroVendedor').modal('show');
});


$("#btnRegistrarVendedor").on("click", function (e) {
    e.preventDefault();

    // Crear objeto FormData
    var formData = new FormData();
    formData.append("rfc", $("#rfc").val());
    formData.append("curp", $("#curp").val());
    formData.append("cuentaBancaria", $("#cuentaBancaria").val());
    formData.append("identificacion", $("#identificacion")[0].files[0]);
    formData.append("comprobanteDomicilio", $("#comprobanteDomicilio")[0].files[0]);
    //debugger;
    $.ajax({
        url: '/Cuenta/RegistrarVendedor',
        type: 'POST',
        data: formData,
        processData: false, 
        contentType: false, 
        success: function (response) {
            if (response.success) {
                //debugger;
                alert(response.response_msg);
                // ejemplo: cerrar modal
                $("#mdlRegistroVendedor").modal("hide");
            } else {
                alert("Error: " + response.response_msg);
            }
        },
        error: function (xhr, status, error) {
            console.log(error);
            alert("Error en la petición AJAX");
        }
    });
});

$("#rfc").on("input", function () {
    if ($(this).val().length > 13) {
        $(this).val($(this).val().substring(0, 13));
    }
});