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

        // Remove Schedule property
        public NPCInfo(string name, string season, int day, List<string> lovedGifts)
        {
            Name = name;
            Season = season;
            Day = day;
            LovedGifts = lovedGifts;
        }
    }
    
    public class ScheduleSlot
    {
        public int StartTime { get; }
        public int EndTime { get; }
        public string Location { get; }

        public ScheduleSlot(int startTime, int endTime, string location)
        {
            StartTime = startTime;
            EndTime = endTime;
            Location = location;
        }
    }
}
