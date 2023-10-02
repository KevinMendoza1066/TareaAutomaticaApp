using iluvadev;
using iluvadev.ConsoleProgressBar;

namespace TareaAutomaticaGasFinder.Utilities
{
    public  class ConsoleProgressBar
    {
        public void Test() {
            const int max = 500;

            //Create the ProgressBar
            using (var pb = new ProgressBar() { Maximum = null })
            {
                //Clear "Description Text"
                pb.Text.Description.Clear();

                //Setting "Description Text" when "Processing"
                pb.Text.Description.Processing.AddNew().SetValue(pb => $"Element: {pb.ElementName}");
                pb.Text.Description.Processing.AddNew().SetValue(pb => $"Count: {pb.Value}");
                pb.Text.Description.Processing.AddNew().SetValue(pb => $"Processing time: {pb.TimeProcessing.TotalSeconds}s.");
                pb.Text.Description.Processing.AddNew().SetValue(pb => $"Estimated remaining time: {pb.TimeRemaining?.TotalSeconds}s.");

                //Setting "Description Text" when "Done"
                pb.Text.Description.Done.AddNew().SetValue(pb => $"{pb.Value} elements in {pb.TimeProcessing.TotalSeconds}s.");

                for (int i = 0; i < max; i++)
                {
                    string elementName = Guid.NewGuid().ToString();

                    Task.Delay(10).Wait(); //Do something
                    pb.PerformStep(elementName); //Step in ProgressBar. Setting current ElementName
                }
            }

        }

        public void ProcessBar(ProgressBar pb,string Gasolinera )
        {

            pb.Text.Description.Clear();
            pb.Text.Body.Processing.SetValue("Procesando archivo , cargando nuevos precios");
            //Setting "Description Text" when "Processing"
            pb.Text.Description.Processing.AddNew().SetValue(pb => $"Procesando: {Gasolinera}");
            pb.Text.Description.Processing.AddNew().SetValue(pb => $"Cantidad de Gasolineras Procesadas: {pb.Value}");
            pb.Text.Description.Processing.AddNew().SetValue(pb => $"Tiempo en proceso : {pb.TimeProcessing.TotalSeconds}s.");

            //Setting "Description Text" when "Done"
            pb.Text.Description.Done.AddNew().SetValue(pb => $"{pb.Value} gasolineras procesadas en  {pb.TimeProcessing.TotalSeconds}s.");

            string elementName = Guid.NewGuid().ToString();
                  pb.PerformStep(elementName); //Step in ProgressBar. Setting current ElementName

        }

        public void InitProcessBar(ProgressBar pb, string mensajeProceso, string mensajeFinal)
        {

            pb.Text.Description.Clear();
            pb.Text.Body.Processing.SetValue(mensajeProceso);
            //Setting "Description Text" when "Processing"
            pb.Text.Description.Processing.AddNew().SetValue(pb => $"Procesando: {pb.ElementName}");
            pb.Text.Description.Processing.AddNew().SetValue(pb => $"Cantidad de Gasolineras Procesadas: {pb.Value}");
            pb.Text.Description.Processing.AddNew().SetValue(pb => $"Tiempo en proceso : {pb.TimeProcessing.TotalSeconds}s.\n");

            //Setting "Description Text" when "Done"
            pb.Text.Description.Done.AddNew().SetValue(pb => pb.Value+ mensajeFinal +pb.TimeProcessing.TotalSeconds+"s");

        }
        public void NextProcessBar(ProgressBar pb,string Gasolinera)
        {
            Task.Delay(100).Wait();
            pb.PerformStep(Gasolinera); 
        }
        public void ProcessBarLoad(ProgressBar pb,string Mensaje, string MensajeFinal)
        {
            pb.Text.Body.Processing.SetValue(Mensaje);
            pb.Text.Body.Done.SetValue(MensajeFinal);
            pb.Text.Description.Clear();
            Task.Delay(10).Wait(); 
              pb.PerformStep(); 

        }
    }
}
