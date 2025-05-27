namespace Ventixe.MVC.Models.Events
{
    public class EventViewModel
    {
        public string EventId { get; set; } = Guid.NewGuid().ToString();
        public string EventTitle { get; set; } = null!;
        public string? EventImage { get; set; }

        public string Description { get; set; } = null!;

        public DateTime? Date { get; set; }
        public decimal? Price { get; set; }

        public int? Quantity { get; set; }
        public int? SoldQuantity { get; set; }
        public string? Location { get; set; }
        public string? Category { get; set; }
        public string? Status { get; set; }
    }
}