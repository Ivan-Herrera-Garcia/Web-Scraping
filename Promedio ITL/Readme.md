# Como obtener un promedio con web scraping 

### Ya llegaron las fechas para concluir el semestre en el Instituto Tecnologico de la Laguna y si quieres saber tu promedio del siguiente semestre puedes usar el siguiente codigo que esta hecho en C# para calcular el promedio

``` C#
            var url = "http://apps2.itlalaguna.edu.mx/StatusAlumno/alumnos/frmKardex.aspx";

            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Cookie", "AQUI VAN TUS DATOS DE LA SESION");
            var response = await client.SendAsync(request);
            if (response.EnsureSuccessStatusCode().IsSuccessStatusCode)
            {
                string htmldoc = await response.Content.ReadAsStringAsync();
                HtmlDocument document = new HtmlDocument();
                document.LoadHtml(htmldoc);

                int cont = 0;
                int ite = 0;
                var main = document.DocumentNode.Descendants("div").Where(div => div.GetClasses().Contains("span-24")).FirstOrDefault();
                foreach (var calif in main.Descendants("td").Where(td => td.GetClasses().Contains("calificacion")))
                {
                    cont += int.Parse(calif.InnerText);
                    ite ++;
                }

                float r = (cont+70) / (ite+1);


```
