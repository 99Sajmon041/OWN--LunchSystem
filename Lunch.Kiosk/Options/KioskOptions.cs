using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunch.Kiosk.Options
{
    public sealed class KioskOptions
    {
        public string ApiBaseUrl { get; set; } = "";
        public int TimeoutSeconds { get; set; } = 3;
        public string KioskId { get; set; } = "";
    }
}
