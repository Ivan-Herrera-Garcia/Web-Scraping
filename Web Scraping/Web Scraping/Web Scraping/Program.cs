using HtmlAgilityPack;
using ScrapySharp.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Web_Scraping
{
    class Program
    {
        static async Task Main(string[] args)
        { 
            #region ¿Que hace el codigo?
            //Este fragmento de código utiliza la librería HTML Agility Pack para
            //analizar un documento HTML y extraer los títulos de los elementos
            //que tengan la clase CSS "entry-title".
            #endregion

            List<String> titulos = new List<string>();
            //Lista de strings para guardar los titulos


            HtmlWeb datos = new HtmlWeb();
            HtmlDocument doc = datos.Load("https://hdeleon.net");

            //HtmlNode body = doc.DocumentNode.CssSelect("body").First();
            //Obtiene todo el codigo html dentro de body

            //string sBody = body.InnerHtml;
            //Obtiene todo el codigo y se almacena en sBody

            foreach (var nodo in doc.DocumentNode.CssSelect(".entry-title")) 
            {
                //Ciclo for utilizado para realizar un recorrido en los nodos que hagan
                //uso de la clase "entry-title"

                
                var NodoAux = nodo.CssSelect("a").First();
                //Se utiliza una variable auxiliar para almacenar el 
                //resultado de CssSelect de a y el primer valor dado

                titulos.Add(NodoAux.InnerHtml);
                //Se obtiene el titulo de forma libre sin formato de codigo HTML

                Console.WriteLine(NodoAux.InnerText);
                //Se muestra el resultado (10 titulos que se encuentran)
                //en la pagina de inicio de HdeLeon.net
                
            }
        }
    }
}
