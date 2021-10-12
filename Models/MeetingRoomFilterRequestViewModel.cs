using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetingRoomScheduler.Models
{
    public class MeetingRoomFilterRequestViewModel
    {
        public DateTime Date { get; set; }
        public double Duration { get; set; }
        public int Attendees { get; set; }
    }
}
