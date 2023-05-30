using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web_Scraper_Telefonos.Models
{
    public class Smartphone
    {
        /*
         *Este proyecto hace uso de las ofertas en smartphones del sitio web
         *www.coppel.com de los cuales se obtendra el nombre, enlace de la 
         *imagen, enlace del producto, pagos del smartphone semanales, precio 
         *con descuento, precio sin descuento o precio normal del producto.
         *
         *El proyecto es de caracter educativo, sin ninguna mala intencion.
        */

        #region Atributos
        public string nombre { get; set; }
        public string precio_descuento { get; set; }
        public string precio_normal { get; set; }
        public string pagos { get; set; }
        public string url { get; set; }
        public string img { get; set; }
        public List<string?> LstCarac { get; set; }
        #endregion

        #region Constructor Vacio
        public Smartphone()
        {
            nombre = "";
            precio_descuento = "";
            precio_normal = "";
            pagos = "";
            url = "";
            img = "";
            LstCarac = new List<string>();
        }
        #endregion

    }
}
