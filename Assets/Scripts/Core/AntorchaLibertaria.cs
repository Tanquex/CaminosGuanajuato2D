using System;
using UnityEngine;

namespace CaminosDeGuanajuato.Core
{
    /// <summary>
    /// Objetivo central de la mision 1 en Dolores Hidalgo: La Antorcha Libertaria.
    /// Si los invasores nocturnos logran danarla, su fuego mengua. Guanajicho
    /// y las defensas populares deben protegerla durante las oleadas.
    /// </summary>
    public class AntorchaLibertaria : MonoBehaviour
    {
        public static AntorchaLibertaria Instance { get; private set; }

        [Header("Vida de la Antorcha")]
        [SerializeField] private int saludMaxima = 100;
        [SerializeField] private int saludActual;

        [Header("Efectos Visuales")]
        [SerializeField] private SpriteRenderer spriteFuego;
        [SerializeField] private Color colorNormal = Color.white;
        [SerializeField] private Color colorDanio = Color.red;

        public int SaludMaxima => saludMaxima;
        public int SaludActual => saludActual;
        public float PorcentajeSalud => (float)saludActual / saludMaxima;
        public bool EstaExtinguida => saludActual <= 0;

        public event Action<int, int> OnSaludCambiada;
        public event Action OnAntorchaExtinguida;

        private Vector3 escalaBaseFuego = Vector3.one;
        private bool estaParpadeandoDanio = false;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }
            Instance = this;
            saludActual = saludMaxima;
            if (spriteFuego == null) spriteFuego = GetComponent<SpriteRenderer>();
            if (spriteFuego != null) escalaBaseFuego = spriteFuego.transform.localScale;
        }

        private void Start()
        {
            OnSaludCambiada?.Invoke(saludActual, saludMaxima);
        }

        private void Update()
        {
            if (EstaExtinguida || spriteFuego == null) return;

            // Crepitar y oscilar sutilmente la llama libertaria
            float ruido = Mathf.PerlinNoise(Time.time * 7f, 0.5f);
            float factorEscala = 1f + (ruido - 0.5f) * 0.12f;
            spriteFuego.transform.localScale = escalaBaseFuego * factorEscala;

            if (!estaParpadeandoDanio)
            {
                // Variación sutil entre dorado cálido y ámbar de fuego insurgente
                float tinte = Mathf.Sin(Time.time * 5f) * 0.5f + 0.5f;
                Color fuegoVivo = Color.Lerp(new Color(1f, 0.92f, 0.5f, 1f), new Color(1f, 0.68f, 0.22f, 1f), tinte);
                spriteFuego.color = fuegoVivo;
            }
        }

        /// <summary>
        /// Recibe danio cuando los invasores impactan contra la antorcha.
        /// </summary>
        public void RecibirDanio(int cantidad)
        {
            if (saludActual <= 0) return;

            saludActual = Mathf.Max(0, saludActual - cantidad);
            OnSaludCambiada?.Invoke(saludActual, saludMaxima);

            // Efecto visual de parpadeo
            if (spriteFuego != null)
            {
                StopAllCoroutines();
                StartCoroutine(ParpadeoDanio());
            }

            if (saludActual <= 0)
            {
                OnAntorchaExtinguida?.Invoke();
                Debug.LogWarning("[Antorcha Libertaria] ¡La antorcha ha sido extinguida por los invasores!");
            }
        }

        public void Reparar(int cantidad)
        {
            saludActual = Mathf.Min(saludMaxima, saludActual + cantidad);
            OnSaludCambiada?.Invoke(saludActual, saludMaxima);
        }

        private System.Collections.IEnumerator ParpadeoDanio()
        {
            estaParpadeandoDanio = true;
            spriteFuego.color = colorDanio;
            yield return new WaitForSeconds(0.12f);
            estaParpadeandoDanio = false;
        }
    }
}
