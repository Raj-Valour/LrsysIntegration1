using System;

namespace LrsysIntegration.DTOs
{
    public class SuspendedOrderDto
    {
        public long OrderRef { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal Total { get; set; }
        public string Table { get; set; }
        public DateTime LastUpdated { get; set; }
        public int OrderStatus { get; set; }
    }
}