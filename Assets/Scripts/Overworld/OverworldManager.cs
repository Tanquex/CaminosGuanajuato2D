using UnityEngine;
using UnityEngine.SceneManagement;
using CaminosDeGuanajuato.Core;

namespace CaminosDeGuanajuato.Overworld
{
    /// <summary>
    /// Controlador de la Pantalla del Overworld (Mapa Interactivo del Estado de Guanajuato).
    /// Basado en la sección 2.3 del GDD de Caminos de Guanajuato 2D:
    /// - Muestra la cartografía estilizada del estado.
    /// - Nodo Dolores Hidalgo C.I.N.: Siempre activo, abre el selector de sus 3 misiones.
    /// - Nodo San Felipe: Bloqueado en gris con ícono de candado hasta completar 3/3 misiones en Dolores.
    /// - Permite viajar a San Felipe una vez obtenido el Salvoconducto Insurgente.
    /// </summary>
    public class OverworldManager : MonoBehaviour
    {
        public static OverworldManager Instance { get; private set; }

        [Header("Configuración de Escenas")]
        [SerializeField] private string escenaMenuPrincipal = "MainMenu";
        [SerializeField] private string escenaSanFelipe = "SanFelipe_PlazaPrincipal";

        [Header("Selector de Misiones de Dolores")]
        [SerializeField] private SelectorMisionesUI selectorMisionesDolores;

        [Header("Fondo Cartográfico")]
        [SerializeField] private SpriteRenderer fondoMapaRenderer;

        // Estilos OnGUI
        private GUIStyle estiloTitulo;
        private GUIStyle estiloSubtitulo;
        private GUIStyle estiloCajaInfo;
        private GUIStyle estiloBotonNodoDolores;
        private GUIStyle estiloBotonNodoSanFelipe;
        private GUIStyle estiloBotonNodoBloqueado;
        private GUIStyle estiloBotonVolver;
        private GUIStyle estiloBarraProgresoTexto;

        private Texture2D texFondoCaja;
        private Texture2D texFondoDolores;
        private Texture2D texFondoSanFelipe;
        private Texture2D texFondoBloqueado;
        private Texture2D texBarraFondo;
        private Texture2D texBarraRelleno;

        private bool mostrarAvisoBloqueoSanFelipe = false;
        private float temporizadorAviso = 0f;

        private Texture2D texFondoMapa;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }
            Instance = this;
            CrearTexturas();

            // Asegurar que exista el ProgresoJuegoManager en escena
            if (ProgresoJuegoManager.Instance == null)
            {
                GameObject pgo = new GameObject("ProgresoJuegoManager");
                pgo.AddComponent<ProgresoJuegoManager>();
            }
        }

        private void Start()
        {
            Time.timeScale = 1f;
            CrearTexturas();
            AsegurarFondoMapa();
            AjustarFondoCamara();

            // Abrir automáticamente el selector de misiones de Dolores para acceso directo
            if (selectorMisionesDolores != null)
            {
                selectorMisionesDolores.Abrir();
            }
        }

        private void AsegurarFondoMapa()
        {
            if (fondoMapaRenderer == null)
            {
                GameObject fondoGO = new GameObject("Fondo_Mapa_Guanajuato");
                fondoMapaRenderer = fondoGO.AddComponent<SpriteRenderer>();
                fondoMapaRenderer.sortingOrder = -10;
            }

            if (fondoMapaRenderer.sprite == null)
            {
                fondoMapaRenderer.sprite = CargarSpriteDesdeAssets("Sprites/Overworld_Mapa_Guanajuato.jpg", 64f);
            }
        }

        public static Sprite CargarSpriteDesdeAssets(string rutaRelativa, float ppu = 64f)
        {
            try
            {
                string ruta = System.IO.Path.Combine(Application.dataPath, rutaRelativa);
                if (System.IO.File.Exists(ruta))
                {
                    byte[] data = System.IO.File.ReadAllBytes(ruta);
                    Texture2D tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                    if (tex.LoadImage(data))
                    {
                        tex.filterMode = FilterMode.Bilinear;
                        return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), ppu);
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"[Overworld] Error cargando sprite {rutaRelativa}: {ex.Message}");
            }
            return null;
        }

        private void Update()
        {
            AjustarFondoCamara();

            if (temporizadorAviso > 0f)
            {
                temporizadorAviso -= Time.deltaTime;
                if (temporizadorAviso <= 0f) mostrarAvisoBloqueoSanFelipe = false;
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (selectorMisionesDolores != null && selectorMisionesDolores.EstaAbierto)
                {
                    selectorMisionesDolores.Cerrar();
                }
                else
                {
                    VolverAlMenu();
                }
            }
        }

        private void AjustarFondoCamara()
        {
            if (fondoMapaRenderer == null || fondoMapaRenderer.sprite == null) return;
            Camera cam = Camera.main;
            if (cam == null || !cam.orthographic) return;

            float altoCam = cam.orthographicSize * 2f;
            float anchoCam = altoCam * cam.aspect;

            Vector2 spriteSize = fondoMapaRenderer.sprite.rect.size / fondoMapaRenderer.sprite.pixelsPerUnit;
            float sX = anchoCam / spriteSize.x;
            float sY = altoCam / spriteSize.y;
            float sFinal = Mathf.Max(sX, sY);
            fondoMapaRenderer.transform.localScale = new Vector3(sFinal, sFinal, 1f);
            fondoMapaRenderer.transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y, 0f);
        }

        private void CrearTexturas()
        {
            if (texFondoCaja != null && texBarraRelleno != null) return;

            texFondoCaja = new Texture2D(1, 1);
            texFondoCaja.SetPixel(0, 0, new Color(0.12f, 0.08f, 0.05f, 0.94f));
            texFondoCaja.Apply();

            texFondoDolores = new Texture2D(1, 1);
            texFondoDolores.SetPixel(0, 0, new Color(0.12f, 0.35f, 0.65f, 0.95f)); // Azul Talavera vibrante
            texFondoDolores.Apply();

            texFondoSanFelipe = new Texture2D(1, 1);
            texFondoSanFelipe.SetPixel(0, 0, new Color(0.15f, 0.55f, 0.25f, 0.95f)); // Verde esmeralda colonial
            texFondoSanFelipe.Apply();

            texFondoBloqueado = new Texture2D(1, 1);
            texFondoBloqueado.SetPixel(0, 0, new Color(0.25f, 0.25f, 0.88f, 0.88f)); // Gris sombreado
            texFondoBloqueado.Apply();

            texBarraFondo = new Texture2D(1, 1);
            texBarraFondo.SetPixel(0, 0, new Color(0.2f, 0.2f, 0.2f, 1f));
            texBarraFondo.Apply();

            texBarraRelleno = new Texture2D(1, 1);
            texBarraRelleno.SetPixel(0, 0, new Color(0.98f, 0.75f, 0.18f, 1f)); // Dorado
            texBarraRelleno.Apply();

            // Cargar imagen de mapa para renderizado directo en OnGUI
            if (texFondoMapa == null)
            {
                string ruta = System.IO.Path.Combine(Application.dataPath, "Sprites/Overworld_Mapa_Guanajuato.jpg");
                if (System.IO.File.Exists(ruta))
                {
                    byte[] data = System.IO.File.ReadAllBytes(ruta);
                    texFondoMapa = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                    texFondoMapa.LoadImage(data);
                }
            }
        }

        private void InicializarEstilos()
        {
            if (texFondoCaja == null || texBarraRelleno == null)
            {
                CrearTexturas();
            }

            if (estiloTitulo != null) return;

            estiloTitulo = new GUIStyle(GUI.skin.label);
            estiloTitulo.fontSize = 24;
            estiloTitulo.fontStyle = FontStyle.Bold;
            estiloTitulo.alignment = TextAnchor.MiddleCenter;
            estiloTitulo.normal.textColor = new Color(0.98f, 0.78f, 0.22f); // Dorado virreinal

            estiloSubtitulo = new GUIStyle(GUI.skin.label);
            estiloSubtitulo.fontSize = 14;
            estiloSubtitulo.fontStyle = FontStyle.Italic;
            estiloSubtitulo.alignment = TextAnchor.MiddleCenter;
            estiloSubtitulo.normal.textColor = new Color(0.88f, 0.88f, 0.88f);

            estiloCajaInfo = new GUIStyle(GUI.skin.box);
            estiloCajaInfo.normal.background = texFondoCaja;

            estiloBotonNodoDolores = new GUIStyle(GUI.skin.button);
            estiloBotonNodoDolores.fontSize = 16;
            estiloBotonNodoDolores.fontStyle = FontStyle.Bold;
            estiloBotonNodoDolores.normal.background = texFondoDolores;
            estiloBotonNodoDolores.normal.textColor = Color.white;
            estiloBotonNodoDolores.alignment = TextAnchor.MiddleCenter;

            estiloBotonNodoSanFelipe = new GUIStyle(GUI.skin.button);
            estiloBotonNodoSanFelipe.fontSize = 16;
            estiloBotonNodoSanFelipe.fontStyle = FontStyle.Bold;
            estiloBotonNodoSanFelipe.normal.background = texFondoSanFelipe;
            estiloBotonNodoSanFelipe.normal.textColor = Color.white;
            estiloBotonNodoSanFelipe.alignment = TextAnchor.MiddleCenter;

            estiloBotonNodoBloqueado = new GUIStyle(GUI.skin.button);
            estiloBotonNodoBloqueado.fontSize = 14;
            estiloBotonNodoBloqueado.fontStyle = FontStyle.Bold;
            estiloBotonNodoBloqueado.normal.background = texFondoBloqueado;
            estiloBotonNodoBloqueado.normal.textColor = new Color(0.7f, 0.7f, 0.7f);
            estiloBotonNodoBloqueado.alignment = TextAnchor.MiddleCenter;

            estiloBotonVolver = new GUIStyle(GUI.skin.button);
            estiloBotonVolver.fontSize = 13;
            estiloBotonVolver.fontStyle = FontStyle.Bold;
            estiloBotonVolver.normal.textColor = Color.white;

            estiloBarraProgresoTexto = new GUIStyle(GUI.skin.label);
            estiloBarraProgresoTexto.fontSize = 12;
            estiloBarraProgresoTexto.alignment = TextAnchor.MiddleCenter;
            estiloBarraProgresoTexto.normal.textColor = Color.white;
        }

        private void OnGUI()
        {
            InicializarEstilos();

            // Dibujar fondo de mapa siempre si está disponible (evita cualquier pantalla café o vacía)
            if (texFondoMapa != null)
            {
                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), texFondoMapa, ScaleMode.ScaleAndCrop);
            }

            // Si el selector de misiones de Dolores está abierto, darle prioridad
            if (selectorMisionesDolores != null && selectorMisionesDolores.EstaAbierto)
            {
                selectorMisionesDolores.DibujarGUI();
                return;
            }

            // 1. Cabecera del Overworld
            DibujarCabeceraOverworld();

            // 2. Nodos Municipales en el Mapa
            DibujarNodosMunicipales();

            // 3. Panel Inferior de Progreso y Navegación
            DibujarPanelInferior();

            // 4. Modal de Aviso de Bloqueo
            if (mostrarAvisoBloqueoSanFelipe)
            {
                DibujarModalAvisoBloqueo();
            }
        }

        private void DibujarCabeceraOverworld()
        {
            float ancho = Mathf.Min(700, Screen.width - 40);
            float alto = 75;
            float x = (Screen.width - ancho) * 0.5f;
            float y = 18;

            GUILayout.BeginArea(new Rect(x, y, ancho, alto), estiloCajaInfo);
            GUILayout.Space(6);
            GUILayout.Label("🗺️ CAMINOS DE GUANAJUATO: MAPA REGIONAL", estiloTitulo);
            GUILayout.Label("Selecciona un municipio para ver sus misiones y tradiciones culturales", estiloSubtitulo);
            GUILayout.EndArea();
        }

        private void DibujarNodosMunicipales()
        {
            int completadasDolores = ProgresoJuegoManager.Instance != null 
                ? ProgresoJuegoManager.Instance.ObtenerTotalCompletadasDolores() 
                : PlayerPrefs.GetInt(ProgresoJuegoManager.KEY_MISION1, 0) + PlayerPrefs.GetInt(ProgresoJuegoManager.KEY_MISION2, 0);

            bool sanFelipeDesbloqueado = completadasDolores >= 3;

            // Nodo 1: DOLORES HIDALGO C.I.N. (Centro / Cuna de la Libertad)
            float xDolores = Screen.width * 0.50f;
            float yDolores = Screen.height * 0.58f;
            float wNodo = 250f;
            float hNodo = 90f;

            GUILayout.BeginArea(new Rect(xDolores - wNodo * 0.5f, yDolores - hNodo * 0.5f, wNodo, hNodo), estiloCajaInfo);
            GUILayout.Space(4);
            if (GUILayout.Button($"🔔 DOLORES HIDALGO C.I.N.\n({completadasDolores}/3 Misiones Resueltas)", estiloBotonNodoDolores, GUILayout.Height(58)))
            {
                AbrirMisionesDolores();
            }
            GUILayout.Label("⭐ Cuna de la Independencia", estiloSubtitulo);
            GUILayout.EndArea();

            // Nodo 2: SAN FELIPE (Norte / Torres Mochas)
            float xSanFelipe = Screen.width * 0.60f;
            float ySanFelipe = Screen.height * 0.28f;

            GUILayout.BeginArea(new Rect(xSanFelipe - wNodo * 0.5f, ySanFelipe - hNodo * 0.5f, wNodo, hNodo), estiloCajaInfo);
            GUILayout.Space(4);

            if (sanFelipeDesbloqueado)
            {
                if (GUILayout.Button("🌾 SAN FELIPE\n(¡Camino Norte Abierto!)", estiloBotonNodoSanFelipe, GUILayout.Height(62)))
                {
                    ViajarASanFelipe();
                }
                GUILayout.Label("✨ Salvoconducto Obtenido", estiloSubtitulo);
            }
            else
            {
                if (GUILayout.Button($"🔒 SAN FELIPE\n({completadasDolores}/3 Misiones Requeridas)", estiloBotonNodoBloqueado, GUILayout.Height(62)))
                {
                    mostrarAvisoBloqueoSanFelipe = true;
                    temporizadorAviso = 4.0f;
                }
                GUILayout.Label("Camino intermunicipal cerrado", estiloSubtitulo);
            }

            GUILayout.EndArea();

            // Trazado de conexión entre Dolores y San Felipe
            DibujarCaminoIntermunicipal(new Vector2(xDolores + 130, yDolores), new Vector2(xSanFelipe - 130, ySanFelipe), sanFelipeDesbloqueado);
        }

        private void DibujarCaminoIntermunicipal(Vector2 inicio, Vector2 fin, bool abierto)
        {
            Color colorRuta = abierto ? new Color(0.98f, 0.8f, 0.2f, 0.9f) : new Color(0.45f, 0.45f, 0.45f, 0.6f);
            Color previo = GUI.color;
            GUI.color = colorRuta;

            // Dibuja una línea punteada estilizada con pequeños rectángulos
            int puntos = 12;
            for (int i = 0; i <= puntos; i++)
            {
                float t = (float)i / puntos;
                Vector2 pos = Vector2.Lerp(inicio, fin, t);
                GUI.DrawTexture(new Rect(pos.x - 3, pos.y - 3, 7, 7), texBarraRelleno);
            }

            GUI.color = previo;
        }

        private void DibujarPanelInferior()
        {
            float w = Screen.width - 40;
            float h = 60;
            float x = 20;
            float y = Screen.height - h - 15;

            GUILayout.BeginArea(new Rect(x, y, w, h), estiloCajaInfo);
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("⬅ VOLVER AL MENÚ", estiloBotonVolver, GUILayout.Width(180), GUILayout.Height(38)))
            {
                VolverAlMenu();
            }

            GUILayout.Space(12);

            if (GUILayout.Button("🍨 JUGAR NIVEL 2: NIEVES", estiloBotonNodoDolores, GUILayout.Width(240), GUILayout.Height(38)))
            {
                Time.timeScale = 1f;
                SceneManager.LoadScene("Nivel2_DesafioNieves");
            }

            GUILayout.FlexibleSpace();

            int comp = ProgresoJuegoManager.Instance != null ? ProgresoJuegoManager.Instance.ObtenerTotalCompletadasDolores() : 0;
            GUILayout.Label($"Progreso: {comp}/3 Misiones", estiloSubtitulo);

            GUILayout.Space(20);

            // Barra de progreso visual
            Rect rBarra = GUILayoutUtility.GetRect(180, 20);
            GUI.DrawTexture(rBarra, texBarraFondo);
            float pct = Mathf.Clamp01((float)comp / 3f);
            GUI.DrawTexture(new Rect(rBarra.x, rBarra.y, rBarra.width * pct, rBarra.height), texBarraRelleno);
            GUI.Label(rBarra, $"{Mathf.RoundToInt(pct * 100)}%", estiloBarraProgresoTexto);

            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        private void DibujarModalAvisoBloqueo()
        {
            float w = 480;
            float h = 180;
            float x = (Screen.width - w) * 0.5f;
            float y = (Screen.height - h) * 0.5f;

            GUILayout.BeginArea(new Rect(x, y, w, h), estiloCajaInfo);
            GUILayout.Space(12);
            GUILayout.Label("🔒 CAMINO A SAN FELIPE CUSTODIADO", estiloTitulo);
            GUILayout.Space(6);
            GUILayout.Label("Los arrieros y las guardias reales custodian el camino norte.\nDebes demostrar valor y resolver las 3 misiones de Dolores Hidalgo\npara que el comité del pueblo te otorgue el Salvoconducto Insurgente.", estiloSubtitulo);
            GUILayout.Space(12);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("ENTENDIDO", estiloBotonVolver, GUILayout.Width(140), GUILayout.Height(32)))
            {
                mostrarAvisoBloqueoSanFelipe = false;
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.EndArea();
        }

        public void AbrirMisionesDolores()
        {
            if (selectorMisionesDolores != null)
            {
                selectorMisionesDolores.Abrir();
            }
            else
            {
                // Fallback directo a NivelPruebas_Defensa si no estuviera configurado
                SceneManager.LoadScene("NivelPruebas_Defensa");
            }
        }

        public void ViajarASanFelipe()
        {
            Debug.Log("<color=green>[Overworld]</color> Viajando a San Felipe...");
            SceneManager.LoadScene(escenaSanFelipe);
        }

        public void VolverAlMenu()
        {
            SceneManager.LoadScene(escenaMenuPrincipal);
        }
    }
}
