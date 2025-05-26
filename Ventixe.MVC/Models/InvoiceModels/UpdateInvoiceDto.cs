using System.ComponentModel.DataAnnotations;

namespace Ventixe.MVC.Models.InvoiceModels
{
    public class UpdateInvoiceDto
    {
        public int Id { get; set; }

        [Required]
        public int BookingId { get; set; }

        [Required, StringLength(100)]
        public string CustomerName { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string EventName { get; set; } = string.Empty;

        [Required, Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }

        [Required]
        public DateTime IssuedDate { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        [Required, StringLength(50)]
        public string Status { get; set; } = string.Empty;
    }
}
