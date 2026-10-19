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
    [RequireComponent(typeof(BoxCollider2D))]
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
        private List<Vector3> waypointsPosiciones;
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
            waypointsPosiciones = null;
            objetivoFinal = antorchaObjetivo;
            indiceWaypointActual = 0;
        }

        public void ConfigurarRutaPosiciones(List<Vector3> rutaPosiciones, Transform antorchaObjetivo)
        {
            waypointsPosiciones = rutaPosiciones;
            waypoints = null;
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
            Vector3 destinoActual = transform.position;
            bool tieneDestino = false;

            if (waypointsPosiciones != null && indiceWaypointActual < waypointsPosiciones.Count)
            {
                destinoActual = waypointsPosiciones[indiceWaypointActual];
                tieneDestino = true;
            }
            else if (waypoints != null && indiceWaypointActual < waypoints.Count)
            {
                if (waypoints[indiceWaypointActual] != null)
                {
                    destinoActual = waypoints[indiceWaypointActual].position;
                    tieneDestino = true;
                }
                else
                {
                    indiceWaypointActual++;
                    return;
                }
            }
            else if (objetivoFinal != null)
            {
                destinoActual = objetivoFinal.position;
                tieneDestino = true;
            }

            if (!tieneDestino) return;

            Vector3 direccion = (destinoActual - transform.position);
            float distancia = direccion.magnitude;

            if (distancia < 0.35f)
            {
                if (waypointsPosiciones != null && indiceWaypointActual < waypointsPosiciones.Count)
                {
                    indiceWaypointActual++;
                }
                else if (waypoints != null && indiceWaypointActual < waypoints.Count)
                {
                    indiceWaypointActual++;
                }
                else if (objetivoFinal != null && distancia < 0.6f)
                {
                    // Llego a la antorcha libertaria! Danar la antorcha
                    AntorchaLibertaria antorcha = objetivoFinal.GetComponent<AntorchaLibertaria>();
                    if (antorcha != null)
                    {
                        antorcha.RecibirDanio(danioAtaque);
                    }
                    Derrotar(false);
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

            if (recompensa)
            {
                if (RecursosCivicosManager.Instance != null)
                {
                    RecursosCivicosManager.Instance.AgregarRecursos(recompensaRecursos);
                }

                // 65% de probabilidad de soltar un botín cívico recolectable en el atrio
                if (UnityEngine.Random.value < 0.65f)
                {
                    try
                    {
                        GameObject dropGO = new GameObject("Drop_RecursoCivico");
                        dropGO.transform.position = transform.position;

                        CircleCollider2D circleCol = dropGO.AddComponent<CircleCollider2D>();
                        if (circleCol != null)
                        {
                            circleCol.isTrigger = true;
                            circleCol.radius = 0.45f;
                        }

                        RecursoCivicoPickup pickup = dropGO.AddComponent<RecursoCivicoPickup>();
                        if (pickup != null)
                        {
                            pickup.Configurar(recompensaRecursos);
                        }
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogWarning($"[InvasorNocturno] Error no critico al soltar botin: {ex.Message}");
                    }
                }
            }

            try
            {
                OnInvasorDerrotado?.Invoke(this);
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"[InvasorNocturno] Error al notificar derrota: {ex.Message}");
            }

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
