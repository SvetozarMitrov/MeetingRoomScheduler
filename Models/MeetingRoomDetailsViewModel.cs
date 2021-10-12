using MeetingRoomScheduler.Entities;
using System;
using System.Collections.Generic;

namespace MeetingRoomScheduler.Models
{
    public class MeetingRoomDetailsViewModel
    {
        public string RoomName { get; set; }
        public int Capacity { get; set; }
        public DateTime AvailableFrom { get; set; }
        public DateTime AvailableTo { get; set; }
        public List<TimeSlot> Schedule { get; set; }
    }
}
