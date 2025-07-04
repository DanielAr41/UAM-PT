var UsuarioIdLog = localStorage.getItem("usuarioId");
$(document).ready(function () {
    $('#AddProducto').on('click', function () {
        $('#mdlAddProducto').modal('show');
        llenarCategorias();
    });

    $('#formAddProducto').on('submit', function (e) {
        e.preventDefault(); 
        AgregaProducto();
    });

    $(document).on('click', '#regresar', function () {
        window.location.href = "/Home/Inicio";
    });

    cargaProductos(UsuarioIdLog);
});

function AgregaProducto() {
    debugger;

        var form = $('#formAddProducto')[0];
        var formData = new FormData(form);
        var vendedorId = localStorage.getItem('vendedorId');
        formData.append('VendedorID', vendedorId); 

        $.ajax({
            url: '/Vender/AgregarProducto',
            type: 'POST',
            data: formData,
            contentType: false,
            processData: false, 
            success: function (response) {
                debugger;
                if (response.success) {
                    alert('Producto guardado correctamente.');
                    $('#mdlAddProducto').modal('hide');
                    $('#formAddProducto')[0].reset();
                } else {
                    alert('Error al guardar el producto: ' + response.response_msg);
                }
            },
            error: function (xhr, status, error) {
                debugger;
                console.error('Error en la solicitud AJAX:', error);
                alert('Ocurrió un error inesperado.');
            }
        });
}

function llenarCategorias() {
    $.ajax({
        url: '/Vender/TraeCategorias',
        type: 'GET',
        success: function (data) {
            debugger;
            if (data != null) {
                var select = $('#productoCategoria');
                select.empty();

                select.append('<option value="" selected>Selecciona una categoría</option>');
                data.forEach(function (categoria) {
                    select.append(`<option value="${categoria.id}">${categoria.nombre}</option>`);
                });
            } 
        },
        error: function (xhr, status, error) {
            console.error('Error en la solicitud AJAX:', error);
            alert('Ocurrió un error inesperado.');
        }
    });

}



function cargaProductos(usuarioID) {
    $.ajax({
        url: '/Vender/ObtenProductosPorUsuarioID',
        type: 'GET',
        data: { usuarioId: usuarioID },
        success: function (productos) {
            const contenedor = $('#contenedor-productos');
            contenedor.empty();
            productos.forEach(producto => {
                const card = `
                        <div class="product-card">
                            <div class="product-image">
                                <img src="${producto.imagenUrl}" alt="${producto.nombre}" />
                                <button class="edit-btn" data-rel="${producto.id}" onclick="abrirModalEdicion(this)">✏️</button>
                            </div>
                            <h3>${producto.nombre}</h3>
                            <p>${producto.descripcion || ''}</p>
                            <div class="info">
                                <span><strong>Precio:</strong> $${producto.precio}</span>
                                <span class="rating">⭐ ${producto.stock}</span>
                            </div>
                        </div>
                    `;
                contenedor.append(card);
            });
        },
        error: function (err) {
            console.error('Error al obtener productos:', err);
        }
    });
}

function abrirModalEdicion(btn) {
    const productoId = $(btn).data('rel');

    $.ajax({
        url: '/Vender/ObtenerProductoPorId',
        type: 'GET',
        data: { id: productoId },
        success: function (producto) {
            $('#productoId').val(producto.id);
            $('#nombreProducto').val(producto.nombre);
            $('#descripcionProducto').val(producto.descripcion);
            $('#precioProducto').val(producto.precio);
            $('#stockProducto').val(producto.stock);
            $('#pesoProducto').val(producto.peso);
            $('#vistaPreviaImagen').attr('src', producto.imagenUrl);

            const modal = new bootstrap.Modal(document.getElementById('modalEditarProducto'));
            modal.show();
        },
        error: function (err) {
            console.error('Error al obtener el producto:', err);
            alert('Error al cargar datos del producto');
        }
    });
}


$('#formEditarProducto').on('submit', function (e) {
    e.preventDefault();

    const formData = new FormData(this);

    $.ajax({
        url: '/Vender/EditarProductoporId',
        type: 'POST',
        data: formData,
        contentType: false,
        processData: false,
        success: function (res) {
            if (res.success) {
                alert(res.message);
                $('#modalEditarProducto').modal('hide');

                    location.reload();
            } else {
                alert('Error: ' + res.message);
            }
        },
        error: function () {
            alert('Error al guardar el producto');
        }
    });
});
