namespace Ventixe.MVC.Models
{
    public class InvoiceDto
    {
        public int Id { get; set; }
        public string? InvoiceNumber { get; set; }
        public decimal Amount { get; set; }
        public DateTime IssuedDate { get; set; }
        public DateTime DueDate { get; set; }
        public string? Status { get; set; }
        public string? BillTo { get; set; }
    }
}
