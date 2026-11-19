using System;
using System.Collections;
using UnityEngine;

namespace CaminosDeGuanajuato.Duelo
{
    /// <summary>
    /// Controlador visual y de animación rítmica de Guanajicho Tamborilero en la tarima de la Plaza de Dolores.
    /// Carga el sprite de Guanajicho con tambor insurgente, marca el compás del son (BPM)
    /// y ejecuta animaciones de golpe de tambor por cada carril acertado o fallo.
    /// </summary>
    public class GuanajichoDueloController : MonoBehaviour
    {
        [Header("Configuración Visual en Tarima")]
        [SerializeField] private int vidasMax = 3;
        [SerializeField] private int vidasActuales = 3;

        private SpriteRenderer spriteRenderer;
        private Vector3 posicionBase;
        private Vector3 escalaBase = Vector3.one;
        private Coroutine corrutinaAccion;
        private float pulsoRitmicoBPM = 100f;

        public int VidasActuales => vidasActuales;
        public int VidasMax => vidasMax;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null) spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sortingOrder = 10;
            posicionBase = transform.position;
        }

        private void Start()
        {
            CargarSpriteTamborilero();
            PonerEnGuardia();
        }

        public void CargarSpriteTamborilero()
        {
            Sprite spr = Resources.Load<Sprite>("PersonajesColonial/GuanajichoTamborilero");
            if (spr == null)
            {
                Texture2D tex = Resources.Load<Texture2D>("PersonajesColonial/GuanajichoTamborilero");
                if (tex != null)
                {
                    spr = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0f), 380f);
                }
            }

            if (spr != null && spriteRenderer != null)
            {
                spriteRenderer.sprite = spr;
                spriteRenderer.flipX = false;
            }
        }

        private void Update()
        {
            if (corrutinaAccion == null)
            {
                // Rebote rítmico al compás de la música sobre la tarima de madera
                float freq = (pulsoRitmicoBPM / 60f) * Mathf.PI;
                float bounce = Mathf.Abs(Mathf.Sin(Time.time * freq)) * 0.045f;
                transform.localScale = new Vector3(escalaBase.x * (1f - bounce * 0.3f), escalaBase.y * (1f + bounce), 1f);
                transform.position = posicionBase;
            }
        }

        public void ConfigurarBPM(float bpm)
        {
            pulsoRitmicoBPM = Mathf.Max(60f, bpm);
        }

        public void RestablecerVidas()
        {
            vidasActuales = vidasMax;
        }

        public void PonerEnGuardia()
        {
            if (corrutinaAccion != null) StopCoroutine(corrutinaAccion);
            corrutinaAccion = null;
            transform.position = posicionBase;
            transform.rotation = Quaternion.identity;
            if (spriteRenderer != null) spriteRenderer.color = Color.white;
        }

        public void AnimarToqueNota(int carril, bool esPerfecto)
        {
            if (corrutinaAccion != null) StopCoroutine(corrutinaAccion);
            corrutinaAccion = StartCoroutine(RutinaToqueTambor(carril, esPerfecto));
        }

        private IEnumerator RutinaToqueTambor(int carril, bool esPerfecto)
        {
            Color[] coloresCarril =
            {
                new Color(1.0f, 0.78f, 0.78f), // Rojo suave
                new Color(1.0f, 0.96f, 0.72f), // Oro brillante
                new Color(0.78f, 1.0f, 0.82f), // Verde claro
                new Color(0.76f, 0.90f, 1.0f)  // Azul celeste
            };

            Color tinte = coloresCarril[Mathf.Clamp(carril, 0, 3)];
            if (spriteRenderer != null) spriteRenderer.color = esPerfecto ? tinte : Color.white;

            float impulsoX = 0.14f;
            float impulsoY = esPerfecto ? 0.12f : 0.06f;
            transform.position = posicionBase + new Vector3(impulsoX, impulsoY, 0f);
            transform.localScale = new Vector3(escalaBase.x * 1.08f, escalaBase.y * 1.08f, 1f);

            yield return new WaitForSeconds(0.11f);

            if (spriteRenderer != null) spriteRenderer.color = Color.white;
            transform.position = posicionBase;
            transform.localScale = escalaBase;
            corrutinaAccion = null;
        }

        public void AnimarFalloNota()
        {
            if (corrutinaAccion != null) StopCoroutine(corrutinaAccion);
            corrutinaAccion = StartCoroutine(RutinaFallo());
        }

        private IEnumerator RutinaFallo()
        {
            if (spriteRenderer != null) spriteRenderer.color = new Color(1f, 0.45f, 0.45f);
            transform.position = posicionBase + new Vector3(-0.15f, 0f, 0f);
            yield return new WaitForSeconds(0.14f);
            if (spriteRenderer != null) spriteRenderer.color = Color.white;
            transform.position = posicionBase;
            corrutinaAccion = null;
        }
    }
}
