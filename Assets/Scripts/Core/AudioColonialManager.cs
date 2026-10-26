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
        private static AudioColonialManager _instance;
        public static AudioColonialManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindFirstObjectByType<AudioColonialManager>();
                    if (_instance == null)
                    {
                        GameObject go = new GameObject("AudioColonialManager_Global");
                        _instance = go.AddComponent<AudioColonialManager>();
                    }
                }
                return _instance;
            }
        }

        [Header("Música Colonial Mexicana (Fondo)")]
        [SerializeField] private AudioClip clipMusicaFondo;
        [Range(0f, 1f)] [SerializeField] private float volumenMusica = 0.65f;

        [Header("Audio Clips Opcionales (Inspector)")]
        [SerializeField] private AudioClip clipDisparoHonda;
        [SerializeField] private AudioClip clipImpacto;
        [SerializeField] private AudioClip clipCampanaGrito;
        [SerializeField] private AudioClip clipMonedaRecogida;
        [SerializeField] private AudioClip clipColocarDefensa;
        [SerializeField] private AudioClip clipAlertaInvasion;

        [Header("Volumen")]
        [Range(0f, 1f)] [SerializeField] private float volumenGeneral = 0.85f;
        [Range(0f, 1f)] [SerializeField] private float volumenSFX = 0.85f;

        private AudioSource audioSourceMusica;
        private AudioSource audioSourceSFX;
        private AudioSource audioSourceCampana;

        // Clips generados proceduralmente si no hay archivos asignados
        private AudioClip proceduralDisparo;
        private AudioClip proceduralImpacto;
        private AudioClip proceduralCampana;
        private AudioClip proceduralMoneda;
        private AudioClip proceduralColocar;
        private AudioClip proceduralAlerta;
        private AudioClip proceduralRasparHielo;
        private AudioClip proceduralPedidoCompletado;
        private AudioClip proceduralErrorPedido;
        private AudioClip proceduralDerrota;
        private AudioClip proceduralClickBoton;

        public float VolumenGeneral => volumenGeneral;
        public float VolumenMusica => volumenMusica;
        public float VolumenSFX => volumenSFX;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                // Solo destruir este componente duplicado. JAMÁS destruir el gameObject,
                // ya que en las escenas comparte GameObject con HUDDefensaUI, NeveriaGameManager, etc.
                Destroy(this);
                return;
            }
            _instance = this;

            // Proteger entre escenas únicamente si es un GameObject raíz dedicado
            if (transform.parent == null && (gameObject.name == "AudioColonialManager_Global" || GetComponents<Component>().Length <= 3))
            {
                DontDestroyOnLoad(gameObject);
            }

            if (audioSourceMusica == null)
            {
                audioSourceMusica = gameObject.AddComponent<AudioSource>();
                audioSourceMusica.loop = true;
                audioSourceMusica.playOnAwake = false;
                audioSourceMusica.volume = volumenMusica * volumenGeneral;
            }

            if (audioSourceSFX == null)
            {
                audioSourceSFX = gameObject.AddComponent<AudioSource>();
                audioSourceSFX.playOnAwake = false;
            }

            if (audioSourceCampana == null)
            {
                audioSourceCampana = gameObject.AddComponent<AudioSource>();
                audioSourceCampana.playOnAwake = false;
            }

            GenerarClipsProcedurales();
        }

        private void Start()
        {
            IniciarMusicaDeFondo();
        }

        private void GenerarClipsProcedurales()
        {
            proceduralDisparo = CrearClipDisparo();
            proceduralImpacto = CrearClipImpacto();
            proceduralCampana = CrearClipCampana();
            proceduralMoneda = CrearClipMoneda();
            proceduralColocar = CrearClipColocar();
            proceduralAlerta = CrearClipAlerta();
            proceduralRasparHielo = CrearClipRasparHielo();
            proceduralPedidoCompletado = CrearClipPedidoCompletado();
            proceduralErrorPedido = CrearClipErrorPedido();
            proceduralDerrota = CrearClipDerrota();
            proceduralClickBoton = CrearClipClickBoton();
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

        public void ReproducirRasparHielo()
        {
            if (proceduralRasparHielo != null)
            {
                audioSourceSFX.pitch = Random.Range(0.92f, 1.15f);
                audioSourceSFX.PlayOneShot(proceduralRasparHielo, volumenSFX * volumenGeneral * 0.7f);
            }
        }

        public void ReproducirPedidoCompletado()
        {
            if (proceduralPedidoCompletado != null)
            {
                audioSourceSFX.pitch = 1.0f;
                audioSourceSFX.PlayOneShot(proceduralPedidoCompletado, volumenSFX * volumenGeneral * 0.9f);
            }
        }

        public void ReproducirErrorPedido()
        {
            if (proceduralErrorPedido != null)
            {
                audioSourceSFX.pitch = 1.0f;
                audioSourceSFX.PlayOneShot(proceduralErrorPedido, volumenSFX * volumenGeneral * 0.85f);
            }
        }

        public void ReproducirDerrota()
        {
            if (proceduralDerrota != null)
            {
                audioSourceSFX.pitch = 1.0f;
                audioSourceSFX.PlayOneShot(proceduralDerrota, volumenSFX * volumenGeneral * 1.0f);
            }
        }

        public void ReproducirClickBoton()
        {
            if (proceduralClickBoton != null)
            {
                audioSourceSFX.pitch = Random.Range(0.96f, 1.04f);
                audioSourceSFX.PlayOneShot(proceduralClickBoton, volumenSFX * volumenGeneral * 0.75f);
            }
        }

        #region Control de Música de Fondo

        public void IniciarMusicaDeFondo(AudioClip nuevoClip = null)
        {
            if (nuevoClip != null)
            {
                clipMusicaFondo = nuevoClip;
            }

            if (clipMusicaFondo == null)
            {
                clipMusicaFondo = CargarClipWavDesdeDisco("Audio/Musica_CaminosDeGuanajuato.wav");
            }

            if (audioSourceMusica != null)
            {
                if (clipMusicaFondo != null)
                {
                    if (audioSourceMusica.clip != clipMusicaFondo || !audioSourceMusica.isPlaying)
                    {
                        audioSourceMusica.clip = clipMusicaFondo;
                        audioSourceMusica.loop = true;
                        audioSourceMusica.volume = volumenMusica * volumenGeneral;
                        audioSourceMusica.Play();
                        Debug.Log("<color=green>[AudioColonial]</color> 🎶 Música de fondo 'Caminos de Guanajuato' sonando en bucle.");
                    }
                }
            }
        }

        public void DetenerMusicaFondo()
        {
            if (audioSourceMusica != null && audioSourceMusica.isPlaying)
            {
                audioSourceMusica.Stop();
            }
        }

        public void PausarMusica()
        {
            if (audioSourceMusica != null && audioSourceMusica.isPlaying)
            {
                audioSourceMusica.Pause();
            }
        }

        public void ReanudarMusica()
        {
            if (audioSourceMusica != null && !audioSourceMusica.isPlaying)
            {
                audioSourceMusica.UnPause();
            }
        }

        public void AjustarVolumenMusica(float nuevoVolumen)
        {
            volumenMusica = Mathf.Clamp01(nuevoVolumen);
            if (audioSourceMusica != null)
            {
                audioSourceMusica.volume = volumenMusica * volumenGeneral;
            }
        }

        public void AjustarVolumenSFX(float nuevoVolumen)
        {
            volumenSFX = Mathf.Clamp01(nuevoVolumen);
        }

        public void AjustarVolumenGeneral(float nuevoVolumen)
        {
            volumenGeneral = Mathf.Clamp01(nuevoVolumen);
            if (audioSourceMusica != null)
            {
                audioSourceMusica.volume = volumenMusica * volumenGeneral;
            }
        }

        private AudioClip CargarClipWavDesdeDisco(string rutaRelativa)
        {
            try
            {
                string path = System.IO.Path.Combine(Application.dataPath, rutaRelativa);
                if (!System.IO.File.Exists(path))
                {
                    Debug.LogWarning($"[AudioColonial] Archivo de audio no encontrado: {path}");
                    return null;
                }

                byte[] bytes = System.IO.File.ReadAllBytes(path);
                if (bytes.Length < 44) return null;

                int channels = System.BitConverter.ToInt16(bytes, 22);
                int sampleRate = System.BitConverter.ToInt32(bytes, 24);
                int bitsPerSample = System.BitConverter.ToInt16(bytes, 34);

                int dataIndex = 12;
                while (dataIndex < bytes.Length - 8)
                {
                    string chunkId = System.Text.Encoding.ASCII.GetString(bytes, dataIndex, 4);
                    int chunkSize = System.BitConverter.ToInt32(bytes, dataIndex + 4);
                    if (chunkId == "data")
                    {
                        dataIndex += 8;
                        int totalSamples = chunkSize / (bitsPerSample / 8);
                        int samplesPerChannel = totalSamples / channels;
                        float[] audioData = new float[totalSamples];

                        for (int i = 0; i < totalSamples; i++)
                        {
                            short sample16 = System.BitConverter.ToInt16(bytes, dataIndex + i * 2);
                            audioData[i] = sample16 / 32768f;
                        }

                        AudioClip clip = AudioClip.Create("Musica_CaminosDeGuanajuato", samplesPerChannel, channels, sampleRate, false);
                        clip.SetData(audioData, 0);
                        return clip;
                    }
                    dataIndex += 8 + chunkSize;
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"[AudioColonial] Error leyendo archivo WAV {rutaRelativa}: {ex.Message}");
            }
            return null;
        }

        #endregion

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

        private AudioClip CrearClipRasparHielo()
        {
            int sampleRate = 44100;
            float duracion = 0.18f;
            int numSamples = (int)(sampleRate * duracion);
            float[] samples = new float[numSamples];

            for (int i = 0; i < numSamples; i++)
            {
                float t = (float)i / sampleRate;
                float env = Mathf.Sin(Mathf.PI * (float)i / numSamples);
                float ruido = (Random.value * 2f - 1f) * 0.85f;
                float crujido = Mathf.Sin(2f * Mathf.PI * 1800f * t) * 0.25f;
                samples[i] = (ruido + crujido) * env * 0.5f;
            }

            AudioClip clip = AudioClip.Create("SFX_RasparHielo", numSamples, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        private AudioClip CrearClipPedidoCompletado()
        {
            int sampleRate = 44100;
            float duracion = 0.38f;
            int numSamples = (int)(sampleRate * duracion);
            float[] samples = new float[numSamples];

            // Acorde alegre de campanillas virreinales (Do mayor: C5, E5, G5, C6)
            for (int i = 0; i < numSamples; i++)
            {
                float t = (float)i / sampleRate;
                float env = Mathf.Exp(-t * 9f);
                float freq = t < 0.08f ? 523.25f : (t < 0.16f ? 659.25f : (t < 0.24f ? 783.99f : 1046.50f));
                samples[i] = Mathf.Sin(2f * Mathf.PI * freq * t) * env * 0.6f;
            }

            AudioClip clip = AudioClip.Create("SFX_PedidoCompletado", numSamples, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        private AudioClip CrearClipErrorPedido()
        {
            int sampleRate = 44100;
            float duracion = 0.25f;
            int numSamples = (int)(sampleRate * duracion);
            float[] samples = new float[numSamples];

            for (int i = 0; i < numSamples; i++)
            {
                float t = (float)i / sampleRate;
                float env = 1f - (float)i / numSamples;
                float onda1 = Mathf.Sin(2f * Mathf.PI * 140f * t);
                float onda2 = Mathf.Sin(2f * Mathf.PI * 135f * t); // Batimiento disonante
                samples[i] = (onda1 + onda2) * 0.5f * env * 0.6f;
            }

            AudioClip clip = AudioClip.Create("SFX_ErrorPedido", numSamples, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        private AudioClip CrearClipDerrota()
        {
            int sampleRate = 44100;
            float duracion = 1.4f;
            int numSamples = (int)(sampleRate * duracion);
            float[] samples = new float[numSamples];

            // Melodía descendente solemne / fúnebre colonial (E4 -> C4 -> A3 -> F3 con tañido grave)
            float[] frecuencias = { 329.63f, 261.63f, 220.00f, 174.61f };
            float tiempoNota = 0.28f;

            for (int i = 0; i < numSamples; i++)
            {
                float t = (float)i / sampleRate;
                float sample = 0f;

                int indiceNota = Mathf.Min((int)(t / tiempoNota), frecuencias.Length - 1);
                float fNota = frecuencias[indiceNota];
                float tNota = t - (indiceNota * tiempoNota);
                float envNota = Mathf.Exp(-tNota * 5.0f);

                // Onda principal con armónico para timbre metálico colonial
                sample += Mathf.Sin(2f * Mathf.PI * fNota * t) * envNota * 0.5f;
                sample += Mathf.Sin(4f * Mathf.PI * fNota * t) * envNota * 0.2f;

                // Campana grave de fondo fúnebre durante todo el stinger
                float envGong = Mathf.Exp(-t * 2.2f);
                sample += Mathf.Sin(2f * Mathf.PI * 110f * t) * envGong * 0.35f;
                sample += Mathf.Sin(2f * Mathf.PI * 164.81f * t) * envGong * 0.2f;

                samples[i] = Mathf.Clamp(sample, -1f, 1f);
            }

            AudioClip clip = AudioClip.Create("SFX_MisionFallida", numSamples, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        private AudioClip CrearClipClickBoton()
        {
            int sampleRate = 44100;
            float duracion = 0.045f;
            int numSamples = (int)(sampleRate * duracion);
            float[] samples = new float[numSamples];

            // Clic cerámico / madera táctil para botones de UI colonial
            for (int i = 0; i < numSamples; i++)
            {
                float t = (float)i / sampleRate;
                float env = Mathf.Exp(-t * 110f); // Caída rápida en ~20ms
                float freq = Mathf.Lerp(1200f, 280f, (float)i / numSamples);
                float chasquido = Mathf.Sin(2f * Mathf.PI * freq * t);
                float ruido = (Random.value * 2f - 1f) * Mathf.Exp(-t * 220f) * 0.3f;

                samples[i] = (chasquido + ruido) * env * 0.7f;
            }

            AudioClip clip = AudioClip.Create("SFX_ClickBoton", numSamples, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        #endregion
    }
}
