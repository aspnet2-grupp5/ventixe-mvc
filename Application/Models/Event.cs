namespace Presentation.Models
{
    public class EventViewModel
    {
        public string EventId { get; set; } = Guid.NewGuid().ToString();
        public string EventTitle { get; set; } = null!;
        public string? Description { get; set; }
        public string? Image { get; set; }
        public DateTime? Date { get; set; }
        public decimal? Price { get; set; }
        public int? Quantity { get; set; }

        public string? Category { get; set; }
        public string? Location { get; set; }
        public string? Status { get; set; }
    }
}
