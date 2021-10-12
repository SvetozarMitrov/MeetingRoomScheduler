using MeetingRoomScheduler.Entities;
using System.Collections.Generic;

namespace MeetingRoomScheduler.Models
{
    public class MeetingRoomFilterResponseViewModel
    {
        public string RoomName { get; set; }
        public int Capacity { get; set; }
        public IEnumerable<TimeSlot> AvailableTimeSlots { get; set; }
    }
}
