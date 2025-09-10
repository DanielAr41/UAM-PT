$(document).ready(function () {
    
});

$(document).on("click", ".btn-add", function () {
    //debugger;
    var productId = $(this).data("pid");

    $.ajax({
        url: '/Carrito/Agregar',
        type: 'POST',
        data: { productoId: productId, cantidad: 1 },
        success: function (response) {
            if (response.success) {
                alert(response.message);
            } else {
                alert("No se pudo agregar al carrito");
            }
        }
    });
});
