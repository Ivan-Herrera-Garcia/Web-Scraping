using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Web_Scraping_Videos.Models;

namespace Web_Scraping_Videos.Scraper
{
    public class Raspador
    {
        #region Variables Globales
        public static List<Video> lista;
        public static bool band = true;
        public static string[] arreglo = { "Descripcion", "Autor", "Fecha" };
        #endregion

        #region Metodo principal
        public static async Task<List<Video>> GetVideo()
        {
            lista = new List<Video>();
            int page = 0;
           

            do
            {
                string url = $"https://cnnespanol.cnn.com/tag/videos-digitales/page/{page++}/";
                await GetInfo(url);
                Thread.Sleep(300);

                if (page > 37)
                {
                    break;
                }
                
            } while (band);

            return lista;
        }
        #endregion


        public static async Task<bool> GetInfo(string url)
        {
            Video info = new Video();

            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            var response = await client.SendAsync(request);

            if (response.EnsureSuccessStatusCode().IsSuccessStatusCode)
            {
                string htmldoc = await response.Content.ReadAsStringAsync();

                HtmlDocument html = new HtmlDocument();

                html.LoadHtml(htmldoc);

                var main = html.DocumentNode.Descendants("div").Where(div => div.GetClasses().Contains("section__content")).FirstOrDefault(); 
                //Se hace un filtro solo trayendo la parte que nos interesa obtener su informacion

                if (main != null)
                {

                    foreach (var video in main.Descendants("div").Where(div => div.GetClasses().Contains("col--4")))
                    {
                        info.nombre = WebUtility.HtmlDecode(video.InnerText.Trim()); //Nombre del video

                        info.nombre = Regex.Replace(info.nombre, @"\b\d+:\d+\b", String.Empty);

                        if (video.Descendants("a").Where(a => a.GetClasses().Contains("news__media-item")).Any())
                        {
                            var link = video.Descendants("a").Where(a => a.GetClasses().Contains("news__media-item"));
                            info.url = link.FirstOrDefault().GetAttributeValue("href", "");

                            await GetMoreData(info.url); //Metodo para recolectar descripcion, fecha y autor de la pagina del video

                        }

                        if (!arreglo[0].Equals("Descripcion"))
                        {
                            info.desc = arreglo[0];
                        }
                        else
                        {
                            info.desc = "Sin descripcion";
                        }

                        if (!arreglo[1].Equals("Autor"))
                        {
                            info.autor = arreglo[1];
                        }
                        else
                        {
                            info.autor = "Autor anonimo";
                        }

                        if (!arreglo[2].Equals("Fecha"))
                        {
                            info.fecha = arreglo[2];
                        }
                        else
                        {
                            info.fecha = "Sin dato";
                        }


                        if (!info.nombre.Equals("")) //Si nombre incluye algo diferente de cadena vacia agrega un nuevo objeto a la lista
                        {                            //de tipo Video
                            lista.Add(new Video
                            {
                                autor = info.autor,
                                desc = info.desc,
                                nombre = info.nombre,
                                url = info.url,
                                fecha = info.fecha,

                            });
                        }
                    }
                }
                else
                {
                    return false;
                }
                return true;
            } else
            {
                return false;
            }
           
        }


        public static async Task<string[]> GetMoreData(string link)
        {

            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, link);
            var response = await client.SendAsync(request);

            if (response.EnsureSuccessStatusCode().IsSuccessStatusCode)
            {

                string htmldoc = await response.Content.ReadAsStringAsync();

                HtmlDocument html = new HtmlDocument();

                html.LoadHtml(htmldoc);

                var main = html.DocumentNode.Descendants("div").Where(div => div.GetClasses().Contains("col--main")).FirstOrDefault(); 
                //Se hace un filtro para obtener la informacion que nos interesa                

                var aux3 = main.Descendants("div").Where(div => div.GetClasses().Contains("news__excerpt")).FirstOrDefault();

                if (aux3 != null)
                    {
                        var aux = main.Descendants("div").Where(div => div.GetClasses().Contains("news__excerpt")).FirstOrDefault().InnerText.Trim(); //Descripcion
                        arreglo[0] = WebUtility.HtmlDecode(aux); //Descripcion
                    }

                var aux2 = main.Descendants("div").Where(div => div.GetClasses().Contains("news__meta")).FirstOrDefault();

                if (aux2 != null)
                   {
                        var aux = main.Descendants("div").Where(div => div.GetClasses().Contains("news__meta")).FirstOrDefault().InnerText.Trim(); //Autor
                        string[] partes = aux.Split("Publicado");

                        arreglo[1] = WebUtility.HtmlDecode(partes[0]); //Autor
                        arreglo[2] = WebUtility.HtmlDecode("Publicado "+partes[1]); //Fecha
                }    
            }
            return arreglo;
        }
    }
}
