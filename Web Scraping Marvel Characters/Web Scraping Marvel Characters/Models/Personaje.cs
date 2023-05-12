using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web_Scraping_Marvel_Characters.Models
{
    public class Personaje
    {
        /*
         *  El proyecto hace uso de los datos de los personajes de marvel
         *  de los cuales se haran web scraping donde se recolectaran los
         *  siguientes datos: nombre, enlace de la pagina del personaje, 
         *  lista de habilidades, descripcion y lista de caracteristicas 
         *  del personaje. 
         *  
         *  Este proyecto es con fines de aprendizaje y sin intenciones
         *  de perjudicar al sitio web.
        */

        #region Constructor
        public Personaje()
        {
            nombre = "";
            url = "";
            descripcion = "";
            caracteristicas = new List<string>();
            habilidades = new List<string>();
        }
        #endregion

        #region Atributos
        public string nombre { get; set; }
        public string url { get; set; }
        public List<string> habilidades { get; set; }
        public List<string> caracteristicas { get; set; }
        public string descripcion { get; set; }
        #endregion

    }
}
