using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RoomWebApi.Common.Data
{
    public partial class Room
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public int? CurrentPeople { get; set; }
        public bool IsAvailble { get; set; }
        public Guid? TypeId { get; set; }
        public Guid? BuildingId { get; set; }
        [JsonIgnore]
        public virtual Building? Building { get; set; }
        [JsonIgnore]
        public virtual Roomtype Type { get; set; } = new Roomtype();
    }
}
