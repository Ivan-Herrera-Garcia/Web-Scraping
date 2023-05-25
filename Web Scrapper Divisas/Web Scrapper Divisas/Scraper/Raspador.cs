using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Web_Scrapper_Divisas.Models;

namespace Web_Scrapper_Divisas.Scraper
{
    public class Raspador
    {
        /*

        Este pequeño proyecto hace raspado de las divisas del sitio https://www.banamex.com
        en el cual extraemos la informacion de diferentes cambios de monedas diferentes, de
        las cuales se extrae el nombre, cierre, venta, compra, variacion mensual y anual.

        Este proyecto es de caracter demostrativo y solo de aprendizaje.

        */

        #region Variables Globales
        public static List<Divisas> lista = new List<Divisas>();
        #endregion

        #region Metodo principal que extrae toda la informacion
        public static async Task<List<Divisas>> GetInformacion()
        {
            //Variables locales -------------------------------------------------------------------------------------

            lista = new List<Divisas>();
            List<string> json = new List<string>();

            //Variables locales inicializadas y/o con valores
            string url = "https://finanzasenlinea.infosel.com/banamex/WSFeedJSON/service.asmx/DivisasLast?callback=";
            string jsn = await GetJson(url);
            JArray arreglo = JArray.Parse(jsn);

            //Fin variables locales ---------------------------------------------------------------------------------
           
            foreach (var obj in arreglo)
            {
                //Se crea e inicializa un objeto tipo Divisas llamado Moneda
                Divisas Moneda = new Divisas();

                //Se crea una variable tipo cadena con un valor vacio
                string moneda = "";

                //Se hace uso de un switch para asignar un nombre a las divisas, en caso de no aparecer dentro de
                //cada caso solo se agrega tal cual se extrae el nombre.
                switch(obj["cveInstrumento"].ToString())
                {
                    case "AUDMXN":
                        moneda = "Dolar (Australia)";
                        break;
                    case "BRLMXN":
                        moneda = "Real (Brasil)";
                        break;
                    case "CADMXN": 
                        moneda = "Dolar (Canada)";
                        break;
                    case "CHFMXN":
                        moneda = "Franco (Suiza)";
                        break;
                    case "COPMXN":
                        moneda = "Peso (Colombia)";
                        break;
                    case "EURMXN":
                        moneda = "Euro Interbancario";
                        break;
                    case "EURUS":
                        moneda = "Euro/Dolar";
                        break;
                    case "GBPMXN":
                        moneda = "Libra (Gran Bretaña)";
                        break;
                    case "JPYMXN":
                        moneda = "Yen (Japon)";
                        break;
                    default:
                        moneda = obj["cveInstrumento"].ToString();
                        break;
                }

                //Se colocan los valores correspondientes a cada atributo del objeto Moneda, nombre, venta, compra, variacion anual y mensual.
                //Ademas de que se muestra informacion mas reducida haciendo uso de Math.Round
                Moneda.nombre = moneda;
                Moneda.Cierre = (float)Math.Round(float.Parse(obj["Cierre"].ToString()), 4);
                Moneda.ValorActualCompra = (float)Math.Round(float.Parse(obj["ValorActualCompra"].ToString()), 4);
                Moneda.ValorActualVenta = (float)Math.Round(float.Parse(obj["ValorActualVenta"].ToString()), 4);
                Moneda.VariacionPorcentualAnualVta = (float)Math.Round(float.Parse(obj["VariacionPorcentualAnualVta"].ToString()), 4);
                Moneda.VariacionPorcentualMensualVta = (float)Math.Round(float.Parse(obj["VariacionPorcentualMensualVta"].ToString()), 4);

                //Se agrega Moneda al objeto lista tipo Divisa llamado lista
                lista.Add(Moneda);
            }


            return lista;
        }
        #endregion

        #region Metodo de recoleccion del Json
        public static async Task<string> GetJson(string url)
        {
            //Crear una solicitud HTTP GET a la URL especifica
            HttpWebRequest Request = (HttpWebRequest)WebRequest.Create(url);
            Request.Method = "GET";

            //Configurar los encabezados de solicitud
            Request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/89.0.4389.82 Safari/537.36";
            Request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            Request.Headers.Add("Accept-Language", "en-US,en;q=0.5");

            //Enviar la solicitud y obtener la respuesta
            HttpWebResponse Response = (HttpWebResponse)await Request.GetResponseAsync();

            //Leer la respuesta como una cadena de texto
            using var streamReader = new StreamReader(Response.GetResponseStream());
            return await streamReader.ReadToEndAsync();
        }
        #endregion
    }
}
