using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Web_Scraping_Lamudi.Scraper
{
    public class Extraccion
    {
        /*
         * La clas Extraccion contiene metodos para la extraccion del contenido html de las paginas a visitar, 
         * puede extraer tambien el contenido de archivos json siempre y cuando se tengan las cookies de los 
         * respectvos sitios.
         */

        #region Metodo por NuGet 
        public static async Task<HtmlDocument> GetHtmlForWeb(string url)
        {
            /*
            Nota:
            Este metodo hace uso del paquete NuGet HtmlAgilityPack y solo se requiere la url para su funcionamiento.
            Para recibir y manipular el dato de regreso se debe de declarar HtmlDocument en vez de string el que recibe el valor del metodo, ejemplo:

            HtmlDocument html = await GetHtmlForWeb(url);

             */
            HtmlWeb web = new HtmlWeb();
            HtmlDocument html = await web.LoadFromWebAsync(url);

            if (!html.ToString().Equals("") || !html.ToString().Equals(null))
            {
                return html;
            } else
            {
                return null;
            }
           
        }
        #endregion

        #region Metodo por Postman
        public static async Task<string> GetHtmlForPostman(string url)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Cookie", "__Secure-3PSID=XAiC3G6brc05R_fzBZVVjiFfqgEz-m-oi5pCJJ94Besktlr6T1UQGTB2Bdr_JzXLNf546A.; __Secure-3PAPISID=RG9acChLT5a-DpGT/AzkUVcCGoR7BQ9TWi; NID=511=QD4B4U__98sR3KRCJGUU3dthC9a_i9i2S-IagUNSXpfV-07l_XDfryIMAGLiHBcfnEmkysRrDyLo4oL3jHlJw5NiXlucRHPIn2RzTempLNBWRXc0ErYxJRsIPfQfNJIz62OktQxqRQqRdN2idYFBK0sk7OcW4tB9VRuggI8VEcpoHkBHSw-ySfvuZHL62_MQhqnIcOLx02FWNuRHrXrytyE17I-lL0UxUh840LE1EE9zHxlvXdHSVTERxOyqCNhaN4zYj1VItAbKAHLSqpWnQkKtuQpE6LGoFYvns8c4IpqEx1tO_TcD92gQkHl_l2g3aWdVIw6kMBoLQiqpLbSaT1JO3ShVZStG7BSX; __Secure-3PSIDTS=sidts-CjEBLFra0lSeOW_ENK4l_I23vaHdKGDtvfC09vA_hbUCdZGto-OiZK0KPSKVNZnUSD1TEAA; __Secure-3PSIDCC=AP8dLtwu4yV2_ccCGJVzXuYC3Dn-UbbtBU_NhtM5r5lJQ_uImERXOtS8AXE4WKQz11jLFcWCDxo; Origin=1; _lamudi_user_id=f850366c-64cc-4786-ad63-d1a4bbff6dc7; page=1; t_or=1; t_pvid=910eb4e0-ca66-439e-9e45-161c50203ed0");
            request.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7");
            var response = await client.SendAsync(request);
            if (response.EnsureSuccessStatusCode().IsSuccessStatusCode)
            {
                string htmldoc = await response.Content.ReadAsStringAsync();

                /*
                Nota: 
                Para hacer uso del string htmldoc se debe de implementar HtmlDocument y cargar el string en el objeto de la clase antes mencionada, codigo abajo:

                HtmlDocument document = new HtmlDocument();
                document.LoadHtml(htmldoc);

                */
                return htmldoc;
            } else
            {
                return "No se logro acceder mediante Postman";
            }
        }
        #endregion

        #region Metodo por HttpWebRequest
        public static async Task<string> GetArchivoHTML(string url) { 
            /*
            Nota:
            Cambiar la Cookie por la necesaria para acceder al sitio web, en caso de no poder acceder aun con el cambio hecho, utilice otro medoto de extraccion.
            */
           string validacion = @"^(http|https)://([\w-]+\.)+[\w-]+(/[\w-./?%&=]*)?$";
           Regex regex = new Regex(validacion);

           if (regex.IsMatch(url))
            {

                HttpWebRequest Request = (HttpWebRequest)WebRequest.Create(url);
                Request.Method = "GET";

                Request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/112.0.0.0 Safari/537.36 OPR/98.0.0.0";
                Request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
                Request.Headers.Add("Accept-Language", "en-US,en;q=0.5");
                Request.Headers.Add("Cookie", "__Secure-3PSID=XAiC3G6brc05R_fzBZVVjiFfqgEz-m-oi5pCJJ94Besktlr6T1UQGTB2Bdr_JzXLNf546A.; __Secure-3PAPISID=RG9acChLT5a-DpGT/AzkUVcCGoR7BQ9TWi; NID=511=QD4B4U__98sR3KRCJGUU3dthC9a_i9i2S-IagUNSXpfV-07l_XDfryIMAGLiHBcfnEmkysRrDyLo4oL3jHlJw5NiXlucRHPIn2RzTempLNBWRXc0ErYxJRsIPfQfNJIz62OktQxqRQqRdN2idYFBK0sk7OcW4tB9VRuggI8VEcpoHkBHSw-ySfvuZHL62_MQhqnIcOLx02FWNuRHrXrytyE17I-lL0UxUh840LE1EE9zHxlvXdHSVTERxOyqCNhaN4zYj1VItAbKAHLSqpWnQkKtuQpE6LGoFYvns8c4IpqEx1tO_TcD92gQkHl_l2g3aWdVIw6kMBoLQiqpLbSaT1JO3ShVZStG7BSX; __Secure-3PSIDTS=sidts-CjEBLFra0lSeOW_ENK4l_I23vaHdKGDtvfC09vA_hbUCdZGto-OiZK0KPSKVNZnUSD1TEAA; __Secure-3PSIDCC=AP8dLtwu4yV2_ccCGJVzXuYC3Dn-UbbtBU_NhtM5r5lJQ_uImERXOtS8AXE4WKQz11jLFcWCDxo; Origin=1; _lamudi_user_id=f850366c-64cc-4786-ad63-d1a4bbff6dc7; page=1; t_or=1; t_pvid=910eb4e0-ca66-439e-9e45-161c50203ed0");

                HttpWebResponse Response = (HttpWebResponse)await Request.GetResponseAsync();

                using var streamReader = new StreamReader(Response.GetResponseStream());
                return await streamReader.ReadToEndAsync();
            } else
            {
                return "No se logro acceder mediante HtmlWebRequest";
            }
        }
        #endregion
    }
}
