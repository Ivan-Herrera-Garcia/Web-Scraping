using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Web_Scraping_Libros.Models;

namespace Web_Scraping_Libros.Scraping
{
    public class Scraper
    {
        #region Variables Globales
        public static List<Libros> Datos;
        public static bool band = true;
        #endregion



        #region Metodos 

        #region Metodo para accesar a sitio web
        public static async Task<List<Libros>> DescargarDatos()
        {
            //Variables de control y de recoleccion de datos
            Datos = new List<Libros>();
            int pag = 1;

            do
            {
                string url = $"https://www.gandhi.com.mx/libros/novela-grafica?p={pag++}"; //Sitio web a ingresar
                await DescargarInfo(url);
                Thread.Sleep(300);
                if (pag > 2) //Control para finalizar la recoleccion de datos del sitio
                {
                    break;
                }
            } while (band);
            return Datos;
        }
        #endregion

        #region Metodo para descargar informacion de sitio web
        public static async Task<bool> DescargarInfo(string url)
        {
            //Variables y funciones proveidas por Postman para ingresar al sitio web, en algunos casos se requiere la cookie y en este sitio es necesaria
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Cookie", "_gcl_au=1.1.178827755.1682706061; _tt_enable_cookie=1; _ttp=8s8HhLrgXi2V75ej9gpgS5kQLei; _fbp=fb.2.1682706061627.996919957; form_key=PvTxoa4JQaMKf9VU; _pin_unauth=dWlkPVl6SXhNalkzT1RZdE9UQTRNUzAwTURFMkxUbGlZemd0WlRjek1XUTNOV05qT1dKbQ; __zlcmid=1FblmUtoUZSOyAO; PHPSESSID=0241f72d6d2b96ac74c1d366e32bd687; form_key=PvTxoa4JQaMKf9VU; private_content_version=43ccb4412b3c75860f8c88e2b3f9ccca; AWSALB=s40DLdRY625fp3kLnqh4kjur7+hNXIBdYomrcbHPopjm03vXS/YKrZIfyzC25lhXAKfInlspr5BQGVijWuImSwuv2Pus0O33WOxl6WmQ/EGUmiweCvsz8qldFOKO; AWSALBCORS=s40DLdRY625fp3kLnqh4kjur7+hNXIBdYomrcbHPopjm03vXS/YKrZIfyzC25lhXAKfInlspr5BQGVijWuImSwuv2Pus0O33WOxl6WmQ/EGUmiweCvsz8qldFOKO; __atuvc=14%7C17; mage-cache-storage=%7B%7D; mage-cache-storage-section-invalidation=%7B%7D; mage-cache-sessid=true; recently_viewed_product=%7B%7D; recently_viewed_product_previous=%7B%7D; recently_compared_product=%7B%7D; recently_compared_product_previous=%7B%7D; product_data_storage=%7B%7D; mage-messages=; _gid=GA1.3.1561207854.1683040340; _dc_gtm_UA-43934200-1=1; _derived_epik=dj0yJnU9VFUxUmh4NWg4bHZZUDViTXBMMXdUdXItUXlTeV9RS2Imbj1TZ1NFaVgydkJ6ZWpBSTdCcU1uRWRBJm09MSZ0PUFBQUFBR1JSS2pjJnJtPTEmcnQ9QUFBQUFHUlJLamMmc3A9Mg; _ga_G6CZSVEKBQ=GS1.1.1683040337.7.1.1683040831.53.0.0; _ga=GA1.1.1533605102.1682706061");
            var response = await client.SendAsync(request);


            if (response.EnsureSuccessStatusCode().IsSuccessStatusCode)
            {
                //Variable htmldoc y document para obtener el cuerpo html del sitio y su posterior filtro de datos mediante los desencadenadores o selectores
                string htmldoc = await response.Content.ReadAsStringAsync();
                Console.WriteLine(htmldoc);
                HtmlDocument document = new HtmlDocument();
                document.LoadHtml(htmldoc);

                var main = document.DocumentNode.SelectSingleNode(".//div[@class='column main']");

                foreach (var libro in main.SelectNodes("//div[@class='product-item-info']"))
                {
                      
                    /*
                    * Declaracion de variables auxiliares de tipo string
                    * para la creacion de objetos tipo Libros
                    */

                    string titulo = "";
                    string precio = "";
                    string autor = "";
                    string tipo = "";
                    string enlace = "";
                    string imagen = "";

                    if (Regex.IsMatch(libro.InnerText, "^  Agregar"))
                    {
                        break; //Al final de la pagina se tiene un texto largo que inicia con el texto de arriba, si se llega ahi finaliza la busqueda
                    }


                    if (Regex.IsMatch(libro.InnerText, "$")) {
                        if (libro.Descendants("span").Where(span => span.GetClasses().Contains("price")) != null)
                        {
                            var preci = libro.Descendants("span").Where(span => span.GetClasses().Contains("price") & Regex.IsMatch(span.InnerText, "$"));
                            precio = preci.FirstOrDefault().InnerText; //Obtencion del precio
                        } }


                    if (libro.Descendants("a").Where(a => a.GetClasses().Contains("product-item-link")) != null) {
                        titulo = libro.Descendants("a").Where(a => a.GetClasses().Contains("product-item-link")).FirstOrDefault().InnerText.Trim(); //Obtencion del titulo
                    }


                    if (Regex.IsMatch(libro.InnerText, "Narrador"))
                    {
                        var aautor = libro.Descendants("span").Where(span => span.GetClasses().Contains("ammenu-wrapper")).FirstOrDefault().InnerText;
                        autor = aautor; //Obtencion del narrador si es audio libro

                    } else { 
                        if (Regex.IsMatch(libro.InnerText, "Autor"))
                        {
                            var aautor = libro.Descendants("div").Where(div => div.GetClasses().Contains("autor")).FirstOrDefault().InnerText;
                            autor = aautor; //Obtencion del autor
                        } else
                        {
                            autor = "NA"; //Si no hay autor y narrador se mostrara un NA (No aplica)
                        }
                    } 


                     if (libro.SelectSingleNode("//div[@class='autor']") != null)
                     {
                         tipo = libro.SelectSingleNode("//p[@class='product-item-link']").InnerText.Trim(); //Obtencion del tipo de libro apartir del autor
                    }
                    
                    if (libro.SelectSingleNode(".//a[@class='product-item-link']").Attributes["href"] != null)
                    {
                        var aux = libro.SelectSingleNode(".//a[@class='product-item-link']").Attributes["href"].Value;
                        enlace = aux; //Obtencion del enlace del libro
                    }

                    if (libro.SelectSingleNode("//img") != null)
                    {
                        var aux = libro.Descendants("img").Where(img => img.GetClasses().Contains("lazy") & img.GetClasses().Contains("product-image-photo")).FirstOrDefault();
                        imagen = aux.GetAttributeValue("data-srclazy", ""); //Obtencion de la imagen del libro (Caratula)
                    }

                    //Insercion de datos a objeto Libro haciendo uso de las variables auxiliares antes declaradas
                    Datos.Add(new Libros
                    {
                        autor = autor,
                        titulo = titulo,
                        precio = precio,
                        enlace = enlace,
                        imagen = imagen,
                        tapa = tipo
                    });
                }   
            }
            return true;
        }
        #endregion

        #endregion
    }
}
