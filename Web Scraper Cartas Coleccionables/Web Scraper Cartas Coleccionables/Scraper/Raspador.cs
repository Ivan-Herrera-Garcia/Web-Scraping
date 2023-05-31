using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Web_Scraper_Cartas_Coleccionables.Models;

namespace Web_Scraper_Cartas_Coleccionables.Scraper
{
    public class Raspador
    {
        #region Variables Globales
        public static List<Cartas> datos;
        public static bool band = true;
        public static List<string> lsCarac;
        #endregion

        #region Metodo principal
        public static async Task<List<Cartas>> main ()
        {
            datos = new List<Cartas>();
            lsCarac = new List<string>();
            int pag = 1;
            do
            {
                string url = $"https://www.ebay.com/e/latam/latm-ccg-individual-cards?rt=nc&_pgn={pag++}";
                await GetPage(url);
                if (pag > 5) //Con el contador de pag se controla el numero de paginas a visitar, por fines practicos solo se visitaron 5.
                {
                    break;
                }
            } while (band);
            return datos;
        }
        #endregion

        #region Obtener los datos de cada producto
        public static async Task<bool> GetPage(string url)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Cookie", "ak_bmsc=22DC754A3407CDDB6842B3C7E4DF4452~000000000000000000000000000000~YAAQVdX3vZBuAG6IAQAAtQKAchNa05tPrzIclDmQLt2D7wElIbFXIuKxSe/3ifyWkspOAeEybiCDFH5QnfP4o07e0ZAmXATxArK1ZD7gq3L5JADEbzhxGgC9xPlrTlmmfFoFo8fgQ0JlQITFrsy8rr1XQmhFilPd+R/xLAZAScHs8/gvlJqIkzryL+gwRtE1JqE30h81o53s6K66f5fyLS6y6OXPhpS6avbyIvgxbQpB9Ov1h/hlCblXLGCy/zNHdkpZswwK/gt9A4g2TFfh2wmSxcT/2YGmxt/Kp6rmjSKaw/Qb3fj3+j+LT+2+CzYvw0H0YyW6gZFKAXszt8acltAGbh6b5GFPYIe7sRcc/1nSwzVYIw7KB3hy; dp1=bbl/MX6839d38b^; ebay=%5Esbf%3D%23000000%5E; nonsession=BAQAAAYhF58hiAAaAADMABWZYoAsyNzEzMADKACBoOdOLNzI4MDAxZjkxODgwYTc3ZDM2MTViMGE5ZmZmZTg5MmYAywABZHdzkzHQBAk8f31+DkVFMVEbLQ+Kt4e0Dg**; s=CgAD4ACBkeL4LNzI4MDAxZjkxODgwYTc3ZDM2MTViMGE5ZmZmZTg5MmYY8mb2");
            var response = await client.SendAsync(request);
            
            if (response.EnsureSuccessStatusCode().IsSuccessStatusCode)
            {
                string htmldoc = await response.Content.ReadAsStringAsync();
                HtmlDocument document = new HtmlDocument();
                document.LoadHtml(htmldoc);

                var main = document.DocumentNode.Descendants("section").Where(section => section.GetClasses().Contains("b-listing")).FirstOrDefault();

                foreach (var lista in main.Descendants("li").Where(li => li.GetClasses().Contains("s-item--large")))
                    {
                    Cartas data = new Cartas();

                    //Obtener el enlace de la carta en ebay
                    var enlace = lista.Descendants("a").Where(a => a.GetClasses().Contains("s-item__link")).FirstOrDefault();
                    data.enlace = enlace.GetAttributeValue("href", "");

                    //Obtener el nombre aplicando normalizacion con WebUtility.HtmlDecode
                    var nombre = lista.Descendants("h3").Where(h3 => h3.GetClasses().Contains("s-item__title")).FirstOrDefault();
                    data.nombre = WebUtility.HtmlDecode(nombre.InnerText.Trim());

                    if (data.nombre.ToLower().Contains("como nueva") || data.nombre.ToLower().Contains("casi nueva") || data.nombre.ToLower().Contains("casi nuevo")) {
                        data.estado = "Semi-nueva";
                        data.nombre = Regex.Replace(data.nombre.ToLower(), "como nueva", string.Empty).ToUpper();
                        data.nombre = Regex.Replace(data.nombre.ToLower(), "como nuevo", string.Empty).ToUpper();
                        data.nombre = Regex.Replace(data.nombre.ToLower(), "casi nueva", string.Empty).ToUpper();
                    }
                    else
                    {
                        data.estado = "Sin informacion";
                    }

                    //Se obtiene la imagen
                    var img = lista.Descendants("img").Where(img => img.GetClasses().Contains("s-item__image-img")).FirstOrDefault();
                    data.imagen = img.GetAttributeValue("src", "");                         //s-item__image-img
                    if (data.imagen.Contains(".gif")) {
                        data.imagen = img.GetAttributeValue("data-src", "");
                    }

                    //Se obtiene el precio
                    var precio = lista.Descendants("span").Where(span => span.GetClasses().Contains("s-item__price")).FirstOrDefault();
                    data.precio = WebUtility.HtmlDecode(precio.InnerText.Trim());

                    //Si tiene descuento, se muestra el precio anterior
                    var precio_anterior = lista.Descendants("span").Where(span => span.GetClasses().Contains("STRIKETHROUGH"));
                    if (lista.Descendants("span").Where(span => span.GetClasses().Contains("STRIKETHROUGH")).Any())
                    {
                        data.precio_anterior = WebUtility.HtmlDecode(precio_anterior.FirstOrDefault().InnerText.Trim());
                    } else
                    {
                        data.precio_anterior = "Sin descuento";
                    }

                    //Se obtiene el costo del envio
                    var envio = lista.Descendants("span").Where(span => span.GetClasses().Contains("s-item__logisticsCost")).FirstOrDefault();
                    data.envio = WebUtility.HtmlDecode(envio.InnerText.Trim());

                    //Se obtiene informacion adicional
                    var info = lista.Descendants("span").Where(span => span.GetClasses().Contains("s-item__itemHotness")).FirstOrDefault();
                    if (info != null)
                    {
                        data.info_adic = WebUtility.HtmlDecode(info.InnerText.Trim());
                    }

                    await GetInfo(data.enlace);

                    lsCarac = lsCarac.Distinct().ToList();
                    data.ListCarac = lsCarac.ToList();

                    datos.Add(data);

                    lsCarac.Clear();
                }
                band = true;
            } else
            {
                band = false;
            }
            return band;
        }
        #endregion

        #region Obtener informacion de un producto especifico
        public static async Task<bool> GetInfo(string enlace)
        {
            //List<string> carac = new List<string>();
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, enlace);
            request.Headers.Add("Cookie", "ak_bmsc=22DC754A3407CDDB6842B3C7E4DF4452~000000000000000000000000000000~YAAQVdX3vZBuAG6IAQAAtQKAchNa05tPrzIclDmQLt2D7wElIbFXIuKxSe/3ifyWkspOAeEybiCDFH5QnfP4o07e0ZAmXATxArK1ZD7gq3L5JADEbzhxGgC9xPlrTlmmfFoFo8fgQ0JlQITFrsy8rr1XQmhFilPd+R/xLAZAScHs8/gvlJqIkzryL+gwRtE1JqE30h81o53s6K66f5fyLS6y6OXPhpS6avbyIvgxbQpB9Ov1h/hlCblXLGCy/zNHdkpZswwK/gt9A4g2TFfh2wmSxcT/2YGmxt/Kp6rmjSKaw/Qb3fj3+j+LT+2+CzYvw0H0YyW6gZFKAXszt8acltAGbh6b5GFPYIe7sRcc/1nSwzVYIw7KB3hy; dp1=bbl/MX6839d38b^; ebay=%5Esbf%3D%23000000%5E; nonsession=BAQAAAYhF58hiAAaAADMABWZYoAsyNzEzMADKACBoOdOLNzI4MDAxZjkxODgwYTc3ZDM2MTViMGE5ZmZmZTg5MmYAywABZHdzkzHQBAk8f31+DkVFMVEbLQ+Kt4e0Dg**; s=CgAD4ACBkeL4LNzI4MDAxZjkxODgwYTc3ZDM2MTViMGE5ZmZmZTg5MmYY8mb2");
            var response = await client.SendAsync(request);

            if (response.EnsureSuccessStatusCode().IsSuccessStatusCode)
            {
                string htmldoc = await response.Content.ReadAsStringAsync();
                HtmlDocument document = new HtmlDocument();
                document.LoadHtml(htmldoc);

                var main = document.DocumentNode.Descendants("div").Where(div => div.GetClasses().Contains("ux-layout-section-module-evo")).FirstOrDefault();

                foreach(var listcarac in main.Descendants("div").Where(div => div.GetClasses().Contains("ux-layout-section-evo__col")))
                {
                    // carac.Add(WebUtility.HtmlDecode(listcarac.InnerText.Trim()));
                    var label = listcarac.Descendants("div").Where(div => div.GetClasses().Contains("ux-labels-values__labels")).FirstOrDefault();
                    var value = listcarac.Descendants("div").Where(div => div.GetClasses().Contains("ux-labels-values__values")).FirstOrDefault();

                    if (label != null)
                    {
                        lsCarac.Add(WebUtility.HtmlDecode(label.InnerText.Trim() + ": " + value.InnerText.Trim()+"\n"));
                    }
                }
            }
            return true;
        }
        #endregion
    }
}
