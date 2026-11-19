using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using CaminosDeGuanajuato.Core;

namespace CaminosDeGuanajuato.Alfareria
{
    /// <summary>
    /// Las 3 fases simples y secuenciales (1 sola estación activa a la vez)
    /// para que el jugador nunca vea demasiadas cosas en pantalla.
    /// </summary>
    public enum FaseTallerSimple
    {
        Paso1_ElegirForma = 0,   // Clic 1: Elegir Jarro, Cazuela o Cántaro
        Paso2_ElegirColor = 1,   // Clic 2: Elegir color del esmalte (Terracota, Verde, Azul u Oro)
        Paso3_HornearYEntregar = 2 // Clic 3: Pulsar [ESPACIO / Clic] cuando la barra esté en la Zona Verde
    }

    /// <summary>
    /// Controlador simplificado, ligero e intuitivo del Nivel 4:
    /// El Taller Alfarero de la Francia Chiquita (San Felipe Torres Mochas).
    /// Todo se juega en 3 pasos guiados (1 clic por paso), mostrando solo un panel limpio a la vez.
    /// </summary>
    public class AlfareriaGameManager : MonoBehaviour
    {
        public static AlfareriaGameManager Instance { get; private set; }

        [Header("Reglas Amigables del Taller")]
        [SerializeField] private int metaPiezasEntregadas = 6;
        [SerializeField] private int maxClientesImpacientes = 3;
        [SerializeField] private float pacienciaBaseSegundos = 35f;
        [SerializeField] private float velocidadBarraHorno = 0.75f; // Oscila suavemente de ida y vuelta

        // Estado general de la partida
        private bool juegoActivo = false;
        private bool juegoTerminado = false;
        private bool victoriaAlcanzada = false;
        private int piezasEntregadas = 0;
        private int clientesPerdidos = 0;
        private int puntajeTotal = 0;
        private int rachaPerfectas = 0;
        private string motivoDerrota = "";

        // Estado del Cliente Activo en el Patio
        private PedidoAlfareria pedidoActivo;
        private float pacienciaRestante;
        private float pacienciaMaximaActual;
        private Texture2D retratoClienteActivo;
        private int indicePlantillaAnterior = -1;

        // Representación 2D del cliente caminando en el patio de la Francia Chiquita
        private GameObject clientePatioGO;
        private SpriteRenderer clientePatioRenderer;
        private Vector3 posEntradaPatio = new Vector3(6.6f, -1.65f, 0f);
        private Vector3 posMostradorTaller = new Vector3(2.45f, -1.65f, 0f);
        private float faseCaminataCliente = 0f;

        // Estado guiado paso a paso (1 sola tarea en pantalla a la vez)
        private FaseTallerSimple faseActual = FaseTallerSimple.Paso1_ElegirForma;
        private TipoFormaBarro formaSeleccionada = TipoFormaBarro.Ninguna;
        private TipoEsmalteBarro esmalteSeleccionado = TipoEsmalteBarro.Ninguno;
        private float cursorHorno01 = 0f;
        private float direccionHorno = 1f;

        // Eventos para notificaciones flotantes
        public event Action<string, Color> OnNotificacionFlotante;

        // Propiedades públicas para el HUD ligero
        public bool JuegoActivo => juegoActivo;
        public bool JuegoTerminado => juegoTerminado;
        public bool VictoriaAlcanzada => victoriaAlcanzada;
        public int PiezasEntregadas => piezasEntregadas;
        public int MetaPiezas => metaPiezasEntregadas;
        public int ClientesPerdidos => clientesPerdidos;
        public int MaxClientesImpacientes => maxClientesImpacientes;
        public int PuntajeTotal => puntajeTotal;
        public int RachaPerfectas => rachaPerfectas;
        public string MotivoDerrota => motivoDerrota;

        public PedidoAlfareria PedidoActivo => pedidoActivo;
        public float PacienciaRestante => pacienciaRestante;
        public float PacienciaMaximaActual => Mathf.Max(1f, pacienciaMaximaActual);
        public Texture2D RetratoClienteActivo => retratoClienteActivo;
        public Vector3 PosicionClienteMundo => clientePatioGO != null ? clientePatioGO.transform.position : posMostradorTaller;

        public FaseTallerSimple FaseActual => faseActual;
        public TipoFormaBarro FormaSeleccionada => formaSeleccionada;
        public TipoEsmalteBarro EsmalteSeleccionado => esmalteSeleccionado;
        public float CursorHorno01 => cursorHorno01;

        private struct PlantillaVecinoSanFelipe
        {
            public string nombre;
            public string rol;
            public string recursoSprite;
            public bool miraOriginalmenteDerecha;
        }

        private readonly PlantillaVecinoSanFelipe[] plantillasVecinos = new PlantillaVecinoSanFelipe[]
        {
            new PlantillaVecinoSanFelipe
            {
                nombre = "Doña Carlota",
                rol = "La Francia Chiquita",
                recursoSprite = "PersonajesColonial/CriollaSanFelipe",
                miraOriginalmenteDerecha = false
            },
            new PlantillaVecinoSanFelipe
            {
                nombre = "Mateo el Alfarero",
                rol = "Artesano de San Felipe",
                recursoSprite = "PersonajesColonial/JovenAlfarero",
                miraOriginalmenteDerecha = false
            },
            new PlantillaVecinoSanFelipe
            {
                nombre = "Don Silvestre",
                rol = "Arriero del Camino Real",
                recursoSprite = "PersonajesColonial/DonSilvestre",
                miraOriginalmenteDerecha = false
            },
            new PlantillaVecinoSanFelipe
            {
                nombre = "Doña Remedios",
                rol = "Cocinera Tradicional",
                recursoSprite = "PersonajesColonial/DonaRemedios",
                miraOriginalmenteDerecha = false
            },
            new PlantillaVecinoSanFelipe
            {
                nombre = "Capitán de Dragones",
                rol = "Custodio de Torres Mochas",
                recursoSprite = "PersonajesColonial/CapitanRealista",
                miraOriginalmenteDerecha = false
            }
        };

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }
            Instance = this;

            if (ProgresoJuegoManager.Instance == null)
            {
                GameObject pgo = new GameObject("ProgresoJuegoManager");
                pgo.AddComponent<ProgresoJuegoManager>();
            }

            if (GetComponent<HUDAlfareriaUI>() == null)
            {
                gameObject.AddComponent<HUDAlfareriaUI>();
            }
        }

        private void Start()
        {
            AsegurarEscenarioSanFelipe();

            if (AudioColonialManager.Instance != null)
            {
                AudioColonialManager.Instance.IniciarMusicaSanFelipe();
            }

            juegoActivo = false;
            juegoTerminado = false;
        }

        private void AsegurarEscenarioSanFelipe()
        {
            GameObject goFondo = GameObject.Find("Fondo_SanFelipe_Taller");
            SpriteRenderer sr = goFondo != null ? goFondo.GetComponent<SpriteRenderer>() : null;

            if (goFondo == null)
            {
                goFondo = new GameObject("Fondo_SanFelipe_Taller");
                sr = goFondo.AddComponent<SpriteRenderer>();
                sr.sortingOrder = -10;
            }

            if (sr.sprite == null)
            {
                Texture2D tex = Resources.Load<Texture2D>("FondoSanFelipeTorresMochas");
                if (tex != null)
                {
                    sr.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100f);
                }
            }

            Camera cam = Camera.main;
            if (cam != null && cam.orthographic && sr != null && sr.sprite != null)
            {
                float altoCam = cam.orthographicSize * 2f;
                float anchoCam = altoCam * cam.aspect;
                Vector2 tamSprite = sr.sprite.bounds.size;
                if (tamSprite.x > 0.01f && tamSprite.y > 0.01f)
                {
                    float escala = Mathf.Max(anchoCam / tamSprite.x, altoCam / tamSprite.y);
                    goFondo.transform.localScale = new Vector3(escala, escala, 1f);
                    goFondo.transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y, 0f);
                }
            }
        }

        public void ComenzarJornadaTaller()
        {
            piezasEntregadas = 0;
            clientesPerdidos = 0;
            puntajeTotal = 0;
            rachaPerfectas = 0;
            juegoTerminado = false;
            victoriaAlcanzada = false;
            juegoActivo = true;

            ReiniciarMesa();
            GenerarSiguienteCliente();
        }

        private void Update()
        {
            if (!juegoActivo || juegoTerminado) return;

            if (pedidoActivo != null)
            {
                AnimarClienteEnPatio();

                pacienciaRestante -= Time.deltaTime;
                if (pacienciaRestante <= 0f)
                {
                    RegistrarClienteImpaciente();
                    return;
                }
            }

            // En el Paso 3, el marcador del horno oscila de ida y vuelta suavemente (nunca se quema solo)
            if (faseActual == FaseTallerSimple.Paso3_HornearYEntregar)
            {
                cursorHorno01 += direccionHorno * velocidadBarraHorno * Time.deltaTime;
                if (cursorHorno01 >= 1f)
                {
                    cursorHorno01 = 1f;
                    direccionHorno = -1f;
                }
                else if (cursorHorno01 <= 0f)
                {
                    cursorHorno01 = 0f;
                    direccionHorno = 1f;
                }
            }

            ProcesarAtajosSimples();
        }

        private void ProcesarAtajosSimples()
        {
            if (faseActual == FaseTallerSimple.Paso1_ElegirForma)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
                    ElegirFormaPaso1(TipoFormaBarro.JarroChocolatero);
                else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
                    ElegirFormaPaso1(TipoFormaBarro.CazuelaMoletera);
                else if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
                    ElegirFormaPaso1(TipoFormaBarro.CantaroCaminoReal);
            }
            else if (faseActual == FaseTallerSimple.Paso2_ElegirColor)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1) || Input.GetKeyDown(KeyCode.Q))
                    ElegirColorPaso2(TipoEsmalteBarro.TerracotaBrunida);
                else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown(KeyCode.W))
                    ElegirColorPaso2(TipoEsmalteBarro.VerdeVidriado);
                else if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3) || Input.GetKeyDown(KeyCode.E))
                    ElegirColorPaso2(TipoEsmalteBarro.AzulMayolica);
                else if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4) || Input.GetKeyDown(KeyCode.R))
                    ElegirColorPaso2(TipoEsmalteBarro.OroDeGala);
            }
            else if (faseActual == FaseTallerSimple.Paso3_HornearYEntregar)
            {
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
                {
                    ConfirmarHornoYEntregarPaso3();
                }
            }
        }

        #region Los 3 Clics Guiados (Uno por Paso)

        /// <summary>
        /// PASO 1: El jugador hace 1 clic en la forma de vasija que pidió el vecino.
        /// Si elige la correcta, pasa automáticamente al Paso 2 (Pintar color).
        /// </summary>
        public void ElegirFormaPaso1(TipoFormaBarro forma)
        {
            if (!juegoActivo || juegoTerminado || pedidoActivo == null) return;

            if (forma != pedidoActivo.formaRequerida)
            {
                rachaPerfectas = 0;
                pacienciaRestante = Mathf.Max(1.5f, pacienciaRestante - 4.5f);
                if (AudioColonialManager.Instance != null) AudioColonialManager.Instance.ReproducirErrorPedido();
                OnNotificacionFlotante?.Invoke("❌ ¡Forma equivocada! (-4.5s)", new Color(1f, 0.40f, 0.35f));
                return;
            }

            formaSeleccionada = forma;
            faseActual = FaseTallerSimple.Paso2_ElegirColor;

            if (AudioColonialManager.Instance != null)
            {
                AudioColonialManager.Instance.ReproducirTornoAlfarero(0.8f);
            }
        }

        public void ElegirColorPaso2(TipoEsmalteBarro esmalte)
        {
            if (!juegoActivo || juegoTerminado || pedidoActivo == null) return;

            if (esmalte != pedidoActivo.esmalteRequerido)
            {
                rachaPerfectas = 0;
                pacienciaRestante = Mathf.Max(1.5f, pacienciaRestante - 4.5f);
                if (AudioColonialManager.Instance != null) AudioColonialManager.Instance.ReproducirErrorPedido();
                OnNotificacionFlotante?.Invoke("❌ ¡Esmalte equivocado! (-4.5s)", new Color(1f, 0.40f, 0.35f));
                return;
            }

            esmalteSeleccionado = esmalte;
            faseActual = FaseTallerSimple.Paso3_HornearYEntregar;
            cursorHorno01 = 0.08f;
            direccionHorno = 1f;

            if (AudioColonialManager.Instance != null)
            {
                AudioColonialManager.Instance.ReproducirPincelEsmalte();
                AudioColonialManager.Instance.ReproducirFuegoHorno();
            }
        }

        /// <summary>
        /// Comprueba si el marcador del horno está dentro de la Zona Verde central (32% a 72%).
        /// </summary>
        public bool EstaHornoEnZonaVerde()
        {
            return cursorHorno01 >= 0.32f && cursorHorno01 <= 0.72f;
        }

        /// <summary>
        /// PASO 3: El jugador hace 1 clic (o pulsa ESPACIO) para sacar la pieza del horno y entregarla.
        /// </summary>
        public void ConfirmarHornoYEntregarPaso3()
        {
            if (!juegoActivo || juegoTerminado || pedidoActivo == null) return;
            if (faseActual != FaseTallerSimple.Paso3_HornearYEntregar) return;

            bool enZonaVerde = EstaHornoEnZonaVerde();
            int puntos = pedidoActivo.recompensaBase;
            string msg;
            Color colMsg;

            if (enZonaVerde)
            {
                rachaPerfectas++;
                puntos += 70 + (rachaPerfectas * 15);
                msg = $"🔥 ¡COCCIÓN PERFECTA! +{puntos} pts";
                colMsg = new Color(0.35f, 1.0f, 0.48f);
            }
            else
            {
                rachaPerfectas = 0;
                msg = $"🏺 ¡Pieza Entregada! +{puntos} pts";
                colMsg = new Color(1.0f, 0.88f, 0.38f);
            }

            puntajeTotal += puntos;
            piezasEntregadas++;

            if (AudioColonialManager.Instance != null)
            {
                AudioColonialManager.Instance.ReproducirPedidoCompletado();
            }

            if (RecursosCivicosManager.Instance != null)
            {
                RecursosCivicosManager.Instance.AgregarRecursos(25);
            }

            OnNotificacionFlotante?.Invoke(msg, colMsg);
            ReiniciarMesa();

            if (piezasEntregadas >= metaPiezasEntregadas)
            {
                FinalizarTaller(true, "");
            }
            else
            {
                GenerarSiguienteCliente();
            }
        }

        private void ReiniciarMesa()
        {
            faseActual = FaseTallerSimple.Paso1_ElegirForma;
            formaSeleccionada = TipoFormaBarro.Ninguna;
            esmalteSeleccionado = TipoEsmalteBarro.Ninguno;
            cursorHorno01 = 0f;
            direccionHorno = 1f;
        }

        #endregion

        #region Gestión de Vecinos en el Patio

        private void GenerarSiguienteCliente()
        {
            int idx = UnityEngine.Random.Range(0, plantillasVecinos.Length);
            if (idx == indicePlantillaAnterior)
            {
                idx = (idx + 1) % plantillasVecinos.Length;
            }
            indicePlantillaAnterior = idx;

            PlantillaVecinoSanFelipe p = plantillasVecinos[idx];
            TipoFormaBarro forma = (TipoFormaBarro)UnityEngine.Random.Range(1, 4);
            TipoEsmalteBarro esmalte = (TipoEsmalteBarro)UnityEngine.Random.Range(1, 5);

            pedidoActivo = new PedidoAlfareria(forma, esmalte, p.nombre, p.rol, "", 180);
            pacienciaMaximaActual = Mathf.Max(26f, pacienciaBaseSegundos - piezasEntregadas * 1.0f);
            pacienciaRestante = pacienciaMaximaActual;

            retratoClienteActivo = Resources.Load<Texture2D>(p.recursoSprite);
            if (retratoClienteActivo == null)
            {
                Sprite spRes = Resources.Load<Sprite>(p.recursoSprite);
                if (spRes != null) retratoClienteActivo = spRes.texture;
            }
            ConfigurarSpriteClienteEnPatio(p, retratoClienteActivo);
        }

        private void ConfigurarSpriteClienteEnPatio(PlantillaVecinoSanFelipe plantilla, Texture2D tex)
        {
            if (clientePatioGO == null)
            {
                clientePatioGO = new GameObject("Vecino_Patio_FranciaChiquita");
                clientePatioRenderer = clientePatioGO.AddComponent<SpriteRenderer>();
                clientePatioRenderer.sortingOrder = 5;

                GameObject goFondo = GameObject.Find("Fondo_SanFelipe_Taller");
                if (goFondo != null)
                {
                    SpriteRenderer srFondo = goFondo.GetComponent<SpriteRenderer>();
                    if (srFondo != null && srFondo.sharedMaterial != null)
                    {
                        clientePatioRenderer.sharedMaterial = srFondo.sharedMaterial;
                    }
                }
            }

            if (tex != null)
            {
                float ppu = Mathf.Max(160f, tex.height / 3.4f);
                Sprite sp = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.0f), ppu);
                clientePatioRenderer.sprite = sp;
                clientePatioRenderer.flipX = plantilla.miraOriginalmenteDerecha;
            }

            clientePatioGO.transform.position = posEntradaPatio;
            faseCaminataCliente = 0f;
        }

        private void AnimarClienteEnPatio()
        {
            if (clientePatioGO == null) return;

            Vector3 posActual = clientePatioGO.transform.position;
            float distX = Mathf.Abs(posActual.x - posMostradorTaller.x);

            if (distX > 0.05f)
            {
                faseCaminataCliente += Time.deltaTime * 11f;
                float nuevoX = Mathf.MoveTowards(posActual.x, posMostradorTaller.x, 4.4f * Time.deltaTime);
                float reboteY = Mathf.Abs(Mathf.Sin(faseCaminataCliente)) * 0.11f;
                clientePatioGO.transform.position = new Vector3(nuevoX, posMostradorTaller.y + reboteY, 0f);
                clientePatioGO.transform.localScale = Vector3.one;
            }
            else
            {
                float resp = 1.0f + Mathf.Sin(Time.time * 3.2f) * 0.018f;
                clientePatioGO.transform.position = posMostradorTaller;
                clientePatioGO.transform.localScale = new Vector3(1f, resp, 1f);
            }
        }

        private void RegistrarClienteImpaciente()
        {
            clientesPerdidos++;
            rachaPerfectas = 0;

            if (AudioColonialManager.Instance != null)
            {
                AudioColonialManager.Instance.ReproducirErrorPedido();
            }

            OnNotificacionFlotante?.Invoke("😠 ¡El vecino se retiró!", new Color(1f, 0.35f, 0.32f));
            ReiniciarMesa();

            if (clientesPerdidos >= maxClientesImpacientes)
            {
                FinalizarTaller(false, $"Se retiraron {clientesPerdidos} vecinos sin su pieza.");
            }
            else
            {
                GenerarSiguienteCliente();
            }
        }

        private void FinalizarTaller(bool esVictoria, string razonDerrota)
        {
            juegoActivo = false;
            juegoTerminado = true;
            victoriaAlcanzada = esVictoria;
            motivoDerrota = razonDerrota;

            if (esVictoria)
            {
                if (AudioColonialManager.Instance != null)
                {
                    AudioColonialManager.Instance.ReproducirCampanaGritoLibertad();
                }

                if (ProgresoJuegoManager.Instance != null)
                {
                    ProgresoJuegoManager.Instance.RegistrarMisionCompletada(TipoMision.SanFelipe_Alfareria, puntajeTotal);
                }

                if (RecursosCivicosManager.Instance != null)
                {
                    RecursosCivicosManager.Instance.AgregarRecursos(160);
                }
            }
            else
            {
                if (AudioColonialManager.Instance != null)
                {
                    AudioColonialManager.Instance.ReproducirDerrota();
                }
            }
        }

        #endregion

        #region Navegación de Escenas

        public void ReiniciarNivel()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void IrAlOverworld()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("Overworld_MapaGuanajuato");
        }

        public void IrAlMenuPrincipal()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("MainMenu");
        }

        #endregion
    }
}
