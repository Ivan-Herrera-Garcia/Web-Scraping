using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web_Scrapper_Divisas.Models
{
    public class Divisas
    {
        /*

       Este pequeño proyecto hace raspado de las divisas del sitio https://www.banamex.com
       en el cual extraemos la informacion de diferentes cambios de monedas diferentes, de
       las cuales se extrae el nombre, cierre, venta, compra, variacion mensual y anual.

       Este proyecto es de caracter demostrativo y solo de aprendizaje.

       */

        #region Atributos
        public string nombre { get; set; }
        public float ValorActualCompra { get; set; }
        public float ValorActualVenta { get; set; }
        public float Cierre { get; set; }
        public float VariacionPorcentualMensualVta { get; set; }
        public float VariacionPorcentualAnualVta { get; set; }
        #endregion

        #region Constructor vacio
        public Divisas()
        {
            nombre = "";
            ValorActualCompra = 0;
            ValorActualVenta = 0;
            Cierre = 0;
            VariacionPorcentualAnualVta = 0;
            VariacionPorcentualAnualVta = 0;
        }
        #endregion

    }
}
