using System;
using UnityEngine;

namespace CaminosDeGuanajuato.Core
{
    public enum TipoMision
    {
        Dolores_TowerDefense = 1, // Misión 1: La Defensa del Atrio Parroquial
        Dolores_Nieves = 2,       // Misión 2: El Desafío de las Nieves Tradicionales
        Dolores_Duelo = 3         // Misión 3: Duelo de Destreza en la Plaza
    }

    /// <summary>
    /// Administrador central de persistencia y progreso del jugador en Guanajuato.
    /// Almacena el estado de las misiones en PlayerPrefs según la arquitectura del GDD:
    /// - Dolores_TowerDefense (0 o 1) -> Medalla 1: Emblema de la Libertad
    /// - Dolores_Nieves (0 o 1) -> Medalla 2: Cuchara Dorada Artesanal
    /// - Dolores_Duelo (0 o 1) -> Desbloqueo del Salvoconducto a San Felipe
    /// </summary>
    public class ProgresoJuegoManager : MonoBehaviour
    {
        public static ProgresoJuegoManager Instance { get; private set; }

        public const string KEY_MISION1 = "Dolores_TowerDefense";
        public const string KEY_MISION2 = "Dolores_Nieves";
        public const string KEY_MISION3 = "Dolores_Duelo";
        public const string KEY_RECORD_NIEVES = "Record_Puntos_Nieves";

        public event Action OnProgresoActualizado;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }
            Instance = this;
            if (transform.parent == null)
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
            return ObtenerTotalCompletadasDolores() >= 3;
        }

        /// <summary>
        /// Comprueba si una misión está desbloqueada y lista para jugar.
        /// Misión 1: Siempre disponible.
        /// Misión 2: Disponible de inmediato o tras Misión 1 (según el GDD ambas son locales).
        /// Misión 3: Requiere haber completado al menos la Misión 1 o 2.
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
                default:
                    return false;
            }
        }

        public int ObtenerRecordNieves()
        {
            return PlayerPrefs.GetInt(KEY_RECORD_NIEVES, 0);
        }

        /// <summary>
        /// Método de utilidad para pruebas de reseteo de progreso.
        /// </summary>
        public void ReiniciarProgresoDolores()
        {
            PlayerPrefs.DeleteKey(KEY_MISION1);
            PlayerPrefs.DeleteKey(KEY_MISION2);
            PlayerPrefs.DeleteKey(KEY_MISION3);
            PlayerPrefs.DeleteKey(KEY_RECORD_NIEVES);
            PlayerPrefs.Save();
            Debug.Log("<color=yellow>[Progreso]</color> Progreso de Dolores Hidalgo reiniciado a 0/3.");
            OnProgresoActualizado?.Invoke();
        }

        private string ObtenerClaveMision(TipoMision mision)
        {
            switch (mision)
            {
                case TipoMision.Dolores_TowerDefense: return KEY_MISION1;
                case TipoMision.Dolores_Nieves: return KEY_MISION2;
                case TipoMision.Dolores_Duelo: return KEY_MISION3;
                default: return KEY_MISION1;
            }
        }
    }
}
