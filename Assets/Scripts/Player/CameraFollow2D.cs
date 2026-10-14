using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CaminosDeGuanajuato.Player
{
    /// <summary>
    /// Camara 2D suave que sigue a Guanajicho en el nivel de pruebas.
    /// </summary>
    public class CameraFollow2D : MonoBehaviour
    {
        [Header("Objetivo")]
        [SerializeField] private Transform objetivo;

        [Header("Ajustes de Suavizado")]
        [SerializeField] private float suavizado = 5.0f;
        [SerializeField] private Vector3 desplazamiento = new Vector3(0f, 0f, -10f);

        [Header("Limites (Opcional)")]
        [SerializeField] private bool usarLimites = false;
        [SerializeField] private Vector2 limiteMin = new Vector2(-20f, -20f);
        [SerializeField] private Vector2 limiteMax = new Vector2(20f, 20f);

        public void SetObjetivo(Transform nuevoObjetivo)
        {
            objetivo = nuevoObjetivo;
        }

        private void Start()
        {
            if (objetivo == null)
            {
                GuanajichoController player = FindAnyObjectByType<GuanajichoController>();
                if (player != null)
                {
                    objetivo = player.transform;
                }
            }
        }

        private void LateUpdate()
        {
            if (objetivo == null) return;

            Vector3 posicionDeseada = objetivo.position + desplazamiento;

            if (usarLimites)
            {
                posicionDeseada.x = Mathf.Clamp(posicionDeseada.x, limiteMin.x, limiteMax.x);
                posicionDeseada.y = Mathf.Clamp(posicionDeseada.y, limiteMin.y, limiteMax.y);
            }

            transform.position = Vector3.Lerp(transform.position, posicionDeseada, suavizado * Time.deltaTime);
        }
    }
}
