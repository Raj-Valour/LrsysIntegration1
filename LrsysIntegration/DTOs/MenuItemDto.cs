namespace LrsysIntegration.DTOs
{
    public class NoteOptionDto
    {
        public int DepartmentId { get; set; }
        public int NoteId { get; set; }
        public string NoteName { get; set; }
        public string BackColor { get; set; }
        public string ForeColor { get; set; }
    }

    public class MenuItemDto
    {
        public int ProductId { get; set; }
        public string Description { get; set; }
        public string BackColor { get; set; }
        public string ForeColor { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public string ProductCode { get; set; }
        public decimal Price { get; set; }
        public int VatId { get; set; }
        public decimal VatPercent { get; set; }
        public bool HasMealDeal { get; set; }

        public System.Collections.Generic.List<NoteOptionDto> NotesOptions { get; set; } = new System.Collections.Generic.List<NoteOptionDto>();

        public string DepartmentBackColor { get; set; }
        public string DepartmentForeColor { get; set; }
    }
}
