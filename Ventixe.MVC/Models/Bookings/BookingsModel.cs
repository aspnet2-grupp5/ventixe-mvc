namespace Ventixe.MVC.Models.Bookings;

public class BookingsModel
{
    public string? BookingId { get; set; }
    public string? InvoiceId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime BookingDate { get; set; }
    public string? UserId { get; set; }
    public string? CustomerName { get; set; }
    public string? EventId { get; set; }
    public string? EventName { get; set; }
    public string? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public string? TicketCategoryId { get; set; }
    public string? TicketCategoryName { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public decimal TotalAmount => Price * Quantity;
    public BookingStatus? Status { get; set; }
    public string? EVoucher { get; set; }
}
