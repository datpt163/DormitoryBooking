using System;
using System.Collections.Generic;

namespace BookingWebApi.Common.Data
{
    public partial class Booking
    {
        public Guid Id { get; set; }
        public Guid? SemesterId { get; set; }
        public Guid RoomId { get; set; }
        public Guid? UserId { get; set; }

        public virtual Semester Semester { get; set; } = new Semester();
    }
}
