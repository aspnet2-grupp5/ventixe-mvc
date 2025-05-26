using Ventixe.MVC.Models.Bookings.Dto;
using Ventixe.MVC.Models.Bookings;

namespace Ventixe.MVC.Models.Bookings;

public class BookingIndexViewModel
{
    public List<BookingsModel> Bookings { get; set; } = new();
    public BookingStatsDto? Stats { get; set; }
    public BookingFilterDto Filter { get; set; } = new();
}
