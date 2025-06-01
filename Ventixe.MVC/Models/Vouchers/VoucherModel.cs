namespace Ventixe.MVC.Models.Vouchers;

public class VoucherModel
{
    public string Id { get; set; }
    public string BookingId { get; set; }
    public string HolderName { get; set; }
    public string TicketCategory { get; set; }
    public string SeatNumber { get; set; }
    public string Gate { get; set; }
    public DateTime EventDate { get; set; }
    public string Location { get; set; }
    public string Barcode { get; set; }
}