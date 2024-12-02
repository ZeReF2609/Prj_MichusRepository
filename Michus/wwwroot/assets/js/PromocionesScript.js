
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




function enviarCorreo() {
    // Obtener el correo del destinatario desde el formulario o el contexto
    let destinatario = 'josejuliosanchezcruzado1@gmail.com';  

    fetch(`/Promociones/EnviarTokenPorPromocion?destinatario=${encodeURIComponent(destinatario)}`, {
        method: 'GET',
    })
        .then(response => response.json())
        .then(data => {
            // Mostrar el mensaje en el modal o notificación
            document.getElementById('mensajeModal').innerText = data.message || "Correo enviado correctamente.";
            $('#exampleModalCenter').modal('show');
        })
        .catch(error => {
            console.error('Error al enviar el correo:', error);
            alert('Hubo un problema al enviar el correo.');
        });
}



function validarToken() {
    var promoId = $('#promoIdInput').val();  
    var token = $('#tokenInput').val(); 

    // Validar que el token no esté vacío
    if (!token) {
        alert('Por favor ingresa un token');
        return;
    }

    // Hacer la solicitud AJAX al backend
    $.ajax({
        url: '/Promociones/ValidarToken',  
        type: 'POST',
        data: JSON.stringify({
            Token: token,
            PromoId: promoId 
        }),
        contentType: 'application/json',
        success: function (response) {
            if (response.success) {
                $('#mensajeModal').text('Token válido. ¡Promoción activada!');
                $('#exampleModalCenter').modal('hide');
            } else {
                $('#mensajeModal').text('Token inválido o expirado.');
            }
        },
        error: function (xhr, status, error) {
            $('#mensajeModal').text('Hubo un error al validar el token.');
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