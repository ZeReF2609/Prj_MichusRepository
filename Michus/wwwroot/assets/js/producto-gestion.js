// Función para abrir el modal en modo de creación de producto
function abrirModalNuevoProducto() {
    // Limpia el formulario antes de abrir el modal
    document.getElementById('productoForm').reset();
    document.getElementById('productoModalLabel').innerText = "Nuevo Producto";
    document.getElementById('ID_PRODUCTO').disabled = false; // Habilita el campo de ID
}

// Función para abrir el modal en modo de edición de producto
function editarProducto(id) {
    // Cambia el título del modal
    document.getElementById('productoModalLabel').innerText = "Editar Producto";
    document.getElementById('ID_PRODUCTO').disabled = true; // Desactiva el campo de ID

    // Llamada AJAX para obtener el producto por ID
    fetch(`/Logistica/ObtenerProductoPorId/${id}`)
        .then(response => response.json())
        .then(data => {
            // Rellena el formulario con los datos del producto
            document.getElementById('ID_PRODUCTO').value = data.idProducto;
            document.getElementById('PROD_NOM').value = data.prodNom;
            document.getElementById('PROD_NOMWEB').value = data.prodNomweb;
            document.getElementById('DESCRIPCION').value = data.descripcion;
            document.getElementById('ID_CATEGORIA').value = data.idCategoria;
            document.getElementById('ProdFchcmrl').value = data.prodFchcmrl ? data.prodFchcmrl.split("T")[0] : ""; // Formatea la fecha
            document.getElementById('PRECIO').value = data.precio;
            document.getElementById('ESTADO').value = data.estado;

            // Abre el modal
            const modal = new bootstrap.Modal(document.getElementById('productoModal'));
            modal.show();
        })
        .catch(error => console.error('Error al obtener el producto:', error));
}

// Función para guardar un producto (crear o actualizar)
function guardarProducto() {
    const idProducto = document.getElementById('ID_PRODUCTO').value;
    const prodNom = document.getElementById('PROD_NOM').value;
    const prodNomweb = document.getElementById('PROD_NOMWEB').value;
    const descripcion = document.getElementById('DESCRIPCION').value;
    const idCategoria = document.getElementById('ID_CATEGORIA').value;
    const prodFchcmrl = document.getElementById('ProdFchcmrl').value;
    const precio = parseFloat(document.getElementById('PRECIO').value);
    const estado = parseInt(document.getElementById('ESTADO').value);

    const producto = {
        idProducto,
        prodNom,
        prodNomweb,
        descripcion,
        idCategoria,
        prodFchcmrl,
        precio,
        estado
    };

    const url = idProducto ? `/Logistica/ActualizarProducto` : `/Logistica/InsertarProducto`;
    const metodo = idProducto ? 'PUT' : 'POST';

    fetch(url, {
        method: metodo,
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(producto)
    })
        .then(response => {
            if (response.ok) {
                alert('Producto guardado correctamente.');
                location.reload(); // Recarga la página para ver los cambios
            } else {
                alert('Error al guardar el producto.');
            }
        })
        .catch(error => console.error('Error al guardar el producto:', error));
}

// Función para desactivar un producto
function desactivarProducto(id) {
    if (confirm("¿Estás seguro de que deseas desactivar este producto?")) {
        fetch(`/Logistica/DesactivarProducto/${id}`, {
            method: 'PUT'
        })
            .then(response => {
                if (response.ok) {
                    alert('Producto desactivado correctamente.');
                    location.reload(); // Recarga la página para ver los cambios
                } else {
                    alert('Error al desactivar el producto.');
                }
            })
            .catch(error => console.error('Error al desactivar el producto:', error));
    }
}

// Evento para abrir el modal en modo de creación
document.querySelector('[data-bs-target="#productoModal"]').addEventListener('click', abrirModalNuevoProducto);
