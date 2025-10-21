// Script específico para partida.html
// Funciones para la lógica del juego

// Variables globales OPCIONAÑES
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
    console.log("Página de partida cargada correctamente");
    inicializarPartida();
});



async function inicializarPartida() {
    console.log("Página de partida cargada correctamente");
    const urlParams = new URLSearchParams(window.location.search);
    const numeroPartida = urlParams.get('numeroPartida');

    if (!numeroPartida || isNaN(parseInt(numeroPartida))) {
        alert('Número de partida no válido. Volviendo al inicio.');
        window.location.href = 'index.html';
        return;
    }

    const idPartida = parseInt(numeroPartida);
    console.log("Se intentará cargar la partida con el ID:", idPartida);

    try {
        await cargarPartidaCompleta(idPartida);
        
        crearTablero();
        actualizarPosicionesJugadores();
        configurarEventosPartida(idPartida);
        cargarDetallesJugadores(idPartida); // Pasamos el ID aquí también
    } catch (error) {
        console.error('Error al inicializar partida:', error);
        alert(`Error al cargar la partida: ${error.message}`);
    }
}

// La función ahora recibe el ID como parámetro
async function cargarPartidaCompleta(idPartida) {
    try {
        // ✨ CORRECCIÓN #3: Usamos el parámetro 'idPartida' en lugar de la variable global
        const response = await fetch(`${API_BASE_URL}/Partida/${idPartida}`);
        
        if (!response.ok) {
            const errorText = await response.text(); 
            throw new Error(`Error del servidor: ${response.status} - ${errorText}`);
        }
        
        const apiResponse = await response.json();
        if (!apiResponse.success) {
            throw new Error(apiResponse.message || 'La API indicó un error al cargar la partida.');
        }

        partidaActual = apiResponse.data;
        detallesJugadores = partidaActual.detallesJugadores || [];
        detallesJugadores.sort((a, b) => a.dniJugador.localeCompare(b.dniJugador));

        // Actualizar la interfaz con los datos frescos
        actualizarInterfazPartida();
        actualizarPosicionesJugadores();
        actualizarInterfaz();
        cargarDetallesJugadores(idPartida);
        
    } catch (error) {
        console.error('Error en cargarPartidaCompleta:', error);
        throw error;
    }
}

// Asegúrate de que las otras funciones que usan el ID también lo reciban
/*
function configurarEventosPartida(idPartida) {
    document.getElementById('btnLanzarDado').addEventListener('click', () => lanzarDado(idPartida));
    // ... y así con el resto de los botones (pausar, reanudar, etc.)
    document.getElementById('btnPausar').addEventListener('click', () => pausarPartida(idPartida));
    document.getElementById('btnReanudar').addEventListener('click', () => reanudarPartida(idPartida));
    document.getElementById('btnSuspender').addEventListener('click', () => suspenderPartida(idPartida));
}
    */
// ... (El resto de tus funciones como crearTablero, etc. pueden quedar como estaban) ...
//Analizar 
async function cargarDetallesJugadores(idPartida) {
    if (!idPartida) {
        return;
    }
    
    try {
        // Usar el nuevo endpoint para obtener todos los detalles de la partida
        const response = await fetch(`${API_BASE_URL}/Partida/ObtenerDetallesJugadores/${idPartida}`, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
            }
        }); 
        
        if (response.ok) {
            const apiResponse = await response.json();
                if (apiResponse.success) {
                // Ordenar jugadores por DNI para mantener orden consistente en pantalla
               // detallesJugadores = apiResponse.data.sort((a, b) => a.dniUsuario.localeCompare(b.dniUsuario));
               detallesJugadores = apiResponse.data.sort((a, b) => a.dniUsuario - b.dniUsuario);
               actualizarInterfaz();
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
        detallesJugadores = [];
}
}

function actualizarInterfazPartida() {
    if (!partidaActual) return;
    
    // Actualizar información general de la partida
    document.title = `Partida ${partidaActual.numeroPartida} - El Estanciero Digital`;
    
    // Actualizar nombres de jugadores (si hay configuración de turnos)
    if (partidaActual.configuracionTurnos && partidaActual.configuracionTurnos.length > 0) {
        // Ordenar configuración de turnos por DNI para mantener orden consistente
        const turnosOrdenados = [...partidaActual.configuracionTurnos].sort((a, b) => String(a.dniJugador).localeCompare(String(b.dniJugador)));
        
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
        btnLanzarDado.disabled = false;
        btnPausar.disabled = false; 
        btnReanudar.disabled = true;
    } else if (estado === 'Pausada') {
        //DESABILITAR LO QUE SEA NECESARIO
        btnLanzarDado.disabled = true; 
        btnPausar.disabled = true;
        btnReanudar.disabled = false;
    } else if (estado === 'Suspendida' || estado === 'Finalizada') {
        //DESABILITAR LO QUE SEA NECESARIO
        btnLanzarDado.disabled = true; 
        btnPausar.disabled = true; 
        btnReanudar.disabled = true;
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

function configurarEventosPartida(idPartida) {
    // Botón lanzar dado
    const btnLanzarDado = document.getElementById('btnLanzarDado');
    btnLanzarDado.addEventListener('click', lanzarDado);
    
    // Botones de control
    const btnPausar = document.getElementById('btnPausar');
    const btnReanudar = document.getElementById('btnReanudar');
    const btnSuspender = document.getElementById('btnSuspender');
    
    btnPausar.addEventListener('click', () => pausarPartida(idPartida));
    btnReanudar.addEventListener('click', () => reanudarPartida(idPartida));
    btnSuspender.addEventListener('click', () => suspenderPartida(idPartida));
}

async function lanzarDado() {
    const idPartida = partidaActual.numeroPartida;
    if (!partidaActual || partidaActual.estado !== 1) {
        console.log(partidaActual.estado);
        alert('La partida no está en juego o está pausada/suspendida');
        return;
    }
    
    try {
        // Llamar a la API para lanzar el dado
        console.log("ID PARTIDA:", idPartida);
        const response = await fetch(`http://localhost:5070/Partida/LanzarDado/${idPartida}`, {
            method: /*'POST'*/ 'PUT',
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

         if (apiResponse.winner) {
            await cargarPartidaCompleta(idPartida);
                document.getElementById('btnLanzarDado').disabled = true; 
                document.getElementById('btnPausar').disabled = true; 
                document.getElementById('btnReanudar').disabled =true;   
            alert(`PARTIDA FINALIZADA: el dni numero ${apiResponse.dniwinner} gano`)

        }

        if (!apiResponse.success) {
            alert(`ERROR: ${apiResponse.message || 'Error al lanzar el dado'}`);
            return;
        }

        
        
        // La respuesta contiene la información del dado lanzado
        const resultadoDado = apiResponse.data;
        
        // Mostrar popup con el resultado del dado
       const resultado = apiResponse.data;
        const mensajePopup = `${resultado.descripcion}\nNúmero del dado: ${resultado.numeroDado}\n${resultado.siguienteTurno}`;
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
        document.getElementById(`saldoJugador${index + 1}`).textContent = detalleJugador.dineroActual;
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

async function pausarPartida(idPartida) {
    try {
        const response = await fetch(`http://localhost:5070/Partida/PausarPartida/${idPartida}`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
            }
        });
        
        console.log(idPartida)
        if (response.ok) {
            const apiResponse = await response.json();
            if (apiResponse.success) {
                // Recargar la partida para obtener el estado actualizado
                await cargarPartidaCompleta(idPartida);
                document.getElementById('btnLanzarDado').disabled = true; 
                document.getElementById('btnPausar').disabled = true; 
                document.getElementById('btnReanudar').disabled = false;
                alert('Partida pausada');
            } else {
                alert('Error al pausar la partida: ' + apiResponse.message);
            }
        } else {
            alert('Errorr al pausar la partida');
        }
    } catch (error) {
        console.error('Error al pausar partida:', error);
        alert('Erroor al pausar la partida');
    }
}

async function reanudarPartida(idPartida) {
   
    try {
        const response = await fetch(`${API_BASE_URL}/Partida/ContinuarPartida/${idPartida}`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
            }
        });
        
        if (response.ok) {
            const apiResponse = await response.json();
            if (apiResponse.success) {
                // Recargar la partida para obtener el estado actualizado
                await cargarPartidaCompleta(idPartida);
                document.getElementById('btnLanzarDado').disabled = false; 
                document.getElementById('btnPausar').disabled = false; 
                document.getElementById('btnReanudar').disabled =true;
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

async function suspenderPartida(idPartida) {
    if (confirm('¿Está seguro de que desea suspender la partida? Esto finalizará el juego.')) {
        try {
            const response = await fetch(`${API_BASE_URL}/Partida/SuspenderPartida/${idPartida}`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                }
            });
        
            if (response.ok) {
                const apiResponse = await response.json();
                if (apiResponse.success) {
                    // Recargar la partida para obtener el estado actualizado
                    await cargarPartidaCompleta(idPartida);
                    document.getElementById('btnLanzarDado').disabled = true; 
                document.getElementById('btnPausar').disabled = true; 
                document.getElementById('btnReanudar').disabled =true;
                    alert('Partida suspendida, el ganador es  '  + apiResponse.dniwinner);
                    
                    // Mostrar ganador si hay uno
                    /*if (partidaActual && partidaActual.dniGanador) {
                        alert(`¡Partida finalizada! El ganador es: ${partidaActual.dniGanador}`);
                    }*/
                } else {
                    alert('Erroor al suspender la partida: ' + apiResponse.message);
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
