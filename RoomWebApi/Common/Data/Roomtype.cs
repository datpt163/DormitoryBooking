using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RoomWebApi.Common.Data
{
    public partial class Roomtype
    {
        public Roomtype()
        {
            Rooms = new HashSet<Room>();
        }

        public Guid Id { get; set; }
        public int Capacity { get; set; }
        public float Price { get; set; }
        public string? ImageUrl { get; set; }
        [JsonIgnore]
        public virtual ICollection<Room> Rooms { get; set; }
    }
}
