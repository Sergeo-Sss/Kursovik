using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kursovik
{
    public class Inspection
    {
        public string InspectionDate { get; set; }
        public string Result { get; set; }
        public string Notes { get; set; }
        public string VIN { get; set; }
        public int? ServiceEmployeeId { get; set; }
    }
}