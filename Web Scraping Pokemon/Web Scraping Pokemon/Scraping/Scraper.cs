using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Web_Scraping_Pokemon.Models;

namespace Web_Scraping_Pokemon.Scraping
{
    public class Scraper
    {
        #region Variables Globales
        public static List<Pokemon> Datos;
        public static List<string> Lista;
        public static bool band = true;
        #endregion

        #region Metodo principal
        public static async Task<List<Pokemon>> DescargarAsync()
        {
            Lista = new List<string>();
            Datos = new List<Pokemon>();

            string url = "https://www.pokemon.com/el/api/pokedex/";

            await GetNombresAsync(url);

            Lista = Lista.Distinct().ToList();

            foreach (var info in Lista)
            {
                await GetInformacion(info);
                Thread.Sleep(300);
            }

            Datos = Datos.Distinct().ToList();
            return Datos;
        }

        #endregion

        #region Extraccion de los URLS
        public static async Task<bool> GetNombresAsync(string url)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            
            //Estas son las cookies, si no hace la conexion, tal vez se deba a esto y se tienen que cambiar haciendo una peticion GET en postman o cualquier sw que te las de
            request.Headers.Add("Cookie", "incap_ses_7226_2884021=OIFkCXib31c0Pt6uXulHZGdvYmQAAAAAN45dX9bZGpcTI5zhAIOmbA==; nlbi_2884021=m26jS85we2eTpTgIgQq3qwAAAAAzozj7aUC9kdTXXLF+fx44; visid_incap_2884021=pPQONnv5Q5SRfJmpc8OnnGdvYmQAAAAAQUIPAAAAAACPeIaszoFL8WwtBh3H1MLU; AWSALB=UeKmeabcTHkFLMXxPjtWXCdPrWzojoplE/Er8svCM5De8EAmQAtqBXgBXDPO3xkLjs4RZMu99UhiLYBm3LpZw4NjxJ52QGjyVkAX+xKc5HAJfYbKhcY/ihB8Edjn; AWSALBCORS=UeKmeabcTHkFLMXxPjtWXCdPrWzojoplE/Er8svCM5De8EAmQAtqBXgBXDPO3xkLjs4RZMu99UhiLYBm3LpZw4NjxJ52QGjyVkAX+xKc5HAJfYbKhcY/ihB8Edjn; crafterSite=pcom-main; django_language=es-xl");
            var response = await client.SendAsync(request);

            if (response.EnsureSuccessStatusCode().IsSuccessStatusCode)
            {
                JArray urls = JArray.Parse(await response.Content.ReadAsStringAsync());
                foreach (var pokemon in urls.Take(10))
                {
                    var nombre = pokemon["slug"].ToString().Trim().ToLower();
                    Lista.Add($"https://pokeapi.co/api/v2/pokemon/{nombre}");
                }
                return band;
            }
            return band; ;
        }
        #endregion

        #region Extraccion de informacion
        public static async Task<bool> GetInformacion(string url)
        {
            Pokemon data = new Pokemon();
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            var response = await client.SendAsync(request);

            if (response.EnsureSuccessStatusCode().IsSuccessStatusCode)
            {
                string html = await response.Content.ReadAsStringAsync();

                JObject json = JObject.Parse(html);

                data.nombre = json["forms"][0]["name"].ToString().Trim().ToUpper();
                data.altura = json["height"].ToString().Trim();
                data.peso = json["weight"].ToString().Trim();
                data.numero = json["id"].ToString().Trim().ToUpper();

                foreach (var tipo in json["abilities"])
                {
                    data.habilidad.Add(tipo["ability"]["name"].ToString().Trim().ToUpper());
                }

                foreach (var tipo in json["moves"])
                {
                    data.movimientos.Add(tipo["move"]["name"].ToString().Trim().ToUpper());
                }

                foreach (var tipo in json["types"])
                {
                    data.tipo.Add(tipo["type"]["name"].ToString().Trim().ToUpper());
                }
                foreach (var puntos in json["stats"])
                {
                   if (puntos["stat"]["name"].ToString().ToUpper().Equals("HP"))
                    {
                        data.puntos_base.Add("Puntos de vida: " + puntos["base_stat"].ToString().Trim());
                    }
                    if (puntos["stat"]["name"].ToString().ToUpper().Equals("ATTACK"))
                    {
                        data.puntos_base.Add("Puntos de ataque: " + puntos["base_stat"].ToString().Trim());
                    }
                    if (puntos["stat"]["name"].ToString().ToUpper().Equals("DEFENSE"))
                    {
                        data.puntos_base.Add("Puntos de defensa: " + puntos["base_stat"].ToString().Trim());
                    }
                    if (puntos["stat"]["name"].ToString().ToUpper().Equals("SPEED"))
                    {
                        data.puntos_base.Add("Puntos de velocidad: " + puntos["base_stat"].ToString().Trim());
                    }
                }
                Datos.Add(data);
            }
            return true;
        }
        #endregion
    }
}
