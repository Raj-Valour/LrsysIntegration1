using System;

using System.ComponentModel.DataAnnotations;

namespace LrsysIntegration.Models

{

    public class ApplicationErrorLog

    {

        [Key]

        public int ErrorLogID { get; set; }

        public string Errordescription { get; set; }

        public string ErrorModule { get; set; }

        public string Errorform { get; set; }

        public DateTime ErrordateTime { get; set; }

    }

}