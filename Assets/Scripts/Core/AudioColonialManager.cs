using UnityEngine;

namespace CaminosDeGuanajuato.Core
{
    /// <summary>
    /// Gestor de Audio de época para Caminos de Guanajuato 2D.
    /// Sintetiza de forma procedural efectos de sonido de estilo 16-bit retro
    /// (campanadas de Dolores Hidalgo, disparos de honda, impactos, recolección de monedas y colocación de defensas)
    /// garantizando que el juego tenga retroalimentación acústica viva sin dependencias externas.
    /// También permite asignar clips .wav o .mp3 en el Inspector cuando estén disponibles.
    /// </summary>
    public class AudioColonialManager : MonoBehaviour
    {
        public static AudioColonialManager Instance { get; private set; }

        [Header("Audio Clips Opcionales (Inspector)")]
        [SerializeField] private AudioClip clipDisparoHonda;
        [SerializeField] private AudioClip clipImpacto;
        [SerializeField] private AudioClip clipCampanaGrito;
        [SerializeField] private AudioClip clipMonedaRecogida;
        [SerializeField] private AudioClip clipColocarDefensa;
        [SerializeField] private AudioClip clipAlertaInvasion;

        [Header("Volumen")]
        [Range(0f, 1f)] [SerializeField] private float volumenGeneral = 0.85f;
        [Range(0f, 1f)] [SerializeField] private float volumenSFX = 0.9f;

        private AudioSource audioSourceSFX;
        private AudioSource audioSourceCampana;

        // Clips generados proceduralmente si no hay archivos asignados
        private AudioClip proceduralDisparo;
        private AudioClip proceduralImpacto;
        private AudioClip proceduralCampana;
        private AudioClip proceduralMoneda;
        private AudioClip proceduralColocar;
        private AudioClip proceduralAlerta;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            audioSourceSFX = gameObject.AddComponent<AudioSource>();
            audioSourceSFX.playOnAwake = false;

            audioSourceCampana = gameObject.AddComponent<AudioSource>();
            audioSourceCampana.playOnAwake = false;

            GenerarClipsProcedurales();
        }

        private void GenerarClipsProcedurales()
        {
            proceduralDisparo = CrearClipDisparo();
            proceduralImpacto = CrearClipImpacto();
            proceduralCampana = CrearClipCampana();
            proceduralMoneda = CrearClipMoneda();
            proceduralColocar = CrearClipColocar();
            proceduralAlerta = CrearClipAlerta();
        }

        public void ReproducirDisparoHonda()
        {
            AudioClip clip = clipDisparoHonda != null ? clipDisparoHonda : proceduralDisparo;
            if (clip != null)
            {
                audioSourceSFX.pitch = Random.Range(0.92f, 1.08f);
                audioSourceSFX.PlayOneShot(clip, volumenSFX * volumenGeneral * 0.7f);
            }
        }

        public void ReproducirImpacto()
        {
            AudioClip clip = clipImpacto != null ? clipImpacto : proceduralImpacto;
            if (clip != null)
            {
                audioSourceSFX.pitch = Random.Range(0.88f, 1.12f);
                audioSourceSFX.PlayOneShot(clip, volumenSFX * volumenGeneral * 0.8f);
            }
        }

        public void ReproducirCampanaGritoLibertad()
        {
            AudioClip clip = clipCampanaGrito != null ? clipCampanaGrito : proceduralCampana;
            if (clip != null)
            {
                audioSourceCampana.pitch = 1.0f;
                audioSourceCampana.PlayOneShot(clip, volumenGeneral * 1.0f);
            }
        }

        public void ReproducirMonedaRecogida()
        {
            AudioClip clip = clipMonedaRecogida != null ? clipMonedaRecogida : proceduralMoneda;
            if (clip != null)
            {
                audioSourceSFX.pitch = Random.Range(0.96f, 1.06f);
                audioSourceSFX.PlayOneShot(clip, volumenSFX * volumenGeneral * 0.75f);
            }
        }

        public void ReproducirColocarDefensa()
        {
            AudioClip clip = clipColocarDefensa != null ? clipColocarDefensa : proceduralColocar;
            if (clip != null)
            {
                audioSourceSFX.pitch = Random.Range(0.94f, 1.05f);
                audioSourceSFX.PlayOneShot(clip, volumenSFX * volumenGeneral * 0.9f);
            }
        }

        public void ReproducirAlertaInvasion()
        {
            AudioClip clip = clipAlertaInvasion != null ? clipAlertaInvasion : proceduralAlerta;
            if (clip != null)
            {
                audioSourceSFX.pitch = 1.0f;
                audioSourceSFX.PlayOneShot(clip, volumenSFX * volumenGeneral * 0.95f);
            }
        }

        #region Generadores de Formas de Onda

        private AudioClip CrearClipDisparo()
        {
            int sampleRate = 44100;
            float duracion = 0.16f;
            int numSamples = (int)(sampleRate * duracion);
            float[] samples = new float[numSamples];

            for (int i = 0; i < numSamples; i++)
            {
                float t = (float)i / sampleRate;
                float env = 1f - (float)i / numSamples;
                float freq = Mathf.Lerp(680f, 140f, (float)i / numSamples);
                float noise = (Random.value * 2f - 1f) * 0.18f;
                samples[i] = (Mathf.Sin(2f * Mathf.PI * freq * t) + noise) * env * 0.6f;
            }

            AudioClip clip = AudioClip.Create("SFX_DisparoHonda", numSamples, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        private AudioClip CrearClipImpacto()
        {
            int sampleRate = 44100;
            float duracion = 0.14f;
            int numSamples = (int)(sampleRate * duracion);
            float[] samples = new float[numSamples];

            for (int i = 0; i < numSamples; i++)
            {
                float t = (float)i / sampleRate;
                float env = Mathf.Exp(-t * 28f);
                float sub = Mathf.Sin(2f * Mathf.PI * 130f * t);
                float noise = (Random.value * 2f - 1f) * 0.45f;
                samples[i] = (sub * 0.6f + noise * 0.4f) * env * 0.7f;
            }

            AudioClip clip = AudioClip.Create("SFX_ImpactoPiedra", numSamples, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        private AudioClip CrearClipCampana()
        {
            int sampleRate = 44100;
            float duracion = 2.4f;
            int numSamples = (int)(sampleRate * duracion);
            float[] samples = new float[numSamples];

            float f1 = 440f; // Fundamental (La4)
            float f2 = 880f; // Armónico
            float f3 = 1320f;// Armónico agudo
            float f4 = 660f; // Resonancia de campana colonial

            for (int i = 0; i < numSamples; i++)
            {
                float t = (float)i / sampleRate;
                float env1 = Mathf.Exp(-t * 1.8f);
                float env2 = Mathf.Exp(-t * 3.2f);
                float env3 = Mathf.Exp(-t * 5.0f);
                float tremolo = 1f + 0.08f * Mathf.Sin(2f * Mathf.PI * 5.5f * t);

                float signal = (Mathf.Sin(2f * Mathf.PI * f1 * t) * env1 * 0.5f)
                             + (Mathf.Sin(2f * Mathf.PI * f4 * t) * env1 * 0.3f)
                             + (Mathf.Sin(2f * Mathf.PI * f2 * t) * env2 * 0.25f)
                             + (Mathf.Sin(2f * Mathf.PI * f3 * t) * env3 * 0.15f);

                samples[i] = signal * tremolo * 0.85f;
            }

            AudioClip clip = AudioClip.Create("SFX_CampanaLibertad", numSamples, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        private AudioClip CrearClipMoneda()
        {
            int sampleRate = 44100;
            float duracion = 0.28f;
            int numSamples = (int)(sampleRate * duracion);
            float[] samples = new float[numSamples];

            for (int i = 0; i < numSamples; i++)
            {
                float t = (float)i / sampleRate;
                float env = Mathf.Exp(-t * 14f);
                float freq = t < 0.08f ? 987.77f : 1318.51f; // B5 -> E6
                samples[i] = Mathf.Sin(2f * Mathf.PI * freq * t) * env * 0.5f;
            }

            AudioClip clip = AudioClip.Create("SFX_MonedaCivica", numSamples, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        private AudioClip CrearClipColocar()
        {
            int sampleRate = 44100;
            float duracion = 0.18f;
            int numSamples = (int)(sampleRate * duracion);
            float[] samples = new float[numSamples];

            for (int i = 0; i < numSamples; i++)
            {
                float t = (float)i / sampleRate;
                float env = Mathf.Exp(-t * 22f);
                float f = Mathf.Lerp(340f, 90f, (float)i / numSamples);
                float click = (Random.value * 2f - 1f) * 0.35f;
                samples[i] = (Mathf.Sin(2f * Mathf.PI * f * t) + click) * env * 0.75f;
            }

            AudioClip clip = AudioClip.Create("SFX_ColocarDefensa", numSamples, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        private AudioClip CrearClipAlerta()
        {
            int sampleRate = 44100;
            float duracion = 0.35f;
            int numSamples = (int)(sampleRate * duracion);
            float[] samples = new float[numSamples];

            for (int i = 0; i < numSamples; i++)
            {
                float t = (float)i / sampleRate;
                float env = 1f - (float)i / numSamples;
                float f = (t < 0.16f) ? 330f : 440f;
                samples[i] = Mathf.Sin(2f * Mathf.PI * f * t) * env * 0.6f;
            }

            AudioClip clip = AudioClip.Create("SFX_AlertaInvasion", numSamples, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        #endregion
    }
}
