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
    /// Representa a un comensal que se aproxima al quiosco de nieves en el Nivel 2.
    /// Soporta los personajes modulares por capas de Custom Character 2D Vol 1.
    /// </summary>
    public class ClienteTurista : MonoBehaviour
    {
        [Header("Datos del Cliente")]
        [SerializeField] private string nombreCliente = "Visitante";
        [SerializeField] private PedidoNieve pedido;
        [SerializeField] private EstadoCliente estado = EstadoCliente.Esperando;
        [SerializeField] private float velocidadCaminar = 2.5f;

        private Vector3 posicionMostrador;
        private Vector3 posicionSalida;
        private SpriteRenderer[] renderersCapas;
        private Vector3 escalaBase = new Vector3(0.135f, 0.135f, 1f);
        private float tiempoGracia = 2.5f;

        public PedidoNieve Pedido => pedido;
        public EstadoCliente Estado => estado;
        public string NombreCliente => nombreCliente;
        public float PorcentajePaciencia => pedido != null ? pedido.PorcentajePaciencia : 0f;

        public event Action<ClienteTurista> OnPacienciaAgotada;

        private void Awake()
        {
            ActualizarCacheRenderers();
        }

        public void ActualizarCacheRenderers()
        {
            renderersCapas = GetComponentsInChildren<SpriteRenderer>(true);
        }

        public void Configurar(PedidoNieve nuevoPedido, Vector3 destinoMostrador, Vector3 destinoSalida, float escalaPersonaje = 0.135f)
        {
            pedido = nuevoPedido;
            nombreCliente = nuevoPedido.nombreCliente;
            posicionMostrador = destinoMostrador;
            posicionSalida = destinoSalida;
            estado = EstadoCliente.Llegando;
            tiempoGracia = 2.5f;
            escalaBase = new Vector3(escalaPersonaje, escalaPersonaje, 1f);

            ActualizarCacheRenderers();
            // Al llegar desde la derecha hacia el mostrador en la izquierda, mira hacia la izquierda (-X)
            float dirX = (posicionMostrador.x < transform.position.x) ? -1f : 1f;
            transform.localScale = new Vector3(escalaBase.x * dirX, escalaBase.y, 1f);
        }

        private void Update()
        {
            switch (estado)
            {
                case EstadoCliente.Llegando:
                    transform.position = Vector3.MoveTowards(transform.position, posicionMostrador, velocidadCaminar * Time.deltaTime);

                    float dirLlegada = (posicionMostrador.x <= transform.position.x) ? -1f : 1f;
                    float balanceo = Mathf.Sin(Time.time * 12f) * (escalaBase.y * 0.045f);
                    transform.localScale = new Vector3(escalaBase.x * dirLlegada, escalaBase.y + balanceo, 1f);

                    if (Vector3.Distance(transform.position, posicionMostrador) < 0.05f)
                    {
                        estado = EstadoCliente.Esperando;
                        // Mirar hacia el puesto de nieves (izquierda)
                        transform.localScale = new Vector3(-escalaBase.x, escalaBase.y, 1f);
                    }
                    break;

                case EstadoCliente.Esperando:
                    if (pedido != null)
                    {
                        pedido.tiempoPacienciaRestante -= Time.deltaTime;

                        // Respiración suave mirando hacia el puesto (-X)
                        float respiracion = Mathf.Sin(Time.time * 4f) * (escalaBase.y * 0.025f);
                        transform.localScale = new Vector3(-escalaBase.x, escalaBase.y + respiracion, 1f);

                        if (pedido.EstaExpirado)
                        {
                            tiempoGracia -= Time.deltaTime;
                            Color tinteAlerta = Color.Lerp(Color.white, new Color(1f, 0.45f, 0.45f), Mathf.PingPong(Time.time * 5f, 1f));
                            AplicarColorACapas(tinteAlerta);

                            if (tiempoGracia <= 0f)
                            {
                                AplicarColorACapas(Color.white);
                                estado = EstadoCliente.MolestoSalida;
                                OnPacienciaAgotada?.Invoke(this);
                            }
                        }
                    }
                    break;

                case EstadoCliente.AtendidoFeliz:
                case EstadoCliente.MolestoSalida:
                    transform.position = Vector3.MoveTowards(transform.position, posicionSalida, velocidadCaminar * 1.6f * Time.deltaTime);

                    // Al retirarse hacia la derecha (+X), el personaje mira a la derecha (+escalaBase.x)
                    float dirSalida = (posicionSalida.x >= transform.position.x) ? 1f : -1f;
                    float pasoSalida = Mathf.Sin(Time.time * 14f) * (escalaBase.y * 0.045f);
                    transform.localScale = new Vector3(escalaBase.x * dirSalida, escalaBase.y + pasoSalida, 1f);

                    if (Vector3.Distance(transform.position, posicionSalida) < 0.2f)
                    {
                        Destroy(gameObject);
                    }
                    break;
            }
        }

        private void AplicarColorACapas(Color color)
        {
            if (renderersCapas == null || renderersCapas.Length == 0)
            {
                ActualizarCacheRenderers();
            }
            for (int i = 0; i < renderersCapas.Length; i++)
            {
                if (renderersCapas[i] != null)
                {
                    renderersCapas[i].color = color;
                }
            }
        }

        public void DespacharFeliz()
        {
            AplicarColorACapas(Color.white);
            estado = EstadoCliente.AtendidoFeliz;
        }

        public void DespacharMolesto()
        {
            AplicarColorACapas(Color.white);
            estado = EstadoCliente.MolestoSalida;
        }
    }
}
