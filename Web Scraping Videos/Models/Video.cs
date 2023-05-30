using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web_Scraping_Videos.Models
{
    public class Video
    {

        /*
         * Este proyecto hace uso de la pagina de videos de CNN
         * en la cual obtiene el nombre, url, descripcion, fecha 
         * y autor. Todo mediante el uso de los paquetes NuGet
         * HtmlAgilityPack.
         * 
         * Este proyecto es de caracter educativo y sin ninguna
         * intencion de perjudicar el sitio web.
        */

        #region Atributos
        public string nombre { get; set; }
        public string url { get; set; }
        public string desc { get; set; }
        public string autor { get; set; }
        public string fecha { get; set; }
        #endregion

        #region Constructor
        public Video()
        {
            nombre = "";
            url = "";
            desc = "";
            autor = "";
            fecha = "";
        }
        #endregion

    }
}
