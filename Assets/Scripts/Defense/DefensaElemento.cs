using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CaminosDeGuanajuato.Core;

namespace CaminosDeGuanajuato.Defense
{
    public enum TipoDefensa
    {
        BarricadaColonial, // Bloquea el paso y resiste ataques
        HonderoInsurgente, // Ataca a distancia a invasores
        CampaneroAlerta    // Ataca en area o da apoyo
    }

    /// <summary>
    /// Componente que rige el comportamiento de cualquier elemento defensivo colocado en el atrio.
    /// Soporta tanto estructuras defensivas fijas (barricadas) como unidades ofensivas (honderos/campaneros).
    /// </summary>
    public class DefensaElemento : MonoBehaviour
    {
        [Header("Datos de la Defensa")]
        [SerializeField] private TipoDefensa tipo = TipoDefensa.BarricadaColonial;
        [SerializeField] private string nombreDefensa = "Barricada Colonial";
        [SerializeField] private int costoRecursos = 25;
        [SerializeField] private int saludMaxima = 150;
        [SerializeField] private int saludActual;

        [Header("Combate Ofensivo (Para Honderos / Campaneros)")]
        [SerializeField] private bool esOfensiva = false;
        [SerializeField] private float rangoAtaque = 4.5f;
        [SerializeField] private float cadenciaAtaque = 1.0f;
        [SerializeField] private int danioAtaque = 25;
        [SerializeField] private GameObject prefabProyectil;
        [SerializeField] private Transform puntoDisparo;

        [Header("Efectos Visuales")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Color colorNormal = Color.white;
        [SerializeField] private Color colorDanio = Color.red;

        private float temporizadorAtaque;
        private InvasorNocturno objetivoActual;

        public TipoDefensa Tipo => tipo;
        public string NombreDefensa => nombreDefensa;
        public int CostoRecursos => costoRecursos;
        public int SaludActual => saludActual;
        public int SaludMaxima => saludMaxima;
        public float PorcentajeSalud => (float)saludActual / saludMaxima;

        public event Action<DefensaElemento> OnDefensaDestruida;

        private void Awake()
        {
            if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
            saludActual = saludMaxima;
            if (puntoDisparo == null) puntoDisparo = transform;
        }

        private void Update()
        {
            if (!esOfensiva) return;

            temporizadorAtaque += Time.deltaTime;

            // Si el objetivo murio o salio del rango, buscar uno nuevo
            if (objetivoActual == null || Vector3.Distance(transform.position, objetivoActual.transform.position) > rangoAtaque)
            {
                BuscarObjetivoMasCercano();
            }

            if (objetivoActual != null && temporizadorAtaque >= cadenciaAtaque)
            {
                AtacarObjetivo();
                temporizadorAtaque = 0f;
            }
        }

        private void BuscarObjetivoMasCercano()
        {
            InvasorNocturno[] todosInvasores = FindObjectsByType<InvasorNocturno>(FindObjectsSortMode.None);
            float menorDistancia = float.MaxValue;
            InvasorNocturno masCercano = null;

            foreach (var invasor in todosInvasores)
            {
                if (invasor == null || invasor.SaludActual <= 0) continue;

                float dist = Vector3.Distance(transform.position, invasor.transform.position);
                if (dist <= rangoAtaque && dist < menorDistancia)
                {
                    menorDistancia = dist;
                    masCercano = invasor;
                }
            }

            objetivoActual = masCercano;
        }

        private void AtacarObjetivo()
        {
            if (objetivoActual == null) return;

            if (prefabProyectil != null)
            {
                GameObject proyectilGO = Instantiate(prefabProyectil, puntoDisparo.position, Quaternion.identity);
                ProyectilDefensa proyectil = proyectilGO.GetComponent<ProyectilDefensa>();
                if (proyectil != null)
                {
                    proyectil.Configurar(objetivoActual.transform, danioAtaque);
                }
            }
            else
            {
                // Ataque directo si no hay prefab de proyectil
                objetivoActual.RecibirDanio(danioAtaque);
            }
        }

        public void RecibirDanio(int danio)
        {
            saludActual -= danio;

            if (spriteRenderer != null)
            {
                StopAllCoroutines();
                StartCoroutine(FlashDanio());
            }

            if (saludActual <= 0)
            {
                DestruirDefensa();
            }
        }

        private void DestruirDefensa()
        {
            OnDefensaDestruida?.Invoke(this);
            Destroy(gameObject);
        }

        private IEnumerator FlashDanio()
        {
            spriteRenderer.color = colorDanio;
            yield return new WaitForSeconds(0.1f);
            if (spriteRenderer != null) spriteRenderer.color = colorNormal;
        }

        private void OnDrawGizmosSelected()
        {
            if (esOfensiva)
            {
                Gizmos.color = new Color(0.2f, 0.8f, 1f, 0.35f);
                Gizmos.DrawWireSphere(transform.position, rangoAtaque);
            }
        }
    }
}
