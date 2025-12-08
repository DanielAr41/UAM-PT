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

    $(".tabs a").on("click", function (e) {
        e.preventDefault();

        $(".tabs a").removeClass("active");
        $(this).addClass("active");
    });


    $("#tab-todos").on("click", function () {
        $("#contenedor-productos").show();
        $("#contenedor-inactivos").hide();
    });

    $("#tab-inactivos").on("click", function () {

        $("#contenedor-productos").hide();
        $("#contenedor-inactivos").show();

        cargaProductosInactivos(UsuarioIdLog);
    });

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
                //debugger;
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
            //debugger;
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

            productosMostrados = 0;
            $('.btn-vermas').show();

            productos.forEach(producto => {
                const card = `
                                <div class="product-card">
                                    <div class="product-image">
                                        <img src="${producto.imagenUrl}" alt="${producto.nombre}" />

                                        <!-- Botón editar -->
                                        <button class="edit-btn" data-rel="${producto.id}" onclick="abrirModalEdicion(this)">✏️</button>

                                        <!-- Botón inactivar -->
                                        <button class="inactive-btn" data-id="${producto.id}" onclick="inactivarProducto(this)">🛑</button>
                                    </div>

                                    <h3>${producto.nombre}</h3>
                                    <p>${producto.descripcion || ''}</p>

                                    <div class="info">
                                        <span><strong>Precio:</strong> $${producto.precio}</span>
                                        <span class="rating">⭐ ${producto.stock}</span>
                                    </div>
                                </div>
                            `;

                contenedor.append($(card).hide());
            });
            mostrarMasProductos();

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


// ----- VER MÁS -----
let productosMostrados = 0;
const productosPorPagina = 6;

$(document).on('click', '.btn-vermas', function () {
    mostrarMasProductos();
});

function mostrarMasProductos() {
    const cards = $('#contenedor-productos .product-card');

    for (let i = productosMostrados; i < productosMostrados + productosPorPagina && i < cards.length; i++) {
        $(cards[i]).show();
    }

    productosMostrados += productosPorPagina;

    if (productosMostrados >= cards.length) {
        $('.btn-vermas').hide();
    }
}


function inactivarProducto(btn) {
    const idProducto = $(btn).data("id");

    Swal.fire({
        title: "¿Inactivar producto?",
        text: "El producto ya no será visible para los compradores.",
        icon: "warning",
        showCancelButton: true,
        confirmButtonText: "Sí, inactivar",
        cancelButtonText: "Cancelar",
        confirmButtonColor: "#d33",
        cancelButtonColor: "#6c757d"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: '/Vender/InactivarProducto',
                type: 'POST',
                data: { productoId: idProducto },
                success: function () {
                    Swal.fire({
                        title: "Producto inactivado",
                        text: "El producto se ha inactivado correctamente.",
                        icon: "success",
                        showConfirmButton: true
                    });

                   
                    cargaProductos(UsuarioIdLog);
                },
                error: function (err) {
                    console.error(err);
                    Swal.fire({
                        title: "Error",
                        text: "Ocurrió un error al inactivar el producto.",
                        icon: "error"
                    });
                }
            });
        }
    });
}


function cargaProductosInactivos(usuarioID) {
    $.ajax({
        url: '/Vender/ObtenProductosInactivos',
        type: 'GET',
        data: { usuarioId: usuarioID },
        success: function (productos) {

            const contenedor = $('#contenedor-inactivos');
            contenedor.empty();

            if (productos.length === 0) {
                contenedor.html('<p>No tienes productos inactivos.</p>');
                return;
            }
            //debugger;
            productos.forEach(producto => {
                const card = `
                    <div class="product-card">
                        <div class="product-image">
                            <img src="${producto.imagenUrl}" alt="${producto.nombre}" />

                            <!-- Botón activar -->
                            <button class="activate-btn" data-id="${producto.id}" onclick="activarProducto(this)">
                                <i class="bi bi-unlock"></i>
                            </button>

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
            console.error('Error al obtener productos inactivos:', err);
        }
    });
}


$("#tab-inactivos").on("click", function () {

    $(".tabs a").removeClass("active");
    $(this).addClass("active");

    cargaProductosInactivos(UsuarioIdLog);
});

$("#tab-todos").on("click", function (e) {
    e.preventDefault();

    $(".tabs a").removeClass("active");
    $(this).addClass("active");

    $("#contenedor-productos").show();
    $("#contenedor-inactivos").hide();

    cargaProductos(UsuarioIdLog);
});


function activarProducto(btn) {
    const id = $(btn).data('id');

    Swal.fire({
        title: "¿Desea activar el producto?",
        text: "El producto volverá a ser visible para los compradores.",
        icon: "warning",
        showCancelButton: true,
        confirmButtonText: "Activar",
        cancelButtonText: "Cancelar",
        confirmButtonColor: "#d33",
        cancelButtonColor: "#6c757d"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: '/Vender/ActivarProducto',
                type: 'POST',
                data: { productoId: id },
                success: function () {
                    Swal.fire({
                        title: "Producto activado",
                        text: "El producto se ha activado correctamente.",
                        icon: "success",
                        showConfirmButton: true
                    });


                    cargaProductosInactivos(UsuarioIdLog);
                },
                error: function (err) {
                    console.error(err);
                    Swal.fire({
                        title: "Error",
                        text: "Ocurrió un error al activar el producto.",
                        icon: "error"
                    });
                }
            });
        }
    });
}


$("#productoImagen").on("change", function () {
    const fileName = this.files.length > 0 ? this.files[0].name : "Selecciona una imagen...";
    $("#file-name").text(fileName);
});

$(".btn-upload-trigger").on("click", function () {
    $("#productoImagen").click();
});

document.getElementById("productoImagen").addEventListener("change", function (e) {
    const file = e.target.files[0];
    const preview = document.getElementById("previewProductoImg");

    if (file) {
        const reader = new FileReader();

        reader.onload = function (event) {
            preview.src = event.target.result;
            preview.classList.remove("d-none");
        }

        reader.readAsDataURL(file);
    } else {
        preview.src = "#";
        preview.classList.add("d-none");
    }
});