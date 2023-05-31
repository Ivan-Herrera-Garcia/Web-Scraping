using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web_Scraper_Cartas_Coleccionables.Models
{
    public class Cartas
    {
        #region Atributos
        public string nombre { get; set; }
        public string imagen { get; set; }
        public string estado { get; set; }
        public string enlace { get; set; }
        public string envio { get; set; }
        public string precio { get; set; }
        public string precio_anterior { get; set; }
        public string info_adic { get; set; }
        public List<string> ListCarac { get; set; }
        #endregion

        #region Constructores
        public Cartas ()
        {
            nombre = "";
            imagen = "";
            envio = "";
            estado = "";
            enlace = "";
            precio = "";
            precio = "";
            precio_anterior = "";
            info_adic = "";
            ListCarac = new List<string>();
        }
        #endregion
    }
}
