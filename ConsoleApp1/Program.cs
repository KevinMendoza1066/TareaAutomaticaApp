using System;
using System.Threading.Tasks;
using PuppeteerSharp;

class Program
{
    static async Task Main(string[] args)
    {
       await new BrowserFetcher().DownloadAsync();

        using (var browser = await Puppeteer.LaunchAsync(new LaunchOptions
        {
            Headless = false
        }))
        {
            using (var page = await browser.NewPageAsync())
            {
                string url = "http://sinapp.dgehm.gob.sv/drhm/estadisticas.aspx?uid=2";
                await page.GoToAsync(url);

                var link = await page.XPathAsync("//a[contains(text(), 'CSV')]");
       

            
                if (link.Length > 0)
                {
                    var downloadLink = link[0];

                    // Obtiene la URL del enlace
                    var downloadUrl = await page.EvaluateFunctionAsync<string>("el => el.getAttribute('onclick')", downloadLink);


                    // Descarga el archivo en segundo plano
                    await page.EvaluateExpressionAsync(downloadUrl);

                    Console.WriteLine("Simulación de clic realizada. Descarga Realizada");

                    
                    await page.WaitForTimeoutAsync(10000); 


                }
                else
                {
                    Console.WriteLine("No se encontró el enlace PDF.");
                }
            }
            Procesar_Archivo();
        }


    }

    public static  void  Procesar_Archivo() {

        string fileName = "Reporte_precios_bajos.csv"; 
        string downloadsFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Downloads";
        string filePath = Path.Combine(downloadsFolder, fileName);

        try
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] fields = line.Split(',');

                    foreach (string field in fields)
                    {
                        Console.Write(field + " ");
                    }

                    Console.WriteLine(); // Salto de línea después de cada fila
                }
               
            }
            File.Delete(filePath); // Elimina el archivo después de procesarlo
            Console.WriteLine("Archivo eliminado.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }

    }
}