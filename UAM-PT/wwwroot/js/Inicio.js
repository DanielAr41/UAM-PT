$(document).ready(function () {
    traeProductos();
});

function traeProductos() {
    $.ajax({
        url: "/Home/ObtenProductos",
        type: "GET",
        dataType: "json",
        success: function (data) {
            //debugger;
            const container = $("#productosContainer");

            if (data.length === 0) {
                container.html("<p>No hay productos disponibles.</p>");
                return;
            }

            data.forEach(function (producto) {
                const card = `
                    <div class="producto-card card-hover-novamer" data-pid="${producto.id}">
                        <img src="${producto.imagenUrl}" alt="${producto.nombre}">
                        <h3 class="product-title">${producto.nombre}</h3>
                        <p class="product-price">$${producto.precio.toFixed(2)}</p>
                    </div>
                `;
                container.append(card);
            });

        },
        error: function () {
            $("#productosContainer").html("<p>Error al cargar productos.</p>");
        }
    });
}
$(document).on('click', '#vender', function () {
    window.location.href = "/Vender/PerfilVendedor";
});

$('#CerrarSesion').on('click', function () {
    localStorage.removeItem('token');
    localStorage.removeItem('usuarioId');
    localStorage.removeItem('vendedorId');
    window.location.href = '/Auth/InicioSesion';
});


$(document).on('click', '.carrito', function () {
    window.location.href = "/Carrito/CarritoDeCompras";
});

$(document).on('click', '#MisPedidos', function () {
    window.location.href = "/Pedidos/MisPedidos";
});

$(document).on("click", ".producto-card", function () {
    var productId = $(this).data("pid");
    window.location.href = '/Producto/VerProducto?id=' + productId;
});


