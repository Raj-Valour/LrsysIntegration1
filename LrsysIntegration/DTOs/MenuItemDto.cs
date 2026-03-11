namespace LrsysIntegration.DTOs
{
    // NoteOptionDto included here so it's compiled without modifying the csproj
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

        // Added notes options for the menu item (per-department predefined notes)
        public System.Collections.Generic.List<NoteOptionDto> NotesOptions { get; set; } = new System.Collections.Generic.List<NoteOptionDto>();
    }
}
