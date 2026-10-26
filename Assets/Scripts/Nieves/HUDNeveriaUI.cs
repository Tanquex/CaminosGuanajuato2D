using UnityEngine;
using UnityEngine.SceneManagement;
using CaminosDeGuanajuato.Core;

namespace CaminosDeGuanajuato.Nieves
{
    /// <summary>
    /// Interfaz gráfica (HUD) para el Nivel 2: El Desafío de las Nieves Tradicionales.
    /// Presenta las estaciones de preparación culinaria, la comanda del cliente activo,
    /// la barra de paciencia y las pantallas modales de Victoria y Derrota con estética colonial.
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

        // Sprites de Botones temáticos coloniales (Madera tallada, Talavera, Verde colonial, Terracota)
        private Texture2D texBotonMadera;
        private Texture2D texBotonTalavera;
        private Texture2D texBotonVerdeColonial;
        private Texture2D texBotonRojoColonial;

        private float tiempoUltimoRaspado = -1f;

        // Estado de Pausa y Opciones
        private bool estaPausado = false;
        private bool mostrarOpcionesPausa = false;

        private float volumenGeneral = 0.85f;
        private float volumenMusica = 0.65f;
        private float volumenSFX = 0.85f;
        private bool pantallaCompleta = true;

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

        private void Start()
        {
            CrearTexturas();
            Time.timeScale = 1f;
            pantallaCompleta = Screen.fullScreen;

            if (AudioColonialManager.Instance != null)
            {
                volumenGeneral = AudioColonialManager.Instance.VolumenGeneral;
                volumenMusica = AudioColonialManager.Instance.VolumenMusica;
                volumenSFX = AudioColonialManager.Instance.VolumenSFX;
            }
        }

        private void Update()
        {
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

        private Texture2D CargarTexturaAsset(string rutaRelativa)
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
            texFondoColonial.SetPixel(0, 0, new Color(0.1f, 0.06f, 0.04f, 0.88f)); // Traslúcido elegante
            texFondoColonial.Apply();

            texFondoComanda = new Texture2D(1, 1);
            texFondoComanda.SetPixel(0, 0, new Color(0.15f, 0.1f, 0.05f, 0.90f));
            texFondoComanda.Apply();

            texFondoDonChon = new Texture2D(1, 1);
            texFondoDonChon.SetPixel(0, 0, new Color(0.08f, 0.16f, 0.28f, 0.90f));
            texFondoDonChon.Apply();

            texOverlay = new Texture2D(1, 1);
            texOverlay.SetPixel(0, 0, new Color(0.04f, 0.02f, 0.02f, 0.8f));
            texOverlay.Apply();

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

            // 3. Crear botones temáticos procedurales de alta legibilidad (fondo liso, cero ruido visual)
            texBotonMadera = CrearTexturaBotonColonial(
                new Color(0.20f, 0.12f, 0.08f, 0.98f),
                new Color(0.60f, 0.42f, 0.22f, 1f),
                new Color(0.08f, 0.05f, 0.03f, 1f),
                new Color(0.85f, 0.68f, 0.30f, 1f)
            );

            // EL SELECCIONADOR DEL INDICADOR: Fondo azul medianoche liso (CERO RUIDO) con doble marco de ORO ESPAÑOL resplandeciente
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

            estiloTitulo = new GUIStyle(GUI.skin.label);
            estiloTitulo.fontSize = 20;
            estiloTitulo.fontStyle = FontStyle.Bold;
            estiloTitulo.alignment = TextAnchor.MiddleCenter;
            estiloTitulo.normal.textColor = new Color(0.98f, 0.75f, 0.18f);

            estiloSubtitulo = new GUIStyle(GUI.skin.label);
            estiloSubtitulo.fontSize = 13;
            estiloSubtitulo.fontStyle = FontStyle.Italic;
            estiloSubtitulo.alignment = TextAnchor.MiddleCenter;
            estiloSubtitulo.normal.textColor = new Color(0.85f, 0.85f, 0.85f);

            estiloPuntaje = new GUIStyle(GUI.skin.label);
            estiloPuntaje.fontSize = 15;
            estiloPuntaje.fontStyle = FontStyle.Bold;
            estiloPuntaje.normal.textColor = new Color(1f, 0.9f, 0.35f);

            estiloTemporizador = new GUIStyle(GUI.skin.label);
            estiloTemporizador.fontSize = 14;
            estiloTemporizador.fontStyle = FontStyle.Bold;
            estiloTemporizador.normal.textColor = new Color(0.3f, 0.95f, 1f);

            estiloTextoBlanco = new GUIStyle(GUI.skin.label);
            estiloTextoBlanco.fontSize = 12;
            estiloTextoBlanco.normal.textColor = Color.white;

            // Botón de acción: Madera tallada virreinal con remaches de latón
            estiloBotonAccion = new GUIStyle(GUI.skin.button);
            estiloBotonAccion.fontSize = 13;
            estiloBotonAccion.fontStyle = FontStyle.Bold;
            estiloBotonAccion.border = new RectOffset(10, 10, 10, 10);
            estiloBotonAccion.normal.background = texBotonMadera;
            estiloBotonAccion.normal.textColor = new Color(0.98f, 0.94f, 0.82f); // Dorado crema cálido

            // Botón seleccionado / destacado: Resplandor Dorado / Azul Noche de Alto Contraste (CERO RUIDO)
            estiloBotonSeleccionado = new GUIStyle(estiloBotonAccion);
            estiloBotonSeleccionado.border = new RectOffset(10, 10, 10, 10);
            estiloBotonSeleccionado.normal.background = texBotonTalavera;
            estiloBotonSeleccionado.normal.textColor = new Color(1.0f, 0.95f, 0.45f); // Oro brillante súper legible

            // Botón de servir: Verde esmeralda colonial con bordes dorados
            estiloBotonServir = new GUIStyle(GUI.skin.button);
            estiloBotonServir.fontSize = 14;
            estiloBotonServir.fontStyle = FontStyle.Bold;
            estiloBotonServir.border = new RectOffset(10, 10, 10, 10);
            estiloBotonServir.normal.background = texBotonVerdeColonial;
            estiloBotonServir.normal.textColor = Color.white;

            // Botón desechar: Terracota artesanal
            estiloBotonDesechar = new GUIStyle(GUI.skin.button);
            estiloBotonDesechar.fontSize = 12;
            estiloBotonDesechar.fontStyle = FontStyle.Bold;
            estiloBotonDesechar.border = new RectOffset(10, 10, 10, 10);
            estiloBotonDesechar.normal.background = texBotonRojoColonial;
            estiloBotonDesechar.normal.textColor = Color.white;
        }

        private void OnGUI()
        {
            InicializarEstilos();

            if (NeveriaGameManager.Instance == null) return;

            // Comprobar estado de fin de partida
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

            // Si el juego está pausado, dibujar modal de pausa u opciones
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

            // 1. Barra Superior Compacta de Telemetría
            DibujarBarraSuperior();

            // 2. Comanda del Cliente y Consejo de Don Chon (Unificados y Limpios)
            DibujarComandaYConsejero();

            // 3. Estación de Preparación Guiada Paso a Paso
            DibujarMesaPreparacionGuiada();
        }

        private void DibujarBarraSuperior()
        {
            float ancho = Screen.width - 30;
            float alto = 42;
            GUILayout.BeginArea(new Rect(15, 10, ancho, alto), estiloCajaOscura);
            GUILayout.BeginHorizontal();

            GUILayout.Label("🍨 NIEVES DE DOLORES HIDALGO", estiloPuntaje, GUILayout.Width(270));

            // Clientes atendidos
            int at = NeveriaGameManager.Instance.ClientesAtendidos;
            int obj = NeveriaGameManager.Instance.ClientesObjetivo;
            GUILayout.Label($"Comensales: <b>{at} / {obj}</b>", estiloTextoBlanco, GUILayout.Width(140));

            // Vidas / Paciencia del pueblo
            int perdidos = NeveriaGameManager.Instance.ClientesPerdidos;
            int maxPerd = NeveriaGameManager.Instance.MaxClientesPerdidos;
            string strikes = "";
            for (int i = 0; i < maxPerd; i++)
            {
                strikes += (i < perdidos) ? "💔 " : "❤️ ";
            }
            GUILayout.Label($"Paciencia: {strikes}", estiloTextoBlanco, GUILayout.Width(160));

            GUILayout.FlexibleSpace();

            // Temporizador y Puntaje
            float t = NeveriaGameManager.Instance.TiempoRestante;
            GUILayout.Label($"⏳ Tiempo: {Mathf.CeilToInt(t):D2}s", estiloTemporizador, GUILayout.Width(120));
            GUILayout.Label($"⭐ Puntos: {NeveriaGameManager.Instance.PuntajeTotal}", estiloPuntaje, GUILayout.Width(120));

            // Botón de Pausa en barra superior
            if (GUILayout.Button("⏸️ PAUSA [ESC]", estiloBotonAccion, GUILayout.Width(130), GUILayout.Height(28)))
            {
                AlternarPausa();
            }

            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        private void DibujarComandaYConsejero()
        {
            var cliente = NeveriaGameManager.Instance.ClienteActual;
            float ancho = Mathf.Min(440, Screen.width * 0.40f);
            float alto = 92;

            GUILayout.BeginArea(new Rect(12, 48, ancho, alto), estiloCajaComanda);
            GUILayout.Space(4);

            if (cliente == null || cliente.Pedido == null)
            {
                GUILayout.Label("⏳ Viene llegando un nuevo visitante a la plaza...", estiloSubtitulo);
                if (DonChonConsejero.Instance != null)
                {
                    GUILayout.Space(2);
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
            GUILayout.Label($"🛎️ <b>{cliente.NombreCliente}</b> pide:", estiloPuntaje, GUILayout.Width(220));
            GUILayout.FlexibleSpace();
            GUILayout.Label($"Paciencia: {Mathf.CeilToInt(pedido.tiempoPacienciaRestante)}s", estiloTextoBlanco);
            GUILayout.EndHorizontal();

            GUILayout.Space(2);

            // Fila visual compacta con los iconos
            GUILayout.BeginHorizontal();

            if (texRec != null)
            {
                Rect rIcoRec = GUILayoutUtility.GetRect(26, 26, GUILayout.Width(26), GUILayout.Height(26));
                GUI.DrawTexture(rIcoRec, texRec, ScaleMode.ScaleToFit);
                GUILayout.Space(4);
            }
            GUILayout.Label($"<color=#ffdd44><b>{recNombre}</b></color>", estiloTextoBlanco, GUILayout.Width(150));

            GUILayout.Space(8);

            if (texSab != null)
            {
                Rect rIcoSab = GUILayoutUtility.GetRect(26, 26, GUILayout.Width(26), GUILayout.Height(26));
                GUI.DrawTexture(rIcoSab, texSab, ScaleMode.ScaleToFit);
                GUILayout.Space(4);
            }
            GUILayout.Label($"<color=#ffaa33><b>{saborNombre}</b></color>", estiloTextoBlanco);

            GUILayout.EndHorizontal();

            GUILayout.Space(4);

            // Barra de paciencia fluida
            Rect rBarra = GUILayoutUtility.GetRect(ancho - 16, 9);
            GUI.DrawTexture(rBarra, texBarraFondo);
            float pct = cliente.PorcentajePaciencia;
            Texture2D texColor = (pct > 0.5f) ? texBarraVerde : (pct > 0.25f ? texBarraAmarilla : texBarraRoja);
            GUI.DrawTexture(new Rect(rBarra.x, rBarra.y, rBarra.width * pct, rBarra.height), texColor);

            GUILayout.EndArea();
        }

        private void DibujarMesaPreparacionGuiada()
        {
            if (MesaPreparacion.Instance == null) return;

            float anchoPanel = Mathf.Min(940, Screen.width - 16);
            float altoPanel = 142;
            float xPanel = (Screen.width - anchoPanel) * 0.5f;
            float yPanel = Screen.height - altoPanel;

            GUILayout.BeginArea(new Rect(xPanel, yPanel, anchoPanel, altoPanel), estiloCajaOscura);
            GUILayout.Space(4);

            // 1. Barra de Progreso de Pasos (Breadcrumbs visuales)
            bool tieneBase = MesaPreparacion.Instance.RecipienteActual != TipoRecipiente.Ninguno;
            bool tieneHielo = MesaPreparacion.Instance.TieneHieloListo;
            bool tieneSabor = MesaPreparacion.Instance.SaborActual != SaborNieve.Ninguno;
            bool nieveLista = tieneBase && tieneHielo && tieneSabor;

            string txtP1 = tieneBase ? $"✓ {PedidoNieve.ObtenerNombreRecipiente(MesaPreparacion.Instance.RecipienteActual)}" : "1. Base";
            string txtP2 = tieneHielo ? "✓ Hielo Listo" : (tieneBase ? $"2. Raspar ({Mathf.RoundToInt(MesaPreparacion.Instance.ProgresoHielo * 100)}%)" : "2. Hielo");
            string txtP3 = tieneSabor ? $"✓ {PedidoNieve.ObtenerNombreSabor(MesaPreparacion.Instance.SaborActual)}" : "3. Sabor";
            string txtP4 = nieveLista ? "4. ¡Servir!" : "4. Servir";

            GUILayout.BeginHorizontal();
            GUILayout.Label("PREPARACIÓN:", estiloPuntaje, GUILayout.Width(130));
            GUILayout.Label(tieneBase ? $"<color=#55ff55>[ {txtP1} ]</color>" : $"<color=#ffdd44><b>▶ [ {txtP1} ]</b></color>", estiloTextoBlanco);
            GUILayout.Label("➔", estiloTextoBlanco, GUILayout.Width(14));
            GUILayout.Label(tieneHielo ? $"<color=#55ff55>[ {txtP2} ]</color>" : (tieneBase ? $"<color=#ffdd44><b>▶ [ {txtP2} ]</b></color>" : $"<color=#888888>[ {txtP2} ]</color>"), estiloTextoBlanco);
            GUILayout.Label("➔", estiloTextoBlanco, GUILayout.Width(14));
            GUILayout.Label(tieneSabor ? $"<color=#55ff55>[ {txtP3} ]</color>" : (tieneHielo ? $"<color=#ffdd44><b>▶ [ {txtP3} ]</b></color>" : $"<color=#888888>[ {txtP3} ]</color>"), estiloTextoBlanco);
            GUILayout.Label("➔", estiloTextoBlanco, GUILayout.Width(14));
            GUILayout.Label(nieveLista ? $"<color=#55ff55><b>▶ [ {txtP4} ]</b></color>" : $"<color=#888888>[ {txtP4} ]</color>", estiloTextoBlanco);

            GUILayout.FlexibleSpace();
            if (MesaPreparacion.Instance.TieneArticuloEnPreparacion)
            {
                if (BotonConIcono("🗑️ Desechar", null, estiloBotonDesechar, 110, 24))
                {
                    MesaPreparacion.Instance.DesecharPreparacion();
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(4);

            // 2. Área Dinámica: Muestra ÚNICAMENTE las opciones del paso actual
            if (!tieneBase)
            {
                // PASO 1: Elegir Base
                var cliente = NeveriaGameManager.Instance.ClienteActual;
                TipoRecipiente recPedido = (cliente != null && cliente.Pedido != null)
                    ? cliente.Pedido.recipienteDeseado
                    : TipoRecipiente.Ninguno;

                GUILayout.Label("👉 <b>PASO 1:</b> Elige el recipiente para servir la nieve dolorense:", estiloSubtitulo);
                GUILayout.Space(4);
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();

                GUIStyle stBarquillo = (recPedido == TipoRecipiente.BarquilloCrujiente) ? estiloBotonSeleccionado : estiloBotonAccion;
                string lblBarquillo = (recPedido == TipoRecipiente.BarquilloCrujiente) ? "⭐ [1] Barquillo Crujiente (Requerido)" : "[1] Barquillo Crujiente";
                if (BotonConIcono(lblBarquillo, texIconoBarquillo, stBarquillo, 320, 50))
                {
                    MesaPreparacion.Instance.SeleccionarRecipiente(TipoRecipiente.BarquilloCrujiente);
                }

                GUILayout.Space(20);

                GUIStyle stVaso = (recPedido == TipoRecipiente.VasoDeBarro) ? estiloBotonSeleccionado : estiloBotonAccion;
                string lblVaso = (recPedido == TipoRecipiente.VasoDeBarro) ? "⭐ [2] Vaso de Barro (Requerido)" : "[2] Vaso de Barro Tradicional";
                if (BotonConIcono(lblVaso, texIconoVasoBarro, stVaso, 320, 50))
                {
                    MesaPreparacion.Instance.SeleccionarRecipiente(TipoRecipiente.VasoDeBarro);
                }

                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
            else if (!tieneHielo)
            {
                // PASO 2: Raspar Hielo en Garrafa con Animación visual en 4 etapas
                float pctHielo = MesaPreparacion.Instance.ProgresoHielo;
                int etapa = Mathf.Clamp(Mathf.FloorToInt(pctHielo * 3.99f), 0, 3);
                Texture2D texGarrafa = texGarrafaEtapas[etapa];

                bool estaRaspando = Input.GetKey(KeyCode.Space) || (Time.time - tiempoUltimoRaspado < 0.25f);
                if (Input.GetKey(KeyCode.Space))
                {
                    tiempoUltimoRaspado = Time.time;
                }

                GUILayout.Label($"👉 <b>PASO 2:</b> Raspa y gira la garrafa tradicional de madera ({Mathf.RoundToInt(pctHielo * 100)}%):", estiloSubtitulo);
                GUILayout.Space(2);

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();

                // Garrafa animada compacta
                float offsetVibracion = estaRaspando ? (Mathf.Sin(Time.time * 40f) * 3f) : 0f;
                Rect rAreaGarrafa = GUILayoutUtility.GetRect(72, 72, GUILayout.Width(72), GUILayout.Height(72));
                Rect rGarrafaImg = new Rect(rAreaGarrafa.x + offsetVibracion, rAreaGarrafa.y, 72, 72);
                if (texGarrafa != null)
                {
                    GUI.DrawTexture(rGarrafaImg, texGarrafa, ScaleMode.ScaleToFit);
                }

                GUILayout.Space(14);

                GUILayout.BeginVertical(GUILayout.Width(500));

                string nombreEtapa = etapa switch
                {
                    0 => "Garrafa vacía enfriándose en barrica de sal y hielo",
                    1 => "¡Aparece la primera escarcha de raspado tradicional!",
                    2 => "¡La masa de nieve artesanal sube y toma consistencia!",
                    3 => "¡Nieve suave, tersa y copeteada lista para servir!",
                    _ => ""
                };

                GUILayout.Label($"<b>Etapa {etapa + 1}/4:</b> <color=#ffdd44>{nombreEtapa}</color>", estiloSubtitulo);
                GUILayout.Space(2);

                if (BotonConIcono("❄️ [ESPACIO o Clic] ¡GIRAR Y RASPAR GARRAFA ARTESANAL!", null, estiloBotonSeleccionado, 500, 42))
                {
                    MesaPreparacion.Instance.RasparHielo(0.35f);
                    tiempoUltimoRaspado = Time.time;
                }

                GUILayout.Space(3);

                Rect rBarraRaspado = GUILayoutUtility.GetRect(500, 12);
                GUI.DrawTexture(rBarraRaspado, texBarraFondo);
                GUI.DrawTexture(new Rect(rBarraRaspado.x, rBarraRaspado.y, rBarraRaspado.width * pctHielo, rBarraRaspado.height), texBarraVerde);

                GUILayout.EndVertical();

                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
            else if (!tieneSabor)
            {
                // PASO 3: Elegir Sabor con Iconos y Atajos
                SaborNieve saborPedido = NeveriaGameManager.Instance.ClienteActual != null && NeveriaGameManager.Instance.ClienteActual.Pedido != null
                    ? NeveriaGameManager.Instance.ClienteActual.Pedido.saborDeseado
                    : SaborNieve.Ninguno;

                GUILayout.Label($"👉 <b>PASO 3:</b> Añade la pulpa artesanal a la nieve copeteada:", estiloSubtitulo);
                GUILayout.Space(4);
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();

                BotonSaborDinamico("[Q] Garambullo", SaborNieve.Garambullo, saborPedido);
                GUILayout.Space(6);
                BotonSaborDinamico("[W] Borrachita", SaborNieve.Borrachita, saborPedido);
                GUILayout.Space(6);
                BotonSaborDinamico("[E] Q. Fresa", SaborNieve.QuesoConFresa, saborPedido);
                GUILayout.Space(6);
                BotonSaborDinamico("[R] Aguacate", SaborNieve.Aguacate, saborPedido);
                GUILayout.Space(6);
                BotonSaborDinamico("[T] Limón", SaborNieve.LimonVirreinal, saborPedido);

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
                    GUILayout.Label("⏳ <b>El comensal anterior se retiró por tiempo.</b> ¡Tu nieve sigue en el mostrador para el siguiente comensal!", estiloSubtitulo);
                    GUILayout.Space(4);
                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();

                    if (texGarrafaEtapas[3] != null)
                    {
                        Rect rIcoListo = GUILayoutUtility.GetRect(50, 50, GUILayout.Width(50), GUILayout.Height(50));
                        GUI.DrawTexture(rIcoListo, texGarrafaEtapas[3], ScaleMode.ScaleToFit);
                        GUILayout.Space(12);
                    }

                    if (BotonConIcono("🗑️ DESECHAR Y PREPARAR DE NUEVO [BORRAR]", null, estiloBotonDesechar, 360, 48))
                    {
                        MesaPreparacion.Instance.DesecharPreparacion();
                    }
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                }
                else if (coincide)
                {
                    GUILayout.Label($"👉 <b>PASO 4:</b> ¡La nieve está lista y coincide con el antojo de {cliente.NombreCliente}! Sírvela:", estiloSubtitulo);
                    GUILayout.Space(4);
                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();

                    Texture2D icoFinal = ObtenerIconoSabor(MesaPreparacion.Instance.SaborActual);
                    if (icoFinal != null)
                    {
                        Rect rIcoFinal = GUILayoutUtility.GetRect(48, 48, GUILayout.Width(48), GUILayout.Height(48));
                        GUI.DrawTexture(rIcoFinal, icoFinal, ScaleMode.ScaleToFit);
                        GUILayout.Space(12);
                    }

                    if (BotonConIcono($"🍧 ¡ENTREGAR A {cliente.NombreCliente.ToUpper()}! [ENTER o Clic]", null, estiloBotonServir, 520, 50))
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
                    GUILayout.Space(4);
                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();

                    if (BotonConIcono($"🔄 DESECHAR Y PREPARAR {sabPed.ToUpper()} [BORRAR]", null, estiloBotonDesechar, 380, 48))
                    {
                        MesaPreparacion.Instance.DesecharPreparacion();
                    }

                    GUILayout.Space(12);

                    if (BotonConIcono("❌ Entregar con fallo", null, estiloBotonAccion, 180, 48))
                    {
                        NeveriaGameManager.Instance.EntregarPedidoConFalloForzado();
                    }

                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                }
            }

            GUILayout.EndArea();
        }

        private void BotonSaborDinamico(string texto, SaborNieve sabor, SaborNieve saborPedido)
        {
            bool esElPedido = (sabor == saborPedido);
            GUIStyle st = esElPedido ? estiloBotonSeleccionado : estiloBotonAccion;
            string etiqueta = esElPedido ? $"⭐ {texto}" : texto;
            Texture2D ico = ObtenerIconoSabor(sabor);

            if (BotonConIcono(etiqueta, ico, st, 168, 48))
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
                float tamIcono = Mathf.Min(alto - 12, 32);
                Rect rIcono = new Rect(r.x + 8, r.y + (alto - tamIcono) * 0.5f, tamIcono, tamIcono);
                GUI.DrawTexture(rIcono, icono, ScaleMode.ScaleToFit);

                float xTexto = r.x + tamIcono + 14;
                float wTexto = r.width - (tamIcono + 18);
                Rect rTexto = new Rect(xTexto, r.y, wTexto, r.height);

                GUIStyle estiloTxt = new GUIStyle(GUI.skin.label);
                estiloTxt.fontSize = estilo.fontSize;
                estiloTxt.fontStyle = FontStyle.Bold;
                estiloTxt.normal.textColor = estilo.normal.textColor;
                estiloTxt.alignment = TextAnchor.MiddleLeft;
                estiloTxt.wordWrap = false;

                // Sombra de texto para contraste perfecto
                GUIStyle estiloSombra = new GUIStyle(estiloTxt);
                estiloSombra.normal.textColor = new Color(0f, 0f, 0f, 0.95f);
                GUI.Label(new Rect(rTexto.x + 1, rTexto.y + 1, rTexto.width, rTexto.height), texto, estiloSombra);

                // Texto frontal nítido
                GUI.Label(rTexto, texto, estiloTxt);
            }
            else
            {
                GUIStyle estiloTxt = new GUIStyle(GUI.skin.label);
                estiloTxt.fontSize = estilo.fontSize;
                estiloTxt.fontStyle = FontStyle.Bold;
                estiloTxt.normal.textColor = estilo.normal.textColor;
                estiloTxt.alignment = TextAnchor.MiddleCenter;
                estiloTxt.wordWrap = false;

                // Sombra de texto
                GUIStyle estiloSombra = new GUIStyle(estiloTxt);
                estiloSombra.normal.textColor = new Color(0f, 0f, 0f, 0.95f);
                GUI.Label(new Rect(r.x + 1, r.y + 1, r.width, r.height), texto, estiloSombra);

                // Texto frontal
                GUI.Label(r, texto, estiloTxt);
            }

            return clicked;
        }

        private void DibujarModalVictoria()
        {
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), texOverlay);

            float w = 580;
            float h = 300;
            float x = (Screen.width - w) * 0.5f;
            float y = (Screen.height - h) * 0.5f;

            GUILayout.BeginArea(new Rect(x, y, w, h), estiloModalVictoria);
            GUILayout.Space(16);
            GUILayout.Label("⭐ ¡FESTIVAL DE LA NIEVE SUPERADO! ⭐", estiloTitulo);
            GUILayout.Label("¡Has obtenido la Cuchara Dorada Artesanal (Medalla 2)!", estiloSubtitulo);
            GUILayout.Space(12);

            int pts = NeveriaGameManager.Instance != null ? NeveriaGameManager.Instance.PuntajeTotal : 0;
            GUILayout.Label($"Has atendido con éxito a todos los comensales de Dolores Hidalgo.\nPuntaje Culinario: <b>{pts} Puntos</b>\n\nDon Chon y los artesanos del pueblo apoyan plenamente la causa libertaria.", estiloTextoBlanco);
            GUILayout.Space(18);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (BotonConIcono("🔄 REPETIR", null, estiloBotonAccion, 140, 42))
            {
                ReiniciarNivel();
            }
            GUILayout.Space(10);
            if (BotonConIcono("🗺️ MAPA / MISIONES", null, estiloBotonServir, 220, 42))
            {
                IrAlOverworld();
            }
            GUILayout.Space(10);
            if (BotonConIcono("🏠 MENÚ", null, estiloBotonAccion, 120, 42))
            {
                IrAlMenu();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.EndArea();
        }

        private void DibujarModalDerrota()
        {
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), texOverlay);

            float w = 520;
            float h = 260;
            float x = (Screen.width - w) * 0.5f;
            float y = (Screen.height - h) * 0.5f;

            GUILayout.BeginArea(new Rect(x, y, w, h), estiloModalDerrota);
            GUILayout.Space(16);
            GUILayout.Label("💀 ¡JORNADA TERMINADA! 💀", estiloTitulo);
            GUILayout.Label("Demasiados comensales se retiraron inconformes", estiloSubtitulo);
            GUILayout.Space(10);

            GUILayout.Label("La rapidez y la precisión son esenciales en el quiosco de Don Chon.\nRecuerda raspar el hielo y fijarte bien en el sabor que pide cada cliente.\n\n¡Organízate y vuelve a intentarlo!", estiloTextoBlanco);
            GUILayout.Space(18);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (BotonConIcono("🔄 REINTENTAR", null, estiloBotonServir, 180, 42))
            {
                ReiniciarNivel();
            }
            GUILayout.Space(12);
            if (BotonConIcono("🗺️ MAPA GENERAL", null, estiloBotonAccion, 180, 42))
            {
                IrAlOverworld();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.EndArea();
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
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), texOverlay);

            float ancho = 460;
            float alto = 380;
            float x = (Screen.width - ancho) * 0.5f;
            float y = (Screen.height - alto) * 0.5f;

            GUILayout.BeginArea(new Rect(x, y, ancho, alto), estiloCajaOscura);
            GUILayout.Space(14);
            GUILayout.Label("⏸️ JUEGO EN PAUSA", estiloTitulo);
            GUILayout.Label("Nivel 2: Desafío de las Nieves de Dolores", estiloSubtitulo);
            GUILayout.Space(14);

            GUILayout.BeginVertical();

            if (BotonConIcono("▶️ CONTINUAR [ESC]", null, estiloBotonServir, 420, 42))
            {
                AlternarPausa();
            }

            GUILayout.Space(8);

            if (BotonConIcono("⚙ OPCIONES DE AUDIO / PANTALLA", null, estiloBotonAccion, 420, 42))
            {
                mostrarOpcionesPausa = true;
            }

            GUILayout.Space(8);

            if (BotonConIcono("🔄 REINICIAR JORNADA", null, estiloBotonAccion, 420, 42))
            {
                ReiniciarNivel();
            }

            GUILayout.Space(8);

            if (BotonConIcono("🗺️ MAPA GENERAL (OVERWORLD)", null, estiloBotonAccion, 420, 42))
            {
                IrAlOverworld();
            }

            GUILayout.Space(8);

            if (BotonConIcono("🏠 MENÚ PRINCIPAL", null, estiloBotonAccion, 420, 42))
            {
                IrAlMenu();
            }

            GUILayout.EndVertical();
            GUILayout.EndArea();
        }

        private void DibujarModalOpcionesPausa()
        {
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), texOverlay);

            float ancho = 480;
            float alto = 340;
            float x = (Screen.width - ancho) * 0.5f;
            float y = (Screen.height - alto) * 0.5f;

            GUILayout.BeginArea(new Rect(x, y, ancho, alto), estiloCajaOscura);
            GUILayout.Space(14);
            GUILayout.Label("⚙ OPCIONES DE JUEGO", estiloTitulo);
            GUILayout.Label("Ajustes de audio y visualización", estiloSubtitulo);
            GUILayout.Space(14);

            // Slider Volumen General
            GUILayout.BeginHorizontal();
            GUILayout.Label($"Volumen General: {Mathf.RoundToInt(volumenGeneral * 100)}%", estiloTextoBlanco, GUILayout.Width(180));
            float nuevoGen = GUILayout.HorizontalSlider(volumenGeneral, 0f, 1f);
            if (Mathf.Abs(nuevoGen - volumenGeneral) > 0.001f)
            {
                volumenGeneral = nuevoGen;
                AudioListener.volume = volumenGeneral;
                if (AudioColonialManager.Instance != null)
                {
                    AudioColonialManager.Instance.AjustarVolumenGeneral(volumenGeneral);
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(8);

            // Slider Música
            GUILayout.BeginHorizontal();
            GUILayout.Label($"Música Tradicional: {Mathf.RoundToInt(volumenMusica * 100)}%", estiloTextoBlanco, GUILayout.Width(180));
            float nuevaMus = GUILayout.HorizontalSlider(volumenMusica, 0f, 1f);
            if (Mathf.Abs(nuevaMus - volumenMusica) > 0.001f)
            {
                volumenMusica = nuevaMus;
                if (AudioColonialManager.Instance != null)
                {
                    AudioColonialManager.Instance.AjustarVolumenMusica(volumenMusica);
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(8);

            // Slider SFX
            GUILayout.BeginHorizontal();
            GUILayout.Label($"Efectos de Sonido: {Mathf.RoundToInt(volumenSFX * 100)}%", estiloTextoBlanco, GUILayout.Width(180));
            float nuevoSFX = GUILayout.HorizontalSlider(volumenSFX, 0f, 1f);
            if (Mathf.Abs(nuevoSFX - volumenSFX) > 0.001f)
            {
                volumenSFX = nuevoSFX;
                if (AudioColonialManager.Instance != null)
                {
                    AudioColonialManager.Instance.AjustarVolumenSFX(volumenSFX);
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            // Toggle Pantalla Completa
            bool nuevaPantalla = GUILayout.Toggle(pantallaCompleta, " Pantalla Completa (Full Screen)", estiloTextoBlanco);
            if (nuevaPantalla != pantallaCompleta)
            {
                pantallaCompleta = nuevaPantalla;
                Screen.fullScreen = pantallaCompleta;
            }

            GUILayout.Space(18);

            // Botón Volver
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (BotonConIcono("GUARDAR Y VOLVER", null, estiloBotonServir, 220, 40))
            {
                mostrarOpcionesPausa = false;
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.EndArea();
        }

        public void ReiniciarNivel()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void IrAlOverworld()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("Overworld_MapaGuanajuato");
        }

        public void IrAlMenu()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("MainMenu");
        }
    }
}
