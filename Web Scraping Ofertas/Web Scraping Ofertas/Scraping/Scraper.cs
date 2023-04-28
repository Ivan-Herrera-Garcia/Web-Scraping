using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Web_Scraping_Ofertas.Models;

namespace Web_Scraping_Ofertas.Scraping
{
    public class Scraper
    {
        #region Comentarios
        /*
         *Este proyecto es de caracter educativo y sin fines malisiosos.
         *La clase Articulos cuenta con atributos los cuales hacen referencia a las caracteristicas 
         *de un articulo, de las cuales pueden ser enlace del articulo, precio original, precio con descuento,
         *descripcion y vendedor.
         *
         *El sitio web a utilizar sera mercado libre en el apartado de ofertas, con un total de 20 paginas 
         *hacer raspado, pero para fines de eficiencia se rasparan 2 a 3 paginas segun el desarrollador lo 
         *configure. (Para hacer el cambio)
         */
        #endregion

        #region Variables Globales
        public static bool band = true;
        public static List<Articulos> Datos;
        #endregion

        #region Metodo de acceso al sitio web
        public static async Task<List<Articulos>> DescargarAsync()
        {
            Datos = new List<Articulos>();
            int pag = 1;
            do
            {
                string url = $"https://www.mercadolibre.com.mx/ofertas?container_id=MLM779363-1&page={pag++}";
                await DescargarDatos(url);
                Thread.Sleep(300);
                //Para realizar el cambio debe hacerce en esta parte la modificacion
                //if (pag > N ) N es el numero de paginas a visitar, lo maximo es 20 y lo minimo es 1
                if (pag > 5)
                {
                    break;
                }
            } while (band);
            return Datos;
        }
        #endregion

        #region Metodo de DescargarDatos
        public static async Task<bool> DescargarDatos(string url)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            var response = await client.SendAsync(request);

            if (response.EnsureSuccessStatusCode().IsSuccessStatusCode)
            {
                string htmldoc = await response.Content.ReadAsStringAsync();
                HtmlDocument document = new HtmlDocument();
                document.LoadHtml(htmldoc);
                var main = document.DocumentNode.SelectSingleNode("//div[@class='promotions_boxed-width']");

                #region Iteraciones
                //Se realizaron 4 iteraciones ya que la lista de productos que nos cada iteracion es diferente, en total son 4 tipos
                //de productos diferentes, con numeros de productos diferentes entre si.
                foreach (var catalogo in main.Descendants("li").Where(li => li.Attributes["class"].Value == "promotion-item sup"))
                {
                    //Varibales utilizadas para guardar la informacion que arroga el scraping
                    string? enlace = "";
                    string? precioOriginal = "";
                    string? precioDesc = "";
                    string? porcDesc = "";
                    string? descripcion = "";
                    string? vendedor = "";

                    var enlacehref = catalogo.Descendants("a").Where(a => a.GetClasses().Contains("promotion-item__link-container"));
                    enlace = enlacehref.FirstOrDefault().GetAttributeValue("href", "");


                    if (catalogo.Descendants("span").Where(span => span.GetClasses().Contains("andes-visually-hidden") & Regex.IsMatch(span.InnerText, "^Antes:")).FirstOrDefault() != null)
                    {
                        precioOriginal = catalogo.Descendants("span").Where(span => span.GetClasses().Contains("andes-visually-hidden") & Regex.IsMatch(span.InnerText, "^Antes:")).FirstOrDefault().InnerText.Trim();
                    }

                    if (catalogo.Descendants("span").Where(span => span.GetClasses().Contains("andes-visually-hidden") & Regex.IsMatch(span.InnerText, "^Ahora:")).FirstOrDefault() != null)
                    {
                        precioDesc = catalogo.Descendants("span").Where(span => span.GetClasses().Contains("andes-visually-hidden") & Regex.IsMatch(span.InnerText,"^Ahora:")).FirstOrDefault().InnerText.Trim();
                    }

                    if (catalogo.Descendants("span").Where(span => span.GetClasses().Contains("andes-money-amount__discount")).FirstOrDefault() != null)
                    {
                        porcDesc = catalogo.Descendants("span").Where(span => span.GetClasses().Contains("andes-money-amount__discount")).FirstOrDefault().InnerText.Trim();
                    }

                    if (catalogo.Descendants("p").Where(p => p.GetClasses().Contains("promotion-item__title")).FirstOrDefault() != null)
                    {
                        descripcion = catalogo.Descendants("p").Where(p => p.GetClasses().Contains("promotion-item__title")).FirstOrDefault().InnerText.Trim();
                    }

                    if (catalogo.Descendants("span").Where(span => span.GetClasses().Contains("promotion-item__seller")).FirstOrDefault() != null)
                    {
                        vendedor = catalogo.Descendants("span").Where(span => span.GetClasses().Contains("promotion-item__seller")).FirstOrDefault().InnerText.Trim();
                        vendedor = Regex.Replace(vendedor, "por", "").Trim();    
                    } else
                    {
                        vendedor = "Sin informacion";
                    }

                    Datos.Add(new Articulos
                    {
                        descripcion = descripcion,
                        enlace = enlace,
                        porcDesc = porcDesc,
                        precioDesc = precioDesc,
                        precioOriginal = precioOriginal,
                        vendedor = vendedor

                    });
                }

                foreach (var catalogo in main.Descendants("li").Where(li => li.Attributes["class"].Value == "promotion-item avg"))
                {
                    string? enlace = "";
                    string? precioOriginal = "";
                    string? precioDesc = "";
                    string? porcDesc = "";
                    string? descripcion = "";
                    string? vendedor = "";

                    var enlacehref = catalogo.Descendants("a").Where(a => a.GetClasses().Contains("promotion-item__link-container"));
                    enlace = enlacehref.FirstOrDefault().GetAttributeValue("href", "");

                    if (catalogo.SelectNodes(".//span[@class='andes-visually-hidden']").Where(span => Regex.IsMatch(span.InnerText, "^Antes:")).FirstOrDefault() != null)
                    {
                        precioOriginal = catalogo.Descendants("span").Where(span => span.GetClasses().Contains("andes-visually-hidden") & Regex.IsMatch(span.InnerText, "^Antes:")).FirstOrDefault().InnerText.Trim();
                    }

                    if (catalogo.SelectNodes(".//span[@class='andes-visually-hidden']").FirstOrDefault() != null)
                    {
                        precioDesc = catalogo.Descendants("span").Where(span => span.GetClasses().Contains("andes-visually-hidden") & Regex.IsMatch(span.InnerText, "^Ahora:")).FirstOrDefault().InnerText.Trim();
                    }

                    if (catalogo.Descendants("span").Where(span => span.GetClasses().Contains("andes-money-amount__discount")).FirstOrDefault() != null)
                    {
                        porcDesc = catalogo.Descendants("span").Where(span => span.GetClasses().Contains("andes-money-amount__discount")).FirstOrDefault().InnerText.Trim();
                    }

                    if (catalogo.SelectNodes(".//p[@class='promotion-item__title']").FirstOrDefault() != null)
                    {
                        descripcion = catalogo.Descendants("p").Where(p => p.GetClasses().Contains("promotion-item__title")).FirstOrDefault().InnerText.Trim();
                    }

                    if (catalogo.SelectNodes(".//span[@class='promotion-item__seller']") != null)
                    {
                        vendedor = catalogo.Descendants("span").Where(span => span.GetClasses().Contains("promotion-item__seller")).FirstOrDefault().InnerText.Trim();
                        vendedor = Regex.Replace(vendedor, "por", "").Trim();

                    }
                    else
                    {
                        vendedor = "Sin informacion";
                    }
                    Datos.Add(new Articulos
                    {
                        descripcion = descripcion,
                        enlace = enlace,
                        porcDesc = porcDesc,
                        precioDesc = precioDesc,
                        precioOriginal = precioOriginal,
                        vendedor = vendedor

                    });
                }

                foreach (var catalogo in main.Descendants("li").Where(li => li.Attributes["class"].Value == "promotion-item default"))
                {
                    string? enlace = "";
                    string? precioOriginal = "";
                    string? precioDesc = "";
                    string? porcDesc = "";
                    string? descripcion = "";
                    string? vendedor = "";

                    var enlacehref = catalogo.Descendants("a").Where(a => a.GetClasses().Contains("promotion-item__link-container"));
                    enlace = enlacehref.FirstOrDefault().GetAttributeValue("href", "");

                    if (catalogo.SelectNodes(".//span[@class='andes-visually-hidden']").Where(span => Regex.IsMatch(span.InnerText, "^Antes:")).FirstOrDefault() != null)
                    {
                        precioOriginal = catalogo.Descendants("span").Where(span => span.GetClasses().Contains("andes-visually-hidden") & Regex.IsMatch(span.InnerText, "^Antes:")).FirstOrDefault().InnerText.Trim();
                    }

                    if (catalogo.SelectNodes(".//span[@class='andes-visually-hidden']").FirstOrDefault() != null)
                    {
                        precioDesc = catalogo.Descendants("span").Where(span => span.GetClasses().Contains("andes-visually-hidden") & Regex.IsMatch(span.InnerText, "^Ahora:")).FirstOrDefault().InnerText.Trim();
                    }

                    if (catalogo.SelectNodes(".//span[@class='andes-money-amount__discount']").FirstOrDefault() != null)
                    {
                        porcDesc = catalogo.Descendants("span").Where(span => span.GetClasses().Contains("andes-money-amount__discount")).FirstOrDefault().InnerText.Trim();
                    }

                    if (catalogo.SelectNodes(".//p[@class='promotion-item__title']").FirstOrDefault() != null)
                    {
                        descripcion = catalogo.Descendants("p").Where(p => p.GetClasses().Contains("promotion-item__title")).FirstOrDefault().InnerText.Trim();
                    }

                    if (catalogo.SelectNodes(".//span[@class='promotion-item__seller']") != null)
                    {
                        vendedor = catalogo.Descendants("span").Where(span => span.GetClasses().Contains("promotion-item__seller")).FirstOrDefault().InnerText.Trim();
                        vendedor = Regex.Replace(vendedor, "por", "").Trim();
                    }
                    else
                    {
                        vendedor = "Sin informacion";
                    }
                    Datos.Add(new Articulos
                    {
                        descripcion = descripcion,
                        enlace = enlace,
                        porcDesc = porcDesc,
                        precioDesc = precioDesc,
                        precioOriginal = precioOriginal,
                        vendedor = vendedor
                    });
                }

                foreach (var catalogo in main.Descendants("li").Where(li => li.Attributes["class"].Value == "promotion-item min"))
                {
                    string? enlace = "";
                    string? precioOriginal = "";
                    string? precioDesc = "";
                    string? porcDesc = "";
                    string? descripcion = "";
                    string? vendedor = "";

                    var enlacehref = catalogo.Descendants("a").Where(a => a.GetClasses().Contains("promotion-item__link-container"));
                    enlace = enlacehref.FirstOrDefault().GetAttributeValue("href", "");

                    if (catalogo.SelectNodes(".//span[@class='andes-visually-hidden']").Where(span => Regex.IsMatch(span.InnerText, "^Antes:")).FirstOrDefault() != null)
                    {
                        precioOriginal = catalogo.Descendants("span").Where(span => span.GetClasses().Contains("andes-visually-hidden") & Regex.IsMatch(span.InnerText, "^Antes:")).FirstOrDefault().InnerText.Trim();
                    }

                    if (catalogo.SelectNodes(".//span[@class='andes-visually-hidden']").FirstOrDefault() != null)
                    {
                        precioDesc = catalogo.Descendants("span").Where(span => span.GetClasses().Contains("andes-visually-hidden") & Regex.IsMatch(span.InnerText, "^Ahora:")).FirstOrDefault().InnerText.Trim();
                    }

                    if (catalogo.SelectNodes(".//span[@class='andes-money-amount__discount']").FirstOrDefault() != null)
                    {
                        porcDesc = catalogo.Descendants("span").Where(span => span.GetClasses().Contains("andes-money-amount__discount")).FirstOrDefault().InnerText.Trim();
                    }

                    if (catalogo.SelectNodes(".//p[@class='promotion-item__title']").FirstOrDefault() != null)
                    {
                        descripcion = catalogo.Descendants("p").Where(p => p.GetClasses().Contains("promotion-item__title")).FirstOrDefault().InnerText.Trim();
                    }

                    if (catalogo.SelectNodes(".//span[@class='promotion-item__seller']") != null)
                    {
                        vendedor = catalogo.Descendants("span").Where(span => span.GetClasses().Contains("promotion-item__seller")).FirstOrDefault().InnerText.Trim();
                        vendedor = Regex.Replace(vendedor, "por", "").Trim();
                    }
                    else
                    {
                        vendedor = "Sin informacion";
                    }
                    Datos.Add(new Articulos
                    {
                        descripcion = descripcion,
                        enlace = enlace,
                        porcDesc = porcDesc,
                        precioDesc = precioDesc,
                        precioOriginal = precioOriginal,
                        vendedor = vendedor

                    });
                }
                #endregion

                band = true;
                return band;
            }
            else
            {
                band = false;
                return band;
            }
        }
        #endregion
    }
}
