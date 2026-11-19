using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using CaminosDeGuanajuato.Core;
using CaminosDeGuanajuato.UI;

namespace CaminosDeGuanajuato.Nieves
{
    /// <summary>
    /// Interfaz gráfica (HUD) para el Nivel 2: El Desafío de las Nieves Tradicionales.
    /// Presenta las estaciones de preparación culinaria con ensamble visual en tiempo real,
    /// la comanda del cliente activo, textos flotantes de puntuación, la guía interactiva
    /// de bienvenida con Don Chon y las pantallas modales coloniales.
    /// </summary>
    public class HUDNeveriaUI : MonoBehaviour
    {
        private GUIStyle estiloCajaOscura;
        private GUIStyle estiloCajaComanda;
        private GUIStyle estiloCajaDonChon;
        private GUIStyle estiloTitulo;
        private GUIStyle estiloSubtitulo;
        private GUIStyle estiloPuntaje;
        private GUIStyle estiloTemporizador;
        private GUIStyle estiloBotonAccion;
        private GUIStyle estiloBotonSeleccionado;
        private GUIStyle estiloBotonServir;
        private GUIStyle estiloBotonDesechar;
        private GUIStyle estiloModalVictoria;
        private GUIStyle estiloModalDerrota;
        private GUIStyle estiloTextoBlanco;

        private Texture2D texFondoColonial;
        private Texture2D texFondoComanda;
        private Texture2D texFondoDonChon;
        private Texture2D texBarraVerde;
        private Texture2D texBarraAmarilla;
        private Texture2D texBarraRoja;
        private Texture2D texBarraFondo;
        private Texture2D texBotonAzul;
        private Texture2D texBotonDorado;
        private Texture2D texBotonVerde;
        private Texture2D texBotonCarmesi;
        private Texture2D texOverlay;
        private Texture2D texOverlayNegro;

        // Sprites de animación de Garrafa artesanal (4 etapas visuales de raspado)
        private Texture2D[] texGarrafaEtapas = new Texture2D[4];

        // Sprites de Iconos de ingredientes
        private Texture2D texIconoBarquillo;
        private Texture2D texIconoVasoBarro;
        private Texture2D texIconoGarambullo;
        private Texture2D texIconoBorrachita;
        private Texture2D texIconoQuesoFresa;
        private Texture2D texIconoAguacate;
        private Texture2D texIconoLimon;

        // Sprite de Don Chon y Doña Remedios para el diálogo interactivo de inicio
        private Texture2D texDonChonGuia;
        private Texture2D texDonaRemediosGuia;
        private int indiceDialogoGuia = 0;
        private ColonialUIHelper.LineaDialogoVN[] lineasDialogoGuiaNeveria;

        // Sprites de Botones temáticos coloniales (Madera tallada, Talavera, Verde colonial, Terracota)
        private Texture2D texBotonMadera;
        private Texture2D texBotonTalavera;
        private Texture2D texBotonVerdeColonial;
        private Texture2D texBotonRojoColonial;

        private float tiempoUltimoRaspado = -1f;

        // Estado de Guía de Inicio (Onboarding con Don Chon)
        private bool mostrarGuiaInicial = true;

        // Estado de Pausa y Opciones
        private bool estaPausado = false;
        private bool mostrarOpcionesPausa = false;

        private float volumenGeneral = 0.85f;
        private float volumenMusica = 0.65f;
        private float volumenSFX = 0.85f;
        private bool pantallaCompleta = true;

        // Sistema de Efectos Visuales y Textos Flotantes (Juice)
        private class TextoFlotante
        {
            public string texto;
            public Color color;
            public Vector3 posMundo;
            public float tiempoVida;
            public float duracion;
        }
        private List<TextoFlotante> textosFlotantes = new List<TextoFlotante>();

        public Texture2D ObtenerIconoRecipiente(TipoRecipiente rec)
        {
            return rec switch
            {
                TipoRecipiente.BarquilloCrujiente => texIconoBarquillo,
                TipoRecipiente.VasoDeBarro => texIconoVasoBarro,
                _ => null
            };
        }

        public Texture2D ObtenerIconoSabor(SaborNieve sabor)
        {
            return sabor switch
            {
                SaborNieve.Garambullo => texIconoGarambullo,
                SaborNieve.Borrachita => texIconoBorrachita,
                SaborNieve.QuesoConFresa => texIconoQuesoFresa,
                SaborNieve.Aguacate => texIconoAguacate,
                SaborNieve.LimonVirreinal => texIconoLimon,
                _ => null
            };
        }

        private void Awake()
        {
            CrearTexturas();
        }

        private void OnEnable()
        {
            if (NeveriaGameManager.Instance != null)
            {
                NeveriaGameManager.Instance.OnNotificacionFlotante += AgregarTextoFlotante;
            }
        }

        private void OnDisable()
        {
            if (NeveriaGameManager.Instance != null)
            {
                NeveriaGameManager.Instance.OnNotificacionFlotante -= AgregarTextoFlotante;
            }
        }

        private void Start()
        {
            CrearTexturas();
            pantallaCompleta = Screen.fullScreen;

            // Abrir la guía al iniciar para que el jugador conozca su misión de inmediato
            mostrarGuiaInicial = true;
            Time.timeScale = 0f;

            if (NeveriaGameManager.Instance != null)
            {
                NeveriaGameManager.Instance.OnNotificacionFlotante -= AgregarTextoFlotante;
                NeveriaGameManager.Instance.OnNotificacionFlotante += AgregarTextoFlotante;
            }

            if (AudioColonialManager.Instance != null)
            {
                volumenGeneral = AudioColonialManager.Instance.VolumenGeneral;
                volumenMusica = AudioColonialManager.Instance.VolumenMusica;
                volumenSFX = AudioColonialManager.Instance.VolumenSFX;
            }
        }

        private void AgregarTextoFlotante(string texto, Color color, Vector3 posMundo)
        {
            textosFlotantes.Add(new TextoFlotante
            {
                texto = texto,
                color = color,
                posMundo = posMundo,
                tiempoVida = 1.6f,
                duracion = 1.6f
            });
        }

        private void Update()
        {
            // Actualizar animación de textos flotantes
            for (int i = textosFlotantes.Count - 1; i >= 0; i--)
            {
                var tf = textosFlotantes[i];
                tf.tiempoVida -= Time.deltaTime;
                tf.posMundo += Vector3.up * (Time.deltaTime * 0.75f);
                if (tf.tiempoVida <= 0f)
                {
                    textosFlotantes.RemoveAt(i);
                }
            }

            // Atajo para abrir/cerrar la guía con H o F1
            if (Input.GetKeyDown(KeyCode.H) || Input.GetKeyDown(KeyCode.F1))
            {
                if (!estaPausado && NeveriaGameManager.Instance != null &&
                    NeveriaGameManager.Instance.EstadoJuego == EstadoJuegoNeveria.Jugando)
                {
                    if (!mostrarGuiaInicial)
                    {
                        AbrirGuia();
                    }
                    else
                    {
                        CerrarGuiaInicial();
                    }
                }
            }

            if (mostrarGuiaInicial)
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
            {
                if (NeveriaGameManager.Instance != null &&
                    NeveriaGameManager.Instance.EstadoJuego != EstadoJuegoNeveria.Victoria &&
                    NeveriaGameManager.Instance.EstadoJuego != EstadoJuegoNeveria.Derrota)
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
        }

        public void CerrarGuiaInicial()
        {
            mostrarGuiaInicial = false;
            Time.timeScale = 1f;
            if (AudioColonialManager.Instance != null)
            {
                AudioColonialManager.Instance.ReproducirClickBoton();
            }
        }

        public void AbrirGuia()
        {
            mostrarGuiaInicial = true;
            indiceDialogoGuia = 0;
            ColonialUIHelper.ReiniciarSecuenciaDialogoVN();
            Time.timeScale = 0f;
            if (AudioColonialManager.Instance != null)
            {
                AudioColonialManager.Instance.ReproducirClickBoton();
            }
        }

        private Texture2D CargarTexturaAsset(string rutaRelativa)
        {
            string nombreSinExt = System.IO.Path.GetFileNameWithoutExtension(rutaRelativa);

            // 1. Cargar desde Resources (100% compatible con WebGL / itch.io)
            Texture2D resTex = Resources.Load<Texture2D>(nombreSinExt);
            if (resTex != null)
            {
                resTex.filterMode = FilterMode.Point;
                return resTex;
            }

            Sprite resSprite = Resources.Load<Sprite>(nombreSinExt);
            if (resSprite != null && resSprite.texture != null)
            {
                return resSprite.texture;
            }

            // 2. Fallback de desarrollo para Editor / PC
            try
            {
                string ruta = System.IO.Path.Combine(Application.dataPath, rutaRelativa);
                if (System.IO.File.Exists(ruta))
                {
                    byte[] data = System.IO.File.ReadAllBytes(ruta);
                    Texture2D tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                    if (tex.LoadImage(data))
                    {
                        tex.filterMode = FilterMode.Point;
                        return tex;
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"[HUD] Error cargando sprite {rutaRelativa}: {ex.Message}");
            }
            return null;
        }

        private void CrearTexturas()
        {
            texFondoColonial = new Texture2D(1, 1);
            texFondoColonial.SetPixel(0, 0, new Color(0.1f, 0.06f, 0.04f, 0.88f));
            texFondoColonial.Apply();

            texFondoComanda = new Texture2D(1, 1);
            texFondoComanda.SetPixel(0, 0, new Color(0.15f, 0.1f, 0.05f, 0.92f));
            texFondoComanda.Apply();

            texFondoDonChon = new Texture2D(1, 1);
            texFondoDonChon.SetPixel(0, 0, new Color(0.08f, 0.16f, 0.28f, 0.90f));
            texFondoDonChon.Apply();

            texOverlay = new Texture2D(1, 1);
            texOverlay.SetPixel(0, 0, new Color(0.04f, 0.02f, 0.02f, 0.8f));
            texOverlay.Apply();

            texOverlayNegro = new Texture2D(1, 1);
            texOverlayNegro.SetPixel(0, 0, new Color(0.02f, 0.02f, 0.03f, 0.88f));
            texOverlayNegro.Apply();

            texBarraFondo = new Texture2D(1, 1);
            texBarraFondo.SetPixel(0, 0, new Color(0.2f, 0.2f, 0.2f, 1f));
            texBarraFondo.Apply();

            texBarraVerde = new Texture2D(1, 1);
            texBarraVerde.SetPixel(0, 0, new Color(0.2f, 0.85f, 0.3f, 1f));
            texBarraVerde.Apply();

            texBarraAmarilla = new Texture2D(1, 1);
            texBarraAmarilla.SetPixel(0, 0, new Color(0.95f, 0.82f, 0.15f, 1f));
            texBarraAmarilla.Apply();

            texBarraRoja = new Texture2D(1, 1);
            texBarraRoja.SetPixel(0, 0, new Color(0.85f, 0.2f, 0.15f, 1f));
            texBarraRoja.Apply();

            texBotonAzul = new Texture2D(1, 1);
            texBotonAzul.SetPixel(0, 0, new Color(0.14f, 0.32f, 0.58f, 0.95f));
            texBotonAzul.Apply();

            texBotonDorado = new Texture2D(1, 1);
            texBotonDorado.SetPixel(0, 0, new Color(0.92f, 0.68f, 0.15f, 0.95f));
            texBotonDorado.Apply();

            texBotonVerde = new Texture2D(1, 1);
            texBotonVerde.SetPixel(0, 0, new Color(0.15f, 0.65f, 0.25f, 0.95f));
            texBotonVerde.Apply();

            texBotonCarmesi = new Texture2D(1, 1);
            texBotonCarmesi.SetPixel(0, 0, new Color(0.65f, 0.15f, 0.15f, 0.95f));
            texBotonCarmesi.Apply();

            // 1. Cargar las 4 etapas de la garrafa artesanal de nieve
            for (int i = 0; i < 4; i++)
            {
                if (texGarrafaEtapas[i] == null)
                {
                    texGarrafaEtapas[i] = CargarTexturaAsset($"Sprites/NievesUI/Garrafa_Etapa{i}.png");
                }
            }

            // 2. Cargar iconos de ingredientes
            if (texIconoBarquillo == null) texIconoBarquillo = CargarTexturaAsset("Sprites/NievesUI/Icono_Barquillo.png");
            if (texIconoVasoBarro == null) texIconoVasoBarro = CargarTexturaAsset("Sprites/NievesUI/Icono_VasoBarro.png");
            if (texIconoGarambullo == null) texIconoGarambullo = CargarTexturaAsset("Sprites/NievesUI/Icono_Garambullo.png");
            if (texIconoBorrachita == null) texIconoBorrachita = CargarTexturaAsset("Sprites/NievesUI/Icono_Borrachita.png");
            if (texIconoQuesoFresa == null) texIconoQuesoFresa = CargarTexturaAsset("Sprites/NievesUI/Icono_QuesoFresa.png");
            if (texIconoAguacate == null) texIconoAguacate = CargarTexturaAsset("Sprites/NievesUI/Icono_Aguacate.png");
            if (texIconoLimon == null) texIconoLimon = CargarTexturaAsset("Sprites/NievesUI/Icono_Limon.png");

            // 3. Cargar retratos de Don Chon y Doña Remedios para el diálogo interactivo
            if (texDonChonGuia == null)
            {
                texDonChonGuia = Resources.Load<Texture2D>("PersonajesColonial/DonChon");
                if (texDonChonGuia == null)
                {
                    texDonChonGuia = CargarTexturaAsset("Sprites/PersonajesColonial/DonChon.png");
                }
            }
            if (texDonaRemediosGuia == null)
            {
                texDonaRemediosGuia = Resources.Load<Texture2D>("PersonajesColonial/DonaRemedios");
                if (texDonaRemediosGuia == null)
                {
                    texDonaRemediosGuia = CargarTexturaAsset("Sprites/PersonajesColonial/DonaRemedios.png");
                }
            }

            if (lineasDialogoGuiaNeveria == null)
            {
                lineasDialogoGuiaNeveria = new ColonialUIHelper.LineaDialogoVN[]
                {
                    new ColonialUIHelper.LineaDialogoVN(
                        "Don Chon",
                        "¡Qué alegría verte en el quiosco, <b>Guanajicho</b>! Hoy es día de plaza en Dolores y los vecinos vienen con sed de nuestras nieves artesanales.",
                        true,
                        "🍨 OBJETIVO: Atiende a 8 comensales antes de que pierdan la paciencia",
                        0.92f
                    ),
                    new ColonialUIHelper.LineaDialogoVN(
                        "Doña Remedios",
                        "¡Buenos días, Don Chon! Yo voy a querer mi nieve bien fría, fijándose muy bien si la pido en <b>Barquillo</b> o en <b>Vaso de Barro</b>.",
                        false,
                        "📜 PASO 1: Mira la comanda arriba a la izquierda y elige [1] Barquillo o [2] Vaso",
                        1.15f
                    ),
                    new ColonialUIHelper.LineaDialogoVN(
                        "Don Chon",
                        "¡Ya la escuchaste! Después de elegir el recipiente, <b>bate la garrafa con hielo y sal</b> pulsando <b>[ESPACIO]</b> varias veces hasta formar escarcha.",
                        true,
                        "🧊 PASO 2: Pulsa [ESPACIO] o haz clic repetido para raspar el hielo al 100%",
                        0.92f
                    ),
                    new ColonialUIHelper.LineaDialogoVN(
                        "Don Chon",
                        "Por último, agrega el <b>sabor tradicional</b> que te pidieron y pulsa <b>[ENTREGAR]</b>. ¡Vamos a ganar esa Cuchara Dorada!",
                        true,
                        "🍓 PASO 3: Elige el sabor [Q, W, E, R, T] y sirve con [ENTER o Clic]",
                        0.95f
                    )
                };
            }

            // 4. Crear botones temáticos procedurales
            texBotonMadera = CrearTexturaBotonColonial(
                new Color(0.20f, 0.12f, 0.08f, 0.98f),
                new Color(0.60f, 0.42f, 0.22f, 1f),
                new Color(0.08f, 0.05f, 0.03f, 1f),
                new Color(0.85f, 0.68f, 0.30f, 1f)
            );

            texBotonTalavera = CrearTexturaBotonColonial(
                new Color(0.05f, 0.12f, 0.25f, 0.98f),
                new Color(1.0f, 0.84f, 0.15f, 1f),
                new Color(0.80f, 0.55f, 0.08f, 1f),
                new Color(1.0f, 0.98f, 0.50f, 1f)
            );

            texBotonVerdeColonial = CrearTexturaBotonColonial(
                new Color(0.08f, 0.38f, 0.15f, 0.98f),
                new Color(0.95f, 0.82f, 0.22f, 1f),
                new Color(0.04f, 0.18f, 0.07f, 1f),
                new Color(1.0f, 0.95f, 0.40f, 1f)
            );

            texBotonRojoColonial = CrearTexturaBotonColonial(
                new Color(0.40f, 0.12f, 0.10f, 0.98f),
                new Color(0.80f, 0.38f, 0.22f, 1f),
                new Color(0.18f, 0.05f, 0.05f, 1f),
                new Color(0.95f, 0.60f, 0.40f, 1f)
            );
        }

        private Texture2D CrearTexturaBotonColonial(Color colorFondo, Color colorBorde, Color colorBordeExterior, Color colorRemache, int w = 64, int h = 64)
        {
            Texture2D tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Point;
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    bool esBordeExt = (x < 2 || x >= w - 2 || y < 2 || y >= h - 2);
                    bool esBordeInt = (x >= 2 && x < 5) || (x >= w - 5 && x < w - 2) || (y >= 2 && y < 5) || (y >= h - 5 && y < h - 2);
                    bool esRemache = ((x >= 6 && x <= 8) || (x >= w - 9 && x <= w - 7)) && ((y >= 6 && y <= 8) || (y >= h - 9 && y <= h - 7));

                    if (esBordeExt)
                    {
                        tex.SetPixel(x, y, colorBordeExterior);
                    }
                    else if (esRemache)
                    {
                        tex.SetPixel(x, y, colorRemache);
                    }
                    else if (esBordeInt)
                    {
                        tex.SetPixel(x, y, colorBorde);
                    }
                    else
                    {
                        tex.SetPixel(x, y, colorFondo);
                    }
                }
            }
            tex.Apply();
            return tex;
        }

        private void InicializarEstilos()
        {
            if (estiloTitulo != null && texBotonMadera != null) return;
            CrearTexturas();

            estiloCajaOscura = new GUIStyle(GUI.skin.box);
            estiloCajaOscura.normal.background = texFondoColonial;

            estiloCajaComanda = new GUIStyle(GUI.skin.box);
            estiloCajaComanda.normal.background = texFondoComanda;

            estiloCajaDonChon = new GUIStyle(GUI.skin.box);
            estiloCajaDonChon.normal.background = texFondoDonChon;

            estiloModalVictoria = new GUIStyle(GUI.skin.box);
            estiloModalVictoria.normal.background = texFondoColonial;

            estiloModalDerrota = new GUIStyle(GUI.skin.box);
            estiloModalDerrota.normal.background = texFondoColonial;

            // FUENTES GRANDES Y CLARAS PARA PANTALLA COMPLETA
            estiloTitulo = new GUIStyle(GUI.skin.label);
            estiloTitulo.fontSize = 24;
            estiloTitulo.fontStyle = FontStyle.Bold;
            estiloTitulo.alignment = TextAnchor.MiddleCenter;
            estiloTitulo.normal.textColor = new Color(1.0f, 0.85f, 0.25f);

            estiloSubtitulo = new GUIStyle(GUI.skin.label);
            estiloSubtitulo.fontSize = 15;
            estiloSubtitulo.fontStyle = FontStyle.Italic;
            estiloSubtitulo.alignment = TextAnchor.MiddleCenter;
            estiloSubtitulo.normal.textColor = new Color(0.92f, 0.92f, 0.92f);

            estiloPuntaje = new GUIStyle(GUI.skin.label);
            estiloPuntaje.fontSize = 17;
            estiloPuntaje.fontStyle = FontStyle.Bold;
            estiloPuntaje.normal.textColor = new Color(1f, 0.92f, 0.40f);

            estiloTemporizador = new GUIStyle(GUI.skin.label);
            estiloTemporizador.fontSize = 17;
            estiloTemporizador.fontStyle = FontStyle.Bold;
            estiloTemporizador.normal.textColor = new Color(0.35f, 0.95f, 1f);

            estiloTextoBlanco = new GUIStyle(GUI.skin.label);
            estiloTextoBlanco.fontSize = 15;
            estiloTextoBlanco.normal.textColor = Color.white;

            estiloBotonAccion = new GUIStyle(GUI.skin.button);
            estiloBotonAccion.fontSize = 15;
            estiloBotonAccion.fontStyle = FontStyle.Bold;
            estiloBotonAccion.border = new RectOffset(10, 10, 10, 10);
            estiloBotonAccion.normal.background = texBotonMadera;
            estiloBotonAccion.normal.textColor = new Color(0.98f, 0.94f, 0.85f);

            estiloBotonSeleccionado = new GUIStyle(estiloBotonAccion);
            estiloBotonSeleccionado.fontSize = 16;
            estiloBotonSeleccionado.border = new RectOffset(10, 10, 10, 10);
            estiloBotonSeleccionado.normal.background = texBotonTalavera;
            estiloBotonSeleccionado.normal.textColor = new Color(1.0f, 0.96f, 0.50f);

            estiloBotonServir = new GUIStyle(GUI.skin.button);
            estiloBotonServir.fontSize = 17;
            estiloBotonServir.fontStyle = FontStyle.Bold;
            estiloBotonServir.border = new RectOffset(10, 10, 10, 10);
            estiloBotonServir.normal.background = texBotonVerdeColonial;
            estiloBotonServir.normal.textColor = Color.white;

            estiloBotonDesechar = new GUIStyle(GUI.skin.button);
            estiloBotonDesechar.fontSize = 14;
            estiloBotonDesechar.fontStyle = FontStyle.Bold;
            estiloBotonDesechar.border = new RectOffset(10, 10, 10, 10);
            estiloBotonDesechar.normal.background = texBotonRojoColonial;
            estiloBotonDesechar.normal.textColor = Color.white;
        }

        private void OnGUI()
        {
            if (SceneManager.GetActiveScene().name != "Nivel2_DesafioNieves")
            {
                estaPausado = false;
                mostrarOpcionesPausa = false;
                Destroy(gameObject);
                return;
            }

            InicializarEstilos();

            if (NeveriaGameManager.Instance == null) return;

            // 1. Si la guía de inicio está activa, dibujarla y pausar la vista de juego
            if (mostrarGuiaInicial)
            {
                DibujarModalGuiaInicial();
                return;
            }

            // 2. Comprobar estado de fin de partida
            if (NeveriaGameManager.Instance.EstadoJuego == EstadoJuegoNeveria.Victoria)
            {
                DibujarModalVictoria();
                return;
            }
            if (NeveriaGameManager.Instance.EstadoJuego == EstadoJuegoNeveria.Derrota)
            {
                DibujarModalDerrota();
                return;
            }

            // 3. Menú de pausa u opciones
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

            // 4. Barra Superior de Telemetría con botón de Guía [?]
            DibujarBarraSuperior();

            // 5. Comanda del Cliente y Antojo (Grande y Clara)
            DibujarComandaYConsejero();

            // 6. Estación de Preparación Guiada con Ensamble en Tiempo Real
            DibujarMesaPreparacionGuiada();

            // 7. Notificaciones Flotantes y Emociones (Juice)
            DibujarTextosFlotantes();
        }

        /// <summary>
        /// Dibuja los textos flotantes animados sobre la cabeza de los comensales
        /// </summary>
        private void DibujarTextosFlotantes()
        {
            Camera cam = Camera.main;
            if (cam == null || textosFlotantes.Count == 0) return;

            GUIStyle stFlotante = new GUIStyle(GUI.skin.label)
            {
                fontSize = 20,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
            GUIStyle stSombra = new GUIStyle(stFlotante);

            for (int i = 0; i < textosFlotantes.Count; i++)
            {
                var tf = textosFlotantes[i];
                Vector3 screenP = cam.WorldToScreenPoint(tf.posMundo);
                if (screenP.z <= 0f) continue;

                float sx = screenP.x;
                float sy = Screen.height - screenP.y;

                float alpha = Mathf.Clamp01(tf.tiempoVida / 0.5f);
                Color c = new Color(tf.color.r, tf.color.g, tf.color.b, alpha);
                Color cSombra = new Color(0f, 0f, 0f, alpha * 0.95f);

                stFlotante.normal.textColor = c;
                stSombra.normal.textColor = cSombra;

                float w = 280f;
                float h = 36f;
                Rect r = new Rect(sx - w * 0.5f, sy - h * 0.5f, w, h);

                GUI.Label(new Rect(r.x + 2f, r.y + 2f, r.width, r.height), tf.texto, stSombra);
                GUI.Label(r, tf.texto, stFlotante);
            }
        }

        private float tiempoUltimaEscarcha = -1f;

        private void GenerarChispasEscarcha()
        {
            if (Time.time - tiempoUltimaEscarcha < 0.16f) return;
            tiempoUltimaEscarcha = Time.time;

            // Barrica de garrafa en el quiosco colonial
            Vector3 posBarrica = new Vector3(-1.60f + UnityEngine.Random.Range(-0.25f, 0.25f), -1.05f + UnityEngine.Random.Range(0f, 0.20f), 0f);
            string[] particulas = new string[] { "❄️", "✨", "🧊" };
            string p = particulas[UnityEngine.Random.Range(0, particulas.Length)];
            Color col = new Color(0.80f, 0.95f, 1f, 0.95f);
            AgregarTextoFlotante(p, col, posBarrica);
        }

        /// <summary>
        /// Diálogo interactivo paso a paso con Don Chon y Doña Remedios (estilo novela visual con globo de voz)
        /// </summary>
        private void DibujarModalGuiaInicial()
        {
            ColonialUIHelper.DibujarDialogoInteractivoVN(
                lineasDialogoGuiaNeveria,
                texDonChonGuia,
                texDonaRemediosGuia,
                ref indiceDialogoGuia,
                CerrarGuiaInicial
            );
        }

        private void DibujarTarjetaPasoGuia(string titulo, string descripcion, Texture2D icono)
        {
            Rect r = GUILayoutUtility.GetRect(GUILayoutUtility.GetRect(0, 0).width, 66f, GUILayout.ExpandWidth(true), GUILayout.Height(66f));
            ColonialUIHelper.DibujarTarjetaInterior(r);

            if (icono != null)
            {
                Rect rIco = new Rect(r.x + 8f, r.y + 10f, 44f, 44f);
                GUI.DrawTexture(rIco, icono, ScaleMode.ScaleToFit);
            }

            float xTxt = (icono != null) ? r.x + 60f : r.x + 10f;
            float wTxt = r.width - (xTxt - r.x) - 10f;

            GUIStyle stTit = new GUIStyle(GUI.skin.label)
            {
                fontSize = 15,
                fontStyle = FontStyle.Bold,
                wordWrap = false,
                clipping = TextClipping.Overflow,
                normal = { textColor = new Color(1.0f, 0.88f, 0.35f) }
            };
            GUI.Label(new Rect(xTxt, r.y + 6f, wTxt, 22f), titulo, stTit);

            GUIStyle stDesc = new GUIStyle(GUI.skin.label)
            {
                fontSize = 13,
                wordWrap = true,
                normal = { textColor = Color.white }
            };
            GUI.Label(new Rect(xTxt, r.y + 28f, wTxt, 34f), descripcion, stDesc);
        }

        private void DibujarBarraSuperior()
        {
            float ancho = Screen.width - 24f;
            float alto = 46f;
            Rect rBarra = new Rect(12f, 8f, ancho, alto);

            ColonialUIHelper.DibujarTarjetaInterior(rBarra);

            int at = NeveriaGameManager.Instance.ClientesAtendidos;
            int obj = NeveriaGameManager.Instance.ClientesObjetivo;
            int perdidos = NeveriaGameManager.Instance.ClientesPerdidos;
            int maxPerd = NeveriaGameManager.Instance.MaxClientesPerdidos;
            string strikes = "";
            for (int i = 0; i < maxPerd; i++)
            {
                strikes += (i < perdidos) ? "💔" : "❤️";
            }
            float t = NeveriaGameManager.Instance.TiempoRestante;

            // Botón de Ayuda y Botón de Pausa fijos a la derecha
            float wPausa = 112f;
            float wGuia = 92f;
            Rect rBtnPausa = new Rect(rBarra.xMax - wPausa - 8f, rBarra.y + 7f, wPausa, 32f);
            Rect rBtnGuia = new Rect(rBtnPausa.x - wGuia - 6f, rBarra.y + 7f, wGuia, 32f);

            if (ColonialUIHelper.BotonColonial(rBtnGuia, "❓ GUÍA [H]", ColonialUIHelper.VarianteBoton.Talavera, 12))
            {
                AbrirGuia();
            }

            if (ColonialUIHelper.BotonColonial(rBtnPausa, "⏸ PAUSA [ESC]", ColonialUIHelper.VarianteBoton.Madera, 12))
            {
                AlternarPausa();
            }

            // Anchos optimizados sin solapamiento ni saltos de línea
            float yTxt = rBarra.y + 10f;
            float hTxt = 26f;
            float xCur = rBarra.x + 12f;

            GUIStyle stNom = new GUIStyle(estiloPuntaje) { wordWrap = false, clipping = TextClipping.Overflow, fontSize = 16 };
            GUI.Label(new Rect(xCur, yTxt, 185f, hTxt), "🍨 NIEVES DE DOLORES", stNom);
            xCur += 190f;

            GUIStyle stGen = new GUIStyle(estiloTextoBlanco) { wordWrap = false, clipping = TextClipping.Overflow, fontSize = 14 };
            GUI.Label(new Rect(xCur, yTxt, 130f, hTxt), $"👥 Comensales: <b>{at}/{obj}</b>", stGen);
            xCur += 135f;

            GUI.Label(new Rect(xCur, yTxt, 100f, hTxt), $"Paciencia: {strikes}", stGen);
            xCur += 105f;

            GUIStyle stTiempo = new GUIStyle(estiloTemporizador) { wordWrap = false, clipping = TextClipping.Overflow, fontSize = 15 };
            GUI.Label(new Rect(xCur, yTxt, 75f, hTxt), $"⏳ {Mathf.CeilToInt(t):D2}s", stTiempo);
            xCur += 80f;

            GUI.Label(new Rect(xCur, yTxt, 95f, hTxt), $"⭐ {NeveriaGameManager.Instance.PuntajeTotal} pts", stNom);
        }

        private void DibujarComandaYConsejero()
        {
            var cliente = NeveriaGameManager.Instance.ClienteActual;
            float ancho = Mathf.Clamp(Screen.width * 0.46f, 440f, 540f);
            float alto = 124f;
            Rect rComanda = new Rect(12f, 60f, ancho, alto);

            ColonialUIHelper.DibujarTarjetaInterior(rComanda);

            GUILayout.BeginArea(new Rect(rComanda.x + 12f, rComanda.y + 8f, rComanda.width - 24f, rComanda.height - 16f));

            if (cliente == null || cliente.Pedido == null)
            {
                GUILayout.Label("⏳ Viene llegando un nuevo visitante por la plaza empedrada...", estiloSubtitulo);
                if (DonChonConsejero.Instance != null)
                {
                    GUILayout.Space(6);
                    GUILayout.Label($"Don Chon: «{DonChonConsejero.Instance.MensajeActual}»", estiloTextoBlanco);
                }
                GUILayout.EndArea();
                return;
            }

            var pedido = cliente.Pedido;
            string recNombre = PedidoNieve.ObtenerNombreRecipiente(pedido.recipienteDeseado);
            string saborNombre = PedidoNieve.ObtenerNombreSabor(pedido.saborDeseado);
            Texture2D texRec = ObtenerIconoRecipiente(pedido.recipienteDeseado);
            Texture2D texSab = ObtenerIconoSabor(pedido.saborDeseado);

            GUILayout.BeginHorizontal();
            GUIStyle stNomCliente = new GUIStyle(estiloTextoBlanco) { fontSize = 17, fontStyle = FontStyle.Bold, wordWrap = false };
            GUILayout.Label($"🛎️ <b>{cliente.NombreCliente}</b> pide:", stNomCliente, GUILayout.Width(rComanda.width - 130f));
            GUILayout.FlexibleSpace();

            // Tensión: El reloj parpadea en rojo brillante cuando queda poco tiempo (< 35%)
            Color colorTimer = (cliente.PorcentajePaciencia < 0.35f && Mathf.PingPong(Time.time * 6f, 1f) > 0.4f)
                ? new Color(1f, 0.30f, 0.30f)
                : new Color(0.35f, 0.95f, 1f);
            GUIStyle stTimerPulsante = new GUIStyle(estiloTemporizador) { normal = { textColor = colorTimer }, wordWrap = false };
            GUILayout.Label($"⏱ <b>{Mathf.CeilToInt(pedido.tiempoPacienciaRestante)}s</b>", stTimerPulsante);
            GUILayout.EndHorizontal();

            GUILayout.Space(4);

            // Fila visual con iconos grandes de 36px y textos en 16px
            GUILayout.BeginHorizontal();

            if (texRec != null)
            {
                Rect rIcoRec = GUILayoutUtility.GetRect(36, 36, GUILayout.Width(36), GUILayout.Height(36));
                GUI.DrawTexture(rIcoRec, texRec, ScaleMode.ScaleToFit);
                GUILayout.Space(6);
            }
            GUIStyle stItem = new GUIStyle(estiloTextoBlanco) { fontSize = 15, fontStyle = FontStyle.Bold, wordWrap = false };
            GUILayout.Label($"<color=#ffdd44>{recNombre}</color>", stItem, GUILayout.Width(165));

            GUILayout.Space(10);

            if (texSab != null)
            {
                Rect rIcoSab = GUILayoutUtility.GetRect(36, 36, GUILayout.Width(36), GUILayout.Height(36));
                GUI.DrawTexture(rIcoSab, texSab, ScaleMode.ScaleToFit);
                GUILayout.Space(6);
            }
            GUILayout.Label($"<color=#ffaa33>{saborNombre}</color>", stItem);

            GUILayout.EndHorizontal();

            GUILayout.Space(6);

            // Barra de paciencia fluida y visible (12px de alto) con parpadeo urgente en rojo
            Rect rBarra = GUILayoutUtility.GetRect(rComanda.width - 24f, 12f);
            if (texBarraFondo != null) GUI.DrawTexture(rBarra, texBarraFondo);
            float pct = cliente.PorcentajePaciencia;
            Texture2D texColor = (pct > 0.5f) ? texBarraVerde : (pct > 0.25f ? texBarraAmarilla : texBarraRoja);
            if (texColor != null) GUI.DrawTexture(new Rect(rBarra.x, rBarra.y, rBarra.width * pct, rBarra.height), texColor);

            GUILayout.EndArea();
        }

        private void DibujarMesaPreparacionGuiada()
        {
            if (MesaPreparacion.Instance == null) return;

            float anchoPanel = Mathf.Min(980f, Screen.width - 24f);
            float altoPanel = 165f;
            float xPanel = (Screen.width - anchoPanel) * 0.5f;
            float yPanel = Screen.height - altoPanel - 6f;

            Rect rMesa = new Rect(xPanel, yPanel, anchoPanel, altoPanel);
            ColonialUIHelper.DibujarTarjetaInterior(rMesa);

            GUILayout.BeginArea(new Rect(rMesa.x + 12f, rMesa.y + 6f, rMesa.width - 24f, rMesa.height - 12f));
            GUILayout.Space(2);

            // 1. Barra de Progreso de Pasos (Breadcrumbs visuales legibles que nunca se cortan)
            bool tieneBase = MesaPreparacion.Instance.RecipienteActual != TipoRecipiente.Ninguno;
            bool tieneHielo = MesaPreparacion.Instance.TieneHieloListo;
            bool tieneSabor = MesaPreparacion.Instance.SaborActual != SaborNieve.Ninguno;
            bool nieveLista = tieneBase && tieneHielo && tieneSabor;

            string txtP1 = tieneBase ? $"✓ {PedidoNieve.ObtenerNombreRecipiente(MesaPreparacion.Instance.RecipienteActual)}" : "1. Base";
            string txtP2 = tieneHielo ? "✓ Hielo Listo" : (tieneBase ? $"2. Batir ({Mathf.RoundToInt(MesaPreparacion.Instance.ProgresoHielo * 100)}%)" : "2. Hielo");
            string txtP3 = tieneSabor ? $"✓ {PedidoNieve.ObtenerNombreSabor(MesaPreparacion.Instance.SaborActual)}" : "3. Sabor";
            string txtP4 = nieveLista ? "4. ¡Servir!" : "4. Servir";

            GUILayout.BeginHorizontal();
            GUIStyle stPrep = new GUIStyle(estiloPuntaje) { wordWrap = false, clipping = TextClipping.Overflow, fontSize = 16 };
            GUILayout.Label("🍨 PREPARACIÓN:", stPrep, GUILayout.Width(135));
            GUIStyle stStep = new GUIStyle(estiloTextoBlanco) { wordWrap = false, clipping = TextClipping.Overflow };
            GUILayout.Label(tieneBase ? $"<color=#55ff55>[ {txtP1} ]</color>" : $"<color=#ffdd44><b>[ {txtP1} ]</b></color>", stStep);
            GUILayout.Label("➔", stStep, GUILayout.Width(14));
            GUILayout.Label(tieneHielo ? $"<color=#55ff55>[ {txtP2} ]</color>" : (tieneBase ? $"<color=#ffdd44><b>[ {txtP2} ]</b></color>" : $"<color=#888888>[ {txtP2} ]</color>"), stStep);
            GUILayout.Label("➔", stStep, GUILayout.Width(14));
            GUILayout.Label(tieneSabor ? $"<color=#55ff55>[ {txtP3} ]</color>" : (tieneHielo ? $"<color=#ffdd44><b>[ {txtP3} ]</b></color>" : $"<color=#888888>[ {txtP3} ]</color>"), stStep);
            GUILayout.Label("➔", stStep, GUILayout.Width(14));
            GUILayout.Label(nieveLista ? $"<color=#55ff55><b>[ {txtP4} ]</b></color>" : $"<color=#888888>[ {txtP4} ]</color>", stStep);

            GUILayout.FlexibleSpace();
            if (MesaPreparacion.Instance.TieneArticuloEnPreparacion)
            {
                if (BotonConIcono("🗑️ Desechar", null, estiloBotonDesechar, 115, 28))
                {
                    MesaPreparacion.Instance.DesecharPreparacion();
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(6);

            // 2. Fila con Ranura de Ensamble Visual en Tiempo Real + Acciones del Paso
            GUILayout.BeginHorizontal();

            // SLOT DE ENSAMBLE VISUAL ARTESANAL (Muestra el helado armándose en tiempo real)
            Rect rSlotEnsamble = GUILayoutUtility.GetRect(130f, 108f, GUILayout.Width(130f), GUILayout.Height(108f));
            ColonialUIHelper.DibujarTarjetaInterior(rSlotEnsamble);

            if (!tieneBase)
            {
                GUIStyle stVacio = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 11,
                    fontStyle = FontStyle.Italic,
                    alignment = TextAnchor.MiddleCenter,
                    normal = { textColor = new Color(0.7f, 0.7f, 0.7f, 0.8f) }
                };
                GUI.Label(rSlotEnsamble, "🪵 Mostrador\n(Elige base)", stVacio);
            }
            else
            {
                // Dibujar base (Barquillo o Vaso de barro)
                Texture2D texBase = ObtenerIconoRecipiente(MesaPreparacion.Instance.RecipienteActual);
                if (texBase != null)
                {
                    float yBase = tieneSabor ? rSlotEnsamble.y + 44f : rSlotEnsamble.y + 24f;
                    GUI.DrawTexture(new Rect(rSlotEnsamble.x + 45f, yBase, 40f, 40f), texBase, ScaleMode.ScaleToFit);
                }

                // Si tiene hielo batido, mostrar escarcha tradicional
                if (tieneHielo && !tieneSabor)
                {
                    GUI.DrawTexture(new Rect(rSlotEnsamble.x + 48f, rSlotEnsamble.y + 10f, 34f, 34f), texGarrafaEtapas[3], ScaleMode.ScaleToFit);
                }

                // Si tiene sabor, mostrar bola de nieve artesanal con su pulpa
                if (tieneSabor)
                {
                    Texture2D texSab = ObtenerIconoSabor(MesaPreparacion.Instance.SaborActual);
                    if (texSab != null)
                    {
                        GUI.DrawTexture(new Rect(rSlotEnsamble.x + 47f, rSlotEnsamble.y + 10f, 36f, 36f), texSab, ScaleMode.ScaleToFit);
                    }
                }

                string descBase = (MesaPreparacion.Instance.RecipienteActual == TipoRecipiente.BarquilloCrujiente) ? "Barquillo" : "Vaso Barro";
                string descSabor = tieneSabor ? PedidoNieve.ObtenerNombreSabor(MesaPreparacion.Instance.SaborActual) : (tieneHielo ? "Hielo Listo" : "Sin batir");
                GUIStyle stDescArt = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 11,
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.LowerCenter,
                    wordWrap = false,
                    clipping = TextClipping.Overflow,
                    normal = { textColor = new Color(1f, 0.9f, 0.45f) }
                };
                GUI.Label(new Rect(rSlotEnsamble.x, rSlotEnsamble.yMax - 22f, rSlotEnsamble.width, 20f), $"{descBase} • {descSabor}", stDescArt);
            }

            GUILayout.Space(14);

            // Controles dinámicos del paso actual (en el resto del ancho)
            GUILayout.BeginVertical();

            float anchoRestante = rMesa.width - 170f;

            if (!tieneBase)
            {
                // PASO 1: Elegir Base (Sin etiqueta artificial de "Requerido")
                var cliente = NeveriaGameManager.Instance.ClienteActual;
                TipoRecipiente recPedido = (cliente != null && cliente.Pedido != null)
                    ? cliente.Pedido.recipienteDeseado
                    : TipoRecipiente.Ninguno;

                GUILayout.Label("👉 <b>PASO 1:</b> Selecciona el recipiente artesanal para preparar la nieve:", estiloSubtitulo);
                GUILayout.Space(6);
                GUILayout.BeginHorizontal();

                float wBtnBase = Mathf.Min(310f, (anchoRestante - 20f) * 0.48f);

                GUIStyle stBarquillo = (recPedido == TipoRecipiente.BarquilloCrujiente) ? estiloBotonSeleccionado : estiloBotonAccion;
                string lblBarquillo = "[1] Barquillo Crujiente";
                if (BotonConIcono(lblBarquillo, texIconoBarquillo, stBarquillo, wBtnBase, 54))
                {
                    MesaPreparacion.Instance.SeleccionarRecipiente(TipoRecipiente.BarquilloCrujiente);
                }

                GUILayout.Space(14);

                GUIStyle stVaso = (recPedido == TipoRecipiente.VasoDeBarro) ? estiloBotonSeleccionado : estiloBotonAccion;
                string lblVaso = "[2] Vaso de Barro";
                if (BotonConIcono(lblVaso, texIconoVasoBarro, stVaso, wBtnBase, 54))
                {
                    MesaPreparacion.Instance.SeleccionarRecipiente(TipoRecipiente.VasoDeBarro);
                }

                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
            else if (!tieneHielo)
            {
                // PASO 2: Batir Hielo en Garrafa con Animación visual en 4 etapas y partículas
                float pctHielo = MesaPreparacion.Instance.ProgresoHielo;
                int etapa = Mathf.Clamp(Mathf.FloorToInt(pctHielo * 3.99f), 0, 3);
                Texture2D texGarrafa = texGarrafaEtapas[etapa];

                bool estaRaspando = Input.GetKey(KeyCode.Space) || (Time.time - tiempoUltimoRaspado < 0.25f);
                if (Input.GetKey(KeyCode.Space))
                {
                    tiempoUltimoRaspado = Time.time;
                    GenerarChispasEscarcha();
                }

                GUILayout.Label($"👉 <b>PASO 2:</b> Bate y raspa la garrafa tradicional de madera ({Mathf.RoundToInt(pctHielo * 100)}%):", estiloSubtitulo);
                GUILayout.Space(4);

                GUILayout.BeginHorizontal();

                // Garrafa animada con vibración de raspado
                float offsetVibracion = estaRaspando ? (Mathf.Sin(Time.time * 40f) * 3f) : 0f;
                Rect rAreaGarrafa = GUILayoutUtility.GetRect(76, 76, GUILayout.Width(76), GUILayout.Height(76));
                Rect rGarrafaImg = new Rect(rAreaGarrafa.x + offsetVibracion, rAreaGarrafa.y, 76, 76);
                if (texGarrafa != null)
                {
                    GUI.DrawTexture(rGarrafaImg, texGarrafa, ScaleMode.ScaleToFit);
                }

                GUILayout.Space(14);

                float wAreaAccion = Mathf.Min(580f, anchoRestante - 100f);
                GUILayout.BeginVertical(GUILayout.Width(wAreaAccion));

                string nombreEtapa = etapa switch
                {
                    0 => "Garrafa de cobre enfriándose en barrica de sal y hielo...",
                    1 => "¡Aparece la primera escarcha de raspado artesanal!",
                    2 => "¡La masa de nieve artesanal toma consistencia y frío!",
                    3 => "¡Nieve suave, tersa y copeteada lista para el sabor!",
                    _ => ""
                };

                GUILayout.Label($"<b>Etapa {etapa + 1}/4:</b> <color=#ffdd44>{nombreEtapa}</color>", estiloSubtitulo);
                GUILayout.Space(2);

                if (BotonConIcono("❄️ [ESPACIO o Clic] ¡GIRAR Y BATIR GARRAFA DE COBRE!", null, estiloBotonSeleccionado, wAreaAccion, 46))
                {
                    MesaPreparacion.Instance.RasparHielo(0.35f);
                    tiempoUltimoRaspado = Time.time;
                    GenerarChispasEscarcha();
                }

                GUILayout.Space(4);

                Rect rBarraRaspado = GUILayoutUtility.GetRect(wAreaAccion, 14);
                if (texBarraFondo != null) GUI.DrawTexture(rBarraRaspado, texBarraFondo);
                if (texBarraVerde != null) GUI.DrawTexture(new Rect(rBarraRaspado.x, rBarraRaspado.y, rBarraRaspado.width * pctHielo, rBarraRaspado.height), texBarraVerde);

                GUILayout.EndVertical();

                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
            else if (!tieneSabor)
            {
                // PASO 3: Elegir Sabor con Iconos y Atajos (Ancho dinámico equilibrado)
                SaborNieve saborPedido = NeveriaGameManager.Instance.ClienteActual != null && NeveriaGameManager.Instance.ClienteActual.Pedido != null
                    ? NeveriaGameManager.Instance.ClienteActual.Pedido.saborDeseado
                    : SaborNieve.Ninguno;

                GUILayout.Label("👉 <b>PASO 3:</b> Añade la pulpa tradicional a la nieve copeteada:", estiloSubtitulo);
                GUILayout.Space(6);
                GUILayout.BeginHorizontal();

                float wBtnSabor = Mathf.Floor((anchoRestante - 28f) / 5f);

                BotonSaborDinamico("[Q] Garambullo", SaborNieve.Garambullo, saborPedido, wBtnSabor);
                GUILayout.Space(6);
                BotonSaborDinamico("[W] Borrachita", SaborNieve.Borrachita, saborPedido, wBtnSabor);
                GUILayout.Space(6);
                BotonSaborDinamico("[E] Q. Fresa", SaborNieve.QuesoConFresa, saborPedido, wBtnSabor);
                GUILayout.Space(6);
                BotonSaborDinamico("[R] Aguacate", SaborNieve.Aguacate, saborPedido, wBtnSabor);
                GUILayout.Space(6);
                BotonSaborDinamico("[T] Limón", SaborNieve.LimonVirreinal, saborPedido, wBtnSabor);

                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
            else
            {
                // PASO 4: Nieve Completa, lista para servir o resolver
                var cliente = NeveriaGameManager.Instance.ClienteActual;
                bool hayCliente = (cliente != null && cliente.Pedido != null);
                bool coincide = hayCliente && MesaPreparacion.Instance.CoincideConPedido(cliente.Pedido);

                if (!hayCliente)
                {
                    GUILayout.Label("⏳ <b>El comensal anterior se retiró por tiempo.</b> ¡Tu nieve sigue en el mostrador para el siguiente visitante!", estiloSubtitulo);
                    GUILayout.Space(6);
                    GUILayout.BeginHorizontal();

                    if (texGarrafaEtapas[3] != null)
                    {
                        Rect rIcoListo = GUILayoutUtility.GetRect(50, 50, GUILayout.Width(50), GUILayout.Height(50));
                        GUI.DrawTexture(rIcoListo, texGarrafaEtapas[3], ScaleMode.ScaleToFit);
                        GUILayout.Space(14);
                    }

                    float wBtnDesechar = Mathf.Min(400f, anchoRestante - 80f);
                    if (BotonConIcono("🗑️ DESECHAR Y PREPARAR DE NUEVO [BORRAR]", null, estiloBotonDesechar, wBtnDesechar, 50))
                    {
                        MesaPreparacion.Instance.DesecharPreparacion();
                    }
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                }
                else if (coincide)
                {
                    GUILayout.Label($"👉 <b>PASO 4:</b> ¡La nieve coincide con el antojo de {cliente.NombreCliente}! Sírvela con orgullo:", estiloSubtitulo);
                    GUILayout.Space(6);
                    GUILayout.BeginHorizontal();

                    Texture2D icoFinal = ObtenerIconoSabor(MesaPreparacion.Instance.SaborActual);
                    if (icoFinal != null)
                    {
                        Rect rIcoFinal = GUILayoutUtility.GetRect(50, 50, GUILayout.Width(50), GUILayout.Height(50));
                        GUI.DrawTexture(rIcoFinal, icoFinal, ScaleMode.ScaleToFit);
                        GUILayout.Space(14);
                    }

                    float wBtnServir = Mathf.Min(560f, anchoRestante - 80f);
                    if (BotonConIcono($"🍧 ¡ENTREGAR A {cliente.NombreCliente.ToUpper()}! [ENTER o Clic]", null, estiloBotonServir, wBtnServir, 52))
                    {
                        NeveriaGameManager.Instance.IntentarServirPedidoActual();
                    }
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                }
                else
                {
                    string recPed = PedidoNieve.ObtenerNombreRecipiente(cliente.Pedido.recipienteDeseado);
                    string sabPed = PedidoNieve.ObtenerNombreSabor(cliente.Pedido.saborDeseado);
                    string recPrep = PedidoNieve.ObtenerNombreRecipiente(MesaPreparacion.Instance.RecipienteActual);
                    string sabPrep = PedidoNieve.ObtenerNombreSabor(MesaPreparacion.Instance.SaborActual);

                    GUILayout.Label($"⚠️ <b>¡ATENCIÓN!</b> {cliente.NombreCliente} pidió <color=#55ff55><b>{recPed} de {sabPed}</b></color>, pero preparaste <color=#ff6666><b>{recPrep} de {sabPrep}</b></color>.", estiloSubtitulo);
                    GUILayout.Space(6);
                    GUILayout.BeginHorizontal();

                    float wBtnCorregir = Mathf.Min(420f, (anchoRestante - 20f) * 0.65f);
                    float wBtnFallo = Mathf.Min(180f, (anchoRestante - 20f) * 0.32f);

                    if (BotonConIcono($"🔄 DESECHAR Y PREPARAR {sabPed.ToUpper()}", null, estiloBotonDesechar, wBtnCorregir, 48))
                    {
                        MesaPreparacion.Instance.DesecharPreparacion();
                    }

                    GUILayout.Space(12);

                    if (BotonConIcono("❌ Entregar fallo", null, estiloBotonAccion, wBtnFallo, 48))
                    {
                        NeveriaGameManager.Instance.EntregarPedidoConFalloForzado();
                    }

                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                }
            }

            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            GUILayout.EndArea();
        }

        private void BotonSaborDinamico(string texto, SaborNieve sabor, SaborNieve saborPedido, float anchoBoton)
        {
            bool esElPedido = (sabor == saborPedido);
            GUIStyle st = esElPedido ? estiloBotonSeleccionado : estiloBotonAccion;
            Texture2D ico = ObtenerIconoSabor(sabor);

            if (BotonConIcono(texto, ico, st, anchoBoton, 52))
            {
                MesaPreparacion.Instance.AgregarSabor(sabor);
            }
        }

        private bool BotonConIcono(string texto, Texture2D icono, GUIStyle estilo, float ancho, float alto)
        {
            Rect r = GUILayoutUtility.GetRect(ancho, alto, GUILayout.Width(ancho), GUILayout.Height(alto));
            bool clicked = GUI.Button(r, "", estilo);
            if (clicked && AudioColonialManager.Instance != null)
            {
                AudioColonialManager.Instance.ReproducirClickBoton();
            }

            if (icono != null)
            {
                float tamIcono = Mathf.Min(alto - 16f, 30f);
                Rect rIcono = new Rect(r.x + 6f, r.y + (alto - tamIcono) * 0.5f, tamIcono, tamIcono);
                GUI.DrawTexture(rIcono, icono, ScaleMode.ScaleToFit);

                float xTexto = r.x + tamIcono + 6f;
                float wTexto = r.width - (tamIcono + 10f);
                Rect rTexto = new Rect(xTexto, r.y, wTexto, r.height);

                // Auto-escala de fuente para que jamás se corte el texto
                int fontSizeFinal = estilo.fontSize;
                float anchoEst = texto.Length * (fontSizeFinal * 0.56f);
                if (anchoEst > wTexto && wTexto > 20f)
                {
                    fontSizeFinal = Mathf.Max(10, Mathf.FloorToInt(fontSizeFinal * (wTexto / anchoEst)));
                }

                GUIStyle estiloTxt = new GUIStyle(GUI.skin.label);
                estiloTxt.fontSize = fontSizeFinal;
                estiloTxt.fontStyle = FontStyle.Bold;
                estiloTxt.normal.textColor = estilo.normal.textColor;
                estiloTxt.alignment = TextAnchor.MiddleLeft;
                estiloTxt.wordWrap = false;
                estiloTxt.clipping = TextClipping.Overflow;

                // Sombra de texto para contraste perfecto
                GUIStyle estiloSombra = new GUIStyle(estiloTxt);
                estiloSombra.normal.textColor = new Color(0f, 0f, 0f, 0.95f);
                GUI.Label(new Rect(rTexto.x + 1f, rTexto.y + 1f, rTexto.width, rTexto.height), texto, estiloSombra);

                // Texto frontal nítido
                GUI.Label(rTexto, texto, estiloTxt);
            }
            else
            {
                int fontSizeFinal = estilo.fontSize;
                float anchoEst = texto.Length * (fontSizeFinal * 0.56f);
                if (anchoEst > r.width - 12f && r.width > 20f)
                {
                    fontSizeFinal = Mathf.Max(10, Mathf.FloorToInt(fontSizeFinal * ((r.width - 12f) / anchoEst)));
                }

                GUIStyle estiloTxt = new GUIStyle(GUI.skin.label);
                estiloTxt.fontSize = fontSizeFinal;
                estiloTxt.fontStyle = FontStyle.Bold;
                estiloTxt.normal.textColor = estilo.normal.textColor;
                estiloTxt.alignment = TextAnchor.MiddleCenter;
                estiloTxt.wordWrap = false;
                estiloTxt.clipping = TextClipping.Overflow;

                GUIStyle estiloSombra = new GUIStyle(estiloTxt);
                estiloSombra.normal.textColor = new Color(0f, 0f, 0f, 0.95f);
                GUI.Label(new Rect(r.x + 1f, r.y + 1f, r.width, r.height), texto, estiloSombra);

                GUI.Label(r, texto, estiloTxt);
            }

            return clicked;
        }

        private void DibujarModalVictoria()
        {
            // Fondo oscuro cinematográfico sobre toda la pantalla
            if (texOverlayNegro != null)
            {
                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), texOverlayNegro);
            }

            float anchoModal = Mathf.Min(880f, Screen.width - 28f);
            float altoModal = Mathf.Min(560f, Screen.height - 28f);
            float xModal = (Screen.width - anchoModal) * 0.5f;
            float yModal = (Screen.height - altoModal) * 0.5f;
            Rect rModal = new Rect(xModal, yModal, anchoModal, altoModal);

            // Marco colonial completo con cabecera y remates dorados
            ColonialUIHelper.DibujarMarcoVentana(
                rModal,
                "🏆 ¡FESTIVAL DE LA NIEVE CONQUISTADO!",
                "Misión Cumplida en Dolores Hidalgo • 1810"
            );

            // Área interior útil
            float yContenido = rModal.y + 70f;
            float hContenido = rModal.height - 84f;
            Rect rInterior = new Rect(rModal.x + 14f, yContenido, rModal.width - 28f, hContenido);
            ColonialUIHelper.DibujarTarjetaInterior(rInterior);

            // Columna Izquierda: Retrato de Don Chon con su premio Cuchara Dorada
            float wColIzq = Mathf.Clamp(rInterior.width * 0.28f, 190f, 230f);
            Rect rColIzq = new Rect(rInterior.x + 10f, rInterior.y + 10f, wColIzq, rInterior.height - 20f);

            // Renderizar a Don Chon sonriente de cuerpo entero
            if (texDonChonGuia != null)
            {
                Rect rImg = new Rect(rColIzq.x, rColIzq.y + 4f, rColIzq.width, rColIzq.height - 66f);
                GUI.DrawTexture(rImg, texDonChonGuia, ScaleMode.ScaleToFit);
            }

            // Badge colonial "Cuchara Dorada" bajo Don Chon
            Rect rBadgeCuchara = new Rect(rColIzq.x, rColIzq.yMax - 58f, rColIzq.width, 52f);
            ColonialUIHelper.DibujarTarjetaInterior(rBadgeCuchara);
            GUIStyle stBadgeTit = new GUIStyle(GUI.skin.label)
            {
                fontSize = 12,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = new Color(1f, 0.88f, 0.35f) }
            };
            GUIStyle stBadgeSub = new GUIStyle(GUI.skin.label)
            {
                fontSize = 11,
                fontStyle = FontStyle.Italic,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = new Color(0.9f, 0.9f, 0.9f) }
            };
            GUI.Label(new Rect(rBadgeCuchara.x, rBadgeCuchara.y + 5f, rBadgeCuchara.width, 20f), "🥄 CUCHARA DORADA", stBadgeTit);
            GUI.Label(new Rect(rBadgeCuchara.x, rBadgeCuchara.y + 25f, rBadgeCuchara.width, 20f), "Reliquia de Maestro (1810)", stBadgeSub);

            // Columna Derecha: Estadísticas, Estrellas y Cita de Don Chon
            float xColDer = rColIzq.xMax + 14f;
            float wColDer = rInterior.xMax - xColDer - 10f;
            Rect rColDer = new Rect(xColDer, rInterior.y + 10f, wColDer, rInterior.height - 20f);

            GUILayout.BeginArea(rColDer);

            int pts = NeveriaGameManager.Instance.PuntajeTotal;
            int at = NeveriaGameManager.Instance.ClientesAtendidos;
            int obj = NeveriaGameManager.Instance.ClientesObjetivo;

            // Rango de estrellas
            string estrellas = (pts >= 900) ? "⭐⭐⭐ ¡MAESTRO NEVERO VIRREINAL!" :
                               (pts >= 600) ? "⭐⭐ ¡OFICIAL DE GARRAFA!" : "⭐ ¡APRENDIZ DE DOLORES!";

            GUIStyle stRango = new GUIStyle(estiloTitulo)
            {
                fontSize = 19,
                normal = { textColor = new Color(1f, 0.90f, 0.30f) }
            };
            GUILayout.Label(estrellas, stRango);
            GUILayout.Space(2);

            GUIStyle stCita = new GUIStyle(GUI.skin.label)
            {
                fontSize = 13,
                fontStyle = FontStyle.Italic,
                wordWrap = true,
                normal = { textColor = new Color(0.92f, 0.92f, 0.92f) }
            };
            GUILayout.Label("«¡Guanajicho, has honrado a Dolores Hidalgo! El pueblo se deleita con tus nieves y se suma con fervor a la causa de la Libertad.»", stCita);
            GUILayout.Space(10);

            // 3 Tarjetas de logros / stats
            DibujarTarjetaStatVictoria("👥 Comensales Atendidos", $"{at} de {obj} visitantes satisfechos (100%)", texIconoBarquillo);
            GUILayout.Space(5);
            DibujarTarjetaStatVictoria("⭐ Reputación Artesanal", $"{pts} puntos de prestigio virreinal", texIconoGarambullo);
            GUILayout.Space(5);
            DibujarTarjetaStatVictoria("🏛️ Recompensa Cívica", "+120 Recursos Cívicos para la Insurgencia", texBotonDorado);
            GUILayout.Space(14);

            // Botones de acción con ColonialUIHelper.BotonColonial
            GUILayout.BeginHorizontal();

            float wBtnMapa = (wColDer - 24f) * 0.48f;
            float wBtnRepetir = (wColDer - 24f) * 0.28f;
            float wBtnMenu = (wColDer - 24f) * 0.24f;

            Rect rBtnMapa = GUILayoutUtility.GetRect(wBtnMapa, 48f);
            if (ColonialUIHelper.BotonColonial(rBtnMapa, "🗺️ VOLVER AL MAPA", ColonialUIHelper.VarianteBoton.Verde, 15))
            {
                IrAlOverworld();
            }

            GUILayout.Space(8);

            Rect rBtnRepetir = GUILayoutUtility.GetRect(wBtnRepetir, 48f);
            if (ColonialUIHelper.BotonColonial(rBtnRepetir, "🔄 REPETIR", ColonialUIHelper.VarianteBoton.Madera, 14))
            {
                ReiniciarNivel();
            }

            GUILayout.Space(8);

            Rect rBtnMenu = GUILayoutUtility.GetRect(wBtnMenu, 48f);
            if (ColonialUIHelper.BotonColonial(rBtnMenu, "🏠 MENÚ", ColonialUIHelper.VarianteBoton.Talavera, 14))
            {
                IrAlMenu();
            }

            GUILayout.EndHorizontal();

            GUILayout.EndArea();

            // Atajos de teclado en victoria: [Enter] o [Espacio] para volver rápido al mapa
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Space))
            {
                IrAlOverworld();
            }
        }

        private void DibujarTarjetaStatVictoria(string titulo, string valor, Texture2D icono)
        {
            Rect r = GUILayoutUtility.GetRect(GUILayoutUtility.GetRect(0, 0).width, 50f, GUILayout.ExpandWidth(true), GUILayout.Height(50f));
            ColonialUIHelper.DibujarTarjetaInterior(r);

            if (icono != null)
            {
                Rect rIco = new Rect(r.x + 8f, r.y + 8f, 34f, 34f);
                GUI.DrawTexture(rIco, icono, ScaleMode.ScaleToFit);
            }

            float xTxt = (icono != null) ? r.x + 50f : r.x + 12f;
            float wTxt = r.width - (xTxt - r.x) - 10f;

            GUIStyle stTit = new GUIStyle(GUI.skin.label)
            {
                fontSize = 12,
                fontStyle = FontStyle.Bold,
                wordWrap = false,
                normal = { textColor = new Color(1.0f, 0.85f, 0.35f) }
            };
            GUI.Label(new Rect(xTxt, r.y + 5f, wTxt, 18f), titulo, stTit);

            GUIStyle stVal = new GUIStyle(GUI.skin.label)
            {
                fontSize = 14,
                fontStyle = FontStyle.Bold,
                wordWrap = false,
                normal = { textColor = Color.white }
            };
            GUI.Label(new Rect(xTxt, r.y + 24f, wTxt, 20f), valor, stVal);
        }

        private void DibujarModalDerrota()
        {
            if (texOverlayNegro != null)
            {
                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), texOverlayNegro);
            }

            float anchoModal = Mathf.Min(820f, Screen.width - 28f);
            float altoModal = Mathf.Min(520f, Screen.height - 28f);
            float xModal = (Screen.width - anchoModal) * 0.5f;
            float yModal = (Screen.height - altoModal) * 0.5f;
            Rect rModal = new Rect(xModal, yModal, anchoModal, altoModal);

            ColonialUIHelper.DibujarMarcoVentana(
                rModal,
                "💀 JORNADA FALLIDA EN EL QUIOSCO",
                "Demasiados comensales se retiraron inconformes"
            );

            float yContenido = rModal.y + 70f;
            float hContenido = rModal.height - 84f;
            Rect rInterior = new Rect(rModal.x + 14f, yContenido, rModal.width - 28f, hContenido);
            ColonialUIHelper.DibujarTarjetaInterior(rInterior);

            // Columna Izquierda: Don Chon comprensivo
            float wColIzq = Mathf.Clamp(rInterior.width * 0.28f, 190f, 230f);
            Rect rColIzq = new Rect(rInterior.x + 10f, rInterior.y + 10f, wColIzq, rInterior.height - 20f);

            if (texDonChonGuia != null)
            {
                Rect rImg = new Rect(rColIzq.x, rColIzq.y + 4f, rColIzq.width, rColIzq.height - 40f);
                GUI.DrawTexture(rImg, texDonChonGuia, ScaleMode.ScaleToFit);
            }

            GUI.Label(new Rect(rColIzq.x, rColIzq.yMax - 32f, rColIzq.width, 24f), "Don Chon te aconseja", estiloSubtitulo);

            // Columna Derecha: Explicación y consejos
            float xColDer = rColIzq.xMax + 14f;
            float wColDer = rInterior.xMax - xColDer - 10f;
            Rect rColDer = new Rect(xColDer, rInterior.y + 10f, wColDer, rInterior.height - 20f);

            GUILayout.BeginArea(rColDer);

            GUIStyle stAviso = new GUIStyle(estiloTitulo)
            {
                fontSize = 18,
                normal = { textColor = new Color(1f, 0.45f, 0.40f) }
            };
            GUILayout.Label("«No decaigas Guanajicho. El oficio exige temple.»", stAviso);
            GUILayout.Space(6);

            GUIStyle stConsejo = new GUIStyle(GUI.skin.label)
            {
                fontSize = 14,
                wordWrap = true,
                normal = { textColor = Color.white }
            };
            GUILayout.Label(
                "En las nieves de Dolores Hidalgo la rapidez y el ritmo son clave:\n\n" +
                "• <b>Ten listo el barquillo o vaso</b> en cuanto veas al comensal caminar.\n" +
                "• <b>Bate la garrafa con [ESPACIO]</b> sin parar para que la nieve tome escarcha rápido.\n" +
                "• Si te equivocas de sabor, pulsa <b>[Borrar]</b> de inmediato para reiniciar.",
                stConsejo
            );
            GUILayout.Space(14);

            int atendidos = NeveriaGameManager.Instance.ClientesAtendidos;
            int perdidos = NeveriaGameManager.Instance.ClientesPerdidos;
            GUILayout.Label($"<b>Balance de la jornada:</b> {atendidos} servidos • <color=#ff6666>{perdidos} decepcionados</color>", estiloTextoBlanco);
            GUILayout.Space(14);

            GUILayout.BeginHorizontal();

            float wBtnReintentar = (wColDer - 16f) * 0.55f;
            float wBtnSalir = (wColDer - 16f) * 0.45f;

            Rect rBtnReintentar = GUILayoutUtility.GetRect(wBtnReintentar, 48f);
            if (ColonialUIHelper.BotonColonial(rBtnReintentar, "🔄 REINTENTAR JORNADA", ColonialUIHelper.VarianteBoton.Talavera, 15))
            {
                ReiniciarNivel();
            }

            GUILayout.Space(12);

            Rect rBtnSalir = GUILayoutUtility.GetRect(wBtnSalir, 48f);
            if (ColonialUIHelper.BotonColonial(rBtnSalir, "🗺️ VOLVER AL MAPA", ColonialUIHelper.VarianteBoton.Madera, 14))
            {
                IrAlOverworld();
            }

            GUILayout.EndHorizontal();

            GUILayout.EndArea();

            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.R))
            {
                ReiniciarNivel();
            }
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
            ColonialUIHelper.DibujarMenuPausaColonial(
                "Nivel 2: Desafío de las Nieves de Dolores Hidalgo",
                AlternarPausa,
                () => mostrarOpcionesPausa = true,
                ReiniciarNivel,
                IrAlOverworld,
                IrAlMenu
            );
        }

        private void DibujarModalOpcionesPausa()
        {
            ColonialUIHelper.DibujarModalOpcionesColonial(
                ref volumenGeneral,
                ref volumenMusica,
                ref volumenSFX,
                ref pantallaCompleta,
                () => mostrarOpcionesPausa = false
            );
        }

        public void ReiniciarNivel()
        {
            estaPausado = false;
            mostrarOpcionesPausa = false;
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void IrAlOverworld()
        {
            estaPausado = false;
            mostrarOpcionesPausa = false;
            Time.timeScale = 1f;
            SceneManager.LoadScene("Overworld_MapaGuanajuato");
        }

        public void IrAlMenu()
        {
            estaPausado = false;
            mostrarOpcionesPausa = false;
            Time.timeScale = 1f;
            SceneManager.LoadScene("MainMenu");
        }
    }
}
