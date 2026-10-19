using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CaminosDeGuanajuato.Core;
using CaminosDeGuanajuato.Defense;

namespace CaminosDeGuanajuato.Player
{
    /// <summary>
    /// Controlador de movimiento y combate 2D para Guanajicho (Protagonista de Caminos de Guanajuato 2D).
    /// Disenado para maxima compatibilidad y jugabilidad:
    /// - Soporta Flechas y WASD tanto por Legacy Input como por New Input System.
    /// - Movimiento en 4 y 8 direcciones con velocidad normalizada en diagonales.
    /// - Combate activo con Honda Insurgente (Click Izquierdo / Espacio / J).
    /// - Sprint con Shift y habilidad de Avivar la Antorcha Libertaria (E / F).
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class GuanajichoController : MonoBehaviour
    {
        [Header("Configuracion de Movimiento")]
        [Tooltip("Velocidad de caminata base")]
        [SerializeField] private float velocidadCaminata = 5.2f;

        [Tooltip("Multiplicador de velocidad al correr con Shift")]
        [SerializeField] private float multiplicadorCorrer = 1.5f;

        [Header("Combate con Honda Insurgente")]
        [SerializeField] private GameObject prefabProyectilPiedra;
        [SerializeField] private int danioPiedra = 25;
        [SerializeField] private float cadenciaDisparo = 0.32f;
        [SerializeField] private float velocidadProyectil = 11.0f;

        [Header("Avivar Antorcha")]
        [SerializeField] private int costoAvivar = 10;
        [SerializeField] private int curacionAntorcha = 25;

        [Header("Referencias")]
        [SerializeField] private Animator animator;
        [SerializeField] private SpriteRenderer spriteRenderer;

        [Header("Interaccion")]
        [SerializeField] private float radioInteraccion = 1.6f;
        [SerializeField] private LayerMask capaInteractuable = ~0;

        // Estado interno
        private Rigidbody2D rb;
        private Vector2 direccionEntrada;
        private Vector2 ultimaDireccion = Vector2.down;
        private bool estaMoviendose;
        private bool estaCorriendo;
        private float temporizadorDisparo;

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

            // Asegurar que Guanajicho tenga collider para recolectar botines e interactuar
            Collider2D col = GetComponent<Collider2D>();
            if (col == null)
            {
                CapsuleCollider2D cap = gameObject.AddComponent<CapsuleCollider2D>();
                cap.size = new Vector2(0.8f, 1.2f);
            }

            Debug.Log("<color=green>[Guanajicho]</color> ¡Guanajicho inicializado y listo en escena!");
        }

        private void Update()
        {
            ProcesarEntrada();
            ActualizarAnimaciones();
            ProcesarCombateHonda();
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
            if (direccionEntrada.x > 0.05f)
            {
                if (spriteRenderer != null) spriteRenderer.flipX = false;
            }
            else if (direccionEntrada.x < -0.05f)
            {
                if (spriteRenderer != null) spriteRenderer.flipX = true;
            }

            if (animator != null && animator.runtimeAnimatorController != null)
            {
                // Compatibilidad con HeroKnight_AnimController (0 = Idle, 1 = Run)
                animator.SetInteger("AnimState", estaMoviendose ? 1 : 0);
                animator.SetBool("Grounded", true);

                int direccionIndex = 0;
                if (Mathf.Abs(direccionEntrada.x) > Mathf.Abs(direccionEntrada.y))
                {
                    direccionIndex = direccionEntrada.x > 0 ? 2 : 3;
                }
                else if (Mathf.Abs(direccionEntrada.y) > 0.001f)
                {
                    direccionIndex = direccionEntrada.y > 0 ? 1 : 0;
                }

                try
                {
                    animator.SetInteger("Direction", direccionIndex);
                    animator.SetBool("IsMoving", estaMoviendose);
                    animator.SetFloat("MoveX", direccionEntrada.x);
                    animator.SetFloat("MoveY", direccionEntrada.y);
                    animator.SetFloat("Speed", direccionEntrada.magnitude * VelocidadActual);
                }
                catch { }
            }
        }

        // Compatibilidad con eventos de animación de Hero Knight
        public void AE_SlideDust() { }

        private void ProcesarCombateHonda()
        {
            if (temporizadorDisparo > 0f)
            {
                temporizadorDisparo -= Time.deltaTime;
            }

            // Si el modo construccion esta activo en ColocadorDefensas, no disparar al hacer click izquierdo
            bool modoConstruccion = ColocadorDefensas.Instance != null && ColocadorDefensas.Instance.ModoConstruccionActivo;

            bool presionoDisparar = false;
            if (!modoConstruccion && Input.GetMouseButtonDown(0)) presionoDisparar = true;
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.J)) presionoDisparar = true;

#if ENABLE_INPUT_SYSTEM
            if (!presionoDisparar)
            {
                if (!modoConstruccion && UnityEngine.InputSystem.Mouse.current != null && UnityEngine.InputSystem.Mouse.current.leftButton.wasPressedThisFrame)
                {
                    presionoDisparar = true;
                }
                if (UnityEngine.InputSystem.Keyboard.current != null)
                {
                    var kb = UnityEngine.InputSystem.Keyboard.current;
                    if (kb.spaceKey.wasPressedThisFrame || kb.jKey.wasPressedThisFrame) presionoDisparar = true;
                }
            }
#endif

            if (presionoDisparar && temporizadorDisparo <= 0f)
            {
                if (animator != null && animator.runtimeAnimatorController != null)
                {
                    animator.SetTrigger("Attack1");
                }
                DispararPiedraHonda();
            }
        }

        private void DispararPiedraHonda()
        {
            temporizadorDisparo = cadenciaDisparo;

            Vector3 direccionDisparo;
            Camera cam = Camera.main;
            if (cam != null)
            {
                Vector3 mouseScreen = Input.mousePosition;
#if ENABLE_INPUT_SYSTEM
                if (UnityEngine.InputSystem.Mouse.current != null)
                {
                    Vector2 m = UnityEngine.InputSystem.Mouse.current.position.ReadValue();
                    mouseScreen = new Vector3(m.x, m.y, 0f);
                }
#endif
                Vector3 mouseMundo = cam.ScreenToWorldPoint(mouseScreen);
                mouseMundo.z = 0f;
                direccionDisparo = (mouseMundo - transform.position).normalized;
            }
            else
            {
                direccionDisparo = (Vector3)ultimaDireccion.normalized;
            }

            if (direccionDisparo.sqrMagnitude < 0.001f)
            {
                direccionDisparo = (Vector3)ultimaDireccion.normalized;
            }

            Vector3 puntoSpawn = transform.position + direccionDisparo * 0.65f;
            GameObject proyectilGO;

            if (prefabProyectilPiedra != null)
            {
                proyectilGO = Instantiate(prefabProyectilPiedra, puntoSpawn, Quaternion.identity);
            }
            else
            {
                proyectilGO = CrearProyectilProcedural(puntoSpawn);
            }

            ProyectilDefensa proyectil = proyectilGO.GetComponent<ProyectilDefensa>();
            if (proyectil != null)
            {
                proyectil.ConfigurarDireccion(direccionDisparo, danioPiedra);
                proyectil.SetVelocidad(velocidadProyectil);
            }
        }

        private GameObject CrearProyectilProcedural(Vector3 spawn)
        {
            GameObject go = new GameObject("Proyectil_PiedraGuanajicho");
            go.transform.position = spawn;

            SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
            sr.sortingOrder = 25;

            Texture2D tex = new Texture2D(12, 12);
            Color grisPiedra = new Color(0.65f, 0.63f, 0.6f, 1f);
            Color sombraPiedra = new Color(0.35f, 0.33f, 0.3f, 1f);

            for (int y = 0; y < 12; y++)
            {
                for (int x = 0; x < 12; x++)
                {
                    float d = Vector2.Distance(new Vector2(x, y), new Vector2(5.5f, 5.5f));
                    if (d <= 5f)
                    {
                        tex.SetPixel(x, y, (x < 5 && y > 5) ? Color.white : (d > 3.8f ? sombraPiedra : grisPiedra));
                    }
                    else
                    {
                        tex.SetPixel(x, y, Color.clear);
                    }
                }
            }
            tex.Apply();
            sr.sprite = Sprite.Create(tex, new Rect(0, 0, 12, 12), new Vector2(0.5f, 0.5f), 16);
            go.transform.localScale = new Vector3(1.4f, 1.4f, 1f);

            CircleCollider2D col = go.AddComponent<CircleCollider2D>();
            col.isTrigger = true;
            col.radius = 0.35f;

            go.AddComponent<ProyectilDefensa>();
            return go;
        }

        private void ProcesarInteraccion()
        {
            bool presionoInteractuar = Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.F);
#if ENABLE_INPUT_SYSTEM
            if (!presionoInteractuar && UnityEngine.InputSystem.Keyboard.current != null)
            {
                var kb = UnityEngine.InputSystem.Keyboard.current;
                presionoInteractuar = kb.eKey.wasPressedThisFrame || kb.fKey.wasPressedThisFrame;
            }
#endif
            if (presionoInteractuar)
            {
                // Prioridad: Si esta cerca de la Antorcha Libertaria, avivarla con puntos civicos
                if (AntorchaLibertaria.Instance != null)
                {
                    float distAntorcha = Vector2.Distance(transform.position, AntorchaLibertaria.Instance.transform.position);
                    if (distAntorcha < radioInteraccion + 0.9f)
                    {
                        AvivarAntorcha();
                        return;
                    }
                }

                Interactuar();
            }
        }

        public void AvivarAntorcha()
        {
            if (AntorchaLibertaria.Instance == null) return;

            if (AntorchaLibertaria.Instance.SaludActual >= AntorchaLibertaria.Instance.SaludMaxima)
            {
                Debug.Log("<color=cyan>[Antorcha]</color> ¡La Antorcha Libertaria ya está al 100% de fuerza!");
                return;
            }

            if (RecursosCivicosManager.Instance != null)
            {
                if (RecursosCivicosManager.Instance.GastarRecursos(costoAvivar))
                {
                    AntorchaLibertaria.Instance.Reparar(curacionAntorcha);
                    Debug.Log($"<color=orange>[Antorcha]</color> ¡Antorcha avivada! (+{curacionAntorcha} HP). Salud: {AntorchaLibertaria.Instance.SaludActual}/{AntorchaLibertaria.Instance.SaludMaxima}");
                }
                else
                {
                    Debug.Log("<color=red>[Antorcha]</color> Necesitas al menos 10 puntos cívicos para avivar la llama.");
                }
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
