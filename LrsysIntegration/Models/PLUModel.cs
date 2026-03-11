using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LrsysIntegration.Models
{
    public class PLU
    {
        public int PLUID { get; set; }
        public string PLUCode { get; set; }

        public string Description { get; set; }

        public string Department { get; set; }
        public string SubDepartment { get; set; }
        public string BrandName { get; set; }

        public string Barcode { get; set; }

        public decimal MinStock { get; set; }
        public decimal MaxStock { get; set; }
        public decimal ProductStock { get; set; }

        public decimal VAT { get; set; }

        public decimal Cost { get; set; }
        public decimal Price { get; set; }

        public int Deleted { get; set; }

        public string LastModifiedAt { get; set; }

        public decimal warehouseprice { get; set; }
        public decimal WarehouseStock { get; set; }
        public string Warehouse_LastModifiedAt { get; set; }

    }
}