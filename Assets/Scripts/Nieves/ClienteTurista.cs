using System;
using UnityEngine;

namespace CaminosDeGuanajuato.Nieves
{
    public enum EstadoCliente
    {
        Llegando,
        Esperando,
        AtendidoFeliz,
        MolestoSalida
    }

    /// <summary>
    /// Representa a un comensal turista que se aproxima al quiosco de Don Chon.
    /// Utiliza sprites auténticos de Pixel Adventure 1, camina con balanceo y muestra
    /// su globo de pedido en tiempo real.
    /// </summary>
    public class ClienteTurista : MonoBehaviour
    {
        [Header("Datos del Cliente")]
        [SerializeField] private string nombreCliente = "Turista Visitante";
        [SerializeField] private PedidoNieve pedido;
        [SerializeField] private EstadoCliente estado = EstadoCliente.Esperando;
        [SerializeField] private float velocidadCaminar = 2.4f;

        private Vector3 posicionMostrador;
        private Vector3 posicionSalida;
        private SpriteRenderer spriteRenderer;
        private Vector3 escalaBase = new Vector3(1.6f, 1.6f, 1f);
        private float tiempoGracia = 2.5f;

        public PedidoNieve Pedido => pedido;
        public EstadoCliente Estado => estado;
        public string NombreCliente => nombreCliente;
        public float PorcentajePaciencia => pedido != null ? pedido.PorcentajePaciencia : 0f;

        public event Action<ClienteTurista> OnPacienciaAgotada;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null) spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sortingOrder = 10;
        }

        public void Configurar(PedidoNieve nuevoPedido, Vector3 destinoMostrador, Vector3 destinoSalida, Sprite spritePersonaje = null)
        {
            pedido = nuevoPedido;
            nombreCliente = nuevoPedido.nombreCliente;
            posicionMostrador = destinoMostrador;
            posicionSalida = destinoSalida;
            estado = EstadoCliente.Llegando;
            tiempoGracia = 2.5f;

            if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
            if (spritePersonaje != null)
            {
                spriteRenderer.sprite = spritePersonaje;
            }
            transform.localScale = escalaBase;
        }

        private void Update()
        {
            switch (estado)
            {
                case EstadoCliente.Llegando:
                    transform.position = Vector3.MoveTowards(transform.position, posicionMostrador, velocidadCaminar * Time.deltaTime);
                    if (spriteRenderer != null)
                    {
                        spriteRenderer.flipX = (posicionMostrador.x < transform.position.x);
                    }

                    // Animación suave de caminata (balanceo)
                    float balanceo = Mathf.Sin(Time.time * 12f) * 0.08f;
                    transform.localScale = new Vector3(escalaBase.x, escalaBase.y + balanceo, 1f);

                    if (Vector3.Distance(transform.position, posicionMostrador) < 0.05f)
                    {
                        estado = EstadoCliente.Esperando;
                        transform.localScale = escalaBase;
                    }
                    break;

                case EstadoCliente.Esperando:
                    if (pedido != null)
                    {
                        pedido.tiempoPacienciaRestante -= Time.deltaTime;

                        // Respiración / Idle
                        float respiracion = Mathf.Sin(Time.time * 4f) * 0.04f;
                        transform.localScale = new Vector3(escalaBase.x, escalaBase.y + respiracion, 1f);

                        if (pedido.EstaExpirado)
                        {
                            tiempoGracia -= Time.deltaTime;
                            // Tinte rojizo durante el periodo de gracia
                            if (spriteRenderer != null)
                            {
                                spriteRenderer.color = Color.Lerp(Color.white, new Color(1f, 0.4f, 0.4f), Mathf.PingPong(Time.time * 5f, 1f));
                            }

                            if (tiempoGracia <= 0f)
                            {
                                if (spriteRenderer != null) spriteRenderer.color = Color.white;
                                estado = EstadoCliente.MolestoSalida;
                                OnPacienciaAgotada?.Invoke(this);
                            }
                        }
                    }
                    break;

                case EstadoCliente.AtendidoFeliz:
                case EstadoCliente.MolestoSalida:
                    transform.position = Vector3.MoveTowards(transform.position, posicionSalida, velocidadCaminar * 1.6f * Time.deltaTime);
                    if (spriteRenderer != null)
                    {
                        spriteRenderer.flipX = (posicionSalida.x < transform.position.x);
                    }

                    // Balanceo al retirarse
                    float pasoSalida = Mathf.Sin(Time.time * 14f) * 0.08f;
                    transform.localScale = new Vector3(escalaBase.x, escalaBase.y + pasoSalida, 1f);

                    if (Vector3.Distance(transform.position, posicionSalida) < 0.2f)
                    {
                        Destroy(gameObject);
                    }
                    break;
            }
        }

        public void DespacharFeliz()
        {
            estado = EstadoCliente.AtendidoFeliz;
        }

        public void DespacharMolesto()
        {
            estado = EstadoCliente.MolestoSalida;
        }
    }
}
