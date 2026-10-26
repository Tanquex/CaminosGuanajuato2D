using System;
using System.Collections.Generic;
using UnityEngine;
using CaminosDeGuanajuato.Core;
using CaminosDeGuanajuato.Player;

namespace CaminosDeGuanajuato.Defense
{
    [System.Serializable]
    public class OpcionDefensa
    {
        public string nombre;
        public TipoDefensa tipo;
        public int costo;
        public GameObject prefab;
        public Sprite icono;
        [TextArea(1, 2)] public string descripcion;
    }

    /// <summary>
    /// Mecanica principal del Avance 1 (Issue #3):
    /// Sistema de colocacion e interaccion de elementos defensivos en el atrio.
    /// Soporta mouse y colocacion directa por personaje con teclado.
    /// Incluye respaldo para creacion procedural si los prefabs no estan asignados.
    /// </summary>
    public class ColocadorDefensas : MonoBehaviour
    {
        public static ColocadorDefensas Instance { get; private set; }

        [Header("Catalogo de Defensas")]
        [SerializeField] private List<OpcionDefensa> catalogoDefensas = new List<OpcionDefensa>();
        [SerializeField] private int indiceSeleccionado = 0;

        [Header("Ajustes de Cuadricula")]
        [SerializeField] private float tamanoCelda = 1.0f;
        [SerializeField] private LayerMask capaObstaculos;

        [Header("Previsualizacion")]
        [SerializeField] private GameObject objetoFantasma;
        [SerializeField] private SpriteRenderer spriteFantasma;
        [SerializeField] private Color colorValido = new Color(0.2f, 1f, 0.2f, 0.65f);
        [SerializeField] private Color colorInvalido = new Color(1f, 0.2f, 0.2f, 0.65f);

        [Header("Referencia a Guanajicho")]
        [SerializeField] private GuanajichoController guanajicho;

        // Estado
        private Vector3 posicionObjetivoGrid;
        private bool esPosicionValida;
        private Camera camaraPrincipal;

        public List<OpcionDefensa> Catalogo => catalogoDefensas;
        public int IndiceSeleccionado => indiceSeleccionado;
        public OpcionDefensa DefensaSeleccionada => (catalogoDefensas != null && catalogoDefensas.Count > 0 && indiceSeleccionado < catalogoDefensas.Count) ? catalogoDefensas[indiceSeleccionado] : null;

        public event Action<OpcionDefensa, Vector3> OnDefensaColocada;
        public event Action<int> OnDefensaSeleccionadaCambiada;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }
            Instance = this;
            camaraPrincipal = Camera.main;

            VerificarCatalogoPorDefecto();
        }

        private void Start()
        {
            if (guanajicho == null)
            {
                guanajicho = FindAnyObjectByType<GuanajichoController>();
            }

            CrearFantasmaSiNoExiste();
            SeleccionarDefensa(0);
        }

        private void VerificarCatalogoPorDefecto()
        {
            if (catalogoDefensas == null || catalogoDefensas.Count == 0)
            {
                catalogoDefensas = new List<OpcionDefensa>
                {
                    new OpcionDefensa { nombre = "Barricada Colonial", tipo = TipoDefensa.BarricadaColonial, costo = 25, descripcion = "Bloquea el paso de invasores." },
                    new OpcionDefensa { nombre = "Hondero Insurgente", tipo = TipoDefensa.HonderoInsurgente, costo = 50, descripcion = "Lanza proyectiles a distancia." }
                };
            }
        }

        [Header("Modo Construcción")]
        [SerializeField] private bool modoConstruccionActivo = false;

        public bool ModoConstruccionActivo => modoConstruccionActivo;

        public void ActivarModoConstruccion(bool activar)
        {
            modoConstruccionActivo = activar;
            if (objetoFantasma != null) objetoFantasma.SetActive(modoConstruccionActivo);
        }

        private void Update()
        {
            ProcesarTeclasSeleccion();

            if (modoConstruccionActivo)
            {
                if (objetoFantasma != null && !objetoFantasma.activeSelf) objetoFantasma.SetActive(true);
                ActualizarPosicionMouse();
                ProcesarEntradaColocacion();
            }
            else
            {
                if (objetoFantasma != null && objetoFantasma.activeSelf) objetoFantasma.SetActive(false);
            }
        }

        private void ProcesarTeclasSeleccion()
        {
            // Tecla B para alternar modo construccion
            if (Input.GetKeyDown(KeyCode.B))
            {
                modoConstruccionActivo = !modoConstruccionActivo;
                if (modoConstruccionActivo) ActualizarSpriteFantasma();
            }

            // Seleccionar defensa activa directamente el modo construccion
            if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
            {
                SeleccionarDefensa(0);
                modoConstruccionActivo = true;
            }
            if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
            {
                SeleccionarDefensa(1);
                modoConstruccionActivo = true;
            }
            if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
            {
                SeleccionarDefensa(2);
                modoConstruccionActivo = true;
            }

            // Tecla Q o Clic Derecho para cancelar construccion
            if (Input.GetKeyDown(KeyCode.Q) || Input.GetMouseButtonDown(1))
            {
                modoConstruccionActivo = false;
            }

#if ENABLE_INPUT_SYSTEM
            if (UnityEngine.InputSystem.Keyboard.current != null)
            {
                var kb = UnityEngine.InputSystem.Keyboard.current;
                if (kb.bKey.wasPressedThisFrame)
                {
                    modoConstruccionActivo = !modoConstruccionActivo;
                }
                if (kb.digit1Key.wasPressedThisFrame || kb.numpad1Key.wasPressedThisFrame)
                {
                    SeleccionarDefensa(0);
                    modoConstruccionActivo = true;
                }
                if (kb.digit2Key.wasPressedThisFrame || kb.numpad2Key.wasPressedThisFrame)
                {
                    SeleccionarDefensa(1);
                    modoConstruccionActivo = true;
                }
                if (kb.qKey.wasPressedThisFrame)
                {
                    modoConstruccionActivo = false;
                }
            }

            if (UnityEngine.InputSystem.Mouse.current != null && UnityEngine.InputSystem.Mouse.current.rightButton.wasPressedThisFrame)
            {
                modoConstruccionActivo = false;
            }
#endif
        }

        public void SeleccionarDefensa(int indice)
        {
            if (catalogoDefensas == null || catalogoDefensas.Count == 0) return;

            indiceSeleccionado = Mathf.Clamp(indice, 0, catalogoDefensas.Count - 1);
            ActualizarSpriteFantasma();
            OnDefensaSeleccionadaCambiada?.Invoke(indiceSeleccionado);
        }

        private void ActualizarPosicionMouse()
        {
            if (camaraPrincipal == null) camaraPrincipal = Camera.main;
            if (camaraPrincipal == null) return;

            Vector3 mousePantalla = Input.mousePosition;
#if ENABLE_INPUT_SYSTEM
            if (UnityEngine.InputSystem.Mouse.current != null)
            {
                Vector2 mPos = UnityEngine.InputSystem.Mouse.current.position.ReadValue();
                mousePantalla = new Vector3(mPos.x, mPos.y, 0f);
            }
#endif

            Vector3 mouseMundo = camaraPrincipal.ScreenToWorldPoint(mousePantalla);
            mouseMundo.z = 0f;

            float xAjustado = Mathf.Round(mouseMundo.x / tamanoCelda) * tamanoCelda;
            float yAjustado = Mathf.Round(mouseMundo.y / tamanoCelda) * tamanoCelda;

            posicionObjetivoGrid = new Vector3(xAjustado, yAjustado, 0f);

            esPosicionValida = ValidarPosicion(posicionObjetivoGrid, DefensaSeleccionada != null ? DefensaSeleccionada.costo : 0);

            if (objetoFantasma != null)
            {
                objetoFantasma.transform.position = posicionObjetivoGrid;
                if (spriteFantasma != null)
                {
                    spriteFantasma.color = esPosicionValida ? colorValido : colorInvalido;
                }
            }
        }

        private void ProcesarEntradaColocacion()
        {
            bool clicIzquierdo = Input.GetMouseButtonDown(0);
#if ENABLE_INPUT_SYSTEM
            if (!clicIzquierdo && UnityEngine.InputSystem.Mouse.current != null)
            {
                clicIzquierdo = UnityEngine.InputSystem.Mouse.current.leftButton.wasPressedThisFrame;
            }
#endif
            if (clicIzquierdo && esPosicionValida)
            {
                ColocarDefensaEn(posicionObjetivoGrid);
            }

            bool presionoE = Input.GetKeyDown(KeyCode.E);
#if ENABLE_INPUT_SYSTEM
            if (!presionoE && UnityEngine.InputSystem.Keyboard.current != null)
            {
                presionoE = UnityEngine.InputSystem.Keyboard.current.eKey.wasPressedThisFrame;
            }
#endif
            if (presionoE)
            {
                ColocarFrenteAGuanajicho();
            }
        }

        public void ColocarFrenteAGuanajicho()
        {
            if (guanajicho == null) return;

            Vector3 posicionFrente = guanajicho.transform.position + (Vector3)(guanajicho.UltimaDireccion.normalized * tamanoCelda);
            float xAjustado = Mathf.Round(posicionFrente.x / tamanoCelda) * tamanoCelda;
            float yAjustado = Mathf.Round(posicionFrente.y / tamanoCelda) * tamanoCelda;
            Vector3 posGridFrente = new Vector3(xAjustado, yAjustado, 0f);

            if (ValidarPosicion(posGridFrente, DefensaSeleccionada != null ? DefensaSeleccionada.costo : 0))
            {
                ColocarDefensaEn(posGridFrente);
            }
        }

        private bool ValidarPosicion(Vector3 posicion, int costo)
        {
            if (RecursosCivicosManager.Instance != null && !RecursosCivicosManager.Instance.PuedeCostear(costo))
            {
                return false;
            }

            Collider2D colision = Physics2D.OverlapBox(posicion, Vector2.one * (tamanoCelda * 0.7f), 0f, capaObstaculos);
            if (colision != null && !colision.isTrigger)
            {
                return false;
            }

            return true;
        }

        public bool ColocarDefensaEn(Vector3 posicion)
        {
            OpcionDefensa opcion = DefensaSeleccionada;
            if (opcion == null) return false;

            if (RecursosCivicosManager.Instance != null)
            {
                if (!RecursosCivicosManager.Instance.GastarRecursos(opcion.costo))
                {
                    return false;
                }
            }

            GameObject nuevaDefensa;

            if (opcion.prefab != null)
            {
                nuevaDefensa = Instantiate(opcion.prefab, posicion, Quaternion.identity);
            }
            else
            {
                // Respaldo visual garantizado si el prefab no fue vinculado
                nuevaDefensa = CrearDefensaProcedural(opcion, posicion);
            }

            nuevaDefensa.name = $"{opcion.nombre} (Instancia)";

            if (AudioColonialManager.Instance != null)
            {
                AudioColonialManager.Instance.ReproducirColocarDefensa();
            }

            OnDefensaColocada?.Invoke(opcion, posicion);
            Debug.Log($"<color=cyan>[Defensas]</color> {opcion.nombre} colocada en {posicion}. Puntos restantes: {(RecursosCivicosManager.Instance != null ? RecursosCivicosManager.Instance.RecursosActuales : 0)}");
            return true;
        }

        private GameObject CrearDefensaProcedural(OpcionDefensa opcion, Vector3 posicion)
        {
            GameObject go = new GameObject(opcion.nombre);
            go.transform.position = posicion;

            SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
            sr.sortingOrder = 5;

            // Intentar usar un sprite de los recursos cargados
            if (opcion.icono != null) sr.sprite = opcion.icono;
            else
            {
                // Crear sprite de color tematico
                Texture2D tex = new Texture2D(32, 32);
                Color colorBase = opcion.tipo == TipoDefensa.BarricadaColonial ? new Color(0.6f, 0.4f, 0.2f) : new Color(0.2f, 0.6f, 0.8f);
                Color[] cols = new Color[32 * 32];
                for (int i = 0; i < cols.Length; i++) cols[i] = colorBase;
                tex.SetPixels(cols);
                tex.Apply();
                sr.sprite = Sprite.Create(tex, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f), 32);
            }

            BoxCollider2D bc = go.AddComponent<BoxCollider2D>();
            bc.size = Vector2.one * 0.8f;

            DefensaElemento def = go.AddComponent<DefensaElemento>();
            // Se inicializa con los valores del tipo
            return go;
        }

        private void CrearFantasmaSiNoExiste()
        {
            if (objetoFantasma == null)
            {
                objetoFantasma = new GameObject("GhostPreview_Defensa");
                objetoFantasma.transform.SetParent(transform);
                spriteFantasma = objetoFantasma.AddComponent<SpriteRenderer>();
                spriteFantasma.sortingOrder = 100;

                Texture2D tex = new Texture2D(32, 32);
                Color[] cols = new Color[32 * 32];
                for (int i = 0; i < cols.Length; i++) cols[i] = Color.white;
                tex.SetPixels(cols);
                tex.Apply();
                spriteFantasma.sprite = Sprite.Create(tex, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f), 32);
            }
            ActualizarSpriteFantasma();
        }

        private void ActualizarSpriteFantasma()
        {
            if (spriteFantasma == null) return;

            OpcionDefensa actual = DefensaSeleccionada;
            if (actual != null && actual.prefab != null)
            {
                SpriteRenderer srPrefab = actual.prefab.GetComponentInChildren<SpriteRenderer>();
                if (srPrefab != null && srPrefab.sprite != null)
                {
                    spriteFantasma.sprite = srPrefab.sprite;
                    spriteFantasma.transform.localScale = srPrefab.transform.localScale;
                }
            }
        }
    }
}
