using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TareaAutomaticaGasFinder.Models
{
    public  class GasStationModel
    {
        public string? departamento { get;  set; }
        public string? zona { get; set; }
        public string? Nombre { get; set; }
        public double? EspecialSC { get; set; }
        public double? RegularSC { get; set; }
        public double? DieselSC { get; set; }
        public double? IonDieselSC { get; set; }
        public double? DieselLSSC { get; set; }
        public double? EspecialAuto { get; set; }
        public double? RegularAuto { get; set; }
        public double? DieselAuto { get; set; }
        public double? IonDieselAuto { get; set; }
        public double? DieselLSAuto { get; set; }

    }
}
