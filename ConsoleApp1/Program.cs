using System;
using System.Threading.Tasks;
using ConsoleApp1.DataAccess;
using ConsoleApp1.Models;
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
        Operations operations = new Operations();
        try
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                bool isFirstRow = true;
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] fields = line.Split(',');

                    /* foreach (string field in fields)
                     {
                         Console.Write(field + " ");
                     }*/
                    if (isFirstRow) {
                        isFirstRow = false;
                        continue;
                    }
                    try {

                        operations.UpdateGasStations(new GasStationModel
                        {
                            departamento = fields[0],
                            zona = fields[1],
                            Nombre = fields[2],
                            EspecialSC = Double.Parse((fields[4] == "" || fields[4] == null) ? "0.00" : fields[4].Replace("$", "")),
                            RegularSC = Double.Parse((fields[5] == "" || fields[5] == null) ? "0.00" : fields[5].Replace("$", "")),
                            DieselSC = Double.Parse((fields[6] == "" || fields[6] == null) ? "0.00" : fields[6].Replace("$", "")),
                            IonDieselSC = Double.Parse((fields[7] == "" || fields[7] == null) ? "0.00" : fields[7].Replace("$", "")),
                            DieselLSSC = Double.Parse((fields[8] == "" || fields[8] == null) ? "0.00" : fields[8].Replace("$", "")),
                            EspecialAuto = Double.Parse((fields[9] == "" || fields[9] == null) ? "0.00" : fields[9].Replace("$", "")),
                            RegularAuto = Double.Parse((fields[10] == "" || fields[10] == null) ? "0.00" : fields[10].Replace("$", "")),
                            DieselAuto = Double.Parse((fields[11] == "" || fields[11] == null) ? "0.00" : fields[11].Replace("$", "")),
                            IonDieselAuto = Double.Parse((fields[12] == "" || fields[12] == null) ? "0.00" : fields[12].Replace("$", "")),
                            DieselLSAuto = Double.Parse((fields[13] == "" || fields[13] == null) ? "0.00" : fields[13].Replace("$", ""))
                        });
                        Console.WriteLine("Se actulizo Registro : " + fields[2]);
                    } catch (Exception ex){
                        Console.WriteLine("Error:  " + ex.Message + $"Gasolinera { fields[2]}");
                        continue;
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