using System;
using UnityEngine;

namespace CaminosDeGuanajuato.Core
{
    public enum TipoMision
    {
        Dolores_TowerDefense = 1, // Misión 1: La Defensa del Atrio Parroquial
        Dolores_Nieves = 2,       // Misión 2: El Desafío de las Nieves Tradicionales
        Dolores_Duelo = 3,        // Misión 3: Duelo de Sones en la Plaza
        SanFelipe_Alfareria = 4   // Misión 4: El Taller Alfarero de la Francia Chiquita (San Felipe Torres Mochas)
    }

    /// <summary>
    /// Administrador central de persistencia y progreso del jugador en Guanajuato.
    /// Almacena el estado de las misiones en PlayerPrefs según la arquitectura del GDD:
    /// - Dolores_TowerDefense (0 o 1) -> Medalla 1: Emblema de la Libertad
    /// - Dolores_Nieves (0 o 1) -> Medalla 2: Cuchara Dorada Artesanal
    /// - Dolores_Duelo (0 o 1) -> Desbloqueo del Salvoconducto a San Felipe
    /// - SanFelipe_Alfareria (0 o 1) -> Medalla 4: Sello Real de las Torres Mochas
    /// </summary>
    public class ProgresoJuegoManager : MonoBehaviour
    {
        public static ProgresoJuegoManager Instance { get; private set; }

        public const string KEY_MISION1 = "Dolores_TowerDefense";
        public const string KEY_MISION2 = "Dolores_Nieves";
        public const string KEY_MISION3 = "Dolores_Duelo";
        public const string KEY_MISION4 = "SanFelipe_Alfareria";
        public const string KEY_RECORD_NIEVES = "Record_Puntos_Nieves";
        public const string KEY_RECORD_ALFARERIA = "Record_Puntos_Alfareria";

        public event Action OnProgresoActualizado;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }
            Instance = this;
            // Solo marcar DontDestroyOnLoad si es un objeto raíz dedicado exclusivamente a ProgresoJuegoManager.
            // Si comparte GameObject con managers de escena (como NeveriaManagerCore), NO preservar el GameObject entero.
            if (transform.parent == null && GetComponents<MonoBehaviour>().Length <= 1)
            {
                DontDestroyOnLoad(gameObject);
            }
        }

        /// <summary>
        /// Comprueba si una misión específica ya fue superada con éxito.
        /// </summary>
        public bool EstaMisionCompletada(TipoMision mision)
        {
            string clave = ObtenerClaveMision(mision);
            return PlayerPrefs.GetInt(clave, 0) == 1;
        }

        /// <summary>
        /// Registra la victoria en una misión, guarda en disco y emite evento.
        /// </summary>
        public void RegistrarMisionCompletada(TipoMision mision, int puntaje = 0)
        {
            string clave = ObtenerClaveMision(mision);
            PlayerPrefs.SetInt(clave, 1);

            if (mision == TipoMision.Dolores_Nieves)
            {
                int recordPrevio = PlayerPrefs.GetInt(KEY_RECORD_NIEVES, 0);
                if (puntaje > recordPrevio)
                {
                    PlayerPrefs.SetInt(KEY_RECORD_NIEVES, puntaje);
                }
            }
            else if (mision == TipoMision.SanFelipe_Alfareria)
            {
                int recordPrevio = PlayerPrefs.GetInt(KEY_RECORD_ALFARERIA, 0);
                if (puntaje > recordPrevio)
                {
                    PlayerPrefs.SetInt(KEY_RECORD_ALFARERIA, puntaje);
                }
            }

            PlayerPrefs.Save();
            Debug.Log($"<color=green>[Progreso]</color> ¡Misión {mision} registrada como COMPLETADA! Total Dolores: {ObtenerTotalCompletadasDolores()}/3");
            OnProgresoActualizado?.Invoke();
        }

        /// <summary>
        /// Obtiene la cantidad de misiones completadas en Dolores Hidalgo (de 0 a 3).
        /// </summary>
        public int ObtenerTotalCompletadasDolores()
        {
            int m1 = PlayerPrefs.GetInt(KEY_MISION1, 0);
            int m2 = PlayerPrefs.GetInt(KEY_MISION2, 0);
            int m3 = PlayerPrefs.GetInt(KEY_MISION3, 0);
            return m1 + m2 + m3;
        }

        /// <summary>
        /// El acceso al municipio norte (San Felipe) se desbloquea al superar las 3 misiones de Dolores.
        /// </summary>
        public bool EstaDesbloqueadoSanFelipe()
        {
            return ObtenerTotalCompletadasDolores() >= 3 || EstaMisionCompletada(TipoMision.Dolores_Duelo);
        }

        /// <summary>
        /// Comprueba si una misión está desbloqueada y lista para jugar.
        /// </summary>
        public bool EstaMisionDesbloqueada(TipoMision mision)
        {
            switch (mision)
            {
                case TipoMision.Dolores_TowerDefense:
                    return true;
                case TipoMision.Dolores_Nieves:
                    return true; // Abierta para el festival de nieves
                case TipoMision.Dolores_Duelo:
                    return EstaMisionCompletada(TipoMision.Dolores_TowerDefense) || EstaMisionCompletada(TipoMision.Dolores_Nieves);
                case TipoMision.SanFelipe_Alfareria:
                    return EstaDesbloqueadoSanFelipe();
                default:
                    return false;
            }
        }

        public int ObtenerRecordNieves()
        {
            return PlayerPrefs.GetInt(KEY_RECORD_NIEVES, 0);
        }

        public int ObtenerRecordAlfareria()
        {
            return PlayerPrefs.GetInt(KEY_RECORD_ALFARERIA, 0);
        }

        /// <summary>
        /// Método de utilidad para pruebas de reseteo de progreso.
        /// </summary>
        public void ReiniciarProgresoDolores()
        {
            PlayerPrefs.DeleteKey(KEY_MISION1);
            PlayerPrefs.DeleteKey(KEY_MISION2);
            PlayerPrefs.DeleteKey(KEY_MISION3);
            PlayerPrefs.DeleteKey(KEY_MISION4);
            PlayerPrefs.DeleteKey(KEY_RECORD_NIEVES);
            PlayerPrefs.DeleteKey(KEY_RECORD_ALFARERIA);
            PlayerPrefs.Save();
            Debug.Log("<color=yellow>[Progreso]</color> Progreso de Guanajuato reiniciado.");
            OnProgresoActualizado?.Invoke();
        }

        private string ObtenerClaveMision(TipoMision mision)
        {
            switch (mision)
            {
                case TipoMision.Dolores_TowerDefense: return KEY_MISION1;
                case TipoMision.Dolores_Nieves: return KEY_MISION2;
                case TipoMision.Dolores_Duelo: return KEY_MISION3;
                case TipoMision.SanFelipe_Alfareria: return KEY_MISION4;
                default: return KEY_MISION1;
            }
        }
    }
}
