// NPCInfo.cs
namespace BirthdayReminderMod
{
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
