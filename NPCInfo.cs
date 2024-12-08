using System.Collections.Generic;

namespace BirthdayReminderMod
{
    public class NPCInfo
    {
        public string Name { get; }
        public string Birthday { get; }
        public List<string> LovedGifts { get; }

        public NPCInfo(string name, string birthday, List<string> lovedGifts)
        {
            Name = name;
            Birthday = birthday;
            LovedGifts = lovedGifts;
        }
    }
}
