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
    debugger;
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
        //url: `/Auth/RegistrarUsuario?nombre=${nombre}&aPaterno=${apellidop}&aMaterno=${apellidom}&correo=${correo}&pass=${contrasenia}&telefono=${telefono}`,
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
