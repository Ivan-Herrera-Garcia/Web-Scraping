using HtmlAgilityPack;
using Proyecto_Web_Scraping_Cupones.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Proyecto_Web_Scraping_Cupones.Scraping
{
    public class Scraping
    {
        #region Variables globales
        //Se estable la variable Datos de tipo Lista de cupones para almacenar los 
        //datos que descargue la funcion DescargarAsyn()
        public static List<Cupones> Datos;

        //Se establece la variable band de tipo bool (true o false) para tener 
        //control con los ciclos foreach
        public static bool band = true;
        #endregion

        #region Metodo para accesar a la pagina "CursosDev"
        public static async Task<List<Cupones>> DescargarAsyn()
        {
            //Inicializa la lista de "Datos" como una nueva lista de objetos "Cupones".
            Datos = new List<Cupones>();

            //Establece una variable local "pag" a 1.
            int pag = 1;

            //Un ciclo do -while que descarga información de una URL
            //proporcionada, espera 300 milisegundos y comprueba si
            //la página es mayor que 5 para salir del ciclo. La URL
            //a descargar se construye dinámicamente utilizando la
            //variable "pag".
            do
            {
                string url = $"https://www.cursosdev.com/?page={pag++}";
                await DescargarInfoAsyn(url);
                Thread.Sleep(300);
                if (pag > 5)
                {
                    break;
                }
            } while (band);


            //Devuelve una lista de datos
            return Datos;
        }
        #endregion

        #region Metodo para descargar la informacion de cada cupon (Precio, Titulo, etc)
        public static async Task<bool> DescargarInfoAsyn(string url)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Cookie", "XSRF-TOKEN=eyJpdiI6IjFtNXUvT0MxTHZzT3dyOGlpR2lvM1E9PSIsInZhbHVlIjoiUFZCV0d1MEhWZHY4Zk90VUJCUEFpbjIxYnhnTFVGaytLNlVDRlhiL21Dc3hGb1Mvd1NJWHNkbkFJbFNSTzVHT0haYmVTdm1KczUwcXhuRTUwRnliMW40ZnpCY05naWpJUEw2a05JY0cvNGc5eXU1bzh5Vmt3cGJXVHVhU3QyejYiLCJtYWMiOiIxZDUyNDFkYWIxNWIyNjNhYjJiZTBlZGQ3MWFiZWU0YTZhNmIzMjc4YjVjNjhlY2MwMmIxZjZkNDIxM2E2ZDIxIn0%3D; cursosdev_session=eyJpdiI6Imtvcmx4ckdDd2FacHBDOTNCV0Z5cmc9PSIsInZhbHVlIjoiQVBhVHdYYTRKWU1mYjh2NDVjK2JJSDcwU2h5R2hFRkxyTkZ1MEFWZklKeE9xTHNyaEN5ekxoVHBSOUtndUpXY0JOTVFKWk9LMHlGMVVCd3d2NUkwaklaWHlxU2Z3VFd1RkQrTTR1S0VqOVptaVd4Q3kzYWpORTk5VnZuWVRJMnEiLCJtYWMiOiJlYjY2ZmRjY2RkNDM3MGFmYjU1ZTM0NTk2ZDU0M2M4NWZlMzRkNjJlNjU3Y2RhNTJhNTY3N2UzMWU3MjUyZmY0In0%3D");
            var response = await client.SendAsync(request);
            if (response.EnsureSuccessStatusCode().IsSuccessStatusCode)
            {
                string htmldoc = await response.Content.ReadAsStringAsync();
                HtmlDocument document = new HtmlDocument();
                document.LoadHtml(htmldoc);

                var main = document.DocumentNode.SelectSingleNode("//main");

                foreach (var articulos in main.Descendants("a").Where(a => a.GetClasses().Contains("c-card")))
                {

                    var autor = articulos.Descendants("p").Where(p => p.GetClasses().Contains("text-sm")).FirstOrDefault();
                    string autorf = "";

                   
                    if (autor != null)
                    {
                        autorf = autor.InnerText.Trim();

                        if (!autorf.Contains("Crehana"))
                        {
                            string? vacio = "";
                            if (articulos.Descendants("span").Where(span => span.GetClasses().Contains("line-through") & span.GetClasses().Contains("text-sm")).First() != null)
                            {
                                vacio = articulos.Descendants("span").Where(span => span.GetClasses().Contains("line-through") & span.GetClasses().Contains("text-sm")).First().InnerText;
                            }
                            var precio = vacio;

                            string preciof = "";

                            if (precio != null)
                            {
                                preciof = "Curso de " + precio;
                            }

                            var titulo = articulos.Descendants("h2").Where(h2 => h2.GetClasses().Contains("mt-2")).FirstOrDefault();
                            string titulof = "";
                            if (titulo != null)
                            {
                                titulof = titulo.InnerText.Trim();
                            }

                            var puntuacion = articulos.Descendants("div").Where(a => a.GetClasses().Contains("mt-2")).FirstOrDefault();
                            float puntuacionf = 0.0f;
                            if (puntuacion != null)
                            {
                                puntuacionf = float.Parse(puntuacion.InnerText.Trim());
                            }


                            string? idioma = articulos.Descendants("span").Where(span => span.GetClasses().Contains("focus:outline-none")).FirstOrDefault().InnerText;
                            string idiomaf = "";
                            if (idioma != null)
                            {
                                idiomaf = idioma.Trim();
                            }

                            var cupones = articulos.Attributes["href"].Value;

                            string direccion = cupones.ToString();

                            var enlacecupon = await ObtenerCupon(direccion);

                            Thread.Sleep(300);

                            Datos.Add(new Cupones
                            {
                                precio = preciof,
                                titulo = titulof,
                                autor = autorf,
                                puntuacion = puntuacionf,
                                idioma = idiomaf,
                                enlace = enlacecupon
                            });
                        }
                    }
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

        #region Metodo para obtener los cupones (Direccion de sitio con descuento aplicado)
        public static async Task<string> ObtenerCupon(string url)
        {
            //Es un método que devuelve una tarea asíncrona que recibe una URL como parámetro.
            //El método utiliza la clase HttpClient para enviar una solicitud GET a la URL y
            //obtener el contenido de la respuesta. Luego, el contenido se carga en un objeto
            //HtmlDocument de la biblioteca HtmlAgilityPack. El método busca el elemento principal
            //de la página y luego busca un elemento ancla con una clase específica. Si se encuentra
            //el elemento ancla, se devuelve su atributo href; de lo contrario, se devuelve “Error”.

            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Cookie", "XSRF-TOKEN=eyJpdiI6IjFtNXUvT0MxTHZzT3dyOGlpR2lvM1E9PSIsInZhbHVlIjoiUFZCV0d1MEhWZHY4Zk90VUJCUEFpbjIxYnhnTFVGaytLNlVDRlhiL21Dc3hGb1Mvd1NJWHNkbkFJbFNSTzVHT0haYmVTdm1KczUwcXhuRTUwRnliMW40ZnpCY05naWpJUEw2a05JY0cvNGc5eXU1bzh5Vmt3cGJXVHVhU3QyejYiLCJtYWMiOiIxZDUyNDFkYWIxNWIyNjNhYjJiZTBlZGQ3MWFiZWU0YTZhNmIzMjc4YjVjNjhlY2MwMmIxZjZkNDIxM2E2ZDIxIn0%3D; cursosdev_session=eyJpdiI6Imtvcmx4ckdDd2FacHBDOTNCV0Z5cmc9PSIsInZhbHVlIjoiQVBhVHdYYTRKWU1mYjh2NDVjK2JJSDcwU2h5R2hFRkxyTkZ1MEFWZklKeE9xTHNyaEN5ekxoVHBSOUtndUpXY0JOTVFKWk9LMHlGMVVCd3d2NUkwaklaWHlxU2Z3VFd1RkQrTTR1S0VqOVptaVd4Q3kzYWpORTk5VnZuWVRJMnEiLCJtYWMiOiJlYjY2ZmRjY2RkNDM3MGFmYjU1ZTM0NTk2ZDU0M2M4NWZlMzRkNjJlNjU3Y2RhNTJhNTY3N2UzMWU3MjUyZmY0In0%3D");
            var response = await client.SendAsync(request);

            //Si la funcion EnsureSuccessStatusCode() funciona la otra funcion IsSuccessStatusCode
            //nos regresa un true y entra al if
            if (response.EnsureSuccessStatusCode().IsSuccessStatusCode)
            {

                string htmldoc = await response.Content.ReadAsStringAsync();
                HtmlDocument document = new HtmlDocument();
                document.LoadHtml(htmldoc);

                //Este código busca el elemento principal de la página y luego busca
                //un elemento ancla con una clase específica. Si se encuentra el elemento
                //ancla, se devuelve su atributo href; de lo contrario, se devuelve “Error”.
                //En otras palabras, este código busca un enlace en la página que tenga una
                //clase específica y devuelve su dirección URL.
                var main = document.DocumentNode.SelectSingleNode("//main");
                var cupon = main.SelectSingleNode("//a[@class='border border-purple-800 bg-indigo-900 hover:bg-indigo-500 my-8 mr-2 text-white block rounded-sm font-bold py-4 px-6 ml-2 flex text-center items-center']");
                if (cupon != null)
                {
                    string? direccion = cupon.GetAttributeValue("href", "");
                    return direccion;
                } else
                {
                    return "Error";
                }
            }
            else
            {
                return "Error";
            }
        }
        #endregion
    }
}
