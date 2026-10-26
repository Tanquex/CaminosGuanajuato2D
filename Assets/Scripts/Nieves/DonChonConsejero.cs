using System;
using UnityEngine;

namespace CaminosDeGuanajuato.Nieves
{
    /// <summary>
    /// Don Chon (El Nevero Mayor), mentor culinario de Guanajicho en el quiosco de Dolores Hidalgo.
    /// Brinda orientación cultural, reacciona a los pedidos y entrega la Cuchara Dorada Artesanal.
    /// </summary>
    public class DonChonConsejero : MonoBehaviour
    {
        public static DonChonConsejero Instance { get; private set; }

        [Header("Mensaje Actual")]
        [SerializeField] [TextArea(2, 3)] private string mensajeActual = "¡Bienvenido a la barrica, Guanajicho! Elige el barquillo, raspa el hielo y vierte el sabor.";
        [SerializeField] private float duracionMensaje = 5.0f;

        private float temporizadorMensaje;

        public string MensajeActual => mensajeActual;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            DecirFraseInicial();
        }

        private void Update()
        {
            if (temporizadorMensaje > 0f)
            {
                temporizadorMensaje -= Time.deltaTime;
                if (temporizadorMensaje <= 0f)
                {
                    mensajeActual = "¡Atiende al siguiente comensal! Recuerda: [1 o 2] Recipiente, [Espacio] Raspar hielo, [Q/W/E/R/T] Sabor.";
                }
            }
        }

        public void MostrarMensaje(string texto, float duracion = 4.5f)
        {
            mensajeActual = texto;
            temporizadorMensaje = duracion;
        }

        public void DecirFraseInicial()
        {
            MostrarMensaje("Don Chon: «¡Guanajicho! Los visitantes del pueblo buscan probar nuestras nieves tradicionales. ¡Atendamos a 8 comensales para ganar la Cuchara Dorada!»", 7f);
        }

        public void ReaccionarPedidoExitoso(SaborNieve sabor)
        {
            string comentario = "";
            switch (sabor)
            {
                case SaborNieve.Garambullo:
                    comentario = "Don Chon: «¡Magnífico! El garambullo silvestre del semidesierto nunca decepciona.»";
                    break;
                case SaborNieve.Borrachita:
                    comentario = "Don Chon: «¡Ajúa! Esa borrachita tiene el alegre sabor de nuestras ferias virreinales.»";
                    break;
                case SaborNieve.QuesoConFresa:
                    comentario = "Don Chon: «¡Deliciosa combinación! El queso fresco con fresa del Bajío es único.»";
                    break;
                case SaborNieve.Aguacate:
                    comentario = "Don Chon: «¡Atrevido y exquisito! El aguacate en nieve es nuestro orgullo dolorense.»";
                    break;
                case SaborNieve.LimonVirreinal:
                    comentario = "Don Chon: «¡Fresco como el alba en la parroquia! Gran servicio, muchacho.»";
                    break;
                default:
                    comentario = "Don Chon: «¡Bien despachado! Así se atiende a nuestra gente.»";
                    break;
            }
            MostrarMensaje(comentario, 4.0f);
        }

        public void ReaccionarError(string motivo)
        {
            MostrarMensaje($"Don Chon: «¡Cuidado Guanajicho! {motivo}. Pulsa [Borrar] para reiniciar la nieve si te equivocas.»", 4.5f);
        }

        public void ReaccionarClientePerdido()
        {
            MostrarMensaje("Don Chon: «¡Uy! Un comensal se fue decepcionado. ¡No pierdas la concentración, aún podemos lograrlo!»", 4.0f);
        }

        public void ReaccionarVictoria()
        {
            MostrarMensaje("Don Chon: «¡Lo lograste, Guanajicho! Eres digno portador de la Cuchara Dorada Artesanal. ¡El pueblo apoya nuestra causa libertaria!»", 10.0f);
        }
    }
}
