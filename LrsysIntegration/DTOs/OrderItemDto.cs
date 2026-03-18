using System.Collections.Generic;
namespace LrsysIntegration.DTOs
{
    public class OrderItemDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string ItemNote { get; set; }
        public string VoidReason { get; set; } = "";
        public bool IsNew { get; set; }
        public int VatId { get; set; }
        public decimal VatPercent { get; set; }
        public string Description { get; set; }
        public string Barcode { get; set; }
        public bool IsMealDeal { get; set; }

        public List<OrderItemDto> MealItems { get; set; }

        public long? InternalSalesDtlId { get; set; }
        public long? SalesDtlId { get; set; }

        public OrderItemDto()
        {
            ItemNote = "";
            Description = "";
            MealItems = new List<OrderItemDto>();
        }
    }
}