using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CaminosDeGuanajuato.Player
{
    /// <summary>
    /// Controlador de movimiento 2D para Guanajicho (Protagonista de Caminos de Guanajuato 2D).
    /// Cumple con los requisitos del Avance 1 (Issue #2):
    /// - Movimiento en 4 y 8 direcciones (Arriba, Abajo, Izquierda, Derecha).
    /// - Física basada en Rigidbody2D.
    /// - Normalización de vector para velocidad uniforme en diagonales.
    /// - Soporte para controles con Flechas y WASD.
    /// - Integración con Animator y volteo de sprites.
    /// - Interacción en mundo para activar/colocar elementos defensivos.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class GuanajichoController : MonoBehaviour
    {
        [Header("Configuracion de Movimiento")]
        [Tooltip("Velocidad de caminata base")]
        [SerializeField] private float velocidadCaminata = 4.0f;

        [Tooltip("Multiplicador de velocidad al correr con Shift")]
        [SerializeField] private float multiplicadorCorrer = 1.4f;

        [Header("Referencias Opcionales")]
        [SerializeField] private Animator animator;
        [SerializeField] private SpriteRenderer spriteRenderer;

        [Header("Interaccion")]
        [SerializeField] private float radioInteraccion = 1.5f;
        [SerializeField] private LayerMask capaInteractuable;

        // Estado interno
        private Rigidbody2D rb;
        private Vector2 direccionEntrada;
        private Vector2 ultimaDireccion = Vector2.down; // Comienza mirando hacia abajo
        private bool estaMoviendose;
        private bool estaCorriendo;

        // Propiedades publicas
        public Vector2 UltimaDireccion => ultimaDireccion;
        public bool EstaMoviendose => estaMoviendose;
        public float VelocidadActual => estaCorriendo ? (velocidadCaminata * multiplicadorCorrer) : velocidadCaminata;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            if (animator == null) animator = GetComponent<Animator>();
            if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();

            // Configuracion optima del Rigidbody2D para vista Top-Down 2D
            rb.gravityScale = 0f;
            rb.freezeRotation = true;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        }

        private void Update()
        {
            ProcesarEntrada();
            ActualizarAnimaciones();
            ProcesarInteraccion();
        }

        private void FixedUpdate()
        {
            MoverPersonaje();
        }

        /// <summary>
        /// Lee la entrada tanto de WASD como de las Flechas del teclado de forma agil y fluida.
        /// </summary>
        private void ProcesarEntrada()
        {
            float inputX = Input.GetAxisRaw("Horizontal");
            float inputY = Input.GetAxisRaw("Vertical");

            direccionEntrada = new Vector2(inputX, inputY);

            // Normalizacion critica: evita que en diagonales se mueva 1.41x mas rapido
            if (direccionEntrada.sqrMagnitude > 1f)
            {
                direccionEntrada.Normalize();
            }

            estaMoviendose = direccionEntrada.sqrMagnitude > 0.001f;
            estaCorriendo = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

            if (estaMoviendose)
            {
                ultimaDireccion = direccionEntrada;
            }
        }

        /// <summary>
        /// Aplica la velocidad fisica al Rigidbody2D.
        /// Compatible tanto con Unity 6 (linearVelocity) como versiones anteriores (velocity).
        /// </summary>
        private void MoverPersonaje()
        {
            Vector2 velocidadDeseada = direccionEntrada * VelocidadActual;

#if UNITY_6000_0_OR_NEWER
            rb.linearVelocity = velocidadDeseada;
#else
            rb.velocity = velocidadDeseada;
#endif
        }

        /// <summary>
        /// Actualiza parametros de animacion y volteo de sprite segun la direccion cardinal.
        /// Compatible con el controlador de Cainos (Direction: 0=Down, 1=Up, 2=Right, 3=Left)
        /// y controladores basados en MoveX/MoveY.
        /// </summary>
        private void ActualizarAnimaciones()
        {
            int direccionIndex = 0; // 0 = Abajo por defecto

            if (Mathf.Abs(direccionEntrada.x) > Mathf.Abs(direccionEntrada.y))
            {
                if (direccionEntrada.x > 0)
                {
                    direccionIndex = 2; // Derecha
                    if (spriteRenderer != null) spriteRenderer.flipX = false;
                }
                else if (direccionEntrada.x < 0)
                {
                    direccionIndex = 3; // Izquierda
                    if (spriteRenderer != null) spriteRenderer.flipX = true;
                }
            }
            else if (Mathf.Abs(direccionEntrada.y) > 0.001f)
            {
                if (direccionEntrada.y > 0)
                {
                    direccionIndex = 1; // Arriba
                }
                else
                {
                    direccionIndex = 0; // Abajo
                }
            }

            if (animator != null)
            {
                // Compatibilidad con Cainos Animator
                animator.SetInteger("Direction", direccionIndex);
                animator.SetBool("IsMoving", estaMoviendose);

                // Compatibilidad con controladores estandar basados en floats
                animator.SetFloat("MoveX", direccionEntrada.x);
                animator.SetFloat("MoveY", direccionEntrada.y);
                animator.SetFloat("Speed", direccionEntrada.magnitude * VelocidadActual);
            }
        }

        /// <summary>
        /// Detecta si el jugador presiona Tecla E o Espacio para interactuar o colocar defensas.
        /// </summary>
        private void ProcesarInteraccion()
        {
            if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Space))
            {
                Interactuar();
            }
        }

        public void Interactuar()
        {
            // Intentar interactuar con el colocador de defensas en frente de Guanajicho
            Vector3 puntoInteraccion = transform.position + (Vector3)(ultimaDireccion.normalized * 1.0f);
            Collider2D objetoCercano = Physics2D.OverlapCircle(puntoInteraccion, radioInteraccion, capaInteractuable);

            if (objetoCercano != null)
            {
                objetoCercano.SendMessage("OnInteractuarConGuanajicho", this, SendMessageOptions.DontRequireReceiver);
            }
        }

        private void OnDrawGizmosSelected()
        {
            // Dibujar punto y radio de interaccion para facilitar depuracion en la escena
            Gizmos.color = Color.yellow;
            Vector3 puntoFrente = transform.position + (Vector3)(ultimaDireccion.normalized * 1.0f);
            Gizmos.DrawWireSphere(puntoFrente, radioInteraccion);
            Gizmos.DrawLine(transform.position, puntoFrente);
        }
    }
}
