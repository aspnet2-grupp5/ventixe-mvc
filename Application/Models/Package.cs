namespace Application.Models
{
    public class  Package
    {
        public string EventId { get; set; } = null!;
        public string PackageName { get; set; } = null!;
        public decimal? Price { get; set; }
        public string? Placement { get; set; }  
    }
}
