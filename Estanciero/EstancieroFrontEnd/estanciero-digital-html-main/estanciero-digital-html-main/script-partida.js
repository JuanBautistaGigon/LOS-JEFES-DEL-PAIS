// Script específico para partida.html
// Funciones para la lógica del juego

// Variables globales OPCIONAÑES
let partidaActual = null;
let jugadoresRegistrados = []; 
let numeroPartidaActual = null;
let detallesJugadores = [];

const API_BASE_URL = 'URL API';

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
        const response = await fetch(`URL JUGADORES`);
        
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
    console.log("Página de partida cargada correctamente");
    inicializarPartida();
});

// Funciones para la página de partida
async function inicializarPartida() {
    // Obtener número de partida desde la URL
    const urlParams = new URLSearchParams(window.location.search);
    const numeroPartida = urlParams.get('partida');
    
    if (!numeroPartida) {
        alert('No se especificó el número de partida.');
        return;
    }
    
    numeroPartidaActual = parseInt(numeroPartida);
    
    try {
        // Cargar información de la partida desde la API
        await cargarPartidaCompleta();
    
        // Crear tablero
        crearTablero();
        
        // Configurar eventos
        configurarEventosPartida();
        
    } catch (error) {
        console.error('Error al inicializar partida:', error);
        alert('Error al cargar la partida.');
    }
}

async function cargarPartidaCompleta() {
    try {
        // Cargar información básica de la partida
        const response = await fetch(`URL PARTIDA POR ID`, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
            }
        });
        
        if (!response.ok) {
            throw new Error('Error al cargar la partida');
        }
        
        const apiResponse = await response.json();
        if (!apiResponse.success) {
            throw new Error(apiResponse.message || 'Error al cargar la partida');
        }
        
        partidaActual = apiResponse.data;
        
        // Cargar detalles de cada jugador
        await cargarDetallesJugadores();
        
        // Actualizar interfaz con los datos de la partida
        actualizarInterfazPartida();
        
        // Actualizar información de jugadores en la interfaz
        actualizarInterfaz();
        
    } catch (error) {
        console.error('Error al cargar partida completa:', error);
        throw error;
    }
}

async function cargarDetallesJugadores() {
    if (!numeroPartidaActual) {
        return;
    }
    
    try {
        // Usar el nuevo endpoint para obtener todos los detalles de la partida
        const response = await fetch(`URL DETALLES JUGADORES PARTIDA POR ID`, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
            }
        });
        
        if (response.ok) {
            const apiResponse = await response.json();
            if (apiResponse.success) {
                // Ordenar jugadores por DNI para mantener orden consistente en pantalla
                detallesJugadores = apiResponse.data.sort((a, b) => a.dniJugador.localeCompare(b.dniJugador));
            } else {
                console.error('Error al cargar detalles:', apiResponse.message);
                detallesJugadores = [];
            }
        } else {
            console.error('Error al cargar detalles de la partida');
            detallesJugadores = [];
        }
    } catch (error) {
        console.error('Error al cargar detalles de jugadores:', error);
        detallesJugadores = [];
    }
}

function actualizarInterfazPartida() {
    if (!partidaActual) return;
    
    // Actualizar información general de la partida
    document.title = `Partida ${partidaActual.numeroPartida} - El Estanciero Digital`;
    
    // Actualizar nombres de jugadores (si hay configuración de turnos)
    if (partidaActual.configuracionTurnos && partidaActual.configuracionTurnos.length > 0) {
        // Ordenar configuración de turnos por DNI para mantener orden consistente
        const turnosOrdenados = [...partidaActual.configuracionTurnos].sort((a, b) => a.dniJugador.localeCompare(b.dniJugador));
        
        const jugador1 = turnosOrdenados[0];
        const jugador2 = turnosOrdenados[1] || turnosOrdenados[0];
        
        document.getElementById('nombreJugador1').textContent = `Jugador ${jugador1.dniJugador}`;
        document.getElementById('nombreJugador2').textContent = `Jugador ${jugador2.dniJugador}`;
    }
    
    // Actualizar estado de la partida
    actualizarEstadoPartida(partidaActual.estado);
}

function actualizarEstadoPartida(estado) {
    const btnLanzarDado = document.getElementById('btnLanzarDado');
    const btnPausar = document.getElementById('btnPausar');
    const btnReanudar = document.getElementById('btnReanudar');
    
    if (estado === 'EnJuego') {
        //DESABILITAR LO QUE SEA NECESARIO
    } else if (estado === 'Pausada') {
        //DESABILITAR LO QUE SEA NECESARIO
    } else if (estado === 'Suspendida' || estado === 'Finalizada') {
        //DESABILITAR LO QUE SEA NECESARIO
    }
}

function crearTablero() {
    const casillerosGrid = document.getElementById('casilleros-grid');
    
    // Crear 30 casilleros (1-30)
    for (let i = 1; i <= 30; i++) {
        const casillero = document.createElement('div');
        casillero.className = 'casillero';
        casillero.id = `casillero-${i}`;
        
        const numero = document.createElement('span');
        numero.className = 'numero-casillero';
        numero.textContent = i;
        
        const jugadoresEnCasillero = document.createElement('div');
        jugadoresEnCasillero.className = 'jugadores-en-casillero';
        jugadoresEnCasillero.id = `jugadores-${i}`;
        
        casillero.appendChild(numero);
        casillero.appendChild(jugadoresEnCasillero);
        casillerosGrid.appendChild(casillero);
    }
    
    // Posicionar jugadores en el casillero inicial
    actualizarPosicionesJugadores();
}

function configurarEventosPartida() {
    // Botón lanzar dado
    const btnLanzarDado = document.getElementById('btnLanzarDado');
    btnLanzarDado.addEventListener('click', lanzarDado);
    
    // Botones de control
    const btnPausar = document.getElementById('btnPausar');
    const btnReanudar = document.getElementById('btnReanudar');
    const btnSuspender = document.getElementById('btnSuspender');
    
    btnPausar.addEventListener('click', pausarPartida);
    btnReanudar.addEventListener('click', reanudarPartida);
    btnSuspender.addEventListener('click', suspenderPartida);
}

async function lanzarDado() {
    if (!partidaActual || partidaActual.estado !== 'EnJuego') {
        alert('La partida no está en juego o está pausada/suspendida');
        return;
    }
    
    try {
        // Llamar a la API para lanzar el dado
        const response = await fetch(`URL LANZAR DADO`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            }
        });
        
        if (!response.ok) {
            const errorResponse = await response.json();
            const errorMessage = errorResponse.message || 'Error al lanzar el dado';
            alert(`ERROR: ${errorMessage}`);
            return;
        }
        
        const apiResponse = await response.json();
        if (!apiResponse.success) {
            alert(`ERROR: ${apiResponse.message || 'Error al lanzar el dado'}`);
            return;
        }
        
        // La respuesta contiene la información del dado lanzado
        const resultadoDado = apiResponse.data;
        
        // Mostrar popup con el resultado del dado
        const mensajePopup = `MENSAJE POPUP RESULTADO LANZAMIENTO SACADO DE RESULTADO DADO`;
        
        alert(mensajePopup);
        
        // Después de que el usuario acepte el mensaje, recargar la página completa
        window.location.reload();
        
    } catch (error) {
        console.error('Error al lanzar dado:', error);
        alert(`ERROR: No se pudo conectar con la API. Verifique su conexión a internet.\n\nDetalles: ${error.message}`);
    }
}

function moverJugador(jugador, pasos) {
    const nuevaPosicion = jugador.posicion + pasos;
    
    // Si llega al casillero 31 o más, reinicia desde el inicio
    if (nuevaPosicion > 30) {
        jugador.posicion = nuevaPosicion - 30;
    } else {
        jugador.posicion = nuevaPosicion;
    }
    
    // Actualizar posiciones en la interfaz
    actualizarPosicionesJugadores();
}

function actualizarPosicionesJugadores() {
    // Limpiar todas las posiciones
    for (let i = 0; i <= 30; i++) {
        const jugadoresEnCasillero = document.getElementById(`jugadores-${i}`);
        if (jugadoresEnCasillero) {
            jugadoresEnCasillero.innerHTML = '';
        }
    }
    
    // Posicionar jugadores usando los detalles cargados desde la API
    detallesJugadores.forEach((detalleJugador, index) => {
        const casillero = document.getElementById(`casillero-${detalleJugador.posicionActual}`);
        const jugadoresEnCasillero = document.getElementById(`jugadores-${detalleJugador.posicionActual}`);
        
        if (casillero && jugadoresEnCasillero) {
            // Determinar si es el turno de este jugador
            const esTurnoActual = partidaActual && 
                                partidaActual.configuracionTurnos && 
                                partidaActual.configuracionTurnos[partidaActual.turnoActual - 1] &&
                                partidaActual.configuracionTurnos[partidaActual.turnoActual - 1].dniJugador === detalleJugador.dniJugador;
            
            // Asignar color fijo por jugador (basado en el orden de creación)
            let colorFondo;
            if (esTurnoActual) {
                colorFondo = '#3498db'; // Azul para el jugador que tiene el turno
            } else {
                // Colores fijos por jugador (basado en el índice en la configuración de turnos ordenada por DNI)
                const turnosOrdenados = [...partidaActual.configuracionTurnos].sort((a, b) => a.dniJugador.localeCompare(b.dniJugador));
                const indiceJugador = turnosOrdenados.findIndex(t => t.dniJugador === detalleJugador.dniJugador);
                switch (indiceJugador) {
                    case 0:
                        colorFondo = '#e74c3c'; // Rojo para jugador 1
                        break;
                    case 1:
                        colorFondo = '#27ae60'; // Verde para jugador 2
                        break;
                    case 2:
                        colorFondo = '#f39c12'; // Naranja para jugador 3
                        break;
                    case 3:
                        colorFondo = '#9b59b6'; // Púrpura para jugador 4
                        break;
                    default:
                        colorFondo = '#95a5a6'; // Gris por defecto
                }
            }
            
            const indicador = document.createElement('div');
            indicador.className = `indicador-jugador`;
            indicador.textContent = detalleJugador.dniJugador;
            indicador.style.cssText = `
                background-color: ${colorFondo};
                color: white;
                border-radius: 8px;
                padding: 4px 8px;
                min-width: 60px;
                height: 20px;
                display: flex;
                align-items: center;
                justify-content: center;
                font-size: 10px;
                font-weight: bold;
                white-space: nowrap;
                border: ${esTurnoActual ? '2px solid #2980b9' : 'none'};
            `;
            
            jugadoresEnCasillero.appendChild(indicador);
        }
    });
}

function actualizarInterfaz() {
    // Actualizar información de jugadores usando los detalles cargados desde la API
    detallesJugadores.forEach((detalleJugador, index) => {
        document.getElementById(`saldoJugador${index + 1}`).textContent = detalleJugador.dineroDisponible;
        document.getElementById(`estadoJugador${index + 1}`).textContent = detalleJugador.estado;
        document.getElementById(`posicionJugador${index + 1}`).textContent = detalleJugador.posicionActual;
    });
    
    // Actualizar estado de botones según el estado de la partida
    const btnLanzarDado = document.getElementById('btnLanzarDado');
    const btnPausar = document.getElementById('btnPausar');
    const btnReanudar = document.getElementById('btnReanudar');
    
    if (partidaActual && partidaActual.estado === 'EnJuego') {
        //DESABILITAR LO QUE SEA NECESARIO
    } else if (partidaActual && partidaActual.estado === 'Pausada') {
        //DESABILITAR LO QUE SEA NECESARIO
    } else if (partidaActual && (partidaActual.estado === 'Suspendida' || partidaActual.estado === 'Finalizada')) {
        //DESABILITAR LO QUE SEA NECESARIO
    }
}

async function pausarPartida() {
    try {
        const response = await fetch(`URL PARTIDA POR ID`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
            }
        });
        
        if (response.ok) {
            const apiResponse = await response.json();
            if (apiResponse.success) {
                // Recargar la partida para obtener el estado actualizado
                await cargarPartidaCompleta();
                alert('Partida pausada');
            } else {
                alert('Error al pausar la partida: ' + apiResponse.message);
            }
        } else {
            alert('Error al pausar la partida');
        }
    } catch (error) {
        console.error('Error al pausar partida:', error);
        alert('Error al pausar la partida');
    }
}

async function reanudarPartida() {
    try {
        const response = await fetch(`URL PARTIDA POR ID`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
            }
        });
        
        if (response.ok) {
            const apiResponse = await response.json();
            if (apiResponse.success) {
                // Recargar la partida para obtener el estado actualizado
                await cargarPartidaCompleta();
                alert('Partida reanudada');
            } else {
                alert('Error al reanudar la partida: ' + apiResponse.message);
            }
        } else {
            alert('Error al reanudar la partida');
        }
    } catch (error) {
        console.error('Error al reanudar partida:', error);
        alert('Error al reanudar la partida');
    }
}

async function suspenderPartida() {
    if (confirm('¿Está seguro de que desea suspender la partida? Esto finalizará el juego.')) {
        try {
            const response = await fetch(`URL PARTIDA POR ID`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                }
            });
        
            if (response.ok) {
                const apiResponse = await response.json();
                if (apiResponse.success) {
                    // Recargar la partida para obtener el estado actualizado
                    await cargarPartidaCompleta();
                    alert('Partida suspendida');
                    
                    // Mostrar ganador si hay uno
                    if (partidaActual && partidaActual.dniGanador) {
                        alert(`¡Partida finalizada! El ganador es: ${partidaActual.dniGanador}`);
                    }
                } else {
                    alert('Error al suspender la partida: ' + apiResponse.message);
                }
            } else {
                alert('Error al suspender la partida');
            }
        } catch (error) {
            console.error('Error al suspender partida:', error);
            alert('Error al suspender la partida');
        }
    }
}

// Función para regresar al inicio
function regresarAlInicio() {
    window.location.href = 'index.html';
}
