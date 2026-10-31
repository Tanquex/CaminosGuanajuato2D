using UnityEngine;
using UnityEngine.SceneManagement;
using CaminosDeGuanajuato.Core;
using CaminosDeGuanajuato.UI;

namespace CaminosDeGuanajuato.Overworld
{
    /// <summary>
    /// Controlador del Mapa Regional Interactivo del Estado de Guanajuato (Overworld).
    /// Diseñado para respetar el arte cartográfico original (cartela histórica superior izquierda,
    /// rosa de los vientos y simbología inferior izquierda), ofreciendo marcadores limpios sobre
    /// los pueblos y un panel inferior rápido e intuitivo con acceso directo a las misiones.
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

        private GUIStyle estiloTituloBarra;
        private GUIStyle estiloSubtituloBarra;
        private GUIStyle estiloEtiquetaPin;
        private GUIStyle estiloTituloDock;

        private Texture2D texPuntoCaminoOro;
        private Texture2D texPuntoCaminoGris;
        private Texture2D texFondoMapa;

        private bool mostrarAvisoBloqueoSanFelipe = false;
        private float temporizadorAviso = 0f;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }
            Instance = this;

            ColonialUIHelper.AsegurarInicializacion();
            CrearTexturasMapa();

            if (ProgresoJuegoManager.Instance == null)
            {
                GameObject pgo = new GameObject("ProgresoJuegoManager");
                pgo.AddComponent<ProgresoJuegoManager>();
            }
        }

        private void Start()
        {
            Time.timeScale = 1f;
            ColonialUIHelper.AsegurarInicializacion();
            CrearTexturasMapa();
            AsegurarFondoMapa();
            AjustarFondoCamara();

            if (selectorMisionesDolores != null)
            {
                selectorMisionesDolores.Cerrar();
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
            string nombreSinExt = System.IO.Path.GetFileNameWithoutExtension(rutaRelativa);

            Sprite resSprite = Resources.Load<Sprite>(nombreSinExt);
            if (resSprite != null) return resSprite;

            Texture2D resTex = Resources.Load<Texture2D>(nombreSinExt);
            if (resTex != null)
            {
                return Sprite.Create(resTex, new Rect(0, 0, resTex.width, resTex.height), new Vector2(0.5f, 0.5f), ppu);
            }

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
                else if (mostrarAvisoBloqueoSanFelipe)
                {
                    mostrarAvisoBloqueoSanFelipe = false;
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

        private void CrearTexturasMapa()
        {
            if (texPuntoCaminoOro == null)
            {
                texPuntoCaminoOro = new Texture2D(1, 1);
                texPuntoCaminoOro.SetPixel(0, 0, new Color(1.0f, 0.84f, 0.25f, 0.95f));
                texPuntoCaminoOro.Apply();
            }

            if (texPuntoCaminoGris == null)
            {
                texPuntoCaminoGris = new Texture2D(1, 1);
                texPuntoCaminoGris.SetPixel(0, 0, new Color(0.32f, 0.22f, 0.14f, 0.80f));
                texPuntoCaminoGris.Apply();
            }

            if (texFondoMapa == null)
            {
                texFondoMapa = Resources.Load<Texture2D>("Overworld_Mapa_Guanajuato");
                if (texFondoMapa == null)
                {
                    Sprite sp = Resources.Load<Sprite>("Overworld_Mapa_Guanajuato");
                    if (sp != null) texFondoMapa = sp.texture;
                }

                if (texFondoMapa == null)
                {
                    try
                    {
                        string ruta = System.IO.Path.Combine(Application.dataPath, "Sprites/Overworld_Mapa_Guanajuato.jpg");
                        if (System.IO.File.Exists(ruta))
                        {
                            byte[] data = System.IO.File.ReadAllBytes(ruta);
                            texFondoMapa = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                            texFondoMapa.LoadImage(data);
                        }
                    }
                    catch { }
                }
            }
        }

        private void InicializarEstilos()
        {
            ColonialUIHelper.AsegurarInicializacion();
            CrearTexturasMapa();

            if (estiloTituloBarra != null) return;

            estiloTituloBarra = new GUIStyle(GUI.skin.label)
            {
                fontSize = 15,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                richText = true
            };
            estiloTituloBarra.normal.textColor = new Color(0.99f, 0.84f, 0.32f);

            estiloSubtituloBarra = new GUIStyle(GUI.skin.label)
            {
                fontSize = 11,
                fontStyle = FontStyle.Italic,
                alignment = TextAnchor.MiddleCenter,
                richText = true
            };
            estiloSubtituloBarra.normal.textColor = new Color(0.92f, 0.86f, 0.74f);

            estiloEtiquetaPin = new GUIStyle(GUI.skin.label)
            {
                fontSize = 11,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                richText = true
            };
            estiloEtiquetaPin.normal.textColor = new Color(1.0f, 0.92f, 0.68f);

            estiloTituloDock = new GUIStyle(GUI.skin.label)
            {
                fontSize = 13,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleLeft,
                richText = true
            };
            estiloTituloDock.normal.textColor = new Color(0.99f, 0.84f, 0.30f);
        }

        private void OnGUI()
        {
            InicializarEstilos();

            if (texFondoMapa != null)
            {
                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), texFondoMapa, ScaleMode.ScaleAndCrop);
            }

            if (selectorMisionesDolores != null && selectorMisionesDolores.EstaAbierto)
            {
                selectorMisionesDolores.DibujarGUI();
                return;
            }

            // 1. Barra superior compacta a la derecha de la cartela histórica (no tapa el pergamino izquierdo)
            DibujarBarraSuperiorCompacta();

            // 2. Marcadores interactivos limpios sobre Dolores Hidalgo y San Felipe
            DibujarMarcadoresMapa();

            // 3. Panel rápido de misiones en la parte inferior derecha (no tapa la leyenda de la izquierda)
            DibujarDockMisionesDolores();

            // 4. Aviso de camino custodiado a San Felipe
            if (mostrarAvisoBloqueoSanFelipe)
            {
                DibujarModalAvisoBloqueo();
            }
        }

        private void DibujarBarraSuperiorCompacta()
        {
            int completadas = ObtenerTotalCompletadas();

            float xInicio = Mathf.Max(305f, Screen.width * 0.32f);
            float margenDer = Mathf.Max(28f, Screen.width * 0.042f);
            float ancho = Screen.width - xInicio - margenDer;
            float alto = 48f;
            float y = Mathf.Max(20f, Screen.height * 0.042f);

            Rect rBarra = new Rect(xInicio, y, ancho, alto);
            ColonialUIHelper.DibujarTarjetaInterior(rBarra);

            // Botón Menú Principal a la izquierda de la barra
            Rect rBtnMenu = new Rect(rBarra.x + 8f, rBarra.y + 7f, 112f, 34f);
            if (ColonialUIHelper.BotonColonial(rBtnMenu, "⬅ MENÚ", ColonialUIHelper.VarianteBoton.Madera, 12))
            {
                VolverAlMenu();
            }

            // Medalla de progreso a la derecha de la barra
            float wProgreso = 138f;
            Rect rBadge = new Rect(rBarra.xMax - wProgreso - 8f, rBarra.y + 7f, wProgreso, 34f);
            if (ColonialUIHelper.BotonColonial(
                rBadge,
                $"⭐ {completadas}/3 MISIONES",
                completadas >= 3 ? ColonialUIHelper.VarianteBoton.Verde : ColonialUIHelper.VarianteBoton.Talavera,
                12))
            {
                AbrirMisionesDolores();
            }

            // Título central
            float xCentro = rBtnMenu.xMax + 6f;
            float wCentro = Mathf.Max(120f, rBadge.x - xCentro - 6f);
            ColonialUIHelper.DibujarTextoConSombra(
                new Rect(xCentro, rBarra.y + 4f, wCentro, 22f),
                "🗺️ RUTA DE LA LIBERTAD (1810)",
                estiloTituloBarra
            );
            GUI.Label(
                new Rect(xCentro, rBarra.y + 24f, wCentro, 18f),
                "Haz clic en un pueblo o selecciona una misión abajo",
                estiloSubtituloBarra
            );
        }

        private void DibujarMarcadoresMapa()
        {
            int completadas = ObtenerTotalCompletadas();
            bool sanFelipeDesbloqueado = completadas >= 3;

            // Coordenadas alineadas con los pueblos ilustrados en Overworld_Mapa_Guanajuato.jpg
            Vector2 posDolores = new Vector2(Screen.width * 0.515f, Screen.height * 0.505f);
            Vector2 posSanFelipe = new Vector2(Screen.width * 0.555f, Screen.height * 0.215f);

            // Trazado del Camino Real Norte entre Dolores Hidalgo y San Felipe
            DibujarCaminoReal(
                new Vector2(posDolores.x, posDolores.y - 26f),
                new Vector2(posSanFelipe.x, posSanFelipe.y + 26f),
                sanFelipeDesbloqueado
            );

            // PIN 1: SAN FELIPE (Norte)
            float wPinSF = 206f;
            float hPinSF = 48f;
            Rect rPinSF = new Rect(posSanFelipe.x - wPinSF * 0.5f, posSanFelipe.y - hPinSF * 0.5f, wPinSF, hPinSF);

            if (sanFelipeDesbloqueado)
            {
                if (ColonialUIHelper.BotonColonial(
                    rPinSF,
                    "🌾 SAN FELIPE (NORTE)\n✨ ¡Camino Real Abierto!",
                    ColonialUIHelper.VarianteBoton.Verde,
                    12))
                {
                    ViajarASanFelipe();
                }
            }
            else
            {
                if (ColonialUIHelper.BotonColonial(
                    rPinSF,
                    $"🔒 SAN FELIPE (NORTE)\nRequiere 3/3 en Dolores ({completadas}/3)",
                    ColonialUIHelper.VarianteBoton.Madera,
                    11))
                {
                    mostrarAvisoBloqueoSanFelipe = true;
                    temporizadorAviso = 4.5f;
                }
            }

            // PIN 2: DOLORES HIDALGO C.I.N. (Justo debajo de la Parroquia con la bandera)
            float wPinDolores = 224f;
            float hPinDolores = 52f;
            Rect rPinDolores = new Rect(posDolores.x - wPinDolores * 0.5f, posDolores.y - hPinDolores * 0.5f, wPinDolores, hPinDolores);

            if (ColonialUIHelper.BotonColonial(
                rPinDolores,
                $"🏛️ DOLORES HIDALGO C.I.N.\n⭐ {completadas}/3 Misiones • [ Ver Tablero ]",
                ColonialUIHelper.VarianteBoton.Talavera,
                12))
            {
                AbrirMisionesDolores();
            }
        }

        private void DibujarCaminoReal(Vector2 inicio, Vector2 fin, bool abierto)
        {
            int pasos = 11;
            for (int i = 1; i < pasos; i++)
            {
                float t = (float)i / pasos;
                Vector2 p = Vector2.Lerp(inicio, fin, t);
                // Curva sutil siguiendo el trazo del mapa
                p.x += Mathf.Sin(t * Mathf.PI) * -10f;

                GUI.DrawTexture(new Rect(p.x - 4f, p.y - 4f, 8f, 8f), texPuntoCaminoGris);
                if (abierto)
                {
                    GUI.DrawTexture(new Rect(p.x - 3f, p.y - 3f, 6f, 6f), texPuntoCaminoOro);
                }
            }
        }

        /// <summary>
        /// Panel inferior derecho amigable: muestra directamente las 3 misiones de Dolores Hidalgo
        /// con botones de 1 clic para que el jugador nunca se pierda, dejando libre el cuadro
        /// de simbología ("PUEBLOS / CAMINOS REALES") de la esquina inferior izquierda.
        /// </summary>
        private void DibujarDockMisionesDolores()
        {
            bool m1Hecha = ProgresoJuegoManager.Instance != null && ProgresoJuegoManager.Instance.EstaMisionCompletada(TipoMision.Dolores_TowerDefense);
            bool m2Hecha = ProgresoJuegoManager.Instance != null && ProgresoJuegoManager.Instance.EstaMisionCompletada(TipoMision.Dolores_Nieves);
            bool m3Hecha = ProgresoJuegoManager.Instance != null && ProgresoJuegoManager.Instance.EstaMisionCompletada(TipoMision.Dolores_Duelo);

            float xInicio = Mathf.Max(245f, Screen.width * 0.255f);
            float margenDer = Mathf.Max(28f, Screen.width * 0.042f);
            float ancho = Screen.width - xInicio - margenDer;
            float alto = 96f;
            float y = Screen.height - alto - Mathf.Max(20f, Screen.height * 0.042f);

            Rect rDock = new Rect(xInicio, y, ancho, alto);
            ColonialUIHelper.DibujarTarjetaInterior(rDock);

            // Cabecera del Dock
            GUI.Label(
                new Rect(rDock.x + 12f, rDock.y + 6f, rDock.width - 170f, 22f),
                "📜 MISIONES DE DOLORES HIDALGO — Acceso Directo:",
                estiloTituloDock
            );

            Rect rBtnDetalles = new Rect(rDock.xMax - 150f, rDock.y + 6f, 140f, 24f);
            if (ColonialUIHelper.BotonColonial(rBtnDetalles, "📋 Ver Detalles", ColonialUIHelper.VarianteBoton.Madera, 11))
            {
                AbrirMisionesDolores();
            }

            ColonialUIHelper.DibujarSeparador(rDock.x + 10f, rDock.y + 33f, rDock.width - 20f);

            // 3 Botones directos para las 3 misiones
            float padX = 10f;
            float gap = 8f;
            float wDisp = rDock.width - padX * 2f;
            float wBoton = (wDisp - gap * 2f) / 3f;
            float yBoton = rDock.y + 40f;
            float hBoton = 48f;

            // Botón Misión 1: Defensa del Atrio
            string txtM1 = m1Hecha
                ? "⭐ 1. DEFENSA DEL ATRIO\n[ Completada • Repetir ]"
                : "⚔ 1. DEFENSA DEL ATRIO\n[ ▶ Jugar Nivel 1 ]";
            if (ColonialUIHelper.BotonColonial(
                new Rect(rDock.x + padX, yBoton, wBoton, hBoton),
                txtM1,
                m1Hecha ? ColonialUIHelper.VarianteBoton.Dorado : ColonialUIHelper.VarianteBoton.Verde,
                11))
            {
                Time.timeScale = 1f;
                SceneManager.LoadScene("NivelPruebas_Defensa");
            }

            // Botón Misión 2: Puesto de Nieves
            string txtM2 = m2Hecha
                ? "⭐ 2. PUESTO DE NIEVES\n[ Completada • Repetir ]"
                : "🍨 2. PUESTO DE NIEVES\n[ ▶ Jugar Nivel 2 ]";
            if (ColonialUIHelper.BotonColonial(
                new Rect(rDock.x + padX + wBoton + gap, yBoton, wBoton, hBoton),
                txtM2,
                m2Hecha ? ColonialUIHelper.VarianteBoton.Dorado : ColonialUIHelper.VarianteBoton.Verde,
                11))
            {
                Time.timeScale = 1f;
                SceneManager.LoadScene("Nivel2_DesafioNieves");
            }

            // Botón Misión 3: Duelo en la Plaza
            bool m3Desbloqueada = m1Hecha || m2Hecha;
            string txtM3 = m3Hecha
                ? "⭐ 3. DUELO EN LA PLAZA\n[ Completada ]"
                : (m3Desbloqueada
                    ? "📜 3. DUELO EN LA PLAZA\n[ Ver en Tablero ]"
                    : "🔒 3. DUELO EN LA PLAZA\n[ Supera Misión 1 o 2 ]");
            if (ColonialUIHelper.BotonColonial(
                new Rect(rDock.x + padX + (wBoton + gap) * 2f, yBoton, wBoton, hBoton),
                txtM3,
                m3Desbloqueada ? ColonialUIHelper.VarianteBoton.Talavera : ColonialUIHelper.VarianteBoton.Gris,
                11))
            {
                AbrirMisionesDolores();
            }
        }

        private void DibujarModalAvisoBloqueo()
        {
            ColonialUIHelper.DibujarOverlayOscuro();

            float w = Mathf.Min(460f, Screen.width - 32f);
            float h = 210f;
            Rect rModal = new Rect((Screen.width - w) * 0.5f, (Screen.height - h) * 0.5f, w, h);

            ColonialUIHelper.DibujarMarcoVentana(
                rModal,
                "🔒 CAMINO A SAN FELIPE CUSTODIADO",
                "Ruta Norte de los Caminos de Guanajuato"
            );

            GUI.Label(
                new Rect(rModal.x + 22f, rModal.y + 78f, rModal.width - 44f, 68f),
                "Las guardias reales vigilan el paso hacia <b>San Felipe (Torres Mochas)</b>.\nCompleta las <b>3 misiones de Dolores Hidalgo</b> para que el comité insurgente te entregue el Salvoconducto.",
                estiloSubtituloBarra
            );

            float wBtn = 190f;
            float hBtn = 36f;
            Rect rBtn = new Rect(rModal.x + (rModal.width - wBtn) * 0.5f, rModal.yMax - hBtn - 16f, wBtn, hBtn);
            if (ColonialUIHelper.BotonColonial(rBtn, "🏛️ VER MISIONES", ColonialUIHelper.VarianteBoton.Dorado, 13))
            {
                mostrarAvisoBloqueoSanFelipe = false;
                AbrirMisionesDolores();
            }
        }

        private int ObtenerTotalCompletadas()
        {
            if (ProgresoJuegoManager.Instance != null)
            {
                return ProgresoJuegoManager.Instance.ObtenerTotalCompletadasDolores();
            }
            return PlayerPrefs.GetInt(ProgresoJuegoManager.KEY_MISION1, 0)
                 + PlayerPrefs.GetInt(ProgresoJuegoManager.KEY_MISION2, 0)
                 + PlayerPrefs.GetInt(ProgresoJuegoManager.KEY_MISION3, 0);
        }

        public void AbrirMisionesDolores()
        {
            if (AudioColonialManager.Instance != null) AudioColonialManager.Instance.ReproducirClickBoton();
            if (selectorMisionesDolores != null)
            {
                selectorMisionesDolores.Abrir();
            }
            else
            {
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
