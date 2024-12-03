
    // Obtiene el modal
    var modal = document.getElementById('exampleModalCenter');

    // Obtiene el botón que abre el modal
    var btn = document.querySelector('[data-toggle="modal"]');

    // Obtiene el <span> que cierra el modal
        var closeBtn = document.querySelector('.close-btn');

        // Función para abrir el modal
        function openModal() {
            modal.style.display = "block";
    }

        // Función para cerrar el modal
        function closeModal() {
            modal.style.display = "none";
    }

        // Cuando el usuario hace clic en el botón de apertura del modal
        btn.addEventListener('click', openModal);

        // Cuando el usuario hace clic en el botón de cerrar el modal
        closeBtn.addEventListener('click', closeModal);

        // Cuando el usuario hace clic fuera del modal, este se cierra
        window.addEventListener('click', function(event) {
        if (event.target == modal) {
            closeModal();
        }
    });

        // Función para manejar el envío del formulario (opcional)
        var form = document.querySelector('form');
        var passwordInput = form.querySelector('input[type="password"]');
        var submitBtn = form.querySelector('.btn');

        form.addEventListener('submit', function(event) {
            event.preventDefault(); 

        // Aquí puedes manejar la validación de la contraseña
        var password = passwordInput.value;
        if (password === "1234") { 
            alert('Contraseña correcta');
        closeModal();
        } else {
            alert('Contraseña incorrecta');
        }
        });




function updatePromotionDetails() {
    const select = document.getElementById("idPromocion");
    const selectedId = select.value;

    // Acceder a la variable global "promociones"
    const promocion = window.promociones.find(promo => promo.IdPromociones == selectedId);

    if (promocion) {
        // Actualizar los datos en la vista
        document.getElementById("descuentoPromocion").innerText = promocion.TipoPromocion == 1
            ?`% ${promocion.Descuento} - Promoción Porcentual`
            :`S/. ${promocion.Descuento} - Promoción Fija`;

        document.getElementById("descripcionPromocion").innerText = promocion.Descripcion;
    } else {
        // Si no hay selección válida, limpiar los datos
        document.getElementById("descuentoPromocion").innerText = "Seleccione una Promoción";
        document.getElementById("descripcionPromocion").innerText = "Seleccione una Promoción";
    }
}



// Mostrar el modal para enviar el token
function mostrarModalEnviarToken() {
    $('#enviarTokenModal').modal('show');
}

// Enviar el token por correo cuando se hace clic en el botón "Enviar Token"
$('#enviarTokenBtn').click(function () {
    var correoDestino = $('#correoDestino').val();

    if (correoDestino) {
        // Realizar la llamada AJAX para enviar el correo
        $.ajax({
            url: '/Promociones/EnviarTokenPorPromocion',
            type: 'GET',
            data: { destinatario: correoDestino },
            success: function (response) {
                alert('Correo enviado exitosamente con el token.');
                $('#enviarTokenModal').modal('hide');
            },
            error: function (xhr, status, error) {
                var mensaje = xhr.responseText ? xhr.responseText : 'Hubo un error al enviar el correo.';
                alert(mensaje);
                $('#mensajeError').show();
            }
        });
    } else {
        $('#mensajeError').show();
    }
});


// Mostrar el modal para validar el token
// Variable global para rastrear si el token es válido
let tokenValido = false;

// Función para mostrar el modal para validar el token
function mostrarModalValidarToken() {
    $('#validarTokenModal').modal('show');
}

// Función para validar el token
$('#validarTokenBtn').click(function () {
    var tokenIngresado = $('#tokenIngresado').val();

    if (tokenIngresado) {
        $.ajax({
            url: '/Promociones/ValidarToken',
            type: 'POST',
            data: { token: tokenIngresado },
            success: function (response) {
                if (response.valido) {
                    alert('Token válido. Ahora puedes editar el estado.');
                    tokenValido = true; // Actualizar el estado del token
                    $('#validarTokenModal').modal('hide');
                } else {
                    $('#mensajeErrorToken').show(); // Mostrar mensaje de error si el token no es válido
                }
            },
            error: function (xhr, status, error) {
                var mensaje = xhr.responseText ? xhr.responseText : 'Hubo un error al validar el token.';
                alert(mensaje);
                $('#mensajeErrorToken').show(); // Mostrar mensaje de error
            }
        });
    } else {
        $('#mensajeErrorToken').show(); // Mostrar mensaje si no hay token ingresado
    }
});

// Función para validar el token antes de mostrar el modal de actualización de estado
function validarYMostrarModal(idPromocion) {
    if (!tokenValido) {
        alert('Debes validar el token antes de editar el estado.');
        mostrarModalValidarToken(); // Mostrar el modal de validación de token
    } else {
        mostrarModal(idPromocion); // Si el token es válido, mostrar el modal de actualización
    }
}

// Mostrar el modal para actualizar el estado
function mostrarModal(idPromocion) {
    $('#promoIdInput').val(idPromocion); // Asignar el ID de la promoción al input oculto
    $('#modalEstado').modal('show'); // Mostrar el modal
}

// Función para actualizar el estado de la promoción
function actualizarEstadoPromo() {
    var idPromocion = $('#promoIdInput').val(); // Obtener el ID de la promoción

    $.ajax({
        url: '/Promociones/ActualizarEstadoPromo', // Ruta del método en el backend
        type: 'POST',
        data: { idPromocion: idPromocion },
        success: function () {
            alert('Estado de la promoción actualizado correctamente.');
            $('#modalEstado').modal('hide'); // Ocultar el modal
            location.reload(); // Recargar la página
        },
        error: function (xhr, status, error) {
            console.error('Error:', error);
            alert('Ocurrió un error al actualizar el estado.');
        }
    });
}



//AGREGAR PROMOCIONES A PRODUCTOS SELECCUONADOS
function updateSelectedProducts() {
    // Obtener todos los checkboxes seleccionados
    var selectedCheckboxes = document.querySelectorAll('.productoCheckbox:checked');

    // Crear un array con los valores de los productos seleccionados (IDs)
    var selectedProductIds = [];
    selectedCheckboxes.forEach(function (checkbox) {
        selectedProductIds.push(checkbox.value);
    });

    // Actualizar el campo de productos seleccionados con el array en formato JSON
    document.getElementById('selectedProducts').value = JSON.stringify(selectedProductIds);
}

// Añadir un listener de evento a todos los checkboxes para que se ejecute cuando el estado cambie
document.querySelectorAll('.productoCheckbox').forEach(function (checkbox) {
    checkbox.addEventListener('change', updateSelectedProducts);
});




// FUNCION PARA EXTRAER EL ID DE LA PROMOCION
function checkPromocionValue() {
    const idPromocion = document.getElementById('idPromocionInput').value; // Asegúrate de tener este input
    if (!idPromocion) {
        alert("Por favor, seleccione una promoción.");
        return false;  // Evitar el envío si no se seleccionó una promoción
    }
    console.log("ID de promoción:", idPromocion); // Verificar que se está enviando
    return true;  // Permitir el envío si todo está correcto
}




//VALIDACION PARA EL ENVIO DEL FORMULARIO AL APLICAR LA PROMOCION
document.getElementById('formRegistrarDetalle').addEventListener('submit', function (event) {
    // Verificar si al menos un checkbox está seleccionado
    var checkboxes = document.querySelectorAll('.productoCheckbox');
    var isChecked = false;

    checkboxes.forEach(function (checkbox) {
        if (checkbox.checked) {
            isChecked = true;
        }
    });

    // Si no se seleccionó ningún checkbox, evitar el envío del formulario
    if (!isChecked) {
        event.preventDefault(); // Evitar el envío del formulario
        alert('Por favor, seleccione al menos un producto.');
    }
});

//ACTUALIZADO A LA FECHA 02-12-2024