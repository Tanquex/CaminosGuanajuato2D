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
    /// Configura automáticamente el escenario con el Quiosco y la Parroquia de Dolores Hidalgo,
    /// sitúa a Guanajicho y a Don Chon en el puesto, y atiende a comensales reales de Pixel Adventure.
    /// </summary>
    public class NeveriaGameManager : MonoBehaviour
    {
        public static NeveriaGameManager Instance { get; private set; }

        [Header("Objetivos de la Misión")]
        [SerializeField] private int clientesObjetivo = 8;
        [SerializeField] private int maxClientesPerdidos = 3;
        [SerializeField] private float duracionJornada = 140f;

        [Header("Puntos de Movimiento")]
        [SerializeField] private Vector3 puntoSpawnCliente = new Vector3(8.5f, -0.6f, 0f);
        [SerializeField] private Vector3 puntoMostradorCliente = new Vector3(0.6f, -0.6f, 0f);
        [SerializeField] private Vector3 puntoSalidaCliente = new Vector3(9.0f, -0.6f, 0f);

        // Referencias a fondo y personajes del escenario
        private SpriteRenderer fondoEscenarioRenderer;
        private GameObject goGuanajicho;
        private GameObject goDonChon;

        // Sprites de personajes cacheados
        private List<Sprite> spritesComensales = new List<Sprite>();

        // Estado de la partida
        private EstadoJuegoNeveria estadoJuego = EstadoJuegoNeveria.Jugando;
        private int clientesAtendidos = 0;
        private int clientesPerdidos = 0;
        private int puntajeTotal = 0;
        private float tiempoRestante;
        private ClienteTurista clienteActual;
        private string ultimoErrorEntrega = null;

        private List<string> nombresComensales = new List<string>
        {
            "Doña Remedios", "Don Silvestre el Arriero", "Joven Alfarero",
            "Viajero de Irapuato", "Turista Poblana", "Corregidor Suplente",
            "Criolla de San Felipe", "Aprendiz de Cantería", "Comerciante de Loza"
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

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }
            Instance = this;
            tiempoRestante = duracionJornada;
        }

        private void Start()
        {
            Time.timeScale = 1f;
            ConfigurarEscenarioColonial();
            CargarSpritesComensales();
            StartCoroutine(GenerarSiguienteCliente(1.2f));
        }

        private void Update()
        {
            AjustarFondoACamara();

            if (estadoJuego != EstadoJuegoNeveria.Jugando) return;

            tiempoRestante -= Time.deltaTime;
            if (tiempoRestante <= 0f)
            {
                VerificarFinDeJuego();
            }
        }

        /// <summary>
        /// Monta el escenario visual con el quiosco de Dolores Hidalgo y coloca a Guanajicho y a Don Chon.
        /// </summary>
        private void ConfigurarEscenarioColonial()
        {
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

            // 2. Colocar a Guanajicho detrás del mostrador (a la izquierda)
            if (goGuanajicho == null)
            {
                goGuanajicho = new GameObject("Guanajicho_EnMostrador");
                goGuanajicho.transform.position = new Vector3(-2.8f, -0.4f, 0f);
                goGuanajicho.transform.localScale = new Vector3(1.4f, 1.4f, 1f);

                SpriteRenderer srG = goGuanajicho.AddComponent<SpriteRenderer>();
                srG.sortingOrder = 5;

                // Intentar cargar sprite de HeroKnight
                Sprite spriteGuanajicho = CargarPrimerFrameSprite("HeroKnight", 100, 55, 64f);
                if (spriteGuanajicho == null) spriteGuanajicho = CargarPrimerFrameSprite("Hero Knight - Pixel Art/Sprites/HeroKnight.png", 100, 55, 64f);
                if (spriteGuanajicho != null)
                {
                    srG.sprite = spriteGuanajicho;
                }
            }

            // 3. Colocar a Don Chon junto al puesto sobre el piso adoquinado
            if (goDonChon == null)
            {
                goDonChon = new GameObject("DonChon_ElNeveroMayor");
                goDonChon.transform.position = new Vector3(-1.1f, -0.6f, 0f);
                goDonChon.transform.localScale = new Vector3(1.6f, 1.6f, 1f);

                SpriteRenderer srDC = goDonChon.AddComponent<SpriteRenderer>();
                srDC.sortingOrder = 7;

                Sprite spriteDonChon = CargarPrimerFrameSprite("Comensal_MaskDude", 32, 32, 32f);
                if (spriteDonChon == null) spriteDonChon = CargarPrimerFrameSprite("Pixel Adventure 1/Assets/Main Characters/Mask Dude/Idle (32x32).png", 32, 32, 32f);
                if (spriteDonChon != null)
                {
                    srDC.sprite = spriteDonChon;
                }
            }
        }

        private void CargarSpritesComensales()
        {
            spritesComensales.Clear();

            string[] nombresResources = new string[]
            {
                "Comensal_PinkMan",
                "Comensal_MaskDude",
                "Comensal_VirtualGuy",
                "Comensal_NinjaFrog"
            };

            foreach (var nom in nombresResources)
            {
                Sprite sp = CargarPrimerFrameSprite(nom, 32, 32, 32f);
                if (sp != null)
                {
                    spritesComensales.Add(sp);
                }
            }

            if (spritesComensales.Count == 0)
            {
                string[] rutas = new string[]
                {
                    "Pixel Adventure 1/Assets/Main Characters/Pink Man/Idle (32x32).png",
                    "Pixel Adventure 1/Assets/Main Characters/Mask Dude/Idle (32x32).png",
                    "Pixel Adventure 1/Assets/Main Characters/Virtual Guy/Idle (32x32).png",
                    "Pixel Adventure 1/Assets/Main Characters/Ninja Frog/Idle (32x32).png"
                };

                foreach (var r in rutas)
                {
                    Sprite sp = CargarPrimerFrameSprite(r, 32, 32, 32f);
                    if (sp != null)
                    {
                        spritesComensales.Add(sp);
                    }
                }
            }
        }

        private Sprite CargarPrimerFrameSprite(string rutaRelativa, int wFrame, int hFrame, float ppu)
        {
            string nombreSinExt = System.IO.Path.GetFileNameWithoutExtension(rutaRelativa);

            // 1. Obtener la textura desde Resources para recortar con precisión el primer frame (evita mostrar el spritesheet completo)
            Texture2D resTex = Resources.Load<Texture2D>(nombreSinExt);
            if (resTex == null)
            {
                Sprite resSprite = Resources.Load<Sprite>(nombreSinExt);
                if (resSprite != null) resTex = resSprite.texture;
            }

            if (resTex != null)
            {
                resTex.filterMode = FilterMode.Point;
                int ancho = Mathf.Min(wFrame, resTex.width);
                int alto = Mathf.Min(hFrame, resTex.height);
                return Sprite.Create(resTex, new Rect(0, resTex.height - alto, ancho, alto), new Vector2(0.5f, 0f), ppu);
            }

            // 2. Fallback de desarrollo con Application.dataPath en Editor / PC
            try
            {
                string path = System.IO.Path.Combine(Application.dataPath, rutaRelativa);
                if (System.IO.File.Exists(path))
                {
                    byte[] bytes = System.IO.File.ReadAllBytes(path);
                    Texture2D tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                    if (tex.LoadImage(bytes))
                    {
                        tex.filterMode = FilterMode.Point; // Pixel art nítido
                        int ancho = Mathf.Min(wFrame, tex.width);
                        int alto = Mathf.Min(hFrame, tex.height);
                        return Sprite.Create(tex, new Rect(0, tex.height - alto, ancho, alto), new Vector2(0.5f, 0f), ppu);
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"[Nevería] Error cargando sprite {rutaRelativa}: {ex.Message}");
            }
            return null;
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
            string nombre = nombresComensales[UnityEngine.Random.Range(0, nombresComensales.Count)];

            float pacienciaBase = Mathf.Max(26f, 36f - (clientesAtendidos * 0.8f));
            PedidoNieve nuevoPedido = new PedidoNieve(rec, sabor, nombre, pacienciaBase);

            GameObject go = new GameObject($"Cliente_{nombre}");
            go.transform.position = puntoSpawnCliente;

            Sprite spriteElegido = null;
            if (spritesComensales.Count > 0)
            {
                spriteElegido = spritesComensales[UnityEngine.Random.Range(0, spritesComensales.Count)];
            }

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
