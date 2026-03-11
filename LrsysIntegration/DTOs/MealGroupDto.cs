using System;

namespace LrsysIntegration.DTOs

{
    public class MealGroupDto
    {
        public int MealGroupId { get; set; }
        public string MealDescription { get; set; }
        public int MaxAllowed { get; set; }
        public string ButtonColor { get; set; }
        public string TextColor { get; set; }
    }
}