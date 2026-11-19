using System;
using System.Collections;
using UnityEngine;

namespace CaminosDeGuanajuato.Duelo
{
    /// <summary>
    /// Representa al contrincante musical en el lado derecho de la tarima durante el Duelo de Sones (Nivel 3).
    /// Marca el compás según el BPM del son, reacciona a los combos de Guanajicho y muestra su retrato en los diálogos.
    /// </summary>
    public class RivalDueloIA : MonoBehaviour
    {
        [Header("Datos del Duelista")]
        [SerializeField] private string nombreRival = "Duelista";
        [SerializeField] private string tituloRival = "Músico de la Plaza";
        [SerializeField] private string nombreSon = "Son Tradicional";
        [SerializeField] private float bpmSon = 100f;

        private SpriteRenderer spriteRenderer;
        private Vector3 posicionBase;
        private Vector3 escalaBase = Vector3.one;
        private Coroutine corrutinaReaccion;
        private Texture2D retratoTextura;

        public string NombreRival => nombreRival;
        public string TituloRival => tituloRival;
        public string NombreSon => nombreSon;
        public float BpmSon => bpmSon;
        public Texture2D RetratoTextura => retratoTextura;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null) spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sortingOrder = 10;
            posicionBase = transform.position;
        }

        public void Configurar(
            string nombre,
            string titulo,
            string nombreSonMusical,
            float bpm,
            Sprite spriteRival,
            Texture2D texRetrato,
            bool invertirX = false)
        {
            this.nombreRival = nombre;
            this.tituloRival = titulo;
            this.nombreSon = nombreSonMusical;
            this.bpmSon = Mathf.Max(60f, bpm);
            this.retratoTextura = texRetrato;

            if (spriteRival != null && spriteRenderer != null)
            {
                spriteRenderer.sprite = spriteRival;
            }

            if (spriteRenderer != null)
            {
                spriteRenderer.flipX = invertirX;
                spriteRenderer.color = Color.white;
            }

            posicionBase = transform.position;
            transform.rotation = Quaternion.identity;
        }

        private void Update()
        {
            if (corrutinaReaccion == null)
            {
                float freq = (bpmSon / 60f) * Mathf.PI;
                float bounce = Mathf.Abs(Mathf.Sin(Time.time * freq + 0.8f)) * 0.04f;
                transform.localScale = new Vector3(escalaBase.x * (1f - bounce * 0.25f), escalaBase.y * (1f + bounce), 1f);
                transform.position = posicionBase;
            }
        }

        public void ReaccionarComboJugador()
        {
            if (corrutinaReaccion != null) StopCoroutine(corrutinaReaccion);
            corrutinaReaccion = StartCoroutine(RutinaSorpresaRival());
        }

        private IEnumerator RutinaSorpresaRival()
        {
            if (spriteRenderer != null) spriteRenderer.color = new Color(1f, 0.9f, 0.65f);
            transform.position = posicionBase + new Vector3(0.16f, 0.05f, 0f);
            yield return new WaitForSeconds(0.14f);
            if (spriteRenderer != null) spriteRenderer.color = Color.white;
            transform.position = posicionBase;
            corrutinaReaccion = null;
        }

        public void AnimarRivalSuperado()
        {
            if (corrutinaReaccion != null) StopCoroutine(corrutinaReaccion);
            corrutinaReaccion = StartCoroutine(RutinaReverenciaRespeto());
        }

        private IEnumerator RutinaReverenciaRespeto()
        {
            float t = 0f;
            while (t < 0.35f)
            {
                t += Time.deltaTime;
                transform.localScale = Vector3.Lerp(escalaBase, new Vector3(0.95f, 0.92f, 1f), t / 0.35f);
                yield return null;
            }
        }
    }
}
