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

            // Texto del botón
            estiloTextoBoton.fontSize = fontSize;

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
    }
}
