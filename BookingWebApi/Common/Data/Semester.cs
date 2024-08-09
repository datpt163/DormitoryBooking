using System;
using System.Collections.Generic;

namespace BookingWebApi.Common.Data
{
    public partial class Semester
    {
        public Semester()
        {
            Bookings = new HashSet<Booking>();
        }

        public Guid Id { get; set; }
        public string? Name { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public virtual ICollection<Booking> Bookings { get; set; }
    }
}
