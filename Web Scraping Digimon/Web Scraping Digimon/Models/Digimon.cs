using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web_Scraping_Digimon.Models
{
    public class Digimon
    {
        /*
         * Este proyecto es de caracter demostrativo acerca del web scraping.
         * En este proyecto se hara un raspado del sitio web https://digimon.fandom.com
         * En el cual se obtendran todos los digimons en el sitio y sus caractaristicas
         * como nombre, ataques, tipo, nivel, etc. 
         */


        #region Atributos
        public string nombre { get; set; }
        public string url { get; set; }
        public string descripcion { get; set; }
        public string imagen { get; set; }
        public List<string> lstCaracteristicas { get; set; }
        public List<string> lsAtaques { get; set; }
        #endregion

        #region Constructor
        public Digimon()
        {
            nombre = "";
            url = "";
            lstCaracteristicas = new List<string>();
            lsAtaques = new List<string>();
        }
        #endregion
    }
}
