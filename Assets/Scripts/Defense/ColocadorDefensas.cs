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
    /// Permite colocar barricadas y defensores tanto con el cursor del raton (cuadricula)
    /// como directamente frente al personaje Guanajicho interactuando con la tecla [E].
    /// </summary>
    public class ColocadorDefensas : MonoBehaviour
    {
        public static ColocadorDefensas Instance { get; private set; }

        [Header("Catalogo de Defensas")]
        [SerializeField] private List<OpcionDefensa> catalogoDefensas = new List<OpcionDefensa>();
        [SerializeField] private int indiceSeleccionado = 0;

        [Header("Ajustes de Cuadricula (Grid)")]
        [SerializeField] private float tamanoCelda = 1.0f;
        [SerializeField] private LayerMask capaObstaculos;

        [Header("Previsualizacion")]
        [SerializeField] private GameObject objetoFantasma;
        [SerializeField] private SpriteRenderer spriteFantasma;
        [SerializeField] private Color colorValido = new Color(0.2f, 1f, 0.2f, 0.6f);
        [SerializeField] private Color colorInvalido = new Color(1f, 0.2f, 0.2f, 0.6f);

        [Header("Referencia a Guanajicho")]
        [SerializeField] private GuanajichoController guanajicho;

        // Estado
        private Vector3 posicionObjetivoGrid;
        private bool esPosicionValida;
        private Camera camaraPrincipal;

        public List<OpcionDefensa> Catalogo => catalogoDefensas;
        public int IndiceSeleccionado => indiceSeleccionado;
        public OpcionDefensa DefensaSeleccionada => (catalogoDefensas.Count > 0 && indiceSeleccionado < catalogoDefensas.Count) ? catalogoDefensas[indiceSeleccionado] : null;

        public event Action<OpcionDefensa, Vector3> OnDefensaColocada;
        public event Action<int> OnDefensaSeleccionadaCambiada;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            camaraPrincipal = Camera.main;
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

        private void Update()
        {
            ProcesarTeclasSeleccion();
            ActualizarPosicionMouse();
            ProcesarEntradaColocacion();
        }

        private void ProcesarTeclasSeleccion()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) SeleccionarDefensa(0);
            if (Input.GetKeyDown(KeyCode.Alpha2)) SeleccionarDefensa(1);
            if (Input.GetKeyDown(KeyCode.Alpha3)) SeleccionarDefensa(2);
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

            Vector3 mouseMundo = camaraPrincipal.ScreenToWorldPoint(Input.mousePosition);
            mouseMundo.z = 0f;

            // Ajuste a la cuadricula (Grid Snapping)
            float xAjustado = Mathf.Round(mouseMundo.x / tamanoCelda) * tamanoCelda;
            float yAjustado = Mathf.Round(mouseMundo.y / tamanoCelda) * tamanoCelda;

            posicionObjetivoGrid = new Vector3(xAjustado, yAjustado, 0f);

            // Validar si la posicion esta libre y si alcanzan los recursos
            esPosicionValida = ValidarPosicion(posicionObjetivoGrid, DefensaSeleccionada != null ? DefensaSeleccionada.costo : 0);

            // Mover y colorear el fantasma
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
            // Clic izquierdo: Colocar en la posicion del mouse
            if (Input.GetMouseButtonDown(0))
            {
                if (esPosicionValida)
                {
                    ColocarDefensaEn(posicionObjetivoGrid);
                }
            }

            // Tecla E o Espacio: Guanajicho coloca una defensa en la casilla frente a el
            if (Input.GetKeyDown(KeyCode.E))
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
            // 1. Verificar si tiene recursos civicos suficientes
            if (RecursosCivicosManager.Instance != null && !RecursosCivicosManager.Instance.PuedeCostear(costo))
            {
                return false;
            }

            // 2. Verificar que no haya colision o solapamiento con otra estructura
            Collider2D colision = Physics2D.OverlapBox(posicion, Vector2.one * (tamanoCelda * 0.8f), 0f, capaObstaculos);
            if (colision != null)
            {
                return false;
            }

            return true;
        }

        public bool ColocarDefensaEn(Vector3 posicion)
        {
            OpcionDefensa opcion = DefensaSeleccionada;
            if (opcion == null || opcion.prefab == null) return false;

            if (RecursosCivicosManager.Instance != null)
            {
                if (!RecursosCivicosManager.Instance.GastarRecursos(opcion.costo))
                {
                    return false;
                }
            }

            GameObject nuevaDefensa = Instantiate(opcion.prefab, posicion, Quaternion.identity);
            nuevaDefensa.name = $"{opcion.nombre} (Instancia)";

            OnDefensaColocada?.Invoke(opcion, posicion);
            return true;
        }

        private void CrearFantasmaSiNoExiste()
        {
            if (objetoFantasma == null)
            {
                objetoFantasma = new GameObject("GhostPreview_Defensa");
                objetoFantasma.transform.SetParent(transform);
                spriteFantasma = objetoFantasma.AddComponent<SpriteRenderer>();
                spriteFantasma.sortingOrder = 100;
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
                if (srPrefab != null)
                {
                    spriteFantasma.sprite = srPrefab.sprite;
                    spriteFantasma.transform.localScale = srPrefab.transform.localScale;
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = esPosicionValida ? Color.green : Color.red;
            Gizmos.DrawWireCube(posicionObjetivoGrid, Vector3.one * tamanoCelda);
        }
    }
}
