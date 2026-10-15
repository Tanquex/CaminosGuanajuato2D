using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CaminosDeGuanajuato.Core
{
    /// <summary>
    /// Generador de oleadas para la mision del Atrio Parroquial.
    /// Soporta inicio automatico tras tiempo de preparacion (cuenta regresiva),
    /// avance manual con tecla [G] y deteccion de victoria/derrota.
    /// </summary>
    public class OleadasSpawner : MonoBehaviour
    {
        public static OleadasSpawner Instance { get; private set; }

        [Header("Configuracion de Oleadas")]
        [SerializeField] private GameObject prefabInvasor;
        [SerializeField] private int oleadaActual = 0;
        [SerializeField] private int totalOleadas = 5;
        [SerializeField] private int enemigosPorOleadaBase = 3;
        [SerializeField] private float intervaloEntreEnemigos = 1.4f;

        [Header("Automatizacion y Tiempos")]
        [Tooltip("Indica si las oleadas inician solas tras el temporizador")]
        [SerializeField] private bool inicioAutomatico = true;

        [Tooltip("Tiempo de preparacion antes de la primera oleada (segundos)")]
        [SerializeField] private float tiempoPreparacionInicial = 10.0f;

        [Tooltip("Tiempo de descanso entre oleadas completadas (segundos)")]
        [SerializeField] private float tiempoEntreOleadas = 8.0f;

        [Header("Ruta de los Invasores")]
        [SerializeField] private Transform puntoSpawn;
        [SerializeField] private List<Transform> waypoints = new List<Transform>();
        [SerializeField] private Transform antorchaObjetivo;

        [Header("Estado")]
        [SerializeField] private bool oleadaEnProgreso;
        [SerializeField] private int enemigosVivos;
        [SerializeField] private float temporizadorCuentaRegresiva;
        [SerializeField] private bool todasOleadasCompletadas;

        public int OleadaActual => oleadaActual;
        public int TotalOleadas => totalOleadas;
        public bool OleadaEnProgreso => oleadaEnProgreso;
        public int EnemigosVivos => enemigosVivos;
        public float TiempoRestanteCuentaRegresiva => Mathf.Max(0f, temporizadorCuentaRegresiva);
        public bool EstaEnCuentaRegresiva => !oleadaEnProgreso && !todasOleadasCompletadas && inicioAutomatico && (oleadaActual < totalOleadas);
        public bool TodasOleadasCompletadas => todasOleadasCompletadas;

        public event Action<int> OnOleadaIniciada;
        public event Action<int> OnOleadaCompletada;
        public event Action OnTodasOleadasSuperadas;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            temporizadorCuentaRegresiva = tiempoPreparacionInicial;
        }

        private void Start()
        {
            if (antorchaObjetivo == null)
            {
                AntorchaLibertaria antorcha = FindAnyObjectByType<AntorchaLibertaria>();
                if (antorcha != null) antorchaObjetivo = antorcha.transform;
            }

            if (puntoSpawn == null)
            {
                puntoSpawn = transform;
            }
        }

        private void Update()
        {
            // No avanzar si la antorcha fue destruida
            if (AntorchaLibertaria.Instance != null && AntorchaLibertaria.Instance.EstaExtinguida)
            {
                return;
            }

            // Atajo de teclado 'G' para iniciar o adelantar la oleada sin esperar el temporizador
            if (Input.GetKeyDown(KeyCode.G))
            {
                if (!oleadaEnProgreso && !todasOleadasCompletadas)
                {
                    IniciarSiguienteOleada();
                }
            }

            // Logica de cuenta regresiva automatica
            if (inicioAutomatico && !oleadaEnProgreso && !todasOleadasCompletadas && oleadaActual < totalOleadas)
            {
                temporizadorCuentaRegresiva -= Time.deltaTime;
                if (temporizadorCuentaRegresiva <= 0f)
                {
                    IniciarSiguienteOleada();
                }
            }
        }

        public void IniciarSiguienteOleada()
        {
            if (oleadaEnProgreso || todasOleadasCompletadas) return;

            oleadaActual++;
            oleadaEnProgreso = true;
            OnOleadaIniciada?.Invoke(oleadaActual);

            int cantidadEnemigos = enemigosPorOleadaBase + (oleadaActual - 1) * 2;
            StartCoroutine(RutinaSpawnOleada(cantidadEnemigos));
            Debug.Log($"<color=orange>[Oleadas]</color> ¡Iniciando Oleada {oleadaActual} de {totalOleadas}! ({cantidadEnemigos} invasores)");
        }

        private IEnumerator RutinaSpawnOleada(int cantidad)
        {
            enemigosVivos = cantidad;

            for (int i = 0; i < cantidad; i++)
            {
                // Si la antorcha cayo mientras spawnean, detener el spawn
                if (AntorchaLibertaria.Instance != null && AntorchaLibertaria.Instance.EstaExtinguida)
                {
                    yield break;
                }

                if (prefabInvasor != null && puntoSpawn != null)
                {
                    GameObject go = Instantiate(prefabInvasor, puntoSpawn.position, Quaternion.identity);
                    InvasorNocturno invasor = go.GetComponent<InvasorNocturno>();
                    if (invasor != null)
                    {
                        invasor.ConfigurarRuta(waypoints, antorchaObjetivo);
                        invasor.OnInvasorDerrotado += ManejarInvasorDerrotado;
                    }
                }

                yield return new WaitForSeconds(intervaloEntreEnemigos);
            }
        }

        private void ManejarInvasorDerrotado(InvasorNocturno invasor)
        {
            enemigosVivos = Mathf.Max(0, enemigosVivos - 1);

            if (enemigosVivos == 0 && oleadaEnProgreso)
            {
                oleadaEnProgreso = false;
                OnOleadaCompletada?.Invoke(oleadaActual);

                if (oleadaActual >= totalOleadas)
                {
                    todasOleadasCompletadas = true;
                    OnTodasOleadasSuperadas?.Invoke();
                    Debug.Log("<color=green>[Oleadas]</color> ⭐ ¡TODAS LAS OLEADAS COMPLETADAS! ¡VICTORIA!");
                }
                else
                {
                    // Reiniciar cuenta regresiva para la siguiente oleada
                    temporizadorCuentaRegresiva = tiempoEntreOleadas;
                    Debug.Log($"<color=yellow>[Oleadas]</color> Oleada {oleadaActual} superada. Siguiente oleada en {tiempoEntreOleadas}s.");
                }
            }
        }
    }
}
