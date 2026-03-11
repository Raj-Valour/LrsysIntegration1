using System;

namespace LrsysIntegration.DTOs
{
    public class ApplicationErrorLogDTO
    {
        public int ErrorLogID { get; set; }
        public string Errordescription { get; set; }
        public string ErrorModule { get; set; }
        public string Errorform { get; set; }
        public DateTime ErrordateTime { get; set; }
    }
}