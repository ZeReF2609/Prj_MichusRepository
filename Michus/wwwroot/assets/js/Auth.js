// Auth.js

async function handleLogin() {
    const email = document.getElementById('login-email').value;
    const password = document.getElementById('login-password').value;
    const errorDiv = document.getElementById('login-error');
    const successDiv = document.getElementById('login-success');

    try {
        const response = await axios.post('/Login/Login', {
            email: email,
            password: password
        });

        if (response.data.success) {
            successDiv.innerText = 'Inicio de sesión exitoso.';
            successDiv.style.display = 'block';
            errorDiv.style.display = 'none';

            // Redirigir al menú (cambiar la URL según sea necesario)
            window.location.href = '/Menu'; // Cambia esto por tu URL del menú
        } else {
            errorDiv.innerText = response.data.message;
            errorDiv.style.display = 'block';
            successDiv.style.display = 'none';
        }
    } catch (error) {
        console.error('Error en el inicio de sesión:', error);
        errorDiv.innerText = 'Error en el inicio de sesión. Intente nuevamente.';
        errorDiv.style.display = 'block';
        successDiv.style.display = 'none';
    }
}

async function handleRegister() {
    const name = document.getElementById('register-name').value;
    const lastname = document.getElementById('register-lastname').value;
    const email = document.getElementById('register-email').value;
    const password = document.getElementById('register-password').value;

    try {
        const response = await axios.post('/Register', {
            name: name,
            lastname: lastname,
            email: email,
            password: password
        });

        if (response.data.success) {
            alert('Registro exitoso. Ahora puede iniciar sesión.');
        } else {
            alert('Error al registrarse: ' + response.data.message);
        }
    } catch (error) {
        console.error('Error en el registro:', error);
        alert('Error en el registro. Intente nuevamente.');
    }
}
