using System;
using UnityEngine;
using CaminosDeGuanajuato.Core;

namespace CaminosDeGuanajuato.UI
{
    /// <summary>
    /// Sistema compartido de interfaz de usuario con estética Colonial / Talavera / Virreinal.
    /// Dibuja marcos con bordes exactos al píxel (sin deformación por estiramiento) y botones
    /// artesanales sin el fondo gris por defecto de Unity 6.
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

        // Texturas sólidas y gradientes para dibujo exacto al píxel
        private static Texture2D texOverlayOscuro;
        private static Texture2D texFondoCaoba;
        private static Texture2D texFondoCabecera;
        private static Texture2D texFondoTarjeta;
        private static Texture2D texBordeOro;
        private static Texture2D texBordeOroSuave;
        private static Texture2D texBiselOscuro;
        private static Texture2D texEsquinaTalavera;

        // Texturas de relleno de botones (Normal y Hover)
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

        // Texturas de barras
        private static Texture2D texBarraFondo;
        private static Texture2D texBarraRellenoOro;

        // Estilos tipográficos puros (sin depender de GUI.skin para evitar ArgumentException y scaledBackgrounds)
        private static GUIStyle estiloTituloVentana;
        private static GUIStyle estiloSubtituloVentana;
        private static GUIStyle estiloEtiquetaSeccion;
        private static GUIStyle estiloTextoNormal;
        private static GUIStyle estiloPorcentaje;
        private static GUIStyle estiloTextoBoton;

        public static void AsegurarInicializacion()
        {
            if (inicializado && texFondoCaoba != null && texBtnMadera != null && estiloTituloVentana != null)
                return;

            CrearTexturasColoniales();
            CrearEstilosColoniales();
            inicializado = true;
        }

        private static void CrearTexturasColoniales()
        {
            texOverlayOscuro = CrearTexturaSolida(new Color(0.03f, 0.02f, 0.01f, 0.78f));
            texFondoCaoba = CrearTexturaSolida(new Color(0.14f, 0.07f, 0.04f, 0.97f));
            texFondoCabecera = CrearTexturaGradienteVertical(32, new Color(0.20f, 0.09f, 0.04f, 0.98f), new Color(0.30f, 0.14f, 0.07f, 0.98f));
            texFondoTarjeta = CrearTexturaSolida(new Color(0.19f, 0.10f, 0.06f, 0.94f));

            texBordeOro = CrearTexturaSolida(new Color(0.90f, 0.70f, 0.22f, 1.0f));
            texBordeOroSuave = CrearTexturaSolida(new Color(0.68f, 0.48f, 0.16f, 0.92f));
            texBiselOscuro = CrearTexturaSolida(new Color(0.08f, 0.03f, 0.01f, 0.98f));
            texEsquinaTalavera = CrearTexturaSolida(new Color(0.15f, 0.38f, 0.68f, 1.0f));

            texBtnMadera = CrearTexturaGradienteVertical(32, new Color(0.26f, 0.11f, 0.05f, 0.98f), new Color(0.38f, 0.17f, 0.08f, 0.98f));
            texBtnMaderaHover = CrearTexturaGradienteVertical(32, new Color(0.44f, 0.20f, 0.09f, 1f), new Color(0.60f, 0.28f, 0.12f, 1f));

            texBtnDorado = CrearTexturaGradienteVertical(32, new Color(0.74f, 0.48f, 0.10f, 0.98f), new Color(0.92f, 0.68f, 0.18f, 0.98f));
            texBtnDoradoHover = CrearTexturaGradienteVertical(32, new Color(0.86f, 0.58f, 0.14f, 1f), new Color(1.00f, 0.80f, 0.28f, 1f));

            texBtnVerde = CrearTexturaGradienteVertical(32, new Color(0.11f, 0.40f, 0.19f, 0.98f), new Color(0.18f, 0.56f, 0.28f, 0.98f));
            texBtnVerdeHover = CrearTexturaGradienteVertical(32, new Color(0.16f, 0.52f, 0.25f, 1f), new Color(0.25f, 0.70f, 0.36f, 1f));

            texBtnTalavera = CrearTexturaGradienteVertical(32, new Color(0.10f, 0.27f, 0.50f, 0.98f), new Color(0.16f, 0.39f, 0.68f, 0.98f));
            texBtnTalaveraHover = CrearTexturaGradienteVertical(32, new Color(0.15f, 0.36f, 0.66f, 1f), new Color(0.24f, 0.52f, 0.84f, 1f));

            texBtnRojo = CrearTexturaGradienteVertical(32, new Color(0.46f, 0.11f, 0.09f, 0.98f), new Color(0.64f, 0.18f, 0.14f, 0.98f));
            texBtnRojoHover = CrearTexturaGradienteVertical(32, new Color(0.58f, 0.15f, 0.12f, 1f), new Color(0.78f, 0.24f, 0.18f, 1f));

            texBtnGris = CrearTexturaGradienteVertical(32, new Color(0.20f, 0.18f, 0.16f, 0.92f), new Color(0.28f, 0.25f, 0.22f, 0.92f));

            texBarraFondo = CrearTexturaSolida(new Color(0.08f, 0.04f, 0.02f, 0.96f));
            texBarraRellenoOro = CrearTexturaGradienteVertical(16, new Color(0.78f, 0.52f, 0.12f, 1f), new Color(0.99f, 0.80f, 0.25f, 1f));

            CrearTexturasGloboDialogoVN();
        }

        private static void CrearEstilosColoniales()
        {
            estiloTituloVentana = new GUIStyle
            {
                fontSize = 19,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                richText = true
            };
            estiloTituloVentana.normal.textColor = new Color(0.99f, 0.82f, 0.30f);

            estiloSubtituloVentana = new GUIStyle
            {
                fontSize = 12,
                fontStyle = FontStyle.Italic,
                alignment = TextAnchor.MiddleCenter,
                richText = true
            };
            estiloSubtituloVentana.normal.textColor = new Color(0.90f, 0.82f, 0.68f);

            estiloEtiquetaSeccion = new GUIStyle
            {
                fontSize = 13,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleLeft,
                richText = true
            };
            estiloEtiquetaSeccion.normal.textColor = new Color(0.98f, 0.88f, 0.68f);

            estiloTextoNormal = new GUIStyle
            {
                fontSize = 13,
                fontStyle = FontStyle.Normal,
                alignment = TextAnchor.MiddleLeft,
                wordWrap = true,
                richText = true
            };
            estiloTextoNormal.normal.textColor = new Color(0.94f, 0.90f, 0.82f);

            estiloPorcentaje = new GUIStyle
            {
                fontSize = 12,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
            estiloPorcentaje.normal.textColor = new Color(1.0f, 0.86f, 0.35f);

            estiloTextoBoton = new GUIStyle
            {
                fontSize = 13,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                richText = true
            };
        }

        public static void DibujarOverlayOscuro()
        {
            AsegurarInicializacion();
            if (texOverlayOscuro != null)
            {
                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), texOverlayOscuro);
            }
        }

        /// <summary>
        /// Dibuja un borde rectangular exacto al píxel sin deformarse al escalar.
        /// </summary>
        public static void DibujarBordeRect(Rect r, float grosor, Texture2D texBorde)
        {
            if (texBorde == null) return;
            // Arriba
            GUI.DrawTexture(new Rect(r.x, r.y, r.width, grosor), texBorde);
            // Abajo
            GUI.DrawTexture(new Rect(r.x, r.yMax - grosor, r.width, grosor), texBorde);
            // Izquierda
            GUI.DrawTexture(new Rect(r.x, r.y + grosor, grosor, Mathf.Max(0f, r.height - grosor * 2f)), texBorde);
            // Derecha
            GUI.DrawTexture(new Rect(r.xMax - grosor, r.y + grosor, grosor, Mathf.Max(0f, r.height - grosor * 2f)), texBorde);
        }

        public static void DibujarMarcoVentana(Rect rect)
        {
            DibujarMarcoVentana(rect, null, null);
        }

        public static void DibujarMarcoVentana(Rect rect, string titulo, string subtitulo = null)
        {
            AsegurarInicializacion();

            // Sombra proyectada
            GUI.DrawTexture(new Rect(rect.x + 6f, rect.y + 6f, rect.width, rect.height), texOverlayOscuro);

            // Relleno principal caoba
            GUI.DrawTexture(rect, texFondoCaoba);

            // Doble marco dorado exacto al píxel (3px exterior + 2px bisel + 1px filete interior)
            DibujarBordeRect(rect, 3f, texBordeOro);
            DibujarBordeRect(new Rect(rect.x + 3f, rect.y + 3f, rect.width - 6f, rect.height - 6f), 2f, texBiselOscuro);
            DibujarBordeRect(new Rect(rect.x + 5f, rect.y + 5f, rect.width - 10f, rect.height - 10f), 1f, texBordeOroSuave);

            // Pequeños remates de Talavera en las 4 esquinas (8x8 píxeles fijos, nunca deformados)
            DibujarRemateEsquina(rect.x + 5f, rect.y + 5f);
            DibujarRemateEsquina(rect.xMax - 13f, rect.y + 5f);
            DibujarRemateEsquina(rect.x + 5f, rect.yMax - 13f);
            DibujarRemateEsquina(rect.xMax - 13f, rect.yMax - 13f);

            if (!string.IsNullOrEmpty(titulo))
            {
                // Cabecera interna
                float hCabecera = string.IsNullOrEmpty(subtitulo) ? 40f : 54f;
                Rect rectCabecera = new Rect(rect.x + 14f, rect.y + 12f, rect.width - 28f, hCabecera);
                GUI.DrawTexture(rectCabecera, texFondoCabecera);
                DibujarBordeRect(rectCabecera, 1f, texBordeOroSuave);

                Rect rectTitulo = new Rect(rectCabecera.x, rectCabecera.y + 4f, rectCabecera.width, 26f);
                DibujarTextoConSombra(rectTitulo, titulo, estiloTituloVentana);

                if (!string.IsNullOrEmpty(subtitulo))
                {
                    Rect rectSub = new Rect(rectCabecera.x, rectCabecera.y + 28f, rectCabecera.width, 20f);
                    GUI.Label(rectSub, subtitulo, estiloSubtituloVentana);
                }

                // Línea divisoria dorada bajo la cabecera
                GUI.DrawTexture(new Rect(rect.x + 18f, rectCabecera.yMax + 4f, rect.width - 36f, 2f), texBordeOro);
            }
        }

        private static void DibujarRemateEsquina(float x, float y)
        {
            GUI.DrawTexture(new Rect(x, y, 8f, 8f), texEsquinaTalavera);
            DibujarBordeRect(new Rect(x, y, 8f, 8f), 1f, texBordeOro);
        }

        public static void DibujarTarjetaInterior(Rect rect)
        {
            AsegurarInicializacion();
            GUI.DrawTexture(rect, texFondoTarjeta);
            DibujarBordeRect(rect, 1f, texBordeOroSuave);
        }

        public static void DibujarSeparador(float x, float y, float ancho)
        {
            AsegurarInicializacion();
            GUI.DrawTexture(new Rect(x, y, ancho, 1.5f), texBordeOroSuave);
        }

        public static void DibujarTextoConSombra(Rect rect, string texto, GUIStyle estilo)
        {
            Color prev = estilo.normal.textColor;
            estilo.normal.textColor = new Color(0.04f, 0.01f, 0.01f, 0.92f);
            GUI.Label(new Rect(rect.x + 1f, rect.y + 1f, rect.width, rect.height), texto, estilo);
            estilo.normal.textColor = prev;
            GUI.Label(rect, texto, estilo);
        }

        /// <summary>
        /// Dibuja un botón artesanal con textura propia y borde dorado exacto al píxel,
        /// usando GUIStyle.none para evitar que Unity 6 superponga el botón gris predeterminado.
        /// </summary>
        public static bool BotonColonial(Rect rect, string texto, VarianteBoton variante = VarianteBoton.Madera, int fontSize = 13)
        {
            AsegurarInicializacion();

            bool hover = rect.Contains(Event.current.mousePosition);
            Texture2D texFondo = ObtenerTexturaBoton(variante, hover);

            // Sombra sutil
            GUI.DrawTexture(new Rect(rect.x + 2f, rect.y + 2f, rect.width, rect.height), texOverlayOscuro);

            // Relleno con gradiente colonial
            if (texFondo != null)
            {
                GUI.DrawTexture(rect, texFondo);
            }

            // Borde dorado nítido (iluminado en hover)
            Texture2D texBorde = (variante == VarianteBoton.Gris)
                ? texBiselOscuro
                : (hover ? texBordeOro : texBordeOroSuave);
            DibujarBordeRect(rect, hover ? 2f : 1.5f, texBorde);

            // Texto del botón con protección contra corte (auto-escalado)
            int tamFinal = fontSize;
            float anchoEstimado = texto.Length * (fontSize * 0.58f);
            if (anchoEstimado > rect.width - 16f)
            {
                tamFinal = Mathf.Max(10, Mathf.FloorToInt(fontSize * ((rect.width - 16f) / anchoEstimado)));
            }
            estiloTextoBoton.fontSize = tamFinal;
            estiloTextoBoton.clipping = TextClipping.Overflow;
            estiloTextoBoton.wordWrap = false;

            if (variante == VarianteBoton.Dorado)
            {
                estiloTextoBoton.normal.textColor = new Color(0.16f, 0.06f, 0.01f, 1f);
                GUI.Label(rect, texto, estiloTextoBoton);
            }
            else if (variante == VarianteBoton.Gris)
            {
                estiloTextoBoton.normal.textColor = new Color(0.70f, 0.66f, 0.60f, 0.95f);
                GUI.Label(rect, texto, estiloTextoBoton);
            }
            else
            {
                estiloTextoBoton.normal.textColor = new Color(0.05f, 0.02f, 0.01f, 0.90f);
                GUI.Label(new Rect(rect.x + 1f, rect.y + 1f, rect.width, rect.height), texto, estiloTextoBoton);

                estiloTextoBoton.normal.textColor = hover
                    ? new Color(1.0f, 0.96f, 0.78f, 1f)
                    : new Color(0.98f, 0.92f, 0.78f, 1f);
                GUI.Label(rect, texto, estiloTextoBoton);
            }

            bool clic = GUI.Button(rect, GUIContent.none, GUIStyle.none);
            if (clic && variante != VarianteBoton.Gris && AudioColonialManager.Instance != null)
            {
                AudioColonialManager.Instance.ReproducirClickBoton();
            }
            return clic;
        }

        public static void DibujarBarraProgresoColonial(Rect rect, float progreso01, string textoCentro = "")
        {
            AsegurarInicializacion();
            float p = Mathf.Clamp01(progreso01);
            GUI.DrawTexture(rect, texBarraFondo);
            DibujarBordeRect(rect, 1f, texBordeOroSuave);

            float anchoRelleno = (rect.width - 4f) * p;
            if (anchoRelleno > 1f)
            {
                GUI.DrawTexture(new Rect(rect.x + 2f, rect.y + 2f, anchoRelleno, rect.height - 4f), texBarraRellenoOro);
            }

            if (!string.IsNullOrEmpty(textoCentro))
            {
                GUIStyle stTxt = new GUIStyle
                {
                    fontSize = 11,
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleCenter,
                    normal = { textColor = Color.white }
                };
                GUI.Label(rect, textoCentro, stTxt);
            }
        }

        public static float DibujarControlVolumenColonial(Rect rectFila, string etiqueta, float valorActual)
        {
            AsegurarInicializacion();
            DibujarTarjetaInterior(rectFila);

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

            GUI.DrawTexture(rBarra, texBarraFondo);
            DibujarBordeRect(rBarra, 1f, texBordeOroSuave);

            float anchoRelleno = Mathf.Clamp01(nuevoValor) * (rBarra.width - 4f);
            if (anchoRelleno > 1f)
            {
                GUI.DrawTexture(new Rect(rBarra.x + 2f, rBarra.y + 2f, anchoRelleno, rBarra.height - 4f), texBarraRellenoOro);
            }

            for (int i = 1; i <= 3; i++)
            {
                float xDiv = rBarra.x + (rBarra.width * (i * 0.25f));
                GUI.DrawTexture(new Rect(xDiv, rBarra.y + 2f, 1f, rBarra.height - 4f), texBiselOscuro);
            }

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

            GUI.DrawTexture(rPct, texBarraFondo);
            DibujarBordeRect(rPct, 1f, texBordeOroSuave);
            GUI.Label(rPct, $"{Mathf.RoundToInt(nuevoValor * 100f)}%", estiloPorcentaje);

            return nuevoValor;
        }

        public static bool DibujarInterruptorPantallaCompleta(Rect rectFila, bool estadoActual)
        {
            AsegurarInicializacion();
            DibujarTarjetaInterior(rectFila);

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

            bool nuevaPantalla = DibujarInterruptorPantallaCompleta(
                new Rect(xContenido, yCursor, wContenido, hFila),
                pantallaCompleta
            );
            if (nuevaPantalla != pantallaCompleta)
            {
                pantallaCompleta = nuevaPantalla;
                Screen.fullScreen = pantallaCompleta;
            }

            float wBoton = 220f;
            float hBoton = 36f;
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
            float alto = Mathf.Min(364f, Screen.height - 32f);
            Rect rectModal = new Rect((Screen.width - ancho) * 0.5f, (Screen.height - alto) * 0.5f, ancho, alto);

            DibujarMarcoVentana(rectModal, "⏸ PARTIDA EN PAUSA", nombreNivel);

            float padX = 28f;
            float xBtn = rectModal.x + padX;
            float wBtn = rectModal.width - padX * 2f;
            float hBtn = 38f;
            float espacio = 9f;
            float yCursor = rectModal.y + 80f;

            if (BotonColonial(new Rect(xBtn, yCursor, wBtn, hBtn), "▶ CONTINUAR PARTIDA [ESC]", VarianteBoton.Verde, 13))
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

        #region Sistema de Diálogo Interactivo Estilo Novela Visual (Globo de Voz con Cola y Retratos)

        public struct LineaDialogoVN
        {
            public string nombreHablante;
            public string texto;
            public bool hablaIzquierda;
            public string pistaVisualExtra; // Pista opcional debajo del globo (ej. teclas de colores o pasos)
            public float pitchVoz;

            public LineaDialogoVN(string nombre, string texto, bool izquierda, string pistaExtra = "", float pitch = 1.0f)
            {
                this.nombreHablante = nombre;
                this.texto = texto;
                this.hablaIzquierda = izquierda;
                this.pistaVisualExtra = pistaExtra;
                this.pitchVoz = pitch;
            }
        }

        private static Texture2D texGloboNegroRedondeado;
        private static Texture2D texPlacaNombreBlancaIzq;
        private static Texture2D texPlacaNombreBlancaDer;
        private static Texture2D texOverlayDialogoSuave;

        private static int indiceLineaActualVN = -1;
        private static float tiempoInicioLineaVN = 0f;
        private static int ultimoConteoCaracteresSonido = 0;
        private static int frameUltimoAvanceVN = -1;

        public static void ReiniciarSecuenciaDialogoVN()
        {
            indiceLineaActualVN = 0;
            tiempoInicioLineaVN = Time.unscaledTime;
            ultimoConteoCaracteresSonido = 0;
            frameUltimoAvanceVN = Time.frameCount;
        }

        private static void CrearTexturasGloboDialogoVN()
        {
            texOverlayDialogoSuave = CrearTexturaSolida(new Color(0.02f, 0.02f, 0.04f, 0.48f));

            // 1. Textura de globo oscuro con esquinas redondeadas y borde sutil
            int gw = 256, gh = 128, radio = 18;
            texGloboNegroRedondeado = new Texture2D(gw, gh, TextureFormat.RGBA32, false);
            texGloboNegroRedondeado.wrapMode = TextureWrapMode.Clamp;
            Color colFondoGlobo = new Color(0.04f, 0.04f, 0.06f, 0.95f);
            Color colBordeGlobo = new Color(0.28f, 0.28f, 0.34f, 0.95f);

            for (int y = 0; y < gh; y++)
            {
                for (int x = 0; x < gw; x++)
                {
                    int dx = 0, dy = 0;
                    if (x < radio) dx = radio - x;
                    else if (x >= gw - radio) dx = x - (gw - 1 - radio);

                    if (y < radio) dy = radio - y;
                    else if (y >= gh - radio) dy = y - (gh - 1 - radio);

                    float dist = Mathf.Sqrt(dx * dx + dy * dy);
                    if (dist > radio + 0.5f)
                    {
                        texGloboNegroRedondeado.SetPixel(x, y, Color.clear);
                    }
                    else if (dist > radio - 1.8f || x <= 1 || x >= gw - 2 || y <= 1 || y >= gh - 2)
                    {
                        texGloboNegroRedondeado.SetPixel(x, y, colBordeGlobo);
                    }
                    else
                    {
                        texGloboNegroRedondeado.SetPixel(x, y, colFondoGlobo);
                    }
                }
            }
            texGloboNegroRedondeado.Apply();

            // 2. Placa de nombre inclinada blanca con cola tipo relámpago/bocadillo (Izquierda y Derecha)
            texPlacaNombreBlancaIzq = GenerarTexturaPlacaConCola(invertirHorizontal: false);
            texPlacaNombreBlancaDer = GenerarTexturaPlacaConCola(invertirHorizontal: true);
        }

        private static Texture2D GenerarTexturaPlacaConCola(bool invertirHorizontal)
        {
            int pw = 220, ph = 64;
            Texture2D tex = new Texture2D(pw, ph, TextureFormat.RGBA32, false);
            tex.wrapMode = TextureWrapMode.Clamp;
            Color blancoMarfil = new Color(0.98f, 0.98f, 0.99f, 1.0f);
            Color negroBorde = new Color(0.05f, 0.05f, 0.07f, 1.0f);

            for (int y = 0; y < ph; y++)
            {
                for (int x = 0; x < pw; x++)
                {
                    int lx = invertirHorizontal ? (pw - 1 - x) : x;
                    bool dentroPlaca = false;
                    bool esBorde = false;

                    // Parte superior (y: 24..60): paralelogramo inclinado estilo etiqueta "Felix"
                    if (y >= 24 && y <= 60)
                    {
                        float inclinacion = (y - 24) * 0.42f;
                        float xMin = 42f + inclinacion;
                        float xMax = 212f + inclinacion * 0.6f;
                        if (lx >= xMin && lx <= xMax)
                        {
                            dentroPlaca = true;
                            if (lx <= xMin + 2.5f || lx >= xMax - 2.5f || y <= 26 || y >= 58)
                                esBorde = true;
                        }
                    }

                    // Cola diagonal en zig-zag hacia la boca/pecho del personaje (y: 2..32, lx: 4..66)
                    if (y >= 4 && y <= 34)
                    {
                        float tCola = (float)(y - 4) / 30f; // 0 en la punta inferior izquierda, 1 arriba en la unión
                        float centroColaX = Mathf.Lerp(6f, 56f, tCola);
                        // Pequeño quiebre de rayo de cómic a mitad de la cola
                        if (tCola > 0.42f && tCola < 0.62f) centroColaX -= 6f;

                        float grosorMitad = Mathf.Lerp(1.5f, 13f, tCola);
                        if (lx >= centroColaX - grosorMitad && lx <= centroColaX + grosorMitad)
                        {
                            dentroPlaca = true;
                            if (lx <= centroColaX - grosorMitad + 2.2f || lx >= centroColaX + grosorMitad - 2.2f || y <= 5)
                            {
                                // No poner borde negro en la unión interna con la placa blanca
                                if (y < 25) esBorde = true;
                            }
                        }
                    }

                    if (!dentroPlaca)
                        tex.SetPixel(x, y, Color.clear);
                    else if (esBorde)
                        tex.SetPixel(x, y, negroBorde);
                    else
                        tex.SetPixel(x, y, blancoMarfil);
                }
            }
            tex.Apply();
            return tex;
        }

        /// <summary>
        /// Dibuja una conversación interactiva paso a paso con retratos grandes a izquierda/derecha,
        /// globo oscuro redondeado al centro, etiqueta blanca inclinada con cola apuntando al hablante
        /// y texto con efecto máquina de escribir. Devuelve true mientras el diálogo sigue activo,
        /// y false cuando el jugador termina la última frase o pulsa Saltar.
        /// </summary>
        public static bool DibujarDialogoInteractivoVN(
            LineaDialogoVN[] lineas,
            Texture2D retratoIzquierda,
            Texture2D retratoDerecha,
            ref int indiceLinea,
            Action onTerminarDialogo)
        {
            AsegurarInicializacion();
            if (texGloboNegroRedondeado == null) CrearTexturasGloboDialogoVN();

            if (lineas == null || lineas.Length == 0 || indiceLinea >= lineas.Length)
            {
                onTerminarDialogo?.Invoke();
                return false;
            }

            if (indiceLinea < 0)
            {
                indiceLinea = 0;
                indiceLineaActualVN = 0;
                tiempoInicioLineaVN = Time.unscaledTime;
                ultimoConteoCaracteresSonido = 0;
            }

            if (indiceLinea != indiceLineaActualVN)
            {
                indiceLineaActualVN = indiceLinea;
                tiempoInicioLineaVN = Time.unscaledTime;
                ultimoConteoCaracteresSonido = 0;
            }

            LineaDialogoVN actual = lineas[indiceLinea];

            // 1. Fondo traslúcido suave que deja ver el escenario detrás (como en la imagen de referencia)
            if (texOverlayDialogoSuave != null)
            {
                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), texOverlayDialogoSuave);
            }

            // Calcular cuántos caracteres visibles lleva el efecto máquina de escribir (~46 chars/seg)
            string textoPlanoSinTags = QuitarEtiquetasRichText(actual.texto);
            int totalCharsVisibles = textoPlanoSinTags.Length;
            float tiempoTranscurrido = Time.unscaledTime - tiempoInicioLineaVN;
            int charsMostrados = Mathf.Clamp(Mathf.FloorToInt(tiempoTranscurrido * 46f), 0, totalCharsVisibles);
            bool fraseCompletada = (charsMostrados >= totalCharsVisibles);

            // Reproducir suave blip de voz al aparecer nuevas letras
            if (!fraseCompletada && charsMostrados > ultimoConteoCaracteresSonido)
            {
                ultimoConteoCaracteresSonido = charsMostrados;
                if (AudioColonialManager.Instance != null)
                {
                    AudioColonialManager.Instance.ReproducirBlipDialogo(actual.pitchVoz);
                }
            }

            // Animación sutil de rebote para el retrato que está hablando
            float reboteHabla = (!fraseCompletada) ? Mathf.Abs(Mathf.Sin(Time.unscaledTime * 14f)) * 7f : Mathf.Sin(Time.unscaledTime * 3f) * 3f;

            // 2. Retratos laterales grandes en primer plano (Izquierda y Derecha)
            float altoRetrato = Mathf.Clamp(Screen.height * 0.72f, 280f, 560f);
            float anchoRetrato = altoRetrato * 0.56f;
            float yBaseRetratos = Screen.height - altoRetrato + 14f;

            Color colorAntiguo = GUI.color;

            if (retratoIzquierda != null)
            {
                float offsetY = actual.hablaIzquierda ? -reboteHabla : 10f;
                float escala = actual.hablaIzquierda ? 1.02f : 0.94f;
                GUI.color = actual.hablaIzquierda ? Color.white : new Color(0.52f, 0.52f, 0.58f, 0.92f);
                Rect rIzq = new Rect(
                    6f,
                    yBaseRetratos + offsetY,
                    anchoRetrato * escala,
                    altoRetrato * escala
                );
                GUI.DrawTexture(rIzq, retratoIzquierda, ScaleMode.ScaleToFit);
            }

            if (retratoDerecha != null)
            {
                float offsetY = !actual.hablaIzquierda ? -reboteHabla : 10f;
                float escala = !actual.hablaIzquierda ? 1.02f : 0.94f;
                GUI.color = !actual.hablaIzquierda ? Color.white : new Color(0.52f, 0.52f, 0.58f, 0.92f);
                float wDer = anchoRetrato * escala;
                Rect rDer = new Rect(
                    Screen.width - wDer - 6f,
                    yBaseRetratos + offsetY,
                    wDer,
                    altoRetrato * escala
                );
                GUI.DrawTexture(rDer, retratoDerecha, ScaleMode.ScaleToFit);
            }

            GUI.color = colorAntiguo;

            // 3. Preparar estilos y medir alturas dinámicas para que ningún texto se encime ni se salga
            GUIStyle estiloTextoGlobo = new GUIStyle
            {
                fontSize = 15,
                fontStyle = FontStyle.Normal,
                alignment = TextAnchor.UpperLeft,
                wordWrap = true,
                richText = true,
                normal = { textColor = new Color(0.97f, 0.97f, 0.98f, 1f) }
            };

            GUIStyle estiloPista = new GUIStyle
            {
                fontSize = 12,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                wordWrap = true,
                richText = true,
                padding = new RectOffset(10, 10, 5, 5),
                normal = { textColor = new Color(1.0f, 0.90f, 0.40f, 1f) }
            };

            // Ancho del globo centrado entre los dos personajes sin tocar sus manos ni sprites
            float espacioEntrePersonajes = Screen.width - (anchoRetrato * 2.12f) - 24f;
            float anchoGlobo = Mathf.Clamp(espacioEntrePersonajes, 390f, 510f);
            float anchoTextoInterior = anchoGlobo - 48f;
            float anchoCajaPista = anchoGlobo - 40f;

            // Medir la altura real del texto completo (evaluando con y sin etiquetas RichText para margen 100% seguro)
            float altoTextoRich = estiloTextoGlobo.CalcHeight(new GUIContent(actual.texto), anchoTextoInterior);
            float altoTextoPlano = estiloTextoGlobo.CalcHeight(new GUIContent(textoPlanoSinTags), anchoTextoInterior - 16f);
            float altoTextoFinal = Mathf.Max(48f, Mathf.Max(altoTextoRich, altoTextoPlano) + 10f);

            bool tienePista = !string.IsNullOrEmpty(actual.pistaVisualExtra);
            float altoPistaFinal = 0f;
            if (tienePista)
            {
                string pistaPlana = QuitarEtiquetasRichText(actual.pistaVisualExtra);
                float altoPistaRich = estiloPista.CalcHeight(new GUIContent(actual.pistaVisualExtra), anchoCajaPista - 24f);
                float altoPistaPlana = estiloPista.CalcHeight(new GUIContent(pistaPlana), anchoCajaPista - 32f);
                altoPistaFinal = Mathf.Max(36f, Mathf.Max(altoPistaRich, altoPistaPlana) + 12f);
            }

            float padSuperior = 22f;
            float espacioEntreTextoYPista = tienePista ? 10f : 0f;
            float altoZonaPie = 40f;
            float altoGlobo = padSuperior + altoTextoFinal + espacioEntreTextoYPista + altoPistaFinal + altoZonaPie;

            float xGlobo = (Screen.width - anchoGlobo) * 0.5f;
            float yGlobo = Mathf.Clamp(Screen.height * 0.30f, 74f, Screen.height - altoGlobo - 46f);

            Rect rectGlobo = new Rect(xGlobo, yGlobo, anchoGlobo, altoGlobo);
            GUI.DrawTexture(rectGlobo, texGloboNegroRedondeado, ScaleMode.StretchToFill);

            // 4. Etiqueta blanca inclinada con cola apuntando al personaje que habla
            float wPlaca = 200f;
            float hPlaca = 58f;
            Rect rectPlaca;
            Rect rectTextoNombre;

            if (actual.hablaIzquierda)
            {
                rectPlaca = new Rect(rectGlobo.x - 32f, rectGlobo.y - 30f, wPlaca, hPlaca);
                GUI.DrawTexture(rectPlaca, texPlacaNombreBlancaIzq, ScaleMode.StretchToFill);
                rectTextoNombre = new Rect(rectPlaca.x + 44f, rectPlaca.y + 4f, wPlaca - 52f, 30f);
            }
            else
            {
                rectPlaca = new Rect(rectGlobo.xMax - wPlaca + 32f, rectGlobo.y - 30f, wPlaca, hPlaca);
                GUI.DrawTexture(rectPlaca, texPlacaNombreBlancaDer, ScaleMode.StretchToFill);
                rectTextoNombre = new Rect(rectPlaca.x + 10f, rectPlaca.y + 4f, wPlaca - 52f, 30f);
            }

            GUIStyle estiloNombrePlaca = new GUIStyle
            {
                fontSize = 15,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = new Color(0.06f, 0.06f, 0.09f, 1f) }
            };
            GUI.Label(rectTextoNombre, actual.nombreHablante, estiloNombrePlaca);

            // 5. Texto del diálogo dentro del globo oscuro (con recorte progresivo de máquina de escribir)
            string textoRecortado = RecortarRichTextVisibles(actual.texto, charsMostrados);
            Rect rectTextoInterior = new Rect(rectGlobo.x + 24f, rectGlobo.y + padSuperior, anchoTextoInterior, altoTextoFinal);
            GUI.Label(rectTextoInterior, textoRecortado, estiloTextoGlobo);

            // Pista visual extra colocada exactamente debajo del texto principal, con wordWrap activo
            if (tienePista)
            {
                Rect rectPista = new Rect(rectGlobo.x + 20f, rectTextoInterior.yMax + espacioEntreTextoYPista, anchoCajaPista, altoPistaFinal);
                GUI.DrawTexture(rectPista, texFondoTarjeta);
                DibujarBordeRect(rectPista, 1f, texBordeOroSuave);
                GUI.Label(rectPista, actual.pistaVisualExtra, estiloPista);
            }

            // 6. Indicador de paso (ej. "1 / 4") y botón interactivo "Continuar ▶" en la base del globo
            GUIStyle estiloPaso = new GUIStyle
            {
                fontSize = 12,
                fontStyle = FontStyle.Italic,
                alignment = TextAnchor.MiddleLeft,
                normal = { textColor = new Color(0.68f, 0.68f, 0.74f, 1f) }
            };
            GUI.Label(
                new Rect(rectGlobo.x + 24f, rectGlobo.yMax - 30f, 130f, 22f),
                $"Diálogo {indiceLinea + 1} de {lineas.Length}",
                estiloPaso
            );

            bool esUltima = (indiceLinea >= lineas.Length - 1);
            float pulso = 0.82f + 0.18f * Mathf.Sin(Time.unscaledTime * 6f);
            string etiquetaContinuar = !fraseCompletada
                ? "⏩ Mostrar todo [ESPACIO / Clic]"
                : (esUltima ? "⭐ ¡EMPEZAR A JUGAR! [ESPACIO] ▶" : "Continuar [ESPACIO / Clic] ▶");

            GUIStyle estiloPromptContinuar = new GUIStyle
            {
                fontSize = 12,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleRight,
                normal = { textColor = new Color(1f, 0.88f, 0.32f, pulso) }
            };
            Rect rectBtnContinuar = new Rect(rectGlobo.x + 150f, rectGlobo.yMax - 31f, rectGlobo.width - 172f, 24f);
            GUI.Label(rectBtnContinuar, etiquetaContinuar, estiloPromptContinuar);

            // 7. Botón superior derecho para Saltar toda la introducción si ya sabe jugar
            Rect rectBtnSaltar = new Rect(Screen.width - 196f, 16f, 180f, 34f);
            if (BotonColonial(rectBtnSaltar, "⏭ SALTAR GUÍA [TAB]", VarianteBoton.Gris, 12) || Input.GetKeyDown(KeyCode.Tab))
            {
                if (AudioColonialManager.Instance != null) AudioColonialManager.Instance.ReproducirClickBoton();
                indiceLinea = lineas.Length;
                onTerminarDialogo?.Invoke();
                return false;
            }

            // 8. Detección de interacción para avanzar (Tecla Espacio, Enter o Clic Izquierdo fuera del botón Saltar)
            bool clicEnPantalla = Event.current.type == EventType.MouseDown && Event.current.button == 0 && !rectBtnSaltar.Contains(Event.current.mousePosition);
            bool teclaAvance = Event.current.type == EventType.KeyDown &&
                               (Event.current.keyCode == KeyCode.Space || Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter || Event.current.keyCode == KeyCode.RightArrow);

            if ((clicEnPantalla || teclaAvance) && Time.frameCount != frameUltimoAvanceVN)
            {
                frameUltimoAvanceVN = Time.frameCount;
                if (Event.current != null) Event.current.Use();

                if (!fraseCompletada)
                {
                    // Completar la frase actual de inmediato
                    tiempoInicioLineaVN = Time.unscaledTime - 100f;
                }
                else
                {
                    if (AudioColonialManager.Instance != null) AudioColonialManager.Instance.ReproducirClickBoton();
                    indiceLinea++;
                    if (indiceLinea >= lineas.Length)
                    {
                        onTerminarDialogo?.Invoke();
                        return false;
                    }
                    else
                    {
                        indiceLineaActualVN = indiceLinea;
                        tiempoInicioLineaVN = Time.unscaledTime;
                        ultimoConteoCaracteresSonido = 0;
                    }
                }
            }

            return true;
        }

        private static string QuitarEtiquetasRichText(string input)
        {
            if (string.IsNullOrEmpty(input)) return "";
            System.Text.StringBuilder sb = new System.Text.StringBuilder(input.Length);
            bool enTag = false;
            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];
                if (c == '<') enTag = true;
                else if (c == '>') enTag = false;
                else if (!enTag) sb.Append(c);
            }
            return sb.ToString();
        }

        private static string RecortarRichTextVisibles(string input, int maxVisibles)
        {
            if (string.IsNullOrEmpty(input) || maxVisibles <= 0) return "";
            System.Text.StringBuilder sb = new System.Text.StringBuilder(input.Length);
            System.Collections.Generic.Stack<string> tagsAbiertos = new System.Collections.Generic.Stack<string>();
            int visibles = 0;
            int i = 0;

            while (i < input.Length && visibles < maxVisibles)
            {
                if (input[i] == '<')
                {
                    int cierre = input.IndexOf('>', i);
                    if (cierre != -1)
                    {
                        string tagCompleto = input.Substring(i, cierre - i + 1);
                        sb.Append(tagCompleto);

                        if (tagCompleto.StartsWith("</"))
                        {
                            if (tagsAbiertos.Count > 0) tagsAbiertos.Pop();
                        }
                        else if (!tagCompleto.EndsWith("/>"))
                        {
                            int finNombre = tagCompleto.IndexOf('=');
                            if (finNombre == -1) finNombre = tagCompleto.Length - 1;
                            string nombreTag = tagCompleto.Substring(1, finNombre - 1).Trim();
                            tagsAbiertos.Push(nombreTag);
                        }

                        i = cierre + 1;
                        continue;
                    }
                }

                sb.Append(input[i]);
                visibles++;
                i++;
            }

            while (tagsAbiertos.Count > 0)
            {
                sb.Append("</").Append(tagsAbiertos.Pop()).Append(">");
            }

            return sb.ToString();
        }

        #endregion
    }
}
