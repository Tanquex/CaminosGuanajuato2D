using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using CaminosDeGuanajuato.Core;
using CaminosDeGuanajuato.UI;

namespace CaminosDeGuanajuato.Duelo
{
    /// <summary>
    /// Interfaz gráfica (HUD) del Nivel 3: Duelo en la Plaza (Estilo Guitar Hero / Festival de Sones Insurgentes).
    /// Dibuja el diapasón musical de 4 cuerdas, las gemas de notas que descienden al ritmo del son,
    /// los aros receptores interactivos, explosiones de chispas al acertar, el medidor de Fervor de la Plaza
    /// y los diálogos interactivos paso a paso con globos de voz estilo novela visual.
    /// </summary>
    public class HUDDueloUI : MonoBehaviour
    {
        private GUIStyle estiloTitulo;
        private GUIStyle estiloSubtitulo;
        private GUIStyle estiloTextoBlanco;
        private GUIStyle estiloLetraNota;
        private GUIStyle estiloComboGigante;
        private GUIStyle estiloJuicio;

        // Texturas del Diapasón Musical (Guitar Hero)
        private Texture2D texDiapasonMadera;
        private Texture2D texCuerdaDorada;
        private Texture2D texTrasteHorizontal;
        private Texture2D texOverlayNegro;
        private Texture2D texFondoPlacaTecla;
        private Texture2D[] texGemaNota = new Texture2D[4];
        private Texture2D[] texAroReceptor = new Texture2D[4];
        private Texture2D[] texAroReceptorActivo = new Texture2D[4];
        private Texture2D[] texFlechaColor = new Texture2D[4];
        private Texture2D[] texFlechaBlanca = new Texture2D[4];

        // Retratos para los diálogos interactivos estilo Novela Visual
        private Texture2D texRetratoDonChon;
        private Texture2D texRetratoGuanajicho;

        // Colores emblemáticos de los 4 carriles (Rojo Insurgente, Oro Virreinal, Verde Libertad, Azul Talavera)
        private readonly Color[] coloresCarril = new Color[]
        {
            new Color(0.98f, 0.24f, 0.24f, 1f), // Carril 0: [A / Izquierda] Rojo
            new Color(1.00f, 0.84f, 0.16f, 1f), // Carril 1: [S / Abajo] Amarillo
            new Color(0.18f, 0.90f, 0.38f, 1f), // Carril 2: [D / Arriba] Verde
            new Color(0.22f, 0.68f, 1.00f, 1f)  // Carril 3: [F / Derecha] Azul
        };

        private readonly string[] letrasCarril = { "A", "S", "D", "F" };
        private readonly string[] nombresColorCarril = { "Rojo", "Amarillo", "Verde", "Azul" };

        // Control de Diálogos Interactivos (Novela Visual)
        private int indiceDialogoIntro = 0;
        private int indiceDialogoRival = 0;
        private int ultimaRondaDialogoConfigurada = -1;
        private ColonialUIHelper.LineaDialogoVN[] lineasIntro;
        private ColonialUIHelper.LineaDialogoVN[] lineasRetoRival;

        // Partículas visuales al acertar notas en los aros receptores
        private class ChispaMusical
        {
            public Vector2 posPantalla;
            public Vector2 velocidad;
            public Color color;
            public float vidaRestante;
            public float vidaMax;
            public string simbolo;
        }
        private List<ChispaMusical> chispasActivas = new List<ChispaMusical>();
        private Vector2[] centrosArosPantalla = new Vector2[4];

        // Pausa y Opciones
        private bool estaPausado = false;
        private bool mostrarOpcionesPausa = false;
        private float volumenGeneral = 0.85f;
        private float volumenMusica = 0.65f;
        private float volumenSFX = 0.85f;
        private bool pantallaCompleta = true;

        private void Awake()
        {
            CrearTexturasMusicales();
        }

        private void Start()
        {
            CrearTexturasMusicales();
            ConfigurarDialogoInicial();

            if (DueloArenaManager.Instance != null)
            {
                DueloArenaManager.Instance.OnNotaEvaluada -= ManejarEfectoNotaEvaluada;
                DueloArenaManager.Instance.OnNotaEvaluada += ManejarEfectoNotaEvaluada;
            }

            if (AudioColonialManager.Instance != null)
            {
                volumenGeneral = AudioColonialManager.Instance.VolumenGeneral;
                volumenMusica = AudioColonialManager.Instance.VolumenMusica;
                volumenSFX = AudioColonialManager.Instance.VolumenSFX;
            }
        }

        private void OnDestroy()
        {
            if (DueloArenaManager.Instance != null)
            {
                DueloArenaManager.Instance.OnNotaEvaluada -= ManejarEfectoNotaEvaluada;
            }
        }

        private void CrearTexturasMusicales()
        {
            if (texOverlayNegro == null)
            {
                texOverlayNegro = new Texture2D(1, 1);
                texOverlayNegro.SetPixel(0, 0, new Color(0f, 0f, 0f, 0.78f));
                texOverlayNegro.Apply();
            }

            if (texDiapasonMadera == null)
            {
                texDiapasonMadera = new Texture2D(64, 16, TextureFormat.RGBA32, false);
                for (int y = 0; y < 16; y++)
                {
                    for (int x = 0; x < 64; x++)
                    {
                        bool bordeOro = (x <= 1 || x >= 62);
                        bool biselInterior = (x == 2 || x == 61);
                        Color c = bordeOro
                            ? new Color(0.92f, 0.72f, 0.22f, 0.98f)
                            : (biselInterior ? new Color(0.45f, 0.28f, 0.12f, 0.95f) : new Color(0.09f, 0.05f, 0.04f, 0.88f));
                        texDiapasonMadera.SetPixel(x, y, c);
                    }
                }
                texDiapasonMadera.Apply();
            }

            if (texCuerdaDorada == null)
            {
                texCuerdaDorada = new Texture2D(1, 1);
                texCuerdaDorada.SetPixel(0, 0, new Color(0.85f, 0.72f, 0.42f, 0.42f));
                texCuerdaDorada.Apply();
            }

            if (texTrasteHorizontal == null)
            {
                texTrasteHorizontal = new Texture2D(1, 1);
                texTrasteHorizontal.SetPixel(0, 0, new Color(0.75f, 0.60f, 0.35f, 0.24f));
                texTrasteHorizontal.Apply();
            }

            if (texFondoPlacaTecla == null)
            {
                texFondoPlacaTecla = new Texture2D(32, 16, TextureFormat.RGBA32, false);
                for (int y = 0; y < 16; y++)
                {
                    for (int x = 0; x < 32; x++)
                    {
                        bool borde = (x == 0 || x == 31 || y == 0 || y == 15);
                        texFondoPlacaTecla.SetPixel(x, y, borde ? new Color(0.95f, 0.78f, 0.28f, 0.98f) : new Color(0.08f, 0.05f, 0.03f, 0.94f));
                    }
                }
                texFondoPlacaTecla.Apply();
            }

            if (texGemaNota[0] == null)
            {
                string[] recursosFlechaCol = { "UI/FlechaRitmoIzq", "UI/FlechaRitmoAbajo", "UI/FlechaRitmoArriba", "UI/FlechaRitmoDer" };
                string[] recursosFlechaBla = { "UI/FlechaBlancaIzq", "UI/FlechaBlancaAbajo", "UI/FlechaBlancaArriba", "UI/FlechaBlancaDer" };

                for (int i = 0; i < 4; i++)
                {
                    texGemaNota[i] = GenerarTexturaGemaNota(coloresCarril[i]);
                    texAroReceptor[i] = GenerarTexturaAroReceptor(coloresCarril[i], activo: false);
                    texAroReceptorActivo[i] = GenerarTexturaAroReceptor(coloresCarril[i], activo: true);

                    texFlechaColor[i] = Resources.Load<Texture2D>(recursosFlechaCol[i]);
                    if (texFlechaColor[i] == null)
                    {
                        texFlechaColor[i] = GenerarTexturaFlechaDireccional(i, coloresCarril[i]);
                    }

                    texFlechaBlanca[i] = Resources.Load<Texture2D>(recursosFlechaBla[i]);
                    if (texFlechaBlanca[i] == null)
                    {
                        texFlechaBlanca[i] = GenerarTexturaFlechaDireccional(i, new Color(1f, 0.99f, 0.94f, 1f));
                    }
                }
            }

            if (texRetratoDonChon == null)
            {
                texRetratoDonChon = Resources.Load<Texture2D>("PersonajesColonial/DonChon");
            }

            if (texRetratoGuanajicho == null)
            {
                texRetratoGuanajicho = Resources.Load<Texture2D>("PersonajesColonial/GuanajichoTamborilero");
            }
        }

        /// <summary>
        /// Genera una flecha pixel-art de alto contraste (64x64) con borde oscuro grueso y centro luminoso
        /// para cada dirección: 0=Izquierda, 1=Abajo, 2=Arriba, 3=Derecha.
        /// </summary>
        private Texture2D GenerarTexturaFlechaDireccional(int direccion, Color colorRelleno)
        {
            int tam = 64;
            Texture2D tex = new Texture2D(tam, tam, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Point;
            tex.wrapMode = TextureWrapMode.Clamp;

            Color colBorde = new Color(0.06f, 0.04f, 0.02f, 1f);
            Color colBrillo = Color.Lerp(colorRelleno, Color.white, 0.55f);

            for (int y = 0; y < tam; y++)
            {
                for (int x = 0; x < tam; x++)
                {
                    // Coordenadas normalizadas [-1, 1] orientadas primero hacia ARRIBA (+u)
                    float nx = (x - 31.5f) / 26f;
                    float ny = (y - 31.5f) / 26f;

                    float u, v;
                    if (direccion == 0)      { u = -nx; v = ny; }  // Izquierda
                    else if (direccion == 1) { u = -ny; v = nx; }  // Abajo
                    else if (direccion == 2) { u = ny;  v = nx; }  // Arriba
                    else                     { u = nx;  v = ny; }  // Derecha

                    float av = Mathf.Abs(v);

                    // Geometría de flecha rítmica: punta triangular (u en [-0.05, 0.92]) + tallo (u en [-0.88, 0.05])
                    bool enPuntaExt = (u >= -0.08f && u <= 0.95f && av <= (0.95f - u) * 0.92f);
                    bool enTalloExt = (u >= -0.90f && u <= 0.05f && av <= 0.38f);
                    bool dentroExterior = enPuntaExt || enTalloExt;

                    bool enPuntaInt = (u >= 0.00f && u <= 0.80f && av <= (0.82f - u) * 0.85f);
                    bool enTalloInt = (u >= -0.78f && u <= 0.08f && av <= 0.27f);
                    bool dentroInterior = enPuntaInt || enTalloInt;

                    bool enPuntaNucleo = (u >= 0.08f && u <= 0.64f && av <= (0.66f - u) * 0.78f);
                    bool enTalloNucleo = (u >= -0.64f && u <= 0.10f && av <= 0.16f);
                    bool dentroNucleo = enPuntaNucleo || enTalloNucleo;

                    if (!dentroExterior)
                        tex.SetPixel(x, y, Color.clear);
                    else if (!dentroInterior)
                        tex.SetPixel(x, y, colBorde);
                    else if (dentroNucleo)
                        tex.SetPixel(x, y, colBrillo);
                    else
                        tex.SetPixel(x, y, colorRelleno);
                }
            }
            tex.Apply();
            return tex;
        }

        private Texture2D GenerarTexturaGemaNota(Color colorBase)
        {
            int tam = 64;
            Texture2D tex = new Texture2D(tam, tam, TextureFormat.RGBA32, false);
            tex.wrapMode = TextureWrapMode.Clamp;
            float centro = (tam - 1) * 0.5f;
            float radioMax = tam * 0.46f;

            for (int y = 0; y < tam; y++)
            {
                for (int x = 0; x < tam; x++)
                {
                    float dist = Vector2.Distance(new Vector2(x, y), new Vector2(centro, centro));
                    if (dist > radioMax)
                    {
                        tex.SetPixel(x, y, Color.clear);
                    }
                    else if (dist > radioMax - 2.5f)
                    {
                        // Borde exterior oscuro para alto contraste
                        tex.SetPixel(x, y, new Color(0.06f, 0.04f, 0.02f, 0.98f));
                    }
                    else if (dist > radioMax - 6.0f)
                    {
                        // Anillo dorado colonial
                        tex.SetPixel(x, y, new Color(1f, 0.92f, 0.55f, 1f));
                    }
                    else if (dist < radioMax * 0.42f)
                    {
                        // Núcleo brillante para que la letra se lea perfecto
                        Color nucleo = Color.Lerp(colorBase, Color.white, 0.35f);
                        tex.SetPixel(x, y, nucleo);
                    }
                    else
                    {
                        tex.SetPixel(x, y, colorBase);
                    }
                }
            }
            tex.Apply();
            return tex;
        }

        private Texture2D GenerarTexturaAroReceptor(Color colorCarril, bool activo)
        {
            int tam = 64;
            Texture2D tex = new Texture2D(tam, tam, TextureFormat.RGBA32, false);
            tex.wrapMode = TextureWrapMode.Clamp;
            float centro = (tam - 1) * 0.5f;
            float radioExt = tam * 0.47f;
            float radioInt = tam * 0.32f;

            for (int y = 0; y < tam; y++)
            {
                for (int x = 0; x < tam; x++)
                {
                    float dist = Vector2.Distance(new Vector2(x, y), new Vector2(centro, centro));
                    if (dist > radioExt)
                    {
                        tex.SetPixel(x, y, Color.clear);
                    }
                    else if (dist >= radioInt)
                    {
                        Color colAnillo = activo ? Color.Lerp(colorCarril, Color.white, 0.55f) : colorCarril;
                        if (dist > radioExt - 2f || dist < radioInt + 2f)
                            colAnillo = activo ? new Color(1f, 0.96f, 0.65f, 1f) : new Color(0.12f, 0.08f, 0.05f, 0.95f);
                        tex.SetPixel(x, y, colAnillo);
                    }
                    else
                    {
                        Color fondoInt = activo
                            ? new Color(colorCarril.r, colorCarril.g, colorCarril.b, 0.65f)
                            : new Color(0.05f, 0.04f, 0.04f, 0.72f);
                        tex.SetPixel(x, y, fondoInt);
                    }
                }
            }
            tex.Apply();
            return tex;
        }

        private void ConfigurarDialogoInicial()
        {
            indiceDialogoIntro = 0;
            ColonialUIHelper.ReiniciarSecuenciaDialogoVN();

            lineasIntro = new ColonialUIHelper.LineaDialogoVN[]
            {
                new ColonialUIHelper.LineaDialogoVN(
                    "Don Chon",
                    "¡Qué noche tan festiva en la plaza, <b>Guanajicho</b>! Para conseguir el <b>Salvoconducto a San Felipe</b>, tendrás que triunfar en el <b>Duelo de Sones Insurgentes</b>.",
                    true,
                    "🎵 MISIÓN 3: Supera los 3 Sones Musicales en la tarima de Dolores",
                    0.92f
                ),
                new ColonialUIHelper.LineaDialogoVN(
                    "Guanajicho",
                    "¡Ya tengo mi tambor bien templado, Don Chon! ¿Cómo sigo el ritmo de los músicos en la tarima?",
                    false,
                    "🥁 Guanajicho tocará contra Don Silvestre, el Miliciano y el Capitán",
                    1.08f
                ),
                new ColonialUIHelper.LineaDialogoVN(
                    "Don Chon",
                    "¡Mira el diapasón al centro! Bajarán notas con <b>flechas y letras</b> por 4 cuerdas. Cuando entren al aro receptor, pulsa <b>[A] Rojo</b>, <b>[S] Amarillo</b>, <b>[D] Verde</b> o <b>[F] Azul</b> (o las <b>Flechas del teclado</b>).",
                    true,
                    "🎹 TECLAS: [A / Izq] Rojo • [S / Abajo] Amarillo • [D / Arriba] Verde • [F / Der] Azul",
                    0.92f
                ),
                new ColonialUIHelper.LineaDialogoVN(
                    "Don Chon",
                    "También puedes hacer <b>clic directo sobre cada aro</b>. ¡Encadena aciertos para subir tu multiplicador a <b>x4</b> y llenar de fervor la plaza!",
                    true,
                    "🔥 ¡Mantén el Fervor de la Plaza arriba hasta terminar cada canción!",
                    0.95f
                )
            };
        }

        private void PrepararDialogoRival(int ronda, RivalDueloIA rival)
        {
            if (ultimaRondaDialogoConfigurada == ronda && lineasRetoRival != null) return;
            ultimaRondaDialogoConfigurada = ronda;
            indiceDialogoRival = 0;
            ColonialUIHelper.ReiniciarSecuenciaDialogoVN();

            if (ronda == 1)
            {
                lineasRetoRival = new ColonialUIHelper.LineaDialogoVN[]
                {
                    new ColonialUIHelper.LineaDialogoVN(
                        "Don Silvestre",
                        "¡Ajúa, muchacho! Dicen que traes buen oído. Veamos si puedes seguir el paso de mi <b>Son del Arriero Serranero</b>.",
                        false,
                        "🎵 SON 1 DE 3: Ritmo suave (16 notas) • Usa [A, S, D, F] o las Flechas del teclado",
                        0.85f
                    ),
                    new ColonialUIHelper.LineaDialogoVN(
                        "Guanajicho",
                        "¡Con todo gusto, Don Silvestre! ¡Que retumbe ese primer son en la plaza!",
                        true,
                        "⭐ Pulsa [ESPACIO] o haz Clic para arrancar la canción",
                        1.08f
                    )
                };
            }
            else if (ronda == 2)
            {
                lineasRetoRival = new ColonialUIHelper.LineaDialogoVN[]
                {
                    new ColonialUIHelper.LineaDialogoVN(
                        "Joven Miliciano",
                        "¡Nada mal con ese tambor, compañero! Pero ahora el ritmo sube con el <b>Jarabe de los Insurgentes</b>. ¡No pierdas el compás!",
                        false,
                        "🎵 SON 2 DE 3: Ritmo alegre (22 notas) • ¡Cuidado con las escaleras rítmicas!",
                        1.02f
                    ),
                    new ColonialUIHelper.LineaDialogoVN(
                        "Guanajicho",
                        "¡Adelante! ¡Por la libertad de Dolores Hidalgo nadie se queda atrás!",
                        true,
                        "⭐ Pulsa [ESPACIO] o haz Clic para tocar el 2º Son",
                        1.08f
                    )
                };
            }
            else
            {
                lineasRetoRival = new ColonialUIHelper.LineaDialogoVN[]
                {
                    new ColonialUIHelper.LineaDialogoVN(
                        "Capitán de Dragones",
                        "¡Alto ahí, tamborilero! Si logras superar la velocidad de mi <b>Marcha de los Dragones</b> ante toda la plaza, te firmaré el Salvoconducto.",
                        false,
                        "🎵 SON FINAL (3 DE 3): Ritmo veloz (28 notas) por el Salvoconducto a San Felipe",
                        0.82f
                    ),
                    new ColonialUIHelper.LineaDialogoVN(
                        "Guanajicho",
                        "¡Trato hecho, Capitán! ¡Escuche cómo suena el corazón insurgente de Guanajuato!",
                        true,
                        "⭐ Pulsa [ESPACIO] o haz Clic para el Duelo Estelar",
                        1.08f
                    )
                };
            }
        }

        private void ManejarEfectoNotaEvaluada(int carril, CalificacionNota calificacion)
        {
            if (calificacion == CalificacionNota.Perfecto || calificacion == CalificacionNota.Bien)
            {
                Vector2 posAro = centrosArosPantalla[Mathf.Clamp(carril, 0, 3)];
                if (posAro == Vector2.zero) return;

                int cantidad = (calificacion == CalificacionNota.Perfecto) ? 7 : 4;
                string[] simbolos = { "✨", "🎵", "⭐", "🔥" };

                for (int i = 0; i < cantidad; i++)
                {
                    float angulo = UnityEngine.Random.Range(0f, Mathf.PI * 2f);
                    float rapidez = UnityEngine.Random.Range(45f, 130f);
                    chispasActivas.Add(new ChispaMusical
                    {
                        posPantalla = posAro + new Vector2(UnityEngine.Random.Range(-10f, 10f), UnityEngine.Random.Range(-10f, 10f)),
                        velocidad = new Vector2(Mathf.Cos(angulo) * rapidez, -Mathf.Abs(Mathf.Sin(angulo) * rapidez) - 35f),
                        color = (calificacion == CalificacionNota.Perfecto) ? new Color(1f, 0.92f, 0.30f) : coloresCarril[carril],
                        vidaMax = 0.55f,
                        vidaRestante = 0.55f,
                        simbolo = simbolos[UnityEngine.Random.Range(0, simbolos.Length)]
                    });
                }
            }
        }

        private void Update()
        {
            // Actualizar chispas musicales en pantalla
            for (int i = chispasActivas.Count - 1; i >= 0; i--)
            {
                chispasActivas[i].vidaRestante -= Time.deltaTime;
                chispasActivas[i].posPantalla += chispasActivas[i].velocidad * Time.deltaTime;
                if (chispasActivas[i].vidaRestante <= 0f)
                {
                    chispasActivas.RemoveAt(i);
                }
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                AlternarPausa();
            }

            // Atajo [H] para volver a ver la conversación explicativa de Don Chon
            if (Input.GetKeyDown(KeyCode.H) && !estaPausado)
            {
                var mgr = DueloArenaManager.Instance;
                if (mgr != null && mgr.Estado == EstadoDueloMusical.TocandoSon)
                {
                    AbrirGuiaDonChonEnPartida();
                }
            }
        }

        private bool mostrandoGuiaEnPausa = false;

        private void AbrirGuiaDonChonEnPartida()
        {
            mostrandoGuiaEnPausa = true;
            indiceDialogoIntro = 0;
            ColonialUIHelper.ReiniciarSecuenciaDialogoVN();
            Time.timeScale = 0f;
            if (AudioColonialManager.Instance != null) AudioColonialManager.Instance.ReproducirClickBoton();
        }

        public void AlternarPausa()
        {
            var mgr = DueloArenaManager.Instance;
            if (mgr == null) return;
            if (mgr.Estado == EstadoDueloMusical.DialogoIntroduccion ||
                mgr.Estado == EstadoDueloMusical.VictoriaFinal ||
                mgr.Estado == EstadoDueloMusical.Derrota)
                return;

            estaPausado = !estaPausado;
            mostrarOpcionesPausa = false;
            Time.timeScale = estaPausado ? 0f : 1f;

            if (AudioColonialManager.Instance != null)
            {
                AudioColonialManager.Instance.ReproducirClickBoton();
            }
        }

        private void InicializarEstilos()
        {
            ColonialUIHelper.AsegurarInicializacion();
            if (estiloTitulo != null) return;

            estiloTitulo = new GUIStyle(GUI.skin.label)
            {
                fontSize = 20,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                wordWrap = true,
                richText = true,
                normal = { textColor = new Color(1.0f, 0.88f, 0.35f) }
            };

            estiloSubtitulo = new GUIStyle(GUI.skin.label)
            {
                fontSize = 13,
                fontStyle = FontStyle.Italic,
                alignment = TextAnchor.MiddleCenter,
                wordWrap = true,
                richText = true,
                normal = { textColor = new Color(0.92f, 0.86f, 0.75f) }
            };

            estiloTextoBlanco = new GUIStyle(GUI.skin.label)
            {
                fontSize = 13,
                fontStyle = FontStyle.Normal,
                alignment = TextAnchor.MiddleLeft,
                richText = true,
                normal = { textColor = Color.white }
            };

            estiloLetraNota = new GUIStyle(GUI.skin.label)
            {
                fontSize = 18,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = new Color(0.08f, 0.05f, 0.02f, 1f) }
            };

            estiloComboGigante = new GUIStyle(GUI.skin.label)
            {
                fontSize = 26,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                richText = true,
                normal = { textColor = new Color(1f, 0.90f, 0.28f) }
            };

            estiloJuicio = new GUIStyle(GUI.skin.label)
            {
                fontSize = 22,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                richText = true
            };
        }

        private void OnGUI()
        {
            InicializarEstilos();
            CrearTexturasMusicales();

            var mgr = DueloArenaManager.Instance;
            if (mgr == null) return;

            // Si el jugador abrió la guía rápida [H] a mitad de la partida
            if (mostrandoGuiaEnPausa)
            {
                ColonialUIHelper.DibujarDialogoInteractivoVN(
                    lineasIntro,
                    texRetratoDonChon,
                    texRetratoGuanajicho,
                    ref indiceDialogoIntro,
                    () =>
                    {
                        mostrandoGuiaEnPausa = false;
                        Time.timeScale = 1f;
                    }
                );
                return;
            }

            // 1. Estado de Diálogo Inicial con Don Chon y Guanajicho
            if (mgr.Estado == EstadoDueloMusical.DialogoIntroduccion)
            {
                ColonialUIHelper.DibujarDialogoInteractivoVN(
                    lineasIntro,
                    texRetratoDonChon,
                    texRetratoGuanajicho,
                    ref indiceDialogoIntro,
                    () => mgr.FinalizarDialogoIntroduccion()
                );
                return;
            }

            // 2. Estado de Diálogo de Reto antes de cada Ronda Musical
            if (mgr.Estado == EstadoDueloMusical.DialogoRetoRival)
            {
                PrepararDialogoRival(mgr.RondaActual, mgr.RivalActual);
                Texture2D texRival = mgr.RivalActual != null ? mgr.RivalActual.RetratoTextura : null;

                ColonialUIHelper.DibujarDialogoInteractivoVN(
                    lineasRetoRival,
                    texRetratoGuanajicho,
                    texRival,
                    ref indiceDialogoRival,
                    () => mgr.FinalizarDialogoRetoRival()
                );
                return;
            }

            // 3. Barra Superior de Telemetría y Controles Inferiores
            DibujarBarraSuperior(mgr);
            DibujarBarraInferiorAyuda();

            // 4. Diapasón Central Estilo Guitar Hero + Paneles Laterales de Fervor y Multiplicador
            if (mgr.Estado == EstadoDueloMusical.CuentaRegresiva ||
                mgr.Estado == EstadoDueloMusical.TocandoSon ||
                mgr.Estado == EstadoDueloMusical.RondaSuperada)
            {
                DibujarDiapasonGuitarHero(mgr);
                DibujarPanelesLateralesDiapason(mgr);
                DibujarChispasMusicales();
            }

            // 5. Avisos de Cuenta Regresiva o Ronda Superada
            if (mgr.Estado == EstadoDueloMusical.CuentaRegresiva)
            {
                DibujarBannerCuentaRegresiva(mgr);
            }
            else if (mgr.Estado == EstadoDueloMusical.RondaSuperada)
            {
                DibujarBannerRondaSuperada(mgr);
            }
            else if (mgr.Estado == EstadoDueloMusical.VictoriaFinal)
            {
                DibujarModalVictoria(mgr);
            }
            else if (mgr.Estado == EstadoDueloMusical.Derrota)
            {
                DibujarModalDerrota(mgr);
            }

            // 6. Menú de Pausa
            if (estaPausado)
            {
                DibujarModalPausa(mgr);
            }
        }

        private void DibujarBarraSuperior(DueloArenaManager mgr)
        {
            Rect rTop = new Rect(10f, 8f, Screen.width - 20f, 48f);
            ColonialUIHelper.DibujarTarjetaInterior(rTop);

            string nomRival = mgr.RivalActual != null ? mgr.RivalActual.NombreRival : "Músico";
            string nomSon = mgr.RivalActual != null ? mgr.RivalActual.NombreSon : "Son Tradicional";

            float wTextoIzq = Mathf.Max(240f, rTop.width - 420f);
            GUI.Label(
                new Rect(rTop.x + 14f, rTop.y + 5f, wTextoIzq, 20f),
                $"🎸 <b>DUELO DE SONES</b> • Son {mgr.RondaActual}/{mgr.TotalRondas}: <color=#ffd700>{nomSon}</color>",
                estiloTextoBlanco
            );
            GUI.Label(
                new Rect(rTop.x + 14f, rTop.y + 24f, wTextoIzq, 18f),
                $"<color=#d8c8a8><i>Contrapunto musical frente a: <b>{nomRival}</b></i></color>",
                estiloTextoBlanco
            );

            // Puntaje al centro-derecha
            GUIStyle stPts = new GUIStyle(estiloTitulo) { fontSize = 16, alignment = TextAnchor.MiddleRight };
            GUI.Label(new Rect(rTop.xMax - 404f, rTop.y + 10f, 168f, 28f), $"⭐ {mgr.PuntajeTotal} pts", stPts);

            // Botones Guía [H] y Pausa [ESC]
            Rect rBtnGuia = new Rect(rTop.xMax - 224f, rTop.y + 8f, 102f, 32f);
            if (ColonialUIHelper.BotonColonial(rBtnGuia, "💬 GUÍA [H]", ColonialUIHelper.VarianteBoton.Talavera, 12))
            {
                AbrirGuiaDonChonEnPartida();
            }

            Rect rBtnPausa = new Rect(rTop.xMax - 114f, rTop.y + 8f, 104f, 32f);
            if (ColonialUIHelper.BotonColonial(rBtnPausa, "⏸ PAUSA", ColonialUIHelper.VarianteBoton.Madera, 12))
            {
                AlternarPausa();
            }
        }

        private void DibujarBarraInferiorAyuda()
        {
            float ancho = Mathf.Min(840f, Screen.width - 20f);
            Rect rBot = new Rect((Screen.width - ancho) * 0.5f, Screen.height - 42f, ancho, 36f);
            ColonialUIHelper.DibujarTarjetaInterior(rBot);

            GUIStyle stItem = new GUIStyle(GUI.skin.label)
            {
                fontSize = 12,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleLeft,
                richText = true,
                normal = { textColor = new Color(1f, 0.94f, 0.78f) }
            };

            string[] hexColores = { "#ff6666", "#ffe044", "#44ee66", "#55bbff" };
            float xActual = rBot.x + 18f;
            float yCentro = rBot.y + 8f;

            GUI.Label(new Rect(xActual, yCentro, 125f, 20f), "🎶 <b>Controles:</b>", stItem);
            xActual += 100f;

            float anchoBloque = (rBot.width - 250f) / 4f;
            for (int c = 0; c < 4; c++)
            {
                if (texFlechaColor[c] != null)
                {
                    GUI.DrawTexture(new Rect(xActual, rBot.y + 6f, 24f, 24f), texFlechaColor[c], ScaleMode.ScaleToFit);
                }
                GUI.Label(
                    new Rect(xActual + 27f, yCentro, anchoBloque - 28f, 20f),
                    $"<color={hexColores[c]}><b>[{letrasCarril[c]}] {nombresColorCarril[c]}</b></color>",
                    stItem
                );
                xActual += anchoBloque;
            }

            GUIStyle stClic = new GUIStyle(stItem) { alignment = TextAnchor.MiddleRight, fontSize = 11 };
            GUI.Label(new Rect(rBot.xMax - 148f, yCentro, 136f, 20f), "<i>(o Clic en los aros)</i>", stClic);
        }

        private void DibujarDiapasonGuitarHero(DueloArenaManager mgr)
        {
            float anchoDiapason = 324f;
            float yTop = 62f;
            float yBottom = Screen.height - 48f;
            float altoDiapason = yBottom - yTop;
            float xDiapason = (Screen.width - anchoDiapason) * 0.5f;

            Rect rectDiapason = new Rect(xDiapason, yTop, anchoDiapason, altoDiapason);

            // 1. Fondo de madera de ébano/caoba con rieles dorados
            if (texDiapasonMadera != null)
            {
                GUI.DrawTexture(rectDiapason, texDiapasonMadera, ScaleMode.StretchToFill);
            }

            float margenLateral = 42f;
            float espaciadoCarril = (anchoDiapason - margenLateral * 2f) / 3f;
            float yLineaImpacto = rectDiapason.yMax - 56f;

            // 2. Trastes horizontales que bajan al ritmo de la canción
            if (texTrasteHorizontal != null)
            {
                float intervaloTraste = 85f;
                float desplazamiento = (mgr.TiempoCancion * mgr.VelocidadCaidaNotas) % intervaloTraste;
                for (float yT = rectDiapason.y + desplazamiento; yT < rectDiapason.yMax - 12f; yT += intervaloTraste)
                {
                    GUI.DrawTexture(new Rect(rectDiapason.x + 6f, yT, rectDiapason.width - 12f, 2f), texTrasteHorizontal);
                }
            }

            GUIStyle stPlacaTecla = new GUIStyle(GUI.skin.label)
            {
                fontSize = 12,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = new Color(1f, 0.94f, 0.72f, 1f) }
            };

            // 3. Las 4 Cuerdas Doradas Verticales y Aros Receptores en la Línea de Impacto
            for (int c = 0; c < 4; c++)
            {
                float xCentroCarril = rectDiapason.x + margenLateral + c * espaciadoCarril;
                centrosArosPantalla[c] = new Vector2(xCentroCarril, yLineaImpacto);

                // Cuerda vertical
                float destello = mgr.ObtenerIntensidadDestelloCarril(c);
                float grosorCuerda = (destello > 0.05f) ? 5f : 2f;
                Color colorAnt = GUI.color;
                if (destello > 0.05f)
                {
                    GUI.color = Color.Lerp(Color.white, coloresCarril[c], 0.6f);
                }
                if (texCuerdaDorada != null)
                {
                    GUI.DrawTexture(new Rect(xCentroCarril - grosorCuerda * 0.5f, rectDiapason.y + 4f, grosorCuerda, altoDiapason - 8f), texCuerdaDorada);
                }
                GUI.color = colorAnt;

                // Aro receptor en la base del carril
                float tamAro = 62f + destello * 10f;
                Rect rectAro = new Rect(xCentroCarril - tamAro * 0.5f, yLineaImpacto - tamAro * 0.5f, tamAro, tamAro);
                Texture2D texAro = (destello > 0.05f) ? texAroReceptorActivo[c] : texAroReceptor[c];
                if (texAro != null)
                {
                    GUI.DrawTexture(rectAro, texAro, ScaleMode.ScaleToFit);
                }

                // Flecha Direccional Grande y Nítida en el centro del Aro Receptor
                float tamFlechaAro = 34f + destello * 6f;
                Rect rectFlechaAro = new Rect(
                    xCentroCarril - tamFlechaAro * 0.5f,
                    yLineaImpacto - tamFlechaAro * 0.5f,
                    tamFlechaAro,
                    tamFlechaAro
                );
                Texture2D texFlechaEnAro = (destello > 0.05f) ? texFlechaBlanca[c] : texFlechaColor[c];
                if (texFlechaEnAro != null)
                {
                    GUI.DrawTexture(rectFlechaAro, texFlechaEnAro, ScaleMode.ScaleToFit);
                }

                // Placa de Tecla [A], [S], [D], [F] debajo del aro receptor (sin tapar la flecha)
                Rect rectPlaca = new Rect(xCentroCarril - 19f, rectAro.yMax - 4f, 38f, 18f);
                if (texFondoPlacaTecla != null)
                {
                    GUI.DrawTexture(rectPlaca, texFondoPlacaTecla, ScaleMode.StretchToFill);
                }
                GUI.Label(rectPlaca, $"[{letrasCarril[c]}]", stPlacaTecla);

                // Permitir hacer clic directo con el ratón sobre el aro receptor
                if (Event.current.type == EventType.MouseDown && (rectAro.Contains(Event.current.mousePosition) || rectPlaca.Contains(Event.current.mousePosition)))
                {
                    mgr.ActivarCarril(c);
                    Event.current.Use();
                }
            }

            // 4. Dibujar las Gemas de Notas Musicales descendiendo por las cuerdas (con Flecha Nítida + Letra)
            var notas = mgr.PartituraActual;
            float tamGema = 56f;

            GUIStyle stLetraEtiquetaGema = new GUIStyle(GUI.skin.label)
            {
                fontSize = 11,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = new Color(1f, 0.96f, 0.78f, 1f) }
            };

            for (int i = 0; i < notas.Count; i++)
            {
                NotaRitmica n = notas[i];
                if (n.procesada) continue;

                // Posición vertical según el tiempo restante hasta el impacto
                float deltaTiempo = n.tiempoImpacto - mgr.TiempoCancion;
                float yNota = yLineaImpacto - (deltaTiempo * mgr.VelocidadCaidaNotas);

                // Mostrar únicamente las notas que están dentro de los límites del diapasón
                if (yNota < rectDiapason.y - 24f || yNota > rectDiapason.yMax + 20f) continue;

                int c = Mathf.Clamp(n.carril, 0, 3);
                float xCentro = rectDiapason.x + margenLateral + c * espaciadoCarril;
                Rect rectGema = new Rect(xCentro - tamGema * 0.5f, yNota - tamGema * 0.5f, tamGema, tamGema);

                if (texGemaNota[c] != null)
                {
                    GUI.DrawTexture(rectGema, texGemaNota[c], ScaleMode.ScaleToFit);
                }

                // Flecha direccional blanca con borde negro grueso dentro de la gema
                if (texFlechaBlanca[c] != null)
                {
                    float tamFlechaGema = 32f;
                    Rect rectFlechaGema = new Rect(
                        xCentro - tamFlechaGema * 0.5f,
                        yNota - tamFlechaGema * 0.5f + 2f,
                        tamFlechaGema,
                        tamFlechaGema
                    );
                    GUI.DrawTexture(rectFlechaGema, texFlechaBlanca[c], ScaleMode.ScaleToFit);
                }

                // Pequeña insignia superior en la gema con la letra (A, S, D, F)
                Rect rectEtiquetaLetra = new Rect(xCentro - 12f, rectGema.y - 4f, 24f, 16f);
                if (texFondoPlacaTecla != null)
                {
                    GUI.DrawTexture(rectEtiquetaLetra, texFondoPlacaTecla, ScaleMode.StretchToFill);
                }
                GUI.Label(rectEtiquetaLetra, letrasCarril[c], stLetraEtiquetaGema);
            }

            // 5. Texto de Juicio Instantáneo en la parte media-alta del diapasón (¡PERFECTO!, ¡BIEN!)
            float edadJuicio = Time.time - mgr.TiempoUltimoJuicio;
            if (edadJuicio >= 0f && edadJuicio < 0.65f && !string.IsNullOrEmpty(mgr.TextoUltimoJuicio))
            {
                float alpha = Mathf.Clamp01(1f - (edadJuicio / 0.65f));
                float elevacion = edadJuicio * 28f;
                Rect rJuicio = new Rect(rectDiapason.x, yLineaImpacto - 115f - elevacion, rectDiapason.width, 36f);

                estiloJuicio.normal.textColor = new Color(0f, 0f, 0f, alpha);
                GUI.Label(new Rect(rJuicio.x + 2f, rJuicio.y + 2f, rJuicio.width, rJuicio.height), mgr.TextoUltimoJuicio, estiloJuicio);

                estiloJuicio.normal.textColor = new Color(mgr.ColorUltimoJuicio.r, mgr.ColorUltimoJuicio.g, mgr.ColorUltimoJuicio.b, alpha);
                GUI.Label(rJuicio, mgr.TextoUltimoJuicio, estiloJuicio);
            }
        }

        private void DibujarPanelesLateralesDiapason(DueloArenaManager mgr)
        {
            float xCentro = Screen.width * 0.5f;

            GUIStyle stEncabezadoPanel = new GUIStyle(estiloSubtitulo)
            {
                fontSize = 12,
                fontStyle = FontStyle.Bold
            };

            // Panel Izquierdo: Multiplicador de Son (x1..x4) y Racha de Combo
            Rect rCombo = new Rect(xCentro - 356f, 72f, 186f, 120f);
            ColonialUIHelper.DibujarTarjetaInterior(rCombo);

            GUI.Label(new Rect(rCombo.x + 4f, rCombo.y + 8f, rCombo.width - 8f, 20f), "🔥 <b>MULTIPLICADOR</b>", stEncabezadoPanel);
            GUI.Label(new Rect(rCombo.x + 4f, rCombo.y + 30f, rCombo.width - 8f, 36f), $"x{mgr.Multiplicador}", estiloComboGigante);
            GUI.Label(new Rect(rCombo.x + 4f, rCombo.y + 70f, rCombo.width - 8f, 20f), $"Racha: <b>{mgr.ComboActual}</b> notas", estiloSubtitulo);
            GUI.Label(new Rect(rCombo.x + 4f, rCombo.y + 92f, rCombo.width - 8f, 18f), $"Precisión: <b>{mgr.PorcentajePrecision}%</b>", estiloSubtitulo);

            // Panel Derecho: Fervor de la Plaza (Rock Meter) y Progreso de Canción
            Rect rFervor = new Rect(xCentro + 170f, 72f, 194f, 120f);
            ColonialUIHelper.DibujarTarjetaInterior(rFervor);

            GUI.Label(new Rect(rFervor.x + 4f, rFervor.y + 8f, rFervor.width - 8f, 20f), "👏 <b>FERVOR DE LA PLAZA</b>", stEncabezadoPanel);

            // Barra de Fervor (0 a 100%)
            Rect rBarraFervor = new Rect(rFervor.x + 14f, rFervor.y + 34f, rFervor.width - 28f, 22f);
            GUI.DrawTexture(rBarraFervor, texOverlayNegro);
            float pctFervor = Mathf.Clamp01(mgr.FervorPlaza / 100f);
            Color colFervor = (pctFervor > 0.5f)
                ? new Color(0.22f, 0.85f, 0.35f, 0.95f)
                : (pctFervor > 0.25f ? new Color(0.95f, 0.75f, 0.18f, 0.95f) : new Color(0.95f, 0.25f, 0.22f, 0.95f));

            Color ant = GUI.color;
            GUI.color = colFervor;
            GUI.DrawTexture(new Rect(rBarraFervor.x + 2f, rBarraFervor.y + 2f, (rBarraFervor.width - 4f) * pctFervor, rBarraFervor.height - 4f), Texture2D.whiteTexture);
            GUI.color = ant;

            GUIStyle stCentro = new GUIStyle(GUI.skin.label) { fontSize = 12, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter, normal = { textColor = Color.white } };
            GUI.Label(rBarraFervor, $"{Mathf.RoundToInt(mgr.FervorPlaza)}%", stCentro);

            // Barra de Progreso del Son actual
            GUI.Label(new Rect(rFervor.x + 4f, rFervor.y + 62f, rFervor.width - 8f, 18f), "⏳ <b>Progreso del Son:</b>", estiloSubtitulo);
            Rect rBarraProg = new Rect(rFervor.x + 14f, rFervor.y + 84f, rFervor.width - 28f, 18f);
            float pctCancion = Mathf.Clamp01(mgr.TiempoCancion / Mathf.Max(1f, mgr.DuracionTotalCancion));
            ColonialUIHelper.DibujarBarraProgresoColonial(rBarraProg, pctCancion, $"{Mathf.RoundToInt(pctCancion * 100f)}%");
        }

        private void DibujarChispasMusicales()
        {
            if (chispasActivas.Count == 0) return;

            GUIStyle stChispa = new GUIStyle(GUI.skin.label)
            {
                fontSize = 18,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };

            for (int i = 0; i < chispasActivas.Count; i++)
            {
                var ch = chispasActivas[i];
                float alpha = Mathf.Clamp01(ch.vidaRestante / ch.vidaMax);
                stChispa.normal.textColor = new Color(ch.color.r, ch.color.g, ch.color.b, alpha);
                GUI.Label(new Rect(ch.posPantalla.x - 16f, ch.posPantalla.y - 16f, 32f, 32f), ch.simbolo, stChispa);
            }
        }

        private void DibujarBannerCuentaRegresiva(DueloArenaManager mgr)
        {
            float w = 380f, h = 110f;
            Rect r = new Rect((Screen.width - w) * 0.5f, (Screen.height - h) * 0.42f, w, h);
            ColonialUIHelper.DibujarMarcoVentana(r);
            ColonialUIHelper.DibujarTarjetaInterior(new Rect(r.x + 8f, r.y + 8f, r.width - 16f, r.height - 16f));

            GUIStyle stNum = new GUIStyle(estiloTitulo) { fontSize = 36 };
            int num = Mathf.CeilToInt(mgr.ValorCuentaRegresiva);
            string txt = (num > 0) ? $"🥁 ¡PREPÁRATE: {num}!" : "🎶 ¡¡A TOCAR EL SON!!";
            GUI.Label(new Rect(r.x, r.y + 16f, r.width, 46f), txt, stNum);
            GUI.Label(new Rect(r.x, r.y + 64f, r.width, 26f), "Coloca tus dedos en [A] [S] [D] [F] o en las Flechas", estiloSubtitulo);
        }

        private void DibujarBannerRondaSuperada(DueloArenaManager mgr)
        {
            float w = 480f, h = 120f;
            Rect r = new Rect((Screen.width - w) * 0.5f, (Screen.height - h) * 0.42f, w, h);
            ColonialUIHelper.DibujarMarcoVentana(r);
            ColonialUIHelper.DibujarTarjetaInterior(new Rect(r.x + 8f, r.y + 8f, r.width - 16f, r.height - 16f));

            string nomRival = mgr.RivalActual != null ? mgr.RivalActual.NombreRival : "Rival";
            GUIStyle stSup = new GUIStyle(estiloTitulo) { fontSize = 24, normal = { textColor = new Color(0.4f, 1f, 0.45f) } };
            GUI.Label(new Rect(r.x, r.y + 18f, r.width, 36f), $"🎉 ¡SON {mgr.RondaActual} COMPLETADO! 🎉", stSup);
            GUI.Label(new Rect(r.x, r.y + 62f, r.width, 30f), $"¡La plaza aplaude tu victoria musical sobre <b>{nomRival}</b>!", estiloSubtitulo);
        }

        private void DibujarModalVictoria(DueloArenaManager mgr)
        {
            if (texOverlayNegro != null) GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), texOverlayNegro);

            float anchoModal = Mathf.Min(860f, Screen.width - 24f);
            float altoModal = Mathf.Min(500f, Screen.height - 20f);
            Rect rModal = new Rect((Screen.width - anchoModal) * 0.5f, (Screen.height - altoModal) * 0.5f, anchoModal, altoModal);

            ColonialUIHelper.DibujarMarcoVentana(
                rModal,
                "🏆 ¡MAESTRO DEL SON INSURGENTE!",
                "Has ganado el Salvoconducto Real a San Felipe • 1810"
            );

            Rect rInterior = new Rect(rModal.x + 12f, rModal.y + 66f, rModal.width - 24f, rModal.height - 78f);
            ColonialUIHelper.DibujarTarjetaInterior(rInterior);

            // Columna Izquierda: Don Chon con el Salvoconducto
            float wColIzq = Mathf.Clamp(rInterior.width * 0.25f, 175f, 215f);
            Rect rColIzq = new Rect(rInterior.x + 10f, rInterior.y + 8f, wColIzq, rInterior.height - 16f);

            if (texRetratoDonChon != null)
            {
                Rect rImg = new Rect(rColIzq.x, rColIzq.y + 2f, rColIzq.width, rColIzq.height - 58f);
                GUI.DrawTexture(rImg, texRetratoDonChon, ScaleMode.ScaleToFit);
            }

            Rect rBadge = new Rect(rColIzq.x, rColIzq.yMax - 52f, rColIzq.width, 50f);
            ColonialUIHelper.DibujarTarjetaInterior(rBadge);
            GUIStyle stBadgeTit = new GUIStyle(GUI.skin.label)
            {
                fontSize = 11,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = new Color(1f, 0.88f, 0.35f) }
            };
            GUI.Label(new Rect(rBadge.x, rBadge.y + 4f, rBadge.width, 18f), "📜 SALVOCONDUCTO", stBadgeTit);
            GUI.Label(new Rect(rBadge.x, rBadge.y + 23f, rBadge.width, 18f), "Paso Libre a San Felipe", estiloSubtitulo);

            // Columna Derecha con posicionamiento explícito proporcional (sin desbordes)
            float xColDer = rColIzq.xMax + 14f;
            float wColDer = rInterior.xMax - xColDer - 12f;
            float yCursor = rInterior.y + 10f;

            float prec = mgr.PorcentajePrecision;
            string rango = (prec >= 85f) ? "⭐⭐⭐ ¡VIRTUOSO LEGENDARIO DE DOLORES!" :
                           (prec >= 65f) ? "⭐⭐ ¡TAMBORILERO DE GRAN TEMPLE!" : "⭐ ¡VICTORIA MUSICAL INSURGENTE!";

            GUIStyle stRango = new GUIStyle(estiloTitulo) { fontSize = 18 };
            GUI.Label(new Rect(xColDer, yCursor, wColDer, 24f), rango, stRango);
            yCursor += 26f;

            GUIStyle stCita = new GUIStyle(GUI.skin.label)
            {
                fontSize = 12,
                fontStyle = FontStyle.Italic,
                wordWrap = true,
                alignment = TextAnchor.UpperCenter,
                normal = { textColor = new Color(0.94f, 0.94f, 0.94f) }
            };
            GUI.Label(
                new Rect(xColDer, yCursor, wColDer, 42f),
                "«¡Qué maestría, Guanajicho! Con tu tambor hiciste vibrar toda la plaza frente a Don Silvestre, el Miliciano y el Capitán. El Camino Real hacia San Felipe está abierto.»",
                stCita
            );
            yCursor += 46f;

            float altoBtn = 44f;
            float yBotones = rInterior.yMax - altoBtn - 10f;
            float espacioTarjetas = Mathf.Max(136f, yBotones - yCursor - 8f);
            float altoTarjeta = Mathf.Clamp((espacioTarjetas - 12f) / 3f, 40f, 48f);

            DibujarTarjetaStatRect(
                new Rect(xColDer, yCursor, wColDer, altoTarjeta),
                "🎯 Precisión Musical y Racha",
                $"{prec}% de notas acertadas • Racha Máx: {mgr.ComboMaximo} seguidas"
            );
            yCursor += altoTarjeta + 5f;

            DibujarTarjetaStatRect(
                new Rect(xColDer, yCursor, wColDer, altoTarjeta),
                "⭐ Puntaje de Prestigio",
                $"{mgr.PuntajeTotal} puntos ({mgr.NotasPerfectas} Perfectas / {mgr.NotasBuenas} Buenas)"
            );
            yCursor += altoTarjeta + 5f;

            DibujarTarjetaStatRect(
                new Rect(xColDer, yCursor, wColDer, altoTarjeta),
                "🗺️ Recompensa Desbloqueada",
                "Salvoconducto a San Felipe (Torres Mochas) disponible en el Mapa"
            );

            // Fila de botones inferiores
            float wBtnMapa = (wColDer - 16f) * 0.46f;
            float wBtnRepetir = (wColDer - 16f) * 0.28f;
            float wBtnMenu = (wColDer - 16f) * 0.26f;

            Rect rBtnMapa = new Rect(xColDer, yBotones, wBtnMapa, altoBtn);
            if (ColonialUIHelper.BotonColonial(rBtnMapa, "🗺️ VIAJAR AL MAPA", ColonialUIHelper.VarianteBoton.Verde, 14))
            {
                mgr.IrAlOverworld();
            }

            Rect rBtnRepetir = new Rect(rBtnMapa.xMax + 8f, yBotones, wBtnRepetir, altoBtn);
            if (ColonialUIHelper.BotonColonial(rBtnRepetir, "🔄 REPETIR", ColonialUIHelper.VarianteBoton.Madera, 13))
            {
                mgr.ReiniciarDuelo();
            }

            Rect rBtnMenu = new Rect(rBtnRepetir.xMax + 8f, yBotones, wBtnMenu, altoBtn);
            if (ColonialUIHelper.BotonColonial(rBtnMenu, "🏠 MENÚ", ColonialUIHelper.VarianteBoton.Talavera, 13))
            {
                mgr.IrAlMenuPrincipal();
            }
        }

        private void DibujarTarjetaStatRect(Rect r, string titulo, string valor)
        {
            ColonialUIHelper.DibujarTarjetaInterior(r);

            GUIStyle stTit = new GUIStyle(GUI.skin.label)
            {
                fontSize = 11,
                fontStyle = FontStyle.Bold,
                normal = { textColor = new Color(1.0f, 0.85f, 0.35f) }
            };
            GUI.Label(new Rect(r.x + 12f, r.y + 3f, r.width - 24f, 17f), titulo, stTit);

            GUIStyle stVal = new GUIStyle(GUI.skin.label)
            {
                fontSize = 13,
                fontStyle = FontStyle.Bold,
                normal = { textColor = Color.white }
            };
            GUI.Label(new Rect(r.x + 12f, r.y + 20f, r.width - 24f, 19f), valor, stVal);
        }

        private void DibujarModalDerrota(DueloArenaManager mgr)
        {
            if (texOverlayNegro != null) GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), texOverlayNegro);

            float anchoModal = Mathf.Min(800f, Screen.width - 24f);
            float altoModal = Mathf.Min(460f, Screen.height - 20f);
            Rect rModal = new Rect((Screen.width - anchoModal) * 0.5f, (Screen.height - altoModal) * 0.5f, anchoModal, altoModal);

            ColonialUIHelper.DibujarMarcoVentana(
                rModal,
                "💀 SE APAGÓ EL COMPÁS EN LA PLAZA",
                "El fervor del público llegó a cero"
            );

            Rect rInterior = new Rect(rModal.x + 12f, rModal.y + 66f, rModal.width - 24f, rModal.height - 78f);
            ColonialUIHelper.DibujarTarjetaInterior(rInterior);

            float wColIzq = Mathf.Clamp(rInterior.width * 0.25f, 170f, 205f);
            Rect rColIzq = new Rect(rInterior.x + 10f, rInterior.y + 8f, wColIzq, rInterior.height - 16f);

            if (texRetratoDonChon != null)
            {
                Rect rImg = new Rect(rColIzq.x, rColIzq.y + 2f, rColIzq.width, rColIzq.height - 34f);
                GUI.DrawTexture(rImg, texRetratoDonChon, ScaleMode.ScaleToFit);
            }

            GUI.Label(new Rect(rColIzq.x, rColIzq.yMax - 28f, rColIzq.width, 22f), "Consejo de Don Chon", estiloSubtitulo);

            float xColDer = rColIzq.xMax + 14f;
            float wColDer = rInterior.xMax - xColDer - 12f;
            float yCursor = rInterior.y + 12f;

            GUIStyle stAviso = new GUIStyle(estiloTitulo)
            {
                fontSize = 16,
                normal = { textColor = new Color(1f, 0.45f, 0.40f) }
            };
            GUI.Label(new Rect(xColDer, yCursor, wColDer, 26f), "«¡No te desanimes, Guanajicho! Todo gran músico afina el oído.»", stAviso);
            yCursor += 32f;

            GUIStyle stDesc = new GUIStyle(GUI.skin.label)
            {
                fontSize = 12,
                wordWrap = true,
                richText = true,
                normal = { textColor = Color.white }
            };
            GUI.Label(
                new Rect(xColDer, yCursor, wColDer, 78f),
                "• Espera a que cada gema de color entre al <b>aro receptor</b> de abajo antes de pulsar.\n" +
                "• Cada nota lleva escrita su letra (<b>A, S, D, F</b>) encima para que la identifiques fácil.\n" +
                "• Puedes reintentar <b>este mismo Son</b> sin perder tu avance en las rondas anteriores.",
                stDesc
            );
            yCursor += 86f;

            DibujarTarjetaStatRect(
                new Rect(xColDer, yCursor, wColDer, 46f),
                "🎵 Son Actual",
                $"Ronda {mgr.RondaActual} de {mgr.TotalRondas} • {mgr.MotivoDerrota}"
            );

            float altoBtn = 44f;
            float yBotones = rInterior.yMax - altoBtn - 10f;
            float wBtnReintentar = (wColDer - 10f) * 0.60f;
            float wBtnMapa = (wColDer - 10f) * 0.40f;

            Rect rBtnReintentar = new Rect(xColDer, yBotones, wBtnReintentar, altoBtn);
            if (ColonialUIHelper.BotonColonial(rBtnReintentar, "🔄 REINTENTAR ESTE SON [ESPACIO]", ColonialUIHelper.VarianteBoton.Dorado, 14))
            {
                mgr.ReintentarRondaActual();
            }

            Rect rBtnMapa = new Rect(rBtnReintentar.xMax + 10f, yBotones, wBtnMapa, altoBtn);
            if (ColonialUIHelper.BotonColonial(rBtnMapa, "🗺️ VOLVER AL MAPA", ColonialUIHelper.VarianteBoton.Madera, 13))
            {
                mgr.IrAlOverworld();
            }
        }

        private void DibujarModalPausa(DueloArenaManager mgr)
        {
            if (mostrarOpcionesPausa)
            {
                ColonialUIHelper.DibujarModalOpcionesColonial(
                    ref volumenGeneral,
                    ref volumenMusica,
                    ref volumenSFX,
                    ref pantallaCompleta,
                    onCerrar: () => mostrarOpcionesPausa = false
                );
            }
            else
            {
                ColonialUIHelper.DibujarMenuPausaColonial(
                    "Nivel 3: Duelo de Sones en la Plaza de Dolores",
                    onContinuar: () => AlternarPausa(),
                    onAbrirOpciones: () => mostrarOpcionesPausa = true,
                    onReiniciar: () =>
                    {
                        Time.timeScale = 1f;
                        mgr.ReiniciarDuelo();
                    },
                    onIrOverworld: () =>
                    {
                        Time.timeScale = 1f;
                        mgr.IrAlOverworld();
                    },
                    onIrMenuPrincipal: () =>
                    {
                        Time.timeScale = 1f;
                        mgr.IrAlMenuPrincipal();
                    }
                );
            }
        }
    }
}
