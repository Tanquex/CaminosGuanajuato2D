using UnityEngine;
using CaminosDeGuanajuato.Core;

namespace CaminosDeGuanajuato.Defense
{
    /// <summary>
    /// Proyectil lanzado por las unidades defensivas (ej. proyectil de piedra de los Honderos Insurgentes).
    /// </summary>
    public class ProyectilDefensa : MonoBehaviour
    {
        [Header("Parametros del Proyectil")]
        [SerializeField] private float velocidad = 8.0f;
        [SerializeField] private int danio = 20;
        [SerializeField] private float tiempoVida = 3.5f;

        private Transform objetivo;
        private Vector3 direccion;

        public void Configurar(Transform nuevoObjetivo, int nuevoDanio)
        {
            objetivo = nuevoObjetivo;
            danio = nuevoDanio;

            if (objetivo != null)
            {
                direccion = (objetivo.position - transform.position).normalized;
            }
            else
            {
                direccion = Vector3.right;
            }

            Destroy(gameObject, tiempoVida);
        }

        private void Update()
        {
            // Si el objetivo aun existe, perseguirlo; si no, continuar en la direccion previa
            if (objetivo != null)
            {
                direccion = (objetivo.position - transform.position).normalized;
            }

            transform.position += direccion * velocidad * Time.deltaTime;

            // Rotar hacia la direccion de movimiento
            float angulo = Mathf.Atan2(direccion.y, direccion.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angulo);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            InvasorNocturno invasor = collision.GetComponent<InvasorNocturno>();
            if (invasor != null)
            {
                invasor.RecibirDanio(danio);
                Destroy(gameObject);
            }
        }
    }
}
