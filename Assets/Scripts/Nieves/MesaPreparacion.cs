using System;
using UnityEngine;
using CaminosDeGuanajuato.Core;

namespace CaminosDeGuanajuato.Nieves
{
    /// <summary>
    /// Estación de trabajo y mostrador culinario de Guanajicho.
    /// Controla la preparación paso a paso:
    /// 1. Selección de base (Barquillo o Vaso de barro).
    /// 2. Batido y raspado de hielo en la garrafa tradicional de madera.
    /// 3. Adición de la pulpa/sabor exótico dolorense.
    /// 4. Servido o descarte si hubo equivocación.
    /// </summary>
    public class MesaPreparacion : MonoBehaviour
    {
        public static MesaPreparacion Instance { get; private set; }

        [Header("Estado Actual de Preparación")]
        [SerializeField] private TipoRecipiente recipienteActual = TipoRecipiente.Ninguno;
        [SerializeField] private SaborNieve saborActual = SaborNieve.Ninguno;
        [SerializeField] private float progresoHielo = 0f;
        [SerializeField] private bool tieneHieloListo = false;

        public TipoRecipiente RecipienteActual => recipienteActual;
        public SaborNieve SaborActual => saborActual;
        public float ProgresoHielo => progresoHielo;
        public bool TieneHieloListo => tieneHieloListo;
        public bool TieneArticuloEnPreparacion => recipienteActual != TipoRecipiente.Ninguno || tieneHieloListo || saborActual != SaborNieve.Ninguno;

        public event Action OnMesaActualizada;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Update()
        {
            ProcesarEntradaTeclado();
        }

        private void ProcesarEntradaTeclado()
        {
            // Atajos de Recipiente: [1] Barquillo, [2] Vaso de Barro
            if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
            {
                SeleccionarRecipiente(TipoRecipiente.BarquilloCrujiente);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
            {
                SeleccionarRecipiente(TipoRecipiente.VasoDeBarro);
            }

            // Atajo de Raspado de Hielo: [Espacio]
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKey(KeyCode.Space))
            {
                if (recipienteActual != TipoRecipiente.Ninguno && !tieneHieloListo)
                {
                    RasparHielo(Time.deltaTime * 2.5f);
                }
            }

            // Atajos de Sabores: [Q] Garambullo, [W] Borrachita, [E] Queso c/ Fresa, [R] Aguacate, [T] Limón
            if (Input.GetKeyDown(KeyCode.Q)) AgregarSabor(SaborNieve.Garambullo);
            if (Input.GetKeyDown(KeyCode.W)) AgregarSabor(SaborNieve.Borrachita);
            if (Input.GetKeyDown(KeyCode.E)) AgregarSabor(SaborNieve.QuesoConFresa);
            if (Input.GetKeyDown(KeyCode.R)) AgregarSabor(SaborNieve.Aguacate);
            if (Input.GetKeyDown(KeyCode.T)) AgregarSabor(SaborNieve.LimonVirreinal);

            // Atajo de Desechar: [Backspace] o [Delete]
            if (Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.Delete))
            {
                DesecharPreparacion();
            }

            // Atajo de Servir: [Return] / [Enter]
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                if (NeveriaGameManager.Instance != null)
                {
                    NeveriaGameManager.Instance.IntentarServirPedidoActual();
                }
            }
        }

        public void SeleccionarRecipiente(TipoRecipiente nuevoRecipiente)
        {
            recipienteActual = nuevoRecipiente;
            if (AudioColonialManager.Instance != null)
            {
                AudioColonialManager.Instance.ReproducirColocarDefensa();
            }
            OnMesaActualizada?.Invoke();
        }

        public void RasparHielo(float incremento)
        {
            if (recipienteActual == TipoRecipiente.Ninguno)
            {
                // Primero se necesita base
                return;
            }

            if (tieneHieloListo) return;

            progresoHielo = Mathf.Clamp01(progresoHielo + incremento);

            if (AudioColonialManager.Instance != null)
            {
                AudioColonialManager.Instance.ReproducirRasparHielo();
            }

            if (progresoHielo >= 1f)
            {
                tieneHieloListo = true;
                Debug.Log("<color=cyan>[Nevería]</color> ¡Hielo de garrafa listo y bien raspado!");
            }

            OnMesaActualizada?.Invoke();
        }

        public void AgregarSabor(SaborNieve sabor)
        {
            if (recipienteActual == TipoRecipiente.Ninguno)
            {
                Debug.LogWarning("[Nevería] Selecciona primero un recipiente (Barquillo o Vaso).");
                return;
            }

            if (!tieneHieloListo)
            {
                Debug.LogWarning("[Nevería] Debes raspar el hielo tradicional en la barrica antes de verter el sabor.");
                return;
            }

            saborActual = sabor;

            if (AudioColonialManager.Instance != null)
            {
                AudioColonialManager.Instance.ReproducirMonedaRecogida();
            }

            Debug.Log($"<color=orange>[Nevería]</color> Sabor añadido: {PedidoNieve.ObtenerNombreSabor(sabor)}");
            OnMesaActualizada?.Invoke();
        }

        public bool EsNieveCompleta()
        {
            return recipienteActual != TipoRecipiente.Ninguno && tieneHieloListo && saborActual != SaborNieve.Ninguno;
        }

        public bool CoincideConPedido(PedidoNieve pedido)
        {
            if (pedido == null) return false;
            return EsNieveCompleta() &&
                   pedido.recipienteDeseado == recipienteActual &&
                   pedido.saborDeseado == saborActual;
        }

        public void LimpiarMesa()
        {
            recipienteActual = TipoRecipiente.Ninguno;
            saborActual = SaborNieve.Ninguno;
            progresoHielo = 0f;
            tieneHieloListo = false;
            OnMesaActualizada?.Invoke();
        }

        public void DesecharPreparacion()
        {
            if (!TieneArticuloEnPreparacion) return;

            LimpiarMesa();
            if (AudioColonialManager.Instance != null)
            {
                AudioColonialManager.Instance.ReproducirErrorPedido();
            }
            Debug.Log("<color=red>[Nevería]</color> Preparación desechada en el cubo de desperdicios.");
        }
    }
}
