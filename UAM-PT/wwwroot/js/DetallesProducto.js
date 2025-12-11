$(document).ready(function () {
    
});

$(document).on("click", ".btn-add", function () {
    //debugger;
    let btn = $(this);
    var productId = $(this).data("pid");

    btn.prop("disabled", true).addClass("loading");
    let originalText = btn.html();
    btn.html('<span class="spinner-border spinner-border-sm"></span> Agregando...');

    $.ajax({
        url: '/Carrito/Agregar',
        type: 'POST',
        data: { productoId: productId, cantidad: 1 },
        success: function (response) {
            if (response.success) {
                btn.removeClass("loading").addClass("added");
                btn.html('✔ Agregado!');

                setTimeout(() => {
                    btn.prop("disabled", false)
                        .removeClass("added")
                        .html(originalText);
                }, 2000);
            } else {
                btn.prop("disabled", false);
                btn.html(originalText);
                alert("No se pudo agregar al carrito");
            }
        }
    });
});
