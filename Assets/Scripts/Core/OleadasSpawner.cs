using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CaminosDeGuanajuato.Core
{
    /// <summary>
    /// Generador de oleadas de prueba para la mision del Atrio Parroquial.
    /// Permite invocar oleadas manualmente (con tecla [G] o boton en pantalla)
    /// para comprobar y calibrar la colocacion de defensas y su eficacia.
    /// </summary>
    public class OleadasSpawner : MonoBehaviour
    {
        public static OleadasSpawner Instance { get; private set; }

        [Header("Configuracion de Oleadas")]
        [SerializeField] private GameObject prefabInvasor;
        [SerializeField] private int oleadaActual = 0;
        [SerializeField] private int totalOleadas = 5;
        [SerializeField] private int enemigosPorOleadaBase = 4;
        [SerializeField] private float intervaloEntreEnemigos = 1.5f;

        [Header("Ruta de los Invasores")]
        [SerializeField] private Transform puntoSpawn;
        [SerializeField] private List<Transform> waypoints = new List<Transform>();
        [SerializeField] private Transform antorchaObjetivo;

        [Header("Estado")]
        [SerializeField] private bool oleadaEnProgreso;
        [SerializeField] private int enemigosVivos;

        public int OleadaActual => oleadaActual;
        public int TotalOleadas => totalOleadas;
        public bool OleadaEnProgreso => oleadaEnProgreso;
        public int EnemigosVivos => enemigosVivos;

        public event Action<int> OnOleadaIniciada;
        public event Action<int> OnOleadaCompletada;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
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
            // Atajo de teclado 'G' para iniciar o adelantar una oleada de prueba rapidamente
            if (Input.GetKeyDown(KeyCode.G))
            {
                if (!oleadaEnProgreso)
                {
                    IniciarSiguienteOleada();
                }
            }
        }

        public void IniciarSiguienteOleada()
        {
            if (oleadaEnProgreso) return;

            oleadaActual++;
            oleadaEnProgreso = true;
            OnOleadaIniciada?.Invoke(oleadaActual);

            int cantidadEnemigos = enemigosPorOleadaBase + (oleadaActual - 1) * 2;
            StartCoroutine(RutinaSpawnOleada(cantidadEnemigos));
        }

        private IEnumerator RutinaSpawnOleada(int cantidad)
        {
            enemigosVivos = cantidad;

            for (int i = 0; i < cantidad; i++)
            {
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
                Debug.Log($"[OleadasSpawner] ¡Oleada {oleadaActual} superada con exito!");
            }
        }
    }
}
