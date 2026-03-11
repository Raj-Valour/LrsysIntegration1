using System;
using System.Collections.Generic;

namespace LrsysIntegration.DTOs
{
    public class OrderDto
    {
        public string TableNumber { get; set; }
        public string Notes { get; set; }
        public long? OrderRef { get; set; }
        public int UserID { get; set; }

        // ✅ ADD THIS
        public short OrderType { get; set; }   // 0 = VAT excluded, 1 = VAT included
        public int OrderStatus { get; set; } = 15;
        public List<OrderItemDto> Items { get; set; }

        public OrderDto()
        {
            TableNumber = "";
            Notes = "";
            OrderType = 0;   // default safe
            Items = new List<OrderItemDto>();
        }
    }

    public class OrderResponseDto
    {
        public long OrderRef { get; set; }

        public decimal NetTotal { get; set; }
        public decimal VatTotal { get; set; }
        public decimal ServiceCharge { get; set; }

        public decimal PromoDiscountTotal { get; set; }
        public decimal UserDiscountTotal { get; set; }

        public decimal FinalTotal { get; set; }
    }

}
