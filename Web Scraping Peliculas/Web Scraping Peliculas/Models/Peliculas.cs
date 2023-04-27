using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web_Scraping_Peliculas.Models
{
    public class Peliculas
    {
        /*Esta clase fue creada para obtener lo importante de una pelicula (Abstraccion), como:
         * Titulo
         * Genero
         * Clasificacion
         * Duracion
         * Calificacion
         * Calificacion de MetaScore
         * Descripcion de la pelicula
         * Director y sus actores
         * Datos adicionales de la pelicula 
         */

        #region Atributos
        public string? titulo
        {
            get; set;
        }

        public string? genero
        {
            get; set;
        }

        public string? clasificacion
        {
            get; set;
        }

        public string? duracion
        {
            get; set;
        }

        public string? calif
        {
            get; set;
        }

        public string? CalifMeta
        {
            get; set;
        }

        public string? descripcion
        {
            get; set;
        }

        public string? director
        {
            get; set;
        }

        public string? MasDatos
        {
            get; set;
        }
        #endregion
    }
}
