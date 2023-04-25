using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Web_Scraping_Videojuegos.Models;

namespace Web_Scraping_Videojuegos.Scraping
{
    public class Scraper
    {
        #region Variables globales
        public static List<Juegos> Datos;
        public static bool band = true;
        #endregion

        #region Metodo para accesar al sitio web
        public static async Task<List<Juegos>> DescargarAsync()
        {
            //La función comienza inicializando la variable pag en 1 y creando una nueva lista de objetos Juegos llamada Datos.
            //Luego, entra en un bucle do-while que se ejecuta mientras la variable band sea verdadera.
            //Dentro del bucle, se construye una URL utilizando la variable pag, que se incrementa en cada iteración.
            //Luego, se llama a la función DescargarInfo con la URL como argumento y se espera a que se complete.
            //Después de esperar 300 milisegundos, el bucle verifica si el valor de pag es mayor que 3 y, de ser así, sale del bucle.
            //Finalmente, después de salir del bucle, la función devuelve la lista Datos.

            int pag = 1;
            Datos = new List<Juegos>();
            do
            {
                string url = $"https://www.eneba.com/store/all?page={pag++}";
                await DescargarInfo(url);
                Thread.Sleep(300);

                //Se dejo en 3 ya que hasta el momento 25 de abril del 2023 se contaban con solo 3 paginas en el apartado de store/all
                if (pag > 3)
                {
                    break;
                }
            } while (band);
             return Datos;
        }
        #endregion

        #region Metodo para descargar la informacion de los juegos
        public static async Task<bool> DescargarInfo(string url)
        {
            //La función toma una URL como parámetro y devuelve un valor booleano que indica si la descarga fue exitosa o no.
            //La función crea una instancia de la clase HttpClient, agrega una cookie personalizada a la solicitud y envía
            //una solicitud GET a la URL especificada. Si la respuesta es exitosa, el contenido de la página se lee como
            //una cadena y se carga en un objeto HtmlDocument. Luego, se busca un elemento main con la clase YGeqb0 en el
            //documento HTML y se recorren todos los elementos div con la clase pFaGHa WpvaUk dentro del elemento main.
            //Para cada elemento div, se extraen los valores de las variables imagen, titulo y region.


            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Cookie", "userId=226640863073786062971436770587175; fs=1; zd=21; __utmzzses=1; region=mexico; _gid=GA1.2.1857323434.1682441251; _gcl_au=1.1.571503117.1682441251; _hjFirstSeen=1; _hjSession_1050214=eyJpZCI6IjI2YThkMjA2LTQ3NDUtNDA2MC05NTI3LTZmYzNlYTZkZDgxYiIsImNyZWF0ZWQiOjE2ODI0NDEyNTA4ODMsImluU2FtcGxlIjpmYWxzZX0=; _hjAbsoluteSessionInProgress=1; exchange=MXN; isLang=1; _hjSessionUser_1050214=eyJpZCI6ImVhMTNkODVkLTc0NzItNWZkOS1hMDM4LWUwOTUzYzlhYzJlYyIsImNyZWF0ZWQiOjE2ODI0NDEyNTA4NjQsImV4aXN0aW5nIjp0cnVlfQ==; cconsent=1; _hjIncludedInSessionSample_1050214=0; _fbp=fb.1.1682443194100.66639037; __cf_bm=wIZcMm7BFPvXs0DMIEshFPOzvgR5xBLnFM1Okh3paE0-1682443238-0-AdTJy4vbS6KHd80FWk7RaCFWKW/A+xNbrEVosETXvGK9BKrtEAya+WuaRXjGJDqN03I9YJdwQ8m+eb9VtqbE7vwFE0XzU6dB4Di0aLyXoZFz; _vid_t=V5roRxIlnjJitkh7uYTGKHQeMwAFGuIfae2HZE2xNkuITbWqUfxSyuTkfGH6GP6q+50sYst5wXrt5VSc6IITW6xKTQ==; lng=en; _ga=GA1.1.477616124.1682441251; _ga_DLP0VZCBXJ=GS1.1.1682441250.1.1.1682444311.60.0.0; af_id=operagx-desktop-std; exchange=EUR; userId=102670562124007237628655826314569; fs=1");
            var response = await client.SendAsync(request);
            if (response.EnsureSuccessStatusCode().IsSuccessStatusCode)
            {
                string htmldoc = await response.Content.ReadAsStringAsync();
                HtmlDocument document = new HtmlDocument();
                document.LoadHtml(htmldoc);

                var main = document.DocumentNode.SelectSingleNode("//main[@class='YGeqb0']");

                foreach (var articulo in main.SelectNodes("//div[@class='pFaGHa WpvaUk']"))
                {
                    string? imagen = "";
                    string? titulo = "";
                    string? region = "";
                    string? precio = "";
                    string? popularidad = "";


                    if (articulo.Descendants("img").Where(a => a.GetClasses().Contains("LBwiWP")).FirstOrDefault().Attributes["src"].Value != null)
                    {
                        imagen = articulo.Descendants("img").Where(a => a.GetClasses().Contains("LBwiWP")).FirstOrDefault().Attributes["src"].Value.ToString();
                    }
                    if (articulo.Descendants("div").Where(div => div.GetClasses().Contains("lirayz")).FirstOrDefault() != null)
                    {
                        titulo = articulo.Descendants("div").Where(div => div.GetClasses().Contains("lirayz")).FirstOrDefault().InnerText;
                    }
                    if (articulo.Descendants("div").Where(div => div.GetClasses().Contains("Pm6lW1")).FirstOrDefault() != null)
                    {
                        region = articulo.Descendants("div").Where(div => div.GetClasses().Contains("Pm6lW1")).FirstOrDefault().InnerText;
                    }
                    if (articulo.Descendants("span").Where(span => span.GetClasses().Contains("L5ErLT")).FirstOrDefault() != null)
                    {
                        precio = articulo.Descendants("span").Where(span => span.GetClasses().Contains("L5ErLT")).FirstOrDefault().InnerText;
                    }
                    if (articulo.Descendants("span").Where(span => span.GetClasses().Contains("BwtiXe")).FirstOrDefault() != null)
                    {
                        popularidad = articulo.Descendants("span").Where(span => span.GetClasses().Contains("BwtiXe")).FirstOrDefault().InnerText;
                    }

                    Datos.Add(new Juegos
                    {
                        imagen = imagen,
                        titulo = titulo,
                        region = region,
                        precio = precio,
                        popularidad = popularidad
                    });
                }

                band = true;
                return band;
            } else
            {
                band = false;
                return band;
            }
        }
        #endregion
    }
}
