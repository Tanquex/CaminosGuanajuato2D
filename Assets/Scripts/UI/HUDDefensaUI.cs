using UnityEngine;
using UnityEngine.SceneManagement;
using CaminosDeGuanajuato.Core;
using CaminosDeGuanajuato.Defense;
using CaminosDeGuanajuato.Player;

namespace CaminosDeGuanajuato.UI
{
    /// <summary>
    /// Interfaz de Usuario (HUD) para Caminos de Guanajuato 2D:
    /// - Banner de Bienvenida cinemático (Título del juego y Nivel 1).
    /// - Temporizador automático de oleadas con cuenta regresiva.
    /// - Recursos cívicos y vida de la Antorcha Libertaria en vivo.
    /// - Pantalla de Derrota (LOSE) con botón de Reiniciar Nivel.
    /// - Pantalla de Victoria al superar las 5 oleadas.
    /// - Selector interactivo de defensas y guía de controles.
    /// </summary>
    public class HUDDefensaUI : MonoBehaviour
    {
        [Header("Ajustes de Banner Inicial")]
        [SerializeField] private float duracionBannerInicio = 4.5f;

        [Header("Guia de Controles")]
        [SerializeField] private bool mostrarGuiaControles = true;

        // Estado del Banner y Alertas
        private float tiempoTranscurridoBanner;
        private bool bannerCerradoManualmente;
        private string alertaInvasionTexto = "";
        private float alertaInvasionTiempo = 0f;
        private GuanajichoController player;

        // Estado de Pausa y Opciones
        private bool estaPausado = false;
        private bool mostrarOpcionesPausa = false;
        private bool sonidoDerrotaReproducido = false;

        private float volumenGeneral = 0.85f;
        private float volumenMusica = 0.65f;
        private float volumenSFX = 0.85f;
        private bool pantallaCompleta = true;

        private void OnEnable()
        {
            OleadasSpawner.OnAvisoInvasion += ManejarAvisoInvasion;
        }

        private void OnDisable()
        {
            OleadasSpawner.OnAvisoInvasion -= ManejarAvisoInvasion;
        }

        private void ManejarAvisoInvasion(string mensaje)
        {
            alertaInvasionTexto = mensaje;
            alertaInvasionTiempo = 5.0f;
        }

        // Estilos GUI
        private GUIStyle estiloCajaOscura;
        private GUIStyle estiloCajaDerrota;
        private GUIStyle estiloCajaVictoria;
        private GUIStyle estiloTituloBanner;
        private GUIStyle estiloNivelBanner;
        private GUIStyle estiloTextoBanner;
        private GUIStyle estiloTituloHUD;
        private GUIStyle estiloSubtituloHUD;
        private GUIStyle estiloRecursos;
        private GUIStyle estiloTemporizador;
        private GUIStyle estiloBoton;
        private GUIStyle estiloBotonActivo;
        private GUIStyle estiloBotonReinicio;
        private GUIStyle estiloControles;

        // Texturas
        private Texture2D texturaFondoColonial;
        private Texture2D texturaFondoOverlay;
        private Texture2D texturaFondoDerrota;
        private Texture2D texturaFondoVictoria;
        private Texture2D texturaBarraVerde;
        private Texture2D texturaBarraRoja;

        private void Start()
        {
            CrearTexturas();
            tiempoTranscurridoBanner = 0f;
            Time.timeScale = 1f; // Asegurar tiempo normal al cargar
            pantallaCompleta = Screen.fullScreen;

            if (AudioColonialManager.Instance != null)
            {
                volumenGeneral = AudioColonialManager.Instance.VolumenGeneral;
                volumenMusica = AudioColonialManager.Instance.VolumenMusica;
                volumenSFX = AudioColonialManager.Instance.VolumenSFX;
            }
        }

        private void Update()
        {
            if (alertaInvasionTiempo > 0f)
            {
                alertaInvasionTiempo -= Time.deltaTime;
            }

            if (Input.GetKeyDown(KeyCode.H))
            {
                mostrarGuiaControles = !mostrarGuiaControles;
                if (AudioColonialManager.Instance != null) AudioColonialManager.Instance.ReproducirClickBoton();
            }

            bool estaDerrotado = AntorchaLibertaria.Instance != null && AntorchaLibertaria.Instance.EstaExtinguida;
            bool estaVictoria = OleadasSpawner.Instance != null && OleadasSpawner.Instance.TodasOleadasCompletadas;

            // Atajo de Pausa / Escape
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
            {
                if (!estaDerrotado && !estaVictoria)
                {
                    if (mostrarOpcionesPausa)
                    {
                        mostrarOpcionesPausa = false;
                        if (AudioColonialManager.Instance != null) AudioColonialManager.Instance.ReproducirClickBoton();
                    }
                    else
                    {
                        AlternarPausa();
                    }
                }
            }

            if (tiempoTranscurridoBanner < duracionBannerInicio && !bannerCerradoManualmente)
            {
                tiempoTranscurridoBanner += Time.unscaledDeltaTime;
                if (Input.anyKeyDown)
                {
                    bannerCerradoManualmente = true;
                }
            }
        }

        private void CrearTexturas()
        {
            texturaFondoColonial = new Texture2D(1, 1);
            texturaFondoColonial.SetPixel(0, 0, new Color(0.12f, 0.08f, 0.05f, 0.92f));
            texturaFondoColonial.Apply();

            texturaFondoOverlay = new Texture2D(1, 1);
            texturaFondoOverlay.SetPixel(0, 0, new Color(0.04f, 0.02f, 0.02f, 0.78f));
            texturaFondoOverlay.Apply();

            texturaFondoDerrota = new Texture2D(1, 1);
            texturaFondoDerrota.SetPixel(0, 0, new Color(0.22f, 0.04f, 0.04f, 0.96f)); // Carmesi colonial oscuro
            texturaFondoDerrota.Apply();

            texturaFondoVictoria = new Texture2D(1, 1);
            texturaFondoVictoria.SetPixel(0, 0, new Color(0.04f, 0.18f, 0.08f, 0.96f)); // Verde mayolica oscuro
            texturaFondoVictoria.Apply();

            texturaBarraVerde = new Texture2D(1, 1);
            texturaBarraVerde.SetPixel(0, 0, new Color(0.2f, 0.8f, 0.3f, 1f));
            texturaBarraVerde.Apply();

            texturaBarraRoja = new Texture2D(1, 1);
            texturaBarraRoja.SetPixel(0, 0, new Color(0.85f, 0.18f, 0.15f, 1f));
            texturaBarraRoja.Apply();
        }

        private void InicializarEstilos()
        {
            if (estiloTituloHUD != null) return;

            estiloCajaOscura = new GUIStyle(GUI.skin.box);
            estiloCajaOscura.normal.background = texturaFondoColonial;

            estiloCajaDerrota = new GUIStyle(GUI.skin.box);
            estiloCajaDerrota.normal.background = texturaFondoDerrota;

            estiloCajaVictoria = new GUIStyle(GUI.skin.box);
            estiloCajaVictoria.normal.background = texturaFondoVictoria;

            // Titulos Banner
            estiloTituloBanner = new GUIStyle(GUI.skin.label);
            estiloTituloBanner.fontSize = 24;
            estiloTituloBanner.fontStyle = FontStyle.Bold;
            estiloTituloBanner.alignment = TextAnchor.MiddleCenter;
            estiloTituloBanner.normal.textColor = new Color(0.98f, 0.72f, 0.15f); // Dorado virreinal

            estiloNivelBanner = new GUIStyle(GUI.skin.label);
            estiloNivelBanner.fontSize = 18;
            estiloNivelBanner.fontStyle = FontStyle.Bold;
            estiloNivelBanner.alignment = TextAnchor.MiddleCenter;
            estiloNivelBanner.normal.textColor = new Color(0.9f, 0.35f, 0.2f); // Terracota

            estiloTextoBanner = new GUIStyle(GUI.skin.label);
            estiloTextoBanner.fontSize = 13;
            estiloTextoBanner.alignment = TextAnchor.MiddleCenter;
            estiloTextoBanner.wordWrap = true;
            estiloTextoBanner.normal.textColor = new Color(0.92f, 0.92f, 0.92f);

            // HUD Principal
            estiloTituloHUD = new GUIStyle(GUI.skin.label);
            estiloTituloHUD.fontSize = 16;
            estiloTituloHUD.fontStyle = FontStyle.Bold;
            estiloTituloHUD.normal.textColor = new Color(0.96f, 0.65f, 0.08f);

            estiloSubtituloHUD = new GUIStyle(GUI.skin.label);
            estiloSubtituloHUD.fontSize = 12;
            estiloSubtituloHUD.fontStyle = FontStyle.Italic;
            estiloSubtituloHUD.normal.textColor = new Color(0.8f, 0.8f, 0.8f);

            estiloRecursos = new GUIStyle(GUI.skin.label);
            estiloRecursos.fontSize = 15;
            estiloRecursos.fontStyle = FontStyle.Bold;
            estiloRecursos.normal.textColor = new Color(1f, 0.92f, 0.45f);

            estiloTemporizador = new GUIStyle(GUI.skin.label);
            estiloTemporizador.fontSize = 13;
            estiloTemporizador.fontStyle = FontStyle.Bold;
            estiloTemporizador.normal.textColor = new Color(0.3f, 0.95f, 1f); // Cian brillante

            estiloBoton = new GUIStyle(GUI.skin.button);
            estiloBoton.fontSize = 12;
            estiloBoton.fontStyle = FontStyle.Bold;
            estiloBoton.normal.textColor = Color.white;

            estiloBotonActivo = new GUIStyle(estiloBoton);
            estiloBotonActivo.normal.textColor = new Color(0.2f, 1f, 0.4f);

            estiloBotonReinicio = new GUIStyle(GUI.skin.button);
            estiloBotonReinicio.fontSize = 17;
            estiloBotonReinicio.fontStyle = FontStyle.Bold;
            estiloBotonReinicio.normal.textColor = Color.white;

            estiloControles = new GUIStyle(GUI.skin.label);
            estiloControles.fontSize = 11;
            estiloControles.normal.textColor = new Color(0.9f, 0.9f, 0.9f);
        }

        private void OnGUI()
        {
            InicializarEstilos();

            bool estaDerrotado = AntorchaLibertaria.Instance != null && AntorchaLibertaria.Instance.EstaExtinguida;
            bool estaVictoria = OleadasSpawner.Instance != null && OleadasSpawner.Instance.TodasOleadasCompletadas;

            // 1. Si ocurrió derrota o victoria, renderizar modal y detener HUD interactivo
            if (estaDerrotado)
            {
                if (!sonidoDerrotaReproducido)
                {
                    sonidoDerrotaReproducido = true;
                    if (AudioColonialManager.Instance != null)
                    {
                        AudioColonialManager.Instance.ReproducirDerrota();
                    }
                }
                DibujarPantallaDerrota();
                return;
            }

            if (estaVictoria)
            {
                DibujarPantallaVictoria();
                return;
            }

            // 2. Si el juego está en pausa, dibujar menú de pausa u opciones
            if (estaPausado)
            {
                if (mostrarOpcionesPausa)
                {
                    DibujarModalOpcionesPausa();
                }
                else
                {
                    DibujarMenuPausa();
                }
                return;
            }

            // Botón de Pausa en la esquina superior derecha
            if (GUI.Button(new Rect(Screen.width - 150, 15, 135, 34), "⏸️ PAUSA [ESC]", estiloBoton))
            {
                AlternarPausa();
            }

            // 3. Banner de Inicio de Nivel 1 (durante los primeros segundos)
            if (tiempoTranscurridoBanner < duracionBannerInicio && !bannerCerradoManualmente)
            {
                DibujarBannerInicio();
            }

            // 3. Alerta de invasión multidireccional
            DibujarAlertaInvasion();

            // 4. Panel Superior Izquierdo: Estado General, Recursos y Temporizador
            DibujarPanelEstado();

            // 5. Prompt para avivar la antorcha
            DibujarPromptAntorcha();

            // 6. Panel Inferior Central: Selector de Defensas y Modo
            DibujarSelectorDefensas();

            // 7. Panel Superior Derecho: Guia de Controles
            if (mostrarGuiaControles)
            {
                DibujarGuiaControles();
            }
        }

        private void DibujarBannerInicio()
        {
            float ancho = Mathf.Min(620, Screen.width - 40);
            float alto = 190;
            float x = (Screen.width - ancho) * 0.5f;
            float y = 50;

            GUILayout.BeginArea(new Rect(x, y, ancho, alto), estiloCajaOscura);
            GUILayout.Space(10);
            GUILayout.Label("🏛️ CAMINOS DE GUANAJUATO 2D", estiloTituloBanner);
            GUILayout.Label("NIVEL 1: LA DEFENSA DEL ATRIO PARROQUIAL", estiloNivelBanner);
            GUILayout.Space(8);
            GUILayout.Label("Dolores Hidalgo, Cuna de la Libertad (1810)\nProtege la Antorcha Libertaria de las 5 oleadas de invasores nocturnos.\nColoca barricadas populares y honderos insurgentes en el atrio.", estiloTextoBanner);
            GUILayout.Space(8);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("¡A DEFENDER! [Espacio]", estiloBoton, GUILayout.Width(190), GUILayout.Height(32)))
            {
                bannerCerradoManualmente = true;
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.EndArea();
        }

        private void DibujarPanelEstado()
        {
            GUILayout.BeginArea(new Rect(15, 15, 340, 225), estiloCajaOscura);
            GUILayout.Space(6);
            GUILayout.Label("🏛️ Caminos de Guanajuato 2D", estiloTituloHUD);
            GUILayout.Label("Nivel 1: Atrio Parroquial de Dolores", estiloSubtituloHUD);
            GUILayout.Space(6);

            // Recursos Civicos
            int recursos = RecursosCivicosManager.Instance != null ? RecursosCivicosManager.Instance.RecursosActuales : 0;
            GUILayout.Label($"⚜ Puntos Civicos: {recursos}", estiloRecursos);

            // Salud de la Antorcha Libertaria
            if (AntorchaLibertaria.Instance != null)
            {
                float pct = AntorchaLibertaria.Instance.PorcentajeSalud;
                GUILayout.Label($"🔥 Antorcha Libertaria: {AntorchaLibertaria.Instance.SaludActual} / {AntorchaLibertaria.Instance.SaludMaxima} HP");
                DibujarBarraProgreso(pct);
            }

            // Estado y Temporizador de Oleadas
            if (OleadasSpawner.Instance != null)
            {
                GUILayout.Space(4);
                if (OleadasSpawner.Instance.OleadaEnProgreso)
                {
                    GUILayout.Label($"⚔ Oleada {OleadasSpawner.Instance.OleadaActual}/{OleadasSpawner.Instance.TotalOleadas} (Enemigos restantes: {OleadasSpawner.Instance.EnemigosVivos})");
                }
                else if (OleadasSpawner.Instance.EstaEnCuentaRegresiva)
                {
                    float t = OleadasSpawner.Instance.TiempoRestanteCuentaRegresiva;
                    GUILayout.Label($"⏳ Proxima oleada en: 00:{Mathf.CeilToInt(t):D2}s", estiloTemporizador);

                    if (GUILayout.Button("⚔ Iniciar Oleada Ya [G]", estiloBoton, GUILayout.Height(26)))
                    {
                        OleadasSpawner.Instance.IniciarSiguienteOleada();
                    }
                }
                else
                {
                    GUILayout.Label($"Oleada {OleadasSpawner.Instance.OleadaActual}/{OleadasSpawner.Instance.TotalOleadas} (En espera)");
                    if (GUILayout.Button("⚔ Iniciar Oleada [G]", estiloBoton, GUILayout.Height(26)))
                    {
                        OleadasSpawner.Instance.IniciarSiguienteOleada();
                    }
                }
            }

            GUILayout.EndArea();
        }

        private void DibujarAlertaInvasion()
        {
            if (alertaInvasionTiempo <= 0f || string.IsNullOrEmpty(alertaInvasionTexto)) return;

            float ancho = Mathf.Min(720, Screen.width - 40);
            float alto = 48;
            float x = (Screen.width - ancho) * 0.5f;
            float y = 18;

            GUI.color = Color.Lerp(Color.yellow, new Color(1f, 0.35f, 0.2f), Mathf.PingPong(Time.time * 4f, 1f));
            GUILayout.BeginArea(new Rect(x, y, ancho, alto), estiloCajaOscura);
            GUILayout.Space(12);
            GUILayout.Label(alertaInvasionTexto, estiloTituloHUD);
            GUILayout.EndArea();
            GUI.color = Color.white;
        }

        private void DibujarPromptAntorcha()
        {
            if (AntorchaLibertaria.Instance == null) return;
            if (player == null) player = FindAnyObjectByType<GuanajichoController>();
            if (player == null) return;

            float dist = Vector2.Distance(player.transform.position, AntorchaLibertaria.Instance.transform.position);
            if (dist < 2.6f)
            {
                float ancho = 430;
                float alto = 42;
                float x = (Screen.width - ancho) * 0.5f;
                float y = Screen.height - 145;

                GUILayout.BeginArea(new Rect(x, y, ancho, alto), estiloCajaOscura);
                GUILayout.Space(9);
                if (AntorchaLibertaria.Instance.SaludActual < AntorchaLibertaria.Instance.SaludMaxima)
                {
                    GUILayout.Label("🔥 Pulsa [E] para Avivar la Antorcha (+25 HP / 10 Pts Cívicos)", estiloRecursos);
                }
                else
                {
                    GUILayout.Label("🔥 ¡La Antorcha Libertaria arde en su máximo esplendor! (100% HP)", estiloControles);
                }
                GUILayout.EndArea();
            }
        }

        private void DibujarSelectorDefensas()
        {
            if (ColocadorDefensas.Instance == null) return;

            var catalogo = ColocadorDefensas.Instance.Catalogo;
            int actualSel = ColocadorDefensas.Instance.IndiceSeleccionado;
            bool modoConstruccion = ColocadorDefensas.Instance.ModoConstruccionActivo;

            float anchoPanel = Mathf.Max(460, catalogo.Count * 185 + 30);
            float xPanel = (Screen.width - anchoPanel) * 0.5f;
            float yPanel = Screen.height - 95;

            GUILayout.BeginArea(new Rect(xPanel, yPanel, anchoPanel, 88), estiloCajaOscura);

            // Indicador de Modo
            string textoModo = modoConstruccion
                ? "<color=#55ff55>🔨 MODO CONSTRUCCIÓN</color> (Clic: Colocar | Q / Clic Der: Cancelar y Disparar)"
                : "<color=#ffcc33>🏹 MODO COMBATE</color> (Clic / Espacio: Dispara Honda | Pulsa 1 o 2 para Construir)";
            GUILayout.Label(textoModo, estiloControles);
            GUILayout.Space(2);

            GUILayout.BeginHorizontal();

            for (int i = 0; i < catalogo.Count; i++)
            {
                var def = catalogo[i];
                bool estaActivo = (i == actualSel) && modoConstruccion;
                string etiqueta = $"[{i + 1}] {def.nombre}\n({def.costo} pts)";

                GUIStyle estilo = estaActivo ? estiloBotonActivo : estiloBoton;
                if (GUILayout.Button(etiqueta, estilo, GUILayout.Width(175), GUILayout.Height(46)))
                {
                    ColocadorDefensas.Instance.SeleccionarDefensa(i);
                    ColocadorDefensas.Instance.ActivarModoConstruccion(true);
                }
            }

            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        private void DibujarGuiaControles()
        {
            float anchoGuia = 310;
            float altoGuia = 205;
            GUILayout.BeginArea(new Rect(Screen.width - anchoGuia - 15, 15, anchoGuia, altoGuia), estiloCajaOscura);
            GUILayout.Space(6);
            GUILayout.Label("🎮 Guía de Controles", estiloTituloHUD);
            GUILayout.Space(4);
            GUILayout.Label("• [W,A,S,D] / [Flechas]: Mover a Guanajicho", estiloControles);
            GUILayout.Label("• [Shift]: Correr (Sprint veloz)", estiloControles);
            GUILayout.Label("• [Clic / Espacio / J]: Disparar Honda Insurgente", estiloControles);
            GUILayout.Label("• [1, 2 / B]: Modo Construcción (Barricada/Hondero)", estiloControles);
            GUILayout.Label("• [Q / Clic Der.]: Salir de Construcción", estiloControles);
            GUILayout.Label("• [Tecla E]: Avivar Antorcha / Interactuar", estiloControles);
            GUILayout.Label("• [Rueda Ratón]: Zoom Dinámico In / Out", estiloControles);
            GUILayout.Label("• [Tecla G]: Iniciar Oleada de inmediato", estiloControles);
            GUILayout.Label("• [Tecla H]: Ocultar/Mostrar guía", estiloControles);
            GUILayout.EndArea();
        }

        private void DibujarPantallaDerrota()
        {
            // Fondo oscuro de pantalla completa
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), texturaFondoOverlay);

            float ancho = 520;
            float alto = 260;
            float x = (Screen.width - ancho) * 0.5f;
            float y = (Screen.height - alto) * 0.5f;

            GUILayout.BeginArea(new Rect(x, y, ancho, alto), estiloCajaDerrota);
            GUILayout.Space(15);
            GUILayout.Label("💀 ¡MISIÓN FALLIDA! 💀", estiloTituloBanner);
            GUILayout.Label("La Antorcha Libertaria se ha extinguido", estiloNivelBanner);
            GUILayout.Space(10);
            GUILayout.Label("Los invasores nocturnos han profanado el atrio parroquial.\nDolores Hidalgo ha caido en el olvido.\n\nDebes reorganizar las defensas y volver a intentarlo.", estiloTextoBanner);
            GUILayout.Space(16);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("🔄 REINICIAR", estiloBotonReinicio, GUILayout.Width(190), GUILayout.Height(44)))
            {
                if (AudioColonialManager.Instance != null) AudioColonialManager.Instance.ReproducirClickBoton();
                ReiniciarNivel();
            }
            GUILayout.Space(12);
            if (GUILayout.Button("🏠 MENÚ PRINCIPAL", estiloBotonReinicio, GUILayout.Width(190), GUILayout.Height(44)))
            {
                if (AudioColonialManager.Instance != null) AudioColonialManager.Instance.ReproducirClickBoton();
                IrAlMenuPrincipal();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.EndArea();
        }

        private bool progresoVictoriaRegistrado = false;

        private void DibujarPantallaVictoria()
        {
            if (!progresoVictoriaRegistrado)
            {
                progresoVictoriaRegistrado = true;
                if (ProgresoJuegoManager.Instance != null)
                {
                    ProgresoJuegoManager.Instance.RegistrarMisionCompletada(TipoMision.Dolores_TowerDefense);
                }
                else
                {
                    PlayerPrefs.SetInt(ProgresoJuegoManager.KEY_MISION1, 1);
                    PlayerPrefs.Save();
                }
            }

            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), texturaFondoOverlay);

            float ancho = 560;
            float alto = 270;
            float x = (Screen.width - ancho) * 0.5f;
            float y = (Screen.height - alto) * 0.5f;

            GUILayout.BeginArea(new Rect(x, y, ancho, alto), estiloCajaVictoria);
            GUILayout.Space(14);
            GUILayout.Label("⭐ ¡VICTORIA EN DOLORES HIDALGO! ⭐", estiloTituloBanner);
            GUILayout.Label("¡Has defendido la Cuna de la Libertad!", estiloNivelBanner);
            GUILayout.Space(8);
            GUILayout.Label("La Antorcha Libertaria sigue encendida y los invasores han sido repelidos.\nHas obtenido el Emblema de la Libertad (Medalla 1).\n¡El Desafío de las Nieves Tradicionales está listo en el quiosco!", estiloTextoBanner);
            GUILayout.Space(14);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("🔄 REPETIR", estiloBotonReinicio, GUILayout.Width(130), GUILayout.Height(42)))
            {
                if (AudioColonialManager.Instance != null) AudioColonialManager.Instance.ReproducirClickBoton();
                ReiniciarNivel();
            }
            GUILayout.Space(8);
            if (GUILayout.Button("🗺️ MAPA / MISIONES", estiloBotonReinicio, GUILayout.Width(200), GUILayout.Height(42)))
            {
                if (AudioColonialManager.Instance != null) AudioColonialManager.Instance.ReproducirClickBoton();
                IrAlOverworld();
            }
            GUILayout.Space(8);
            if (GUILayout.Button("🏠 MENÚ", estiloBotonReinicio, GUILayout.Width(130), GUILayout.Height(42)))
            {
                if (AudioColonialManager.Instance != null) AudioColonialManager.Instance.ReproducirClickBoton();
                IrAlMenuPrincipal();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.EndArea();
        }

        public void AlternarPausa()
        {
            estaPausado = !estaPausado;
            Time.timeScale = estaPausado ? 0f : 1f;
            if (AudioColonialManager.Instance != null)
            {
                AudioColonialManager.Instance.ReproducirClickBoton();
            }
        }

        private void DibujarMenuPausa()
        {
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), texturaFondoOverlay);

            float ancho = 460;
            float alto = 380;
            float x = (Screen.width - ancho) * 0.5f;
            float y = (Screen.height - alto) * 0.5f;

            GUILayout.BeginArea(new Rect(x, y, ancho, alto), estiloCajaOscura);
            GUILayout.Space(14);
            GUILayout.Label("⏸️ JUEGO EN PAUSA", estiloTituloBanner);
            GUILayout.Label("Nivel 1: Defensa del Atrio Parroquial", estiloNivelBanner);
            GUILayout.Space(14);

            GUILayout.BeginVertical();

            if (GUILayout.Button("▶️ CONTINUAR [ESC]", estiloBotonReinicio, GUILayout.Height(42)))
            {
                AlternarPausa();
            }

            GUILayout.Space(8);

            if (GUILayout.Button("⚙ OPCIONES DE AUDIO / PANTALLA", estiloBotonReinicio, GUILayout.Height(42)))
            {
                if (AudioColonialManager.Instance != null) AudioColonialManager.Instance.ReproducirClickBoton();
                mostrarOpcionesPausa = true;
            }

            GUILayout.Space(8);

            if (GUILayout.Button("🔄 REINICIAR NIVEL", estiloBotonReinicio, GUILayout.Height(42)))
            {
                if (AudioColonialManager.Instance != null) AudioColonialManager.Instance.ReproducirClickBoton();
                ReiniciarNivel();
            }

            GUILayout.Space(8);

            if (GUILayout.Button("🗺️ MAPA GENERAL (OVERWORLD)", estiloBotonReinicio, GUILayout.Height(42)))
            {
                if (AudioColonialManager.Instance != null) AudioColonialManager.Instance.ReproducirClickBoton();
                IrAlOverworld();
            }

            GUILayout.Space(8);

            if (GUILayout.Button("🏠 MENÚ PRINCIPAL", estiloBotonReinicio, GUILayout.Height(42)))
            {
                if (AudioColonialManager.Instance != null) AudioColonialManager.Instance.ReproducirClickBoton();
                IrAlMenuPrincipal();
            }

            GUILayout.EndVertical();
            GUILayout.EndArea();
        }

        private void DibujarModalOpcionesPausa()
        {
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), texturaFondoOverlay);

            float ancho = 480;
            float alto = 340;
            float x = (Screen.width - ancho) * 0.5f;
            float y = (Screen.height - alto) * 0.5f;

            GUILayout.BeginArea(new Rect(x, y, ancho, alto), estiloCajaOscura);
            GUILayout.Space(14);
            GUILayout.Label("⚙ OPCIONES DE JUEGO", estiloTituloBanner);
            GUILayout.Label("Ajustes de audio y visualización", estiloSubtituloHUD);
            GUILayout.Space(14);

            // Slider Volumen General
            GUILayout.BeginHorizontal();
            GUILayout.Label($"Volumen General: {Mathf.RoundToInt(volumenGeneral * 100)}%", estiloTextoBanner, GUILayout.Width(180));
            float nuevoGen = GUILayout.HorizontalSlider(volumenGeneral, 0f, 1f);
            if (Mathf.Abs(nuevoGen - volumenGeneral) > 0.001f)
            {
                volumenGeneral = nuevoGen;
                AudioListener.volume = volumenGeneral;
                if (AudioColonialManager.Instance != null)
                {
                    AudioColonialManager.Instance.AjustarVolumenGeneral(volumenGeneral);
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(8);

            // Slider Música
            GUILayout.BeginHorizontal();
            GUILayout.Label($"Música Tradicional: {Mathf.RoundToInt(volumenMusica * 100)}%", estiloTextoBanner, GUILayout.Width(180));
            float nuevaMus = GUILayout.HorizontalSlider(volumenMusica, 0f, 1f);
            if (Mathf.Abs(nuevaMus - volumenMusica) > 0.001f)
            {
                volumenMusica = nuevaMus;
                if (AudioColonialManager.Instance != null)
                {
                    AudioColonialManager.Instance.AjustarVolumenMusica(volumenMusica);
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(8);

            // Slider SFX
            GUILayout.BeginHorizontal();
            GUILayout.Label($"Efectos de Sonido: {Mathf.RoundToInt(volumenSFX * 100)}%", estiloTextoBanner, GUILayout.Width(180));
            float nuevoSFX = GUILayout.HorizontalSlider(volumenSFX, 0f, 1f);
            if (Mathf.Abs(nuevoSFX - volumenSFX) > 0.001f)
            {
                volumenSFX = nuevoSFX;
                if (AudioColonialManager.Instance != null)
                {
                    AudioColonialManager.Instance.AjustarVolumenSFX(volumenSFX);
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            // Toggle Pantalla Completa
            bool nuevaPantalla = GUILayout.Toggle(pantallaCompleta, " Pantalla Completa (Full Screen)", estiloTextoBanner);
            if (nuevaPantalla != pantallaCompleta)
            {
                pantallaCompleta = nuevaPantalla;
                Screen.fullScreen = pantallaCompleta;
            }

            GUILayout.Space(18);

            // Botón Volver
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("GUARDAR Y VOLVER", estiloBotonReinicio, GUILayout.Width(220), GUILayout.Height(40)))
            {
                if (AudioColonialManager.Instance != null) AudioColonialManager.Instance.ReproducirClickBoton();
                mostrarOpcionesPausa = false;
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.EndArea();
        }

        public void IrAlOverworld()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("Overworld_MapaGuanajuato");
        }

        public void ReiniciarNivel()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void IrAlMenuPrincipal()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("MainMenu");
        }

        private void DibujarBarraProgreso(float porcentaje)
        {
            Rect rectBarra = GUILayoutUtility.GetRect(180, 14);
            GUI.DrawTexture(rectBarra, texturaBarraRoja);
            Rect rectVerde = new Rect(rectBarra.x, rectBarra.y, rectBarra.width * Mathf.Clamp01(porcentaje), rectBarra.height);
            GUI.DrawTexture(rectVerde, texturaBarraVerde);
        }
    }
}
