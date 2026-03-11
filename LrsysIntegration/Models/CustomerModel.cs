using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LrsysIntegration.Models
{
    public class Customers
    {

        public string CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }

        public string CustomerTown { get; set; }

        public string CustomerCounty { get; set; }
        public string CustomerPostCode { get; set; }
        public string CustomerTelephone { get; set; }

        public string CustomerMobile { get; set; }

        public string CustomerEmail { get; set; }
      //  public string CustomerFax { get; set; }
        public string CustomerVATNo { get; set; }


        public int TotalLoyaltyPoints { get; set; }
        public string BarcodeAllocated { get; set; }
        public string LoyaltyLastUpdatedAt { get; set; }

        public string CustomerDOB { get; set; }

        


    }
}