using System;

namespace LrsysIntegration.DTOs
{
    public class MealItemDto
    {
        public int ProductId { get; set; }
        public string Description { get; set; }
        public int MealGroupId { get; set; }
        public decimal AdditionalPrice { get; set; }
    }
}