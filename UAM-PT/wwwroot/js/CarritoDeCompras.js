$(document).on('click', '.btn-pago', function () {
    window.location.href = "/Carrito/Checkout";
});

$(document).on('click', '.continuar', function () {
    window.location.href = "/Home/Inicio";
});

$(document).off("click", ".btn-quitar").on("click", ".btn-quitar", function () {
    var productoId = $(this).data("id");
    console.log("clic quitar", productoId);

    $.ajax({
        url: '/Carrito/Quitar',
        type: 'POST',
        data: { productoId: productoId },
        success: function (response) {
            if (response.success) {
                location.reload();
            } else {
                alert("No se pudo quitar el producto: " + response.message);
            }
        }
    });
});
