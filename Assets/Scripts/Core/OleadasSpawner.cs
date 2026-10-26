using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CaminosDeGuanajuato.Core
{
    [System.Serializable]
    public class RutaInvasionData
    {
        public string nombre = "Entrada Oeste";
        public Vector3 puntoSpawn;
        public List<Vector3> waypoints = new List<Vector3>();

        public RutaInvasionData(string nombreRuta, Vector3 spawn, params Vector3[] rutaPuntos)
        {
            nombre = nombreRuta;
            puntoSpawn = spawn;
            waypoints = new List<Vector3>(rutaPuntos);
        }
    }

    /// <summary>
    /// Generador de oleadas para la mision del Atrio Parroquial.
    /// Soporta inicio automatico tras tiempo de preparacion (cuenta regresiva),
    /// avance manual con tecla [G], deteccion de victoria/derrota
    /// y generacion de enemigos desde multiples entradas (Norte, Sur, Este y Oeste).
    /// </summary>
    public class OleadasSpawner : MonoBehaviour
    {
        public static OleadasSpawner Instance { get; private set; }

        [Header("Configuracion de Oleadas")]
        [SerializeField] private GameObject prefabInvasor;
        [SerializeField] private int oleadaActual = 0;
        [SerializeField] private int totalOleadas = 5;
        [SerializeField] private int enemigosPorOleadaBase = 3;
        [SerializeField] private float intervaloEntreEnemigos = 1.3f;

        [Header("Automatizacion y Tiempos")]
        [Tooltip("Indica si las oleadas inician solas tras el temporizador")]
        [SerializeField] private bool inicioAutomatico = true;

        [Tooltip("Tiempo de preparacion antes de la primera oleada (segundos)")]
        [SerializeField] private float tiempoPreparacionInicial = 10.0f;

        [Tooltip("Tiempo de descanso entre oleadas completadas (segundos)")]
        [SerializeField] private float tiempoEntreOleadas = 8.0f;

        [Header("Ruta de los Invasores (Legado)")]
        [SerializeField] private Transform puntoSpawn;
        [SerializeField] private List<Transform> waypoints = new List<Transform>();
        [SerializeField] private Transform antorchaObjetivo;

        [Header("Rutas de Invasión Multidireccionales")]
        [SerializeField] private List<RutaInvasionData> rutasInvasion = new List<RutaInvasionData>();

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
        public static event Action<string> OnAvisoInvasion;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }
            Instance = this;
            temporizadorCuentaRegresiva = tiempoPreparacionInicial;

            InicializarRutasMultidireccionales();
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

        /// <summary>
        /// Inicializa 4 rutas auténticas de aproximación al Atrio Parroquial de Dolores Hidalgo:
        /// 1. Callejón del Campanario (Oeste)
        /// 2. Jardín Hidalgo y Huertos (Este)
        /// 3. Calle Principal / Frontispicio (Norte)
        /// 4. Portal del Comercio / Plaza Central (Sur)
        /// </summary>
        private void InicializarRutasMultidireccionales()
        {
            if (rutasInvasion != null && rutasInvasion.Count > 0) return;

            rutasInvasion = new List<RutaInvasionData>
            {
                // Ruta 1: Oeste (Callejón del Campanario)
                new RutaInvasionData(
                    "Callejón del Campanario (Oeste)",
                    new Vector3(-10.5f, -1.8f, 0f),
                    new Vector3(-5.5f, -1.8f, 0f),
                    new Vector3(-1.0f, -1.8f, 0f),
                    new Vector3(0f, 2.2f, 0f)
                ),

                // Ruta 2: Este (Jardín Hidalgo / Huertos)
                new RutaInvasionData(
                    "Jardín Hidalgo (Este)",
                    new Vector3(10.5f, -1.8f, 0f),
                    new Vector3(5.5f, -1.8f, 0f),
                    new Vector3(1.0f, -1.8f, 0f),
                    new Vector3(0f, 2.2f, 0f)
                ),

                // Ruta 3: Norte (Calle Principal / Frontispicio)
                new RutaInvasionData(
                    "Calle Principal (Norte)",
                    new Vector3(0f, 8.5f, 0f),
                    new Vector3(0f, 5.0f, 0f),
                    new Vector3(0f, 2.5f, 0f)
                ),

                // Ruta 4: Sur (Portal del Comercio / Plaza Central)
                new RutaInvasionData(
                    "Portal del Comercio (Sur)",
                    new Vector3(0f, -7.5f, 0f),
                    new Vector3(0f, -3.5f, 0f),
                    new Vector3(0f, 0.2f, 0f),
                    new Vector3(0f, 2.2f, 0f)
                )
            };
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

            if (AudioColonialManager.Instance != null)
            {
                AudioColonialManager.Instance.ReproducirCampanaGritoLibertad();
                AudioColonialManager.Instance.ReproducirAlertaInvasion();
            }

            OnOleadaIniciada?.Invoke(oleadaActual);

            int cantidadEnemigos = enemigosPorOleadaBase + (oleadaActual - 1) * 2;
            StartCoroutine(RutinaSpawnOleada(cantidadEnemigos));
            Debug.Log($"<color=orange>[Oleadas]</color> ¡Iniciando Oleada {oleadaActual} de {totalOleadas}! ({cantidadEnemigos} invasores)");
        }

        private IEnumerator RutinaSpawnOleada(int cantidad)
        {
            enemigosVivos = cantidad;

            // Determinar rutas activas según la oleada
            List<RutaInvasionData> rutasDisponibles = ObtenerRutasActivasParaOleada(oleadaActual);

            string avisoTexto = GenerarTextoAviso(rutasDisponibles);
            OnAvisoInvasion?.Invoke(avisoTexto);
            Debug.Log($"<color=red>[Invasión]</color> {avisoTexto}");

            for (int i = 0; i < cantidad; i++)
            {
                // Si la antorcha cayo mientras spawnean, detener el spawn
                if (AntorchaLibertaria.Instance != null && AntorchaLibertaria.Instance.EstaExtinguida)
                {
                    yield break;
                }

                if (prefabInvasor != null && rutasDisponibles.Count > 0)
                {
                    // Alternar entre las rutas activas
                    RutaInvasionData rutaElegida = rutasDisponibles[i % rutasDisponibles.Count];
                    Vector3 spawnPos = rutaElegida.puntoSpawn;

                    GameObject go = Instantiate(prefabInvasor, spawnPos, Quaternion.identity);
                    InvasorNocturno invasor = go.GetComponent<InvasorNocturno>();
                    if (invasor != null)
                    {
                        invasor.ConfigurarRutaPosiciones(rutaElegida.waypoints, antorchaObjetivo);
                        invasor.OnInvasorDerrotado += ManejarInvasorDerrotado;
                    }
                }

                yield return new WaitForSeconds(intervaloEntreEnemigos);
            }
        }

        private List<RutaInvasionData> ObtenerRutasActivasParaOleada(int oleada)
        {
            List<RutaInvasionData> activas = new List<RutaInvasionData>();
            if (rutasInvasion == null || rutasInvasion.Count == 0) return activas;

            switch (oleada)
            {
                case 1:
                    // Oleada 1: Oeste y Este
                    activas.Add(rutasInvasion[0]); // Oeste
                    if (rutasInvasion.Count > 1) activas.Add(rutasInvasion[1]); // Este
                    break;
                case 2:
                    // Oleada 2: Oeste, Este y Sur
                    activas.Add(rutasInvasion[0]);
                    if (rutasInvasion.Count > 1) activas.Add(rutasInvasion[1]);
                    if (rutasInvasion.Count > 3) activas.Add(rutasInvasion[3]); // Sur
                    break;
                case 3:
                    // Oleada 3: Norte, Oeste y Este
                    if (rutasInvasion.Count > 2) activas.Add(rutasInvasion[2]); // Norte
                    activas.Add(rutasInvasion[0]);
                    if (rutasInvasion.Count > 1) activas.Add(rutasInvasion[1]);
                    break;
                default:
                    // Oleadas 4 y 5: ¡Todas las 4 rutas simultáneamente!
                    activas.AddRange(rutasInvasion);
                    break;
            }

            return activas;
        }

        private string GenerarTextoAviso(List<RutaInvasionData> rutas)
        {
            if (rutas == null || rutas.Count == 0) return "⚠️ ¡Invasores nocturnos aproximándose!";
            if (rutas.Count == 1) return $"⚠️ ¡Invasión aproximándose por: {rutas[0].nombre}!";
            if (rutas.Count == 4) return "🚨 ¡EMBOSCADA TOTAL! ¡Invasores atacando desde los 4 puntos cardinales!";

            string nombres = "";
            for (int i = 0; i < rutas.Count; i++)
            {
                nombres += (i > 0 ? " y " : "") + rutas[i].nombre;
            }
            return $"⚠️ ¡Invasores atacando desde: {nombres}!";
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
