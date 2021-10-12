using System;
using System.Collections.Generic;

namespace MeetingRoomScheduler.Entities
{
    public class MeetingRoom
    {
        public string RoomName { get; set; }
        public int Capacity { get; set; }
        public TimeSpan AvailableFrom { get; set; }
        public TimeSpan AvailableTo { get; set; }
        public List<TimeSlot> Schedule { get; set; }
    }
}
