using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Web_Scraping_Digimon.Models;

namespace Web_Scraping_Digimon.Scrapping
{
    public class Scraper
    {
        #region Variables Globales
        public static List<Digimon> digimon;
        #endregion

        #region Metodo principal
        public static async Task<List<Digimon>> Digimons()
        {
            //Se crean objetos locales e inicializan, uno de tipo digimon y otro objeto lista de tipo cadena (string)
            digimon = new List<Digimon>();
            List<string> lista = new List<string>();

            //Se almacenan las url en lista
            lista = await GetUrls();
        
            //Se hace el recorrido de lista para obtener la informacion con el metodo GetDatos
            foreach (var dir in lista)
            {
                await GetDatos(dir);
                Thread.Sleep(300);
            }
            return digimon;
        }
        #endregion

        #region Metodo
        public static async Task<bool> GetDatos(string dir)
        {
            //Un objeto tipo Digimon
            Digimon datos = new Digimon();

            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, dir);
            request.Headers.Add("Cookie", "Geo={%22region%22:%22COA%22%2C%22city%22:%22la garcita%22%2C%22country_name%22:%22mexico%22%2C%22country%22:%22MX%22%2C%22continent%22:%22NA%22}; _b2=USHyOeyONi.1684886747445; wikia_beacon_id=duAqQh8cbL");
            var response = await client.SendAsync(request);

            if (response.EnsureSuccessStatusCode().IsSuccessStatusCode) 
            {
                HtmlWeb web = new HtmlWeb();
                HtmlDocument html = await web.LoadFromWebAsync(dir);

                //Contenido importante sin filtrar
                var main = html.DocumentNode.Descendants("div").Where(div => div.GetClasses().Contains("mw-parser-output")).FirstOrDefault();

                //Nombre del Digimon
                datos.nombre = dir.Replace("https://digimon.fandom.com/es/wiki/", string.Empty);

                //Enlace del digimon (URL)
                datos.url = dir;

                //Descripcion mediante un recorrido por las etiquetas p
                foreach (var desc in main.Descendants("p"))
                {
                    if (!desc.InnerText.Equals(""))
                    {
                        datos.descripcion = datos.descripcion + "\n" + WebUtility.HtmlDecode(desc.InnerText.Trim());
                        datos.descripcion = Regex.Replace(datos.descripcion, @"\[[^\]]*\]", "");
                    }
                }

                //Enlace de la imagen
                if (main.Descendants("img").Where(img => img.GetClasses().Contains("pi-image-thumbnail")).Any())
                {
                    var pic = main.Descendants("img").Where(img => img.GetClasses().Contains("pi-image-thumbnail")).FirstOrDefault();
                    datos.imagen = Regex.Replace(pic.GetAttributeValue("src", ""), "amp;", string.Empty);
                } else
                {

                }


                //Caracteristicas de los digimon mediante un recorriodo haciendo uso de la etiqueta div
                if (main.SelectNodes("//div[@class='pi-item pi-data pi-item-spacing pi-border-color']") != null)
                {
                    foreach (var carac in main.SelectNodes("//div[@class='pi-item pi-data pi-item-spacing pi-border-color']"))
                    {
                        var texto = Regex.Replace(carac.InnerText.Trim(), @"\[[^\]]*\]", "");
                        datos.lstCaracteristicas.Add(WebUtility.HtmlDecode(texto));
                    }
                }

                //Ataques de los digimon mediante un recorriodo haciendo uso de la etiqueta li
                foreach (var ataques in main.Descendants("li").Where(li => !li.GetClasses().Any()))
                {
                    if (!ataques.InnerText.ToLower().Contains("digimon") && !ataques.InnerText.ToLower().Contains("mon") && !Regex.IsMatch(WebUtility.HtmlDecode(ataques.InnerText), "↑"))
                    {
                        //En esta parte se hace uso de Regex ya que se extrae texto con basura (Caracteres que no sirven como informacion)
                        var texto = Regex.Replace(ataques.InnerText.Trim(), @"\[[^\]]*\]", "");
                        texto = WebUtility.HtmlDecode(texto);
                        datos.lsAtaques.Add(texto); 
                    }
                }

                //Ataques de los digimon mediante un recorriodo haciendo uso de la etiqueta table
                if (main.Descendants("table").Where(table => table.GetClasses().Contains("wikitable")).Any())
                {
                    var aux = main.Descendants("table").Where(table => table.GetClasses().Contains("wikitable")).FirstOrDefault();
                    foreach (var column in aux.Descendants("tr"))
                    {
                        if (!column.InnerText.ToLower().Contains("ataque")) {
                            var texto = Regex.Replace(column.InnerText.Trim(), @"\[[^\]]*\]", "");
                            texto = WebUtility.HtmlDecode(texto);
                            datos.lsAtaques.Add(texto);
                        }
                    }
                }

                //Ataques de los digimon mediante un recorriodo haciendo uso de la etiqueta table
                if (main.Descendants("table").Where(table => table.GetClasses().Contains("jquery-tablesorter")).Any())
                {
                    var aux = main.Descendants("table").Where(table => table.GetClasses().Contains("jquery-tablesorter")).FirstOrDefault();
                    foreach (var column in aux.Descendants("tr"))
                    {
                        if (!column.InnerText.ToLower().Contains("ataque"))
                        {
                            var texto = Regex.Replace(column.InnerText.Trim(), @"\[[^\]]*\]", "");
                            texto = WebUtility.HtmlDecode(texto);
                            datos.lsAtaques.Add(texto);
                        }
                    }
                }

                //Todos los datos recabados se pasan a digimon para volver al metodo principal 
                digimon.Add(datos);
            }
            return true;
        }
        #endregion

        #region Metodo que extrae las direcciones de cada Digimon
        public static async Task<List<string>> GetUrls()
        {
            //Se crea un objeto Lista de tipo string para el almacenamiento 
            //de las direcciones.
            List<string> lista = new List<string>();

            //La variable url es la direccion donde se encuentran todos los digimos a
            //los que se extraera la informacion
            string url = "https://digimon.fandom.com/es/wiki/Listado_de_Digimon";

            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Cookie", "Geo={%22region%22:%22COA%22%2C%22city%22:%22la garcita%22%2C%22country_name%22:%22mexico%22%2C%22country%22:%22MX%22%2C%22continent%22:%22NA%22}; _b2=USHyOeyONi.1684886747445; wikia_beacon_id=duAqQh8cbL");
            var response = await client.SendAsync(request);
           
            if (response.EnsureSuccessStatusCode().IsSuccessStatusCode)
            {
                HtmlWeb web = new HtmlWeb();
                HtmlDocument html = await web.LoadFromWebAsync(url);

                //Se extrae toda la informacion de la etiqueta div con un pequeño filtro para poder obtener la url de cada digimon
                var main = html.DocumentNode.Descendants("div").Where(div => div.GetClasses().Contains("mw-parser-output")).FirstOrDefault();

                //Se extraen las direcciones mediante un recorrido haciendo uso de la etiqueta li
                foreach(var linea in main.Descendants("li"))
                {
                    //En la etiqueta li, debe de existir la etiqueta a para entrar a la condicion verdadera
                    if (linea.Descendants("a").Any())
                    {
                        //Se extrae la informacion de la etiqueta a, siempre y cuando no contenga en su texto "mon" y "digimon".
                        //Esto debido a que existen muchas a con otros contenidos diferentes al que buscamos
                        var aux = linea.Descendants("a").FirstOrDefault().InnerText.ToLower();
                        if (aux.Contains("mon") & !aux.Contains("digimon"))
                        {
                            //Al final se extrae la direccion de la etiqueta a con el atributo href y se le concatena "https://digimon.fandom.com" + a.href
                            lista.Add("https://digimon.fandom.com"+ linea.Descendants("a").FirstOrDefault().GetAttributeValue("href", ""));
                        }
                    }
                }

            }
            return lista;
        }
        #endregion
    }
}
