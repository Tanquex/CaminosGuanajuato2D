using UnityEngine;
using UnityEngine.SceneManagement;
using CaminosDeGuanajuato.Core;
using CaminosDeGuanajuato.UI;

namespace CaminosDeGuanajuato.Overworld
{
    /// <summary>
    /// Tablero modal detallado de misiones para el municipio de Dolores Hidalgo C.I.N.
    /// Diseñado con estética Colonial / Talavera, presenta las 3 misiones históricas
    /// con iconos, estado de progreso, recompensas y acceso directo.
    /// </summary>
    public class SelectorMisionesUI : MonoBehaviour
    {
        [Header("Escenas de Misiones")]
        [SerializeField] private string escenaMision1 = "NivelPruebas_Defensa";
        [SerializeField] private string escenaMision2 = "Nivel2_DesafioNieves";
        [SerializeField] private string escenaMision3 = "Nivel3_DueloDestreza";

        private bool estaAbierto = false;
        public bool EstaAbierto => estaAbierto;

        private GUIStyle estiloBadgeEstado;
        private GUIStyle estiloTituloMision;
        private GUIStyle estiloGeneroMision;
        private GUIStyle estiloDescMision;
        private GUIStyle estiloRecompensa;

        private Texture2D iconoMision1;
        private Texture2D iconoMision2;
        private Texture2D iconoMision3;

        public void Abrir()
        {
            estaAbierto = true;
        }

        public void Cerrar()
        {
            estaAbierto = false;
        }

        private void InicializarEstilos()
        {
            ColonialUIHelper.AsegurarInicializacion();

            if (iconoMision1 == null) iconoMision1 = Resources.Load<Texture2D>("Icono_Resortera");
            if (iconoMision2 == null) iconoMision2 = Resources.Load<Texture2D>("Icono_VasoBarro");
            if (iconoMision3 == null) iconoMision3 = Resources.Load<Texture2D>("Icono_Corazon");

            if (estiloTituloMision != null) return;

            estiloBadgeEstado = new GUIStyle
            {
                fontSize = 11,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                richText = true
            };
            estiloBadgeEstado.normal.textColor = new Color(1.0f, 0.88f, 0.35f);

            estiloTituloMision = new GUIStyle
            {
                fontSize = 14,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                wordWrap = true,
                richText = true
            };
            estiloTituloMision.normal.textColor = new Color(0.99f, 0.82f, 0.30f);

            estiloGeneroMision = new GUIStyle
            {
                fontSize = 11,
                fontStyle = FontStyle.Italic,
                alignment = TextAnchor.MiddleCenter,
                richText = true
            };
            estiloGeneroMision.normal.textColor = new Color(0.86f, 0.78f, 0.65f);

            estiloDescMision = new GUIStyle
            {
                fontSize = 12,
                fontStyle = FontStyle.Normal,
                alignment = TextAnchor.UpperLeft,
                wordWrap = true,
                richText = true
            };
            estiloDescMision.normal.textColor = new Color(0.94f, 0.90f, 0.82f);

            estiloRecompensa = new GUIStyle
            {
                fontSize = 11,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                wordWrap = true,
                richText = true
            };
            estiloRecompensa.normal.textColor = new Color(0.98f, 0.78f, 0.28f);
        }

        public void DibujarGUI()
        {
            if (!estaAbierto) return;
            InicializarEstilos();

            ColonialUIHelper.DibujarOverlayOscuro();

            float anchoModal = Mathf.Min(860f, Screen.width - 28f);
            float altoModal = Mathf.Min(470f, Screen.height - 28f);
            Rect rectModal = new Rect((Screen.width - anchoModal) * 0.5f, (Screen.height - altoModal) * 0.5f, anchoModal, altoModal);

            ColonialUIHelper.DibujarMarcoVentana(
                rectModal,
                "🏛️ DOLORES HIDALGO, CUNA DE LA LIBERTAD",
                "Supera las 3 misiones del pueblo para obtener el Salvoconducto hacia San Felipe"
            );

            bool m1Hecha = ProgresoJuegoManager.Instance != null && ProgresoJuegoManager.Instance.EstaMisionCompletada(TipoMision.Dolores_TowerDefense);
            bool m2Hecha = ProgresoJuegoManager.Instance != null && ProgresoJuegoManager.Instance.EstaMisionCompletada(TipoMision.Dolores_Nieves);
            bool m3Hecha = ProgresoJuegoManager.Instance != null && ProgresoJuegoManager.Instance.EstaMisionCompletada(TipoMision.Dolores_Duelo);

            float padX = 18f;
            float gap = 12f;
            float wTotalTarjetas = rectModal.width - padX * 2f;
            float wTarjeta = (wTotalTarjetas - gap * 2f) / 3f;
            float yTarjeta = rectModal.y + 78f;
            float hTarjeta = rectModal.height - 142f;

            // Misión 1
            DibujarTarjetaMisionColonial(
                new Rect(rectModal.x + padX, yTarjeta, wTarjeta, hTarjeta),
                "MISIÓN 1",
                "DEFENSA DEL ATRIO",
                "Estrategia y Defensa (Tower Defense)",
                "Protege la Antorcha Libertaria frente a las oleadas nocturnas colocando barricadas y honderos insurgentes en el atrio parroquial.",
                "🎖️ Emblema de la Libertad",
                iconoMision1,
                m1Hecha,
                true,
                () => CargarEscena(escenaMision1)
            );

            // Misión 2
            DibujarTarjetaMisionColonial(
                new Rect(rectModal.x + padX + wTarjeta + gap, yTarjeta, wTarjeta, hTarjeta),
                "MISIÓN 2",
                "PUESTO DE NIEVES",
                "Tradición Culinaria y Reflejos",
                "Ayuda a Don Chon en el quiosco del jardín preparando nieves artesanales de garambullo, borrachita, queso y aguacate a tiempo.",
                "🍨 Cuchara Dorada Artesanal",
                iconoMision2,
                m2Hecha,
                true,
                () => CargarEscena(escenaMision2)
            );

            // Misión 3
            DibujarTarjetaMisionColonial(
                new Rect(rectModal.x + padX + (wTarjeta + gap) * 2f, yTarjeta, wTarjeta, hTarjeta),
                "MISIÓN 3",
                "DUELO EN LA PLAZA",
                "Destreza y Ritmo Insurgente",
                "Demuestra tu temple frente a los arrieros y guardias para recibir el Salvoconducto que abre el Camino Real del norte.",
                "📜 Salvoconducto a San Felipe",
                iconoMision3,
                m3Hecha,
                (m1Hecha || m2Hecha),
                () => CargarEscena(escenaMision3)
            );

            // Botón inferior Volver al Mapa
            float wCerrar = 230f;
            float hCerrar = 36f;
            Rect rCerrar = new Rect(
                rectModal.x + (rectModal.width - wCerrar) * 0.5f,
                rectModal.yMax - hCerrar - 14f,
                wCerrar,
                hCerrar
            );

            if (ColonialUIHelper.BotonColonial(rCerrar, "⬅ VOLVER AL MAPA REGIONAL", ColonialUIHelper.VarianteBoton.Madera, 13))
            {
                Cerrar();
            }
        }

        private void DibujarTarjetaMisionColonial(
            Rect rect,
            string numeroMision,
            string titulo,
            string genero,
            string descripcion,
            string recompensa,
            Texture2D icono,
            bool completada,
            bool desbloqueada,
            System.Action onJugar)
        {
            ColonialUIHelper.DibujarTarjetaInterior(rect);

            float xIn = rect.x + 12f;
            float wIn = rect.width - 24f;
            float yCur = rect.y + 10f;

            // Encabezado de estado
            string textoEstado = completada
                ? $"⭐ {numeroMision} • ¡COMPLETADA!"
                : (desbloqueada ? $"⚔ {numeroMision} • DISPONIBLE" : $"🔒 {numeroMision} • BLOQUEADA");

            estiloBadgeEstado.normal.textColor = completada
                ? new Color(1.0f, 0.86f, 0.28f)
                : (desbloqueada ? new Color(0.45f, 0.95f, 0.55f) : new Color(0.75f, 0.70f, 0.65f));

            GUI.Label(new Rect(xIn, yCur, wIn, 18f), textoEstado, estiloBadgeEstado);
            yCur += 20f;

            // Icono + Título
            if (icono != null)
            {
                Rect rIcono = new Rect(rect.x + (rect.width - 32f) * 0.5f, yCur, 32f, 32f);
                GUI.DrawTexture(rIcono, icono, ScaleMode.ScaleToFit);
                yCur += 34f;
            }

            GUI.Label(new Rect(xIn, yCur, wIn, 22f), titulo, estiloTituloMision);
            yCur += 20f;

            GUI.Label(new Rect(xIn, yCur, wIn, 18f), genero, estiloGeneroMision);
            yCur += 20f;

            ColonialUIHelper.DibujarSeparador(xIn + 8f, yCur, wIn - 16f);
            yCur += 8f;

            // Descripción
            float hDesc = Mathf.Max(60f, rect.yMax - yCur - 78f);
            GUI.Label(new Rect(xIn, yCur, wIn, hDesc), descripcion, estiloDescMision);

            // Recompensa
            Rect rRec = new Rect(xIn, rect.yMax - 72f, wIn, 20f);
            GUI.Label(rRec, recompensa, estiloRecompensa);

            // Botón de acción
            Rect rBtn = new Rect(xIn, rect.yMax - 46f, wIn, 36f);
            if (!desbloqueada)
            {
                ColonialUIHelper.BotonColonial(rBtn, "🔒 Supera Misión 1 o 2", ColonialUIHelper.VarianteBoton.Gris, 12);
            }
            else if (completada)
            {
                if (ColonialUIHelper.BotonColonial(rBtn, "⭐ VOLVER A JUGAR", ColonialUIHelper.VarianteBoton.Dorado, 13))
                {
                    onJugar?.Invoke();
                }
            }
            else
            {
                if (ColonialUIHelper.BotonColonial(rBtn, "▶ ¡JUGAR AHORA!", ColonialUIHelper.VarianteBoton.Verde, 13))
                {
                    onJugar?.Invoke();
                }
            }
        }

        public void CargarEscena(string nombreEscena)
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
