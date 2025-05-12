namespace Ventixe.MVC.Models.Dto
{
    public class CreateBookingDto
    {
        public string? InvoiceId { get; set; }
        public DateTime BookingDate { get; set; }
        public string? EventId { get; set; }
        public string? EventName { get; set; }
        public string? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string? TicketCategoryId { get; set; }
        public string? TicketCategoryName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string? EVoucher { get; set; }
    }
}

