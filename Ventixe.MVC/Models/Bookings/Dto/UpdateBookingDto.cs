﻿using Ventixe.MVC.Models.Bookings;

namespace Ventixe.MVC.Models.Bookings.Dto;

public class UpdateBookingDto
{
    public BookingStatus Status { get; set; }
    public int Quantity { get; set; }
    public string? EVoucher { get; set; }
}

