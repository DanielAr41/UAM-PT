var UsuarioIdLog = localStorage.getItem("usuarioId");
direccionIdUsuario = null;
$(document).ready(function () {
    traeInfoPersonal(UsuarioIdLog);
    traeDireccionUsuario(UsuarioIdLog);
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


function traeDireccionUsuario(uid) {
    $.ajax({
        url: '/Cuenta/CargaDireccion',
        type: 'GET',
        data: { usuarioId: uid },
        success: function (data) {
            if (data != null && data.data) {
                let calle = data.data.calle || '';
                let numero = data.data.numero || '';
                let localidad = data.data.localidad || '';
                let estado = data.data.estado || '';
                let codigoPostal = data.data.codigoPostal || '';
                let referencia = data.data.referencias || '';
                direccionIdUsuario = data.data.id;
                let domicilioCompleto = `📍 ${calle} #${numero}, ${localidad}, ${estado}, CP ${codigoPostal}`;
                $('#Referencias').val(referencia);

                $('#Domicilio').val(domicilioCompleto);
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


$(document).on("click", "#ConfirmarCompra", function () {
    $(this).prop("disabled", true);

    $.ajax({
        url: '/Carrito/FinalizarCompra',
        type: 'POST',
        data: { direccionId: direccionIdUsuario },
        success: function (response) {
            if (response.success) {
                window.location.href = '/Pedidos/MisPedidos';
            } else {
                alert("Error al finalizar la compra: " + response.message);
                $("#ConfirmarCompra").prop("disabled", false);
            }
        },
        error: function (xhr, status, error) {
            console.error("Error en la solicitud:", error);
            alert("Ocurrió un error al procesar la compra.");
            $("#ConfirmarCompra").prop("disabled", false);
        }
    });
});
