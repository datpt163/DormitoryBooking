﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonModel.Message
{
    public class PaymentMessage
    {
        public Guid Bookingid { get; set; }
        public Guid RoomId { get; set; }
        public Guid UserId { get; set; }
    }
}
