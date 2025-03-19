// NPCInfo.cs
using System.Collections.Generic;

namespace BirthdayReminderMod
{
    public class NPCInfo
    {
        public string Name { get; }
        public string Season { get; }
        public int Day { get; }
        public List<string> LovedGifts { get; }
        public Dictionary<string, List<ScheduleSlot>> Schedule { get; } // Updated to use List<ScheduleSlot>

        public NPCInfo(string name, string season, int day, List<string> lovedGifts, Dictionary<string, List<ScheduleSlot>> schedule = null)
        {
            Name = name;
            Season = season;
            Day = day;
            LovedGifts = lovedGifts;
            Schedule = schedule ?? new Dictionary<string, List<ScheduleSlot>>();
        }
    }
    public class ScheduleSlot
    {
        public int StartTime { get; } // Start time (e.g., 900 for 9:00 AM)
        public int EndTime { get; }   // End time (e.g., 1030 for 10:30 AM)
        public string Location { get; } // Location name (e.g., "SeedShop")
        public string Description { get; } // Optional description of the activity

        public ScheduleSlot(int startTime, int endTime, string location, string description = null)
        {
            StartTime = startTime;
            EndTime = endTime;
            Location = location;
            Description = description;
        }
    }
}
