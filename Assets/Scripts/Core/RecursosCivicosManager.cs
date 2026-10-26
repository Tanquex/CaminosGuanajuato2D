using System;
using UnityEngine;

namespace CaminosDeGuanajuato.Core
{
    /// <summary>
    /// Administrador del sistema de Recursos Civicos (Puntos Civicos).
    /// En el GDD (Mision 1: Defensa del Atrio Parroquial), el jugador acumula
    /// recursos civicos para convocar y colocar unidades defensivas populares.
    /// </summary>
    public class RecursosCivicosManager : MonoBehaviour
    {
        public static RecursosCivicosManager Instance { get; private set; }

        [Header("Configuracion de Recursos")]
        [Tooltip("Puntos civicos iniciales al comenzar la mision")]
        [SerializeField] private int recursosIniciales = 100;

        [Tooltip("Generacion pasiva de recursos civicos (apoyo del pueblo)")]
        [SerializeField] private bool generacionPasiva = true;
        [SerializeField] private int puntosPorSegundo = 2;

        [Header("Estado Actual")]
        [SerializeField] private int recursosActuales;

        private float temporizadorPasivo;

        // Eventos para actualizar la interfaz grafica (UI) y otros sistemas
        public event Action<int> OnRecursosCambiados;

        public int RecursosActuales => recursosActuales;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }
            Instance = this;
            recursosActuales = recursosIniciales;
        }

        private void Start()
        {
            NotificarCambioRecursos();
        }

        private void Update()
        {
            if (generacionPasiva)
            {
                temporizadorPasivo += Time.deltaTime;
                if (temporizadorPasivo >= 1.0f)
                {
                    temporizadorPasivo = 0f;
                    AgregarRecursos(puntosPorSegundo);
                }
            }
        }

        /// <summary>
        /// Comprueba si el jugador dispone de suficientes recursos para costear una defensa.
        /// </summary>
        public bool PuedeCostear(int costo)
        {
            return recursosActuales >= costo;
        }

        /// <summary>
        /// Gasta los recursos para colocar una defensa. Devuelve true si la operacion fue exitosa.
        /// </summary>
        public bool GastarRecursos(int costo)
        {
            if (!PuedeCostear(costo))
            {
                return false;
            }

            recursosActuales -= costo;
            NotificarCambioRecursos();
            return true;
        }

        /// <summary>
        /// Suma recursos civicos (por ejemplo al vencer un invasor nocturno o completar tareas).
        /// </summary>
        public void AgregarRecursos(int cantidad)
        {
            if (cantidad <= 0) return;

            recursosActuales += cantidad;
            NotificarCambioRecursos();
        }

        private void NotificarCambioRecursos()
        {
            OnRecursosCambiados?.Invoke(recursosActuales);
        }
    }
}
