using UnityEngine;
using UnityEngine.SceneManagement;
using CaminosDeGuanajuato.Core;

namespace CaminosDeGuanajuato.UI
{
    /// <summary>
    /// Controlador principal del Menú de Inicio de "Caminos de Guanajuato 2D: Cuna de la Libertad".
    /// Encaja los botones interactivos exactamente dentro del área interior de terracota
    /// del cartel de Talavera de la ilustración original e integra modales coloniales.
    /// </summary>
    public class MainMenuManager : MonoBehaviour
    {
        [Header("Configuración de Escenas")]
        [SerializeField] private string nombreEscenaJuego = "Overworld_MapaGuanajuato";

        [Header("Fondo del Menú Principal")]
        [SerializeField] private SpriteRenderer fondoSpriteRenderer;
        [SerializeField] private Texture2D imagenFondoMenu;

        // Coordenadas normalizadas del interior de terracota del cartel de Talavera
        // medidas sobre backgroundPantallaPrincipal.jpg (2752 x 1536):
        // X interior: 758..1192 / 2752 -> 0.2755 .. 0.4331
        // Y interior desde arriba: 826..1116 / 1536 -> 0.5378 .. 0.7266
        private const float CARTEL_U_MIN = 0.276f;
        private const float CARTEL_U_MAX = 0.433f;
        private const float CARTEL_VTOP_MIN = 0.540f;
        private const float CARTEL_VTOP_MAX = 0.725f;
        private const float ASPECTO_IMAGEN_FONDO = 2752f / 1536f;

        private bool mostrarOpciones = false;
        private bool mostrarCreditos = false;

        private float volumenGeneral = 0.8f;
        private float volumenMusica = 0.75f;
        private float volumenSFX = 0.85f;
        private bool pantallaCompleta = true;

        private GUIStyle estiloTextoCreditosTitulo;
        private GUIStyle estiloTextoCreditosCuerpo;

        private void Start()
        {
            Time.timeScale = 1f;
            pantallaCompleta = Screen.fullScreen;

            if (AudioColonialManager.Instance == null)
            {
                GameObject audioGO = new GameObject("AudioColonialManager");
                audioGO.AddComponent<AudioColonialManager>();
            }
            else
            {
                AudioColonialManager.Instance.IniciarMusicaDeFondo();
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

            Vector2 tamanoSprite = fondoSpriteRenderer.sprite.rect.size / fondoSpriteRenderer.sprite.pixelsPerUnit;
            if (tamanoSprite.x <= 0.001f || tamanoSprite.y <= 0.001f)
                return;

            float escalaX = anchoCamara / tamanoSprite.x;
            float escalaY = altoCamara / tamanoSprite.y;
            float escalaFinal = Mathf.Max(escalaX, escalaY);

            fondoSpriteRenderer.transform.localScale = new Vector3(escalaFinal, escalaFinal, 1f);
            fondoSpriteRenderer.transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y, 0f);
        }

        private void InicializarEstilos()
        {
            ColonialUIHelper.AsegurarInicializacion();

            if (estiloTextoCreditosTitulo == null)
            {
                estiloTextoCreditosTitulo = new GUIStyle
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
                estiloTextoCreditosCuerpo = new GUIStyle
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
        /// del cartel de Talavera según el escalado Cover (Mathf.Max) de la ilustración 2752x1536.
        /// </summary>
        private Rect ObtenerRectInteriorCartelTalavera()
        {
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

            float padX = Mathf.Max(5f, rInterior.width * 0.045f);
            float padY = Mathf.Max(4f, rInterior.height * 0.055f);

            Rect rArea = new Rect(
                rInterior.x + padX,
                rInterior.y + padY,
                rInterior.width - padX * 2f,
                rInterior.height - padY * 2f
            );

            int numBotones = 4;
            float espacio = Mathf.Clamp(rArea.height * 0.055f, 3f, 8f);
            float altoBoton = (rArea.height - espacio * (numBotones - 1)) / numBotones;
            int tamFuente = Mathf.Clamp(Mathf.RoundToInt(altoBoton * 0.45f), 11, 16);

            float yActual = rArea.y;

            if (ColonialUIHelper.BotonColonial(new Rect(rArea.x, yActual, rArea.width, altoBoton), "▶ INICIAR", ColonialUIHelper.VarianteBoton.Madera, tamFuente))
            {
                IniciarJuego();
            }
            yActual += altoBoton + espacio;

            if (ColonialUIHelper.BotonColonial(new Rect(rArea.x, yActual, rArea.width, altoBoton), "🗺️ CONTINUAR", ColonialUIHelper.VarianteBoton.Madera, tamFuente))
            {
                ContinuarJuego();
            }
            yActual += altoBoton + espacio;

            if (ColonialUIHelper.BotonColonial(new Rect(rArea.x, yActual, rArea.width, altoBoton), "⚙ OPCIONES", ColonialUIHelper.VarianteBoton.Madera, tamFuente))
            {
                mostrarOpciones = true;
            }
            yActual += altoBoton + espacio;

            if (ColonialUIHelper.BotonColonial(new Rect(rArea.x, yActual, rArea.width, altoBoton), "📜 CRÉDITOS", ColonialUIHelper.VarianteBoton.Madera, tamFuente))
            {
                mostrarCreditos = true;
            }
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

            Rect rEquipo = new Rect(xInt, yCursor, wInt, 72f);
            ColonialUIHelper.DibujarTarjetaInterior(rEquipo);
            GUI.Label(new Rect(rEquipo.x, rEquipo.y + 6f, rEquipo.width, 20f), "⚒ EQUIPO DE DESARROLLO", estiloTextoCreditosTitulo);
            GUI.Label(
                new Rect(rEquipo.x + 10f, rEquipo.y + 26f, rEquipo.width - 20f, 42f),
                "<b>Samael (Sama)</b> — Dirección de Arte, Ilustración Pixel-Art y Diseño\n<b>Diego</b> — Programación de Mecánicas Core, Arquitectura y Sistemas",
                estiloTextoCreditosCuerpo
            );
            yCursor += 80f;

            Rect rCultura = new Rect(xInt, yCursor, wInt, 68f);
            ColonialUIHelper.DibujarTarjetaInterior(rCultura);
            GUI.Label(new Rect(rCultura.x, rCultura.y + 6f, rCultura.width, 20f), "🏛️ PATRIMONIO E HISTORIA", estiloTextoCreditosTitulo);
            GUI.Label(
                new Rect(rCultura.x + 10f, rCultura.y + 26f, rCultura.width - 20f, 38f),
                "Inspirado en las tradiciones de <b>Dolores Hidalgo C.I.N.</b> y <b>San Felipe</b>:\nCerámica Mayólica, Nieves Artesanales y la Ruta de la Independencia (1810).",
                estiloTextoCreditosCuerpo
            );
            yCursor += 76f;

            Rect rTools = new Rect(xInt, yCursor, wInt, 48f);
            ColonialUIHelper.DibujarTarjetaInterior(rTools);
            GUI.Label(
                new Rect(rTools.x + 10f, rTools.y + 6f, rTools.width - 20f, 36f),
                "<b>Tecnología y Recursos:</b> Unity 2D Engine • Piskel • Arte Original Colonial",
                estiloTextoCreditosCuerpo
            );

            float wBtn = 200f;
            float hBtn = 36f;
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
