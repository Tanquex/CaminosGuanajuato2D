using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CaminosDeGuanajuato.Core;

namespace CaminosDeGuanajuato.Nieves
{
    public enum EstadoJuegoNeveria
    {
        Jugando,
        Victoria,
        Derrota
    }

    /// <summary>
    /// Administrador del Nivel 2: El Desafío de las Nieves Tradicionales.
    /// Configura el escenario del Jardín Principal y Parroquia de Dolores Hidalgo,
    /// sitúa al maestro nevero Don Chon junto a su puesto artesanal y gestiona la llegada
    /// de habitantes y turistas virreinales (Doña Remedios, Don Silvestre el Arriero,
    /// Joven Alfarero, Criolla de San Felipe) con sprites pixel art 32-bit de 1810.
    /// </summary>
    public class NeveriaGameManager : MonoBehaviour
    {
        public static NeveriaGameManager Instance { get; private set; }

        [Header("Objetivos de la Misión")]
        [SerializeField] private int clientesObjetivo = 8;
        [SerializeField] private int maxClientesPerdidos = 3;
        [SerializeField] private float duracionJornada = 140f;

        [Header("Puntos de Movimiento sobre el Empedrado")]
        [SerializeField] private Vector3 puntoSpawnCliente = new Vector3(7.2f, -2.25f, 0f);
        [SerializeField] private Vector3 puntoMostradorCliente = new Vector3(1.25f, -2.25f, 0f);
        [SerializeField] private Vector3 puntoSalidaCliente = new Vector3(7.8f, -2.25f, 0f);

        private SpriteRenderer fondoEscenarioRenderer;
        private GameObject goDonChon;
        private SpriteRenderer srDonChon;

        // Caché de sprites coloniales
        private Dictionary<string, Sprite> cacheSprites = new Dictionary<string, Sprite>();

        private EstadoJuegoNeveria estadoJuego = EstadoJuegoNeveria.Jugando;
        private int clientesAtendidos = 0;
        private int clientesPerdidos = 0;
        private int puntajeTotal = 0;
        private float tiempoRestante;
        private ClienteTurista clienteActual;
        private string ultimoErrorEntrega = null;

        private List<string> nombresComensales = new List<string>
        {
            "Doña Remedios",
            "Don Silvestre el Arriero",
            "Joven Alfarero",
            "Criolla de San Felipe",
            "Viajero de Irapuato",
            "Turista Poblana",
            "Aprendiz de Cantería",
            "Comerciante de Loza",
            "Corregidor Suplente"
        };

        public EstadoJuegoNeveria EstadoJuego => estadoJuego;
        public int ClientesAtendidos => clientesAtendidos;
        public int ClientesObjetivo => clientesObjetivo;
        public int ClientesPerdidos => clientesPerdidos;
        public int MaxClientesPerdidos => maxClientesPerdidos;
        public int PuntajeTotal => puntajeTotal;
        public float TiempoRestante => Mathf.Max(0f, tiempoRestante);
        public ClienteTurista ClienteActual => clienteActual;
        public string UltimoErrorEntrega => ultimoErrorEntrega;

        public event Action OnEstadoCambiado;
        public event Action<string, Color, Vector3> OnNotificacionFlotante;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }
            Instance = this;
            tiempoRestante = duracionJornada;

            // Altura de referencia sobre la calle empedrada
            puntoSpawnCliente = new Vector3(7.2f, -2.25f, 0f);
            puntoMostradorCliente = new Vector3(1.25f, -2.25f, 0f);
            puntoSalidaCliente = new Vector3(7.8f, -2.25f, 0f);
        }

        private void Start()
        {
            Time.timeScale = 1f;
            ConfigurarEscenarioColonial();
            StartCoroutine(GenerarSiguienteCliente(1.0f));
        }

        private void Update()
        {
            AjustarFondoACamara();
            ActualizarAnimacionDonChon();

            if (estadoJuego != EstadoJuegoNeveria.Jugando) return;

            tiempoRestante -= Time.deltaTime;
            if (tiempoRestante <= 0f)
            {
                VerificarFinDeJuego();
            }
        }

        /// <summary>
        /// Monta el escenario visual con el quiosco de Dolores Hidalgo y sitúa a Don Chon
        /// junto a su puesto de nieves con su sprite pixel art colonial de cuerpo completo.
        /// </summary>
        private void ConfigurarEscenarioColonial()
        {
            // Limpiar objetos temporales de versiones anteriores si existieran
            GameObject viejoGuanajicho = GameObject.Find("Guanajicho_EnMostrador");
            if (viejoGuanajicho != null) Destroy(viejoGuanajicho);

            // 1. Fondo del Quiosco y Parroquia de Dolores Hidalgo
            GameObject fondoGO = GameObject.Find("Fondo_Quiosco_Nieves");
            if (fondoGO == null) fondoGO = new GameObject("Fondo_Quiosco_Nieves");

            fondoEscenarioRenderer = fondoGO.GetComponent<SpriteRenderer>();
            if (fondoEscenarioRenderer == null) fondoEscenarioRenderer = fondoGO.AddComponent<SpriteRenderer>();
            fondoEscenarioRenderer.sortingOrder = -10;

            if (fondoEscenarioRenderer.sprite == null)
            {
                fondoEscenarioRenderer.sprite = Overworld.OverworldManager.CargarSpriteDesdeAssets("Sprites/Fondo_Quiosco_Nieves_Dolores.jpg", 64f);
            }
            AjustarFondoACamara();

            // 2. Colocar a Don Chon junto al puesto de nieves
            goDonChon = GameObject.Find("DonChon_ElNeveroMayor");
            if (goDonChon == null)
            {
                goDonChon = new GameObject("DonChon_ElNeveroMayor");
            }

            // Limpiar hijos antiguos si existieran (ej. partes modulares viejas)
            for (int i = goDonChon.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(goDonChon.transform.GetChild(i).gameObject);
            }

            goDonChon.transform.position = new Vector3(-0.35f, -2.25f, 0f);
            goDonChon.transform.localScale = Vector3.one;

            srDonChon = goDonChon.GetComponent<SpriteRenderer>();
            if (srDonChon == null) srDonChon = goDonChon.AddComponent<SpriteRenderer>();
            srDonChon.sortingOrder = 6;
            srDonChon.sprite = CargarSpriteColonial("DonChon");
            srDonChon.flipX = false; // Mira hacia el mostrador/comensal
        }

        private void ActualizarAnimacionDonChon()
        {
            if (goDonChon == null) return;
            // Respiración sutil y movimiento cálido del maestro nevero
            float respiracion = Mathf.Sin(Time.time * 2.8f) * 0.015f;
            goDonChon.transform.localScale = new Vector3(1f - respiracion * 0.3f, 1f + respiracion, 1f);
        }

        /// <summary>
        /// Carga un sprite colonial pixel art con pivote en los pies (0.5, 0.0) y resolución correcta,
        /// compatible tanto con el Editor de Unity como con compilaciones WebGL.
        /// </summary>
        public Sprite CargarSpriteColonial(string nombre)
        {
            if (cacheSprites.TryGetValue(nombre, out Sprite spCache) && spCache != null)
            {
                return spCache;
            }

            // 1. Cargar desde Resources (PersonajesColonial/Nombre)
            Sprite spRes = Resources.Load<Sprite>("PersonajesColonial/" + nombre);
            if (spRes != null)
            {
                cacheSprites[nombre] = spRes;
                return spRes;
            }

            Texture2D texRes = Resources.Load<Texture2D>("PersonajesColonial/" + nombre);
            if (texRes != null)
            {
                Sprite nuevoSp = Sprite.Create(texRes, new Rect(0, 0, texRes.width, texRes.height), new Vector2(0.5f, 0f), 380f);
                cacheSprites[nombre] = nuevoSp;
                return nuevoSp;
            }

            // 2. Fallback directo desde archivo de disco (Assets/Sprites/PersonajesColonial/)
            try
            {
                string ruta = System.IO.Path.Combine(Application.dataPath, "Sprites/PersonajesColonial", nombre + ".png");
                if (System.IO.File.Exists(ruta))
                {
                    byte[] data = System.IO.File.ReadAllBytes(ruta);
                    Texture2D tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                    if (tex.LoadImage(data))
                    {
                        tex.filterMode = FilterMode.Bilinear;
                        Sprite sp = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0f), 380f);
                        cacheSprites[nombre] = sp;
                        return sp;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[Nevería] No se pudo cargar sprite colonial {nombre}: {ex.Message}");
            }

            return null;
        }

        private Sprite SeleccionarSpriteParaCliente(string nombreCliente, int indexTurno)
        {
            if (nombreCliente.Contains("Remedios"))
            {
                return CargarSpriteColonial("DonaRemedios");
            }
            if (nombreCliente.Contains("Silvestre") || nombreCliente.Contains("Arriero"))
            {
                return CargarSpriteColonial("DonSilvestre");
            }
            if (nombreCliente.Contains("Alfarero") || nombreCliente.Contains("Aprendiz") || nombreCliente.Contains("Cantería"))
            {
                return CargarSpriteColonial("JovenAlfarero");
            }
            if (nombreCliente.Contains("Criolla") || nombreCliente.Contains("Poblana") || nombreCliente.Contains("Felipe"))
            {
                return CargarSpriteColonial("CriollaSanFelipe");
            }

            // Selección rotativa de personajes para nombres adicionales
            string[] variantes = new string[] { "DonaRemedios", "DonSilvestre", "JovenAlfarero", "CriollaSanFelipe" };
            string elegida = variantes[indexTurno % variantes.Length];
            return CargarSpriteColonial(elegida);
        }

        private void AjustarFondoACamara()
        {
            if (fondoEscenarioRenderer == null || fondoEscenarioRenderer.sprite == null) return;
            Camera cam = Camera.main;
            if (cam == null || !cam.orthographic) return;

            float altoCam = cam.orthographicSize * 2f;
            float anchoCam = altoCam * cam.aspect;

            Vector2 spriteSize = fondoEscenarioRenderer.sprite.rect.size / fondoEscenarioRenderer.sprite.pixelsPerUnit;
            float sX = anchoCam / spriteSize.x;
            float sY = altoCam / spriteSize.y;
            float sFinal = Mathf.Max(sX, sY);
            fondoEscenarioRenderer.transform.localScale = new Vector3(sFinal, sFinal, 1f);
            fondoEscenarioRenderer.transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y, 0f);
        }

        private IEnumerator GenerarSiguienteCliente(float retraso)
        {
            yield return new WaitForSeconds(retraso);

            if (estadoJuego != EstadoJuegoNeveria.Jugando) yield break;

            if (clientesAtendidos >= clientesObjetivo)
            {
                DeclararVictoria();
                yield break;
            }

            TipoRecipiente rec = UnityEngine.Random.value > 0.5f ? TipoRecipiente.BarquilloCrujiente : TipoRecipiente.VasoDeBarro;
            SaborNieve sabor = (SaborNieve)UnityEngine.Random.Range(1, 6);
            int idxCliente = (clientesAtendidos + clientesPerdidos) % nombresComensales.Count;
            string nombre = nombresComensales[idxCliente];

            float pacienciaBase = Mathf.Max(11f, 18f - (clientesAtendidos * 1.0f));
            PedidoNieve nuevoPedido = new PedidoNieve(rec, sabor, nombre, pacienciaBase);

            GameObject go = new GameObject($"Cliente_{nombre}");
            go.transform.position = puntoSpawnCliente;

            Sprite spriteElegido = SeleccionarSpriteParaCliente(nombre, idxCliente);

            clienteActual = go.AddComponent<ClienteTurista>();
            clienteActual.Configurar(nuevoPedido, puntoMostradorCliente, puntoSalidaCliente, spriteElegido);
            clienteActual.OnPacienciaAgotada += ManejarClientePerdido;
            ultimoErrorEntrega = null;

            Debug.Log($"<color=yellow>[Comensal]</color> {nombre} llegó al quiosco pidiendo {PedidoNieve.ObtenerNombreSabor(sabor)} en {PedidoNieve.ObtenerNombreRecipiente(rec)}");
            OnEstadoCambiado?.Invoke();
        }

        public void IntentarServirPedidoActual()
        {
            if (estadoJuego != EstadoJuegoNeveria.Jugando) return;
            if (clienteActual == null || clienteActual.Pedido == null)
            {
                ultimoErrorEntrega = "El comensal ya no está en el mostrador. Espera al nuevo visitante o desecha la nieve.";
                Debug.LogWarning("[Nevería] " + ultimoErrorEntrega);
                OnEstadoCambiado?.Invoke();
                return;
            }

            if (MesaPreparacion.Instance == null) return;

            if (MesaPreparacion.Instance.CoincideConPedido(clienteActual.Pedido))
            {
                clientesAtendidos++;
                int puntosGanados = clienteActual.Pedido.valorPuntos + Mathf.RoundToInt(clienteActual.Pedido.PorcentajePaciencia * 50f);
                puntajeTotal += puntosGanados;
                ultimoErrorEntrega = null;

                OnNotificacionFlotante?.Invoke($"+{puntosGanados} pts ¡Exquisito! 💖", new Color(1f, 0.90f, 0.25f), clienteActual.transform.position + Vector3.up * 2.6f);

                if (RecursosCivicosManager.Instance != null)
                {
                    RecursosCivicosManager.Instance.AgregarRecursos(15);
                }

                if (AudioColonialManager.Instance != null)
                {
                    AudioColonialManager.Instance.ReproducirPedidoCompletado();
                }

                if (DonChonConsejero.Instance != null)
                {
                    DonChonConsejero.Instance.ReaccionarPedidoExitoso(clienteActual.Pedido.saborDeseado);
                }

                clienteActual.DespacharFeliz();
                clienteActual = null;
                MesaPreparacion.Instance.LimpiarMesa();

                OnEstadoCambiado?.Invoke();

                if (clientesAtendidos >= clientesObjetivo)
                {
                    DeclararVictoria();
                }
                else
                {
                    StartCoroutine(GenerarSiguienteCliente(1.2f));
                }
            }
            else
            {
                if (AudioColonialManager.Instance != null)
                {
                    AudioColonialManager.Instance.ReproducirErrorPedido();
                }

                string recDeseado = PedidoNieve.ObtenerNombreRecipiente(clienteActual.Pedido.recipienteDeseado);
                string sabDeseado = PedidoNieve.ObtenerNombreSabor(clienteActual.Pedido.saborDeseado);
                string recActual = PedidoNieve.ObtenerNombreRecipiente(MesaPreparacion.Instance.RecipienteActual);
                string sabActual = PedidoNieve.ObtenerNombreSabor(MesaPreparacion.Instance.SaborActual);

                ultimoErrorEntrega = $"¡No coincide! {clienteActual.NombreCliente} pidió {recDeseado} de {sabDeseado}, pero preparaste {recActual} de {sabActual}.";
                OnNotificacionFlotante?.Invoke("¡No coincide! ❌", new Color(1f, 0.40f, 0.35f), clienteActual.transform.position + Vector3.up * 2.6f);

                if (DonChonConsejero.Instance != null)
                {
                    DonChonConsejero.Instance.ReaccionarError($"¡Cuidado! El cliente pidió {sabDeseado}.");
                }
                OnEstadoCambiado?.Invoke();
            }
        }

        public void EntregarPedidoConFalloForzado()
        {
            if (estadoJuego != EstadoJuegoNeveria.Jugando) return;

            if (clienteActual != null)
            {
                clientesPerdidos++;
                OnNotificacionFlotante?.Invoke("¡Entrega fallida! 💔", new Color(1f, 0.35f, 0.35f), clienteActual.transform.position + Vector3.up * 2.6f);
                if (AudioColonialManager.Instance != null)
                {
                    AudioColonialManager.Instance.ReproducirErrorPedido();
                }
                if (DonChonConsejero.Instance != null)
                {
                    DonChonConsejero.Instance.ReaccionarError("¡El cliente se fue disgustado por la nieve incorrecta!");
                }
                clienteActual.DespacharMolesto();
                clienteActual = null;
            }

            if (MesaPreparacion.Instance != null)
            {
                MesaPreparacion.Instance.LimpiarMesa();
            }

            ultimoErrorEntrega = null;
            OnEstadoCambiado?.Invoke();

            if (clientesPerdidos >= maxClientesPerdidos)
            {
                DeclararDerrota();
            }
            else
            {
                StartCoroutine(GenerarSiguienteCliente(1.2f));
            }
        }

        private void ManejarClientePerdido(ClienteTurista cliente)
        {
            if (estadoJuego != EstadoJuegoNeveria.Jugando) return;

            clientesPerdidos++;
            if (cliente != null)
            {
                OnNotificacionFlotante?.Invoke("¡Paciencia agotada! 💔", new Color(1f, 0.40f, 0.40f), cliente.transform.position + Vector3.up * 2.6f);
            }
            if (AudioColonialManager.Instance != null)
            {
                AudioColonialManager.Instance.ReproducirErrorPedido();
            }

            if (DonChonConsejero.Instance != null)
            {
                DonChonConsejero.Instance.ReaccionarClientePerdido();
            }

            ultimoErrorEntrega = "¡El comensal perdió la paciencia y se marchó! Desecha la nieve o sírvela al siguiente comensal si coincide.";
            clienteActual = null;
            OnEstadoCambiado?.Invoke();

            if (clientesPerdidos >= maxClientesPerdidos)
            {
                DeclararDerrota();
            }
            else
            {
                StartCoroutine(GenerarSiguienteCliente(1.5f));
            }
        }

        private void VerificarFinDeJuego()
        {
            if (clientesAtendidos >= clientesObjetivo)
            {
                DeclararVictoria();
            }
            else
            {
                DeclararDerrota();
            }
        }

        private void DeclararVictoria()
        {
            estadoJuego = EstadoJuegoNeveria.Victoria;

            if (ProgresoJuegoManager.Instance != null)
            {
                ProgresoJuegoManager.Instance.RegistrarMisionCompletada(TipoMision.Dolores_Nieves, puntajeTotal);
            }
            else
            {
                PlayerPrefs.SetInt(ProgresoJuegoManager.KEY_MISION2, 1);
                PlayerPrefs.Save();
            }

            if (DonChonConsejero.Instance != null)
            {
                DonChonConsejero.Instance.ReaccionarVictoria();
            }

            if (AudioColonialManager.Instance != null)
            {
                AudioColonialManager.Instance.ReproducirCampanaGritoLibertad();
            }

            Debug.Log("<color=green>[Nevería]</color> ⭐ ¡VICTORIA EN EL FESTIVAL DE LA NIEVE! Cuchara Dorada obtenida.");
            OnEstadoCambiado?.Invoke();
        }

        private void DeclararDerrota()
        {
            estadoJuego = EstadoJuegoNeveria.Derrota;
            Debug.Log("<color=red>[Nevería]</color> 💀 Jornada fallida. Demasiados comensales decepcionados.");
            if (AudioColonialManager.Instance != null)
            {
                AudioColonialManager.Instance.ReproducirDerrota();
            }
            OnEstadoCambiado?.Invoke();
        }
    }
}
