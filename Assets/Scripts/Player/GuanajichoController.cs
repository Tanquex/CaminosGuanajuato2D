using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CaminosDeGuanajuato.Player
{
    /// <summary>
    /// Controlador de movimiento 2D para Guanajicho (Protagonista de Caminos de Guanajuato 2D).
    /// Disenado para maxima compatibilidad y robustez:
    /// - Soporta Flechas y WASD tanto por Legacy Input como por New Input System.
    /// - Movimiento en 4 y 8 direcciones con velocidad normalizada en diagonales.
    /// - Fisica basada en Rigidbody2D con respaldo cinetico garantizado.
    /// - Soporte para sprint (Shift) e interaccion con defensas (E / Espacio).
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class GuanajichoController : MonoBehaviour
    {
        [Header("Configuracion de Movimiento")]
        [Tooltip("Velocidad de caminata base")]
        [SerializeField] private float velocidadCaminata = 5.0f;

        [Tooltip("Multiplicador de velocidad al correr con Shift")]
        [SerializeField] private float multiplicadorCorrer = 1.4f;

        [Header("Referencias")]
        [SerializeField] private Animator animator;
        [SerializeField] private SpriteRenderer spriteRenderer;

        [Header("Interaccion")]
        [SerializeField] private float radioInteraccion = 1.5f;
        [SerializeField] private LayerMask capaInteractuable = ~0;

        // Estado interno
        private Rigidbody2D rb;
        private Vector2 direccionEntrada;
        private Vector2 ultimaDireccion = Vector2.down;
        private bool estaMoviendose;
        private bool estaCorriendo;

        public Vector2 UltimaDireccion => ultimaDireccion;
        public bool EstaMoviendose => estaMoviendose;
        public float VelocidadActual => estaCorriendo ? (velocidadCaminata * multiplicadorCorrer) : velocidadCaminata;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            if (animator == null) animator = GetComponent<Animator>();
            if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();

            // Asegurar que el SpriteRenderer sea siempre visible al frente
            if (spriteRenderer != null)
            {
                spriteRenderer.sortingOrder = 10;
                spriteRenderer.color = Color.white;
            }

            // Configuracion robusta del Rigidbody2D para 2D Top-Down
            if (rb != null)
            {
                rb.gravityScale = 0f;
                rb.freezeRotation = true;
                rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
                rb.interpolation = RigidbodyInterpolation2D.Interpolate;
            }

            Debug.Log("<color=green>[Guanajicho]</color> ¡Guanajicho inicializado y listo en escena!");
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
        /// Lee la entrada soportando WASD, flechas, Input Manager y el nuevo Input System.
        /// </summary>
        private void ProcesarEntrada()
        {
            float inputX = 0f;
            float inputY = 0f;

            // 1. Lectura directa por teclas (funciona siempre)
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) inputX -= 1f;
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) inputX += 1f;
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) inputY += 1f;
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) inputY -= 1f;

            // 2. Si no hubo tecla directa, intentar por ejes de Unity Input Manager
            if (Mathf.Approximately(inputX, 0f) && Mathf.Approximately(inputY, 0f))
            {
                try
                {
                    inputX = Input.GetAxisRaw("Horizontal");
                    inputY = Input.GetAxisRaw("Vertical");
                }
                catch { }
            }

            // 3. Respaldo para el New Input System si esta activo
#if ENABLE_INPUT_SYSTEM
            if (UnityEngine.InputSystem.Keyboard.current != null)
            {
                var kb = UnityEngine.InputSystem.Keyboard.current;
                if (kb.aKey.isPressed || kb.leftArrowKey.isPressed) inputX = -1f;
                if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) inputX = 1f;
                if (kb.wKey.isPressed || kb.upArrowKey.isPressed) inputY = 1f;
                if (kb.sKey.isPressed || kb.downArrowKey.isPressed) inputY = -1f;
                estaCorriendo = kb.leftShiftKey.isPressed || kb.rightShiftKey.isPressed;
            }
            else
#endif
            {
                estaCorriendo = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            }

            direccionEntrada = new Vector2(inputX, inputY);

            // Normalizacion uniforme para no correr mas rapido en diagonal
            if (direccionEntrada.sqrMagnitude > 1f)
            {
                direccionEntrada.Normalize();
            }

            estaMoviendose = direccionEntrada.sqrMagnitude > 0.001f;

            if (estaMoviendose)
            {
                ultimaDireccion = direccionEntrada;
            }
        }

        private void MoverPersonaje()
        {
            Vector2 velocidadDeseada = direccionEntrada * VelocidadActual;

            if (rb != null)
            {
#if UNITY_6000_0_OR_NEWER
                rb.linearVelocity = velocidadDeseada;
#else
                rb.velocity = velocidadDeseada;
#endif
                // Respaldo de movimiento por posicion si la velocidad física esta bloqueada
                if (estaMoviendose && rb.bodyType == RigidbodyType2D.Kinematic)
                {
                    rb.MovePosition(rb.position + velocidadDeseada * Time.fixedDeltaTime);
                }
            }
            else
            {
                transform.position += (Vector3)velocidadDeseada * Time.fixedDeltaTime;
            }
        }

        private void ActualizarAnimaciones()
        {
            int direccionIndex = 0; // 0 = Abajo

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
                if (direccionEntrada.y > 0) direccionIndex = 1; // Arriba
                else direccionIndex = 0; // Abajo
            }

            if (animator != null && animator.runtimeAnimatorController != null)
            {
                animator.SetInteger("Direction", direccionIndex);
                animator.SetBool("IsMoving", estaMoviendose);
                animator.SetFloat("MoveX", direccionEntrada.x);
                animator.SetFloat("MoveY", direccionEntrada.y);
                animator.SetFloat("Speed", direccionEntrada.magnitude * VelocidadActual);
            }
        }

        private void ProcesarInteraccion()
        {
            bool presionoInteractuar = Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Space);
#if ENABLE_INPUT_SYSTEM
            if (!presionoInteractuar && UnityEngine.InputSystem.Keyboard.current != null)
            {
                var kb = UnityEngine.InputSystem.Keyboard.current;
                presionoInteractuar = kb.eKey.wasPressedThisFrame || kb.spaceKey.wasPressedThisFrame;
            }
#endif
            if (presionoInteractuar)
            {
                Interactuar();
            }
        }

        public void Interactuar()
        {
            Vector3 puntoInteraccion = transform.position + (Vector3)(ultimaDireccion.normalized * 1.0f);
            Collider2D objetoCercano = Physics2D.OverlapCircle(puntoInteraccion, radioInteraccion, capaInteractuable);

            if (objetoCercano != null && objetoCercano.gameObject != gameObject)
            {
                objetoCercano.SendMessage("OnInteractuarConGuanajicho", this, SendMessageOptions.DontRequireReceiver);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Vector3 puntoFrente = transform.position + (Vector3)(ultimaDireccion.normalized * 1.0f);
            Gizmos.DrawWireSphere(puntoFrente, radioInteraccion);
            Gizmos.DrawLine(transform.position, puntoFrente);
        }
    }
}
