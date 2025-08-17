using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunch.Kiosk.Models
{
    public sealed class ValidationProblem
    {
        public string? Title { get; set; }
        public string? Detail { get; set; }
        public Dictionary<string, string[]>? Errors { get; set; }
    }
}
