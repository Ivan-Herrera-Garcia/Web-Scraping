using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Web_Scraping_Marvel_Characters.Models;

namespace Web_Scraping_Marvel_Characters.Scraping
{
    public class Scraper
    {
        /*
         *  Mi superheroe favorito es Spiderman <3
         */

        #region Varaibles Globales
        public static List<Personaje> Datos;
        public static List<string> Caracteristicas;
        public static bool band = true;
        #endregion

        #region Metodo Principal
        public static async Task<List<Personaje>> DescargarAsync()
        {
            Datos = new List<Personaje>();
            int pag = 0;
            do
            {
                string url = $"https://www.marvel.com/v1/pagination/grid_cards?offset={pag}&limit=36&entityType=character&sortField=title&sortDirection=asc";
                //     url = Obtenido de www.marvel.com
                string result = await GetJsonUrl(url);
                //Metodo creado para obtener el json del url

                JObject json = JObject.Parse(result);
                //Objeto Json creado y parseado del metodo GetJsonUrl
                int i = 0;

                var auxx = json["data"]["results"]["data"].ToString();
                if (json["data"]["results"]["data"].ToString().Equals("[]")) { band = false; }
                //Se obtienen datos con json[][][] ya que se accede mediante las rutas [][][]
                else
                {
                    foreach (var iteraciones in json["data"]["results"]["data"])
                    {
                        var data = json["data"]["results"]["data"][i]["link"]["link"].ToString().Trim();
                        await DescargarInfo(data);
                        //Metodo para extraer los datos de la pagina principal de personajes mediante el
                        //link que proporciona el json["data"]["results"]["data"][i]["link"]["link"]
                        i++;
                        Thread.Sleep(300);
                        //Tiempo de espera entre peticion a metodos
                    }
                    pag += 36;
                    if (pag > 180)
                        //Se detiene en 180 personajes, cada "pagina" contiene 36 personajes
                    {
                        break;
                        //Cuando llegue a mas de 1
                    }
                }
            } while (band);
            
            return Datos;
        }
        #endregion

        #region Extraccion de datos
        public static async Task<bool> DescargarInfo(string url)
        {
            Personaje pers = new Personaje();
            Caracteristicas = new List<string>();
            //Se instancian las variables

            string urlorg = "https://www.marvel.com" + url; // Construir la URL completa concatenando el fragmento de URL recibido con la URL base 
            var client = new HttpClient(); // Crear una instancia de HttpClient
            var request = new HttpRequestMessage(HttpMethod.Get, urlorg); // Crear una solicitud HTTP GET con la URL completa
            request.Headers.Add("Cookie", "country=mx"); // Agregar un encabezado "Cookie" a la solicitud para establecer la cookie "country" con el valor "mx"
            var response = await client.SendAsync(request); // Enviar la solicitud y obtener la respuesta como objeto HttpResponseMessage


            if (response.EnsureSuccessStatusCode().IsSuccessStatusCode)
            {
                HtmlWeb Web = new HtmlWeb(); // Crear una instancia de HtmlWeb para cargar y analizar el HTML de una página web
                HtmlDocument html = await Web.LoadFromWebAsync(urlorg); // Cargar de forma asíncrona el contenido HTML de la URL especificada y almacenarlo en un objeto HtmlDocument

                //El código procesa un documento HTML para extraer información sobre un personaje. Primero, decodifica el nombre del personaje y guarda la URL original.
                //Luego, busca secciones específicas del HTML, como habilidades y características físicas, y las extrae. También recupera una descripción del personaje
                //y su universo. Toda la información extraída se guarda en un objeto y se agrega a una lista. En resumen, el código analiza el HTML, extrae datos relevantes
                //sobre el personaje y los organiza para su uso posterior.

                pers.nombre = WebUtility.HtmlDecode(html.DocumentNode.Descendants("span").Where(span => span.GetClasses().Contains("masthead__headline")).FirstOrDefault().InnerText.Trim());

                pers.url = urlorg;

                var main = html.DocumentNode.Descendants("div").Where(div => div.GetClasses().Contains("two-column")).FirstOrDefault();

                var habilidades = html.DocumentNode.Descendants("div").Where(div => div.GetClasses().Contains("grid-wrapper--nested")).FirstOrDefault();

                if (habilidades != null)
                {
                    int cont = 1;
                    foreach (var skills in habilidades.Descendants("div").Where(div => div.GetClasses().Contains("power-circle__wrapper")))
                    {
                        switch (cont)
                        {
                            case 1:
                                pers.habilidades.Add("Durabilidad: "+ skills.Descendants("span").Where(span => span.GetClasses().Contains("power-circle__rating")).FirstOrDefault().InnerText.Trim());
                                break;
                            case 2:
                                pers.habilidades.Add("Energia: " + skills.Descendants("span").Where(span => span.GetClasses().Contains("power-circle__rating")).FirstOrDefault().InnerText.Trim());
                                break;
                            case 3:
                                pers.habilidades.Add("Habilidades de combate: " + skills.Descendants("span").Where(span => span.GetClasses().Contains("power-circle__rating")).FirstOrDefault().InnerText.Trim());
                                break;
                            case 4:
                                pers.habilidades.Add("Inteligencia: " + skills.Descendants("span").Where(span => span.GetClasses().Contains("power-circle__rating")).FirstOrDefault().InnerText.Trim());
                                break;
                            case 5:
                                pers.habilidades.Add("Velocidad: " + skills.Descendants("span").Where(span => span.GetClasses().Contains("power-circle__rating")).FirstOrDefault().InnerText.Trim());
                                break;
                            case 6:
                                pers.habilidades.Add("Fuerza: " + skills.Descendants("span").Where(span => span.GetClasses().Contains("power-circle__rating")).FirstOrDefault().InnerText.Trim());
                                break;
                            default:
                                break;
                        }
                        cont++;
                    }
                }


                if (main != null)
                {
                    if (main.Descendants("div").Where(div => div.GetClasses().Contains("text")).Any())
                    {
                        var descripcion = main.Descendants("div").Where(div => div.GetClasses().Contains("text")).FirstOrDefault();
                        if (descripcion.Descendants("p").Any())
                        {
                            string desc = descripcion.Descendants("p").FirstOrDefault().InnerText.Trim();
                          
                            pers.descripcion = WebUtility.HtmlDecode(desc);
                        }
                    }

                    foreach (var carac in main.Descendants("div").Where(div => div.GetClasses().Contains("bioheader__stats")))
                    {
                        string aux = WebUtility.HtmlDecode(carac.InnerText);
                        aux = aux.ToUpper();

                        if (Regex.IsMatch(aux, "HEIGHT"))
                        {
                            Caracteristicas.Add(aux.Replace("HEIGHT", "HEIGHT: "));
                        }
                        else if (Regex.IsMatch(aux, "EYES"))
                        {
                            Caracteristicas.Add(aux.Replace("EYES", "EYES: "));
                        }
                        else if (Regex.IsMatch(aux, "WEIGHT"))
                        {
                            Caracteristicas.Add(aux.Replace("WEIGHT", "WEIGHT: "));
                        }
                        else if (Regex.IsMatch(aux, "HAIR"))
                        {
                            Caracteristicas.Add(aux.Replace("HAIR", "HAIR: "));
                        }
                    }
                    if (main.Descendants("ul").Where(ul => ul.GetClasses().Contains("railBioInfo")).Any())
                    {
                        var uni = main.Descendants("ul").Where(ul => ul.GetClasses().Contains("railBioInfo")).FirstOrDefault().InnerText.Trim().ToUpper();
                        Caracteristicas.Add("Universo: " + uni.Replace("UNIVERSE", ""));
                    }
                    pers.caracteristicas = Caracteristicas;

                    Datos.Add(pers);
                }
            }
            return true;

        }
        #endregion

        #region Extraccion del ID con JSON
        public static async Task<string> GetJsonUrl(string url)
        {
            // Crear una solicitud HTTP GET a la URL especificada
            HttpWebRequest Request = (HttpWebRequest)WebRequest.Create(url);
            Request.Method = "GET"; // Establecer el método de solicitud como "GET"

            // Configurar los encabezados de la solicitud
            Request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/89.0.4389.82 Safari/537.36";
            Request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            Request.Headers.Add("Accept-Language", "en-US,en;q=0.5");

            // Enviar la solicitud y obtener la respuesta
            HttpWebResponse Response = (HttpWebResponse)await Request.GetResponseAsync();

            // Leer la respuesta como una cadena de texto
            using var streamReader = new StreamReader(Response.GetResponseStream());
            return await streamReader.ReadToEndAsync();
        }
        #endregion
    }
}
