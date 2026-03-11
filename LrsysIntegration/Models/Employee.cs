using System.ComponentModel.DataAnnotations;

namespace LrsysIntegration.Models
{
    public class Employees
    {
        [Key]
        public int UserID { get; set; }


        public string Usercode { get; set; }

        public string UserPassword { get; set; }

        public string User_Fname { get; set; }
        public string User_Lname { get; set; }

        public string Usr_MobileNo { get; set; }
        public int? JobType { get; set; }

        public bool? Inactive { get; set; }
    }
}