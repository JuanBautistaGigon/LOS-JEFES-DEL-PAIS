// Script específico para index.html
// Funciones para la página de inicio

// Variables globales OPCIONALES
let partidaActual = null;
let jugadoresRegistrados = []; 
let numeroPartidaActual = null;
let detallesJugadores = [];
const API_BASE_URL = 'http://localhost:5070';

// Funciones de utilidad
function mostrarError(mensaje) {
    alert(mensaje);
}

function limpiarErrores() {
    const errores = document.querySelectorAll('.error-message');
    errores.forEach(error => {
        error.textContent = '';
        error.style.display = 'none';
    });
}

function mostrarErrorCampo(campoId, mensaje) {
    const errorElement = document.getElementById(campoId);
    if (errorElement) {
        errorElement.textContent = mensaje;
        errorElement.style.display = 'block';
    }
}

function validarEmail(email) {
    const regex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return regex.test(email);
}

// Función para cargar jugadores registrados
async function cargarJugadoresRegistrados() {
    try {
        const response = await fetch(`${API_BASE_URL}/Usuario`);
        
        if (response.ok) {
            const jugadores = await response.json();
            
            jugadoresRegistrados = Array.isArray(jugadores.data) ? jugadores.data : [];
            return jugadoresRegistrados;
        } else {
            console.error('Error al cargar jugadores');
            jugadoresRegistrados = [];
            return [];
        }
    } catch (error) {
        console.error('Error al cargar jugadores:', error);
        jugadoresRegistrados = [];
        return [];
    }
}


// Inicialización cuando el DOM está listo
document.addEventListener("DOMContentLoaded", function(event) {
    console.log("Página de inicio cargada correctamente");
    inicializarInicio();
});

// Funciones para la página de inicio
function inicializarInicio() {
    const formNuevaPartida = document.getElementById('formNuevaPartida');
    const formContinuarPartida = document.getElementById('formContinuarPartida');
    const btnGestionarJugadores = document.getElementById('btnGestionarJugadores');
    
    if (formNuevaPartida) {
        formNuevaPartida.addEventListener('submit', function(event) {
            event.preventDefault();
            crearNuevaPartida();
        });
    }
    
    if (formContinuarPartida) {
        formContinuarPartida.addEventListener('submit', function(event) {
            event.preventDefault();
            continuarPartida();
        });
    }

    
    // Cargar jugadores registrados
    cargarJugadoresRegistrados().then(() => {
        actualizarSelectoresJugadores();
    });
}

async function crearNuevaPartida() {
    const dniJugador1 = document.getElementById('dniJugador1').value;
    const dniJugador2 = document.getElementById('dniJugador2').value;
    
    // --- Validaciones ---
    let esValido = true;
    if (!dniJugador1) {
        mostrarErrorCampo('errorDni1', 'Debe seleccionar al jugador 1.');
        esValido = false;
    }
    if (!dniJugador2) {
        mostrarErrorCampo('errorDni2', 'Debe seleccionar al jugador 2.');
        esValido = false;
    }
    if (dniJugador1 && dniJugador1 === dniJugador2) {
        mostrarErrorCampo('errorDni2', 'Los jugadores deben ser diferentes.');
        esValido = false;
    }

    if (!esValido) return; // Si no es válido, no continuamos
    try {
        // Crear partida usando la API
        const response = await fetch(`${API_BASE_URL}/Partida`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({
                dnis: [dniJugador1, dniJugador2]
            })
        });
        
        if (response.ok) {
            const apiResponse = await response.json();
            if (apiResponse.success) {
                const numeroPartida = apiResponse.data.numeroPartida;
                // Redirigir a la página de partida con el número de partida
                window.location.href = `partida.html?partida=${numeroPartida}`;
            } else {
                alert('Error al crear la partida: ' + apiResponse.message);
            }
        } else {
            const error = await response.json();
            alert('Error al crear la partida: ' + (error.message || 'Error desconocido'));
        }
        
    } catch (error) {
        console.error('Error al crear partida:', error);
        alert('Error al crear la partida. Verifique que la API esté funcionando.');
    }
}

async function continuarPartida() {
    const numeroPartida = document.getElementById('numeroPartida').value;
    
    //SE PUEDEN REALIZAR VALIDACIONES DEL NUMERO DE PARTIDA
        
    try {
        // Verificar que la partida existe
        const response = await fetch(`${API_BASE_URL}/Partida/ReanudarPartida?numeroPartida=${numeroPartida}`, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
            }
        });
        
        if (response.ok) {
            const apiResponse = await response.json();
            if (apiResponse.success) {
                // Redirigir a la página de partida con el número de partida
                window.location.href = `partida.html?partida=${numeroPartida}`;
            } else {
                alert('Error al cargar la partida: ' + apiResponse.message);
            }
        } else if (response.status === 404) {
           alert('Error al buscar la partida: ' + apiResponse.message);
        } else {
            //COMPLETAR CODIGO PARA MANEJO DE ERROR
            alert(apiResponse.message)
        }
        
    } catch (error) {
        console.error('Error al continuar partida:', error);
        alert('Error al cargar la partida. Verifique que la API esté funcionando.');
    }
}

function actualizarSelectoresJugadores() {
    const selects = [
        document.getElementById('dniJugador1'),
        document.getElementById('dniJugador2'),
    ];

    selects.forEach((select, i) => {
        if (!select) return;

        select.innerHTML = ''; // limpiar opciones

        const opcionDefecto = document.createElement('option');
        opcionDefecto.value = '';
        opcionDefecto.textContent = `Seleccione Jugador ${i + 1}`;
        select.appendChild(opcionDefecto);

        jugadoresRegistrados.forEach(jugador => {
            const opcion = document.createElement('option');
            opcion.value = jugador.dni;
            opcion.textContent = `${jugador.nombre} ${jugador.apellido}`;
            select.appendChild(opcion);
        });
    });
}

    
