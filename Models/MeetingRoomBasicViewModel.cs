using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetingRoomScheduler.Models
{
    public class MeetingRoomBasicViewModel
    {
        public string RoomName { get; set; }
        public int Capacity { get; set; }
        public TimeSpan AvailableFrom { get; set; }
        public TimeSpan AvailableTo { get; set; }
    }
}
