using UnityEngine;
using UnityEngine.SceneManagement;
using CaminosDeGuanajuato.Core;

namespace CaminosDeGuanajuato.UI
{
    /// <summary>
    /// Controlador del Menu Principal de Caminos de Guanajuato 2D: Cuna de la Libertad.
    /// Basado en el arte de la Parroquia de Dolores Hidalgo y el puesto de nieves tradicional.
    /// Soporta:
    /// - START: Inicia la mision 1 en el Atrio Parroquial.
    /// - CONTINUE: Continua la partida guardada.
    /// - OPTIONS: Ajuste de volumen y pantalla completa.
    /// - CREDITS: Creditos de desarrollo (Samael & Diego).
    /// - QUIT: Salir de la aplicacion.
    /// </summary>
    public class MainMenuManager : MonoBehaviour
    {
        [Header("Escena del Overworld / Mapa")]
        [SerializeField] private string nombreEscenaJuego = "Overworld_MapaGuanajuato";

        [Header("Imagen de Fondo")]
        [SerializeField] private SpriteRenderer fondoSpriteRenderer;
        [SerializeField] private Texture2D imagenFondoMenu;

        // Estado de ventanas modales
        private bool mostrarOpciones = false;
        private bool mostrarCreditos = false;

        // Ajustes de opciones
        private float volumenGeneral = 0.8f;
        private float volumenMusica = 0.8f;
        private float volumenSFX = 0.8f;
        private bool pantallaCompleta = true;

        // Estilos GUI
        private GUIStyle estiloBotonMenu;
        private GUIStyle estiloBotonMenuHover;
        private GUIStyle estiloModal;
        private GUIStyle estiloTituloModal;
        private GUIStyle estiloTextoModal;
        private GUIStyle estiloCreditos;
        private GUIStyle estiloSubtitulo;
        private GUIStyle estiloBotonModal;
        private Texture2D texturaFondoModal;
        private Texture2D texturaFondoBoton;
        private Texture2D texturaFondoBotonHover;
        private Texture2D texturaOverlay;

        private void Start()
        {
            Time.timeScale = 1f;
            pantallaCompleta = Screen.fullScreen;
            CrearTexturasGUI();
            AjustarFondoACamara();

            // Iniciar música tradicional colonial desde el primer instante en el Menú Principal
            if (AudioColonialManager.Instance != null)
            {
                AudioColonialManager.Instance.IniciarMusicaDeFondo();
            }
        }

        private void Update()
        {
            // Ajustar fondo si cambia la resolución de ventana
            AjustarFondoACamara();

            // Atajos de teclado
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (mostrarOpciones || mostrarCreditos)
                {
                    mostrarOpciones = false;
                    mostrarCreditos = false;
                }
            }

            if (!mostrarOpciones && !mostrarCreditos && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)))
            {
                IniciarJuego();
            }
        }

        /// <summary>
        /// Ajusta el tamaño del SpriteRenderer de fondo para que cubra la vista de la cámara
        /// en cualquier resolución o aspecto de pantalla (16:9, 16:10, 4:3, etc.).
        /// </summary>
        private void AjustarFondoACamara()
        {
            if (fondoSpriteRenderer == null) return;
            Camera cam = Camera.main;
            if (cam == null || !cam.orthographic) return;

            float altoCamara = cam.orthographicSize * 2f;
            float anchoCamara = altoCamara * cam.aspect;

            if (fondoSpriteRenderer.sprite != null)
            {
                Vector2 tamañoSprite = fondoSpriteRenderer.sprite.rect.size / fondoSpriteRenderer.sprite.pixelsPerUnit;
                float escalaX = anchoCamara / tamañoSprite.x;
                float escalaY = altoCamara / tamañoSprite.y;
                float escalaFinal = Mathf.Max(escalaX, escalaY);

                fondoSpriteRenderer.transform.localScale = new Vector3(escalaFinal, escalaFinal, 1f);
                fondoSpriteRenderer.transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y, 0f);
            }
        }

        private void CrearTexturasGUI()
        {
            texturaFondoModal = new Texture2D(1, 1);
            texturaFondoModal.SetPixel(0, 0, new Color(0.12f, 0.08f, 0.05f, 0.96f)); // Madera y terracota colonial
            texturaFondoModal.Apply();

            texturaOverlay = new Texture2D(1, 1);
            texturaOverlay.SetPixel(0, 0, new Color(0.04f, 0.02f, 0.04f, 0.75f));
            texturaOverlay.Apply();

            texturaFondoBoton = new Texture2D(1, 1);
            texturaFondoBoton.SetPixel(0, 0, new Color(0.12f, 0.28f, 0.52f, 0.88f)); // Azul Cobalto Talavera
            texturaFondoBoton.Apply();

            texturaFondoBotonHover = new Texture2D(1, 1);
            texturaFondoBotonHover.SetPixel(0, 0, new Color(0.96f, 0.68f, 0.18f, 0.95f)); // Amarillo canario virreinal
            texturaFondoBotonHover.Apply();
        }

        private void InicializarEstilos()
        {
            if (estiloBotonMenu != null) return;

            estiloBotonMenu = new GUIStyle(GUI.skin.button);
            estiloBotonMenu.fontSize = 17;
            estiloBotonMenu.fontStyle = FontStyle.Bold;
            estiloBotonMenu.normal.background = texturaFondoBoton;
            estiloBotonMenu.normal.textColor = Color.white;
            estiloBotonMenu.alignment = TextAnchor.MiddleCenter;

            estiloBotonMenuHover = new GUIStyle(estiloBotonMenu);
            estiloBotonMenuHover.normal.background = texturaFondoBotonHover;
            estiloBotonMenuHover.normal.textColor = new Color(0.15f, 0.08f, 0.02f);

            estiloModal = new GUIStyle(GUI.skin.box);
            estiloModal.normal.background = texturaFondoModal;

            estiloTituloModal = new GUIStyle(GUI.skin.label);
            estiloTituloModal.fontSize = 20;
            estiloTituloModal.fontStyle = FontStyle.Bold;
            estiloTituloModal.alignment = TextAnchor.MiddleCenter;
            estiloTituloModal.normal.textColor = new Color(0.98f, 0.75f, 0.18f); // Dorado

            estiloSubtitulo = new GUIStyle(GUI.skin.label);
            estiloSubtitulo.fontSize = 14;
            estiloSubtitulo.fontStyle = FontStyle.Italic;
            estiloSubtitulo.alignment = TextAnchor.MiddleCenter;
            estiloSubtitulo.normal.textColor = new Color(0.85f, 0.85f, 0.85f);

            estiloTextoModal = new GUIStyle(GUI.skin.label);
            estiloTextoModal.fontSize = 13;
            estiloTextoModal.normal.textColor = new Color(0.92f, 0.92f, 0.92f);

            estiloCreditos = new GUIStyle(GUI.skin.label);
            estiloCreditos.fontSize = 13;
            estiloCreditos.wordWrap = true;
            estiloCreditos.alignment = TextAnchor.MiddleCenter;
            estiloCreditos.normal.textColor = Color.white;

            estiloBotonModal = new GUIStyle(GUI.skin.button);
            estiloBotonModal.fontSize = 14;
            estiloBotonModal.fontStyle = FontStyle.Bold;
            estiloBotonModal.normal.textColor = Color.white;
        }

        private void OnGUI()
        {
            InicializarEstilos();

            // Si hay un modal abierto (Opciones o Creditos), dibujarlo por encima
            if (mostrarOpciones)
            {
                DibujarModalOpciones();
                return;
            }

            if (mostrarCreditos)
            {
                DibujarModalCreditos();
                return;
            }

            // Dibujar el panel interactivo de botones alineado con el cartel de Talavera
            DibujarCartelMenu();
        }

        /// <summary>
        /// Dibuja los botones interactivos (START, CONTINUE, OPTIONS, CREDITS, QUIT)
        /// sobre la zona del cartel de mayólica de la ilustración.
        /// </summary>
        private void DibujarCartelMenu()
        {
            // Posicion proporcional respecto a la resolucion de la pantalla
            // En la ilustracion 16:9, el cartel está centrado-izquierda: X: ~31%..41%, Y: ~52%..77%
            float anchoCartel = Mathf.Clamp(Screen.width * 0.17f, 210f, 270f);
            float altoCartel = 285f;
            float xCartel = Screen.width * 0.355f - (anchoCartel * 0.5f);
            float yCartel = Screen.height * 0.53f;

            GUILayout.BeginArea(new Rect(xCartel, yCartel, anchoCartel, altoCartel));
            GUILayout.Space(5);

            // Boton 1: START
            if (DibujarBotonMenu("START"))
            {
                IniciarJuego();
            }

            GUILayout.Space(6);

            // Acceso directo de prueba para Nivel 2
            if (DibujarBotonMenu("🍨 PROBAR NIVEL 2"))
            {
                Debug.Log("<color=cyan>[MainMenu]</color> Accediendo directamente a Nivel 2: Desafío de Nieves.");
                Time.timeScale = 1f;
                SceneManager.LoadScene("Nivel2_DesafioNieves");
            }

            GUILayout.Space(6);

            // Boton 2: CONTINUE
            if (DibujarBotonMenu("CONTINUE"))
            {
                ContinuarJuego();
            }

            GUILayout.Space(8);

            // Boton 3: OPTIONS
            if (DibujarBotonMenu("OPTIONS"))
            {
                mostrarOpciones = true;
            }

            GUILayout.Space(8);

            // Boton 4: CREDITS
            if (DibujarBotonMenu("CREDITS"))
            {
                mostrarCreditos = true;
            }

            GUILayout.Space(8);

            // Boton 5: QUIT (en caso de querer salir)
            if (DibujarBotonMenu("QUIT"))
            {
                SalirDelJuego();
            }

            GUILayout.EndArea();
        }

        private bool DibujarBotonMenu(string texto)
        {
            Rect rect = GUILayoutUtility.GetRect(new GUIContent(texto), estiloBotonMenu, GUILayout.Height(36));
            bool estaEncima = rect.Contains(Event.current.mousePosition);
            GUIStyle estilo = estaEncima ? estiloBotonMenuHover : estiloBotonMenu;

            bool presionado = GUI.Button(rect, texto, estilo);
            if (presionado && AudioColonialManager.Instance != null)
            {
                AudioColonialManager.Instance.ReproducirClickBoton();
            }
            return presionado;
        }

        private void DibujarModalOpciones()
        {
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), texturaOverlay);

            float ancho = 440;
            float alto = 320;
            float x = (Screen.width - ancho) * 0.5f;
            float y = (Screen.height - alto) * 0.5f;

            GUILayout.BeginArea(new Rect(x, y, ancho, alto), estiloModal);
            GUILayout.Space(12);
            GUILayout.Label("⚙ OPCIONES DE JUEGO", estiloTituloModal);
            GUILayout.Label("Caminos de Guanajuato 2D", estiloSubtitulo);
            GUILayout.Space(14);

            // Slider Volumen General
            GUILayout.BeginHorizontal();
            GUILayout.Label($"Volumen General: {Mathf.RoundToInt(volumenGeneral * 100)}%", estiloTextoModal, GUILayout.Width(170));
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

            // Slider Musica
            GUILayout.BeginHorizontal();
            GUILayout.Label($"Música Tradicional: {Mathf.RoundToInt(volumenMusica * 100)}%", estiloTextoModal, GUILayout.Width(170));
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
            GUILayout.Label($"Efectos de Sonido: {Mathf.RoundToInt(volumenSFX * 100)}%", estiloTextoModal, GUILayout.Width(170));
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
            bool nuevaPantalla = GUILayout.Toggle(pantallaCompleta, " Pantalla Completa (Full Screen)", estiloTextoModal);
            if (nuevaPantalla != pantallaCompleta)
            {
                pantallaCompleta = nuevaPantalla;
                Screen.fullScreen = pantallaCompleta;
            }

            GUILayout.Space(18);

            // Boton Volver
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("GUARDAR Y VOLVER", estiloBotonModal, GUILayout.Width(200), GUILayout.Height(36)))
            {
                if (AudioColonialManager.Instance != null) AudioColonialManager.Instance.ReproducirClickBoton();
                mostrarOpciones = false;
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.EndArea();
        }

        private void DibujarModalCreditos()
        {
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), texturaOverlay);

            float ancho = 520;
            float alto = 360;
            float x = (Screen.width - ancho) * 0.5f;
            float y = (Screen.height - alto) * 0.5f;

            GUILayout.BeginArea(new Rect(x, y, ancho, alto), estiloModal);
            GUILayout.Space(12);
            GUILayout.Label("📜 CRÉDITOS", estiloTituloModal);
            GUILayout.Label("Caminos de Guanajuato 2D: Cuna de la Libertad", estiloSubtitulo);
            GUILayout.Space(12);

            GUILayout.Label("Equipo de Desarrollo:\n<b>Samael (Sama)</b> — Diseño de Arte, Sprites y Game Design\n<b>Diego</b> — Programación de Mecánicas Core, Lógica y Setup", estiloCreditos);
            GUILayout.Space(10);
            GUILayout.Label("Contexto Cultural e Histórico:\nDolores Hidalgo C.I.N. y San Felipe, Guanajuato, México\nRescate de tradiciones: Mayólica, Nieves Típicas y el Grito de Independencia.", estiloCreditos);
            GUILayout.Space(10);
            GUILayout.Label("Herramientas y Recursos:\nUnity 2D Engine • Piskel • Kenney.nl • Freesound.org (CC0)", estiloCreditos);
            GUILayout.Space(16);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("VOLVER AL MENÚ", estiloBotonModal, GUILayout.Width(190), GUILayout.Height(36)))
            {
                if (AudioColonialManager.Instance != null) AudioColonialManager.Instance.ReproducirClickBoton();
                mostrarCreditos = false;
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.EndArea();
        }

        public void IniciarJuego()
        {
            Debug.Log($"<color=green>[MainMenu]</color> Iniciando aventura hacia el mapa de misiones: {nombreEscenaJuego}");
            try
            {
                SceneManager.LoadScene(nombreEscenaJuego);
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"[MainMenu] No se pudo cargar {nombreEscenaJuego}, cargando nivel de pruebas: {ex.Message}");
                SceneManager.LoadScene("NivelPruebas_Defensa");
            }
        }

        public void ContinuarJuego()
        {
            Debug.Log($"<color=cyan>[MainMenu]</color> Continuando partida en: {nombreEscenaJuego}");
            try
            {
                SceneManager.LoadScene(nombreEscenaJuego);
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"[MainMenu] Respaldo al nivel de pruebas: {ex.Message}");
                SceneManager.LoadScene("NivelPruebas_Defensa");
            }
        }

        public void SalirDelJuego()
        {
            Debug.Log("<color=red>[MainMenu]</color> Saliendo del juego...");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
