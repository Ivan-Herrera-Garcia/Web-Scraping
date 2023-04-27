using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Web_Scraping_Peliculas.Models;

namespace Web_Scraping_Peliculas.Scraping
{
    public class Scraper
    {
        /*
         * Este proyecto tiene como finalidad realizar el scraping/raspado de un sitio web de peliculas
         *usando C# y sus correspondientes componentes NuGet (ScrapySharp, HtmlAgilityPack).
         *El proyecto es hecho con fines de aprendizaje, sin malisia alguna y sin perjudicar al mismo.
         */

        #region Variables Globales
        public static List<Peliculas> Datos;
        public static bool band = true;
        #endregion

        #region Metodos
        #region Metodo DescargarDatos
        public static async Task<List<Peliculas>> DescargarDatos()
        {
            int pag = 1;
            Datos = new List<Peliculas>();
            do
            {
                string url = "https://www.imdb.com/list/ls054250335/?ref_=otl_1";
                await DescargarPeliculas(url);
                pag++;
                Thread.Sleep(300);
                if (pag > 1)
                {
                    break;
                }
            } while (band);

            return Datos;
        }
        #endregion

        #region Metodo DescargarPeliculas
        public static async Task<bool> DescargarPeliculas(string url)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Cookie", "session-id=143-3121129-3685302; session-id-time=2313244704; uu=eyJpZCI6InV1ZjJjYTlhYzM0MGRkNDc1Mjg4OWUiLCJwcmVmZXJlbmNlcyI6eyJmaW5kX2luY2x1ZGVfYWR1bHQiOmZhbHNlfX0=");
            var response = await client.SendAsync(request);

            if (response.EnsureSuccessStatusCode().IsSuccessStatusCode)
            {
                string htmldoc = await response.Content.ReadAsStringAsync();
                HtmlDocument document = new HtmlDocument();
                document.LoadHtml(htmldoc);

                var main = document.DocumentNode.SelectSingleNode("//div[@class='lister list detail sub-list']");

                foreach (var pelis in main.SelectNodes("//div[@class='lister-item mode-detail']"))
                {
                    string? titulo = "";
                    string? genero = "";
                    string? clasificacion = "";
                    string? CalifMeta = "";
                    string? calif = "";
                    string? descripcion = "";
                    string? director = "";
                    string? duracion = "";
                    string? datos = "";

                    var aux = pelis.Descendants("h3").Where(h3 => h3.GetClasses().Contains("lister-item-header"));
                    if (aux != null)
                    {
                        titulo = aux.FirstOrDefault().InnerText.Trim();
                    }

                    var aux2 = pelis.SelectSingleNode(".//span[@class='certificate']");
                    if (aux2 != null)
                    {
                        clasificacion = aux2.InnerText;
                        clasificacion.First();
                    }

                    var aux3 = pelis.Descendants("span").Where(span => span.GetClasses().Contains("runtime"));
                    if (aux3 != null)
                    {
                        duracion = aux3.FirstOrDefault().InnerText.Trim();
                    }

                    var aux4 = pelis.Descendants("span").Where(span => span.GetClasses().Contains("genre"));
                    if (aux4 != null)
                    {
                        genero = aux4.First().InnerText.Trim();
                    }

                    var aux5 = pelis.Descendants("span").Where(span => span.GetClasses().Contains("ipl-rating-star__rating"));
                    if (aux5 != null)
                    {
                        calif = aux5.FirstOrDefault().InnerText.Trim();
                    }

                    var aux6 = pelis.SelectNodes(".//div[@class='inline-block ratings-metascore']");
                    if (aux6 != null)
                    {
                        CalifMeta = aux6.FirstOrDefault().InnerText.Trim();
                    }
                    else
                    {
                        CalifMeta = "NA";
                    }

                    
                    var aux7 = pelis.Descendants("p").Where(p => p.Attributes["class"].Value == "");
                    if (aux7 != null)
                    {
                        descripcion = aux7.First().InnerText.Trim();
                    }


                    var aaux = pelis.SelectNodes(".//p[@class='text-muted text-small']").Where(p => Regex.IsMatch(p.InnerText, "\n    Director:") || Regex.IsMatch(p.InnerText, "\n    Stars:"));
                    if ((Regex.IsMatch(aaux.First().InnerText.Trim(), "^\n    Director:") | Regex.IsMatch(aux.First().InnerText.Trim(), "\n    Stars:")) != null)
                    {
                        if (pelis.Descendants("p").Where(p => p.Attributes["class"].Value == "text-muted text-small" & Regex.IsMatch(p.InnerText, "\n    Director:") || Regex.IsMatch(p.InnerText, "\n    Stars:")) != null)
                        {
                            
                            director = aaux.First().InnerText.Trim();
                        }
                    }

                    var aaux2 = pelis.SelectNodes(".//p[@class='text-muted text-small']").Where(p => Regex.IsMatch(p.InnerText, "\n                Votes:"));
                    
                        if (pelis.Descendants("p").Where(p => p.Attributes["class"].Value == "text-muted text-small" & Regex.IsMatch(p.InnerText, "\n                Votes:")) != null)
                        {
                        datos = aaux2.First().InnerText.Trim();
                        
                    }


                    Datos.Add(new Peliculas
                    {
                        titulo = titulo.Substring(3, titulo.Length-3),
                        clasificacion = clasificacion,
                        genero = genero,
                        calif = calif,
                        CalifMeta = CalifMeta,
                        descripcion = descripcion,
                        director = director,
                        duracion = duracion,
                        MasDatos = datos
                    }) ;
                }
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
        #endregion
    }
}
