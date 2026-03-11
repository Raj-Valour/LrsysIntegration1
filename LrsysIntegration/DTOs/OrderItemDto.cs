using System.Collections.Generic;

public class OrderItemDto
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public string ItemNote { get; set; }
    public bool IsNew { get; set; }

    public int VatId { get; set; }
    public decimal VatPercent { get; set; }
    public string Description { get; set; }
    public string Barcode { get; set; }
    public bool IsMealDeal { get; set; }   // 🔥 Important

    public List<OrderItemDto> MealItems { get; set; }

    public OrderItemDto()
    {
        ItemNote = "";
        Description = "";
        MealItems = new List<OrderItemDto>();
    }
}