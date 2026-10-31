using UnityEngine;
using UnityEngine.SceneManagement;
using CaminosDeGuanajuato.Core;

namespace CaminosDeGuanajuato.UI
{
    /// <summary>
    /// Controlador principal del Menú de Inicio de "Caminos de Guanajuato 2D: Cuna de la Libertad".
    /// Alinea dinámicamente los botones sobre el área interior de terracota del cartel de Talavera
    /// de la ilustración original en cualquier resolución o relación de aspecto, e integra
    /// ventanas modales de Opciones y Créditos con acabado artesanal colonial.
    /// </summary>
    public class MainMenuManager : MonoBehaviour
    {
        [Header("Configuración de Escenas")]
        [SerializeField] private string nombreEscenaJuego = "Overworld_MapaGuanajuato";

        [Header("Fondo del Menú Principal")]
        [Tooltip("SpriteRenderer que muestra el arte del menú principal")]
        [SerializeField] private SpriteRenderer fondoSpriteRenderer;

        [Tooltip("Imagen de respaldo opcional")]
        [SerializeField] private Sprite imagenFondoMenu;

        // Coordenadas normalizadas exactas del rectángulo interior de terracota
        // medido sobre backgroundPantallaPrincipal.jpg (2752 x 1536):
        // X interior: 758..1192 / 2752 -> 0.2755 .. 0.4331
        // Y interior desde arriba: 826..1116 / 1536 -> 0.5378 .. 0.7266
        private const float CARTEL_U_MIN = 0.276f;
        private const float CARTEL_U_MAX = 0.433f;
        private const float CARTEL_VTOP_MIN = 0.540f;
        private const float CARTEL_VTOP_MAX = 0.725f;
        private const float ASPECTO_IMAGEN_FONDO = 2752f / 1536f;

        // Estado de modales
        private bool mostrarOpciones = false;
        private bool mostrarCreditos = false;

        // Opciones de Audio y Pantalla
        private float volumenGeneral = 0.8f;
        private float volumenMusica = 0.75f;
        private float volumenSFX = 0.85f;
        private bool pantallaCompleta = true;

        // Texturas y estilos para los botones incrustados en el cartel de Talavera
        private Texture2D texPlacaCartelNormal;
        private Texture2D texPlacaCartelHover;
        private GUIStyle estiloBotonCartel;
        private GUIStyle estiloTextoCreditosTitulo;
        private GUIStyle estiloTextoCreditosCuerpo;

        private void Awake()
        {
            ColonialUIHelper.AsegurarInicializacion();
            CrearTexturasPlacaCartel();
        }

        private void Start()
        {
            Time.timeScale = 1f;
            pantallaCompleta = Screen.fullScreen;

            if (AudioColonialManager.Instance == null)
            {
                GameObject audioGO = new GameObject("AudioColonialManager");
                audioGO.AddComponent<AudioColonialManager>();
            }

            ConfigurarFondoMenu();
            AjustarFondoACamara();
        }

        private void Update()
        {
            AjustarFondoACamara();

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (mostrarOpciones) mostrarOpciones = false;
                else if (mostrarCreditos) mostrarCreditos = false;
            }
        }

        private void ConfigurarFondoMenu()
        {
            if (fondoSpriteRenderer == null)
            {
                GameObject objFondo = GameObject.Find("FondoMenu");
                if (objFondo != null)
                {
                    fondoSpriteRenderer = objFondo.GetComponent<SpriteRenderer>();
                }
            }

            if (fondoSpriteRenderer != null && imagenFondoMenu != null && fondoSpriteRenderer.sprite == null)
            {
                fondoSpriteRenderer.sprite = imagenFondoMenu;
            }
        }

        private void AjustarFondoACamara()
        {
            if (fondoSpriteRenderer == null || fondoSpriteRenderer.sprite == null)
                return;

            Camera cam = Camera.main;
            if (cam == null || !cam.orthographic)
                return;

            float altoCamara = cam.orthographicSize * 2f;
            float anchoCamara = altoCamara * cam.aspect;

            Vector2 tamanoSprite = fondoSpriteRenderer.sprite.bounds.size;
            if (tamanoSprite.x <= 0.001f || tamanoSprite.y <= 0.001f)
                return;

            float escalaX = anchoCamara / tamanoSprite.x;
            float escalaY = altoCamara / tamanoSprite.y;
            float escalaFinal = Mathf.Max(escalaX, escalaY);

            fondoSpriteRenderer.transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y, 0f);
            fondoSpriteRenderer.transform.localScale = new Vector3(escalaFinal, escalaFinal, 1f);
        }

        private void CrearTexturasPlacaCartel()
        {
            if (texPlacaCartelNormal != null && texPlacaCartelHover != null)
                return;

            // Placa de madera caoba/terracota oscuro con filete dorado que armoniza con la pintura
            texPlacaCartelNormal = CrearTexturaPlaca(
                new Color(0.26f, 0.10f, 0.05f, 0.92f),
                new Color(0.38f, 0.15f, 0.08f, 0.92f),
                new Color(0.92f, 0.72f, 0.26f, 0.95f)
            );

            // Al pasar el cursor se ilumina en oro cálido / ámbar virreinal
            texPlacaCartelHover = CrearTexturaPlaca(
                new Color(0.58f, 0.25f, 0.08f, 0.98f),
                new Color(0.76f, 0.36f, 0.12f, 0.98f),
                new Color(1.00f, 0.90f, 0.45f, 1.00f)
            );
        }

        private Texture2D CrearTexturaPlaca(Color colorInf, Color colorSup, Color colorBorde)
        {
            int w = 48;
            int h = 24;
            Texture2D tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
            tex.wrapMode = TextureWrapMode.Clamp;

            for (int y = 0; y < h; y++)
            {
                float t = (float)y / (h - 1);
                Color relleno = Color.Lerp(colorInf, colorSup, t);

                for (int x = 0; x < w; x++)
                {
                    int dx = Mathf.Min(x, w - 1 - x);
                    int dy = Mathf.Min(y, h - 1 - y);
                    int dMin = Mathf.Min(dx, dy);

                    if (dMin == 0)
                        tex.SetPixel(x, y, new Color(0.12f, 0.04f, 0.02f, 0.95f));
                    else if (dMin == 1)
                        tex.SetPixel(x, y, colorBorde);
                    else
                        tex.SetPixel(x, y, relleno);
                }
            }
            tex.Apply();
            return tex;
        }

        private void InicializarEstilos()
        {
            ColonialUIHelper.AsegurarInicializacion();
            CrearTexturasPlacaCartel();

            if (estiloBotonCartel == null)
            {
                estiloBotonCartel = new GUIStyle(GUI.skin.button)
                {
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleCenter,
                    padding = new RectOffset(4, 4, 2, 2)
                };
            }

            if (estiloTextoCreditosTitulo == null)
            {
                estiloTextoCreditosTitulo = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 14,
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleCenter,
                    richText = true
                };
                estiloTextoCreditosTitulo.normal.textColor = new Color(0.98f, 0.80f, 0.28f);
            }

            if (estiloTextoCreditosCuerpo == null)
            {
                estiloTextoCreditosCuerpo = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 13,
                    fontStyle = FontStyle.Normal,
                    alignment = TextAnchor.MiddleCenter,
                    wordWrap = true,
                    richText = true
                };
                estiloTextoCreditosCuerpo.normal.textColor = new Color(0.95f, 0.91f, 0.84f);
            }
        }

        private void OnGUI()
        {
            InicializarEstilos();

            if (mostrarOpciones)
            {
                ColonialUIHelper.DibujarModalOpcionesColonial(
                    ref volumenGeneral,
                    ref volumenMusica,
                    ref volumenSFX,
                    ref pantallaCompleta,
                    () => mostrarOpciones = false
                );
                return;
            }

            if (mostrarCreditos)
            {
                DibujarModalCreditosColonial();
                return;
            }

            DibujarCartelMenuIntegrado();
            DibujarBotonSalirEsquina();
        }

        /// <summary>
        /// Calcula en coordenadas de pantalla el rectángulo exacto del interior de terracota
        /// del cartel de Talavera pintado en el fondo, adaptándose a cualquier aspecto de cámara.
        /// </summary>
        private Rect ObtenerRectInteriorCartelTalavera()
        {
            Camera cam = Camera.main;
            if (cam != null && fondoSpriteRenderer != null && fondoSpriteRenderer.sprite != null)
            {
                Bounds b = fondoSpriteRenderer.bounds;
                if (b.size.x > 0.01f && b.size.y > 0.01f)
                {
                    float worldXMin = b.min.x + CARTEL_U_MIN * b.size.x;
                    float worldXMax = b.min.x + CARTEL_U_MAX * b.size.x;
                    float worldYMax = b.max.y - CARTEL_VTOP_MIN * b.size.y;
                    float worldYMin = b.max.y - CARTEL_VTOP_MAX * b.size.y;

                    Vector3 spTopLeft = cam.WorldToScreenPoint(new Vector3(worldXMin, worldYMax, 0f));
                    Vector3 spBottomRight = cam.WorldToScreenPoint(new Vector3(worldXMax, worldYMin, 0f));

                    float guiX = spTopLeft.x;
                    float guiY = Screen.height - spTopLeft.y;
                    float guiW = spBottomRight.x - spTopLeft.x;
                    float guiH = spTopLeft.y - spBottomRight.y;

                    if (guiW > 40f && guiH > 40f)
                    {
                        return new Rect(guiX, guiY, guiW, guiH);
                    }
                }
            }

            // Respaldo matemático exacto para modo Cover (Mathf.Max(escalaX, escalaY))
            float screenW = Screen.width;
            float screenH = Screen.height;
            float screenAspect = screenW / Mathf.Max(1f, screenH);

            float renderedW;
            float renderedH;
            if (screenAspect < ASPECTO_IMAGEN_FONDO)
            {
                renderedH = screenH;
                renderedW = screenH * ASPECTO_IMAGEN_FONDO;
            }
            else
            {
                renderedW = screenW;
                renderedH = screenW / ASPECTO_IMAGEN_FONDO;
            }

            float offsetX = (screenW - renderedW) * 0.5f;
            float offsetY = (screenH - renderedH) * 0.5f;

            float x = offsetX + CARTEL_U_MIN * renderedW;
            float y = offsetY + CARTEL_VTOP_MIN * renderedH;
            float w = (CARTEL_U_MAX - CARTEL_U_MIN) * renderedW;
            float h = (CARTEL_VTOP_MAX - CARTEL_VTOP_MIN) * renderedH;

            return new Rect(x, y, w, h);
        }

        /// <summary>
        /// Dibuja los 4 botones principales perfectamente encajados dentro del rectángulo
        /// interior de terracota del cartel de mayólica sin invadir los azulejos ni el carrito.
        /// </summary>
        private void DibujarCartelMenuIntegrado()
        {
            Rect rInterior = ObtenerRectInteriorCartelTalavera();

            // Margen interior sutil para respetar el borde pintado del cartel
            float padX = Mathf.Max(4f, rInterior.width * 0.04f);
            float padY = Mathf.Max(4f, rInterior.height * 0.05f);

            Rect rArea = new Rect(
                rInterior.x + padX,
                rInterior.y + padY,
                rInterior.width - padX * 2f,
                rInterior.height - padY * 2f
            );

            int numBotones = 4;
            float espacio = Mathf.Clamp(rArea.height * 0.055f, 3f, 8f);
            float altoBoton = (rArea.height - espacio * (numBotones - 1)) / numBotones;
            int tamFuente = Mathf.Clamp(Mathf.RoundToInt(altoBoton * 0.46f), 11, 17);

            float yActual = rArea.y;

            if (DibujarBotonPlacaCartel(new Rect(rArea.x, yActual, rArea.width, altoBoton), "▶ INICIAR", tamFuente))
            {
                IniciarJuego();
            }
            yActual += altoBoton + espacio;

            if (DibujarBotonPlacaCartel(new Rect(rArea.x, yActual, rArea.width, altoBoton), "🗺️ CONTINUAR", tamFuente))
            {
                ContinuarJuego();
            }
            yActual += altoBoton + espacio;

            if (DibujarBotonPlacaCartel(new Rect(rArea.x, yActual, rArea.width, altoBoton), "⚙ OPCIONES", tamFuente))
            {
                mostrarOpciones = true;
            }
            yActual += altoBoton + espacio;

            if (DibujarBotonPlacaCartel(new Rect(rArea.x, yActual, rArea.width, altoBoton), "📜 CRÉDITOS", tamFuente))
            {
                mostrarCreditos = true;
            }
        }

        private bool DibujarBotonPlacaCartel(Rect rect, string texto, int fontSize)
        {
            bool hover = rect.Contains(Event.current.mousePosition);
            Texture2D tex = hover ? texPlacaCartelHover : texPlacaCartelNormal;

            if (tex != null)
            {
                GUI.DrawTexture(rect, tex);
            }

            estiloBotonCartel.fontSize = fontSize;
            estiloBotonCartel.normal.background = null;
            estiloBotonCartel.hover.background = null;
            estiloBotonCartel.active.background = null;

            // Sombra del texto
            estiloBotonCartel.normal.textColor = new Color(0.08f, 0.02f, 0.01f, 0.90f);
            GUI.Label(new Rect(rect.x + 1f, rect.y + 1f, rect.width, rect.height), texto, estiloBotonCartel);

            estiloBotonCartel.normal.textColor = hover
                ? new Color(1.0f, 0.96f, 0.78f)
                : new Color(0.99f, 0.92f, 0.76f);
            estiloBotonCartel.hover.textColor = new Color(1.0f, 0.97f, 0.84f);

            bool presionado = GUI.Button(rect, texto, estiloBotonCartel);
            if (presionado && AudioColonialManager.Instance != null)
            {
                AudioColonialManager.Instance.ReproducirClickBoton();
            }
            return presionado;
        }

        private void DibujarBotonSalirEsquina()
        {
#if !UNITY_WEBGL
            float w = 110f;
            float h = 32f;
            Rect rSalir = new Rect(Screen.width - w - 16f, 14f, w, h);
            if (ColonialUIHelper.BotonColonial(rSalir, "🚪 SALIR", ColonialUIHelper.VarianteBoton.Rojo, 12))
            {
                SalirDelJuego();
            }
#endif
        }

        private void DibujarModalCreditosColonial()
        {
            ColonialUIHelper.DibujarOverlayOscuro();

            float ancho = Mathf.Min(520f, Screen.width - 32f);
            float alto = Mathf.Min(360f, Screen.height - 32f);
            Rect rectModal = new Rect((Screen.width - ancho) * 0.5f, (Screen.height - alto) * 0.5f, ancho, alto);

            ColonialUIHelper.DibujarMarcoVentana(
                rectModal,
                "📜 CRÉDITOS DEL PROYECTO",
                "Caminos de Guanajuato 2D: Cuna de la Libertad"
            );

            float xInt = rectModal.x + 22f;
            float wInt = rectModal.width - 44f;
            float yCursor = rectModal.y + 78f;

            // Tarjeta 1: Equipo de Desarrollo
            Rect rEquipo = new Rect(xInt, yCursor, wInt, 72f);
            ColonialUIHelper.DibujarTarjetaInterior(rEquipo);
            GUI.Label(new Rect(rEquipo.x, rEquipo.y + 6f, rEquipo.width, 20f), "⚒ EQUIPO DE DESARROLLO", estiloTextoCreditosTitulo);
            GUI.Label(
                new Rect(rEquipo.x + 10f, rEquipo.y + 26f, rEquipo.width - 20f, 42f),
                "<b>Samael (Sama)</b> — Dirección de Arte, Ilustración Pixel-Art y Diseño\n<b>Diego</b> — Programación de Mecánicas Core, Arquitectura y Sistemas",
                estiloTextoCreditosCuerpo
            );
            yCursor += 80f;

            // Tarjeta 2: Contexto Cultural
            Rect rCultura = new Rect(xInt, yCursor, wInt, 68f);
            ColonialUIHelper.DibujarTarjetaInterior(rCultura);
            GUI.Label(new Rect(rCultura.x, rCultura.y + 6f, rCultura.width, 20f), "🏛️ PATRIMONIO E HISTORIA", estiloTextoCreditosTitulo);
            GUI.Label(
                new Rect(rCultura.x + 10f, rCultura.y + 26f, rCultura.width - 20f, 38f),
                "Inspirado en las tradiciones de <b>Dolores Hidalgo C.I.N.</b> y <b>San Felipe</b>:\nCerámica Mayólica, Nieves Artesanales y la Ruta de la Independencia (1810).",
                estiloTextoCreditosCuerpo
            );
            yCursor += 76f;

            // Tarjeta 3: Herramientas
            Rect rTools = new Rect(xInt, yCursor, wInt, 48f);
            ColonialUIHelper.DibujarTarjetaInterior(rTools);
            GUI.Label(
                new Rect(rTools.x + 10f, rTools.y + 6f, rTools.width - 20f, 36f),
                "<b>Tecnología y Recursos:</b> Unity 2D Engine • Piskel • Arte Original Colonial",
                estiloTextoCreditosCuerpo
            );

            float wBtn = 200f;
            float hBtn = 38f;
            Rect rVolver = new Rect(
                rectModal.x + (rectModal.width - wBtn) * 0.5f,
                rectModal.yMax - hBtn - 16f,
                wBtn,
                hBtn
            );

            if (ColonialUIHelper.BotonColonial(rVolver, "⬅ VOLVER AL MENÚ", ColonialUIHelper.VarianteBoton.Dorado, 13))
            {
                mostrarCreditos = false;
            }
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
