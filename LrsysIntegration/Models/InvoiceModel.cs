using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LrsysIntegration.Models
{
    public class InvoiceModel
    {


        public class Invoice_Response
        {
            public string InvoiceRef { get; set; }
            public string InvoiceStatus { get; set; }

            public string Reason { get; set; }
            
        }

        public class Invoice_Payments
        {
            public string Paymentby { get; set; }

            public string Paymentref { get; set; }
            public decimal PaidAmount { get; set; }
        }
        public class Invoice_Detail
        {
            public int PLUID { get; set; }
            public decimal Qty { get; set; }
            public string Description { get; set; }
            //public string Product_Color { get; set; }
            //public string Product_Size { get; set; }
            public decimal ItemPrice { get; set; }
            public decimal TotalItemsPrice { get; set; }
            public decimal Vat { get; set; }
            
        }

        public class Invoice_Header
        {
            public String InvoiceRef { get; set; }
            public DateTime Invoicedate { get; set; }

            public decimal Invoice_OrderTotal { get; set; }
            public string Invoice_CustomerID { get; set; }
            public string Invoice_BarcodeAllocated { get; set; }

            public string Invoice_Customername { get; set; }

            public string Invoice_Address { get; set; }

            public string Invoice_City { get; set; }
            public string Invoice_PostCode { get; set; }
            public int Invoice_LoyaltyPointsGained { get; set; }
            public int Invoice_LoyaltyPointsRedeemed { get; set; }
            public decimal Invoice_DeliveryCost { get; set; }

            public string Invoice_Email { get; set; }
            public string Invoice_PhoneNumner { get; set; }

        }


        public class Invoice
        {
            public Invoice_Header Invoice_Header { get; set; }
            public List<Invoice_Detail> Invoice_Detail { get; set; }
            public List<Invoice_Payments> Invoice_Payments { get; set; }
        }



    }
}