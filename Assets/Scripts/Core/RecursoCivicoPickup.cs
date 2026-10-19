using UnityEngine;
using CaminosDeGuanajuato.Player;

namespace CaminosDeGuanajuato.Core
{
    /// <summary>
    /// Objeto recolectable que sueltan los invasores al ser derrotados en el atrio.
    /// Representa suministros cívicos, monedas virreinales o garrafas de nieve tradicional
    /// que Guanajicho puede recoger para colocar más defensas o avivar la antorcha.
    /// </summary>
    [RequireComponent(typeof(CircleCollider2D))]
    public class RecursoCivicoPickup : MonoBehaviour
    {
        [Header("Valor del Recurso")]
        [SerializeField] private int valorRecursos = 20;
        [SerializeField] private float tiempoDeVida = 18.0f;

        [Header("Animación")]
        [SerializeField] private float velocidadOscilacion = 4.0f;
        [SerializeField] private float amplitudOscilacion = 0.15f;

        private Vector3 posicionInicial;
        private SpriteRenderer spriteRenderer;
        private bool yaRecogido = false;

        private static Sprite spriteLootCompartido = null;

        public void Configurar(int valor)
        {
            valorRecursos = valor;
        }

        private void Awake()
        {
            posicionInicial = transform.position;
            spriteRenderer = GetComponent<SpriteRenderer>();

            CircleCollider2D col = GetComponent<CircleCollider2D>();
            if (col == null)
            {
                col = gameObject.AddComponent<CircleCollider2D>();
            }
            if (col != null)
            {
                col.isTrigger = true;
                col.radius = 0.45f;
            }

            // Asegurar que tenga un sprite visible si no fue asignado
            if (spriteRenderer == null)
            {
                spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            }

            if (spriteRenderer.sprite == null)
            {
                if (spriteLootCompartido == null)
                {
                    spriteLootCompartido = GenerarSpritePorDefecto();
                }
                spriteRenderer.sprite = spriteLootCompartido;
            }

            spriteRenderer.sortingOrder = 9;
        }

        private void Start()
        {
            Destroy(gameObject, tiempoDeVida);
        }

        private void Update()
        {
            if (yaRecogido) return;

            // Animación de flotado suave
            float desplazamientoY = Mathf.Sin(Time.time * velocidadOscilacion) * amplitudOscilacion;
            transform.position = posicionInicial + new Vector3(0f, desplazamientoY, 0f);

            // Rotación leve o brillo
            transform.Rotate(0f, 0f, 45f * Time.deltaTime);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (yaRecogido) return;

            GuanajichoController player = collision.GetComponent<GuanajichoController>();
            if (player != null)
            {
                Recoger();
            }
        }

        private void Recoger()
        {
            yaRecogido = true;

            if (RecursosCivicosManager.Instance != null)
            {
                RecursosCivicosManager.Instance.AgregarRecursos(valorRecursos);
            }

            Debug.Log($"<color=yellow>[Botín Cívico]</color> ¡Guanajicho recogió suministros (+{valorRecursos} puntos cívicos)!");

            // Pequeño efecto de escala al recoger
            StartCoroutine(RutinaDesvanecer());
        }

        private System.Collections.IEnumerator RutinaDesvanecer()
        {
            float tiempo = 0.2f;
            Vector3 escalaInicial = transform.localScale;

            for (float t = 0; t < tiempo; t += Time.deltaTime)
            {
                float progreso = t / tiempo;
                transform.localScale = Vector3.Lerp(escalaInicial, escalaInicial * 1.6f, progreso);
                if (spriteRenderer != null)
                {
                    Color c = spriteRenderer.color;
                    c.a = 1f - progreso;
                    spriteRenderer.color = c;
                }
                yield return null;
            }

            Destroy(gameObject);
        }

        private static Sprite GenerarSpritePorDefecto()
        {
            Texture2D tex = new Texture2D(16, 16);
            Color dorado = new Color(0.98f, 0.8f, 0.18f, 1f);
            Color borde = new Color(0.7f, 0.5f, 0.05f, 1f);

            for (int y = 0; y < 16; y++)
            {
                for (int x = 0; x < 16; x++)
                {
                    float dist = Vector2.Distance(new Vector2(x, y), new Vector2(7.5f, 7.5f));
                    if (dist <= 6.5f)
                    {
                        tex.SetPixel(x, y, dist > 5.0f ? borde : dorado);
                    }
                    else
                    {
                        tex.SetPixel(x, y, Color.clear);
                    }
                }
            }
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, 16, 16), new Vector2(0.5f, 0.5f), 16);
        }
    }
}
