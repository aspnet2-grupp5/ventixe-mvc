        namespace Application.Models
    {
        public class CreateEventRequest
        {
            public string Id { get; set; } = Guid.NewGuid().ToString();
            public string Title { get; set; } = null!;
            public string? Description { get; set; }
            public string? Image { get; set; }
            public DateTime? Date { get; set; }
            public decimal? Price { get; set; }
            public int? Quantity { get; set; }
            public int? SoldQuantity { get; set; }

            // Foreign Keys
            public string CategoryId { get; set; }
            public string CategoryName { get; set; }

            public string LocationId { get; set; }
            public string Address { get; set; }
            public string City { get; set; }

            public string StatusId { get; set; }
            public string StatusName { get; set; }
        }
    }
}
