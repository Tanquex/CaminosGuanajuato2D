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

        [Header("Ajustes de Suavizado y Desplazamiento")]
        [SerializeField] private float suavizado = 5.0f;
        [SerializeField] private Vector3 desplazamiento = new Vector3(0f, 0f, -10f);

        [Header("Limites (Opcional)")]
        [SerializeField] private bool usarLimites = false;
        [SerializeField] private Vector2 limiteMin = new Vector2(-20f, -20f);
        [SerializeField] private Vector2 limiteMax = new Vector2(20f, 20f);

        [Header("Zoom de Cámara (Rueda del Ratón)")]
        [Tooltip("Tamaño ortográfico inicial (más bajo = personajes más grandes y cercanos)")]
        [SerializeField] private float tamanoOrtograficoBase = 3.2f;
        [SerializeField] private float zoomMinimo = 2.2f;
        [SerializeField] private float zoomMaximo = 4.8f;
        [SerializeField] private float sensibilidadZoom = 0.5f;
        [SerializeField] private float suavizadoZoom = 8.0f;

        [Header("Anticipación de Movimiento")]
        [SerializeField] private float anticipacionDistancia = 0.8f;

        private Camera camaraComponente;
        private float tamanoOrtograficoDeseado;
        private GuanajichoController controladorPlayer;

        public void SetObjetivo(Transform nuevoObjetivo)
        {
            objetivo = nuevoObjetivo;
            if (objetivo != null)
            {
                controladorPlayer = objetivo.GetComponent<GuanajichoController>();
            }
        }

        private void Start()
        {
            camaraComponente = GetComponent<Camera>();
            tamanoOrtograficoDeseado = tamanoOrtograficoBase;

            if (camaraComponente != null && camaraComponente.orthographic)
            {
                camaraComponente.orthographicSize = tamanoOrtograficoBase;
            }

            if (objetivo == null)
            {
                GuanajichoController player = FindAnyObjectByType<GuanajichoController>();
                if (player != null)
                {
                    SetObjetivo(player.transform);
                }
            }
        }

        private void Update()
        {
            ProcesarZoom();
        }

        private void ProcesarZoom()
        {
            float deltaScroll = Input.mouseScrollDelta.y;

#if ENABLE_INPUT_SYSTEM
            if (Mathf.Approximately(deltaScroll, 0f) && UnityEngine.InputSystem.Mouse.current != null)
            {
                deltaScroll = UnityEngine.InputSystem.Mouse.current.scroll.ReadValue().y * 0.01f;
            }
#endif

            if (Mathf.Abs(deltaScroll) > 0.01f)
            {
                tamanoOrtograficoDeseado = Mathf.Clamp(tamanoOrtograficoDeseado - deltaScroll * sensibilidadZoom, zoomMinimo, zoomMaximo);
            }

            if (camaraComponente != null && camaraComponente.orthographic)
            {
                camaraComponente.orthographicSize = Mathf.Lerp(camaraComponente.orthographicSize, tamanoOrtograficoDeseado, suavizadoZoom * Time.deltaTime);
            }
        }

        private void LateUpdate()
        {
            if (objetivo == null) return;

            Vector3 anticipacion = Vector3.zero;
            if (controladorPlayer != null && controladorPlayer.EstaMoviendose)
            {
                anticipacion = (Vector3)(controladorPlayer.UltimaDireccion * anticipacionDistancia);
            }

            Vector3 posicionDeseada = objetivo.position + desplazamiento + anticipacion;

            if (usarLimites)
            {
                posicionDeseada.x = Mathf.Clamp(posicionDeseada.x, limiteMin.x, limiteMax.x);
                posicionDeseada.y = Mathf.Clamp(posicionDeseada.y, limiteMin.y, limiteMax.y);
            }

            transform.position = Vector3.Lerp(transform.position, posicionDeseada, suavizado * Time.deltaTime);
        }
    }
}
