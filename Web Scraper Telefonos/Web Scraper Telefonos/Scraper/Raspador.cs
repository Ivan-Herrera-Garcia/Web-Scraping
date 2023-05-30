using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Web_Scraper_Telefonos.Models;

namespace Web_Scraper_Telefonos.Scraper
{
    public class Raspador
    {
        #region Variables Globales
        public static bool band = true;
        public static List<Smartphone> lista;
        #endregion

        #region Metodo principal
        public static async Task<List<Smartphone>> main()
        {
            lista = new List<Smartphone>();
            string url = "https://www.coppel.com/ofertas-celulares?fromPage=catalogEntryList&beginIndex=0&pageSize=72&pageView=grid";
            string pagina = await GetPage(url);
            await GetData(pagina);

            return lista;
        }
        #endregion

        #region Metodo para obtener la informacion de la pagina
        public static async Task<string> GetPage(string url) 
        {
            string validacion = @"^(http|https)://([\w-]+\.)+[\w-]+(/[\w-./?%&=]*)?$";
            Regex regex = new Regex(validacion);

            if (regex.IsMatch(url))
            {

                HttpWebRequest Request = (HttpWebRequest)WebRequest.Create(url);
                Request.Method = "GET";

                Request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/112.0.0.0 Safari/537.36 OPR/98.0.0.0";
                Request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
                Request.Headers.Add("Accept-Language", "en-US,en;q=0.5");
                Request.Headers.Add("Cookie", "_abck=F1B6A4DCA3E3049FB9EC273E9A647909~-1~YAAQlLU7F5do9RyIAQAAIrZGbQmJiZuL9Rhe6f3wPBmm63T/1KFmxy7ONQF/4Z16dexxfrKyRguPCpK8trhm5H3jyzao5gnjQwpphlfCTPCbuVq5tnsUnANJmfbtNhe/Mco3NlfpsjSDh2QVTVecEnfnNJHk7PePqPl0+ncUuu8bMNYz2tMh4xKViM3WuMU79Lq9WMGzgk21OmLD+V1F7qEXk5MVxPBXbgGw+ba81hxZvoacZq/IKdiGq9nyRZ2c9xTtmX6R7DcZ6nAcvVD8UtzmafX9oNkYrnV0/WrvcAEcowSUJDGN4LL+5yJG2p42yi4/ODBvIU2IT1XbLvzhFSyebd3P7nJyLhzpihftEZiDtmyEoT7bK7Q=~-1~-1~1685464088; ak_bmsc=11EDB81400805A0FD02629C553E6CAE5~000000000000000000000000000000~YAAQlLU7F+OvAB2IAQAAKE/CbRNSf9n7O33MWwlOW/IBjL8RFE81ofsDiUIS+MYgorC7zyxFzTlAsUyisg0VnnudblqeDD3q+ZH404UALiTUVBsgmnbjARg7xPZbqyLco3RfB7yEeTmQT7+Bz1qeFvkYOySZsV4ASB1wRszdISy/LSKfKVsjjydmnxq5uhLWZ1EFMSWm5iqi8pei46C1ZJ78l1+dCR1/7oGGZFABtppDIhcZiV6vL2ZdPJK+yKwOdQQiQknrbbLwdErGb3FlBQKSoYnl8I1oML5Oz7AAEj67dbtUo5B392zTvzM2B/mw0du9ahQuH4U0qo6vR6KhX///iwKukZUfjYSHjEqppSWpCKi6xmIxoONDO0o=; bm_mi=BBF5AC9446703939B5CB9E9832C26FE7~YAAQlLU7F1WzAB2IAQAAdmPCbRMpax7gv2vc8gMBEDstweFDZAsMkXe+I+effEQ258Q5sXpXDu5c08PsO1FmqpOCsenSdD71z8QwszL5jSWdTDSk8u3YEkZravLsziGWWlV9ZotdzRxGsOAg/O5nbxenC3M9DhDb98k2SNtr59lS1jB/e05sZw1B/HMjAl++iQzS+JdCxr4uSajJ9G1aTosWOuR5HYNHUKbj8mlqtfoFSk5H8iunRPEoLw43zp6u9iDYEUeT3v1Co5dwqsBA81anZAcToj8paDkEurGP4gk01EtxFfXUsjlCIpLpkhBcWr4HpoHRA96bGlurlMcV~1; bm_sz=0399A7E327AC532BBD96E3491E873DC1~YAAQlLU7F5po9RyIAQAAIrZGbRNJOQzVM2sm2J6jW5BZS2FeKyCRX5tE3yJ5nN2rn5LuiOKNgxBvkcjIIM/pkCXeFGhwRqwFFfape7wovrT0kyYWow3bgbRXEClPbj9kugdmtx7BZIGeSoSXpj01O4S2DNFzYDfogEFlMJ4NA5gOQ3cdhReYCEgYpSsfrvCOyopM2/oaOGc+z6Dr3Q+zR4pPsNRbEXsoUjobg7oKf0Aguv4+Y0kYR1HLMKWBpUQwx8irxHaxA0UWkBTIsAXVyKV6c/L0hgu5lnk0rJJw8zJgzbY=~3359302~3551557; JSESSIONID=0000W8Nn5coQIOzxQPl8YLFxqGz:-1; WC_ACTIVEPOINTER=-5%2C10151; WC_ACTIVITYDATA_-1002=G%2C-5%2CMXN%2C10051%2C4000000000000000003%2C-2000%2C1685468630053-16548%2C-5%2CMXN%2Clp5VKCLiHTBMznSrSMBPykI1DDVj%2BONeYvUNAb5%2B0daCpPXZsPEb2eWfYrsYWny1BLLTNB5BDdkAKFD3JaIx67SaU1NkhhX5ajPwWy8sFdOK8alUVFWmsVPS2ARdt%2BpRoH2f8EnofuzxY6oBOvPT2zF2ulke0Uz3GxAMWU0kwKvgUTcy5IVS%2B1AzKv4eG6a8Y9LgpdY8HQKwmzWiyqZ%2BP29l3%2BeryZw%2FmoBed1cH4f0%3D; WC_AUTHENTICATION_-1002=-1002%2CJdh5JVzqovGDGuZIS9A0yAc11LCiDPi1CWNOoTX%2B9xo%3D; WC_PERSISTENT=w6YHE5n4IkzIB7cyqJ2DQTNUFGbySLOVvECXE1PqqIY%3D%3B2023-05-30+11%3A43%3A50.059_1685468630053-16548_10151_-1002%2C-5%2CMXN%2C2023-05-30+11%3A43%3A50.059_10151; WC_SESSION_ESTABLISHED=true; WC_USERACTIVITY_-1002=-1002%2C10151%2C0%2Cnull%2Cnull%2Cnull%2Cnull%2Cnull%2Cnull%2Cnull%2C628538906%2Cver_null%2CTc20RE65Fgb95Hb6CC%2F1u5mSn9e1LQP10aU5vArlwUN1RA1cNHng8kSKp7xuAgqIFZHns6yyS8XbwrXvMFtkJykvC5%2Fk8kPmNrTGjhz0regx5rntlySGVmEFTUjNPwKUAkV%2BpT96uZAEtwnDGFC8I1iWZWz536lHuaHGIGMrJ7QstxtBMkCX14DYDdleYywXQcTsVcUnkvNMJrprSUekB7uPjvedxAa1nK9DOHRZF34%3D");

                HttpWebResponse Response = (HttpWebResponse)await Request.GetResponseAsync();

                using var streamReader = new StreamReader(Response.GetResponseStream());
                return await streamReader.ReadToEndAsync();
            } else
            {
                return null;
            }
        }
        #endregion

        #region Metodo para obtener los datos de cada producto
        public static async Task<bool> GetData(string pagina)
        {
            //Variables Locales
            string htmldoc = pagina;
            HtmlDocument html = new HtmlDocument();

            html.LoadHtml(htmldoc);
            if (html.DocumentNode.Descendants("div").Where(div => div.GetClasses().Contains("product_listing_container")).Any())
            {
                var main = html.DocumentNode.Descendants("div").Where(div => div.GetClasses().Contains("product_listing_container")).FirstOrDefault();
           
            
                foreach (var lista_a in main.Descendants("li")) {
                    Smartphone datos = new Smartphone();

                    if (!lista_a.InnerText.Equals(""))
                    {
                        foreach(var url in lista_a.Descendants("a"))
                        {
                            if (datos.url.Equals(""))
                            {
                                if (!url.GetAttributeValue("href", "").Equals(""))
                                {
                                    datos.url = url.GetAttributeValue("href", "");
                                }
                            }

                            if (datos.img.Equals("")) {
                                if (url.Descendants("img").Any())
                                {
                                    var url_img = url.Descendants("img").FirstOrDefault();
                                    datos.img = url_img.GetAttributeValue("src", "");
                                } 
                            }
                        }

                        string nombre = lista_a.Descendants("div").Where(div => div.GetClasses().Contains("product_name")).FirstOrDefault().InnerText.Trim();
                        nombre = Regex.Replace(nombre, "contado:", string.Empty);

                        datos.nombre = WebUtility.HtmlDecode(nombre);

                        if (lista_a.Descendants("span").Where(span => span.GetClasses().Contains("unique_price")).Any())
                        {
                            string precio = lista_a.Descendants("span").Where(span => span.GetClasses().Contains("unique_price")).FirstOrDefault().InnerText.Trim();
                            datos.precio_normal = WebUtility.HtmlDecode(precio);
                        }
                        else
                        {
                            string precio_normal = lista_a.Descendants("span").Where(span => span.GetClasses().Contains("old_price")).FirstOrDefault().InnerText.Trim();
                            datos.precio_normal = WebUtility.HtmlDecode(precio_normal);

                            string precio_descuento = lista_a.Descendants("span").Where(span => span.GetClasses().Contains("price")).FirstOrDefault().InnerText.Trim();
                            datos.precio_descuento = WebUtility.HtmlDecode(precio_descuento);
                        }

                        string pagos = lista_a.Descendants("p").Where(p => p.GetClasses().Contains("catalog-entry-details")).FirstOrDefault().InnerText.Trim();
                        datos.pagos = WebUtility.HtmlDecode(pagos);

                        datos.LstCarac.AddRange(await GetCarac(datos.url));

                        lista.Add(new Smartphone
                        {
                            nombre = datos.nombre,
                            precio_normal = datos.precio_normal,
                            img = datos.img,
                            pagos = datos.pagos,
                            precio_descuento = datos.precio_descuento,
                            url = datos.url,
                            LstCarac = datos.LstCarac,
                        }) ;
                    } 
                }
            } else
            {
                return false;
            }
            return true;
        }
        #endregion

        #region Metodo para obtener las caracteristicas
        public static async Task<List<string>> GetCarac(string url)
        {
            //Variables locales
            List<string> lista = new List<string>();
            string page = await GetPage(url);
            string htmldoc = page;

            if (htmldoc != null)
            {

                HtmlDocument html = new HtmlDocument();
                html.LoadHtml(htmldoc);

                var main = html.DocumentNode.Descendants("div").Where(div => div.GetClasses().Contains("productContainerDescription")).FirstOrDefault();

                foreach (var desc in main.Descendants("div").Where(div => div.GetClasses().Contains("content")))
                {
                    foreach (var list in desc.Descendants("li"))
                    {
                        lista.Add(WebUtility.HtmlDecode(list.InnerText.Trim()));
                    }

                    lista.Add(WebUtility.HtmlDecode(desc.InnerText.Trim()));
                }
                return lista;
            } else
            {
                lista.Add("Sin piezas disponibles");
                return lista;
            }
        }
        #endregion
    }
}
