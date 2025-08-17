using Lunch.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lunch.Domain.Models
{
    public class Customer : IdentityUser
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string FullName => $"{FirstName} {LastName}";
        public DateOnly DateOfBirth { get; set; }
        public Role Role { get; set; }
        public decimal Credit { get; set; }
        public bool Active { get; set; } = true;
        public bool MustChangePassword { get; set; } = true;

        [NotMapped]
        public bool IsAdult => DateOfBirth.AddYears(18) <= DateOnly.FromDateTime(DateTime.Today);
        public string CardId { get; set; } = null!;
    }
}
