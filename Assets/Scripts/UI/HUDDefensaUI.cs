using UnityEngine;
using CaminosDeGuanajuato.Core;
using CaminosDeGuanajuato.Defense;
using CaminosDeGuanajuato.Player;

namespace CaminosDeGuanajuato.UI
{
    /// <summary>
    /// Interfaz de Usuario (HUD) para el nivel de pruebas del Avance 1.
    /// Proporciona retroalimentacion visual completa e inmediata sin requerir
    /// configuraciones manuales complejas de Canvas.
    /// Muestra recursos civicos, estado de la antorcha, seleccion de defensas y guia de controles.
    /// </summary>
    public class HUDDefensaUI : MonoBehaviour
    {
        [Header("Ajustes Visuales")]
        [SerializeField] private bool mostrarGuiaControles = true;

        private GUIStyle estiloTitulo;
        private GUIStyle estiloSubtitulo;
        private GUIStyle estiloRecursos;
        private GUIStyle estiloBoton;
        private GUIStyle estiloBotonActivo;
        private GUIStyle estiloCaja;
        private GUIStyle estiloControles;
        private Texture2D texturaFondo;
        private Texture2D texturaBarraVerde;
        private Texture2D texturaBarraRoja;

        private void Start()
        {
            CrearTexturas();
        }

        private void CrearTexturas()
        {
            texturaFondo = new Texture2D(1, 1);
            texturaFondo.SetPixel(0, 0, new Color(0.12f, 0.08f, 0.05f, 0.88f)); // Terracota / Nogal colonial oscuro
            texturaFondo.Apply();

            texturaBarraVerde = new Texture2D(1, 1);
            texturaBarraVerde.SetPixel(0, 0, new Color(0.18f, 0.75f, 0.28f, 1f));
            texturaBarraVerde.Apply();

            texturaBarraRoja = new Texture2D(1, 1);
            texturaBarraRoja.SetPixel(0, 0, new Color(0.85f, 0.2f, 0.18f, 1f));
            texturaBarraRoja.Apply();
        }

        private void InicializarEstilos()
        {
            if (estiloTitulo != null) return;

            estiloCaja = new GUIStyle(GUI.skin.box);
            estiloCaja.normal.background = texturaFondo;

            estiloTitulo = new GUIStyle(GUI.skin.label);
            estiloTitulo.fontSize = 17;
            estiloTitulo.fontStyle = FontStyle.Bold;
            estiloTitulo.normal.textColor = new Color(0.96f, 0.62f, 0.05f); // Amarillo canario virreinal

            estiloSubtitulo = new GUIStyle(GUI.skin.label);
            estiloSubtitulo.fontSize = 12;
            estiloSubtitulo.fontStyle = FontStyle.Italic;
            estiloSubtitulo.normal.textColor = new Color(0.85f, 0.85f, 0.85f);

            estiloRecursos = new GUIStyle(GUI.skin.label);
            estiloRecursos.fontSize = 15;
            estiloRecursos.fontStyle = FontStyle.Bold;
            estiloRecursos.normal.textColor = new Color(1f, 0.9f, 0.4f);

            estiloBoton = new GUIStyle(GUI.skin.button);
            estiloBoton.fontSize = 12;
            estiloBoton.fontStyle = FontStyle.Bold;
            estiloBoton.normal.textColor = Color.white;

            estiloBotonActivo = new GUIStyle(estiloBoton);
            estiloBotonActivo.normal.textColor = new Color(0.2f, 1f, 0.4f);

            estiloControles = new GUIStyle(GUI.skin.label);
            estiloControles.fontSize = 11;
            estiloControles.normal.textColor = new Color(0.9f, 0.9f, 0.9f);
        }

        private void OnGUI()
        {
            InicializarEstilos();

            // 1. Panel Superior Izquierdo: Estado General y Recursos
            GUILayout.BeginArea(new Rect(15, 15, 340, 210), estiloCaja);
            GUILayout.Space(6);
            GUILayout.Label("🏛️ Caminos de Guanajuato 2D", estiloTitulo);
            GUILayout.Label("Cuna de la Libertad - Atrio Parroquial (Avance 1)", estiloSubtitulo);
            GUILayout.Space(6);

            // Recursos Civicos
            int recursos = RecursosCivicosManager.Instance != null ? RecursosCivicosManager.Instance.RecursosActuales : 0;
            GUILayout.Label($"⚜ Puntos Civicos: {recursos}", estiloRecursos);

            // Salud de la Antorcha Libertaria
            if (AntorchaLibertaria.Instance != null)
            {
                float pct = AntorchaLibertaria.Instance.PorcentajeSalud;
                GUILayout.Label($"🔥 Antorcha Libertaria: {AntorchaLibertaria.Instance.SaludActual} / {AntorchaLibertaria.Instance.SaludMaxima} HP");
                DibujarBarraProgreso(pct);
            }

            // Estado de Oleada
            if (OleadasSpawner.Instance != null)
            {
                GUILayout.Space(4);
                string estadoOleada = OleadasSpawner.Instance.OleadaEnProgreso
                    ? $"⚔ Oleada {OleadasSpawner.Instance.OleadaActual}/{OleadasSpawner.Instance.TotalOleadas} (Enemigos: {OleadasSpawner.Instance.EnemigosVivos})"
                    : $"Oleada {OleadasSpawner.Instance.OleadaActual}/{OleadasSpawner.Instance.TotalOleadas} (En espera)";
                GUILayout.Label(estadoOleada);

                if (!OleadasSpawner.Instance.OleadaEnProgreso)
                {
                    if (GUILayout.Button("⚔ Invocar Oleada de Prueba [G]", estiloBoton, GUILayout.Height(26)))
                    {
                        OleadasSpawner.Instance.IniciarSiguienteOleada();
                    }
                }
            }

            GUILayout.EndArea();

            // 2. Panel Inferior Central: Selector de Defensas
            if (ColocadorDefensas.Instance != null)
            {
                var catalogo = ColocadorDefensas.Instance.Catalogo;
                int actualSel = ColocadorDefensas.Instance.IndiceSeleccionado;

                float anchoPanel = Mathf.Max(380, catalogo.Count * 180 + 30);
                float xPanel = (Screen.width - anchoPanel) * 0.5f;
                float yPanel = Screen.height - 75;

                GUILayout.BeginArea(new Rect(xPanel, yPanel, anchoPanel, 65), estiloCaja);
                GUILayout.BeginHorizontal();

                for (int i = 0; i < catalogo.Count; i++)
                {
                    var def = catalogo[i];
                    bool estaActivo = (i == actualSel);
                    string etiqueta = $"[{i + 1}] {def.nombre}\n({def.costo} pts)";

                    GUIStyle estilo = estaActivo ? estiloBotonActivo : estiloBoton;
                    if (GUILayout.Button(etiqueta, estilo, GUILayout.Width(170), GUILayout.Height(50)))
                    {
                        ColocadorDefensas.Instance.SeleccionarDefensa(i);
                    }
                }

                GUILayout.EndHorizontal();
                GUILayout.EndArea();
            }

            // 3. Panel Superior Derecho: Guia de Controles para el Evaluador
            if (mostrarGuiaControles)
            {
                float anchoGuia = 280;
                GUILayout.BeginArea(new Rect(Screen.width - anchoGuia - 15, 15, anchoGuia, 165), estiloCaja);
                GUILayout.Space(6);
                GUILayout.Label("🎮 Guia de Controles (Avance 1)", estiloTitulo);
                GUILayout.Space(4);
                GUILayout.Label("• [W,A,S,D] / [Flechas]: Mover a Guanajicho", estiloControles);
                GUILayout.Label("• [Shift]: Correr", estiloControles);
                GUILayout.Label("• [Clic Izquierdo]: Colocar defensa con mouse", estiloControles);
                GUILayout.Label("• [Tecla E]: Colocar defensa frente al personaje", estiloControles);
                GUILayout.Label("• [Teclas 1, 2]: Cambiar defensa seleccionada", estiloControles);
                GUILayout.Label("• [Tecla G]: Invocar oleada de invasores", estiloControles);
                GUILayout.EndArea();
            }
        }

        private void DibujarBarraProgreso(float porcentaje)
        {
            Rect rectBarra = GUILayoutUtility.GetRect(180, 14);
            // Fondo rojo (dano)
            GUI.DrawTexture(rectBarra, texturaBarraRoja);
            // Barra verde (salud restante)
            Rect rectVerde = new Rect(rectBarra.x, rectBarra.y, rectBarra.width * Mathf.Clamp01(porcentaje), rectBarra.height);
            GUI.DrawTexture(rectVerde, texturaBarraVerde);
        }
    }
}
