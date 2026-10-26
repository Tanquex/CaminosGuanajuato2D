using System;
using UnityEngine;

namespace CaminosDeGuanajuato.Nieves
{
    public enum TipoRecipiente
    {
        Ninguno = 0,
        BarquilloCrujiente = 1,
        VasoDeBarro = 2
    }

    public enum SaborNieve
    {
        Ninguno = 0,
        Garambullo = 1,      // Púrpura intenso - Cactácea semidesértica
        Borrachita = 2,      // Ámbar dorado - Miel de agave y biznaga
        QuesoConFresa = 3,   // Rosa crema - Queso fresco y fresa del Bajío
        Aguacate = 4,        // Verde olivo - Pulpa exótica artesanal
        LimonVirreinal = 5   // Verde lima fresco - El clásico refrescante
    }

    [System.Serializable]
    public class PedidoNieve
    {
        public TipoRecipiente recipienteDeseado;
        public SaborNieve saborDeseado;
        public float tiempoPacienciaMax = 18f;
        public float tiempoPacienciaRestante;
        public int valorPuntos = 100;
        public string nombreCliente;

        public PedidoNieve(TipoRecipiente rec, SaborNieve sabor, string cliente = "Turista", float paciencia = 18f)
        {
            recipienteDeseado = rec;
            saborDeseado = sabor;
            nombreCliente = cliente;
            tiempoPacienciaMax = paciencia;
            tiempoPacienciaRestante = paciencia;
            valorPuntos = 100;
        }

        public float PorcentajePaciencia => Mathf.Clamp01(tiempoPacienciaRestante / tiempoPacienciaMax);
        public bool EstaExpirado => tiempoPacienciaRestante <= 0f;

        public static string ObtenerNombreSabor(SaborNieve sabor)
        {
            switch (sabor)
            {
                case SaborNieve.Garambullo: return "Garambullo Silvestre";
                case SaborNieve.Borrachita: return "Borrachita Tradicional";
                case SaborNieve.QuesoConFresa: return "Queso con Fresa del Bajío";
                case SaborNieve.Aguacate: return "Aguacate Exótico";
                case SaborNieve.LimonVirreinal: return "Limón Virreinal";
                default: return "Nieve Tradicional";
            }
        }

        public static Color ObtenerColorSabor(SaborNieve sabor)
        {
            switch (sabor)
            {
                case SaborNieve.Garambullo: return new Color(0.55f, 0.15f, 0.85f, 1f); // Púrpura garambullo
                case SaborNieve.Borrachita: return new Color(0.92f, 0.62f, 0.12f, 1f); // Ámbar dorado
                case SaborNieve.QuesoConFresa: return new Color(0.95f, 0.45f, 0.55f, 1f); // Rosa crema
                case SaborNieve.Aguacate: return new Color(0.42f, 0.65f, 0.15f, 1f); // Verde olivo
                case SaborNieve.LimonVirreinal: return new Color(0.68f, 0.92f, 0.22f, 1f); // Verde lima
                default: return Color.white;
            }
        }

        public static string ObtenerNombreRecipiente(TipoRecipiente recipiente)
        {
            switch (recipiente)
            {
                case TipoRecipiente.BarquilloCrujiente: return "Barquillo de Trigo";
                case TipoRecipiente.VasoDeBarro: return "Vaso de Barro Dolorense";
                default: return "Sin recipiente";
            }
        }
    }
}
