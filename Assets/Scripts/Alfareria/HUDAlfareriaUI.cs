using System;
using System.Collections.Generic;
using UnityEngine;
using CaminosDeGuanajuato.Core;
using CaminosDeGuanajuato.UI;

namespace CaminosDeGuanajuato.Alfareria
{
    /// <summary>
    /// Interfaz gráfica ligera, limpia y guiada paso a paso para el Nivel 4:
    /// El Taller Alfarero de la Francia Chiquita (San Felipe Torres Mochas).
    /// Muestra únicamente UNA estación activa a la vez en la parte inferior central,
    /// dejando libre el escenario del torno y el horno, y guiando cada uno de los 3 clics.
    /// </summary>
    public class HUDAlfareriaUI : MonoBehaviour
    {
        private GUIStyle estiloTitulo;
        private GUIStyle estiloSubtitulo;
        private GUIStyle estiloTextoBlanco;
        private GUIStyle estiloNotificacion;

        // Texturas de iconos de Alfarería de San Felipe
        private Texture2D texIconoJarro;
        private Texture2D texIconoCazuela;
        private Texture2D texIconoCantaro;
        private Texture2D texEsmalteTerracota;
        private Texture2D texEsmalteVerde;
        private Texture2D texEsmalteAzul;
        private Texture2D texEsmalteOro;

        private Texture2D texOverlayNegro;
        private Texture2D texBarraVerde;
        private Texture2D texBarraAmarilla;
        private Texture2D texBarraRoja;
        private Texture2D texZonaOptimaHorno;
        private Texture2D texCursorBlanco;

        // Retratos para los diálogos estilo Novela Visual
        private Texture2D texDonChon;
        private Texture2D texCriollaCarlota;
        private Texture2D texJovenAlfarero;

        // Control de Diálogo Inicial (Novela Visual)
        private bool mostrarDialogoInicial = true;
        private int indiceDialogo = 0;
        private ColonialUIHelper.LineaDialogoVN[] lineasDialogoSanFelipe;

        // Textos flotantes de puntuación
        private class NotificacionFlotante
        {
            public string texto;
            public Color color;
            public Vector2 posPantalla;
            public float vidaRestante;
            public float vidaMax;
        }
        private readonly List<NotificacionFlotante> notificaciones = new List<NotificacionFlotante>();

        // Pausa y Opciones
        private bool estaPausado = false;
        private bool mostrarOpcionesPausa = false;
        private float volumenGeneral = 0.85f;
        private float volumenMusica = 0.65f;
        private float volumenSFX = 0.85f;
        private bool pantallaCompleta = true;

        private void Awake()
        {
            CargarTexturasAlfareria();
            ConfigurarDialogoSanFelipe();
        }

        private void Start()
        {
            CargarTexturasAlfareria();

            if (AlfareriaGameManager.Instance != null)
            {
                AlfareriaGameManager.Instance.OnNotificacionFlotante -= AgregarNotificacionFlotante;
                AlfareriaGameManager.Instance.OnNotificacionFlotante += AgregarNotificacionFlotante;
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
            if (AlfareriaGameManager.Instance != null)
            {
                AlfareriaGameManager.Instance.OnNotificacionFlotante -= AgregarNotificacionFlotante;
            }
        }

        private void CargarTexturasAlfareria()
        {
            if (texOverlayNegro == null)
            {
                texOverlayNegro = CrearTexturaSolida(new Color(0f, 0f, 0f, 0.78f));
                texBarraVerde = CrearTexturaSolida(new Color(0.22f, 0.86f, 0.38f, 0.96f));
                texBarraAmarilla = CrearTexturaSolida(new Color(0.96f, 0.78f, 0.18f, 0.96f));
                texBarraRoja = CrearTexturaSolida(new Color(0.95f, 0.26f, 0.22f, 0.96f));
                texZonaOptimaHorno = CrearTexturaSolida(new Color(0.22f, 0.88f, 0.38f, 0.55f));
                texCursorBlanco = CrearTexturaSolida(new Color(1f, 0.96f, 0.75f, 1f));
            }

            if (texIconoJarro == null)
            {
                texIconoJarro = Resources.Load<Texture2D>("Alfareria/Icono_JarroSanFelipe") ?? GenerarIconoVasijaProcedural(1, new Color(0.80f, 0.40f, 0.20f));
                texIconoCazuela = Resources.Load<Texture2D>("Alfareria/Icono_CazuelaSanFelipe") ?? GenerarIconoVasijaProcedural(2, new Color(0.80f, 0.40f, 0.20f));
                texIconoCantaro = Resources.Load<Texture2D>("Alfareria/Icono_CantaroSanFelipe") ?? GenerarIconoVasijaProcedural(3, new Color(0.80f, 0.40f, 0.20f));

                texEsmalteTerracota = Resources.Load<Texture2D>("Alfareria/Icono_EsmalteTerracota") ?? GenerarIconoEsmalteProcedural(new Color(0.84f, 0.38f, 0.18f));
                texEsmalteVerde = Resources.Load<Texture2D>("Alfareria/Icono_EsmalteVerdeVidriado") ?? GenerarIconoEsmalteProcedural(new Color(0.18f, 0.74f, 0.36f));
                texEsmalteAzul = Resources.Load<Texture2D>("Alfareria/Icono_EsmalteAzulMayolica") ?? GenerarIconoEsmalteProcedural(new Color(0.20f, 0.52f, 0.92f));
                texEsmalteOro = Resources.Load<Texture2D>("Alfareria/Icono_EsmalteOroGala") ?? GenerarIconoEsmalteProcedural(new Color(0.95f, 0.76f, 0.18f));
            }

            if (texDonChon == null) texDonChon = Resources.Load<Texture2D>("PersonajesColonial/DonChon");
            if (texCriollaCarlota == null) texCriollaCarlota = Resources.Load<Texture2D>("PersonajesColonial/CriollaSanFelipe");
            if (texJovenAlfarero == null) texJovenAlfarero = Resources.Load<Texture2D>("PersonajesColonial/JovenAlfarero");
        }

        private Texture2D CrearTexturaSolida(Color col)
        {
            Texture2D t = new Texture2D(1, 1);
            t.SetPixel(0, 0, col);
            t.Apply();
            return t;
        }

        private Texture2D GenerarIconoVasijaProcedural(int tipo, Color colBarro)
        {
            int tam = 48;
            Texture2D tex = new Texture2D(tam, tam, TextureFormat.RGBA32, false);
            for (int y = 0; y < tam; y++)
            {
                for (int x = 0; x < tam; x++)
                {
                    float nx = (x - 23.5f) / 20f;
                    float ny = (y - 23.5f) / 20f;
                    bool dentro = false;
                    if (tipo == 1) dentro = (nx * nx + (ny + 0.15f) * (ny + 0.15f) < 0.42f) || (Mathf.Abs(nx) < 0.32f && ny > 0.1f && ny < 0.65f);
                    else if (tipo == 2) dentro = (nx * nx * 0.55f + (ny + 0.1f) * (ny + 0.1f) < 0.36f && ny < 0.32f);
                    else dentro = (nx * nx * 1.2f + (ny + 0.05f) * (ny + 0.05f) * 0.7f < 0.42f);

                    tex.SetPixel(x, y, dentro ? colBarro : Color.clear);
                }
            }
            tex.Apply();
            return tex;
        }

        private Texture2D GenerarIconoEsmalteProcedural(Color col)
        {
            int tam = 48;
            Texture2D tex = new Texture2D(tam, tam, TextureFormat.RGBA32, false);
            for (int y = 0; y < tam; y++)
            {
                for (int x = 0; x < tam; x++)
                {
                    float d = Vector2.Distance(new Vector2(x, y), new Vector2(23.5f, 23.5f));
                    if (d > 20f) tex.SetPixel(x, y, Color.clear);
                    else if (d > 17f) tex.SetPixel(x, y, new Color(0.12f, 0.08f, 0.04f, 1f));
                    else if (d < 6f) tex.SetPixel(x, y, Color.Lerp(col, Color.white, 0.55f));
                    else tex.SetPixel(x, y, col);
                }
            }
            tex.Apply();
            return tex;
        }

        private void ConfigurarDialogoSanFelipe()
        {
            indiceDialogo = 0;
            ColonialUIHelper.ReiniciarSecuenciaDialogoVN();

            lineasDialogoSanFelipe = new ColonialUIHelper.LineaDialogoVN[]
            {
                new ColonialUIHelper.LineaDialogoVN(
                    "Don Chon",
                    "¡Llegamos a <b>San Felipe Torres Mochas</b>, Guanajicho! Estamos en el patio de la <b>Francia Chiquita</b>, donde el Cura Hidalgo enseñó el arte del barro vidriado.",
                    true,
                    "🏺 NIVEL 4: Prepara 6 vasijas artesanales en solo 3 clics",
                    0.92f
                ),
                new ColonialUIHelper.LineaDialogoVN(
                    "Doña Carlota",
                    "¡Jugar es súper sencillo! Mira el <b>globo sobre el vecino</b> para ver qué pieza pide, y abajo te irá apareciendo <b>un solo paso a la vez</b>:",
                    false,
                    "1️⃣ Clic en la Vasija ➔ 2️⃣ Clic en el Color ➔ 3️⃣ Clic en el Horno",
                    1.08f
                ),
                new ColonialUIHelper.LineaDialogoVN(
                    "Don Chon",
                    "Fíjate bien en lo que pide cada cliente para no equivocarte de forma ni de esmalte, y sácala del horno justo en la <b>Zona Verde</b>. ¡Manos a la obra!",
                    true,
                    "⭐ Pulsa [ESPACIO] o haz Clic para abrir el taller",
                    0.92f
                )
            };
        }

        private void AgregarNotificacionFlotante(string texto, Color color)
        {
            notificaciones.Add(new NotificacionFlotante
            {
                texto = texto,
                color = color,
                posPantalla = new Vector2(Screen.width * 0.5f, Screen.height * 0.52f),
                vidaMax = 1.4f,
                vidaRestante = 1.4f
            });
        }

        private void Update()
        {
            for (int i = notificaciones.Count - 1; i >= 0; i--)
            {
                notificaciones[i].vidaRestante -= Time.deltaTime;
                notificaciones[i].posPantalla.y -= 32f * Time.deltaTime;
                if (notificaciones[i].vidaRestante <= 0f)
                {
                    notificaciones.RemoveAt(i);
                }
            }

            if (Input.GetKeyDown(KeyCode.Escape) && !mostrarDialogoInicial)
            {
                AlternarPausa();
            }

            if (Input.GetKeyDown(KeyCode.H) && !mostrarDialogoInicial && !estaPausado)
            {
                var mgr = AlfareriaGameManager.Instance;
                if (mgr != null && !mgr.JuegoTerminado)
                {
                    AbrirGuiaDonChon();
                }
            }
        }

        public void AbrirGuiaDonChon()
        {
            mostrarDialogoInicial = true;
            indiceDialogo = 0;
            ColonialUIHelper.ReiniciarSecuenciaDialogoVN();
            Time.timeScale = 0f;
            if (AudioColonialManager.Instance != null) AudioColonialManager.Instance.ReproducirClickBoton();
        }

        public void AlternarPausa()
        {
            var mgr = AlfareriaGameManager.Instance;
            if (mgr == null || mgr.JuegoTerminado) return;

            estaPausado = !estaPausado;
            mostrarOpcionesPausa = false;
            Time.timeScale = estaPausado ? 0f : 1f;
            if (AudioColonialManager.Instance != null) AudioColonialManager.Instance.ReproducirClickBoton();
        }

        private void InicializarEstilos()
        {
            ColonialUIHelper.AsegurarInicializacion();
            if (estiloTitulo != null) return;

            estiloTitulo = new GUIStyle(GUI.skin.label)
            {
                fontSize = 17,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                wordWrap = true,
                richText = true,
                normal = { textColor = new Color(1.0f, 0.88f, 0.35f) }
            };

            estiloSubtitulo = new GUIStyle(GUI.skin.label)
            {
                fontSize = 12,
                fontStyle = FontStyle.Italic,
                alignment = TextAnchor.MiddleCenter,
                wordWrap = true,
                richText = true,
                normal = { textColor = new Color(0.92f, 0.86f, 0.74f) }
            };

            estiloTextoBlanco = new GUIStyle(GUI.skin.label)
            {
                fontSize = 13,
                fontStyle = FontStyle.Normal,
                alignment = TextAnchor.MiddleLeft,
                wordWrap = true,
                richText = true,
                normal = { textColor = Color.white }
            };

            estiloNotificacion = new GUIStyle(GUI.skin.label)
            {
                fontSize = 20,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                richText = true
            };
        }

        private void OnGUI()
        {
            InicializarEstilos();
            CargarTexturasAlfareria();

            var mgr = AlfareriaGameManager.Instance;
            if (mgr == null) return;

            // 1. Diálogo Interactivo de Bienvenida
            if (mostrarDialogoInicial)
            {
                ColonialUIHelper.DibujarDialogoInteractivoVN(
                    lineasDialogoSanFelipe,
                    texDonChon,
                    texCriollaCarlota != null ? texCriollaCarlota : texJovenAlfarero,
                    ref indiceDialogo,
                    () =>
                    {
                        mostrarDialogoInicial = false;
                        Time.timeScale = 1f;
                        if (!mgr.JuegoActivo && !mgr.JuegoTerminado)
                        {
                            mgr.ComenzarJornadaTaller();
                        }
                    }
                );
                return;
            }

            // 2. Barra Superior Minimalista
            DibujarBarraSuperiorLigera(mgr);

            // 3. Globo del Pedido sobre el Vecino + Tarjeta Inferior de 1 Solo Paso a la Vez
            if (mgr.JuegoActivo && !mgr.JuegoTerminado)
            {
                DibujarGloboPedidoSobreVecino(mgr);
                DibujarTarjetaPasoUnicoInferior(mgr);
                DibujarNotificacionesFlotantes();
            }

            // 4. Pantallas Modales de Fin de Partida o Pausa
            if (mgr.JuegoTerminado)
            {
                if (mgr.VictoriaAlcanzada)
                    DibujarModalVictoria(mgr);
                else
                    DibujarModalDerrota(mgr);
            }
            else if (estaPausado)
            {
                DibujarModalPausa(mgr);
            }
        }

        private void DibujarBarraSuperiorLigera(AlfareriaGameManager mgr)
        {
            Rect rTop = new Rect(10f, 8f, Screen.width - 20f, 44f);
            ColonialUIHelper.DibujarTarjetaInterior(rTop);

            GUIStyle stIzq = new GUIStyle(estiloTextoBlanco) { fontSize = 12, wordWrap = false };
            GUI.Label(
                new Rect(rTop.x + 14f, rTop.y + 11f, 385f, 22f),
                "🏺 <b>SAN FELIPE TORRES MOCHAS</b> • <i>La Francia Chiquita</i>",
                stIzq
            );

            int vidas = Mathf.Max(0, mgr.MaxClientesImpacientes - mgr.ClientesPerdidos);
            string corazones = new string('♥', vidas);

            GUIStyle stCentro = new GUIStyle(estiloTitulo) { fontSize = 14 };
            GUI.Label(
                new Rect(rTop.x + 375f, rTop.y + 8f, rTop.width - 605f, 28f),
                $"🏺 Piezas: <color=#44ff66>{mgr.PiezasEntregadas} / {mgr.MetaPiezas}</color>   •   ⭐ {mgr.PuntajeTotal} pts   •   <color=#ff6666>{corazones}</color>",
                stCentro
            );

            Rect rBtnGuia = new Rect(rTop.xMax - 220f, rTop.y + 6f, 100f, 32f);
            if (ColonialUIHelper.BotonColonial(rBtnGuia, "💬 GUÍA [H]", ColonialUIHelper.VarianteBoton.Talavera, 12))
            {
                AbrirGuiaDonChon();
            }

            Rect rBtnPausa = new Rect(rTop.xMax - 112f, rTop.y + 6f, 102f, 32f);
            if (ColonialUIHelper.BotonColonial(rBtnPausa, "⏸ PAUSA", ColonialUIHelper.VarianteBoton.Madera, 12))
            {
                AlternarPausa();
            }
        }

        /// <summary>
        /// Globo de pedido limpio y visual colocado justo encima del vecino en el patio,
        /// dejando totalmente despejado el torno y el horno de la izquierda.
        /// </summary>
        private void DibujarGloboPedidoSobreVecino(AlfareriaGameManager mgr)
        {
            PedidoAlfareria ped = mgr.PedidoActivo;
            if (ped == null || Camera.main == null) return;

            Vector3 posMundo = mgr.PosicionClienteMundo + new Vector3(0f, 3.0f, 0f);
            Vector3 posScreen = Camera.main.WorldToScreenPoint(posMundo);
            if (posScreen.z < 0f) return;

            float guiX = posScreen.x;
            float guiY = Screen.height - posScreen.y;

            float w = 278f;
            float h = 86f;
            float xCard = Mathf.Clamp(guiX - w * 0.5f, Screen.width * 0.42f, Screen.width - w - 14f);
            float yCard = Mathf.Clamp(guiY - h - 10f, 58f, Screen.height - 255f);

            Rect rGlobo = new Rect(xCard, yCard, w, h);
            ColonialUIHelper.DibujarMarcoVentana(rGlobo);
            ColonialUIHelper.DibujarTarjetaInterior(new Rect(rGlobo.x + 5f, rGlobo.y + 5f, rGlobo.width - 10f, rGlobo.height - 10f));

            // Icono grande de la vasija coloreada con el esmalte que pide el vecino
            Rect rCajaIcono = new Rect(rGlobo.x + 12f, rGlobo.y + 13f, 60f, 60f);
            ColonialUIHelper.DibujarTarjetaInterior(rCajaIcono);

            Texture2D texForma = ObtenerIconoForma(ped.formaRequerida);
            if (texForma != null)
            {
                Color ant = GUI.color;
                GUI.color = PedidoAlfareria.ObtenerColorEsmalte(ped.esmalteRequerido);
                GUI.DrawTexture(new Rect(rCajaIcono.x + 6f, rCajaIcono.y + 6f, 48f, 48f), texForma, ScaleMode.ScaleToFit);
                GUI.color = ant;
            }

            float xTxt = rCajaIcono.xMax + 10f;
            float wTxt = rGlobo.xMax - xTxt - 10f;

            GUIStyle stNombre = new GUIStyle(GUI.skin.label)
            {
                fontSize = 12,
                fontStyle = FontStyle.Bold,
                wordWrap = false,
                normal = { textColor = new Color(1f, 0.88f, 0.35f) }
            };
            GUI.Label(new Rect(xTxt, rGlobo.y + 10f, wTxt, 18f), $"💬 {ped.nombreCliente} pide:", stNombre);

            string hexCol = PedidoAlfareria.ObtenerHexEsmalte(ped.esmalteRequerido);
            GUIStyle stDetalle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 13,
                fontStyle = FontStyle.Bold,
                wordWrap = false,
                richText = true,
                normal = { textColor = Color.white }
            };
            GUI.Label(new Rect(xTxt, rGlobo.y + 28f, wTxt, 18f), $"• {PedidoAlfareria.ObtenerNombreForma(ped.formaRequerida)}", stDetalle);
            GUI.Label(new Rect(xTxt, rGlobo.y + 45f, wTxt, 18f), $"• <color={hexCol}>{PedidoAlfareria.ObtenerNombreEsmalte(ped.esmalteRequerido)}</color>", stDetalle);

            // Barra de paciencia delgada en la base del globo
            Rect rBar = new Rect(xTxt, rGlobo.y + 65f, wTxt - 4f, 8f);
            GUI.DrawTexture(rBar, texOverlayNegro);
            float pct = Mathf.Clamp01(mgr.PacienciaRestante / mgr.PacienciaMaximaActual);
            Texture2D texFill = pct > 0.5f ? texBarraVerde : (pct > 0.25f ? texBarraAmarilla : texBarraRoja);
            GUI.DrawTexture(new Rect(rBar.x + 1f, rBar.y + 1f, (rBar.width - 2f) * pct, rBar.height - 2f), texFill);
        }

        /// <summary>
        /// Panel inferior único y compacto: muestra SOLO el paso actual (Paso 1, Paso 2 o Paso 3)
        /// sin revelar qué botón pulsar para que el jugador observe la comanda del cliente.
        /// </summary>
        private void DibujarTarjetaPasoUnicoInferior(AlfareriaGameManager mgr)
        {
            PedidoAlfareria ped = mgr.PedidoActivo;
            if (ped == null) return;

            float wPanel = Mathf.Min(560f, Screen.width - 28f);
            float hPanel = 148f;
            Rect rPanel = new Rect((Screen.width - wPanel) * 0.5f, Screen.height - hPanel - 10f, wPanel, hPanel);

            ColonialUIHelper.DibujarMarcoVentana(rPanel);
            Rect rInt = new Rect(rPanel.x + 8f, rPanel.y + 8f, rPanel.width - 16f, rPanel.height - 16f);
            ColonialUIHelper.DibujarTarjetaInterior(rInt);

            GUIStyle stTituloPaso = new GUIStyle(estiloTitulo) { fontSize = 15, wordWrap = false };

            if (mgr.FaseActual == FaseTallerSimple.Paso1_ElegirForma)
            {
                // =====================================================================
                // PASO 1 DE 3: ELEGIR LA VASIJA
                // =====================================================================
                GUI.Label(
                    new Rect(rInt.x + 10f, rInt.y + 6f, rInt.width - 20f, 24f),
                    "PASO 1 DE 3: Selecciona la forma de barro en el torno",
                    stTituloPaso
                );

                float gap = 12f;
                float wBtn = (rInt.width - 28f - gap * 2f) / 3f;
                float yBtn = rInt.y + 36f;
                float hBtn = 86f;

                DibujarBotonGrandeForma(
                    new Rect(rInt.x + 14f, yBtn, wBtn, hBtn),
                    TipoFormaBarro.JarroChocolatero,
                    "[1] Jarro",
                    texIconoJarro,
                    mgr
                );
                DibujarBotonGrandeForma(
                    new Rect(rInt.x + 14f + wBtn + gap, yBtn, wBtn, hBtn),
                    TipoFormaBarro.CazuelaMoletera,
                    "[2] Cazuela",
                    texIconoCazuela,
                    mgr
                );
                DibujarBotonGrandeForma(
                    new Rect(rInt.x + 14f + (wBtn + gap) * 2f, yBtn, wBtn, hBtn),
                    TipoFormaBarro.CantaroCaminoReal,
                    "[3] Cántaro",
                    texIconoCantaro,
                    mgr
                );
            }
            else if (mgr.FaseActual == FaseTallerSimple.Paso2_ElegirColor)
            {
                // =====================================================================
                // PASO 2 DE 3: ELEGIR EL COLOR DEL ESMALTE
                // =====================================================================
                GUI.Label(
                    new Rect(rInt.x + 10f, rInt.y + 6f, rInt.width - 20f, 24f),
                    "PASO 2 DE 3: Selecciona el esmalte vidriado tradicional",
                    stTituloPaso
                );

                float gap = 10f;
                float wBtn = (rInt.width - 28f - gap * 3f) / 4f;
                float yBtn = rInt.y + 36f;
                float hBtn = 86f;

                DibujarBotonGrandeColor(
                    new Rect(rInt.x + 14f, yBtn, wBtn, hBtn),
                    TipoEsmalteBarro.TerracotaBrunida,
                    "[1] Terracota",
                    texEsmalteTerracota,
                    mgr
                );
                DibujarBotonGrandeColor(
                    new Rect(rInt.x + 14f + (wBtn + gap), yBtn, wBtn, hBtn),
                    TipoEsmalteBarro.VerdeVidriado,
                    "[2] Verde",
                    texEsmalteVerde,
                    mgr
                );
                DibujarBotonGrandeColor(
                    new Rect(rInt.x + 14f + (wBtn + gap) * 2f, yBtn, wBtn, hBtn),
                    TipoEsmalteBarro.AzulMayolica,
                    "[3] Azul",
                    texEsmalteAzul,
                    mgr
                );
                DibujarBotonGrandeColor(
                    new Rect(rInt.x + 14f + (wBtn + gap) * 3f, yBtn, wBtn, hBtn),
                    TipoEsmalteBarro.OroDeGala,
                    "[4] Oro",
                    texEsmalteOro,
                    mgr
                );
            }
            else
            {
                // =====================================================================
                // PASO 3 DE 3: HORNO DE LEÑA (Barra de sincronización + Botón)
                // =====================================================================
                GUI.Label(
                    new Rect(rInt.x + 10f, rInt.y + 6f, rInt.width - 20f, 24f),
                    "PASO 3 DE 3: Retira la pieza del horno en la Zona de Cocción",
                    stTituloPaso
                );

                // Barra grande de sincronización del horno
                Rect rBarra = new Rect(rInt.x + 24f, rInt.y + 36f, rInt.width - 48f, 28f);
                GUI.DrawTexture(rBarra, texOverlayNegro);

                // Zona Óptima central (32% a 72%)
                float xVerde = rBarra.x + rBarra.width * 0.32f;
                float wVerde = rBarra.width * 0.40f;
                GUI.DrawTexture(new Rect(xVerde, rBarra.y + 2f, wVerde, rBarra.height - 4f), texZonaOptimaHorno);

                GUIStyle stZ = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 11,
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleCenter,
                    normal = { textColor = Color.white }
                };
                GUI.Label(new Rect(xVerde, rBarra.y, wVerde, rBarra.height), "PUNTO EXACTO DE COCCIÓN", stZ);

                // Marcador móvil (aguja luminosa del horno)
                float xCursor = rBarra.x + 4f + (rBarra.width - 14f) * mgr.CursorHorno01;
                GUI.DrawTexture(new Rect(xCursor, rBarra.y - 3f, 10f, rBarra.height + 6f), texCursorBlanco);

                // Botón para Sacar y Entregar
                Rect rBtnSacar = new Rect(rInt.x + 24f, rInt.y + 74f, rInt.width - 48f, 48f);
                if (ColonialUIHelper.BotonColonial(rBtnSacar, "🔥 SACAR DEL HORNO Y ENTREGAR [ESPACIO / CLIC]", ColonialUIHelper.VarianteBoton.Dorado, 14))
                {
                    mgr.ConfirmarHornoYEntregarPaso3();
                }
            }
        }

        private void DibujarBotonGrandeForma(
            Rect rect,
            TipoFormaBarro forma,
            string nombre,
            Texture2D icono,
            AlfareriaGameManager mgr)
        {
            if (ColonialUIHelper.BotonColonial(rect, "", ColonialUIHelper.VarianteBoton.Madera, 12))
            {
                mgr.ElegirFormaPaso1(forma);
            }

            if (icono != null)
            {
                GUI.DrawTexture(new Rect(rect.x + (rect.width - 46f) * 0.5f, rect.y + 6f, 46f, 46f), icono, ScaleMode.ScaleToFit);
            }

            GUIStyle st = new GUIStyle(GUI.skin.label)
            {
                fontSize = 12,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = Color.white }
            };
            GUI.Label(new Rect(rect.x + 2f, rect.yMax - 26f, rect.width - 4f, 22f), nombre, st);
        }

        private void DibujarBotonGrandeColor(
            Rect rect,
            TipoEsmalteBarro esmalte,
            string nombre,
            Texture2D icono,
            AlfareriaGameManager mgr)
        {
            if (ColonialUIHelper.BotonColonial(rect, "", ColonialUIHelper.VarianteBoton.Madera, 12))
            {
                mgr.ElegirColorPaso2(esmalte);
            }

            if (icono != null)
            {
                GUI.DrawTexture(new Rect(rect.x + (rect.width - 44f) * 0.5f, rect.y + 6f, 44f, 44f), icono, ScaleMode.ScaleToFit);
            }

            GUIStyle st = new GUIStyle(GUI.skin.label)
            {
                fontSize = 12,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = Color.white }
            };
            GUI.Label(new Rect(rect.x + 2f, rect.yMax - 26f, rect.width - 4f, 22f), nombre, st);
        }

        private Texture2D ObtenerIconoForma(TipoFormaBarro forma)
        {
            switch (forma)
            {
                case TipoFormaBarro.JarroChocolatero: return texIconoJarro;
                case TipoFormaBarro.CazuelaMoletera: return texIconoCazuela;
                case TipoFormaBarro.CantaroCaminoReal: return texIconoCantaro;
                default: return texIconoJarro;
            }
        }

        private void DibujarNotificacionesFlotantes()
        {
            for (int i = 0; i < notificaciones.Count; i++)
            {
                var n = notificaciones[i];
                float alpha = Mathf.Clamp01(n.vidaRestante / n.vidaMax);
                Rect r = new Rect(n.posPantalla.x - 180f, n.posPantalla.y, 360f, 34f);

                estiloNotificacion.normal.textColor = new Color(0f, 0f, 0f, alpha);
                GUI.Label(new Rect(r.x + 2f, r.y + 2f, r.width, r.height), n.texto, estiloNotificacion);

                estiloNotificacion.normal.textColor = new Color(n.color.r, n.color.g, n.color.b, alpha);
                GUI.Label(r, n.texto, estiloNotificacion);
            }
        }

        private void DibujarModalVictoria(AlfareriaGameManager mgr)
        {
            if (texOverlayNegro != null) GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), texOverlayNegro);

            float anchoModal = Mathf.Min(860f, Screen.width - 24f);
            float altoModal = Mathf.Min(500f, Screen.height - 20f);
            Rect rModal = new Rect((Screen.width - anchoModal) * 0.5f, (Screen.height - altoModal) * 0.5f, anchoModal, altoModal);

            ColonialUIHelper.DibujarMarcoVentana(
                rModal,
                "🏆 ¡MAESTRO ALFARERO DE LA FRANCIA CHIQUITA!",
                "San Felipe Torres Mochas • Camino Real de Tierra Adentro (1810)"
            );

            Rect rInterior = new Rect(rModal.x + 12f, rModal.y + 66f, rModal.width - 24f, rModal.height - 78f);
            ColonialUIHelper.DibujarTarjetaInterior(rInterior);

            float wColIzq = Mathf.Clamp(rInterior.width * 0.25f, 175f, 215f);
            Rect rColIzq = new Rect(rInterior.x + 10f, rInterior.y + 8f, wColIzq, rInterior.height - 16f);

            Texture2D texRetratoVic = texCriollaCarlota != null ? texCriollaCarlota : texDonChon;
            if (texRetratoVic != null)
            {
                Rect rImg = new Rect(rColIzq.x, rColIzq.y + 2f, rColIzq.width, rColIzq.height - 58f);
                GUI.DrawTexture(rImg, texRetratoVic, ScaleMode.ScaleToFit);
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
            GUI.Label(new Rect(rBadge.x, rBadge.y + 4f, rBadge.width, 18f), "🏺 SELLO DE SAN FELIPE", stBadgeTit);
            GUI.Label(new Rect(rBadge.x, rBadge.y + 23f, rBadge.width, 18f), "Torres Mochas • 1810", estiloSubtitulo);

            float xColDer = rColIzq.xMax + 14f;
            float wColDer = rInterior.xMax - xColDer - 12f;
            float yCursor = rInterior.y + 10f;

            GUIStyle stRango = new GUIStyle(estiloTitulo) { fontSize = 18 };
            GUI.Label(new Rect(xColDer, yCursor, wColDer, 24f), "⭐⭐⭐ ¡ORGULLO ARTESANAL DE LAS TORRES MOCHAS!", stRango);
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
                "«¡Qué manos tan prodigiosas, Guanajicho! Los jarros, cazuelas y cántaros de la Francia Chiquita ya van en camino con la caravana bajo el repique de las Torres Mochas.»",
                stCita
            );
            yCursor += 46f;

            float altoBtn = 44f;
            float yBotones = rInterior.yMax - altoBtn - 10f;
            float espacioTarjetas = Mathf.Max(136f, yBotones - yCursor - 8f);
            float altoTarjeta = Mathf.Clamp((espacioTarjetas - 12f) / 3f, 40f, 48f);

            DibujarTarjetaStatRect(
                new Rect(xColDer, yCursor, wColDer, altoTarjeta),
                "🏺 Alfarería Entregada",
                $"{mgr.PiezasEntregadas} de {mgr.MetaPiezas} piezas tradicionales cocidas para San Felipe"
            );
            yCursor += altoTarjeta + 5f;

            DibujarTarjetaStatRect(
                new Rect(xColDer, yCursor, wColDer, altoTarjeta),
                "⭐ Puntuación de Prestigio Artesanal",
                $"{mgr.PuntajeTotal} puntos acumulados en el taller"
            );
            yCursor += altoTarjeta + 5f;

            DibujarTarjetaStatRect(
                new Rect(xColDer, yCursor, wColDer, altoTarjeta),
                "🎖️ Insignia Histórica Obtenida",
                "Sello Real de las Torres Mochas y la Francia Chiquita (+160 Recursos)"
            );

            float wBtnMapa = (wColDer - 16f) * 0.46f;
            float wBtnRepetir = (wColDer - 16f) * 0.28f;
            float wBtnMenu = (wColDer - 16f) * 0.26f;

            Rect rBtnMapa = new Rect(xColDer, yBotones, wBtnMapa, altoBtn);
            if (ColonialUIHelper.BotonColonial(rBtnMapa, "🗺️ VOLVER AL MAPA", ColonialUIHelper.VarianteBoton.Verde, 14))
            {
                mgr.IrAlOverworld();
            }

            Rect rBtnRepetir = new Rect(rBtnMapa.xMax + 8f, yBotones, wBtnRepetir, altoBtn);
            if (ColonialUIHelper.BotonColonial(rBtnRepetir, "🔄 REPETIR", ColonialUIHelper.VarianteBoton.Madera, 13))
            {
                mgr.ReiniciarNivel();
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

        private void DibujarModalDerrota(AlfareriaGameManager mgr)
        {
            if (texOverlayNegro != null) GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), texOverlayNegro);

            float anchoModal = Mathf.Min(800f, Screen.width - 24f);
            float altoModal = Mathf.Min(460f, Screen.height - 20f);
            Rect rModal = new Rect((Screen.width - anchoModal) * 0.5f, (Screen.height - altoModal) * 0.5f, anchoModal, altoModal);

            ColonialUIHelper.DibujarMarcoVentana(
                rModal,
                "🏺 EL HORNO DE LA FRANCIA CHIQUITA SE ENFRIÓ",
                "La caravana de San Felipe necesita tus piezas a tiempo"
            );

            Rect rInterior = new Rect(rModal.x + 12f, rModal.y + 66f, rModal.width - 24f, rModal.height - 78f);
            ColonialUIHelper.DibujarTarjetaInterior(rInterior);

            float wColIzq = Mathf.Clamp(rInterior.width * 0.25f, 170f, 205f);
            Rect rColIzq = new Rect(rInterior.x + 10f, rInterior.y + 8f, wColIzq, rInterior.height - 16f);

            if (texDonChon != null)
            {
                Rect rImg = new Rect(rColIzq.x, rColIzq.y + 2f, rColIzq.width, rColIzq.height - 34f);
                GUI.DrawTexture(rImg, texDonChon, ScaleMode.ScaleToFit);
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
            GUI.Label(new Rect(xColDer, yCursor, wColDer, 26f), "«¡Ánimo, Guanajicho! Solo sigue el botón verde con estrella (★).»", stAviso);
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
                "• <b>Paso 1:</b> Haz clic en la vasija marcada en verde <b>(★)</b>.\n" +
                "• <b>Paso 2:</b> Haz clic en el color marcado en verde <b>(★)</b>.\n" +
                "• <b>Paso 3:</b> Pulsa <b>[ESPACIO]</b> o haz clic cuando la aguja pase por la <b>Zona Verde</b>.",
                stDesc
            );
            yCursor += 86f;

            DibujarTarjetaStatRect(
                new Rect(xColDer, yCursor, wColDer, 46f),
                "📋 Avance en el Taller",
                $"Entregaste {mgr.PiezasEntregadas} de {mgr.MetaPiezas} piezas • {mgr.MotivoDerrota}"
            );

            float altoBtn = 44f;
            float yBotones = rInterior.yMax - altoBtn - 10f;
            float wBtnReintentar = (wColDer - 10f) * 0.58f;
            float wBtnMapa = (wColDer - 10f) * 0.42f;

            Rect rBtnReintentar = new Rect(xColDer, yBotones, wBtnReintentar, altoBtn);
            if (ColonialUIHelper.BotonColonial(rBtnReintentar, "🔄 REINTENTAR TALLER", ColonialUIHelper.VarianteBoton.Dorado, 14))
            {
                mgr.ReiniciarNivel();
            }

            Rect rBtnMapa = new Rect(rBtnReintentar.xMax + 10f, yBotones, wBtnMapa, altoBtn);
            if (ColonialUIHelper.BotonColonial(rBtnMapa, "🗺️ VOLVER AL MAPA", ColonialUIHelper.VarianteBoton.Madera, 13))
            {
                mgr.IrAlOverworld();
            }
        }

        private void DibujarModalPausa(AlfareriaGameManager mgr)
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
                    "Nivel 4: El Taller Alfarero de la Francia Chiquita (San Felipe)",
                    onContinuar: () => AlternarPausa(),
                    onAbrirOpciones: () => mostrarOpcionesPausa = true,
                    onReiniciar: () => mgr.ReiniciarNivel(),
                    onIrOverworld: () => mgr.IrAlOverworld(),
                    onIrMenuPrincipal: () => mgr.IrAlMenuPrincipal()
                );
            }
        }
    }
}
