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
    /// y ensambla los personajes del paquete importado Custom Character 2D Vol 1
    /// para Don Chon y los comensales sobre el empedrado de la plaza.
    /// </summary>
    public class NeveriaGameManager : MonoBehaviour
    {
        public static NeveriaGameManager Instance { get; private set; }

        [Header("Objetivos de la Misión")]
        [SerializeField] private int clientesObjetivo = 8;
        [SerializeField] private int maxClientesPerdidos = 3;
        [SerializeField] private float duracionJornada = 140f;

        [Header("Puntos de Movimiento sobre el Empedrado")]
        [SerializeField] private Vector3 puntoSpawnCliente = new Vector3(7.2f, -2.28f, 0f);
        [SerializeField] private Vector3 puntoMostradorCliente = new Vector3(1.25f, -2.28f, 0f);
        [SerializeField] private Vector3 puntoSalidaCliente = new Vector3(7.8f, -2.28f, 0f);

        private SpriteRenderer fondoEscenarioRenderer;
        private GameObject goDonChon;

        // Caché de texturas y sprites de Custom Character 2D Vol 1 (exclusivo Nivel 2)
        private Dictionary<string, Texture2D> cacheTexturasCC = new Dictionary<string, Texture2D>();
        private Dictionary<string, Sprite> cacheSpritesCC = new Dictionary<string, Sprite>();

        private struct ConfiguracionPersonajeCC
        {
            public string body;
            public string hairBack;
            public string hairFront;
            public string ear;
            public Rect rectEyebrow;
            public Rect rectEye;
            public Rect rectMouth;
        }

        private List<ConfiguracionPersonajeCC> catalogoVariantesCC = new List<ConfiguracionPersonajeCC>();

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

            // Posiciones sobre la calle empedrada a la derecha del puesto
            puntoSpawnCliente = new Vector3(7.2f, -2.28f, 0f);
            puntoMostradorCliente = new Vector3(1.25f, -2.28f, 0f);
            puntoSalidaCliente = new Vector3(7.8f, -2.28f, 0f);
        }

        private void Start()
        {
            Time.timeScale = 1f;
            InicializarCatalogoCustomCharacterVol1();
            ConfigurarEscenarioColonial();
            StartCoroutine(GenerarSiguienteCliente(1.0f));
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
        /// Prepara las combinaciones de cuerpo, peinado y rostro del paquete Custom Character 2D Vol 1.
        /// </summary>
        private void InicializarCatalogoCustomCharacterVol1()
        {
            catalogoVariantesCC.Clear();

            // Coordenadas de sub-sprites en eye.png (540x300) y mouth.png (192x192)
            Rect cejaNormal = new Rect(0, 200, 180, 100);     // eye_0
            Rect cejaAmable = new Rect(180, 200, 180, 100);   // eye_1
            Rect cejaFirme = new Rect(360, 200, 180, 100);    // eye_2

            Rect ojosAbiertos = new Rect(180, 100, 180, 100); // eye_4
            Rect ojosAlegres = new Rect(0, 0, 180, 100);      // eye_6

            Rect bocaSonrisa = new Rect(64, 0, 64, 64);       // mouth_7
            Rect bocaAlegre = new Rect(0, 128, 64, 64);       // mouth_0
            Rect bocaSerena = new Rect(64, 64, 64, 64);       // mouth_4
            Rect bocaEntusiasmo = new Rect(0, 0, 64, 64);     // mouth_6

            // Variante 1: Casaca Azul, Cabello Rubio Lacio
            catalogoVariantesCC.Add(new ConfiguracionPersonajeCC
            {
                body = "body/body1",
                hairBack = "hair_back/h_b_b1",
                hairFront = "hair_front/h_f_b1",
                ear = "face/face_ear_00",
                rectEyebrow = cejaAmable,
                rectEye = ojosAbiertos,
                rectMouth = bocaSonrisa
            });

            // Variante 2: Casaca Roja, Cabello Oscuro Rebelde
            catalogoVariantesCC.Add(new ConfiguracionPersonajeCC
            {
                body = "body/body2",
                hairBack = "hair_back/h_b_a3",
                hairFront = "hair_front/h_f_a3",
                ear = "face/face_ear_00",
                rectEyebrow = cejaNormal,
                rectEye = ojosAbiertos,
                rectMouth = bocaAlegre
            });

            // Variante 3: Casaca Naranja, Cabello Castaño/Rojo Lacio
            catalogoVariantesCC.Add(new ConfiguracionPersonajeCC
            {
                body = "body/body3",
                hairBack = "hair_back/h_b_b2",
                hairFront = "hair_front/h_f_b2",
                ear = "face/face_ear_00",
                rectEyebrow = cejaNormal,
                rectEye = ojosAbiertos,
                rectMouth = bocaSonrisa
            });

            // Variante 4: Casaca Negra, Cabello Rubio Rebelde
            catalogoVariantesCC.Add(new ConfiguracionPersonajeCC
            {
                body = "body/body4",
                hairBack = "hair_back/h_b_a1",
                hairFront = "hair_front/h_f_a1",
                ear = "face/face_ear_00",
                rectEyebrow = cejaFirme,
                rectEye = ojosAbiertos,
                rectMouth = bocaSerena
            });

            // Variante 5: Casaca Roja, Cabello Pelirrojo Rebelde
            catalogoVariantesCC.Add(new ConfiguracionPersonajeCC
            {
                body = "body/body2",
                hairBack = "hair_back/h_b_a2",
                hairFront = "hair_front/h_f_a2",
                ear = "face/face_ear_00",
                rectEyebrow = cejaAmable,
                rectEye = ojosAlegres,
                rectMouth = bocaEntusiasmo
            });

            // Variante 6: Casaca Azul, Cabello Oscuro Lacio
            catalogoVariantesCC.Add(new ConfiguracionPersonajeCC
            {
                body = "body/body1",
                hairBack = "hair_back/h_b_b3",
                hairFront = "hair_front/h_f_b3",
                ear = "face/face_ear_00",
                rectEyebrow = cejaNormal,
                rectEye = ojosAbiertos,
                rectMouth = bocaAlegre
            });
        }

        /// <summary>
        /// Monta el escenario visual con el quiosco de Dolores Hidalgo y sitúa a Don Chon
        /// junto al mostrador usando Custom Character 2D Vol 1.
        /// </summary>
        private void ConfigurarEscenarioColonial()
        {
            // Limpiar cualquier objeto heredado fuera de lugar si existiera
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

            // 2. Colocar a Don Chon junto al puesto usando Custom Character 2D Vol 1
            if (goDonChon == null)
            {
                goDonChon = GameObject.Find("DonChon_ElNeveroMayor");
                if (goDonChon != null) Destroy(goDonChon);

                goDonChon = new GameObject("DonChon_ElNeveroMayor");
                goDonChon.transform.position = new Vector3(-0.35f, -2.28f, 0f);
                goDonChon.transform.localScale = new Vector3(0.135f, 0.135f, 1f);

                ConfiguracionPersonajeCC configDonChon = new ConfiguracionPersonajeCC
                {
                    body = "body/body3",
                    hairBack = "hair_back/h_b_b3",
                    hairFront = "hair_front/h_f_b3",
                    ear = "face/face_ear_00",
                    rectEyebrow = new Rect(0, 200, 180, 100),   // eye_0
                    rectEye = new Rect(180, 100, 180, 100),     // eye_4
                    rectMouth = new Rect(0, 0, 64, 64)          // mouth_6 (sonrisa cálida)
                };

                EnsamblarPersonajeCustomVol1(goDonChon, configDonChon, ordenBase: 5);
            }
        }

        /// <summary>
        /// Construye la jerarquía por capas (body, hair_back, face_ear, face_eye, face_bllow,
        /// face_mouth, hair_front) idéntica a sample.unity de Custom Character 2D Vol 1,
        /// con origen en la suela de las botas para caminar sobre el empedrado.
        /// </summary>
        private void EnsamblarPersonajeCustomVol1(GameObject raizPersonaje, ConfiguracionPersonajeCC config, int ordenBase)
        {
            // En sample.unity el borde inferior de las botas (body 624x2048 centrado en -3.65)
            // queda en y = -3.65 - 10.24 = -13.89. Desplazamos todas las capas +13.89 en Y
            // para que el punto (0,0,0) del objeto raíz sea exactamente la planta de los pies.
            Vector3 offsetPies = new Vector3(0.50f, 13.89f, 0f);

            CrearCapaSprite(
                raizPersonaje.transform,
                "body",
                ObtenerSpriteCompletoCC(config.body),
                new Vector3(-0.94f, -3.65f, 0f) + offsetPies,
                ordenBase + 0
            );

            CrearCapaSprite(
                raizPersonaje.transform,
                "hair_back",
                ObtenerSpriteCompletoCC(config.hairBack),
                new Vector3(-0.461f, 4.729f, 0f) + offsetPies,
                ordenBase + 1
            );

            CrearCapaSprite(
                raizPersonaje.transform,
                "face_ear",
                ObtenerSpriteCompletoCC(config.ear),
                new Vector3(-0.151f, 3.954f, 0f) + offsetPies,
                ordenBase + 2
            );

            CrearCapaSprite(
                raizPersonaje.transform,
                "face_eye",
                ObtenerSubSpriteCC("face/eye", config.rectEye),
                new Vector3(0.349f, 3.92f, 0f) + offsetPies,
                ordenBase + 2
            );

            CrearCapaSprite(
                raizPersonaje.transform,
                "face_bllow",
                ObtenerSubSpriteCC("face/eye", config.rectEyebrow),
                new Vector3(0.408f, 4.327f, 0f) + offsetPies,
                ordenBase + 2
            );

            CrearCapaSprite(
                raizPersonaje.transform,
                "face_mouth",
                ObtenerSubSpriteCC("face/mouth", config.rectMouth),
                new Vector3(0.167f, 3.104f, 0f) + offsetPies,
                ordenBase + 2
            );

            CrearCapaSprite(
                raizPersonaje.transform,
                "hair_front",
                ObtenerSpriteCompletoCC(config.hairFront),
                new Vector3(0.273f, 4.185f, 0f) + offsetPies,
                ordenBase + 3
            );
        }

        private void CrearCapaSprite(Transform padre, string nombreCapa, Sprite sprite, Vector3 posLocal, int sortingOrder)
        {
            if (sprite == null) return;
            GameObject goCapa = new GameObject(nombreCapa);
            goCapa.transform.SetParent(padre, false);
            goCapa.transform.localPosition = posLocal;
            goCapa.transform.localRotation = Quaternion.identity;
            goCapa.transform.localScale = Vector3.one;

            SpriteRenderer sr = goCapa.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;
            sr.sortingOrder = sortingOrder;
        }

        private Texture2D CargarTexturaCC(string subRutaSinExtension)
        {
            if (cacheTexturasCC.TryGetValue(subRutaSinExtension, out Texture2D texCache) && texCache != null)
            {
                return texCache;
            }

            // 1. Cargar desde Resources/CustomCharacter (compatible con WebGL y Editor)
            Texture2D tex = Resources.Load<Texture2D>("CustomCharacter/" + subRutaSinExtension);
            if (tex != null)
            {
                cacheTexturasCC[subRutaSinExtension] = tex;
                return tex;
            }

#if UNITY_EDITOR || UNITY_STANDALONE
            // 2. Fallback directo desde Assets/Custom Character_Vol1/png/character/
            string rutaArchivo = System.IO.Path.Combine(Application.dataPath, "Custom Character_Vol1/png/character", subRutaSinExtension + ".png");
            if (System.IO.File.Exists(rutaArchivo))
            {
                byte[] bytes = System.IO.File.ReadAllBytes(rutaArchivo);
                Texture2D nuevaTex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                if (ImageConversion.LoadImage(nuevaTex, bytes))
                {
                    nuevaTex.filterMode = FilterMode.Bilinear;
                    nuevaTex.wrapMode = TextureWrapMode.Clamp;
                    cacheTexturasCC[subRutaSinExtension] = nuevaTex;
                    return nuevaTex;
                }
            }
#endif
            Debug.LogWarning($"[Neveria] No se pudo cargar textura de Custom Character Vol 1: {subRutaSinExtension}");
            return null;
        }

        private Sprite ObtenerSpriteCompletoCC(string subRutaSinExtension)
        {
            if (cacheSpritesCC.TryGetValue(subRutaSinExtension, out Sprite spCache) && spCache != null)
            {
                return spCache;
            }

            // Intentar cargar como Sprite directo desde Resources
            Sprite spRes = Resources.Load<Sprite>("CustomCharacter/" + subRutaSinExtension);
            if (spRes != null)
            {
                cacheSpritesCC[subRutaSinExtension] = spRes;
                return spRes;
            }

            Texture2D tex = CargarTexturaCC(subRutaSinExtension);
            if (tex == null) return null;

            Sprite nuevoSp = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100f);
            cacheSpritesCC[subRutaSinExtension] = nuevoSp;
            return nuevoSp;
        }

        private Sprite ObtenerSubSpriteCC(string subRutaSinExtension, Rect rectSubSprite)
        {
            string clave = $"{subRutaSinExtension}_{rectSubSprite.x}_{rectSubSprite.y}_{rectSubSprite.width}_{rectSubSprite.height}";
            if (cacheSpritesCC.TryGetValue(clave, out Sprite spCache) && spCache != null)
            {
                return spCache;
            }

            Texture2D tex = CargarTexturaCC(subRutaSinExtension);
            if (tex == null) return null;

            Sprite subSp = Sprite.Create(tex, rectSubSprite, new Vector2(0.5f, 0.5f), 100f);
            cacheSpritesCC[clave] = subSp;
            return subSp;
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

            float pacienciaBase = Mathf.Max(26f, 36f - (clientesAtendidos * 0.8f));
            PedidoNieve nuevoPedido = new PedidoNieve(rec, sabor, nombre, pacienciaBase);

            GameObject go = new GameObject($"Cliente_{nombre}");
            go.transform.position = puntoSpawnCliente;

            if (catalogoVariantesCC.Count > 0)
            {
                ConfiguracionPersonajeCC configElegida = catalogoVariantesCC[(clientesAtendidos + clientesPerdidos) % catalogoVariantesCC.Count];
                EnsamblarPersonajeCustomVol1(go, configElegida, ordenBase: 10);
            }

            clienteActual = go.AddComponent<ClienteTurista>();
            clienteActual.Configurar(nuevoPedido, puntoMostradorCliente, puntoSalidaCliente, escalaPersonaje: 0.135f);
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
