using MeetingRoomScheduler.Data;
using MeetingRoomScheduler.Entities;
using MeetingRoomScheduler.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MeetingRoomScheduler.Controllers
{
    public class MeetingRoomController : Controller
    {
        private readonly JsonFileReaderService jsonFileReaderService;
        private List<MeetingRoom> allRooms = new List<MeetingRoom>();
        private List<MeetingRoomFilterResponseViewModel> availableRooms = new List<MeetingRoomFilterResponseViewModel>();

        public MeetingRoomController(JsonFileReaderService jsonFileReaderService)
        {
            this.jsonFileReaderService = jsonFileReaderService;
        }
        public IActionResult List()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AvailableMeetingRooms(MeetingRoomFilterRequestViewModel filterRequest)
        {
            allRooms = jsonFileReaderService.LoadRoomData<MeetingRoom>();

            foreach (var room in allRooms.Where(r => r.Capacity >= filterRequest.Attendees))
            {
                room.Schedule.OrderBy(slot => slot.From);
                var availableTimeSlots = CheckForAvailableTimeslots(room.AvailableFrom, room.AvailableTo, room.Schedule.Where(s => s.To.Date == filterRequest.Date || s.From.Date == filterRequest.Date).ToList(), TimeSpan.FromMinutes(filterRequest.Duration), filterRequest.Date);
                if (availableTimeSlots.Count > 0)
                {
                    availableRooms.Add(new MeetingRoomFilterResponseViewModel
                    {
                        RoomName = room.RoomName,
                        Capacity = room.Capacity,
                        AvailableTimeSlots = availableTimeSlots
                    });
                }
            }

            return View(availableRooms);
        }

        [HttpPost]
        public IActionResult BookMeetingRoom([FromBody] MeetingRoomBookRequestViewModel request)
        {
            var parsedDataFromFile = jsonFileReaderService.LoadRoomData<MeetingRoom>();
            var meetingRoom = parsedDataFromFile.Where(r => r.RoomName == request.RoomName).SingleOrDefault();
            var timeSlotToAdd = new TimeSlot
            {
                From = request.From,
                To = request.To
            };
            meetingRoom.Schedule.Add(timeSlotToAdd);

            string json = JsonConvert.SerializeObject(parsedDataFromFile, Formatting.Indented);
            System.IO.File.WriteAllText("./Data/RoomData.json", json);
            return Ok("Successfully booked !");
        }

        private List<TimeSlot> CheckForAvailableTimeslots(TimeSpan availableFrom, TimeSpan availableTo, List<TimeSlot> schedule, TimeSpan duration, DateTime date)
        {
            var availableSlots = new List<TimeSlot>();
            var scheduledSlots = schedule.ToArray();
            var roomStartTime = new DateTime(date.Year, date.Month, date.Day, availableFrom.Hours, availableFrom.Minutes, availableFrom.Seconds);
            var roomClosingTime = new DateTime(date.Year, date.Month, date.Day, availableTo.Hours, availableTo.Minutes, availableTo.Seconds);

            if (schedule.Count == 0)
            {
                availableSlots.Add(new TimeSlot { From = roomStartTime, To = roomClosingTime });
                return SplitTimeslotsIntoSegments(availableSlots, duration);
            }

            else if(schedule.Count == 1)
            {
                var scheduledSlot = schedule.FirstOrDefault();
                if (scheduledSlot.From == roomStartTime)
                {
                    availableSlots.Add(new TimeSlot { From = scheduledSlot.To, To = roomClosingTime });
                }

                else if(scheduledSlot.To == roomClosingTime)
                {
                    availableSlots.Add(new TimeSlot { From = roomStartTime, To = scheduledSlot.From });
                }

                else
                {
                    availableSlots.AddRange(new List<TimeSlot> 
                    {
                        new TimeSlot { From = roomStartTime, To = scheduledSlot.From},
                        new TimeSlot { From = scheduledSlot.To, To = roomClosingTime}
                    });
                }
                return SplitTimeslotsIntoSegments(availableSlots, duration);
            }

            foreach (var timeSlot in scheduledSlots)
            {
                if (Array.IndexOf(scheduledSlots, timeSlot) == 0)
                {
                    if (timeSlot.From - roomStartTime >= duration)
                    {
                        availableSlots.Add(new TimeSlot
                        {
                            From = roomStartTime,
                            To = timeSlot.From
                        });
                    }
                }

                else if (Array.IndexOf(scheduledSlots, timeSlot) == scheduledSlots.Length - 1)
                {
                    if (timeSlot.From - scheduledSlots[Array.IndexOf(scheduledSlots, timeSlot) - 1].To >= duration)
                    {
                        availableSlots.Add(new TimeSlot
                        {
                            From = scheduledSlots[Array.IndexOf(scheduledSlots, timeSlot) - 1].To,
                            To = timeSlot.From
                        });
                    }

                    if (roomClosingTime - timeSlot.To >= duration)
                    {
                        availableSlots.Add(new TimeSlot
                        {
                            From = timeSlot.To,
                            To = roomClosingTime
                        });
                    }
                }

                else
                {
                    if (timeSlot.From - scheduledSlots[Array.IndexOf(scheduledSlots, timeSlot) - 1].To >= duration)
                    {
                        availableSlots.Add(new TimeSlot
                        {
                            From = scheduledSlots[Array.IndexOf(scheduledSlots, timeSlot) - 1].To,
                            To = timeSlot.From
                        });
                    }
                }
            }

            if (availableSlots.Count == 0)
            {
                availableSlots.Add(new TimeSlot { From = roomStartTime, To = roomClosingTime });
                return SplitTimeslotsIntoSegments(availableSlots, duration);
            }

            var splitTimeSlots = SplitTimeslotsIntoSegments(availableSlots.OrderBy(x => x.From), duration);

            return splitTimeSlots;
        }

        private List<TimeSlot> SplitTimeslotsIntoSegments(IEnumerable<TimeSlot> timeSlots, TimeSpan meetingDuration)
        {
            var splitTimeSlots = new List<TimeSlot>();

            foreach (var timeSlot in timeSlots)
            {
                var possibleMeetingStartTime = timeSlot.From;
                var possibleMeetingEndTime = possibleMeetingStartTime + meetingDuration;
                while (possibleMeetingEndTime <= timeSlot.To)
                {
                    splitTimeSlots.Add(new TimeSlot
                    {
                        From = possibleMeetingStartTime,
                        To = possibleMeetingEndTime
                    });

                    possibleMeetingStartTime += new TimeSpan(0, 15, 0);
                    possibleMeetingEndTime += new TimeSpan(0, 15, 0);
                }
            }

            return splitTimeSlots;
        }
    }
}
