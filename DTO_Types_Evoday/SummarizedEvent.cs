using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenCare.EVODAY
{
    public class SensorEvent
    {
        public DateTime Received{ get; set; }
        public DateTime Timestamp{ get; set; }
        public int SensorType { get; set; }
    }
    public class SummarizedEvent
    {
        public DateTime TimePeriodStart{ get; set; }
        public DateTime TimePeriodEnd{ get; set; }

        public DateTime LastUpdated{ get; set; }

        public int CurrentBed { get;  set; }
        public int CurrentPIRRoom { get;  set; }
        public int CurrentPIRBathroom { get;  set; }

        public long LatestEvent { get; set; }
        public int LastState { get;  set; }


    }
}
