using System;
using System.Threading.Tasks;
using TareaAutomaticaGasFinder.DataAccess;
using TareaAutomaticaGasFinder.Models;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using PuppeteerSharp;
using TareaAutomaticaGasFinder.Utilities;
using iluvadev.ConsoleProgressBar;

class Program
{

    static async Task Main(string[] args)
    {

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"Inicia tarea automatica para procesos de Precios de Gasolineras");
        Console.ResetColor();
        await new BrowserFetcher().DownloadAsync();

        using (var browser = await Puppeteer.LaunchAsync(new LaunchOptions
        {
            Headless = false
        }))
        {
            using (var page = await browser.NewPageAsync())
            {
                ConsoleProgressBar progressBar = new ConsoleProgressBar();
                //Para validar proceso de descarga de archivo 
                using (var pb = new ProgressBar() { Maximum = null })
                {
                    progressBar.ProcessBarLoad(pb,"Iniciando proceso de descarga de archivo de precios", "Archivo descargado correctamente");
                    string url = "http://sinapp.dgehm.gob.sv/drhm/estadisticas.aspx?uid=2";
                    await page.GoToAsync(url);

                    var link = await page.XPathAsync("//a[contains(text(), 'CSV')]");

                    if (link.Length > 0)
                    {
                        var downloadLink = link[0];

                        // Obtiene la URL del enlace
                        var downloadUrl = await page.EvaluateFunctionAsync<string>("el => el.getAttribute('onclick')", downloadLink);
                        //Esperamos para cargar la pagina 
                        await page.WaitForTimeoutAsync(10000);
                        // Descarga el archivo en segundo plano
                        await page.EvaluateExpressionAsync(downloadUrl);
                        //Esperamos para descargar el archivo 
                        await page.WaitForTimeoutAsync(10000);

                    }
                    else
                    {
                        Console.WriteLine("No se encontró el enlace CSV.");
                    }
                }
            }
            //Termina de descargar el archivo para procesarlo 
             Procesar_Archivo();
        }
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"Inicia Proceso de Precios Actuales de Gasolina\n");
        Console.ResetColor();
        Procesar_Precios();
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
                using (var pb = new ProgressBar() { Maximum = null })
                {
                    ConsoleProgressBar progressBar = new ConsoleProgressBar();
                    progressBar.InitProcessBar(pb, "Procesando archivo , cargando nuevos precios", " gasolineras procesadas en ");
                    
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();

                        string[] fields = line.Split(',');
                       
                    if (isFirstRow) {
                        isFirstRow = false;
                        continue;
                    }
                    try {
                            if (line == String.Empty ) {
                                continue;
                            }
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
                            progressBar.NextProcessBar(pb, $"Se actulizo Registro : {fields[2]}");
                     
                    } catch (Exception ex){
                        Console.WriteLine("\nError:  " + ex.Message + $"Gasolinera { fields[2]}");
                        continue;
                    }
                }
                }
            }
            
        }
        catch (Exception ex)
        {
            Console.WriteLine("\n Error: " + ex.Message);
            
        }
        File.Delete(filePath); // Elimina el archivo después de procesarlo
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Archivo eliminado.");
        Console.ResetColor();
        

    }

    public static void Procesar_Precios() {

            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--headless");
            List<object> prices = new List<object>();
            Operations operations = new Operations();
            using (IWebDriver driver = new ChromeDriver(options))
            {
                driver.Navigate().GoToUrl("http://sinapp.dgehm.gob.sv/drhm/precios.aspx");
                System.Threading.Thread.Sleep(5000);
                IWebElement table = driver.FindElement(By.Id("ctl00_ContentPlaceHolder1_rpv_precios_com_fixedTable"));
                string tablaTexto = table.Text;


                string[] words = tablaTexto.Split(new char[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                var CountPrecios = -1;
                string[] gasTypes =["GASOLINA ESPECIAL", "GASOLINA REGULAR", "DIESEL BAJO EN AZUFRE"];
                var zone = "";

                foreach (string word in words)
                {

                    if (CountPrecios > 0 && CountPrecios <= 3)
                    {

                        prices.Add(new PricesModel { Zona = zone, TipoGasolina = gasTypes[CountPrecios - 1], Valor = word });
                        CountPrecios += 1;
                    }
                    else
                    {
                        CountPrecios = -1;
                    }

                    if (word.ToLower() == "central" || word.ToLower() == "occidental" || word.ToLower() == "oriental")
                    {
                        CountPrecios = 0;
                        CountPrecios += 1;
                        zone = word;

                    }

                }
            using (var pb = new ProgressBar() { Maximum = null })
            {
                ConsoleProgressBar progressBar = new ConsoleProgressBar();
                progressBar.InitProcessBar(pb, "Actualizando precios de Gasolina", " Precios actualizados en ");
                foreach (PricesModel price in prices)
                {
                    progressBar.NextProcessBar(pb,price.TipoGasolina + ": Zona" + price.Zona);
                    operations.UpdatePrices(price);

                }
            }
            // Cierra el navegador
            driver.Quit();
            
            }
        }
}