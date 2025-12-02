$(document).ready(function () {
    $('.toggle-password').on('click', function () {
        let $password = $('#password');
        let $icon = $('#toggleIcon');
        let type = $password.attr('type') === 'password' ? 'text' : 'password';

        $password.attr('type', type);

        $icon.toggleClass('fa-eye fa-eye-slash');
    });
});

$(document).on('click', '#Ingresar', function (e) {
    e.preventDefault();

    // Mostrar la animación de carga en el botón
    $('#Ingresar').prop('disabled', true);
    $('#Ingresar').html(`
        <span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span>
        Cargando...
    `);

    let usuario = $("#usuario").val();
    let password = $("#password").val();

    $.ajax({
        url: '/Auth/login',
        type: "POST",
        contentType: "application/json",
        dataType: "json",
        data: JSON.stringify({ usuario: usuario, password: password }),
        success: function (response) {
            //debugger;
            console.log("Respuesta del servidor:", response);
            if (response.token) {
                localStorage.setItem("token", response.token);
                localStorage.setItem("usuarioId", response.usuarioId);
                localStorage.setItem("vendedorId", response.vendedorId);
                
                window.location.href = "/Home/Inicio";
            } else {
                alert(response.message);
            }
        },
        error: function () {
            alert("Ocurrió un error en el servidor");
        },
        complete: function () {
            $('#Ingresar').prop('disabled', false);
            $('#Ingresar').html('Ingresar');
        }
    });

  
});

$(document).on('click', '#Registrar', function () {
    window.location.href = "/Auth/Registro";
});
//$(document).on('click', '#Registrar', function () {
//    window.location.href = "/Account/RecuperarContrasenia";
//});

$(document).on('click', '#Regresar', function () {
    window.location.href = "/Auth/InicioSesion";
});

$(document).on('click', '#registro', function (e) {
    //debugger;
    e.preventDefault();
    let nombre = $("#NombreUsuario").val();
    let apellidop = $("#ApaternoUsuario").val();
    let apellidom = $("#AmaternoUsuario").val();
    let correo = $("#correoUsuario").val();
    let contrasenia = $("#pass").val();
    let telefono = $("#telefono").val();

    if (!nombre || !apellidop || !correo || !contrasenia || !telefono) {
        alert("Por favor, complete todos los campos obligatorios.");
        return; 
    }

    $.ajax({
        url: '/Auth/RegistrarUsuario',
        type: "POST",
        contentType: "application/json",
        dataType: "json",
        data: JSON.stringify({
            nombre: nombre,
            aPaterno: apellidop,
            aMaterno: apellidom,
            correo: correo,
            pass: contrasenia,
            telefono: telefono
        }),
        success: function (response) {
            //debugger;
            console.log("Respuesta del servidor:", response);
            if (response.success) {
                //debugger;
                alert("Registro exitoso");
                window.location.href = "/Auth/InicioSesion";
            } else {
                alert(response.message);
            }
        },
        error: function () {
            alert("Ocurrió un error en el servidor");
        }
    });
});

//$(document).ready(function () {
//    $('#loginForm').on('submit', function (e) {
//        e.preventDefault();
//        $('#Ingresar').prop('disabled', true);
//        $('#Ingresar span').removeClass('d-none');
//        $('#Ingresar').text('Cargando...');
//        setTimeout(function () {
//            $('#loginForm').off('submit').submit();
//        }, 2000);
//    });
//});


document.getElementById("loginGoogle").addEventListener("click", function () {

    const width = 520, height = 600;
    const left = (screen.width / 2) - (width / 2);
    const top = (screen.height / 2) - (height / 2);

    window.open('/Auth/GoogleLogin', 'googleSim', `width=${width},height=${height},top=${top},left=${left}`);

    window.addEventListener("message", function (e) {
        if (!e.data || e.data.type !== "google_sim") return;

        const token = e.data.token;

        fetch("/Auth/GoogleSimCallback", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ token })
        })
            .then(r => r.text())
            .then(txt => { console.log(txt) })
    });
});



/// Registro
$(function () {

    // --- Helpers ---
    function showError(id) {
        $('#' + id).fadeIn(120);
    }
    function hideError(id) {
        $('#' + id).fadeOut(120);
    }

    // Toggle password (usa mismo id #togglePass y #pass)
    $('#togglePass').off('click').on('click', function () {
        var $input = $('#pass');
        if ($input.attr('type') === 'password') {
            $input.attr('type', 'text');
            $(this).css('color', '#FF8800');
        } else {
            $input.attr('type', 'password');
            $(this).css('color', '#777');
        }
    });

    // Form submit (validación + animación)
    var $form = $('#RegistroForm');
    var $btn = $('#registro');

    $form.off('submit').on('submit', function (e) {
        e.preventDefault();

        // esconder todos errores primero
        hideError('errNombre'); hideError('errApaterno'); hideError('errCorreo');
        hideError('errPass'); hideError('errTel');

        var valido = true;

        var nombre = $('#NombreUsuario').val() || '';
        var ap = $('#ApaternoUsuario').val() || '';
        var correo = $('#correoUsuario').val() || '';
        var pass = $('#pass').val() || '';
        var tel = $('#telefono').val() || '';

        if (nombre.trim().length < 2) { showError('errNombre'); valido = false; }
        if (ap.trim().length < 2) { showError('errApaterno'); valido = false; }

        var regexCorreo = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        if (!regexCorreo.test(correo)) { showError('errCorreo'); valido = false; }

        if (pass.length < 6) { showError('errPass'); valido = false; }

        if (!/^\d{10}$/.test(tel)) { showError('errTel'); valido = false; }

        if (!valido) {
            // opcional: llevar foco al primer error
            var primero = $('.text-error:visible').first();
            if (primero.length) {
                $('html,body').animate({ scrollTop: primero.offset().top - 120 }, 200);
            }
            return;
        }

        // Animación del botón: deshabilitar y mostrar spinner (Bootstrap spinner)
        $btn.prop('disabled', true);
        var originalHtml = $btn.html();
        $btn.html('<span class="spinner-border spinner-border-sm text-white" role="status" aria-hidden="true"></span> Procesando...');

        // Si tu flujo actual hace AJAX aquí, reemplaza el setTimeout por la llamada AJAX.
        // Si quieres enviar el form normal, desregistramos este submit y hacemos submit nativo.
        setTimeout(function () {
            // restaurar (por si la llamada falla, aunque aquí estamos enviando)
            $btn.prop('disabled', false);
            $btn.html(originalHtml);

            // evitar rebinding: quitar handler y hacer submit nativo
            $form.off('submit');
            $form.submit();

        }, 900); // 900ms de animación antes de enviar (ajusta si quieres)
    });

});