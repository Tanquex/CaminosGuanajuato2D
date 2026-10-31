using System;
using UnityEngine;
using CaminosDeGuanajuato.Core;

namespace CaminosDeGuanajuato.UI
{
    /// <summary>
    /// Sistema compartido de interfaz de usuario con estética Colonial / Talavera / Virreinal.
    /// Proporciona ventanas enmarcadas en oro viejo y caoba, botones artesanales con estados hover/activo,
    /// controles de volumen interactivos personalizados y modales unificados de Pausa y Opciones.
    /// </summary>
    public static class ColonialUIHelper
    {
        public enum VarianteBoton
        {
            Madera,
            Dorado,
            Verde,
            Talavera,
            Rojo,
            Gris
        }

        private static bool inicializado = false;

        // Texturas procedurales con bordes biselados
        private static Texture2D texOverlayOscuro;
        private static Texture2D texVentanaColonial;
        private static Texture2D texCabeceraColonial;
        private static Texture2D texTarjetaInterior;
        private static Texture2D texSeparadorOro;

        // Texturas de botones (Normal y Hover)
        private static Texture2D texBtnMadera;
        private static Texture2D texBtnMaderaHover;
        private static Texture2D texBtnDorado;
        private static Texture2D texBtnDoradoHover;
        private static Texture2D texBtnVerde;
        private static Texture2D texBtnVerdeHover;
        private static Texture2D texBtnTalavera;
        private static Texture2D texBtnTalaveraHover;
        private static Texture2D texBtnRojo;
        private static Texture2D texBtnRojoHover;
        private static Texture2D texBtnGris;

        // Texturas para barras de volumen y progreso
        private static Texture2D texBarraFondo;
        private static Texture2D texBarraRellenoOro;
        private static Texture2D texBarraRellenoVerde;

        // Estilos tipográficos
        private static GUIStyle estiloTituloVentana;
        private static GUIStyle estiloSubtituloVentana;
        private static GUIStyle estiloEtiquetaSeccion;
        private static GUIStyle estiloTextoNormal;
        private static GUIStyle estiloTextoCentrado;
        private static GUIStyle estiloPorcentaje;
        private static GUIStyle estiloBotonBase;

        public static void AsegurarInicializacion()
        {
            if (inicializado && texVentanaColonial != null && texBtnMadera != null && estiloTituloVentana != null)
                return;

            CrearTexturasColoniales();
            CrearEstilosColoniales();
            inicializado = true;
        }

        private static void CrearTexturasColoniales()
        {
            texOverlayOscuro = CrearTexturaSolida(new Color(0.04f, 0.02f, 0.01f, 0.78f));

            // Ventana principal: Caoba oscuro profundo con doble marco de oro viejo y esquinas azul Talavera
            texVentanaColonial = CrearTexturaMarcoColonial(
                64, 64,
                new Color(0.14f, 0.07f, 0.04f, 0.97f), // Fondo caoba oscuro
                new Color(0.88f, 0.68f, 0.22f, 1.0f),  // Borde exterior dorado
                new Color(0.45f, 0.26f, 0.12f, 1.0f),  // Bisel intermedio terracota
                new Color(0.16f, 0.38f, 0.68f, 1.0f)   // Detalle esquina Talavera
            );

            // Cabecera interna
            texCabeceraColonial = CrearTexturaMarcoSimple(
                48, 48,
                new Color(0.22f, 0.10f, 0.05f, 0.95f),
                new Color(0.78f, 0.56f, 0.18f, 0.90f),
                2
            );

            // Tarjeta o contenedor interior
            texTarjetaInterior = CrearTexturaMarcoSimple(
                48, 48,
                new Color(0.19f, 0.10f, 0.06f, 0.92f),
                new Color(0.55f, 0.36f, 0.16f, 0.85f),
                2
            );

            texSeparadorOro = CrearTexturaSolida(new Color(0.85f, 0.65f, 0.20f, 0.75f));

            // Botones con marco artesanal
            texBtnMadera = CrearTexturaBoton(
                new Color(0.27f, 0.12f, 0.06f, 0.96f),
                new Color(0.36f, 0.17f, 0.09f, 0.96f),
                new Color(0.86f, 0.65f, 0.22f, 1f)
            );
            texBtnMaderaHover = CrearTexturaBoton(
                new Color(0.45f, 0.21f, 0.09f, 0.98f),
                new Color(0.58f, 0.28f, 0.12f, 0.98f),
                new Color(1.0f, 0.85f, 0.35f, 1f)
            );

            texBtnDorado = CrearTexturaBoton(
                new Color(0.72f, 0.45f, 0.10f, 0.98f),
                new Color(0.88f, 0.62f, 0.16f, 0.98f),
                new Color(1.0f, 0.90f, 0.48f, 1f)
            );
            texBtnDoradoHover = CrearTexturaBoton(
                new Color(0.85f, 0.56f, 0.14f, 1f),
                new Color(0.98f, 0.74f, 0.22f, 1f),
                new Color(1.0f, 0.96f, 0.68f, 1f)
            );

            texBtnVerde = CrearTexturaBoton(
                new Color(0.12f, 0.42f, 0.20f, 0.96f),
                new Color(0.18f, 0.56f, 0.28f, 0.96f),
                new Color(0.85f, 0.72f, 0.25f, 1f)
            );
            texBtnVerdeHover = CrearTexturaBoton(
                new Color(0.18f, 0.55f, 0.26f, 0.99f),
                new Color(0.25f, 0.70f, 0.35f, 0.99f),
                new Color(1.0f, 0.88f, 0.38f, 1f)
            );

            texBtnTalavera = CrearTexturaBoton(
                new Color(0.10f, 0.28f, 0.52f, 0.96f),
                new Color(0.16f, 0.38f, 0.68f, 0.96f),
                new Color(0.90f, 0.72f, 0.24f, 1f)
            );
            texBtnTalaveraHover = CrearTexturaBoton(
                new Color(0.16f, 0.38f, 0.68f, 0.99f),
                new Color(0.24f, 0.50f, 0.84f, 0.99f),
                new Color(1.0f, 0.88f, 0.42f, 1f)
            );

            texBtnRojo = CrearTexturaBoton(
                new Color(0.48f, 0.12f, 0.10f, 0.96f),
                new Color(0.64f, 0.18f, 0.14f, 0.96f),
                new Color(0.88f, 0.66f, 0.22f, 1f)
            );
            texBtnRojoHover = CrearTexturaBoton(
                new Color(0.62f, 0.16f, 0.13f, 0.99f),
                new Color(0.78f, 0.24f, 0.18f, 0.99f),
                new Color(1.0f, 0.85f, 0.36f, 1f)
            );

            texBtnGris = CrearTexturaBoton(
                new Color(0.22f, 0.20f, 0.18f, 0.90f),
                new Color(0.28f, 0.26f, 0.24f, 0.90f),
                new Color(0.48f, 0.44f, 0.38f, 0.90f)
            );

            texBarraFondo = CrearTexturaMarcoSimple(
                32, 16,
                new Color(0.09f, 0.05f, 0.03f, 0.95f),
                new Color(0.55f, 0.38f, 0.18f, 0.90f),
                1
            );

            texBarraRellenoOro = CrearTexturaGradienteVertical(
                16,
                new Color(0.78f, 0.52f, 0.12f, 1f),
                new Color(0.98f, 0.78f, 0.24f, 1f)
            );

            texBarraRellenoVerde = CrearTexturaGradienteVertical(
                16,
                new Color(0.16f, 0.56f, 0.25f, 1f),
                new Color(0.28f, 0.78f, 0.38f, 1f)
            );
        }

        private static void CrearEstilosColoniales()
        {
            estiloTituloVentana = new GUIStyle(GUI.skin.label)
            {
                fontSize = 20,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                richText = true
            };
            estiloTituloVentana.normal.textColor = new Color(0.99f, 0.82f, 0.30f);

            estiloSubtituloVentana = new GUIStyle(GUI.skin.label)
            {
                fontSize = 12,
                fontStyle = FontStyle.Italic,
                alignment = TextAnchor.MiddleCenter,
                richText = true
            };
            estiloSubtituloVentana.normal.textColor = new Color(0.90f, 0.82f, 0.68f);

            estiloEtiquetaSeccion = new GUIStyle(GUI.skin.label)
            {
                fontSize = 13,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleLeft,
                richText = true
            };
            estiloEtiquetaSeccion.normal.textColor = new Color(0.98f, 0.88f, 0.68f);

            estiloTextoNormal = new GUIStyle(GUI.skin.label)
            {
                fontSize = 13,
                fontStyle = FontStyle.Normal,
                alignment = TextAnchor.MiddleLeft,
                wordWrap = true,
                richText = true
            };
            estiloTextoNormal.normal.textColor = new Color(0.94f, 0.90f, 0.82f);

            estiloTextoCentrado = new GUIStyle(estiloTextoNormal)
            {
                alignment = TextAnchor.MiddleCenter
            };

            estiloPorcentaje = new GUIStyle(GUI.skin.label)
            {
                fontSize = 12,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
            estiloPorcentaje.normal.textColor = new Color(1.0f, 0.86f, 0.35f);

            estiloBotonBase = new GUIStyle(GUI.skin.button)
            {
                fontSize = 13,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                richText = true,
                padding = new RectOffset(8, 8, 4, 4)
            };
            estiloBotonBase.normal.textColor = new Color(0.99f, 0.95f, 0.84f);
            estiloBotonBase.hover.textColor = new Color(1f, 0.98f, 0.90f);
            estiloBotonBase.active.textColor = new Color(1f, 0.90f, 0.60f);
        }

        public static void DibujarOverlayOscuro()
        {
            AsegurarInicializacion();
            if (texOverlayOscuro != null)
            {
                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), texOverlayOscuro);
            }
        }

        public static void DibujarMarcoVentana(Rect rect, string titulo, string subtitulo = null)
        {
            AsegurarInicializacion();

            // Sombra proyectada suave
            Color prev = GUI.color;
            GUI.color = new Color(0f, 0f, 0f, 0.45f);
            GUI.DrawTexture(new Rect(rect.x + 6, rect.y + 6, rect.width, rect.height), texOverlayOscuro);
            GUI.color = prev;

            // Fondo con doble marco dorado
            GUI.DrawTexture(rect, texVentanaColonial);

            // Cabecera decorativa
            float hCabecera = string.IsNullOrEmpty(subtitulo) ? 42f : 56f;
            Rect rectCabecera = new Rect(rect.x + 12, rect.y + 10, rect.width - 24, hCabecera);
            GUI.DrawTexture(rectCabecera, texCabeceraColonial);

            // Sombra de texto para el título
            Rect rectTitulo = new Rect(rectCabecera.x, rectCabecera.y + 4, rectCabecera.width, 26);
            DibujarTextoConSombra(rectTitulo, titulo, estiloTituloVentana);

            if (!string.IsNullOrEmpty(subtitulo))
            {
                Rect rectSub = new Rect(rectCabecera.x, rectCabecera.y + 29, rectCabecera.width, 20);
                GUI.Label(rectSub, subtitulo, estiloSubtituloVentana);
            }

            // Línea divisoria dorada bajo la cabecera
            GUI.DrawTexture(new Rect(rect.x + 18, rectCabecera.yMax + 4, rect.width - 36, 2), texSeparadorOro);
        }

        public static void DibujarTarjetaInterior(Rect rect)
        {
            AsegurarInicializacion();
            GUI.DrawTexture(rect, texTarjetaInterior);
        }

        public static void DibujarSeparador(float x, float y, float ancho)
        {
            AsegurarInicializacion();
            GUI.DrawTexture(new Rect(x, y, ancho, 2), texSeparadorOro);
        }

        public static void DibujarTextoConSombra(Rect rect, string texto, GUIStyle estilo)
        {
            Color prev = estilo.normal.textColor;
            estilo.normal.textColor = new Color(0.05f, 0.02f, 0.01f, 0.90f);
            GUI.Label(new Rect(rect.x + 1, rect.y + 1, rect.width, rect.height), texto, estilo);
            estilo.normal.textColor = prev;
            GUI.Label(rect, texto, estilo);
        }

        public static bool BotonColonial(Rect rect, string texto, VarianteBoton variante = VarianteBoton.Madera, int fontSize = 13)
        {
            AsegurarInicializacion();

            bool hover = rect.Contains(Event.current.mousePosition);
            Texture2D texFondo = ObtenerTexturaBoton(variante, hover);

            // Sombra sutil del botón
            Color prev = GUI.color;
            GUI.color = new Color(0f, 0f, 0f, 0.35f);
            GUI.DrawTexture(new Rect(rect.x + 2, rect.y + 2, rect.width, rect.height), texOverlayOscuro);
            GUI.color = prev;

            if (texFondo != null)
            {
                GUI.DrawTexture(rect, texFondo);
            }

            estiloBotonBase.fontSize = fontSize;
            estiloBotonBase.normal.background = null;
            estiloBotonBase.hover.background = null;
            estiloBotonBase.active.background = null;

            if (variante == VarianteBoton.Dorado)
            {
                estiloBotonBase.normal.textColor = new Color(0.16f, 0.07f, 0.02f);
                estiloBotonBase.hover.textColor = new Color(0.10f, 0.04f, 0.01f);
            }
            else if (variante == VarianteBoton.Gris)
            {
                estiloBotonBase.normal.textColor = new Color(0.72f, 0.70f, 0.66f);
                estiloBotonBase.hover.textColor = new Color(0.72f, 0.70f, 0.66f);
            }
            else
            {
                estiloBotonBase.normal.textColor = hover
                    ? new Color(1.0f, 0.96f, 0.80f)
                    : new Color(0.98f, 0.92f, 0.78f);
                estiloBotonBase.hover.textColor = new Color(1.0f, 0.96f, 0.80f);
            }

            // Sombra de texto en botones oscuros
            if (variante != VarianteBoton.Dorado)
            {
                Color cOrig = estiloBotonBase.normal.textColor;
                estiloBotonBase.normal.textColor = new Color(0.05f, 0.02f, 0.01f, 0.85f);
                GUI.Label(new Rect(rect.x + 1, rect.y + 1, rect.width, rect.height), texto, estiloBotonBase);
                estiloBotonBase.normal.textColor = cOrig;
            }

            bool clic = GUI.Button(rect, texto, estiloBotonBase);
            if (clic && variante != VarianteBoton.Gris && AudioColonialManager.Instance != null)
            {
                AudioColonialManager.Instance.ReproducirClickBoton();
            }
            return clic;
        }

        /// <summary>
        /// Dibuja un control de volumen colonial con etiqueta, botones [-] y [+],
        /// barra de progreso dorada interactiva (clic o arrastrar) y medalla de porcentaje.
        /// </summary>
        public static float DibujarControlVolumenColonial(Rect rectFila, string etiqueta, float valorActual)
        {
            AsegurarInicializacion();

            GUI.DrawTexture(rectFila, texTarjetaInterior);

            float pad = 10f;
            float wEtiqueta = Mathf.Min(185f, rectFila.width * 0.40f);
            Rect rLabel = new Rect(rectFila.x + pad, rectFila.y, wEtiqueta, rectFila.height);
            GUI.Label(rLabel, etiqueta, estiloEtiquetaSeccion);

            float btnSize = 26f;
            float wPct = 48f;
            float yCtrl = rectFila.y + (rectFila.height - btnSize) * 0.5f;

            Rect rMenos = new Rect(rLabel.xMax + 4f, yCtrl, btnSize, btnSize);
            Rect rPct = new Rect(rectFila.xMax - pad - wPct, yCtrl, wPct, btnSize);
            Rect rMas = new Rect(rPct.x - btnSize - 6f, yCtrl, btnSize, btnSize);
            Rect rBarra = new Rect(rMenos.xMax + 8f, rectFila.y + (rectFila.height - 16f) * 0.5f, Mathf.Max(60f, rMas.x - rMenos.xMax - 16f), 16f);

            float nuevoValor = Mathf.Clamp01(valorActual);

            if (BotonColonial(rMenos, "−", VarianteBoton.Madera, 15))
            {
                nuevoValor = Mathf.Clamp01(Mathf.Round((nuevoValor - 0.10f) * 10f) / 10f);
            }

            // Barra interactiva (soporta clic y arrastre)
            GUI.DrawTexture(rBarra, texBarraFondo);
            float anchoRelleno = Mathf.Clamp01(nuevoValor) * (rBarra.width - 4f);
            if (anchoRelleno > 1f)
            {
                GUI.DrawTexture(new Rect(rBarra.x + 2f, rBarra.y + 2f, anchoRelleno, rBarra.height - 4f), texBarraRellenoOro);
            }

            // Divisiones decorativas cada 25%
            for (int i = 1; i <= 3; i++)
            {
                float xDiv = rBarra.x + (rBarra.width * (i * 0.25f));
                GUI.DrawTexture(new Rect(xDiv, rBarra.y + 2f, 1f, rBarra.height - 4f), texOverlayOscuro);
            }

            // Interacción directa con el ratón sobre la barra
            Event ev = Event.current;
            if ((ev.type == EventType.MouseDown || ev.type == EventType.MouseDrag) && rBarra.Contains(ev.mousePosition))
            {
                nuevoValor = Mathf.Clamp01((ev.mousePosition.x - rBarra.x) / rBarra.width);
                ev.Use();
            }

            if (BotonColonial(rMas, "+", VarianteBoton.Madera, 15))
            {
                nuevoValor = Mathf.Clamp01(Mathf.Round((nuevoValor + 0.10f) * 10f) / 10f);
            }

            // Insignia de porcentaje
            GUI.DrawTexture(rPct, texBarraFondo);
            GUI.Label(rPct, $"{Mathf.RoundToInt(nuevoValor * 100f)}%", estiloPorcentaje);

            return nuevoValor;
        }

        /// <summary>
        /// Dibuja una fila de interruptor estilizado para Pantalla Completa / Modo Ventana.
        /// </summary>
        public static bool DibujarInterruptorPantallaCompleta(Rect rectFila, bool estadoActual)
        {
            AsegurarInicializacion();
            GUI.DrawTexture(rectFila, texTarjetaInterior);

            Rect rLabel = new Rect(rectFila.x + 10f, rectFila.y, rectFila.width * 0.52f, rectFila.height);
            GUI.Label(rLabel, "🖥️ Modo de Visualización:", estiloEtiquetaSeccion);

            float wBtn = Mathf.Min(190f, rectFila.width * 0.42f);
            float hBtn = 28f;
            Rect rBtn = new Rect(rectFila.xMax - wBtn - 10f, rectFila.y + (rectFila.height - hBtn) * 0.5f, wBtn, hBtn);

            string textoEstado = estadoActual ? "◉ PANTALLA COMPLETA" : "○ MODO VENTANA";
            VarianteBoton variante = estadoActual ? VarianteBoton.Verde : VarianteBoton.Talavera;

            if (BotonColonial(rBtn, textoEstado, variante, 12))
            {
                return !estadoActual;
            }
            return estadoActual;
        }

        /// <summary>
        /// Modal unificado y profesional de Opciones de Juego (usado en Menú Principal, Nivel 1 y Nivel 2).
        /// </summary>
        public static void DibujarModalOpcionesColonial(
            ref float volumenGeneral,
            ref float volumenMusica,
            ref float volumenSFX,
            ref bool pantallaCompleta,
            Action onCerrar)
        {
            AsegurarInicializacion();
            DibujarOverlayOscuro();

            float ancho = Mathf.Min(500f, Screen.width - 32f);
            float alto = Mathf.Min(356f, Screen.height - 32f);
            Rect rectModal = new Rect((Screen.width - ancho) * 0.5f, (Screen.height - alto) * 0.5f, ancho, alto);

            DibujarMarcoVentana(rectModal, "⚙ AJUSTES Y OPCIONES", "Configuración de Audio Regional y Pantalla");

            float xContenido = rectModal.x + 20f;
            float wContenido = rectModal.width - 40f;
            float yCursor = rectModal.y + 78f;
            float hFila = 42f;
            float espaciado = 8f;

            // 1. Volumen General
            float nuevoGen = DibujarControlVolumenColonial(
                new Rect(xContenido, yCursor, wContenido, hFila),
                "🔊 Volumen General",
                volumenGeneral
            );
            if (Mathf.Abs(nuevoGen - volumenGeneral) > 0.001f)
            {
                volumenGeneral = nuevoGen;
                AudioListener.volume = volumenGeneral;
                if (AudioColonialManager.Instance != null)
                    AudioColonialManager.Instance.AjustarVolumenGeneral(volumenGeneral);
            }
            yCursor += hFila + espaciado;

            // 2. Música Tradicional
            float nuevaMus = DibujarControlVolumenColonial(
                new Rect(xContenido, yCursor, wContenido, hFila),
                "🎸 Música Tradicional",
                volumenMusica
            );
            if (Mathf.Abs(nuevaMus - volumenMusica) > 0.001f)
            {
                volumenMusica = nuevaMus;
                if (AudioColonialManager.Instance != null)
                    AudioColonialManager.Instance.AjustarVolumenMusica(volumenMusica);
            }
            yCursor += hFila + espaciado;

            // 3. Efectos de Sonido
            float nuevoSFX = DibujarControlVolumenColonial(
                new Rect(xContenido, yCursor, wContenido, hFila),
                "🔔 Efectos de Sonido",
                volumenSFX
            );
            if (Mathf.Abs(nuevoSFX - volumenSFX) > 0.001f)
            {
                volumenSFX = nuevoSFX;
                if (AudioColonialManager.Instance != null)
                    AudioColonialManager.Instance.AjustarVolumenSFX(volumenSFX);
            }
            yCursor += hFila + espaciado;

            // 4. Pantalla Completa
            bool nuevaPantalla = DibujarInterruptorPantallaCompleta(
                new Rect(xContenido, yCursor, wContenido, hFila),
                pantallaCompleta
            );
            if (nuevaPantalla != pantallaCompleta)
            {
                pantallaCompleta = nuevaPantalla;
                Screen.fullScreen = pantallaCompleta;
            }

            // Botón inferior Guardar y Volver
            float wBoton = 220f;
            float hBoton = 38f;
            Rect rGuardar = new Rect(
                rectModal.x + (rectModal.width - wBoton) * 0.5f,
                rectModal.yMax - hBoton - 16f,
                wBoton,
                hBoton
            );

            if (BotonColonial(rGuardar, "✓ GUARDAR Y VOLVER", VarianteBoton.Dorado, 13))
            {
                onCerrar?.Invoke();
            }
        }

        /// <summary>
        /// Menú de Pausa unificado con estética Colonial / Talavera para todos los niveles.
        /// </summary>
        public static void DibujarMenuPausaColonial(
            string nombreNivel,
            Action onContinuar,
            Action onAbrirOpciones,
            Action onReiniciar,
            Action onIrOverworld,
            Action onIrMenuPrincipal)
        {
            AsegurarInicializacion();
            DibujarOverlayOscuro();

            float ancho = Mathf.Min(430f, Screen.width - 32f);
            float alto = Mathf.Min(368f, Screen.height - 32f);
            Rect rectModal = new Rect((Screen.width - ancho) * 0.5f, (Screen.height - alto) * 0.5f, ancho, alto);

            DibujarMarcoVentana(rectModal, "⏸ PARTIDA EN PAUSA", nombreNivel);

            float padX = 28f;
            float xBtn = rectModal.x + padX;
            float wBtn = rectModal.width - padX * 2f;
            float hBtn = 40f;
            float espacio = 9f;
            float yCursor = rectModal.y + 80f;

            if (BotonColonial(new Rect(xBtn, yCursor, wBtn, hBtn), "▶ CONTINUAR PARTIDA [ESC]", VarianteBoton.Verde, 14))
            {
                onContinuar?.Invoke();
            }
            yCursor += hBtn + espacio;

            if (BotonColonial(new Rect(xBtn, yCursor, wBtn, hBtn), "⚙ OPCIONES DE AUDIO Y PANTALLA", VarianteBoton.Talavera, 13))
            {
                onAbrirOpciones?.Invoke();
            }
            yCursor += hBtn + espacio;

            if (BotonColonial(new Rect(xBtn, yCursor, wBtn, hBtn), "🔄 REINICIAR MISIÓN", VarianteBoton.Madera, 13))
            {
                onReiniciar?.Invoke();
            }
            yCursor += hBtn + espacio;

            if (BotonColonial(new Rect(xBtn, yCursor, wBtn, hBtn), "🗺️ VOLVER AL MAPA REGIONAL", VarianteBoton.Madera, 13))
            {
                onIrOverworld?.Invoke();
            }
            yCursor += hBtn + espacio;

            if (BotonColonial(new Rect(xBtn, yCursor, wBtn, hBtn), "🏠 SALIR AL MENÚ PRINCIPAL", VarianteBoton.Rojo, 13))
            {
                onIrMenuPrincipal?.Invoke();
            }
        }

        private static Texture2D ObtenerTexturaBoton(VarianteBoton variante, bool hover)
        {
            switch (variante)
            {
                case VarianteBoton.Dorado:
                    return hover ? texBtnDoradoHover : texBtnDorado;
                case VarianteBoton.Verde:
                    return hover ? texBtnVerdeHover : texBtnVerde;
                case VarianteBoton.Talavera:
                    return hover ? texBtnTalaveraHover : texBtnTalavera;
                case VarianteBoton.Rojo:
                    return hover ? texBtnRojoHover : texBtnRojo;
                case VarianteBoton.Gris:
                    return texBtnGris;
                default:
                    return hover ? texBtnMaderaHover : texBtnMadera;
            }
        }

        private static Texture2D CrearTexturaSolida(Color color)
        {
            Texture2D tex = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            tex.SetPixel(0, 0, color);
            tex.Apply();
            return tex;
        }

        private static Texture2D CrearTexturaGradienteVertical(int alto, Color colorAbajo, Color colorArriba)
        {
            Texture2D tex = new Texture2D(4, alto, TextureFormat.RGBA32, false);
            tex.wrapMode = TextureWrapMode.Clamp;
            for (int y = 0; y < alto; y++)
            {
                float t = (float)y / Mathf.Max(1, alto - 1);
                Color c = Color.Lerp(colorAbajo, colorArriba, t);
                for (int x = 0; x < 4; x++)
                    tex.SetPixel(x, y, c);
            }
            tex.Apply();
            return tex;
        }

        private static Texture2D CrearTexturaMarcoSimple(int w, int h, Color relleno, Color borde, int grosor)
        {
            Texture2D tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
            tex.wrapMode = TextureWrapMode.Clamp;
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    bool esBorde = x < grosor || x >= w - grosor || y < grosor || y >= h - grosor;
                    tex.SetPixel(x, y, esBorde ? borde : relleno);
                }
            }
            tex.Apply();
            return tex;
        }

        private static Texture2D CrearTexturaMarcoColonial(int w, int h, Color fondo, Color oro, Color bisel, Color esquinaTalavera)
        {
            Texture2D tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
            tex.wrapMode = TextureWrapMode.Clamp;

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    int distX = Mathf.Min(x, w - 1 - x);
                    int distY = Mathf.Min(y, h - 1 - y);
                    int distMin = Mathf.Min(distX, distY);

                    Color c = fondo;

                    // Marco exterior de oro viejo (3px)
                    if (distMin < 3)
                    {
                        c = oro;
                    }
                    // Bisel oscuro intermedio (2px)
                    else if (distMin < 5)
                    {
                        c = bisel;
                    }
                    // Segundo filete dorado fino interior (1px)
                    else if (distMin == 5)
                    {
                        c = new Color(oro.r * 0.85f, oro.g * 0.85f, oro.b * 0.85f, 0.95f);
                    }

                    // Adorno de esquina estilo azulejo de Talavera
                    if (distX >= 3 && distX <= 9 && distY >= 3 && distY <= 9)
                    {
                        if (distX == 9 || distY == 9)
                            c = oro;
                        else
                            c = esquinaTalavera;
                    }

                    tex.SetPixel(x, y, c);
                }
            }

            tex.Apply();
            return tex;
        }

        private static Texture2D CrearTexturaBoton(Color colorAbajo, Color colorArriba, Color colorBorde)
        {
            int w = 48;
            int h = 32;
            Texture2D tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
            tex.wrapMode = TextureWrapMode.Clamp;

            for (int y = 0; y < h; y++)
            {
                float t = (float)y / (h - 1);
                Color grad = Color.Lerp(colorAbajo, colorArriba, t);

                for (int x = 0; x < w; x++)
                {
                    int distX = Mathf.Min(x, w - 1 - x);
                    int distY = Mathf.Min(y, h - 1 - y);
                    int distMin = Mathf.Min(distX, distY);

                    if (distMin < 2)
                    {
                        tex.SetPixel(x, y, colorBorde);
                    }
                    else if (distMin == 2)
                    {
                        // Bisel interior oscuro/iluminado
                        tex.SetPixel(x, y, y > h / 2
                            ? Color.Lerp(grad, Color.white, 0.18f)
                            : Color.Lerp(grad, Color.black, 0.25f));
                    }
                    else
                    {
                        tex.SetPixel(x, y, grad);
                    }
                }
            }
            tex.Apply();
            return tex;
        }
    }
}
