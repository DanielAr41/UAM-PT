const nodemailer = require('nodemailer');

function generateCode() {
    return Math.floor(100000 + Math.random() * 900000);
}

async function sendPasswordRecoveryEmail(toEmail) {
    const code = generateCode();

    const message = {
        from: 'danielarereyes55@gmail.com',
        to: toEmail,
        subject: 'Recuperación de Contraseña',
        text: `Tu código de recuperación de contraseña es: ${code}`,
    };

    const transporter = nodemailer.createTransport({
        service: 'gmail',
        auth: {
            type: 'OAuth2',
            user: 'tu-email@gmail.com',
            clientId: client_id,
            clientSecret: client_secret,
            refreshToken: oauth2Client.credentials.refresh_token,
        },
    });

    try {
        const info = await transporter.sendMail(message);
        console.log('Correo enviado: %s', info.messageId);
    } catch (error) {
        console.error('Error al enviar el correo:', error);
    }
}

// Llama a la función pasando el correo del usuario
sendPasswordRecoveryEmail('usuario@dominio.com');
