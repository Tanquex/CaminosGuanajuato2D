using UnityEngine;
using UnityEngine.SceneManagement;
using CaminosDeGuanajuato.Core;

namespace CaminosDeGuanajuato.Overworld
{
    /// <summary>
    /// Subpanel modal de selección de misiones locales para el municipio de Dolores Hidalgo C.I.N.
    /// Presenta las 3 misiones oficiales del GDD con sus tarjetas descriptivas,
    /// estado de victoria, medallas obtenidas y botón directo para jugar.
    /// </summary>
    public class SelectorMisionesUI : MonoBehaviour
    {
        [Header("Escenas de Misiones")]
        [SerializeField] private string escenaMision1 = "NivelPruebas_Defensa";
        [SerializeField] private string escenaMision2 = "Nivel2_DesafioNieves";
        [SerializeField] private string escenaMision3 = "Nivel3_DueloDestreza";

        private bool estaAbierto = false;
        public bool EstaAbierto => estaAbierto;

        private GUIStyle estiloModal;
        private GUIStyle estiloTitulo;
        private GUIStyle estiloSubtitulo;
        private GUIStyle estiloTarjetaMision;
        private GUIStyle estiloTituloMision;
        private GUIStyle estiloDescMision;
        private GUIStyle estiloBotonJugar;
        private GUIStyle estiloBotonCompletado;
        private GUIStyle estiloBotonBloqueado;
        private GUIStyle estiloBotonCerrar;

        private Texture2D texFondoModal;
        private Texture2D texTarjetaFondo;
        private Texture2D texBotonVerde;
        private Texture2D texBotonDorado;
        private Texture2D texBotonGris;

        public void Abrir()
        {
            estaAbierto = true;
        }

        public void Cerrar()
        {
            estaAbierto = false;
        }

        private void CrearTexturas()
        {
            if (texFondoModal != null) return;

            texFondoModal = new Texture2D(1, 1);
            texFondoModal.SetPixel(0, 0, new Color(0.1f, 0.06f, 0.04f, 0.97f)); // Terracota virreinal oscuro
            texFondoModal.Apply();

            texTarjetaFondo = new Texture2D(1, 1);
            texTarjetaFondo.SetPixel(0, 0, new Color(0.16f, 0.12f, 0.08f, 0.92f));
            texTarjetaFondo.Apply();

            texBotonVerde = new Texture2D(1, 1);
            texBotonVerde.SetPixel(0, 0, new Color(0.18f, 0.65f, 0.3f, 0.95f)); // Verde acción
            texBotonVerde.Apply();

            texBotonDorado = new Texture2D(1, 1);
            texBotonDorado.SetPixel(0, 0, new Color(0.92f, 0.72f, 0.15f, 0.95f)); // Dorado victoria
            texBotonDorado.Apply();

            texBotonGris = new Texture2D(1, 1);
            texBotonGris.SetPixel(0, 0, new Color(0.35f, 0.35f, 0.35f, 0.85f));
            texBotonGris.Apply();
        }

        private void InicializarEstilos()
        {
            CrearTexturas();
            if (estiloTitulo != null) return;

            estiloModal = new GUIStyle(GUI.skin.box);
            estiloModal.normal.background = texFondoModal;

            estiloTarjetaMision = new GUIStyle(GUI.skin.box);
            estiloTarjetaMision.normal.background = texTarjetaFondo;

            estiloTitulo = new GUIStyle(GUI.skin.label);
            estiloTitulo.fontSize = 22;
            estiloTitulo.fontStyle = FontStyle.Bold;
            estiloTitulo.alignment = TextAnchor.MiddleCenter;
            estiloTitulo.normal.textColor = new Color(0.98f, 0.75f, 0.18f); // Dorado

            estiloSubtitulo = new GUIStyle(GUI.skin.label);
            estiloSubtitulo.fontSize = 13;
            estiloSubtitulo.fontStyle = FontStyle.Italic;
            estiloSubtitulo.alignment = TextAnchor.MiddleCenter;
            estiloSubtitulo.normal.textColor = new Color(0.85f, 0.85f, 0.85f);

            estiloTituloMision = new GUIStyle(GUI.skin.label);
            estiloTituloMision.fontSize = 15;
            estiloTituloMision.fontStyle = FontStyle.Bold;
            estiloTituloMision.normal.textColor = new Color(0.95f, 0.6f, 0.2f); // Terracota cálido

            estiloDescMision = new GUIStyle(GUI.skin.label);
            estiloDescMision.fontSize = 12;
            estiloDescMision.wordWrap = true;
            estiloDescMision.normal.textColor = new Color(0.9f, 0.9f, 0.9f);

            estiloBotonJugar = new GUIStyle(GUI.skin.button);
            estiloBotonJugar.fontSize = 14;
            estiloBotonJugar.fontStyle = FontStyle.Bold;
            estiloBotonJugar.normal.background = texBotonVerde;
            estiloBotonJugar.normal.textColor = Color.white;

            estiloBotonCompletado = new GUIStyle(GUI.skin.button);
            estiloBotonCompletado.fontSize = 13;
            estiloBotonCompletado.fontStyle = FontStyle.Bold;
            estiloBotonCompletado.normal.background = texBotonDorado;
            estiloBotonCompletado.normal.textColor = new Color(0.15f, 0.08f, 0.02f);

            estiloBotonBloqueado = new GUIStyle(GUI.skin.button);
            estiloBotonBloqueado.fontSize = 12;
            estiloBotonBloqueado.fontStyle = FontStyle.Bold;
            estiloBotonBloqueado.normal.background = texBotonGris;
            estiloBotonBloqueado.normal.textColor = new Color(0.75f, 0.75f, 0.75f);

            estiloBotonCerrar = new GUIStyle(GUI.skin.button);
            estiloBotonCerrar.fontSize = 14;
            estiloBotonCerrar.fontStyle = FontStyle.Bold;
            estiloBotonCerrar.normal.textColor = Color.white;
        }

        public void DibujarGUI()
        {
            if (!estaAbierto) return;
            InicializarEstilos();

            // Fondo semitransparente oscuro detrás del modal
            GUI.Box(new Rect(0, 0, Screen.width, Screen.height), GUIContent.none);

            float anchoModal = Mathf.Min(880, Screen.width - 30);
            float altoModal = Mathf.Min(560, Screen.height - 40);
            float xModal = (Screen.width - anchoModal) * 0.5f;
            float yModal = (Screen.height - altoModal) * 0.5f;

            GUILayout.BeginArea(new Rect(xModal, yModal, anchoModal, altoModal), estiloModal);
            GUILayout.Space(14);

            GUILayout.Label("🏛️ DOLORES HIDALGO, CUNA DE LA LIBERTAD", estiloTitulo);
            GUILayout.Label("Elige una misión para participar en los hitos históricos y tradiciones dolorenses", estiloSubtitulo);
            GUILayout.Space(12);

            // Verificación del estado de las misiones
            bool m1Hecha = ProgresoJuegoManager.Instance != null && ProgresoJuegoManager.Instance.EstaMisionCompletada(TipoMision.Dolores_TowerDefense);
            bool m2Hecha = ProgresoJuegoManager.Instance != null && ProgresoJuegoManager.Instance.EstaMisionCompletada(TipoMision.Dolores_Nieves);
            bool m3Hecha = ProgresoJuegoManager.Instance != null && ProgresoJuegoManager.Instance.EstaMisionCompletada(TipoMision.Dolores_Duelo);

            // Contenedor de las 3 tarjetas de misión
            GUILayout.BeginHorizontal();

            // TARJETA 1: DEFESNSA DEL ATRIO (Tower Defense)
            DibujarTarjetaMision(
                "NIVEL 1: DEFENSA DEL ATRIO",
                "Tower Defense 2D Activo",
                "Defiende la Antorcha Libertaria de 5 oleadas de invasores nocturnos colocando barricadas y honderos en la plaza parroquial.",
                "🎖️ Medalla: Emblema de la Libertad",
                m1Hecha,
                true,
                () => CargarEscena(escenaMision1)
            );

            GUILayout.Space(10);

            // TARJETA 2: DESAFÍO DE LAS NIEVES (Gestión culinaria / QTE)
            DibujarTarjetaMision(
                "NIVEL 2: PUESTO DE NIEVES",
                "Gestión de Tiempo y QTE",
                "Asiste al maestro Don Chon en el quiosco tradicional preparando nieves exóticas (garambullo, borrachita, queso y aguacate).",
                "🍨 Medalla: Cuchara Dorada Artesanal",
                m2Hecha,
                true, // Siempre disponible para jugar
                () => CargarEscena(escenaMision2)
            );

            GUILayout.Space(10);

            // TARJETA 3: DUELO EN LA PLAZA (Próxima misión)
            DibujarTarjetaMision(
                "NIVEL 3: DUELO EN LA PLAZA",
                "Combate Rítmico y Destreza",
                "Enfrentamiento amistoso frente a los mejores arrieros y defensores para demostrar agilidad y obtener el salvoconducto.",
                "📜 Recompensa: Salvoconducto a San Felipe",
                m3Hecha,
                (m1Hecha || m2Hecha), // Desbloqueable al superar nivel 1 o 2
                () => CargarEscena(escenaMision3)
            );

            GUILayout.EndHorizontal();

            GUILayout.Space(16);

            // Botón de Cerrar Modal y volver al mapa
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("⬅ VOLVER AL MAPA GENERAL", estiloBotonCerrar, GUILayout.Width(240), GUILayout.Height(38)))
            {
                if (AudioColonialManager.Instance != null) AudioColonialManager.Instance.ReproducirClickBoton();
                Cerrar();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.EndArea();
        }

        private void DibujarTarjetaMision(string titulo, string genero, string descripcion, string recompensa, bool completada, bool desbloqueada, System.Action onJugar)
        {
            float anchoTarjeta = 270;
            float altoTarjeta = 380;

            GUILayout.BeginVertical(estiloTarjetaMision, GUILayout.Width(anchoTarjeta), GUILayout.Height(altoTarjeta));
            GUILayout.Space(8);

            // Estado encabezado
            if (completada)
            {
                GUILayout.Label("⭐ ¡MISIÓN SUPERADA!", estiloTituloMision);
            }
            else if (!desbloqueada)
            {
                GUILayout.Label("🔒 BLOQUEADA", estiloDescMision);
            }
            else
            {
                GUILayout.Label("⚔ DISPONIBLE", estiloTituloMision);
            }

            GUILayout.Label(titulo, estiloTituloMision);
            GUILayout.Label($"<i>{genero}</i>", estiloDescMision);
            GUILayout.Space(8);

            GUILayout.Label(descripcion, estiloDescMision, GUILayout.Height(100));
            GUILayout.Space(6);

            GUILayout.Label(recompensa, estiloDescMision);
            GUILayout.FlexibleSpace();

            // Botón de acción
            if (!desbloqueada)
            {
                GUILayout.Button("🔒 Requiere Nivel 1 o 2", estiloBotonBloqueado, GUILayout.Height(42));
            }
            else if (completada)
            {
                if (GUILayout.Button("⭐ REPETIR MISIÓN", estiloBotonCompletado, GUILayout.Height(42)))
                {
                    if (AudioColonialManager.Instance != null) AudioColonialManager.Instance.ReproducirClickBoton();
                    onJugar?.Invoke();
                }
            }
            else
            {
                if (GUILayout.Button("▶ ¡JUGAR AHORA!", estiloBotonJugar, GUILayout.Height(42)))
                {
                    if (AudioColonialManager.Instance != null) AudioColonialManager.Instance.ReproducirClickBoton();
                    onJugar?.Invoke();
                }
            }

            GUILayout.Space(6);
            GUILayout.EndVertical();
        }

        private void CargarEscena(string nombreEscena)
        {
            try
            {
                Debug.Log($"<color=green>[Selector]</color> Cargando misión en escena: {nombreEscena}");
                Time.timeScale = 1f;
                SceneManager.LoadScene(nombreEscena);
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"[Selector] La escena '{nombreEscena}' aún no está registrada en Build Settings: {ex.Message}");
            }
        }
    }
}
