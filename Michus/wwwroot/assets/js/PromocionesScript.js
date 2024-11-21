
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
            event.preventDefault(); // Prevenir el comportamiento predeterminado del formulario

        // Aquí puedes manejar la validación de la contraseña
        var password = passwordInput.value;
        if (password === "1234") { // Contraseña correcta
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
            ? `S/. ${promocion.Descuento} - Promoción Fija`
            : `% ${promocion.Descuento} - Promoción Porcentual`;

        document.getElementById("descripcionPromocion").innerText = promocion.Descripcion;
    } else {
        // Si no hay selección válida, limpiar los datos
        document.getElementById("descuentoPromocion").innerText = "Seleccione una Promoción";
        document.getElementById("descripcionPromocion").innerText = "Seleccione una Promoción";
    }
}




function enviarCorreo() {
    // Obtener el correo del destinatario desde el formulario o el contexto
    let destinatario = 'josejuliosanchezcruzado1@gmail.com';  // Asegúrate de que este valor se obtenga dinámicamente de la UI si es necesario

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
    var promoId = $('#promoIdInput').val();  // Obtener el ID de la promoción
    var token = $('#tokenInput').val();  // Obtener el valor del token

    // Validar que el token no esté vacío
    if (!token) {
        alert('Por favor ingresa un token');
        return;
    }

    // Hacer la solicitud AJAX al backend
    $.ajax({
        url: '/Promociones/ValidarToken',  // Cambia la URL según tu ruta
        type: 'POST',
        data: JSON.stringify({
            Token: token,
            PromoId: promoId  // Incluir el ID de la promoción en la solicitud
        }),
        contentType: 'application/json',
        success: function (response) {
            if (response.success) {
                $('#mensajeModal').text('Token válido. ¡Promoción activada!');
                // Puedes cerrar el modal aquí
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



// SECCION DE  SELECCION 
// Evento que captura los productos seleccionados
function updateSelectedProducts() {
    // Obtenemos los checkboxes seleccionados
    var selectedProducts = [];
    $(".productoCheckbox:checked").each(function () {
        selectedProducts.push($(this).data("producto-id"));
    });

    // Convertimos los IDs seleccionados a formato JSON
    $("#productosSeleccionados").val(JSON.stringify(selectedProducts));
}

// Vinculamos el evento change al checkbox para que actualice el campo oculto
$(".productoCheckbox").on("change", updateSelectedProducts);

//Actualizado a la fecha 21/11/2024
