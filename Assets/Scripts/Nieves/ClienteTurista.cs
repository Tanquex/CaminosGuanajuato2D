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
    /// Representa a un habitante o turista de la época virreinal / colonial
    /// que visita el quiosco de Don Chon en Dolores Hidalgo.
    /// Utiliza sprites auténticos en pixel art colonial 32-bit generados con IA.
    /// </summary>
    public class ClienteTurista : MonoBehaviour
    {
        [Header("Datos del Cliente")]
        [SerializeField] private string nombreCliente = "Visitante";
        [SerializeField] private PedidoNieve pedido;
        [SerializeField] private EstadoCliente estado = EstadoCliente.Esperando;
        [SerializeField] private float velocidadCaminar = 2.4f;

        private Vector3 posicionMostrador;
        private Vector3 posicionSalida;
        private SpriteRenderer spriteRenderer;
        private float basePosY = -2.25f;
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

        public void Configurar(PedidoNieve nuevoPedido, Vector3 destinoMostrador, Vector3 destinoSalida, Sprite spritePersonaje)
        {
            pedido = nuevoPedido;
            nombreCliente = nuevoPedido.nombreCliente;
            posicionMostrador = destinoMostrador;
            posicionSalida = destinoSalida;
            basePosY = destinoMostrador.y;
            estado = EstadoCliente.Llegando;
            tiempoGracia = 2.5f;

            if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
            if (spritePersonaje != null)
            {
                spriteRenderer.sprite = spritePersonaje;
            }

            // Los sprites nativamente miran a la izquierda hacia el mostrador (flipX = false)
            spriteRenderer.flipX = false;
            transform.localScale = Vector3.one;
            transform.position = new Vector3(transform.position.x, basePosY, 0f);
        }

        private void Update()
        {
            if (spriteRenderer == null) return;

            switch (estado)
            {
                case EstadoCliente.Llegando:
                    float nuevoX = Mathf.MoveTowards(transform.position.x, posicionMostrador.x, velocidadCaminar * Time.deltaTime);
                    // Suave paso de caminata sobre el empedrado
                    float pasoCaminata = Mathf.Abs(Mathf.Sin(Time.time * 11f)) * 0.05f;
                    transform.position = new Vector3(nuevoX, basePosY + pasoCaminata, 0f);

                    // Mirar en dirección de avance hacia el mostrador
                    spriteRenderer.flipX = (posicionMostrador.x > transform.position.x);

                    if (Mathf.Abs(transform.position.x - posicionMostrador.x) < 0.05f)
                    {
                        estado = EstadoCliente.Esperando;
                        transform.position = new Vector3(posicionMostrador.x, basePosY, 0f);
                        spriteRenderer.flipX = false; // Mirar hacia el quiosco
                    }
                    break;

                case EstadoCliente.Esperando:
                    if (pedido != null)
                    {
                        pedido.tiempoPacienciaRestante -= Time.deltaTime;

                        float pctPaciencia = pedido.PorcentajePaciencia;

                        if (pctPaciencia < 0.35f && !pedido.EstaExpirado)
                        {
                            // Tensión: El cliente zapatea o tiembla de impaciencia
                            float vibracionNerviosa = Mathf.Sin(Time.time * 26f) * 0.03f;
                            transform.position = new Vector3(posicionMostrador.x + vibracionNerviosa, basePosY, 0f);
                            spriteRenderer.color = Color.Lerp(Color.white, new Color(1f, 0.75f, 0.70f), Mathf.PingPong(Time.time * 4f, 1f));
                        }
                        else if (!pedido.EstaExpirado)
                        {
                            // Respiración tranquila frente al mostrador
                            float respiracion = Mathf.Sin(Time.time * 3.5f) * 0.015f;
                            transform.localScale = new Vector3(1f - respiracion * 0.5f, 1f + respiracion, 1f);
                            transform.position = new Vector3(posicionMostrador.x, basePosY, 0f);
                            spriteRenderer.color = Color.white;
                        }

                        if (pedido.EstaExpirado)
                        {
                            tiempoGracia -= Time.deltaTime;
                            // Alerta visual parpadeante si se acaba el tiempo
                            spriteRenderer.color = Color.Lerp(Color.white, new Color(1f, 0.35f, 0.35f), Mathf.PingPong(Time.time * 8f, 1f));

                            if (tiempoGracia <= 0f)
                            {
                                spriteRenderer.color = Color.white;
                                estado = EstadoCliente.MolestoSalida;
                                OnPacienciaAgotada?.Invoke(this);
                            }
                        }
                    }
                    break;

                case EstadoCliente.AtendidoFeliz:
                case EstadoCliente.MolestoSalida:
                    float retX = Mathf.MoveTowards(transform.position.x, posicionSalida.x, velocidadCaminar * 1.5f * Time.deltaTime);
                    float pasoRetiro = Mathf.Abs(Mathf.Sin(Time.time * 13f)) * 0.05f;
                    transform.position = new Vector3(retX, basePosY + pasoRetiro, 0f);

                    // Al retirarse hacia la derecha, gira para mirar hacia la salida (flipX = true)
                    spriteRenderer.flipX = (posicionSalida.x >= transform.position.x);

                    if (Mathf.Abs(transform.position.x - posicionSalida.x) < 0.2f)
                    {
                        Destroy(gameObject);
                    }
                    break;
            }
        }

        public void DespacharFeliz()
        {
            if (spriteRenderer != null) spriteRenderer.color = Color.white;
            estado = EstadoCliente.AtendidoFeliz;
            // Salto de alegría al recibir la nieve deliciosa
            transform.position = new Vector3(transform.position.x, basePosY + 0.32f, 0f);
        }

        public void DespacharMolesto()
        {
            if (spriteRenderer != null) spriteRenderer.color = Color.white;
            estado = EstadoCliente.MolestoSalida;
        }
    }
}
