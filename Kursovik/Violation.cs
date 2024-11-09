using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kursovik
{
    public class Violation
    {
        public int ProtocolNumber { get; set; }
        public string IssueDate { get; set; }
        public string ViolationType { get; set; }
        public int FineAmount { get; set; }
        public string VIN { get; set; }
        public int? OfficerId { get; set; }
    }
}