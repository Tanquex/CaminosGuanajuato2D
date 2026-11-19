using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using CaminosDeGuanajuato.Core;

namespace CaminosDeGuanajuato.Duelo
{
    public enum EstadoDueloMusical
    {
        DialogoIntroduccion,
        DialogoRetoRival,
        CuentaRegresiva,
        TocandoSon,
        RondaSuperada,
        VictoriaFinal,
        Derrota
    }

    public enum CalificacionNota
    {
        Ninguna,
        Perfecto,
        Bien,
        Fallo
    }

    [Serializable]
    public class NotaRitmica
    {
        public int carril;          // 0: Rojo [A/⬅], 1: Amarillo [S/⬇], 2: Verde [D/⬆], 3: Azul [F/➡]
        public float tiempoImpacto; // Segundo exacto dentro de la canción
        public bool procesada;
        public bool acertada;
        public CalificacionNota calificacion;
    }

    /// <summary>
    /// Administrador central del Nivel 3: Duelo en la Plaza (Estilo Guitar Hero / Festival de Sones Insurgentes).
    /// Controla las partituras de los 3 sones tradicionales contra Don Silvestre, el Joven Miliciano y el Capitán de Dragones,
    /// el medidor de Fervor de la Plaza, las rachas de combo (x1 a x4) y el desbloqueo del Salvoconducto a San Felipe.
    /// </summary>
    public class DueloArenaManager : MonoBehaviour
    {
        public static DueloArenaManager Instance { get; private set; }

        [Header("Configuración de Rondas Musicales")]
        [SerializeField] private int rondaActual = 1;
        [SerializeField] private int totalRondas = 3;

        [Header("Posiciones en la Tarima de la Plaza")]
        [SerializeField] private Vector3 puntoSpawnJugador = new Vector3(-4.1f, -2.35f, 0f);
        [SerializeField] private Vector3 puntoSpawnRival = new Vector3(4.1f, -2.35f, 0f);

        [Header("Referencias de Escena")]
        [SerializeField] private GuanajichoDueloController jugador;
        [SerializeField] private RivalDueloIA rivalActual;

        [Header("Estadísticas del Duelo Musical")]
        [SerializeField] private int puntajeTotal = 0;
        [SerializeField] private int comboActual = 0;
        [SerializeField] private int comboMaximo = 0;
        [SerializeField] private int multiplicador = 1;
        [SerializeField] private float fervorPlaza = 65f; // 0 a 100 (empieza a favor de Guanajicho)
        [SerializeField] private int notasPerfectas = 0;
        [SerializeField] private int notasBuenas = 0;
        [SerializeField] private int notasFalladas = 0;

        // Ventanas de tiempo (segundos) para acertar notas estilo Guitar Hero
        private const float VENTANA_PERFECTO = 0.125f;
        private const float VENTANA_BIEN = 0.255f;

        private EstadoDueloMusical estado = EstadoDueloMusical.DialogoIntroduccion;
        private List<NotaRitmica> partituraActual = new List<NotaRitmica>();
        private float tiempoCancion = 0f;
        private float duracionTotalCancion = 20f;
        private float velocidadCaidaNotas = 250f; // Píxeles por segundo en el diapasón
        private float valorCuentaRegresiva = 3f;
        private string motivoDerrota = "";

        // Feedback visual instantáneo por carril al presionar
        private float[] tiempoDestelloCarril = new float[4];
        private CalificacionNota[] ultimaCalificacionCarril = new CalificacionNota[4];
        private string textoUltimoJuicio = "";
        private Color colorUltimoJuicio = Color.white;
        private float tiempoUltimoJuicio = -10f;

        public EstadoDueloMusical Estado => estado;
        public int RondaActual => rondaActual;
        public int TotalRondas => totalRondas;
        public int PuntajeTotal => puntajeTotal;
        public int ComboActual => comboActual;
        public int ComboMaximo => comboMaximo;
        public int Multiplicador => multiplicador;
        public float FervorPlaza => fervorPlaza;
        public int NotasPerfectas => notasPerfectas;
        public int NotasBuenas => notasBuenas;
        public int NotasFalladas => notasFalladas;
        public List<NotaRitmica> PartituraActual => partituraActual;
        public float TiempoCancion => tiempoCancion;
        public float DuracionTotalCancion => duracionTotalCancion;
        public float VelocidadCaidaNotas => velocidadCaidaNotas;
        public float ValorCuentaRegresiva => valorCuentaRegresiva;
        public string MotivoDerrota => motivoDerrota;
        public GuanajichoDueloController Jugador => jugador;
        public RivalDueloIA RivalActual => rivalActual;
        public string TextoUltimoJuicio => textoUltimoJuicio;
        public Color ColorUltimoJuicio => colorUltimoJuicio;
        public float TiempoUltimoJuicio => tiempoUltimoJuicio;

        public float PorcentajePrecision
        {
            get
            {
                int totalIntentos = notasPerfectas + notasBuenas + notasFalladas;
                if (totalIntentos <= 0) return 100f;
                return Mathf.Round(((float)(notasPerfectas + notasBuenas) / totalIntentos) * 100f);
            }
        }

        // Eventos para efectos visuales en el HUD
        public event Action<int, CalificacionNota> OnNotaEvaluada;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            Time.timeScale = 1f;
            AplicarFondoEspectacularNivel3();
            AsegurarReferencias();
            PrepararRivalDeRonda(1);
            estado = EstadoDueloMusical.DialogoIntroduccion;
        }

        private void AplicarFondoEspectacularNivel3()
        {
            GameObject goFondo = GameObject.Find("Fondo_Atrio_Plaza");
            if (goFondo == null)
            {
                goFondo = new GameObject("Fondo_Atrio_Plaza");
                goFondo.transform.position = Vector3.zero;
            }

            SpriteRenderer srFondo = goFondo.GetComponent<SpriteRenderer>();
            if (srFondo == null) srFondo = goFondo.AddComponent<SpriteRenderer>();
            srFondo.sortingOrder = -10;

            Sprite sprPlazaNocturna = CargarSpriteFlexible("FondoPlazaDueloMusical", 64f, new Vector2(0.5f, 0.5f));
            if (sprPlazaNocturna != null)
            {
                srFondo.sprite = sprPlazaNocturna;
                // Ajustar escala para cubrir toda la cámara ortográfica (size 5 -> alto 10, ancho ~17.8)
                float anchoSprite = sprPlazaNocturna.bounds.size.x;
                float altoSprite = sprPlazaNocturna.bounds.size.y;
                if (anchoSprite > 0.1f && altoSprite > 0.1f)
                {
                    float escalaX = 18.2f / anchoSprite;
                    float escalaY = 10.4f / altoSprite;
                    float escalaUniforme = Mathf.Max(escalaX, escalaY);
                    goFondo.transform.localScale = new Vector3(escalaUniforme, escalaUniforme, 1f);
                }
            }

            // Ubicar espectadores Don Chon y Doña Remedios a los lados de la tarima
            GameObject goDonChon = GameObject.Find("Espectador_DonChon");
            if (goDonChon != null) goDonChon.transform.position = new Vector3(-6.8f, -2.55f, 0f);

            GameObject goDonaRemedios = GameObject.Find("Espectador_DonaRemedios");
            if (goDonaRemedios != null) goDonaRemedios.transform.position = new Vector3(6.8f, -2.55f, 0f);
        }

        private void AsegurarReferencias()
        {
            if (jugador == null)
            {
                jugador = FindAnyObjectByType<GuanajichoDueloController>();
                if (jugador == null)
                {
                    GameObject goJugador = GameObject.Find("Guanajicho_Duelo");
                    if (goJugador != null) jugador = goJugador.GetComponent<GuanajichoDueloController>();
                }
            }

            if (jugador != null)
            {
                jugador.transform.position = puntoSpawnJugador;
                jugador.CargarSpriteTamborilero();
            }
        }

        private void Update()
        {
            if (Time.timeScale == 0f) return;

            if (estado == EstadoDueloMusical.TocandoSon)
            {
                ActualizarProgresoCancion();
                ProcesarEntradasTecladoCarriles();
            }
        }

        public void FinalizarDialogoIntroduccion()
        {
            rondaActual = 1;
            puntajeTotal = 0;
            comboActual = 0;
            comboMaximo = 0;
            multiplicador = 1;
            notasPerfectas = 0;
            notasBuenas = 0;
            notasFalladas = 0;

            PrepararRivalDeRonda(1);
            estado = EstadoDueloMusical.DialogoRetoRival;
        }

        public void FinalizarDialogoRetoRival()
        {
            StartCoroutine(RutinaCuentaRegresivaYArranque());
        }

        private IEnumerator RutinaCuentaRegresivaYArranque()
        {
            estado = EstadoDueloMusical.CuentaRegresiva;
            fervorPlaza = 68f;
            tiempoCancion = 0f;

            for (int i = 3; i >= 1; i--)
            {
                valorCuentaRegresiva = i;
                if (AudioColonialManager.Instance != null) AudioColonialManager.Instance.ReproducirClickBoton();
                yield return new WaitForSeconds(0.65f);
            }

            valorCuentaRegresiva = 0f;
            if (AudioColonialManager.Instance != null) AudioColonialManager.Instance.ReproducirColocarDefensa();
            estado = EstadoDueloMusical.TocandoSon;
        }

        public void PrepararRivalDeRonda(int numeroRonda)
        {
            rondaActual = Mathf.Clamp(numeroRonda, 1, totalRondas);

            if (rivalActual != null)
            {
                Destroy(rivalActual.gameObject);
            }

            GameObject goRival = new GameObject($"Rival_Musical_Ronda_{rondaActual}");
            goRival.transform.position = puntoSpawnRival;
            SpriteRenderer sr = goRival.AddComponent<SpriteRenderer>();
            sr.sortingOrder = 10;

            rivalActual = goRival.AddComponent<RivalDueloIA>();

            switch (rondaActual)
            {
                case 1:
                    Sprite sprSilvestre = CargarSpriteFlexible("PersonajesColonial/DonSilvestre", 380f, new Vector2(0.5f, 0f));
                    Texture2D texSilvestre = Resources.Load<Texture2D>("PersonajesColonial/DonSilvestre");
                    rivalActual.Configurar(
                        "Don Silvestre",
                        "Arriero Coplero de la Sierra",
                        "Son del Arriero Serranero",
                        96f,
                        sprSilvestre,
                        texSilvestre,
                        invertirX: false
                    );
                    velocidadCaidaNotas = 235f;
                    break;

                case 2:
                    Sprite sprAlfarero = CargarSpriteFlexible("PersonajesColonial/JovenAlfarero", 380f, new Vector2(0.5f, 0f));
                    Texture2D texAlfarero = Resources.Load<Texture2D>("PersonajesColonial/JovenAlfarero");
                    rivalActual.Configurar(
                        "Joven Miliciano",
                        "Jarabero Rebelde de Dolores",
                        "Jarabe de los Insurgentes",
                        112f,
                        sprAlfarero,
                        texAlfarero,
                        invertirX: false
                    );
                    velocidadCaidaNotas = 275f;
                    break;

                case 3:
                default:
                    Sprite sprCapitan = CargarSpriteFlexible("PersonajesColonial/CapitanRealista", 380f, new Vector2(0.5f, 0f));
                    Texture2D texCapitan = Resources.Load<Texture2D>("PersonajesColonial/CapitanRealista");
                    rivalActual.Configurar(
                        "Capitán de Dragones",
                        "Director de la Banda del Regimiento",
                        "Marcha de Dragones y Libertad",
                        126f,
                        sprCapitan,
                        texCapitan,
                        invertirX: false
                    );
                    velocidadCaidaNotas = 315f;
                    break;
            }

            if (jugador != null)
            {
                jugador.transform.position = puntoSpawnJugador;
                jugador.ConfigurarBPM(rivalActual.BpmSon);
                jugador.PonerEnGuardia();
            }

            GenerarPartituraParaRonda(rondaActual);
        }

        private void GenerarPartituraParaRonda(int ronda)
        {
            partituraActual.Clear();

            if (ronda == 1)
            {
                // Ronda 1: 16 notas muy claras y rítmicas (cada ~0.85s) para aprender con facilidad
                int[] secuenciaCarriles = { 0, 1, 2, 3, 2, 1, 0, 1, 2, 3, 1, 2, 0, 3, 1, 2 };
                float t = 1.6f;
                for (int i = 0; i < secuenciaCarriles.Length; i++)
                {
                    partituraActual.Add(new NotaRitmica
                    {
                        carril = secuenciaCarriles[i],
                        tiempoImpacto = t
                    });
                    t += (i % 4 == 3) ? 1.05f : 0.78f;
                }
                duracionTotalCancion = t + 1.2f;
            }
            else if (ronda == 2)
            {
                // Ronda 2: 22 notas con ritmo de Jarabe (escaleras y alternancias alegres)
                int[] secuenciaCarriles = { 0, 2, 1, 3, 0, 1, 2, 3, 3, 2, 1, 0, 1, 2, 1, 3, 0, 2, 3, 1, 2, 0 };
                float t = 1.4f;
                for (int i = 0; i < secuenciaCarriles.Length; i++)
                {
                    partituraActual.Add(new NotaRitmica
                    {
                        carril = secuenciaCarriles[i],
                        tiempoImpacto = t
                    });
                    t += (i % 6 == 5) ? 0.90f : 0.64f;
                }
                duracionTotalCancion = t + 1.2f;
            }
            else
            {
                // Ronda 3: 28 notas estelares contra el Capitán de Dragones
                int[] secuenciaCarriles =
                {
                    0, 1, 2, 3, 3, 1, 2, 0,
                    1, 3, 0, 2, 0, 1, 2, 3,
                    2, 1, 0, 2, 3, 1, 0, 3,
                    0, 1, 2, 3
                };
                float t = 1.3f;
                for (int i = 0; i < secuenciaCarriles.Length; i++)
                {
                    partituraActual.Add(new NotaRitmica
                    {
                        carril = secuenciaCarriles[i],
                        tiempoImpacto = t
                    });
                    t += (i % 4 == 3) ? 0.78f : 0.54f;
                }
                duracionTotalCancion = t + 1.2f;
            }
        }

        private void ProcesarEntradasTecladoCarriles()
        {
            // Carril 0 (Rojo): [A], [Flecha Izquierda] o [1]
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.Alpha1))
            {
                ActivarCarril(0);
            }

            // Carril 1 (Amarillo): [S], [Flecha Abajo] o [2]
            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.Alpha2))
            {
                ActivarCarril(1);
            }

            // Carril 2 (Verde): [D], [Flecha Arriba] o [3]
            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Alpha3))
            {
                ActivarCarril(2);
            }

            // Carril 3 (Azul): [F], [Flecha Derecha] o [4]
            if (Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.Alpha4))
            {
                ActivarCarril(3);
            }
        }

        /// <summary>
        /// Llamado al pulsar la tecla de un carril o hacer clic sobre su aro receptor en la base del diapasón.
        /// </summary>
        public void ActivarCarril(int carril)
        {
            if (estado != EstadoDueloMusical.TocandoSon) return;
            carril = Mathf.Clamp(carril, 0, 3);
            tiempoDestelloCarril[carril] = Time.time;

            // Buscar la nota no procesada más cercana en este carril
            NotaRitmica mejorNota = null;
            float menorDiferencia = float.MaxValue;

            for (int i = 0; i < partituraActual.Count; i++)
            {
                NotaRitmica n = partituraActual[i];
                if (n.procesada || n.carril != carril) continue;

                float diff = Mathf.Abs(n.tiempoImpacto - tiempoCancion);
                if (diff < menorDiferencia)
                {
                    menorDiferencia = diff;
                    mejorNota = n;
                }
            }

            if (mejorNota != null && menorDiferencia <= VENTANA_BIEN)
            {
                mejorNota.procesada = true;
                mejorNota.acertada = true;

                bool esPerfecto = (menorDiferencia <= VENTANA_PERFECTO);
                mejorNota.calificacion = esPerfecto ? CalificacionNota.Perfecto : CalificacionNota.Bien;

                RegistrarAcierto(carril, esPerfecto);
            }
            else
            {
                // Si presionó cuando no había ninguna nota cerca (más de 0.45s de distancia), penalizar levemente
                if (menorDiferencia > 0.42f)
                {
                    RegistrarPulsacionEnVacio(carril);
                }
            }
        }

        private void RegistrarAcierto(int carril, bool esPerfecto)
        {
            comboActual++;
            if (comboActual > comboMaximo) comboMaximo = comboActual;

            multiplicador = 1 + Mathf.Clamp(comboActual / 5, 0, 3); // x1, x2, x3, x4

            if (esPerfecto)
            {
                notasPerfectas++;
                puntajeTotal += 100 * multiplicador;
                fervorPlaza = Mathf.Clamp(fervorPlaza + 6.5f, 0f, 100f);
                MostrarJuicio("🔥 ¡PERFECTO!", new Color(1.0f, 0.92f, 0.25f));
                ultimaCalificacionCarril[carril] = CalificacionNota.Perfecto;
            }
            else
            {
                notasBuenas++;
                puntajeTotal += 60 * multiplicador;
                fervorPlaza = Mathf.Clamp(fervorPlaza + 4.0f, 0f, 100f);
                MostrarJuicio("✨ ¡BIEN!", new Color(0.35f, 0.98f, 0.50f));
                ultimaCalificacionCarril[carril] = CalificacionNota.Bien;
            }

            if (AudioColonialManager.Instance != null)
            {
                AudioColonialManager.Instance.ReproducirNotaMusical(carril, esPerfecto);
            }

            if (jugador != null)
            {
                jugador.AnimarToqueNota(carril, esPerfecto);
            }

            if (comboActual % 6 == 0 && rivalActual != null)
            {
                rivalActual.ReaccionarComboJugador();
            }

            OnNotaEvaluada?.Invoke(carril, ultimaCalificacionCarril[carril]);
        }

        private void RegistrarPulsacionEnVacio(int carril)
        {
            comboActual = 0;
            multiplicador = 1;
            fervorPlaza = Mathf.Clamp(fervorPlaza - 4.0f, 0f, 100f);
            ultimaCalificacionCarril[carril] = CalificacionNota.Fallo;
            MostrarJuicio("¡DESTIEMPO!", new Color(1.0f, 0.50f, 0.30f));

            if (AudioColonialManager.Instance != null)
            {
                AudioColonialManager.Instance.ReproducirErrorPedido();
            }

            VerificarEstadoFervor();
        }

        private void ActualizarProgresoCancion()
        {
            tiempoCancion += Time.deltaTime;

            // Verificar notas que se pasaron de la línea de impacto sin ser tocadas
            bool todasProcesadas = true;
            for (int i = 0; i < partituraActual.Count; i++)
            {
                NotaRitmica n = partituraActual[i];
                if (!n.procesada)
                {
                    if (tiempoCancion - n.tiempoImpacto > VENTANA_BIEN)
                    {
                        n.procesada = true;
                        n.acertada = false;
                        n.calificacion = CalificacionNota.Fallo;
                        RegistrarNotaPerdida(n.carril);
                    }
                    else
                    {
                        todasProcesadas = false;
                    }
                }
            }

            if (estado != EstadoDueloMusical.TocandoSon) return;

            if (todasProcesadas || tiempoCancion >= duracionTotalCancion)
            {
                CompletarRondaMusical();
            }
        }

        private void RegistrarNotaPerdida(int carril)
        {
            notasFalladas++;
            comboActual = 0;
            multiplicador = 1;
            fervorPlaza = Mathf.Clamp(fervorPlaza - 9.0f, 0f, 100f);
            ultimaCalificacionCarril[carril] = CalificacionNota.Fallo;
            MostrarJuicio("❌ ¡SE ESCAPÓ!", new Color(1.0f, 0.32f, 0.32f));

            if (jugador != null) jugador.AnimarFalloNota();
            OnNotaEvaluada?.Invoke(carril, CalificacionNota.Fallo);

            VerificarEstadoFervor();
        }

        private void VerificarEstadoFervor()
        {
            if (fervorPlaza <= 0f)
            {
                fervorPlaza = 0f;
                estado = EstadoDueloMusical.Derrota;
                string nomRival = rivalActual != null ? rivalActual.NombreRival : "el rival";
                motivoDerrota = $"El público perdió el entusiasmo del son frente a {nomRival}.";

                if (AudioColonialManager.Instance != null)
                {
                    AudioColonialManager.Instance.ReproducirDerrota();
                }
            }
        }

        private void CompletarRondaMusical()
        {
            estado = EstadoDueloMusical.RondaSuperada;
            puntajeTotal += 250;

            if (rivalActual != null) rivalActual.AnimarRivalSuperado();
            if (AudioColonialManager.Instance != null) AudioColonialManager.Instance.ReproducirPedidoCompletado();

            StartCoroutine(RutinaAvanzarSiguienteSon());
        }

        private IEnumerator RutinaAvanzarSiguienteSon()
        {
            yield return new WaitForSeconds(2.1f);

            if (rondaActual >= totalRondas)
            {
                DeclararVictoriaFinal();
            }
            else
            {
                PrepararRivalDeRonda(rondaActual + 1);
                estado = EstadoDueloMusical.DialogoRetoRival;
            }
        }

        private void DeclararVictoriaFinal()
        {
            estado = EstadoDueloMusical.VictoriaFinal;
            puntajeTotal += 500;

            if (ProgresoJuegoManager.Instance != null)
            {
                ProgresoJuegoManager.Instance.RegistrarMisionCompletada(TipoMision.Dolores_Duelo, puntajeTotal);
            }
            else
            {
                PlayerPrefs.SetInt(ProgresoJuegoManager.KEY_MISION3, 1);
                PlayerPrefs.Save();
            }

            if (AudioColonialManager.Instance != null)
            {
                AudioColonialManager.Instance.ReproducirCampanaGritoLibertad();
                AudioColonialManager.Instance.ReproducirMonedaRecogida();
            }
        }

        private void MostrarJuicio(string texto, Color color)
        {
            textoUltimoJuicio = texto;
            colorUltimoJuicio = color;
            tiempoUltimoJuicio = Time.time;
        }

        public float ObtenerIntensidadDestelloCarril(int carril)
        {
            float elapsed = Time.time - tiempoDestelloCarril[Mathf.Clamp(carril, 0, 3)];
            if (elapsed < 0f || elapsed > 0.22f) return 0f;
            return 1f - (elapsed / 0.22f);
        }

        private Sprite CargarSpriteFlexible(string rutaResources, float ppu, Vector2 pivot)
        {
            Sprite spr = Resources.Load<Sprite>(rutaResources);
            if (spr != null) return spr;

            Texture2D tex = Resources.Load<Texture2D>(rutaResources);
            if (tex != null)
            {
                return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), pivot, ppu);
            }
            return null;
        }

        public void ReintentarRondaActual()
        {
            Time.timeScale = 1f;
            comboActual = 0;
            multiplicador = 1;
            PrepararRivalDeRonda(rondaActual);
            StartCoroutine(RutinaCuentaRegresivaYArranque());
        }

        public void ReiniciarDuelo()
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
    }
}
