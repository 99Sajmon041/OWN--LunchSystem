using System.ComponentModel.DataAnnotations.Schema;

namespace Lunch.Domain.Models
{
    public class WrittenOffFood
    {
        public int Id { get; set; }

        [ForeignKey(nameof(Customer))]
        public string CustomerId { get; set; } = null!;
        public Customer Customer { get; set; } = null!;

        [ForeignKey(nameof(DailyOffer))]
        public int? DailyOfferId { get; set; }
        public DailyOffer? DailyOffer { get; set; }
        public DateTime TimeOfCreate { get; set; }
        public decimal PriceAfterDiscount { get; set; }
        public string? KioskId { get; set; }
    }
}
