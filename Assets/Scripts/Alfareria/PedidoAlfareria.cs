using System;
using UnityEngine;

namespace CaminosDeGuanajuato.Alfareria
{
    /// <summary>
    /// Formas tradicionales de barro moldeadas en los talleres de San Felipe ("La Francia Chiquita").
    /// </summary>
    public enum TipoFormaBarro
    {
        Ninguna = 0,
        JarroChocolatero = 1, // Jarro panzón tradicional para chocolate y atole
        CazuelaMoletera = 2,  // Cazuela ancha de dos orejas para guisos de hacienda
        CantaroCaminoReal = 3 // Cántaro alto para conservar agua fresca en las caravanas
    }

    /// <summary>
    /// Esmaltes y decorados tradicionales de la alfarería de San Felipe Torres Mochas.
    /// </summary>
    public enum TipoEsmalteBarro
    {
        Ninguno = 0,
        TerracotaBrunida = 1, // Barro rojo bruñido a mano con grecas crema
        VerdeVidriado = 2,    // El famoso vidriado verde esmeralda tradicional de San Felipe
        AzulMayolica = 3,     // Esmalte azul cobalto sobre blanco estilo mayólica
        OroDeGala = 4         // Decorado de gala oscuro con filigrana dorada de la Francia Chiquita
    }

    /// <summary>
    /// Estado de cocción de la pieza dentro del horno de leña artesanal.
    /// </summary>
    public enum EstadoCoccionHorno
    {
        FueraDelHorno = 0,
        Calentando = 1,       // Aún tibia (0% a 44%)
        PuntoOptimo = 2,      // Cocción perfecta y brillante (45% a 90%)
        Sobrecalentado = 3    // Pasada de fuego / ahumada (> 90%)
    }

    /// <summary>
    /// Datos de una orden de alfarería solicitada por un vecino o viajero en San Felipe.
    /// </summary>
    [Serializable]
    public class PedidoAlfareria
    {
        public TipoFormaBarro formaRequerida;
        public TipoEsmalteBarro esmalteRequerido;
        public string nombreCliente;
        public string rolCliente;
        public string fraseHistorica;
        public int recompensaBase;

        public PedidoAlfareria(
            TipoFormaBarro forma,
            TipoEsmalteBarro esmalte,
            string cliente,
            string rol,
            string frase,
            int puntos = 180)
        {
            formaRequerida = forma;
            esmalteRequerido = esmalte;
            nombreCliente = cliente;
            rolCliente = rol;
            fraseHistorica = frase;
            recompensaBase = puntos;
        }

        public static string ObtenerNombreForma(TipoFormaBarro forma)
        {
            switch (forma)
            {
                case TipoFormaBarro.JarroChocolatero: return "Jarro Chocolatero";
                case TipoFormaBarro.CazuelaMoletera: return "Cazuela Moletera";
                case TipoFormaBarro.CantaroCaminoReal: return "Cántaro Real";
                default: return "Sin forma";
            }
        }

        public static string ObtenerNombreEsmalte(TipoEsmalteBarro esmalte)
        {
            switch (esmalte)
            {
                case TipoEsmalteBarro.TerracotaBrunida: return "Terracota Bruñida";
                case TipoEsmalteBarro.VerdeVidriado: return "Verde Vidriado";
                case TipoEsmalteBarro.AzulMayolica: return "Azul Mayólica";
                case TipoEsmalteBarro.OroDeGala: return "Oro de Gala";
                default: return "Sin esmalte";
            }
        }

        public static Color ObtenerColorEsmalte(TipoEsmalteBarro esmalte)
        {
            switch (esmalte)
            {
                case TipoEsmalteBarro.TerracotaBrunida: return new Color(0.86f, 0.42f, 0.22f, 1f);
                case TipoEsmalteBarro.VerdeVidriado: return new Color(0.22f, 0.78f, 0.42f, 1f);
                case TipoEsmalteBarro.AzulMayolica: return new Color(0.26f, 0.58f, 0.96f, 1f);
                case TipoEsmalteBarro.OroDeGala: return new Color(0.96f, 0.78f, 0.22f, 1f);
                default: return new Color(0.68f, 0.52f, 0.38f, 1f);
            }
        }

        public static string ObtenerHexEsmalte(TipoEsmalteBarro esmalte)
        {
            switch (esmalte)
            {
                case TipoEsmalteBarro.TerracotaBrunida: return "#ff9966";
                case TipoEsmalteBarro.VerdeVidriado: return "#55ee88";
                case TipoEsmalteBarro.AzulMayolica: return "#66bbff";
                case TipoEsmalteBarro.OroDeGala: return "#ffe055";
                default: return "#d8c29d";
            }
        }
    }
}

