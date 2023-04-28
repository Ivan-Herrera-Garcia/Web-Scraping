using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web_Scraping_Ofertas.Models
{
    public class Articulos
    {
        #region Comentarios
        /*
         *Este proyecto es de caracter educativo y sin fines malisiosos.
         *La clase Articulos cuenta con atributos los cuales hacen referencia a las caracteristicas 
         *de un articulo, de las cuales pueden ser enlace del articulo, precio original, precio con descuento,
         *descripcion y vendedor.
         *
         *El sitio web a utilizar sera mercado libre en el apartado de ofertas, con un total de 20 paginas 
         *hacer raspado, pero para fines de eficiencia se rasparan 2 a 3 paginas segun el desarrollador lo 
         *configure.
         */
        #endregion

        #region Atributos
        public string? enlace
        {
            get; set;
        }

        public string? precioOriginal
        {
            get; set;
        }
        
        public string? precioDesc
        {
            get; set;
        }

        public string? porcDesc
        {
            get; set;
        }

        public string? descripcion
        {
            get; set;
        }

        public string? vendedor
        {
            get; set;
        }
        #endregion
    }
}
