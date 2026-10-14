using System;
using System.Collections.Generic;
using UnityEngine;
using CaminosDeGuanajuato.Defense;

namespace CaminosDeGuanajuato.Core
{
    /// <summary>
    /// Invasor Nocturno que intenta apagar la Antorcha Libertaria en Dolores Hidalgo.
    /// Avanza siguiendo una ruta de puntos, combate barricadas que bloquean su paso
    /// y otorga recursos civicos al ser derrotado.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class InvasorNocturno : MonoBehaviour
    {
        [Header("Estadisticas")]
        [SerializeField] private int saludMaxima = 60;
        [SerializeField] private int saludActual;
        [SerializeField] private float velocidadMovimiento = 1.8f;
        [SerializeField] private int danioAtaque = 15;
        [SerializeField] private float cadenciaAtaque = 1.2f;
        [SerializeField] private int recompensaRecursos = 15;

        [Header("Efectos Visuales")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Color colorNormal = Color.white;
        [SerializeField] private Color colorDanio = Color.red;

        // Ruta de movimiento
        private List<Transform> waypoints;
        private int indiceWaypointActual;
        private Transform objetivoFinal;
        private DefensaElemento defensaBloqueante;
        private float temporizadorAtaque;
        private bool estaDerrotado;

        public event Action<InvasorNocturno> OnInvasorDerrotado;

        public int SaludActual => saludActual;
        public int SaludMaxima => saludMaxima;

        private void Awake()
        {
            if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
            saludActual = saludMaxima;
        }

        public void ConfigurarRuta(List<Transform> ruta, Transform antorchaObjetivo)
        {
            waypoints = ruta;
            objetivoFinal = antorchaObjetivo;
            indiceWaypointActual = 0;
        }

        private void Update()
        {
            if (estaDerrotado) return;

            // Si hay una barricada o defensa enfrente bloqueando, atacarla
            if (defensaBloqueante != null)
            {
                AtacarDefensa();
                return;
            }

            MoverHaciaObjetivo();
        }

        private void MoverHaciaObjetivo()
        {
            Transform destinoActual = null;

            if (waypoints != null && indiceWaypointActual < waypoints.Count)
            {
                destinoActual = waypoints[indiceWaypointActual];
            }
            else if (objetivoFinal != null)
            {
                destinoActual = objetivoFinal;
            }

            if (destinoActual == null) return;

            Vector3 direccion = (destinoActual.position - transform.position);
            float distancia = direccion.magnitude;

            if (distancia < 0.25f)
            {
                if (waypoints != null && indiceWaypointActual < waypoints.Count)
                {
                    indiceWaypointActual++;
                }
                else if (objetivoFinal != null && distancia < 0.5f)
                {
                    // Llego a la antorcha libertaria! Danar la antorcha
                    AntorchaLibertaria antorcha = objetivoFinal.GetComponent<AntorchaLibertaria>();
                    if (antorcha != null)
                    {
                        antorcha.RecibirDanio(danioAtaque);
                    }
                    Derrotar(false); // Desaparece al impactar
                    return;
                }
            }

            Vector3 paso = direccion.normalized * velocidadMovimiento * Time.deltaTime;
            transform.position += paso;

            // Voltear sprite hacia donde camina
            if (spriteRenderer != null)
            {
                if (direccion.x > 0.05f) spriteRenderer.flipX = false;
                else if (direccion.x < -0.05f) spriteRenderer.flipX = true;
            }
        }

        private void AtacarDefensa()
        {
            temporizadorAtaque += Time.deltaTime;
            if (temporizadorAtaque >= cadenciaAtaque)
            {
                temporizadorAtaque = 0f;
                if (defensaBloqueante != null)
                {
                    defensaBloqueante.RecibirDanio(danioAtaque);
                }
            }
        }

        public void RecibirDanio(int danio)
        {
            if (estaDerrotado) return;

            saludActual -= danio;

            if (spriteRenderer != null)
            {
                StopAllCoroutines();
                StartCoroutine(FlashDanio());
            }

            if (saludActual <= 0)
            {
                Derrotar(true);
            }
        }

        private void Derrotar(bool recompensa)
        {
            if (estaDerrotado) return;
            estaDerrotado = true;

            if (recompensa && RecursosCivicosManager.Instance != null)
            {
                RecursosCivicosManager.Instance.AgregarRecursos(recompensaRecursos);
            }

            OnInvasorDerrotado?.Invoke(this);
            Destroy(gameObject, 0.05f);
        }

        private System.Collections.IEnumerator FlashDanio()
        {
            spriteRenderer.color = colorDanio;
            yield return new WaitForSeconds(0.1f);
            if (spriteRenderer != null) spriteRenderer.color = colorNormal;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            DefensaElemento defensa = collision.gameObject.GetComponent<DefensaElemento>();
            if (defensa != null)
            {
                defensaBloqueante = defensa;
            }
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            DefensaElemento defensa = collision.gameObject.GetComponent<DefensaElemento>();
            if (defensa != null && defensa == defensaBloqueante)
            {
                defensaBloqueante = null;
            }
        }
    }
}
