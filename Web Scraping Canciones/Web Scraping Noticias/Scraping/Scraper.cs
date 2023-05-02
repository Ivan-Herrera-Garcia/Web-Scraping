using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Web_Scraping_Noticias.Models;

namespace Web_Scraping_Noticias.Scraping
{
    public class Scraper
    {

        #region Variables Globales

        public static List<Canciones> Datos;
        public static bool band = true;

        #endregion

        #region Metodos

        #region Metodo para accesar al sitio web
        public static async Task<List<Canciones>> DescargarAsync()
        {
            Datos = new List<Canciones>();
            int pag = 1;

            do
            {
                string url = $"https://www.elportaldemusica.es/lists/top-100-canciones/2023/{pag++}";
                await DescargarInfo(url);
                Thread.Sleep(300);
                pag++;
                if (pag > 16)
                {
                    break;
                } 
            } while (band);
            return Datos;
        }
        #endregion

        #region Metodo para descargar la informacion
        public static async Task<bool> DescargarInfo(String url)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Cookie", "sp_t=7abef01e-316b-4078-920f-73c4e1dd144b; sp_adid=257d01ee-1062-4e98-9088-f3eba001cce5; sp_landing=https%3A%2F%2Fopen.spotify.com%2Fplaylist%2F37i9dQZEVXbMDoHDwVN2tF%3Fsp_cid%3D7abef01e-316b-4078-920f-73c4e1dd144b%26device%3Ddesktop; _gcl_au=1.1.892717327.1683046478; _gid=GA1.2.831475731.1683046478; _scid=f95633bd-f5ea-48da-a213-d2fca0b2efe6; _fbp=fb.1.1683046479180.309090824; OptanonAlertBoxClosed=2023-05-02T16:55:27.880Z; sp_m=mx; _cs_mk_ga=0.9118266277406057_1683048847169; _cs_c=0; _sctr=1%7C1683007200000; _cs_id=ce7fbb73-4801-a617-ab4e-a2983ec82b65.1683048847.1.1683048893.1683048847.1.1717212847911; _cs_s=3.5.1.1683050693004; justRegistered=1; sp_dc=AQCFsBkyt_Z0B0hhaVEEpconlKFpbYR2bxLjC_f3382ehNSGYoj094PjeacezRVewyJLRgRghiXmpZjIRZCCvUUw-ut-inSTGnuRU-ItUChDriDB1NMac228gO19WyCjlAgb95OWwJganLCuCcc-aO6HD4yMiRoM; sp_gaid=0088fcab1e6da434d075f3da149b74b18eafd804f089a4544f9bb1; _ga_S35RN5WNT2=GS1.1.1683048847.1.1.1683048920.0.0.0; OptanonConsent=isIABGlobal=false&datestamp=Tue+May+02+2023+11%3A36%3A13+GMT-0600+(hora+est%C3%A1ndar+central)&version=6.26.0&hosts=&landingPath=NotLandingPage&groups=s00%3A1%2Cf00%3A1%2Cm00%3A1%2Ct00%3A1%2Ci00%3A1%2Cf11%3A1&geolocation=MX%3BCOA&AwaitingReconsent=false; _scid_r=f95633bd-f5ea-48da-a213-d2fca0b2efe6; _ga_ZWG1NSHWD8=GS1.1.1683046478.1.1.1683049082.0.0.0; _ga=GA1.2.42929408.1683046478; _gat_UA-5784146-31=1; ApplicationGatewayAffinity=3272f8c8e1c8c4b6b43cbe298b9050be; ApplicationGatewayAffinityCORS=3272f8c8e1c8c4b6b43cbe298b9050be; EPDMSESSID=olcbcdso911sr3f4n2btd7l6h0; _SEC-EPDM=2aec4e2268c15903075f3cca50c0b9ed5c6858bf37faddb20af7efd7e4ecaafaa%3A2%3A%7Bi%3A0%3Bs%3A9%3A%22_SEC-EPDM%22%3Bi%3A1%3Bs%3A32%3A%22ztjPJjYbAQLPD-BULpVp4hu6F7CB9vLz%22%3B%7D");
            var response = await client.SendAsync(request);

            if (response.EnsureSuccessStatusCode().IsSuccessStatusCode)
            {
                string htmldoc = await response.Content.ReadAsStringAsync();
                HtmlDocument document = new HtmlDocument();
                document.LoadHtml(htmldoc);

                var main = document.DocumentNode.SelectSingleNode("//div[@class='list-view']");
                 
                foreach (var cancion in main.SelectNodes("//div[@class='item']") )
                {
                    string nombre = "";
                    string autor = "";
                    string enlace = "";
                    string puntuacion = "";
                    string imagen = "";

                    if (cancion.Descendants("div").Where(div => div.GetClasses().Contains("name")) != null)
                    {
                        nombre = cancion.Descendants("div").Where(div => div.GetClasses().Contains("name")).FirstOrDefault().InnerText.Trim();
                    }

                    if (cancion.Descendants("div").Where(div => div.GetClasses().Contains("related")) != null)
                    {
                        autor = cancion.Descendants("div").Where(div => div.GetClasses().Contains("related")).FirstOrDefault().InnerText.Trim();
                    }

                    if (cancion.Descendants("a").Where(a => a.GetClasses().Contains("link")) != null)
                    {
                        enlace = "https://www.elportaldemusica.es" + cancion.Descendants("a").Where(a => a.GetClasses().Contains("link")).First().Attributes["href"].Value;
                    }

                    if (cancion.SelectSingleNode("//div[@class='lazy thumbnail default cover']").Attributes["data-bg"].Value != null)
                    {
                        imagen = cancion.Descendants("div").Where(div => div.GetClasses().Contains("thumbnail")).First().Attributes["data-bg"].Value;
                    }

                    if (cancion.Descendants("div").Where(div => div.GetClasses().Contains("data-rateyo-rating")) != null)
                    {
                        puntuacion = cancion.Descendants("div").Where(div => div.GetClasses().Contains("rating")).First().Attributes["data-rateyo-rating"].Value;
                    }

                    Datos.Add(new Canciones
                    {
                        autor = autor,
                        enlace = enlace,
                        imagen = imagen,
                        puntuacion = puntuacion,
                        titulo = nombre
                    });
                }
            }
            return false;
        }
        #endregion

        #endregion
    }
}
