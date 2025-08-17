namespace Lunch.Domain.Models
{
    public class RoleDiscount
    {
        public int Id { get; set; }

        public string RoleId { get; set; } = null!;
        public decimal Discount { get; set; }
    }
}