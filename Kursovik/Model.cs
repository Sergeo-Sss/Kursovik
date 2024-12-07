using Newtonsoft.Json;
using System;

namespace Kursovik
{
    public class AccidentParticipant
    {
        public string OwnerName { get; set; }
        public string ContactNumber { get; set; }
        public string CarVin { get; set; }
        public string CarBrand { get; set; }
        public string CarModel { get; set; }
        public DateTime AccidentDate { get; set; }
        public string AccidentDetails { get; set; }
    }

    public class FailedInspectionOwner
    {
        public string OwnerName { get; set; }
        public string ContactNumber { get; set; }
        public string CarBrand { get; set; }
        public string CarModel { get; set; }
        public DateTime FailedDate { get; set; }
    }

    public class LuxuryCarOwner
    {
        public string OwnerName { get; set; }
        public string ContactNumber { get; set; }
        public string CarBrand { get; set; }
        public string CarModel { get; set; }
        public int CarYear { get; set; }
        public string CarColor { get; set; }
    }

    public class ServiceCenterInspection
    {
        public string ServiceCenterName { get; set; }
        public DateTime InspectionDate { get; set; }
        public string InspectionResult { get; set; }
        public string CarVin { get; set; }
        public string CarBrand { get; set; }
        public string CarModel { get; set; }
        public string EmployeeName { get; set; }
    }

    public class OfficerProtocol
    {
        public string OfficerName { get; set; }
        public string OfficerDepartment { get; set; }
        public int ProtocolNumber { get; set; }
        public DateTime IssueDate { get; set; }
        public string ViolationType { get; set; }
        public int FineAmount { get; set; }
        public string CarVin { get; set; }
        public string CarBrand { get; set; }
        public string CarModel { get; set; }
    }

    public class LogEntry
    {
        [JsonProperty("log_id")]
        public int LogId { get; set; }

        [JsonProperty("table_name")]
        public string TableName { get; set; }

        [JsonProperty("action_type")]
        public string ActionType { get; set; }

        [JsonProperty("action_time")]
        public DateTime ActionTime { get; set; }
    }
}