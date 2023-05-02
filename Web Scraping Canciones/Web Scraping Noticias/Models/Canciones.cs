using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web_Scraping_Noticias.Models
{
    public class Canciones
    {
        /*
         * Este proyecto hace uso de un sitio web que nos muestra un top de 100 canciones mas populares, dependiendo del dia y en este caso 
         * el ultimo registro es del 16 de abril. En el cual se mostrará el titulo de la cancion, autor, imagen, puntuacion y enlace a la misma.
         * Los datos recabados son de caracter academico, sin algun otro fin no academico.
         * 
        */

        #region Atributos

        public string titulo
        {
            get; set;
        }

        public string autor
        {
            get; set;
        }

        public string imagen
        {
            get; set;
        }

        public string puntuacion
        {
            get; set;
        }

        public string enlace
        {
            get; set;
        }
        #endregion
    }
}
