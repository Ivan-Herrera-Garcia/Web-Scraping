using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Proyecto_Web_Scraping_Cupones.Models
{
    public class Cupones
    {
        #region Atributos
        public string? precio
        {
            get; set;
        }
        public string? titulo
        {
            get; set;
        }

        public string? autor
        {
            get; set;
        }

        public float? puntuacion
        {
            get; set;
        }

        public string? idioma
        {
            get; set;
        }

        public string? enlace
        {
            get; set;
        }
        #endregion
    }
}
