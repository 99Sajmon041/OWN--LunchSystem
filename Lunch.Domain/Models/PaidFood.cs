using System.ComponentModel.DataAnnotations.Schema;

namespace Lunch.Domain.Models
{
    public class PaidFood
    {
        public int Id { get; set; }

        [ForeignKey(nameof(Customer))]
        public string CustomerId { get; set; } = null!;
        public Customer Customer { get; set; } = null!;

        [ForeignKey(nameof(DailyOffer))]
        public int DailyOfferId { get; set; }
        public DailyOffer DailyOffer { get; set; } = null!;
        public bool Paid { get; set; } = false;
        public decimal FinalPaidPrice { get; set; }
    }
}
