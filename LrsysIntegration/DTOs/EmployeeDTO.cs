using System;

namespace LrsysIntegration.DTOs

{

    public class EmployeeDTO

    {

        public int UserID { get; set; }

        public string UserCode { get; set; }

        public string FullName { get; set; }

        public string MobileNo { get; set; }

        public int? JobType { get; set; }

        public bool? Inactive { get; set; }

    }

}


