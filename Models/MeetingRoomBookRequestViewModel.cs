using System;

namespace MeetingRoomScheduler.Models
{
    public class MeetingRoomBookRequestViewModel
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public string RoomName { get; set; }
    }
}
