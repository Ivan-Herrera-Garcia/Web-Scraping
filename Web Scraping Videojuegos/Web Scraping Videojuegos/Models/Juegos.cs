using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web_Scraping_Videojuegos.Models
{
    public class Juegos
    {
        #region Atributos de Juegos
        public string? titulo
        {
            get; set;
        }

        public string? region
        {
            get; set;
        }
        public string? precio
        {
            get; set;
        }

        public string? popularidad
        {
            get; set;
        }

        public string? imagen
        {
            get; set;
        }
        #endregion
    }
}
