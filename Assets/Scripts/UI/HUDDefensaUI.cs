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

        // Estado del Banner
        private float tiempoTranscurridoBanner;
        private bool bannerCerradoManualmente;

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
        }

        private void Update()
        {
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

            // 1. Si ocurrio derrota o victoria, renderizar modal y detener HUD interactivo
            if (estaDerrotado)
            {
                DibujarPantallaDerrota();
                return;
            }

            if (estaVictoria)
            {
                DibujarPantallaVictoria();
                return;
            }

            // 2. Banner de Inicio de Nivel 1 (durante los primeros segundos)
            if (tiempoTranscurridoBanner < duracionBannerInicio && !bannerCerradoManualmente)
            {
                DibujarBannerInicio();
            }

            // 3. Panel Superior Izquierdo: Estado General, Recursos y Temporizador
            DibujarPanelEstado();

            // 4. Panel Inferior Central: Selector de Defensas
            DibujarSelectorDefensas();

            // 5. Panel Superior Derecho: Guia de Controles
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

        private void DibujarSelectorDefensas()
        {
            if (ColocadorDefensas.Instance == null) return;

            var catalogo = ColocadorDefensas.Instance.Catalogo;
            int actualSel = ColocadorDefensas.Instance.IndiceSeleccionado;

            float anchoPanel = Mathf.Max(380, catalogo.Count * 180 + 30);
            float xPanel = (Screen.width - anchoPanel) * 0.5f;
            float yPanel = Screen.height - 75;

            GUILayout.BeginArea(new Rect(xPanel, yPanel, anchoPanel, 65), estiloCajaOscura);
            GUILayout.BeginHorizontal();

            for (int i = 0; i < catalogo.Count; i++)
            {
                var def = catalogo[i];
                bool estaActivo = (i == actualSel);
                string etiqueta = $"[{i + 1}] {def.nombre}\n({def.costo} pts)";

                GUIStyle estilo = estaActivo ? estiloBotonActivo : estiloBoton;
                if (GUILayout.Button(etiqueta, estilo, GUILayout.Width(170), GUILayout.Height(50)))
                {
                    ColocadorDefensas.Instance.SeleccionarDefensa(i);
                }
            }

            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        private void DibujarGuiaControles()
        {
            float anchoGuia = 280;
            GUILayout.BeginArea(new Rect(Screen.width - anchoGuia - 15, 15, anchoGuia, 165), estiloCajaOscura);
            GUILayout.Space(6);
            GUILayout.Label("🎮 Guia de Controles (Avance 1)", estiloTituloHUD);
            GUILayout.Space(4);
            GUILayout.Label("• [W,A,S,D] / [Flechas]: Mover a Guanajicho", estiloControles);
            GUILayout.Label("• [Shift]: Correr", estiloControles);
            GUILayout.Label("• [Clic Izquierdo]: Colocar defensa con cursor", estiloControles);
            GUILayout.Label("• [Tecla E]: Colocar defensa frente al personaje", estiloControles);
            GUILayout.Label("• [Teclas 1, 2]: Elegir tipo de defensa", estiloControles);
            GUILayout.Label("• [Tecla G]: Adelantar oleada de invasores", estiloControles);
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
                ReiniciarNivel();
            }
            GUILayout.Space(12);
            if (GUILayout.Button("🏠 MENÚ PRINCIPAL", estiloBotonReinicio, GUILayout.Width(190), GUILayout.Height(44)))
            {
                IrAlMenuPrincipal();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.EndArea();
        }

        private void DibujarPantallaVictoria()
        {
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), texturaFondoOverlay);

            float ancho = 540;
            float alto = 260;
            float x = (Screen.width - ancho) * 0.5f;
            float y = (Screen.height - alto) * 0.5f;

            GUILayout.BeginArea(new Rect(x, y, ancho, alto), estiloCajaVictoria);
            GUILayout.Space(15);
            GUILayout.Label("⭐ ¡VICTORIA EN DOLORES HIDALGO! ⭐", estiloTituloBanner);
            GUILayout.Label("¡Has defendido la Cuna de la Libertad!", estiloNivelBanner);
            GUILayout.Space(10);
            GUILayout.Label("La Antorcha Libertaria sigue encendida y los invasores han sido repelidos.\nHas obtenido el Emblema de la Libertad (Medalla 1).\n\n¡Felicidades, Guanajicho!", estiloTextoBanner);
            GUILayout.Space(16);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("🔄 JUGAR DE NUEVO", estiloBotonReinicio, GUILayout.Width(190), GUILayout.Height(44)))
            {
                ReiniciarNivel();
            }
            GUILayout.Space(12);
            if (GUILayout.Button("🏠 MENÚ PRINCIPAL", estiloBotonReinicio, GUILayout.Width(190), GUILayout.Height(44)))
            {
                IrAlMenuPrincipal();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.EndArea();
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
